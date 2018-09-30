using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using BackEnd.Models.Settings;
using Microsoft.Extensions.Options;

namespace BackEnd.Controllers
{
    [Route("api/about")]
    public class AboutController : Controller
    {
        // GET: api/values
        [HttpGet]
        public object Get()
        {

            return new
            {
                StatusCode = Enum.GetNames(typeof(ResponseStatusCode))
                                 .Select(n => new { name = n, code = (int)Enum.Parse(typeof(ResponseStatusCode), n) })
            };
        }
        [HttpGet("build")]
        public BuildInformation GetBuild(
            [FromServices]IOptions<BuildInformation> buildInfo)
        {
            return buildInfo.Value;
        }

    }
}
