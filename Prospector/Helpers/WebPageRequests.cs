using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Prospector.Helpers
{
    public class WebPageRequests
    {

        /// <summary>
        /// Gets the response text for a given url.
        /// </summary>
        /// <param name="url">The url whose text needs to be fetched.</param>
        /// <returns>The text of the response.</returns>
        public static string GetPage(string url, bool isHideMyAss)
        {

            string htmlText = "";
            //HttpWebResponse response = null;
            //Stream stream = null;

            try
            {

                    //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    //response = (HttpWebResponse)request.GetResponse();

                    //stream = response.GetResponseStream();
                    //StreamReader reader = null;

                    //if (response.CharacterSet == null)
                    //{
                    //    reader = new StreamReader(stream);
                    //}
                    //else
                    //{
                    //    reader = new StreamReader(stream, Encoding.UTF8);
                    //}

                    //htmlText = reader.ReadToEnd();

                WebClient client = new WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                htmlText = client.DownloadString(url);
                //htmlText = WebUtility.HtmlDecode(htmlText);
            }
            catch (Exception e)
            {
                return "Exception Thrown";
            }
            //finally
            //{
            //    if (response != null)
            //        response.Close();
            //    if (stream != null)
            //        stream.Close();
            //}
            return htmlText + "[(TCAI)]";
        }
        private static string S_htmlText = "";
        private static void OnDocComplete(string html)
        {
            S_htmlText = html;
        }


        public static async Task<string> GetPageAsync(string urlAddress)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(urlAddress);
            }
        }

        public static string GetPageWithProxy(string urlSite, string proxyIP, string port, string user, string pass)
        {
            HttpWebResponse response = null;
            Stream stream = null;
            string htmlText = "";

            try
            {

                WebProxy myProxy = new WebProxy(proxyIP, Convert.ToInt32(port));
                // myProxy.Address = new Uri("http://" + proxyIP + ":" + port);
                // myProxy.UseDefaultCredentials = true;
                //myProxy.BypassProxyOnLocal = true;
                System.Security.SecureString ss = new System.Security.SecureString();
                if (user != "")
                    myProxy.Credentials = new NetworkCredential(user, pass);

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlSite);
                //request.Proxy = myProxy;
                //request.Proxy.Credentials = myProxy.Credentials;
                //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2227.0 Safari/537.36";

                WebClient client = new WebClient();
                client.Proxy = myProxy;
                htmlText = client.DownloadString(urlSite);
                //response = (HttpWebResponse)request.GetResponse();

                //stream = response.GetResponseStream();
                //StreamReader reader = null;

                //if (response.CharacterSet == null)
                //{
                //    reader = new StreamReader(stream);
                //}
                //else
                //{
                //    reader = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet));
                //}

                //htmlText = reader.ReadToEnd();
            }
            catch (WebException ex)
            {
                //string message = ex.Message;
                //if (response != null)
                //    response = (HttpWebResponse)ex.Response;
                //if (null != response)
                //{
                //    message = response.StatusDescription;
                //    response.Close();
                //}
                return "A Burnt Proxy";
            }
            catch (Exception ex)
            {
                return "A Burnt Proxy";
            }
            finally
            {
                if (response != null)
                    response.Close();
                if (stream != null)
                    stream.Close();
            }

            return htmlText + "[(TCAI)]";
        }

        public static byte[] Post(string uri, NameValueCollection pairs)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, pairs);
            }
            return response;
        }
    }
}
