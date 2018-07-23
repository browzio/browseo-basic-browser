using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models
{
    public class FacebookUserFeed
    {
        private ObservableCollection<FeedData> mydata;
        public ObservableCollection<FeedData> data
        {
            get { return mydata; }
            set { mydata = value; }
        }
    }

    public class FeedData
    {
        public string picture { get; set; }
        public string created_time { get; set; }
        public string description { get; set; }
        public string message { get; set; }
        public string type { get; set; }
        public string id { get; set; }

        public From from { get; set; }
        public FacebookLikesBase likes { get; set; }
        public FacebookCommentsBase comments
        { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string id { get; set; }
    }
}
