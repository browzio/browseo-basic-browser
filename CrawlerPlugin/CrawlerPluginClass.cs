using CrawlerContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerPlugin
{
    public class CrawlerPluginClass: MarshalByRefObject, IPlugin
    {
        Crawler.Crawler crawler;

        public CrawlerPluginClass()
        {
            crawler = new Crawler.Crawler();
            crawler.OnReportInitialized += Crawler_OnReportInitialized;
            crawler.OnReportSerializedResult += Crawler_OnReportSerializedResult;
        }

        private void Crawler_OnReportSerializedResult(string obj)
        {
            OnReportSerializedResult(obj);
        }

        private void Crawler_OnReportInitialized()
        {
            OnReportInitialized();
        }

        public event Action OnReportInitialized;
        public event Action<string> OnReportSerializedResult;

        public virtual object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(GetType())) return this;
            return null;
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }

        public void InitializeCefWithCachePath(string path)
        {
            crawler.InitializeCefWithCachePath(path);
        }

        public void NavigateToUrl(string url)
        {
            crawler.NavigateToUrl(url);
        }

        public void SetAccessToken(string fbtokenLink)
        {
            crawler.SetAccessToken(fbtokenLink);
        }

        public void SetCrawlerState(int state)
        {
            crawler.SetCrawlerState(state);
        }

        public void SetPersonData(string serializedPdata)
        {
            crawler.SetPersonData(serializedPdata);
        }

        public void Shutdown()
        {
            crawler.Shutdown();
        }
    }
}
