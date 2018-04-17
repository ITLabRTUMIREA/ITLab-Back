using System;
using Microsoft.AspNetCore.Hosting;

namespace Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseConfigFile(this IWebHostBuilder builder, string fileName){
            try {
                return builder;
                    //.ConfigureAppConfiguration((arg1, arg2) => )config => config.AddJsonFile(fileName, false));
            } catch {}
            return builder;
        }
    }
}
