using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Tag
    {
        [Required]
        public Guid Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public List<Document> Documents { get; set; }
        
        public override bool Equals(object? obj)
        {
            if (obj is Tag tag)
                return tag.Id == Id;

            return false;
        }
    }
}

