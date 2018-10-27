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
        public Task AddNewEvent(EventView eventVew)
        {
            var message = "Event Add New Event, content: \n"
                + JsonConvert.SerializeObject(eventVew, Newtonsoft.Json.Formatting.Indented);
            logger.LogInformation(message);
            return Task.CompletedTask;
        }
    }
}