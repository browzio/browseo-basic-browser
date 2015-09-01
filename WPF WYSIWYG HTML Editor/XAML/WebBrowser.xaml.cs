using mshtml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace WPF_WYSIWYG_HTML_Editor
{
    /// <summary>
    /// Interaction logic for WebBrowser.xaml
    /// </summary>
    public partial class WPFWebBrowser : UserControl
    {
        public HTMLDocument doc;
        public WebBrowser webBrowser;

        public WPFWebBrowser()
        {
            InitializeComponent();
        }


        void WPFWebBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.ContextIdle,
                new Action(delegate()
                {
                    webBrowser.Focus();
                }));
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
          //  MessageBox.Show("DVsdvsddsgvzxv");
            base.OnPreviewKeyDown(e);
        }

        public void newWb(string url)
        {
            if (webBrowser != null)
            {
                return;
                //webBrowser.LoadCompleted -= completed;
                //webBrowser.Dispose();
                //gridwebBrowser.Children.Remove(webBrowser);
            }

            if (doc != null)
            {
                return;
                // doc.clear();
            }

            webBrowser = new WebBrowser();
            webBrowser.PreviewKeyDown += webBrowser_PreviewKeyDown;
            webBrowser.LoadCompleted += completed;
            gridwebBrowser.Children.Add(webBrowser);
            Script.HideScriptErrors(webBrowser, true);
            if (url == "")
            {
                webBrowser.NavigateToString(Properties.Resources.New);
                doc = webBrowser.Document as HTMLDocument;
                doc.designMode = "On";
                Format.doc = doc;
                doc.focus();
                return;
            }
            else
            {
                webBrowser.Navigate(url);
            }


            doc = webBrowser.Document as HTMLDocument;
            Format.doc = doc;
        }




        void webBrowser_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            //MessageBox.Show("dsjlksdlksa");
        }

        private void completed(object sender, NavigationEventArgs e)
        {
            doc = webBrowser.Document as HTMLDocument;
            doc.designMode = "On";
            webBrowser.Focusable = true;
            webBrowser.Focus();
            doc.focus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Focusable = true;
            //Keyboard.Focus(this);
            //webBrowser.Focusable = true;
            //webBrowser.Focus();
            //Keyboard.Focus(webBrowser);
        }

        private void gridwebBrowser_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        internal void unhook()
        {
        }
    }

    public class LowLevelKeyboardListener
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
 
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
 
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
 
        public event EventHandler<KeyPressedArgs> OnKeyPressed;
 
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
 
        public LowLevelKeyboardListener()
        {
            _proc = HookCallback;
        }
 
        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
        }
 
        public void UnHookKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }
 
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
 
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
 
                if (OnKeyPressed != null) { OnKeyPressed(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
            }
 
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
 
    public class KeyPressedArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }
 
        public KeyPressedArgs(Key key)
        {
            KeyPressed = key;
        }
    }
}

