using BackEnd.Models.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public class HttpNotifierHostSaver
    {
        public string Host { get; set; }

        public HttpNotifierHostSaver(IOptions<NotifierSettings> options)
        {
            Host = options.Value.Host;
        }
    }
}
