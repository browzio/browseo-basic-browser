using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerContracts
{
    public interface IPlugin
    {
        event Action OnReportInitialized;
        event Action<string> OnReportSerializedResult;

        void SetCrawlerState(int state);
        void InitializeCefWithCachePath(string path);

        object GetService(Type serviceType);
        void SetPersonData(string serializedPdata);
        void NavigateToUrl(string url);
        void SetAccessToken(string fbtokenLink);
        void Shutdown();
    }
}
