using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models
{
    public class FacebookLikesBase
    {
        private ObservableCollection<FeedData> mydata;
        public ObservableCollection<FeedData> data
        {
            get { return mydata; }
            set { mydata = value; }
        }

        public SummaryLikes summary { get; set; }
    }

    public class SummaryLikes
    {
        public string total_count { get; set; }
        public string can_like { get; set; }
        public string has_liked { get; set; }
    }

    public class FacebookCommentsBase
    {
        private ObservableCollection<FeedData> mydata;
        public ObservableCollection<FeedData> data
        {
            get { return mydata; }
            set { mydata = value; }
        }

        public SummaryComments summary { get; set; }
    }

    public class SummaryComments
    {
        public string order { get; set; }
        public string total_count { get; set; }
        public string can_comment { get; set; }
    }
}
