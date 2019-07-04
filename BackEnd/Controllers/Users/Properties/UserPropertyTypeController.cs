using Microsoft.AspNetCore.Identity;
using Models.People;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Models.PublicAPI.Responses.People.Properties;
using BackEnd.DataBase;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Requests.User.Properties.UserPropertyType;
using System.Threading.Tasks;
using AutoMapper;
using Models.People.UserProperties;
using System.Collections.Generic;

namespace BackEnd.Controllers.Users.Properties
{
    [Route("api/account/property/type")]
    public class UserPropertyTypeController : AuthorizeController
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;

        public UserPropertyTypeController(
            UserManager<User> userManager,
            DataBaseContext dbContext,
            IMapper mapper) : base(userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserPropertyTypeView>>> GetAsync()
            => await dbContext
                .UserPropertyTypes
                .ProjectTo<UserPropertyTypeView>()
                .ToListAsync();

        //TODO Lock (semaphore) on method
        [HttpPost]
        public async Task<ActionResult<UserPropertyTypeView>> PostAsync(
            [FromBody]UserPropertyTypeCreateRequest request)
        {
            if (await dbContext.UserPropertyTypes.AnyAsync(upt => upt.Name == request.Name))
                return Conflict("Field exists");
            var newType = mapper.Map<UserPropertyType>(request);
            await dbContext.AddAsync(newType);
            await dbContext.SaveChangesAsync();
            return mapper.Map<UserPropertyTypeView>(newType);
        }
    }
}
