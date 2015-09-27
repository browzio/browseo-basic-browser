using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes
{
    public class BacklinkHistoryLine : ViewModelBase
    {
        private string site;
        public string Site
        {
            get { return site; }
            set
            {
                site = value;
                RaisePropertyChanged("Site");
            }
        }

        private string moneySite;
        public string MoneySite
        {
            get { return moneySite; }
            set
            {
                moneySite = value;
                RaisePropertyChanged("MoneySite");
            }
        }

        private string backlinkText;
        public string BacklinkText
        {
            get { return backlinkText; }
            set
            {
                backlinkText = value;
                RaisePropertyChanged("BacklinkText");
            }
        }

    }
}
