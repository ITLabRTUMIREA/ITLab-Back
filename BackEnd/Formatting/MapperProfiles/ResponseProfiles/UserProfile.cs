using System;
using AutoMapper;
using Models.People;
using Models.People.Roles;
using Models.People.UserProperties;
using Models.PublicAPI.Responses.People;
using Models.PublicAPI.Responses.People.Properties;

namespace BackEnd.Formatting.MapperProfiles.ResponseProfiles
{
	public class UserProfile : Profile
    {
        public UserProfile()
        {
            PropertiesMaps();
            RolesMaps();
        }

        private void PropertiesMaps()
        {
            CreateMap<UserPropertyType, UserPropertyTypeView>();
            CreateMap<UserProperty, UserPropertyView>();
            CreateMap<User, UserView>()
                .ForMember(uv => uv.Properties, map => map.MapFrom(u => u.UserProperties));
        }

        private void RolesMaps()
        {
            CreateMap<Role, RoleView>();
        }

    }
}
