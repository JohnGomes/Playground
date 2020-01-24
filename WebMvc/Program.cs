using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using WebApplication;

namespace Microsoft.eShopContainers.WebMVC
{
    
    public class Program
    {
        public static readonly string Namespace = typeof(Program).Namespace;
        public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            var configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);

            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = BuildWebHost(configuration, args);

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost BuildWebHost(IConfiguration configuration, string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(false)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .UseConfiguration(configuration)
                .UseSerilog()
                .Build();

         private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var seqServerUrl = configuration["Serilog:SeqServerUrl"];
            var logstashUrl = configuration["Serilog:LogstashgUrl"];
            var cfg = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console();
            if (!string.IsNullOrWhiteSpace(seqServerUrl)) {
                cfg.WriteTo.Seq(seqServerUrl);
            }
            if (!string.IsNullOrWhiteSpace(logstashUrl)) {
                cfg.WriteTo.Http(logstashUrl);
            }
            return cfg.CreateLogger();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
    
    // public class Program
    // {
    //     public static void Main(string[] args)
    //     {
    //         CreateHostBuilder(args).Build().Run();
    //     }
    //
    //     public static IHostBuilder CreateHostBuilder(string[] args) =>
    //         Host.CreateDefaultBuilder(args)
    //             .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    // }
}