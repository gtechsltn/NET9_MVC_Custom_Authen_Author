using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMvc.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; } // In a real application, this should be hashed
    }
}