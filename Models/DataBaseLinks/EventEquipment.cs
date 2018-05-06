using Models.Equipments;
using Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.DataBaseLinks
{
    public class EventEquipment
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public static EventEquipment Create(Event ev, Equipment eq)
            => new EventEquipment
            {
                EventId = ev.Id,
                Event = ev,
                EquipmentId = eq.Id,
                Equipment = eq
            };
        public static EventEquipment Create(Event ev, Guid eqId)
            => new EventEquipment
            {
                EventId = ev.Id,
                Event = ev,
                EquipmentId = eqId,
            };
    }
}
