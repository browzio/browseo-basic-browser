using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WYSIWYG_HTML_Editor.Models
{
    public class PBNProject : INotifyPropertyChanged
    {
        public const int TYPE_WORDPRESS = 0;
        public const int TYPE_DRUPAL = 1;

        public string Name { get; set; }
        public string ProjectName { get; set; }
        public string FilePath { get; set; }
        public int SIType { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        private string domain;
        public string DomainAuthority
        {
            get { return domain; }
            set { domain = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("DomainAuthority"));
            }
        }

        private string page;
        public string PageAuthority
        {
            get { return page; }
            set
            {
                page = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PageAuthority"));
            }
        }

        private System.Windows.Visibility vis;
        public System.Windows.Visibility AuthorityVisible
        {
            get { return vis; }
            set
            {
                vis = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AuthorityVisible"));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
