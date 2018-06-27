using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Core.DataAccess;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowserAndFeatures2
{
    public enum AffiliateType
    {
        None,
        SeoWebGeek
    }
    /// <summary>
    /// Interaction logic for FeatureCallage2.xaml
    /// </summary>
    public partial class FeatureCallage2 : UserControl, IListenToFXManagerFC
    {
        private AffiliateType affiliateType = AffiliateType.SeoWebGeek;

        public FeatureCallage2()
        {
            InitializeComponent();

            MyFilesDatabase.GetSites();

            BrowseoFXManager.Instance.ListenToFXManagerFC = this;
            Loaded += FeatureCallage2_Loaded;
        }

        public static void SetPersonData()
        {
            BrowseoFXManager.Instance.PanelUIHandler.IsEnabledForKK = true;
            string path = System.IO.Path.Combine(MyFilesDatabase.GetBaseDir(), "Temp");
            string sitesFilePath = System.IO.Path.Combine(File.ReadAllText(path + "\\info.txt"), "ProjectData.ini");
            //string sitesFilePath = System.IO.Path.Combine(@"C:\Users\eli\AppData\Local\RAWSocialOrganizer\Projects\worpress", "ProjectData.ini");
            //string sitesFilePath = System.IO.Path.Combine(@"C:\Users\eli\AppData\Local\RAWSocialOrganizer\Projects\microsoft rename", "ProjectData.ini");
            IniFile ini = new IniFile(sitesFilePath);
            PersonData profile = new PersonData();
            try
            {
                profile.ProjectName = ini.IniReadValue("Data", "ProjectName");
                profile.ProfileName = ini.IniReadValue("Data", "ProfileName");
                profile.FirstName = ini.IniReadValue("Data", "FirstName");
                profile.LastName = ini.IniReadValue("Data", "LastName");
                profile.Email = ini.IniReadValue("Data", "Email");
                profile.Password = ini.IniReadValue("Data", "Password");
                profile.Username = ini.IniReadValue("Data", "Username");
                profile.ProxyIP = ini.IniReadValue("Data", "ProxyIP");
                profile.ProxyPort = ini.IniReadValue("Data", "ProxyPort");
                profile.ProxyUsername = ini.IniReadValue("Data", "ProxyUsername");
                profile.ProxyPassword = ini.IniReadValue("Data", "ProxyPassword");
                profile.PhoneNumber = ini.IniReadValue("Data", "PhoneNumber");
                profile.Street = ini.IniReadValue("Data", "Street");
                profile.City = ini.IniReadValue("Data", "City");
                profile.State = ini.IniReadValue("Data", "State");
                profile.Zip = ini.IniReadValue("Data", "Zip");
                profile.Country = ini.IniReadValue("Data", "Country");
                profile.WebAddress = ini.IniReadValue("Data", "WebAddress");
                profile.Notes = ini.IniReadValue("Data", "Notes");
                try
                {
                    profile.CmbSelectedIndexSex = Convert.ToInt32(ini.IniReadValue("Data", "Sex"));
                    profile.CmbSelectedIndexDay = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayDay"));
                    profile.CmbSelectedIndexMonth = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayMonth"));
                }
                catch { }
                profile.ProjectDir = sitesFilePath.Replace("\\ProjectData.ini", "");
                try
                {
                    profile.BirthdayYear = Convert.ToInt32(ini.IniReadValue("Data", "BirthdayYear"));
                }
                catch { }
            }
            catch { }
            GloableProfData.PData = profile;
            // BrowserLibraryInit.Instance.PlatformInitialize(profile);
            MyFilesDatabase.SetUpImacroProfileInfo();
            //FoxInit.Init(profile);
            // Initializer.Init(profile);
            //browser.OpenTabsFromPastSessions();
        }

        public void OnCurateToPBN(string html, string uri)
        {
        }

        public void OnSentForSeo(string sitename, string url)
        {
        }

        private void tbPreviewLink_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            if (tb == null) return;

            Process.Start(tb.Text);
        }

        string chromeUrl = "https://browz.io/chrome-lite";
        string prospectorUrl = "https://browz.io/prospector-lite";
        string rssUrl = "https://browz.io/rss-lite";
        string publisHubUrl = "https://browz.io/publishub-lite";
        string IAUrl = "https://browz.io/ia-lite";
        string seoWebGeek = "https://www.seowebgeek.com/lite";
        private void FeatureCallage2_Loaded(object sender, RoutedEventArgs e)
        {
            tbLinkChrome.Text = chromeUrl;
            tbLinkProspector.Text = prospectorUrl;
            tbLinkRss.Text = rssUrl;
            tbPublishHub.Text = publisHubUrl;
            tbIA.Text = IAUrl;
            switch (affiliateType)
            {
                case AffiliateType.SeoWebGeek:
                    chromeUrl = prospectorUrl = rssUrl = publisHubUrl = IAUrl = seoWebGeek;
                    tbLinkChrome.Text = tbLinkProspector.Text = tbLinkRss.Text = tbPublishHub.Text = tbIA.Text = seoWebGeek;
                    break;

                default:
                    break;
            }
        }

        bool? launchedChrome = false;
        bool? launchedProspector = false;
        bool? launchedRSS = false;
        bool? launchedPublishHub = false;
        bool? launchedIA = false;
        private void tbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbControl.SelectedIndex == 1 && launchedChrome == false)
            {
                //GrEvIwmJrwY
                ChromeVideoPopup.Navigate(chromeUrl, "https://www.youtube.com/embed/GrEvIwmJrwY?autoplay=true");
            }
            else if (tbControl.SelectedIndex == 2 && launchedProspector == false)
            {
                //SpnF9Cjx2M4
                ProspectorVideoPopup.Navigate(prospectorUrl, "https://www.youtube.com/embed/SpnF9Cjx2M4?autoplay=true");
            }
            else if (tbControl.SelectedIndex == 3 && launchedRSS == false)
            {
                //nA1EAvFUhD4
                RSSVideoPopup.Navigate(rssUrl, "https://www.youtube.com/embed/nA1EAvFUhD4?autoplay=true");
            }
            else if (tbControl.SelectedIndex == 4 && launchedPublishHub == false)
            {
                //9ax3_dtYzuk
                PublishHUBVideoPopup.Navigate(publisHubUrl, "https://www.youtube.com/embed/9ax3_dtYzuk?autoplay=true");
            }
            else if (tbControl.SelectedIndex == 5 && launchedIA == false)
            {
                //Y5hfVUKffP0
                IAVideoPopup.Navigate(IAUrl, "https://www.youtube.com/embed/Y5hfVUKffP0?autoplay=true");
            }
        }

        private void ChromeVideoPopup_BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //    launchedChrome = ChromeVideoPopup.cbDontShow.IsChecked;
            //    launchedProspector = ProspectorVideoPopup.cbDontShow.IsChecked;
            //    launchedRSS = RSSVideoPopup.cbDontShow.IsChecked;
            //    launchedPublishHub = PublishHUBVideoPopup.cbDontShow.IsChecked;
            //    launchedIA = IAVideoPopup.cbDontShow.IsChecked;


            ChromeVideoPopup.Visibility = Visibility.Collapsed;
            ProspectorVideoPopup.Visibility = Visibility.Collapsed;
            RSSVideoPopup.Visibility = Visibility.Collapsed;
            PublishHUBVideoPopup.Visibility = Visibility.Collapsed;
            IAVideoPopup.Visibility = Visibility.Collapsed;
        }
    }
}
