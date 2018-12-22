using Models.PublicAPI.NotifyRequests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public class CuncurrentBagMessagesQueue : INotifyMessagesQueue
    {
        private ConcurrentBag<(NotifyType notifyType, object data)> messages = new ConcurrentBag<(NotifyType notifyType, object data)>();
        public void AddMessage((NotifyType notifyType, object data) message)
        {
            messages.Add(message);
        }

        public bool TryGetMessage(out (NotifyType notifyType, object data) message)
        {
            return messages.TryTake(out message);
        }
    }
}
