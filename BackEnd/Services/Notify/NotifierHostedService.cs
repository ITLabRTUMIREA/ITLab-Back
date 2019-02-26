using BackEnd.Models.Settings;
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
        public const string HttpClientName = "KotlinNotifierHttpClient";

        private readonly INotifyMessagesQueue messagesQueue;
        private readonly ILogger<NotifierHostedService> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<NotifierSettings> settings;
        private TimeSpan delay = TimeSpan.FromSeconds(5);
        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public NotifierHostedService(
            INotifyMessagesQueue messagesQueue,
            ILogger<NotifierHostedService> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<NotifierSettings> settings)
        {
            this.messagesQueue = messagesQueue;
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
            this.settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var httpClient = httpClientFactory.CreateClient(HttpClientName);
                if (!messagesQueue.TryGetMessage(out (NotifyType notifyType, object data) message))
                {
                    await Task.Delay(delay);
                    continue;
                }
                try
                {
                    var content = JsonConvert.SerializeObject(new NotifyRequest<object>
                    {
                        Type = message.notifyType,
                        Data = message.data,
                        Secret = settings.Value.NotifySecret
                    }, SerializeSettings);
                    var result = await httpClient.PostAsync("", new StringContent(content, Encoding.UTF8, "application/json"));
                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        logger.LogWarning($"error while send info to notify hub (bot) statusCode: {result.StatusCode}, base address: {httpClient.BaseAddress}, content: {content}");
                        messagesQueue.AddMessage(message);
                        await Task.Delay(delay);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"error while send info to notify hub (bot) base address: {httpClient.BaseAddress}");
                    messagesQueue.AddMessage(message);
                    await Task.Delay(delay);
                }
            }
        }
    }
}
