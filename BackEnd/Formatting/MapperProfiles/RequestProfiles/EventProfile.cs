using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Models.Events.Roles;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;

namespace BackEnd.Formatting.MapperProfiles.RequestProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventRoleCreateRequest, EventRole>();
            CreateMap<EventRoleEditRequest, EventRole>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

        }
    }
}
