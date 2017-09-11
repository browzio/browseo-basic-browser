using CookComputing.XmlRpc;
using Drupal7.Services;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using System.IO;
using Delimon.Win32.IO;
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
using WordPressSharp.Constants;

using Directory = Organiser.Common.Classes.MyFilesDatabase.Directory;
using Path = Organiser.Common.Classes.MyFilesDatabase.Path;
using File = Organiser.Common.Classes.MyFilesDatabase.File;
using System.Net.Http;

namespace WPF_WYSIWYG_HTML_Editor
{
    public class XmlRpcVM : INotifyPropertyChanged
    {
        public const string PBNCONFIG = "vaultConfig.txt";
        public const string PBNCONFIGMONEY = "vaultMoneyConfig.txt";

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

        private string tags;
        public string Tags
        {
            get { return tags; }
            set
            {
                tags = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tags"));
                }
            }
        }

        private string categories;
        public string Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Categories"));
                }
            }
        }

        private string featuredImage;
        public string FeaturedImage
        {
            get { return featuredImage; }
            set
            {
                featuredImage = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FeaturedImage"));
                }
            }
        }
        //FeaturedDirPath
        int alreadyUsedImagesFromPath = 0;
        private string featuredDirPath;
        public string FeaturedDirPath
        {
            get { return featuredDirPath; }
            set
            {
                featuredDirPath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FeaturedDirPath"));
                }
            }
        }
        

        //SpinCategoriesChecked
        private bool spinCategoriesChecked;
        public bool SpinCategoriesChecked
        {
            get { return spinCategoriesChecked; }
            set
            {
                spinCategoriesChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpinCategoriesChecked"));
                }
            }
        }
        //SpinTagsChecked
        private bool spinTagsChecked;
        public bool SpinTagsChecked
        {
            get { return spinTagsChecked; }
            set
            {
                spinTagsChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpinTagsChecked"));
                }
            }
        }
        //SpinExcerptChecked
        private bool spinExcerptChecked;
        public bool SpinExcerptChecked
        {
            get { return spinExcerptChecked; }
            set
            {
                spinExcerptChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SpinExcerptChecked"));
                }
            }
        }

        //Exerpt
        private string exerpt;
        public string Exerpt
        {
            get { return exerpt; }
            set
            {
                exerpt = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Exerpt"));
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

        //SavedPBNProjects
        public ObservableCollection<PBNProject> SavedPBNProjects { get; set; }
        public ObservableCollection<PBNProjectsFolder> SavedPBNProjectsFolders { get; set; }
        //SavedMoney
        public ObservableCollection<PBNProject> SavedMoneyProjects { get; set; }
        public ObservableCollection<PBNProjectsFolder> SavedMoneyProjectsFolders { get; set; }
        bool refresshedMoney, refreshedpbn;

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

        //TimeInterval
        private int timeInterval;
        public int TimeInterval
        {
            get { return timeInterval; }
            set
            {
                timeInterval = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TimeInterval"));
                }
            }
        }

        private DateTime publishDate;
        public DateTime PublishDate
        {
            get { return publishDate; }
            set
            {
                publishDate = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PublishDate"));
                }
            }
        }


        public int SIMoney { get; set; }
        public int SIPBN { get; set; }



        string errorString = "";
        string successString = "";

        bool inmiddleofStuff = false;

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
            SavedPBNProjectsFolders = new ObservableCollection<PBNProjectsFolder>();
            SavedMoneyProjectsFolders = new ObservableCollection<PBNProjectsFolder>();

            Status = "Select platforms to pulbilsh to from settings.";
            EnableBtns = true;
            TabsVisible = Visibility.Collapsed;
            AutoSpinChecked = false;
            TimesToSpin = 0;
            PublishDate = DateTime.Now;
            FeaturedDirPath = null;

            OnRefreshPBNVaultClick("");
        }

        private void OnSettingsClicked(object param)
        {
            switch ((string)param)
            {
                case "FeaturedImage":
                    using (System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog())
                    {
                        // openFileDialog.InitialDirectory = @"C:\";
                        openFileDialog.Filter = "All files (*.*)|*.*|jpg files (*.jpg)|*.jpg|png files (*.png.*)|*.png|gif files (*.gif)|*.gif";
                        openFileDialog.RestoreDirectory = true;

                        System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            FeaturedImage = openFileDialog.FileName;
                        }
                    }
                    break;

                case "Settings":
                    SelectPlatformWindow slw = new SelectPlatformWindow();
                    slw.DataContext = this;
                    slw.ShowDialog();
                    if (isAnySelected())
                        Status = "Ready to publish.";
                    break;

                case "ClearFeaturedImage":
                    FeaturedImage = string.Empty;
                    FeaturedDirPath = null;
                    break;

                case "UseDirectory":
                    System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

                    FeaturedDirPath = fbd.SelectedPath;
                    break;

                default:
                    break;
            }
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

        //public async void OnPublishClick(string content)
        //{
        //    if (!isAnySelected())
        //    {
        //        MessageBox.Show("Select platforms to publish to from settings.");
        //        return;
        //    }
        //    errorString = "";
        //    successString = "";

        //    if (UseSpunArticlesChecked)
        //    {
        //       await publishSpun(content, "", true);

        //        foreach (SpinningVM spin in Tabs)
        //        {
        //            spin.WasUsed = false;
        //        }

        //        if (successString != "")
        //            MessageBox.Show(successString);
        //        if (errorString != "")
        //            MessageBox.Show(errorString);
        //        return;
        //    }
        //    if (TimesToSpin > 0 && AutoSpinChecked)
        //    {
        //        for (int i = 1; i < TimesToSpin; i++)
        //        {
        //            string ttl = Spinner.Spin(PostTitle);
        //            if (ttl == "")
        //                ttl = " ";
        //            await publishSpun(Spinner.Spin(content), IsSpinChecked ? ttl : "");
        //        }

        //        if (successString != "")
        //            MessageBox.Show(successString);
        //        if (errorString != "")
        //            MessageBox.Show(errorString);

        //        return;
        //    }
        //    await publishSpun(content);
        //}

        internal async void OnPubFromVaultClick(string content)
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
                    await publishpbn(pbnProj, content);
                }
                foreach (var p in SavedPBNProjectsFolders)
                {
                    foreach (var pbnProj in p.PBNProjects)
                    {
                        await publishpbn(pbnProj, content);
                    }
                }
                foreach (PBNProject pbnProj in SavedMoneyProjects)
                {
                    await publishpbn(pbnProj, content);
                }
                foreach (var p in SavedMoneyProjectsFolders)
                {
                    foreach (var pbnProj in p.PBNProjects)
                    {
                        await publishpbn(pbnProj, content);
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
        }

        internal void ValidateSpintax(string body)
        {
            string errors = "";
            string content = "";
            try
            {
                if(!PostTitle.IsNullOrEmpty())
                    content += Spinner.Spin(PostTitle) + Environment.NewLine + Environment.NewLine;
            }
            catch (Exception ex)
            {
                errors += ex.Message + Environment.NewLine + "  :    In Title" + Environment.NewLine + Environment.NewLine;
            }
            try
            {
                if (!body.IsNullOrEmpty())
                    content += Spinner.Spin(body) + Environment.NewLine + Environment.NewLine;
            }
            catch (Exception ex)
            {
                errors += ex.Message + Environment.NewLine + "  :    In Body" + Environment.NewLine + Environment.NewLine;
            }
            try
            {
                if (!Tags.IsNullOrEmpty())
                    content += Spinner.Spin(Tags) + Environment.NewLine + Environment.NewLine;
            }
            catch (Exception ex)
            {
                errors += ex.Message + "  :    In Tags" + Environment.NewLine + Environment.NewLine;
            }
            try
            {
                if (!Categories.IsNullOrEmpty())
                    content += Spinner.Spin(Categories) + Environment.NewLine + Environment.NewLine;
            }
            catch (Exception ex)
            {
                errors += ex.Message + Environment.NewLine + "  :    In Categories" + Environment.NewLine + Environment.NewLine;
            }
            try
            {
                if (!Exerpt.IsNullOrEmpty())
                    content += Spinner.Spin(Exerpt) + Environment.NewLine + Environment.NewLine;
            }
            catch (Exception ex)
            {
                errors += ex.Message + Environment.NewLine + "  :    In Excerpt" + Environment.NewLine + Environment.NewLine;
            }

            if (!errors.IsNullOrEmpty())
            {
                FlexibleMessageBox.Show(errors);
            }
            else
            {
                FlexibleMessageBox.Show(content,"Preview");
            }
        }

        private async Task publishpbn(PBNProject pbnProj,string content)
        {
            if (!pbnProj.IsSelected) return;

            if (UseSpunArticlesChecked)
            {
                if (Tabs.Count <= 0) return;

                string title = PostTitle;
                getSpunContent(ref content, ref title);

                switch (pbnProj.SIType)
                {
                    case PBNProject.TYPE_WORDPRESS:
                        await publishFromVaultWP(pbnProj, title, content, PublishDate);
                        break;
                    case PBNProject.TYPE_DRUPAL:
                        publishFromVaultDrupal(pbnProj, title, content, PublishDate);
                        break;
                    default:
                        break;
                }
                return;
            }
            if (TimesToSpin > 0 && AutoSpinChecked)
            {
                DateTime publishdt = PublishDate;

                for (int i = 0; i < TimesToSpin; i++)
                {
                    switch (pbnProj.SIType)
                    {
                        case PBNProject.TYPE_WORDPRESS:
                            await publishFromVaultWP(pbnProj, IsSpinChecked ? Spinner.Spin(PostTitle) : PostTitle, Spinner.Spin(content), publishdt);
                            break;
                        case PBNProject.TYPE_DRUPAL:
                            publishFromVaultDrupal(pbnProj, IsSpinChecked ? Spinner.Spin(PostTitle) : PostTitle, Spinner.Spin(content), publishdt);
                            break;
                        default:
                            break;
                    }

                    publishdt = publishdt.AddHours(TimeInterval);
                }
                return;
            }
            switch (pbnProj.SIType)
            {
                case PBNProject.TYPE_WORDPRESS:
                    await publishFromVaultWP(pbnProj, PostTitle, content, PublishDate);
                    break;
                case PBNProject.TYPE_DRUPAL:
                    publishFromVaultDrupal(pbnProj, PostTitle, content, PublishDate);
                    break;
                default:
                    break;
            }
        }

        private void publishFromVaultDrupal(PBNProject pbnProj, string title, string content , DateTime publishdt)
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
                    saveHistory(moneyProj,content,profile.WebAddress);
                }
                foreach (var p in SavedMoneyProjectsFolders)
                {
                    foreach (var moneyProj in p.PBNProjects)
                    {
                        saveHistory(moneyProj, content, profile.WebAddress);
                    }
                }
            }

            d.Logout();
        }

        private void saveHistory(PBNProject moneyProj,string content,string webAddress)
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
                            BacklinksHistoryVM.SaveLink(moneyProfile, link, text, webAddress);
                        }
                        m = m.NextMatch();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Saveing backlink history. " + ex.Message);
                }
            }
        }

        private async Task publishFromVaultWP(PBNProject pbnProj, string title, string content, DateTime publishdt)
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

            string link = await publishToWP(content, title, profile, publishdt);

            if(!link.IsNullOrEmpty())
                successString += "Post succesfull to " + link + Environment.NewLine;
        }

        private async Task<string> publishToWP(string content,string title, PersonData profile, DateTime publishdt)
        {
           return await Task.Run(async () =>
            {
                string link = profile.WebAddress;


                if (!profile.ProxyIP.IsNullOrEmpty() && publishdt <= DateTime.Now.AddHours(-1))
                {
                    publishdt = TimeHelper.GetTimeOfProxy(profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword).Date;
                }

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
                        List<Term> postTerms = new List<Term>();

                        var termies = client.GetTerms(TaxonomyType.Tags, new TermFilter()).ToList();
                        var categories = client.GetTerms(TaxonomyType.Category, new TermFilter()).ToList();

                        Status = "Setting Tags For " + link + ".";
                        if (!Tags.IsNullOrEmpty())
                        {
                            string[] tags = Tags.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var tagie in tags)
                            {
                                string tag = tagie.Trim();
                                if (SpinTagsChecked) tag = Spinner.Spin(tag);
                                bool found = false;
                                foreach (var term in termies)
                                {
                                    if (term.Name == tag)
                                    {
                                        if(!postTerms.Any(p=>p.Name == term.Name && p.Taxonomy == TaxonomyType.Tags)) postTerms.Add(term);
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Term t = new Term
                                    {
                                        Name = tag,
                                        Description = tag,
                                        Taxonomy = TaxonomyType.Tags
                                    };
                                    var termId = client.NewTerm(t);
                                    t.Id = termId;
                                    postTerms.Add(t);
                                    termies.Add(t);
                                }
                            }
                        }

                        Status = "Setting Categories For " + link + ".";
                        if (!Categories.IsNullOrEmpty())
                        {
                            string[] cats = Categories.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var catie in cats)
                            {
                                string cat = catie.Trim();
                                if (spinCategoriesChecked) cat = Spinner.Spin(cat);
                                bool found = false;
                                foreach (var term in categories)
                                {
                                    if (term.Name == cat)
                                    {
                                        if (!postTerms.Any(p => p.Name == term.Name && p.Taxonomy == TaxonomyType.Category)) postTerms.Add(term);
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Term t = new Term
                                    {
                                        Name = cat,
                                        Description = cat,
                                        Taxonomy = TaxonomyType.Category
                                    };
                                    var termId = client.NewTerm(t);
                                    t.Id = termId;
                                    postTerms.Add(t);
                                    categories.Add(t);
                                }
                            }
                        }

                        string featuredimageId = string.Empty;

                        if (FeaturedDirPath == null)
                        {
                            if (!FeaturedImage.IsNullOrEmpty() && File.Exists(FeaturedImage))
                            {
                                Status = "Uploading Featured Image For " + link + ".";
                                string mime = GetMimeType(FeaturedImage);
                                Data data = Data.CreateFromFilePath(FeaturedImage, mime);
                                UploadResult uResult = client.UploadFile(data);
                                featuredimageId = uResult.Id;
                            }
                        }
                        else
                        {
                            if (Directory.Exists(FeaturedDirPath))
                            {
                                FileInfo[] imageFiles = new DirectoryInfo(FeaturedDirPath).GetFiles();
                                if (imageFiles.Length > 0)
                                {
                                    if (alreadyUsedImagesFromPath >= imageFiles.Length) alreadyUsedImagesFromPath = 0;

                                    Status = "Uploading Featured Image For " + link + ". From Selected Directory";

                                    string mime = GetMimeType(imageFiles[alreadyUsedImagesFromPath].FullName);
                                    Data data = Data.CreateFromFilePath(imageFiles[alreadyUsedImagesFromPath].FullName, mime);
                                    UploadResult uResult = client.UploadFile(data);
                                    featuredimageId = uResult.Id;

                                    alreadyUsedImagesFromPath++;
                                }
                            }
                        }

                        var post = new WordPressSharp.Models.Post
                        {
                            PostType = "post", // "post" or "page"
                            Title = title,
                            Content = content,
                            PublishDateTime = publishdt,
                            Status = "publish", // "draft" or "publish"

                        };
                        if (postTerms.Count > 0) post.Terms = postTerms.ToArray();
                        if (!featuredimageId.IsNullOrEmpty()) post.FeaturedImageId = featuredimageId;
                        if (!Exerpt.IsNullOrEmpty())
                        {
                            string exrt = Exerpt;
                            if (spinExcerptChecked) exrt = Spinner.Spin(exrt);
                            post.Exerpt = exrt;
                        }

                        Status = "Posting to " + link + ".";

                        var id = client.NewPost(post);

                        try
                        {
                            link = client.GetPost(Convert.ToInt32(id)).Link;
                        }
                        catch { }

                        if(SavedMoneyProjects.Count == 0 && SavedMoneyProjectsFolders.Count == 0)
                            await RefreshPbnProjects(PBNCONFIGMONEY, SavedMoneyProjects, SavedMoneyProjectsFolders);

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
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error Saveing backlink history. " + ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errorString += "Unable to post to " + profile.WebAddress + " " + ex.Message + Environment.NewLine;
                        return null;
                    }
                }

                return link;
            });
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
                        link = link.Replace("%20", " ");
                        string mime = GetMimeType(link);
                        Data data = Data.CreateFromFilePath(link.Replace("file:///", ""), mime);
                        UploadResult uResult = client.UploadFile(data);
                        content = content.Replace(link.Replace(" ", "%20"), uResult.Url);
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

        //private async Task publishSpun(string content, string title = "", bool fromtabs = false)
        //{
        //    //EnableBtns = false;
        //    //ProgressBarStart = true;
        //    ////Application.Current.Dispatcher.Invoke((Action)delegate
        //    ////{
        //    ////    Mouse.OverrideCursor = Cursors.Wait;
        //    ////});

        //    //try
        //    //{
        //    //    #region --wp--
        //    //    if (IsWPChecked)
        //    //    {
        //    //        Status = "Gathering wordpress profiles.";

        //    //        foreach (SelectedProfile prof in CmbBoxWPList)
        //    //        {
        //    //            if (!prof.IsSelected) continue;


        //    //            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(prof.Path);

        //    //            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
        //    //            {
        //    //                MessageBox.Show("Website in profile data cannot be empty. " + profile.ProfileName);
        //    //                continue;
        //    //            }
        //    //            Status = "Posting to " + profile.WebAddress + ".";
        //    //            DateTime publishdt = DateTime.Now;
        //    //            try
        //    //            {
        //    //                publishdt = TimeHelper.GetTimeOfProxy(profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword).Date;
        //    //            }
        //    //            catch { publishdt = DateTime.Now; }
        //    //            if (UseSpunArticlesChecked)
        //    //            {
        //    //                title = PostTitle;
        //    //                getSpunContent(ref content, ref title);
        //    //            }

        //    //            string link = await publishToWP(content, title, profile);

        //    //            if (!link.IsNullOrEmpty())
        //    //                successString += "Post succesfull to " + profile.WebAddress + Environment.NewLine;
        //    //        }
        //    //    }
        //    //    #endregion

        //    //    #region --drupal--
        //    //    if (IsDrupalChecked)
        //    //    {
        //    //        Status = "Gathering drupal profiles.";
        //    //        foreach (SelectedProfile prof in CmbBoxDrupalList)
        //    //        {
        //    //            if (!prof.IsSelected) continue;

        //    //            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(prof.Path);
        //    //            if (string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress))
        //    //            {
        //    //                MessageBox.Show("Website in profile data cannot be empty. " + profile.ProfileName);
        //    //                continue;
        //    //            }
        //    //            Status = "Posting to " + profile.WebAddress + ".";

        //    //            string url = profile.WebAddress;
        //    //            if (url[url.Length - 1] != '/')
        //    //                url += '/';
        //    //            url += "xmlrpc.php";

        //    //            DrupalServices d = new Drupal7.Services.DrupalServices(url, profile.ProxyIP, profile.ProxyPort, profile.ProxyUsername, profile.ProxyPassword);
        //    //            bool isin = d.Login(profile.Username, profile.Password);
        //    //            if (!isin)
        //    //            {
        //    //                errorString += "Was unable to authenticate " + profile.WebAddress + Environment.NewLine;
        //    //                continue;
        //    //            }

        //    //            if (UseSpunArticlesChecked)
        //    //            {
        //    //                title = PostTitle;
        //    //                getSpunContent(ref content, ref title);
        //    //            }

        //    //            XmlRpcStruct postStruct = new XmlRpcStruct();
        //    //            postStruct.Add("type", "article");
        //    //            postStruct.Add("title", title == "" ? PostTitle : title);

        //    //            XmlRpcStruct postBodyStructParams = new XmlRpcStruct();
        //    //            postBodyStructParams.Add("format", "full_html");
        //    //            postBodyStructParams.Add("value", content);


        //    //            XmlRpcStruct[] postBodyStructParamsArr = new XmlRpcStruct[1];
        //    //            postBodyStructParamsArr[0] = postBodyStructParams;

        //    //            XmlRpcStruct postBodyStruct = new XmlRpcStruct();
        //    //            postBodyStruct.Add("und", postBodyStructParamsArr);

        //    //            postStruct.Add("body", postBodyStruct);

        //    //            XmlRpcStruct s = d.NodeCreate(postStruct);
        //    //            if (s == null)
        //    //            {
        //    //                errorString += "Was Unable to post to " + profile.WebAddress + Environment.NewLine;
        //    //            }
        //    //            else
        //    //            {
        //    //                successString += "Post succesfull to " + profile.WebAddress + Environment.NewLine;
        //    //            }

        //    //            d.Logout();
        //    //        }
        //    //    }
        //    //    #endregion
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    if (!UseSpunArticlesChecked && TimesToSpin <= 0 && !AutoSpinChecked)
        //    //    {
        //    //        MessageBox.Show("Whoops something went wrong: " + ex.Message);
        //    //    }
        //    //    else
        //    //    {
        //    //        errorString += "Whoops something went wrong: " + ex.Message;
        //    //    }
        //    //}

        //    ////Application.Current.Dispatcher.Invoke((Action)delegate
        //    ////{
        //    ////    Mouse.OverrideCursor = null;
        //    ////});

        //    //if (!UseSpunArticlesChecked && TimesToSpin <= 0 && !AutoSpinChecked)
        //    //{
        //    //    if (successString != "")
        //    //        MessageBox.Show(successString);
        //    //    if (errorString != "")
        //    //        MessageBox.Show(errorString);
        //    //}

        //    //Status = "Ready to publish.";
        //    //EnableBtns = true;
        //    //ProgressBarStart = false;
        //}

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



        private async void OnRefreshPBNVaultClick(object param)
        {
            try
            {
                if (inmiddleofStuff) return;

                inmiddleofStuff = true;

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
                        refreshedpbn = true;
                        SavedPBNProjects.Clear();
                        SavedPBNProjectsFolders.Clear();

                        await RefreshPbnProjects(PBNCONFIG, SavedPBNProjects, SavedPBNProjectsFolders);
                        await RefreshRest(PBNCONFIG, SavedPBNProjects, SavedPBNProjectsFolders);
                        break;

                    case "RefreshMoney":
                        refresshedMoney = true;
                        SavedMoneyProjects.Clear();
                        SavedMoneyProjectsFolders.Clear();

                        await RefreshPbnProjects(PBNCONFIGMONEY, SavedMoneyProjects, SavedMoneyProjectsFolders);
                        await RefreshRest(PBNCONFIGMONEY, SavedMoneyProjects, SavedMoneyProjectsFolders);
                        break;

                    case "NEWFolderPBN":
                        SetNameAndDataWindow sadw = new SetNameAndDataWindow();
                        sadw.Title = "New Folder";
                        sadw.tblockInfo.Text = "Enter Name For Folder";
                        sadw.ShowDialog();
                        if (sadw.OkClicked)
                        {
                            if (sadw.tbInputText.Text.IsNullOrEmpty()) return;

                            if (SavedPBNProjectsFolders.Any(p => p.FolderName.Trim().ToLower() == sadw.tbInputText.Text.Trim().ToLower()))
                            {
                                "Folder name already exists".Show();
                                return;
                            }
                            PBNProjectsFolder pp = new PBNProjectsFolder() { FolderName = sadw.tbInputText.Text.Trim().ToLower() };
                            pp.OnVautContextMenuClick_Clicked += OnVautContextMenuClick;
                            SavedPBNProjectsFolders.Add(pp);
                        }
                        break;

                    case "Save":
                    case "SaveMoney":
                        if(refreshedpbn)
                            await resaveList(SavedPBNProjects, SavedPBNProjectsFolders, PBNCONFIG);
                        if(refresshedMoney)
                            await resaveList(SavedMoneyProjects, SavedMoneyProjectsFolders, PBNCONFIGMONEY);
                        break;

                    case "NEWFolderPBNMoney":
                        SetNameAndDataWindow sadwM = new SetNameAndDataWindow();
                        sadwM.Title = "New Folder";
                        sadwM.tblockInfo.Text = "Enter Name For Folder";
                        sadwM.ShowDialog();
                        if (sadwM.OkClicked)
                        {
                            if (sadwM.tbInputText.Text.IsNullOrEmpty()) return;

                            if (SavedMoneyProjectsFolders.Any(p => p.FolderName.Trim().ToLower() == sadwM.tbInputText.Text.Trim().ToLower()))
                            {
                                "Folder name already exists".Show();
                                return;
                            }
                            PBNProjectsFolder pp = new PBNProjectsFolder() { FolderName = sadwM.tbInputText.Text.Trim().ToLower() };
                            pp.OnVautContextMenuClick_Clicked += OnVautContextMenuClick;
                            SavedMoneyProjectsFolders.Add(pp);
                        }
                        break;

                    case "InexCheckPBN":
                        //SavedPBNProjectsFolders
                        //SavedPBNProjects
                        foreach (var pbnInFolder in SavedPBNProjectsFolders)
                        {
                            foreach (var pbn in pbnInFolder.PBNProjects)
                            {
                                if (!pbn.IsSelected) continue;
                                pbn.IndexCheckText = await Task.Run(() => { return ChecIndexOf(pbn.FilePath); });
                            }
                        }
                        foreach (var pbn in SavedPBNProjects)
                        {
                            if (!pbn.IsSelected) continue;
                            pbn.IndexCheckText = await Task.Run(() => { return ChecIndexOf(pbn.FilePath); });
                        }
                        break;

                    case "InexCheckMoney":
                        //SavedMoneyProjects
                        //SavedMoneyProjectsFolders
                        foreach (var pbnInFolder in SavedMoneyProjectsFolders)
                        {
                            foreach (var pbn in pbnInFolder.PBNProjects)
                            {
                                if (!pbn.IsSelected) continue;
                                pbn.IndexCheckText = await Task.Run(() => { return ChecIndexOf(pbn.FilePath); });
                            }
                        }
                        foreach (var pbn in SavedMoneyProjects)
                        {
                            if (!pbn.IsSelected) continue;
                            pbn.IndexCheckText = await Task.Run(() => { return ChecIndexOf(pbn.FilePath); });
                        }
                        break;

                    default:
                        break;
                }
            }
            catch { }

            inmiddleofStuff = false;
        }

        private async Task<string> ChecIndexOf(string filePath)
        {
            string toReturn = "Not Found";

            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(filePath);
            if (profile == null || profile.WebAddress.IsNullOrEmpty()) return toReturn;

            //WebClient webClient = new WebClient();
            //webClient.Encoding = System.Text.Encoding.UTF8;
            var httpClientHandler = new HttpClientHandler();
            if (!profile.ProxyIP.IsNullOrEmpty() && !profile.ProxyPort.IsNullOrEmpty())
            {
                httpClientHandler.Proxy = new WebProxy(profile.ProxyIP + ":" + profile.ProxyPort, false);
                httpClientHandler.UseProxy = true;
                if (!profile.ProxyUsername.IsNullOrEmpty() && !profile.ProxyPassword.IsNullOrEmpty())
                {
                    httpClientHandler.Proxy.Credentials = new NetworkCredential(profile.ProxyUsername, profile.ProxyPassword);
                }
            }
            var client = new HttpClient(httpClientHandler);
            try
            {
                //webClient.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                //webClient.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                //webClient.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                //webClient.Headers.Add(HttpRequestHeader.UserAgent, BrowserSettimgs.UserAgentChrome);
                //string html = webClient.DownloadString("https://www.google.com/search?q=site:" + profile.WebAddress);

                HttpResponseMessage response = await client.GetAsync("https://www.google.com/search?q=site:" + profile.WebAddress);
                HttpContent responseContent = response.Content;
                string html = "";
                using (var reader = new System.IO.StreamReader(await responseContent.ReadAsStreamAsync()))
                {
                    html = await reader.ReadToEndAsync();
                }
                
                if (html.ToLower().Contains("resultstats\">"))
                {
                    var shtml = html.ToLower().Split(new[] { "resultstats\">" }, StringSplitOptions.None)[1];
                    shtml = shtml.Remove(shtml.IndexOf("<"));
                    toReturn = shtml;
                    toReturn = toReturn.Replace("�", "");
                    toReturn = toReturn.Replace("-", "");
                    toReturn = toReturn.Trim();
                    if (toReturn.IsNullOrEmpty()) toReturn = "0 results";
                    else if (!toReturn.Contains("results")) toReturn = toReturn + " results";
                }
                else if(html.Contains("To continue, please type the characters below:"))
                {
                    toReturn = "hit CAPTCHA";
                }
            }
            catch (Exception ex){ toReturn = ex.Message; }
            finally
            {
                client.Dispose();
            }
            return toReturn;
        }

        private async Task RefreshRest(string configFile, ObservableCollection<PBNProject> savedpbns, ObservableCollection<PBNProjectsFolder> savedfolders)
        {
            await Task.Run(async ()=> 
            {
                List<PersonData> allProfiles = MyFilesDatabase.GetAllProfiles();
                bool needsSave = false;

                foreach (var profile in allProfiles)
                {
                    if ((profile.InMonney && configFile == PBNCONFIGMONEY) || 
                        (profile.InPBNVault && configFile == PBNCONFIG))
                    {
                        if (!savedpbns.Any(p => p.FilePath == profile.ProjectDir))
                        {
                            bool found = false;
                            foreach (var f in savedfolders)
                            {
                                found = f.PBNProjects.Any(p => p.FilePath == profile.ProjectDir);
                                if (found) break;
                            }

                            if (!found)
                            {
                                needsSave = true;

                                PBNProject proj = new PBNProject()
                                {
                                    Name = profile.ProfileName.IsNullOrEmpty() ? profile.ProjectName : profile.ProfileName,
                                    SIType = profile.SIPBNType,
                                    FilePath = profile.ProjectDir,
                                    ProjectName ="("+ profile.ProjectName+")",
                                };
                                switch (configFile)
                                {
                                    case PBNCONFIG:
                                        if(profile.InPBNVault) Application.Current.Dispatcher.Invoke(() => { SavedPBNProjects.Add(proj); });
                                        break;

                                    case PBNCONFIGMONEY:
                                        if (profile.InMonney) Application.Current.Dispatcher.Invoke(() => { SavedMoneyProjects.Add(proj); });
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                if (needsSave)
                {
                    await resaveList(savedpbns, savedfolders, configFile);
                }
            });
        }

        private async Task RefreshPbnProjects(string configFile, ObservableCollection<PBNProject> pBNProjects, ObservableCollection<PBNProjectsFolder> pBNFolders)
        {
            string vaultDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault");
            if (!Directory.Exists(vaultDir)) return;

            bool? hadErrors = false;

            foreach (var dir in new DirectoryInfo(vaultDir).GetDirectories())
            {
                string fPath = Path.Combine(dir.FullName, configFile);
                if (!File.Exists(fPath)) continue;

                List<string> lines = File.ReadAllLines(fPath).ToList();
                PBNProjectsFolder nFolder = new PBNProjectsFolder() { FolderName = dir.Name };
                nFolder.OnVautContextMenuClick_Clicked += OnVautContextMenuClick;

                foreach (var line in lines)
                {
                    if (line.IsNullOrEmpty()) continue;
                    string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                    string dirPath = lineInfo[2];

                    hadErrors = checkDirHasErrors(ref dirPath, lineInfo[3], lineInfo[0]);


                    if (!dirPath.IsNullOrEmpty() && (hadErrors == false || hadErrors == null))
                    {
                        if (!getAnyDirsExist(dirPath, pBNFolders))
                        {
                            string pName = lineInfo[3];
                            pName = pName.Replace("(", "");
                            pName = pName.Replace(")", "");
                            pName = "(" + pName + ")";

                            string profname = lineInfo[0];
                            try
                            {
                                string pnamefromFile = await Task.Run(() => { return MyFilesDatabase.GetSubProjectPersonData(dirPath).ProfileName; });
                                profname = pnamefromFile;
                            }
                            catch
                            { }

                            if (!nFolder.PBNProjects.Any(f => f.FilePath == dirPath))
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    nFolder.PBNProjects.Add(new PBNProject() { Name = lineInfo[0], SIType = Convert.ToInt32(lineInfo[1]), FilePath = dirPath, ProjectName = pName });
                                });
                            }
                        }
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    pBNFolders.Add(nFolder);
                });
            }

            string filePath = Path.Combine(vaultDir, configFile);
            if (!File.Exists(filePath)) return;

            string[] fileLines = File.ReadAllLines(filePath);
            if (fileLines == null) return;

            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];
                if (line.IsNullOrEmpty()) continue;
                string[] lineInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                string DirPath = lineInfo[2];

                hadErrors = checkDirHasErrors(ref DirPath, lineInfo[3], lineInfo[0]);


                if (!DirPath.IsNullOrEmpty() && (hadErrors == false || hadErrors == null))
                {
                    if (!getAnyDirsExist(DirPath, pBNFolders))
                    {
                        string pName = lineInfo[3];
                        pName = pName.Replace("(", "");
                        pName = pName.Replace(")", "");
                        pName = "(" + pName + ")";

                        string profname = lineInfo[0];
                        try
                        {
                            string pnamefromFile = await Task.Run(() => { return MyFilesDatabase.GetSubProjectPersonData(DirPath).ProfileName; });
                            profname = pnamefromFile;
                        }
                        catch
                        { }

                        if (!pBNProjects.Any(f => f.FilePath == DirPath))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                pBNProjects.Add(new PBNProject() { Name = profname, SIType = Convert.ToInt32(lineInfo[1]), FilePath = DirPath, ProjectName = pName });
                            });
                        }
                    }
                }
            }

            //if (hadErrors == true)
            //{
            //    await resaveList(pBNProjects, pBNFolders, configFile);
            //}
        }

        private bool getAnyDirsExist(string dirPath, ObservableCollection<PBNProjectsFolder> pBNFolders)
        {
            foreach (var f in pBNFolders)
            {
                foreach (var pbn in f.PBNProjects)
                {
                    if (pbn.FilePath == dirPath)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool? checkDirHasErrors(ref string dirPath, string name, string profname)
        {
            bool? hadErrors = false;
            if (!Directory.Exists(dirPath) || (!File.Exists(Path.Combine(dirPath, "UserData.ini")) && !File.Exists(Path.Combine(dirPath, "ProjectData.ini"))))
            {
                hadErrors = null;
                dirPath = MyFilesDatabase.FindProjectDirByName(name, profname);
            }
            PersonData profile = null;
            try
            {
                 profile = MyFilesDatabase.GetSubProjectPersonData(dirPath);
            }
            catch { profile = null; }
            if (profile == null || (!profile.InPBNVault && !profile.InMonney))
            {
                hadErrors = true;
            }

            return hadErrors;
        }


        private async Task resaveList(ObservableCollection<PBNProject> pBNProjects, ObservableCollection<PBNProjectsFolder> pBNFolders, string configFile)
        {
            await Task.Factory.StartNew(() =>
            {
                string vaultDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault");
                if (!Directory.Exists(vaultDir)) Directory.CreateDirectory(vaultDir);

                string filePath = Path.Combine(vaultDir, configFile);

                List<string> newLines = new List<string>();

                foreach (var pbn in pBNProjects)
                {
                    checkRewrigghtAndAddPbn(configFile, pbn, ref newLines);
                }
                File.WriteAllLines(filePath, newLines);

                foreach (var f in pBNFolders)
                {
                    string dPath = Path.Combine(vaultDir, f.FolderName);
                    List<string> newfLines = new List<string>();
                    foreach (var pbn in f.PBNProjects)
                    {
                        checkRewrigghtAndAddPbn(configFile, pbn, ref newfLines);
                    }

                    if (newfLines.Count > 0)
                    {
                        if (!Directory.Exists(dPath)) Directory.CreateDirectory(dPath);
                        string filePathf = Path.Combine(dPath, configFile);
                        File.WriteAllLines(filePathf, newfLines);
                    }
                    else
                    {
                        if (Directory.Exists(dPath)) Directory.Delete(dPath, true);
                    }
                }
            });
        }

        private void checkRewrigghtAndAddPbn(string configFile, PBNProject pbn, ref List<string> newLines)
        {
            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbn.FilePath);
            if (profile.InMonney || profile.InPBNVault)
            {
                if (newLines != null)
                    newLines.Add(pbn.Name + MyFilesDatabase.SPLITTER + pbn.SIType + MyFilesDatabase.SPLITTER + pbn.FilePath + MyFilesDatabase.SPLITTER + pbn.ProjectName);

                switch (configFile)
                {
                    case PBNCONFIG:
                        profile.InMonney = false;
                        profile.InPBNVault = true;
                        break;

                    case PBNCONFIGMONEY:
                        profile.InPBNVault = false;
                        profile.InMonney = true;
                        break;

                    default:
                        break;
                }

                MyFilesDatabase.ReWrightProjData(profile, pbn.FilePath);
            }
        }


        private void OnVautContextMenuClick(object param)
        {
            try
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
                                    GetMozMetricsForProject(pbnProj, mozAPI1);
                                }

                                foreach (PBNProjectsFolder pbnProjFolder in SavedPBNProjectsFolders)
                                {
                                    foreach (PBNProject pbnProj in pbnProjFolder.PBNProjects)
                                    {
                                        GetMozMetricsForProject(pbnProj, mozAPI1);
                                    }
                                }

                                foreach (PBNProject pbnProj in SavedMoneyProjects)
                                {
                                    GetMozMetricsForProject(pbnProj, mozAPI1);
                                }

                                foreach (PBNProjectsFolder pbnProjFolder in SavedMoneyProjectsFolders)
                                {
                                    foreach (PBNProject pbnProj in pbnProjFolder.PBNProjects)
                                    {
                                        GetMozMetricsForProject(pbnProj, mozAPI1);
                                    }
                                }
                            }
                            catch (Exception ex)
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

                    case "RemovePBN":
                        PersonData profilep = MyFilesDatabase.GetSubProjectPersonData(SavedPBNProjects[SIPBN].FilePath);
                        profilep.InMonney = false;
                        profilep.InPBNVault = false;
                        MyFilesDatabase.ReWrightProjData(profilep, SavedPBNProjects[SIPBN].FilePath);
                        SavedPBNProjects.RemoveAt(SIPBN);
                        break;

                    case "RemoveMoney":
                        PersonData profilem = MyFilesDatabase.GetSubProjectPersonData(SavedMoneyProjects[SIMoney].FilePath);
                        profilem.InMonney = false;
                        profilem.InPBNVault = false;
                        MyFilesDatabase.ReWrightProjData(profilem, SavedMoneyProjects[SIMoney].FilePath);
                        SavedMoneyProjects.RemoveAt(SIMoney);
                        break;

                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        private void GetMozMetricsForProject(PBNProject pbnProj, MozscapeAPI mozAPI1)
        {
            if (!pbnProj.IsSelected) return;
            PersonData profile = MyFilesDatabase.GetSubProjectPersonData(pbnProj.FilePath);
            if (profile.WebAddress == "") return;

            string strAPIURL1 = mozAPI1.CreateAPIURL(MozscapeAPI.mozId, MozscapeAPI.mozSecret, 1, "url metrics", profile.WebAddress, "");
            string strResults1 = mozAPI1.FetchResults(strAPIURL1);
            MozscapeLinkMetric msURLMetrics1 = mozAPI1.ParseURLMetrics(strResults1);

            string pageAuthority1 = msURLMetrics1.upa;
            string domainAuthority1 = msURLMetrics1.pda;

            if (pageAuthority1.Contains('.')) pageAuthority1 = pageAuthority1.Split('.')[0];
            if (domainAuthority1.Contains('.')) domainAuthority1 = domainAuthority1.Split('.')[0];
            pbnProj.PageAuthority = "PA: " + pageAuthority1;
            pbnProj.DomainAuthority = "DA: " + domainAuthority1;
            pbnProj.AuthorityVisible = Visibility.Visible;

            Thread.Sleep(1100);
        }
    }
}
