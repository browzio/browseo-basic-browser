using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Organiser.Common.Classes
{
    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public WaitCursor()
        {
            setCursor();
        }

        private void setCursor()
        {
           if(Application.Current.Dispatcher.Thread != Thread.CurrentThread)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(setCursor));
                return;
            }

            _previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            disposeCusrsor();
        }

        private void disposeCusrsor()
        {
            if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(disposeCusrsor));
                return;
            }

            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
