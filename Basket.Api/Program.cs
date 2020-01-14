using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            CreateHostBuilder(configuration, args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Grpc", LogLevel.Debug);
                    logging.AddConsole(o => o.IncludeScopes = true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(false);
                    webBuilder.ConfigureKestrel(o =>
                    {
                        o.Listen(IPAddress.Loopback, 44376);
                        var ports = GetDefinedPorts(configuration);

                        // o.Listen(IPAddress.Any, 44376, options => { options.Protocols = HttpProtocols.Http1AndHttp2; });
                        // o.Listen(IPAddress.Any, ports.grpcPort,
                        //     options => { options.Protocols = HttpProtocols.Http1AndHttp2; });
                        // o.Listen(IPAddress.Any, 44376, options => { options.Protocols = HttpProtocols.Http2; });
                    });
                    webBuilder.UseUrls("http://localhost:44376");
                    webBuilder.UseStartup<Startup>();
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

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            var port = config.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }
}