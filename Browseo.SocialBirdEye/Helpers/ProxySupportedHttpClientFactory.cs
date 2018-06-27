using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Browseo.SocialBirdEye.Helpers
{
    /// <summary>
    /// var credentials = ....
    ///
    ///   var linkService = new Google.Apis.Urlshortener.v1.UrlshortenerService(new BaseClientService.Initializer()
    ///    {
    ///        HttpClientInitializer = credentials,
    ///        ApplicationName = "MyURLThingy",
    ///        HttpClientFactory = new ProxySupportedHttpClientFactory()
    ///    });
    /// </summary>
    public class ProxySupportedHttpClientFactory : HttpWebRequestWrapper
    {
        public ProxySupportedHttpClientFactory(IWebProxy proxy)
        {
            this.Proxy = proxy;
        }
    }
}
