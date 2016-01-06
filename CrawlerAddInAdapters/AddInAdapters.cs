using System;
using System.AddIn.Pipeline;

namespace AddInSideAdapter
{
    /// <summary>
    /// Adapter use to talk to AddIn <see cref="Contract.INumberProcessorContract">AddIn Contract</see>
    /// </summary>
    [AddInAdapter]
    public class ProcessorViewToContractAdapter :ContractBase, Contract.IProcessorContract
    {
        #region Data
        private AddInView.ProcessorAddInView view;
        #endregion

        #region Ctor
        public ProcessorViewToContractAdapter(AddInView.ProcessorAddInView view)
        {
            this.view = view;
        }
        #endregion

        #region Public Methods
        public void Initialize(Contract.IHostObjectContract hostObj)
        {
            view.Initialize(new HostObjectContractToViewAddInAdapter(hostObj));
        }

        public void Shutdown()
        {
            view.ShutDown();
        }

        public void InitializeCefWithCachePath(string path)
        {
            view.InitializeCefWithCachePath(path);
        }

        public void SetPersonData(string serializedPdataXml)
        {
            view.SetPersonData(serializedPdataXml);
        }

        public void NavigateToUrl(string url)
        {
            view.NavigateToUrl(url);
        }

        public void SetAccessToken(string url)
        {
            view.SetAccessToken(url);
        }

        public void SetCrawlerState(int state)
        {
            view.SetCrawlerState(state);
        }
        #endregion
    }


    /// <summary>
    /// Allows AddIn adapter to talk back to HostView
    /// </summary>
    public class HostObjectContractToViewAddInAdapter : AddInView.HostObject
    {
        #region Data
        private Contract.IHostObjectContract contract;
        private ContractHandle handle;
        #endregion

        #region Ctor
        public HostObjectContractToViewAddInAdapter(Contract.IHostObjectContract contract)
        {
            this.contract = contract;
            this.handle = new ContractHandle(contract);
        }
        #endregion

        #region Public Methods
        public override void ReportInitialized()
        {
            contract.ReportInitialized();
        }

        public override void ReportSerializedLikesResult(string serializedXML)
        {
            contract.ReportSerializedLikesResult(serializedXML);
        }

        public override void ReportSerializedResult(string serializedXML)
        {
            contract.ReportSerializedResult(serializedXML);
        }
        #endregion
    }
}
