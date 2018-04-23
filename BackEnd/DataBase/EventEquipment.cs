using Models.Equipments;
using Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.DataBase
{
    public class EventEquipment
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }
    }
}
