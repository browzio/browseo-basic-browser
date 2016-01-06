using System.AddIn.Contract;
using System.AddIn.Pipeline;  

namespace Contract
{
    /// <summary>
    /// The actual AddIn contract that is implemented by the
    /// <see cref="AddInSideAdapter.NumberProcessorViewToContractAdapter">AddIn Adapter</see>
    /// </summary>
    [AddInContract]
    public interface IProcessorContract : IContract
    {
        #region Methods 
        void Initialize(IHostObjectContract hostObj);

        void InitializeCefWithCachePath(string path);

        void Shutdown();

        void SetPersonData(string serializedPdataXml);

        void NavigateToUrl(string url);

        void SetAccessToken(string url);

        void SetCrawlerState(int state);
        #endregion
    }

    /// <summary>
    /// The actual Host contract that is implemented by the
    /// <see cref="HostInSideAdapter.HostObjectViewToContractHostAdapter">Host Adapter</see>
    /// Which enabled the AddIn to talk back to the host
    /// </summary>
    public interface IHostObjectContract : IContract
    {
        #region Methods
        void ReportInitialized();
        void ReportSerializedResult(string serializedXML);
        void ReportSerializedLikesResult(string serializedXML);
        #endregion
    }
}
