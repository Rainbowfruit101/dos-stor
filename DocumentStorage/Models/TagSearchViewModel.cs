using DocumentStorage.Enums;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentStorage.Models
{
    public class TagSearchViewModel
    {
        public TagSearchMode Mode { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
