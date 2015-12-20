using System;
using CrawlerContracts;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Diagnostics;

namespace Contract
{
    ///// <summary>
    ///// The actual AddIn contract that is implemented by the
    ///// <see cref="AddInSideAdapter.NumberProcessorViewToContractAdapter">AddIn Adapter</see>
    ///// </summary>
    //[AddInContract]
    //public interface IProcessorContract : IContract
    //{
    //    #region Methods 
    //    void Initialize(IHostObjectContract hostObj);

    //    void InitializeCefWithCachePath(string path);

    //    void Shutdown();

    //    void SetPersonData(string serializedPdataXml);

    //    void NavigateToUrl(string url);

    //    void SetAccessToken(string url);

    //    void SetCrawlerState(int state);
    //    #endregion
    //}

    ///// <summary>
    ///// The actual Host contract that is implemented by the
    ///// <see cref="HostInSideAdapter.HostObjectViewToContractHostAdapter">Host Adapter</see>
    ///// Which enabled the AddIn to talk back to the host
    ///// </summary>
    //public interface IHostObjectContract : IContract
    //{
    //    #region Methods
    //    void ReportInitialized();
    //    void ReportSerializedResult(string serializedXML);
    //    void ReportSerializedLikesResult(string serializedXML);
    //    #endregion
    //}

    public interface IHostToPluginContract : IServiceProvider
    {
        #region host to plugin
        int HostProcessId { get; }   
        void LoadPlugin(PluginStartupInfo startupInfo);


        void SetCrawlerState(int state);
        void InitializeCefWithCachePath(string path);
        void SetPersonData(string serializedPdata);
        void NavigateToUrl(string url);
        #endregion

        #region plugin to host
        void ReportFatalError(string userMessage, string fullExceptionText);
        void ReportInitialized();
        void SetAccessToken(string fbtokenLink);
        void ReportSerializedResult(string serializedFBresult);         

        void Shutdown();

        #endregion   
    }

    public class HostToPluginContract : MarshalByRefObject, IHostToPluginContract
    {
        public event ReportSerializedResultDel OnSerializedResultsArived;

        public IHost Host { get; private set; }
        public IRemotePlugin RemotePlugin { get; private set; }

        public Process MProcess { get; private set; }

        private EventWaitHandle mReadyEvent;

        private string mName;
        private bool isDisposing = false;

        public HostToPluginContract(IHost host)
        {
            Host = host;
        }

        #region host to plugin
        public int HostProcessId { get { return Process.GetCurrentProcess().Id; } }

        public void LoadPlugin(PluginStartupInfo startupInfo)
        { 
            //RemotingServices.Marshal(this, "BrowseoNinjaCrawlerHost", typeof(IHostToPluginContract));

            mName = startupInfo.Name + "." + Guid.NewGuid();
            mReadyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, mName + ".Ready");

            var directory = Path.GetDirectoryName(GetType().Assembly.Location);
            var exeFile = "CrawlerProcess.exe";
            var processName = Path.Combine(directory, exeFile);

            if (!File.Exists(processName)) throw new InvalidOperationException("Could not find file '" + processName + "'");

            const string quote = "\"";
            const string doubleQuote = "\"\"";

            var quotedAssemblyPath = quote + startupInfo.FullAssemblyPath.Replace(quote, doubleQuote) + quote;

            var info = new ProcessStartInfo
            {
                Arguments = mName + " " + quotedAssemblyPath,
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = processName
            };

            MProcess = Process.Start(info);

            new ProcessMonitor(OnProcessExit).Start(MProcess);

            if (MProcess.HasExited)
            {
                throw new InvalidOperationException("Crawler process has terminated unexpectedly");
            }

            if (!mReadyEvent.WaitOne(3000))
            {
                throw new InvalidOperationException("Crawler process did not respond within timeout period");
            }

            string hostChannelName = startupInfo.Name + "." + Guid.NewGuid();
            //string hostChannelName = startupInfo.Name + "." + Process.GetCurrentProcess().Id;
            IpcServices.RegisterChannel(hostChannelName);

            RemotePlugin = (IRemotePlugin)Activator.GetObject(typeof(IRemotePlugin), "ipc://" + mName + "/BrowseoNinjaCrawlerLoader");
            RemotePlugin.LoadPlugin(startupInfo, hostChannelName,this);
            //RemotePlugin.OnReportTheSerializedResult += new ReportSerializedResultDel(ReportSerializedResult);
            //RemotePlugin.ReportSerializedResult += ReportSerializedResult;
        }

        public void SetCrawlerState(int state)
        {
            RemotePlugin.SetCrawlerState(state);
        }

        public void InitializeCefWithCachePath(string path)
        {
            RemotePlugin.InitializeCefWithCachePath(path);
        } 

        public void SetPersonData(string serializedPdata)
        {
            RemotePlugin.SetPersonData(serializedPdata);
        }

        public void NavigateToUrl(string url)
        {
            RemotePlugin.NavigateToUrl(url);
        } 

        public void SetAccessToken(string fbtokenLink)
        {
            RemotePlugin.SetAccessToken(fbtokenLink);
        }
        public void Shutdown()
        {
            isDisposing = true;
            if (RemotePlugin != null)
            {
                try
                {
                    RemotePlugin.Shutdown();
                    RemotePlugin = null;
                }
                catch
                {
                }
            }
           

            if (MProcess != null)
            {
                try
                {
                    MProcess.Kill();
                }
                catch
                {
                    MProcess = null;
                }
            }

            try
            {
                IpcServices.UnRegisterChannel();
            }
            catch
            {
            }
        }
        #endregion

        #region plugin to host
        private void OnProcessExit()
        {
            if(!isDisposing)
                 ReportFatalError("Crawler process shutdown.", "Crawler process quit unexpectidly.");
        }
        public void ReportFatalError(string userMessage, string fullExceptionText)
        {
            Host.ReportFatalError(userMessage, fullExceptionText);
            Shutdown();
        }

        public void ReportInitialized()
        {
            Host.ReportInitialized();
        }

        public void ReportSerializedResult(string serializedFBresult)
        {
            //System.Threading.Tasks.Task.Factory.StartNew(()=> {
            //NavigateToUrl("https://www.facebook.com/VapingCheap/");
            Host.ReportSerializedResult(serializedFBresult);
            //if (OnSerializedResultsArived != null)
            //  OnSerializedResultsArived(serializedFBresult);
            // });
            //Host.ReportInitialized();
        }
        #endregion

        public override object InitializeLifetimeService()
        {
            return null; //live forever
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableFrom(GetType())) return this;
            return null;
        } 
    }
}
