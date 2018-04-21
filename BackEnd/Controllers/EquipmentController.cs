using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;

namespace BackEnd.Controllers
{
    [Produces("application/json")]
    [Route("api/Equipment")]
    public class EquipmentController : Controller
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EquipmentTypeController> logger;
        private readonly IMapper mapper;

        public EquipmentController(
            DataBaseContext dbContext,
            ILogger<EquipmentTypeController> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public ListResponse<Equipment> Get()
            =>
            ListResponse<Equipment>.Create(dbContext.Equipments);

        [HttpGet("{id}")]
        public async Task<OneObjectResponse<Equipment>> GetAsync(Guid id)
            =>
            OneObjectResponse<Equipment>.Create(
                await dbContext.Equipments.FindAsync(id)
                ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound));

        // POST: api/Equipment
        [HttpPost]
        public async Task<OneObjectResponse<Equipment>> PostAsync([FromBody]EquipmentCreateRequest value)
        {
            var type = dbContext.EquipmentTypes.FirstOrDefault(eqt => eqt.Id == value.EquipmentTypeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.EquipmentTypeNotFound);
            var now = dbContext.Equipments.FirstOrDefault(eq => eq.SerialNumber == value.SerialNumber);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);

            var newEquipment = mapper.Map<Equipment>(value);
            await dbContext.Equipments.AddAsync(newEquipment);
            await dbContext.SaveChangesAsync();
            return OneObjectResponse<Equipment>.Create(newEquipment);
        }

        // PUT: api/Equipment/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
