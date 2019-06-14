using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Core.Services.Browser.Crawlers;
using BrowseoFX_WPF.ViewModels.Addons;
using GoViral.ViewModels;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowseoFX_WPF.Controls
{

    /// <summary>
    /// Interaction logic for BrowserControl.xaml
    /// </summary>
    public partial class BrowserControl : UserControl,IListenToFXManager
    {
        public Action<string, string, List<string>> OnAddedToGoViral;
        public Action OnSpinClicked;
        public Action OnGloableWebView_Loaded;
        
        bool fbconverseoExpanded = false;
        bool lsbExpanded = false;
        bool seoExpanded = false;
        bool SocialStatsExpanderIsExpanded;
        bool fbSearchExpanded { get { return expFBSearch.Visibility == Visibility.Visible; } }

        #region init
        public BrowserControl()
        {
            InitializeComponent();

            this.Loaded += BrowserControl_Loaded;
            this.SizeChanged += BrowserControl_SizeChanged;
        }

        private async void BrowserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BrowserControl_Loaded;


            BrowseoFXManager.Instance.FxMAnagerListenerNotifyer = this;
            await BrowseoFXManager.Instance.Init();
            host.Children.Add(BrowseoFXManager.Instance.GloableWebView);
            BrowseoFXManager.Instance.GloableWebView.Loaded += GloableWebView_Loaded;
        }

        private Action EmptyDelegate = delegate () { };
        private void GloableWebView_Loaded(object sender, RoutedEventArgs e)
        {
            BrowseoFXManager.Instance.GloableWebView.Loaded -= GloableWebView_Loaded;

            UpdateLayouts();

            host.Width = this.ActualWidth + 15;

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            host.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            BrowseoFXManager.Instance.GloableWebView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            //AirspaceHost.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);

            OnGloableWebView_Loaded?.Invoke();
            BrowseoFXManager.Instance.TabbrowserHandler.OnAddedToGoViral += TabbrowserHandler_OnAddedToGoViral;
        }
        #endregion

        #region util
        
        private void CollapseExpanderBase()
        {
            AirspaceHost.Width = this.ActualWidth;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            host.Width = this.ActualWidth + 15;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void SetExpandSize()
        {
            AirspaceHost.Width = this.ActualWidth - 505;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Left;
            host.Width = this.ActualWidth - 485;
            host.HorizontalAlignment = HorizontalAlignment.Left;
        }

        private void SetExpanderStatesAll(bool fbConverse, bool fbSearch, bool lsb, bool seo)
        {
            if (!fbConverse)
            {
                fbConverseoCollapse();
            }
            if (!fbSearch)
            {
                fbSreachCollapse();
            }
            if (!lsb)
            {
                lsbCollapse();
            }
            if (!seo)
            {
                seoCollapse();
            }


            if (fbConverse)
            {
                if (fbconverseoExpanded)
                {
                    fbConverseoCollapse();
                }
                else
                {
                    fbConverseoExpand();
                }
            }

            if(fbSearch)
            {
                if(fbSearchExpanded)
                {
                    fbSreachCollapse();
                }
                else
                {
                    fbSreachExpand();
                }
            }

            if(lsb)
            {
                if (lsbExpanded)
                {
                    lsbCollapse();
                }
                else
                {
                    lsbExpand();
                }
            }

            if(seo)
            {
                if (seoExpanded)
                {
                    seoCollapse();
                }
                else
                {
                    seoExpand();
                }
            }
        }

        private void UpdateLayouts()
        {
            UpdateLayout();
            host.UpdateLayout();
            BrowseoFXManager.Instance.GloableWebView.UpdateLayout();
            BrowseoFXManager.Instance.GloableWebView.Widget.BaseWindow.Instance.Repaint(true);
        }

        public void CloseAllTabs()
        {
            BrowseoFXManager.Instance.Shutdown();
        }

        public void SearchFor(string query)
        {
            //    BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.LoadHtml("", query);
            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(query);
        }

        public void LaunchNewWindow(string link, string rssLink)
        {
            BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.Open(link, rssLink, "resizable,scrollbars,status");
        }

        public void AsyncAddLinkToList(string link, string type, List<string> multiLinks, bool showLinksWindow)
        {
            //GoViralVM goViralVM; AsyncAddLinkToList
            (ucGoViral.DataContext as GoViralVM).AsyncAddLinkToList(link, type, multiLinks, showLinksWindow);
        }

        #endregion

        #region IListenToFXManager

        public void OnOpenIAMacros()
        {
            // throw new NotImplementedException();
        }

        public void OnOpenFBConverseo()
        {
            (ucGoViral.DataContext as GoViralVM).OnSelectedTabNavigate -= BrowserControl_OnSelectedTabNavigate;
            (ucGoViral.DataContext as GoViralVM).OnSelectedTabNavigate += BrowserControl_OnSelectedTabNavigate;

            (ucGoViral.DataContext as GoViralVM).OnDominateAll -= GoViralVM_OnDominateAll;
            (ucGoViral.DataContext as GoViralVM).OnDominateAll += GoViralVM_OnDominateAll;

            SetExpanderStatesAll(true, false, false, false);
        }

        public void OnOpenFbSearch()
        {
            SetExpanderStatesAll(false, true, false, false);
        }

        public void OnOpenLSB()
        {
            SetExpanderStatesAll(false, false, true, false);
        }

        public void OnOpenSEO()
        {
            SetExpanderStatesAll(false, false, false, true);
        }

        public void OnOpenSocialStats(SocialStatsCrawlerService sscs)
        {
            if (SocialStatsExpanderIsExpanded)
            {
                SocialStatsExpander_Collapsed(null, null);
            }
            else
            {
                SocialStatsExpander.DataContext = sscs;
                SocialCrawl_Click(null, null);
            }
        }

        #endregion

        #region events

        private void BrowserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (host.HorizontalAlignment == HorizontalAlignment.Left)
            //{
            host.Width = this.ActualWidth + 15;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    if (resizeBigger) host.Width -= 400;
            host.HorizontalAlignment = HorizontalAlignment.Left;
            //}

            if (SocialStatsExpander.IsExpanded && host.ActualHeight > 350)
            {
                host.Height = host.ActualHeight - 300;
            }

            if (fbconverseoExpanded && !seoExpanded && !lsbExpanded && !fbSearchExpanded)
            {
                fbConverseoExpand();
            }
            else if (!fbconverseoExpanded && !seoExpanded && lsbExpanded && !fbSearchExpanded)
            {
                lsbExpand();
            }
            else if (!fbconverseoExpanded && seoExpanded && !lsbExpanded && !fbSearchExpanded)
            {
                seoExpand();
            }
            else if (!fbconverseoExpanded && !seoExpanded && !lsbExpanded && fbSearchExpanded)
            {
                fbSreachExpand();
            }
            else
            {
                AirspaceHost.Width = this.ActualWidth;
            }
        }

        private void TabbrowserHandler_OnAddedToGoViral(string arg1, string arg2, List<string> linksToReturn)
        {
            OnAddedToGoViral?.Invoke(arg1, arg2, linksToReturn);
        }

        private void ManagerForTab_OnPlayMacro(MacroManger manger, IIMPlayType type, int loop)
        {
            //if (DataContext is BrowserTabViewModel)
            //{
            //    await (DataContext as BrowserTabViewModel).OnPlayMacro(manger, type, loop);
            //}
        }

        private void GoViralVM_OnDominateAll()
        {
            BrowseoFXManager.Instance.TabbrowserHandler.DominateAll();
        }

        private void SocialCrawl_Click(object sender, RoutedEventArgs e)
        {
            SocialStatsExpander.Expanded -= SocialCrawl_Click;

            //if (!SocialStatsExpanderIsExpanded)
            //{
            SocialStatsExpander.Visibility = Visibility.Visible;
            SocialStatsExpander.IsExpanded = SocialStatsExpanderIsExpanded = true;
            host.Height = host.ActualHeight - 300;
            host.VerticalAlignment = VerticalAlignment.Bottom;
            //}
            //else
            //{
            //    SocialStatsExpander_Collapsed(null, null);
            //}

            SocialStatsExpander.Expanded += SocialCrawl_Click;
        }

        private void SocialStatsExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            SocialStatsExpander.Collapsed -= SocialStatsExpander_Collapsed;

            SocialStatsExpander.Visibility = Visibility.Collapsed;
            SocialStatsExpander.IsExpanded = SocialStatsExpanderIsExpanded = false;
            host.Height = host.ActualHeight + 300;
            host.VerticalAlignment = VerticalAlignment.Stretch;

            SocialStatsExpander.Collapsed += SocialStatsExpander_Collapsed;
        }

        private void BrowserControl_OnSelectedTabNavigate(string url)
        {
            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(url);
        }

        private void GloableWebView_Navigated(object sender, Gecko.GeckoNavigatedEventArgs e)
        {
            if (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.ReadyState == "loading") return;
            BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            string source = (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DocumentElement as Gecko.DOM.HTML.GeckoHTMLHtmlElement).OuterHtml;

            string accessTiken = source.Trim().Split(new string[] { "placeholder=\"Paste in an existing Access Token or click &quot;Get User Access Token" }, StringSplitOptions.None)[1];
            accessTiken = accessTiken.Split(new string[] { "value=" }, StringSplitOptions.None)[1];
            accessTiken = accessTiken.Remove(accessTiken.IndexOf(@">"));
            (ucGoViral.DataContext as GoViralVM).AccessToken = accessTiken.Replace("\"", "");
            (ucGoViral.DataContext as GoViralVM).AccessToken = (ucGoViral.DataContext as GoViralVM).AccessToken.Replace(" type=text", "");
        }

        private void REFRESHTOKEN_Button_Click(object sender, RoutedEventArgs e)
        {
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;
            BrowserControl_OnSelectedTabNavigate(Social.FACEBOOK_GRAPH_LINK);
        }

        private void ucGoViral_OnClickedSendSocialLink(string command, string link, string imgLink)
        {
            BrowseoFXManager.Instance.TabbrowserHandler.OpenWindow(Social.GetShareUrl(command, link, imgLink));
        }

        private void btnCloseFBConverseo_Click(object sender, RoutedEventArgs e)
        {
            if (fbconverseoExpanded)
                fbConverseoCollapse();
        }

        private void btnCloseexpFBSearch_Click(object sender, RoutedEventArgs e)
        {
            if(fbSearchExpanded)
                fbSreachCollapse();
        }

        private void btnCloseLSB_Click(object sender, RoutedEventArgs e)
        {
            if (lsbExpanded)
            {
                lsbCollapse();
            }
        }

        private void btnCloseSEO_Click(object sender, RoutedEventArgs e)
        {
            if (seoExpanded)
            {
                seoCollapse();
            }
        }

        private void ucSystemBrowSERLauncher_Loaded(object sender, RoutedEventArgs e)
        {
            if (ucSystemBrowSERLauncher.ViewModel == null)
            {
                ucSystemBrowSERLauncher.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfSystemBrowSERLauncher);
                ucSystemBrowSERLauncher.DataContext = ucSystemBrowSERLauncher.ViewModel;
            }
        }

        private void ucSharedSync_Loaded(object sender, RoutedEventArgs e)
        {
            if (ucSharedSync.ViewModel == null)
            {
                ucSharedSync.ViewModel = new SyncedProjectsVM(SyncedProjectsVM.TypeOfSEO);
                ucSharedSync.DataContext = ucSharedSync.ViewModel;
            }
        }

        private void btnSpin_Click(object sender, RoutedEventArgs e)
        {
            OnSpinClicked?.Invoke();
        }

        private void ucFBSearch_Loaded(object sender, RoutedEventArgs e)
        {
            ucFBSearch.DataContext = new FBSearchViewModel();
        }
        #endregion

        #region expanders

        #region fb search

        private void fbSreachExpand()
        {
            SetExpandSize();
            expFBSearch.Visibility = Visibility.Visible;
        }

        private void fbSreachCollapse()
        {
            CollapseExpanderBase();
            expFBSearch.Visibility = Visibility.Collapsed;
        }

        #endregion;

        #region fbConverseo

        private void fbConverseoExpand()
        {
            SetExpandSize();
            expFBConverSEO.Visibility = Visibility.Visible;
            fbconverseoExpanded = true;
        }

        private void fbConverseoCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            CollapseExpanderBase();
            expFBConverSEO.Visibility = Visibility.Collapsed;
            fbconverseoExpanded = false;
        }

        #endregion

        #region lsb

        private void lsbExpand()
        {
            SetExpandSize();
            expLSB.Visibility = Visibility.Visible;
            lsbExpanded = true;
        }


        private void lsbCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            CollapseExpanderBase();
            expLSB.Visibility = Visibility.Collapsed;
            lsbExpanded = false;
        }

        #endregion

        #region seo

        private void seoExpand()
        {
            SetExpandSize();
            expSEO.Visibility = Visibility.Visible;
            seoExpanded = true;
        }


        private void seoCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            CollapseExpanderBase();
            expSEO.Visibility = Visibility.Collapsed;
            seoExpanded = false;
        }

        #endregion

        #endregion
    }
}



//public void GotScreenCords(string message)
//{
//  //TODO:  throw new NotImplementedException();
//}

//public void SetBookmarksEvents(bool v)
//{
//    //TODO throw new NotImplementedException();
//}



//
//MacroManger managerForTab;
//public event Action OnInitializedMacros = delegate { };
//public async void OnOpenIAMacros()
//{
//    if (BrowseoFXManager.Instance.GloableWebView == null ||
//        BrowseoFXManager.Instance.GloableWebView.Widget == null ||
//        BrowseoFXManager.Instance.GloableWebView.Widget.BaseWindow == null) return;

//    host.Width = resizeBigger ? host.ActualWidth + 400 : host.ActualWidth - 400;
//    host.HorizontalAlignment = resizeBigger ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
//    resizeBigger = !resizeBigger;




//    MacroflyOut.Cursor = Cursors.Wait;
//    await MacroSettings.InitMacrosSettings();
//    if (MacroflyOut.DataContext == null || MacroflyOut.DataContext.GetType() != typeof(MacroManger))
//    {
//        if (managerForTab == null)
//        {
//            managerForTab = new MacroManger();
//            managerForTab.OnPlayMacro += ManagerForTab_OnPlayMacro;
//        }
//        MacroflyOut.DataContext = managerForTab;
//        try
//        {
//            await managerForTab.LoadIMacros(false);
//        }
//        catch { "Failed To Load Macro List".Show(); }

//        OnInitializedMacros();

//    }
//    MacroflyOut.Cursor = this.Cursor;


//    UpdateLayouts();
//}