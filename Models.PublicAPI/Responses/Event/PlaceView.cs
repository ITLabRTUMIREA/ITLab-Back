using System;
using System.Collections.Generic;
using Models.PublicAPI.Responses.Equipment;

namespace Models.PublicAPI.Responses.Event
{
    public class PlaceView
    {
        public Guid Id { get; set; }
        public int TargetParticipantsCount { get; set; }
        public List<EquipmentView> Equipment { get; set; }
        public List<UserAndRole> Participants { get; set; }
        public List<UserAndRole> Invited { get; set; }
        public List<UserAndRole> Wishers { get; set; }
        public List<UserAndRole> Unknowns { get; set; }
    }
}