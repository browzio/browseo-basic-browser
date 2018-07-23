using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.SocialBirdEye.Models
{
    public class FacebookPages
    {
        private ObservableCollection<PageData> mydata;
        public ObservableCollection<PageData> data
        {
            get { return mydata; }
            set { mydata = value; }
        }
    }

    public class PageData
    {
        public string access_token { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }
}
