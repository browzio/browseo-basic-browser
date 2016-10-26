using BrowserAndFeatures;
using Eli.WpfHost.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xilium.CefGlue.Client;
using System.Diagnostics;
using zFirefoxBrowser.Helpers;

namespace BrowserPlugin
{
    public class Plugin : PluginBase
    {
        FeatureCallage fc;

        public override event Action<string> OnMessageFromPlugin;

        public override void CheckVersion(string version)
        {
           // if(version!="")
        }

        public override FrameworkElement CreateControl(double v, int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            while (true) { }
        }

        public override FrameworkElement CreateControl(bool cbAllowProspector, bool cbAllowRSS, bool cbAllowPBN, bool cbAllowFeedMash, bool cbAllowIndexer, bool cbYoutube, bool canSeeProxyData, bool hasKK, int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            //fc.SetPersonData();
            //fc = new FeatureCallage(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            //fc.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);

            BrowserInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            FoxInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            fc = new FeatureCallage();
            fc.SetPermissions(cbAllowProspector, cbAllowRSS, cbAllowPBN, cbAllowFeedMash, cbAllowIndexer, cbYoutube, canSeeProxyData, hasKK);
            fc.OnClickedReminders += Fc_OnClickedReminders;
            fc.OnRequestedScreenLocation += Fc_OnRequestedScreenLocation;
            return fc;
        }

        public override void Dispose()
        {
            if (fc != null)
                fc.CloseAll();
            base.Dispose();
        }

        public override List<Process> GetProjectsProcesses()
        {
            return ProcessManager.Instance.Processes;
        }

        public override void KillAllProcesses()
        {
            ProcessManager.Instance.DisposeAllProcess();
        }

        public override void OnTabFocused()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                if (BrowserSettimgs.SetSysDateEnabled)
                {
                    try
                    {
                        TimeHelper.StartSetTimeAndZoneProcess(new DateAndTimeZone() { TimeZone = TimeZoneInfo.GetSystemTimeZones()[BrowserSettimgs.SITimeZone] });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    TimeHelper.SetOriginalTimeZonesFromFile();
                }
            });
        }

        public override void RaiseMessageFromHost(string message)
        {
            switch (message)
            {
                case "REMINDERS_CHANGED":
                    fc.SetRemindersCount();
                    break;

                default:
                    fc.GotScrennCords(message);
                    break;
            }
        }

        public override void SetBrowserPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            BrowserInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            FoxInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
        }

        private void Fc_OnClickedReminders()
        {
            if(OnMessageFromPlugin != null) OnMessageFromPlugin("REMINDERS_CLICK");
        }



        private void Fc_OnRequestedScreenLocation()
        {
            if (OnMessageFromPlugin != null) OnMessageFromPlugin("SCREEN_SIZE");
        }
    }
}
