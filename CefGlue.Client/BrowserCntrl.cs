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

        public CefWebBrowser CBrowser { get; set; }

        public string CurrAddress { get; set; }

        private bool isWindowPopUp;

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

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        //Declare the hook handle as an int.
        static int hHookMouse = 0;
        static int hHookKeyboard = 0;

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        private const int WH_KEYBOARD = 13;
        private const int WH_MOUSE = 7;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;

        //Declare the wrapper managed POINT class.
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        //Declare the wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }
        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the hook information to the next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        public HookProc KeyboardHookProcedure { get; set; }

        public HookProc MouseHookProcedure { get; set; }

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
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr hhook = IntPtr.Zero;
        private keyboardHookProc hookProcDelegate;

        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
      {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                //if (HookedKeys.Contains(key))
                //{
                    KeyEventArgs kea = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (key == Keys.T))
                    {
                        if (!CefRenderProcessHandler.NoBeforeNavigation) CefRenderProcessHandler.NoBeforeNavigation = true;
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                    {
                        if (CefRenderProcessHandler.NoBeforeNavigation) CefRenderProcessHandler.NoBeforeNavigation = false;
                    }
                    if (kea.Handled)
                        return 1;
            }
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        object mouseLock = new object();
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Marshall the data from the callback.
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));

            if (nCode < 0)
            {
                return CallNextHookEx(hHookMouse, nCode, wParam, lParam);
            }
            else
            {
                lock (mouseLock) 
                {
                    if (wParam.ToInt32() == WM_LBUTTONDOWN)
                    {
                        if (!CefRenderProcessHandler.NoBeforeNavigation)
                        {
                            //if (NichResearch.Windows.CopyPasteWindow.HasToPaste)
                           // {
                              //  InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                              //  NichResearch.Windows.CopyPasteWindow.HasToPaste = false;
                            //}
                        }
                        else
                        {
                            if (huverLunk != null && huverLunk != "")
                            {
                                setBrowserEnabled(false);
                                CefRenderProcessHandler.NoBeforeNavigation = false;
                                OnCreateNewTab(huverLunk,false);
                                System.Threading.Thread.Sleep(500);
                                setBrowserEnabled(true);
                            }
                        }
                    }
                    
                }
                return CallNextHookEx(hHookMouse, nCode, wParam, lParam);
            }
        }

        private void setBrowserEnabled(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(setBrowserEnabled), enabled);
                return;
            }
            CBrowser.Enabled = enabled;
        }
        
        private int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            CefRenderProcessHandler.NoBeforeNavigation = false;
            //Marshall the data from the callback.

           // KeyboardHookStruct keyInfo = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            int VK_CNTRL = 17;

            if (nCode < 0)
            {
                return CallNextHookEx(hHookKeyboard, nCode, wParam, lParam);
            }
            else
            {
                int keyCode = wParam.ToInt32();
                if (keyCode == VK_CNTRL)
                {
                    CefRenderProcessHandler.NoBeforeNavigation = true;
                }

                return CallNextHookEx(hHookKeyboard, nCode, wParam, lParam);
            }

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

            if (huverLunk == null || huverLunk == "") return;

            if (contextMenueItemID == 999)
            {
                OnCreateNewTab(huverLunk,false);
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

        public void InjectData()
        {

            if (CBrowser.Browser.GetMainFrame().Url == null) return;

            //bool isFromMulti = false;
            //string selectedPath = "";
            
            PersonData profile = BrowserInit.pData;
            if (pdataForImgur == null)
            {
                if (MyFilesDatabase.HasMultipleProfiles(BrowserInit.pData.ProjectName))
                {
                    SelectProfileWindow selectProfile = new SelectProfileWindow(BrowserInit.pData.ProjectName);
                    selectProfile.ShowDialog();
                    if (!selectProfile.OkClicked) return;
                    profile = MyFilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
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

            if (disposing)
            {
                if (hHookKeyboard != 0)
                {
                    UnhookWindowsHookEx(hHookKeyboard);
                }
                if (hHookMouse != 0)
                {
                    UnhookWindowsHookEx(hHookMouse);
                }
            }

            base.Dispose(disposing);
        }
    }
}
