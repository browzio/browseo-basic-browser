using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospector.Models
{
    public class Footprint : INotifyPropertyChanged
    {
        public const int TYPE_WebsitesForBlogs = 0;
        public const int TYPE_TLDs = 1;
        public const int TYPE_TimeFrames = 2;
        public const int TYPE_Comments = 3;

        private string option;
        public string Option
        {
            get { return option; }
            set 
            {
                option = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Option"));
            }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set 
            {
                _checked = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Checked"));
            }
        }

        public webhose.Languages LangForWebhose { get; set; }
        public webhose.SiteTypes SiteTypeWebhose { get; set; }

        private string query;
        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Query"));
            }
        }

        public int Type
        {
            get;
            set;
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
