using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Settings
{
    /// <summary>
    /// Settings for redis notify service
    /// </summary>
    public class RedisNotifierSettings
    {
        /// <summary>
        /// Host path for redis
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Redis cahnnel name
        /// </summary>
        public string ChannelName { get; set; }
    }
}
