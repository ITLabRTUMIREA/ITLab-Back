using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Equipment
{
    public class EquipmentView
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public EquipmentTypeView EquipmentType { get; set; }

        public Guid EquipmentTypeId { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
