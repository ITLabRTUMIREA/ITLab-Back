using AutoMapper;
using Models.Equipments;
using Models.Events;
using Models.PublicAPI.Requests.Equipment;
using Models.PublicAPI.Requests.Equipment.Equipment;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Requests.Events.EventType;
using Models.PublicAPI.Responses.Equipment;
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
            CreateMap<EventTypeCreateRequest, EventType>();
        }
    }
}
