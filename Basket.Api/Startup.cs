using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Basket.Api.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Basket.Api
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
            services.AddControllers();
            services.AddGrpc(o=>o.EnableDetailedErrors = true);
            services.AddCustomHealthCheck(Configuration);

            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Basket API",
                    Version = "v1",
                    Description = "The Basket Service HTTP API"
                });
            
            
                //TODO
                // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                // {
                //     Type = SecuritySchemeType.OAuth2,
                //     Flows = new OpenApiOAuthFlows()
                //     {
                //         Implicit = new OpenApiOAuthFlow()
                //         {
                //             AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                //             TokenUrl = new Uri($"{Configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                //             Scopes = new Dictionary<string, string>()
                //             {
                //                 { "basket", "Basket API" }
                //             }
                //         }
                //     }
                // });
                //
                // options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = string.Empty;// Configuration["PATH_BASE"];
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseSwagger()
                .UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
                    setup.RoutePrefix = string.Empty;
                    //TODO
                    // setup.OAuthClientId("basketswaggerui");
                    // setup.OAuthAppName("Basket Swagger UI");
                });

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BasketGrpcService>();
                endpoints.MapControllers();
                endpoints.MapGet("/_proto/", GetProtoBuf(env));
                
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }

        private static RequestDelegate GetProtoBuf(IWebHostEnvironment env)
        {
            return async ctx =>
            {
                ctx.Response.ContentType = "text/plain";
                using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    if (line != "/* >>" || line != "<< */")
                    {
                        await ctx.Response.WriteAsync(line);
                    }
                }
            };
        }
    }
    
    public static class CustomExtensionMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            //TODO
            // hcBuilder
            //     .AddRedis(
            //         configuration["ConnectionString"],
            //         name: "redis-check",
            //         tags: new string[] { "redis" });
            //
            // if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
            // {
            //     hcBuilder
            //         .AddAzureServiceBusTopic(
            //             configuration["EventBusConnection"],
            //             topicName: "eshop_event_bus",
            //             name: "basket-servicebus-check",
            //             tags: new string[] { "servicebus" });
            // }
            // else
            // {
            //     hcBuilder
            //         .AddRabbitMQ(
            //             $"amqp://{configuration["EventBusConnection"]}",
            //             name: "basket-rabbitmqbus-check",
            //             tags: new string[] { "rabbitmqbus" });
            // }

            return services;
        }
    }
}