using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Extensions;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;

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
                .UseStartup<Startup>()
                .Build();
    }
}
