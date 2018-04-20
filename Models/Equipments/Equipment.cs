using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Equipments
{
    public class Equipment
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }


        public Guid EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }
    }
}
