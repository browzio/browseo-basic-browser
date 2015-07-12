using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DragDropListview
{
    public class Bookmark :INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value;
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
            }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Link"));
                }
            }
        }

        private string dateTimeStamp;
        public string DateTimeStamp
        {
            get { return dateTimeStamp; }
            set
            {
                dateTimeStamp = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DateTimeStamp"));
                }
            }
        }

        public BitmapImage BitmapImg { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
