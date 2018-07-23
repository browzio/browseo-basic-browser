using BrowseoFX_WPF.Core.BrowserHandlers;
using BrowseoFX_WPF.Core.Services.Browseo;
using BrowseoFX_WPF.Core.Services.Interfaces;
using Gecko;
using Gecko.DOM;
using Gecko.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Organiser.Common.Classes.MyFilesDatabase;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class BFXMacroPlayerService
    {
    } 
        //public class BFXMacroPlayerService
        //{
        //    public bool AnyPlaingJS { get; set; }

        //    private bool isrunningInJsMode;
        //    public bool runningInJsMode
        //    {
        //        get { return isrunningInJsMode; }
        //        set
        //        {
        //            isrunningInJsMode = value;
        //        }
        //    }

        //    public bool InStopRequest
        //    {
        //        get
        //        {
        //            if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning)
        //            {
        //                if (runningInJsMode && JSMacroPlayer != null) JSMacroPlayer.macroDone(DomWindow, NativeDomDocument);
        //                if (!runningInJsMode || macroPlayer.StopRequested || !macroPlayer.IsRunning)
        //                {
        //                    macroPlayer.IsRunning = false;
        //                    if (macroPlayer.StopRequested) runningInJsMode = false;
        //                }
        //                return true;
        //            }
        //            return false;
        //        }
        //    }

        //    private nsIDOMWindow DomWindow
        //    {
        //        get
        //        {
        //            return BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DefaultView.Window.Instance;
        //        }
        //    }
        //    private nsIDOMDocument NativeDomDocument
        //    {
        //        get
        //        {
        //            return BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.Instance;
        //        }
        //    }
        //    private GeckoDocument GecoDocument
        //    {
        //        get
        //        {
        //            return BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument;
        //        }
        //    }

        //    public MacroVariables macVals = new MacroVariables();

        //    public MacroManger macroPlayer;

        //    private nsIMacroPlayer JSMacroPlayer;
        //    private MacroCommandExecutions JSMacroCommandCallbacks;
        //    private MacroPromptService macropromt;

        //    private bool setfromwindow = false, startedTimer = false;

        //    public async Task OnPlayMacro(MacroManger manger, IIMPlayType type, int loop)
        //    {
        //        if (type == IIMPlayType.js && AnyPlaingJS)
        //        {
        //            "Only 1 js script can run at a time, play a .iim in the meantime or run multiple js in the macros module.".Show();
        //                manger.StopRequested = true;
        //                manger.IsRunning = false;
        //                return;
        //        }
        //        setfromwindow = false;
        //        await RunMacro(manger, type, loop);
        //        try
        //        {
        //            manger.semaphoreSlim.Release();
        //        }
        //        catch { }
        //    }

        //    public void OnIIMSet(string values)
        //    {
        //        if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

        //        string[] vals = values.Split(new string[] { "{[|!1001!|]}" }, StringSplitOptions.None);
        //        if (macVals[vals[0]] == null) macVals.MacroVariablesValues.Add(vals[0].ToUpper(), "");
        //        macVals[vals[0]] = vals[1];
        //    }
        //    public void OnAfterSandboxEval(string aParameters)
        //    {
        //        if (macroPlayer == null) return;
        //        macroPlayer.IsRunning = false;
        //        runningInJsMode = false;
        //    }

        //    public async Task OnIIMPlay(string code)
        //    {
        //        await Application.Current.Dispatcher.Invoke(async () =>
        //        {
        //            if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

        //            string codeOrPath = code;
        //            if (codeOrPath.StartsWith("CODE:"))
        //            {
        //                codeOrPath = codeOrPath.Substring(codeOrPath.IndexOf("CODE:") + 5);
        //                codeOrPath = codeOrPath.Trim();
        //            }
        //            else if (codeOrPath.StartsWith("Code:"))
        //            {
        //                codeOrPath = codeOrPath.Substring(codeOrPath.IndexOf("Code:") + 5);
        //                codeOrPath = codeOrPath.Trim();
        //            }
        //            else
        //            {
        //                if (!codeOrPath.Contains(".iim")) codeOrPath = codeOrPath + ".iim";
        //                var path = codeOrPath;
        //                if (!File.Exists(path)) path = MacroSettings.DefaulFoldertMacros + "\\" + codeOrPath;
        //                if (!File.Exists(path)) path = macroPlayer.FileDirectory + "\\" + codeOrPath;
        //                if (!File.Exists(path)) return;

        //                codeOrPath = File.ReadAllText(path);
        //            }

        //            macroPlayer.MacroPlayer.InitMacroCommandsList(codeOrPath);
        //            await RunMacro(macroPlayer, IIMPlayType.macroFromjs, 1, macVals);

        //            if (macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

        //            bool timeout = await CheckBrowserBusyTillTimeOut(macVals);
        //            if (!timeout)
        //            {
        //                GecoDocument.DefaultView.Stop();
        //            }
        //            JSMacroPlayer.macroDone(DomWindow, NativeDomDocument);
        //        });
        //    }

        //    public async Task OnIIMPlayCode(string code)
        //    {
        //        await Application.Current.Dispatcher.Invoke(async () =>
        //        {
        //            if (macroPlayer == null || macroPlayer.StopRequested || !macroPlayer.IsRunning) return;
        //            macroPlayer.MacroPlayer.InitMacroCommandsList(code);
        //            await RunMacro(macroPlayer, IIMPlayType.macroFromjs, 1, macVals);

        //            if (macroPlayer.StopRequested || !macroPlayer.IsRunning) return;

        //            bool timeout = await CheckBrowserBusyTillTimeOut(macVals);
        //            if (!timeout)
        //            {
        //                GecoDocument.DefaultView.Stop();
        //            }
        //            JSMacroPlayer.macroDone(DomWindow, NativeDomDocument);
        //        });
        //    }


        //    private async void StartTimer(MacroVariables macVals, MacroManger player)
        //    {
        //        if (startedTimer) return;

        //        startedTimer = true;
        //        double runtime = 0.0;
        //        player.UpdateText = "0";
        //        while (player.IsRunning && !player.StopRequested)
        //        {
        //            if (await QuitableDelay(5)) break;

        //            //if (player.Paused) continue;


        //            runtime += .5;
        //            player.UpdateText = runtime.ToString();
        //            macVals[MacroVariables.STOPWATCHTIME] = runtime.ToString();
        //        }
        //        startedTimer = false;
        //    }

        //    /// <summary>
        //    /// delay in a loop by 100ms per run
        //    /// </summary>
        //    /// <param name="times">amount of times to wait 100ms</param>
        //    /// <returns></returns>
        //    private async Task<bool> QuitableDelay(int times)
        //    {
        //        for (int i = 0; i < times; i++)
        //        {
        //            await Task.Delay(100);
        //            if (InStopRequest) return true;
        //        }

        //        return InStopRequest;
        //    }

        //    private async Task<bool> CheckBrowserBusyTillTimeOut(MacroVariables macVals)
        //    {
        //        double sleepduringbusy = 0.0;
        //        while (GecoDocument.DefaultView.Status == "")
        //        {
        //            if (macroPlayer.StopRequested || !macroPlayer.IsRunning) break;

        //            // await Task.Run(() => Thread.Sleep(350));
        //            if (await QuitableDelay(3)) return false;
        //            sleepduringbusy += .25;
        //            if (macVals[MacroVariables.TIMEOUT_PAGE] != "")
        //            {
        //                int timeouttime = 0;
        //                int.TryParse(macVals[MacroVariables.TIMEOUT_PAGE], out timeouttime);
        //                if (sleepduringbusy > timeouttime)
        //                {
        //                    return false;
        //                }
        //            }
        //        }

        //        return true;
        //    }

        //    internal async Task RunMacro(MacroManger mPlayer, IIMPlayType isiim, int times, MacroVariables variablescontinuefromjs = null)
        //    {
        //        try
        //        {
        //            if (JSMacroPlayer == null) JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;2");
        //            if (JSMacroCommandCallbacks == null)
        //            {
        //                JSMacroCommandCallbacks = new MacroCommandExecutions();
        //                JSMacroPlayer.setHandler(JSMacroCommandCallbacks);
        //            }
        //            macroPlayer = mPlayer;

        //            StartTimer(macVals, mPlayer);
        //            if (macropromt == null)
        //            {
        //                macropromt = new MacroPromptService();
        //                PromptFactory.PromptServiceCreator = () => macropromt;
        //            }
        //            int totalDataSourceLines = 1;
        //            if (!setfromwindow && (isiim != IIMPlayType.macroFromjs || macVals == null))
        //            {
        //                macVals = new MacroVariables();
        //                macVals.OnSetExtract += MacVals_OnSetExtract;
        //            }

        //            if (!setfromwindow)
        //            {
        //                CurrentlyActiveBrowser.Focus();
        //            }
        //            else
        //            {
        //                //  ffpopupMacros.Focus();
        //                ffpopupMacrosBrowser.Focus();
        //            }

        //            if (isiim == IIMPlayType.macro) mPlayer.MacroPlayer.InitMacroCommandsList(mPlayer.FileText);
        //            else if (isiim == IIMPlayType.js)
        //            {
        //                runningInJsMode = true;
        //                mPlayer.OnStopRequested -= MPlayer_OnStopRequested;
        //                mPlayer.OnStopRequested += MPlayer_OnStopRequested;
        //                try
        //                {
        //                    JSMacroPlayer.playMacro(DomWindow, NativeDomDocument, mPlayer.FileText);
        //                    OnAfterSandboxEval("");
        //                }
        //                catch (System.Runtime.InteropServices.InvalidComObjectException)
        //                {
        //                    JSMacroPlayer = Xpcom.CreateInstance<nsIMacroPlayer>("@eli.browz.io/jsmacroaddon;2");
        //                    JSMacroPlayer.playMacro(DomWindow, NativeDomDocument, mPlayer.FileText);
        //                    OnAfterSandboxEval("");
        //                }
        //                catch (AccessViolationException)
        //                {
        //                    try
        //                    {
        //                        if (ffpopupMacros != null) ffpopupMacros.Close();
        //                    }
        //                    catch { }
        //                    macroPlayer.StopRequested = true;
        //                    try
        //                    {
        //                        if (tabsWindows.Count > 0)
        //                        {
        //                            foreach (var tw in tabsWindows)
        //                            {
        //                                tw.OnClosedWindow -= Tcn_OnClosedWindow;
        //                                tw.CloseAndDispose();
        //                            }
        //                            tabsWindows.Clear();

        //                            setFromTabsWindows = false;
        //                            windowIndexToSet = -1;
        //                        }
        //                    }
        //                    catch { }

        //                    OnCloseTab(false, this);

        //                    "Stopped macro and closed tab because of an unexpected page process you can reopen a new one and try the macro again".Show();
        //                    return;
        //                }
        //                return;
        //            }

        //            if (isiim == IIMPlayType.macroFromjs) isrunningiimInJsMode = true;

        //            //if (!isSettingFromTab)
        //            //{
        //            //    dataCourceLine = 0;
        //            //}
        //            for (int dataCourceLine = 0; dataCourceLine < totalDataSourceLines; dataCourceLine++)
        //            {
        //                if (isiim == IIMPlayType.macro) macVals[MacroVariables.EXTRACT] = "NULL";
        //                await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
        //                if (InStopRequest) return;

        //                //if (!isSettingFromTab)
        //                //{
        //                //    iTimesToRun = 0;
        //                //}
        //                for (int iTimesToRun = 0; iTimesToRun < times; iTimesToRun++)
        //                {
        //                    if (runningInJsMode)
        //                    {
        //                        if (JSMacroPlayer != null) JSMacroPlayer.setVariableMessage("iimReturnVal", "1");
        //                    }
        //                    await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
        //                    if (InStopRequest) return;

        //                    macVals[MacroVariables.LOOP] = !runningInJsMode ? (iTimesToRun + 1).ToString().ToString() : (mPlayer.JSLoopPos + 1).ToString();

        //                    mPlayer.CurrentLoopPos = macVals[MacroVariables.LOOP];


        //                    GeckoDomDocument currentMacroContentDocument;
        //                    if (!setfromwindow) currentMacroContentDocument = CurrentlyActiveBrowser.Document;
        //                    else currentMacroContentDocument = ffpopupMacrosBrowser.Document;
        //                    GeckoIFrameElement currentContentDocumentIframe = null;
        //                    GeckoFrameElement currentContentDocumentFrame = null;
        //                    if (!setfromwindow) setfromframe = false;

        //                    GeckoHtmlElement previosTagElementFound = null;

        //                    #region play macro
        //                    int retrystep = 0;
        //                    //if (!isSettingFromTab)
        //                    //{
        //                    //     macroIndex = 0;
        //                    //}
        //                    for (int macroIndex = 0; macroIndex < mPlayer.MacroPlayer.Macros.Count; macroIndex++)
        //                    {
        //                        //bool foundlinecommand = false;
        //                        //if (isSettingFromTab)
        //                        //{
        //                        //    if(mPlayer.MacroPlayer.Macros.Count - 1 > macroIndex)
        //                        //    {
        //                        //        var macfinding = mPlayer.MacroPlayer.Macros[macroIndex-1];
        //                        //        if(macroWaitingFor == macfinding.Command + macfinding.Line + macfinding.Value)
        //                        //        {
        //                        //            foundlinecommand = true;
        //                        //            isSettingFromTab = false;
        //                        //        }
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        break;
        //                        //    }
        //                        //    if (!foundlinecommand) break;
        //                        //}
        //                        if (macVals[MacroVariables.SINGLESTEP].ToUpper() == "YES") mPlayer.OnCommandFromView_Raised("MacroPause");
        //                        await Task.Run(() => { while (mPlayer.Paused) { Thread.Sleep(500); } });
        //                        if (InStopRequest) return;

        //                        mPlayer.MacroPlayer.SIMacroCommand = macroIndex;
        //                        Macro mac = mPlayer.MacroPlayer.Macros[macroIndex];
        //                        mac.SetGreen();
        //                        try
        //                        {

        //                            #region Before Eache Command
        //                            if (CheckTimeOutMax(macVals)) { mPlayer.MacroPlayer.Macros.Clear(); break; }
        //                            switch (macVals[MacroVariables.REPLAYSPEED])
        //                            {
        //                                case "MEDIUM":
        //                                    if (await QuitableDelay(10)) return;
        //                                    break;// await Task.Run(() => Thread.Sleep(1000)); break;
        //                                case "SLOW":
        //                                    if (await QuitableDelay(20)) return;
        //                                    break;// await Task.Run(() => Thread.Sleep(2000)); break;
        //                                default: break;
        //                            }
        //                            //await Task.Run(() => Thread.Sleep(1000));
        //                            if (await CheckBrowserBusyTillTimeOut(macVals) == false) CurrentlyActiveBrowser.Stop();

        //                            if (InStopRequest) return;

        //                            while (CurrentlyActiveBrowser.Document == null && !CheckTimeOutMax(macVals) && !InStopRequest) { await Task.Delay(100); }// await Task.Run(() => Thread.Sleep(250)); }

        //                            if (InStopRequest) return;

        //                            if (CheckTimeOutMax(macVals)) { mPlayer.MacroPlayer.Macros.Clear(); break; }

        //                            if (setfromframe &&
        //                                !setfromwindow &&
        //                                currentMacroContentDocument != null &&
        //                                currentMacroContentDocument.Location != null &&
        //                                currentMacroContentDocument.Location.Href != null) macVals[MacroVariables.URLCURRENT] = currentMacroContentDocument.Location.Href;
        //                            else
        //                            {
        //                                if (CurrentlyActiveBrowser.Document.Location.Href == "" || CurrentlyActiveBrowser.Document.Location.Href == "about:blank")
        //                                {
        //                                    if (WebBrowser.Browser.Document.Location.Href != "") macVals[MacroVariables.URLCURRENT] = WebBrowser.Browser.Document.Location.Href;
        //                                }
        //                                else
        //                                {
        //                                    macVals[MacroVariables.URLCURRENT] = CurrentlyActiveBrowser.Document.Location.Href;
        //                                }
        //                            }

        //                            if ((string)GeckoPreferences.User["general.useragent.override"] != macVals[MacroVariables.USERAGENT])
        //                            {
        //                                GeckoPreferences.User["general.useragent.override"] = macVals[MacroVariables.USERAGENT];
        //                            }

        //                            macropromt.OnLoginDidRetryCount = 0;
        //                            #endregion

        //                            Console.WriteLine(mac.Command + " " + mac.Value);
        //                            switch (mac.Command)
        //                            {
        //                                #region ADD
        //                                case MacroCommands.ADD:
        //                                    string ADDVARIABLE = "", ADDVALUE = "";
        //                                    var addVals = GetRegexMacroCommands(mac.Value)?.ToList();
        //                                    for (int k = 0; k < addVals.Count; k++)
        //                                    {
        //                                        var macval = addVals[k];
        //                                        switch (k)
        //                                        {
        //                                            case 0:
        //                                                ADDVARIABLE = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;

        //                                            case 1:
        //                                                ADDVALUE = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;

        //                                            default: break;
        //                                        }
        //                                    }

        //                                    if (macVals[ADDVARIABLE] != null)
        //                                    {
        //                                        if (macVals[ADDVARIABLE].Trim().ToUpper() == "NULL") macVals[ADDVARIABLE] = "";
        //                                        int nowVal = 0, newval = 0;

        //                                        bool convertedMacVal = int.TryParse(macVals[ADDVARIABLE], out nowVal);
        //                                        bool convertedNewVal = int.TryParse(ADDVALUE, out newval);

        //                                        if (convertedMacVal && convertedNewVal) macVals[ADDVARIABLE] = (nowVal += newval).ToString();
        //                                        else
        //                                        {
        //                                            if (ADDVARIABLE == MacroVariables.EXTRACT) macVals[ADDVARIABLE] = ADDVALUE;
        //                                            else macVals[ADDVARIABLE] += ADDVALUE;
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region BACK
        //                                case MacroCommands.BACK:
        //                                    if (CurrentlyActiveBrowser.CanGoBack) CurrentlyActiveBrowser.GoBack();
        //                                    if (await QuitableDelay(5)) return;
        //                                    // await Task.Run(() => Thread.Sleep(500));
        //                                    break;
        //                                #endregion

        //                                #region CLEAR
        //                                case MacroCommands.CLEAR:
        //                                    CurrentlyActiveBrowser.Stop();
        //                                    await Task.Delay(500);
        //                                    //await Task.Run(() => Thread.Sleep(500));
        //                                    //Clears the browsers cache and all cookies. Can be useful, for example, 
        //                                    //to delete Web site cookies so every macro run starts at the same point.
        //                                    //It is also useful to use this command before doing website response measurements.
        //                                    nsIBrowserHistory historyMan = Xpcom.GetService<nsIBrowserHistory>(Gecko.Contracts.NavHistoryService);
        //                                    historyMan = Xpcom.QueryInterface<nsIBrowserHistory>(historyMan);
        //                                    historyMan.RemoveAllPages();
        //                                    ImageCache.ClearCache(true);
        //                                    ImageCache.ClearCache(false);
        //                                    nsICookieManager CookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
        //                                    var cookies = Xpcom.QueryInterface<nsICookieManager>(CookieMan);
        //                                    cookies.RemoveAll();
        //                                    await Task.Delay(500);
        //                                    //await Task.Run(() => Thread.Sleep(500));
        //                                    break;
        //                                #endregion

        //                                #region CLICK
        //                                case MacroCommands.CLICK:
        //                                    float xx = 0, yy = 0;
        //                                    string X, Y;
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "X":
        //                                                X = macVariableValue.Trim();
        //                                                float.TryParse(X, out xx);
        //                                                break;

        //                                            case "Y":
        //                                                Y = macVariableValue.Trim();
        //                                                float.TryParse(Y, out yy);
        //                                                break;

        //                                            default: break;
        //                                        }
        //                                    }

        //                                    CurrentlyActiveBrowser.Window.WindowUtils.SendMouseEvent("mousedown", xx, yy, GeckoMouseButton.Left, 1, 0, true, 0, 0);
        //                                    CurrentlyActiveBrowser.Window.WindowUtils.SendMouseEvent("mouseup", xx, yy, GeckoMouseButton.Left, 2, 0, true, 0, 0);
        //                                    break;
        //                                #endregion

        //                                #region EVENT
        //                                case MacroCommands.EVENT:
        //                                case MacroCommands.EVENTS:
        //                                    //EVENT TYPE=type [SELECTOR|XPATH]=localizer [BUTTON|POINT|CHAR|KEY]=[button|point|char|key] [MODIFIERS=modifiers]
        //                                    await EventsCommand(mac.Value, mPlayer.MacroPlayer, macVals, (setfromframe && currentMacroContentDocument != null) ? currentMacroContentDocument : CurrentlyActiveBrowser.Document, currentContentDocumentIframe, currentContentDocumentFrame, iTimesToRun);
        //                                    break;
        //                                #endregion

        //                                #region FILEDELETE
        //                                case MacroCommands.FILEDELETE:
        //                                    // FILEDELETE NAME=c:\output\mydata.csv
        //                                    string path = mac.Value.Substring(mac.Value.IndexOf("NAME=") + 5).Trim().Replace("\"", "").Replace("<SP>", " ");
        //                                    if (File.Exists(path)) File.Delete(path);
        //                                    break;
        //                                #endregion

        //                                #region FILTER
        //                                case MacroCommands.FILTER:
        //                                    string FILTERTYPE = "", FILTERVALUE = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "TYPE":
        //                                                FILTERTYPE = macVariableValue.ToUpper();
        //                                                break;

        //                                            case "STATUS":
        //                                                FILTERVALUE = macVariableValue.ToUpper();
        //                                                break;

        //                                            default: break;
        //                                        }
        //                                    }

        //                                    if (FILTERTYPE == "IMAGES")
        //                                    {
        //                                        if (FILTERVALUE == "ON")
        //                                        {
        //                                            GeckoPreferences.Default["permissions.default.image"] = (int)2;
        //                                        }
        //                                        else if (FILTERVALUE == "OFF")
        //                                        {
        //                                            GeckoPreferences.Default["permissions.default.image"] = (int)1;
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region FRAME
        //                                case MacroCommands.FRAME:
        //                                    currentMacroContentDocument = CurrentlyActiveBrowser.Document;
        //                                    setfromframe = false;
        //                                    currentContentDocumentIframe = null;
        //                                    currentContentDocumentFrame = null;
        //                                    if (mac.Value.ToLower() != "f=0")
        //                                    {
        //                                        GeckoDomDocument d = FrameCommandRecursive(mac.Value, macVals, currentMacroContentDocument, 0, ref currentContentDocumentIframe, ref currentContentDocumentFrame);
        //                                        if (d != null)
        //                                        {
        //                                            currentMacroContentDocument = d;
        //                                            setfromframe = true;
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region ONDIALOG
        //                                case MacroCommands.ONDIALOG:
        //                                    //string POS = "", DIALOG_BUTTON = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "POS":
        //                                                int atpos = 1;
        //                                                int.TryParse(macVariableValue, out atpos);
        //                                                macropromt.AtPos = atpos;
        //                                                macropromt.POS = 0;
        //                                                break;

        //                                            case "BUTTON":
        //                                                macropromt.ButtonState = macVariableValue;
        //                                                break;

        //                                            case "CONTENT":
        //                                                macropromt.CONTENT = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region ONDOWNLOAD
        //                                case MacroCommands.ONDOWNLOAD:
        //                                    //no support CHECKSUM
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "FOLDER":
        //                                                MacroOnDownload.FOLDER = macVariableValue;
        //                                                break;

        //                                            case "FILE":
        //                                                MacroOnDownload.FILE = macVariableValue;
        //                                                if (MacroOnDownload.FILE.Contains(":") || MacroOnDownload.FILE.Contains("/"))
        //                                                {
        //                                                    MacroOnDownload.FILE = MacroOnDownload.FILE.Replace("/", "_").Replace(":", "_");
        //                                                }
        //                                                break;

        //                                            case "WAIT":
        //                                                MacroOnDownload.WAIT = macVariableValue;
        //                                                break;

        //                                            case "SIZE":
        //                                                MacroOnDownload.SIZE = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }
        //                                    if (MacroOnDownload.FOLDER == "*") MacroOnDownload.FOLDER = MacroSettings.DefaultFolderDownloads;

        //                                    EventHandler<LauncherDialogEvent> onDownloads = async (sender, launcherdialoge) =>
        //                                    {
        //                                        string file = MacroOnDownload.FILE;
        //                                        if (file == "*") file = launcherdialoge.Filename;
        //                                        if (file.Contains("+") || !file.Contains("."))
        //                                        {
        //                                            file = file.Replace("+", "");
        //                                            file = file + launcherdialoge.Filename;
        //                                        }

        //                                        string url = launcherdialoge.Url;  //url to download
        //                                        string fullpath = System.IO.Path.Combine(MacroOnDownload.FOLDER, file); //destination file absolute path
        //                                        if (File.Exists(fullpath)) File.Delete(fullpath);
        //                                        await GracefullyTryDownload(fullpath, url, launcherdialoge.Mime, macVals);
        //                                    };
        //                                    if (handlers.Count > 0)
        //                                    {
        //                                        foreach (var handler in handlers)
        //                                        {
        //                                            LauncherDialog.Download -= handler;
        //                                        }

        //                                        handlers.Clear();
        //                                    }

        //                                    handlers.Add(onDownloads);
        //                                    LauncherDialog.Download += onDownloads;


        //                                    //if (WAIT.ToUpper() != "YES") continue;
        //                                    break;
        //                                #endregion

        //                                #region ONERRORDIALOG
        //                                case MacroCommands.ONERRORDIALOG:
        //                                    //ONERRORDIALOG BUTTON=(YES|NO) CONTINUE=(YES|NO)
        //                                    string ERRBUTTON = "", ERRCONTINUE = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "BUTTON":
        //                                                ERRBUTTON = macVariableValue;
        //                                                break;

        //                                            case "CONTINUE":
        //                                                ERRCONTINUE = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }

        //                                    macropromt.OnErrorButton = ERRBUTTON;
        //                                    macropromt.OnErrorContinue = ERRCONTINUE;
        //                                    break;
        //                                #endregion

        //                                #region ONLOGIN
        //                                case MacroCommands.ONLOGIN:
        //                                    //ONLOGIN USER=username PASSWORD=password RETRY=[YES|NO]
        //                                    string LOGINUSER = "", LOGINPASS = "", LOGINRETRY = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "USER":
        //                                                LOGINUSER = macVariableValue;
        //                                                break;

        //                                            case "PASSWORD":
        //                                                LOGINPASS = macVariableValue;
        //                                                break;

        //                                            case "RETRY":
        //                                                LOGINRETRY = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }

        //                                    macropromt.OnLoginUsername = LOGINUSER;
        //                                    macropromt.OnLoginPass = LOGINPASS;
        //                                    macropromt.OnLoginRetry = LOGINRETRY;
        //                                    break;
        //                                #endregion

        //                                #region PAUSE
        //                                case MacroCommands.PAUSE:
        //                                    mPlayer.OnCommandFromView_Raised("MacroPause");
        //                                    break;
        //                                #endregion

        //                                #region PROMPT
        //                                case MacroCommands.PROMPT:
        //                                    string PROMPTVARTYPE = "", PROMPTMESSAGE = "", PROMPTDEFAULTVAL = "";
        //                                    bool showtextbox = false;
        //                                    var promptVals = GetRegexMacroCommands(mac.Value)?.ToList();
        //                                    for (int k = 0; k < promptVals.Count; k++)
        //                                    {
        //                                        var macval = promptVals[k];
        //                                        switch (k)
        //                                        {
        //                                            case 0:
        //                                                PROMPTMESSAGE = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;

        //                                            case 1:
        //                                                PROMPTVARTYPE = macval;
        //                                                showtextbox = true;
        //                                                break;

        //                                            case 2:
        //                                                PROMPTDEFAULTVAL = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;

        //                                            default: break;
        //                                        }
        //                                    }

        //                                    MacroPromptWindow prompt = new MacroPromptWindow();
        //                                    prompt.tbInfo.Text = PROMPTMESSAGE;
        //                                    if (showtextbox) prompt.tbInput.Text = PROMPTDEFAULTVAL;
        //                                    else prompt.tbInput.Visibility = Visibility.Collapsed;

        //                                    if (prompt.ShowDialog() == true && showtextbox)
        //                                    {
        //                                        string input = prompt.tbInput.Text;
        //                                        if (PROMPTVARTYPE != "")
        //                                        {
        //                                            if (macVals[PROMPTVARTYPE.ToUpper()] != null) macVals[PROMPTVARTYPE.ToUpper()] = input;
        //                                            else macVals.MacroVariablesValues.Add(PROMPTVARTYPE.ToUpper(), input);
        //                                        }
        //                                        if (!input.IsNullOrEmpty() && runningInJsMode)
        //                                        {
        //                                            JSMacroPlayer.setVariableMessage("iimPromptValue", input);
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region REFRESH
        //                                case MacroCommands.REFRESH:
        //                                    //CurrentlyActiveBrowser.Navigate(CurrentlyActiveBrowser.Url.ToString());
        //                                    CurrentlyActiveBrowser.Refresh();
        //                                    CurrentlyActiveBrowser.Reload();
        //                                    if (await QuitableDelay(5)) return;
        //                                    // await Task.Run(() => Thread.Sleep(500));
        //                                    break;
        //                                #endregion

        //                                #region SAVEAS
        //                                case MacroCommands.SAVEAS:
        //                                    try
        //                                    {
        //                                        //SAVEAS TYPE=(CPL|MHT|HTM|TXT|EXTRACT|BMP|PNG|JPEG) FOLDER=folder_name FILE=file_name
        //                                        //no support for SAVEAS TYPE=EXTRACT FOLDER="C:\\My Macros\\Downloads FILE=*" format
        //                                        string SAVEASTYPE = "", SAVEASFOLDER = "", SAVEASFILE = "", SAVEASURL = "", SAVEASTEXT = "", saveasFilepath = "";
        //                                        await Task.Run(() =>
        //                                        {
        //                                            foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                            {
        //                                                if (!macval.Contains("=")) continue;
        //                                                var macVariable = macval.Remove(macval.IndexOf('='));
        //                                                var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                                switch (macVariable.ToUpper())
        //                                                {
        //                                                    case "TYPE":
        //                                                        SAVEASTYPE = macVariableValue;
        //                                                        break;

        //                                                    case "FOLDER":
        //                                                        SAVEASFOLDER = macVariableValue;
        //                                                        break;

        //                                                    case "FILE":
        //                                                        SAVEASFILE = macVariableValue;
        //                                                        break;

        //                                                    case "URL":
        //                                                        SAVEASURL = macVariableValue;
        //                                                        break;

        //                                                    case "TEXT":
        //                                                        SAVEASTEXT = macVariableValue;
        //                                                        break;
        //                                                    default: break;
        //                                                }
        //                                            }

        //                                            if (SAVEASFOLDER.Contains("*")) SAVEASFOLDER = MacroSettings.DefaultFolderDownloads;
        //                                            //if (SAVEASFOLDER.Contains("*")) SAVEASFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SAVEAS", SAVEASTYPE, browser.DocumentTitle != null ? RemoveSpecialCharacters(browser.DocumentTitle) : "default");
        //                                            if (SAVEASFILE.Contains("*")) SAVEASFILE = "SAVED_FILE " + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".";
        //                                            if (!SAVEASFILE.Contains(".")) SAVEASFILE += SAVEASTYPE.ToLower() == "cpl" ? "html" : SAVEASTYPE.ToLower();
        //                                            saveasFilepath = System.IO.Path.Combine(SAVEASFOLDER, SAVEASFILE);
        //                                            if (!Directory.Exists(SAVEASFOLDER))
        //                                            {
        //                                                Directory.CreateDirectory(SAVEASFOLDER);
        //                                            }
        //                                            //else
        //                                            //{
        //                                            //    string folder = SAVEASFOLDER;
        //                                            //    if (folder.Contains("\\")) folder = folder.Substring(SAVEASFOLDER.LastIndexOf("\\"));
        //                                            //    folder = folder.Replace("\\", "");
        //                                            //    int looped = 0;
        //                                            //    string fold = folder;
        //                                            //    while (Directory.Exists(SAVEASFOLDER))
        //                                            //    {
        //                                            //        fold = looped + folder;
        //                                            //        looped++;
        //                                            //        SAVEASFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SAVEAS", fold);
        //                                            //        saveasFilepath = System.IO.Path.Combine(SAVEASFOLDER, SAVEASFILE);
        //                                            //    }
        //                                            //    Directory.CreateDirectory(SAVEASFOLDER);
        //                                            //}
        //                                        });
        //                                        switch (SAVEASTYPE.ToUpper())
        //                                        {
        //                                            case "STRING":
        //                                                await Task.Run(() => { File.AppendAllText(saveasFilepath, SAVEASTEXT + Environment.NewLine); });
        //                                                break;

        //                                            case "DOWNLOADIMAGEURL":
        //                                                await Task.Run(() =>
        //                                                {
        //                                                    string imagename = SAVEASURL;
        //                                                    if (imagename.Contains("/"))
        //                                                    {
        //                                                        imagename = imagename.Substring(imagename.LastIndexOf("/") + 1);
        //                                                    }
        //                                                    MyFilesDatabase.DownloadImage(SAVEASURL, saveasFilepath + imagename);
        //                                                });
        //                                                break;

        //                                            case "BMP":
        //                                            case "PNG":
        //                                            case "JPEG":
        //                                                ImageCreator creator = new ImageCreator(CurrentlyActiveBrowser);
        //                                                byte[] mBytes = creator.CanvasGetPngImage((uint)0, (uint)0, (uint)CurrentlyActiveBrowser.Width, (uint)CurrentlyActiveBrowser.Height);
        //                                                using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(mBytes)))
        //                                                {
        //                                                    image.Save(saveasFilepath);
        //                                                }
        //                                                break;

        //                                            case "EXTRACT":
        //                                                var extract = macVals[MacroVariables.EXTRACT];
        //                                                if (saveasFilepath.Contains("ShortenedURLs"))
        //                                                {
        //                                                    extract = extract.Replace("content_copyCopy", "");
        //                                                    extract = extract.Replace("short", "");
        //                                                    extract = extract.Replace("URL", "");
        //                                                    extract = extract.Trim();
        //                                                }
        //                                                hadSaveAsFilePath = saveasFilepath.Replace("extract", "txt");
        //                                                await Task.Run(() => { File.AppendAllText(hadSaveAsFilePath, isiim == IIMPlayType.macroFromjs ? extract : extract + Environment.NewLine); });
        //                                                break;

        //                                            case "CPL":
        //                                            case "MHT":
        //                                                List<Task> taskList = new List<Task>();
        //                                                string ttl = currentMacroContentDocument.Title;
        //                                                var document = currentMacroContentDocument as Gecko.GeckoDocument;
        //                                                if (document != null)
        //                                                {
        //                                                    if (SAVEASTYPE.ToUpper() == "CPL" && document.StyleSheets != null && document.StyleSheets.Count > 0)
        //                                                    {
        //                                                        taskList.Add(Task.Run(() =>
        //                                                        {
        //                                                            Application.Current.Dispatcher.Invoke(async () =>
        //                                                            {
        //                                                                string saveAsCssDir = System.IO.Path.Combine(SAVEASFOLDER, "CSS");
        //                                                                if (!Directory.Exists(saveAsCssDir)) Directory.CreateDirectory(saveAsCssDir);
        //                                                                for (int k = 0; k < document.StyleSheets.Count; k++)
        //                                                                {
        //                                                                    var css = document.StyleSheets[k];
        //                                                                    if (css.CssRules == null) continue;

        //                                                                    //string cssRules = "";
        //                                                                    //GeckoStyleSheet.StyleRuleCollection coll = css.CssRules;
        //                                                                    //foreach (var rule in css.CssRules)
        //                                                                    //{
        //                                                                    //    cssRules += rule + Environment.NewLine;
        //                                                                    //}
        //                                                                    List<string> rules = css.CssRules.ToList().ConvertAll(x => x.ToString());
        //                                                                    string fname = css.Href;
        //                                                                    if (fname == null)
        //                                                                    {
        //                                                                        fname = "inline " + k + ".css";
        //                                                                    }
        //                                                                    else
        //                                                                    {
        //                                                                        if (fname.Contains("/")) fname = fname.Substring(fname.LastIndexOf("/") + 1);
        //                                                                        if (fname.Contains("?")) fname = fname.Remove(fname.IndexOf("?"));
        //                                                                    }
        //                                                                    while (fname.Length > 20) fname = fname.Substring(10);
        //                                                                    if (!fname.EndsWith(".css")) fname = fname + ".css";
        //                                                                    await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsCssDir, fname), rules.ToArray()));
        //                                                                }
        //                                                            });
        //                                                        }));
        //                                                    }
        //                                                    if (document.Images != null && document.Images.Length > 0)
        //                                                    {
        //                                                        taskList.Add(Task.Run(() =>
        //                                                        {
        //                                                            string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "IMAGES");
        //                                                            if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
        //                                                            Application.Current.Dispatcher.Invoke(async () =>
        //                                                            {
        //                                                                List<byte[]> alreadycreated = new List<byte[]>();
        //                                                                for (int k = 0; k < document.Images.Length; k++)
        //                                                                {
        //                                                                    var img = document.Images[k] as GeckoImageElement;

        //                                                                    try
        //                                                                    {
        //                                                                        //byte[] imgBytes = Gecko.Utils.SaveImageElement.ConvertGeckoImageElementToPng(browser, img, img.OffsetLeft, img.OffsetTop, img.Width, img.Height);
        //                                                                        string imgsrc = img.Src;
        //                                                                        byte[] imgBytes = null;
        //                                                                        string mimeType = "png";
        //                                                                        if (imgsrc.Contains("data:image/") && imgsrc.Contains(";base64,"))
        //                                                                        {
        //                                                                            imgsrc = imgsrc.Substring(imgsrc.IndexOf(";base64,") + ";base64,".Length);
        //                                                                            imgBytes = Convert.FromBase64String(imgsrc);
        //                                                                        }
        //                                                                        else
        //                                                                        {
        //                                                                            string daters = img.Src.Replace(document.Url.ToString(), "");
        //                                                                            string lastsrc = img.Src;
        //                                                                            if (lastsrc.EndsWith("/")) lastsrc = lastsrc.Remove(lastsrc.LastIndexOf("/"));
        //                                                                            if (lastsrc.Contains("/")) lastsrc = lastsrc.Substring(lastsrc.LastIndexOf("/"));
        //                                                                            if (daters.Contains(".jpg"))
        //                                                                            {
        //                                                                                mimeType = "jpg";
        //                                                                            }
        //                                                                            if (!string.IsNullOrEmpty(document.Url.Query) || !string.IsNullOrWhiteSpace(document.Url.Query)) daters = daters.Replace(document.Url.Query, "");
        //                                                                            if (!string.IsNullOrEmpty(document.Url.Query) || !string.IsNullOrWhiteSpace(document.Url.Query)) daters = daters.Replace(document.Url.Query, "");
        //                                                                            string data;
        //                                                                            using (var context = new AutoJSContext(CurrentlyActiveBrowser.Window))
        //                                                                            {
        //                                                                                context.EvaluateScript(@"
        //                                                                                    function getBase64Image() 
        //                                                                                    {
        //                                                                                        var img = document.getElementById('" + img.Id + @"');
        //                                                                                        if(img == null || img == 'undefined')
        //                                                                                        {
        //                                                                                                var elements = document.getElementsByTagName('img');
        //                                                                                                for (var i=0; i<elements.length; i++) 
        //                                                                                                {
        //                                                                                                for (var j = 0; j < elements[i].attributes.length; j++)
        //                                                                                                {
        //                                                                                                    var attrib = elements[i].attributes[j]; 
        //                                                                                                    if(attrib.value.toLowerCase().indexOf('" + daters.ToLower() + @"') > -1 || 
        //                                                                                                       attrib.value.toLowerCase().indexOf('" + img.Src.ToLower() + @"') > -1 ||
        //                                                                                                       attrib.value.toLowerCase().indexOf('" + lastsrc.ToLower() + @"') > -1)
        //                                                                                                    {
        //                                                                                                        img = elements[i];
        //                                                                                                        break;
        //                                                                                                    }
        //                                                                                                }
        //                                                                                                if(img != null)break;
        //                                                                                                }
        //                                                                                        }
        //                                                                                        // Create an empty canvas element
        //                                                                                        var canvas = document.createElement('canvas');
        //                                                                                        canvas.width = img.width;
        //                                                                                        canvas.height = img.height;

        //                                                                                        // Copy the image contents to the canvas
        //                                                                                        var ctx = canvas.getContext('2d');
        //                                                                                        ctx.drawImage(img, 0, 0);

        //                                                                                        // Get the data-URL formatted image
        //                                                                                        // Firefox supports PNG and JPEG. You could check img.src to
        //                                                                                        // guess the original format, but be aware the using 'image/jpg'
        //                                                                                        // will re-encode the image.
        //                                                                                        return canvas.toDataURL('image/" + mimeType + @"');
        //                                                                                    }

        //                                                                                    getBase64Image();", out data);
        //                                                                            }
        //                                                                            //Console.WriteLine(img.Src.ToLower());
        //                                                                            //Console.WriteLine(mimeType);
        //                                                                            //Console.WriteLine(data);
        //                                                                            if (data == null || !data.StartsWith("data:image/" + "png" + @";base64,")) continue;
        //                                                                            imgBytes = Convert.FromBase64String(data.Substring(("data:image/" + "png" + @";base64,").Length));
        //                                                                        }
        //                                                                        if (imgBytes == null) continue;
        //                                                                        if (alreadycreated.Any(b => b.SequenceEqual(imgBytes))) continue;
        //                                                                        string alt = img.Alt, src = img.Src;
        //                                                                        await Task.Run(() =>
        //                                                                        {
        //                                                                            alreadycreated.Add(imgBytes);
        //                                                                            using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(imgBytes)))
        //                                                                            {
        //                                                                                string imgname = alt;
        //                                                                                if (string.IsNullOrEmpty(imgname) || string.IsNullOrWhiteSpace(imgname))
        //                                                                                {
        //                                                                                    imgname = src;
        //                                                                                    if (imgname.Contains("/")) imgname = imgname.Substring(imgname.LastIndexOf("/") + 1);
        //                                                                                    if (imgname.Contains(".jpg?") || imgname.Contains(".png?")) imgname = imgname.Remove(imgname.IndexOf("?"));
        //                                                                                    if (!imgname.EndsWith(".png") && !imgname.EndsWith(".jpg")) imgname = imgname + mimeType;
        //                                                                                    if (!imgname.EndsWith(".png") && !imgname.EndsWith(".jpg")) imgname = imgname + ".png";
        //                                                                                }
        //                                                                                else
        //                                                                                {
        //                                                                                    imgname = imgname + k + "." + mimeType;
        //                                                                                }
        //                                                                                imgname = RemoveSpecialCharacters(imgname);
        //                                                                                while (imgname.Length > 15)
        //                                                                                {
        //                                                                                    imgname = imgname.Substring(1);
        //                                                                                }
        //                                                                                int adderfilename = 0;
        //                                                                                string imgfilePathe = System.IO.Path.Combine(saveAsDir, imgname.Replace("/", ""));
        //                                                                                string imnam = imgname;

        //                                                                                while (File.Exists(imgfilePathe))
        //                                                                                {
        //                                                                                    imgname = adderfilename + imnam;
        //                                                                                    adderfilename++;
        //                                                                                    imgfilePathe = System.IO.Path.Combine(saveAsDir, imgname.Replace("/", ""));
        //                                                                                }
        //                                                                                image.Save(imgfilePathe);
        //                                                                            }
        //                                                                        });
        //                                                                    }
        //                                                                    catch (Exception ex)
        //                                                                    {
        //                                                                    }
        //                                                                }
        //                                                            });
        //                                                        }));
        //                                                    }
        //                                                    if (SAVEASTYPE.ToUpper() == "CPL" && document.Links != null && document.Links.Length > 0)
        //                                                    {
        //                                                        taskList.Add(Task.Run(() =>
        //                                                        {
        //                                                            string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "LINKS");
        //                                                            if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
        //                                                            Application.Current.Dispatcher.Invoke(async () =>
        //                                                            {
        //                                                                List<string> rules = document.Links.ToList().ConvertAll(x => (x as GeckoAnchorElement).Href);
        //                                                                await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsDir, "links.txt"), rules.ToArray()));
        //                                                            });
        //                                                        }));
        //                                                    }
        //                                                    if (SAVEASTYPE.ToUpper() == "CPL" && document.Anchors != null && document.Anchors.Length > 0)
        //                                                    {
        //                                                        taskList.Add(Task.Run(() =>
        //                                                        {
        //                                                            string saveAsDir = System.IO.Path.Combine(SAVEASFOLDER, "ANCHORS");
        //                                                            if (!Directory.Exists(saveAsDir)) Directory.CreateDirectory(saveAsDir);
        //                                                            Application.Current.Dispatcher.Invoke(async () =>
        //                                                            {
        //                                                                List<string> rules = document.Anchors.ToList().ConvertAll(x => (x as GeckoAnchorElement).Href);
        //                                                                await Task.Run(() => File.WriteAllLines(System.IO.Path.Combine(saveAsDir, "anchors.txt"), rules.ToArray()));
        //                                                            });
        //                                                        }));
        //                                                    }

        //                                                    await Task.Run(() => Task.WaitAll(taskList.ToArray()));
        //                                                }

        //                                                GeckoHtmlElement element = null;
        //                                                GeckoElement geckoDomElement;
        //                                                if (currentMacroContentDocument != null && setfromframe) geckoDomElement = currentMacroContentDocument.DocumentElement;
        //                                                else geckoDomElement = CurrentlyActiveBrowser.Document.DocumentElement;
        //                                                if (geckoDomElement is GeckoHtmlElement)
        //                                                {
        //                                                    element = (GeckoHtmlElement)geckoDomElement;
        //                                                    var innerHtml = element.InnerHtml;

        //                                                    File.WriteAllText(saveasFilepath, innerHtml);
        //                                                }
        //                                                break;

        //                                            //case "MHT":
        //                                            //    break;

        //                                            case "HTM":
        //                                                List<Task> allhtml = new List<Task>();
        //                                                List<string> htmlContents = new List<string>();
        //                                                SaveAllHtmlPlusFrames(currentMacroContentDocument, htmlContents);
        //                                                for (int j = 0; j < htmlContents.Count; j++)
        //                                                {
        //                                                    allhtml.Add(Task.Run(() => File.WriteAllText(System.IO.Path.Combine(SAVEASFOLDER, j + SAVEASFILE), htmlContents[j])));
        //                                                }
        //                                                await Task.Run(() => Task.WaitAll(allhtml.ToArray()));
        //                                                break;

        //                                            case "TXT":
        //                                                GeckoHtmlElement elementtxt = null;
        //                                                GeckoElement geckoDomElementtxt;
        //                                                if (currentMacroContentDocument != null && setfromframe) geckoDomElementtxt = currentMacroContentDocument.DocumentElement;
        //                                                else geckoDomElementtxt = CurrentlyActiveBrowser.Document.DocumentElement;
        //                                                if (geckoDomElementtxt is GeckoHtmlElement)
        //                                                {
        //                                                    elementtxt = (GeckoHtmlElement)geckoDomElementtxt;
        //                                                    var innerHtml = elementtxt.TextContent;

        //                                                    await Task.Run(() => File.WriteAllText(saveasFilepath, innerHtml));
        //                                                }
        //                                                break;

        //                                            default:
        //                                                break;
        //                                        }
        //                                    }
        //                                    catch (Exception ex)
        //                                    {

        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region SCREENSHOT
        //                                case MacroCommands.SCREENSHOT:
        //                                    //SCREENSHOT TYPE=(PAGE|BROWSER) FOLDER=folder_name FILE=file_name
        //                                    string SCREENSHOTTYPE = "", SCREENSHOTFOLDER = "", SCREENSHOTFILE = "", filepath = "";
        //                                    await Task.Run(() =>
        //                                    {
        //                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                        {
        //                                            if (!macval.Contains("=")) continue;
        //                                            var macVariable = macval.Remove(macval.IndexOf('='));
        //                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                            switch (macVariable.ToUpper())
        //                                            {
        //                                                case "TYPE":
        //                                                    SCREENSHOTTYPE = macVariableValue;
        //                                                    break;

        //                                                case "FOLDER":
        //                                                    SCREENSHOTFOLDER = macVariableValue;
        //                                                    break;

        //                                                case "FILE":
        //                                                    SCREENSHOTFILE = macVariableValue;
        //                                                    break;
        //                                                default: break;
        //                                            }
        //                                        }

        //                                        if (SCREENSHOTFOLDER.Contains("*")) SCREENSHOTFOLDER = MacroSettings.DefaultFolderDownloads;
        //                                        //if (SCREENSHOTFOLDER.Contains("*")) SCREENSHOTFOLDER = System.IO.Path.Combine(GetBaseMacroDownloadDir(), "SCREENSHOT", browser.DocumentTitle != null ? browser.DocumentTitle : "default");
        //                                        if (SCREENSHOTFILE.Contains("*")) SCREENSHOTFILE = "SCREEN_SHOT " + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".png";
        //                                        filepath = System.IO.Path.Combine(SCREENSHOTFOLDER, SCREENSHOTFILE);
        //                                        if (!Directory.Exists(SCREENSHOTFOLDER))
        //                                        {
        //                                            Directory.CreateDirectory(SCREENSHOTFOLDER);
        //                                        }
        //                                    });
        //                                    if (SCREENSHOTTYPE.ToUpper() == "PAGE")
        //                                    {
        //                                        ImageCreator creator = new ImageCreator(CurrentlyActiveBrowser);
        //                                        byte[] mBytes = creator.CanvasGetPngImage((uint)0, (uint)0, (uint)CurrentlyActiveBrowser.Width, (uint)CurrentlyActiveBrowser.Height);
        //                                        using (System.Drawing.Image image = System.Drawing.Image.FromStream(new System.IO.MemoryStream(mBytes)))
        //                                        {
        //                                            image.Save(filepath);
        //                                        }
        //                                    }
        //                                    else if (SCREENSHOTTYPE.ToUpper() == "BROWSER")
        //                                    {
        //                                        using (Bitmap bmpScreenCapture = new Bitmap(System.Windows.Forms.SystemInformation.WorkingArea.Width, (int)System.Windows.Forms.SystemInformation.WorkingArea.Height))
        //                                        {
        //                                            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
        //                                            {
        //                                                g.CopyFromScreen(0,
        //                                                                 0,
        //                                                                 0, 0,
        //                                                                 bmpScreenCapture.Size,
        //                                                                 CopyPixelOperation.SourceCopy);

        //                                                bmpScreenCapture.Save(filepath);
        //                                            }
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region SEARCH
        //                                case MacroCommands.SEARCH:
        //                                    //SEARCH SOURCE=(TXT|REGEXP) IGNORE_CASE=YES EXTRACT=$1
        //                                    string SEARCHSOURCE = "", SEARCHIGNORE_CASE = "NO", SEARCHEXTRACT = "", textToSearchFor = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "SOURCE":
        //                                                SEARCHSOURCE = macVariableValue.Contains(":") ? macVariableValue.Remove(macVariableValue.IndexOf(":")) : macVariableValue;
        //                                                textToSearchFor = macVariableValue.Contains(":") ? macVariableValue.Substring(macVariableValue.IndexOf(":") + 1) : macVariableValue;
        //                                                break;

        //                                            case "IGNORE_CASE":
        //                                                SEARCHIGNORE_CASE = macVariableValue;
        //                                                break;

        //                                            case "EXTRACT":
        //                                                SEARCHEXTRACT = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }

        //                                    switch (SEARCHSOURCE.ToUpper())
        //                                    {
        //                                        case "TXT":
        //                                        case "REGEXP":
        //                                            int timesStepped = 0, timeoutstep = 6;
        //                                            int.TryParse(macVals[MacroVariables.TIMEOUT_STEP], out timeoutstep);
        //                                            bool found = false;
        //                                            while (!found && timeoutstep >= timesStepped)
        //                                            {
        //                                                GeckoHtmlElement elementtxt = null;
        //                                                GeckoElement geckoDomElementtxt;
        //                                                if (currentMacroContentDocument != null && setfromframe) geckoDomElementtxt = currentMacroContentDocument.DocumentElement;
        //                                                else geckoDomElementtxt = CurrentlyActiveBrowser.Document.DocumentElement;
        //                                                if (geckoDomElementtxt is GeckoHtmlElement)
        //                                                {
        //                                                    elementtxt = (GeckoHtmlElement)geckoDomElementtxt;
        //                                                    var innerHtml = elementtxt.TextContent;
        //                                                    if (SEARCHIGNORE_CASE.ToUpper() == "YES")
        //                                                    {
        //                                                        innerHtml = innerHtml.ToLower();
        //                                                        if (SEARCHSOURCE.ToUpper() == "TXT") textToSearchFor = textToSearchFor.ToLower();
        //                                                    }

        //                                                    if (SEARCHSOURCE.ToUpper() == "TXT") found = innerHtml.Contains(textToSearchFor);
        //                                                    else
        //                                                    {
        //                                                        var matches = Regex.Split(innerHtml, textToSearchFor);
        //                                                        if (matches != null && matches.Length > 0)
        //                                                        {
        //                                                            found = true;
        //                                                            SEARCHEXTRACT = SEARCHEXTRACT.Replace("$1", matches[matches.Length == 1 ? 0 : 1]);
        //                                                            macVals[MacroVariables.EXTRACT] = SEARCHEXTRACT;
        //                                                        }
        //                                                    }
        //                                                }

        //                                                timesStepped++;
        //                                                if (await QuitableDelay(10)) return;
        //                                                // await Task.Run(() => Thread.Sleep(1000));
        //                                            }
        //                                            if (!found && macVals[MacroVariables.ERRORIGNORE] == "NO")
        //                                            {
        //                                                mPlayer.MacroPlayer.Macros.Clear();
        //                                            }
        //                                            break;

        //                                        default:
        //                                            break;
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region SET
        //                                case MacroCommands.SET:
        //                                case MacroCommands.CMDLINE:
        //                                    string VARIABLE = "", VALUE = "";
        //                                    var setValues = GetRegexMacroCommands(mac.Value).ToList();
        //                                    for (int k = 0; k < setValues.Count; k++)
        //                                    {
        //                                        var macval = setValues[k];
        //                                        switch (k)
        //                                        {
        //                                            case 0:
        //                                                VARIABLE = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;

        //                                            case 1:
        //                                                VALUE = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                                break;
        //                                            default:
        //                                                break;
        //                                        }
        //                                    }
        //                                    if (VARIABLE != "")
        //                                    {
        //                                        if (macVals[VARIABLE] == null) macVals.MacroVariablesValues.Add(VARIABLE.ToUpper(), "");
        //                                        if (VARIABLE != MacroVariables.DATASOURCE_LINE) macVals[VARIABLE] = VALUE;

        //                                        switch (VARIABLE)
        //                                        {
        //                                            case MacroVariables.CLIPBOARD:
        //                                                MyFilesDatabase.SetClipboardText(macVals[VARIABLE], false);
        //                                                break;

        //                                            case MacroVariables.DATASOURCE_LINE:
        //                                                if (mac.Value.ToUpper().Contains("{{!LOOP}}"))
        //                                                {
        //                                                    macVals[MacroVariables.DATASOURCE_LINE] = !runningInJsMode ? (dataCourceLine + 1).ToString() : (mPlayer.CurrentJSDatasourceLoopPos + 1).ToString();
        //                                                }
        //                                                else macVals[MacroVariables.DATASOURCE_LINE] = VALUE;
        //                                                if (macVals.Columns.Count < Convert.ToInt32(macVals[MacroVariables.DATASOURCE_LINE]) || !macVals.Columns.Any(c => c.Row == Convert.ToInt32(macVals[MacroVariables.DATASOURCE_LINE]) - 1))
        //                                                {
        //                                                    mPlayer.StopRequested = true;
        //                                                }
        //                                                break;

        //                                            case MacroVariables.DATASOURCE:
        //                                                macVals.Columns.Clear();
        //                                                macVals[MacroVariables.DATASOURCE_COLUMNS] = "";
        //                                                PersonData pdataSource = null;
        //                                                string[] lines = null;
        //                                                bool wasSlideOut = false;
        //                                                switch (VALUE.ToUpper())
        //                                                {
        //                                                    case MacroVariables.MacroDatasourceValues.DATASOURCE_SLIDEOUT:
        //                                                        if (mPlayer.DataSourceSlideoutText.IsNullOrEmpty()) break;
        //                                                        lines = mPlayer.DataSourceSlideoutText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        //                                                        wasSlideOut = true;
        //                                                        break;

        //                                                    case MacroVariables.MacroDatasourceValues.DATASOURCE_MAINPROJECTPROFILE:
        //                                                        pdataSource = GloableProfData.PData;
        //                                                        break;
        //                                                    case MacroVariables.MacroDatasourceValues.DATASOURCE_SELECTPROFILE:
        //                                                        pdataSource = await getSelectedProfile(true, mPlayer.SelectedMacroPlayingFileName);
        //                                                        break;

        //                                                    default:
        //                                                        if (VALUE.ToUpper().Contains(MacroVariables.MacroDatasourceValues.DATASOURCE_PROFILENAME))
        //                                                        {
        //                                                            string profileValue = VALUE.ToUpper().Trim();
        //                                                            var split = profileValue.Split('=');
        //                                                            if (split.Length > 0)
        //                                                            {
        //                                                                profileValue = split[1];

        //                                                                if (MyFilesDatabase.HasMultipleProfiles(GloableProfData.PData.ProjectDir))
        //                                                                {
        //                                                                    var directoryValues = MyFilesDatabase.GetSubProjectsFolders(GloableProfData.PData.ProjectDir, GloableProfData.PData.ProjectName);
        //                                                                    foreach (var prof in directoryValues)
        //                                                                    {
        //                                                                        string pname = prof.Key;
        //                                                                        if (pname.Contains("_folder_")) continue;

        //                                                                        if (pname.Contains("_tier_"))
        //                                                                            pname = pname.Replace("_tier_", "");

        //                                                                        if (pname.Trim().ToLower() == profileValue.ToLower())
        //                                                                        {
        //                                                                            pdataSource = MyFilesDatabase.GetSubProjectPersonData(prof.Value);
        //                                                                        }
        //                                                                    }

        //                                                                }
        //                                                            }
        //                                                        }
        //                                                        break;
        //                                                }
        //                                                if (pdataSource != null)
        //                                                {
        //                                                    string sex = pdataSource.CmbSelectedIndexSex == 0 ? "MALE" : "FEMALE";
        //                                                    lines = new string[]
        //                                                    {
        //                                                pdataSource.ProjectName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.ProfileName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.ProxyIP+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.ProxyPort+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.ProxyUsername+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.ProxyPassword+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.FirstName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.LastName+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.PhoneNumber+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Username+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Email+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Password+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                (pdataSource.CmbSelectedIndexSex + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                (pdataSource.CmbSelectedIndexDay + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                (pdataSource.CmbSelectedIndexMonth + 1).ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.BirthdayYear.ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                (pdataSource.CmbSelectedIndexMonth + 1).ToString() +(pdataSource.CmbSelectedIndexDay + 1).ToString() + pdataSource.BirthdayYear.ToString()+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Street+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.City+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.State+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Zip+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Country+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.WebAddress+macVals[MacroVariables.DATASOURCE_DELIMITER]+
        //                                                pdataSource.Notes
        //                                                    };
        //                                                }
        //                                                if (lines == null)
        //                                                {
        //                                                    string fpath = VALUE;
        //                                                    if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaulFoldertDataSources, VALUE);
        //                                                    if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaultFolderDownloads, VALUE);
        //                                                    if (!File.Exists(fpath)) fpath = System.IO.Path.Combine(MacroSettings.DefaulFoldertMacros, VALUE);
        //                                                    if (File.Exists(fpath)) lines = File.ReadAllLines(fpath);
        //                                                }
        //                                                if (lines != null)
        //                                                {
        //                                                    for (int k = 0; k < lines.Length; k++)
        //                                                    {
        //                                                        if (lines[k].IsNullOrEmpty()) continue;

        //                                                        string[] cols = lines[k].Split(new string[] { macVals[MacroVariables.DATASOURCE_DELIMITER] }, StringSplitOptions.None);
        //                                                        for (int j = 0; j < cols.Length; j++)
        //                                                        {
        //                                                            string dsclmns = macVals[MacroVariables.DATASOURCE_COLUMNS];
        //                                                            if (dsclmns != "") if (Convert.ToInt32(dsclmns) > j) break;
        //                                                            macVals.SetDSColVariablesValues(k, j, cols[j]);
        //                                                        }
        //                                                    }

        //                                                    if (wasSlideOut)
        //                                                    {
        //                                                        if (!runningInJsMode) totalDataSourceLines = lines.Length;
        //                                                        else mPlayer.DatasourceMaxLoop = lines.Length;
        //                                                    }
        //                                                }
        //                                                break;

        //                                            case MacroVariables.LOOP:
        //                                                int cuuLoop = -1;
        //                                                int.TryParse(macVals[MacroVariables.LOOP], out cuuLoop);
        //                                                cuuLoop = cuuLoop - 1;
        //                                                if (cuuLoop > 0)
        //                                                {
        //                                                    iTimesToRun = cuuLoop;
        //                                                    break;
        //                                                }
        //                                                break;

        //                                            default:
        //                                                break;
        //                                        }
        //                                    }
        //                                    break;

        //                                #endregion

        //                                #region TAG
        //                                case MacroCommands.TAG:
        //                                    //currentContentDocument = CurrentlyActiveBrowser.Document;
        //                                    previosTagElementFound = await TagCommand
        //                                        (
        //                                            mac.Value, mPlayer.MacroPlayer, macVals,
        //                                            (setfromframe && currentMacroContentDocument != null) ? currentMacroContentDocument : CurrentlyActiveBrowser.Document, currentContentDocumentIframe, currentContentDocumentFrame,
        //                                            previosTagElementFound,
        //                                            iTimesToRun
        //                                        );
        //                                    break;
        //                                #endregion

        //                                #region URL
        //                                case MacroCommands.URL:
        //                                    string LINK = "";
        //                                    if (mac.Value.StartsWith("GOTO=javascript:((") && mac.Value.EndsWith("();"))
        //                                    {
        //                                        LINK = mac.Value.Replace("GOTO=", "");
        //                                    }
        //                                    else
        //                                    {
        //                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                        {
        //                                            if (!macval.Contains("=")) continue;
        //                                            var macVariable = macval.Remove(macval.IndexOf('='));
        //                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                            switch (macVariable.ToUpper())
        //                                            {
        //                                                case "GOTO":
        //                                                    LINK = macVariableValue;
        //                                                    break;
        //                                                default: break;
        //                                            }
        //                                        }
        //                                    }
        //                                    CurrentlyActiveBrowser.Navigate(LINK.Trim());
        //                                    if (await QuitableDelay(10)) return;
        //                                    //await Task.Run(() => Thread.Sleep(1000));
        //                                    break;
        //                                #endregion

        //                                #region WAIT
        //                                case MacroCommands.WAIT:
        //                                    string LENGTH = "1";
        //                                    bool hadSeconds = false;
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (macval.ToUpper() == "SECONDS")
        //                                        {
        //                                            hadSeconds = true;
        //                                            continue;
        //                                        }
        //                                        if (hadSeconds)
        //                                        {
        //                                            int foundNumber = 0;
        //                                            bool parsed = int.TryParse(macval, out foundNumber);
        //                                            if (parsed)
        //                                            {
        //                                                LENGTH = macval;
        //                                                break;
        //                                            }
        //                                            else
        //                                            {
        //                                                continue;
        //                                            }
        //                                        }
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "SECONDS":
        //                                                LENGTH = macVariableValue;
        //                                                break;
        //                                            default: break;
        //                                        }
        //                                    }
        //                                    int amount = Convert.ToInt32(LENGTH.Trim());
        //                                    for (int waiter = 0; waiter < amount; waiter++)
        //                                    {
        //                                        if (mPlayer.StopRequested || !macroPlayer.IsRunning)
        //                                        {
        //                                            if (runningInJsMode) JSMacroPlayer.macroDone(CurrentlyActiveBrowser.Window.DomWindow, CurrentlyActiveBrowser.DomDocument.NativeDomDocument);
        //                                            break;
        //                                        }

        //                                        if (await QuitableDelay(10)) return;
        //                                        // await Task.Run(() => Thread.Sleep(1000));
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region TAB
        //                                case MacroCommands.TAB:
        //                                    //TAB (T=n|OPEN|CLOSE|CLOSEALLOTHERS)
        //                                    string TAB_T = "", TAB_ACTION = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (macval.Contains("="))
        //                                        {
        //                                            var macVariable = macval.Remove(macval.IndexOf('='));
        //                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                            switch (macVariable.ToUpper())
        //                                            {
        //                                                case "T":
        //                                                    TAB_T = macVariableValue;
        //                                                    break;
        //                                                default: break;
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            TAB_ACTION = GetMacroVariableAfterDynamicCheck(macval, macVals);
        //                                        }
        //                                    }
        //                                    //macroWaitingFor = mac.Command + mac.Line + mac.Value;
        //                                    if (!TAB_ACTION.IsNullOrEmpty())
        //                                    {
        //                                        switch (TAB_ACTION.ToUpper())
        //                                        {
        //                                            case "OPEN":
        //                                                TabCommandNavigation tcn = new TabCommandNavigation((tabsWindows.Count + 2).ToString());
        //                                                tcn.OnClosedWindow += Tcn_OnClosedWindow;
        //                                                await tcn.WaitForComplete();
        //                                                tabsWindows.Add(tcn);
        //                                                //OnOpenNewTab();
        //                                                break;

        //                                            case "CLOSE":
        //                                                if (!setFromTabsWindows)
        //                                                {
        //                                                    macroPlayer.StopRequested = true;
        //                                                    OnCloseTab(false, this);
        //                                                }
        //                                                else
        //                                                {
        //                                                    if (windowIndexToSet >= 0 && setFromTabsWindows && tabsWindows.Count - 1 >= windowIndexToSet)
        //                                                    {
        //                                                        tabsWindows[windowIndexToSet].OnClosedWindow -= Tcn_OnClosedWindow;
        //                                                        tabsWindows[windowIndexToSet].CloseAndDispose();
        //                                                        tabsWindows.RemoveAt(windowIndexToSet);
        //                                                        windowIndexToSet -= 1;
        //                                                        setFromTabsWindows = false;

        //                                                        if (tabsWindows.Count > 0 && windowIndexToSet >= 0)
        //                                                        {
        //                                                            setFromTabsWindows = true;
        //                                                        }
        //                                                    }
        //                                                }
        //                                                break;

        //                                            case "CLOSEALLOTHERS":
        //                                                //OnCloseTab(true,this);
        //                                                if (tabsWindows.Count > 0)
        //                                                {
        //                                                    foreach (var tw in tabsWindows)
        //                                                    {
        //                                                        tw.OnClosedWindow -= Tcn_OnClosedWindow;
        //                                                        tw.CloseAndDispose();
        //                                                    }
        //                                                    tabsWindows.Clear();

        //                                                    setFromTabsWindows = false;
        //                                                    windowIndexToSet = -1;
        //                                                }
        //                                                break;

        //                                            default:
        //                                                break;
        //                                        }
        //                                    }
        //                                    if (!TAB_T.IsNullOrEmpty())
        //                                    {
        //                                        int tabToSet = -1;
        //                                        int.TryParse(TAB_T, out tabToSet);
        //                                        if (tabToSet != -1)
        //                                        {
        //                                            if (tabToSet == 1 || (tabToSet > 1 && tabsWindows.Count < tabToSet - 1))
        //                                            {
        //                                                setFromTabsWindows = false;
        //                                                windowIndexToSet = -1;
        //                                            }
        //                                            else
        //                                            {
        //                                                setFromTabsWindows = true;
        //                                                windowIndexToSet = tabToSet - 2;
        //                                            }
        //                                            //OnChangeTabContext(tabToSet,this);
        //                                        }
        //                                    }
        //                                    break;
        //                                #endregion

        //                                #region CAPTCHA
        //                                case MacroCommands.SOLVE:
        //                                    if (MacroSettings.TwoCaptchaKey.IsNullOrEmpty()) continue;
        //                                    try
        //                                    {
        //                                        string TYPE = "", KEY = "";
        //                                        foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                        {
        //                                            if (!macval.Contains("=")) continue;
        //                                            var macVariable = macval.Remove(macval.IndexOf('='));
        //                                            var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                            switch (macVariable.ToUpper())
        //                                            {
        //                                                case "TYPE":
        //                                                    TYPE = macVariableValue.Trim();
        //                                                    break;

        //                                                case "KEY":
        //                                                    KEY = macVariableValue.Trim();
        //                                                    break;
        //                                                default: break;
        //                                            }
        //                                        }
        //                                        //CAPTCHAv2
        //                                        string result = string.Empty;
        //                                        using (var context = new AutoJSContext(CurrentlyActiveBrowser.Window))
        //                                        {
        //                                            bool success = context.EvaluateScript("var a=window.content.document.getElementsByTagName('iframe');  var k='';  for(var x=0;x<a.length;x++)  {   if(a[x].src.includes('https://www.google.com/recaptcha/api2/anchor?k'))   {    k=a[x].src.split('?k=')[1].split('&')[0];    a[x].setAttribute(\"name\",\"I0_myownid\");    window.content.document.getElementById('g-recaptcha-response').style.display='';    break;   }  }  window.content.document.getElementById('g-recaptcha-response').textContent=k;", out result);
        //                                        }

        //                                        if (!result.IsNullOrEmpty())
        //                                        {
        //                                            Organiser.Common.Classes.Helpers.TwoCaptchaClient client = new Organiser.Common.Classes.Helpers.TwoCaptchaClient(MacroSettings.TwoCaptchaKey);
        //                                            string captchaResult = "", proxy = "", currenturl = CurrentlyActiveBrowser.Document.Location.Href;
        //                                            if (!GloableProfData.PData.ProxyPassword.IsNullOrEmpty())
        //                                            {
        //                                                proxy = GloableProfData.PData.ProxyUsername + ":" + GloableProfData.PData.ProxyPassword + "@" + GloableProfData.PData.ProxyIP + ":" + GloableProfData.PData.ProxyPort;
        //                                            }
        //                                            bool succeeded = await Task.Run(() => { return client.SolveRecaptchaV2(result, currenturl, proxy, Organiser.Common.Classes.Helpers.ProxyType.HTTP, out captchaResult); });
        //                                            if (succeeded)
        //                                            {
        //                                                var textArea = CurrentlyActiveBrowser.Document.GetElementById("g-recaptcha-response") as GeckoTextAreaElement;
        //                                                textArea.Style.SetPropertyValue("display", "");
        //                                                textArea.Value = captchaResult;
        //                                            }
        //                                        }
        //                                    }
        //                                    catch (Exception exxxxx)
        //                                    {
        //                                    }

        //                                    break;
        //                                #endregion

        //                                case MacroCommands.PASTE:
        //                                    var focusedElement = CurrentlyActiveBrowser.Document.ActiveElement;
        //                                    //      OnBringToFrontForPaste();
        //                                    //      await Task.Delay(200);
        //                                    //      //focusedElement.Focus();
        //                                    //      //focusedElement.Click();
        //                                    //      //focusedElement.DOMHtmlElement.SetCapture(true);
        //                                    //      
        //                                    //      //System.Drawing.Rectangle rect = focusedElement.GetBoundingClientRect();
        //                                    //      //float rectx = (rect.Left + rect.Right) / 2;
        //                                    //      //float recty = (rect.Top + rect.Bottom) / 2;
        //                                    //      //CurrentlyActiveBrowser.Window.WindowUtils.SendMouseEvent("mousedown", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
        //                                    //      //CurrentlyActiveBrowser.Window.WindowUtils.SendMouseEvent("mouseup", rectx, recty, GeckoMouseButton.Left, 1, 0, true, 0, 0);
        //                                    //      
        //                                    //      //await Task.Delay(1500);
        //                                    //      System.Windows.Forms.SendKeys.SendWait("^V");

        //                                    DomEventArgs ev = CurrentlyActiveBrowser.Document.CreateEvent("Event");
        //                                    var webEvent = new Gecko.WebIDL.Event(CurrentlyActiveBrowser.Window.DomWindow, ev.DomEvent as nsISupports);
        //                                    webEvent.InitEvent("paste", true, true);
        //                                    focusedElement.GetEventTarget().DispatchEvent(ev);
        //                                    break;

        //                                case MacroCommands.OPENFILE:
        //                                    string OPENFILE_FOLDER = "", OPENFILE_FILE = "";
        //                                    foreach (var macval in GetRegexMacroCommands(mac.Value))
        //                                    {
        //                                        if (!macval.Contains("=")) continue;
        //                                        var macVariable = macval.Remove(macval.IndexOf('='));
        //                                        var macVariableValue = GetMacroVariableAfterDynamicCheck(macval.Substring(macval.IndexOf('=') + 1), macVals);
        //                                        switch (macVariable.ToUpper())
        //                                        {
        //                                            case "FOLDER":
        //                                                OPENFILE_FOLDER = macVariableValue;
        //                                                break;

        //                                            case "FILE":
        //                                                OPENFILE_FILE = macVariableValue;
        //                                                break;

        //                                            default: break;
        //                                        }
        //                                    }

        //                                    if (OPENFILE_FOLDER.Contains("*")) OPENFILE_FOLDER = MacroSettings.DefaultFolderDownloads;

        //                                    string filePath = Path.Combine(OPENFILE_FOLDER, OPENFILE_FILE);
        //                                    if (File.Exists(filePath)) Process.Start(filePath);
        //                                    break;

        //                                case MacroCommands.PROXY:
        //                                    break;

        //                                case MacroCommands.IMAGESEARCH:
        //                                    break;

        //                                case MacroCommands.STOPWATCH:
        //                                    break;

        //                                //case MacroCommands.SAVEITEM: with tag command
        //                                //    break;

        //                                //case MacroCommands.TRAY:only ie
        //                                //    break;

        //                                //case MacroCommands.DS: only ie
        //                                //    break;

        //                                //case MacroCommands.IMAGECLICK:only ie
        //                                //    break;

        //                                //case MacroCommands.ONCERTIFICATEDIALOG:only ie
        //                                //    break;

        //                                //case MacroCommands.ONINSECURECONNECTION: not on ff
        //                                //    break;

        //                                //case MacroCommands.ONPRINT: only ie
        //                                //    break;

        //                                //case MacroCommands.ONSECURITYDIALOG: only ie
        //                                //    break;

        //                                //case MacroCommands.ONWEBPAGEDIALOG: only ie
        //                                //    break;

        //                                //case MacroCommands.PRINT: only ie
        //                                //    break;

        //                                //case MacroCommands.SIZE: ie only
        //                                //    break;

        //                                default: break;
        //                            }

        //                        }
        //                        catch (System.Runtime.InteropServices.InvalidComObjectException)
        //                        {
        //                            if (retrystep++ <= 2)
        //                            {
        //                                macroIndex--;
        //                                if (macroIndex < 0) macroIndex = 0;
        //                                try
        //                                {
        //                                    if (!setfromwindow)
        //                                    {
        //                                        WebBrowser.Focus();
        //                                        CurrentlyActiveBrowser.Focus();
        //                                    }
        //                                    else
        //                                    {
        //                                        //ffpopupMacros.Focus();
        //                                        ffpopupMacrosBrowser.Focus();
        //                                    }
        //                                }
        //                                catch { }
        //                                continue;
        //                            }
        //                        }

        //                        mac.SetTransparent();
        //                    }
        //                    #endregion
        //                }
        //            }
        //        }
        //        catch
        //        {
        //            macroPlayer.StopRequested = true;
        //            JSMacroPlayer.setVariableMessage("iimReturnVal", "-1");
        //        }

        //        if (!runningInJsMode || macroPlayer.StopRequested || !macroPlayer.IsRunning)
        //        {
        //            mPlayer.IsRunning = false;
        //            if (macroPlayer.StopRequested) runningInJsMode = false;
        //            if (!hadSaveAsFilePath.IsNullOrEmpty())
        //            {
        //                Process.Start(hadSaveAsFilePath);
        //                hadSaveAsFilePath = "";
        //            }
        //        }

        //        if (isiim == IIMPlayType.macroFromjs)
        //        {
        //            isrunningiimInJsMode = false;
        //            //JSMacroCommandCallbacks.DummyForm.Close();
        //            //WebBrowserHost.Dispatcher.InvokeShutdown();
        //            //try
        //            //{
        //            //    using (new ErrorModeContext(ErrorModes.FailCriticalErrors | ErrorModes.NoGpFaultErrorBox | ErrorModes.SEM_NOGPFAULTERRORBOX))
        //            //    {
        //            //        IntPtr pUnk = Marshal.GetIUnknownForObject(JSMacroCommandCallbacks);
        //            //        Marshal.Release(pUnk);
        //            //    }
        //            //}
        //            //catch { }
        //            //JSMacroCommandCallbacks = null;
        //            //GC.Collect();

        //            //JSMacroCommandCallbacks = new MacroCommandExecutions();
        //            //JSMacroCommandCallbacks.ParentVM = this;
        //            //JSMacroPlayer.setHandler(JSMacroCommandCallbacks);
        //        }
        //    }
        //}
    }
