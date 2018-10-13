using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Services.Notify
{
    class Notifier : INotifier
    {
        private readonly ILogger<Notifier> logger;
        private readonly HttpClient httpClient;
        public const string HttpClientName = "KotlinNotifierHttpClient";
        public Notifier(
            IHttpClientFactory httpClientFactory,
            ILogger<Notifier> logger)
        {
            this.logger = logger;
            httpClient = httpClientFactory.CreateClient(HttpClientName);
        }
        public async Task AddNewEvent(EventView eventVew)
        {
            var content = JsonConvert.SerializeObject(new NotifyRequest<EventView> {Type = NotifyType.EventNew.ToString(), Data = eventVew}, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var result = await httpClient.PostAsync("", new StringContent(content, Encoding.UTF8, "application/json"));
            if (result.StatusCode != HttpStatusCode.OK)
                logger.LogWarning($"error while send info to notify hub (bot) statusCode: {result.StatusCode}, base address: {httpClient.BaseAddress}, content: {content}");
        }
    }
}