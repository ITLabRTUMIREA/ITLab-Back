using Models.PublicAPI.Responses.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Responses.Event.CreateEdit
{
    public class CreateEditPlaceView
    {
        public Guid Id { get; set; }
        public int ClientId { get; set; }
        public int TargetParticipantsCount { get; set; }
        public string Description { get; set; }
        public List<EquipmentView> Equipment { get; set; }
        public List<UserAndEventRole> Participants { get; set; }
        public List<UserAndEventRole> Invited { get; set; }
        public List<UserAndEventRole> Wishers { get; set; }
        public List<UserAndEventRole> Unknowns { get; set; }
    }
}
