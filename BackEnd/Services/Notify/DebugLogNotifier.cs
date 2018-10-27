using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Services.Notify
{
    class DebugLogNotifier : INotifier
    {
        private readonly ILogger<DebugLogNotifier> logger;

        public DebugLogNotifier(ILogger<DebugLogNotifier> logger)
        {
            this.logger = logger;
        }
        public Task Notify(NotifyType notifyType, object data)
        {
            var message = "Event Add New Event, content: \n"
                + JsonConvert.SerializeObject(new { notifyType, data }, Newtonsoft.Json.Formatting.Indented);
            logger.LogInformation(message);
            return Task.CompletedTask;
        }
    }
}