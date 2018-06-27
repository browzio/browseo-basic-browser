using Browser.Common.Models;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Browser.Common.ViewModels
{
    public class UserAgentVM : ViewModelBase
    {
        public event Action<string> OnSelectedUserAgentChange = delegate { };//useragent

        public ObservableCollection<UserAgent> UserAgentList { get; set; }

        public ICommand OnCommandFromView { get; set; }

        private string customeAgentText;
        public string CustomeAgentText
        {
            get { return customeAgentText; }
            set { customeAgentText = value; RaisePropertyChanged("CustomeAgentText"); }
        }


        public UserAgentVM()
        {
            UserAgentList = new ObservableCollection<UserAgent>();
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
        }

        private void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            if (param == null) return;
            try
            {
                switch (param)
                {
                    case "Custom":
                        if (CustomeAgentText.IsNullOrEmpty()) return;

                        OnSelectedUserAgentChange(CustomeAgentText);
                        break;

                    case "FromList":
                        foreach (var ua in UserAgentList)
                        {
                            if (ua.IsSelected)
                            {
                                OnSelectedUserAgentChange(ua.Agent);
                                break;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SetUpFFAgents()
        {
            //
            UserAgentList.Add(new UserAgent() { Version = "Firefox 52", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 47 win10", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 47 win7", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:47.0) Gecko/20100101 Firefox/47.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 45.0", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 40.1", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 36.0", Agent = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 31.0", Agent = "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 29.0", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:29.0) Gecko/20120101 Firefox/29.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 28.0", Agent = "Mozilla/5.0 (X11; OpenBSD amd64; rv:28.0) Gecko/20100101 Firefox/28.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 27.3", Agent = "Mozilla/5.0 (Windows NT 6.1; rv:27.3) Gecko/20130101 Firefox/27.3" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 27.0", Agent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64; rv:27.0) Gecko/20121011 Firefox/27.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 25.0", Agent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 24.0", Agent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 23.0", Agent = "Mozilla/5.0 (Windows NT 6.2; rv:22.0) Gecko/20130405 Firefox/23.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 22.0", Agent = "Mozilla/5.0 (Microsoft Windows NT 6.2.9200.0); rv:22.0) Gecko/20130405 Firefox/22.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 21.0.0", Agent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64; rv:21.0.0) Gecko/20121011 Firefox/21.0.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 20.0", Agent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64;) Gecko/20100101 Firefox/20.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 19.0", Agent = "Mozilla/5.0 (Windows NT 6.1; rv:6.0) Gecko/20100101 Firefox/19.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 18.0", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 17.0", Agent = "Mozilla/5.0 (X11; Ubuntu; Linux armv7l; rv:17.0) Gecko/20100101 Firefox/17.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 16.0", Agent = "Mozilla/5.0 (X11; NetBSD amd64; rv:16.0) Gecko/20121102 Firefox/16.0" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 15.0.1", Agent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; rv:15.0) Gecko/20121011 Firefox/15.0.1" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 14.0.1", Agent = "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20120403211507 Firefox/14.0.1" });
            UserAgentList.Add(new UserAgent() { Version = "Firefox 12.0", Agent = "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20120403211507 Firefox/12.0" });
            foreach (var agent in UserAgentList)
            {
                if(agent.Agent == BrowserSettimgs.UserAgentFF)
                {
                    agent.IsSelected = true;
                    break;
                }
            }

            CustomeAgentText = BrowserSettimgs.UserAgentFF;
        }

        public void SetUpChromeAgents()
        {
            UserAgentList.Add(new UserAgent() { Version = "Chrome 66 win10", Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.170 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 56 win10", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 51 win10", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 51 win7", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 49.0", Agent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 41.0.2228.0", Agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 41.0.2227.0", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2227.0 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 40.0.2214.93", Agent = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.93 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 37.0.2049.0", Agent = "Mozilla/5.0 (Windows NT 4.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 36.0.1985.67", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.67 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 35.0.3319.102", Agent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.3319.102 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 34.0.1866.237", Agent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1866.237 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 32.0.1667.0", Agent = "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 31.0.1650.16", Agent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.16 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 29.0.1547.62", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.62 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 28.0.1467.0", Agent = "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1467.0 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 27.0.1453.93", Agent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 24.0.1295.0", Agent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.15 (KHTML, like Gecko) Chrome/24.0.1295.0 Safari/537.15" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 23.0.1271.17", Agent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.17 Safari/537.11" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 22.0.1229.94", Agent = "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.94 Safari/537.4" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 20.0.1092.0", Agent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1092.0 Safari/536.6" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 19.0.1084.36", Agent = "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.36 Safari/536.5" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 18.0.1025.45", Agent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.45 Safari/535.19" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 17.0.963.66", Agent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.11 (KHTML, like Gecko) Chrome/17.0.963.66 Safari/535.11" });
            UserAgentList.Add(new UserAgent() { Version = "Chrome 16.0.912.75", Agent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/535.7 (KHTML, like Gecko) Chrome/16.0.912.75 Safari/535.7" });
            foreach (var agent in UserAgentList)
            {
                if (agent.Agent == BrowserSettimgs.UserAgentChrome)
                {
                    agent.IsSelected = true;
                    break;
                }
            }

            CustomeAgentText = BrowserSettimgs.UserAgentChrome;
        }
    }
}
