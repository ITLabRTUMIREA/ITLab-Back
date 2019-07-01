using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackEnd.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Extensions;
using Models.PublicAPI.Responses.Equipment;
using System.Collections.Generic;
using BackEnd.Models.Roles;
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
        public async Task<ActionResult<List<CompactEquipmentTypeView>>> GetAsync(string match, bool all = false)
            => await dbContext
                .EquipmentTypes
                .WhereIf(!all, t => t.ParentId == null)
                .WhereIf(!string.IsNullOrEmpty(match), eq => eq.Title.ToUpper().Contains(match.ToUpper()))
                .If(!all, types => types.Take(5))
                .ProjectTo<CompactEquipmentTypeView>()
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentTypeView>> GetAsync(Guid id)
        {
            var one = await dbContext
                      .EquipmentTypes
                      .Where(eqt => eqt.Id == id)
                      .SingleOrDefaultAsync();
            if (one == null)
                return NotFound();

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
        public async Task<ActionResult<EquipmentTypeView>> Post([FromBody]EquipmentTypeCreateRequest request)
        {
            var equipmentType = mapper.Map<EquipmentType>(request);
            if (request.ParentId.HasValue)
            {
                var parent = await dbContext
                    .EquipmentTypes
                    .SingleOrDefaultAsync(et => et.Id == request.ParentId);
                if (parent == null)
                    return NotFound();
                equipmentType.Deep = parent.Deep + 1;
                equipmentType.RootId = parent.RootId ?? parent.Id;
            }
            var added = await dbContext.EquipmentTypes.AddAsync(equipmentType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentTypeView>(added.Entity);
        }

        [RequireRole(RoleNames.CanEditEquipmentType)]
        [HttpPut]
        public async Task<ActionResult<List<EquipmentTypeView>>> Put([FromBody]List<EquipmentTypeEditRequest> request)
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
                .Where(et => ids.Contains(et.Id))
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
        public async Task<ActionResult<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await dbContext
                          .EquipmentTypes
                          .Where(et => et.Id == request.Id)
                          .Select(et => new { equipmentType = et, childsCount = et.Children.Count, equipmentCount = et.Equipment.Count })
                          .SingleOrDefaultAsync();
            if (now == null)
                return NotFound();

            if (now.childsCount != 0 || now.equipmentCount != 0)
                return Conflict("Children exists");//TODO meta

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
