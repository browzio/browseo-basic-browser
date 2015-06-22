namespace Xilium.CefGlue.Client
{
    using Organiser.Common;
    using PData.FilesReader;
    using Organiser.Common.Classes;
    using SocialOrganizer.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using WindowsInput;
    using Xilium.CefGlue;

    internal sealed class DemoApp : CefApp
    {
        public DemoApp() : base()
        {
            _renderProcessHandler = new DemoCefRenderProcessHandler();
        }
        private readonly DemoCefRenderProcessHandler _renderProcessHandler;

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            //if (!System.IO.File.Exists("C:\\file.txt"))
              //  commandLine.AppendSwitch("proxy-server", "23.94.20.30:80");
            //else
             //   commandLine.AppendSwitch("proxy-server", "192.171.233.149:80");

            //System.IO.File.Create("C:\\file.txt");

            //commandLine.AppendArgument("request-context-per-browser");
            if (BrowserInit.pData != null && !string.IsNullOrEmpty(BrowserInit.pData.ProxyIP) && !string.IsNullOrWhiteSpace(BrowserInit.pData.ProxyIP))
            {
                try
                {
                    commandLine.AppendSwitch("proxy-server", BrowserInit.pData.ProxyIP+":"+BrowserInit.pData.ProxyPort);
                }
                catch 
                {
                    MessageBox.Show("failed to set proxy");
                }
            }
        }
    }

    internal class DemoCefRenderProcessHandler : CefRenderProcessHandler
    {
        bool hasToInject;
        PersonData profile;
        bool isTumblr;
        int tumblrcounter;

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (message.Name == "NavChange")
            {
                hasToInject = false;
            }
            else if (message.Name.Contains("{||}"))
            {
                this.isTumblr = false;
                tumblrcounter = 0;

                string[] splitPersonDatas = message.Name.Split(new string[] { "{||}" }, StringSplitOptions.None);
                string path = splitPersonDatas[0];
                string isTheMulti = splitPersonDatas[1];
                string selectedMulti = splitPersonDatas[2];
                string isTumblr = splitPersonDatas[3];
                if (isTumblr == "true")
                    this.isTumblr = true;

                profile = new PersonData();

                if (isTheMulti == "false")
                {
                    profile = MyFilesDatabase.SetProfileFromini(path);
                }
                else
                {
                    profile = MyFilesDatabase.GetSubProjectPersonData(selectedMulti);
                }

                hasToInject = true;
            }

            return false;
        }


        protected override void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDomNode node)
        {
            //string jsToExecute = "var all = document.getElementsByTagName('*');" +
            //                      "for (var i=0, max=all.length; i < max; i++) {" +
            //                        "if(all[i].tagName.indexOf('INPUT') > -1){" +
            //                            "for (var j = 0; j < all[i].attributes.length; j++) {" +
            //                                "var attrib = all[i].attributes[j]; " +
            //                                "if(attrib.value.indexOf('password') > -1){" +
            //                                     "all[i].value=123456; break;" +
            //                                 "}" +
            //                            "}" +
            //                        "}" +
            //                      "}";
            //frame.ExecuteJavaScript(jsToExecute, frame.Url, 0);
            if (!hasToInject) return;
            if (node == null) return;
            if (!node.IsFormControlElement) return;
            try
            {
                foreach (var item in node.GetAttributes())
                {
                    string val = item.Value;
                    if (val.Contains("first"))
                    {
                        Clipboard.SetText(profile.FirstName);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("last"))
                    {
                        Clipboard.SetText(profile.LastName);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("mail"))
                    {
                        Clipboard.SetText(profile.Email);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        if (isTumblr) tumblrcounter++;
                        break;
                    }
                    else if (val.Contains("user"))
                    {
                        Clipboard.SetText(profile.Username);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        if (isTumblr) tumblrcounter++;
                        break;
                    }
                    else if (val.Contains("phone"))
                    {
                        Clipboard.SetText(profile.PhoneNumber);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("gender"))
                    {
                        Clipboard.SetText(profile.SexList[profile.CmbSelectedIndexSex]);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("day"))
                    {
                        Clipboard.SetText(profile.DayList[profile.CmbSelectedIndexDay]);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V); 
                        break;
                    }
                    else if (val.Contains("month"))
                    {
                        Clipboard.SetText(profile.MonthList[profile.CmbSelectedIndexMonth]);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("year"))
                    {
                        Clipboard.SetText(profile.BirthdayYear.ToString());
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        break;
                    }
                    else if (val.Contains("pass"))
                    {
                        Clipboard.SetText(profile.Password);
                        InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                        if (isTumblr) tumblrcounter++;
                        break;
                    }
                }

                InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);

                if (!node.IsFormControlElement || tumblrcounter >= 3)
                {
                    hasToInject = false;
                }
            }
            catch { }
        }
    }
}
