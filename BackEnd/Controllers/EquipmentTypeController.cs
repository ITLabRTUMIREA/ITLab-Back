using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.DataBase;
using BackEnd.Exceptions;
using BackEnd.Formatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Equipments;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Equipment;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Responses;
using Models.PublicAPI.Responses.General;

namespace BackEnd.Controllers
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

        public ListResponse<EquipmentType> Get()
            =>
            ListResponse<EquipmentType>.Create(dbContext.EquipmentTypes);


        [HttpGet("{id}")]
        public async Task<OneObjectResponse<EquipmentType>> GetAsync(Guid id)
            =>
            OneObjectResponse<EquipmentType>.Create(
                await dbContext.EquipmentTypes.FindAsync(id)
                ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound));

        [HttpPost]
        public async Task<OneObjectResponse<EquipmentType>> Post([FromBody]EquipmentTypeCreateRequest request)
        {
            var equipmentType = mapper.Map<EquipmentType>(request);
            var now = dbContext.EquipmentTypes.FirstOrDefault(et => et.Title == request.Title);
            if (now != null)
                throw ApiLogicException.Create(ResponseStatusCode.FieldExist);
            var added = await dbContext.EquipmentTypes.AddAsync(equipmentType);
            await dbContext.SaveChangesAsync();
            return OneObjectResponse<EquipmentType>.Create(added.Entity);
        }


        [HttpPut]
        public async Task<OneObjectResponse<EquipmentType>> Put([FromBody]EquipmentTypeEditRequest request)
        {
            var now = await dbContext.EquipmentTypes.FindAsync(request.Id) ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            now.Title = request.Title;
            await dbContext.SaveChangesAsync();
            return OneObjectResponse<EquipmentType>.Create(now);
        }

        [HttpDelete]
        public async Task<OneObjectResponse<Guid>> Delete([FromBody]IdRequest request)
        {
            var now = await dbContext.EquipmentTypes.FindAsync(request.Id) ?? throw ApiLogicException.Create(ResponseStatusCode.NotFound);
            dbContext.Remove(now);
            await dbContext.SaveChangesAsync();
            return OneObjectResponse<Guid>.Create(now.Id);
        }
    }
}
