using Organiser.Common.Classes;
using RssReader.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RssReader.Models
{
    public class RssList : INotifyPropertyChanged
    {
        public event Action<string> OnSelectedLaunchLink = delegate { };//send to browser
        public event Action<string> OnSelectedLaunchLinkMasher = delegate { };//send to MAsher
        public event Action<string,string,string,string,string> OnSelectedSendToPbn = delegate { };//send to MAsher

        public ICommand SendToBrowser { get; set; }

        public RssList()
        {
            PBarVis = true;
            SendToBrowser = new RelayCommand(OnSendToBrowser);
        }

        private void OnSendToBrowser(object obj)
        {
            switch ((string)obj)
            {
                case "Browser":
                    OnSelectedLaunchLink(ListResults[SIListResults].Link);
                    break;

                case "Masher":
                    OnSelectedLaunchLinkMasher(ListResults[SIListResults].Link);
                    break;

                case "PBNPOSTER":
                    OnSelectedSendToPbn(ListResults[SIListResults].Link, ListResults[SIListResults].Title, ListResults[SIListResults].ImageLink, ListResults[SIListResults].Date, ListResults[SIListResults].Description);
                    break;

                default:
                    break;
            }
        }

        private string rssLink;
        public string RssLink
        {
            get { return rssLink; }
            set
            {
                rssLink = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("RssLink"));
            }
        }
        //
        private bool pBarVis;
        public bool PBarVis
        {
            get { return pBarVis; }
            set
            {
                pBarVis = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PBarVis"));
            }
        }

        private bool listResultVis;
        public bool ListResultVis
        {
            get { return listResultVis; }
            set
            {
                listResultVis = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ListResultVis"));
            }
        }

        private int sIListResults;
        public int SIListResults
        {
            get { return sIListResults; }
            set
            {
                sIListResults = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SIListResults"));
            }
        }

        //

        public List<RssResult> ListResults { get; set; }

        public void RaisListPropChanged()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("ListResults"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
