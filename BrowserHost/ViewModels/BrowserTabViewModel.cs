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
using WpfCefDynamBrowser.Views;
using Xilium.CefGlue;
using Xilium.CefGlue.Client;
using System.Collections.Generic;
using System.Linq;

namespace WpfCefDynamBrowser.ViewModels
{
    public class BrowserTabViewModel : Organiser.Common.Classes.ViewModelBase
    {
        #region events and commands 

        public event Action<string> OnCreateNewTab = delegate { }; 
        public event Action<string,string> OnCurateToPBN = delegate { };//content,link
        public event Action<string,string, List<string>> OnAddedToGoViral = delegate { };//link,type
        public event Action OnClickedSaveSession = delegate { };
        public event Action OnClickedDeleteSession = delegate { };
        public event Action OnClickedSaveSessionToBookmarks = delegate { };
        public event Action<string> OnShouldChangePropertyAddress = delegate { }; 
        public event Action OnRefreshSessionSettings = delegate { }; //javascriptEnabled,JavaEnabled
        public event Action<BrowserTabViewModel> OnRefreshTabSettings = delegate { }; //javascriptEnabled,JavaEnabled
        public event Action<string,string> OnSentForSeo = delegate { };//currenturlName,url

        public ICommand GoCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand InjectCommand { get; set; }
        public ICommand OpenCPCommand { get; set; }
        public ICommand FillListCommand { get; set; }
        public ICommand SaveSession { get; set; }
        public ICommand DeleteSession { get; set; }
        public ICommand SaveSessionToBMs { get; set; }
        public ICommand SendToBrowserSocial { get; set; }
        public ICommand SettingsCTClick { get; set; }

        #endregion

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

        #region browser hosting
        private BrowserCntrl webBrowser;
        public BrowserCntrl WebBrowser
        {
            get 
            {
                //webBrowser.
                return webBrowser;
            }
            set { webBrowser = value; RaisePropertyChanged("WebBrowser"); }
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
             set { wfh = value; RaisePropertyChanged("WebBrowserHost"); }
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
                SetBrowser(address);
            }

            AddressEditable = address;

            GoCommand = new DelegateCommand(Go);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            ReloadCommand = new DelegateCommand(Reload);
            InjectCommand = new DelegateCommand(Inject);
            OpenCPCommand = new DelegateCommand(OpenCP);
            SaveSession = new DelegateCommand(SaveSessionClicked);
            DeleteSession = new DelegateCommand(DeleteSessionClicked);
            SaveSessionToBMs = new DelegateCommand(SaveSessionToBMsClicked);  
            SendToBrowserSocial = new RelayCommand(SendToSocialBrowserPopUp);
            SettingsCTClick = new RelayCommand(OnSettingsCTButtonClick);
                                                      
            var version = "Brow·SEO";
            OutputMessage = version;

            Title = "New Tab";
            VisibleDtPbar = Visibility.Collapsed;
        }

        #region browser
        public void SetBrowser(string address)
        {
            WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
            WebBrowser.init(address,
                JavascriptEnabled ? CefState.Enabled : CefState.Disabled,
                JavaEnabled ? CefState.Enabled : CefState.Disabled,
                FlashEnabled ? CefState.Enabled : CefState.Disabled);

            WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            WebBrowser.OnBrowserMessageChanged += WebBrowser_OnBrowserMessageChanged;
            WebBrowser.OnBrowserTitleChanged += WebBrowser_OnBrowserTitleChanged;
            WebBrowser.OnBrowserAddressChanged += WebBrowser_OnBrowserAddressChanged;
            WebBrowser.OnBrowserStatusChanged += WebBrowser_OnBrowserStatusChanged;
            WebBrowser.OnCreateNewTab += WebBrowser_OnCreateNewTab;   
            WebBrowser.OnBrowserContextMenuClicked += WebBrowser_OnBrowserContextMenuClicked;
        }
        //class DemoClient : CefClient { }
        private void WebBrowser_OnBrowserContextMenuClicked(int contextMenueItemID)
        {
            switch (contextMenueItemID)
            {
                case 333:
                    if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
                    {
                        string sitename = AddressEditable.Replace("http://","");
                        sitename = sitename.Replace("https://", "");
                        sitename = sitename.Replace("www.", "");
                        if (sitename.Contains("."))
                        {
                            sitename = sitename.Remove(sitename.IndexOf("."));
                        }
                        OnSentForSeo(sitename, HuverLink);
                    }
                    //WebBrowser.CBrowser.Browser.GetHost().SendFocusEvent
                    // WebBrowser.CBrowser.Browser.GetHost().ShowDevTools(CefWindowInfo.Create(), new DemoClient(), new CefBrowserSettings() { }, new CefPoint(110,110));
                    break;
                #region curate
                case 666:
                case 222:
                    try
                    {
                        if (WebBrowser.CBrowser.Browser.GetMainFrame() == null || WebBrowser.CBrowser.Browser.GetMainFrame().Url == null) return;

                        string dir = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempHTML");
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        string file = Path.Combine(dir, "html.txt");
                        if (File.Exists(file)) File.Delete(file);

                        //the javascript
                        string jsForExecution = "var range = window.getSelection().getRangeAt(0)," +
                                                "content = range.extractContents()," +
                                                "span = document.createElement('SPAN');" +
                                                "span.appendChild(content);" +
                                                "var htmltext = span.innerHTML.toString();" +
                                                "range.insertNode(span);" +
                                                "nativeImplementation(htmltext);";
                        WebBrowser.CBrowser.Browser.GetMainFrame().ExecuteJavaScript(jsForExecution, WebBrowser.CBrowser.Browser.GetMainFrame().Url, 0);



                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            while (!File.Exists(file))
                            {
                                System.Threading.Thread.Sleep(150);
                            }

                            if (contextMenueItemID == 666)
                            {
                                OnCurateToPBN(File.ReadAllText(file), AddressEditable);
                            }
                            else
                            {
                                string thecontent = "<blockquote>" + File.ReadAllText(file) + "<br />";
                                if (!string.IsNullOrEmpty(AddressEditable) && !string.IsNullOrWhiteSpace(AddressEditable))
                                    thecontent += "<a href=\"" + AddressEditable + " \" > " + AddressEditable + " </a>";
                                thecontent += "</blockquote>";
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    MyFilesDatabase.SetClipboardText(thecontent);
                                });
                            }
                            File.Delete(file);
                        });

                    }
                    catch (Exception ex)
                    {

                    }
                    break;
                #endregion

                #region newTab
                case 999:
                    if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
                    {
                        OnCreateNewTab(HuverLink);
                    }
                    break;
                #endregion

                #region copy link
                case 888:
                    if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
                    {
                        MyFilesDatabase.SetClipboardText(HuverLink);
                    }
                    break;
                #endregion

                #region imageDownload
                case 777:
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        string imgUrl = "";
                        if (WebBrowser.CBrowser.Browser.GetMainFrame() != null && WebBrowser.CBrowser.Browser.GetMainFrame().Url != null)
                        {
                            string url = WebBrowser.CBrowser.Browser.GetMainFrame().Url;

                            imgUrl = getImageUrl(url);
                        }

                        if (imgUrl == "")
                        {
                            if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
                            {
                                string url = HuverLink;
                                if (url.ToLower().Contains("imgurl=") && url.ToLower().Contains("google."))
                                {
                                    url = url.Split(new string[] { "imgurl=" }, StringSplitOptions.None)[1];
                                    if (url.Contains("%253"))
                                    {
                                        url = url.Remove(url.IndexOf("%253"));
                                    }
                                    if (url.Contains("&imgrefurl"))
                                    {
                                        url = url.Remove(url.IndexOf("&imgrefurl"));
                                    }
                                }

                                if (url.Contains("%3A"))
                                {
                                    url = url.Replace("%3A", ":");
                                }
                                if (url.Contains("%2F"))
                                {
                                    url = url.Replace("%2F", "/");
                                }
                                if (url.Contains("%2520"))
                                {
                                    url = url.Replace("%2520", " ");
                                }
                                if (url.Contains("%20"))
                                {
                                    url = url.Replace("%20", " ");
                                }
                                imgUrl = getImageUrl(url);
                            }
                        }

                        if (imgUrl == "")
                        {
                            MessageBox.Show("No image found to download. Make sure the mouse is over a image and try again, or open the image as a tab and then download it.");
                            return;
                        }

                        MyFilesDatabase.DownloadImage(imgUrl);

                        IsLoading = false;
                        StatusMessage = "Done";
                    });
                    break;
                #endregion

                #region go viral
                case 555:
                    if ((string.IsNullOrEmpty(HuverLink) && string.IsNullOrWhiteSpace(HuverLink)) ||
                        (string.IsNullOrEmpty(AddressEditable) && string.IsNullOrWhiteSpace(AddressEditable)))
                    {
                        MessageBox.Show("Cant complete action make sure the mouse pointer is hovering over the link you want.");
                        return;
                    }
                        WebBrowser.CBrowser.Browser.GetMainFrame().GetSource(new SourceVisitor(htmlSource =>
                        {
                            try
                            {
                                string splitter = getsplitter();
                                ///events/174736672890597/?ref=br_rs&amp;action_history=null
                                ///events/656447624373019/?ref=br_rs&action_history=null
                                string linkToGet = HuverLink;
                                string link = huverLink;
                                if (AddressEditable.Contains("facebook.com/groups/?category=membership"))
                                {
                                    string fromsource = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
                                    fromsource = htmlSource.Substring(htmlSource.IndexOf(fromsource));
                                    string name = link.Substring(link.IndexOf(">")+1);
                                    name = name.Remove(name.IndexOf("<"));

                                    string id = link.Substring(link.IndexOf("id="));
                                    id = id.Replace("id=","");
                                    id = id.Remove(id.IndexOf("\""));

                                    link = Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id;
                                }
                                else
                                {
                                    linkToGet = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
                                    linkToGet = linkToGet.Replace(Social.FACEBOOK_EVENTS_DEFAULT_URL, "/events/");
                                    linkToGet = linkToGet.Replace("?ref=br_rs&action_history=null", "?ref=br_rs&amp;action_history=null");
                                    link = getLinkFromUrlAndSource(linkToGet, htmlSource, splitter);
                                }
                                
                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    OnAddedToGoViral(link, "", null);
                                });
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("Couldnt pull data.");
                            }
                        }));
                    break;

                case 444:
                    SourceVisitor visitor = new SourceVisitor(htmlSource =>
                    {
                        try
                        {
                            List<string> linksToReturn = new List<string>();
                            if (AddressEditable.Contains("facebook.com/groups/?category=membership"))
                            {
                                List<string> links = htmlSource.Split(new string[] { "groupsRecommendedTitle" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                links.RemoveAt(0);
                                foreach (var link in links)
                                {
                                    string name = link.Substring(link.IndexOf(">") + 1);
                                    name = name.Remove(name.IndexOf("<"));

                                    string id = link.Substring(link.IndexOf("id=")+3);
                                    id = id.Remove(id.IndexOf("\""));

                                    linksToReturn.Add(Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id);
                                }
                            }
                            else
                            {
                                string splitter = getsplitter();

                                List<string> links = htmlSource.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                links.RemoveAt(0);


                                foreach (string link in links)
                                {
                                    string linkToGet = link.Remove(link.IndexOf("\""));
                                    string linkToAdd = getLinkFromUrlAndSource(linkToGet, htmlSource, splitter);
                                    linksToReturn.Add(linkToAdd);
                                }
                            }

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                OnAddedToGoViral(null, "", linksToReturn);
                            });
                        }
                        catch
                        {
                            MessageBox.Show("Couldnt pull pages.");
                        }
                    });
                    WebBrowser.CBrowser.Browser.GetMainFrame().GetSource(visitor);
                    break;
                #endregion

                default:
                    break;
            }
        }

        private string getsplitter()
        {
            string splitter =  "<div class=\"_gll\"><a href=\"";
            if (AddressEditable.Contains("/places/"))
            {
                splitter = "<a target=\"_blank\" href=\"";
            }

            return splitter;
        }

        private string getLinkFromUrlAndSource(string url, string htmlSource, string splitter)
        {
            string type = AddressEditable.Replace("https://www.facebook.com/search/", "");
            type = type.Remove(type.IndexOf("/"));

            string id = htmlSource.Substring(0, htmlSource.IndexOf(splitter + url));
            id = id.Replace("&quot;", "");
            id = id.Replace("quot;", "");
            id = id.Substring(id.LastIndexOf("data-bt=\"{id:"));
            id = id.Substring(id.IndexOf(":") + 1);
            id = id.Remove(id.IndexOf(","));

            string name = url;
            if (name.Contains("/?ref=br_rs"))
            {
                bool gotName = false;
                if (name.Contains("&amp;action_history=null"))
                {
                    try
                    {
                        name = htmlSource.Substring(htmlSource.IndexOf(splitter + url));
                        name = name.Substring(0, name.IndexOf("</a>"));
                        name = name.Substring(name.LastIndexOf(">") + 1);
                        gotName = true;
                    }
                    catch { name = url; }
                }
                if (!gotName)
                {
                    name = name.Replace("/?ref=br_rs", "");
                    name = name.Substring(name.LastIndexOf("/") + 1);
                }
            }
            else
            {
                name = name.Replace("https://www.facebook.com/", "");
                name = name.Replace("/", "");
            }

            string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
            link = link.Replace("&amp;action_history=null", "");
            return link;
        }

        private string getImageUrl(string url)
        {
            try
            {
                var req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "HEAD";
                req.Proxy = MyFilesDatabase.GetRequestsProxy();
                using (var resp = req.GetResponse())
                {
                    if (!resp.ContentType.ToLower(CultureInfo.InvariantCulture).StartsWith("image/"))
                    {
                        url = "";
                    }
                }

                return url;
            }
            catch { }
            return "";
        }

        void WebBrowser_OnBrowserStatusChanged(string oMessage)
        {
            HuverLink = oMessage;
        }

        void WebBrowser_OnCreateNewTab(string url)
        {
            OnCreateNewTab(url);
        }
          
        void WebBrowser_OnBrowserAddressChanged(string address)
        {
            OnShouldChangePropertyAddress(address);

            StatusMessage = "Loading...";
            if (address == "about:blank")
            {
                if (WebBrowserHost.Visibility == Visibility.Visible)
                {
                    WebBrowserHost.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (WebBrowserHost.Visibility == Visibility.Collapsed)
                {
                    WebBrowserHost.Visibility = Visibility.Visible;
                }
            }

            //AddressEditable = address;
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Address Changed " + address);
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
            {
                StatusMessage = "Loading...";
            }
            else
            {
                StatusMessage = "Done";
            }
        }

        public void ChangeAddressEditable(string address)
        {
            AddressEditable = address;
        }

        private void Go()
        {  
            WebBrowser.Navigate(AddressEditable);
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
   
        private async void Inject()
        {
            if (WebBrowser.CBrowser.Browser.GetMainFrame() == null || WebBrowser.CBrowser.Browser.GetMainFrame().Url == null) return;
            string curUrl = WebBrowser.CBrowser.Browser.GetMainFrame().Url;

            PersonData profile = await getSelectedProfile();    
            if (profile == null) return;

            string linkToExecute = WebBrowser.CBrowser.Browser.GetMainFrame().Url;
            string emailForBlogOrWp = profile.Username;
            if (linkToExecute.ToLower().Contains("blog.com"))
            {
                emailForBlogOrWp = profile.Email;
            }

            int indexMonth = profile.CmbSelectedIndexMonth + 1;
            int indexDay = profile.CmbSelectedIndexDay + 1;
            int indexYear = profile.BirthdayYear;
            int indexGender = profile.CmbSelectedIndexSex + 1;

            #region mail.com
            string jsForMailDotCom = "var all = document.getElementsByTagName('*');" +
          "for (var i=0, max=all.length; i < max; i++) {" +
                    "for (var j = 0; j < all[i].attributes.length; j++) {" +
                        "var attrib = all[i].attributes[j]; " +
                        "if(attrib.value.toLowerCase().indexOf('first') > -1){" +
                             "for (var k = 0; k < all[i].childNodes.length; k++){" +
                                 "for (var r = 0; r < all[i].childNodes[k].childNodes.length; r++){" +
                                    "for (var t = 0; t < all[i].childNodes[k].childNodes[r].childNodes.length; t++){" +
                                      "try {" +
                                        "if(all[i].childNodes[k].childNodes[r].childNodes[t].tagName.indexOf('INPUT') > -1){" +
                                            "all[i].childNodes[k].childNodes[r].childNodes[t].value=" + "'" + profile.FirstName + "'" + "; break;" +
                                        "}" +
                                       "} catch(err) { }" +
                                    "}" +
                                 "}" +
                             "}" +
                         "}" +
                         "if(attrib.value.toLowerCase().indexOf('mail') > -1){" +
                             "for (var k = 0; k < all[i].childNodes.length; k++){" +
                                 "for (var r = 0; r < all[i].childNodes[k].childNodes.length; r++){" +
                                    "for (var t = 0; t < all[i].childNodes[k].childNodes[r].childNodes.length; t++){" +
                                      "try {" +
                                        "if(all[i].childNodes[k].childNodes[r].childNodes[t].tagName.indexOf('INPUT') > -1){" +
                                            "all[i].childNodes[k].childNodes[r].childNodes[t].value=" + "'" + profile.Email + "'" + "; break;" +
                                        "}" +
                                       "} catch(err) { }" +
                                    "}" +
                                 "}" +
                             "}" +
                         "}" +
                         "if(attrib.value.indexOf('EmailAddress') > -1){" +
                             "for (var k = 0; k < all[i].childNodes.length; k++){" +
                                 "try {" +
                                        "if(all[i].childNodes[k].tagName.indexOf('INPUT') > -1){" +
                                            "all[i].childNodes[k].value=" + "'" + profile.Username + "'" + "; break;" +
                                        "}" +
                                   "} catch(err) { }" +
                             "}" +
                         "}" +
                         "if(attrib.value.toLowerCase().indexOf('last') > -1){" +
                             "for (var k = 0; k < all[i].childNodes.length; k++){" +
                                 "for (var r = 0; r < all[i].childNodes[k].childNodes.length; r++){" +
                                    "for (var t = 0; t < all[i].childNodes[k].childNodes[r].childNodes.length; t++){" +
                                      "try {" +
                                        "if(all[i].childNodes[k].childNodes[r].childNodes[t].tagName.indexOf('INPUT') > -1){" +
                                            "all[i].childNodes[k].childNodes[r].childNodes[t].value=" + "'" + profile.LastName + "'" + "; break;" +
                                        "}" +
                                       "} catch(err) { }" +
                                    "}" +
                                 "}" +
                             "}" +
                         "}" +
                    "}" +
            "if(all[i].tagName.indexOf('INPUT') > -1){" +
                "for (var j = 0; j < all[i].attributes.length; j++) {" +
                    "var attrib = all[i].attributes[j]; " +
                     "if(attrib.value.toLowerCase().indexOf('password') > -1){" +
                         "all[i].value=" + "'" + profile.Password + "'" + "; break;" +
                     "}" +
                "}" +
            "}" +
          "}";
            #endregion

            #region yahoo.com
            string yahooJsToExecute = "var all = document.getElementsByTagName('*');" +
        "for (var i=0, max=all.length; i < max; i++) {" +
         "if(all[i].tagName.indexOf('SELECT') > -1){" +
              "for (var j = 0; j < all[i].attributes.length; j++) {" +
                "var attrib = all[i].attributes[j]; " +

"if(attrib.value.toLowerCase().indexOf('month') > -1){" +
        "for (var t=0; t < all[i].parentNode.childNodes.length; t++) {" +
            "try {" +
                "if(all[i].parentNode.childNodes[t].tagName.indexOf('DIV') > -1){" +
                    "for (var h = 0; h < all[i].parentNode.childNodes[t].attributes.length; h++) {" +
                            "var attribv = all[i].parentNode.childNodes[t].attributes[h]; " +
                            "if(attribv.value.indexOf('birthday') > -1){" +
                                "all[i].parentNode.childNodes[k].innerText = '" + profile.MonthList[profile.CmbSelectedIndexMonth] + "';break;" +
                            "}" +
                    "}" +
                "}" +
            "} catch(err) { }" +
        "}" +
"all[i].value=" + indexMonth + "; break;" +
"}" +
                 "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                 "alert('ip');" +
         "for (var t=0; t < all[i].parentNode.childNodes.length; t++) {" +
            "try {" +
                "if(all[i].parentNode.childNodes[t].tagName.indexOf('DIV') > -1){" +
                    "for (var h = 0; h < all[i].parentNode.childNodes[t].attributes.length; h++) {" +
                            "var attribv = all[i].parentNode.childNodes[t].attributes[h]; " +
                            "if(attribv.value.indexOf('birthday') > -1){" +
                                "all[i].parentNode.childNodes[k].innerText = '" + indexDay + "';break;" +
                            "}" +
                    "}" +
                "}" +
            "} catch(err) { }" +
        "}" +
                       "all[i].value=" + indexDay + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                  "alert('yo2');" +
                  "for (var t=0; t < all[i].parentNode.childNodes.length; t++) {" +
            "try {" +
                "if(all[i].parentNode.childNodes[t].tagName.indexOf('DIV') > -1){" +
                    "for (var h = 0; h < all[i].parentNode.childNodes[t].attributes.length; h++) {" +
                            "var attribv = all[i].parentNode.childNodes[t].attributes[h]; " +
                            "if(attribv.value.indexOf('birthday') > -1){" +
                                "all[i].parentNode.childNodes[k].innerText = '" + indexYear + "';break;" +
                            "}" +
                    "}" +
                "}" +
            "} catch(err) { }" +
        "}" +
                       "all[i].value=" + indexYear + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('gender') > -1 || attrib.value.toLowerCase().indexOf('sex') > -1){" +
                         "all[i].value=" + indexGender + "; break;" +
                   "}" +
                "}" +
            "}" +

            "if(all[i].tagName.indexOf('INPUT') > -1){" +
                "for (var j = 0; j < all[i].attributes.length; j++) {" +
                    "var attrib = all[i].attributes[j]; " +
                    // "alert(attrib.value.toLowerCase());" +
                    "if(attrib.value.toLowerCase().indexOf('first') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('last') > -1){" +
                         "all[i].value=" + "'" + profile.LastName + "'" + "; break;" +
                     "}" +
                      "if(attrib.value.toLowerCase().indexOf('full') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + " ' + ' " + profile.LastName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('mail') > -1 && attrib.value.toLowerCase().indexOf('name') <= 0){" +
                         "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('session_key') > -1){" +
                         "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('user_login') > -1){" +
                         "all[i].value=" + "'" + emailForBlogOrWp + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('username') > -1){" +
                         "all[i].value=" + "'" + profile.Username + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('phone') > -1 || attrib.value.toLowerCase().indexOf('mobile') > -1){" +
                        "if(attrib.value.toLowerCase().indexOf('mobileNumberAnnotation') < 0){" +
                            "all[i].value=" + "'" + profile.PhoneNumber + "'" + "; break;" +
                         "}" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('gender') > -1){" +
                         "all[i].value=" + "'" + profile.SexList[profile.CmbSelectedIndexSex] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                         "all[i].value=" + "'" + profile.DayList[profile.CmbSelectedIndexDay] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('month') > -1){" +
                         "all[i].value=" + "'" + profile.MonthList[profile.CmbSelectedIndexMonth] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                         "all[i].value=" + "'" + profile.BirthdayYear + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('zip') > -1){" +
                         "all[i].value=" + "'" + profile.Zip + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('password') > -1){" +
                         "all[i].value=" + "'" + profile.Password + "'" + "; break;" +
                     "}" +
                "}" +
            "}" +
                      "if(all[i].tagName.indexOf('DIV') > -1){" +
                        "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                        "}" +
                      "}" +
                      "if(all[i].tagName.indexOf('SPAN') > -1){" +
                         "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                        "}" +
                     "}" +
          "}";

            #endregion

            #region main js
            string jsToExecute = "var all = document.getElementsByTagName('*');" +
        "for (var i=0, max=all.length; i < max; i++) {" +
         "if(all[i].tagName.indexOf('SELECT') > -1){" +
              "for (var j = 0; j < all[i].attributes.length; j++) {" +
                "var attrib = all[i].attributes[j]; " +
                "if(attrib.value.toLowerCase().indexOf('month') > -1){" +
                       "all[i].value=" + indexMonth + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                       "all[i].value=" + indexDay + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                       "all[i].value=" + indexYear + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('gender') > -1 || attrib.value.toLowerCase().indexOf('sex') > -1){" +
                         "all[i].value=" + indexGender + "; break;" +
                   "}" +
                "}" +
            "}" +
            "if(all[i].tagName.indexOf('INPUT') > -1){" +
                "for (var j = 0; j < all[i].attributes.length; j++) {" +
                    "var attrib = all[i].attributes[j]; " +
                    // "alert(attrib.value.toLowerCase());" +
                    "if(attrib.value.toLowerCase().indexOf('first') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('name_f') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('last') > -1){" +
                         "all[i].value=" + "'" + profile.LastName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('name_l') > -1){" +
                         "all[i].value=" + "'" + profile.LastName + "'" + "; break;" +
                     "}" +
                      "if(attrib.value.toLowerCase().indexOf('full') > -1 && attrib.value.toLowerCase().indexOf('email') < -1){" +
                         "all[i].value=" + "'" + profile.FirstName + " ' + ' " + profile.LastName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('mail') > -1 && attrib.value.toLowerCase().indexOf('name') <= 0){" +
                         "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('aria-describedby') > -1){" +
                         "all[i].removeAttribute('aria-describedby'); break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('session_key') > -1){" +
                         "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('user_login') > -1){" +
                         "all[i].value=" + "'" + emailForBlogOrWp + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('username') > -1){" +
                         "all[i].value=" + "'" + profile.Username + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('login') > -1){" +
                         "all[i].value=" + "'" + profile.Username + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('phone') > -1 || attrib.value.toLowerCase().indexOf('mobile') > -1){" +
                        "if(attrib.value.toLowerCase().indexOf('mobileNumberAnnotation') < 0){" +
                            "all[i].value=" + "'" + profile.PhoneNumber + "'" + "; break;" +
                         "}" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('gender') > -1){" +
                         "all[i].value=" + "'" + profile.SexList[profile.CmbSelectedIndexSex] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                         "all[i].value=" + "'" + profile.DayList[profile.CmbSelectedIndexDay] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('month') > -1){" +
                         "all[i].value=" + "'" + profile.MonthList[profile.CmbSelectedIndexMonth] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                         "all[i].value=" + "'" + profile.BirthdayYear + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('zip') > -1){" +
                         "all[i].value=" + "'" + profile.Zip + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('password') > -1){" +
                         "all[i].value=" + "'" + profile.Password + "'" + "; break;" +
                     "}" +
                "}" +
            "}" +
                      "if(all[i].tagName.indexOf('DIV') > -1){" +
                        "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                            "if(attrib.value.indexOf('uiStickyPlaceholderInput uiStickyPlaceholderEmptyInput') > -1){" +
                                "all[i].setAttribute('class', 'uiStickyPlaceholderInput');" + "break;" +
                            "}" +
                        "}" +
                      "}" +
                      "if(all[i].tagName.indexOf('SPAN') > -1){" +
                         "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                        "}" +
                     "}" +

                      "if(all[i].tagName.indexOf('BUTTON') > -1){" +
                         "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('disabled') > -1){" +
                                "document.getElementsByTagName('*')[i].removeAttribute('disabled'); break;" +
                            "}" +
                        "}" +
                     "}" +
          "}";
            #endregion

            #region facebook js
            string jsToExecuteFacebook = "var all = document.getElementsByTagName('*');" +
        "for (var i=0, max=all.length; i < max; i++) {" +
         "if(all[i].tagName.indexOf('SELECT') > -1){" +
              "for (var j = 0; j < all[i].attributes.length; j++) {" +
                "var attrib = all[i].attributes[j]; " +
                "if(attrib.value.toLowerCase().indexOf('month') > -1){" +
                       "all[i].value=" + indexMonth + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                       "all[i].value=" + indexDay + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                       "all[i].value=" + indexYear + "; break;" +
                 "}" +
                 "if(attrib.value.toLowerCase().indexOf('gender') > -1 || attrib.value.toLowerCase().indexOf('sex') > -1){" +
                         "all[i].value=" + indexGender + "; break;" +
                   "}" +
                "}" +
            "}" +
            "if(all[i].tagName.indexOf('INPUT') > -1){" +
                "for (var j = 0; j < all[i].attributes.length; j++) {" +
                    "var attrib = all[i].attributes[j]; " +
                    // "alert(attrib.value.toLowerCase());" +
                    "if(attrib.value.toLowerCase().indexOf('first') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('last') > -1){" +
                         "all[i].value=" + "'" + profile.LastName + "'" + "; break;" +
                     "}" +
                      "if(attrib.value.toLowerCase().indexOf('full') > -1){" +
                         "all[i].value=" + "'" + profile.FirstName + " ' + ' " + profile.LastName + "'" + "; break;" +
                     "}" +
                     //"if(attrib.value.toLowerCase().indexOf('mail') > -1 && attrib.value.toLowerCase().indexOf('name') <= 0){" +
                     //    "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     //"}" +
                     //"if(attrib.value.toLowerCase().indexOf('aria-describedby') > -1){" +
                     //    "all[i].removeAttribute('aria-describedby'); break;" +
                     //"}" +
                     //"if(attrib.value.toLowerCase().indexOf('session_key') > -1){" +
                     //    "all[i].value=" + "'" + profile.Email + "'" + "; break;" +
                     //"}" +
                     "if(attrib.value.toLowerCase().indexOf('user_login') > -1){" +
                         "all[i].value=" + "'" + emailForBlogOrWp + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('username') > -1){" +
                         "all[i].value=" + "'" + profile.Username + "'" + "; break;" +
                     "}" +
                     //"if(attrib.value.toLowerCase().indexOf('phone') > -1 || attrib.value.toLowerCase().indexOf('mobile') > -1){" +
                     //   "if(attrib.value.toLowerCase().indexOf('mobileNumberAnnotation') < 0){" +
                     //       "all[i].value=" + "'" + profile.PhoneNumber + "'" + "; break;" +
                     //    "}" +
                     //"}" +
                     "if(attrib.value.toLowerCase().indexOf('gender') > -1){" +
                         "all[i].value=" + "'" + profile.SexList[profile.CmbSelectedIndexSex] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('day') > -1){" +
                         "all[i].value=" + "'" + profile.DayList[profile.CmbSelectedIndexDay] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('month') > -1){" +
                         "all[i].value=" + "'" + profile.MonthList[profile.CmbSelectedIndexMonth] + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('year') > -1){" +
                         "all[i].value=" + "'" + profile.BirthdayYear + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('zip') > -1){" +
                         "all[i].value=" + "'" + profile.Zip + "'" + "; break;" +
                     "}" +
                     "if(attrib.value.toLowerCase().indexOf('password') > -1){" +
                         "all[i].value=" + "'" + profile.Password + "'" + "; break;" +
                     "}" +
                "}" +
            "}" +
                      "if(all[i].tagName.indexOf('DIV') > -1){" +
                        "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                            "if(attrib.value.indexOf('uiStickyPlaceholderInput uiStickyPlaceholderEmptyInput') > -1){" +
                                "all[i].setAttribute('class', 'uiStickyPlaceholderInput');" + "break;" +
                            "}" +
                        "}" +
                      "}" +
                      "if(all[i].tagName.indexOf('SPAN') > -1){" +
                         "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('placeholder') > -1){" +
                                "all[i].innerText=" + "''" + "; break;" +
                            "}" +
                        "}" +
                     "}" +
          "}";
            #endregion

            string messageBoxMessage = "Register With This Site Using C/P";
            string jsForExecution = jsToExecute;

            //change js
            if (linkToExecute.ToLower().Contains("mail.com"))
            {
                jsForExecution = jsForMailDotCom;
            }
            else if (linkToExecute.ToLower().Contains("facebook.com"))
            {
                MessageBox.Show(messageBoxMessage);
                jsForExecution = jsToExecuteFacebook;
            }
            else if (linkToExecute.ToLower().Contains("livejournal.com"))
            {
                MessageBox.Show(messageBoxMessage);
                return;
            }
            else if (linkToExecute.ToLower().Contains("accounts.google.com"))
            {
                MessageBox.Show(messageBoxMessage);
                return;
            }

            WebBrowser.CBrowser.Browser.GetMainFrame().ExecuteJavaScript(jsForExecution, linkToExecute, 0);                    


            //string hasMultiple = "false";
            //if (isFromMulti)
            //    hasMultiple = "true";

            //string isTumblr = "false";
            //if (CBrowser.Address.ToLower().Contains("tumblr"))
            //    isTumblr = "true";

            //if (isTumblr == "true")
            //{
            //    Process[] processes = Process.GetProcessesByName("BrowserAndFeatures");
            //    Process lol = processes[0];
            //    IntPtr ptr = lol.MainWindowHandle;
            //    Rect NotepadRect = new Rect();
            //    GetWindowRect(ptr, ref NotepadRect);

            //    int X = NotepadRect.Right+100;
            //    int Y = NotepadRect.Top + 300;
            //    mouse_event((uint)MOUSEEVENTF_LEFTDOWN | (uint)MOUSEEVENTF_LEFTUP, (uint)X, (uint)Y, 0, 0);
            //}
            //hasToInject = true;
            //CBrowser.Browser.SendProcessMessage(CefProcessId.Renderer, CefProcessMessage.Create(BrowserInit.SitesFilePath + "{||}" + hasMultiple + "{||}" + selectedPath + "{||}" + isTumblr));



            //CBrowser.Browser.GetMainFrame().ExecuteJavaScript("document.getElementById(\"signup_email\").value=result1", CBrowser.Address, 0);
            //CBrowser.Browser.SendProcessMessage(CefProcessId.Renderer, CefProcessMessage.Create("GetHackerNewsTitles"));
            //CBrowser.Browser.GetMainFrame().VisitDom(new Xilium.CefGlue.Client.BrowserInit.DemoCefDomVisitor());  
        }

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

        private async Task<PersonData> getSelectedProfile()
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

        public void NavigateToSelectedSite(string site)
        {
            AddressEditable = site;
            WebBrowser.Navigate(site);  
        }
        #endregion

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

        bool oldJavaCript, oldJava, oldFlash, oldSysDate, oldDnt;
        int oldTZSI = 0;

        internal void SettingsMenuOpen()
        {
            oldJavaCript = JavascriptEnabled;
            oldJava = JavaEnabled;
            oldFlash = FlashEnabled;
            oldSysDate = SetSysDateEnabled;
            oldDnt = DoNotTrackEnabled;
            oldTZSI = SITimeZone;
        }

        internal void SettingsMenuClosed()
        {
            JavascriptEnabled = oldJavaCript;
            JavaEnabled = oldJava;
            FlashEnabled = oldFlash;
            SetSysDateEnabled = oldSysDate;
            DoNotTrackEnabled = oldDnt;
            SITimeZone = oldTZSI;
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

        #region share
        private void SendToSocialBrowserPopUp(object param)
        {
            string shareType = (string)param;
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Share From Browser " + shareType);

            string fullUrl = Social.GetShareUrl(shareType, AddressEditable);

            if (fullUrl != "" && fullUrl !="pin")
                launchSharePopUP(fullUrl);
            else if (fullUrl == "pin")
            {
                PinterestImagePickerVM pinterestImagePicker = new PinterestImagePickerVM();
                pinterestImagePicker.OnLaunchSharePopup += launchSharePopUP;
                pinterestImagePicker.VisitSource(AddressEditable);
                WebBrowser.CBrowser.Browser.GetMainFrame().GetSource(pinterestImagePicker.Visitor);
            }
        }

        private void launchSharePopUP(string fullUrl)
        {
            BrowserForSocialShare bfss = new BrowserForSocialShare();
            bfss.Text = "Loading... " + AddressEditable;
            bfss.browserCntrl1.init(fullUrl,
                BrowserSettimgs.JavascriptEnabled ? CefState.Enabled : CefState.Disabled,
                BrowserSettimgs.JavaEnabled ? CefState.Enabled : CefState.Disabled,
                BrowserSettimgs.FlashEnabled ? CefState.Enabled : CefState.Disabled);
            bfss.Show();
        }
        #endregion
    }
}
