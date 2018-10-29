using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Equipment
{
    public class EquipmentView
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }

        public CompactEquipmentTypeView EquipmentType { get; set; }

        public Guid EquipmentTypeId { get; set; }
        public Guid? OwnerId { get; set; }

        public Guid? ParentId { get; set; }
        public List<EquipmentView> Children { get; set; }
    }
}
