using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly List<string> HardUserPropertyTitles = Enum.GetNames(typeof(UserPropertyNames)).ToList();

        private void PropertiesMaps()
        {
            CreateMap<UserPropertyType, UserPropertyTypeView>()
                .ForMember(uptv => uptv.InstancesCount, map => map.MapFrom(upt => upt.UserProperties.Count))
                .ForMember(uptv => uptv.Title, map => map.MapFrom(upt => upt.PublicName))
                .ForMember(uptv => uptv.IsLocked, map => map.MapFrom(upt => HardUserPropertyTitles.Contains(upt.InternalName)));
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
