using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Organiser.Common.Classes.Helpers
{
    public class WebRequests
    {
        public static async Task<string> AsyncDownloadStringWithProfileProxy(string url)
        {
            string source = "";

            using (var client = new WebClient())
            {
                if (!GloableProfData.PData.ProxyIP.IsNullOrEmpty() && !GloableProfData.PData.ProxyPort.IsNullOrEmpty())
                    client.Proxy = new WebProxy(GloableProfData.PData.ProxyIP, Convert.ToInt32(GloableProfData.PData.ProxyPort));
                if (!GloableProfData.PData.ProxyUsername.IsNullOrEmpty() && !GloableProfData.PData.ProxyPassword.IsNullOrEmpty())
                    client.Proxy.Credentials = new NetworkCredential(GloableProfData.PData.ProxyUsername, GloableProfData.PData.ProxyPassword);

                source = await client.DownloadStringTaskAsync(url);
            }

            return source;
        }
    }
}
