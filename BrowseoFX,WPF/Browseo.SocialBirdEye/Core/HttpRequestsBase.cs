using BrowseoFX_WPF.Core;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Core
{
    public class HttpRequestsBase
    {
        public enum Method { GET, POST, DELETE };

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public static string WebRequest(Method method, string url, string postData)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Proxy = MyFilesDatabase.GetRequestsProxy();
            webRequest.UserAgent  = BrowseoFXManager.Instance.SettingsHandler.UserAgent_Current;
            //webRequest.Timeout = 20000;
            if (method == Method.POST || method == Method.DELETE)
            {
                //webRequest.ContentType = "application/json";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                //  webRequest.ContentType = "multipart/form-data;";
                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {

                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;

        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public static string WebResponseGet(HttpWebRequest webRequest)
        {
            //StreamReader responseReader = null;
            string responseData = "";

            try
            {
                using (HttpWebResponse httResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    Stream responseStream = httResponse.GetResponseStream();
                    using (StreamReader responseReader = new StreamReader(responseStream, Encoding.Default))
                    {
                        responseData = responseReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return responseData;
        }

        public static string GetByWebRequest(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserSettimgs.UserAgent_CurrentFFBuild);
            webClient.Proxy = MyFilesDatabase.GetRequestsProxy();

            var downloaded = webClient.DownloadString(url);

            webClient.Dispose();

            return downloaded;
        }

        //public static string GetByCurlWebRequest(string url, string AccessToken)
        //{
        //    HttpWebRequest req = (HttpWebRequest)System.Net.WebRequest.Create(url);
        //    req.Proxy = MyFilesDatabase.GetRequestsProxy();
        //    req.Host = "api.linkedin.com";
        //    //req.Connection = "Keep-Alive";
        //    //req.Headers.Add("Proxy-Connection", "Keep-Alive");
        //    req.Headers.Add("Authorization", AccessToken);
        //    req.UserAgent = BrowseoFXManager.Instance.SettingsHandler.UserAgent_Current;
        //    // req.CookieContainer allows you access cookies.

        //    var response = req.GetResponse();
        //    string webcontent;
        //    using (var strm = new StreamReader(response.GetResponseStream()))
        //    {
        //        webcontent = strm.ReadToEnd();
        //    }

        //    return webcontent;
        //}

        public static string HttpWebGetRequest(string Url)
        {
            string output = string.Empty;
            var Pinterestrequest = (HttpWebRequest)System.Net.WebRequest.Create(Url);
            Pinterestrequest.Method = "GET";
            Pinterestrequest.Credentials = CredentialCache.DefaultCredentials;
            Pinterestrequest.AllowWriteStreamBuffering = true;
            Pinterestrequest.ServicePoint.Expect100Continue = false;
            Pinterestrequest.PreAuthenticate = false;
            Pinterestrequest.Proxy = MyFilesDatabase.GetRequestsProxy();

            using (var response = Pinterestrequest.GetResponse())
            {
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1252)))
                {
                    output = stream.ReadToEnd();
                }
            }
            return output;
        }


        public static async Task<T> GetUrlRequest<T>(string url)
        {
            string jsonResponse = await Task.Run(() =>
            {
                return HttpRequestsBase.HttpWebGetRequest(url);
            });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonResponse);
        }
    }
}
