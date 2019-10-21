using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.PublicAPI.NotifyRequests;
using ServiceStack.Redis;

namespace BackEnd.Services.Notify
{
    public class RedisNotifySender : JsonNotifySender
    {
        private readonly RedisNotifierSettings settings;
        private readonly ILogger<RedisNotifySender> logger;

        private readonly Task<bool> FalseResult = Task.FromResult(false);
        private readonly Task<bool> TrueResult = Task.FromResult(true);

        public RedisNotifySender(
            IOptions<RedisNotifierSettings> settings,
            ILogger<RedisNotifySender> logger)
        {
            this.settings = settings.Value;
            this.logger = logger;
        }

        public override Task<bool> TrySendNotify(NotifyType notifyType, object data)
        {
            try
            {
                using (var redisClient = new RedisClient(settings.ConnectionString))
                {
                    var message = ToJson(notifyType, data);
                    var readed = redisClient.PublishMessage(settings.ChannelName, message);
                    logger.LogTrace($"message {message.Length}, length in bytes: {Encoding.UTF8.GetByteCount(message)}, sended: {readed}");
                }
                return TrueResult;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "error whgile sending message to redis");
                return FalseResult;
            }
        }
    }
}
