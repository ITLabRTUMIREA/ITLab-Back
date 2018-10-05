using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Equipment.EquipmentType
{
    public class EquipmentTypeEditRequest : DeletableRequest
    {
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}
