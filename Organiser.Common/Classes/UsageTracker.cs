using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Organiser.Common.Classes
{
    public class UsageTracker
    {
        public const string Usage_Type_AddressChange = "Address Changed";
        public const string Usage_Type_BrowserShare = "Share From Browser";
        public const string Usage_Type_BrowserStart = "Browser Started";
        public const string Usage_Type_OpenedIndeser = "Indexer Opened";
        public const string Usage_Type_IndexedLinks = "Indexed Links";
        public const string Usage_Type_KKSearch = "Prospector King Kontent Search";
        public const string Usage_Type_ProspectorSearch = "Prospector Search";
        public const string Usage_Type_ProspectorToPBne = "Prospector Sent Link To pbn";
        public const string Usage_Type_ProspectorToBrowser = "Prospector Sent Link To browser";
        public const string Usage_Type_Navigatedtorsstab = "Navigated To Rss Tab";
        public const string Usage_Type_SentToBrowserfromrss = "Sent To Browser From RSS tab";
        public const string Usage_Type_ShareFromRss = "Social Share From RSS";
        public const string Usage_Type_TpBroserFromRss = "Sent To Browser From RSS tab";
        public const string Usage_Type_OpenedRssMash= "Opened Rss Masher";
        public const string Usage_Type_SentToMasher= "Sent To Masher";
        public const string Usage_Type_GotRssMashResults = "Rss Masher Results";
        public const string Usage_Type_ToYoutubeUrler = "Youtube Urlr Opened";
        public const string Usage_Type_CreatedYoutubeUrls = "Created Links for video";
        public const string Usage_Type_FacebookCralEvent = "Facebook Crawler Module";
        public const string Usage_Type_SEOEvent = "SEO Module";

        const string SPLITTER = "qwertyuiiioop";   

        private static List<KeyValuePair<string, string>> UsageList;

        static object mLock = new object();

        public static void AddTraceCookie(string traceType)
        {
            new Thread(() =>
                       {
                           try
                           {
                               if (UsageList == null)
                                   UsageList = new List<KeyValuePair<string, string>>();

                               UsageList.Add(new KeyValuePair<string, string>(traceType, DateTime.Now.ToString()));
                               if (UsageList.Count > 5)
                               {

                                   try
                                   {
                                       SaveAllTrackedDataList();
                                       UsageList.Clear();
                                   }
                                   catch { }
                               }
                           }
                           catch { }
                       }).Start();
        }

        public static void SaveAllTrackedDataList()
        {
            lock (mLock)
            {
                var datetime = DateTime.Now;
                var date = datetime.Date;
                var dtString = date.Day + "_" + datetime.Month + "_" + datetime.Year;

                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Track", GloableProfData.PData.ProjectName, dtString);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Track", GloableProfData.PData.ProjectName, dtString, "Usage.txt");
                foreach (KeyValuePair<string, string> trackCookie in UsageList)
                {
                    string line = trackCookie.Key + SPLITTER + trackCookie.Value;
                    line = MyFilesDatabase.EncodeTo64(line);
                    File.AppendAllText(filePath, line + Environment.NewLine);
                }
            }
        }
    }
}
