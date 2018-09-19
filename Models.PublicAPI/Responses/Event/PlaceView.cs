using System;
using System.Collections.Generic;
using Models.PublicAPI.Responses.Equipment;

namespace Models.PublicAPI.Responses.Event
{
    public class PlaceView
    {
        public Guid Id { get; set; }
        public int TargetParticipantsCount { get; set; }
        public string Description { get; set; }
        public List<EquipmentView> Equipment { get; set; }
        public List<UserAndEventRole> Participants { get; set; }
        public List<UserAndEventRole> Invited { get; set; }
        public List<UserAndEventRole> Wishers { get; set; }
        public List<UserAndEventRole> Unknowns { get; set; }
    }
}