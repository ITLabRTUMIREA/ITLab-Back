using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Equipment
{
    public class EquipmentPresent
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }

        public Guid EquipmentTypeId { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
