using Models;
using System.Collections.Generic;
using ViewModels.Enums;

namespace ViewModels
{
    public class TagSearchViewModel
    {
        public TagSearchMode Mode { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
