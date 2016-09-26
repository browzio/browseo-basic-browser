using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows;

namespace DragDropListview
{
    public class FolderVM : INotifyPropertyChanged
    {
        public FolderVM()
        {
            Sites = new ObservableCollection<Bookmark>();
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

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
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
        public bool IsFolder { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }

        //public bool IsSession { get; set; }

        //public bool IsImported { get; set; }
        public FolderTypes TypeOfFolder { get; set; }

        public string ImportType { get; set; }

        public ObservableCollection<Bookmark> Sites { get;  set; }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
