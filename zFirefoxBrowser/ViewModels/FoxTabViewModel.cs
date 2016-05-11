using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using zFirefoxBrowser.Controls;
using Browser.Common.ViewModels;
using SocialOrganizer.Models;
using Gecko;
using Gecko.DOM;

namespace zFirefoxBrowser.ViewModels
{
    public class FoxTabViewModel : Browser.Common.ViewModels.BrowserTabViewModel
    {
        #region events and commands 

        public ICommand GoCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand ForwardCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand InjectCommand { get; set; }
        public ICommand SendToBrowserSocial { get; set; }

        #endregion

        private FFBrowserControl webBrowser;
        public FFBrowserControl WebBrowser
        {
            get
            {
                //webBrowser.
                return webBrowser;
            }
            set { webBrowser = value; RaisePropertyChanged("WebBrowser"); }
        }

        public override System.Windows.Forms.UserControl Browser
        {
            get
            {
                if (base.Browser == null)
                {
                    base.Browser = new System.Windows.Forms.UserControl();
                    base.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
                }
                return base.Browser;
            }
        }

        public FoxTabViewModel(string address, bool setTheBrowser = true) : base(address, setTheBrowser)
        {
            GoCommand = new DelegateCommand(Go);
            BackCommand = new DelegateCommand(Back);
            ForwardCommand = new DelegateCommand(Forward);
            ReloadCommand = new DelegateCommand(Reload);
            InjectCommand = new DelegateCommand(Inject);
            SendToBrowserSocial = new RelayCommand(SendToSocialBrowserPopUp);

            base.OnRefreshTabSettings += ChromeBrowserTabViewModel_OnRefreshTabSettings;

            SetBrowserSettingsAvailable = Visibility.Collapsed;
        }



        private void SendToSocialBrowserPopUp(object url)
        {
        }

        private async void Inject()
        {

            if (WebBrowser.Browser.Document == null || WebBrowser.Browser.Url == null) return;
            string curUrl = WebBrowser.Browser.Url.ToString();


            if (curUrl.ToLower().Contains("accounts.google.com"))
            {
                "Register To This Site Using C/P".Show();
                return;
            }

            PersonData profile = await getSelectedProfile();
            if (profile == null) return;

            GeckoElementCollection elements = WebBrowser.Browser.Document.GetElementsByTagName("input");
            foreach (var element in elements)
            {
                GeckoInputElement input = (GeckoInputElement)element;
                if (input == null) continue;
                if(input.Type!=null && input.Type.ToLower() == "radio")
                {
                    bool male = GloableProfData.PData.CmbSelectedIndexSex == 0;
                    if(male && input.Id.ToLower().Contains("ma") || input.Id.ToLower().Contains("u_0_f") || input.Name.ToLower().Contains("ma"))
                    {
                        input.Click();
                    }
                    else if (!male && input.Id.ToLower().Contains("fe") || input.Id.ToLower().Contains("u_0_e") || input.Name.ToLower().Contains("fe"))
                    {
                        input.Click();
                    }
                }
                else
                {
                    if (isInputOf(input, "first") || isInputOf(input, "name_f"))
                    {
                        setNodeValue(input, GloableProfData.PData.FirstName);
                    }
                    else if (isInputOf(input, "last") || isInputOf(input, "name_l"))
                    {
                        setNodeValue(input, GloableProfData.PData.LastName);
                    }
                    else if (isInputOf(input, "full") && !isInputOf(input, "email"))
                    {
                        setNodeValue(input, GloableProfData.PData.FirstName + " " + GloableProfData.PData.LastName);
                    }
                    else if ((isInputOf(input, "mail") && !isInputOf(input, "name")) ||
                        isInputOf(input, "session_key"))
                    {
                        setNodeValue(input, GloableProfData.PData.Email);
                    }
                    else if (isInputOf(input, "username") || isInputOf(input, "login") || isInputOf(input, "user_login"))
                    {
                        setNodeValue(input, GloableProfData.PData.Username);
                    }
                    else if (isInputOf(input, "phone") || isInputOf(input, "number"))
                    {
                        setNodeValue(input, GloableProfData.PData.PhoneNumber);
                    }
                    else if (isInputOf(input, "password") || isInputOf(input, "pass"))
                    {
                        setNodeValue(input, GloableProfData.PData.Password);
                    }

                    WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keydown", 0, 9, 0, false);
                    WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keypress", 0, 9, 0, false);
                    WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keyup", 0, 9, 0, false);
                }
            }

            GeckoElementCollection selectelements = WebBrowser.Browser.Document.GetElementsByTagName("select");
            foreach (var sel in selectelements)
            {
                GeckoSelectElement element = (GeckoSelectElement)sel;
                if (element.Attributes == null) continue;

                foreach (var attrib in element.Attributes)
                {
                    if (attrib.NodeValue == null) continue;

                    if (attrib.NodeValue.ToLower().Contains("month"))
                    {
                        element.Click();
                        element.Focus();
                        element.Value = (GloableProfData.PData.CmbSelectedIndexMonth + 1).ToString();
                        element.Blur();
                        break;
                    }
                    else if (attrib.NodeValue.ToLower().Contains("day"))
                    {
                        element.Click();
                        element.Focus();
                        element.Value = (GloableProfData.PData.CmbSelectedIndexDay+1).ToString();
                        element.Blur();
                        break;
                    }
                    else if (attrib.NodeValue.ToLower().Contains("year"))
                    {
                        element.Click();
                        element.Focus();
                        element.Value = GloableProfData.PData.BirthdayYear.ToString();
                        element.Blur();
                        break;
                    }
                    else if (attrib.NodeValue.ToLower().Contains("gend") || attrib.NodeValue.ToLower().Contains("sex"))
                    {
                        element.Click();
                        element.Focus();
                        element.Value = GloableProfData.PData.CmbSelectedIndexSex.ToString();
                        element.Blur();
                        break;
                    }
                }
            }
        }

        private void setNodeValue(GeckoInputElement input, string valu)
        {
            input.Click();
            input.Focus();
            input.select();
            input.Value = valu;
            input.Blur();
        }

        private bool isInputOf(GeckoInputElement input, string idorname)
        {
            if (input.Type != null && input.Type.ToLower() == "submit") return false;
            bool isIt = input.Id.ToLower().Contains(idorname) || input.Name.ToLower().Contains(idorname);

            if (isIt) return isIt;
            else recursiveFindId(input.Parent, idorname, out isIt);

            return isIt;

            //return input.Id.ToLower().Contains(idorname) || input.Name.ToLower().Contains(idorname);
        }
        private void recursiveFindId(GeckoHtmlElement input, string idorname, out bool isIt)
        {
            isIt = false;
            if (input.NodeName.ToUpper() == "FORM") return;

            isIt = input.Id.ToLower().Contains(idorname) || input.ClassName.ToLower().Contains(idorname);
            if (isIt) return;

            if(input.Parent != null && input.Parent.NodeName.ToUpper() != "FORM")
            {
                recursiveFindId(input.Parent, idorname, out isIt);
            }
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

        private void Go()
        {
            WebBrowser.Navigate(AddressEditable);
        }

        void WebBrowser_OnBrowserStatusChanged(string oMessage)
        {
            if(!oMessage.IsNullOrEmpty())
            HuverLink = oMessage;
        }

        void WebBrowser_OnCreateNewTab(string url)
        {
            RaisOnCreateNewTab(url);
        }

        void WebBrowser_OnBrowserAddressChanged(string address)
        {
            StatusMessage = "Loading...";
            if (address == "about:blank")
            {
                //if (WebBrowserHost.Visibility == Visibility.Visible)
                //{
                //    WebBrowserHost.Visibility = Visibility.Collapsed;
                //}
            }
            else
            {
                if (WebBrowserHost.Visibility == Visibility.Collapsed)
                {
                    WebBrowserHost.Visibility = Visibility.Visible;
                }

                //if (WebBrowser.Browser != null && WebBrowser.Browser.Url != null)
                //    RaiseOnShouldChangePropertyAddress(WebBrowser.Browser.Url.ToString());
            }

            //AddressEditable = address;
            UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_AddressChange + " " + address);
        }

        void WebBrowser_OnBrowserTitleChanged(string ttl)
        {
            Title = ttl;
        }

        void WebBrowser_OnBrowserMessageChanged(string oMessage)
        {
            if (oMessage.Length > 100) oMessage = oMessage.Remove(99);
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

                if (WebBrowser.Browser != null && WebBrowser.Browser.Url != null)
                    RaiseOnShouldChangePropertyAddress(WebBrowser.Browser.Url.ToString());
            }

        }

        private void WebBrowser_OnBrowserContextMenuClicked(string param)
        {
            try
            {
                switch (param)
                {
                    //To Social Enagager
                    case "888":
                        if (!string.IsNullOrEmpty(HuverLink) && !string.IsNullOrWhiteSpace(HuverLink))
                        {
                            string sitename = AddressEditable.Replace("http://", "");
                            sitename = sitename.Replace("https://", "");
                            sitename = sitename.Replace("www.", "");
                            if (sitename.Contains("."))
                            {
                                sitename = sitename.Remove(sitename.IndexOf("."));
                            }
                            RaiseOnSentForSeo(sitename, HuverLink);
                        }
                        break;

                    //Curaste
                    case "222":
                    //Curate
                    case "666":
                        if (AddressEditable.IsNullOrEmpty()) return;
                        string result;
                        using (AutoJSContext context = new AutoJSContext(WebBrowser.Browser.Window.DomWindow))
                        {
                            context.EvaluateScript(@" function GetSelected() 
                                                   { 
                                                        var range = window.getSelection().getRangeAt(0), 
                                                        content = range.extractContents(),
                                                        span = document.createElement('SPAN');
                                                        span.appendChild(content);
                                                        var htmltext = span.innerHTML.toString();
                                                        range.insertNode(span);
                                                        return htmltext; 
                                                   } 
                                                    GetSelected(); 
                                                ", out result);
                        }

                        if (!result.IsNullOrEmpty())
                        {
                            if (param == "666")
                            {
                                RaiseOnCurateToPBN(result, AddressEditable);
                            }
                            else
                            {
                                string thecontent = "<blockquote>" + result + "<br />";
                                if (!AddressEditable.IsNullOrEmpty()) thecontent += "<a href=\"" + AddressEditable + " \" > " + AddressEditable + " </a>";
                                thecontent += "</blockquote>";
                                MyFilesDatabase.SetClipboardText(thecontent);
                            }
                        }
                        break;

                    //Dominate
                    case "555":
                        string addy = WebBrowser.Browser.Url.ToString();
                        if ((string.IsNullOrEmpty(HuverLink) && string.IsNullOrWhiteSpace(HuverLink)) ||
                            (string.IsNullOrEmpty(addy) && string.IsNullOrWhiteSpace(addy)))
                        {
                            MessageBox.Show("Cant complete action make sure the mouse pointer is hovering over the link you want.");
                            return;
                        }

                        GeckoHtmlElement element = null;
                        var geckoDomElement = WebBrowser.Browser.Document.DocumentElement;
                        if (geckoDomElement is GeckoHtmlElement)
                        {
                            element = (GeckoHtmlElement)geckoDomElement;
                            string html = element.InnerHtml;

                            if (!html.IsNullOrEmpty())
                            {
                                string splitter = getsplitter();
                                ///events/174736672890597/?ref=br_rs&amp;action_history=null
                                ///events/656447624373019/?ref=br_rs&action_history=null
                                string linkToGet = HuverLink;
                                string link = HuverLink;
                                if (addy.Contains("facebook.com/groups/?category=membership"))
                                {
                                    string fromsource = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
                                    fromsource = html.Substring(html.IndexOf(fromsource));
                                    string name = link.Substring(link.IndexOf(">") + 1);
                                    name = name.Remove(name.IndexOf("<"));

                                    string id = link.Substring(link.IndexOf("id="));
                                    id = id.Replace("id=", "");
                                    id = id.Remove(id.IndexOf("\""));

                                    link = Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id;
                                }
                                else
                                {
                                    linkToGet = linkToGet.Replace(Social.FACEBOOK_GROUPS_DEFAULT_URL, "/groups/");
                                    linkToGet = linkToGet.Replace(Social.FACEBOOK_EVENTS_DEFAULT_URL, "/events/");
                                    linkToGet = linkToGet.Replace("?ref=br_rs&action_history=null", "?ref=br_rs&amp;action_history=null");
                                    // linkToGet = linkToGet.Replace("?ref=br_rs", "?ref=br_rs&amp;action_history=null");
                                    link = getLinkFromUrlAndSource(linkToGet, html, splitter);
                                }

                                RaiseOnAddedToGoViral(link, "", null);
                            }
                        }
                        break;

                    //Dominate All
                    case "444":
                        try
                        {
                            GeckoHtmlElement elementa = null;
                            var geckoDomElementa = WebBrowser.Browser.Document.DocumentElement;
                            if (geckoDomElementa is GeckoHtmlElement)
                            {
                                elementa = (GeckoHtmlElement)geckoDomElementa;
                                string html = elementa.InnerHtml;

                                if (!html.IsNullOrEmpty())
                                {
                                    List<string> linksToReturn = new List<string>();
                                    if (AddressEditable.Contains("facebook.com/groups/?category=membership"))
                                    {
                                        List<string> links = html.Split(new string[] { "groupsRecommendedTitle" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        links.RemoveAt(0);
                                        foreach (var link in links)
                                        {
                                            string name = link.Substring(link.IndexOf(">") + 1);
                                            name = name.Remove(name.IndexOf("<"));

                                            string id = link.Substring(link.IndexOf("id=") + 3);
                                            id = id.Remove(id.IndexOf("\""));

                                            linksToReturn.Add(Social.FACEBOOK_GROUPS_DEFAULT_URL + name + "-" + id);
                                        }
                                    }
                                    else
                                    {
                                        string splitter = getsplitter();

                                        List<string> links = html.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        links.RemoveAt(0);


                                        foreach (string link in links)
                                        {
                                            string linkToGet = link.Remove(link.IndexOf("\""));
                                            string linkToAdd = getLinkFromUrlAndSource(linkToGet, html, splitter);
                                            linksToReturn.Add(linkToAdd);
                                        }
                                    }

                                    RaiseOnAddedToGoViral(null, "", linksToReturn);
                                }
                            }

                        }
                        catch
                        {
                            MessageBox.Show("Couldnt pull pages.");
                        }
                        break;

                    default: break;
                }
            }
            catch { }
        }
        private string getsplitter()
        {
            string splitter = "<div class=\"_gll\"><a href=\"";
            if (WebBrowser.Browser.Url.ToString().Contains("/places/"))
            {
                splitter = "<a target=\"_blank\" href=\"";
            }

            return splitter;
        }
        private string getLinkFromUrlAndSource(string url, string htmlSource, string splitter)
        {
            string type = WebBrowser.Browser.Url.ToString().Replace("https://www.facebook.com/search/", "");
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



        private void ChromeBrowserTabViewModel_OnRefreshTabSettings(BrowserTabViewModel obj)
        {
        }

        public override void SetBrowser(string address)
        {
            WebBrowser = new FFBrowserControl();
            WebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            WebBrowser.initBrowser(address);

            WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            WebBrowser.OnBrowserMessageChanged += WebBrowser_OnBrowserMessageChanged;
            WebBrowser.OnBrowserTitleChanged += WebBrowser_OnBrowserTitleChanged;
            WebBrowser.OnBrowserAddressChanged += WebBrowser_OnBrowserAddressChanged;
            WebBrowser.OnBrowserStatusChanged += WebBrowser_OnBrowserStatusChanged;
            WebBrowser.OnCreateNewTab += WebBrowser_OnCreateNewTab;
            WebBrowser.OnBrowserContextMenuClicked += WebBrowser_OnBrowserContextMenuClicked;

            Browser.SuspendLayout();
            Browser.Controls.Add(WebBrowser);
            Browser.ResumeLayout(false);
            Browser.PerformLayout();
            RaisePropertyChanged("Browser");
        }


        public override void ChangeAddressEditable(string address)
        {
            AddressEditable = address;
        }

        public override void NavigateToSelectedSite(string site)
        {
            AddressEditable = site;
            WebBrowser.Navigate(site);
            RaiseOnShouldChangePropertyAddress(AddressEditable);
        }

        public override void Dispose()
        {
            try
            {
                if (WebBrowser != null)
                {
                    if (webBrowser.Browser != null) webBrowser.Browser.Dispose();
                    webBrowser.Dispose();
                }

                if (WebBrowserHost != null)
                {
                    WebBrowserHost.Dispose();
                }
            }
            catch
            { }
        }
    }
}
