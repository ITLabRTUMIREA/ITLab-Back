using BackEnd.Models.Settings;
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
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public class HttpNotifySender : INotifySender
    {
        public const string HttpClientName = nameof(HttpNotifySender);

        private readonly ILogger<HttpNotifySender> logger;
        private readonly HttpClient httpClient;
        private readonly IOptions<NotifierSettings> settings;
        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public HttpNotifySender(
            IHttpClientFactory httpClientFactory,
            IOptions<NotifierSettings> settings,
            ILogger<HttpNotifySender> logger)
        {
            httpClient = httpClientFactory.CreateClient(HttpClientName);
            this.settings = settings;
            this.logger = logger;
        }

        public async Task<bool> TrySendNotify(NotifyType notifyType, object data)
        {
            try
            {
                var content = JsonConvert.SerializeObject(new NotifyRequest<object>
                {
                    Type = notifyType,
                    Data = data,
                    Secret = settings.Value.NotifySecret
                }, SerializeSettings);

                var result = await httpClient.PostAsync("", new StringContent(content, Encoding.UTF8, "application/json"));
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    logger.LogWarning($"error while send info to notify hub (bot) statusCode: {result.StatusCode}, base address: {httpClient.BaseAddress}, content: {content}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"error while send info to notify hub (bot) base address: {httpClient.BaseAddress}");
                return false;
            }
            return true;
        }
    }
}
