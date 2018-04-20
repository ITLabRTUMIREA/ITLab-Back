using System;
using System.Collections.Generic;
using System.Text;

namespace Models.PublicAPI.Requests.Equipment
{
    public class EquipmentTypeEditRequest : IdRequest
    {
        public string Title { get; set; }
    }
}
