using AutoMapper;
using Models;
using Models.Equipments;
using Models.Events;
using Models.People;
using Models.PublicAPI.Requests.Account;
using Models.PublicAPI.Requests.Equipment;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.Event;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Requests.Roles;
using Models.PublicAPI.Responses.Equipment;
using Models.PublicAPI.Responses.Event;
using Models.PublicAPI.Responses.Login;
using Models.PublicAPI.Responses.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Formatting;
using Extensions;
using Models.DataBaseLinks;
using Models.PublicAPI.Requests;
using Models.PublicAPI.Requests.Events.Event.Create;
using Models.PublicAPI.Requests.Events.Event.Edit;

namespace BackEnd.Formatting
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            EventEditMaps();
            CreateMap<EquipmentTypeCreateRequest, EquipmentType>();
            CreateMap<EventTypeEditRequest, EventType>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

            CreateMap<EquipmentCreateRequest, Equipment>();
            CreateMap<Equipment, EquipmentView>();

            CreateMap<EquipmentType, EquipmentTypeView>();
            CreateMap<AccountCreateRequest, User>()
                .ForMember(u => u.UserName, map => map.MapFrom(ac => ac.Email));
            CreateMap<User, LoginResponse>();

            CreateMap<EquipmentEditRequest, Equipment>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));


            CreateMap<EventTypeCreateRequest, EventType>();
            CreateMap<EventType, EventTypeView>();
            CreateMap<EventCreateRequest, Event>();
            CreateMap<Event, EventView>();

            CreateMap<Event, CompactEventView>()
                .ForMember(cev => cev.ShiftsCount, map => map.MapFrom(ev => ev.Shifts.Count))
                .ForMember(cev => cev.TotalDurationInMinutes, map => map.MapFrom(ev =>
                    ev
                        .Shifts
                        .Select(s => s.EndTime.Subtract(s.BeginTime).TotalMinutes)
                        .Sum()))
                .ForMember(cev => cev.Сompleteness, map => map.MapFrom(ev =>
                    100 *
                    ev.Shifts
                        .SelectMany(s => s.Places)
                        .SelectMany(p => p.PlaceUserRoles)
                        .Count(pur => pur.Role.NormalizedName == "PARTICIPANT")
                    /
                    ev.Shifts
                        .SelectMany(s => s.Places)
                        .Sum(p => p.TargetParticipantsCount)));


            CreateMap<ShiftCreateRequest, Shift>();
            CreateMap<Shift, ShiftView>();


            CreateMap<PersonWorkRequest, PlaceUserRole>()
                .ForMember(pur => pur.UserId, map => map.MapFrom(pwr => pwr.Id));
            CreateMap<Guid, PlaceEquipment>()
                .ForMember(pe => pe.EquipmentId, map => map.MapFrom(g => g));
            CreateMap<PlaceCreateRequest, Place>()
                .ForMember(p => p.PlaceEquipments, map => map.MapFrom(s => s.Equipment))
                .ForMember(p => p.PlaceUserRoles, map => map.MapFrom(pcr => pcr.Participants));

            CreateMap<Place, PlaceView>()
                .ForMember(p => p.Equipment, map => map.MapFrom(p =>
                    p.PlaceEquipments.Select(pe => pe.Equipment)
                ))
                .ForMember(p => p.Participants, map => map.MapFrom(p => p.PlaceUserRoles));

            CreateMap<RoleCreateRequest, Role>();
            CreateMap<Role, RoleView>();

            CreateMap<PlaceUserRole, UserAndRole>();

            CreateMap<User, UserView>();
            CreateMap<UserSetting, UserSettingPresent>()
                .ForMember(usp => usp.Value, map => map.MapFrom(us => us.Value.ParseToJson()));
        }

        private void EventEditMaps()
        {
            CreateMap<List<ShiftEditRequest>,List<Shift>>()
                .ConvertUsing(new ListsConverter<ShiftEditRequest, Shift>(s => s.Id));
            CreateMap<EventEditRequest, Event>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));

            CreateMap<List<PlaceEditRequest>,List<Place>>()
                .ConvertUsing(new ListsConverter<PlaceEditRequest, Place>(p => p.Id));
            CreateMap<ShiftEditRequest, Shift>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));
            
            CreateMap<List<DeletableRequest>,List<PlaceEquipment>>()
                .ConvertUsing(new ListsConverter<DeletableRequest, PlaceEquipment>(eq => eq.EquipmentId));
            CreateMap<List<PersonWorkRequest>,List<PlaceUserRole>>()
                .ConvertUsing(new ListsConverter<PersonWorkRequest, PlaceUserRole>(pur => pur.UserId));
            CreateMap<PlaceEditRequest, Place>()
                .ForMember(p => p.PlaceEquipments, map => map.MapFrom(per => per.Equipment))
                .ForMember(p => p.PlaceUserRoles, map => map.MapFrom(per => per.Participants))
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));
            CreateMap<DeletableRequest, PlaceEquipment>()
                .ForMember(pe => pe.EquipmentId, map => map.MapFrom(eer => eer.Id));
        }
    }
}