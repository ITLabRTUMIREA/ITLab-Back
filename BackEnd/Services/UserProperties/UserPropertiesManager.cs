using System;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Services.Interfaces;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using BackEnd.DataBase;
using Microsoft.EntityFrameworkCore;
using Models.PublicAPI.Responses;
using BackEnd.Extensions;
using AutoMapper;
using BackEnd.Exceptions;

namespace BackEnd.Services.UserProperties
{
    public class UserPropertiesManager : IUserPropertiesManager
    {
        private readonly DataBaseContext dbContext;
        private readonly IMapper mapper;
        private readonly IUserPropertiesConstants constants;

        public UserPropertiesManager(
            DataBaseContext dbContext,
            IMapper mapper,
            IUserPropertiesConstants constants
        )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.constants = constants;
        }

        public async Task<IQueryable<UserProperty>> PutUserProperty(UserPropertyEditRequest request, Guid userId)
        {
            var targetType = await dbContext
                .UserPropertyTypes
                .SingleOrDefaultAsync(upt => upt.Id == request.Id)
                ?? throw ApiLogicException.NotFound();
            var currentProperty = await dbContext
                .UserProperties
                .Where(up => up.UserId == userId)
                .Where(up => up.UserPropertyTypeId == targetType.Id)
                .SingleOrDefaultAsync();
            if (currentProperty == null)
            {
                currentProperty = mapper.Map<UserProperty>(request);
                dbContext.UserProperties.Add(currentProperty);
            }
            else
                mapper.Map(request, currentProperty);

            currentProperty.Status = targetType.DefaultStatus;
            currentProperty.UserId = userId;
            await dbContext.SaveChangesAsync();
            return dbContext
                .UserProperties
                .Where(up => up.Id == currentProperty.Id);
        }
        public async Task<Guid> DeleteUserProperty(Guid typeId, Guid userId)
        {
            var targetProperty = await dbContext
                .UserProperties
                .Where(up => up.UserPropertyTypeId == typeId && up.UserId == userId)
                .SingleOrDefaultAsync()
                ?? throw ApiLogicException.NotFound();
            dbContext.Remove(targetProperty);
            await dbContext.SaveChangesAsync();
            return typeId;
        }
    }
}
