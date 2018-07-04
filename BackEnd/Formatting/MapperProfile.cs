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
using Extensions;

namespace BackEnd.Formating
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
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
            CreateMap<EventType, EventTypePresent>();
            CreateMap<EventCreateRequest, Event>();
            CreateMap<Event, EventPresent>()
                .ForMember(ep => ep.EquipmentIds, conf => conf.MapFrom(e => e.EventEquipments.Select(ee => ee.EquipmentId)))
                .ForMember(ep => ep.ParticipantsIds, conf => conf.MapFrom(e => e.EventUsers.Select(ee => ee.UserId)));
            CreateMap<EventEditRequest, Event>()
                  .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));


            CreateMap<RoleCreateRequest, Role>();

            CreateMap<User, UserPresent>();
            CreateMap<UserSetting, UserSettingPresent>()
                .ForMember(usp => usp.Value, map => map.MapFrom(us => us.Value.ParseToJson()));
        }
    }
}
