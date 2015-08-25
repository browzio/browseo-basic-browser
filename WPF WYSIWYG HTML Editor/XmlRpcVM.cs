using CookComputing.XmlRpc;
using Drupal7.Services;
using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using WordPressSharp;
using WordPressSharp.Models;
using WPF_WYSIWYG_HTML_Editor.Helpers;
using WPF_WYSIWYG_HTML_Editor.Models;
using WPF_WYSIWYG_HTML_Editor.XAML;

namespace WPF_WYSIWYG_HTML_Editor
{
    public class XmlRpcVM : INotifyPropertyChanged
    {
        public ICommand SettingsClicked { get; set; }

        private bool enableBtns;
        public bool EnableBtns
        {
            get { return enableBtns; }
            set
            {
                enableBtns = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("EnableBtns"));
                }
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        private string postTitle;
        public string PostTitle
        {
            get { return postTitle; }
            set
            {
                postTitle = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PostTitle"));
                }
            }
        }

        //settings window
        public bool IsWPChecked { get; set; }
        public bool IsDrupalChecked { get; set; }

        private ObservableCollection<SelectedProfile> cmbBoxWPList;
        public ObservableCollection<SelectedProfile> CmbBoxWPList
        {
            get { return cmbBoxWPList; }
            set { cmbBoxWPList = value; }
        }

        private ObservableCollection<SelectedProfile> cmbBoxDrupalList;
        public ObservableCollection<SelectedProfile> CmbBoxDrupalList
        {
            get { return cmbBoxDrupalList; }
            set { cmbBoxDrupalList = value; }
        }

        public XmlRpcVM()
        {
            SettingsClicked = new RelayCommand(OnSettingsClicked);

            CmbBoxWPList = new ObservableCollection<SelectedProfile>();
            CmbBoxDrupalList = new ObservableCollection<SelectedProfile>();

            Status = "Select platforms to pulbilsh to from settings.";
            EnableBtns = true;
        }

        private void OnSettingsClicked(object param)
        {
            SelectPlatformWindow slw = new SelectPlatformWindow();
            slw.DataContext = this;
            slw.ShowDialog();
            if (isAnySelected())
                Status = "Ready to publish.";
        }

        private bool isAnySelected()
        {
            bool didSelectSomeThing = false;
            if (IsDrupalChecked || IsWPChecked)
            {
                foreach (SelectedProfile item in CmbBoxWPList)
                {
                    if (IsWPChecked && item.IsSelected)
                    {
                        didSelectSomeThing = true;
                        break;
                    }
                }
                foreach (SelectedProfile item in CmbBoxDrupalList)
                {
                    if (IsDrupalChecked && item.IsSelected)
                    {
                        didSelectSomeThing = true;
                        break;
                    }
                }
            }
            return didSelectSomeThing;
        }

        public void OnPublishClick(string content)
        {
            if (!isAnySelected())
            {
                MessageBox.Show("Select platforms to publish to from settings.");
                return;
            }
            new Thread(() =>
            {
                EnableBtns = false;
                App.Current.Dispatcher.Invoke((Action)delegate 
                { 
                   Mouse.OverrideCursor = Cursors.Wait;
                });
                string errorString = "";
                string successString = "";
                try
                {
                    #region --wp--
                    if (IsWPChecked)
                    {
                        Status = "Gathering wordpress profiles.";

                        foreach (SelectedProfile prof in CmbBoxWPList)
                        {
                            if (!prof.IsSelected) continue;


                            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(prof.Path);

                            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
                            {
                                MessageBox.Show("Website in profile data cannot be empty. " + profile.ProfileName);
                                continue;
                            }
                            Status = "Posting to " + profile.WebAddress + ".";
                            DateTime publishdt = DateTime.Now;
                            try
                            {
                                publishdt = GetNistTime(profile.ProxyIP,profile.ProxyPort,profile.ProxyUsername,profile.ProxyPassword);
                            }
                            catch { publishdt = DateTime.Now; }
                            var post = new Post
                            {
                                PostType = "post", // "post" or "page"
                                Title = PostTitle,
                                Content = content,
                                PublishDateTime = publishdt,
                                Status = "publish" // "draft" or "publish"
                            };

                            using (var client = new WordPressClient(new WordPressSiteConfig
                            {
                                BaseUrl = profile.WebAddress,
                                BlogId = 1,
                                Username = profile.Username,
                                Password = profile.Password,
                            }, profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword))
                            {
                                try
                                {
                                    var id = client.NewPost(post);
                                }
                                catch
                                {
                                    errorString += "Unable to post to " + profile.WebAddress + Environment.NewLine;
                                    continue;
                                }
                            }

                             successString += "Post succesfull to "+profile.WebAddress + Environment.NewLine;
                        }
                    }
                    #endregion

                    #region --drupal--
                    if (IsDrupalChecked)
                    {
                        Status = "Gathering drupal profiles.";
                        foreach (SelectedProfile prof in CmbBoxDrupalList)
                        {
                            if (!prof.IsSelected) continue;

                            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(prof.Path);
                            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
                            {
                                MessageBox.Show("Website in profile data cannot be empty. " + profile.ProfileName);
                                continue;
                            }
                            Status = "Posting to " + profile.WebAddress + ".";

                            string url = profile.WebAddress;
                            if (url[url.Length - 1] != '/')
                                url += '/';
                            url += "xmlrpc.php";

                            DrupalServices d = new Drupal7.Services.DrupalServices(url, profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword);
                            bool isin = d.Login(profile.Username, profile.Password);
                            if(!isin)
                            {
                                errorString += "Was unable to authenticate " + profile.WebAddress + Environment.NewLine;
                                continue;
                            }

                            XmlRpcStruct postStruct = new XmlRpcStruct();
                            postStruct.Add("type", "article");
                            postStruct.Add("title", PostTitle);

                            XmlRpcStruct postBodyStructParams = new XmlRpcStruct();
                            postBodyStructParams.Add("format", "full_html");
                            postBodyStructParams.Add("value", content);


                            XmlRpcStruct[] postBodyStructParamsArr = new XmlRpcStruct[1];
                            postBodyStructParamsArr[0] = postBodyStructParams;

                            XmlRpcStruct postBodyStruct = new XmlRpcStruct();
                            postBodyStruct.Add("und", postBodyStructParamsArr);

                            postStruct.Add("body", postBodyStruct);

                            XmlRpcStruct s = d.NodeCreate(postStruct);
                            if (s == null)
                            {
                                errorString += "Was Unable to post to " + profile.WebAddress + Environment.NewLine;
                            }
                            else
                            {
                                successString += "Post succesfull to " + profile.WebAddress + Environment.NewLine;
                            }

                            d.Logout();
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Whoops something went wrong: " + ex.Message);
                }

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Mouse.OverrideCursor = null;
                });

                if (successString != "")
                    MessageBox.Show(successString);
                if (errorString != "")
                    MessageBox.Show(errorString);

                Status = "Ready to publish.";
                EnableBtns = true;
            }).Start();
        }

        public static DateTime GetNistTime(string ip,string port,string username,string pass)
        {
            DateTime dateTime = DateTime.MinValue;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nist.time.gov/actualtime.cgi?lzbc=siqm9b");
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore); //No caching

            if (!string.IsNullOrEmpty(ip) && !string.IsNullOrWhiteSpace(ip) &&
                !string.IsNullOrEmpty(port) && !string.IsNullOrWhiteSpace(port))
                request.Proxy = new WebProxy(ip, Convert.ToInt32(port)); // You may or may not need this
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrWhiteSpace(username) &&
               !string.IsNullOrEmpty(pass) && !string.IsNullOrWhiteSpace(pass))
                request.Proxy.Credentials = new NetworkCredential(username, pass);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader stream = new StreamReader(response.GetResponseStream());
                string html = stream.ReadToEnd();//<timestamp time=\"1395772696469995\" delay=\"1395772696469995\"/>
                string time = Regex.Match(html, @"(?<=\btime="")[^""]*").Value;
                double milliseconds = Convert.ToInt64(time) / 1000.0;
                dateTime = new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);
            }

            return dateTime;
        }

        public void SetProfileDate(PersonData profile)
        {
            List<KeyValuePair<string, string>> directoryValues = MyFilesDatabase.GetSubProjectsFolders(profile.ProjectDIr, profile.ProjectName);
            foreach (KeyValuePair<string, string> prof in directoryValues)
            {
                CmbBoxWPList.Add(new SelectedProfile() { ProfileName = prof.Key, Path = prof.Value });
                CmbBoxDrupalList.Add(new SelectedProfile() { ProfileName = prof.Key, Path = prof.Value });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
