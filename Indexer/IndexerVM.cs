using Helpers;
using Organiser.Common.Classes;
using PData.FilesReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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
            Organiser.Common.Classes.UsageTracker.AddTraceCookie("Indexer Opened");

            BtnOkClicked = new RelayCommand(OnBtnOkClicked);

            BtnOkEnabled = true;
            ResponseVisible = Visibility.Collapsed;
            BacklinksindexerChecked = CrazyindexerChecked = InstantlinkindexerChecked = false;

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
                string crazyUrl = "http://api.crazyindexer.com/tci-api.php";
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

                    Organiser.Common.Classes.UsageTracker.AddTraceCookie("Indexed Links " + InputedLinks + " Response: " + Response);

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
            string apiDirs = Path.Combine(MyFilesDatabase.GetBaseDir(), "IndexerApi");
            if (!Directory.Exists(apiDirs)) Directory.CreateDirectory(apiDirs);

            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "IndexerApi", "keys.ini");
            try
            {
                IniFile fileWrighter = new IniFile(filePath);
                fileWrighter.IniWriteValue("Data", "BacklinksindexerAPIKey", BacklinksindexerAPIKey);
                fileWrighter.IniWriteValue("Data", "CrazyindexerAPIKey", CrazyindexerAPIKey);
                fileWrighter.IniWriteValue("Data", "InstantlinkindexerAPIKey", InstantlinkindexerAPIKey);
            }
            catch { MessageBox.Show("Keys not saved."); }
        }

        private void getApiKeys()
        {
            string filePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "IndexerApi", "keys.ini");
            if (File.Exists(filePath))
            {
                    IniFile ini = new IniFile(filePath);
                    try
                    {
                        BacklinksindexerAPIKey = ini.IniReadValue("Data", "BacklinksindexerAPIKey");
                        CrazyindexerAPIKey = ini.IniReadValue("Data", "CrazyindexerAPIKey");
                        InstantlinkindexerAPIKey = ini.IniReadValue("Data", "InstantlinkindexerAPIKey");
                    }
                    catch { }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
