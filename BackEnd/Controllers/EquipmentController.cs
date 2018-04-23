using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Equipment;
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
        public async Task<ListResponse<Equipment>> Get()
            => await dbContext.Equipments.ToListAsync();

        [HttpGet("{id}")]
        public async Task<OneObjectResponse<Equipment>> GetAsync(Guid id)
            => await CheckAndGetEquipmentAsync(id);


        [HttpPost]
        public async Task<OneObjectResponse<EquipmentPresent>> PostAsync([FromBody]EquipmentCreateRequest request)
        {
            var type = await CheckAndGetEquipmentTypeAsync(request.EquipmentTypeId);
            await CheckNotExist(request.SerialNumber);

            var newEquipment = mapper.Map<Equipment>(request);
            await dbContext.Equipments.AddAsync(newEquipment);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentPresent>(newEquipment);
        }

        [HttpPut]
        public async Task<OneObjectResponse<EquipmentPresent>> PutAsync(int id, [FromBody]EquipmentEditRequest request)
        {
            var toEdit = await CheckAndGetEquipmentAsync(request.Id);
            var targetType = await CheckAndGetEquipmentTypeAsync(request.EquipmentTypeId);
            await CheckNotExist(request.SerialNumber);

            toEdit.EquipmentType = targetType;
            toEdit.SerialNumber = request.SerialNumber;

            await dbContext.SaveChangesAsync();

            return mapper.Map<EquipmentPresent>(toEdit);
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            var toDelete = await CheckAndGetEquipmentAsync(request.Id);
            dbContext.Equipments.Remove(toDelete);
            await dbContext.SaveChangesAsync();
            return request.Id;
        }

        private async Task<Equipment> CheckAndGetEquipmentAsync(Guid id)
            => await dbContext.Equipments.FindAsync(id)
              ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);

        private async Task CheckNotExist(string serialNumber)
        {
            var now = await dbContext.Equipments.FirstOrDefaultAsync(eq => eq.SerialNumber == serialNumber);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
        }

        private async Task<EquipmentType> CheckAndGetEquipmentTypeAsync(Guid typeId)
            => await dbContext.EquipmentTypes.FindAsync(typeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.EquipmentTypeNotFound);

    }
}
