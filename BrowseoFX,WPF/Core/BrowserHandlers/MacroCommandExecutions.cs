using BrowseoFX_WPF.Core.Services;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.BrowserHandlers
{
    public class MacroCommandExecutions : nsICommandHandler
    {
        //public System.Windows.Forms.Form DummyForm;
        //System.Windows.Forms.ApplicationContext context;

        public string Exec(string aCommand, string aParameters)
        {
            // MacroFromJsRunner runner = new MacroFromJsRunner() { ParamsCode = aParameters, CommandCode = aCommand, ParentVM = ParentVM };

            //////switch (aCommand)
            //////{
            //////    case "iimPlayCode":
            //////        FXServices.BFXMacroPlayerService.OnIIMPlayCode(aParameters);
            //////        break;

            //////    case "iimPlay":
            //////        FXServices.BFXMacroPlayerService.OnIIMPlay(aParameters);
            //////        break;

            //////    case "iimSet":
            //////        FXServices.BFXMacroPlayerService.OnIIMSet(aParameters);
            //////        break;

            //////    case "iimDisplay":
            //////        //ParentVM.WebBrowser_OnBrowserMessageChanged(aParameters);
            //////        break;

            //////    case "afterSandboxEval":
            //////        FXServices.BFXMacroPlayerService.OnAfterSandboxEval(aParameters);
            //////        break;

            //////    default:
            //////        break;
            //////}
            //app.Run();

            //Application.Current.Run(new Window());
            //try
            //{
            //    //DummyForm = new System.Windows.Forms.Form();
            //    //System.Windows.Forms.Application.Run(DummyForm);

            //}
            //catch
            //{
            //    //while (ParentVM.isrunningiimInJsMode)
            //    //{
            //    //    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
            //    //}
            //}

            //  System.Windows.Threading.Dispatcher.Run();

            // return GetExecAsync(aCommand,aParameters).Result;

            //using (var ct = new AutoJSContext(ParentVM.WebBrowser.Browser.Window))
            //{



            //var instance = Xpcom.GetService<nsIThreadManager>("@mozilla.org/thread-manager;1");
            //var cur = instance.GetCurrentThreadAttribute();
            //while (ParentVM.isrunningiimInJsMode)
            //{
            //    bool processed = cur.ProcessNextEvent(true);
            //}




            //}

            //var instance = Xpcom.GetService<nsIThreadManager>("@mozilla.org/thread-manager;1").AsComPtr();
            //var cur = instance.Instance.NewThread(0, 0).AsComPtr();
            //runner.workThread = cur;
            //cur.Instance.Dispatch(runner, 1);

            //MacroFromJsSleeper sleeper = new MacroFromJsSleeper() { ParamsCode = aParameters, CommandCode = aCommand, ParentVM = ParentVM };
            //var cur2 = instance.Instance.NewThread(0, 0).AsComPtr();
            //sleeper.workThread = cur2;
            //while (ParentVM.isrunningiimInJsMode)
            //{
            //    cur2.Instance.Dispatch(sleeper, 1);
            //}


            //while (ParentVM.isrunningiimInJsMode)
            //{
            //    runner.workThread.Instance.ProcessNextEvent(true);
            //}

            //return GetExecAsync(aCommand,aParameters).Result;
            //while (ParentVM.isrunningiimInJsMode)
            //{
            //    if (!ParentVM.isrunningiimInJsMode)
            //    {
            //        break;
            //    }
            //    //Thread.Sleep(0);
            //    //System.Windows.Forms.Application.DoEvents();
            //    // ParentVM.WebBrowserHost.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
            //    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
            //    //Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,new ThreadStart(delegate { }));
            //    // Thread.Yield();
            //}

            //CallAwait();

            return null;
        }


        public class MacroFromJsRunner : nsIRunnable
        {
            public string ParamsCode { get; set; }
            public string CommandCode { get; set; }
            

            object mlock = new object();

            public void Run()
            {
                lock (mlock)
                {
                    //switch (CommandCode)
                    //{
                    //    case "iimPlayCode":
                    //         ParentVM.OnIIMPlayCode(ParamsCode);
                    //        break;

                    //    case "iimPlay":
                    //         ParentVM.OnIIMPlay(ParamsCode);
                    //        break;

                    //    case "iimSet":
                    //        ParentVM.OnIIMSet(ParamsCode);
                    //        break;

                    //    case "iimDisplay":
                    //        ParentVM.WebBrowser_OnBrowserMessageChanged(ParamsCode);
                    //        break;

                    //    case "afterSandboxEval":
                    //        ParentVM.OnAfterSandboxEval(ParamsCode);
                    //        break;

                    //    default:
                    //        break;
                    //}
                    string val = GetExecAsync(CommandCode, ParamsCode).Result;

                    //while (ParentVM.isrunningiimInJsMode) { Thread.Sleep(250); }

                    //workThread.Instance.Shutdown();
                }
            }

            private async Task<string> GetExecAsync(string aCommand, string aParameters)
            {
                //switch (aCommand)
                //{
                //    case "iimPlayCode":
                //        await FXServices.BFXMacroPlayerService.OnIIMPlayCode(aParameters);
                //        break;

                //    case "iimPlay":
                //        await FXServices.BFXMacroPlayerService.OnIIMPlay(aParameters);
                //        break;

                //    case "iimSet":
                //        FXServices.BFXMacroPlayerService.OnIIMSet(aParameters);
                //        break;

                //    case "iimDisplay":
                //       // FXServices.BFXMacroPlayerService.WebBrowser_OnBrowserMessageChanged(aParameters);
                //        break;

                //    case "afterSandboxEval":
                //        FXServices.BFXMacroPlayerService.OnAfterSandboxEval(aParameters);
                //        break;

                //    default:
                //        break;
                //}
                return null;
            }
        }

        //private async Task<string> GetExecAsync(string aCommand, string aParameters)
        //{
        //    using (var ct = new AutoJSContext(ParentVM.WebBrowser.Browser.Window))
        //    {
        //        switch (aCommand)
        //        {
        //            case "iimPlayCode":
        //                await ParentVM.OnIIMPlayCode(aParameters);
        //                break;

        //            case "iimPlay":
        //                await ParentVM.OnIIMPlay(aParameters);
        //                break;

        //            case "iimSet":
        //                ParentVM.OnIIMSet(aParameters);
        //                break;

        //            case "iimDisplay":
        //                ParentVM.WebBrowser_OnBrowserMessageChanged(aParameters);
        //                break;

        //            case "afterSandboxEval":
        //                ParentVM.OnAfterSandboxEval(aParameters);
        //                break;

        //            default:
        //                break;
        //        }
        //    }
        //    return null;
        //}

        public string Query(string aCommand, string aParameters)
        {
            return null;
        }
    }
}
