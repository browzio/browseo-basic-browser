using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko;
using Gecko.CustomMarshalers;
using System.Runtime.InteropServices;

namespace BrowseoFX_WPF.Core.Services.Browser
{
    public class nsWinTaskbar : nsIWinTaskbar
    {
        private static nsWinTaskbar _instancs;
        public static nsWinTaskbar Instance
        {
            get
            {
                if (_instancs == null) _instancs = new nsWinTaskbar();
                return _instancs;
            }
        }
        private nsWinTaskbar()
        {
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIJumpListBuilder CreateJumpListBuilder()
        {
            var builder = Xpcom.CreateInstance<nsIJumpListBuilder>("@mozilla.org/windows-jumplistbuilder;1");
            return builder;
            //throw new NotImplementedException();
            //using(var builder = Xpcom.CreateInstance2<nsIJumpListBuilder>("@mozilla.org/windows-jumplistbuilder;1"))
            //{
            //    //builder.Instance.InitListBuild(null);
            //    return builder.Instance;
            //}
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsITaskbarTabPreview CreateTaskbarTabPreview([MarshalAs(UnmanagedType.Interface)] nsIDocShell shell, [MarshalAs(UnmanagedType.Interface)] nsITaskbarPreviewController controller)
        {
            throw new NotImplementedException();
        }

        /// <summary>
		/// Returns true if the operating system supports Win7+ taskbar features.
		/// This property acts as a replacement for in-place os version checking.
		/// </summary>
        [return: MarshalAs(UnmanagedType.U1)]
        public bool GetAvailableAttribute()
        {
            return false;
        }

        public void GetDefaultGroupIdAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            string filename = new Guid().ToString();
            int hash = filename.GetHashCode() % 10000;
            result.SetData(hash.ToString("0000"));
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsITaskbarOverlayIconController GetOverlayIconController([MarshalAs(UnmanagedType.Interface)] nsIDocShell shell)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsITaskbarProgress GetTaskbarProgress([MarshalAs(UnmanagedType.Interface)] nsIDocShell shell)
        {
            var taskbarProgress = Xpcom.CreateInstance<nsITaskbarProgress>("@mozilla.org/windows-taskbar;1");
            //taskbarProgress.SetProgressState(nsITaskbarProgressConsts.STATE_NO_PROGRESS, 0, 0);
            return taskbarProgress;
            //todo nsITaskbarWindowPreview preview = GetTaskbarWindowPreview(shell);

            //using (var progressListener = Xpcom.QueryInterface2<nsIWebProgressListener>(shell))
            //{

            //    //return null;
            //    using (var taskbarProgress = Xpcom.CreateInstance2<nsITaskbarProgress>("@mozilla.org/windows-taskbar;1"))
            //    {
            //        taskbarProgress.Instance.SetProgressState(nsITaskbarProgressConsts.STATE_INDETERMINATE,0, 100);
            //        return taskbarProgress.Instance;
            //    }
            //}
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsITaskbarWindowPreview GetTaskbarWindowPreview([MarshalAs(UnmanagedType.Interface)] nsIDocShell shell)
        {
            throw new NotImplementedException();
        }

        public void PrepareFullScreen([MarshalAs(UnmanagedType.Interface)] mozIDOMWindow aWindow, [MarshalAs(UnmanagedType.U1)] bool aFullScreen)
        {
            throw new NotImplementedException();
        }

        public void PrepareFullScreenHWND(IntPtr aWindow, [MarshalAs(UnmanagedType.U1)] bool aFullScreen)
        {
           // throw new NotImplementedException();
        }

        public void SetGroupIdForWindow([MarshalAs(UnmanagedType.Interface)] mozIDOMWindow aParent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
