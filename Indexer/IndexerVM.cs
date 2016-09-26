using LocalHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Indexer
{
    public class IndexerVM : INotifyPropertyChanged
    {
        public ICommand BtnOkClicked { get; set; }

        public string InputedLinks { get; set; }

        private bool btnOkEnabled;
        public bool BtnOkEnabled
        {
            get { return btnOkEnabled; }
            set { btnOkEnabled = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("btnOkEnabled"));
            }
        }

        private string response;
        public string Response
        {
            get { return response; }
            set { response = value;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Response"));
            }
        }
        private Visibility responseVisible;
        public Visibility ResponseVisible
        {
            get { return responseVisible; }
            set
            {
                responseVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ResponseVisible"));
            }
        }

        private bool backlinksindexerChecked;
        public bool BacklinksindexerChecked
        {
            get { return backlinksindexerChecked; }
            set
            {
                backlinksindexerChecked = value;
                if (value)
                    BacklinksindexerVisible = Visibility.Visible;
                else
                    BacklinksindexerVisible = Visibility.Collapsed;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BacklinksindexerChecked"));
            }
        }
        private Visibility backlinksindexerVisible;
        public Visibility BacklinksindexerVisible
        {
            get { return backlinksindexerVisible; }
            set
            {
                backlinksindexerVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BacklinksindexerVisible"));
            }
        }
        public string BacklinksindexerAPIKey { get; set; }
        //
        private bool linkindexerChecked;
        public bool LinkindexerChecked
        {
            get { return linkindexerChecked; }
            set
            {
                linkindexerChecked = value;
                if (value)
                    LinkindexerVisible = Visibility.Visible;
                else
                    LinkindexerVisible = Visibility.Collapsed;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LinkindexerChecked"));
            }
        }
        private Visibility linkindexerVisible;
        public Visibility LinkindexerVisible
        {
            get { return linkindexerVisible; }
            set
            {
                linkindexerVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LinkindexerVisible"));
            }
        }
        public string LinkindexerAPIKey { get; set; }
        public string LinkIndexerCampaign { get; set; }
        public int SIDripFeed { get; set; }
        private ObservableCollection<string> dripFeeds;
        public ObservableCollection<string> DripFeeds
        {
            get { return dripFeeds; }
            set { dripFeeds = value; }
        }


        private bool crazyindexerChecked;
        public bool CrazyindexerChecked
        {
            get { return crazyindexerChecked; }
            set
            {
                crazyindexerChecked = value;
                if (value)
                    CrazyindexerVisible = Visibility.Visible;
                else
                    CrazyindexerVisible = Visibility.Collapsed;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CrazyindexerChecked"));
            }
        }
        private Visibility crazyindexerVisible;
        public Visibility CrazyindexerVisible
        {
            get { return crazyindexerVisible; }
            set
            {
                crazyindexerVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CrazyindexerVisible"));
            }
        }
        public string CrazyindexerAPIKey { get; set; }

        private bool instantlinkindexerChecked;
        public bool InstantlinkindexerChecked
        {
            get { return instantlinkindexerChecked; }
            set
            {
                instantlinkindexerChecked = value;
                if (value)
                    InstantlinkindexerVisible = Visibility.Visible;
                else
                    InstantlinkindexerVisible = Visibility.Collapsed;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("InstantlinkindexerChecked"));
            }
        }
        private Visibility instantlinkindexerVisible;
        public Visibility InstantlinkindexerVisible
        {
            get { return instantlinkindexerVisible; }
            set
            {
                instantlinkindexerVisible = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("InstantlinkindexerVisible"));
            }
        }
        public string InstantlinkindexerAPIKey { get; set; }

        public IndexerVM()
        {
           // Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_OpenedIndeser + "");

            BtnOkClicked = new RelayCommand(OnBtnOkClicked);

            BtnOkEnabled = true;
            ResponseVisible = Visibility.Collapsed;
            BacklinksindexerChecked = CrazyindexerChecked = InstantlinkindexerChecked = LinkindexerChecked = false;

            DripFeeds = new ObservableCollection<string>();
            DripFeeds.Add("INSTANT");
            DripFeeds.Add("3 days");
            DripFeeds.Add("5 days");
            DripFeeds.Add("7 days");
            DripFeeds.Add("15 days");
            DripFeeds.Add("30 days");

            SIDripFeed = 0;

            getApiKeys();
        }

        private void OnBtnOkClicked(object obj)
        {
            try
            {
                if (string.IsNullOrEmpty(InputedLinks) || string.IsNullOrWhiteSpace(InputedLinks)) return;
                ResponseVisible = Visibility.Collapsed;
                Response = "";
                string[] links = InputedLinks.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                string backlinksindexerurl = "http://backlinksindexer.com/api.php?key=" + BacklinksindexerAPIKey + "&urls=" + InputedLinks;
                string crazyUrl = "http://crazyindexer.com/tci-api/tci-api.php";
                string linkIndexerURl = "http://linkindexr.info/api.php";
                string crazyLinks = "";
                string instantUrl = "http://www.instantlinkindexer.com/api.php";

                foreach (string link in links)
                {
                    crazyLinks += link + ",";
                }
                crazyLinks = crazyLinks.TrimEnd(',');

                new Thread(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                    });

                    BtnOkEnabled = false;

                    WebClient client = new WebClient();
                    if (BacklinksindexerChecked)
                    {
                        try
                        {
                            Response += "Backlinksindexer Response = " + client.DownloadString(backlinksindexerurl) + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            Response += "Backlinksindexer Response = failed"+ Environment.NewLine;
                            MessageBox.Show("Backlinksindexer Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (LinkindexerChecked)
                    {
                        try
                        {
                            string linkLinks = "";
                            foreach (string link in links)
                            {
                                linkLinks += link + "|";
                            }
                            linkLinks = linkLinks.Remove(linkLinks.Length - 1);
                            var request = (HttpWebRequest)WebRequest.Create(linkIndexerURl);

                            var postData = "apikey=" + WebUtility.UrlEncode(LinkindexerAPIKey);
                            postData += "&campaign=" + WebUtility.UrlEncode(LinkIndexerCampaign);
                            if (SIDripFeed>=1)
                                postData += "&campaign=" + WebUtility.UrlEncode(DripFeeds[SIDripFeed].Replace(" ", "_"));
                            postData += "&urls=" + crazyLinks;
                            var data = Encoding.ASCII.GetBytes(postData);

                            request.Method = "POST";
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.ContentLength = data.Length;
                            using (var stream = request.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }

                            var response = (HttpWebResponse)request.GetResponse();

                            Response += "Link Indexer Response = " + response.StatusCode + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            Response += "Link Indexer Response = failed" + Environment.NewLine;
                            MessageBox.Show("Link Indexer Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (CrazyindexerChecked)
                    {
                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create(crazyUrl);

                            var postData = "apikey=" + CrazyindexerAPIKey;
                            postData += "&links=" + crazyLinks;
                            var data = Encoding.ASCII.GetBytes(postData);

                            request.Method = "POST";
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.ContentLength = data.Length;
                            using (var stream = request.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }

                            var response = (HttpWebResponse)request.GetResponse();

                            Response += "Crazyindexer Response = " + response.StatusCode + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            Response += "Crazyindexer Response = failed" + Environment.NewLine;
                            MessageBox.Show("Crazyindexer Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (InstantlinkindexerChecked)
                    {
                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create(instantUrl);

                            var postData = "apikey=" + InstantlinkindexerAPIKey;
                            postData += "&urls=" + InputedLinks;
                            var data = Encoding.ASCII.GetBytes(postData);

                            request.Method = "POST";
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.ContentLength = data.Length;
                            using (var stream = request.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }

                            var response = (HttpWebResponse)request.GetResponse();

                            Response += "Instantlinkindexer Response = " + response.StatusCode + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            Response += "Instantlinkindexer Response = failed" + Environment.NewLine;
                            MessageBox.Show("Crazyindexer Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    client.Dispose();

                    //Organiser.Common.Classes.UsageTracker.AddTraceCookie(UsageTracker.Usage_Type_IndexedLinks + " " + InputedLinks + " Response: " + Response);

                    BtnOkEnabled = true;
                    ResponseVisible = Visibility.Visible;

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        Mouse.OverrideCursor = null;
                    });
                    saveApiKeys();
                }).Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Indexing Faild: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveApiKeys()
        {
            string apiDirs = Path.Combine(GetBaseDir(), "IndexerApi");
            if (!Directory.Exists(apiDirs)) Directory.CreateDirectory(apiDirs);

            string filePath = Path.Combine(GetBaseDir(), "IndexerApi", "keys.ini");
            try
            {
                IniFile fileWrighter = new IniFile(filePath);
                fileWrighter.IniWriteValue("Data", "BacklinksindexerAPIKey", BacklinksindexerAPIKey);
                fileWrighter.IniWriteValue("Data", "CrazyindexerAPIKey", CrazyindexerAPIKey);
                fileWrighter.IniWriteValue("Data", "InstantlinkindexerAPIKey", InstantlinkindexerAPIKey);
                fileWrighter.IniWriteValue("Data", "LinkindexerAPIKey", LinkindexerAPIKey);
            }
            catch { MessageBox.Show("Keys not saved."); }
        }

        private void getApiKeys()
        {
            string filePath = Path.Combine(GetBaseDir(), "IndexerApi", "keys.ini");
            if (File.Exists(filePath))
            {
                    IniFile ini = new IniFile(filePath);
                    try
                    {
                        BacklinksindexerAPIKey = ini.IniReadValue("Data", "BacklinksindexerAPIKey");
                        CrazyindexerAPIKey = ini.IniReadValue("Data", "CrazyindexerAPIKey");
                        InstantlinkindexerAPIKey = ini.IniReadValue("Data", "InstantlinkindexerAPIKey");
                        LinkindexerAPIKey = ini.IniReadValue("Data", "LinkindexerAPIKey");
                    }
                    catch { }
            }
        }

        private string GetBaseDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }

        private class IniFile
        {
            public string path;

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            /// <summary>
            /// INIFile Constructor.
            /// </summary>
            /// <param name="INIPath"></param>
            public IniFile(string INIPath)
            {
                path = INIPath;
            }
            /// <summary>
            /// Write Data to the INI File
            /// </summary>
            /// <param name="Section"></param>
            /// Section name
            /// <param name="Key"></param>
            /// Key Name
            /// <param name="Value"></param>
            /// Value Name
            public void IniWriteValue(string Section, string Key, string Value)
            {
                var vals = WritePrivateProfileString(Section, Key, Value, this.path);
                if (vals == 0)
                {
                    if (!File.Exists(this.path))
                    {
                        File.AppendAllText(path, "[" + Section + "]" + Environment.NewLine);
                    }
                    string fileText = File.ReadAllText(this.path);
                    if (!fileText.Contains(Key))
                    {
                        File.AppendAllText(path, Key + "=" + Value + Environment.NewLine);
                    }
                    else
                    {
                        string[] lines = fileText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var user in lines)
                        {
                            string fline = user;
                            if (fline.Split(new string[] { "=" }, StringSplitOptions.None)[0].Trim() == Key.Trim())
                            {
                                fileText = fileText.Replace(fline, Key + "=" + Value);
                                File.WriteAllText(this.path, fileText);
                                return;
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Read Data Value From the Ini File
            /// </summary>
            /// <param name="Section"></param>
            /// <param name="Key"></param>
            /// <param name="Path"></param>
            /// <returns></returns>
            public string IniReadValue(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
                if (i == 0)
                {
                    foreach (var line in File.ReadAllLines(this.path))
                    {
                        if (!line.Contains("=")) continue;
                        string[] keyvale = line.Split('=');
                        string sectionVal = keyvale[1], sectionkey = keyvale[0];
                        if (sectionkey == Key)
                        {
                            return sectionVal;
                        }
                    }
                }
                return temp.ToString();

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
