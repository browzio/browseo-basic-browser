using BrowserHost;
using BrowserHost.Models;
using BrowserHost.Windows;
using DragDropListview;
using DragDropListview.Windows;
using Organiser.Common;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
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
        public event Action OnAddedBookmark = delegate { };
        public event Action OnRefreshReminders = delegate { };
        public event Action OnRemindersChanged = delegate { };
        public event Action OnRefreshBookmarksList = delegate { };
        public event Action<string> OnCurateToPBN = delegate { };
        public event Action OnClickedSaveSession = delegate { };
        public event Action OnClickedDeleteSession = delegate { };
        public event Action OnClickedSaveSessionToBookmarks = delegate { };

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
            get 
            {
                //webBrowser.
                return webBrowser;
            }
            set { PropertyChanged.ChangeAndNotify(ref webBrowser, value, () => WebBrowser); }
        }

        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                     wfh = new WindowsFormsHost() { Child = WebBrowser };
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

        private Thread CPWthread;

        
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
        public ICommand OpenCPCommand { get; set; }
        public ICommand SaveSiteCommand { get; set; }
        public ICommand FillListCommand { get; set; }
        public ICommand DeleteSiteMenueItemCommand { get; set; }
        public ICommand NavigateListItemCommand { get; set; }
        public ICommand SaveSession { get; set; }
        public ICommand DeleteSession { get; set; }
        public ICommand SaveSessionToBMs { get; set; }

        public ICommand SendToBrowserSocial { get; set; }

        int lastProfileIndex = 0; //last index for profile picker for cp ontop

        public event PropertyChangedEventHandler PropertyChanged;

        public BrowserTabViewModel(string address)
        {
            IsLoading = true;

            SitesList = new ObservableCollection<SavedSite>();
            WebPageImages = new ObservableCollection<WebPageImg>();

            setBrowser(address);

            Address = address;
            AddressEditable = Address;

            GoCommand = new DelegateCommand(Go);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            ReloadCommand = new DelegateCommand(Reload);

            InjectCommand = new DelegateCommand(Inject);
            OpenCPCommand = new DelegateCommand(OpenCP);
            SaveSiteCommand = new DelegateCommand(SaveSite);
            FillListCommand = new DelegateCommand(FillList);
            DeleteSiteMenueItemCommand = new DelegateCommand(DeleteSiteMenueItem);
            NavigateListItemCommand = new DelegateCommand(NavigateToListItem);
            SaveSession = new DelegateCommand(SaveSessionClicked);
            DeleteSession = new DelegateCommand(DeleteSessionClicked);
            SaveSessionToBMs = new DelegateCommand(SaveSessionToBMsClicked);

            SendToBrowserSocial = new RelayCommand(SendToSocialBrowserPopUp);

            PropertyChanged += OnPropertyChanged;

            var version = "Brow·SEO";
            OutputMessage = version;

            Title = "New Tab";
        }

        private void SaveSessionToBMsClicked()
        {
            OnClickedSaveSessionToBookmarks();
        }

        private void DeleteSessionClicked()
        {
            OnClickedDeleteSession();
        }

        private void SaveSessionClicked()
        {
            OnClickedSaveSession();
        }

        private void SendToSocialBrowserPopUp(object param)
        {
            string fullUrl = "";
            bool wasPin = false;
            string shareType = (string)param;
            switch (shareType)
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
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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

            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Share From Browser " + shareType);

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
            bfss.Show();
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
            WebBrowser.OnCurateToPBN += WebBrowser_OnCurateToPBN;
        }

        private void WebBrowser_OnCurateToPBN(string highlighttext)
        {
            OnCurateToPBN(highlighttext);
        }

        void WebBrowser_OnBrowserStatusChanged(string oMessage)
        {
            OutputMessage = oMessage;
        }

        void WebBrowser_OnCreateNewTab(string url, bool showNewTab)
        {
            OnCreateNewTab(url, showNewTab);
        }

        public event Action<string> OnShouldChangePropertyAddress = delegate { };
        void WebBrowser_OnBrowserAddressChanged(string address)
        {
            dontGo = true;
            OnShouldChangePropertyAddress(address);
            //AddressEditable = address;
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Address Changed " + address);
        }

        public void ChangeAddressEditable(string address)
        {
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
            //Keyboard.ClearFocus();
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

        private void OpenCP()
        {
            PersonData profile = BrowserInit.pData.Clone() as PersonData;
            bool usingImport = false;
            try
            {
                if (DragDropMainViewModel.Instance.FoldersAndSitesList != null && DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                {
                    string urltoCheck = AddressEditable.Substring(AddressEditable.IndexOf('.') + 1);
                    if (urltoCheck.Contains("."))
                        urltoCheck = urltoCheck.Split('.')[0];
                    foreach (Bookmark b in DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].Sites)
                    {
                        if (!b.IsImported) continue;
                        string blinkToChek = b.Link.Substring(b.Link.IndexOf('.') + 1);
                        if (blinkToChek.Contains("."))
                            blinkToChek = blinkToChek.Split('.')[0];
                        if (urltoCheck.Contains(blinkToChek))
                        {
                            profile.Username = b.Username;
                            profile.Email = b.Email;
                            profile.Password = b.Password;
                            usingImport = true;
                            break;
                        }
                    }
                }
            }
            catch { }

            if (!usingImport && MyFilesDatabase.HasMultipleProfiles(BrowserInit.pData.ProjectDIr))
            {
                SelectProfileWindow selectProfile = new SelectProfileWindow(BrowserInit.pData.ProjectName, BrowserInit.pData.ProjectDIr, lastProfileIndex, "");
                //selectProfile.Closed += selectProfile_Closed;
                selectProfile.ShowDialog();
                if (!selectProfile.OkClicked)
                {
                    return;
                }
                lastProfileIndex = selectProfile.cmProfiles.SelectedIndex;
                profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
            }

            if (CPWthread == null || !CPWthread.IsAlive)
            {
                CPWthread = new Thread(() =>
                {
                    CreateProjectWindow projWindow = new CreateProjectWindow();
                    projWindow.DataContext = profile;
                    projWindow.btnDelete.Visibility = System.Windows.Visibility.Hidden;
                    if (!CreateProjectWindow.CanSeeProxys)
                    {
                        projWindow.tbProxys.Visibility = System.Windows.Visibility.Collapsed;
                        projWindow.dpProxys.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    projWindow.projName.Text = profile.ProfileName;
                    projWindow.Topmost = true;
                    projWindow.WindowStyle = System.Windows.WindowStyle.None;
                    projWindow.AllowsTransparency = true;
                    projWindow.Opacity = 0.9;
                    projWindow.grdinfo.Opacity = 0.9;
                    projWindow.tbbutton.Text = "Close";
                    projWindow.IsReadOnly = true;
                    projWindow.cbSex.IsEnabled = projWindow.spBirth.IsEnabled = projWindow.spPBN.IsEnabled =
                    projWindow.spMoney.IsEnabled = projWindow.cmbMoney.IsEnabled = projWindow.cmbPbn.IsEnabled = false;
                    projWindow.Closed += ProjWindow_Closed;
                    projWindow.Show();

                    System.Windows.Threading.Dispatcher.Run();
                });

                CPWthread.SetApartmentState(ApartmentState.STA);
                CPWthread.Start();
            }
        }

        private void ProjWindow_Closed(object sender, EventArgs e)
        {
            CPWthread = null;
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
          //  SitesList.Clear();
           // SitesList = PersonDataFileReader.GetSavedSites(SitesList, BrowserInit.pData.ProjectName);
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

        public void RaiseAddedBookmark()
        {
            OnAddedBookmark();
        }

        internal void RaiseRefreshBookmarksList()
        {
            OnRefreshBookmarksList();
        }

        internal void RaiseRemindersChanged()
        {
            OnRemindersChanged();
        }

        internal void RaiseRefreshRemindersList()
        {
            OnRefreshReminders();
        }
    }
}
