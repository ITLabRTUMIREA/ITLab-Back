using System;
using System.Collections.Generic;

namespace Models.PublicAPI.Responses.Equipment
{
    public class CompactEquipmentTypeView
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
