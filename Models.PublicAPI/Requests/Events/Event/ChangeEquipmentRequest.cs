using System;
using System.Collections.Generic;
using System.Text;
using Models.PublicAPI.Requests.Edit;

namespace Models.PublicAPI.Requests.Events.Event
{
    public class ChangeEquipmentRequest : IdRequest
    {
        public List<EditItemInfo> EquipmentEditPack { get; set; }
    }
}
