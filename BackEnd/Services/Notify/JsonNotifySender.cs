using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.PublicAPI.NotifyRequests;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BackEnd.Services.Notify
{
    public abstract class JsonNotifySender : INotifySender
    {
        private static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        public abstract Task<bool> TrySendNotify(NotifyType notifyType, object data);

        protected string ToJson(NotifyType notifyType, object data, string secret = "")
        {
            return JsonConvert.SerializeObject(new NotifyRequest<object>
            {
                Type = notifyType,
                Data = data,
                Secret = secret
            }, SerializeSettings);
        }
    }
}
