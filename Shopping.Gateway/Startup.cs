using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Shopping.Gateway.Config;
using Shopping.Gateway.Filters.Basket.API.Infrastructure.Filters;
using Shopping.Gateway.Services;

namespace Shopping.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IWebHostEnvironment Env { get; set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri(Configuration["BasketUrlHC"]), "basketapi-check", tags: new[] {"basketapi"});
                // .AddUrlGroup(new Uri(Configuration["CatalogUrlHC"]), name: "catalogapi-check", tags: new string[] {"catalogapi"})
                // .AddUrlGroup(new Uri(Configuration["OrderingUrlHC"]), name: "orderingapi-check", tags: new string[] {"orderingapi"})
                // .AddUrlGroup(new Uri(Configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] {"identityapi"});
            // .AddUrlGroup(new Uri(Configuration["MarketingUrlHC"]), name: "marketingapi-check", tags: new string[] { "marketingapi" })
            // .AddUrlGroup(new Uri(Configuration["PaymentUrlHC"]), name: "paymentapi-check", tags: new string[] { "paymentapi" })
            // .AddUrlGroup(new Uri(Configuration["LocationUrlHC"]), name: "locationapi-check", tags: new string[] { "locationapi" });

            services.AddControllers().AddNewtonsoftJson();
            // services.AddMvc();
            services.AddCustomMvc(Configuration);
            services.AddCustomAuthentication(Configuration);
            services.AddApplicationServices(Env, Configuration);
            // services.Configure<UrlsConfig>(Configuration.GetSection("urls"));
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo {Title = "Shopping Gateway", Version = "v1"});
            // });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO
            var pathBase = ""; //"/basket-api";//Configuration["PATH_BASE"];

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // app.UseStaticFiles();
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.DocumentTitle = "Shopping Gateway Swagger";
                c.SwaggerEndpoint(
                    $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                    "Shopping Gateway V1");
                c.RoutePrefix = string.Empty;
                //TODO
                // c.OAuthClientId("mobileshoppingaggswaggerui");
                // c.OAuthClientSecret(string.Empty);
                // c.OAuthRealm(string.Empty);
                // c.OAuthAppName("Purchase BFF Swagger UI");
            });


            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            var identityUrl = configuration.GetValue<string>("urls:identity");
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = identityUrl;
                    options.RequireHttpsMetadata = false;
                    options.Audience = "webshoppingagg";
                });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<UrlsConfig>(configuration.GetSection("urls"));

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSwaggerGen(options =>
            {
                //options.DescribeAllEnumsAsStrings();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Shopping Gateway",
                    Version = "v1",
                    Description = "Shopping Aggregator for Web Clients"
                });
                options.CustomSchemaIds(x => x.FullName);
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),

                            Scopes = new Dictionary<string, string>()
                            {
                                {"webshoppingagg", "Shopping Aggregator for Web Clients"}
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IHostEnvironment env,
            IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                services.AddHttpClient<IBasketApiClient, BasketApiClient>()
                    .ConfigurePrimaryHttpMessageHandler(ByPassSslCert);

                services.AddHttpClient<IBasketService, BasketService>()
                    .ConfigurePrimaryHttpMessageHandler(ByPassSslCert);

                services.AddHttpClient<IOrderingService, OrderingService>()
                    .ConfigurePrimaryHttpMessageHandler(ByPassSslCert);

                services.AddHttpClient<IBasketGrpcClient, BasketGrpcClient>()
                    .ConfigurePrimaryHttpMessageHandler(ByPassSslCert);

                services.AddHttpClient<ICatalogGrpcService, CatalogGrpcService>()
                    .ConfigurePrimaryHttpMessageHandler(ByPassSslCert);
                //.AddDevspacesSupport();
            }
            else
            {
                services.AddHttpClient<IBasketApiClient, BasketApiClient>();
                services.AddHttpClient<IBasketService, BasketService>();
                services.AddHttpClient<IBasketGrpcClient, BasketGrpcClient>();
                services.AddHttpClient<ICatalogGrpcService, CatalogGrpcService>();
            }

            //TODO

            //register delegating handlers
            //services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register http services


            //.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            //.AddDevspacesSupport();


            return services;
        }

        public static HttpClientHandler ByPassSslCert()
        {
            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            // (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true;
            return handler;
        }
    }
}