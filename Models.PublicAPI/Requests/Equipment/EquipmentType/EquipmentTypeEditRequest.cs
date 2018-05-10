using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Equipment.EquipmentType
{
    public class EquipmentTypeEditRequest : IdRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
