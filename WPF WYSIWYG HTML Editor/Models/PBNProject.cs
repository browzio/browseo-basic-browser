using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WYSIWYG_HTML_Editor.Models
{
    public class PBNProject
    {
        public const int TYPE_WORDPRESS = 0;
        public const int TYPE_DRUPAL = 1;

        public string Name { get; set; }
        public string FilePath { get; set; }
        public int SIType { get; set; }
        public bool IsSelected { get; set; }
    }
}
