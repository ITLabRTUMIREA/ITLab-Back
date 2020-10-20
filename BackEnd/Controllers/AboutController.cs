using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using BackEnd.Models.Settings;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using _build.shared;

namespace BackEnd.Controllers
{
    /// <summary>
    /// Controller for some information about server
    /// </summary>
    [AllowAnonymous]
    [Route("api/about")]
    public class AboutController : Controller
    {
        private readonly IOptions<BuildInformation> buildInfo;

        public AboutController(IOptions<BuildInformation> buildInfo)
        {
            this.buildInfo = buildInfo;
        }
        /// <summary>
        /// Get build information
        /// </summary>
        [HttpGet("build")]
        public ActionResult<BuildInformation> GetBuild()
        {
            return buildInfo.Value;
        }
    }
}
