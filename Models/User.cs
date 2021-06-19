using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required, StringLength(25)]
        public string Username { get; set; }
        
        [Required, StringLength(25,MinimumLength = 8)]
        public string Password { get; set; }
        
        public Role Role { get; set; }
    }
}
