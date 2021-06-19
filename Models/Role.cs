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
    }
}
