using System;
using System.Collections.Generic;
using System.Text;
using Extensions.General;
using Models.DataBaseLinks;
using Newtonsoft.Json.Serialization;

namespace Models.Events
{
    public class Place : IdInterface
    {
        public Guid Id { get; set; }


        public int TargetParticipantsCount { get; set; }
        public List<PlaceEquipment> PlaceEquipments { get; set; }
        public List<PlaceUserRole> PlaceUserRoles { get; set; }


        public Shift Shift { get; set; }
        public Guid ShiftId { get; set; }
    }
}
