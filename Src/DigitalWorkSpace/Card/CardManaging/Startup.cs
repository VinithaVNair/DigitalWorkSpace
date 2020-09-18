using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CardManaging.Core;
using CardManaging.Core.Contracts;
using CardManaging.Core.Events;
using CardManaging.Infrastructure.Data;
using CardManaging.Infrastructure.EventBus.Consumer;
using CardManaging.Infrastructure.EventBus.Producer;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CardManaging
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CardContext>(optBuilder =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CardDb");
                optBuilder.UseMySQL(connectionString);
            });

            services.AddScoped<ICardContext, CardContext>();
            services.AddScoped<ICardOperations, CardOperations>();
            services.AddScoped<ICardEventHandler, CardEventHandler>();

            var prconfig = new Dictionary<string, string>
            {
                {"bootstrap.servers",Environment.GetEnvironmentVariable("Producer") }
            };
            var producerConfig = new ProducerConfig(prconfig);
            services.AddSingleton<ProducerConfig>(producerConfig);
            var cardEventHandler = new CardEventHandler(producerConfig, services.BuildServiceProvider().GetRequiredService<ILogger<CardEventHandler>>());
            services.AddSingleton<CardEventHandler>(cardEventHandler);

            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("Producer"),
                GroupId = Environment.GetEnvironmentVariable("GroupId"),
                AllowAutoCreateTopics = true,
                EnableAutoCommit = false
            };
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddHostedService<UrlDeletionEventListener>();
            services.AddHostedService<CardEventListener>();
            services.AddControllers(setupAction =>
            {
                setupAction.Filters.Add(new ProducesResponseTypeAttribute((int)HttpStatusCode.BadRequest));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute((int)HttpStatusCode.NotAcceptable));
                setupAction.Filters.Add(new ProducesResponseTypeAttribute((int)HttpStatusCode.InternalServerError));
                setupAction.Filters.Add(new ProducesAttribute("application/json"));
                setupAction.ReturnHttpNotAcceptable = true;
            });

            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc("CardOpenApiSpecification",
                    new OpenApiInfo()
                    {
                        Title = "Cards Api",
                        Version = "1",
                        Description = "Through this api user can create and  access the cards"
                    });
                var xmlCommentFile = $"{ Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);

                setup.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appbuilder =>
                {
                    appbuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happend");
                    });
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint(Environment.GetEnvironmentVariable("SwaggerEndpoint"),
                    Environment.GetEnvironmentVariable("SwaggerName"));
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowOrigin");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
