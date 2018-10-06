using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Models.Roles;
using Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Equipments;
using Models.People;
using Models.People.Roles;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.General;
using Microsoft.Data.Edm.Csdl;
using BackEnd.Extensions;
using System.Threading;

namespace BackEnd.Controllers.Equipments
{
    [Produces("application/json")]
    [Route("api/Equipment")]
    public class EquipmentController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
            => await dbContext
                .Equipments
                .ProjectTo<EquipmentView>()
                .SingleOrDefaultAsync(eq => eq.Id == id)
                ?? throw NotFoundMyApi();

        [HttpGet]
        public async Task<ListResponse<CompactEquipmentView>> GetAsync(
            Guid? eventId,
            Guid? equipmentTypeId,
            string match
        )
            => await dbContext
                .Equipments
                .If(eventId.HasValue,
                    equipments =>
                        equipments.Where(eq => eq.PlaceEquipments.Any(pe => pe.Place.Shift.EventId == eventId)))
                .If(equipmentTypeId.HasValue,
                    equipments => equipments.Where(eq => eq.EquipmentTypeId == equipmentTypeId))
                .IfNotNull(match, equipments => equipments.ForAll(match.Split(' '), (equipments2, matcher) =>
                        equipments2.Where(eq => eq.SerialNumber.ToUpper().Contains(matcher)
                                                || eq.EquipmentType.Title.ToUpper().Contains(matcher))))
                .ProjectTo<CompactEquipmentView>()
                .ToListAsync();

        [RequireRole(RoleNames.CanEditEquipment)]
        [HttpPost]
        public async Task<OneObjectResponse<EquipmentView>> PostAsync([FromBody]EquipmentCreateRequest request)
        {
            try
            {
                await semaphore.WaitAsync();

                var type = await CheckAndGetEquipmentTypeAsync(request.EquipmentTypeId);
                await CheckNotExist(request.SerialNumber);

                var newEquipment = mapper.Map<Equipment>(request);
                newEquipment.Number = type.LastNumber++;
                if (request.Children?.Count > 0)
                    newEquipment.Children =
                        await dbContext
                        .Equipments
                        .Where(eq => request.Children.Contains(eq.Id))
                        .ToListAsync();

                if (newEquipment.Children?.Count != request.Children?.Count)
                    throw ResponseStatusCode.IncorrectEquipmentIds.ToApiException();

                await dbContext.Equipments.AddAsync(newEquipment);
                await dbContext.SaveChangesAsync();
                return mapper.Map<EquipmentView>(newEquipment);
            }
            finally
            {
                semaphore.Release();
            }

        }

        [RequireRole(RoleNames.CanEditEquipment)]
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

        [RequireRole(RoleNames.CanEditEquipment)]
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
              ?? throw NotFoundMyApi();

        private async Task CheckNotExist(string serialNumber)
        {
            if (await dbContext
                .Equipments
                .AnyAsync(eq => eq.SerialNumber == serialNumber))
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
        }

        private async Task<EquipmentType> CheckAndGetEquipmentTypeAsync(Guid typeId)
            => await dbContext.EquipmentTypes.FindAsync(typeId)
                ?? throw ApiLogicException.Create(ResponseStatusCode.EquipmentTypeNotFound);

    }
}
