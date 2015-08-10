using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Models
{
    public class SavedPostedFeed : INotifyPropertyChanged
    {
        private string feedlinks;
        public string FeedLinks
        {
            get { return feedlinks; }
            set
            {
                feedlinks = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FeedLinks"));
            }
        }

        private string feedTitle;
        public string FeedTitle
        {
            get { return feedTitle; }
            set
            {
                feedTitle = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FeedTitle"));
            }
        }

        private int feedCategory;
        public int FeedCategory
        {
            get { return feedCategory; }
            set
            {
                feedCategory = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FeedCategory"));
            }
        }

        private string feedResult;
        public string FeedResult
        {
            get { return feedResult; }
            set
            {
                feedResult = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FeedResult"));
            }
        }

        private bool isRssMashup;
        public bool FeedIsRssMashup
        {
            get { return isRssMashup; }
            set
            {
                isRssMashup = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("FeedIsRssMashup"));
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
