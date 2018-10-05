using System.ComponentModel.DataAnnotations;
using System;

namespace Models.PublicAPI.Requests.Equipment.EquipmentType
{
    public class EquipmentTypeCreateRequest
    {
        [Required]
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}
