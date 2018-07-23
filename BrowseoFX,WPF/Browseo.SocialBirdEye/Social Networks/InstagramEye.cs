using Browseo.SocialBirdEye.ViewModels;
using BrowseoFX_WPF.Browseo.SocialBirdEye.Models.Instagram;
using BrowseoFX_WPF.Core;
using Gecko;
using Gecko.Interfaces;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Organiser.Common.Classes.MyFilesDatabase;

namespace BrowseoFX_WPF.Browseo.SocialBirdEye.Social_Networks
{

    //https://developers.facebook.com/docs/instagram-api/getting-started/
    public class InstagramEye : ViewModelBase
    {
        //public string AccessToken = "EAADqmMyVg3kBADQqz2JT8bQMc5Sj0JvQXCdWCDDLRTn8x2Tl9gfhn0uLJ2LZAsl1DBqr1gZA4m8g4J1djpzNoPB0n1EYiO6QL9X3AFZBmUU7KjuPKvfPMmsC8Ct3VrWXK9ZCZB4BEH1nNxbixx9x7aoWDnKfUJdkoHTRjNjXaZAmui0CoEJg3yFLfmVtNVxM7HbcHo3k5fVwZDZD";
        //public string InstagramBuisnessPageID = "17841400288180705";
        //FacebookAppId: 257941988279161

        public string SaveFile { get { return Path.Combine(BirdsEyeDashboardViewModel.SaveDirectory, "Instagram"); } }

        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<ValuesData> InsightsValuesViews { get; set; }
        public ObservableCollection<ValuesData> InsightsValuesViewsUnique { get; set; }

        private InstagramUserInfo instagramUserInfo;
        public InstagramUserInfo InstagramUserInfo
        {
            get { return instagramUserInfo; }
            set { instagramUserInfo = value; NotifyOfPropertyChange(); }
        }

        private string facebookAppId;
        public string FacebookAppId
        {
            get { return facebookAppId; }
            set { facebookAppId = value; NotifyOfPropertyChange(); }
        }

        private string instagramBuisnessPageID;
        public string InstagramBuisnessPageID
        {
            get { return instagramBuisnessPageID; }
            set { instagramBuisnessPageID = value; NotifyOfPropertyChange(); }
        }

        public string AccessToken { get; set; }

        public InstagramEye()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raisev);

            InsightsValuesViews = new ObservableCollection<ValuesData>();
            InsightsValuesViewsUnique = new ObservableCollection<ValuesData>();

            ReadTokenKeys();
        }

        private void OnCommandFromView_Raisev(object obj)
        {
            var param = obj as string;
            if (param == null) return;

            switch (param)
            {
                case "SaveFile":
                    SaveTokenKeys();
                    break;

                default:
                    break;
            }
        }

        private void ReadTokenKeys()
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) return;
                if (!File.Exists(SaveFile)) return;

                var fileContents = File.ReadAllLines(SaveFile);
                if (fileContents.Length < 2) return;

                FacebookAppId = fileContents[0];
                InstagramBuisnessPageID = fileContents[1];
            });
        }

        private void SaveTokenKeys()
        {
            Task.Run(() =>
            {
                if (!Directory.Exists(BirdsEyeDashboardViewModel.SaveDirectory)) Directory.CreateDirectory(BirdsEyeDashboardViewModel.SaveDirectory);

                File.WriteAllLines(SaveFile, new string[] { FacebookAppId, InstagramBuisnessPageID});
            });
        }

        public void GetAccessToken()
        {
            BrowseoFXManager.Instance.GloableWebView.Navigated += GloableWebView_Navigated;
            BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate(Social.FACEBOOK_GRAPH_LINK+ FacebookAppId);
        }
        private void GloableWebView_Navigated(object sender, Gecko.GeckoNavigatedEventArgs e)
        {
            if (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.ReadyState == "loading") return;
            BrowseoFXManager.Instance.GloableWebView.Navigated -= GloableWebView_Navigated;
            string source = (BrowseoFXManager.Instance.TabbrowserHandler.SelectedContentDocument.DocumentElement as Gecko.DOM.HTML.GeckoHTMLHtmlElement).OuterHtml;

            AccessToken = source.Trim().Split(new string[] { "placeholder=\"Paste in an existing Access Token or click &quot;Get User Access Token" }, StringSplitOptions.None)[1];
            AccessToken = AccessToken.Split(new string[] { "value=" }, StringSplitOptions.None)[1];
            AccessToken = AccessToken.Remove(AccessToken.IndexOf(@">"));
            AccessToken = AccessToken.Replace("\"", "");
            AccessToken = AccessToken.Replace(" type=text", "");
        }

        //week: impressions, reach, 
        //day: email_contacts, phone_call_clicks, text_message_clicks, get_directions_clicks, website_clicks, profile_views, audience_locale, audience_country, audience_city, online_followers",

        //17841400288180705?fields=followers_count,username,follows_count,media_count,insights.metric(impressions,reach).period(week){description,name,title,values,id}

        public async Task<InstagramUserInfo> GetInstagramInfo()
        {
            var requestUrl = "https://graph.facebook.com/v3.0/" + InstagramBuisnessPageID + "?fields=followers_count,username,follows_count,media_count,insights.metric(impressions,reach).period(week){description,name,title,values,id}&access_token="+ AccessToken;
            InstagramUserInfo = await InstagramEyeHttpRequests.GetInstaUserInfo(requestUrl);

            InsightsValuesViews.Clear();
            InsightsValuesViewsUnique.Clear();
            if(InstagramUserInfo.insights != null && 
                InstagramUserInfo.insights.data != null &&
                InstagramUserInfo.insights.data.Length == 2)
            {
                if(InstagramUserInfo.insights.data[0].values != null)
                    foreach (var item in InstagramUserInfo.insights.data[0].values)
                    {
                        InsightsValuesViews.Add(item);
                    }

                if (InstagramUserInfo.insights.data[0].values != null)
                    foreach (var item in InstagramUserInfo.insights.data[1].values)
                    {
                        InsightsValuesViewsUnique.Add(item);
                    }
            }

            return InstagramUserInfo;
        }
    }

    public class InstagramEyeHttpRequests
    {
        public static async Task<InstagramUserInfo> GetInstaUserInfo(string fullOgUrl)
        {
            nsICookieService CookieMan = Xpcom.GetService<nsICookieService>("@mozilla.org/cookieService;1");
            var cookies = Xpcom.QueryInterface<nsICookieService>(CookieMan);
            Marshal.ReleaseComObject(CookieMan);

            var uri = IOService.GetService().CreateNsIUri(fullOgUrl);
            string cookie = cookies.GetCookieString(uri, null); //i've implemented my own cookie service

            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Cookie, cookie);
            webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserSettimgs.UserAgent_CurrentFFBuild);
            webClient.Proxy = MyFilesDatabase.GetRequestsProxy();

            string json = await Task.Run(() => { return webClient.DownloadString(fullOgUrl); });
            return Newtonsoft.Json.JsonConvert.DeserializeObject<InstagramUserInfo>(json);
        }
    }
}
