using System;
using System.Collections.Generic;
using System.Text;
using Models.DataBaseLinks;
using Newtonsoft.Json.Serialization;

namespace Models.Events
{
    public class Place
    {
        public Guid Id { get; set; }


        public int TargetParticipantsCount { get; set; }
        public List<PlaceEquipment> PlaceEquipments { get; set; }
            = new List<PlaceEquipment>();

        public List<PlaceUserEventRole> PlaceUserEventRoles { get; set; }
            = new List<PlaceUserEventRole>();

        public string Description { get; set; }

        public Shift Shift { get; set; }
        public Guid ShiftId { get; set; }
    }
}
