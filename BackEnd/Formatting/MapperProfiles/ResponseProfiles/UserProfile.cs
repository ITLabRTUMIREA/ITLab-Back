using System;
using AutoMapper;
using Models.People.UserProperties;
using Models.PublicAPI.Responses.People.Properties;

namespace BackEnd.Formatting.MapperProfiles.ResponseProfiles
{
	public class UserProfile : Profile
    {
        public UserProfile()
        {
            PropertiesMaps();
        }

        private void PropertiesMaps()
        {
            CreateMap<UserPropertyType, UserPropertyTypeView>();
            CreateMap<UserProperty, UserPropertyView>();
        }
    }
}
