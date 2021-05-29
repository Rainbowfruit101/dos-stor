using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Role
    {
        [Required]
        public Guid Id { get; set; }
        [Required, StringLength(20)]
        public string Name { get; set; }
    }
}
