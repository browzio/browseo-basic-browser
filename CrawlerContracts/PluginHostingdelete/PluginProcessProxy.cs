using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CrawlerContracts.PluginHosting
{
    [Serializable]
    public class PluginProcessProxy : MarshalByRefObject//, IDisposable
    {
        //private readonly PluginStartupInfo _startupInfo;

        //public  IHost Host { get; private set; }
        //public IRemotePlugin RemotePlugin { get; private set; }

       

        ////private EventWaitHandle _readyEvent;

        ////private Process _process;
        ////public Process Process { get { return _process; } } 

        //public PluginProcessProxy(PluginStartupInfo startupInfo, IHost host)
        //{
        //    _startupInfo = startupInfo;
        //    Host = host;
        //}

        //public void StartPluginProcess()
        //{
        //    //_name = _startupInfo.Name+ "." + Guid.NewGuid();
        //    //_readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, _name+ ".Ready");

        //    //var directory = Path.GetDirectoryName(GetType().Assembly.Location);
        //    //var exeFile = "CrawlerProcess.exe";
        //    //var processName = Path.Combine(directory, exeFile);

        //    //if (!File.Exists(processName)) throw new InvalidOperationException("Could not find file '" + processName + "'");

        //    //const string quote = "\"";
        //    //const string doubleQuote = "\"\"";

        //    //var quotedAssemblyPath = quote + _startupInfo.FullAssemblyPath.Replace(quote, doubleQuote) + quote;          

        //    //var info = new ProcessStartInfo
        //    //{
        //    //    Arguments = _name + " " + quotedAssemblyPath,
        //    //    CreateNoWindow = false,
        //    //    UseShellExecute = false,
        //    //    FileName = processName
        //    //}; 

        //    //_process = Process.Start(info);

        //    //new ProcessMonitor(OnProcessExited).Start(Process);
        //}

        ////public void LoadPlugin()
        ////{
        ////    RemotePlugin = GetRemotePlugin();
        ////    RemotePlugin.LoadPlugin(_startupInfo, hostChannelName);
        ////}

        ////private IRemotePlugin GetRemotePlugin()
        ////{
        ////    if (Process.HasExited)
        ////    {
        ////        throw new InvalidOperationException("Crawler process has terminated unexpectedly");
        ////    }

        ////    if (!_readyEvent.WaitOne(3000))
        ////    {
        ////        throw new InvalidOperationException("Crawler process did not respond within timeout period");
        ////    }

        ////    hostChannelName = _startupInfo.Name+ "." + Process.GetCurrentProcess().Id;
        ////    IpcServices.RegisterChannel(hostChannelName);

        ////    var url = "ipc://" + _name + "/BrowseoNinjaCrawlerLoader";
        ////    var pluginLoader = (IRemotePlugin)Activator.GetObject(typeof(IRemotePlugin), url);
        ////    return pluginLoader;
        ////}

        //private void OnProcessExited()
        //{
        //    Host.ReportFatalError("OnProcessExited", "PluginProcessProxy crawler process quit unexpectedly");
        //}

        //public override object InitializeLifetimeService()
        //{
        //    return null; // live forever
        //}

        //public void Dispose()
        //{
        //    if (RemotePlugin != null)
        //    {
        //        try
        //        {
        //            RemotePlugin.Dispose();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error disposing remote plugin for " + _startupInfo.Name, ex);
        //        }
        //    }

        //    if (Process != null)
        //    {
        //        try
        //        { 
        //            Process.Kill();
        //        }
        //        catch { }
        //    }
        //}


        //#region host to crawler
        //public void SetCrawlerState(int state)
        //{
        //    RemotePlugin.SetCrawlerState(state);
        //}

        //public void InitializeCefWithCachePath(string path)
        //{
        //    RemotePlugin.InitializeCefWithCachePath(path);
        //}
        //#endregion

        //#region crawler to host

        //#endregion
    }
}