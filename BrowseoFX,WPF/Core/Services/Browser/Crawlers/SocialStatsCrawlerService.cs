using Browser.Common.Models;
using Gecko.DOM.HTML;
using Organiser.Common.Classes;
using Organiser.Common.Classes.SocialHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrowseoFX_WPF.Core.Services.Browser.Crawlers
{
    public class SocialStatsCrawlerService : PropertyChangedBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<LinkOnPage> CrawledLinksOnPage { get; set; }

        private bool socialStatsCrawlActive;
        public bool SocialStatsCrawlActive
        {
            get { return socialStatsCrawlActive; }
            set
            {
                socialStatsCrawlActive = value;
                if (value) SocialStatsCrawl();
                NotifyOfPropertyChange();
            }
        }

        private bool isCralAllActive;
        public bool IsCrawlAllActive
        {
            get { return isCralAllActive; }
            set
            {
                isCralAllActive = value;
                if (value) SocialStatsCrawl();
                NotifyOfPropertyChange("IsCrawlAllActive");
            }
        }

        private string linksToStatCheck;
        public string LinksToStatCheck
        {
            get { return linksToStatCheck; }
            set { linksToStatCheck = value; NotifyOfPropertyChange("LinksToStatCheck"); }
        }

        private Visibility isLoadingStatsVisible = Visibility.Visible;
        public Visibility IsLoadingStatsVisible
        {
            get { return isLoadingStatsVisible; }
            set { isLoadingStatsVisible = value; RaisePropertyChanged("IsLoadingStatsVisible"); }
        }

        public string previusStatsCrawlUrl = "";

        public SocialStatsCrawlerService()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            CrawledLinksOnPage = new ObservableCollection<LinkOnPage>();
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            if (param == null) return;

            try
            {
                switch (param)
                {
                    case "ClearSocialStats":
                        CrawledLinksOnPage.Clear();
                        previusStatsCrawlUrl = "";
                        IsLoadingStatsVisible = Visibility.Collapsed;
                        break;

                    case "CheckTheseLinks":
                        if (LinksToStatCheck.IsNullOrEmpty()) return;
                        var links = LinksToStatCheck.SplitAndRemoveEmpty(Environment.NewLine);
                        if (links == null) return;
                        foreach (var link in links)
                        {
                            var linkOnPage = new LinkOnPage() { Url = link };
                            linkOnPage.SocialStatsReplys.GetAllStatsFor(linkOnPage.Url);
                            CrawledLinksOnPage.Add(linkOnPage);
                        }
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
                        var resulstsList = SocialStatsFunctions.OrderStatsBy(CrawledLinksOnPage.ToList(), param);
                        if (resulstsList == null || resulstsList.Count() == 0) return;

                        CrawledLinksOnPage.Clear();
                        foreach (var r in resulstsList) CrawledLinksOnPage.Add(r as LinkOnPage);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //if (ex != null) ex.Message.Show();
            }
        }

        //needs selected tab
        public async void SocialStatsCrawl()
        {
            try
            {
                string absUri = BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.Url.AbsoluteUri;

                if (previusStatsCrawlUrl == absUri) CrawledLinksOnPage.Clear();

                previusStatsCrawlUrl = absUri;
                IsLoadingStatsVisible = Visibility.Visible;

                if (!CrawledLinksOnPage.Any(l => l.Url == absUri))
                {
                    var linkOfPage = new LinkOnPage()
                    {
                        Url = absUri,
                        SocialStatsReplys = new SocialStatsReplys()
                    };
                    CrawledLinksOnPage.Add(linkOfPage);
                    await linkOfPage.SocialStatsReplys.AsyncGetAllStatsFor(linkOfPage.Url);
                    linkOfPage.RaisePropertyChanged("SocialStatsReplys");
                }

                if (IsCrawlAllActive)
                {
                    var ahrefs = BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.GetElementsByTagName("A");
                    if (ahrefs == null || ahrefs.Count() == 0) return;

                    var ahrefsList = ahrefs.ToList();
                    foreach (var element in ahrefsList)
                    {
                        if (IsLoadingStatsVisible == Visibility.Collapsed) return;

                        var link = element as GeckoHTMLAnchorElement;
                        if (link == null || link.Href.IsNullOrEmpty() ||
                            link.Href.ToLower().Contains("login") || link.Href.ToLower().Contains("sign up") || link.Href.ToLower().Contains("void(0)") || link.Href.ToLower().Contains("javascript:;") || link.Href.ToLower().Contains("javascript ") ||
                            (absUri.Contains("google") && absUri.Contains("search") && link.Href.ToLower().Contains("google")) ||
                            CrawledLinksOnPage.Any(l => l.Url == link.Href)) continue;

                        var linkOnPage = new LinkOnPage()
                        {
                            Url = link.Href,
                            SocialStatsReplys = new SocialStatsReplys()
                        };
                        await linkOnPage.SocialStatsReplys.AsyncGetAllStatsFor(linkOnPage.Url);
                        CrawledLinksOnPage.Add(linkOnPage);
                    }
                }
            }
            catch(Exception ex)
            {
            }
            IsLoadingStatsVisible = Visibility.Collapsed;
        }
    }
}
