using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Models.Equipments;
using Models.PublicAPI.Requests.Equipment.EquipmentType;
using Models.PublicAPI.Responses.Equipment;

namespace BackEnd.Formatting.MapperProfiles.ResponseProfiles
{
    public class EquipmentProfile : Profile
    {
        public EquipmentProfile()
        {
            EquipmentMaps();
        }
        private void EquipmentMaps()
        {
            CreateMap<Equipment, CompactEquipmentView>();
        }
    }

}
