using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApp.Configure.Models.Behavior.Interfaces;
namespace WebApp.Configure.Models.Behavior
{
    public class DefaultBehavior : IBehavior
    {

        public virtual Task OnContinue(HttpContext context, RequestDelegate next)
            => next(context);

        public virtual Task OnLock(HttpContext context, RequestDelegate next)
        {
            context.Response.StatusCode = 503;
            return Task.CompletedTask;
        }
    }
}
