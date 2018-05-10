using System.ComponentModel.DataAnnotations;

namespace Models.PublicAPI.Requests.Equipment.EquipmentType
{
    public class EquipmentTypeCreateRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
