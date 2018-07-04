using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Equipments;
using Models.People;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;

namespace BackEnd.Controllers.Equipments
{
    [Produces("application/json")]
    [Route("api/Equipment")]
    public class EquipmentController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EquipmentTypeController> logger;
        private readonly IMapper mapper;

        public EquipmentController(
            DataBaseContext dbContext,
            ILogger<EquipmentTypeController> logger,
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EquipmentView>> GetAsync(Guid id)
            => mapper.Map<EquipmentView>(await CheckAndGetEquipmentAsync(id));

        [HttpGet]
        public async Task<ListResponse<EquipmentView>> GetAsync(
            [FromQuery] Guid? eventId,
            [FromQuery] Guid? equipmentTypeId,
            [FromQuery] string equipmentTypeName
        )
        {
            IQueryable<Equipment> equipments = dbContext.Equipments;
            if (eventId.HasValue)
            {
                equipments = equipments
                    .Where(eq => eq.EventEquipments.Any(eveq => eveq.EventId == eventId.Value));
            }
            if (equipmentTypeId.HasValue)
                equipments = equipments.Where(eq => eq.EquipmentTypeId == equipmentTypeId);
            if (!string.IsNullOrEmpty(equipmentTypeName))
                equipments = equipments.Where(eq => eq.EquipmentType.Title == equipmentTypeName);
            return await equipments.ProjectTo<EquipmentView>().ToListAsync();
        }

        [HttpPost]
        public async Task<OneObjectResponse<EquipmentView>> PostAsync([FromBody]EquipmentCreateRequest request)
        {
            var type = await CheckAndGetEquipmentTypeAsync(request.EquipmentTypeId);
            await CheckNotExist(request.SerialNumber);

            var newEquipment = mapper.Map<Equipment>(request);
            await dbContext.Equipments.AddAsync(newEquipment);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentView>(newEquipment);
        }

        [HttpPut]
        public async Task<OneObjectResponse<EquipmentView>> PutAsync(int id, [FromBody]EquipmentEditRequest request)
        {
            var toEdit = await CheckAndGetEquipmentAsync(request.Id);
            if (request.EquipmentTypeId.HasValue)
                await CheckAndGetEquipmentTypeAsync(request.EquipmentTypeId.Value);
            if (string.IsNullOrEmpty(request.SerialNumber))
                await CheckNotExist(request.SerialNumber);

            mapper.Map(request, toEdit);
            await dbContext.SaveChangesAsync();

            return mapper.Map<EquipmentView>(toEdit);
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
            => await dbContext.Equipments.Include(eq => eq.EquipmentType).FirstOrDefaultAsync(eq => eq.Id == id)
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
