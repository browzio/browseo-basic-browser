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
        const string SPLITTER = "qwertyuiiioop"; 

        public static string ProjectName;

        private static List<KeyValuePair<string, string>> UsageList;

        public static void AddTraceCookie(string traceType)
        {
            if (UsageList == null)
                UsageList = new List<KeyValuePair<string, string>>();

            UsageList.Add(new KeyValuePair<string,string>(traceType, DateTime.Now.ToString()));
            if (UsageList.Count > 10)
            {
                new Thread(() => {
                    SaveAllTrackedDataList();
                    UsageList.Clear();
                }).Start();
            }
        }

        public static void SaveAllTrackedDataList()
        {
            string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Track", ProjectName);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "Track", ProjectName, "Usage.txt");
            foreach (KeyValuePair<string, string> trackCookie in UsageList)
            {
                string line = trackCookie.Key + SPLITTER + trackCookie.Value;
                line = MyFilesDatabase.EncodeTo64(line);
                File.AppendAllText(filePath, line + Environment.NewLine);
            }
        }
    }
}
