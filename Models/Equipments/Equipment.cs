using Models.DataBaseLinks;
using System;
using System.Collections.Generic;
using System.Text;
using Models.People;

namespace Models.Equipments
{
    public class Equipment
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        // For many to many links, not ideal model
        public List<PlaceEquipment> PlaceEquipments { get; set; }


        public Guid EquipmentTypeId { get; set; }
        public EquipmentType EquipmentType { get; set; }

        public Guid? OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
