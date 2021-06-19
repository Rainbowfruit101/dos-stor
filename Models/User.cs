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
        
        [Required]
        public string Password { get; set; }
        
        public Role Role { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is User user)
                return user.Id == Id;

            return false;
        }
    }
}
