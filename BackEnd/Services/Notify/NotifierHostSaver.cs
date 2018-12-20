using BackEnd.Models.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Services.Notify
{
    public class NotifierHostSaver
    {
        public string Host { get; set; }

        public NotifierHostSaver(IOptions<NotifierSettings> options)
        {
            Host = options.Value.Host;
        }
    }
}
