using BackEnd.DataBase;
using Microsoft.AspNetCore.Mvc;
using static Models.PublicAPI.Response;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/EuipmentType")]
    public class EuipmentTypeController : Controller
    {
        private readonly DataBaseContext dbContext;

        public EuipmentTypeController(DataBaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/EuipmentType
        public Base Get()
        {
            return new Base(Models.PublicAPI.Response.StatusCode.OK);
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
