using Organiser.Common.Classes;
using RssReader.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RssReader.Models
{
    public class RssResult : INotifyPropertyChanged
    {
        public event Action<string, string,string> OnClickedSendSocialLink = delegate { }; //socialType , link, imgLink

        public ICommand SendToBrowserSocial { get; set; }

        public RssResult()
        {
            SendToBrowserSocial = new RelayCommand(SendToBrowserSocialClick);
        }

        private void SendToBrowserSocialClick(object param)
        {
            OnClickedSendSocialLink((string)param, link, imageLink);
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("Link"));
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
            }
        }

        private string imageLink;
        public string ImageLink
        {
            get { return imageLink; }
            set
            {
                imageLink = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageLink"));
            }
        }

        private Visibility imageLinkVisible;
        public Visibility ImageLinkVisible
        {
            get { return imageLinkVisible; }
            set
            {
                imageLinkVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ImageLinkVisible"));
            }
        }

        private string date;
        public string Date
        {
            get { return "published: " + date; }
            set
            {
                date = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
