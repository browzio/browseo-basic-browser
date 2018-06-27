using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace BrowserHost.Helpers
{
    public class ImageDownloadCallback : CefDownloadImageCallback
    {
        public event Action<CefImage> OnDownloadImageFinishedEvent;
        public event Action<CefImage, CefBinaryValue> OnDownloadImageFinishedEvent2;
        protected override void OnDownloadImageFinished(string imageUrl, int httpStatusCode, CefImage image)
        {
            OnDownloadImageFinishedEvent?.Invoke(image);
        }

        //protected override void OnDownloadImageFinished2(string imageUrl, int httpStatusCode, CefImage image, CefBinaryValue binaryFromPng)
        //{
        //    OnDownloadImageFinishedEvent2?.Invoke(image, binaryFromPng);

        //}
    }
}
