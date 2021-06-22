using System.Collections.Generic;
using ViewModels.Enums;
using ViewModels.Views;

namespace ViewModels
{
    public class TagSearchViewModel
    {
        public TagSearchMode Mode { get; set; }
        public List<TagView> Tags { get; set; }
        
    }
}
