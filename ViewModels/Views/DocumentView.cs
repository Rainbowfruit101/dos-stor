using System;
using System.Collections.Generic;

namespace ViewModels.Views
{
    public class DocumentView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public List<TagView> Tags { get; set; }
        public List<RoleView> OwnRoles { get; set; }
    }
}
