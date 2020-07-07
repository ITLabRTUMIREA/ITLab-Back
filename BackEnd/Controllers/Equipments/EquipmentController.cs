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
using NPOI.SS.Formula.Functions;
using BackEnd.Services.EquipmentServices;
using Exceptions.EquipmentExceptions;
using Exceptions.EquipmentTypeExceptions;

namespace BackEnd.Controllers.Equipments
{

    [Route("api/Equipment")]
    public class EquipmentController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly EquipmentService equipmentService;
        private readonly ILogger<EquipmentTypeController> logger;
        private readonly IMapper mapper;

        public EquipmentController(
            DataBaseContext dbContext,
            EquipmentService equipmentService,
            ILogger<EquipmentTypeController> logger,
            IMapper mapper,
            UserManager<User> userManager) : base(userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.equipmentService = equipmentService;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentView>> GetAsync(Guid id)
        {
            var equipmentView = await dbContext
                                      .Equipments
                                      .ProjectTo<EquipmentView>(mapper.ConfigurationProvider)
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
                .ProjectTo<CompactEquipmentView>(mapper.ConfigurationProvider)
                .ToListAsync();

        /// <summary>
        /// Create new Equipment
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A newly created Equipment</returns>
        /// <response code="201">Returns newly created equipment</response>
        /// <response code="400">If incorrect children id passed</response>
        /// <response code="404">If equipment type id don't exist</response>
        /// <response code="409">If serial number exists</response>
        [RequireRole(RoleNames.CanEditEquipment)]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<EquipmentView>> PostAsync([FromBody]EquipmentCreateRequest request)
        {
            try
            {
                var newEquipment = mapper.Map<Equipment>(request);

                newEquipment = await equipmentService.AddEquipment(newEquipment, request.Children, UserId);

                return Ok(mapper.Map<EquipmentView>(newEquipment));
            }
            catch (EquipmentUpdateException sex)
            {
                return Conflict(sex.Message);
            }
            catch (EquipmentTypeFindException sex)
            {
                return NotFound(sex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error while adding equipment");
                throw;
            }
        }
        /// <summary>
        /// Edit equipment
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Edited equipment</returns>
        /// <response code="200">Returns edited equipment</response>
        /// <response code="404">If equipment with passed id don't exists</response>
        /// <response code="409">Id passed serial number exists</response>
        [RequireRole(RoleNames.CanEditEquipment)]
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<EquipmentView>> PutAsync([FromBody]EquipmentEditRequest request)
        {
            var toEdit = await CheckAndGetEquipmentAsync(request.Id);
            if (request.EquipmentTypeId.HasValue)
            {
                if (!await dbContext.EquipmentTypes.AnyAsync(et => et.Id == request.EquipmentTypeId.Value))
                    return NotFound();
            }
            if (string.IsNullOrEmpty(request.SerialNumber))
                if (!await CheckExist(request.SerialNumber))
                    return Conflict("Serial number exists");

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

        private Task<bool> CheckExist(string serialNumber)
        {
            return dbContext
                .Equipments
                .AnyAsync(eq => eq.SerialNumber == serialNumber);
        }

    }
}
