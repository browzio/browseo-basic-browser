using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using zFirefoxXulBrowser.Controls;

namespace zFirefoxXulBrowser.ViewModels
{
    public class FoxXulViewModel : ViewModelBase
    {
        private FFXulBrowserControl webBrowser;
        public FFXulBrowserControl WebBrowser
        {
            get
            {
                return webBrowser;
            }
            set { webBrowser = value; RaisePropertyChanged("WebBrowser"); }
        }

        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (WebBrowser == null) WebBrowser = new FFXulBrowserControl();

                if (wfh == null)
                    wfh = new WindowsFormsHost() { Child = WebBrowser };

                return wfh;
            }
            set { wfh = value; RaisePropertyChanged("WebBrowserHost"); }
        }

        public FoxXulViewModel()
        {
        }

        private void InitBrowser(string address)
        {
            WebBrowser.initBrowser(address);
        }
    }
}
