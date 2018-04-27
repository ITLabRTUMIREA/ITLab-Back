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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Formating
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<EquipmentTypeCreateRequest, EquipmentType>();
            CreateMap<EquipmentCreateRequest, Equipment>();
            CreateMap<Equipment, EquipmentPresent>();
            CreateMap<AccountCreateRequest, User>().ForMember(u => u.UserName, map => map.MapFrom(ac => ac.Email));
            CreateMap<User, LoginResponse>();

            CreateMap<EqiupmentEditRequest, Equipment>()
               .ForAllMembers(opt => opt.Condition(a =>
                 a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));


            CreateMap<EventTypeCreateRequest, EventType>();
            CreateMap<EventCreateRequest, Event>();
            CreateMap<Event, EventPresent>()
                .AfterMap((e, ep, ctxt) => ep.EquipmentIds = e.EventEquipments?.Select(local => local.EquipmentId)?.ToList());
            CreateMap<EqiupmentEditRequest, Event>()
                  .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));


            CreateMap<RoleCreateRequest, Role>();

        }
    }
}
