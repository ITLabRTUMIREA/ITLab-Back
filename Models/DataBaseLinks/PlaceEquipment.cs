using System;
using System.Collections.Generic;
using System.Text;
using Models.Equipments;
using Models.Events;

namespace Models.DataBaseLinks
{
    public class PlaceEquipment
    {
        public Guid PlaceId { get; set; }
        public Place Place { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
    }
}
