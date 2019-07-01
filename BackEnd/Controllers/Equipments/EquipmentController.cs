using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Models.Roles;
using Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.People;
using Models.People.Roles;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Responses.Equipment;
using System.Threading;

namespace BackEnd.Controllers.Equipments
{

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
        public async Task<ActionResult<EquipmentView>> GetAsync(Guid id)
        {
            var equipmentView = await dbContext
                                      .Equipments
                                      .ProjectTo<EquipmentView>()
                                      .SingleOrDefaultAsync(eq => eq.Id == id);
            if (equipmentView == null)
                return NotFound();
            return equipmentView;
        }

        [HttpGet]
        public async Task<ActionResult<List<CompactEquipmentView>>> GetAsync(
            Guid? eventId,
            Guid? equipmentTypeId,
            string match,
            bool all
        )
            => await dbContext
                .Equipments
                .WhereIf(string.IsNullOrEmpty(match) && !eventId.HasValue && !equipmentTypeId.HasValue && !all,
                         eq => eq.ParentId == null)
                .WhereIf(eventId.HasValue, eq => eq.PlaceEquipments.Any(pe => pe.Place.Shift.EventId == eventId))
                .WhereIf(equipmentTypeId.HasValue, eq => eq.EquipmentTypeId == equipmentTypeId)

                .IfNotNull(match, equipments => equipments
                    .Variable(out var tokens, () => match.Split(' '))
                    .Variable(out var ints, () => tokens.GetInts())
                    .ForAll(tokens,
                    (equipments2, token) =>
                        equipments2.Where(eq => eq.SerialNumber.ToUpper().Contains(token)
                                                || eq.EquipmentType.Title.ToUpper().Contains(token)
                                                || ints.Contains(eq.Number))))
                .ProjectTo<CompactEquipmentView>()
                .ToListAsync();

        /// <summary>
        /// Create new Equipment
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created Equipment</returns>
        /// <response code="200">Returns newly created Equipment</response>
        /// <response code="400">If incorrect children id passed</response>
        /// <response code="404">If equipment type id don't exist</response>
        /// <response code="409">If serial number exists</response>
        [RequireRole(RoleNames.CanEditEquipment)]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<EquipmentView>> PostAsync([FromBody]EquipmentCreateRequest request)
        {
            try
            {
                await semaphore.WaitAsync();

                var type = await dbContext.EquipmentTypes.FindAsync(request.EquipmentTypeId);
                if (type == null)
                    return NotFound();

                if (!await CheckNotExist(request.SerialNumber))
                    return Conflict("Serial number exists");

                var newEquipment = mapper.Map<Equipment>(request);
                newEquipment.Number = type.LastNumber++;
                if (request.Children?.Count > 0)
                    newEquipment.Children =
                        await dbContext
                        .Equipments
                        .Where(eq => request.Children.Contains(eq.Id))
                        .ToListAsync();

                if (newEquipment.Children?.Count != request.Children?.Count)
                    return BadRequest("Incorrect equipment ids");

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
        public async Task<ActionResult<EquipmentView>> PutAsync(int id, [FromBody]EquipmentEditRequest request)
        {
            var toEdit = await CheckAndGetEquipmentAsync(request.Id);
            if (request.EquipmentTypeId.HasValue)
            {
                if (!await dbContext.EquipmentTypes.AnyAsync(et => et.Id == request.EquipmentTypeId.Value))
                    return NotFound();
            }
            if (string.IsNullOrEmpty(request.SerialNumber))
                if (!await CheckNotExist(request.SerialNumber))
                    return Conflict("Serial number exists");//TODO meta

            mapper.Map(request, toEdit);
            await dbContext.SaveChangesAsync();

            return mapper.Map<EquipmentView>(toEdit);
        }

        [RequireRole(RoleNames.CanEditEquipment)]
        [HttpDelete]
        public async Task<ActionResult<Guid>> DeleteAsync([FromBody]IdRequest request)
        {
            var toDelete = await CheckAndGetEquipmentAsync(request.Id);
            if (toDelete == null)
                return NotFound();
            dbContext.Equipments.Remove(toDelete);
            await dbContext.SaveChangesAsync();
            return request.Id;
        }

        private Task<Equipment> CheckAndGetEquipmentAsync(Guid id)
        {
            return dbContext.Equipments.Include(eq => eq.EquipmentType).FirstOrDefaultAsync(eq => eq.Id == id);
        }

        private Task<bool> CheckNotExist(string serialNumber)
        {
            return dbContext
                .Equipments
                .AnyAsync(eq => eq.SerialNumber == serialNumber);
        }

    }
}
