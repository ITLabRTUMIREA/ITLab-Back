using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Extensions;

namespace BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfigFile("appsettings.Secret.json")
                .ConfigureAppConfiguration(config => config
                                              .AddCommandLine(args)
                                              .AddEnvironmentVariables())
                .UseStartup<Startup>()
                .Build();
    }
}
