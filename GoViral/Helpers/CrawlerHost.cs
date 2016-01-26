using Contract;
using CrawlerContracts;
using GoViral.Models;
using Organiser.Common.Classes;
using Organiser.Common.Classes.Crawler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoViral.Helpers
{
    public class CrawlerPreInitState
    {
        public CrawlerStates state = CrawlerStates.FbGraphCrawl;
        public LikesData likesData;
        public FacebookGraphPostResult graphResult;
        public Folder folder;
        public ListOption option;
        public string url;
    }

    public class CrawlerHost : IHost
    {
        public event Action<string, string> OnReportFatalError = delegate { };
        public event Action<string> OnReportProgress = delegate { };
        public event Action<string, CrawlerPreInitState> OnReportGotGraphData = delegate { };//likes, url 

        public List<CrawlerPreInitState> PreInitStates { get; set; }

        public HostToPluginContract HostToPluginContract { get; private set; }

        public int Initialized = 0;
        public int totalToCrawl;

        public CrawlerHost()
        {
            PreInitStates = new List<CrawlerPreInitState>();

            HostToPluginContract = new HostToPluginContract(this);
            HostToPluginContract.LoadPlugin(new PluginStartupInfo()
            {
                FullAssemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Crawler.dll"),
                MainClass = "Crawler.Crawler",
                Name = "BrowseoNinjaCrawler",
                AssemblyName = "Crawler",
            });
            HostToPluginContract.OnSerializedResultsArived += new ReportSerializedResultDel(ReportSerializedResult);
        }

        #region ihost stuff
        public int HostProcessId { get { return Process.GetCurrentProcess().Id; } }

        public void ReportInitialized()
        {
            if (Initialized == 1)
            {
                Console.WriteLine("Initialized.");
                Initialized = 2;
                HostToPluginContract.SetAccessToken(Social.FACEBOOK_GRAPH_LINK);
            }
            else
            {
                navigateToNextUrl();
            }
        }

        public void ReportFatalError(string userMessage, string fullExceptionText)
        {
            Console.WriteLine(userMessage + " " + fullExceptionText);
            OnReportFatalError(userMessage, fullExceptionText);
        }

        public void ReportSerializedResult(string serializedFBresult)
        {
            if (PreInitStates.Count > 0)
            {
                OnReportGotGraphData(serializedFBresult, PreInitStates[0]);        
            }
        }
        #endregion

        internal void IninAdin()
        {   
            totalToCrawl = PreInitStates.Count;

            if (Initialized == 0)
            {
                Initialized = 1;
                HostToPluginContract.SetPersonData(GloableProfData.PData.XmlSerializeToString());
                HostToPluginContract.InitializeCefWithCachePath(path: Path.Combine(Organiser.Common.Classes.MyFilesDatabase.GetBaseDir(), "Caches\\" + GloableProfData.PData.ProjectName));
            }
            else
            {
                navigateToNextUrl();
            }
        }

        public void navigateToNextUrl()
        {
            if (PreInitStates != null && PreInitStates.Count > 0 && Initialized == 2)
            {
                if (PreInitStates.Count == 0) return;
                OnReportProgress("START: " + PreInitStates[0].url.Replace("https://www.facebook.com/","") + " " + PreInitStates[0].state.GetDescription());
                HostToPluginContract.SetCrawlerState(Convert.ToInt32(PreInitStates[0].state));
                HostToPluginContract.NavigateToUrl(PreInitStates[0].url);
            }
        }

        public void Shutdown()
        {
            HostToPluginContract.Shutdown(); 
        }
    }
}
