using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Omi.Education.Web.Management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .UseSerilog((hostingContext, loggerConfiguration) => { loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration); })
                          .Build();
        }
    }
}