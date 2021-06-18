using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Document
    {
        [Required]
        public Guid Id {get; set;}
        
        [Required, StringLength(80)]
        public string Name { get; set; }
        
        [Required]
        public DateTime CreationTime { get; set; }
        
        public List<Tag> Tags { get; set; }
        
        public List<Role> OwnRoles { get; set; }
    }
}
