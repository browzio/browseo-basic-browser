using BrowserAndFeatures2;
using Eli.WpfHost.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xilium.CefGlue.Client;

namespace BrowserPlugin2
{
    public class Plugin : PluginBase
    {
        FeatureCallage2 fc;

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

            GloableProfData.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            fc = new FeatureCallage2();
            return fc;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override List<Process> GetProjectsProcesses()
        {
            return ProcessManager.Instance.Processes;
        }

        public override void KillAllProcesses()
        {
            //if (fc != null)
            //{
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        fc.CloseAll();
            //    });
            //    ProcessManager.Instance.DisposeAllProcess();
            //}
        }

        public override void OnTabFocused()
        {
            //System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    if (BrowserSettimgs.SetSysDateEnabled)
            //    {
            //        try
            //        {
            //            TimeHelper.StartSetTimeAndZoneProcess(new DateAndTimeZone() { TimeZone = TimeZoneInfo.GetSystemTimeZones()[BrowserSettimgs.SITimeZone] });
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //        }
            //    }
            //    else
            //    {
            //        TimeHelper.SetOriginalTimeZonesFromFile();
            //    }
            //});
        }

        public override void RaiseMessageFromHost(string message)
        {
            //switch (message)
            //{
            //    case "REMINDERS_CHANGED":
            //        fc.SetRemindersCount();
            //        break;

            //    default:
            //        fc.GotScrennCords(message);
            //        break;
            //}
        }

        public override void SetBrowserPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            GloableProfData.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            //FoxInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
        }

        private void Fc_OnClickedReminders()
        {
            if (OnMessageFromPlugin != null) OnMessageFromPlugin("REMINDERS_CLICK");
        }



        private void Fc_OnRequestedScreenLocation()
        {
            if (OnMessageFromPlugin != null) OnMessageFromPlugin("SCREEN_SIZE");
        }
    }
}
