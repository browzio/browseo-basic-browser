using NichResearch.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NichResearch.Crawlers
{
    public class SocialMentionCrawler
    {
        public Action<List<SocialMentionItem>> OnReturnResults = delegate { };
        public Action OnFinished = delegate { };

        Regex hrefExpresion = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");

        public void Search(string KeyWords, string searchType)
        {
            if (KeyWords == null)
            {
                OnFinished();
                return;
            }
            new Thread(() =>
            {
                List<Task> taskList = new List<Task>();
                foreach (string item in KeyWords.Split(','))
                {
                    taskList.Add(Task.Run(() =>
                                    {
                                        string searchWord = item;
                                        searchWord = searchWord.Trim();
                                        searchWord = searchWord.Replace(" ", "+");
                                        WebClient client = new WebClient();
                                        client.Encoding = System.Text.Encoding.UTF8;
                                        string htmlText = "";
                                        for (int i = 0; i < 5; i++)
                                        {
                                            htmlText = client.DownloadString("http://socialmention.com/search?q=" + searchWord + "&t=" + searchType);
                                            if (!htmlText.Contains("<h1>Searching content from across the universe...</h1>"))
                                                break;
                                        }

                                        List<SocialMentionItem> results = Find(htmlText);
                                        OnReturnResults(results);
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

        private List<SocialMentionItem> Find(string htmlText)
        {
            List<SocialMentionItem> resultList = new List<SocialMentionItem>();

            string[] resultsList = htmlText.Split(new string[] { "<div id=\"results\" class=\"\">" }, StringSplitOptions.None)[1]
                                           .Split(new string[] { "<div class=\"result clearfix\">" }, StringSplitOptions.None);
            foreach (string result in resultsList)
            {
                if (result.Length < 10) continue;

                try
                {
                    SocialMentionItem resultItem = new SocialMentionItem();

                    //images
                    string both = "";
                    foreach (Match item in Regex.Matches(result, "<img.+?src=[\"'](.+?)[\"'].*?>", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        both += item.Groups[1].Value + "{[||]}";
                    }
                    resultItem.IconSentiment = "http://socialmention.com" + both.Split(new string[] { "{[||]}" }, StringSplitOptions.None)[0];
                    resultItem.Icon = both.Split(new string[] { "{[||]}" }, StringSplitOptions.None)[1].Replace("{[||]}", "");

                    //title and link
                    string title = result.Split(new string[] { "<h3>" }, StringSplitOptions.None)[1];
                    title = title.Remove(title.IndexOf("</h3>"));
                    foreach (Match match in hrefExpresion.Matches(title))
                    {
                        resultItem.Title = match.Groups["value"].Value;
                        resultItem.Link = match.Groups["href"].Value;
                    }

                    //description
                    resultItem.Description = result.Split(new string[] { "<div class=\"description\">" }, StringSplitOptions.None)[1];
                    resultItem.Description = resultItem.Description.Remove(resultItem.Description.IndexOf("</div>"));
                    resultItem.Description = resultItem.Description.Trim();

                    //info
                    string when = result.Split(new string[] { "<br />" }, StringSplitOptions.None)[1];
                    string whenFull = when.Remove(when.IndexOf("<a"));
                    int i = 0;
                    foreach (Match match in hrefExpresion.Matches(when))
                    {
                        i++;
                        if (i >= 2) whenFull += " on ";
                        whenFull += match.Groups["value"].Value;
                    }
                    if (whenFull.Contains("on <div"))
                        whenFull = whenFull.Remove(whenFull.IndexOf("on <div"));
                    resultItem.Info = whenFull;

                    resultList.Add(resultItem);
                }
                catch { }
            }

            return resultList;
        }
    }
}
