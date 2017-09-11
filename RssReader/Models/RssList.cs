using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
using Organiser.Common.Windows;
using RssReader.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RssReader.Models
{
    public class RssList : INotifyPropertyChanged
    {
        public event Action<string,bool> OnSelectedLaunchLink = delegate { };//send to browser| url,isff
        public event Action<string,string> OnSelectedSendToSeo = delegate { };//title,url
        public event Action<string> OnSelectedLaunchLinkMasher = delegate { };//send to MAsher
        public event Action<string,string,string,string,string> OnSelectedSendToPbn = delegate { };//send to MAsher
        public event Action<RssList> OnListItemChanged = delegate { };

        public ICommand SendToBrowser { get; set; }

        public RssList()
        {
            PBarVis = true;
            SendToBrowser = new RelayCommand(OnSendToBrowser);
        }

        private void OnSendToBrowser(object obj)
        {
            var param = (string)obj;
            switch (param)
            {
                case "Browser":
                case "BrowserFF":
                    OnSelectedLaunchLink(ListResults[SIListResults].Link,param== "BrowserFF");
                    break;

                case "ORDERBY_FBSHARES":
                case "ORDERBY_FBLIKES":
                case "ORDERBY_FBCOMMENTS":
                case "ORDERBY_GPLUSONES":
                case "ORDERBY_PINTERESTPINS":
                case "ORDERBY_STUMBLEVIEWS":
                case "ORDERBY_LINKEDINCOUNT":
                case "ORDERBY_BUFFERSHARES":
                case "ORDERBY_REDDITUPS":
                case "ORDERBY_REDDITSCORE":
                    var resulstsList = SocialStatsFunctions.OrderStatsBy(ListResults.ToList(), param);
                    if (resulstsList == null || resulstsList.Count() == 0) return;

                    ListResults.Clear();
                    foreach (var r in resulstsList) ListResults.Add(r as RssResult);
                    OnListItemChanged(this);
                    break;

                case "GrabAllLinks":
                    var textWindow = new RssFeedsLinksMultiWindow();
                    textWindow.Title = RssLink;
                    foreach (var result in ListResults)
                    {
                        textWindow.tbInputedText.Text += result.Link + Environment.NewLine;
                    }
                    textWindow.Show();
                    break;

                case "Masher":
                    OnSelectedLaunchLinkMasher(ListResults[SIListResults].Link);
                    break;

                case "PBNPOSTER":
                    OnSelectedSendToPbn(ListResults[SIListResults].Link, ListResults[SIListResults].Title, ListResults[SIListResults].ImageLink, ListResults[SIListResults].Date, ListResults[SIListResults].Description);
                    break;

                case "Curaste":
                    try {
                        string htmlstring = "<blockquote>";
                        if (!string.IsNullOrEmpty(ListResults[SIListResults].Title) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Title))
                            htmlstring += "<h1>" + ListResults[SIListResults].Title + "</h1>";
                        if (!string.IsNullOrEmpty(ListResults[SIListResults].Date) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Date))
                            htmlstring += "<p>" + ListResults[SIListResults].Date + "</p>";
                        if (ListResults[SIListResults].ImageLink != "https:" && ListResults[SIListResults].ImageLink != "http:" &&
                            !string.IsNullOrEmpty(ListResults[SIListResults].ImageLink) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].ImageLink))
                            htmlstring += "<img src=\"" + ListResults[SIListResults].ImageLink + "\" />";
                        if (!string.IsNullOrEmpty(ListResults[SIListResults].Description) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Description))
                            htmlstring += "<p>" + ListResults[SIListResults].Description + "</p>";
                        if (!string.IsNullOrEmpty(ListResults[SIListResults].Link) && !string.IsNullOrWhiteSpace(ListResults[SIListResults].Link))
                            htmlstring += "<a href=\"" + ListResults[SIListResults].Link + " \" > " + ListResults[SIListResults].Link + " </a>";
                        htmlstring += "</blockquote>";

                        MyFilesDatabase.SetClipboardText(htmlstring);
                    }
                    catch
                    {
                        MessageBox.Show("Failed to set clipboard curation.");
                    }
                    break;

                case "TOSEO":
                    OnSelectedSendToSeo(ListResults[SIListResults].Title, ListResults[SIListResults].Link);
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
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("ListResults"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
