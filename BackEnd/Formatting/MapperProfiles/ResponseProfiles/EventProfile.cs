using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Models.Events.Roles;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;
using Models.PublicAPI.Responses.Event;

namespace BackEnd.Formatting.MapperProfiles.ResponseProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventRole, EventRoleView>();
        }
    }
}
