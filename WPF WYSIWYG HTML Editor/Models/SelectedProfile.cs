using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_WYSIWYG_HTML_Editor.Models
{
    public class SelectedProfile : INotifyPropertyChanged
    {
        private string profileName;
        public string ProfileName
        {
            get { return profileName; }
            set
            {
                profileName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProfileName"));
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
            }
            }
        }


        public string Path { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
