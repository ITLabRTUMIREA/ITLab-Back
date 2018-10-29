using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;

namespace BackEnd.Services.Notify
{
    public interface INotifier
    {
        Task Notify(NotifyType notifyType, object data);
    }
}
