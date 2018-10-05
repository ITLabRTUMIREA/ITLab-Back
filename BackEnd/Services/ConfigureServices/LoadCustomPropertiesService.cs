using System;
using System.Threading.Tasks;
using BackEnd.Services.UserProperties;
using WebApp.Configure.Models.Configure.Interfaces;
using System.Reflection;
using System.Linq;

namespace BackEnd.Services.ConfigureServices
{
    public class LoadCustomPropertiesService : IConfigureWork
    {
        private readonly IUserPropertiesConstants constants;

        public LoadCustomPropertiesService(IUserPropertiesConstants constants)
        {
            this.constants = constants;
        }

        public Task Configure()
        {
            constants.CustomTypeNames =
                Assembly
                    .GetAssembly(GetType())
                    .GetTypes()
                    .Where(t => t.GetCustomAttribute<UserPropertyInitializerAttribute>() != null)
                    .Select(t => t.GetCustomAttribute<UserPropertyInitializerAttribute>().UserPropertyName.ToString())
                    .ToArray();
            return Task.CompletedTask;
        }
    }
}
