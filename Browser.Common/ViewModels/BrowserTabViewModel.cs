using BrowserHost;
using BrowserHost.Models;
using BrowserHost.ViewModels;
using BrowserHost.Windows;
using DragDropListview;
using DragDropListview.Windows;
using Microsoft.Win32;
using Organiser.Common;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;
using System.Collections.Generic;
using System.Linq;
using Browser.Common.Windows;

namespace Browser.Common.ViewModels
{
    public class BrowserTabViewModel : ViewModelBase
    {
        #region events and commands 

        public event Action<string> OnCreateNewTab = delegate { };
        public event Action<string, string> OnCurateToPBN = delegate { };//content,link
        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };//link,type
        public event Action OnClickedSaveSession = delegate { };
        public event Action OnClickedDeleteSession = delegate { };
        public event Action OnClickedSaveSessionToBookmarks = delegate { };
        public event Action OnClickedReminders = delegate { };
        public event Action<string> OnShouldChangePropertyAddress = delegate { };
        public event Action OnRefreshSessionSettings = delegate { }; //javascriptEnabled,JavaEnabled
        public event Action<BrowserTabViewModel> OnRefreshTabSettings = delegate { }; //javascriptEnabled,JavaEnabled
        public event Action<string, string> OnSentForSeo = delegate { };//currenturlName,url


        public ICommand OpenCPCommand { get; set; }
        public ICommand FillListCommand { get; set; }
        public ICommand SaveSession { get; set; }
        public ICommand DeleteSession { get; set; }
        public ICommand SaveSessionToBMs { get; set; }
        public ICommand SettingsCTClick { get; set; }
        public ICommand BoockmarksCommand { get; set; }

        #endregion
        private System.Windows.Forms.UserControl webBrowser;
        public virtual System.Windows.Forms.UserControl Browser
        {
            get
            {
                //webBrowser.
                return webBrowser;
            }
            set { webBrowser = value; RaisePropertyChanged("Browser"); }
        }

        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                    wfh = new WindowsFormsHost() { Child = Browser };
                return wfh;
            }
            set { wfh = value; RaisePropertyChanged("WebBrowserHost"); }
        }

        #region browser statuses and messages

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; RaisePropertyChanged("IsLoading"); }
        }

        private string addressEditable;
        public string AddressEditable
        {
            get { return addressEditable; }
            set { addressEditable = value; RaisePropertyChanged("AddressEditable"); }
        }

        private string outputMessage;
        public string OutputMessage
        {
            get { return outputMessage; }
            set { outputMessage = value; RaisePropertyChanged("OutputMessage"); }
        }
        //HuverLink
        private string huverLink;
        public string HuverLink
        {
            get { return huverLink; }
            set { huverLink = value; RaisePropertyChanged("HuverLink"); }
        }

        private string statusMessage;
        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; RaisePropertyChanged("StatusMessage"); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value; RaisePropertyChanged("Title");
            }
        }
        #endregion

        private Thickness tabMargin;
        public Thickness TabMargin
        {
            get { return tabMargin; }
            set { tabMargin = value; RaisePropertyChanged("TabMargin"); }
        }

        private Thread CPWthread;
        private static int lastProfileIndex = 0; //last index for profile picker        

        public BrowserTabViewModel(string address, bool setTheBrowser = true)
        {
            IsLoading = true;

            SetSysDateEnabled = BrowserSettimgs.SetSysDateEnabled;
            DoNotTrackEnabled = BrowserSettimgs.DoNotTrackEnabled;

            if (setTheBrowser)
            {
                JavascriptEnabled = BrowserSettimgs.JavascriptEnabled;
                JavaEnabled = BrowserSettimgs.JavaEnabled;
                FlashEnabled = BrowserSettimgs.FlashEnabled;
                WebRTCEnabled = BrowserSettimgs.WebRTCEnabled;
                SetBrowser(address);
            }

            AddressEditable = address;

            //GoCommand = new DelegateCommand(Go);
            //BackCommand = new DelegateCommand(Back);
            //ForwardCommand = new DelegateCommand(Forward);
            //ReloadCommand = new DelegateCommand(Reload);
            //InjectCommand = new DelegateCommand(Inject);
            //SendToBrowserSocial = new RelayCommand(SendToSocialBrowserPopUp);

            OpenCPCommand = new DelegateCommand(OpenCP);
            SaveSession = new DelegateCommand(SaveSessionClicked);
            DeleteSession = new DelegateCommand(DeleteSessionClicked);
            SaveSessionToBMs = new DelegateCommand(SaveSessionToBMsClicked);
            SettingsCTClick = new RelayCommand(OnSettingsCTButtonClick);
            BoockmarksCommand = new RelayCommand(OnBoockmarksCommand_Raised);

            var version = "Brow·SEO";
            OutputMessage = version;

            Title = "New Tab";
            VisibleDtPbar = Visibility.Collapsed;
        }

        public virtual void SetBrowser(string addressEditable) { }

        public virtual void Dispose() { }

        public virtual void ChangeAddressEditable(string addy) { }

        public virtual void NavigateToSelectedSite(string text) { }


        public void RaiseOnAddedToGoViral(string link, string v, List<string> p)
        {
            OnAddedToGoViral(link,v,p);
        }

        public void RaisOnCreateNewTab(string huverLink)
        {
            OnCreateNewTab(huverLink);
        }

        public void RaiseOnCurateToPBN(string v, string addressEditable)
        {
            OnCurateToPBN(v, addressEditable);
        }

        public void RaiseOnSentForSeo(string sitename, string huverLink)
        {
            OnSentForSeo(sitename, huverLink);
        }

        public void RaiseOnShouldChangePropertyAddress(string address)
        {
            OnShouldChangePropertyAddress(address);
        }



        #region save open session tabs
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
        #endregion

        #region settings
        public List<string> AvailableTimeZones
        {
            get
            {
                List<string> avail = BrowserSettimgs.AvailableTimeZones;
                RaisePropertyChanged("SITimeZone");
                return avail;
            }
        }
        public int SITimeZone
        {
            get { return BrowserSettimgs.SITimeZone; }
            set { BrowserSettimgs.SITimeZone = value; RaisePropertyChanged("SITimeZone"); }
        }

        private Visibility visibleDtPbar;
        public Visibility VisibleDtPbar
        {
            get { return visibleDtPbar; }
            set { visibleDtPbar = value; RaisePropertyChanged("VisibleDtPbar"); }
        }

        private bool javascriptEnabled;
        public bool JavascriptEnabled
        {
            get { return javascriptEnabled; }
            set
            {
                javascriptEnabled = value;
                RaisePropertyChanged("JavascriptEnabled");

            }
        }

        private bool flashEnabled;
        public bool FlashEnabled
        {
            get { return flashEnabled; }
            set { flashEnabled = value; RaisePropertyChanged("FlashEnabled"); }
        }

        private bool javaEnabled;
        public bool JavaEnabled
        {
            get { return javaEnabled; }
            set
            {
                javaEnabled = value;
                RaisePropertyChanged("JavaEnabled");
            }
        }

        //DoNotTrackEnabled
        private bool doNotTrackEnabled;
        public bool DoNotTrackEnabled
        {
            get { return doNotTrackEnabled; }
            set
            {
                doNotTrackEnabled = value;
                RaisePropertyChanged("DoNotTrackEnabled");
            }
        }

        //WebRTCEnabled
        private bool webRTCEnabled;
        public bool WebRTCEnabled
        {
            get { return webRTCEnabled; }
            set
            {
                webRTCEnabled = value;
                RaisePropertyChanged("WebRTCEnabled");
            }
        }

        private bool setSysDateEnabled;
        public bool SetSysDateEnabled
        {
            get { return setSysDateEnabled; }
            set
            {
                setSysDateEnabled = value;
                RaisePropertyChanged("SetSysDateEnabled");

                Task.Factory.StartNew(() =>
                {
                    VisibleDtPbar = Visibility.Visible;

                    if (value)
                    {
                        DateAndTimeZone dtz = TimeHelper.GetTimeOfProxy(GloableProfData.PData.ProxyIP,
                            GloableProfData.PData.ProxyPort,
                            GloableProfData.PData.ProxyUsername,
                            GloableProfData.PData.ProxyPassword);
                        if (dtz != null)
                        {
                            for (int i = 0; i < AvailableTimeZones.Count; i++)
                            {
                                string displayName = AvailableTimeZones[i];
                                if (dtz.TimeZone.DisplayName == displayName)
                                {
                                    SITimeZone = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        TimeZoneInfo fromFile = TimeHelper.GetOldTZFromFile();
                        ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                        for (int i = 0; i < timeZones.Count; i++)
                        {
                            TimeZoneInfo tz = timeZones[i];
                            if (tz.DisplayName == fromFile.DisplayName)
                            {
                                SITimeZone = i;
                                break;
                            }
                        }
                    }

                    VisibleDtPbar = Visibility.Collapsed;
                });
            }
        }


        private Visibility setBrowserSetingsAvailable = Visibility.Visible;
        public Visibility SetBrowserSettingsAvailable
        {
            get { return setBrowserSetingsAvailable; }
            set { setBrowserSetingsAvailable = value; RaisePropertyChanged("SetBrowserSettingsAvailable"); }
        }

        private Visibility setWebrtcVisible = Visibility.Visible;
        public Visibility SetWebrtcVisible
        {
            get { return setWebrtcVisible; }
            set { setWebrtcVisible = value; RaisePropertyChanged("SetWebrtcVisible"); }
        }

        bool oldJavaCript, oldJava, oldFlash, oldSysDate, oldDnt, oldwebrtc;
        int oldTZSI = 0;

        internal void SettingsMenuOpen()
        {
            oldJavaCript = JavascriptEnabled;
            oldJava = JavaEnabled;
            oldFlash = FlashEnabled;
            oldSysDate = SetSysDateEnabled;
            oldDnt = DoNotTrackEnabled;
            oldTZSI = SITimeZone;
            oldwebrtc = WebRTCEnabled;
        }

        internal void SettingsMenuClosed()
        {
            JavascriptEnabled = oldJavaCript;
            JavaEnabled = oldJava;
            FlashEnabled = oldFlash;
            SetSysDateEnabled = oldSysDate;
            DoNotTrackEnabled = oldDnt;
            SITimeZone = oldTZSI;
            WebRTCEnabled = oldwebrtc;
        }

        private void OnSettingsCTButtonClick(object param)
        {
            switch (Convert.ToString(param))
            {
                case "TAB":
                    OnRefreshTabSettings(this);
                    break;

                case "SESSION":
                    BrowserSettimgs.SetSysDateEnabled = SetSysDateEnabled;
                    BrowserSettimgs.JavascriptEnabled = JavascriptEnabled;
                    BrowserSettimgs.JavaEnabled = JavaEnabled;
                    BrowserSettimgs.FlashEnabled = FlashEnabled;
                    BrowserSettimgs.DoNotTrackEnabled = DoNotTrackEnabled;
                    BrowserSettimgs.WebRTCEnabled = WebRTCEnabled;
                    if (BrowserSettimgs.SetSysDateEnabled)
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                TimeHelper.StartSetTimeAndZoneProcess(new DateAndTimeZone() { TimeZone = TimeZoneInfo.GetSystemTimeZones()[SITimeZone] });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OnRefreshSessionSettings();
                            });
                        });
                    }
                    else
                    {
                        if (oldSysDate) TimeHelper.SetOriginalTimeZonesFromFile();
                        OnRefreshSessionSettings();
                    }
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region bookmarks / reminders
        private int reminderCount;
        public int ReminderCount
        {
            get { return reminderCount; }
            set { reminderCount = value; RaisePropertyChanged("ReminderCount"); }
        }

        private void OnBoockmarksCommand_Raised(object param)
        {
            switch ((string)param)
            {
                case "REMINDERS":
                    RemindersVM rwVM = new RemindersVM();
                    rwVM.OnOpen += OnClickedReminders;
                    //TODO: rwVM.OnNavigate += (uri) => { WebBrowser.Navigate(uri); };
                    RemindersWindow rw = new RemindersWindow();
                    rw.DataContext = rwVM;
                    rw.Show();
                    Task.Factory.StartNew(() =>
                    {
                        List<string> jsonTaskList = MyFilesDatabase.GetRemindersText(GloableProfData.PData.ProjectName);
                        if (jsonTaskList != null) rwVM.FillReminders(jsonTaskList);
                    });
                    break;

                default:
                    break;
            }
        }
        #endregion

        private async void OpenCP()
        {
            PersonData profile = await getSelectedProfile();
            if (profile == null) return;

            if (CPWthread == null || !CPWthread.IsAlive)
            {
                CPWthread = new Thread(() =>
                {
                    CreateProjectWindow projWindow = new CreateProjectWindow();
                    projWindow.DataContext = profile;
                    if (!MyFilesDatabase.CanSeeProxys)
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
                    projWindow.ShowDialog();
                });

                CPWthread.SetApartmentState(ApartmentState.STA);
                CPWthread.Start();
            }
        }

        public async Task<PersonData> getSelectedProfile()
        {
            return await Task<PersonData>.Factory.StartNew(() =>
            {
                PersonData profile = ObjectCopier.DeepClone<PersonData>(GloableProfData.PData);
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
                                return profile;
                            }
                        }
                    }
                }
                catch { }

                if (MyFilesDatabase.HasMultipleProfiles(GloableProfData.PData.ProjectDir))
                {
                    SelectProfileWindow selectProfile = new SelectProfileWindow(GloableProfData.PData.ProjectName, GloableProfData.PData.ProjectDir, lastProfileIndex, "");
                    selectProfile.ShowDialog();
                    if (!selectProfile.OkClicked)
                    {
                        return null;
                    }
                    lastProfileIndex = selectProfile.cmProfiles.SelectedIndex;
                    profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
                }

                return profile;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ProjWindow_Closed(object sender, EventArgs e)
        {
            CPWthread = null;
        }
    }
}
