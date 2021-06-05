using DocumentStorage.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Models
{
    public class DateSearchViewModel
    {
        public ComparisonMode Mode { get; set; }    
        public DateTime Date { get; set; }

    }
}
