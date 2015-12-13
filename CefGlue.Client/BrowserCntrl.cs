using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xilium.CefGlue.WindowsForms;
using SocialOrganizer.Models;
using Organiser.Common.Classes;
using Organiser.Common;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DragDropListview;
using System.Net;
using System.IO;

namespace Xilium.CefGlue.Client
{
    public partial class BrowserCntrl : UserControl
    {

        public event Action<string> OnBrowserTitleChanged = delegate { };
        public event Action<string> OnBrowserAddressChanged = delegate { };
        public event Action<string> OnBrowserStatusChanged = delegate { };
        public event Action<string> OnBrowserMessageChanged = delegate { };
        public event Action<string,bool> OnCreateNewTab = delegate { };
        public event Action<bool> OnBrowserLoadingChanged = delegate { };
        public event Action<string> OnPinIt = delegate { };
        public event Action<string> OnCurateToPBN = delegate { };

        public CefWebBrowser CBrowser { get; set; }

        public string CurrAddress { get; set; }

        private bool isWindowPopUp;

        private static int lastProfileIndex = 0;


        private string startUrl = null, huverLunk = "";

        public BrowserCntrl()
        {
            InitializeComponent();
        }

        public void init(string startUrl)
        {
            CBrowser = new CefWebBrowser();
            CBrowser.HandleWasCreated += browser_OnHandleCreated;
            CBrowser.StartUrl = startUrl;
            CBrowser.Width = this.Width;
            CBrowser.Height = this.Height;
            CBrowser.Dock = DockStyle.Fill;
            CBrowser.BringToFront();

            
            CBrowser.TitleChanged += CBrowser_TitleChanged;
            CBrowser.AddressChanged += CBrowser_AddressChanged;
            CBrowser.StatusMessage += CBrowser_StatusMessage;
            CBrowser.BeforePopup += CBrowser_BeforePopup;
            CBrowser.LoadingStateChange += CBrowser_LoadingStateChange;
            CBrowser.ConsoleMessage += CBrowser_ConsoleMessage;
            CBrowser.OnContextMenuItemClicked += CBrowser_OnContextMenuItemClicked;

            this.SuspendLayout();
            this.Controls.Add(CBrowser);
            this.ResumeLayout(false);
            this.PerformLayout();

            //CBrowser.BrowserCreated += CBrowser_BrowserCreated;
        }

        #region mouse hooks

        //public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        ////Declare the hook handle as an int.
        //static int hHookMouse = 0;
        //static int hHookKeyboard = 0;

        ////Declare the mouse hook constant.
        ////For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        //private const int WH_KEYBOARD = 13;
        //private const int WH_MOUSE = 7;
        //private const int WM_MOUSEMOVE = 0x0200;
        //private const int WM_LBUTTONDOWN = 0x0201;

        ////Declare the wrapper managed POINT class.
        //[StructLayout(LayoutKind.Sequential)]
        //public class POINT
        //{
        //    public int x;
        //    public int y;
        //}

        ////Declare the wrapper managed MouseHookStruct class.
        //[StructLayout(LayoutKind.Sequential)]
        //public class MouseHookStruct
        //{
        //    public POINT pt;
        //    public int hwnd;
        //    public int wHitTestCode;
        //    public int dwExtraInfo;
        //}
        //public struct KeyboardHookStruct
        //{
        //    public int vkCode;
        //    public int scanCode;
        //    public int flags;
        //    public int time;
        //    public int dwExtraInfo;
        //}

        ////This is the Import for the SetWindowsHookEx function.
        ////Use this function to install a thread-specific hook.
        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint threadId);

        ////This is the Import for the UnhookWindowsHookEx function.
        ////Call this function to uninstall the hook.
        //[DllImport("user32.dll", CharSet = CharSet.Auto,
        // CallingConvention = CallingConvention.StdCall)]
        //public static extern bool UnhookWindowsHookEx(int idHook);

        ////This is the Import for the CallNextHookEx function.
        ////Use this function to pass the hook information to the next hook procedure in chain.
        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        //[DllImport("user32.dll")]
        //static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        //public HookProc KeyboardHookProcedure { get; set; }

        //public HookProc MouseHookProcedure { get; set; }

        void CBrowser_BrowserCreated(object sender, EventArgs e)
        {
            //uint BrowserThreadId = GetWindowThreadProcessId(CBrowser.Browser.GetHost().GetWindowHandle(), IntPtr.Zero);

            //MouseHookProcedure = new HookProc(this.MouseHookProc);

            //hHookMouse = SetWindowsHookEx(WH_MOUSE,
            //            MouseHookProcedure,
            //            (IntPtr)0,
            //            BrowserThreadId);

            //if (hHookMouse == 0)
            //{
            //    Cursor.Show();
            //}

            //KeyboardHookProcedure = new HookProc(this.KeyboardHookProc);
            //hHookKeyboard = SetWindowsHookEx(WH_KEYBOARD,
            //            KeyboardHookProcedure,
            //            (IntPtr)0,
            //            BrowserThreadId);

            //IntPtr hInstance = (IntPtr)0;
            //hookProcDelegate = hookProc;
            //hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProcDelegate, hInstance, 0);
        }

        #endregion

        #region events

        void CBrowser_OnContextMenuItemClicked(int contextMenueItemID)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(CBrowser_OnContextMenuItemClicked), contextMenueItemID);
                return;
            }

            if (contextMenueItemID == 666)
            {
                try
                {
                    if (CBrowser.Browser.GetMainFrame() == null || CBrowser.Browser.GetMainFrame().Url == null) return;

                    //the javascript
                    string jsForExecution = "var range = window.getSelection().getRangeAt(0),"+
                                            "content = range.extractContents(),"+
                                            "span = document.createElement('SPAN');"+
                                            "span.appendChild(content);"+
                                            "var htmltext = span.innerHTML.toString();" +
                                            "range.insertNode(span);"+
                                            "nativeImplementation(htmltext);";
                    CBrowser.Browser.GetMainFrame().ExecuteJavaScript(jsForExecution, CBrowser.Browser.GetMainFrame().Url, 0);

                    string dir = Path.Combine(MyFilesDatabase.GetBaseDir(), "TempHTML");
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    string file = Path.Combine(dir, "html.txt");

                    System.Threading.Tasks.Task.Factory.StartNew(()=>
                    {
                        while (!File.Exists(file))
                        {
                            System.Threading.Thread.Sleep(150);
                        }

                        OnCurateToPBN(File.ReadAllText(file));
                        File.Delete(file);
                    });

                }
                catch (Exception ex)
                {

                }
                return;
            }

            if (huverLunk == null || huverLunk == "") return;

            if (contextMenueItemID == 999)
            {
                OnCreateNewTab(huverLunk,false);
            }

            if (contextMenueItemID == 888)
            {
                MyFilesDatabase.SetClipboardText(huverLunk);
            }

 

            if (contextMenueItemID == 777)
            {
                bool errored = false;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Png files (*.png)|*.png|JPeg files (*.jpeg)|*.jpeg|All files (*.*)|*.*";
                sfd.FilterIndex = 3;
                sfd.RestoreDirectory = true;
                sfd.ShowDialog();
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] data = webClient.DownloadData(huverLunk);

                            using (MemoryStream mem = new MemoryStream(data))
                            {
                                using (var yourImage = Image.FromStream(mem))
                                {
                                    // If you want it as Png
                                    yourImage.Save(sfd.FileName);

                                    // If you want it as Jpeg
                                    //yourImage.Save("path_of_your_file.jpg", ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            if (CBrowser.Browser.GetMainFrame().Url == null)
                            {
                                MessageBox.Show("Failed to save image.");
                                return;
                            }

                            using (WebClient webClient = new WebClient())
                            {
                                byte[] data = webClient.DownloadData(CBrowser.Browser.GetMainFrame().Url);

                                using (MemoryStream mem = new MemoryStream(data))
                                {
                                    using (var yourImage = Image.FromStream(mem))
                                    {
                                        // If you want it as Png
                                        yourImage.Save(sfd.FileName);

                                        // If you want it as Jpeg
                                        //yourImage.Save("path_of_your_file.jpg", ImageFormat.Jpeg);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Failed to save image.");
                            errored = true;
                        }
                    }

                    if (!errored)
                    {
                        try
                        {
                            //MessageBox.Show("Image downloaded to " + sfd.FileName);
                            FileInfo fileInfo = new FileInfo(sfd.FileName);
                            Process.Start(fileInfo.DirectoryName);
                        }
                        catch { }
                    }
                });
            }
        }

        void CBrowser_BeforePopup(object sender, BeforePopupEventArgs e)
        {
            e.Handled = true;
            //if (e.TargetUrl.Contains("popup") || e.TargetUrl.Contains("plus.login"))
            //{
            //    BrowserPopUpWindow bpw = new BrowserPopUpWindow();
            //    bpw.Text = e.TargetUrl;
            //    bpw.FormClosed += bpw_FormClosed;
            //    bpw.OnBeforClosed += bpw_OnBeforClosed;
            //    bpw.Show();
            //    bpw.init(e);
            //    return;
            //};
            isWindowPopUp = true;
            if (e.TargetUrl != null)
                OnCreateNewTab(e.TargetUrl,true);
            isWindowPopUp = false;
        }

        void bpw_OnBeforClosed(string url)
        {
            //CBrowser.Focus();
           // Reload();
          //  CBrowser.Refresh();
            Navigate(url);
        }

        void bpw_FormClosed(object sender, FormClosedEventArgs e)
        {
            CBrowser.Focus();
            Reload();
            CBrowser.Refresh();
        }

        void CBrowser_StatusMessage(object sender, StatusMessageEventArgs e)
        {
            if (isWindowPopUp)
                return;
            huverLunk = e.Value;
            OnBrowserStatusChanged(huverLunk);
        }

        void CBrowser_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            if (isWindowPopUp)
                return;

            OnBrowserAddressChanged(CBrowser.Address);
            CurrAddress = CBrowser.Address;

            if (pdataForImgur != null)
            {
                InjectData();
            }
        }

        void CBrowser_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            if (isWindowPopUp)
                return;
            var title = CBrowser.Title;
            if (title != null)
            {
                if (title.Length > 18)
                {
                    title = title.Substring(0, 18) + "...";
                }
                OnBrowserTitleChanged(title);
            }
        }

        void CBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (isWindowPopUp)
                return;

            OnBrowserMessageChanged(e.Message);
        }

        void CBrowser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
        {
            if (isWindowPopUp)
                return;

            OnBrowserLoadingChanged(e.IsLoading);
        }

        void browser_OnHandleCreated()
        {
            CBrowser.CreateWebClient(new RequestHandleing(CBrowser), null);
        }

        #endregion

        #region navigation

        public void Navigate(string url)
        {
            try
            {
                if (!url.Contains(".") && url.Length > 1)
                {
                    string linkb = url;
                    linkb = url.Replace(' ', '+');
                    linkb = String.Format(@"http://google.com/search?v=1.0&q={0}", linkb);
                    CBrowser.Browser.GetMainFrame().LoadUrl(linkb);
                }
                else
                {
                    CBrowser.Browser.GetMainFrame().LoadUrl(url);
                    MyFilesDatabase.AppendToSavedSites(url);
                }
            }catch { }
        }

        public void Reload()
        {
            try
            {
                CBrowser.Browser.Reload();
            }
            catch { }
        }

        public void Forward()
        {
            try
            {
                if (CBrowser.Browser.CanGoForward)
                    CBrowser.Browser.GoForward();
            }
            catch { }
        }

        public void Back()
        {
            try
            {
                if (CBrowser.Browser.CanGoBack)
                    CBrowser.Browser.GoBack();
            }
            catch { }
        }

        #endregion

        #region injection of form data

        #region unused
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        bool hasToInject;

        PersonData pdataForImgur = null;
        #endregion

        SelectProfileWindow selectProfile;
        
        public void InjectData()
        {

            if (CBrowser.Browser.GetMainFrame() == null || CBrowser.Browser.GetMainFrame().Url == null) return;
            string curUrl = CBrowser.Browser.GetMainFrame().Url;
            //bool isFromMulti = false;
            //string selectedPath = "";
            
            PersonData profile = BrowserInit.pData.Clone() as PersonData;
            if (pdataForImgur == null)
            {
                bool found = false;
                bool isImportedList = false;
                try
                {
                    if (DragDropMainViewModel.Instance.FoldersAndSitesList != null && DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].TypeOfFolder == FolderTypes.Import)
                    {
                        string urltoCheck = curUrl.Substring(curUrl.IndexOf('.') + 1);
                        if (urltoCheck.Contains('.'))
                            urltoCheck = urltoCheck.Split('.')[0];
                        isImportedList = true;
                        foreach (Bookmark b in DragDropMainViewModel.Instance.FoldersAndSitesList[DragDropMainViewModel.Instance.SIFoldersSide].Sites)
                        {
                            if (!b.IsImported) continue;
                            string blinkToChek = b.Link.Substring(b.Link.IndexOf('.') + 1);
                            if (blinkToChek.Contains('.'))
                                blinkToChek = blinkToChek.Split('.')[0];
                            if (urltoCheck.Contains(blinkToChek))
                            {
                                profile.Username = b.Username;
                                profile.Email = b.Email;
                                profile.Password = b.Password;
                                found = true;
                                break;
                            }
                        }
                    }
                }
                catch {}
                if (!found && !isImportedList)
                {
                    if (MyFilesDatabase.HasMultipleProfiles(BrowserInit.pData.ProjectDIr))
                    {
                        if (selectProfile == null)
                        {
                            selectProfile = new SelectProfileWindow(BrowserInit.pData.ProjectName, BrowserInit.pData.ProjectDIr, lastProfileIndex, CBrowser.Browser.GetMainFrame().Url.ToLower());
                            //selectProfile.Closed += selectProfile_Closed;
                            selectProfile.ShowDialog();
                            if (!selectProfile.OkClicked)
                            {
                                selectProfile = null;
                                return;
                            }
                            lastProfileIndex = selectProfile.cmProfiles.SelectedIndex;
                            profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
                        }
                        else
                        {
                            selectProfile.Focus();
                        }
                        selectProfile = null;
                    }
                }
            }
            else
            {
                profile = pdataForImgur;
            }

            string linkToExecute = CBrowser.Browser.GetMainFrame().Url;
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
                 "alert('ip');"+
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
                "}"+
            "}"+
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
                         "all[i].value=" + "'" +profile.FirstName +" ' + ' "+ profile.LastName + "'" + "; break;" +
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
                         "all[i].value=" + "'"+profile.Username +"'" + "; break;" +
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
                                "all[i].setAttribute('class', 'uiStickyPlaceholderInput');" +"break;" +
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
                     "}"+

                      "if(all[i].tagName.indexOf('BUTTON') > -1){" +
                         "for (var j = 0; j < all[i].attributes.length; j++) {" +
                            "var attrib = all[i].attributes[j]; " +
                            "if(attrib.value.indexOf('disabled') > -1){" +
                                "document.getElementsByTagName('*')[i].removeAttribute('disabled'); break;"+
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
            //if (linkToExecute.ToLower().Contains("yahoo.com"))
            //{
            //    jsForExecution = yahooJsToExecute;
            //}
            //else if (linkToExecute.ToLower().Contains("imgur.com") && pdataForImgur == null)
            //{
              //  Navigate("imgur.com/register");
               // pdataForImgur = profile;
                //return;
            //}

            CBrowser.Browser.GetMainFrame().ExecuteJavaScript(jsForExecution, linkToExecute, 0);
            pdataForImgur = null;

             
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

        #endregion



        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            //if (disposing)
            //{
            //    if (hHookKeyboard != 0)
            //    {
            //        UnhookWindowsHookEx(hHookKeyboard);
            //    }
            //    if (hHookMouse != 0)
            //    {
            //        UnhookWindowsHookEx(hHookMouse);
            //    }
            //}

            base.Dispose(disposing);
        }
    }
}
