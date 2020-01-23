using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class RegisterTokenPair
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
