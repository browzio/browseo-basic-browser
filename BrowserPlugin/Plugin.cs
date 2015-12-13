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

namespace BrowserPlugin
{
    public class Plugin : PluginBase
    {
        FeatureCallage fc;

        public override FrameworkElement CreateControl(bool cbAllowProspector, bool cbAllowRSS, bool cbAllowPBN, bool cbAllowFeedMash, bool cbAllowIndexer, bool cbYoutube, bool canSeeProxyData, bool hasKK, int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            //fc.SetPersonData();
            //fc = new FeatureCallage(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            //fc.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);

            BrowserInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
            fc = new FeatureCallage();
            fc.SetPermissions(cbAllowProspector, cbAllowRSS, cbAllowPBN, cbAllowFeedMash, cbAllowIndexer, cbYoutube, canSeeProxyData, hasKK);
            return fc;
        }

        public override void Dispose()
        {
            if (fc != null)
                fc.CloseAll();
            base.Dispose();
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

        public override void SetBrowserPersonData(int birthdayYear, string children, string city, int cmbSelectedIndexDay, int cmbSelectedIndexMonth, int cmbSelectedIndexSex, string country, string dir, string email, string filePath, string firstName, bool inMonney, bool inPBNVault, string lastName, string notes, string password, string phoneNumber, string profileName, string projectDir, string projectName, string proxyIP, string proxyPassword, string proxyPort, string proxyUsername, int sIPBNType, string state, string street, string username, string webAddress, string zip)
        {
            BrowserInit.SetPersonData(birthdayYear, children, city, cmbSelectedIndexDay, cmbSelectedIndexMonth, cmbSelectedIndexSex, country, dir, email, filePath, firstName, inMonney, inPBNVault, lastName, notes, password, phoneNumber, profileName, projectDir, projectName, proxyIP, proxyPassword, proxyPort, proxyUsername, sIPBNType, state, street, username, webAddress, zip);
        }
    }
}
