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
                .WhereIf(string.IsNullOrEmpty(match), t => t.ParentId == null) 
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
            var now = dbContext.EquipmentTypes.FirstOrDefault(et => et.Title == request.Title);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
            var added = await dbContext.EquipmentTypes.AddAsync(equipmentType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentTypeView>(added.Entity);
        }


        [HttpPut]
        public async Task<OneObjectResponse<EquipmentTypeView>> Put([FromBody]EquipmentTypeEditRequest request)
        {
            var now = await dbContext.EquipmentTypes.FindAsync(request.Id) ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            mapper.Map(request, now);
            await dbContext.SaveChangesAsync();
            return mapper.Map<EquipmentTypeView>(now);
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
