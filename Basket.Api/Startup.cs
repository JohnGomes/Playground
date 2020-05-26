using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Basket.Api.Configuration;
using Basket.Api.Controllers;
using Basket.Api.Grpc;
using Basket.Api.Infrastructure.Filters;
using Basket.Api.Infrastructure.Repositories;
using Basket.Api.Middlewares;
using Basket.Api.Model;
using Basket.Api.Services;
using EventBus;
using EventBus.Abstractions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Playground.EventBusRabbitMQ;
using RabbitMQ.Client;
using StackExchange.Redis;

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
        public  void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddGrpc(o=>o.EnableDetailedErrors = true);
            services.AddCustomHealthCheck(Configuration);
            
            // RegisterAppInsights(services);

            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                    options.Filters.Add(typeof(ValidateModelStateFilter));

                }) // Added for functional tests
                .AddApplicationPart(typeof(BasketController).Assembly);
                //.AddNewtonsoftJson();


            
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
                
                //options.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            
            ConfigureAuthService(services);
            
            // services.AddCustomHealthCheck(Configuration);
            //
            services.Configure<BasketSettings>(Configuration);
            //
            // //By connecting here we are making sure that our service
            // //cannot start until redis is ready. This might slow down startup,
            // //but given that there is a delay on resolving the ip address
            // //and then creating the connection it seems reasonable to move
            // //that cost to startup instead of having the first request pay the
            // //penalty.
            services.AddSingleton<ConnectionMultiplexer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<BasketSettings>>().Value;
                var configuration = ConfigurationOptions.Parse(settings.ConnectionString, true);
            
                configuration.ResolveDns = true;
            
                return ConnectionMultiplexer.Connect(configuration);
            });
            //
            //
            // if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
            // {
            //     services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            //     {
            //         var logger = sp.GetRequiredService<ILogger<DefaultServiceBusPersisterConnection>>();
            //
            //         var serviceBusConnectionString = Configuration["EventBusConnection"];
            //         var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);
            //
            //         return new DefaultServiceBusPersisterConnection(serviceBusConnection, logger);
            //     });
            // }
            // else
            // {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            
                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                factory.UserName = Configuration["EventBusUserName"] ?? factory.UserName;
                factory.Password = Configuration["EventBusPassword"] ?? factory.Password;
                var retryCount = int.TryParse(Configuration["EventBusRetryCount"], out var i) ? i : 5;

            
                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            // }
            //
             RegisterEventBus(services);
            //
            //
            // services.AddCors(options =>
            // {
            //     options.AddPolicy("CorsPolicy",
            //         builder => builder
            //         .SetIsOriginAllowed((host) => true)
            //         .AllowAnyMethod()
            //         .AllowAnyHeader()
            //         .AllowCredentials());
            // });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IBasketRepository, RedisBasketRepository>();
            services.AddTransient<IIdentityService, IdentityService>();
            
            services.AddOptions();
            
            // var container = new ContainerBuilder();
            // container.Populate(services);
            //
            // return new AutofacServiceProvider(container.Build());
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
                    setup.DocumentTitle = "Basket.Api Swagger";
                    setup.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/v1/swagger.json", "Basket.API V1");
                    setup.RoutePrefix = string.Empty;
                    //TODO
                    // setup.OAuthClientId("basketswaggerui");
                    // setup.OAuthAppName("Basket Swagger UI");
                });

            // app.UseRouting();
            ConfigureAuth(app);

            // app.UseStaticFiles();

            // app.UseCors("CorsPolicy");

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
        
        // private void RegisterAppInsights(IServiceCollection services)
        // {
        //     services.AddApplicationInsightsTelemetry(Configuration);
        //     services.AddApplicationInsightsKubernetesEnricher();
        // }
        
        private void ConfigureAuthService(IServiceCollection services)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            var identityUrl = Configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "basket";
            });
        }

        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            app.UseAuthentication();
            app.UseAuthorization();
        }
        
                private void RegisterEventBus(IServiceCollection services)
        {
             var subscriptionClientName = Configuration["SubscriptionClientName"];
        //
        //     if (Configuration.GetValue<bool>("AzureServiceBusEnabled"))
        //     {
        //         services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
        //         {
        //             var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
        //             var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        //             var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
        //             var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
        //
        //             return new EventBusServiceBus(serviceBusPersisterConnection, logger,
        //                 eventBusSubcriptionsManager, subscriptionClientName, iLifetimeScope);
        //         });
        //     }
        //     else
        //     {
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
        
            var retryCount = 5;
            if (!string.IsNullOrEmpty(Configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(Configuration["EventBusRetryCount"]);
            }
        
            return new EventBusRabbitMQ(rabbitMqPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
        });
        //     }
        //
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        //
        //     services.AddTransient<ProductPriceChangedIntegrationEventHandler>();
        //     services.AddTransient<OrderStartedIntegrationEventHandler>();
         }
        //
        // private void ConfigureEventBus(IApplicationBuilder app)
        // {
        //     var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        //
        //     eventBus.Subscribe<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        //     eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        // }


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