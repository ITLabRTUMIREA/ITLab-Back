using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackEnd.DataBase;
using BackEnd.Extensions;
using BackEnd.Models.Settings;
using BackEnd.Services.Notify;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.PublicAPI.Requests.Summary;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers
{
    [Route("api/debug")]
    public class DebugController : Controller
    {
        private readonly IOptions<NotifierSettings> options;
        private readonly NotifierHostSaver notifierHostSaver;

        public DebugController(
            IOptions<NotifierSettings> options,
            NotifierHostSaver notifierUrlSaver)
        {
            this.options = options;
            this.notifierHostSaver = notifierUrlSaver;
        }

        [HttpPost]
        public IActionResult ChangeUrl([FromBody]string notifierHost)
        {
            if (!options.Value.NeedChangeUrl)
                return BadRequest();
            notifierHostSaver.Host = notifierHost;
            return Content(notifierHost);
        }
    }
}
