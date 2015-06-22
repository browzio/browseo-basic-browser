

using NichResearch.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
namespace NichResearch.Crawlers
{
    public class YoutubeCrawler
    {
        public Action<List<YoutubeItem>> OnReturnResults = delegate { };
        public Action OnFinished = delegate { };

        public void Search(string keyWordsString)
        {
            if (keyWordsString == null)
            {
                OnFinished();
                return;
            }
            new Thread(() =>
            {
                List<Task> taskList = new List<Task>();

                string[] keywords = keyWordsString.Split(',');
                foreach (string keyword in keywords)
                {
                    taskList.Add(Task.Run(() =>
                                {
                                    string searchWord = keyword;
                                    searchWord = searchWord.Trim();
                                    searchWord = searchWord.Replace(" ", "+");

                                    WebClient client = new WebClient();
                                    client.Encoding = System.Text.Encoding.UTF8;
                                    string htmlText = client.DownloadString("https://www.youtube.com/results?search_query=" + searchWord);
                                    List<YoutubeItem> items = Find(htmlText);
                                    OnReturnResults(items);
                                }));
                }
                while (taskList.Count > 0)
                {
                    int SelectedLink = Task.WaitAny(taskList.ToArray());
                    taskList.RemoveAt(SelectedLink);
                }
                OnFinished();
            }).Start();
        }

        public List<YoutubeItem> Find(string file)
        {
            List<YoutubeItem> list = new List<YoutubeItem>();

            // Find all matches in file.
            MatchCollection mc = Regex.Matches(file, "<ol class=\"item-section\">.+?</ol>", RegexOptions.Singleline);
            string[] listContent = mc[0].Value.Split(new string[] { "<div class=\"yt-lockup yt-lockup-tile yt-lockup-video vve-check clearfix yt-uix-tile\"" }, System.StringSplitOptions.None);

            foreach (string linkItem in listContent)
            {
                if (!linkItem.Contains("data-context-item")) continue;

                try
                {
                    YoutubeItem youtubeItem = new YoutubeItem();

                    //get the image link
                    string[] imageTagSplit = linkItem.Split(new string[] { "<img src=\"" }, System.StringSplitOptions.None);
                    youtubeItem.ImageLink = "https:" + imageTagSplit[1].Split(new string[] { "\" width=" }, System.StringSplitOptions.None)[0];
                    if (youtubeItem.ImageLink.Contains("data-thumb"))
                    {
                        youtubeItem.ImageLink = "https:" + youtubeItem.ImageLink.Split(new string[] { "data-thumb=\"" }, System.StringSplitOptions.None)[1];
                    }
                    //get content title link
                    youtubeItem.Link = "https://www.youtube.com" + linkItem.Split(new string[] { "<a href=\"" }, System.StringSplitOptions.None)[1]
                                                                   .Split(new string[] { "\" class=" }, System.StringSplitOptions.None)[0];
                    youtubeItem.Title = linkItem.Split(new string[] { "\" title=\"" }, System.StringSplitOptions.None)[3]
                                       .Split(new string[] { "\" rel=\"" }, System.StringSplitOptions.None)[0];
                    youtubeItem.Title = youtubeItem.Title.Replace("&#39;", "");
                    if (youtubeItem.Title.Contains("\" aria-describedby="))
                    {
                        youtubeItem.Title = youtubeItem.Title.Remove(youtubeItem.Title.IndexOf("\" aria-describedby="));
                    }
                    //get by link and title
                    youtubeItem.ByLink = "https://www.youtube.com" + linkItem.Split(new string[] { "<div class=\"yt-lockup-byline\">by <a href=\"" }, System.StringSplitOptions.None)[1]
                                           .Split(new string[] { "\" class=\"" }, System.StringSplitOptions.None)[0];
                    youtubeItem.ByName = youtubeItem.ByLink.Substring(youtubeItem.ByLink.LastIndexOf("/")).Replace("/", "");

                    //meta data
                    string[] metaData = linkItem.Split(new string[] { "<div class=\"yt-lockup-meta\"><ul class=\"yt-lockup-meta-info\">" }, System.StringSplitOptions.None);
                    youtubeItem.TimeAgo = metaData[1].Split(new string[] { "<li>" }, System.StringSplitOptions.None)[1];
                    youtubeItem.TimeAgo = youtubeItem.TimeAgo.Replace("</li>", "");
                    youtubeItem.Views = metaData[1].Split(new string[] { "<li>" }, System.StringSplitOptions.None)[2];
                    youtubeItem.Views = youtubeItem.Views.Split(new string[] { "</li>" }, System.StringSplitOptions.None)[0];

                    //description 
                    if (!linkItem.Contains("<div class=\"yt-lockup-description yt-ui-ellipsis yt-ui-ellipsis-2\" dir=\"ltr\">")) continue;
                    youtubeItem.Description = linkItem.Split(new string[] { "<div class=\"yt-lockup-description yt-ui-ellipsis yt-ui-ellipsis-2\" dir=\"ltr\">" }, System.StringSplitOptions.None)[1]
                                             .Split(new string[] { "</div>" }, System.StringSplitOptions.None)[0];
                    if (youtubeItem.Description.Contains("<a href="))
                    {
                        MatchCollection matches = Regex.Matches(youtubeItem.Description, "<a href=\"(.*?)\">", RegexOptions.Singleline); //spelling error
                        foreach (Match item in matches)
                        {
                            var url = item.Groups[1].Value;
                            youtubeItem.Description = youtubeItem.Description.Replace(item.Value, url.Split(new string[] { "\" target=\"" }, System.StringSplitOptions.None)[0]);
                            youtubeItem.Description = youtubeItem.Description.Replace("</a>", "");
                        }
                    }
                    youtubeItem.Description = youtubeItem.Description.Replace("<b>", "");
                    youtubeItem.Description = youtubeItem.Description.Replace("</b>", "");
                    youtubeItem.Description = youtubeItem.Description.Replace("&#39;", "");

                    list.Add(youtubeItem);
                }
                catch { }
            }

            return list;
        }
    }
}
