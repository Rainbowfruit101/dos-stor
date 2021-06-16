using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DocumentView
    {
        public string Name { get; set; }
        public List<TagView> Tags { get; set; }
    }
}
