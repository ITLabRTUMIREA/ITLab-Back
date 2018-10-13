using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.General;

namespace BackEnd.Services.Notify
{

    public class NotifyAttribute : Attribute, IFilterFactory
    {
        private readonly NotifyType type;

        public bool IsReusable => false;

        public NotifyAttribute(NotifyType type)
        {
            this.type = type;
        }
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
            => new NotifyFilter(serviceProvider.GetRequiredService<INotifier>(), type);


        private class NotifyFilter : IAsyncActionFilter
        {
            private readonly INotifier notifier;
            private readonly NotifyType type;

            public NotifyFilter(INotifier notifier, NotifyType type)
            {
                this.notifier = notifier;
                this.type = type;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var resultContext = await next();
                if (!(resultContext.Result is ObjectResult result))
                    return;
                switch (type)
                {
                    case NotifyType.EventNew:
                        await notifier.AddNewEvent((result.Value as OneObjectResponse<EventView>)?.Data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type));
                }
            }
        }
    }
}
