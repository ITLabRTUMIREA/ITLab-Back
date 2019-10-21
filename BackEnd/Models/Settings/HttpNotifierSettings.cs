using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Settings
{
    /// <summary>
    /// Settings for notify service
    /// </summary>
    public class HttpNotifierSettings
    {
        /// <summary>
        /// Host path for notify service
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Change Host in runtime
        /// </summary>
        public bool NeedChangeUrl { get; set; }
        /// <summary>
        /// Secret to send notify messages
        /// </summary>
        public string NotifySecret { get; set; }
    }
}
