using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
