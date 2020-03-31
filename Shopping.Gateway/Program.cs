using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Shopping.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration);
            CreateHostBuilder(configuration,args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Grpc", LogLevel.Information);
                    logging.AddConsole(o => o.IncludeScopes = true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(false);
                    webBuilder.ConfigureKestrel(o =>
                    {
                        var ports = GetDefinedPorts(configuration);

                        // o.Listen(IPAddress.Any, ports.httpPort, options => { options.Protocols = HttpProtocols.Http1AndHttp2; });
                        // o.Listen(IPAddress.Any, ports.grpcPort, options => { options.Protocols = HttpProtocols.Http2; });
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
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
        
        private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", "Shopping.Gateway")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://localhost:8080" : logstashUrl)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        
        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            var port = config.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }
    
    
}