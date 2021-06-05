using Models.Enums;
using Models;
using System.Collections.Generic;

namespace ViewModels
{
    public class TagSearchViewModel
    {
        public TagSearchMode Mode { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
