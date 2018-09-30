using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApp.Configure.Models.Behavior.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace WebApp.Configure.Models.Behavior
{
    public class InDIBehavior<T> : IBehavior where T: class, IBehavior
    {

        public Task OnContinue(HttpContext context, RequestDelegate next)
            => context.RequestServices.GetService<T>().OnContinue(context, next); 

        public Task OnLock(HttpContext context, RequestDelegate next)
            => context.RequestServices.GetService<T>().OnLock(context, next);

    }
}
