using Gecko.Windows;
using Gecko.Windows.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko.GUI;
using BrowseoFX_WPF.Windows;
using Organiser.Common.Classes;
using Gecko.DOM;
using Gecko.Interfaces;
using Gecko.Interop;
using Gecko;

namespace BrowseoFX_WPF.Crawlers
{
    public class FBConverseoOGCrawler 
    {
        //private FBConverseoOGCrawler() { }
        //private static FBConverseoOGCrawler instance;
        //public static FBConverseoOGCrawler Instance { get { if (instance == null) { instance = new FBConverseoOGCrawler(); } return instance; } } 

        public const string PageSearchUrl = "https://www.facebook.com/search/pages/?q=";

        public string AccessToken = "";

        public FBrowserWindow window { get; set; }
        public WebView MainWebView { get { return window.MainWebView; } }

        string resultsClassname = "_3u1 _gli _uvb";

        int maxScrollTrys = 25, currentScrollTry = 0;


        private GeckoXULElement tabbrowser;
        /// <summary>
        /// <tabbrowser id="content"
        ///             flex="1" 
        ///             contenttooltip="aHTMLTooltip"
        ///             tabcontainer="tabbrowser-tabs"
        ///             contentcontextmenu="contentAreaContextMenu"
        ///             autocompletepopup="PopupAutoComplete"
        ///             selectmenulist="ContentSelectDropdown"
        ///             datetimepicker="DateTimePickerPanel"/>
        /// </summary>
        public GeckoXULElement Tabbrowser
        {
            get
            {
                if (tabbrowser == null) tabbrowser = MainWebView.Widget.View.Document.GetElementById("content") as GeckoXULElement;
                return tabbrowser;
            }
        }

        /// <summary>
        /// returns the selected browser xul element
        /// </summary>
        public GeckoXULElement SelectedBrowser
        {
            get
            {
                return GeckoNode.Create(Tabbrowser.GetProperty<nsIDOMNode>("selectedBrowser")) as GeckoXULElement;
            }
        }

        /// <summary>
        /// returns the selected browsers html contentDocument
        /// </summary>
        public GeckoDocument SelectedContentDocument
        {
            get
            {
                return GeckoDocument.Create(SelectedBrowser.GetProperty<nsIDOMDocument>("contentDocument"));
            }
        }

        public void StartWindow()
        {
            window = new FBrowserWindow();
            window.Title = GloableProfData.PData.ProjectName;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.Width = System.Windows.SystemParameters.WorkArea.Width;
            window.Height = System.Windows.SystemParameters.WorkArea.Height;

            window.Show();
        }

        public void SearchForPage(string keyword)
        {
            var url = PageSearchUrl + keyword;
            MainWebView.Navigate(url);
            MainWebView.Navigated += MainWebView_Navigated_SearchForPage;
        }

        private void MainWebView_Navigated_SearchForPage(object sender, Gecko.GeckoNavigatedEventArgs e)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    MainWebView.Window.ScrollTo(0, (int)(MainWebView.Window.ScrollY + window.ActualHeight));
            //    await Task.Delay(200);
            //}
            window.WindowState = System.Windows.WindowState.Maximized;
            //GetMinResults();

        }

        #region search
        private async void GetMinPageResults()
        {
            var results = MainWebView.Document.GetElementsByClassName(resultsClassname);

            if (results.Length < 150 && results.Length > 0)
            {
                MainWebView.Window.ScrollTo(0, (int)(MainWebView.Window.ScrollY + window.ActualHeight));
                currentScrollTry++;
                if (currentScrollTry >= maxScrollTrys)
                {
                    ReadPageResults();
                    return;
                }
                else
                {
                    await Task.Delay(500);
                    GetMinPageResults();
                }
            }
        }

        private void ReadPageResults()
        {
            //https://graph.facebook.com/v2.12/
            var graphUrl = "https://graph.facebook.com/v2.12/";

            var results = MainWebView.Document.GetElementsByClassName(resultsClassname);
            var ids = new List<string>();

            foreach (var result in results)
            {
                foreach (var attribute in result.Attributes)
                {
                    if (attribute.LocalName == "data-bt")
                    {
                        //{"id":352751268256569,"rank":1,"abtest_version":null,"abtest_params":{"abtest_version":null,"origin":"A","ranker":null},"section":"main_column","owner_id":null,"sub_id":null,"browse_location":null,"query_data":{"q":"tech"},"is_headline":false}

                        var nodeValue = attribute.NodeValue;
                        nodeValue = nodeValue.Remove(nodeValue.IndexOf(","));
                        nodeValue = nodeValue.Replace("{\"id\":" , "");
                        ids.Add(nodeValue);
                    }
                }
            }

            foreach (var id in ids)
            {

            }

            if(ids.Count > 0)
            {

            }
        }
        #endregion



        public void ReadStats(string url)
        {
            if (AccessToken.IsNullOrEmpty())
            {
                "You must refresh the Access Token before continuing.".Show();
                return;
            }
            if (url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");
            if (url.Contains("?ref=br_rs")) url = url.Replace("?ref=br_rs", "");

            string pageName = url;
            string urltillId = url;
            string fullOgUrl = "";

            pageName = getPageNameOrIdFromUrl(url);
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrWhiteSpace(pageName))
            {
                return;
            }
            if (url.Contains("/")) urltillId = url.Remove(url.LastIndexOf("/") + 1);


            if (url.StartsWith("https://www.facebook.com/pages/"))
            {
                fullOgUrl = "https://graph.facebook.com/v2.12/" + pageName + "?fields=" +
                                            @"about,id,link,founded,can_post,category,talking_about_count,likes,
                                        photos.limit(30){picture,id,link,updated_time,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        videos.limit(30){permalink_url,picture,id,length,embed_html,source,updated_time,description,embeddable,title,likes.limit(0).summary(true),comments.limit(0).summary(true)},
                                        posts.limit(100){caption,description,picture,full_picture,shares,link,message,via,source,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)},
                                        feed.limit(70){caption,created_time,description,full_picture,id,is_expired,is_hidden,is_published,link,message,name,object_id,picture,shares,source,story,type,updated_time,comments.limit(0).summary(true),likes.limit(0).summary(true)}
                                        &access_token=" + AccessToken;
            }

            MainWebView.Navigated += MainWebView_NavigatedReadStats;
            MainWebView.Navigate(fullOgUrl);
        }

        private void MainWebView_NavigatedReadStats(object sender, Gecko.GeckoNavigatedEventArgs e)
        {
            window.WindowState = System.Windows.WindowState.Maximized;
            // MainWebView.Navigated -= MainWebView_NavigatedReadStats;

            //string json = "";
            //string source = (SelectedContentDocument.DocumentElement as Gecko.DOM.HTML.GeckoHTMLHtmlElement).OuterHtml;

            //if (source.Contains("<html><head></head><body>"))
            //{
            //    json = source.Split(new string[] { @"<html><head></head><body>" }, StringSplitOptions.None)[1];
            //    json = json.Split('>')[1];
            //    json = json.Remove(json.IndexOf("</pre"));
            //}
        }

        private string getPageNameOrIdFromUrl(string url)
        {
            try
            {
                if (!url.Contains("https://www.facebook.com/")) return "";

                string pageName = url;
                pageName = url.Substring(url.LastIndexOf("/") + 1);
                if (url.Contains("-"))
                {
                    string id = url.Substring(url.LastIndexOf("-") + 1);
                    long tryparseResult = 0;
                    if (Int64.TryParse(id, out tryparseResult))
                    {
                        pageName = id;
                    }
                }

                return pageName;
            }
            catch
            {
                return "";
            }
        }
    }
}
