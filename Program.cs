using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<FileWatcherOptions>(hostContext.Configuration.GetSection("fileWatcherOptions"));
                    services.Configure<SmtpEmailClientOptions>(hostContext.Configuration.GetSection("smtpEmailClientOptions"));
                    services.AddHostedService<Worker>();
                });
    }
}
