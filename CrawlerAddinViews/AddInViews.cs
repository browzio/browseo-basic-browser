using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddInView
{
    /// <summary>
    /// Abstract base class that should be inherited by all AddIns
    /// </summary>
    [AddInBase]
    public abstract class ProcessorAddInView
    {
        #region Abstract Methods
        public abstract void Initialize(HostObject hostObj);

        public abstract void SetPersonData(string serializedPdataXml);

        public abstract void InitializeCefWithCachePath(string path);

        public abstract void ShutDown();

        public abstract void NavigateToUrl(string url);

        public abstract void SetAccessToken(string url);

        public abstract void SetCrawlerState(int state);
        #endregion
    }

    /// <summary>
    /// Abstract class that should be inherited by an object that needs to communicate
    /// between the host Contract to View adapter <see cref="AddInSideAdapter.HostObjectContractToViewAddInAdapter">
    /// HostObjectContractToViewAddInAdapter</see>
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
