using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Basket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            
            Log.Logger = CreateSerilogLogger(configuration);
            CreateHostBuilder(configuration, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Grpc", LogLevel.Debug);
                    logging.AddConsole(o => o.IncludeScopes = true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(false);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
        }

        
        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", "Basket.Api")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            //TODO
            // if (config.GetValue<bool>("UseVault", false))
            // {
            //     builder.AddAzureKeyVault(
            //         $"https://{config["Vault:Name"]}.vault.azure.net/",
            //         config["Vault:ClientId"],
            //         config["Vault:ClientSecret"]);
            // }

            return builder.Build();
        }

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            var port = config.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }
}