using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostSideAdapter
{
    /// <summary>
    /// Adapter use to talk to <see cref="HostView.NumberProcessorHostView">Host View</see>
    /// </summary>
    [HostAdapter]
    public class ProcessorContractToViewHostAdapter : HostView.ProcessorHostView
    {
        #region Data
        private Contract.IProcessorContract contract;
        private ContractHandle contractHandle;
        #endregion

        #region Ctor
        public ProcessorContractToViewHostAdapter(Contract.IProcessorContract contract)
        {
            this.contract = contract;
            contractHandle = new ContractHandle(contract);     
        }
        #endregion

        #region Public Methods
        public override void Initialize(HostView.HostObject host)
        {
            HostObjectViewToContractHostAdapter hostAdapter = new HostObjectViewToContractHostAdapter(host);
            contract.Initialize(hostAdapter);
        }

        public override void Shutdown()
        {
            contract.Shutdown();
        }

        public override void InitializeCefWithCachePath(string path)
        {
            contract.InitializeCefWithCachePath(path);
        }

        public override void SetPersonData(string serializedPdataXml)
        {
            contract.SetPersonData(serializedPdataXml);
        }

        public override void NavigateToUrl(string url)
        {
            contract.NavigateToUrl(url);
        }

        public override void SetAccessToken(string url)
        {
            contract.SetAccessToken(url);
        }

        public override void SetCrawlerState(int state)
        {
            contract.SetCrawlerState(state);
        }
        #endregion
    }


    /// <summary>
    /// Allows Host side adapter to talk back to HostView
    /// </summary>
    public class HostObjectViewToContractHostAdapter : ContractBase, Contract.IHostObjectContract
    {
        #region Data
        private HostView.HostObject view;
        #endregion

        #region Public Methods
        public HostObjectViewToContractHostAdapter(HostView.HostObject view)
        {                                                         
            this.view = view;   
        }

        public void ReportInitialized()
        {
            view.ReportInitialized();
        }

        public void ReportSerializedLikesResult(string serializedXML)
        {
            view.ReportSerializedLikesResult(serializedXML);
        }

        public void ReportSerializedResult(string serializedXML)
        {
            new System.Threading.Thread(() =>
            {
                view.ReportSerializedResult(serializedXML);
            }).Start();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion
    }
}
