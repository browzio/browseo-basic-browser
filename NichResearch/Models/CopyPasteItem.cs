using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NichResearch.Models
{
    public class CopyPasteItem
    {
        [DisplayName("Title")]
        public string Title { get; set; }
        [DisplayName("Web Link")]
        public string Link { get; set; }
    }
}
