using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Services.Notify
{
    class MessageQueueNotifier : INotifier
    {
        private readonly INotifyMessagesQueue notifyMessagesQueue;
        private readonly ILogger<MessageQueueNotifier> logger;

        public MessageQueueNotifier(
            INotifyMessagesQueue notifyMessagesQueue,
            ILogger<MessageQueueNotifier> logger)
        {
            this.notifyMessagesQueue = notifyMessagesQueue;
            this.logger = logger;
        }
        public void Notify(NotifyType notifyType, object data)
        {
            notifyMessagesQueue.AddMessage((notifyType, data));
        }
    }
}