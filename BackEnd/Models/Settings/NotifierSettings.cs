using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Models.Settings
{
    public class NotifierSettings
    {
        public string Host { get; set; }
        public string AccessToken { get; set; }
        public bool NeedChangeUrl { get; set; }
        public string NotifySecret { get; set; }
    }
}
