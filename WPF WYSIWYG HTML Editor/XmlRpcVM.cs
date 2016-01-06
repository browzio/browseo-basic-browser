using CookComputing.XmlRpc;
using Drupal7.Services;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
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
using mshtml;
using WPF_WYSIWYG_HTML_Editor.MVVM;

namespace WPF_WYSIWYG_HTML_Editor
{
    public class XmlRpcVM : INotifyPropertyChanged
    {
        public ICommand SettingsClicked { get; set; }
        //RefreshPBNVault
        public ICommand RefreshPBNVault { get; set; }
        //VautContextMenu
        public ICommand VautContextMenu { get; set; }

        //TabsVisible
        private Visibility tabsVisible;
        public Visibility TabsVisible
        {
            get { return tabsVisible; }
            set
            {
                tabsVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TabsVisible"));
                }
            }
        }

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
        //ProgressBarStart
        private bool progressBarStart;
        public bool ProgressBarStart
        {
            get { return progressBarStart; }
            set
            {
                progressBarStart = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgressBarStart"));
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
        public bool IsSpinChecked { get; set; }

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

        //SavedPBNProjects
        private ObservableCollection<PBNProject> savedPBNProjects;
        public ObservableCollection<PBNProject> SavedPBNProjects
        {
            get { return savedPBNProjects; }
            set { savedPBNProjects = value; }
        }

        //SavedMoney
        private ObservableCollection<PBNProject> savedMoneyProjects;
        public ObservableCollection<PBNProject> SavedMoneyProjects
        {
            get { return savedMoneyProjects; }
            set { savedMoneyProjects = value; }
        }

        //Tabs
        private ObservableCollection<SpinningVM> tabs;
        public ObservableCollection<SpinningVM> Tabs
        {
            get { return tabs; }
            set { tabs = value;
            }
        }
        public bool UseSpunArticlesChecked { get; set; }

        private bool autoSpinChecked;
        public bool AutoSpinChecked
        {
            get { return autoSpinChecked; }
            set
            {
                autoSpinChecked = value;
                if (value)
                    DPtimesToSPinVisible = Visibility.Visible;
                else
                    DPtimesToSPinVisible = Visibility.Collapsed;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AutoSpinChecked"));
                }
            }
        }
        //DPtimesToSPinVisible
        private Visibility dPtimesToSPinVisible;
        public Visibility DPtimesToSPinVisible
        {
            get { return dPtimesToSPinVisible; }
            set
            {
                dPtimesToSPinVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DPtimesToSPinVisible"));
                }
            }
        }

        //TimesToSpin
        private int timesToSpin;
        public int TimesToSpin
        {
            get { return timesToSpin; }
            set
            {
                timesToSpin = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TimesToSpin"));
                }
            }
        }

        public int SIMoney { get; set; }
        public int SIPBN { get; set; }

        string errorString = "";
        string successString = "";

        public XmlRpcVM()
        {
            SettingsClicked = new RelayCommand(OnSettingsClicked);
            RefreshPBNVault = new RelayCommand(OnRefreshPBNVaultClick);
            VautContextMenu = new RelayCommand(OnVautContextMenuClick);

            CmbBoxWPList = new ObservableCollection<SelectedProfile>();
            CmbBoxDrupalList = new ObservableCollection<SelectedProfile>();
            Tabs = new ObservableCollection<SpinningVM>();
            SavedPBNProjects = new ObservableCollection<PBNProject>();
            SavedMoneyProjects = new ObservableCollection<PBNProject>();

            Status = "Select platforms to pulbilsh to from settings.";
            EnableBtns = true;
            TabsVisible = Visibility.Collapsed;
            AutoSpinChecked = false;
            TimesToSpin = 0;

            OnRefreshPBNVaultClick("");
        }

        private void OnRefreshPBNVaultClick(object param)
        {
            try
            {
                switch ((string)param)
                {
                    case "SetMozKey":
                        SaveMozKeysWindow smw = new SaveMozKeysWindow();
                        smw.tbSecret.Text = MozscapeAPI.mozSecret;
                        smw.tbID.Text = MozscapeAPI.mozId;
                        smw.ShowDialog();
                        if (smw.OKClicked)
                        {
                            MyFilesDatabase.SetMozIds();
                        }
                        break;

                    case "Refresh":
                        SavedPBNProjects.Clear();
                        string vaultDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault");
                        if (!Directory.Exists(vaultDir)) return;

                        string filePath = Path.Combine(vaultDir, "vaultConfig.txt");
                        if (!File.Exists(filePath)) return;

                        List<string> fileLines = File.ReadAllLines(filePath).ToList();
                        List<string> linesToRemove = new List<string>();
                        List<string> linesToAdd = new List<string>();

                        for (int i = 0; i < fileLines.Count; i++)
                        {
                            string line = fileLines[i];
                            string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                            string DirPath = lineInfo[2];

                            if (!Directory.Exists(DirPath) || (!File.Exists(Path.Combine(DirPath, "UserData.ini")) && !File.Exists(Path.Combine(DirPath, "ProjectData.ini"))))
                            {
                                linesToRemove.Add(lineInfo[0] + MyFilesDatabase.SPLITTER + lineInfo[1] + MyFilesDatabase.SPLITTER + DirPath + MyFilesDatabase.SPLITTER + lineInfo[3]);

                                DirPath = MyFilesDatabase.FindProjectDirByName(lineInfo[3], lineInfo[0]);
                                if (DirPath != "")
                                    linesToAdd.Add(lineInfo[0] + MyFilesDatabase.SPLITTER + lineInfo[1] + MyFilesDatabase.SPLITTER + DirPath + MyFilesDatabase.SPLITTER + lineInfo[3]);
                            }

                            if (DirPath != "")
                                SavedPBNProjects.Add(new PBNProject() { Name = lineInfo[0], SIType = Convert.ToInt32(lineInfo[1]), FilePath = DirPath, ProjectName = "(" + lineInfo[3] + ")" }); 
                        }

                        if (linesToRemove.Count > 0)
                        {
                            foreach (string ind in linesToRemove)
                            {
                                fileLines.Remove(ind);
                            }
                            foreach (string l in linesToAdd)
                            {
                                fileLines.Add(l);
                            }

                            File.WriteAllLines(filePath, fileLines);
                        }


                        //foreach (string line in fileLines)
                        //{
                        //    string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.RemoveEmptyEntries);
                        //    SavedPBNProjects.Add(new PBNProject() { Name = lineInfo[0], SIType = Convert.ToInt32(lineInfo[1]), FilePath = lineInfo[2], ProjectName ="(" + lineInfo[3] +")" });
                        //}
                        break;

                    case "RefreshMoney":
                        SavedMoneyProjects.Clear();
                        string vaultDir1 = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault");
                        if (!Directory.Exists(vaultDir1)) return;

                        string filePath1 = Path.Combine(vaultDir1, "vaultMoneyConfig.txt");
                        if (!File.Exists(filePath1)) return;

                        List<string> fileLines1 = File.ReadAllLines(filePath1).ToList();
                        List<string> linesToRemove1 = new List<string>();
                        List<string> linesToAdd1 = new List<string>();

                        for (int i = 0; i < fileLines1.Count; i++)
                        {
                            string line = fileLines1[i];
                            string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                            string DirPath = lineInfo[2];

                            if (!Directory.Exists(DirPath) || (!File.Exists(Path.Combine(DirPath, "UserData.ini")) && !File.Exists(Path.Combine(DirPath, "ProjectData.ini"))))
                            {
                                linesToRemove1.Add(lineInfo[0] + MyFilesDatabase.SPLITTER + lineInfo[1] + MyFilesDatabase.SPLITTER + DirPath + MyFilesDatabase.SPLITTER + lineInfo[3]);

                                DirPath = MyFilesDatabase.FindProjectDirByName(lineInfo[3], lineInfo[0]);
                                if (DirPath != "")
                                    linesToAdd1.Add(lineInfo[0] + MyFilesDatabase.SPLITTER + lineInfo[1] + MyFilesDatabase.SPLITTER + DirPath + MyFilesDatabase.SPLITTER + lineInfo[3]);
                            }

                            if (DirPath != "")
                                SavedMoneyProjects.Add(new PBNProject() { Name = lineInfo[0], SIType = Convert.ToInt32(lineInfo[1]), FilePath = DirPath, ProjectName = "(" + lineInfo[3] + ")" });
                        }

                        if (linesToRemove1.Count > 0)
                        {
                            foreach (string l in linesToRemove1)
                            {
                                fileLines1.Remove(l);
                            }
                                  

                            foreach (string l in linesToAdd1)
                            {
                                fileLines1.Add(l);
                            }

                            File.WriteAllLines(filePath1, fileLines1);
                        }

                        //foreach (string line in fileLines1)
                        //{
                        //    string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                        //    SavedMoneyProjects.Add(new PBNProject() { Name = lineInfo[0], SIType = Convert.ToInt32(lineInfo[1]), FilePath = lineInfo[2], ProjectName = "(" + lineInfo[3] + ")" });
                        //}
                        break;

                    default:
                        break;
                }
            }
            catch { }
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
               errorString = "";
               successString = "";
               
               if (UseSpunArticlesChecked)
               {
                   publishSpun(content,"", true);

                   foreach (SpinningVM spin in Tabs)
                   {
                       spin.WasUsed = false;
                   }

                   if (successString != "")
                       MessageBox.Show(successString);
                   if (errorString != "")
                       MessageBox.Show(errorString);
                   return;
               }
               if (TimesToSpin > 0 && AutoSpinChecked)
               {
                   for (int i = 1; i < TimesToSpin; i++)
                   {
                       string ttl = Spinner.Spin(PostTitle);
                       if(ttl == "")
                           ttl = " ";
                       publishSpun(Spinner.Spin(content), IsSpinChecked ? ttl : "");
                   }

                   if (successString != "")
                       MessageBox.Show(successString);
                   if (errorString != "")
                       MessageBox.Show(errorString);

                   return;
               }
               publishSpun(content);
           }).Start();
        }

        internal void OnPubFromVaultClick(string content)
        {
            new Thread(() =>
            {
                try
                {
                    ProgressBarStart = true;
                    EnableBtns = false;

                    errorString = "";
                    successString = "";
                    

                    foreach (SpinningVM spin in Tabs)
                    {
                        spin.WasUsed = false;
                    }

                    foreach (PBNProject pbnProj in SavedPBNProjects)
                    {
                        if (!pbnProj.IsSelected) continue;

                        if (UseSpunArticlesChecked)
                        {
                            string title = PostTitle;
                            getSpunContent(ref content, ref title);

                            switch (pbnProj.SIType)
                            {
                                case PBNProject.TYPE_WORDPRESS:
                                    publishFromVaultWP(pbnProj, title, content);
                                    break;
                                case PBNProject.TYPE_DRUPAL:
                                    publishFromVaultDrupal(pbnProj, title, content);
                                    break;
                                default:
                                    break;
                            }
                            continue;
                        }
                        if (TimesToSpin > 0 && AutoSpinChecked)
                        {
                             switch (pbnProj.SIType)
                            {
                                case PBNProject.TYPE_WORDPRESS:
                                    publishFromVaultWP(pbnProj, IsSpinChecked ? Spinner.Spin(PostTitle) : PostTitle, Spinner.Spin(content));
                                    break;
                                case PBNProject.TYPE_DRUPAL:
                                    publishFromVaultDrupal(pbnProj, IsSpinChecked ? Spinner.Spin(PostTitle) : PostTitle, Spinner.Spin(content));
                                    break;
                                default:
                                    break;
                            }
                            continue;
                        }
                        switch (pbnProj.SIType)
                        {
                            case PBNProject.TYPE_WORDPRESS:
                                publishFromVaultWP(pbnProj, PostTitle, content);
                                break;
                            case PBNProject.TYPE_DRUPAL:
                                publishFromVaultDrupal(pbnProj, PostTitle, content);
                                break;
                            default:
                                break;
                        }
                    }

                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (successString != "" && errorString == "")
                            FlexibleMessageBox.Show(successString);
                        else if (errorString != "" && successString == "")
                            FlexibleMessageBox.Show(errorString);
                        else
                            FlexibleMessageBox.Show(successString + Environment.NewLine + "Errors:" + Environment.NewLine + errorString);
                    });

                    Status = "Done : )   ";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    Status = "Whoops @#$%@!!   ";
                }

                ProgressBarStart = false;
                EnableBtns = true;
            }).Start();
        }

        private void publishFromVaultDrupal(PBNProject pbnProj, string title, string content)
        {
            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbnProj.FilePath);
            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
            {
                errorString += "Website in profile data cannot be empty. " + profile.ProfileName + Environment.NewLine;
                return;
            }
            Status = "Posting to " + profile.WebAddress + ".";

            string url = profile.WebAddress;
            if (url[url.Length - 1] != '/')
                url += '/';
            url += "xmlrpc.php";

            DrupalServices d = new Drupal7.Services.DrupalServices(url, profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword);
            bool isin = d.Login(profile.Username, profile.Password);
            if (!isin)
            {
                errorString += "Was unable to authenticate " + profile.WebAddress + Environment.NewLine;
                return;
            }

            XmlRpcStruct postStruct = new XmlRpcStruct();
            postStruct.Add("type", "article");
            postStruct.Add("title", title);

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
                foreach (PBNProject moneyProj in SavedMoneyProjects)
                {
                    PersonData moneyProfile = MyFilesDatabase.GetSubProjectPersonData(moneyProj.FilePath);
                    if (content.Contains(moneyProfile.WebAddress))
                    {
                        Match m;
                        string HRefPattern = "\\<a.+?href=(?<q>[\"'])(.+?)\\k<q>.*?>([^\\<]+)";

                        try
                        {
                            m = Regex.Match(content, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                            while (m.Success)
                            {
                                string link = m.Groups[1].ToString();
                                string text = m.Groups[2].ToString();
                                if (link.Contains(moneyProfile.WebAddress))
                                {
                                    BacklinksHistoryVM.SaveLink(moneyProfile, link, text, profile.WebAddress);
                                }
                                m = m.NextMatch();
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Error Saveing backlink history. " + ex.Message);
                        }
                    }
                }
            }

            d.Logout();
        }

        private void publishFromVaultWP(PBNProject pbnProj, string title, string content)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                errorString += "content cannot be empty. " + Environment.NewLine;
                return;
            }

            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbnProj.FilePath);
            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
            {
                errorString += "Website in profile data cannot be empty. " + profile.ProfileName + Environment.NewLine;
                return;
            }

            Status = "Posting to " + profile.WebAddress + ".";

            DateTime publishdt = DateTime.Now;
            try
            {
                publishdt = TimeHelper.GetNistTime(profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword).Date;
            }
            catch { publishdt = DateTime.Now; }

            string link = profile.WebAddress;
            using (WordPressClient client = new WordPressClient(new WordPressSiteConfig
            {
                BaseUrl = link,
                BlogId = 1,
                Username = profile.Username,
                Password = profile.Password,
            }, profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword))
            {
                try
                {
                    if (content.Contains("<IMG"))
                    {
                        content = getcontentAfterImgUpload(content, client);
                    }

                    var post = new Post
                    {
                        PostType = "post", // "post" or "page"
                        Title = title,
                        Content = content,
                        PublishDateTime = publishdt,
                        Status = "publish" // "draft" or "publish"
                    };

                    Status = "Posting to " + link + ".";

                    var id = client.NewPost(post);
                    
                    try
                    {
                        link = client.GetPost(Convert.ToInt32(id)).Link;
                    }
                    catch { }
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        OnRefreshPBNVaultClick("RefreshMoney");
                    });
                    foreach (PBNProject moneyProj in SavedMoneyProjects)
                    {
                        PersonData moneyProfile = MyFilesDatabase.GetSubProjectPersonData(moneyProj.FilePath);
                        if (content.Contains(moneyProfile.WebAddress))
                        {
                            Match m;
                            string HRefPattern = "\\<a.+?href=(?<q>[\"'])(.+?)\\k<q>.*?>([^\\<]+)";

                            try
                            {
                                m = Regex.Match(content, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                                while (m.Success)
                                {
                                    string mlink = m.Groups[1].ToString();
                                    string text = m.Groups[2].ToString();
                                    if (mlink.Contains(moneyProfile.WebAddress))
                                    {
                                        BacklinksHistoryVM.SaveLink(moneyProfile, mlink, text, moneyProfile.WebAddress);
                                    }
                                    m = m.NextMatch();
                                }
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("Error Saveing backlink history. " + ex.Message);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    errorString += "Unable to post to " + profile.WebAddress +" " + ex.Message + Environment.NewLine;
                    return;
                }
            }

            successString += "Post succesfull to " + link + Environment.NewLine;
        }

        private string getcontentAfterImgUpload(string content, WordPressClient client)
        {
            Match m;
            string HRefPattern = @"<img.*?src=""(?<url>.*?)"".*?>";

            try
            {
                m = Regex.Match(content, HRefPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                while (m.Success)
                {
                    string link = m.Groups[1].ToString();
                    try
                    { 
                        Status = "Uploading file " + link + ".";
                        // string text = m.Groups[2].ToString();
                        string mime = GetMimeType(link);
                        Data data = Data.CreateFromFilePath(link.Replace("file:///", ""), mime);
                        UploadResult uResult = client.UploadFile(data);
                        content = content.Replace(link, uResult.Url);
                    }
                    catch (Exception ex)
                    {
                        errorString += "Image upload failed " + ex.Message+Environment.NewLine;
                        content = content.Replace(link,"");
                    }
                    
                    m = m.NextMatch();
                }
            }
            catch (Exception ex)
            {
                errorString += "Image upload failed " + ex.Message + Environment.NewLine;
            }

            return content;
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        private void publishSpun(string content, string title = "", bool fromtabs = false)
        {
            EnableBtns = false;
            ProgressBarStart = true;
            //Application.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    Mouse.OverrideCursor = Cursors.Wait;
            //});

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
                            publishdt = TimeHelper.GetNistTime(profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword).Date;
                        }
                        catch { publishdt = DateTime.Now; }
                        if (UseSpunArticlesChecked)
                        {
                            title = PostTitle;
                            getSpunContent(ref content, ref title);
                        }

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
                                if (content.Contains("<IMG"))
                                {
                                    content = getcontentAfterImgUpload(content, client);
                                }

                                var post = new Post
                                {
                                    PostType = "post", // "post" or "page"
                                    Title = title == "" ? PostTitle : title,
                                    Content = content,
                                    PublishDateTime = publishdt,
                                    Status = "publish" // "draft" or "publish"
                                };

                                Status = "Posting to " + profile.WebAddress + ".";

                                var id = client.NewPost(post);
                            }
                            catch
                            {
                                errorString += "Unable to post to " + profile.WebAddress + Environment.NewLine;
                                continue;
                            }
                        }

                        successString += "Post succesfull to " + profile.WebAddress + Environment.NewLine;
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
                        if (!isin)
                        {
                            errorString += "Was unable to authenticate " + profile.WebAddress + Environment.NewLine;
                            continue;
                        }

                        if (UseSpunArticlesChecked)
                        {
                            title = PostTitle;
                            getSpunContent(ref content, ref title);
                        }

                        XmlRpcStruct postStruct = new XmlRpcStruct();
                        postStruct.Add("type", "article");
                        postStruct.Add("title", title == "" ? PostTitle : title);

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
                if (!UseSpunArticlesChecked && TimesToSpin <= 0 && !AutoSpinChecked)
                {
                    MessageBox.Show("Whoops something went wrong: " + ex.Message);
                }
                else
                {
                    errorString += "Whoops something went wrong: " + ex.Message;
                }
            }

            //Application.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    Mouse.OverrideCursor = null;
            //});

            if (!UseSpunArticlesChecked && TimesToSpin <= 0 && !AutoSpinChecked)
            {
                if (successString != "")
                    MessageBox.Show(successString);
                if (errorString != "")
                    MessageBox.Show(errorString);
            }

            Status = "Ready to publish.";
            EnableBtns = true;
            ProgressBarStart = false;
        }

        public void getSpunContent(ref string content, ref string title)
        {

            bool found = false;
            foreach (SpinningVM spin in Tabs)
            {
                if (!spin.WasUsed && spin.IsChecked)
                {
                    if (!UseSpunArticlesChecked)
                        content = Spinner.Spin(content);
                    else
                        content = spin.Content;

                    if (IsSpinChecked && !UseSpunArticlesChecked)
                        title = Spinner.Spin(PostTitle);
                    else
                        title = spin.Title;

                    spin.WasUsed = true;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                foreach (SpinningVM spin in Tabs)
                {
                    if (spin.WasUsed && spin.IsChecked)
                    {
                        spin.WasUsed = false;
                        break;
                    }
                }

                getSpunContent(ref content, ref  title);
            }
        }

        public void SetProfileDate(PersonData profile)
        {
            List<KeyValuePair<string, string>> directoryValues = MyFilesDatabase.GetSubProjectsFolders(profile.ProjectDir, profile.ProjectName);
            foreach (KeyValuePair<string, string> prof in directoryValues)
            {
                CmbBoxWPList.Add(new SelectedProfile() { ProfileName = prof.Key, Path = prof.Value });
                CmbBoxDrupalList.Add(new SelectedProfile() { ProfileName = prof.Key, Path = prof.Value });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Spin(string text, string title)
        {
            try
            {   
                if(IsSpinChecked)
                    title = Spinner.Spin(title);
                string content = Spinner.Spin(text);
                Tabs.Add(new SpinningVM() { IsChecked = true, Title = title, Content = content });
                if (Tabs.Count == 1)
                {
                    Tabs.Add(new SpinningVM() { IsChecked = true, Title = title, Content = content });
                    Tabs.RemoveAt(Tabs.Count - 1);
                }
                TabsVisible = Visibility.Visible;
            }
            catch
            {
                MessageBox.Show("Error Spinning Content, Unbalanced brace.");
            }
        }

        internal void ClearSpunTabs()
        {
            Tabs.Clear();
            TabsVisible = Visibility.Collapsed;
        }

        private void OnVautContextMenuClick(object param)
        {
            switch ((string)param)
            {
                case "MozRank":
                    Mouse.OverrideCursor = Cursors.Wait;
                    new Thread(() =>
                    {
                        try
                        {
                            MozscapeAPI mozAPI1 = new MozscapeAPI();
                            foreach (PBNProject pbnProj in SavedPBNProjects)
                            {
                                if (!pbnProj.IsSelected) continue;
                                PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbnProj.FilePath);
                                if (profile.WebAddress == "") continue;

                                string strAPIURL1 = mozAPI1.CreateAPIURL(MozscapeAPI.mozId, MozscapeAPI.mozSecret, 1, "url metrics", profile.WebAddress, "");
                                string strResults1 = mozAPI1.FetchResults(strAPIURL1);
                                MozscapeLinkMetric msURLMetrics1 = mozAPI1.ParseURLMetrics(strResults1);

                                string pageAuthority1 = msURLMetrics1.upa;
                                string domainAuthority1 = msURLMetrics1.pda;
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    if (pageAuthority1.Contains('.')) pageAuthority1 = pageAuthority1.Split('.')[0];
                                    if (domainAuthority1.Contains('.')) domainAuthority1 = domainAuthority1.Split('.')[0];
                                    pbnProj.PageAuthority = "PA: " + pageAuthority1;
                                    pbnProj.DomainAuthority = "DA: " + domainAuthority1;
                                    pbnProj.AuthorityVisible = Visibility.Visible;
                                    Mouse.OverrideCursor = null;
                                });

                                Thread.Sleep(1100);
                            }

                            foreach (PBNProject pbnProj in SavedMoneyProjects)
                            {
                                if (!pbnProj.IsSelected) continue;
                                PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbnProj.FilePath);
                                if (profile.WebAddress == "") continue;

                                string strAPIURL1 = mozAPI1.CreateAPIURL(MozscapeAPI.mozId, MozscapeAPI.mozSecret, 1, "url metrics", profile.WebAddress, "");
                                string strResults1 = mozAPI1.FetchResults(strAPIURL1);
                                MozscapeLinkMetric msURLMetrics1 = mozAPI1.ParseURLMetrics(strResults1);

                                string pageAuthority1 = msURLMetrics1.upa;
                                string domainAuthority1 = msURLMetrics1.pda;
                                Application.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    if (pageAuthority1.Contains('.')) pageAuthority1 = pageAuthority1.Split('.')[0];
                                    if (domainAuthority1.Contains('.')) domainAuthority1 = domainAuthority1.Split('.')[0];
                                    pbnProj.PageAuthority = "PA: " + pageAuthority1;
                                    pbnProj.DomainAuthority = "DA: " + domainAuthority1;
                                    pbnProj.AuthorityVisible = Visibility.Visible;
                                    Mouse.OverrideCursor = null;
                                });

                                Thread.Sleep(1100);
                            }
                        }
                        catch(Exception ex)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Mouse.OverrideCursor = null;
                            });
                            MessageBox.Show("If this is a moz restriction wait between 5 - 10 seconds before using there api again. Error: " + ex.Message);
                        }
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            Mouse.OverrideCursor = null;
                        });
                    }).Start();
                    break;

                case "BACKLINK_HISTORY":
                    try
                    {
                        BacklinksHistoryWindow bhw = new BacklinksHistoryWindow();
                        BacklinksHistoryVM vm = new BacklinksHistoryVM();
                        vm.FillHistoryList(MyFilesDatabase.GetSubProjectPersonData(SavedMoneyProjects[SIMoney].FilePath));
                        bhw.DataContext = vm;
                        bhw.Show();
                    }
                    catch { }
                    break;

                case "CopyLinkPBN":
                    try
                    {
                        PersonData profile = MyFilesDatabase.GetSubProjectPersonData(SavedPBNProjects[SIPBN].FilePath);
                        if (profile == null || string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress)) return;
                        MyFilesDatabase.SetClipboardText(profile.WebAddress);
                    }
                    catch { }
                    break;

                case "CopyLinkMoney":
                    try
                    {
                        PersonData profile = MyFilesDatabase.GetSubProjectPersonData(SavedMoneyProjects[SIMoney].FilePath);
                        if (profile == null || string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress)) return;
                        MyFilesDatabase.SetClipboardText(profile.WebAddress);
                    }
                    catch { }
                    break;

                default:
                    break;
            }
        }


    }
}
