using BackEnd.Models.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.PublicAPI.NotifyRequests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public class NotifierHostedService : BackgroundService
    {

        private readonly INotifyMessagesQueue messagesQueue;
        private readonly ILogger<NotifierHostedService> logger;
        private readonly IServiceProvider serviceProvider;
        private TimeSpan delay = TimeSpan.FromSeconds(0.5);


        public NotifierHostedService(
            INotifyMessagesQueue messagesQueue,
            ILogger<NotifierHostedService> logger,
            IServiceProvider serviceProvider)
        {
            this.messagesQueue = messagesQueue;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!messagesQueue.TryGetMessage(out (NotifyType notifyType, object data) message))
                {
                    await Task.Delay(delay);
                    continue;
                }
                try
                {
                    var notifySender = serviceProvider.GetRequiredService<INotifySender>();
                    var success = await notifySender.TrySendNotify(message.notifyType, message.data);
                    if (!success)
                    {
                        messagesQueue.AddMessage(message);
                        await Task.Delay(delay);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"error while sending notify");
                    messagesQueue.AddMessage(message);
                    await Task.Delay(delay);
                }
            }
        }
    }
}
