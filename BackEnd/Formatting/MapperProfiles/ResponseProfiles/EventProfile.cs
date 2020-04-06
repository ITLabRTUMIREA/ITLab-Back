using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackEnd.Models;
using Models.DataBaseLinks;
using Models.Events;
using Models.Events.Roles;
using Models.People;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Create.Roles;
using Models.PublicAPI.Requests.Events.Event.Edit.Roles;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.Event.CreateEdit;

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
                .ForMember(uev => uev.BeginTime, map => map.MapFrom(puer => puer.Place.Shift.BeginTime))
                .ForMember(uev => uev.Role, map => map.MapFrom(puer => puer.EventRole));

            CreateMap<Event, EventView>();
            CreateMap<Event, CreateEditEventView>();

            Guid userId = default;
            CreateMap<Event, EventAndUserId>()
                .ForMember(evuid => evuid.UserId, map => map.MapFrom(ev => userId));

            CreateMap<EventAndUserId, CompactEventView>()
                .ForMember(cev => cev.ShiftsCount, map => map.MapFrom(ev => ev.Shifts.Count))
                .ForMember(cev => cev.EndTime, map => map.MapFrom(ev =>
                    ev
                        .Shifts
                        .Select(s => s.EndTime)
                        .Max()))
                .ForMember(cev => cev.CurrentParticipantsCount, map => map.MapFrom(ev =>
                    ev.Shifts
                        .SelectMany(s => s.Places)
                        .SelectMany(p => p.PlaceUserEventRoles)
                        .Count(pur => pur.UserStatus == UserStatus.Accepted)))
                .ForMember(cev => cev.TargetParticipantsCount, map => map.MapFrom(ev =>
                    ev.Shifts
                        .SelectMany(s => s.Places)
                        .Sum(p => p.TargetParticipantsCount)))
                .ForMember(cev => cev.Participating, map => map.MapFrom(evuid =>
                    evuid
                        .Shifts
                        .SelectMany(s => s.Places)
                        .SelectMany(p => p.PlaceUserEventRoles)
                        .Where(pur => pur.UserStatus == UserStatus.Accepted)
                        .Any(pur => pur.UserId == evuid.UserId)));
            ShiftMaps();
            PlaceMaps();
        }

        private void ShiftMaps()
        {
            CreateMap<Shift, ShiftView>();
            CreateMap<Shift, CreateEditShiftView>();
        }

        private void PlaceMaps()
        {
            CreateMap<Place, PlaceView>()
               .ForMember(p => p.Equipment, map => map.MapFrom(p =>
                   p.PlaceEquipments.Select(pe => pe.Equipment)
               ))
               .ForMember(p => p.Participants, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                                  .Where(pur => pur.UserStatus == UserStatus.Accepted)))
               .ForMember(p => p.Invited, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                              .Where(pur => pur.UserStatus == UserStatus.Invited)))
               .ForMember(p => p.Wishers, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                             .Where(pur => pur.UserStatus == UserStatus.Wisher)))
               .ForMember(p => p.Unknowns, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                              .Where(pur => pur.UserStatus == UserStatus.Unknown)));

            CreateMap<Place, CreateEditPlaceView>()
               .ForMember(p => p.Equipment, map => map.MapFrom(p =>
                   p.PlaceEquipments.Select(pe => pe.Equipment)
               ))
               .ForMember(p => p.Participants, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                                  .Where(pur => pur.UserStatus == UserStatus.Accepted)))
               .ForMember(p => p.Invited, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                              .Where(pur => pur.UserStatus == UserStatus.Invited)))
               .ForMember(p => p.Wishers, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                             .Where(pur => pur.UserStatus == UserStatus.Wisher)))
               .ForMember(p => p.Unknowns, map => map.MapFrom(p => p.PlaceUserEventRoles
                                                              .Where(pur => pur.UserStatus == UserStatus.Unknown)));
        }
    }
}
