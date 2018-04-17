using System.Linq;
using BackEnd.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.PublicAPI.Responses;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/EuipmentType")]
    public class EuipmentTypeController : Controller
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EuipmentTypeController> logger;

        public EuipmentTypeController(
            DataBaseContext dbContext,
            ILogger<EuipmentTypeController> logger)
        {
            this.logger = logger;
            this.dbContext = dbContext;
        }

        [HttpGet("{a}/{b}")]
        public ResponseBase Get(int a, int b)
        {
            logger.LogDebug((a / b).ToString());
            return new ResponseBase(ResponseStatusCode.OK);
        }

        // GET: api/EuipmentType/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/EuipmentType
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/EuipmentType/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
