using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Equipment.Equipment
{
    public class EquipmentEditRequest : EquipmentCreateRequest
    {
        public Guid Id { get; set; }
    }
}
