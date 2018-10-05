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
using Microsoft.WindowsAzure.Storage.Table;

namespace BackEnd.Controllers.Equipments
{
    [Produces("application/json")]
    [Route("api/EquipmentType")]
    public class EquipmentTypeController : Controller
    {
        private readonly DataBaseContext dbContext;

        private readonly ILogger<EquipmentTypeController> logger;
        private readonly IMapper mapper;

        public EquipmentTypeController(
            DataBaseContext dbContext,
            ILogger<EquipmentTypeController> logger,
            IMapper mapper)
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
                .If(!all, eqtypes => eqtypes.Take(5))
                .ProjectTo<CompactEquipmentTypeView>()
                .ToListAsync();


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EquipmentTypeView>> GetAsync(Guid id)
            => await dbContext
                   .EquipmentTypes
                   .Where(eqt => eqt.Id == id)
                    .ProjectTo<EquipmentTypeView>()
                    .SingleOrDefaultAsync()
                    ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);

        [HttpPost]
        public async Task<OneObjectResponse<EquipmentTypeView>> Post([FromBody]EquipmentTypeCreateRequest request)
        {
            var equipmentType = mapper.Map<EquipmentType>(request);
            if(await dbContext.EquipmentTypes.AnyAsync(et => et.Title == request.Title))
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
            var added = await dbContext.EquipmentTypes.AddAsync(equipmentType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentTypeView>(added.Entity);
        }


        [HttpPut]
        public async Task<ListResponse<EquipmentTypeView>> Put([FromBody]List<EquipmentTypeEditRequest> request)
        {
            var ids = request
                .SelectMany(eter => (eter.Id, eter.ParentId))
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Distinct()
                .ToArray();
            var forEdit = await dbContext
                .EquipmentTypes
                .Where(et => ids.Contains(et.Id))
                .ToListAsync();
            if (ids.Length != forEdit.Count)
                throw ResponseStatusCode.IncorrectRequestData.ToApiException($"incorrect ids, wait {ids.Length}, found {forEdit.Count}");

            mapper.Map(request, forEdit);
            var result = await dbContext.SaveChangesAsync();
            return await dbContext
                .EquipmentTypes
                .Where(et => ids.Contains(et.Id))
                .ProjectTo<EquipmentTypeView>()
                .ToListAsync();
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await dbContext.EquipmentTypes.FindAsync(request.Id) ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            dbContext.Remove(now);
            await dbContext.SaveChangesAsync();
            return now.Id;
        }
    }
}
