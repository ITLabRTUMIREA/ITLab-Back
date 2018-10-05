using System;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using WebApp.Configure.Models.Behavior;

namespace WebApp.Configure.Models.Configure.Interfaces
{
    public interface IConfigurationWorkBuilder
    {
        Type ConfigureWorkType { get; }
        WorkHandlePath WorkHandlePath { get; }
    }
}
