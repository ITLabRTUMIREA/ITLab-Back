using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApp.Configure.Models.Shared.Interfaces;
using WebApp.Configure.Models.Behavior;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Configure.Models.Invokations
{
    public class WebAppConfigureMiddleware
    {


        private readonly RequestDelegate next;

        public WebAppConfigureMiddleware(RequestDelegate next)
        {
            this.next = next;
        }


        public Task InvokeAsync(HttpContext context)
        {
            var workStatusGetter = context.RequestServices.GetService<IWorkPathGetter>();
            var configureBuilder = context.RequestServices.GetService<ConfigureBuilder>();

            var workStatus = workStatusGetter.GetHandlePath();
            switch (workStatus)
            {
                case WorkHandlePath.Lock:
                    return configureBuilder.Behavior.OnLock(context, next);
                case WorkHandlePath.Continue:
                    return configureBuilder.Behavior.OnContinue(context, next);
                default:
                    throw new ArgumentOutOfRangeException(nameof(WorkHandlePath));
            }
        }
    }
    public static class WebAppConfigureMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebAppConfigure(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebAppConfigureMiddleware>();
        }
    }
}
