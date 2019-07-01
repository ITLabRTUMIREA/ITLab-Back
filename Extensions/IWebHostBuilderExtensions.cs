using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
namespace Extensions
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseConfigFile(this IWebHostBuilder builder, string fileName, bool optional = false)
        {
            try
            {
                return builder
                    .ConfigureAppConfiguration(config => config.AddJsonFile(fileName, optional));
            }
            catch (Exception ex)
            {
                throw new Exception($"Please, add JSON file {fileName} in project folder", ex);
            }
        }
    }
}
