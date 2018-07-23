using BrowseoFX_WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using BrowseoFX_WPF.Core.Services.Browser.Crawlers;
using Organiser.Common.Classes;

namespace BrowseoFX_WPF.Controls
{
    /// <summary>
    /// Interaction logic for CmdBrowserControl.xaml
    /// </summary>
    public partial class CmdBrowserControl : UserControl, IListenToFXManager
    {
        public Action<string, string, List<string>> OnAddedToGoViral;
        public Action OnGloableWebView_Loaded;

        bool resizeBigger = false;

        public CmdBrowserControl()
        {
            InitializeComponent();

            this.Loaded += BrowserControl_Loaded;
            this.SizeChanged += BrowserControl_SizeChanged;
        }

        private void BrowserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (host.HorizontalAlignment == HorizontalAlignment.Left)
            {
                host.Width = this.ActualWidth + 20;
                host.HorizontalAlignment = HorizontalAlignment.Stretch;
                if (resizeBigger) host.Width -= 400;
                host.HorizontalAlignment = HorizontalAlignment.Left;
            }

            if (SocialStatsExpander.IsExpanded && host.ActualHeight > 350)
            {
                host.Height = host.ActualHeight - 300;
            }
            
        }

        private async void BrowserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BrowserControl_Loaded;


            BrowseoFXManager.Instance.FxMAnagerListenerNotifyer = this;
            BrowseoFXManager.Instance.PanelUIHandler.NoFB = true;
            await BrowseoFXManager.Instance.Init();
            host.Children.Add(BrowseoFXManager.Instance.GloableWebView);
            BrowseoFXManager.Instance.GloableWebView.Loaded += GloableWebView_Loaded;
        }



        private Action EmptyDelegate = delegate () { };
        private void GloableWebView_Loaded(object sender, RoutedEventArgs e)
        {
            BrowseoFXManager.Instance.GloableWebView.Loaded -= GloableWebView_Loaded;

            UpdateLayouts();

            host.Width = this.ActualWidth + 17;
            host.HorizontalAlignment = HorizontalAlignment.Left;

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            host.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
            BrowseoFXManager.Instance.GloableWebView.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);

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

        public void OnOpenFBConverseo()
        {
            //throw new NotImplementedException();
        }

        public void OnOpenLSB()
        {
           /// throw new NotImplementedException();
        }

        public void OnOpenSEO()
        {
           // throw new NotImplementedException();
        }
    }
}
