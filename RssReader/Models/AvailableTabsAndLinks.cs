using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RssReader.Models
{
    public class AvailableTabsAndLinks : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
            }
        }


        private Visibility visibleHasNext = Visibility.Collapsed;
        public Visibility VisibleHasNext
        {
            get { return visibleHasNext; }
            set
            {
                visibleHasNext = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VisibleHasNext"));
                }
            }
        }
        private Visibility visibleProjIcon = Visibility.Collapsed;
        public Visibility VisibleProjIcon
        {
            get { return visibleProjIcon; }
            set
            {
                visibleProjIcon = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VisibleProjIcon"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
