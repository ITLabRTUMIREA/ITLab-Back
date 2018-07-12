using System.Collections.Generic;
using Models.PublicAPI.Responses.Equipment;

namespace Models.PublicAPI.Responses.Event
{
    public class PlaceView
    {
        public  List<EquipmentView> Equipment { get; set; }
        public List<UserAndRole> Users { get; set; }
    }
}