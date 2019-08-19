using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Models.DataBaseLinks;
using Models.Events;
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

            CreateMap<PlaceUserEventRole, UsersEventsView>()
                .ForMember(uev => uev.Id, map => map.MapFrom(puer => puer.Place.Shift.Event.Id))
                .ForMember(uev => uev.Address, map => map.MapFrom(puer => puer.Place.Shift.Event.Address))
                .ForMember(uev => uev.Title, map => map.MapFrom(puer => puer.Place.Shift.Event.Title))
                .ForMember(uev => uev.EventType, map => map.MapFrom(puer => puer.Place.Shift.Event.EventType))
                .ForMember(uev => uev.BeginTime, map => map.MapFrom(puer => puer.Place.Shift.Event.BeginTime))
                .ForMember(uev => uev.Role, map => map.MapFrom(puer => puer.EventRole));
        }
    }
}
