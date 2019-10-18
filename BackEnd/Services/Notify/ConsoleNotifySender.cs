using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.NotifyRequests;
using Models.PublicAPI.Responses.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Services.Notify
{
    class ConsoleNotifySender : INotifySender
    {
        private static readonly Task<bool> succesResult = Task.FromResult(true);
        private readonly ILogger<ConsoleNotifySender> logger;

        public ConsoleNotifySender(ILogger<ConsoleNotifySender> logger)
        {
            this.logger = logger;
        }

        public Task<bool> TrySendNotify(NotifyType notifyType, object data)
        {
            var message = "Event Add New Event, content: \n"
                + JsonConvert.SerializeObject(new { notifyType, data }, Newtonsoft.Json.Formatting.Indented);
            logger.LogInformation(message);
            return succesResult;
        }
    }
}