using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Models.People;
using BackEnd.DataBase;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Responses;
using BackEnd.Extensions;
using Models.PublicAPI.Responses.General;
using Models.PublicAPI.Responses.Equipment;
using AutoMapper.QueryableExtensions;
using Models.PublicAPI.Requests;
using AutoMapper;
using BackEnd.Models.Roles;
using Models.People.Roles;

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
        public async Task<ListResponse<EquipmentView>> Get()
        {
            return await dataBaseContext
                .Equipments
                .Where(e => e.OwnerId == null)
                .ProjectTo<EquipmentView>()
                .ToListAsync();
        }

        [HttpGet("{userId?}")]
        public async Task<ListResponse<EquipmentView>> Get(Guid? userId)
        {
            var uId = await GetUserId(userId);
            return await dataBaseContext
                .Equipments
                .Where(e => e.OwnerId == uId)
                .ProjectTo<EquipmentView>()
                .ToListAsync();
        }

        [RequireRole(RoleNames.CanEditEquipmentOwner)]
        [HttpPost("{userId?}")]
        public async Task<OneObjectResponse<EquipmentView>> Post(
            [FromRoute]Guid? userId,
            [FromBody] IdRequest equipmentIdRequest)
        {
            var uId = await GetUserId(userId);
            var targetEquipment = await dataBaseContext
                .Equipments
                .SingleOrDefaultAsync(e => e.Id == equipmentIdRequest.Id)
                ?? throw ResponseStatusCode.NotFound.ToApiException();

            if (targetEquipment.OwnerId.HasValue && targetEquipment.OwnerId != uId)
                throw ResponseStatusCode.EquipmentReserved.ToApiException();

            targetEquipment.OwnerId = uId;
            await dataBaseContext.SaveChangesAsync();
            return mapper.Map<EquipmentView>(targetEquipment);
        }
        [RequireRole(RoleNames.CanEditEquipmentOwner)]
        [HttpDelete("{userId?}")]
        public async Task<OneObjectResponse<EquipmentView>> Delete(
            [FromRoute]Guid? userId,
            [FromBody] IdRequest equipmentIdRequest)
        {
            var uId = await GetUserId(userId);
            var targetEquipment = await dataBaseContext
                .Equipments
                .Where(e => e.Id == equipmentIdRequest.Id)
                .Where(e => e.OwnerId == uId)
                .SingleOrDefaultAsync()
                ?? throw ResponseStatusCode.NotFound.ToApiException();
            targetEquipment.OwnerId = null;
            await dataBaseContext.SaveChangesAsync();
            return mapper.Map<EquipmentView>(targetEquipment);
        }
        private async Task<Guid> GetUserId(Guid? targetId)
        {
            return !targetId.HasValue ? UserId :
                              await dataBaseContext.Users
                              .AnyAsync(u => u.Id == targetId) ? targetId.Value :
                              throw ResponseStatusCode.UserNotFound.ToApiException();
        }
    }
}
