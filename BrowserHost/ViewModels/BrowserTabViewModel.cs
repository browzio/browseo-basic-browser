using BrowserHost;
using BrowserHost.Models;
using BrowserHost.Windows;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using WpfCefDynamBrowser.Mvvm;
using WpfCefDynamBrowser.Views;
using Xilium.CefGlue.Client;

namespace WpfCefDynamBrowser.ViewModels
{
    public class BrowserTabViewModel : INotifyPropertyChanged
    {
        public event Action<string, bool> OnCreateNewTab = delegate { };

        private Thickness tabMargin;
        public Thickness TabMargin
        {
            get { return tabMargin; }
            set { PropertyChanged.ChangeAndNotify(ref tabMargin, value, () => TabMargin); }
        }

        bool dontGo;
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                if (WebBrowser != null && !dontGo)
                {
                    WebBrowser.Navigate(address);
                }
                PropertyChanged.ChangeAndNotify(ref address, value, () => Address);
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { PropertyChanged.ChangeAndNotify(ref isLoading, value, () => IsLoading); }
        }

        private string addressEditable;
        public string AddressEditable
        {
            get { return addressEditable; }
            set { PropertyChanged.ChangeAndNotify(ref addressEditable, value, () => AddressEditable); }
        }

        private string outputMessage;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { PropertyChanged.ChangeAndNotify(ref outputMessage, value, () => OutputMessage); }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { PropertyChanged.ChangeAndNotify(ref statusMessage, value, () => StatusMessage); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set 
            { 
                PropertyChanged.ChangeAndNotify(ref title, value, () => Title); 
            }
        }

        private BrowserCntrl webBrowser;
        public BrowserCntrl WebBrowser
        {
            get { return webBrowser; }
            set { PropertyChanged.ChangeAndNotify(ref webBrowser, value, () => WebBrowser); }
        }

        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                WindowsFormsHost wfh = new WindowsFormsHost() { Child = WebBrowser };
                return wfh;
            }
            // set { PropertyChanged.ChangeAndNotify(ref webBrowserHost, value, () => WebBrowserHost); }
        }

        private ObservableCollection<SavedSite> siteslist;
        public ObservableCollection<SavedSite> SitesList
        {
            get { return siteslist; }
            set { siteslist = value; }
        }

        private int siitelist;
        public int SISitesList
        {
            get { return siitelist; }
            set { siitelist = value; }
        }


        private ObservableCollection<WebPageImg> webPageImages;
        public ObservableCollection<WebPageImg> WebPageImages
        {
            get { return webPageImages; }
            set { webPageImages = value; }
        }
        private int sLImageLink;
        public int SLImageLink
        {
            get { return sLImageLink; }
            set { sLImageLink = value; }
        }

        
        private object evaluateJavaScriptResult;

        public object EvaluateJavaScriptResult
        {
            get { return evaluateJavaScriptResult; }
            set { PropertyChanged.ChangeAndNotify(ref evaluateJavaScriptResult, value, () => EvaluateJavaScriptResult); }
        }

        
        public ICommand GoCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand ReloadCommand { get; set; }

        public ICommand InjectCommand { get; set; }
        public ICommand SaveSiteCommand { get; set; }
        public ICommand FillListCommand { get; set; }
        public ICommand DeleteSiteMenueItemCommand { get; set; }
        public ICommand NavigateListItemCommand { get; set; }

        public ICommand SendToBrowserSocial { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BrowserTabViewModel(string address)
        {
            IsLoading = true;

            SitesList = new ObservableCollection<SavedSite>();
            WebPageImages = new ObservableCollection<WebPageImg>();

            setBrowser(address);

            Address = address;
            AddressEditable = Address;

            GoCommand = new DelegateCommand(Go, () => !String.IsNullOrWhiteSpace(Address));
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            ReloadCommand = new DelegateCommand(Reload);

            InjectCommand = new DelegateCommand(Inject);
            SaveSiteCommand = new DelegateCommand(SaveSite);
            FillListCommand = new DelegateCommand(FillList);
            DeleteSiteMenueItemCommand = new DelegateCommand(DeleteSiteMenueItem);
            NavigateListItemCommand = new DelegateCommand(NavigateToListItem);

            SendToBrowserSocial = new RelayCommand(SendToSocialBrowserPopUp);

            PropertyChanged += OnPropertyChanged;

            var version = "browe·seo";
            OutputMessage = version;

            Title = "New Tab";
        }

        private void SendToSocialBrowserPopUp(object param)
        {
            string fullUrl = "";
            bool wasPin = false;
            switch ((string)param)
            {
                case Social.SOCIALTYPE_fb:
                    fullUrl = Social.SHARELINK_facebook + AddressEditable;
                    break;

                case Social.SOCIALTYPE_gp:
                    fullUrl = Social.SHARELINK_googleplus + AddressEditable;
                    break;

                case Social.SOCIALTYPE_digg:
                    fullUrl = Social.SHARELINK_digg + AddressEditable;
                    break;

                case Social.SOCIALTYPE_pin:
                    wasPin = true;
                    var visitor = new SourceVisitor(text =>
                    {
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            WebPageImages.Clear();
                            foreach (Match m in Regex.Matches(text, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                            {
                                string src = m.Groups[1].Value;
                                WebPageImages.Add(new WebPageImg()
                                {
                                    ImgUrl = src,
                                    WebUrl = AddressEditable
                                });
                            }

                            if (WebPageImages.Count > 0)
                            {
                                ChoosePinterestImageWindow cpiw = new ChoosePinterestImageWindow();
                                cpiw.DataContext = this;
                                cpiw.ShowDialog();
                                if (cpiw.OkClicked)
                                {
                                    launchSharePopUP(Social.SHARELINK_pintrest + AddressEditable + "&media=" + WebPageImages[SLImageLink].ImgUrl);
                                }
                            }
                        }));
                    });
                    WebBrowser.CBrowser.Browser.GetMainFrame().GetSource(visitor);
                    break;

                case Social.SOCIALTYPE_reddit:
                    fullUrl = Social.SHARELINK_reddit + AddressEditable;
                    break;

                case Social.SOCIALTYPE_stumble:
                    fullUrl = Social.SHARELINK_stumbleupon + AddressEditable;
                    break;

                case Social.SOCIALTYPE_tumblr:
                    fullUrl = Social.SHARELINK_tumblr + AddressEditable;
                    break;

                case Social.SOCIALTYPE_twit:
                    fullUrl = Social.SHARELINK_twitter + AddressEditable;
                    break;

                case Social.SOCIALTYPE_wp:
                    AddLinkDataWindow alw = new AddLinkDataWindow();
                    alw.tblockInfo.Text = "Enter wordpress site (browzio.wordpress.com):";
                    alw.ShowDialog();
                    if (!alw.OkClicked) return;
                    string wpUrl = alw.tbInputText.Text;
                    if (!wpUrl.Contains("http"))
                        wpUrl = "https://" + wpUrl;
                    fullUrl = wpUrl + Social.SHARELINK_wordpress + AddressEditable;
                    break;

                default:
                    fullUrl = AddressEditable;
                    break;
            }

            if (!wasPin)
            {
                launchSharePopUP(fullUrl);
            }
        }

        private void launchSharePopUP(string fullUrl)
        {
            BrowserForSocialShare bfss = new BrowserForSocialShare();
            bfss.Text = "Loading... " + AddressEditable;
            bfss.browserCntrl1.init(fullUrl);
            bfss.ShowDialog();
        }

        private sealed class SourceVisitor : Xilium.CefGlue.CefStringVisitor
        {
            private readonly Action<string> _callback;

            public SourceVisitor(Action<string> callback)
            {
                _callback = callback;
            }

            protected override void Visit(string value)
            {
                _callback(value);
            }
        }

        private void setBrowser(string address)
        {
            WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
            WebBrowser.init(address);

            WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            WebBrowser.OnBrowserMessageChanged += WebBrowser_OnBrowserMessageChanged;
            WebBrowser.OnBrowserTitleChanged += WebBrowser_OnBrowserTitleChanged;
            WebBrowser.OnBrowserAddressChanged += WebBrowser_OnBrowserAddressChanged;
            WebBrowser.OnBrowserStatusChanged += WebBrowser_OnBrowserStatusChanged;
            WebBrowser.OnCreateNewTab += WebBrowser_OnCreateNewTab;
        }

        void WebBrowser_OnBrowserStatusChanged(string oMessage)
        {
            OutputMessage = oMessage;
        }

        void WebBrowser_OnCreateNewTab(string url, bool showNewTab)
        {
            OnCreateNewTab(url, showNewTab);
        }

        void WebBrowser_OnBrowserAddressChanged(string address)
        {
            dontGo = true;
            AddressEditable = address;
        }

        void WebBrowser_OnBrowserTitleChanged(string ttl)
        {
            Title = ttl;
        }

        void WebBrowser_OnBrowserMessageChanged(string oMessage)
        {
            OutputMessage = oMessage;
        }

        void WebBrowser_OnBrowserLoadingChanged(bool loading)
        {
            IsLoading = loading;
            if (loading)
                StatusMessage = "Loading...";
            else
                StatusMessage = "Done";
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    AddressEditable = Address;
                    break;

                //case "WebBrowser":
                  //  if (WebBrowser != null)
                   // {
                        // TODO: This is a bit of a hack. It would be nicer/cleaner to give the webBrowser focus in the Go()
                        // TODO: method, but it seems like "something" gets messed up (= doesn't work correctly) if we give it
                        // TODO: focus "too early" in the loading process...
                       // WebBrowser.Browser..Loaded += delegate { Application.Current.Dispatcher.BeginInvoke((Action)(() => webBrowser.Focus())); };
                    //}

                    //break;
            }
        }

        private void Go()
        {
            dontGo = false;
            Address = AddressEditable;

            // Part of the Focus hack further described in the OnPropertyChanged() method...
            Keyboard.ClearFocus();
        }

        private void Reload()
        {
            WebBrowser.Reload();
        }

        private void Forward()
        {
            WebBrowser.Forward();
        }

        private void Back()
        {
            WebBrowser.Back();
        }

        private void Inject()
        {
            WebBrowser.InjectData();
        }

        private void NavigateToListItem()
        {
            WebBrowser.Navigate(SitesList[SISitesList].Site);
        }

        private void DeleteSiteMenueItem()
        {
            SitesList.Remove(SitesList[SISitesList]);
            PersonDataFileReader.DeleteSite(SitesList, BrowserInit.pData.ProjectName);
        }

        private void FillList()
        {
            SitesList.Clear();
            SitesList = PersonDataFileReader.GetSavedSites(SitesList, BrowserInit.pData.ProjectName);
        }

        private void SaveSite()
        {
            try
            {
                ObservableCollection<SavedSite> SitesList = new ObservableCollection<SavedSite>();
                SitesList = PersonDataFileReader.GetSavedSites(SitesList, BrowserInit.pData.ProjectName);
                foreach (SavedSite site in SitesList)
                {
                    if (AddressEditable == site.Site)
                        return;
                }
                PersonDataFileReader.SaveSite(AddressEditable, BrowserInit.pData.ProjectName);
                SitesList = PersonDataFileReader.GetSavedSites(SitesList, BrowserInit.pData.ProjectName);
            }
            catch { }
        }


        public void NavigateToSelectedSite(string site)
        {
            WebBrowser.Navigate(site);
            //WebBrowser.Navigate(SitesList[SISitesList].Site);
        }
    }
}
