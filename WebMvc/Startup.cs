using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
// using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Certificate;
// using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.eShopOnContainers.WebMVC;
using Microsoft.eShopOnContainers.WebMVC.Services;
using Microsoft.eShopOnContainers.WebMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Logging;
using WebMVC.Infrastructure;
using WebMVC.Infrastructure.Middlewares;
using WebMVC.Services;

namespace WebApplication
{
        public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the IoC container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO
            IdentityModelEventSource.ShowPII = true;
            services.AddControllersWithViews()
                .Services
                .AddAppInsight(Configuration)
                .AddHealthChecks(Configuration)
                .AddCustomMvc(Configuration)
                // .AddDevspaces()
                .AddHttpClientServices(Configuration, _env);

            // IdentityModelEventSource.ShowPII  = true;       // Caution! Do NOT use in production: https://aka.ms/IdentityModel/PII
            
            services.AddControllers();

            services.AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseStaticFiles();
            app.UseSession();

            if (Configuration.GetValue<bool>("UseLoadTest"))
            {
                app.UseMiddleware<ByPassAuthMiddleware>();
            }

            WebContextSeed.Seed(app, env);

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Catalog}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultError", "{controller=Error}/{action=Error}");
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }

    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddAppInsight(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(configuration);
            // services.AddApplicationInsightsKubernetesEnricher();

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddCheck("identityapi-check", () =>
                {
                    try
                    {
                        using (var ping = new Ping())
                        {
                            var reply = ping.Send(configuration["IdentityUrlHC"]);
                            if (reply.Status != IPStatus.Success)
                            {
                                return HealthCheckResult.Unhealthy();
                            }

                            if (reply.RoundtripTime > 100)
                            {
                                return HealthCheckResult.Degraded();
                            }

                            return HealthCheckResult.Healthy();
                        }
                    }
                    catch
                    {
                        return HealthCheckResult.Unhealthy();
                    }
                });
                // .AddUrlGroup(new Uri(configuration["IdentityUrlHC"]), name: "identityapi-check", tags: new string[] { "identityapi" });

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);
            services.AddSession();
            services.AddDistributedMemoryCache();

            // if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            // {
            //     services.AddDataProtection(opts =>
            //     {
            //         opts.ApplicationDiscriminator = "eshop.webmvc";
            //     })
            //     .PersistKeysToRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys");
            // }

            return services;
        }

        // Adds all Http client services
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestIdDelegatingHandler>();

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            // services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5)).AddDevspacesSupport();

            //add http client services
            services.AddHttpClient<IBasketService, BasketService>()
                .ConfigurePrimaryHttpMessageHandler(IgnoreSslErrors())
                .ConfigureHttpMessageHandlerBuilder(HandlerIgnoreSsl())
                .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //Sample. Default lifetime is 2 minutes
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
                   // .AddDevspacesSupport();

                   if (env.IsDevelopment())
                       services.AddHttpClient<ICatalogService, CatalogService>()
                           .ConfigurePrimaryHttpMessageHandler(IgnoreSslErrors())
                           .ConfigureHttpMessageHandlerBuilder(HandlerIgnoreSsl());
                   

                   // if (!env.IsDevelopment())
                   //     services.AddHttpClient<ICatalogService, CatalogService>();// .AddDevspacesSupport();

                   services.AddHttpClient<IOrderingService, OrderingService>()
                       .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                       .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>();
                 // .AddDevspacesSupport();

                 services.AddHttpClient<ICampaignService, CampaignService>()
                     .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
                // .AddDevspacesSupport();

                services.AddHttpClient<ILocationService, LocationService>()
                    .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
               // .AddDevspacesSupport();

            //add custom application services
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();
            

            return services;
        }

        private static Action<HttpMessageHandlerBuilder> HandlerIgnoreSsl()
        {
            return handler => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        }

        private static Func<HttpMessageHandler> IgnoreSslErrors()
        {
            return () => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    // (httpRequestMessage, cert, cetChain, policyErrors) =>
                    // {
                    //     return true;
                    // }
            };
        }


        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var useLoadTest = configuration.GetValue<bool>("UseLoadTest");
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            // Add Authentication services         

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                    //TODO
                    // options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = identityUrl;
                    options.SignedOutRedirectUri = callBackUrl;
                    options.ClientId = useLoadTest ? "mvctest" : "mvc";
                    options.ClientSecret = "secret";
                    options.ResponseType = useLoadTest ? "code id_token token" : "code id_token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = false;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("orders");
                    options.Scope.Add("basket");
                    options.Scope.Add("marketing");
                    options.Scope.Add("locations");
                    options.Scope.Add("webshoppingagg");
                    options.Scope.Add("orders.signalrhub");
                });

            return services; 
        }
    }
}