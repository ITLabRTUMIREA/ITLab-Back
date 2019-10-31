using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder UseConfigFile(this IHostBuilder builder, string fileName, bool optional = false)
        {
            try
            {
                return builder
                    .ConfigureAppConfiguration((ctx, config) => config.AddJsonFile(fileName, optional));
            }
            catch (Exception ex)
            {
                throw new Exception($"Please, add JSON file {fileName} in project folder", ex);
            }
        }
    }
}
