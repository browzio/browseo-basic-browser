using Contract;
using System;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CrawlerContracts
{
    public delegate void ReportSerializedResultDel(string Message);

    public interface IRemotePlugin : IServiceProvider
    {
        #region host to crawler
        void LoadPlugin(PluginStartupInfo startupInfo, string hostname, IHostToPluginContract hostChannel);

        void SetCrawlerState(int state);
        void InitializeCefWithCachePath(string path);
        void SetPersonData(string serializedPdata);
        void NavigateToUrl(string url);
        void SetAccessToken(string fbtokenLink);
        void Shutdown();
        #endregion 

        event ReportSerializedResultDel OnReportTheSerializedResult;   
    }

    public class RemotePlugin : MarshalByRefObject, IRemotePlugin
    {
        private Dispatcher mDispatcher;

        public event ReportSerializedResultDel OnReportTheSerializedResult;

        //event Action<string> ReportSerializedResult = delegate { };
        //event Action<string> IRemotePlugin.ReportSerializedResult
        //{
        //    add
        //    {
        //        lock (mLock)
        //        {
        //            ReportSerializedResult += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (mLock)
        //        {
        //            ReportSerializedResult -= value;
        //        }
        //    }
        //}

        public IHostToPluginContract HostPluginContract { get; private set; }
        public IPlugin Plugin { get; private set; }

        private object mLock = new object();

        public void Run(string name)
        {
           mDispatcher = Dispatcher.CurrentDispatcher;

            try
            {
                Console.WriteLine("Run " + name);
                new AssemblyResolver().Setup();
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                IpcServices.RegisterChannel(name);

                //Register object
                RemotingServices.Marshal(this, "BrowseoNinjaCrawlerLoader", typeof(IRemotePlugin));

                EventWaitHandle.OpenExisting(name + ".Ready").Set();
                Dispatcher.Run();
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Thread.Sleep(100); // allow any pending remoting operations to finish 
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {  
            Console.WriteLine("[PluginLoader] OnUnhandledException");

            HostPluginContract.ReportFatalError(Convert.ToString(sender), (e.ExceptionObject as Exception).Message);
            Shutdown();
        }        

        #region host to plugin
        public void LoadPlugin(PluginStartupInfo startupInfo, string hostname, IHostToPluginContract hostChanne)
        {
            Console.WriteLine("[PluginLoader] LoadPlugin");
            Console.WriteLine("LoadPlugin " + hostChanne);
            HostPluginContract = hostChanne;   
            Console.WriteLine("LoadPlugin " + HostPluginContract);

            new ProcessMonitor(Shutdown).Start(HostPluginContract.HostProcessId);

            Plugin = PluginCreator.CreatePlugin(startupInfo.AssemblyName, startupInfo.MainClass, HostPluginContract) as IPlugin;
            Plugin.OnReportInitialized += Plugin_OnReportInitialized;
            Plugin.OnReportSerializedResult += Plugin_OnReportSerializedResult;
        }


        [STAThread]
        public void InitializeCefWithCachePath(string path)
        {
            Plugin.InitializeCefWithCachePath(path);
        }

        public void SetCrawlerState(int state)
        {
            Console.WriteLine("[PluginLoader] state" + state);
            Plugin.SetCrawlerState(state);
        }

        public void SetPersonData(string serializedPdata)
        {
            Plugin.SetPersonData(serializedPdata);
        }

        public void NavigateToUrl(string url)
        {
            Plugin.NavigateToUrl(url);
        }

        public void SetAccessToken(string fbtokenLink)
        {
            Plugin.SetAccessToken(fbtokenLink);
        }

        public void Shutdown()
        {
            if (Plugin != null)
                Plugin.Shutdown();

            if (mDispatcher != null)
                mDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);

            IpcServices.UnRegisterChannel();

            if (Plugin == null && mDispatcher == null)
                Environment.Exit(1);
        }
        #endregion

        #region crawler to host
        private void Plugin_OnReportInitialized()
        {
            Console.WriteLine("[PluginLoader] ReportInitialized ");
            Console.WriteLine("[PluginLoader] ReportInitialized "+ HostPluginContract);
            //OnReportTheSerializedResult.Invoke("");
            HostPluginContract.ReportInitialized();
        }

        private void Plugin_OnReportSerializedResult(string serializedFBresult)
        {
            //mDispatcher.BeginInvoke((Action)delegate
            //{
            //Task.Factory.StartNew(()=> {
            //   Delegate del = (Delegate)ReportSerializedResult(serializedFBresult);
            //});
            // Host.navigateToNextUrl();
            //}, DispatcherPriority.Normal);

            // OnReportTheSerializedResult.Invoke(serializedFBresult);
            //OnReportTheSerializedResult.BeginInvoke(serializedFBresult, new AsyncCallback((ar)=> { Console.WriteLine("invoked " + ar); }),new object());

            //NavigateToUrl("https://www.facebook.com/VapingCheap/");

            HostPluginContract.ReportSerializedResult(serializedFBresult);
        }
        #endregion

        public object GetService(Type serviceType)
        {
            return Plugin.GetService(serviceType);  
        }

        public override object InitializeLifetimeService()
        {
            return null; // live forever
        }
    }
}

//public override object InitializeLifetimeService()
//{
//    return null; // live forever
//}    

//public object GetService(Type serviceType)
//{
//    return _plugin.GetService(serviceType);
//}

//public void Dispose()
//{
//    if (_dispatcher != null)
//    {
//        //_plugin.Dispose();
//        _dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
//    }
//    else
//    {
//        Environment.Exit(1);
//    }
//}
//public interface IPlugin : IServiceProvider, IDisposable 
//{
//    void SetCrawlerState(int state);
//    void InitializeCefWithCachePath(string path);
//}

//public abstract class PluginBase : MarshalByRefObject, IPlugin
//{   
//    public virtual object GetService(Type serviceType)
//    {
//        if (serviceType.IsAssignableFrom(GetType())) return this;
//        return null;
//    }

//    public override object InitializeLifetimeService()
//    {
//        return null; // live forever
//    }

//    public abstract void Dispose();


//    public abstract void SetCrawlerState(int state);
//    public abstract void InitializeCefWithCachePath(string path);

//    public abstract void SetRemotePluginHost(IRemotePlugin remotePlugin);
//}