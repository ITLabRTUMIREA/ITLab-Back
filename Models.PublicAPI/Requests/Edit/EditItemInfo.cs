using System;

namespace Models.PublicAPI.Requests.Edit 
{
    public class EditItemInfo 
    {
        public Guid Id { get; set; }
        public EditAction EditAction { get; set; }
    }
}