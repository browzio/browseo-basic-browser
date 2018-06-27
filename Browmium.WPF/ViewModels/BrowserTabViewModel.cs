using Browmium.WPF.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace Browmium.WPF.ViewModels
{
    public class BrowserTabViewModel : INotifyPropertyChanged
    {
        //BrowserTabUserControl
        private BrowserTabUserControl browserTabUserControl;
        public BrowserTabUserControl BrowserTabUserControl
        {
            get
            {
                return browserTabUserControl;
            }
            set { browserTabUserControl = value; RaisePropertyChanged("BrowserTabUserControl"); }
        }

        //WebBrowserHost
        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                    wfh = new WindowsFormsHost() { Child = BrowserTabUserControl };
                return wfh;
            }
            set { wfh = value; RaisePropertyChanged("WebBrowserHost"); }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public BrowserTabViewModel()
        {
            BrowserTabUserControl = new BrowserTabUserControl();
            BrowserTabUserControl.Init("https://www.google.com/");
        }

        #region propertyChanged
        public virtual void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
