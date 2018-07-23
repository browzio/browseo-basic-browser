using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Core.Services.Browser.Crawlers;
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

        bool resizeBigger = false;

        public BrowserControl()
        {
            InitializeComponent();

            this.Loaded += BrowserControl_Loaded;
            this.SizeChanged += BrowserControl_SizeChanged;
        }

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

            if (fbconverseoExpanded && !seoExpanded && !lsbExpanded)
            {
                fbConverseoExpand();
            }
            else if (!fbconverseoExpanded && !seoExpanded && lsbExpanded)
            {
                lsbExpand();
            }
            else if (!fbconverseoExpanded && seoExpanded && !lsbExpanded)
            {
                seoExpand();
            }
            else
            {
                AirspaceHost.Width = this.ActualWidth;
            }
        }

        private void SetExpandSize()
        {
            AirspaceHost.Width = this.ActualWidth - 505;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Left;
            host.Width = this.ActualWidth - 485;
            host.HorizontalAlignment = HorizontalAlignment.Left;
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

        private void TabbrowserHandler_OnAddedToGoViral(string arg1, string arg2, List<string> linksToReturn)
        {
            OnAddedToGoViral?.Invoke(arg1, arg2, linksToReturn);
        }

        private void UpdateLayouts()
        {
            UpdateLayout();
            host.UpdateLayout();
            BrowseoFXManager.Instance.GloableWebView.UpdateLayout();
            BrowseoFXManager.Instance.GloableWebView.Widget.BaseWindow.Instance.Repaint(true);
        }

        //public void GotScreenCords(string message)
        //{
        //  //TODO:  throw new NotImplementedException();
        //}

        public void CloseAllTabs()
        {
            BrowseoFXManager.Instance.Shutdown();
        }

        //public void SetBookmarksEvents(bool v)
        //{
        //    //TODO throw new NotImplementedException();
        //}

        public void SearchFor(string query)
        {
            //    BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.LoadHtml("", query);
            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(query);
        }

        public void LaunchNewWindow(string link, string rssLink)
        {

            BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.Open(link, rssLink, "resizable,scrollbars,status");
        }

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

        private async void ManagerForTab_OnPlayMacro(MacroManger manger, IIMPlayType type, int loop)
        {
            //if (DataContext is BrowserTabViewModel)
            //{
            //    await (DataContext as BrowserTabViewModel).OnPlayMacro(manger, type, loop);
            //}
        }


        bool SocialStatsExpanderIsExpanded;
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
            collapseExpander();
            SocialStatsExpander.Collapsed += SocialStatsExpander_Collapsed;
        }

        void collapseExpander()
        {
            SocialStatsExpander.Visibility = Visibility.Collapsed;
            SocialStatsExpander.IsExpanded = SocialStatsExpanderIsExpanded = false;
            host.Height = host.ActualHeight + 300;
            host.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public void OnOpenIAMacros()
        {
           // throw new NotImplementedException();
        }

        bool fbconverseoExpanded = false;
        public void OnOpenFBConverseo()
        {
            (ucGoViral.DataContext as GoViralVM).OnSelectedTabNavigate -= BrowserControl_OnSelectedTabNavigate;
            (ucGoViral.DataContext as GoViralVM).OnSelectedTabNavigate += BrowserControl_OnSelectedTabNavigate;
            
            (ucGoViral.DataContext as GoViralVM).OnDominateAll -= GoViralVM_OnDominateAll; 
            (ucGoViral.DataContext as GoViralVM).OnDominateAll += GoViralVM_OnDominateAll;

            if (lsbExpanded)
            {
                lsbCollapse();
            }
            if (seoExpanded)
            {
                seoCollapse();
            }

            if (fbconverseoExpanded)
            {
                fbConverseoCollapse();
            }
            else
            {
                fbConverseoExpand();
            }
        }

        private void GoViralVM_OnDominateAll()
        {
            BrowseoFXManager.Instance.TabbrowserHandler.DominateAll();
        }

        private void fbConverseoExpand()
        {
            SetExpandSize();
            expFBConverSEO.Visibility = Visibility.Visible;
            fbconverseoExpanded = true;
        }

        private void fbConverseoCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            AirspaceHost.Width = this.ActualWidth;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            host.Width = this.ActualWidth + 15;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
            expFBConverSEO.Visibility = Visibility.Collapsed;
            fbconverseoExpanded = false;
            
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

        public void AsyncAddLinkToList(string link, string type, List<string> multiLinks, bool showLinksWindow)
        {
            //GoViralVM goViralVM; AsyncAddLinkToList
            (ucGoViral.DataContext as GoViralVM).AsyncAddLinkToList(link, type, multiLinks, showLinksWindow);
        }
        
        private void REFRESHTOKEN_Button_Click(object sender, RoutedEventArgs e)
        {
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;
            BrowserControl_OnSelectedTabNavigate(Social.FACEBOOK_GRAPH_LINK);
        }

        private void btnCloseFBConverseo_Click(object sender, RoutedEventArgs e)
        {
            fbConverseoCollapse();
        }

        private void ucGoViral_OnClickedSendSocialLink(string command, string link, string imgLink)
        {
            BrowseoFXManager.Instance.TabbrowserHandler.OpenWindow(Social.GetShareUrl(command,link,imgLink));
        }


        bool lsbExpanded = false;
        public void OnOpenLSB()
        {
            if (fbconverseoExpanded)
            {
                fbConverseoCollapse();
            }
            if (seoExpanded)
            {
                seoCollapse();
            }

            if (lsbExpanded)
            {
                lsbCollapse();
            }
            else
            {
               lsbExpand();
            }
        }

        private void lsbExpand()
        {
            SetExpandSize();
            expLSB.Visibility = Visibility.Visible;
            lsbExpanded = true;
        }


        private void lsbCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            AirspaceHost.Width = this.ActualWidth;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            host.Width = this.ActualWidth + 15;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
            expLSB.Visibility = Visibility.Collapsed;
            lsbExpanded = false;
        }

        private void btnCloseLSB_Click(object sender, RoutedEventArgs e)
        {
            if (lsbExpanded)
            {
                lsbCollapse();
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


        bool seoExpanded = false;
        public void OnOpenSEO()
        {
            if (fbconverseoExpanded)
            {
                fbConverseoCollapse();
            }
            if (lsbExpanded)
            {
                lsbCollapse();
            }

            if (seoExpanded)
            {
                seoCollapse();
            }
            else
            {
                seoExpand();
            }
        }


        private void seoExpand()
        {
            SetExpandSize();
            expSEO.Visibility = Visibility.Visible;
            seoExpanded = true;
        }


        private void seoCollapse()
        {
            //todo BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            AirspaceHost.Width = this.ActualWidth;
            AirspaceHost.HorizontalAlignment = HorizontalAlignment.Stretch;
            host.Width = this.ActualWidth + 15;
            host.HorizontalAlignment = HorizontalAlignment.Stretch;
            expSEO.Visibility = Visibility.Collapsed;
            seoExpanded = false;
        }

        private void btnCloseSEO_Click(object sender, RoutedEventArgs e)
        {
            if (seoExpanded)
            {
                seoCollapse();
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
    }
}
