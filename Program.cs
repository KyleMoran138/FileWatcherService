using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace FileWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(@"C:\\temp\\fileTrackingWorkerService\\Log.txt")
                .CreateLogger();

            try
            {
                Log.Information("Starting the file watcher service");
                CreateHostBuilder(args).Build().Run();
            }catch(Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<FileWatcherOptions>(hostContext.Configuration.GetSection("fileWatcherOptions"));
                    services.Configure<SmtpEmailClientOptions>(hostContext.Configuration.GetSection("smtpEmailClientOptions"));
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
    }
}
