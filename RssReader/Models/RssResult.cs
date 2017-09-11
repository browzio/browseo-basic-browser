using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
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
    public class RssResult : ViewModelBase ,IHaveSocialStats
    {
        public event Action<string, string,string> OnClickedSendSocialLink = delegate { }; //socialType , link, imgLink

        public ICommand SendToBrowserSocial { get; set; }

        public RssResult()
        {
            SendToBrowserSocial = new RelayCommand(SendToBrowserSocialClick);
            SocialStatsReplys = new Organiser.Common.Classes.SocialHelpers.SocialStatsReplys();
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
                RaisePropertyChanged("Title");
            }
        }

        private string link;
        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                RaisePropertyChanged("Link");
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }

        private string imageLink;
        public string ImageLink
        {
            get { return imageLink; }
            set
            {
                imageLink = value;
                if (imageLink == "") ImageLinkVisible = Visibility.Collapsed;
                else if (imageLink != "" && !imageLink.StartsWith("http"))
                {
                    imageLink = "http:" + imageLink;
                }
                RaisePropertyChanged("ImageLink");
            }
        }

        private Visibility imageLinkVisible;
        public Visibility ImageLinkVisible
        {
            get { return imageLinkVisible; }
            set
            {
                imageLinkVisible = value;
                RaisePropertyChanged("ImageLinkVisible");
            }
        }

        private string date;
        public string Date
        {
            get { return "published: " + date; }
            set
            {
                date = value;
                RaisePropertyChanged("Date");
            }
        }

        private SocialStatsReplys socialStatsReplys;
        public SocialStatsReplys SocialStatsReplys
        {
            get { return socialStatsReplys; }
            set { socialStatsReplys = value; RaisePropertyChanged("SocialStatsReplys"); }
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
