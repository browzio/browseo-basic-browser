using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browser.Common.Models
{
    public class LinkOnPage : ViewModelBase, IHaveSocialStats
    {
        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; RaisePropertyChanged("Url"); }
        }

        private SocialStatsReplys socialStatsReplys;
        public SocialStatsReplys SocialStatsReplys
        {
            get { return socialStatsReplys; }
            set { socialStatsReplys = value; RaisePropertyChanged("SocialStatsReplys"); }
        }

        public LinkOnPage()
        {
            SocialStatsReplys = new SocialStatsReplys();
        }
    }
}
