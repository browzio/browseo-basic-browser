using Organiser.Common.Classes;
using Prospector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospector.Helpers
{
 public class GoogleCrawler
    {
        string[] WebsiteUrl { get; set; }
        IList<string> Keywords { get; set; }
        IList<string> PresentableKeywords { get; set; }
        int MaxPagesToCheck { get; set; }

        string searchUrl = "https://www.google.co.il/search?q=";
        const string pageNumber = "&start=";
        const string googleSearchSplit = "class=\"r\"";
        const string searchEngine = "Google";

        public Action<SearchResult, bool> LinkWasAddedToList = delegate { };
        public Action<int,int> OnPageCountUpdate = delegate { };
        public Action<bool> OnReturnResults = delegate { };
        public bool DidCancelSearch { get; set; }
       
        private object m_lock = new object();
        private bool didShowException;

        public GoogleCrawler(string websiteUrl, IList<string> keywords, IList<string> presentableKeywords, int maxPagesToCheck,string LinkCode)
        {
            this.WebsiteUrl = websiteUrl.Split(',');
            this.Keywords = keywords;
            this.PresentableKeywords = presentableKeywords;
            this.MaxPagesToCheck = maxPagesToCheck;
            searchUrl = "https://www.google"+LinkCode+"/search?q=";
        }

        public GoogleCrawler(string websiteUrl, int maxPagesToCheck)
        {
            MaxPagesToCheck = maxPagesToCheck;
            searchUrl = websiteUrl;
        }

        public void FindResults(bool useproxy, int maxThreads = 1)
        {
            didShowmsgBox = false;
            DidCancelSearch = false;

            int pageCount = 0;
            List<MyKeyVal> searchUrls = new List<MyKeyVal>();
                pageCount = 0;
                while (pageCount != MaxPagesToCheck)
                {
                    string nextSearchUrl = searchUrl;
                    if (pageCount != 0)
                        nextSearchUrl += pageNumber + (pageCount * 10).ToString();
                    searchUrls.Add(new MyKeyVal() { link = nextSearchUrl, pagenum = pageCount });
                    pageCount += 1;
                }

            pageCount = 0;
            if (maxThreads > 1)
            {
                List<Task> taskList = new List<Task>();
                List<List<MyKeyVal>> chunckList = Lists.BreakIntoChunks(searchUrls, 
                    maxThreads < searchUrls.Count ? 
                            (int)Math.Round(searchUrls.Count / (decimal)maxThreads) : searchUrls.Count);
                foreach (List<MyKeyVal> list in chunckList)
                {
                    taskList.Add(Task.Run(() =>
                            {
                                loopThroughAngGetResults(list, useproxy);
                            }));
                }
                int tasklistcount = taskList.Count;
                while (taskList.Count > 0)
                {
                    int SelectedLink = Task.WaitAny(taskList.ToArray());
                    pageCount++;
                    OnPageCountUpdate(pageCount, tasklistcount);
                    taskList.RemoveAt(SelectedLink);
                }
                //Task.WaitAll(taskList.ToArray());
            }
            else
            {
                loopThroughAngGetResults(searchUrls, useproxy);
            }
        }

        bool didShowmsgBox;
        private void loopThroughAngGetResults(List<MyKeyVal> searchUrls, bool useProxy)
        {
            string[] individualResults;
            for (int i = 0; i < searchUrls.Count; i++)
            {
                if (DidCancelSearch || didShowmsgBox)
                    return;

                MyKeyVal url = searchUrls[i];

                string resultPage = WebPageRequests.GetPage(url.link, false, useProxy);
                if (resultPage == "Exception Thrown")
                {
                    OnReturnResults(false);
                    return;
                }

                lock (m_lock)
                {
                    if (resultPage == "")
                        return;
                    resultPage = resultPage.Substring(resultPage.IndexOf("<h3") + 1);
                    resultPage = resultPage.Substring(0, resultPage.LastIndexOf("</body"));
                    individualResults = resultPage.Split(new string[] { googleSearchSplit }, StringSplitOptions.None);
                    splitResults(individualResults, url.pagenum);
                }
            }
        }

        private void splitResults(string[] individualResults, int pageNum)
        {
            int counts = 0;
            for (int i = 0; i < individualResults.Length; i++)
            {
                try
                {
                    string linkSplit = "url?q=";
                    string[] splistResult = individualResults[i].Split(new string[] { linkSplit }, StringSplitOptions.None);
                    //splistResult[1] = splistResult[1].Replace("&amp;", "");
                    string link = "";
                    foreach (char c in splistResult[1])
                    {
                        if (c == '&') break;
                        link += c;
                    }
                    link = System.Net.WebUtility.UrlDecode(link);

                    string title = splistResult[1].Split(new string[] { "</a>" }, StringSplitOptions.None)[0];
                    title = title.Substring(title.IndexOf('>') + 1);
                    title = title.Replace("<br>", "");//
                    title = title.Replace("<b>", "");
                    title = title.Replace("</b>", "");
                    title = title.Replace("�", "");
                    title = cleanString(title);


                    string description = splistResult[2].Split(new string[] { @"class=""st"">" }, StringSplitOptions.None)[1];
                    description = description.Remove(description.IndexOf("</span>"));
                    description = description.Replace("<br>", "");
                    description = description.Replace("<b>", "");
                    description = description.Replace("</b>", ""); 
                    description = description.Replace("�", "");
                    description = cleanString(description);



                    int linkPos = pageNum == 0 ? (i - counts) : pageNum * 10 + (i - counts);
                    SearchResult sr = new SearchResult
                    {
                        Link = link,
                        Title = title,
                        Description=description,
                        Position = linkPos,
                        SearchEngine = searchEngine
                    };
                    LinkWasAddedToList(sr, false);

                }
                catch { counts++; }
            }
        }

        private string cleanString(string stringToClean)
        {
            string returnstring = "";
            if (stringToClean.Contains('&'))
            {
                bool noAdd = false;
                foreach (char c in stringToClean)
                {
                    if (c == '&') { noAdd = true; continue; }
                    if (noAdd) { if (c == ';') noAdd = false; continue; }
                    returnstring += c;
                }
            }
            else
            {
                returnstring = stringToClean;
            }

            return returnstring;
        }
    }
}
