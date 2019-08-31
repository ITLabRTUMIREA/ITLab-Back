using System;
using System.Linq;
using System.Threading.Tasks;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using Models.PublicAPI.Requests;
using BackEnd.Exceptions;

namespace BackEnd.Services.UserProperties
{
    public interface IUserPropertiesManager
    {
        /// <summary>
        /// Add or update user property
        /// </summary>
        /// <param name="request">Request for change property</param>
        /// <param name="userId">Id of target user</param>
        /// <returns>IQueryable with one new user property</returns>
        /// <exception cref="ApiLogicException">When property type id not found</exception>
        Task<IQueryable<UserProperty>> PutUserProperty(UserPropertyEditRequest request, Guid userId);
        /// <summary>
        /// Delete user property by type id
        /// </summary>
        /// <param name="typeId">Id of property type for delete</param>
        /// <param name="userId">Id of target user</param>
        /// <returns>Deleted type id</returns>
        Task<Guid> DeleteUserProperty(Guid typeId, Guid userId);
    }
}
