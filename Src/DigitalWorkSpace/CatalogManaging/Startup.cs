using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CatalogManaging.Core.Contracts;
using CatalogManaging.Infrastructure;
using CatalogManaging.Infrastructure.Data;
using CatalogManaging.Infrastructure.EventBus.Consumer;
using CatalogManaging.Infrastructure.EventBus.Producer;
using CatalogManaging.Infrastructure.Interfaces;
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

namespace CatalogManaging
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
            services.AddDbContext<CatalogContext>(optBuilder =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CatalogDb");
                optBuilder.UseMySQL(connectionString);
            });
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICatalogContext, CatalogContext>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<IEventConsumerHandler, CardConsumerHandler>();
            services.AddScoped<ICardEventHandler, CardEventHandler>();

            var prconfig = new Dictionary<string, string>
            {
                {"bootstrap.servers",Environment.GetEnvironmentVariable("Producer") }
            };
            var producerConfig = new ProducerConfig(prconfig);
            services.AddSingleton<ProducerConfig>(producerConfig);
            var cardEventHandler = new CardEventHandler(producerConfig);
            services.AddSingleton<CardEventHandler>(cardEventHandler);


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
                setup.SwaggerDoc("CatalogOpenApiSpecification",
                    new OpenApiInfo()
                    {
                        Title = "Catalog Api",
                        Version = "1",
                        Description = "Through this api user car access the catalog and the cards linked to it "
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
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happend. Try again later.");
                    });
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSwagger();

            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint(Environment.GetEnvironmentVariable("SwaggerEndpoint"),
                    Environment.GetEnvironmentVariable("SwaggerName"));
            });

            app.UseCors("AllowOrigin");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
