using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Instagram
{
    public class InstagramUserInfo
    {
        public string followers_count { get; set; }
        public string username { get; set; }
        public string media_count { get; set; }
        public Insights insights { get; set; }
    }

    public class Insights
    {
        public InsightsData[] data
        {
            get;
            set;
        }
    }

    public class InsightsData
    {
        public string description { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public ValuesData[] values { get; set; }
    }

    public class ValuesData
    {
        public string value { get; set; }
        public string end_time { get; set; }
    }
}
