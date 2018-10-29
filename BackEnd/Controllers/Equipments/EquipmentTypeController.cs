using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Formatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;
using Extensions;
using Models.PublicAPI.Responses.Equipment;
using System.Collections.Generic;
using BackEnd.Extensions;
using BackEnd.Models.Roles;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.AspNetCore.Identity;
using Models.People;
using Models.People.Roles;

namespace BackEnd.Controllers.Equipments
{
    [Produces("application/json")]
    [Route("api/EquipmentType")]
    public class EquipmentTypeController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EquipmentTypeController> logger;
        private readonly IMapper mapper;

        public EquipmentTypeController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            ILogger<EquipmentTypeController> logger,
            IMapper mapper)
            : base(userManager)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<ListResponse<CompactEquipmentTypeView>> GetAsync(string match, bool all = false)
            => await dbContext
                .EquipmentTypes
                .WhereIf(!all, t => t.ParentId == null)
                .WhereIf(!string.IsNullOrEmpty(match), eq => eq.Title.ToUpper().Contains(match.ToUpper()))
                .If(!all, types => types.Take(5))
                .ProjectTo<CompactEquipmentTypeView>()
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EquipmentTypeView>> GetAsync(Guid id)
        {
            var one = await dbContext
                      .EquipmentTypes
                      .Where(eqt => eqt.Id == id)
                      .SingleOrDefaultAsync()
                      ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            var targetRootId = one.RootId ?? one.Id;
            var all = await dbContext
                      .EquipmentTypes
                      .Where(et => et.RootId == targetRootId && et.Deep >= one.Deep)
                      .ToListAsync();
            all.Add(one);
            BuildTree(all);
            return mapper.Map<EquipmentTypeView>(all.Single(et => et.Id == id));
        }

        [RequireRole(RoleNames.CanEditEquipmentType)]
        [HttpPost]
        public async Task<OneObjectResponse<EquipmentTypeView>> Post([FromBody]EquipmentTypeCreateRequest request)
        {
            var equipmentType = mapper.Map<EquipmentType>(request);
            if (request.ParentId.HasValue)
            {
                var parent = await dbContext
                    .EquipmentTypes
                    .SingleOrDefaultAsync(et => et.Id == request.ParentId)
                    ?? throw NotFoundMyApi();
                equipmentType.Deep = parent.Deep + 1;
                equipmentType.RootId = parent.RootId ?? parent.Id;
            }
            var added = await dbContext.EquipmentTypes.AddAsync(equipmentType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentTypeView>(added.Entity);
        }

        [RequireRole(RoleNames.CanEditEquipmentType)]
        [HttpPut]
        public async Task<ListResponse<EquipmentTypeView>> Put([FromBody]List<EquipmentTypeEditRequest> request)
        {
            var ids = request
                .SelectMany(editRequest => (editRequest.Id, editRequest.ParentId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToList();
            var forEdits = await dbContext
                .EquipmentTypes
                .Where(et => ids.Contains(et.Id))
                .SelectMany(et => et.Id, et => et.RootId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToListAsync();
            ids.AddRange(forEdits);
            ids = ids.Distinct().ToList();
            var forEdit = await dbContext
                .EquipmentTypes
                .Where(et => ids.Contains(et.Id) || ids.Contains(et.RootId.Value))
                .ToListAsync();

            mapper.Map(request, forEdit);
            BuildTree(forEdit);
            var result = await dbContext.SaveChangesAsync();
            logger.LogDebug($"saved {result} items");
            return await dbContext
                .EquipmentTypes
                .Where(et => ids.Contains(et.Id))
                .ProjectTo<EquipmentTypeView>()
                .ToListAsync();
        }


        [RequireRole(RoleNames.CanEditEquipmentType)]
        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await dbContext
                          .EquipmentTypes
                          .Where(et => et.Id == request.Id)
                          .Select(et => new { equipmentType = et, childsCount = et.Children.Count, equipmentCount = et.Equipment.Count })
                          .SingleOrDefaultAsync() ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            if (now.childsCount == 0 || now.equipmentCount == 0)
                throw ResponseStatusCode.ChildrenExists.ToApiException();
            dbContext.EquipmentTypes.Remove(now.equipmentType);
            await dbContext.SaveChangesAsync();
            return now.equipmentType.Id;
        }

        private static void BuildTree(List<EquipmentType> types)
        {
            types.ForEach(t => t.Deep = 0);
            types.ForEach(t => t.Children = types
                          .Where(t1 => t1.ParentId == t.Id)
                          .ToList());
            types.ForEach(UpdateDeep);
        }

        private static void UpdateDeep(EquipmentType type)
            => type?.Children?.WithActions(UpdateDeep).WithActions(t => t.Deep++).Iterate();

    }
}
