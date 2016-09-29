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
using System.Runtime.InteropServices;
using System.Threading;
using zFirefoxBrowser.Helpers;
using System.IO;
using Organiser.Common.Windows;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net;
using Gecko.Interop;
using Gecko.Cache;
using static zFirefoxBrowser.Helpers.FoxInit;
using BrowserHost.ViewModels;

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

        public event Action OnRequestedWindowLocation = delegate { };

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
            SetMacrosAvailableVisible = Visibility.Visible;
            SetWebrtcVisible = Visibility.Visible;
        }

        private string getSource()
        {
            GeckoHtmlElement elementtxt = null;
            GeckoElement geckoDomElementtxt;
            if (WebBrowser.Browser.Document != null && setfromframe) geckoDomElementtxt = WebBrowser.Browser.Document.DocumentElement;
            else geckoDomElementtxt = WebBrowser.Browser.Document.DocumentElement;
            if (geckoDomElementtxt is GeckoHtmlElement)
            {
                elementtxt = (GeckoHtmlElement)geckoDomElementtxt;
                var innerHtml = elementtxt.TextContent;
                return innerHtml;
            }

            return "";
        }


        private void SendToSocialBrowserPopUp(object param)
        {
            string shareType = (string)param;
            Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_BrowserShare + " " + shareType);

            string fullUrl = Social.GetShareUrl(shareType, AddressEditable);

            if (fullUrl != "" && fullUrl != "pin")
                launchSharePopUP(fullUrl);
            else if (fullUrl == "pin")
            {
                PinterestImagePickerVM pinterestImagePicker = new PinterestImagePickerVM();
                pinterestImagePicker.OnLaunchSharePopup += launchSharePopUP;
                pinterestImagePicker.VisitSource(AddressEditable,true, getSource());
            }
        }

        private void launchSharePopUP(string fullUrl)
        {
            GeckoWebBrowser ffpopupMacrosBrowser = new GeckoWebBrowser();
            ffpopupMacrosBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            ffpopupMacrosBrowser.Navigate(fullUrl);

            FFBrowserPopup ffpopupMacros = new FFBrowserPopup();
            ffpopupMacros.Text = fullUrl;
            ffpopupMacros.SuspendLayout();
            ffpopupMacros.Controls.Add(ffpopupMacrosBrowser);
            ffpopupMacros.ResumeLayout(false);
            ffpopupMacros.PerformLayout();
            ffpopupMacros.Show();
        }

        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        double windowLeft, windowTop,windowHeight;
        bool gotCords = false;

        //System.Drawing.Rectangle workingRectangle = System.Windows.Forms.Screen.FromControl(WebBrowser).WorkingArea;
        //rectx += workingRectangle.Width - WebBrowser.Width;
        //

        private async Task GetBoundingRectNClick(GeckoInputElement input)
        {
            try
            {
                gotCords = false;
                OnRequestedWindowLocation();
                await Task.Run(()=> { while (!gotCords) { Thread.Sleep(200); } });

                double windowX = windowLeft;// Application.Current.MainWindow.Left;
                double windowY = windowTop; // Application.Current.MainWindow.Top;
                double winHeight = windowHeight; // Application.Current.MainWindow.Height;

                float rectx = 0, recty = 0;

                System.Drawing.Rectangle rect = input.GetBoundingClientRect();
                rectx = rect.Right - new Random().Next(7, rect.Width - 7);
                recty = (rect.Top + rect.Bottom) / 2;

                rectx += (int)windowX;

                recty += (int)windowY;
                recty += (int)winHeight - WebBrowser.Height;
                //recty -= System.Windows.Forms.SystemInformation.CaptionHeight;
                //recty -= 25;

                SetCursorPos((int)rectx, (int)recty);
                await Task.Run(() => System.Threading.Thread.Sleep(100));
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            catch { }
        }

        internal void GotScreenCords(string message)
        {
            var cords = message.Split(',');
            windowLeft = Convert.ToDouble(cords[0]);
            windowTop = Convert.ToDouble(cords[1]);
            windowHeight = Convert.ToDouble(cords[3]);
            gotCords = true;
        }

        private void Send(char c)
        {
            string s = c.ToString();
            if (s == "+" || s == "^" || s == "%" || s == "~" || s == "(" || s == ")" || s == "{" || s == "}") s = "{" + s + "}";
            System.Windows.Forms.SendKeys.SendWait(s);
        }

        private async Task LoopAndType(string txt)
        {
            foreach (var c in txt)
            {
                Send(c);
                await Task.Run(() => System.Threading.Thread.Sleep(new Random().Next(200, 500)));
            }
        }

        private async void Inject()
        {
            try
            {
                if (WebBrowser.Browser.Document == null || WebBrowser.Browser.Url == null) return;
                string curUrl = WebBrowser.Browser.Url.ToString();

                PersonData profile = await getSelectedProfile(false);
                if (profile == null) return;


                if (curUrl.ToLower().Contains("accounts.google.com/signup"))
                {
                    foreach (var element in WebBrowser.Browser.Document.GetElementsByTagName("INPUT"))
                    {
                        GeckoInputElement input = element as GeckoInputElement;
                        if (input == null) continue;

                        if (input.Id == "FirstName")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.FirstName);
                        }
                        else if (element.Id == "LastName")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.LastName);
                        }
                        else if (element.Id == "GmailAddress")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.Username);
                        }
                        else if (element.Id == "Passwd")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.Password);
                        }
                        else if (element.Id == "PasswdAgain")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.Password);
                        }
                        else if (element.Id == "BirthDay")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType((GloableProfData.PData.CmbSelectedIndexDay + 1).ToString());
                        }
                        else if (element.Id == "BirthYear")
                        {
                            await GetBoundingRectNClick(input);
                            await LoopAndType(GloableProfData.PData.BirthdayYear.ToString());
                        }
                    }

                    return;
                }

                if (curUrl.ToLower().Contains("accounts.google.com/servicelogin"))
                {
                    var elm = WebBrowser.Browser.Document.GetElementById("Email");
                    if (elm != null) await setNodeValue((GeckoInputElement)elm, profile.Email);

                    var pass = WebBrowser.Browser.Document.GetElementById("Passwd");
                    if (pass != null) await setNodeValue((GeckoInputElement)pass, profile.Password);

                    return;
                }

                try
                {
                    if (WebBrowser.Browser.Document.ActiveElement != null)
                    {
                        WebBrowser.Browser.Document.ActiveElement.Blur();
                        WebBrowser.Browser.Document.ActiveElement.DOMHtmlElement.SetCapture(false);
                    }
                    WebBrowser.Browser.Document.Body.Click();
                }
                catch { }
                GeckoElementCollection elements = WebBrowser.Browser.Document.GetElementsByTagName("input");
                foreach (var element in elements)
                {
                    GeckoInputElement input = (GeckoInputElement)element;
                    if (input == null) continue;
                    if (input.Type != null && input.Type.ToLower() == "radio")
                    {
                        bool male = profile.CmbSelectedIndexSex == 0;
                        if (male && input.Id.ToLower().Contains("ma") || input.Id.ToLower().Contains("u_0_f") || input.Name.ToLower().Contains("ma"))
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
                        if (input.Disabled || input.TabIndex == -1) continue;
                        if (WebBrowser.Browser.Url.AbsoluteUri == "https://twitter.com/signup")
                        {
                            if (input.Id == "full-name")
                            {
                                await setNodeValue(input, profile.FirstName + " " + profile.LastName);
                            }
                            else if (input.Id == "email")
                            {
                                await setNodeValue(input, profile.Email);
                            }
                            else if (input.Id == "password")
                            {
                                await setNodeValue(input, profile.Password);
                            }
                            continue;
                        }

                        if (input.Type.ToLower() == "password" && isInputOf(input, "password") || isInputOf(input, "pass"))
                        {
                            await setNodeValue(input, profile.Password);
                        }
                        if (input.Type.ToLower() == "hidden" || input.Type.ToLower() == "password") continue;

                        if (isInputOf(input, "first") || isInputOf(input, "name_f"))
                        {
                            await setNodeValue(input, profile.FirstName);
                        }
                        else if (isInputOf(input, "last") || isInputOf(input, "name_l"))
                        {
                            await setNodeValue(input, profile.LastName);
                        }
                        else if (isInputOf(input, "full") && !isInputOf(input, "email"))
                        {
                            await setNodeValue(input, profile.FirstName + " " + profile.LastName);
                        }
                        else if ((isInputOf(input, "mail") && !isInputOf(input, "name")) ||
                            isInputOf(input, "session_key"))
                        {
                            await setNodeValue(input, profile.Email);
                        }
                        else if (isInputOf(input, "username") || isInputOf(input, "login") || isInputOf(input, "user_login"))
                        {
                            await setNodeValue(input, profile.Username);
                        }
                        else if (isInputOf(input, "phone") || isInputOf(input, "number"))
                        {
                            if (WebBrowser.Browser.Url.AbsoluteUri == "https://www.diigo.com/sign-up") continue;
                            await setNodeValue(input, profile.PhoneNumber);
                        }

                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keydown", 0, 9, 0, false);
                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keypress", 0, 9, 0, false);
                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keyup", 0, 9, 0, false);
                    }
                }

                GeckoElementCollection selectelements = WebBrowser.Browser.Document.GetElementsByTagName("select");
                foreach (var sel in selectelements)
                {
                    GeckoSelectElement element = (GeckoSelectElement)sel;
                    if (element.Attributes == null) continue;

                    foreach (var attrib in element.Attributes)
                    {
                        if (attrib.NodeValue == null || WebBrowser.Browser.Url.AbsoluteUri == "https://www.diigo.com/sign-up") continue;

                        if (attrib.NodeValue.ToLower().Contains("month"))
                        {
                            element.Click();
                            element.Focus();
                            element.Value = (profile.CmbSelectedIndexMonth + 1).ToString();
                            element.Blur();
                            break;
                        }
                        else if (attrib.NodeValue.ToLower().Contains("day"))
                        {
                            element.Click();
                            element.Focus();
                            element.Value = (profile.CmbSelectedIndexDay + 1).ToString();
                            element.Blur();
                            break;
                        }
                        else if (attrib.NodeValue.ToLower().Contains("year"))
                        {
                            element.Click();
                            element.Focus();
                            element.Value = profile.BirthdayYear.ToString();
                            element.Blur();
                            break;
                        }
                        else if (attrib.NodeValue.ToLower().Contains("gend") || attrib.NodeValue.ToLower().Contains("sex"))
                        {
                            element.Click();
                            element.Focus();
                            element.Value = profile.CmbSelectedIndexSex.ToString();
                            element.Blur();
                            break;
                        }
                    }
                }
            }
            catch { }
        }
        
        private async Task setNodeValue(GeckoInputElement input, string valu)
        {
            if (input.Value.ToLower().Trim() == valu.ToLower().Trim()) return;
            await Task.Delay(new Random().Next(100, 200));

            input.DOMHtmlElement.SetCapture(true);
            input.Focus();
            input.Click();
            //input.select();
            try
            {
                input.OwnerDocument.DefaultView.WindowUtils.SendNativeKeyEvent(0, 0, 0, valu, valu);
            }
            catch
            {
                WebBrowser.Browser.Window.WindowUtils.SendNativeKeyEvent(0, 0, 0, valu, valu);
            }
            await Task.Delay(new Random().Next(300, 400));
            if (input.Value.ToLower().Trim() != valu.ToLower().Trim()) input.Value = valu;
            input.DOMHtmlElement.SetCapture(false);
            input.Blur();
        }

        private bool isInputOf(GeckoInputElement input, string idorname)
        {
            if (input.Type != null && input.Type.ToLower() == "submit") return false;
            bool isIt = input.Id.ToLower().Contains(idorname) || input.Name.ToLower().Contains(idorname) || input.Name.ToLower().Contains(idorname);

            if (isIt) return isIt;
            else
            {
                if (input.HasAttribute("placeholder"))
                {
                    isIt = input.GetAttribute("placeholder").ToLower().Contains(idorname);
                    if (isIt) return isIt;
                }
                recursiveFindId(input.Parent, idorname, out isIt);
            }

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
            //StatusMessage = "Loading...";
            //if (address == "about:blank")
            //{
            //    //if (WebBrowserHost.Visibility == Visibility.Visible)
            //    //{
            //    //    WebBrowserHost.Visibility = Visibility.Collapsed;
            //    //}
            //}
            //else
            //{
            //    if (WebBrowserHost.Visibility == Visibility.Collapsed)
            //    {
            //        WebBrowserHost.Visibility = Visibility.Visible;
            //    }

            //    //if (WebBrowser.Browser != null && WebBrowser.Browser.Url != null)
            //    //    RaiseOnShouldChangePropertyAddress(WebBrowser.Browser.Url.ToString());
            //}

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
                                                        var range = window.getSelection().getRangeAt(0);
                                                        content = range.extractContents();
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

        /// <summary>
        /// initializes the browser with events and url
        /// </summary>
        /// <param name="address"></param>
        public override void SetBrowser(string address)
        {
            try
            {
                WebBrowser = new FFBrowserControl();
                WebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
                WebBrowser.initBrowser(address, () =>
                {
                    IsLoading = false;
                });

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

                //WebBrowser.Browser.AddMessageEventListener("iimPlayCode", OnIIMPlayCode);
                //WebBrowser.Browser.AddMessageEventListener("iimPlay", OnIIMPlay);
                WebBrowser.Browser.AddMessageEventListener("iimSet", OnIIMSet);
                WebBrowser.Browser.AddMessageEventListener("iimGetVal", OniIImGetVal);
                WebBrowser.Browser.AddMessageEventListener("iimDisplay", WebBrowser_OnBrowserMessageChanged);
                WebBrowser.Browser.AddMessageEventListener("afterSandboxEval", OnAfterSandboxEval);
                WebBrowser.Browser.CreateWindow += Browser_CreateWindow;
            }
            catch { }
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

        public override async void Dispose()
        {
            try
            {
                try
                {
                    if (macroPlayer != null)
                    {
                        if (macroPlayer.IsRunning)
                        {
                            macroPlayer.StopRequested = true;
                            macroPlayer.IsRunning = false;
                            await Task.Run(()=>Thread.Sleep(500));
                        }
                        try
                        {
                            macroPlayer.semaphoreSlim.Release();
                        }
                        catch { }
                        try
                        {
                            macroPlayer.semaphoreSlim.Dispose();
                        }
                        catch { }
                    }
                    if (JSMacroPlayer != null)
                    {
                        using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
                        {
                            IntPtr pUnk = Marshal.GetIUnknownForObject(JSMacroPlayer);
                            Marshal.Release(pUnk);
                        }
                    }
                }
                catch { }
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


        #region macros
        //public class MacroPlayerClassFactory : nsIFactory
        //{
        //    MacroPlayerClass thisClass;
        //    public MacroPlayerClassFactory(MacroPlayerClass ccc)
        //    {
        //        thisClass = ccc;

        //    }
        //    public IntPtr CreateInstance(nsISupports aOuter, ref Guid iid)
        //    {
        //        if (aOuter != null)
        //            Marshal.ThrowExceptionForHR(GeckoError.NS_ERROR_NO_AGGREGATION);

        //        IntPtr pvv;
        //        IntPtr pUnk = Marshal.GetIUnknownForObject(thisClass);
        //        try
        //        {
        //            Marshal.ThrowExceptionForHR(Marshal.QueryInterface(pUnk, ref iid, out pvv));
        //        }
        //        finally
        //        {
        //            Marshal.Release(pUnk);
        //        }
        //        return pvv;
        //    }

        //    public void LockFactory(bool @lock)
        //    {

        //    }
        //}

        //// if you want you use a custom com interface one has to register it with firefox
        //// see interfaces in https://developer.mozilla.org/en/Chrome_Registration#manifest
        //// to produce a xpt file one has to convert a idl file to xpt.
        //public class MacroPlayerClass : nsIMacroPlayer
        //{
        //    private static MacroPlayerClass instance;
        //    public static MacroPlayerClass Instance
        //    {
        //        get
        //        {
        //            if (instance == null) instance = new MacroPlayerClass();
        //            return instance;
        //        }
        //    }

        //    public void logSomething()
        //    {
        //    }

        //    public void playMacro([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.Interface)] nsIDOMDocument aDocument, [MarshalAs(UnmanagedType.LPStr)] string text)
        //    {
        //    }

        //    public void macroDone([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.Interface)] nsIDOMDocument aDocument)
        //    {
        //    }
        //}

        //public class MacrosComClassFactory : nsIFactory
        //{
        //    public IntPtr CreateInstance(nsISupports aOuter, ref Guid iid)
        //    {
        //        var obj = new MacroCommandExecutions();
        //        return Marshal.GetIUnknownForObject(obj);
        //    }

        //    public void LockFactory(bool @lock)
        //    {

        //    }
        //}

        // if you want you use a custom com interface one has to register it with firefox
        // see interfaces in https://developer.mozilla.org/en/Chrome_Registration#manifest
        // to produce a xpt file one has to convert a idl file to xpt.


        #region from jsAddon
        private void OnAfterSandboxEval(string obj)
        {
            if (macroPlayer == null) return;
            macroPlayer.IsRunning = false;
            runningInJsMode = false;
        }

        private void OnIIMSet(string values)
        {
            if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

            string[] vals = values.Split(new string[] { "{[|!1001!|]}" }, StringSplitOptions.None);
            if (macVals[vals[0]] == null) macVals.MacroVariablesValues.Add(vals[0].ToUpper(), "");
            macVals[vals[0]] = vals[1];
        }

        private void OniIImGetVal(string variable)
        {
            if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

            if (!variable.StartsWith("{{")) variable = "{{" + variable;
            if (!variable.EndsWith("}}")) variable = variable + "}}";

            string valueReturned = GetMacroVariableAfterDynamicCheck(variable, macVals);
            JSMacroPlayer.setVariableMessage("iimGetVal", valueReturned != null ? valueReturned : "undefined");
        }

        private void MacVals_OnSetExtract()
        {
            try
            {
                JSMacroPlayer.setVariableMessage("iimExtract", macVals[MacroVariables.EXTRACT]);
            }
            catch { }
        }

        private async Task OnIIMPlay(string code)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

                string codeOrPath = code;
                if (codeOrPath.StartsWith("CODE:"))
                {
                    codeOrPath = codeOrPath.Substring(codeOrPath.IndexOf("CODE:") + 5);
                    codeOrPath = codeOrPath.Trim();
                }
                else if (codeOrPath.StartsWith("Code:"))
                {
                    codeOrPath = codeOrPath.Substring(codeOrPath.IndexOf("Code:") + 5);
                    codeOrPath = codeOrPath.Trim();
                }
                else
                {
                    if (!codeOrPath.Contains(".iim")) codeOrPath = codeOrPath + ".iim";
                    var path = codeOrPath;
                    if (!File.Exists(path)) path = MacroSettings.DefaulFoldertMacros + "\\" + codeOrPath;
                    if (!File.Exists(path)) path = macroPlayer.FileDirectory + "\\" + codeOrPath;
                    if (!File.Exists(path)) return;

                    codeOrPath = File.ReadAllText(path);
                }

                macroPlayer.MacroPlayer.InitMacroCommandsList(codeOrPath);
                await RunMacro(macroPlayer, IIMPlayType.macroFromjs, 1, macVals);

                if (macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

                bool timeout = await CheckBrowserBusyTillTimeOut(macVals, WebBrowser.Browser);
                if (!timeout)
                {
                    WebBrowser.Browser.Stop();
                }
                JSMacroPlayer.macroDone(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument);
            });
        }

        private async Task OnIIMPlayCode(string code)
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;
                macroPlayer.MacroPlayer.InitMacroCommandsList(code);
                await RunMacro(macroPlayer, IIMPlayType.macroFromjs, 1, macVals);

                if (macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

                bool timeout = await CheckBrowserBusyTillTimeOut(macVals, WebBrowser.Browser);
                if (!timeout)
                {
                    WebBrowser.Browser.Stop();
                }
                JSMacroPlayer.macroDone(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument);
            });
        }
        #endregion
        public class MacroCommandExecutions : nsICommandHandler
        {
            public FoxTabViewModel ParentVM { get; set; }

            //public System.Windows.Forms.Form DummyForm;
            //System.Windows.Forms.ApplicationContext context;

            public string Exec(string aCommand, string aParameters)
            {
                // MacroFromJsRunner runner = new MacroFromJsRunner() { ParamsCode = aParameters, CommandCode = aCommand, ParentVM = ParentVM };

                switch (aCommand)
                {
                    case "iimPlayCode":
                        ParentVM.OnIIMPlayCode(aParameters);
                        break;

                    case "iimPlay":
                        ParentVM.OnIIMPlay(aParameters);
                        break;

                    case "iimSet":
                        ParentVM.OnIIMSet(aParameters);
                        break;

                    case "iimDisplay":
                        ParentVM.WebBrowser_OnBrowserMessageChanged(aParameters);
                        break;

                    case "afterSandboxEval":
                        ParentVM.OnAfterSandboxEval(aParameters);
                        break;

                    default:
                        break;
                }
                //app.Run();

                //Application.Current.Run(new Window());
                //try
                //{
                //    //DummyForm = new System.Windows.Forms.Form();
                //    //System.Windows.Forms.Application.Run(DummyForm);

                //}
                //catch
                //{
                //    //while (ParentVM.isrunningiimInJsMode)
                //    //{
                //    //    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                //    //}
                //}

                //  System.Windows.Threading.Dispatcher.Run();

                // return GetExecAsync(aCommand,aParameters).Result;

                //using (var ct = new AutoJSContext(ParentVM.WebBrowser.Browser.Window))
                //{



                //var instance = Xpcom.GetService<nsIThreadManager>("@mozilla.org/thread-manager;1");
                //var cur = instance.GetCurrentThreadAttribute();
                //while (ParentVM.isrunningiimInJsMode)
                //{
                //    bool processed = cur.ProcessNextEvent(true);
                //}




                //}

                //var instance = Xpcom.GetService<nsIThreadManager>("@mozilla.org/thread-manager;1").AsComPtr();
                //var cur = instance.Instance.NewThread(0, 0).AsComPtr();
                //runner.workThread = cur;
                //cur.Instance.Dispatch(runner, 1);

                //MacroFromJsSleeper sleeper = new MacroFromJsSleeper() { ParamsCode = aParameters, CommandCode = aCommand, ParentVM = ParentVM };
                //var cur2 = instance.Instance.NewThread(0, 0).AsComPtr();
                //sleeper.workThread = cur2;
                //while (ParentVM.isrunningiimInJsMode)
                //{
                //    cur2.Instance.Dispatch(sleeper, 1);
                //}


                //while (ParentVM.isrunningiimInJsMode)
                //{
                //    runner.workThread.Instance.ProcessNextEvent(true);
                //}

                //return GetExecAsync(aCommand,aParameters).Result;
                //while (ParentVM.isrunningiimInJsMode)
                //{
                //    if (!ParentVM.isrunningiimInJsMode)
                //    {
                //        break;
                //    }
                //    //Thread.Sleep(0);
                //    //System.Windows.Forms.Application.DoEvents();
                //    // ParentVM.WebBrowserHost.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                //    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                //    //Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,new ThreadStart(delegate { }));
                //    // Thread.Yield();
                //}

                //CallAwait();

                return null;
            }

            private void CallAwait()
            {
                var task = Task.Run(async () => { await yolo(); });
                task.Wait();
            }

            async Task yolo()
            {
                await Task.Run(() => { while (ParentVM.isrunningInJsMode) Thread.Sleep(1000); });
            }
            public class MacroFromJsSleeper : nsIRunnable
            {
                public FoxTabViewModel ParentVM { get; set; }

                public string ParamsCode { get; set; }
                public string CommandCode { get; set; }

                public ComPtr<nsIThread> workThread { get; set; }

                object mlock = new object();

                public void Run()
                {
                    lock (mlock)
                    {
                        Thread.Sleep(500);
                    }
                }
            }

            public class MacroFromJsRunner : nsIRunnable
            {
                public FoxTabViewModel ParentVM { get; set; }

                public string ParamsCode { get; set; }
                public string CommandCode { get; set; }

                public ComPtr<nsIThread> workThread { get; set; }

                object mlock = new object();

                public void Run()
                {
                    lock (mlock)
                    {
                        //switch (CommandCode)
                        //{
                        //    case "iimPlayCode":
                        //         ParentVM.OnIIMPlayCode(ParamsCode);
                        //        break;

                        //    case "iimPlay":
                        //         ParentVM.OnIIMPlay(ParamsCode);
                        //        break;

                        //    case "iimSet":
                        //        ParentVM.OnIIMSet(ParamsCode);
                        //        break;

                        //    case "iimDisplay":
                        //        ParentVM.WebBrowser_OnBrowserMessageChanged(ParamsCode);
                        //        break;

                        //    case "afterSandboxEval":
                        //        ParentVM.OnAfterSandboxEval(ParamsCode);
                        //        break;

                        //    default:
                        //        break;
                        //}
                        string val = GetExecAsync(CommandCode, ParamsCode).Result;

                        //while (ParentVM.isrunningiimInJsMode) { Thread.Sleep(250); }

                        //workThread.Instance.Shutdown();
                    }
                }

                private async Task<string> GetExecAsync(string aCommand, string aParameters)
                {
                        switch (aCommand)
                        {
                            case "iimPlayCode":
                                await ParentVM.OnIIMPlayCode(aParameters);
                                break;

                            case "iimPlay":
                                await ParentVM.OnIIMPlay(aParameters);
                                break;

                            case "iimSet":
                                ParentVM.OnIIMSet(aParameters);
                                break;

                            case "iimDisplay":
                                ParentVM.WebBrowser_OnBrowserMessageChanged(aParameters);
                                break;

                            case "afterSandboxEval":
                                ParentVM.OnAfterSandboxEval(aParameters);
                                break;

                            default:
                                break;
                        }
                    return null;
                }
            }

            //private async Task<string> GetExecAsync(string aCommand, string aParameters)
            //{
            //    using (var ct = new AutoJSContext(ParentVM.WebBrowser.Browser.Window))
            //    {
            //        switch (aCommand)
            //        {
            //            case "iimPlayCode":
            //                await ParentVM.OnIIMPlayCode(aParameters);
            //                break;

            //            case "iimPlay":
            //                await ParentVM.OnIIMPlay(aParameters);
            //                break;

            //            case "iimSet":
            //                ParentVM.OnIIMSet(aParameters);
            //                break;

            //            case "iimDisplay":
            //                ParentVM.WebBrowser_OnBrowserMessageChanged(aParameters);
            //                break;

            //            case "afterSandboxEval":
            //                ParentVM.OnAfterSandboxEval(aParameters);
            //                break;

            //            default:
            //                break;
            //        }
            //    }
            //    return null;
            //}

            public string Query(string aCommand, string aParameters)
            {
                return null;
            }
        }

        nsIMacroPlayer JSMacroPlayer;
        MacroCommandExecutions JSMacroCommandCallbacks;
        List<EventHandler<LauncherDialogEvent>> handlers = new List<EventHandler<LauncherDialogEvent>>();
        MacroManger macroPlayer;
        MacroVariables macVals = new MacroVariables();
        FFBrowserPopup ffpopupMacros;
        GeckoWebBrowser ffpopupMacrosBrowser;
        bool startedTimer = false, setfromframe = false, setfromwindow = false,wasSetFromWindow = false;
        public bool isrunningiimInJsMode;
        private bool isrunningInJsMode;
        public bool runningInJsMode
        {
            get { return isrunningInJsMode; }
            set
            {
                isrunningInJsMode = value;
                WebBrowser.InMacroPlaying = value;
            }
        }
        public bool InStopRequest
        {
            get
            {
                if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning)
                {
                    if (runningInJsMode && JSMacroPlayer != null) JSMacroPlayer.macroDone(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument);
                    if (!runningInJsMode || macroPlayer.StopRequested || !macroPlayer.IsRunning)
                    {
                        macroPlayer.IsRunning = false;
                        if (macroPlayer.StopRequested) runningInJsMode = false;
                    }
                    return true;
                }
                return false;
            }
        }

        //nsIXULWindow ffpopupMacrosXulWindow;
        //GeckoWebBrowser ffpopupMacrosBrowser;
        //nsIWebBrowserChrome ffpopupMacrosChromeBrowser;
        private async void Browser_CreateWindow(object sender,  GeckoCreateWindowEventArgs e)
        {
            if (!WebBrowser.InMacroPlaying) return;
            if (e.Uri.Contains("about:blank"))
            {
                e.Cancel = true;
                return;
            }
            GeckoPreferences.Default["allow_scripts_to_close_windows"] = true;

            macroPlayer.Paused = true;

            ffpopupMacrosBrowser = new GeckoWebBrowser();
            e.WebBrowser = ffpopupMacrosBrowser;
            ffpopupMacrosBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            ffpopupMacrosBrowser.Navigate(e.Uri);

            ffpopupMacros = new FFBrowserPopup();
            ffpopupMacros.SuspendLayout();
            ffpopupMacros.Controls.Add(ffpopupMacrosBrowser);
            ffpopupMacros.ResumeLayout(false);
            ffpopupMacros.PerformLayout();
            ffpopupMacros.Show();
            ffpopupMacros.TopMost = true;
            await Task.Delay(300);
            //await Task.Run(() => Thread.Sleep(300));
            ffpopupMacros.TopMost = false;

            ffpopupMacros.Focus();
            ffpopupMacrosBrowser.Focus();

            bool docComplete = false;
            ffpopupMacrosBrowser.DocumentCompleted += (s, ee) => { docComplete = true; };
            int awaitedComplete = 0;
            while (!docComplete)
            {
                await Task.Delay(500);
                //await Task.Run(() => Thread.Sleep(500));
                if (awaitedComplete++ >= 25) break;
            }

            ffpopupMacrosBrowser.WebBrowserFocus.Activate();
            while (ffpopupMacrosBrowser == null || ffpopupMacrosBrowser.Document == null) await Task.Delay(500);// await Task.Run(() => Thread.Sleep(500));
            bool timeout = await CheckBrowserBusyTillTimeOut(macVals, ffpopupMacrosBrowser);
            if (!timeout)
            {
                ffpopupMacrosBrowser.Stop();
            }
            setfromframe = setfromwindow = true;

            ffpopupMacrosBrowser.WindowClosed += (s, ee) =>
            {
                GeckoPreferences.Default["allow_scripts_to_close_windows"] = false;
                macroPlayer.Paused = true;
                try
                {
                    wasSetFromWindow = true;
                    setfromframe = setfromwindow = false;
                    ffpopupMacrosBrowser.Dispose();
                    ffpopupMacros.Dispose();
                    ffpopupMacros = null;
                    ffpopupMacrosBrowser = null;
                }
                catch { }
                macroPlayer.Paused = false;
            };

            macroPlayer.Paused = false;
        }

        public delegate bool GetAnnyPlayingJS();
        public event GetAnnyPlayingJS AnyPlaingJS;
        public override async Task OnPlayMacro(MacroManger manger, IIMPlayType type, int loop)
        {
            if (type == IIMPlayType.js && AnyPlaingJS != null)
            {
                bool anyinJS = AnyPlaingJS();
                if (anyinJS)
                {
                    MessageBox.Show("Only 1 js script can run at a time, play a .iim in the meantime or run multiple js in the macros module.");
                    manger.StopRequested = true;
                    manger.IsRunning = false;
                    return;
                }
            }
            
            await RunMacro(manger, type, loop);
            try
            {
                manger.semaphoreSlim.Release();
            }
            catch { }
        }

        MacroPromptService macropromt;
        internal async Task RunMacro(MacroManger mPlayer, IIMPlayType isiim, int times, MacroVariables variablescontinuefromjs = null)
        {
            try
            {
                if (JSMacroPlayer == null) JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;2");
                if (JSMacroCommandCallbacks == null)
                {
                    JSMacroCommandCallbacks = new MacroCommandExecutions();
                    JSMacroCommandCallbacks.ParentVM = this;
                    JSMacroPlayer.setHandler(JSMacroCommandCallbacks);
                }
                macroPlayer = mPlayer;

                StartTimer(macVals, mPlayer);
                if (macropromt == null)
                {
                    macropromt = new MacroPromptService();
                    PromptFactory.PromptServiceCreator = () => macropromt;
                }
                int totalDataSourceLines = 1;
                if (isiim != IIMPlayType.macroFromjs || macVals == null)
                {
                    macVals = new MacroVariables();
                    macVals.OnSetExtract += MacVals_OnSetExtract;
                }
                if (!setfromwindow)
                {
                    WebBrowser.Focus();
                    WebBrowser.Browser.Focus();
                }
                else
                {
                    //  ffpopupMacros.Focus();
                    ffpopupMacrosBrowser.Focus();
                }

                if (isiim == IIMPlayType.macro) mPlayer.MacroPlayer.InitMacroCommandsList(mPlayer.FileText);
                else if (isiim == IIMPlayType.js)
                {
                    runningInJsMode = true;
                    mPlayer.OnStopRequested -= MPlayer_OnStopRequested;
                    mPlayer.OnStopRequested += MPlayer_OnStopRequested;
                    try
                    {
                        JSMacroPlayer.playMacro(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument, mPlayer.FileText);
                        OnAfterSandboxEval("");
                    }
                    catch (System.Runtime.InteropServices.InvalidComObjectException)
                    {
                        JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;2");
                        JSMacroPlayer.playMacro(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument, mPlayer.FileText);
                        OnAfterSandboxEval("");
                    }
                    return;
                }

                if (isiim == IIMPlayType.macroFromjs) isrunningiimInJsMode = true;

                for (int dataCourceLine = 0; dataCourceLine < totalDataSourceLines; dataCourceLine++)
                {
                    await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
                    if (InStopRequest) return;

                    for (int i = 0; i < times; i++)
                    {
                        if (runningInJsMode)
                        {
                          if(JSMacroPlayer!=null) JSMacroPlayer.setVariableMessage("iimReturnVal", "1");
                        }
                        await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
                        if (InStopRequest) return;

                        macVals[MacroVariables.LOOP] = !runningInJsMode ? (i + 1).ToString().ToString() : (mPlayer.JSLoopPos + 1).ToString();

                        mPlayer.CurrentLoopPos = macVals[MacroVariables.LOOP];


                        GeckoDomDocument currentMacroContentDocument;
                        if (!setfromwindow) currentMacroContentDocument = WebBrowser.Browser.Document;
                        else currentMacroContentDocument = ffpopupMacrosBrowser.Document;
                        GeckoIFrameElement currentContentDocumentIframe = null;
                        GeckoFrameElement currentContentDocumentFrame = null;
                        if (!setfromwindow) setfromframe = false;

                        GeckoHtmlElement previosTagElementFound = null;

                        #region play macro
                        int retrystep = 0;
                        for (int macroIndex = 0; macroIndex < mPlayer.MacroPlayer.Macros.Count; macroIndex++)
                        {
                            if (macVals[MacroVariables.SINGLESTEP].ToUpper() == "YES") mPlayer.OnCommandFromView_Raised("MacroPause");
                            await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
                            if (InStopRequest) return;

                            mPlayer.MacroPlayer.SIMacroCommand = macroIndex;
                            Macro mac = mPlayer.MacroPlayer.Macros[macroIndex];
                            mac.SetGreen();
                            try
                            {

                                #region Before Eache Command
                                if (CheckTimeOutMax(macVals)) { mPlayer.MacroPlayer.Macros.Clear(); break; }
                                switch (macVals[MacroVariables.REPLAYSPEED])
                                {
                                    case "MEDIUM":
                                        if (await QuitableDelay(10)) return;
                                        break;// await Task.Run(() => Thread.Sleep(1000)); break;
                                    case "SLOW":
                                        if (await QuitableDelay(20)) return;
                                        break;// await Task.Run(() => Thread.Sleep(2000)); break;
                                    default: break;
                                }
                                //await Task.Run(() => Thread.Sleep(1000));
                                if (await CheckBrowserBusyTillTimeOut(macVals, WebBrowser.Browser) == false) WebBrowser.Browser.Stop();

                                if (InStopRequest) return;

                                while (WebBrowser.Browser.Document == null && !CheckTimeOutMax(macVals) && !InStopRequest) { await Task.Delay(100); }// await Task.Run(() => Thread.Sleep(250)); }

                                if (InStopRequest) return;

                                if (CheckTimeOutMax(macVals)) { mPlayer.MacroPlayer.Macros.Clear(); break; }

                                if (setfromframe &&
                                    !setfromwindow &&
                                    currentMacroContentDocument != null &&
                                    currentMacroContentDocument.Location != null &&
                                    currentMacroContentDocument.Location.Href != null) macVals[MacroVariables.URLCURRENT] = currentMacroContentDocument.Location.Href;
                                else macVals[MacroVariables.URLCURRENT] = WebBrowser.Browser.Document.Location.Href;

                                if ((string)GeckoPreferences.User["general.useragent.override"] != macVals[MacroVariables.USERAGENT])
                                {
                                    GeckoPreferences.User["general.useragent.override"] = macVals[MacroVariables.USERAGENT];
                                }

                                macropromt.OnLoginDidRetryCount = 0;
                                #endregion

                                Console.WriteLine(mac.Command + " " + mac.Value);
                                switch (mac.Command)
                                {
                                    #region ADD
                                    case MacroCommands.ADD:
                                        string ADDVARIABLE = "", ADDVALUE = "";
                                        var addVals = GetRegexMacroCommands(mac.Value)?.ToList();
                                        for (int k = 0; k < addVals.Count; k++)
                                        {
                                            var macval = addVals[k];
                                            switch (k)
                                            {
                                                case 0:
                                                    ADDVARIABLE = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;

                                                case 1:
                                                    ADDVALUE = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;

                                                default: break;
                                            }
                                        }

                                        if (macVals[ADDVARIABLE] != null)
                                        {
                                            if (macVals[ADDVARIABLE].Trim().ToUpper() == "NULL") macVals[ADDVARIABLE] = "";
                                            int nowVal = 0, newval = 0;

                                            bool convertedMacVal = int.TryParse(macVals[ADDVARIABLE], out nowVal);
                                            bool convertedNewVal = int.TryParse(ADDVALUE, out newval);

                                            if (convertedMacVal && convertedNewVal) macVals[ADDVARIABLE] = (nowVal += newval).ToString();
                                            else
                                            {
                                                if(ADDVARIABLE == MacroVariables.EXTRACT) macVals[ADDVARIABLE] = ADDVALUE;
                                                else macVals[ADDVARIABLE] += ADDVALUE;
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region BACK
                                    case MacroCommands.BACK:
                                        if (WebBrowser.Browser.CanGoBack) WebBrowser.Browser.GoBack();
                                        if (await QuitableDelay(5)) return;
                                        // await Task.Run(() => Thread.Sleep(500));
                                        break;
                                    #endregion

                                    #region CLEAR
                                    case MacroCommands.CLEAR:
                                        WebBrowser.Browser.Stop();
                                        await Task.Delay(500);
                                        //await Task.Run(() => Thread.Sleep(500));
                                        //Clears the browsers cache and all cookies. Can be useful, for example, 
                                        //to delete Web site cookies so every macro run starts at the same point.
                                        //It is also useful to use this command before doing website response measurements.
                                        nsIBrowserHistory historyMan = Xpcom.GetService<nsIBrowserHistory>(Gecko.Contracts.NavHistoryService);
                                        historyMan = Xpcom.QueryInterface<nsIBrowserHistory>(historyMan);
                                        historyMan.RemoveAllPages();
                                        ImageCache.ClearCache(true);
                                        ImageCache.ClearCache(false);
                                        nsICookieManager CookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
                                        var cookies = Xpcom.QueryInterface<nsICookieManager>(CookieMan);
                                        cookies.RemoveAll();
                                        await Task.Delay(500);
                                        //await Task.Run(() => Thread.Sleep(500));
                                        break;
                                    #endregion

                                    #region CLICK
                                    case MacroCommands.CLICK:
                                        float xx = 0, yy = 0;
                                        string X, Y;
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "X":
                                                    X = macVariableValue.Trim();
                                                    float.TryParse(X, out xx);
                                                    break;

                                                case "Y":
                                                    Y = macVariableValue.Trim();
                                                    float.TryParse(Y, out yy);
                                                    break;

                                                default: break;
                                            }
                                        }

                                        WebBrowser.Browser.Window.WindowUtils.SendMouseEvent("mousedown", xx, yy, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                        WebBrowser.Browser.Window.WindowUtils.SendMouseEvent("mouseup", xx, yy, GeckoMouseButton.Left, 2, 0, true, 0, 0);
                                        break;
                                    #endregion

                                    #region EVENT
                                    case MacroCommands.EVENT:
                                    case MacroCommands.EVENTS:
                                        //EVENT TYPE=type [SELECTOR|XPATH]=localizer [BUTTON|POINT|CHAR|KEY]=[button|point|char|key] [MODIFIERS=modifiers]
                                        await EventsCommand(mac.Value, mPlayer.MacroPlayer, macVals, (setfromframe && currentMacroContentDocument != null) ? currentMacroContentDocument : WebBrowser.Browser.Document, currentContentDocumentIframe, currentContentDocumentFrame, i);
                                        break;
                                    #endregion

                                    #region FILEDELETE
                                    case MacroCommands.FILEDELETE:
                                        // FILEDELETE NAME=c:\output\mydata.csv
                                        string path = mac.Value.Substring(mac.Value.IndexOf("NAME=") + 5).Trim().Replace("\"", "").Replace("<SP>", " ");
                                        if (File.Exists(path)) File.Delete(path);
                                        break;
                                    #endregion

                                    #region FILTER
                                    case MacroCommands.FILTER:
                                        string FILTERTYPE = "", FILTERVALUE = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "TYPE":
                                                    FILTERTYPE = macVariableValue.ToUpper();
                                                    break;

                                                case "STATUS":
                                                    FILTERVALUE = macVariableValue.ToUpper();
                                                    break;

                                                default: break;
                                            }
                                        }

                                        if (FILTERTYPE == "IMAGES")
                                        {
                                            if (FILTERVALUE == "ON")
                                            {
                                                GeckoPreferences.Default["permissions.default.image"] = (int)2;
                                            }
                                            else if (FILTERVALUE == "OFF")
                                            {
                                                GeckoPreferences.Default["permissions.default.image"] = (int)1;
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region FRAME
                                    case MacroCommands.FRAME:
                                        currentMacroContentDocument = WebBrowser.Browser.Document;
                                        setfromframe = false;
                                        currentContentDocumentIframe = null;
                                        currentContentDocumentFrame = null;
                                        GeckoDomDocument d = FrameCommandRecursive(mac.Value, macVals, currentMacroContentDocument, 0, ref currentContentDocumentIframe, ref currentContentDocumentFrame);
                                        if (d != null)
                                        {
                                            currentMacroContentDocument = d;
                                            setfromframe = true;
                                        }
                                        break;
                                    #endregion

                                    #region ONDIALOG
                                    case MacroCommands.ONDIALOG:
                                        //string POS = "", DIALOG_BUTTON = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "POS":
                                                    int atpos = 1;
                                                    int.TryParse(macVariableValue, out atpos);
                                                    macropromt.AtPos = atpos;
                                                    macropromt.POS = 0;
                                                    break;

                                                case "BUTTON":
                                                    macropromt.ButtonState = macVariableValue;
                                                    break;

                                                case "CONTENT":
                                                    macropromt.CONTENT = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region ONDOWNLOAD
                                    case MacroCommands.ONDOWNLOAD:
                                        //no support CHECKSUM
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "FOLDER":
                                                    MacroOnDownload.FOLDER = macVariableValue;
                                                    break;

                                                case "FILE":
                                                    MacroOnDownload.FILE = macVariableValue;
                                                    if (MacroOnDownload.FILE.Contains(":") || MacroOnDownload.FILE.Contains("/"))
                                                    {
                                                        MacroOnDownload.FILE = MacroOnDownload.FILE.Replace("/", "_").Replace(":", "_");
                                                    }
                                                    break;

                                                case "WAIT":
                                                    MacroOnDownload.WAIT = macVariableValue;
                                                    break;

                                                case "SIZE":
                                                    MacroOnDownload.SIZE = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }
                                        if (MacroOnDownload.FOLDER == "*") MacroOnDownload.FOLDER = MacroSettings.DefaultFolderDownloads;

                                        EventHandler<LauncherDialogEvent> onDownloads = async (sender, launcherdialoge) =>
                                        {
                                            string file = MacroOnDownload.FILE;
                                            if (file == "*") file = launcherdialoge.Filename;
                                            if (file.Contains("+") || !file.Contains("."))
                                            {
                                                file = file.Replace("+", "");
                                                file = file + launcherdialoge.Filename;
                                            }

                                            string url = launcherdialoge.Url;  //url to download
                                            string fullpath = System.IO.Path.Combine(MacroOnDownload.FOLDER, file); //destination file absolute path
                                            if (File.Exists(fullpath)) File.Delete(fullpath);
                                            await GracefullyTryDownload(fullpath, url, launcherdialoge.Mime, macVals);
                                        };
                                        if (handlers.Count > 0)
                                        {
                                            foreach (var handler in handlers)
                                            {
                                                LauncherDialog.Download -= handler;
                                            }

                                            handlers.Clear();
                                        }

                                        handlers.Add(onDownloads);
                                        LauncherDialog.Download += onDownloads;


                                        //if (WAIT.ToUpper() != "YES") continue;
                                        break;
                                    #endregion

                                    #region ONERRORDIALOG
                                    case MacroCommands.ONERRORDIALOG:
                                        //ONERRORDIALOG BUTTON=(YES|NO) CONTINUE=(YES|NO)
                                        string ERRBUTTON = "", ERRCONTINUE = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "BUTTON":
                                                    ERRBUTTON = macVariableValue;
                                                    break;

                                                case "CONTINUE":
                                                    ERRCONTINUE = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }

                                        macropromt.OnErrorButton = ERRBUTTON;
                                        macropromt.OnErrorContinue = ERRCONTINUE;
                                        break;
                                    #endregion

                                    #region ONLOGIN
                                    case MacroCommands.ONLOGIN:
                                        //ONLOGIN USER=username PASSWORD=password RETRY=[YES|NO]
                                        string LOGINUSER = "", LOGINPASS = "", LOGINRETRY = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "USER":
                                                    LOGINUSER = macVariableValue;
                                                    break;

                                                case "PASSWORD":
                                                    LOGINPASS = macVariableValue;
                                                    break;

                                                case "RETRY":
                                                    LOGINRETRY = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }

                                        macropromt.OnLoginUsername = LOGINUSER;
                                        macropromt.OnLoginPass = LOGINPASS;
                                        macropromt.OnLoginRetry = LOGINRETRY;
                                        break;
                                    #endregion

                                    #region PAUSE
                                    case MacroCommands.PAUSE:
                                        mPlayer.Paused = true;
                                        break;
                                    #endregion

                                    #region PROMPT
                                    case MacroCommands.PROMPT:
                                        string PROMPTVARTYPE = "", PROMPTMESSAGE = "", PROMPTDEFAULTVAL = "";
                                        bool showtextbox = false;
                                        var promptVals = GetRegexMacroCommands(mac.Value)?.ToList();
                                        for (int k = 0; k < promptVals.Count; k++)
                                        {
                                            var macval = promptVals[k];
                                            switch (k)
                                            {
                                                case 0:
                                                    PROMPTMESSAGE = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;

                                                case 1:
                                                    PROMPTVARTYPE = macval;
                                                    showtextbox = true;
                                                    break;

                                                case 2:
                                                    PROMPTDEFAULTVAL = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;

                                                default: break;
                                            }
                                        }

                                        MacroPromptWindow prompt = new MacroPromptWindow();
                                        prompt.tbInfo.Text = PROMPTMESSAGE;
                                        if (showtextbox) prompt.tbInput.Text = PROMPTDEFAULTVAL;
                                        else prompt.tbInput.Visibility = Visibility.Collapsed;

                                        if (prompt.ShowDialog() == true && showtextbox)
                                        {
                                            string input = prompt.tbInput.Text;
                                            if (PROMPTVARTYPE != "")
                                            {
                                                if (macVals[PROMPTVARTYPE.ToUpper()] != null) macVals[PROMPTVARTYPE.ToUpper()] = input;
                                                else macVals.MacroVariablesValues.Add(PROMPTVARTYPE.ToUpper(), input);
                                            }
                                            if (!input.IsNullOrEmpty() &&runningInJsMode)
                                            {
                                                JSMacroPlayer.setVariableMessage("iimPromptValue", input);
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region REFRESH
                                    case MacroCommands.REFRESH:
                                        //WebBrowser.Browser.Navigate(WebBrowser.Browser.Url.ToString());
                                        WebBrowser.Browser.Refresh();
                                        if (await QuitableDelay(5)) return;
                                        // await Task.Run(() => Thread.Sleep(500));
                                        break;
                                    #endregion

                                    #region SAVEAS
                                    case MacroCommands.SAVEAS:
                                        try
                                        {
                                            //SAVEAS TYPE=(CPL|MHT|HTM|TXT|EXTRACT|BMP|PNG|JPEG) FOLDER=folder_name FILE=file_name
                                            //no support for SAVEAS TYPE=EXTRACT FOLDER="C:\\My Macros\\Downloads FILE=*" format
                                            string SAVEASTYPE = "", SAVEASFOLDER = "", SAVEASFILE = "", saveasFilepath = "";
                                            await Task.Run(() =>
                                            {
                                                foreach (var macval in GetRegexMacroCommands(mac.Value))
                                                {
                                                    if (!macval.Contains("=")) continue;
                                                    var macVariable = macval.Remove(macval.IndexOf('='));
                                                    var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                                    switch (macVariable.ToUpper())
                                                    {
                                                        case "TYPE":
                                                            SAVEASTYPE = macVariableValue;
                                                            break;

                                                        case "FOLDER":
                                                            SAVEASFOLDER = macVariableValue;
                                                            break;

                                                        case "FILE":
                                                            SAVEASFILE = macVariableValue;
                                                            break;
                                                        default: break;
                                                    }
                                                }

                                                if (SAVEASFOLDER.Contains("*")) SAVEASFOLDER = MacroSettings.DefaultFolderDownloads;
                                                //if (SAVEASFOLDER.Contains("*")) SAVEASFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SAVEAS", SAVEASTYPE, browser.DocumentTitle != null ? RemoveSpecialCharacters(browser.DocumentTitle) : "default");
                                                if (SAVEASFILE.Contains("*")) SAVEASFILE = "SAVED_FILE " + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".";
                                                if (!SAVEASFILE.Contains(".")) SAVEASFILE += SAVEASTYPE.ToLower() == "cpl" ? "html" : SAVEASTYPE.ToLower();
                                                saveasFilepath = System.IO.Path.Combine(SAVEASFOLDER, SAVEASFILE);
                                                if (!Directory.Exists(SAVEASFOLDER))
                                                {
                                                    Directory.CreateDirectory(SAVEASFOLDER);
                                                }
                                                //else
                                                //{
                                                //    string folder = SAVEASFOLDER;
                                                //    if (folder.Contains("\\")) folder = folder.Substring(SAVEASFOLDER.LastIndexOf("\\"));
                                                //    folder = folder.Replace("\\", "");
                                                //    int looped = 0;
                                                //    string fold = folder;
                                                //    while (Directory.Exists(SAVEASFOLDER))
                                                //    {
                                                //        fold = looped + folder;
                                                //        looped++;
                                                //        SAVEASFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SAVEAS", fold);
                                                //        saveasFilepath = System.IO.Path.Combine(SAVEASFOLDER, SAVEASFILE);
                                                //    }
                                                //    Directory.CreateDirectory(SAVEASFOLDER);
                                                //}
                                            });
                                            switch (SAVEASTYPE.ToUpper())
                                            {
                                                case "BMP":
                                                case "PNG":
                                                case "JPEG":
                                                    ImageCreator creator = new ImageCreator(WebBrowser.Browser);
                                                    byte[] mBytes = creator.CanvasGetPngImage((uint)0, (uint)0, (uint)WebBrowser.Browser.Width, (uint)WebBrowser.Browser.Height);
                                                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(mBytes)))
                                                    {
                                                        image.Save(saveasFilepath);
                                                    }
                                                    break;

                                                case "EXTRACT":
                                                    await Task.Run(()=> { File.AppendAllText(saveasFilepath.Replace("extract", "txt"), macVals[MacroVariables.EXTRACT]); });
                                                    break;

                                                case "CPL":
                                                case "MHT":
                                                    List<Task> taskList = new List<Task>();
                                                    string ttl = currentMacroContentDocument.Title;
                                                    var document = currentMacroContentDocument as Gecko.GeckoDocument;
                                                    if (document != null)
                                                    {
                                                        if (SAVEASTYPE.ToUpper() == "CPL" && document.StyleSheets != null && document.StyleSheets.Count > 0)
                                                        {
                                                            taskList.Add(Task.Run(() =>
                                                            {
                                                                Application.Current.Dispatcher.Invoke(async () =>
                                                                {
                                                                    string saveAsCssDir = System.IO.Path.Combine(SAVEASFOLDER, "CSS");
                                                                    if (!Directory.Exists(saveAsCssDir)) Directory.CreateDirectory(saveAsCssDir);
                                                                    for (int k = 0; k < document.StyleSheets.Count; k++)
                                                                    {
                                                                        var css = document.StyleSheets[k];
                                                                        if (css.CssRules == null) continue;

                                                                        //string cssRules = "";
                                                                        //GeckoStyleSheet.StyleRuleCollection coll = css.CssRules;
                                                                        //foreach (var rule in css.CssRules)
                                                                        //{
                                                                        //    cssRules += rule + Environment.NewLine;
                                                                        //}
                                                                        List<string> rules = css.CssRules.ToList().ConvertAll(x => x.ToString());
                                                                        string fname = css.Href;
                                                                        if (fname == null)
                                                                        {
                                                                            fname = "inline " + k + ".css";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (fname.Contains("/")) fname = fname.Substring(fname.LastIndexOf("/") + 1);
                                                                            if (fname.Contains("?")) fname = fname.Remove(fname.IndexOf("?"));
                                                                        }
                                                                        while (fname.Length > 20) fname = fname.Substring(10);
                                                                        if (!fname.EndsWith(".css")) fname = fname + ".css";
                                                                        await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsCssDir, fname), rules.ToArray()));
                                                                    }
                                                                });
                                                            }));
                                                        }
                                                        if (document.Images != null && document.Images.Length > 0)
                                                        {
                                                            taskList.Add(Task.Run(() =>
                                                            {
                                                                string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "IMAGES");
                                                                if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
                                                                Application.Current.Dispatcher.Invoke(async () =>
                                                                {
                                                                    List<byte[]> alreadycreated = new List<byte[]>();
                                                                    for (int k = 0; k < document.Images.Length; k++)
                                                                    {
                                                                        var img = document.Images[k] as GeckoImageElement;

                                                                        try
                                                                        {
                                                                            //byte[] imgBytes = Gecko.Utils.SaveImageElement.ConvertGeckoImageElementToPng(browser, img, img.OffsetLeft, img.OffsetTop, img.Width, img.Height);
                                                                            string imgsrc = img.Src;
                                                                            byte[] imgBytes = null;
                                                                            string mimeType = "png";
                                                                            if (imgsrc.Contains("data:image/") && imgsrc.Contains(";base64,"))
                                                                            {
                                                                                imgsrc = imgsrc.Substring(imgsrc.IndexOf(";base64,") + ";base64,".Length);
                                                                                imgBytes = Convert.FromBase64String(imgsrc);
                                                                            }
                                                                            else
                                                                            {
                                                                                string daters = img.Src.Replace(document.Url.ToString(), "");
                                                                                string lastsrc = img.Src;
                                                                                if (lastsrc.EndsWith("/")) lastsrc = lastsrc.Remove(lastsrc.LastIndexOf("/"));
                                                                                if (lastsrc.Contains("/")) lastsrc = lastsrc.Substring(lastsrc.LastIndexOf("/"));
                                                                                if (daters.Contains(".jpg"))
                                                                                {
                                                                                    mimeType = "jpg";
                                                                                }
                                                                                if (!string.IsNullOrEmpty(document.Url.Query) || !string.IsNullOrWhiteSpace(document.Url.Query)) daters = daters.Replace(document.Url.Query, "");
                                                                                if (!string.IsNullOrEmpty(document.Url.Query) || !string.IsNullOrWhiteSpace(document.Url.Query)) daters = daters.Replace(document.Url.Query, "");
                                                                                string data;
                                                                                using (var context = new AutoJSContext(WebBrowser.Browser.Window))
                                                                                {
                                                                                    context.EvaluateScript(@"
                                                                                        function getBase64Image() 
                                                                                        {
                                                                                            var img = document.getElementById('" + img.Id + @"');
                                                                                            if(img == null || img == 'undefined')
                                                                                            {
                                                                                                    var elements = document.getElementsByTagName('img');
                                                                                                    for (var i=0; i<elements.length; i++) 
                                                                                                    {
                                                                                                    for (var j = 0; j < elements[i].attributes.length; j++)
                                                                                                    {
                                                                                                        var attrib = elements[i].attributes[j]; 
                                                                                                        if(attrib.value.toLowerCase().indexOf('" + daters.ToLower() + @"') > -1 || 
                                                                                                           attrib.value.toLowerCase().indexOf('" + img.Src.ToLower() + @"') > -1 ||
                                                                                                           attrib.value.toLowerCase().indexOf('" + lastsrc.ToLower() + @"') > -1)
                                                                                                        {
                                                                                                            img = elements[i];
                                                                                                            break;
                                                                                                        }
                                                                                                    }
                                                                                                    if(img != null)break;
                                                                                                    }
                                                                                            }
                                                                                            // Create an empty canvas element
                                                                                            var canvas = document.createElement('canvas');
                                                                                            canvas.width = img.width;
                                                                                            canvas.height = img.height;

                                                                                            // Copy the image contents to the canvas
                                                                                            var ctx = canvas.getContext('2d');
                                                                                            ctx.drawImage(img, 0, 0);

                                                                                            // Get the data-URL formatted image
                                                                                            // Firefox supports PNG and JPEG. You could check img.src to
                                                                                            // guess the original format, but be aware the using 'image/jpg'
                                                                                            // will re-encode the image.
                                                                                            return canvas.toDataURL('image/" + mimeType + @"');
                                                                                        }

                                                                                        getBase64Image();", out data);
                                                                                }
                                                                                //Console.WriteLine(img.Src.ToLower());
                                                                                //Console.WriteLine(mimeType);
                                                                                //Console.WriteLine(data);
                                                                                if (data == null || !data.StartsWith("data:image/" + "png" + @";base64,")) continue;
                                                                                imgBytes = Convert.FromBase64String(data.Substring(("data:image/" + "png" + @";base64,").Length));
                                                                            }
                                                                            if (imgBytes == null) continue;
                                                                            if (alreadycreated.Any(b => b.SequenceEqual(imgBytes))) continue;
                                                                            string alt = img.Alt, src = img.Src;
                                                                            await Task.Run(() =>
                                                                            {
                                                                                alreadycreated.Add(imgBytes);
                                                                                using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(imgBytes)))
                                                                                {
                                                                                    string imgname = alt;
                                                                                    if (string.IsNullOrEmpty(imgname) || string.IsNullOrWhiteSpace(imgname))
                                                                                    {
                                                                                        imgname = src;
                                                                                        if (imgname.Contains("/")) imgname = imgname.Substring(imgname.LastIndexOf("/") + 1);
                                                                                        if (imgname.Contains(".jpg?") || imgname.Contains(".png?")) imgname = imgname.Remove(imgname.IndexOf("?"));
                                                                                        if (!imgname.EndsWith(".png") && !imgname.EndsWith(".jpg")) imgname = imgname + mimeType;
                                                                                        if (!imgname.EndsWith(".png") && !imgname.EndsWith(".jpg")) imgname = imgname + ".png";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        imgname = imgname + k + "." + mimeType;
                                                                                    }
                                                                                    imgname = RemoveSpecialCharacters(imgname);
                                                                                    while (imgname.Length > 15)
                                                                                    {
                                                                                        imgname = imgname.Substring(1);
                                                                                    }
                                                                                    int adderfilename = 0;
                                                                                    string imgfilePathe = System.IO.Path.Combine(saveAsDir, imgname.Replace("/", ""));
                                                                                    string imnam = imgname;

                                                                                    while (File.Exists(imgfilePathe))
                                                                                    {
                                                                                        imgname = adderfilename + imnam;
                                                                                        adderfilename++;
                                                                                        imgfilePathe = System.IO.Path.Combine(saveAsDir, imgname.Replace("/", ""));
                                                                                    }
                                                                                    image.Save(imgfilePathe);
                                                                                }
                                                                            });
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                        }
                                                                    }
                                                                });
                                                            }));
                                                        }
                                                        if (SAVEASTYPE.ToUpper() == "CPL" && document.Links != null && document.Links.Length > 0)
                                                        {
                                                            taskList.Add(Task.Run(() =>
                                                            {
                                                                string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "LINKS");
                                                                if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
                                                                Application.Current.Dispatcher.Invoke(async () =>
                                                                {
                                                                    List<string> rules = document.Links.ToList().ConvertAll(x => (x as GeckoAnchorElement).Href);
                                                                    await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsDir, "links.txt"), rules.ToArray()));
                                                                });
                                                            }));
                                                        }
                                                        if (SAVEASTYPE.ToUpper() == "CPL" && document.Anchors != null && document.Anchors.Length > 0)
                                                        {
                                                            taskList.Add(Task.Run(() =>
                                                            {
                                                                string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "ANCHORS");
                                                                if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
                                                                Application.Current.Dispatcher.Invoke(async () =>
                                                                {
                                                                    List<string> rules = document.Anchors.ToList().ConvertAll(x => (x as GeckoAnchorElement).Href);
                                                                    await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsDir, "anchors.txt"), rules.ToArray()));
                                                                });
                                                            }));
                                                        }

                                                        await Task.Run(() => Task.WaitAll(taskList.ToArray()));
                                                    }

                                                    GeckoHtmlElement element = null;
                                                    GeckoElement geckoDomElement;
                                                    if (currentMacroContentDocument != null && setfromframe) geckoDomElement = currentMacroContentDocument.DocumentElement;
                                                    else geckoDomElement = WebBrowser.Browser.Document.DocumentElement;
                                                    if (geckoDomElement is GeckoHtmlElement)
                                                    {
                                                        element = (GeckoHtmlElement)geckoDomElement;
                                                        var innerHtml = element.InnerHtml;

                                                        File.WriteAllText(saveasFilepath, innerHtml);
                                                    }
                                                    break;

                                                //case "MHT":
                                                //    break;

                                                case "HTM":
                                                    List<Task> allhtml = new List<Task>();
                                                    List<string> htmlContents = new List<string>();
                                                    SaveAllHtmlPlusFrames(currentMacroContentDocument, htmlContents);
                                                    for (int j = 0; j < htmlContents.Count; j++)
                                                    {
                                                        allhtml.Add(Task.Run(() => File.WriteAllText(System.IO.Path.Combine(SAVEASFOLDER, j + SAVEASFILE), htmlContents[j])));
                                                    }
                                                    await Task.Run(() => Task.WaitAll(allhtml.ToArray()));
                                                    break;

                                                case "TXT":
                                                    GeckoHtmlElement elementtxt = null;
                                                    GeckoElement geckoDomElementtxt;
                                                    if (currentMacroContentDocument != null && setfromframe) geckoDomElementtxt = currentMacroContentDocument.DocumentElement;
                                                    else geckoDomElementtxt = WebBrowser.Browser.Document.DocumentElement;
                                                    if (geckoDomElementtxt is GeckoHtmlElement)
                                                    {
                                                        elementtxt = (GeckoHtmlElement)geckoDomElementtxt;
                                                        var innerHtml = elementtxt.TextContent;

                                                        await Task.Run(() => File.WriteAllText(saveasFilepath, innerHtml));
                                                    }
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        break;
                                    #endregion

                                    #region SCREENSHOT
                                    case MacroCommands.SCREENSHOT:
                                        //SCREENSHOT TYPE=(PAGE|BROWSER) FOLDER=folder_name FILE=file_name
                                        string SCREENSHOTTYPE = "", SCREENSHOTFOLDER = "", SCREENSHOTFILE = "", filepath = "";
                                        await Task.Run(() =>
                                        {
                                            foreach (var macval in GetRegexMacroCommands(mac.Value))
                                            {
                                                if (!macval.Contains("=")) continue;
                                                var macVariable = macval.Remove(macval.IndexOf('='));
                                                var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                                switch (macVariable.ToUpper())
                                                {
                                                    case "TYPE":
                                                        SCREENSHOTTYPE = macVariableValue;
                                                        break;

                                                    case "FOLDER":
                                                        SCREENSHOTFOLDER = macVariableValue;
                                                        break;

                                                    case "FILE":
                                                        SCREENSHOTFILE = macVariableValue;
                                                        break;
                                                    default: break;
                                                }
                                            }

                                            if (SCREENSHOTFOLDER.Contains("*")) SCREENSHOTFOLDER = MacroSettings.DefaultFolderDownloads;
                                            //if (SCREENSHOTFOLDER.Contains("*")) SCREENSHOTFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SCREENSHOT", browser.DocumentTitle != null ? browser.DocumentTitle : "default");
                                            if (SCREENSHOTFILE.Contains("*")) SCREENSHOTFILE = "SCREEN_SHOT " + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".png";
                                            filepath = System.IO.Path.Combine(SCREENSHOTFOLDER, SCREENSHOTFILE);
                                            if (!Directory.Exists(SCREENSHOTFOLDER))
                                            {
                                                Directory.CreateDirectory(SCREENSHOTFOLDER);
                                            }
                                        });
                                        if (SCREENSHOTTYPE.ToUpper() == "PAGE")
                                        {
                                            ImageCreator creator = new ImageCreator(WebBrowser.Browser);
                                            byte[] mBytes = creator.CanvasGetPngImage((uint)0, (uint)0, (uint)WebBrowser.Browser.Width, (uint)WebBrowser.Browser.Height);
                                            using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(mBytes)))
                                            {
                                                image.Save(filepath);
                                            }
                                        }
                                        else if (SCREENSHOTTYPE.ToUpper() == "BROWSER")
                                        {
                                            using (Bitmap bmpScreenCapture = new Bitmap(System.Windows.Forms.SystemInformation.WorkingArea.Width, (int)System.Windows.Forms.SystemInformation.WorkingArea.Height))
                                            {
                                                using (Graphics g = Graphics.FromImage(bmpScreenCapture))
                                                {
                                                    g.CopyFromScreen(0,
                                                                     0,
                                                                     0, 0,
                                                                     bmpScreenCapture.Size,
                                                                     CopyPixelOperation.SourceCopy);

                                                    bmpScreenCapture.Save(filepath);
                                                }
                                            }
                                        }
                                        break;
                                    #endregion

                                    #region SEARCH
                                    case MacroCommands.SEARCH:
                                        //SEARCH SOURCE=(TXT|REGEXP) IGNORE_CASE=YES EXTRACT=$1
                                        string SEARCHSOURCE = "", SEARCHIGNORE_CASE = "NO", SEARCHEXTRACT = "", textToSearchFor = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "SOURCE":
                                                    SEARCHSOURCE = macVariableValue.Contains(":") ? macVariableValue.Remove(macVariableValue.IndexOf(":")) : macVariableValue;
                                                    textToSearchFor = macVariableValue.Contains(":") ? macVariableValue.Substring(macVariableValue.IndexOf(":") + 1) : macVariableValue;
                                                    break;

                                                case "IGNORE_CASE":
                                                    SEARCHIGNORE_CASE = macVariableValue;
                                                    break;

                                                case "EXTRACT":
                                                    SEARCHEXTRACT = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }

                                        switch (SEARCHSOURCE.ToUpper())
                                        {
                                            case "TXT":
                                            case "REGEXP":
                                                int timesStepped = 0, timeoutstep = 6;
                                                int.TryParse(macVals[MacroVariables.TIMEOUT_STEP], out timeoutstep);
                                                bool found = false;
                                                while (!found && timeoutstep >= timesStepped)
                                                {
                                                    GeckoHtmlElement elementtxt = null;
                                                    GeckoElement geckoDomElementtxt;
                                                    if (currentMacroContentDocument != null && setfromframe) geckoDomElementtxt = currentMacroContentDocument.DocumentElement;
                                                    else geckoDomElementtxt = WebBrowser.Browser.Document.DocumentElement;
                                                    if (geckoDomElementtxt is GeckoHtmlElement)
                                                    {
                                                        elementtxt = (GeckoHtmlElement)geckoDomElementtxt;
                                                        var innerHtml = elementtxt.TextContent;
                                                        if (SEARCHIGNORE_CASE.ToUpper() == "YES")
                                                        {
                                                            innerHtml = innerHtml.ToLower();
                                                            if (SEARCHSOURCE.ToUpper() == "TXT") textToSearchFor = textToSearchFor.ToLower();
                                                        }

                                                        if (SEARCHSOURCE.ToUpper() == "TXT") found = innerHtml.Contains(textToSearchFor);
                                                        else
                                                        {
                                                            var matches = Regex.Split(innerHtml, textToSearchFor);
                                                            if (matches != null && matches.Length > 0)
                                                            {
                                                                found = true;
                                                                SEARCHEXTRACT = SEARCHEXTRACT.Replace("$1", matches[matches.Length == 1 ? 0 : 1]);
                                                                macVals[MacroVariables.EXTRACT] = SEARCHEXTRACT;
                                                            }
                                                        }
                                                    }

                                                    timesStepped++;
                                                    if (await QuitableDelay(10)) return;
                                                    // await Task.Run(() => Thread.Sleep(1000));
                                                }
                                                if (!found && macVals[MacroVariables.ERRORIGNORE] == "NO")
                                                {
                                                    mPlayer.MacroPlayer.Macros.Clear();
                                                }
                                                break;

                                            default:
                                                break;
                                        }
                                        break;
                                    #endregion

                                    #region SET
                                    case MacroCommands.SET:
                                    case MacroCommands.CMDLINE:
                                        string VARIABLE = "", VALUE = "";
                                        var setValues = GetRegexMacroCommands(mac.Value).ToList();
                                        for (int k = 0; k < setValues.Count; k++)
                                        {
                                            var macval = setValues[k];
                                            switch (k)
                                            {
                                                case 0:
                                                    VARIABLE = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;

                                                case 1:
                                                    VALUE = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        if (VARIABLE != "")
                                        {
                                            if (macVals[VARIABLE] == null) macVals.MacroVariablesValues.Add(VARIABLE.ToUpper(), "");
                                            if (VARIABLE != MacroVariables.DATASOURCE_LINE) macVals[VARIABLE] = VALUE;

                                            switch (VARIABLE)
                                            {
                                                case MacroVariables.CLIPBOARD:
                                                    MyFilesDatabase.SetClipboardText(macVals[VARIABLE], false);
                                                    break;

                                                case MacroVariables.DATASOURCE_LINE:
                                                    if (mac.Value.ToUpper().Contains("{{!LOOP}}"))
                                                    {
                                                        macVals[MacroVariables.DATASOURCE_LINE] = !runningInJsMode ? (dataCourceLine + 1).ToString() : (mPlayer.CurrentJSDatasourceLoopPos + 1).ToString();
                                                    }
                                                    else macVals[MacroVariables.DATASOURCE_LINE] = VALUE;
                                                    if (macVals.Columns.Count < Convert.ToInt32(macVals[MacroVariables.DATASOURCE_LINE]) || !macVals.Columns.Any(c => c.Row == Convert.ToInt32(macVals[MacroVariables.DATASOURCE_LINE]) - 1))
                                                    {
                                                        mPlayer.StopRequested = true;
                                                    }
                                                    break;

                                                case MacroVariables.DATASOURCE:
                                                    macVals.Columns.Clear();
                                                    macVals[MacroVariables.DATASOURCE_COLUMNS] = "";
                                                    PersonData pdataSource = null;
                                                    string[] lines = null;
                                                    bool wasSlideOut = false;
                                                    switch (VALUE.ToUpper())
                                                    {
                                                        case MacroVariables.MacroDatasourceValues.DATASOURCE_SLIDEOUT:
                                                            if (mPlayer.DataSourceSlideoutText.IsNullOrEmpty()) break;
                                                            lines = mPlayer.DataSourceSlideoutText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                                            wasSlideOut = true;
                                                            break;

                                                        case MacroVariables.MacroDatasourceValues.DATASOURCE_MAINPROJECTPROFILE:
                                                            pdataSource = GloableProfData.PData;
                                                            break;
                                                        case MacroVariables.MacroDatasourceValues.DATASOURCE_SELECTPROFILE:
                                                            pdataSource = await getSelectedProfile(true, mPlayer.SelectedMAcroPlayingFileName);
                                                            break;

                                                        default:
                                                            if (VALUE.ToUpper().Contains(MacroVariables.MacroDatasourceValues.DATASOURCE_PROFILENAME))
                                                            {
                                                                string profileValue = VALUE.ToUpper().Trim();
                                                                var split = profileValue.Split('=');
                                                                if (split.Length > 0)
                                                                {
                                                                    profileValue = split[1];

                                                                    if (MyFilesDatabase.HasMultipleProfiles(GloableProfData.PData.ProjectDir))
                                                                    {
                                                                        var directoryValues = MyFilesDatabase.GetSubProjectsFolders(GloableProfData.PData.ProjectDir, GloableProfData.PData.ProjectName);
                                                                        foreach (var prof in directoryValues)
                                                                        {
                                                                            string pname = prof.Key;
                                                                            if (pname.Contains("_folder_")) continue;

                                                                            if (pname.Contains("_tier_"))
                                                                                pname = pname.Replace("_tier_", "");

                                                                            if (pname.Trim().ToLower() == profileValue.ToLower())
                                                                            {
                                                                                pdataSource = MyFilesDatabase.GetSubProjectPersonData(prof.Value);
                                                                            }
                                                                        }

                                                                    }
                                                                }
                                                            }
                                                            break;
                                                    }
                                                    if (pdataSource != null)
                                                    {
                                                        string sex = pdataSource.CmbSelectedIndexSex == 0 ? "MALE" : "FEMALE";
                                                        lines = new string[]
                                                        {
                                                    pdataSource.ProjectName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.ProfileName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.ProxyIP+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.ProxyPort+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.ProxyUsername+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.ProxyPassword+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.FirstName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.LastName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.PhoneNumber+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Username+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Email+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Password+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    (pdataSource.CmbSelectedIndexSex + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    (pdataSource.CmbSelectedIndexDay + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    (pdataSource.CmbSelectedIndexMonth + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.BirthdayYear.ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    (pdataSource.CmbSelectedIndexMonth + 1).ToString() +(pdataSource.CmbSelectedIndexDay + 1).ToString() + pdataSource.BirthdayYear.ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Street+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.City+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.State+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Zip+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Country+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.WebAddress+macVals[MacroVariables.DATASOURCE_DELIMITER]+
                                                    pdataSource.Notes
                                                        };
                                                    }
                                                    if (lines == null)
                                                    {
                                                        string fpath = VALUE;
                                                        if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaulFoldertDataSources, VALUE);
                                                        if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaultFolderDownloads, VALUE);
                                                        if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaulFoldertMacros, VALUE);
                                                        if (File.Exists(fpath)) lines = File.ReadAllLines(fpath);
                                                    }
                                                    if (lines != null)
                                                    {
                                                        for (int k = 0; k < lines.Length; k++)
                                                        {
                                                            if (lines[k].IsNullOrEmpty()) continue;

                                                            string[] cols = lines[k].Split(new string[] { macVals[MacroVariables.DATASOURCE_DELIMITER] }, StringSplitOptions.None);
                                                            for (int j = 0; j < cols.Length; j++)
                                                            {
                                                                string dsclmns = macVals[MacroVariables.DATASOURCE_COLUMNS];
                                                                if (dsclmns != "") if (Convert.ToInt32(dsclmns) > j) break;
                                                                macVals.SetDSColVariablesValues(k, j, cols[j]);
                                                            }
                                                        }

                                                        if (wasSlideOut)
                                                        {
                                                            if (!runningInJsMode) totalDataSourceLines = lines.Length;
                                                            else mPlayer.DatasourceMaxLoop = lines.Length;
                                                        }
                                                    }
                                                    break;

                                                case MacroVariables.LOOP:
                                                    int cuuLoop = -1;
                                                    int.TryParse(macVals[MacroVariables.LOOP], out cuuLoop);
                                                    cuuLoop = cuuLoop - 1;
                                                    if (cuuLoop > 0)
                                                    {
                                                        i = cuuLoop;
                                                        break;
                                                    }
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        break;

                                    #endregion

                                    #region TAG
                                    case MacroCommands.TAG:
                                        //currentContentDocument = WebBrowser.Browser.Document;
                                        previosTagElementFound = await TagCommand
                                            (
                                                mac.Value, mPlayer.MacroPlayer, macVals,
                                                (setfromframe && currentMacroContentDocument != null) ? currentMacroContentDocument : WebBrowser.Browser.Document, currentContentDocumentIframe, currentContentDocumentFrame,
                                                previosTagElementFound,
                                                i
                                            );
                                        break;
                                    #endregion

                                    #region URL
                                    case MacroCommands.URL:
                                        string LINK = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "GOTO":
                                                    LINK = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }
                                        WebBrowser.Browser.Navigate(LINK.Trim());
                                        if (await QuitableDelay(10)) return;
                                        //await Task.Run(() => Thread.Sleep(1000));
                                        break;
                                    #endregion

                                    #region WAIT
                                    case MacroCommands.WAIT:
                                        string LENGTH = "1";
                                        bool hadSeconds = false;
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if(macval.ToUpper() == "SECONDS")
                                            {
                                                hadSeconds = true;
                                                continue;
                                            }
                                            if (hadSeconds)
                                            {
                                                int foundNumber = 0;
                                                bool parsed = int.TryParse(macval, out foundNumber);
                                                if (parsed)
                                                {
                                                    LENGTH = macval;
                                                    break;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            if (!macval.Contains("=")) continue;
                                            var macVariable = macval.Remove(macval.IndexOf('='));
                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                            switch (macVariable.ToUpper())
                                            {
                                                case "SECONDS":
                                                    LENGTH = macVariableValue;
                                                    break;
                                                default: break;
                                            }
                                        }
                                        int amount = Convert.ToInt32(LENGTH.Trim());
                                        for (int waiter = 0; waiter < amount; waiter++)
                                        {
                                            if (mPlayer.StopRequested || !macroPlayer.IsRunning)
                                            {
                                                if (runningInJsMode) JSMacroPlayer.macroDone(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument);
                                                break;
                                            }

                                            if (await QuitableDelay(10)) return;
                                            // await Task.Run(() => Thread.Sleep(1000));
                                        }
                                        break;
                                    #endregion

                                    #region TAB
                                    case MacroCommands.TAB:
                                        //TAB (T=n|OPEN|CLOSE|CLOSEALLOTHERS)
                                        string TAB_T = "", TAB_ACTION = "";
                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
                                        {
                                            if (macval.Contains("="))
                                            {
                                                var macVariable = macval.Remove(macval.IndexOf('='));
                                                var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                                                switch (macVariable.ToUpper())
                                                {
                                                    case "T":
                                                        TAB_T = macVariableValue;
                                                        break;
                                                    default: break;
                                                }
                                            }
                                            else
                                            {
                                                TAB_ACTION = GetMacroVariableAfterDynamicCheck(macval, macVals);
                                            }
                                        }
                                        if (!TAB_ACTION.IsNullOrEmpty())
                                        {
                                            switch (TAB_ACTION.ToUpper())
                                            {
                                                case "OPEN":
                                                    break;

                                                case "CLOSE":
                                                    //try
                                                    //{
                                                    //    if (ffpopupMacros != null)
                                                    //    {
                                                    //        ffpopupMacros.Close();
                                                    //    }
                                                    //}
                                                    //catch { }
                                                    break;

                                                case "CLOSEALLOTHERS":
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                        if (!TAB_T.IsNullOrEmpty())
                                        {
                                            int tabToSet = -1;
                                            int.TryParse(TAB_T, out tabToSet);
                                            if (tabToSet != -1)
                                            {

                                            }
                                        }
                                        break;
                                    #endregion

                                    case MacroCommands.PROXY:
                                        break;

                                    case MacroCommands.IMAGESEARCH:
                                        break;

                                    case MacroCommands.STOPWATCH:
                                        break;

                                    //case MacroCommands.SAVEITEM: with tag command
                                    //    break;

                                    //case MacroCommands.TRAY:only ie
                                    //    break;

                                    //case MacroCommands.DS: only ie
                                    //    break;

                                    //case MacroCommands.IMAGECLICK:only ie
                                    //    break;

                                    //case MacroCommands.ONCERTIFICATEDIALOG:only ie
                                    //    break;

                                    //case MacroCommands.ONINSECURECONNECTION: not on ff
                                    //    break;

                                    //case MacroCommands.ONPRINT: only ie
                                    //    break;

                                    //case MacroCommands.ONSECURITYDIALOG: only ie
                                    //    break;

                                    //case MacroCommands.ONWEBPAGEDIALOG: only ie
                                    //    break;

                                    //case MacroCommands.PRINT: only ie
                                    //    break;

                                    //case MacroCommands.SIZE: ie only
                                    //    break;

                                    default: break;
                                }

                            }
                            catch (System.Runtime.InteropServices.InvalidComObjectException)
                            {
                                if (retrystep++ <= 2)
                                {
                                    macroIndex--;
                                    if (macroIndex < 0) macroIndex = 0;
                                    try
                                    {
                                        if (!setfromwindow)
                                        {
                                            WebBrowser.Focus();
                                            WebBrowser.Browser.Focus();
                                        }
                                        else
                                        {
                                            //ffpopupMacros.Focus();
                                            ffpopupMacrosBrowser.Focus();
                                        }
                                    }
                                    catch { }
                                    continue;
                                }
                            }

                            mac.SetTransparent();
                        }
                        #endregion
                    }
                }
            }
            catch 
            {
                macroPlayer.StopRequested = true;
                JSMacroPlayer.setVariableMessage("iimReturnVal", "-1");
            }

            if (!runningInJsMode || macroPlayer.StopRequested || !macroPlayer.IsRunning)
            {
                mPlayer.IsRunning = false;
                if (macroPlayer.StopRequested) runningInJsMode = false;
            }

            if (isiim == IIMPlayType.macroFromjs)
            {
                isrunningiimInJsMode = false;
                //JSMacroCommandCallbacks.DummyForm.Close();
                //WebBrowserHost.Dispatcher.InvokeShutdown();
                //try
                //{
                //    using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
                //    {
                //        IntPtr pUnk = Marshal.GetIUnknownForObject(JSMacroCommandCallbacks);
                //        Marshal.Release(pUnk);
                //    }
                //}
                //catch { }
                //JSMacroCommandCallbacks = null;
                //GC.Collect();

                //JSMacroCommandCallbacks = new MacroCommandExecutions();
                //JSMacroCommandCallbacks.ParentVM = this;
                //JSMacroPlayer.setHandler(JSMacroCommandCallbacks);
            }
        }

        #region timers timeouts and timeout checkers
        private async void StartTimer(MacroVariables macVals, MacroManger player)
        {
            if (startedTimer) return;

            startedTimer = true;
            double runtime = 0.0;
            player.UpdateText = "0";
            while (player.IsRunning && !player.StopRequested)
            {
                if (await QuitableDelay(5)) return;

                if (player.Paused) continue;
                

                runtime += .5;
                player.UpdateText = runtime.ToString();
                macVals[MacroVariables.STOPWATCHTIME] = runtime.ToString();
            }
            startedTimer = false;
        }

        /// <summary>
        /// delay in a loop by 100ms per run
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        private async Task<bool> QuitableDelay(int times)
        {
            for (int i = 0; i < times; i++)
            {
                await Task.Delay(100);
                if (InStopRequest) return true;
            }

            return InStopRequest;
        }

        private void MPlayer_OnStopRequested()
        {
            try
            {
                WebBrowser.InMacroPlaying = false;
                JSMacroPlayer.setVariableMessage("iimReturnVal", "-101");
                JSMacroPlayer.setVariableMessage("StopRequest", "");

                //if (JSMacroPlayer != null)
                //{
                //    using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
                //    {
                //        IntPtr pUnk = Marshal.GetIUnknownForObject(JSMacroPlayer);
                //        Marshal.Release(pUnk);
                //    }
                //}
            }
            catch
            { }
            //JSMacroPlayer = null;
            GC.Collect();


            macroPlayer.OnStopRequested -= MPlayer_OnStopRequested;
            macroPlayer.OnStopRequested += MPlayer_OnStopRequested;
        }

        private async Task<bool> CheckBrowserBusyTillTimeOut(MacroVariables macVals, GeckoWebBrowser browserToCheck)
        {
            double sleepduringbusy = 0.0;
            while (browserToCheck.IsBusy || browserToCheck.IsAjaxBusy)
            {
                if (macroPlayer.StopRequested || !macroPlayer.IsRunning) break;

                // await Task.Run(() => Thread.Sleep(350));
                if (await QuitableDelay(3)) return false;
                sleepduringbusy += .25;
                if (macVals[MacroVariables.TIMEOUT_PAGE] != "")
                {
                    int timeouttime = 0;
                    int.TryParse(macVals[MacroVariables.TIMEOUT_PAGE], out timeouttime);
                    if (sleepduringbusy > timeouttime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckTimeOutMax(MacroVariables macVals)
        {
            if (macVals[MacroVariables.TIMEOUT_MACRO] != "")
            {
                int timeouttime = 0;
                double alltime = 0.0;
                int.TryParse(macVals[MacroVariables.TIMEOUT_MACRO], out timeouttime);
                double.TryParse(macVals[MacroVariables.STOPWATCHTIME], out alltime);
                if (alltime > timeouttime)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Downloads
        private async Task GracefullyTryDownload(string fullpath, string url, nsIMIMEInfo mime, MacroVariables macVals)
        {
            try
            {
                if (mime == null) throw new Exception("mime cant be null on first download going to next try.");
                await DownloadFile(fullpath, url, mime);
            }
            catch (Exception ex)
            {
                try
                {
                    await DownloadFile2After(fullpath, url, mime);
                }
                catch (Exception eex)
                {
                    try
                    {
                        await DownloaFile3LastTry(fullpath, url, mime, macVals);
                    }
                    catch { }
                }
            }
        }

        private async Task DownloaFile3LastTry(string fullpath, string url, nsIMIMEInfo mime, MacroVariables macVals)
        {
            nsICookieService CookieMan = Xpcom.GetService<nsICookieService>("@mozilla.org/cookieService;1");
            var cookies = Xpcom.QueryInterface<nsICookieService>(CookieMan);
            Marshal.ReleaseComObject(CookieMan);
            string cookie = cookies.GetCookieString(IOService.CreateNsIUri(url), null); //i've implemented my own cookie service

            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Cookie, cookie);
            webClient.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            webClient.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            //webClient.Headers.Add(HttpRequestHeader.Referer, currentContentDocument.Referrer);
            webClient.Headers.Add(HttpRequestHeader.UserAgent, macVals[MacroVariables.USERAGENT]);
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            webClient.Proxy = MyFilesDatabase.GetRequestsProxy();

            if (File.Exists(fullpath)) File.Delete(fullpath);
            await Task.Run(() => webClient.DownloadFile(url, fullpath));

            webClient.Dispose();
        }

        private async Task DownloadFile2After(string fullpath, string url, nsIMIMEInfo mime)
        {
            nsIWebBrowserPersist persist = Xpcom.GetService<nsIWebBrowserPersist>("@mozilla.org/embedding/browser/nsWebBrowserPersist;1");
            nsIURI source = IOService.CreateNsIUri(url);
            nsIURI dest = IOService.CreateNsIUri(new Uri(fullpath).AbsoluteUri);
            persist.SetPersistFlagsAttribute(2 | 32 | 16384);
            persist.SaveURI(source, null, null, 0, null, null, (nsISupports)dest, null);
            if (MacroOnDownload.WAIT.ToUpper() != "NO")
            {
                while (persist.GetCurrentStateAttribute() != (uint)nsIWebBrowserPersistConsts.PERSIST_STATE_FINISHED)
                {
                    await Task.Run(() => { Thread.Sleep(1000); });
                }
            }
        }

        private async Task DownloadFile(string fullpath, string url, nsIMIMEInfo mime)
        {
            nsILocalFile objTarget = Xpcom.CreateInstance<nsILocalFile>("@mozilla.org/file/local;1");

            using (nsAString tmp = new nsAString(MyFilesDatabase.GetBaseDir() + "\temp.tmp"))
            {
                objTarget.InitWithPath(tmp);
            }

            nsIURI source = IOService.CreateNsIUri(url);
            nsIURI dest = IOService.CreateNsIUri(new Uri(fullpath).AbsoluteUri);
            nsAStringBase t = (nsAStringBase)new nsAString(System.IO.Path.GetFileName(fullpath));

            nsIWebBrowserPersist persist = Xpcom.CreateInstance<nsIWebBrowserPersist>("@mozilla.org/embedding/browser/nsWebBrowserPersist;1");

            nsITransfer nst = Xpcom.CreateInstance<nsITransfer>("@mozilla.org/transfer;1");
            nst.Init(source, dest, t, mime, 0, null, persist, false);

            if (nst != null)
            {
                persist.SetPersistFlagsAttribute(2 | 32 | 16384);
                persist.SetProgressListenerAttribute((nsIWebProgressListener)nst);
                persist.SaveURI(source, null, null, (uint)Gecko.nsIHttpChannelConsts.REFERRER_POLICY_NO_REFERRER, null, null, (nsISupports)dest, null);
                if (MacroOnDownload.WAIT.ToUpper() != "NO")
                {
                    while (persist.GetCurrentStateAttribute() != (uint)nsIWebBrowserPersistConsts.PERSIST_STATE_FINISHED)
                    {
                        await Task.Run(() => { Thread.Sleep(1000); });
                    }
                }
            }
        }
        #endregion

        #region command reding splitting cleaning...
        /// <summary>
        /// (?<=     # Assert that it's possible to match this before the current position (positive lookbehind):
        /// ^        # The start of the string
        /// [^"]*    # Any number of non-quote characters
        /// (?:      # Match the following group...
        ///  "[^"]*  # a quote, followed by any number of non-quote characters
        ///  "[^"]*  # the same
        /// )*       # ...zero or more times (so 0, 2, 4, ... quotes will match)
        ///)         # End of lookbehind assertion.
        ///[ ]       # Match a space
        ///(?=       # Assert that it's possible to match this after the current position (positive lookahead):
        /// (?:      # Match the following group...
        ///  [^"]*"  # see above
        ///  [^"]*"  # see above
        /// )*       # ...zero or more times.
        /// [^"]*    # Match any number of non-quote characters
        /// $        # Match the end of the string
        ///)         # End of lookahead assertion
        /// </summary>
        /// <param name="value">macro</param>
        /// <returns></returns>
        private IEnumerable<string> GetRegexMacroCommands(string value)
        {
            // return Regex.Matches(value, @"(?<match>[^\s""]+)|(?<match>""[^""]*"") |(?<match>[\""].+?[\""]|[^ ]+)").Cast<Match>().Select(m => m.Groups["match"].Value).ToList();
            //return Regex.Matches(value, @"[^\s""']+|""([^""]*)""|'([^']*)'").Cast<Match>().Select(m=>m.Value).ToList();

            //return Regex.Matches(value, @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m=>m.Value).ToList();
            value = value.Replace("\\\"", "\"\"");
            //value = value.Replace("_", "oop0099poo");
            //value = value.Replace("\\", "oop00787878787899poo");
            var regs = Regex.Split(value, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            List<string> toReturn = new List<string>();
            foreach (var val in regs)
            {
                string cleanval = val;
                cleanval = cleanval.Replace("\"\"", "\\\"");
               // cleanval = cleanval.Replace("oop0099poo", "_");
               // value = value.Replace("oop00787878787899poo", "\\");
                toReturn.Add(cleanval);
            }
            return toReturn;

            //String otherThanQuote = " [^\"] ";
            //String quotedString = String.Format(" \" %s* \" ", otherThanQuote);
            //String regex = String.Format("(?x) " + // enable comments, ignore white spaces
            //        "\\s                       " + // match a comma
            //        "(?=                       " + // start positive look ahead
            //        "  (                       " + //   start group 1
            //        "    %s*                   " + //     match 'otherThanQuote' zero or more times
            //        "    %s                    " + //     match 'quotedString'
            //        "  )*                      " + //   end group 1 and repeat it zero or more times
            //        "  %s*                     " + //   match 'otherThanQuote'
            //        "  $                       " + // match the end of the string
            //        ")                         ", // stop positive look ahead
            //        otherThanQuote, quotedString, otherThanQuote);
            //return Regex.Matches(value, regex).Cast<Match>().Select(m=>m.Value).ToList();
        }

        private string GetMacroVariableAfterDynamicCheck(string macVariableValue, MacroVariables macVals)
        {
            if (macVariableValue.Contains("{{") && macVariableValue.Contains("}}"))
            {
                foreach (var val in Regex.Matches(macVariableValue, "{{.*?}}"))
                {
                    var stringVal = val.ToString();
                    if (!macVariableValue.Contains(stringVal)) continue;

                    if (stringVal.Contains("{{!COL"))
                    {
                        string theval = "", valAfterEdit = stringVal;

                        if (valAfterEdit.Contains("{{!COL_"))
                        {
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROJECTNAME, "{{!COL"+1);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROFILENAME, "{{!COL"+2);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROXYIP, "{{!COL"+3);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROXYPORT, "{{!COL"+4);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROXYUSER, "{{!COL"+5);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PROXYPASS, "{{!COL"+6);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_FIRSTNAME, "{{!COL"+7);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_LASTNAME, "{{!COL"+8);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PHONE, "{{!COL"+9);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_USERNAME, "{{!COL"+10);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_EMAIL, "{{!COL"+11);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_PASS, "{{!COL"+12);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_SEX, "{{!COL"+13);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_BIRTHDAY, "{{!COL"+14);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_BIRTHMONTH, "{{!COL"+15);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_BIRTHYEAR, "{{!COL"+16);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_BIRTHDAYFULL, "{{!COL"+17);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_STREET, "{{!COL"+18);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_CITY, "{{!COL"+19);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_STATE, "{{!COL"+20);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_ZIP, "{{!COL"+21);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_COUNTRY, "{{!COL"+22);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_WEBSITE, "{{!COL"+23);
                            valAfterEdit = valAfterEdit.Replace("{{!COL_"+ MacroVariables.MacroProjectValues.COLn_NOTES, "{{!COL"+24);
                        }
                        int colnumber = 0;
                        int.TryParse(valAfterEdit.Replace("{{!COL", "").Replace("}}", "").Replace("{{", ""), out colnumber);
                        colnumber -= 1;

                        int rowumber = 0;
                        int.TryParse(macVals.MacroVariablesValues[MacroVariables.DATASOURCE_LINE], out rowumber);
                        rowumber -= 1;

                        COLn macColmnv = macVals.Columns.FirstOrDefault(m => m.Column == colnumber && m.Row == rowumber);
                        if (macColmnv != null) theval = macColmnv.Value;

                        macVariableValue = macVariableValue.Replace(stringVal, theval);
                    }
                    else
                    {
                        macVariableValue = macVariableValue.Replace(stringVal, macVals[stringVal.Replace("{{", "").Replace("}}", "").ToUpper()]);
                    }
                }
            }

            string preClean = macVariableValue;
            macVariableValue = macVariableValue.Trim().Replace("<SP>", " ");
            if (macVariableValue.Contains("\\\""))
            {
                if (macVariableValue.StartsWith("\"")) macVariableValue = macVariableValue.Substring(1);
                if (macVariableValue.EndsWith("\"")) macVariableValue = macVariableValue.Remove(macVariableValue.Length - 1);
                macVariableValue = macVariableValue.Replace(":\"", ":");
                macVariableValue = macVariableValue.Replace("&&\"", "&&");
                macVariableValue = macVariableValue.Replace("\\\"", "\"");
            }
            else
            {
                macVariableValue = macVariableValue.Replace("\"", "");
            }


            if (macVariableValue.StartsWith("EVAL(") && macVariableValue.EndsWith(")"))
            {
                macVariableValue = macVariableValue.Replace("\"", "");
                using (var context = new AutoJSContext(WebBrowser.Browser.Window))
                {
                    string result = string.Empty;
                    string javascript = macVariableValue.Substring(macVariableValue.IndexOf("EVAL(") + "EVAL(".Length);
                    javascript = javascript.Remove(javascript.Length - 1);
                    bool success = context.EvaluateScript(javascript, out result);
                    if (success) macVariableValue = result;
                }
            }
            if (macVariableValue == "") macVariableValue = preClean;
            if (!macVariableValue.IsNullOrEmpty())
            {
                macVariableValue = macVariableValue.Replace("<BR>", "\n");
                macVariableValue = macVariableValue.Replace("<br>", "\n");
            }

            return macVariableValue;
        }

        private Dictionary<string, string> GetTagAttributesRules(string val)
        {
            Dictionary<string, string> returnedDic = new Dictionary<string, string>();
            if (val != "*" && val.Contains(":"))
            {
                if (val.Contains("&&"))
                {
                    string[] frmAttrs = val.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var att in frmAttrs)
                    {
                        //string[] kv = att.Split(':');
                        //string k = kv[0], v = kv[1];
                        string k = att.Remove(att.IndexOf(":"));
                        string v = att.Substring(att.IndexOf(":") + 1);
                        if (!returnedDic.ContainsKey(k)) returnedDic.Add(k.ToLower(), v);
                    }
                }
                else
                {
                    //string[] kv = val.Split(':');
                    //string k = kv[0], v = kv[1];
                    string k = val.Remove(val.IndexOf(":"));
                    string v = val.Substring(val.IndexOf(":") + 1);
                    if (!returnedDic.ContainsKey(k)) returnedDic.Add(k.ToLower(), v);
                }
            }
            return returnedDic;
        }

        private string GetUrlFromElement(GeckoHtmlElement elementFound, GeckoDomDocument currentContentDocument)
        {
            string link = elementFound.HasAttribute("href") ? elementFound.GetAttribute("href") :
                         elementFound.HasAttribute("src") ? elementFound.GetAttribute("src") :
                         elementFound.HasAttribute("id") ? elementFound.GetAttribute("id") :
                         "id";
            if (elementFound.GetType() == typeof(GeckoAnchorElement))
            {
                link = (elementFound as GeckoAnchorElement).Href;
            }
            else if (elementFound.GetType() == typeof(GeckoImageElement))
            {
                link = (elementFound as GeckoImageElement).Src;
            }
            else
            {
                if (!link.Contains(currentContentDocument.Location.Protocol) && !link.Contains(currentContentDocument.Location.Href))
                {
                    string fullurlPlusproto = currentContentDocument.Location.Protocol + "//" + currentContentDocument.Location.Host;
                    if (fullurlPlusproto.EndsWith("/")) fullurlPlusproto = fullurlPlusproto.Remove(fullurlPlusproto.Length - 1);
                    if (!link.StartsWith("/")) link = "/" + link;
                    link = fullurlPlusproto + link;
                }
            }

            return link;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            if (str == null) str = "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private string CleanText(string textContent)
        {
            string content = textContent;
            content = content.Replace("\t", "");
            content = content.Replace("\n", "");
            content = content.Replace("\r", "");
            content = textContent.Trim();
            return content;
        }
        #endregion

        #region Command bigMethods
        private async Task EventsCommand(string value, MacroPlayer mPlayer, MacroVariables macVals,
                                        GeckoDomDocument currentContentDocument, GeckoIFrameElement currentContentDocumentIframe, GeckoFrameElement currentContentDocumentFrame,
                                        int i, int timesStepped = 0)
        {
            await Task.Run(() => { while (macroPlayer.Paused) { Thread.Sleep(500); } });
            if (macroPlayer.StopRequested) return;

            try
            {
                var test = currentContentDocument.ActiveElement;
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException rex)
            {
                if (setfromwindow && ffpopupMacrosBrowser.Document != null) currentContentDocument = ffpopupMacrosBrowser.Document;
                else currentContentDocument = WebBrowser.Browser.Document;

                currentContentDocumentIframe = null;
                currentContentDocumentFrame = null;
            }

            #region getValues
            string ETYPE = "", SELECTOR = "", XPATH = "", BUTTON = "", POINT = "", CHAR = "", KEY = "", MODIFIERS = "";
            List<string> eventsList = GetRegexMacroCommands(value).ToList();
            foreach (var macval in eventsList)
            {
                if (!macval.Contains("=")) continue;
                var macVariable = macval.Remove(macval.IndexOf('=')).ToUpper();
                string macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                switch (macVariable.ToUpper())
                {
                    case "TYPE":
                        ETYPE = macVariableValue;
                        break;

                    case "SELECTOR":
                        SELECTOR = macVariableValue;
                        break;

                    case "XPATH":
                        XPATH = macVariableValue;
                        break;

                    case "BUTTON":
                        BUTTON = macVariableValue;
                        break;

                    case "POINT":
                    case "POINTS":
                        POINT = macVariableValue;
                        break;

                    case "CHAR":
                    case "CHARS":
                        CHAR = macVariableValue;
                        break;

                    case "KEY":
                    case "KEYS":
                        KEY = macVariableValue;
                        break;

                    case "MODIFIERS":
                        MODIFIERS = macVariableValue;
                        break;
                    default: break;
                }
            }
            #endregion

            bool haderror = true;
            bool setFromException = false;
            try
            {
                #region findelement
                GeckoElement elemnt = null;
                if (SELECTOR != "")
                {
                    GeckoElement doc = null;

                    doc = currentContentDocument.DocumentElement;
                    elemnt = doc.QuerySelector(SELECTOR);
                    if (elemnt == null)
                    {
                        GeckoElementCollection elements = null;
                        currentContentDocument.GetElementsByTagName("HTML");

                        if (elements != null && elements.Length > 0)
                        {
                            GeckoElement elmfirst = elements[0];
                            if (SELECTOR.StartsWith("HTML>")) SELECTOR = SELECTOR.Replace("HTML>", "");
                            elemnt = elmfirst.QuerySelector(SELECTOR);
                        }
                    }
                }
                else
                {
                    GeckoNode pathressult = currentContentDocument.EvaluateXPath(XPATH).GetSingleNodeValue();
                    if (pathressult != null) elemnt = pathressult as GeckoElement;
                }
                #endregion

                if (elemnt != null)
                {
                    haderror = false;
                    var htmlElm = elemnt as GeckoHtmlElement;
                    //if (htmlElm != null) htmlElm.Focus();
                    if (htmlElm != null) htmlElm.DOMHtmlElement.SetCapture(true);

                    System.Drawing.Rectangle rect = elemnt.GetBoundingClientRect();
                    float rectx = (rect.Left + rect.Right) / 2;
                    float recty = (rect.Top + rect.Bottom) / 2;

                    WindowUtils windowForMouseEvent = WebBrowser.Browser.Window.WindowUtils;
                    try
                    {
                        if (currentContentDocumentIframe != null) windowForMouseEvent = currentContentDocumentIframe.ContentWindow.WindowUtils;
                        if (currentContentDocumentFrame != null) windowForMouseEvent = currentContentDocumentFrame.ContentWindow.WindowUtils;
                    }
                    catch { windowForMouseEvent = WebBrowser.Browser.Window.WindowUtils; }

                    #region events
                    switch (ETYPE.ToUpper())
                    {
                        #region mouse
                        case MacroEventTypes.CLICK:
                            switch (BUTTON)
                            {
                                case "0":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    break;

                                case "1":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    break;

                                case "2":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case MacroEventTypes.DBLCLICK:
                            switch (BUTTON)
                            {
                                case "0":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    await Task.Delay(new Random().Next(10, 30));
                                    //await Task.Run(() => Thread.Sleep(new Random().Next(10, 30)));
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    break;

                                case "1":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    await Task.Delay(new Random().Next(10, 30));
                                    //await Task.Run(() => Thread.Sleep(new Random().Next(10, 30)));
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    break;

                                case "2":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    await Task.Delay(new Random().Next(10, 30));
                                    //await Task.Run(() => Thread.Sleep(new Random().Next(10, 30)));
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case MacroEventTypes.MOUSEDOWN:
                            switch (BUTTON)
                            {
                                case "0":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    break;

                                case "1":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Middle, 1, 0, true, 0, 0);
                                    break;

                                case "2":
                                    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Right, 1, 0, true, 0, 0);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        case MacroEventTypes.MOUSEMOVE:
                            if (POINT != "")
                            {
                                Action<string> MoveAroundPoint = (point) =>
                                {
                                    string thisPoint = point.Replace(")", "").Replace("(", "");
                                    if (!thisPoint.Contains(",")) return;
                                    int x = -10000;
                                    int.TryParse(thisPoint.Split(',')[0], out x);
                                    int y = -10000;
                                    int.TryParse(thisPoint.Split(',')[1], out y);
                                    if (y != -10000 && x != -10000)
                                    {
                                        windowForMouseEvent.SendMouseEvent("mousemove", x, y, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    }
                                };
                                string points = POINT;
                                if (points.Contains("),("))
                                {
                                    string[] poinsarr = points.Split(new string[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (poinsarr != null)
                                    {
                                        foreach (var point in poinsarr)
                                        {
                                            MoveAroundPoint(point);
                                        }
                                    }
                                }
                                else
                                {
                                    MoveAroundPoint(POINT);
                                }
                            }
                            break;

                        case MacroEventTypes.MOUSEUP:
                            // if(POINT != "")
                            // windowForMouseEvent.SendMouseEvent("mouseup", rect.X, rect.Y, GeckoMouseButton.Middle, 1, 0, false, 100, 0);
                            if (POINT != "")
                            {
                                string thisPoint = POINT.Replace(")", "").Replace("(", "");
                                if (thisPoint.Contains(","))
                                {
                                    int x = -10000;
                                    int.TryParse(thisPoint.Split(',')[0], out x);
                                    int y = -10000;
                                    int.TryParse(thisPoint.Split(',')[1], out y);
                                    if (y != -10000 && x != -10000)
                                    {
                                        windowForMouseEvent.SendMouseEvent("mouseup", x, y, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region keyboard
                        case MacroEventTypes.KEYDOWN:
                            if (htmlElm == null)
                            {
                                windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 2, 0, true, 0, 0);
                            }
                            else
                            {
                                htmlElm.Focus();
                                htmlElm.Click();
                            }

                            if (CHAR != "")
                            {
                                if (CHAR.StartsWith("\"")) CHAR = CHAR.Substring(1);
                                if (CHAR.EndsWith("\"")) CHAR = CHAR.Remove(CHAR.Length - 1);
                                foreach (var c in CHAR)
                                {
                                    windowForMouseEvent.SendKeyEvent("keydown", 0, GetJSCharCode(c), 0, false);
                                }
                            }
                            if (KEY != "")
                            {
                                if (KEY.StartsWith("\"")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("\"")) KEY = KEY.Remove(KEY.Length - 1);
                                if (KEY.StartsWith("[")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("]")) KEY = KEY.Remove(KEY.Length - 1);
                                string[] keys = KEY.Split(',');
                                foreach (var item in keys)
                                {
                                    int tringint = -1000;
                                    int.TryParse(item, out tringint);
                                    if (tringint != -1000)
                                    {
                                        windowForMouseEvent.SendKeyEvent("keydown", tringint, 0, 0, false);
                                    }
                                }
                            }
                            break;
                        case MacroEventTypes.KEYPRESS:
                            if (htmlElm == null)
                            {
                                windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 2, 0, true, 0, 0);
                            }
                            else
                            {
                                htmlElm.Focus();
                                htmlElm.Click();
                            }

                            if (CHAR != "")
                            {
                                if (CHAR.StartsWith("\"")) CHAR = CHAR.Substring(1);
                                if (CHAR.EndsWith("\"")) CHAR = CHAR.Remove(CHAR.Length - 1);
                                windowForMouseEvent.SendNativeKeyEvent(0, 0, 0, CHAR, CHAR);
                                await Task.Delay(250);
                                //await Task.Run(() => Thread.Sleep(250));
                                //foreach (var c in CHAR)
                                //{
                                //    //windowForMouseEvent.SendKeyEvent("keydown", 0, GetJSCharCode(c), 0, false);


                                //    //windowForMouseEvent.SendKeyEvent("keypress", 0, GetJSCharCode(c), 0, false);
                                //    //windowForMouseEvent.SendKeyEvent("keyup", 0, GetJSCharCode(c), 0, false);

                                //    //DomEventArgs ev = doc.CreateEvent("KeyboardEvent");
                                //    //var webEvent = new Gecko.WebIDL.KeyboardEvent(domWin, ev.DomEvent as nsISupports);//.Event(domWin, ev.DomEvent as nsISupports);//
                                //    //webEvent.SetProperty("charCode", (uint)GetJSCharCode(c));
                                //    ////webEvent.InitEvent("keypress", true, true);
                                //    //elemnt.GetEventTarget().DispatchEvent(ev);
                                //}
                            }
                            if (KEY != "")
                            {
                                if (KEY.StartsWith("\"")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("\"")) KEY = KEY.Remove(KEY.Length - 1);
                                if (KEY.StartsWith("[")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("]")) KEY = KEY.Remove(KEY.Length - 1);
                                string[] keys = KEY.Split(',');
                                foreach (var item in keys)
                                {
                                    int tringint = -1000;
                                    int.TryParse(item, out tringint);
                                    if (tringint != -1000)
                                    {
                                        windowForMouseEvent.SendKeyEvent("keypress", tringint, 0, 0, false);
                                    }
                                }
                            }
                            break;
                        case MacroEventTypes.KEYUP:
                            if (htmlElm == null)
                            {
                                windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 2, 0, true, 0, 0);
                            }
                            else
                            {
                                htmlElm.Focus();
                                htmlElm.Click();
                            }
                            //windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                            //windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 2, 0, true, 0, 0);
                            if (CHAR != "")
                            {
                                if (CHAR.StartsWith("\"")) CHAR = CHAR.Substring(1);
                                if (CHAR.EndsWith("\"")) CHAR = CHAR.Remove(CHAR.Length - 1);
                                foreach (var c in CHAR)
                                {
                                    windowForMouseEvent.SendKeyEvent("keyup", 0, GetJSCharCode(c), 0, false);
                                }
                            }
                            if (KEY != "")
                            {
                                if (KEY.StartsWith("\"")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("\"")) KEY = KEY.Remove(KEY.Length - 1);
                                if (KEY.StartsWith("[")) KEY = KEY.Substring(1);
                                if (KEY.EndsWith("]")) KEY = KEY.Remove(KEY.Length - 1);
                                string[] keys = KEY.Split(',');
                                foreach (var item in keys)
                                {
                                    int tringint = -1000;
                                    int.TryParse(item, out tringint);
                                    if (tringint != -1000)
                                    {
                                        windowForMouseEvent.SendKeyEvent("keyup", tringint, 0, 0, false);
                                    }
                                }
                            }
                            break;
                        #endregion

                        default: break;
                    }

                    if (htmlElm != null) htmlElm.DOMHtmlElement.ReleaseCapture();
                    #endregion

                    Console.WriteLine("Event Success");
                }
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException rex)
            {
                haderror = true;
                setFromException = true;
                if (setfromwindow && ffpopupMacrosBrowser.Document != null) currentContentDocument = ffpopupMacrosBrowser.Document;
                else currentContentDocument = WebBrowser.Browser.Document;

                currentContentDocumentIframe = null;
                currentContentDocumentFrame = null;
            }
            catch (Exception ex)
            {
                haderror = true;
            }

            if (haderror)
            {
                Console.WriteLine("Event Error");
                if (!setFromException)
                {
                    if (setfromwindow && ffpopupMacrosBrowser.Document != null)
                    {
                        currentContentDocument = ffpopupMacrosBrowser.Document;
                    }

                    if (wasSetFromWindow)
                    {
                        currentContentDocument = WebBrowser.Browser.Document;
                        wasSetFromWindow = false;
                    }
                }
                int timeoutstep = 6;
                int.TryParse(macVals[MacroVariables.TIMEOUT_STEP], out timeoutstep);
                if (timeoutstep >= timesStepped)
                {
                    timesStepped++;
                    for (int td = 0;td < 10; td++)
                    {
                        await Task.Delay(100);
                        if (InStopRequest) return;
                    }
                    await EventsCommand(value, mPlayer, macVals, currentContentDocument, currentContentDocumentIframe, currentContentDocumentFrame, i, timesStepped);
                    return;
                }
                if (macVals[MacroVariables.ERRORIGNORE] == "NO")
                {
                   if(!runningInJsMode) mPlayer.Macros.Clear();
                    JSMacroPlayer.setVariableMessage("iimReturnVal", "-1");
                }
            }
        }

        private GeckoDomDocument FrameCommandRecursive(string elmntnameornum, MacroVariables macVals, GeckoDomDocument currentContentDocument, int upto,
                                                       ref GeckoIFrameElement currentContentDocumentIframe, ref GeckoFrameElement currentContentDocumentFrame)
        {
            string F = "", NAME = "";
            int frameNum = 0;
            foreach (var macval in GetRegexMacroCommands(elmntnameornum))
            {
                if (!macval.Contains("=")) continue;
                var macVariable = macval.Remove(macval.IndexOf('='));
                var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                switch (macVariable.ToUpper())
                {
                    case "F":
                        F = macVariableValue.Trim();
                        int.TryParse(F, out frameNum);
                        break;

                    case "NAME":
                        NAME = macVariableValue;
                        break;

                    default: break;
                }
            }

            var iframes = currentContentDocument.GetElementsByTagName("frame");
            if (iframes == null || iframes.Length == 0)
            {
                iframes = currentContentDocument.GetElementsByTagName("iframe");
            }
            // bool foundFrame = false;

            for (int i = 0; i < iframes.Length; i++)
            {
                GeckoFrameElement frame = iframes[i] as GeckoFrameElement;
                GeckoIFrameElement iframe = iframes[i] as GeckoIFrameElement;

                if (frame == null && iframe == null) continue;

                var innerframes = frame != null ? frame.ContentWindow.Document.GetElementsByTagName("frame") : iframe.ContentWindow.Document.GetElementsByTagName("frame");

                if (innerframes == null || innerframes.Length == 0)
                    innerframes = frame != null ? frame.ContentWindow.Document.GetElementsByTagName("iframe") : iframe.ContentWindow.Document.GetElementsByTagName("iframe");

                if (innerframes != null && innerframes.Length > 0)
                {
                    upto += i + 1;
                    currentContentDocument = FrameCommandRecursive(elmntnameornum, macVals, frame != null ? frame.ContentWindow.Document : iframe.ContentWindow.Document, upto, ref currentContentDocumentIframe, ref currentContentDocumentFrame);
                }


                if ((F != "" && frameNum == i + 1 + upto) || (NAME != "" && NAME == (frame == null ? iframe.Name : frame.Name)))
                {
                    if (iframe == null)
                    {
                        frame.Focus();
                        if (frame.ContentWindow != null)
                        {
                            // foundFrame = true;
                            currentContentDocument = frame.ContentWindow.Document;
                            currentContentDocumentFrame = frame;
                        }
                        break;
                    }
                    else
                    {
                        iframe.Focus();
                        if (iframe.ContentWindow != null)
                        {
                            //foundFrame = true;
                            currentContentDocument = iframe.ContentWindow.Document;
                            currentContentDocumentIframe = iframe;
                        }
                        break;
                    }
                }
            }

            //if (!foundFrame)
            //{
            //    upto += iframes.Length;
            //    foreach (var elm in iframes)
            //    {
            //        GeckoFrameElement frame = elm as GeckoFrameElement;

            //        if (frame.ContentWindow.Document != null && 
            //            (frame.ContentWindow.Document.GetElementsByTagName("frame").Length > 0 || frame.ContentWindow.Document.GetElementsByTagName("iframe").Length > 0))
            //            currentContentDocument = tryfindFrameRecursive(elmntnameornum, macVals, frame.ContentWindow.Document, upto, ref currentContentDocumentIframe, ref currentContentDocumentFrame);
            //    }
            //}

            return currentContentDocument;
        }

        private async Task<GeckoHtmlElement> TagCommand(string value, MacroPlayer mPlayer, MacroVariables macVals,
                                                        GeckoDomDocument currentContentDocument, GeckoIFrameElement currentContentDocumentIframe, GeckoFrameElement currentContentDocumentFrame,
                                                        GeckoHtmlElement previosTagElementFound,
                                                        int i, int timesStepped = 0)
        {
            try
            {
                var test = currentContentDocument.ActiveElement;
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException)
            {
                if (setfromwindow && ffpopupMacrosBrowser.Document != null) currentContentDocument = ffpopupMacrosBrowser.Document;
                else currentContentDocument = WebBrowser.Browser.Document;
                currentContentDocumentIframe = null;
                currentContentDocumentFrame = null;
            }
            if (macroPlayer.StopRequested)
            {
                if (runningInJsMode) JSMacroPlayer.macroDone(WebBrowser.Browser.Window.DomWindow, WebBrowser.Browser.DomDocument.NativeDomDocument);
                return previosTagElementFound;
            }
            #region initialize what to do
            string POS = "", CONTENT = "", FORM = "", ATTR = "", EXTRACT = "", XPATH = "", SELECTOR = "",
                   TYPE = "", typeCmd = "", typeVal = "";
            int posint = -1;
            Dictionary<string, string> formAttrDlist = new Dictionary<string, string>();
            Dictionary<string, string> attrDlist = new Dictionary<string, string>();
            foreach (var macval in GetRegexMacroCommands(value))
            {
                if (!macval.Contains("=")) continue;
                var macVariable = macval.Remove(macval.IndexOf('='));
                string macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
                switch (macVariable.ToUpper())
                {
                    case "POS":
                        POS = macVariableValue.ToUpper();
                        string pos = POS;
                        if (pos.StartsWith("R")) pos = pos.Replace("R", "");
                        if (!int.TryParse(pos, out posint))
                        {
                            posint = 1;
                        }
                        break;

                    case "TYPE":
                        TYPE = typeCmd = macVariableValue;
                        if (TYPE.Contains(":"))
                        {
                            typeCmd = TYPE.Remove(TYPE.IndexOf(":")).ToLower();
                            typeVal = TYPE.Split(':')[1].ToLower();
                        }
                        break;

                    case "FORM":
                        FORM = macVariableValue;
                        formAttrDlist = GetTagAttributesRules(FORM);
                        break;

                    case "ATTR":
                        ATTR = macVariableValue;
                        attrDlist = GetTagAttributesRules(ATTR);
                        if (attrDlist.Count == 0 && ATTR == "*") attrDlist.Add("anyatt", "*");
                        break;

                    case "EXTRACT":
                        EXTRACT = macVariableValue.ToUpper();
                        break;

                    case "XPATH":
                        XPATH = macVariableValue;
                        break;

                    case "SELECTOR":
                        SELECTOR = macVariableValue;
                        break;

                    case "CONTENT":
                        CONTENT = macVariableValue;
                        break;

                    default: break;
                }
            }
            #endregion

            bool haderror = true;
            bool setFromException = false;
            try
            {
                GeckoHtmlElement elementFound = null;
                if (XPATH == "" && SELECTOR == "")
                {
                    #region Find by classic search
                    var elements = currentContentDocument.GetElementsByTagName(typeCmd).ToList();
                    if (POS.Contains("R") && previosTagElementFound != null)
                    {
                        do
                        {
                            var elementscc = previosTagElementFound.GetElementsByTagName(typeCmd);
                            if (elementscc == null || elementscc.Length == 0)
                            {
                                previosTagElementFound = previosTagElementFound.Parent;
                                if (previosTagElementFound == null)
                                {
                                    break;
                                }
                                elementscc = previosTagElementFound.GetElementsByTagName(typeCmd);
                                continue;
                            }
                            else
                            {
                                if (elements != null) elements.Clear();
                                else elements = new List<GeckoHtmlElement>();
                                foreach (var elm in elementscc)
                                {
                                    var htmElm = elm as GeckoHtmlElement;
                                    if (htmElm == null) continue;
                                    elements.Add(htmElm);
                                }
                                break;
                            }
                        } while (true);
                    }

                    int foundCount = 0;
                    foreach (var matchedTagElement in elements)
                    {
                        if (formAttrDlist.Count > 0 && !FORM.Contains("NoFormName"))
                        {
                            bool? isIt = null;
                            recursiveFindForm(matchedTagElement.Parent, formAttrDlist, ref isIt);
                            if (isIt == false) continue;
                        }

                        bool foundIt = false;
                        int foundCountAttributes = 0;

                        if (attrDlist.Any(kv => kv.Key == "txt"))
                        {
                            string textContent = CleanText(matchedTagElement.TextContent).ToLower();
                            if (attrDlist["txt"].ToLower() == textContent || attrDlist["txt"] == "*") foundCountAttributes++;
                            else if (attrDlist["txt"].Contains("*") && attrDlist["txt"] != "*")
                            {
                                List<string> starvals = attrDlist["txt"].Split('*').ToList();
                                if (starvals.Count(sv => textContent.Contains(sv.ToLower())) == starvals.Count) foundCountAttributes++;
                            }

                            if (foundCountAttributes == attrDlist.Count) foundCount++;
                            foundIt = foundCount == posint || (foundCount >= posint && posint <= 1) || (posint <= 0 && foundCount > 0);
                        }

                        if (!foundIt && matchedTagElement.Attributes != null && matchedTagElement.Attributes.Length > 0)
                        {
                            bool matchedTypsOnce = false;
                            foreach (var elementAtribute in matchedTagElement.Attributes)
                            {
                                string nodeName = elementAtribute.NodeName.ToLower().Trim();
                                bool setForattribCheck = nodeName == "src" && attrDlist.Any(kv => kv.Key.ToLower() == "href") && !attrDlist.Any(kv => kv.Key.ToLower() == "src");
                                if (setForattribCheck) nodeName = "href";
                                string nodeVal = elementAtribute.NodeValue.ToLower().Trim();
                                if (!matchedTypsOnce)
                                {
                                    if (typeVal != "" && typeVal.ToLower().Trim() != nodeVal && (nodeName.ToLower() == "type" || typeVal == "submit")) continue;
                                }
                                matchedTypsOnce = true;

                                if (setForattribCheck && nodeName == "href" && !nodeVal.Contains(currentContentDocument.Location.Protocol) && !nodeVal.Contains(currentContentDocument.Location.Host))
                                {
                                    nodeVal = GetUrlFromElement(elementFound, currentContentDocument);
                                }

                                foreach (var kv in attrDlist)
                                {
                                    if (kv.Key != "*" && kv.Key != "anyatt" && kv.Key.ToLower() != nodeName && kv.Value != "null") continue;
                                    else if (kv.Value == "null" && !matchedTagElement.Attributes.Any(mte => mte.NodeName.ToLower().Trim() == kv.Key.ToLower().Trim()))
                                    {
                                        foundCountAttributes++;
                                        continue;
                                    }

                                    bool containsall = false;
                                    if (kv.Value.Contains("*") && kv.Value != "*")
                                    {
                                        List<string> starvals = kv.Value.Split('*').ToList();
                                        containsall = starvals.Count(sv => nodeVal.Contains(sv.ToLower())) == starvals.Count;
                                    }

                                    if (containsall || kv.Value == "*" ||
                                        (kv.Value.ToLower().Trim() == nodeVal.Trim()) || 
                                        (kv.Value.Contains("*") && kv.Value != "*" && nodeVal.Trim().Contains(kv.Value.Replace("*", "").Trim())))
                                    {
                                        foundCountAttributes++;
                                        if (foundCountAttributes == attrDlist.Count) foundCount++;
                                        foundIt = foundCount == posint || (foundCount >= posint && posint <= 1) || (posint <= 0 && foundCount > 0);
                                        if (foundIt) break;
                                    }
                                }

                                if (foundIt) break;
                            }
                        }

                        if (foundIt)
                        {
                            elementFound = matchedTagElement;
                            break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Find by xpath or selector
                    if (SELECTOR != "")
                    {
                        GeckoElementCollection elements = currentContentDocument.GetElementsByTagName("HTML");

                        if (elements != null && elements.Length > 0)
                        {
                            GeckoElement elmfirst = elements[0];
                            if (SELECTOR.StartsWith("HTML>")) SELECTOR = SELECTOR.Replace("HTML>", "");
                            elementFound = elmfirst.QuerySelector(SELECTOR) as GeckoHtmlElement;
                        }
                    }
                    else
                    {
                        GeckoNode pathressult = currentContentDocument.EvaluateXPath(XPATH).GetSingleNodeValue();
                        if (pathressult != null) elementFound = pathressult as GeckoHtmlElement;
                    }
                    #endregion
                }

                if (elementFound != null)
                {
                    haderror = false;
                    previosTagElementFound = elementFound;
                    if (MacroSettings.IsHighlightWhenFoundChecked)
                    {
                        try
                        {
                            elementFound.Style.SetPropertyValue("outline", "1px solid blue");
                        }
                        catch { }
                    }

                    WindowUtils windowForMouseEvent = currentContentDocument.DefaultView.WindowUtils;
                    try
                    {
                        if (currentContentDocumentIframe != null) windowForMouseEvent = currentContentDocumentIframe.ContentWindow.WindowUtils;
                        if (currentContentDocumentFrame != null) windowForMouseEvent = currentContentDocumentFrame.ContentWindow.WindowUtils;
                    }
                    catch { windowForMouseEvent = currentContentDocument.DefaultView.WindowUtils; }
                    if (MacroSettings.IsScrollWhenFoundChecked)
                    {
                        try
                        {
                            int scrollto = GetParentOffsetsForElement(elementFound);
                            if (currentContentDocument.DocumentElement != null && (currentContentDocument.DocumentElement.ScrollTop + WebBrowser.Browser.Height) < scrollto)
                            {
                                WebBrowser.Browser.Window.ScrollTo(elementFound.OffsetLeft, scrollto * 2);
                                await Task.Delay(100);
                                //await Task.Run(() => Thread.Sleep(100));
                                WebBrowser.Browser.Window.ScrollTo(elementFound.OffsetLeft, (scrollto - (WebBrowser.Browser.Height / 2)));
                                await Task.Delay(100);
                                //await Task.Run(() => Thread.Sleep(100));
                            }
                            elementFound.ScrollIntoView(false);
                            elementFound.Focus();
                            await Task.Delay(100);
                           // await Task.Run(() => Thread.Sleep(100));
                        }
                        catch { }
                    }

                    System.Drawing.Rectangle rect = elementFound.GetBoundingClientRect();
                    float rectx = (rect.Left + rect.Right) / 2;
                    float recty = (rect.Top + rect.Bottom) / 2;
                    //await Task.Run(() => Thread.Sleep(1000));

                    if (EXTRACT == "")
                    {
                        #region TAG EVENT
                        if (!string.IsNullOrEmpty(CONTENT) && !string.IsNullOrWhiteSpace(CONTENT) && CONTENT.ToUpper().Contains("EVENT:"))
                        {
                            string eventType = CONTENT.ToUpper();
                            var arrsplit = eventType.Split(new string[] { "EVENT:" }, StringSplitOptions.None);
                            if (arrsplit.Length > 1)
                            {
                                eventType = arrsplit[1];
                                string imgfilename = GetUrlFromElement(elementFound, currentContentDocument);
                                string file = MacroOnDownload.FILE;
                                if (file == "*") file = RemoveSpecialCharacters(imgfilename);
                                if (file.Contains("+") || !file.Contains("."))
                                {

                                    if (imgfilename.EndsWith("/")) imgfilename = imgfilename.Remove(imgfilename.Length - 1);
                                    if (imgfilename.Contains("/")) imgfilename = imgfilename.Substring(imgfilename.LastIndexOf("/") + 1);
                                    if (imgfilename.Count(c => c == '.') == 1)
                                    {
                                        if (file.Contains("+"))
                                        {
                                            file = file.Replace("+", imgfilename.Remove(imgfilename.IndexOf('.')));
                                            file = file + imgfilename.Substring(imgfilename.IndexOf('.'));
                                        }
                                        else
                                        {
                                            file = file + imgfilename;
                                        }
                                    }
                                    else
                                    {
                                        file = file.Replace("+", "");
                                        file = file + ".png";
                                    }
                                }
                                string fullpath = MacroOnDownload.FOLDER;
                                if (fullpath == "*") fullpath = MacroSettings.DefaultFolderDownloads;
                                // string fullpath = System.IO.Path.Combine(MacroOnDownload.FOLDER, "EVENTSAVES", RemoveSpecialCharacters(browser.DocumentTitle));
                                if (!Directory.Exists(fullpath)) Directory.CreateDirectory(fullpath);
                                fullpath = fullpath + "\\" + file;

                                switch (eventType)
                                {
                                    case "SAVEITEM":
                                    case "SAVE_ELEMENT_SCREENSHOT":
                                    case "SAVEPICTUREAS":
                                        ImageCreator creator = new ImageCreator(WebBrowser.Browser);
                                        byte[] mBytes = creator.CanvasGetPngImage((uint)elementFound.OffsetLeft, (uint)elementFound.OffsetTop, (uint)elementFound.OffsetWidth, (uint)elementFound.OffsetHeight);
                                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(mBytes)))
                                        {
                                            image.Save(fullpath);
                                        }
                                        break;

                                    case "SAVETARGETAS":
                                        if (elementFound.HasAttribute("href") || elementFound.HasAttribute("src"))
                                        {
                                            string link = GetUrlFromElement(elementFound, currentContentDocument);

                                            try
                                            {
                                                var uri = IOService.CreateNsIUri(link);

                                                var instance = Xpcom.GetService<nsIMIMEService>("@mozilla.org/mime;1");
                                                var mimeService = Xpcom.QueryInterface<nsIMIMEService>(instance);
                                                Marshal.ReleaseComObject(instance);
                                                var mime = mimeService.GetFromTypeAndExtension(new nsACString(""), new nsAUTF8String(System.IO.Path.GetExtension(fullpath)));

                                                await GracefullyTryDownload(fullpath, link, mime, macVals);
                                            }
                                            catch
                                            {
                                                await GracefullyTryDownload(fullpath, link, null, macVals);
                                            }
                                        }
                                        break;

                                    case "MOUSEOVER":
                                        windowForMouseEvent.SendMouseEvent("mouseover", rectx, recty, GeckoMouseButton.None, 1, 0, true, 0, 0);
                                        //try
                                        //{
                                        //    var document = currentContentDocument as GeckoDocument;
                                        //    DomEventArgs ev = document.CreateEvent("MouseEvent");
                                        //    var webEvent = new Gecko.WebIDL.Event(browser.Window.DomWindow, ev.DomEvent as nsISupports);
                                        //    webEvent.InitEvent("mouseover", true, true);
                                        //    elementFound.GetEventTarget().DispatchEvent(ev);
                                        //}
                                        //catch { }
                                        break;

                                    case "FAIL_IF_FOUND":
                                        throw new Exception("FAIL_IF_FOUND event");

                                    default:
                                        break;
                                }
                            }
                        }
                        #endregion
                        #region TAG REGULAR
                        else
                        {
                            CONTENT = CONTENT.Replace("\\n", Environment.NewLine);
                            CONTENT = CONTENT.Replace("\\t", "   ");

                            GeckoInputElement input = elementFound as GeckoInputElement;
                            if (input != null && typeVal.ToUpper() != "FILE" &&(input.Type == "radio" || input.Type == "checkbox"))
                            {
                                if (!string.IsNullOrEmpty(CONTENT) && !string.IsNullOrWhiteSpace(CONTENT))
                                {
                                    if ((CONTENT == "YES" && !input.Checked) || (CONTENT == "NO" && input.Checked))
                                        elementFound.Click();
                                }
                                else
                                {
                                    elementFound.Click();
                                }
                            }
                            else if (elementFound.GetType() == typeof(GeckoSelectElement))
                            {
                                var selec = elementFound as GeckoSelectElement;
                                selec.Click();
                                selec.Value = CONTENT.Replace("%","");
                                if (selec.Value == "")
                                {
                                    string contentval = CONTENT;
                                    if (contentval.StartsWith("%")) contentval = contentval.Substring(1);
                                    contentval = contentval.Replace("&quot;", "\"");
                                    foreach (var opt in selec.ChildNodes)
                                    {
                                        var option = opt as GeckoOptionElement;
                                        if (option == null) continue;

                                        if (option.Value != null && option.Value.ToLower().Trim() == contentval.ToLower().Trim())
                                        {
                                            option.Selected = true;
                                            break;
                                        }
                                    }
                                }
                                selec.Blur();
                            }
                            else
                            {
                                if (typeVal.ToUpper() == "FILE")
                                {
                                    elementFound.Click();

                                    MacroManger.AnyRunning = true;
                                    MacroFilePicker.Instance.FilePath = CONTENT;
                                    //if (MacroFilePicker.Instance.utils != null || MacroFilePicker.Instance.aFilePickerShownCallback != null)
                                    //{
                                    //    try
                                    //    {
                                    //        using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
                                    //        {
                                    //            if (MacroFilePicker.Instance.utils != null)
                                    //            {
                                    //                IntPtr pUnk = Marshal.GetIUnknownForObject(MacroFilePicker.Instance.utils);
                                    //                Marshal.Release(pUnk);
                                    //            }
                                    //            else
                                    //            {
                                    //                IntPtr pUnk2 = Marshal.GetIUnknownForObject(MacroFilePicker.Instance.aFilePickerShownCallback);
                                    //                Marshal.Release(pUnk2);
                                    //            }
                                    //            GC.Collect();
                                    //        }
                                    //    }
                                    //    catch { }
                                    //}
                                    MacroFilePicker.Instance.utils = Xpcom.QueryInterface<nsIDOMWindowUtils>(WebBrowser.Browser.Window.DomWindow);
                                    MacroFilePicker.Instance.aFilePickerShownCallback.Done(nsIFilePickerConsts.returnOK);
                                    if (await QuitableDelay(5) == false) 
                                    MacroFilePicker.Instance.aFilePickerShownCallback.Done(nsIFilePickerConsts.returnOK);
                                }
                                else
                                {
                                    if (elementFound.GetType() == typeof(GeckoAnchorElement) &&
                                        currentContentDocument.Location != null && currentContentDocument.Location.Href != null && 
                                        currentContentDocument.Location.Href.Contains("facebook") && !currentContentDocument.Location.Href.Contains("ifttt.com"))
                                    {
                                        windowForMouseEvent.SendMouseEvent("mouseover", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                        windowForMouseEvent.SendMouseEvent("mousedown", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                        windowForMouseEvent.SendMouseEvent("mouseup", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                    }
                                    else if (elementFound.GetType() == typeof(GeckoAnchorElement) && WebBrowser.Browser.Url != null && WebBrowser.Browser.Url.AbsoluteUri.ToLower().Contains("iftt"))
                                    {
                                        //if (currentContentDocument.Location.Href.Contains("facebook"))
                                        //{
                                        //    windowForMouseEvent.SendMouseEvent("mouseover", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                        //    windowForMouseEvent.SendMouseEvent("mousedown", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                        //    windowForMouseEvent.SendMouseEvent("mouseup", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                        //}
                                        //else
                                        //{
                                        try
                                        {
                                            GeckoAnchorElement btnelm = elementFound as GeckoAnchorElement;
                                            DomEventArgs ev = WebBrowser.Browser.Document.CreateEvent("MouseEvent");
                                            var webEvent = new Gecko.WebIDL.Event(WebBrowser.Browser.Window.DomWindow, ev.DomEvent as nsISupports);
                                            webEvent.InitEvent("mousedown", true, true);
                                            btnelm.GetEventTarget().DispatchEvent(ev);
                                            await Task.Delay(75);
                                           // await Task.Run(() => Thread.Sleep(75));
                                            webEvent.InitEvent("mouseup", true, true);
                                            btnelm.GetEventTarget().DispatchEvent(ev);

                                            //await Task.Run(() => Thread.Sleep(10));
                                            await Task.Delay(10);
                                            btnelm.Click();
                                        }
                                        catch
                                        {
                                            elementFound.Click();
                                            // await Task.Run(() => Thread.Sleep(10));
                                            await Task.Delay(100);
                                            elementFound.Click();
                                        }
                                        //}
                                    }
                                    //else if (elementFound.GetType() == typeof(GeckoAnchorElement))
                                    //{
                                    //    windowForMouseEvent.SendMouseEvent("mouseover", rectx, recty, GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                    //    windowForMouseEvent.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    //    windowForMouseEvent.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                    //}
                                    else
                                    {
                                       if(elementFound.GetType() != typeof(GeckoButtonElement)) elementFound.Click();
                                    }
                                }

                                if (typeVal.ToUpper() != "FILE" && !string.IsNullOrEmpty(CONTENT) && !string.IsNullOrWhiteSpace(CONTENT))
                                {
                                    if (null == input)
                                    {
                                        GeckoTextAreaElement textArea = elementFound as GeckoTextAreaElement;
                                        if (null == textArea && elementFound.HasAttribute("value")) elementFound.SetAttribute("value", CONTENT);
                                        else textArea.Value = CONTENT;
                                    }
                                    else input.Value = CONTENT;

                                }

                                if (input != null)
                                {
                                    DomEventArgs ev = WebBrowser.Browser.Document.CreateEvent("Event");
                                    var webEvent = new Gecko.WebIDL.Event(WebBrowser.Browser.Window.DomWindow, ev.DomEvent as nsISupports);
                                    webEvent.InitEvent("change", true, true);
                                    input.GetEventTarget().DispatchEvent(ev);
                                }
                                if (elementFound.GetType() == typeof(GeckoButtonElement))
                                {
                                    //if (currentContentDocument.Location.Href.Contains("facebook"))
                                    //{
                                    //    windowForMouseEvent.SendMouseEvent("mouseover", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                    //    windowForMouseEvent.SendMouseEvent("mousedown", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 1, 0, true, 0, 0);
                                    //    windowForMouseEvent.SendMouseEvent("mouseup", elementFound.GetBoundingClientRect().X + 2, (elementFound.GetBoundingClientRect().Y + 2), GeckoMouseButton.Left, 0, 0, false, 0, 0);
                                    //}
                                    //else
                                    //{
                                    GeckoButtonElement btnelm = elementFound as GeckoButtonElement;
                                    DomEventArgs ev = WebBrowser.Browser.Document.CreateEvent("MouseEvent");
                                    var webEvent = new Gecko.WebIDL.Event(WebBrowser.Browser.Window.DomWindow, ev.DomEvent as nsISupports);
                                    webEvent.InitEvent("mousedown", true, true);
                                    btnelm.GetEventTarget().DispatchEvent(ev);
                                    await Task.Delay(75);
                                    //await Task.Run(() => Thread.Sleep(75));
                                    webEvent.InitEvent("mouseup", true, true);
                                    btnelm.GetEventTarget().DispatchEvent(ev);
                                    await Task.Delay(25);
                                    //await Task.Run(() => Thread.Sleep(25));
                                    btnelm.Click();
                                    //}
                                }



                                //elementFound.Click();
                                elementFound.Blur();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region TAG EXTRACT
                        string extracted = "";
                        if (EXTRACT != "TXT" && EXTRACT != "TXTALL")
                        {
                            if (EXTRACT == "HTM" || EXTRACT == "HTML")
                            {
                                extracted = elementFound.OuterHtml;
                            }
                            else
                            {
                                foreach (var item in elementFound.Attributes)
                                {
                                    if (item.NodeName != null && item.NodeName.ToLower() == EXTRACT.ToLower())
                                    {
                                        extracted = item.NodeValue;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (EXTRACT == "TXT" || EXTRACT == "TXTALL")
                        {
                            if (TYPE.ToUpper() == "TABLE" || elementFound.NodeName.ToUpper() == "TABLE")
                            {
                                extracted = "";
                                foreach (var elementHead in elementFound.GetElementsByTagName("thead"))
                                {
                                    if (elementHead == null) continue;

                                    foreach (var element in elementHead.GetElementsByTagName("tr"))
                                    {
                                        if (element == null) continue;
                                        foreach (var elementHeadDef in element.GetElementsByTagName("th"))
                                        {
                                            if (elementHeadDef == null) continue;
                                            extracted += CleanText(elementHeadDef.TextContent) + ",";
                                        }

                                        if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                        extracted += Environment.NewLine;
                                    }

                                    if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                    extracted += Environment.NewLine;
                                }

                                extracted += Environment.NewLine;

                                foreach (var elementBody in elementFound.GetElementsByTagName("tbody"))
                                {
                                    if (elementBody == null) continue;

                                    foreach (var elementRow in elementBody.GetElementsByTagName("tr"))
                                    {
                                        if (elementRow == null) continue;
                                        foreach (var elementColumn in elementRow.GetElementsByTagName("td"))
                                        {
                                            if (elementColumn == null) continue;
                                            extracted += CleanText(elementColumn.TextContent) + ",";
                                        }

                                        if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                        extracted += Environment.NewLine;
                                    }

                                    if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                    extracted += Environment.NewLine;
                                }

                                if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                if (string.IsNullOrEmpty(extracted) || string.IsNullOrWhiteSpace(extracted)) extracted = elementFound.TextContent;
                            }
                            else
                            {
                                extracted = elementFound.TextContent;
                                if (string.IsNullOrEmpty(extracted) || string.IsNullOrWhiteSpace(extracted))
                                {
                                    if (elementFound.GetType() == typeof(GeckoInputElement))
                                    {
                                        extracted = (elementFound as GeckoInputElement).Value;
                                        if (extracted == "") extracted = (elementFound as GeckoInputElement).TextContent;
                                    }
                                    else if (elementFound.GetType() == typeof(GeckoTextAreaElement))
                                    {
                                        extracted = (elementFound as GeckoTextAreaElement).Value;
                                        if (extracted == "") extracted = (elementFound as GeckoTextAreaElement).TextContent;
                                    }
                                    else if (elementFound.GetType() == typeof(GeckoSelectElement))
                                    {
                                        if (EXTRACT == "TXT")
                                        {
                                            extracted = (elementFound as GeckoSelectElement).Value;
                                            if (extracted == "") extracted = (elementFound as GeckoSelectElement).TextContent;
                                        }
                                        else
                                        {
                                            var allOptions = (elementFound as GeckoSelectElement).Options;
                                            for (int k = 0; k < allOptions.Length; k++)
                                            {
                                                extracted += allOptions.item((uint)k) + ",";
                                            }
                                            extracted = extracted.Trim();
                                            if (extracted.EndsWith(",")) extracted = extracted.Remove(extracted.Length - 1);
                                        }
                                    }
                                    else
                                    {
                                        if (elementFound.HasAttribute("value")) extracted = elementFound.GetAttribute("value");
                                        else if (elementFound.HasAttribute("text")) extracted = elementFound.GetAttribute("text");
                                    }
                                }
                            }
                        }

                        if (extracted != null)
                        {
                            extracted = extracted + Environment.NewLine;
                            if (macVals[MacroVariables.EXTRACT] == "NULL")
                            {
                                macVals[MacroVariables.EXTRACT] = extracted;
                            }
                            else
                            {
                                macVals[MacroVariables.EXTRACT] += "," + extracted;
                            }
                        }
                        if (!runningInJsMode && macVals[MacroVariables.EXTRACT_TEST_POPUP] == "YES")
                        {
                            FlexibleMessageBox.Show(extracted, true);
                        }
                        #endregion
                    }

                    // browser.Window.ScrollTo(elementFound.OffsetLeft, scrollto);
                    Console.WriteLine("Tag success");
                }
            }
            catch (System.Runtime.InteropServices.InvalidComObjectException rex)
            {
                setFromException = true;
                haderror = true;
                if (setfromwindow && ffpopupMacrosBrowser.Document != null) currentContentDocument = ffpopupMacrosBrowser.Document;
                else currentContentDocument = WebBrowser.Browser.Document;

                currentContentDocumentIframe = null;
                currentContentDocumentFrame = null;
            }
            catch (Exception ex)
            {
                haderror = true;
            }

            if (haderror)
            {
                Console.WriteLine("Tag error");
                if (!setFromException)
                {
                    if (setfromwindow && ffpopupMacrosBrowser.Document != null)
                    {
                        currentContentDocument = ffpopupMacrosBrowser.Document;
                    }

                    if (wasSetFromWindow)
                    {
                        currentContentDocument = WebBrowser.Browser.Document;
                        wasSetFromWindow = false;
                    }
                }

                int timeoutstep = 6;
                int.TryParse(macVals[MacroVariables.TIMEOUT_STEP], out timeoutstep);
                if (timeoutstep > 0 && timeoutstep >= timesStepped)
                {
                    timesStepped++;
                    if (await QuitableDelay(10)) return previosTagElementFound;
                    if (timesStepped - timeoutstep == 0)
                    {
                        currentContentDocument = WebBrowser.Browser.Document;
                    }
                    //await Task.Run(() => Thread.Sleep(1000));
                    previosTagElementFound = await TagCommand(value, mPlayer, macVals, currentContentDocument, currentContentDocumentIframe, currentContentDocumentFrame, previosTagElementFound, i, timesStepped);
                    return previosTagElementFound;
                }
                if (macVals[MacroVariables.ERRORIGNORE] == "NO")
                {
                    JSMacroPlayer.setVariableMessage("iimReturnVal", "-1");
                    if (!runningInJsMode) mPlayer.Macros.Clear();
                    if (await QuitableDelay(10)) return previosTagElementFound;
                    // await Task.Run(() => Thread.Sleep(1000));
                }
            }

            return previosTagElementFound;
        }

        private void recursiveFindForm(GeckoHtmlElement input, Dictionary<string, string> formAttrDlist, ref bool? isIt)
        {
            if (input == null) return;

            if (input.NodeName.ToUpper() == "FORM")
            {
                if (isIt == null) isIt = false;
                GeckoFormElement frm = input as GeckoFormElement;
                int foundCount = 0;
                foreach (var kvattr in formAttrDlist)
                {
                    foreach (var attr in frm.Attributes)
                    {
                        if ((attr.LocalName.ToLower() == kvattr.Key.ToLower() && attr.NodeValue.ToLower() == kvattr.Value.ToLower()))
                        {
                            foundCount++;
                            break;
                        }
                    }
                }

                if (foundCount == formAttrDlist.Count)
                {
                    isIt = true;
                    return;
                }
            }
            else
            {
                if (input.Parent != null)
                {
                    recursiveFindForm(input.Parent, formAttrDlist, ref isIt);
                }
            }
        }
        #endregion

        private int GetParentOffsetsForElement(GeckoHtmlElement elm)
        {
            int offset = 0;
            do
            {
                offset += elm.OffsetTop;
                elm = elm.OffsetParent;
            } while (elm != null);

            return offset;
        }

        private void SaveAllHtmlPlusFrames(GeckoDomDocument currentContentDocument, List<string> contents)
        {
            GeckoHtmlElement element = null;
            var geckoDomElement = currentContentDocument.DocumentElement;
            if (geckoDomElement is GeckoHtmlElement)
            {
                element = (GeckoHtmlElement)geckoDomElement;
                var innerHtml = element.InnerHtml;
                contents.Add(innerHtml);
            }

            var frames = currentContentDocument.GetElementsByTagName("frame");
            if (frames != null)
            {
                foreach (var f in frames)
                {
                    GeckoFrameElement frame = f as GeckoFrameElement;
                    if (frame == null || frame.ContentWindow == null || frame.ContentWindow.Document == null || frame.ContentWindow.Document.DocumentElement == null) continue;

                    SaveAllHtmlPlusFrames(frame.ContentWindow.Document, contents);
                }
            }
            var iframes = currentContentDocument.GetElementsByTagName("iframe");
            if (iframes != null)
            {
                foreach (var f in iframes)
                {
                    GeckoFrameElement frame = f as GeckoFrameElement;
                    if (frame == null || frame.ContentWindow == null || frame.ContentWindow.Document == null || frame.ContentWindow.Document.DocumentElement == null) continue;

                    SaveAllHtmlPlusFrames(frame.ContentWindow.Document, contents);
                }
            }
        }

        private int GetJSCharCode(char c)
        {
            Dictionary<char, int> chars = new Dictionary<char, int>();
            chars.Add('0', 48);
            chars.Add('1', 49);
            chars.Add('2', 50);
            chars.Add('3', 51);
            chars.Add('4', 52);
            chars.Add('5', 53);
            chars.Add('6', 54);
            chars.Add('7', 55);
            chars.Add('8', 56);
            chars.Add('9', 57);
            chars.Add('a', 65);
            chars.Add('b', 66);
            chars.Add('c', 67);
            chars.Add('d', 68);
            chars.Add('e', 69);
            chars.Add('f', 70);
            chars.Add('g', 71);
            chars.Add('h', 72);
            chars.Add('i', 73);
            chars.Add('j', 74);
            chars.Add('k', 75);
            chars.Add('l', 76);
            chars.Add('m', 77);
            chars.Add('n', 78);
            chars.Add('o', 79);
            chars.Add('p', 80);
            chars.Add('q', 81);
            chars.Add('r', 82);
            chars.Add('s', 83);
            chars.Add('t', 84);
            chars.Add('u', 85);
            chars.Add('v', 86);
            chars.Add('w', 87);
            chars.Add('x', 88);
            chars.Add('y', 89);
            chars.Add('z', 90);

            if (chars.ContainsKey(char.ToLower(c))) return char.IsLower(c) ? chars[c] + 32 : chars[char.ToLower(c)];
            else return 32;
        }
        #endregion
    }
}

 ////nsIPrincipal instance = Xpcom.CreateInstance<nsIPrincipal>("@mozilla.org/systemprincipal;1");
 //                       //var princeipal = Xpcom.QueryInterface<nsIPrincipal>(instance);
 //                       //Marshal.ReleaseComObject(instance);

 //                       //var sandboxInstance = Xpcom.XPConnect.Instance.CreateSandbox(Xpcom.XPConnect.Instance.GetSafeJSContext(), null);
 //                       //var val = Xpcom.XPConnect.Instance.EvalInSandboxObject(new nsAString(js), null, Xpcom.XPConnect.Instance.GetSafeJSContext(), sandboxInstance, 0);

 //                       using (var context = new AutoJSContext(WebBrowser.Browser.Window))
 //                       {
 //                           //context.EvaluateScript("alert(window.Worker)");
 //                          // var instance = Xpcom.GetService<nsIThreadManager>("@mozilla.org/thread-manager;1");
 //                           // var current = instance.GetCurrentThreadAttribute();
 //                          // var suups = (nsISupports)instance;
 //                           var vall = context.EvaluateScript(js);

 //                           //nsIPrincipal instance = Xpcom.CreateInstance<nsIPrincipal>("@mozilla.org/systemprincipal;1");
 //                           // var princeipal = Xpcom.QueryInterface<nsIPrincipal>(instance);
 //                           //Marshal.ReleaseComObject(instance);

 //                           //var sandboxInstance = Xpcom.XPConnect.Instance.CreateSandbox(context.ContextPointer, null);
 //                           //var jsstring = new nsAString("var iii = 0;");
 //                           //var val = Xpcom.XPConnect.Instance.EvalInSandboxObject(jsstring, null, context.ContextPointer, sandboxInstance, 185);

 //                           // var jsValue = new JsVal();
 //                           //SpiderMonkey.JS_ExecuteScript(context.ContextPointer, "function yolo() { return Components; }; yolo();", out jsValue);
 //                           // jsValue = SpiderMonkey.JS_CallFunctionName(context.ContextPointer, IntPtr.Zero, "myfunc", new[] { jsValue });

 //                           //nsIXPCComponents componentsinstance = Xpcom.CreateInstance<nsIXPCComponents>(jsValue.ToObject());
 //                           //var nsIXPCComponents = Xpcom.QueryInterface<nsIXPCComponents>(componentsinstance);

 //                           //componentsinstance..GetUtilsAttribute().

 //                           //var ptr = (IntPtr)Xpcom.GetService(new Guid("CB6593E0-F9B2-11d2-BDD6-000064657374"));
 //                           //var nsIXPCComponents = (nsIXPConnect)Xpcom.GetObjectForIUnknown(ptr);

 //                           // var ptr = (IntPtr)Xpcom.GetService(new Guid("CB6593E0-F9B2-11d2-BDD6-000064657374"));
 //                           //var nsIXPCComponents = (nsIXPConnect)Xpcom.GetObjectForIUnknown(ptr);

 //                           //Gecko.Interop.ComPtr<nsIXPConnect> nsIXPCComponents = Xpcom.GetService<nsIXPConnect>("@mozilla.org/js/xpc/XPConnect;1").AsComPtr();
 //                           //var sandboxInstance = nsIXPCComponents.Instance.CreateSandbox(context.ContextPointer, princeipal);
 //                           //var val = Xpcom.XPConnect.Instance.EvalInSandboxObject(new nsAString(js), null, context.ContextPointer, sandboxInstance, 0);


 //                           //var please = context.GetComponentsObject();

 //                           //var ptr = Xpcom.CreateInstance<nsIXPCComponents>("aa28aaf6-70ce-4b03-9514-afe43c7dfda8");
 //                           ////var nsIXPCComponents = (nsIXPCComponents)Xpcom.GetObjectForIUnknown(ptr);

 //                           // Gecko.Interop.ComPtr<nsIXPConnect> nsIXPCComponents = Xpcom.GetService<nsIXPConnect>("@mozilla.org/js/xpc/XPConnect;1").AsComPtr();
 //                           //nsIXPCComponents.Instance.

 //                           // var jsValue = context.EvaluateScript("function yolo() { return Components; }; yolo();");
 //                           // nsIXPCComponents nsIXPCComponents = Xpcom.QueryInterface<nsIXPCComponents>(jsValue.ToObject());
 //                           // var utils = nsIXPCComponents.GetUtilsAttribute();

 //                           // var sandbox = Xpcom.XPConnect.Instance.CreateSandbox(context.ContextPointer, null);

 //                          // context.EvaluateScript("var Cu  = require('chrome'); alert(Cu);");
 //                       }