using AutoMapper;
using Models.Equipments;
using Models.Events;
using Models.People;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.Login;
using Models.PublicAPI.Responses.People;
using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Models.DataBaseLinks;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;
using BackEnd.Models;
using Models.Events.Roles;
using Models.People.Roles;
using Models.PublicAPI.Responses.Event.Invitations;

namespace BackEnd.Formatting
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            EventEditMaps();
            Invitations();
            CreateMap<EquipmentTypeCreateRequest, EquipmentType>();
            CreateMap<EventTypeEditRequest, EventType>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

            CreateMap<EquipmentCreateRequest, Equipment>()
                .ForMember(eq => eq.Children, map => map.Ignore());


            CreateMap<Equipment, EquipmentView>();

            CreateMap<EquipmentType, EquipmentTypeView>();
            CreateMap<EquipmentType, CompactEquipmentTypeView>();

            CreateMap<AccountCreateRequest, User>()
                .ForMember(u => u.UserName, map => map.MapFrom(ac => ac.Email));
            CreateMap<User, LoginResponse>();
            CreateMap<RefreshToken, RefreshTokenView>();

            CreateMap<EquipmentEditRequest, Equipment>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));


            CreateMap<EventTypeCreateRequest, EventType>();
            CreateMap<EventType, EventTypeView>();
            CreateMap<EventCreateRequest, Event>();
            CreateMap<Event, EventView>();
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


            CreateMap<ShiftCreateRequest, Shift>();
            CreateMap<Shift, ShiftView>();


            CreateMap<Guid, PlaceUserEventRole>()
                .ForMember(pur => pur.UserId, map => map.MapFrom(pwr => pwr));
            CreateMap<Guid, PlaceEquipment>()
                .ForMember(pe => pe.EquipmentId, map => map.MapFrom(g => g));
            CreateMap<PlaceCreateRequest, Place>()
                .ForMember(p => p.PlaceEquipments, map => map.MapFrom(s => s.Equipment))
                .ForMember(p => p.PlaceUserEventRoles, map => map.MapFrom(pcr => pcr.Invited));

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

            CreateMap<PlaceUserEventRole, UserAndEventRole>();

            CreateMap<AccountEditRequest, User>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));
            CreateMap<UserSetting, UserSettingPresent>()
                .ForMember(usp => usp.Value, map => map.MapFrom(us => us.Value.ParseToJson()));
        }

        private void EventEditMaps()
        {
            CreateMap<List<ShiftEditRequest>, List<Shift>>()
                .ConvertUsing(new ListsConverter<ShiftEditRequest, Shift>(s => s.Id));
            CreateMap<EventEditRequest, Event>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

            CreateMap<List<PlaceEditRequest>, List<Place>>()
                .ConvertUsing(new ListsConverter<PlaceEditRequest, Place>(p => p.Id));
            CreateMap<ShiftEditRequest, Shift>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

            CreateMap<List<DeletableRequest>, List<PlaceEquipment>>()
                .ConvertUsing(new ListsConverter<DeletableRequest, PlaceEquipment>(eq => eq.EquipmentId, (dl, pe) => dl.Id != pe?.EquipmentId));
            CreateMap<List<PersonWorkRequest>, List<PlaceUserEventRole>>()
                .ConvertUsing(new ListsConverter<PersonWorkRequest, PlaceUserEventRole>(pur => pur.UserId, (pwr, pur) => pwr.EventRoleId != pur?.EventRoleId || pwr.Id == pur?.UserId));

            CreateMap<PlaceEditRequest, Place>()
                .ForMember(p => p.PlaceEquipments, map => map.MapFrom(per => per.Equipment))
                .ForMember(p => p.PlaceUserEventRoles, map => map.MapFrom(per => per.Invited))
                .ForAllMembers(opt => opt.Condition((source, destination, sourceMember, destMember) =>
                    sourceMember != null));
            CreateMap<DeletableRequest, PlaceEquipment>()
                .ForMember(pe => pe.EquipmentId, map => map.MapFrom(eer => eer.Id));
            CreateMap<PersonWorkRequest, PlaceUserEventRole>()
                .ForMember(pur => pur.UserId, map => map.MapFrom(pwr => pwr.Id));
        }

        private void Invitations()
        {
            CreateMap<PlaceUserEventRole, EventApplicationView>()
                .ForMember(eav => eav.Id, map => map.MapFrom(pur => pur.Place.Shift.EventId))
                .ForMember(eav => eav.Title, map => map.MapFrom(pur => pur.Place.Shift.Event.Title))
                .ForMember(eav => eav.EventType, map => map.MapFrom(pur => pur.Place.Shift.Event.EventType))
                .ForMember(eav => eav.BeginTime, map => map.MapFrom(pur => pur.Place.Shift.BeginTime))
                .ForMember(eav => eav.ShiftDurationInMinutes,
                    map => map.MapFrom(pur => pur.Place.Shift.EndTime.Subtract(pur.Place.Shift.BeginTime).TotalMinutes))
                .ForMember(eav => eav.PlaceDescription, map => map.MapFrom(pur => pur.Place.Description))
                .ForMember(eav => eav.ShiftDescription, map => map.MapFrom(pur => pur.Place.Shift.Description));


            CreateMap<PlaceUserEventRole, WisherEventView>()
                .ForMember(wev => wev.Id, map => map.MapFrom(pur => pur.Place.Shift.EventId))
                .ForMember(wev => wev.Title, map => map.MapFrom(pur => pur.Place.Shift.Event.Title))
                .ForMember(wev => wev.EventType, map => map.MapFrom(pur => pur.Place.Shift.Event.EventType))
                .ForMember(wev => wev.BeginTime, map => map.MapFrom(pur => pur.Place.Shift.BeginTime))
                .ForMember(wev => wev.TargetParticipantsCount, map => map.MapFrom(pur => pur.Place.TargetParticipantsCount))
                .ForMember(wev => wev.PlaceDescription, map => map.MapFrom(pur => pur.Place.Description))
                .ForMember(wev => wev.ShiftDescription, map => map.MapFrom(pur => pur.Place.Shift.Description))
                .ForMember(wev => wev.CurrentParticipantsCount, map => map.MapFrom(pur => pur.Place.PlaceUserEventRoles.Count(pur1 => pur1.UserStatus == UserStatus.Accepted)));
        }
    }
}