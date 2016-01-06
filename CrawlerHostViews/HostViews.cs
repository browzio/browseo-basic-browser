using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostView
{
    /// <summary>
    /// Abstract base class that should be inherited by the Host view
    /// </summary>
    public abstract class ProcessorHostView
    {
        #region Abstract Methods
        public abstract void Initialize(HostObject hostObj);

        public abstract void InitializeCefWithCachePath(string path);

        public abstract void NavigateToUrl(string url);

        public abstract void Shutdown();

        public abstract void SetPersonData(string serializedPdataXml);

        public abstract void SetAccessToken(string url);

        public abstract void SetCrawlerState(int state);
        #endregion
    }

    /// <summary>
    /// Abstract base class that should be inherited by a class within the host
    /// application that can make use of the reported progress
    /// </summary>
    public abstract class HostObject
    {
        #region Abstract Methods
        public abstract void ReportInitialized();

        public abstract void ReportSerializedResult(string serializedXML);

        public abstract void ReportSerializedLikesResult(string serializedXML);
        #endregion
    }
}
