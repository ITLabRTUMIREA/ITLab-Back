using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Equipments
{
    public class Equipment
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        // For many to many links, not ideal model
        public List<EventEquipment> EventEquipments { get; set; }


        public Guid EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }
    }
}
