using System;
using AutoMapper;
using Models.PublicAPI.Requests.User.Properties.UserProperty;
using Models.People.UserProperties;
using Models.PublicAPI.Requests.User.Properties.UserPropertyType;
using Models.PublicAPI.Responses.People.Properties;

namespace BackEnd.Formatting.MapperProfiles.RequestProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            PropertiesMaps();
        }

        private void PropertiesMaps()
        {
            CreateMap<UserPropertyEditRequest, UserProperty>()
                .ForMember(up => up.UserPropertyTypeId, map => map.MapFrom(uper => uper.Id))
                .ForMember(up => up.Id, map => map.Ignore());

            CreateMap<UserPropertyTypeCreateRequest, UserPropertyType>()
                .ForMember(up => up.PublicName, map => map.MapFrom(upcr => upcr.Title))
                .ForMember(up => up.InternalName, map => map.MapFrom(upcr => upcr.Title));

            CreateMap<UserPropertyTypeEditRequest, UserPropertyType>()
                .ForMember(up => up.PublicName, map => map.MapFrom(uper => uper.Title))
                .ForMember(up => up.InternalName, map => map.MapFrom(upcr => upcr.Title));
        }
    }
}
