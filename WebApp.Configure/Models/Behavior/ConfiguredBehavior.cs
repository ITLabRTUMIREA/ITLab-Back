using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApp.Configure.Models.Behavior.Interfaces;
using System.Collections.Generic;
namespace WebApp.Configure.Models.Behavior
{
    public class ConfiguredBehavior : DefaultBehavior
    {
        public Func<HttpContext, RequestDelegate, Task> ContinueAction { get; set; }
        public Func<HttpContext, RequestDelegate, Task> LockAction { get; set; }

        public override Task OnContinue(HttpContext context, RequestDelegate next)
            => (ContinueAction ?? base.OnContinue)(context, next);

        public override Task OnLock(HttpContext context, RequestDelegate next)
            => (LockAction ?? base.OnLock)(context, next);

    }
}
