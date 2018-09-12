using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Equipment.Equipment
{
    public class EquipmentCreateRequest
    {
        public string SerialNumber { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public string Description { get; set; }
    }
}
