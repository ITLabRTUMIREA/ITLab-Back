using Models.PublicAPI.NotifyRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public interface INotifySender
    {
        Task<bool> TrySendNotify(NotifyType notifyType, object data);
    }
}
