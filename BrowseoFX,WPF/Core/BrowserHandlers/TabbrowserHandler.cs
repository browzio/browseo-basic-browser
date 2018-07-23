using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Gecko;
using Gecko.Interop;
using Gecko.DOM;
using BrowseoFX_WPF.Core.DataAccess;
using BrowseoFX_WPF.Core.Services;
using Gecko.DOM.HTML;
using SocialOrganizer.Models;
using System.Threading;
using Organiser.Common.Windows;
using Organiser.Common.Classes;
using DragDropListview;
using Organiser.Common;
using Gecko.Javascript;

namespace BrowseoFX_WPF.Core.BrowserHandlers
{
    public class TabbrowserHandler:
        nsIDOMEventListener,
        IDisposable
    {
        private GeckoXULElement tabbrowser;
        private GeckoXULElement tabsContainer;

        private GeckoWindow gloableWebViewWindow;

        public IList<string> TabEvents
        {
            get
            {
                return new List<string>(new string[] {
                    "TabOpen",
                    "TabClose",
                    "TabSelect",
                    "TabShow",
                    "TabHide",
                    "TabPinned",
                    "TabUnpinned",
                    "SSTabClosing",
                    "SSTabRestoring",
                    "SSTabRestored",
                    "visibilitychange",
                });
            }
        }


        #region cpontop
        //Thread CPWthread;
        //int lastProfileIndex;
        //public async void OpenCP()
        //{
        //    PersonData profile = await getSelectedProfile(false);
        //    if (profile == null) return;

        //    if (CPWthread == null || !CPWthread.IsAlive)
        //    {
        //        CPWthread = new Thread(() =>
        //        {
        //            CreateProjectWindow projWindow = new CreateProjectWindow();
        //            projWindow.DataContext = profile;
        //            if (!MyFilesDatabase.CanSeeProxys)
        //            {
        //                projWindow.tbProxys.Visibility = System.Windows.Visibility.Collapsed;
        //                projWindow.dpProxys.Visibility = System.Windows.Visibility.Collapsed;
        //            }
        //            projWindow.projName.Text = profile.ProfileName;
        //            projWindow.Topmost = true;
        //            projWindow.WindowStyle = System.Windows.WindowStyle.None;
        //            projWindow.AllowsTransparency = true;
        //            projWindow.Opacity = 0.9;
        //            projWindow.grdinfo.Opacity = 0.9;
        //            projWindow.tbbutton.Text = "Close";
        //            projWindow.IsReadOnly = true;
        //            projWindow.cbSex.IsEnabled = projWindow.spBirth.IsEnabled = projWindow.spPBN.IsEnabled =
        //            projWindow.spMoney.IsEnabled = projWindow.cmbMoney.IsEnabled = projWindow.cmbPbn.IsEnabled = false;
        //            projWindow.Closed += ProjWindow_Closed;
        //            projWindow.ShowDialog();
        //        });

        //        CPWthread.SetApartmentState(ApartmentState.STA);
        //        CPWthread.Start();
        //    }
        //}

        //public async Task<PersonData> getSelectedProfile(bool isfromMacro, string imacroName = "")
        //{
        //    return await Task<PersonData>.Factory.StartNew(() =>
        //    {
        //        PersonData profile = ObjectCopier.DeepClone<PersonData>(GloableProfData.PData);
        //        //try
        //        //{
        //        //    if (DragDropMainViewModel.Instance.FoldersAndSitesList != null && DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].TypeOfFolder == FolderTypes.Import)
        //        //    {
        //        //        string urltoCheck = AddressEditable.Substring(AddressEditable.IndexOf('.') + 1);
        //        //        if (urltoCheck.Contains("."))
        //        //            urltoCheck = urltoCheck.Split('.')[0];
        //        //        foreach (Bookmark b in DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].Sites)
        //        //        {
        //        //            if (!b.IsImported) continue;
        //        //            string blinkToChek = b.Link.Substring(b.Link.IndexOf('.') + 1);
        //        //            if (blinkToChek.Contains("."))
        //        //                blinkToChek = blinkToChek.Split('.')[0];
        //        //            if (urltoCheck.Contains(blinkToChek))
        //        //            {
        //        //                profile.Username = b.Username;
        //        //                profile.Email = b.Email;
        //        //                profile.Password = b.Password;
        //        //                return profile;
        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //catch { }

        //        if (MyFilesDatabase.HasMultipleProfiles(GloableProfData.PData.ProjectDir))
        //        {
        //            SelectProfileWindow selectProfile = new SelectProfileWindow(GloableProfData.PData.ProjectName, GloableProfData.PData.ProjectDir, lastProfileIndex, "");
        //            selectProfile.FromMacro = isfromMacro;
        //            if (isfromMacro) selectProfile.Title = "For " + imacroName;
        //            if (isfromMacro)
        //            {
        //                selectProfile.Topmost = true;
        //                selectProfile.Topmost = false;
        //            }
        //            selectProfile.ShowDialog();
        //            if (!selectProfile.OkClicked)
        //            {
        //                return null;
        //            }
        //            lastProfileIndex = selectProfile.cmProfiles.SelectedIndex;
        //            profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
        //        }

        //        return profile;
        //    }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        //}

        //private void ProjWindow_Closed(object sender, EventArgs e)
        //{
        //    CPWthread = null;
        //}
        #endregion

        //BrowseoFXManager.GloableWebView.Window
        public GeckoWindow GloableWebViewWindow
        {
            get
            {
                if (gloableWebViewWindow == null) gloableWebViewWindow = BrowseoFXManager.Instance.GloableWebView.Window;
                return gloableWebViewWindow;
            }
        }

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
                if(tabbrowser == null) tabbrowser = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("content") as GeckoXULElement;
                return tabbrowser;
            }
        }

        /// <summary>
        /// <tabs id="tabbrowser-tabs"
        ///       class="tabbrowser-tabs"
        ///       tabbrowser="content"
        ///       flex="1"
        ///       setfocus="false"
        ///       tooltip="tabbrowser-tab-tooltip"
        ///       stopwatchid="FX_TAB_CLICK_MS">
        ///     <tab class="tabbrowser-tab" selected="true" visuallyselected="true" fadein="true"/>
        /// </tabs>
        /// </summary>
        public GeckoXULElement TabsContainer
        {
            get
            {
                if(tabsContainer == null) tabsContainer = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("tabbrowser-tabs") as GeckoXULElement;
                return tabsContainer;
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

        public nsIDOMNode SelectedTab
        {
            get
            {
                return Tabbrowser.GetProperty<nsIDOMNode>("selectedTab");
            }
            set
            {
                Tabbrowser.SetProperty<nsIDOMNode>("selectedTab", value);
            }
        }
        public nsIDOMNodeList Tabs
        {
            get
            {
                return TabsContainer.GetProperty<nsIDOMNodeList>("childNodes");
            }
            set
            {
                TabsContainer.SetProperty<nsIDOMNodeList>("childNodes", value);
            }
        }

        public void Init()
        {
            GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(BrowseoFXManager.Instance.MainWindowElement.Instance).Wrap(GeckoDOMEventTarget.Create);
            foreach (var evnt in TabEvents)
            {
                eventTarget.AddEventListener(evnt, this, true, true, 2);
            }
        }

        #region nsIDOMEventListener
        public void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create);
            //try
            //{
            //    var sessionStore = Xpcom.GetService<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1");
            //    var eventListenern = Xpcom.QueryInterface<Gecko.Interfaces.nsIDOMEventListener>(sessionStore);
            //    @event.SetOwner(Marshal.GetIUnknownForObject(BrowseoFXManager.Instance.GloableWebView.Widget));
            //    eventListenern.HandleEvent(@event);
            //}
            //catch (Exception e)
            //{ }
            switch (args.Type)
            {
                case "TabOpen":

                    break;

                case "TabClose":
                    break;

                case "TabSelect":
                    break;

                case "TabShow":
                    break;

                case "TabHide":
                    break;

                case "TabPinned":
                    break;

                case "TabUnpinned":
                    break;

                case "SSTabClosing":
                    break;

                case "SSTabRestoring":
                    break;

                case "SSTabRestored":
                    break;

                case "visibilitychange":
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Custom Functionality


        public async void FillForm(string profilename)
        {
            try
            {
                if (SelectedContentDocument == null || SelectedContentDocument.Uri == null) return;
                string curUrl = SelectedContentDocument.Uri;

                PersonData profile = await BrowseoFXManager.Instance.getSelectedProfile(false);
                if (profile == null) return;


                if (curUrl.ToLower().Contains("accounts.google.com/signup"))
                {
                //    foreach (var element in SelectedContentDocument.GetElementsByTagName("INPUT"))
                //    {
                //        var input = element as GeckoHTMLInputElement;
                //        if (input == null) continue;

                //        if (input.Id == "FirstName")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.FirstName);
                //        }
                //        else if (input.Id == "LastName")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.LastName);
                //        }
                //        else if (input.Id == "GmailAddress")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.Username);
                //        }
                //        else if (input.Id == "Passwd")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.Password);
                //        }
                //        else if (input.Id == "PasswdAgain")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.Password);
                //        }
                //        else if (input.Id == "BirthDay")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType((GloableProfData.PData.CmbSelectedIndexDay + 1).ToString());
                //        }
                //        else if (input.Id == "BirthYear")
                //        {
                //            await GetBoundingRectNClick(input);
                //            await LoopAndType(GloableProfData.PData.BirthdayYear.ToString());
                //        }
                //    }

                   return;
                }

                if (curUrl.ToLower().Contains("accounts.google.com/servicelogin"))
                {
                    var elm = SelectedContentDocument.GetElementById("Email");
                    if (elm != null) await setNodeValue((GeckoHTMLInputElement)elm, profile.Email);

                    var pass = SelectedContentDocument.GetElementById("Passwd");
                    if (pass != null) await setNodeValue((GeckoHTMLInputElement)pass, profile.Password);

                    return;
                }

                try
                {
                    if (SelectedContentDocument.ActiveElement != null)
                    {
                        (SelectedContentDocument.ActiveElement as GeckoHTMLInputElement).Blur();
                    }
                }
                catch { }
                foreach (var element in SelectedContentDocument.GetElementsByTagName("input"))
                {
                    var input = element as GeckoHTMLInputElement;
                    if (input == null) continue;

                    if (input.Type != null && input.Type.ToLower() == "radio")
                    {
                        if (curUrl.Contains("twitter.com")) continue;
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
                        if (curUrl == "https://twitter.com/signup")
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
                        else if (curUrl.Contains("https://twitter.com/login") || curUrl == "https://twitter.com/")
                        {
                            if (input.Name == "session[username_or_email]")
                            {
                                await setNodeValue(input, profile.Email);
                            }
                            else if (input.Name == "session[password]")
                            {
                                await setNodeValue(input, profile.Password);
                            }
                            continue;
                        }
                        else if (curUrl.Contains("https://login.yahoo.com/account/create"))
                        {
                            if (input.Id == "usernamereg-email")
                            {
                                await setNodeValue(input, profile.Email);
                            }
                            else if (input.Id == "usernamereg-password")
                            {
                                await setNodeValue(input, profile.Password);
                            }
                            else if (input.Id == "usernamereg-freeformGender")
                            {
                                await setNodeValue(input, profile.CmbSelectedIndexSex == 0 ? "Male" : "Female");
                            }
                            else if (input.Id == "usernamereg-freeformGender")
                            {
                                await setNodeValue(input, profile.CmbSelectedIndexSex == 0 ? "Male" : "Female");
                            }
                            continue;
                        }
                        else if (curUrl.Contains("https://www.linkedin.com/"))
                        {
                            if (input.Id == "login-email" || input.Id == "reg-email")
                            {
                                await setNodeValue(input, profile.Email);
                            }
                            else if (input.Id == "login-password" || input.Id == "reg-password")
                            {
                                await setNodeValue(input, profile.Password);
                            }
                            else if (input.Id == "reg-firstname")
                            {
                                await setNodeValue(input, profile.FirstName);
                            }
                            else if (input.Id == "reg-lastname")
                            {
                                await setNodeValue(input, profile.LastName);
                            }
                            continue;
                        }
                        else if (curUrl == ("https://www.reddit.com/"))
                        {
                            if (input.Id == "user_reg")
                            {
                                await setNodeValue(input, profile.Username);
                            }
                            else if (input.Id == "passwd_reg" || input.Id == "passwd2_reg")
                            {
                                await setNodeValue(input, profile.Password);
                            }
                            else if (input.Id == "email_reg")
                            {
                                await setNodeValue(input, profile.Email);
                            }
                            continue;
                        }
                        if (input.Type.ToLower() == "password" && isInputOf(input, "password") || isInputOf(input, "pass"))
                        {
                            await setNodeValue(input, profile.Password);
                        }
                        if (input.Type.ToLower() == "hidden" || input.Type.ToLower() == "password") continue;

                        if (!isInputOf(input, "lastname") && (isInputOf(input, "first") || isInputOf(input, "name_f")))
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
                        else if (input.Type.ToLower() == "email" || ((isInputOf(input, "mail") && !isInputOf(input, "name")) ||
                            isInputOf(input, "session_key")))
                        {
                            await setNodeValue(input, profile.Email);
                        }
                        else if (isInputOf(input, "username") || isInputOf(input, "login") || isInputOf(input, "user_login"))
                        {
                            await setNodeValue(input, profile.Username);
                        }
                        else if (isInputOf(input, "phone") || isInputOf(input, "number"))
                        {
                            if (curUrl == "https://www.diigo.com/sign-up") continue;
                            await setNodeValue(input, profile.PhoneNumber);
                        }

                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keydown", 0, 9, 0, false);
                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keypress", 0, 9, 0, false);
                        //WebBrowser.Browser.Window.WindowUtils.SendKeyEvent("keyup", 0, 9, 0, false);
                    }
                }

                foreach (var sel in SelectedContentDocument.GetElementsByTagName("select"))
                {
                    var element = sel as GeckoHTMLInputElement;
                    if (element == null) continue;

                    foreach (var attrib in element.Attributes)
                    {
                        if (attrib.NodeValue == null || curUrl == "https://www.diigo.com/sign-up") continue;

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

            var selectedProfile = BrowseoFXManager.Instance.Project.Profiles.FirstOrDefault(p => p.ProfOrProjName == profilename);
            if (selectedProfile == null) return;

            // BrowseoFXManager.Instance.Project.SelectedProfile = selectedProfile;
            //FormsManagerService.Instance.AddFormsEventListeners(SelectedContentDocument);

            //FormsManagerService.Instance.CheckFormsAndDisplayOptions(SelectedContentDocument);

            //FormsManagerService.Instance.SaveProjectsProfilesToFormHistory(SelectedContentDocument);
            //FormsManagerService.Instance.RequestFormFill(SelectedContentDocument);



            //  var entryAdded = formFillService.FormHistory.AddedEntry(focusedElement.Id, selectedProfile.Email);





            // var focusedElement = formFillService.FormFillController.Instance.GetFocusedInputAttribute().Wrap(Gecko.DOM.HTML.GeckoHTMLInputElement.Create);
            // formFillService.FormFillController.Instance.ShowPopup();



            //formFillService.FormFillController.Instance.ShowPopup();
            //var htmlElementsFound = SelectedContentDocument.GetElementsByTagName("input");
            //foreach (var element in htmlElementsFound)
            //{
            //    Gecko.DOM.HTML.GeckoHTMLInputElement input = element as Gecko.DOM.HTML.GeckoHTMLInputElement;
            //    if (input.Id.IsNullOrSpace()) continue;

            //    var inputId = input.Id.ToLower();
            //    var inputName = input.Id.IsNullOrSpace() ? "" : input.Name.ToLower();
            //    var inputType = input.Type.IsNullOrSpace() ? "" : input.Type.ToLower();

            //    if (input.Id.Contains("email"))
            //    {

            //    }
            //}
        }

        public void OpenWindow(string link)
        {
            SelectedContentDocument.DefaultView.Open(link, link, "resizable,scrollbars,status");
        }

        private async Task setNodeValue(GeckoHTMLInputElement input, string valu)
        {
            if (input.Value.ToLower().Trim() == valu.ToLower().Trim()) return;
            await Task.Delay(new Random().Next(100, 200));
            
            input.Focus();
           // input.Click();
            //input.select();
            //try
            //{
            //    input.OwnerDocument.DefaultView.WindowUtils.SendNativeKeyEvent(0, 0, 0, valu, valu, null);
            //}
            //catch
            //{
            //    WebBrowser.Browser.Window.WindowUtils.SendNativeKeyEvent(0, 0, 0, valu, valu);
            //}
            await Task.Delay(new Random().Next(300, 400));
            if (input.Value.ToLower().Trim() != valu.ToLower().Trim()) input.Value = valu;
            input.Blur();
        }

        private bool isInputOf(GeckoHTMLInputElement input, string idorname)
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
                recursiveFindId(input.Parent as GeckoHTMLInputElement, idorname, out isIt);
            }

            return isIt;

            //return input.Id.ToLower().Contains(idorname) || input.Name.ToLower().Contains(idorname);
        }
        private void recursiveFindId(GeckoHTMLInputElement input, string idorname, out bool isIt)
        {
            isIt = false;
            if (input == null || input.NodeName.ToUpper() == "FORM") return;

            isIt = input.Id.ToLower().Contains(idorname) || input.ClassName.ToLower().Contains(idorname);
            if (isIt) return;

            if (input.Parent != null && input.Parent.NodeName.ToUpper() != "FORM")
            {
                recursiveFindId(input.Parent as GeckoHTMLInputElement, idorname, out isIt);
            }
        }


        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };

        internal void DominateAll()
        {
            List<string> linksToReturn = new List<string>();
            string type = "", name = "", id = "";

            if (SelectedContentDocument.Location.Href == "https://www.facebook.com/bookmarks/groups")
            {
                type = "groups";

                var groupLists = SelectedContentDocument.GetElementsByTagName("li");
                foreach (var ellement in groupLists)
                {
                    var geckoElleInnerHtml = (ellement as GeckoHTMLElement).InnerHtml;
                    if (ellement.HasAttribute("id") && ellement.GetAttribute("id").Contains("navItem_")
                        && geckoElleInnerHtml.Contains("<span class=\"imgWrap\" data-testid=\"bookmark_icon_see_all\">"))
                    {
                        id = ellement.GetAttribute("id").Replace("navItem_", "");


                        var nameSubstring = "<div dir=\"ltr\" class=\"linkWrap hasCount\"><span>";
                        var innetHtml = geckoElleInnerHtml;
                        name = innetHtml.Substring(innetHtml.IndexOf(nameSubstring) + nameSubstring.Length);
                        name = name.Remove(name.IndexOf("<"));
                        name = name.Replace("&amp;", "&");

                        if (name.StartsWith("ditButton\">")) continue;

                        string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
                        link = link.Replace("?ref=br_rs", "");
                        linksToReturn.Add(link);
                    }

                }

                OnAddedToGoViral?.Invoke(null, "", linksToReturn);
                return;
            }

            if (SelectedContentDocument == null ||
                (!SelectedContentDocument.Location.Href.Contains("https://www.facebook.com/search/") && !SelectedContentDocument.Location.Href.Contains("https://www.facebook.com/groups/"))) return;
            try
            {
                type = "groups";
                if (SelectedContentDocument.Location.Href.Contains("https://www.facebook.com/groups/"))
                {
                    var groupLists = SelectedContentDocument.GetElementsByTagName("li");
                    foreach (var ellement in groupLists)
                    {
                        if (ellement.HasAttribute("id") && ellement.GetAttribute("id").Contains("GroupDiscoverCard_"))
                        {
                            id = ellement.GetAttribute("id").Replace("GroupDiscoverCard_", "");


                            var nameSubstring = "<div class=\"_266w\"><a href=\"/groups/";
                            var innetHtml = (ellement as GeckoHTMLElement).InnerHtml;
                            name = innetHtml.Substring(innetHtml.IndexOf(nameSubstring) + nameSubstring.Length);
                            name = name.Remove(name.IndexOf("/"));

                            if (name.StartsWith("\"><div class=\"_4-jy\">")) continue;

                            string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
                            link = link.Replace("?ref=br_rs", "");
                            linksToReturn.Add(link);
                        }
                    }
                }
                else if (SelectedContentDocument.Location.Href.Contains("keywords_places"))
                {
                    type = "places";
                    var groupLists = SelectedContentDocument.GetElementsByClassName("_1wu_");
                    foreach (var ellement in groupLists)
                    {
                        if (ellement.HasAttribute("data-et"))
                        {
                            id = ellement.GetAttribute("data-et");
                            id = id.Replace("{\"click_point\":\"name\",\"data\":\"{\\\"entity_id\\\":\\\"", "");
                            id = id.Replace("\\\"}\"}", "");

                            var nameSubstring = ">";
                            var innetHtml = (ellement as GeckoHTMLElement).InnerHtml;
                            name = innetHtml.Substring(innetHtml.IndexOf(nameSubstring) + nameSubstring.Length);
                            name = name.Remove(name.IndexOf("<"));

                            if (name.StartsWith("\"><div class=\"_4-jy\">")) continue;

                            string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
                            link = link.Replace("?ref=br_rs", "");
                            linksToReturn.Add(link);
                        }

                    }

                }
                else if (SelectedContentDocument.Location.Href.Contains("https://www.facebook.com/search/events/"))
                {
                    type = "events";
                    var groupLists = SelectedContentDocument.GetElementsByClassName("_3u1 _gli _uvb");
                    foreach (var ellement in groupLists)
                    {
                        if (ellement.HasAttribute("data-bt"))
                        {
                            id = ellement.GetAttribute("data-bt");
                            id = id.Remove(id.IndexOf(","));
                            id = id.Replace("{\"id\":", "");

                            var nameSubstring = "<a href=\"/events/" +id;
                            var innetHtml = (ellement as GeckoHTMLElement).InnerHtml;
                            name = innetHtml.Substring(innetHtml.IndexOf(nameSubstring) + nameSubstring.Length);
                            name = name.Remove(name.IndexOf("<"));
                            name = name.Substring(name.IndexOf(">") + 1);

                            string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
                            link = link.Replace("?ref=br_rs", "");
                            linksToReturn.Add(link);
                        }

                    }

                }
                else
                {
                    var resultsClassname = "_3u1 _gli _uvb";
                    var ogDataAttribute = "data-bt";
                    var ogphotovideoDataAttribute = "data-gt";
                    var urlsClassName = "_32mo";

                    type = SelectedContentDocument.Location.Href.Replace("https://www.facebook.com/search/", "");
                    type = type.Remove(type.IndexOf("/"));

                    if (type == "photos" || type == "videos")
                    {
                        resultsClassname = "_401d";
                    }

                    var results = SelectedContentDocument.GetElementsByClassName(resultsClassname);
                    foreach (var result in results)
                    {
                        foreach (var attribute in result.Attributes)
                        {
                            if (attribute.LocalName == ogDataAttribute)
                            {
                                //{"id":352751268256569,"rank":1,"abtest_version":null,"abtest_params":{"abtest_version":null,"origin":"A","ranker":null},"section":"main_column","owner_id":null,"sub_id":null,"browse_location":null,"query_data":{"q":"tech"},"is_headline":false}

                                id = attribute.NodeValue;
                                id = id.Remove(id.IndexOf(","));
                                id = id.Replace("{\"id\":", "");

                            }
                            else if (attribute.LocalName == ogphotovideoDataAttribute && (type == "photos" || type == "videos"))
                            {
                                var dataSubstring = "<div data-bt=\"";
                                var innetHtml = (result as GeckoHTMLElement).InnerHtml;
                                id = innetHtml.Substring(innetHtml.IndexOf(dataSubstring) + dataSubstring.Length);
                                id = id.Remove(id.IndexOf(","));
                                id = id.Replace("{&quot;id&quot;:", "");
                                id = id.Replace("4ou6\" data-bt=\"", "");
                            }
                        }


                        switch (type)
                        {
                            case "groups":
                                var groupsSubstring = "<div class=\"_52eh _ajx\"><a href=\"/groups/";
                                name = (result as GeckoHTMLElement).InnerHtml.Substring((result as GeckoHTMLElement).InnerHtml.IndexOf(groupsSubstring) + groupsSubstring.Length);
                                name = name.Remove(name.IndexOf("/"));
                                break;

                            case "videos":
                            case "photos":
                                name = id;
                                break;

                            default:
                                //https://www.facebook.com/TechRadar/?ref=br_rs
                                var urls = result.GetElementsByClassName(urlsClassName);
                                if (urls.Count > 0)
                                {
                                    var urlvalue = urls.ElementAt(0).GetAttribute("href");

                                    name = urlvalue.Replace("/?ref=br_rs", "");
                                    name = name.Replace("https://www.facebook.com/", "");
                                }
                                break;
                        }


                        if (name.IsNullOrEmpty() || id.IsNullOrEmpty()
                            || name.StartsWith("ss=\"_4-u2 _1c4m _14ay _4-u8\">")
                            || name.StartsWith("=\"_3n00\" data-bt=")) continue;

                        string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;
                        link = link.Replace("?ref=br_rs", "");
                        linksToReturn.Add(link);
                    }
                }
            }
            catch { }

            OnAddedToGoViral?.Invoke(null, "", linksToReturn);
        }

        public void NewTab(string url)
        {
            //openLinkIn(\"chrome://browser/content/places/places.xul\", \"tab\", { charset: undefined, referrerURI: undefined, inBackground: false });
            //GloableWebViewWindow.OpenDialog(url, "Library", "chrome,toolbar=yes,dialog=no,resizable", null);
            //GeckoJavascriptBridge.GetService().EvaluateInWindow(GloableWebViewWindow, "gbrowser.openLinkIn(\"" +url+"\", \"tab\", { charset: undefined, referrerURI: undefined, inBackground: false });");
        }
        #endregion

        public void SelectedTabNavigate(string query)
        {
            SelectedContentDocument.DefaultView.Content.Location.Replace(query);
        }

        public void Dispose()
        {
        }
    }
}
