using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Role
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required, StringLength(50)]
        public string Name { get; set; }
        
        public List<Document> AllowDocuments { get; set; }

        public List<User> Users { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Role role)
                return role.Id == Id;

            return false;
        }
    }
}
