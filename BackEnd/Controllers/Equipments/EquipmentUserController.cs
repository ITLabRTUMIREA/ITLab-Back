using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.People;
using BackEnd.DataBase;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Responses.Equipment;
using AutoMapper.QueryableExtensions;
using Models.PublicAPI.Requests;
using AutoMapper;
using BackEnd.Models.Roles;
using Models.People.Roles;
using Models.Equipments;

namespace BackEnd.Controllers.Equipments
{
    [Route("api/equipment/user")]
    public class EquipmentUserController : AuthorizeController
    {
        private readonly DataBaseContext dataBaseContext;
        private readonly IMapper mapper;

        public EquipmentUserController(
            UserManager<User> userManager,
            DataBaseContext dataBaseContext,
            IMapper mapper)
            : base(userManager)
        {
            this.dataBaseContext = dataBaseContext;
            this.mapper = mapper;
        }

        [HttpGet("free")]
        public async Task<ActionResult<List<EquipmentView>>> Get()
        {
            return await dataBaseContext
                .Equipments
                .Where(e => e.OwnerId == null)
                .ProjectTo<EquipmentView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [HttpGet("{userId?}")]
        public async Task<ActionResult<List<EquipmentView>>> Get(Guid? userId)
        {
            var uId = await GetUserId(userId);
            if (uId == null)
                return NotFound();
            return await dataBaseContext
                .Equipments
                .Where(e => e.OwnerId == uId)
                .ProjectTo<EquipmentView>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        [RequireRole(RoleNames.CanEditEquipmentOwner)]
        [HttpPost("{userId?}")]
        public async Task<ActionResult<EquipmentView>> Post(
            [FromRoute]Guid? userId,
            [FromBody] IdRequest equipmentIdRequest)
        {
            var uId = await GetUserId(userId);
            if (uId == null)
                return NotFound();
            var targetEquipment = await dataBaseContext
                .Equipments
                .SingleOrDefaultAsync(e => e.Id == equipmentIdRequest.Id);
            if (targetEquipment == null)
                return NotFound();

            if (targetEquipment.OwnerId.HasValue && targetEquipment.OwnerId != uId)
                return Conflict("Equipment reserved");


            targetEquipment.OwnerId = uId;
            dataBaseContext.EquipmentOwnerChanges.Add(new EquipmentOwnerChangeRecord
            {
                ChangeOwnerTime = DateTime.UtcNow,
                Equipment = targetEquipment,
                NewOwnerId = targetEquipment.OwnerId,
                GranterId = UserId
            });
            await dataBaseContext.SaveChangesAsync();
            return mapper.Map<EquipmentView>(targetEquipment);
        }
        [RequireRole(RoleNames.CanEditEquipmentOwner)]
        [HttpDelete("{userId?}")]
        public async Task<ActionResult<EquipmentView>> Delete(
            [FromRoute]Guid? userId,
            [FromBody] IdRequest equipmentIdRequest)
        {
            var uId = await GetUserId(userId);
            if (uId == null)
                return NotFound();
            var targetEquipment = await dataBaseContext
                .Equipments
                .Where(e => e.Id == equipmentIdRequest.Id)
                .Where(e => e.OwnerId == uId)
                .SingleOrDefaultAsync();
            if (targetEquipment == null)
                return NotFound();


            targetEquipment.OwnerId = null;
            dataBaseContext.EquipmentOwnerChanges.Add(new EquipmentOwnerChangeRecord
            {
                ChangeOwnerTime = DateTime.UtcNow,
                Equipment = targetEquipment,
                NewOwnerId = targetEquipment.OwnerId,
                GranterId = UserId
            });
            await dataBaseContext.SaveChangesAsync();
            return mapper.Map<EquipmentView>(targetEquipment);
        }
        private async Task<Guid?> GetUserId(Guid? targetId)
        {
            return !targetId.HasValue ? UserId :
                              await dataBaseContext.Users
                              .AnyAsync(u => u.Id == targetId) ? targetId.Value :
                               null as Guid?;
        }
    }
}
