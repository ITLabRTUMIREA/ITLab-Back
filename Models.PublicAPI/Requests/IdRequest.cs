using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.PublicAPI.Requests
{
    public class IdRequest
    {
        [Required]
        public Guid Id { get; set; }
    }
}
