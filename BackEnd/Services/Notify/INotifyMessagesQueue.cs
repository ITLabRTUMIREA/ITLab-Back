using Models.PublicAPI.NotifyRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public interface INotifyMessagesQueue
    {
        void AddMessage((NotifyType notifyType, object data) message);
        bool TryGetMessage(out (NotifyType notifyType, object data) message);
    }
}
