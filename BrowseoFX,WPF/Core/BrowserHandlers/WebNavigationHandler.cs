using System;
using Gecko.Interfaces;
using Gecko.Interop;
using Gecko.IO;
using Gecko.DOM;
using Gecko;

namespace BrowseoFX_WPF.Core.BrowserHandlers
{
    public sealed class GeckoWebNavigation : ComObject<nsIWebNavigation>, IGeckoObjectWrapper
    {
        public static GeckoWebNavigation Create(nsIWebNavigation instance)
        {
            return new GeckoWebNavigation(instance);
        }

        private GeckoWebNavigation(nsIWebNavigation instance)
            : base(instance)
        {

        }

        public bool CanGoBack
        {
            get { return Instance.GetCanGoBackAttribute(); }
        }

        public bool CanCanGoForward
        {
            get { return Instance.GetCanGoForwardAttribute(); }
        }

        public Uri CurrentUri
        {
            get
            {
                Uri url;
                nsIURI currentUri = Instance.GetCurrentURIAttribute();
                try
                {
                    url = currentUri.ToUri();
                }
                finally
                {
                    Xpcom.FreeComObject(ref currentUri);
                }
                return url;
            }
        }

        public Uri ReferringUri
        {
            get
            {
                Uri url;
                nsIURI currentUri = Instance.GetReferringURIAttribute();
                try
                {
                    url = currentUri.ToUri();
                }
                finally
                {
                    Xpcom.FreeComObject(ref currentUri);
                }
                return url;
            }
        }

        public GeckoDocument Document
        {
            get { return Instance.GetDocumentAttribute().Wrap(GeckoDocument.Create); }
        }

        public void GoBack()
        {
            Instance.GoBack();
        }

        public void GoForward()
        {
            Instance.GoForward();
        }

        /// <summary>
        /// Loads a given URL. This will give priority to loading the requested URL
        /// in the object implementing	this interface.  If it can't be loaded here
        /// however, the URL dispatcher will go through its normal process of content
        /// loading.
        /// </summary>
        /// <param name="url">
        /// The URL string to load. For HTTP and FTP URLs and possibly others,
        /// characters above U+007F will be converted to UTF-8 and then URL-escaped
        /// per the rules of RFC 2396.
        /// </param>
        /// <param name="loadFlags">Flags modifying load behaviour.</param>
        /// <param name="referrer">The referring URL.</param>
        /// <param name="postData">
        /// If the URL corresponds to a HTTP request, then this stream is
        /// appended directly to the HTTP request headers. It may be prefixed
        /// with additional HTTP headers. This stream must contain a &quot;\r\n&quot;
        /// sequence separating any HTTP headers from the HTTP request body.
        /// This parameter is optional and may be null.
        /// </param>
        /// <param name="headers">
        /// If the URI corresponds to a HTTP request, then any HTTP headers
        /// contained in this stream are set on the HTTP request. The HTTP
        /// header stream is formatted as: ( HEADER &quot;\r\n&quot; )*.
        /// This parameter is optional and may be null.
        /// </param>
        public void LoadURI(string url, GeckoLoadFlags loadFlags, string referrer, MimeInputStream postData, MimeInputStream headers)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nsIURI referrerUri = null;
            try
            {
                if (!string.IsNullOrEmpty(referrer))
                    referrerUri = IOService.GetService().CreateNsIUri(referrer);

                Instance.LoadURI(url, (uint)loadFlags, referrerUri, postData != null ? postData.Instance : null, headers != null ? headers.Instance : null);
            }
            finally
            {
                Xpcom.FreeComObject(ref referrerUri);
            }
        }

        /// <summary>
        /// Loads a given URL. This will give priority to loading the requested URL
        /// in the object implementing	this interface.  If it can't be loaded here
        /// however, the URL dispatcher will go through its normal process of content
        /// loading.
        /// </summary>
        /// <param name="url">
        /// The URL string to load. For HTTP and FTP URLs and possibly others,
        /// characters above U+007F will be converted to UTF-8 and then URL-escaped
        /// per the rules of RFC 2396.
        /// </param>
        /// <param name="loadFlags">Flags modifying load behaviour.</param>
        /// <param name="referrer">The referring URL.</param>
        /// <param name="referrerPolicy">The Referrer Policy.</param>
        /// <param name="postData">
        /// If the URL corresponds to a HTTP request, then this stream is
        /// appended directly to the HTTP request headers. It may be prefixed
        /// with additional HTTP headers. This stream must contain a &quot;\r\n&quot;
        /// sequence separating any HTTP headers from the HTTP request body.
        /// This parameter is optional and may be null.
        /// </param>
        /// <param name="headers">
        /// If the URI corresponds to a HTTP request, then any HTTP headers
        /// contained in this stream are set on the HTTP request. The HTTP
        /// header stream is formatted as: ( HEADER &quot;\r\n&quot; )*.
        /// This parameter is optional and may be null.
        /// </param>
        public void LoadURIWithOptions(string url, GeckoLoadFlags loadFlags, string referrer, ReferrerPolicy referrerPolicy, MimeInputStream postData, MimeInputStream headers, string baseUrl)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nsIURI baseUri = null;
            nsIURI referrerUri = null;
            try
            {
                IOService ioSvc = IOService.GetService();
                if (!string.IsNullOrEmpty(referrer))
                    referrerUri = ioSvc.CreateNsIUri(referrer);

                if (!string.IsNullOrEmpty(baseUrl))
                    baseUri = ioSvc.CreateNsIUri(baseUrl);

                Instance.LoadURIWithOptions(url, (uint)loadFlags, referrerUri, (uint)referrerPolicy, postData != null ? postData.Instance : null, headers != null ? headers.Instance : null, baseUri);
            }
            finally
            {
                Xpcom.FreeComObject(ref baseUri);
                Xpcom.FreeComObject(ref referrerUri);
            }
        }

        public void GotoIndex(int index)
        {
            Instance.GotoIndex(index);
        }

        public void Reload(GeckoLoadFlags flags)
        {
            Instance.Reload((uint)flags);
        }

        public void Stop(uint stopFlags)
        {
            Instance.Stop(stopFlags);
        }

    }
}
