using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WpfCefBrowser.Mvvm;
using WpfCefBrowser.Views;

namespace WpfCefBrowser.ViewModels
{
    public class BrowserTabViewModel : INotifyPropertyChanged
    {
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
               if(WebBrowser != null) WebBrowser.NavigateTo(address);
               // PropertyChanged.ChangeAndNotify(ref address, value, () => Address);
            }
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
            set { PropertyChanged.ChangeAndNotify(ref title, value, () => Title); }
        }

        private Xilium.CefGlue.WPF.WpfCefBrowser webBrowser;
        public Xilium.CefGlue.WPF.WpfCefBrowser WebBrowser
        {
            get { return webBrowser; }
            set { PropertyChanged.ChangeAndNotify(ref webBrowser, value, () => WebBrowser); }
        }

        private object evaluateJavaScriptResult;

        public object EvaluateJavaScriptResult
        {
            get { return evaluateJavaScriptResult; }
            set { PropertyChanged.ChangeAndNotify(ref evaluateJavaScriptResult, value, () => EvaluateJavaScriptResult); }
        }

        private bool showSidebar;
        public bool ShowSidebar
        {
            get { return showSidebar; }
            set { PropertyChanged.ChangeAndNotify(ref showSidebar, value, () => ShowSidebar); }
        }

        public ICommand GoCommand { get; set; }
        public ICommand HomeCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BrowserTabViewModel(string address)
        {
            BrowserTabView.OnSetBrowser += BrowserTabView_OnSetBrowser;
            Address = address;
            AddressEditable = Address;

            GoCommand = new DelegateCommand(Go, () => !String.IsNullOrWhiteSpace(Address));
            HomeCommand = new DelegateCommand(() => AddressEditable = Address = "https://google.com");

            PropertyChanged += OnPropertyChanged;

            var version = "browe·seo Beta";
            OutputMessage = version;
        }

        void BrowserTabView_OnSetBrowser(Xilium.CefGlue.WPF.WpfCefBrowser brwsr)
        {
            WebBrowser = brwsr;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    AddressEditable = Address;
                    break;

                case "WebBrowser":
                    if (WebBrowser != null)
                    {
                        // TODO: This is a bit of a hack. It would be nicer/cleaner to give the webBrowser focus in the Go()
                        // TODO: method, but it seems like "something" gets messed up (= doesn't work correctly) if we give it
                        // TODO: focus "too early" in the loading process...
                        WebBrowser.Loaded += delegate { Application.Current.Dispatcher.BeginInvoke((Action)(() => webBrowser.Focus())); };
                    }

                    break;
            }
        }

        private void Go()
        {
            Address = AddressEditable;

            // Part of the Focus hack further described in the OnPropertyChanged() method...
            Keyboard.ClearFocus();
        }
    }
}
