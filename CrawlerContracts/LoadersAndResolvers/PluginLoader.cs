using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CrawlerContracts
{
    //public interface IPluginLoader : IDisposable
    //{
    //    IRemotePlugin LoadPlugin(ISendToHost _host, PluginStartupInfo _startupInfo);
    //}

    //public class PluginLoader : MarshalByRefObject, IPluginLoader
    //{
    //    private Dispatcher _dispatcher;

    //    private ISendToHost _host;
    //    private IRemotePlugin _plugin;

    //    public void Run(string name)
    //    {
    //        _dispatcher = Dispatcher.CurrentDispatcher;

    //        try
    //        {  
    //            new AssemblyResolver().Setup();  
    //            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
    //            IpcServices.RegisterChannel(name);

    //            //Register object
    //            RemotingServices.Marshal(this, "CrawlerLoader", typeof(IPluginLoader));

    //            EventWaitHandle.OpenExisting(name + ".Ready").Set();
    //            Dispatcher.Run();
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }

    //        Thread.Sleep(100); // allow any pending remoting operations to finish 
    //    }

    //    public IRemotePlugin LoadPlugin(ISendToHost host, PluginStartupInfo startupInfo)
    //    {
    //        _host = host;

    //        new ProcessMonitor(Dispose).Start(_host.HostProcessId); 

    //        var localPlugin = PluginCreator.CreatePlugin(startupInfo.AssemblyName, startupInfo.MainClass, _host);

    //        _plugin = new RemotePlugin(localPlugin as IPlugin);

    //        return _plugin;
    //    }




    //    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    //    {
    //        _host.ReportFatalError("[PluginLoader] OnUnhandledException", e.ExceptionObject.ToString());
    //        Console.WriteLine("[PluginLoader] OnUnhandledException"+e.ExceptionObject);
    //    }

    //    public override object InitializeLifetimeService()
    //    {
    //        return null; // live forever
    //    }

    //    public void Dispose()
    //    {
    //        if (_dispatcher != null)
    //        {
    //            _dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
    //        }
    //        else
    //        {
    //            Environment.Exit(1);
    //        }
    //    }
    //}
}
