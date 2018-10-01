using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Models.Equipments;
using Models.PublicAPI.Requests.Equipment.EquipmentType;

namespace BackEnd.Formatting.MapperProfiles.RequestProfiles
{
    public class EquipmentProfile : Profile
    {
        public EquipmentProfile()
        {
            EquipmentTypeRules();
        }
        private void EquipmentTypeRules()
        {
            CreateMap<EquipmentTypeEditRequest, EquipmentType>()
                .ForAllMembers(opt => opt.Condition(a =>
                    a.GetType().GetProperty(opt.DestinationMember.Name)?.GetValue(a) != null));
        }
    }

}
