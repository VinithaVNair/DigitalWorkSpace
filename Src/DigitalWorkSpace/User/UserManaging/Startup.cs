using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManaging.Core;
using UserManaging.Core.Contracts;
using UserManaging.Infrastructure.Data;
using System.Net;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace UserManaging
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
            services.AddDbContext<UserContext>(optBuilder =>
            {
                var connectionString = Environment.GetEnvironmentVariable("UserDb");
                optBuilder.UseMySQL(connectionString);
            });
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<IUserOperations, UserOperations>();
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
                setup.SwaggerDoc("UserOpenApiSpecification",
                    new OpenApiInfo()
                    {
                        Title = "User Api",
                        Version = "1",
                        Description = "Through this api user can create and  access the catalog and the cards linked to it "
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
