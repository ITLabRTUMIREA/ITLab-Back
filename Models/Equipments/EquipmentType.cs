using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Equipments
{
    public class EquipmentType
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Description { get; set; }
        public int LastNumber { get; set; }

        public List<EquipmentType> Children { get; set; }

        public Guid? ParentId { get; set; }
        public EquipmentType Parent { get; set; }
    }
}
