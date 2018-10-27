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
    class Notifier : INotifier
    {
        private readonly ILogger<Notifier> logger;
        private readonly HttpClient httpClient;
        public const string HttpClientName = "KotlinNotifierHttpClient";


        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public Notifier(
            IHttpClientFactory httpClientFactory,
            ILogger<Notifier> logger)
        {
            this.logger = logger;
            httpClient = httpClientFactory.CreateClient(HttpClientName);
            SerializeSettings.Converters.Add(new StringEnumConverter());
        }
        public async Task Notify(NotifyType notifyType, object eventVew)
        {
            var content = JsonConvert.SerializeObject(new NotifyRequest<object> { Type = notifyType, Data = eventVew }, SerializeSettings);
            var result = await httpClient.PostAsync("", new StringContent(content, Encoding.UTF8, "application/json"));
            if (result.StatusCode != HttpStatusCode.OK)
                logger.LogWarning($"error while send info to notify hub (bot) statusCode: {result.StatusCode}, base address: {httpClient.BaseAddress}, content: {content}");
        }
    }
}