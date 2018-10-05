using System;
using System.Linq;
using System.Threading.Tasks;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using Models.PublicAPI.Requests;
namespace BackEnd.Services.UserProperties
{
    public interface IUserPropertiesManager
    {
        Task<IQueryable<UserProperty>> PutUserProperty(UserPropertyEditRequest request, Guid userId);
        Task<Guid> DeleteUserProperty(Guid typeId, Guid userId);
    }
}
