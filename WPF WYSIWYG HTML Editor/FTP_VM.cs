using Organiser.Common.Classes;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_WYSIWYG_HTML_Editor.Helpers;
using WPF_WYSIWYG_HTML_Editor.Models;
using WPF_WYSIWYG_HTML_Editor.XAML;

namespace WPF_WYSIWYG_HTML_Editor
{
    /**
 * sidebar.php
<div class="box">
<span>{SideBarTitle}</span>
<p>{SideBarText}</p>
</div>
 * 
 * footer.php
<p>{FooterText}</p>
 * 
 * header.php
<div id="header">
<div class="width">
<img src="images/logo.png" class="logo" />
<div id="menu">
    <a href="#" id="menu-mobile">Menu <img src="/images/toggle.png" align="right" /></a>
    <ul class="menu">
        <li><a href="#">Home</a></li>
        <li><a href="#">Link</a></li>
        <li><a href="#">Link</a></li>
        <li><a href="#">Link</a></li>
        <li class="search">
            <img src="images/rss.png" />
            <img src="images/pinterest.png" />
            <img src="images/twitter.png" />
            <img src="images/facebook.png" />
        </li>
    </ul>
</div>
</div>
</div>
 * 
 * index.php
<title>{PageTitleHere}</title>
 * 
 * content.php
can be anything done by wysiwyg
 * */

    public class FTP_VM : INotifyPropertyChanged
    {
        public const string RPLACER_PAGETITLE_INDEX = "{PageTitleHere}";
        
        public const string RPLACER_FOOTERTEXT_FOOTER = "{FooterText}";
        
        public const string RPLACER_TITLETEXT_SIDEBAR = "{SideBarTitle}";
        public const string RPLACER_TEXTTEXT_SIDEBAR = "{SideBarText}";
        
        public const string RPLACER_LINK_HOME = "{HOME_LINK}";
       // public const string RPLACER_LINK_HOME = "{DEFAULT_LINK}";
        
        public const string RPLACER_LINK_CATEGORY = "{LinkToPost}";
        public const string RPLACER_LINK_CATEGORY_TITLE = "{PostTitle}";
        //public const string RPLACER_LINK_CATEGORY_TITLE = "{PostTitle}";

        public const string FILENAME_CATEGORY = "category.php";
        public const string FILENAME_CONTENT = "content.php";
        public const string FILENAME_FOOTER = "footer.php";
        public const string FILENAME_HEADER = "header.php";
        public const string FILENAME_INDEX = "index-category.php";
        public const string FILENAME_SIDEBAR = "sidebar.php";

        public ICommand CreatNewClick { get; set; }

        private ObservableCollection<FTPprofile> savedFtbSites;
        public ObservableCollection<FTPprofile> SavedFtbSites
        {
            get { return savedFtbSites; }
            set { savedFtbSites = value; }
        }

        private PersonData mPData;

        private string mDirToProjects, mFilePath;

        public FTP_VM()
        {
            CreatNewClick = new RelayCommand(On_CreatNewClick);
            SavedFtbSites = new ObservableCollection<FTPprofile>();
        }

        internal void SetProfile(PersonData profile)
        {
            mPData = profile;
            mDirToProjects = Path.Combine(MyFilesDatabase.GetBaseDir(), "FTP_Projects", mPData.ProfileName);
            mFilePath = Path.Combine(MyFilesDatabase.GetBaseDir(), "FTP_Projects", mPData.ProfileName, "FtpProjects.txt");
            FillList();
        }

        public void On_CreatNewClick(object param)
        {
            FTPprofile prof = new FTPprofile();
            CreateNewFTPprojectWindow cnftpw = new CreateNewFTPprojectWindow();
            cnftpw.DataContext = prof;
            cnftpw.ShowDialog();
            if (cnftpw.OkClicked)
            {
                SavedFtbSites.Add(prof);
                SaveList();
                UploadNew(prof);
            }
        }

        private void UploadNew(FTPprofile prof)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            string basePath = AppDomain.CurrentDomain.BaseDirectory + "\\Default-Theme\\";
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(prof.Username, prof.Password);

                #region default files and folder creation
                WebClient c = new WebClient();

                //style
                string styleFile = basePath + "style.css";
                client.UploadFile(prof.Link+"/style.css", "STOR", styleFile);

                //upload js
                string fromDirjs = basePath + "js";
                DirectoryInfo dirInfoJs = new DirectoryInfo(fromDirjs);
                bool dirExists = false;
                try
                {
                    c.DownloadString(prof.Homelink+"/js/");
                    dirExists = true;
                }
                catch
                {
                    dirExists = false;
                }
                if (!dirExists)
                {
                    WebRequest requestJS = WebRequest.Create(prof.Link+"/js");
                    requestJS.Method = WebRequestMethods.Ftp.MakeDirectory;
                    requestJS.Credentials = client.Credentials;
                    using (var resp = (FtpWebResponse)requestJS.GetResponse())
                    {
                       // Console.WriteLine(resp.StatusCode);
                    }
                }
                foreach (FileInfo file in dirInfoJs.GetFiles())
                {
                    client.UploadFile(prof.Link+"/js/" + file.Name, "STOR", file.FullName);
                }

                //upload images
                string fromDirImages = basePath+"images";
                DirectoryInfo dirInfoImages = new DirectoryInfo(fromDirImages);

                dirExists = false;
                try
                {
                    c.DownloadString(prof.Homelink+"/images/");
                    dirExists = true;
                }
                catch
                {
                    dirExists = false;
                }
                if (!dirExists)
                {
                    WebRequest request = WebRequest.Create(prof.Link+"/images");
                    request.Credentials = client.Credentials;
                    request.Method = WebRequestMethods.Ftp.MakeDirectory;
                    using (var resp = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine(resp.StatusCode);
                    }
                }
                foreach (FileInfo file in dirInfoImages.GetFiles())
                {
                    client.UploadFile(prof.Link+"/images/" + file.Name, "STOR", file.FullName);
                }
                #endregion

                string contentText = "", footerText = "", headerText = "", indexText = "", sidebarText = "";
                string fromDir = basePath;
                DirectoryInfo dirInfo = new DirectoryInfo(fromDir);

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    switch (file.Name)
                    {
                        case FILENAME_CONTENT:
                            contentText = File.ReadAllText(file.FullName);
                            break;
                        case FILENAME_FOOTER:
                            footerText = File.ReadAllText(file.FullName);
                            footerText = footerText.Replace(RPLACER_FOOTERTEXT_FOOTER, "Hello World Footter!!");
                            break;
                        case FILENAME_HEADER:
                            headerText = File.ReadAllText(file.FullName);
                            break;
                        case FILENAME_INDEX:
                            indexText = File.ReadAllText(file.FullName);
                            indexText = indexText.Replace(RPLACER_PAGETITLE_INDEX, "HELLO WORLD");
                            break;
                        case FILENAME_SIDEBAR:
                            sidebarText = File.ReadAllText(file.FullName); ;
                            sidebarText = sidebarText.Replace(RPLACER_TITLETEXT_SIDEBAR, "Super Awesome Side Bar Title");
                            sidebarText = sidebarText.Replace(RPLACER_TEXTTEXT_SIDEBAR, "Super awesome side bar text goes here it is awesome text wohoo.");
                            break;
                        default:
                            break;
                    }
                }

                string toDir = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNToUpload");

                string pathToContentFile = Path.Combine(toDir, FILENAME_CONTENT);
                File.WriteAllText(pathToContentFile, contentText);

                string pathToFooterFile = Path.Combine(toDir, FILENAME_FOOTER);
                File.WriteAllText(pathToFooterFile, footerText);

                string pathToHeaderFile = Path.Combine(toDir, FILENAME_HEADER);
                File.WriteAllText(pathToHeaderFile, headerText);

                string pathToIndexFile = Path.Combine(toDir, FILENAME_INDEX);
                File.WriteAllText(pathToIndexFile, indexText);

                string pathToSideBArFile = Path.Combine(toDir, FILENAME_SIDEBAR);
                File.WriteAllText(pathToSideBArFile, sidebarText);

                dirExists = false;
                try
                {
                    c.DownloadString(prof.Homelink + "/default/");
                    dirExists = true;
                }
                catch
                {
                    dirExists = false;
                }
                if (!dirExists)
                {
                    WebRequest requestJS = WebRequest.Create(prof.Link + "/default");
                    requestJS.Method = WebRequestMethods.Ftp.MakeDirectory;
                    requestJS.Credentials = client.Credentials;
                    using (var resp = (FtpWebResponse)requestJS.GetResponse()){}
                }
                client.UploadFile(prof.Link+"/default/" + FILENAME_CONTENT, "STOR", pathToContentFile);

                client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + "testingFolder" + "/" + FILENAME_FOOTER, "STOR", pathToFooterFile);
                client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + "testingFolder" + "/" + FILENAME_HEADER, "STOR", pathToHeaderFile);
                client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + "testingFolder" + "/" + FILENAME_INDEX, "STOR", pathToIndexFile);
                client.UploadFile("ftp://192.185.150.11/public_html/rssey.com/" + "testingFolder" + "/" + FILENAME_SIDEBAR, "STOR", pathToSideBArFile);

                client.Dispose();
            }

            Mouse.OverrideCursor = null;
        }

        private void SaveList()
        {
            if (!Directory.Exists(mDirToProjects)) Directory.CreateDirectory(mDirToProjects);

            string fileText = "";
            foreach (FTPprofile prof in SavedFtbSites)
            {
                fileText += prof.Link + MyFilesDatabase.SPLITTER +
                            prof.Homelink + MyFilesDatabase.SPLITTER + 
                            prof.Password + MyFilesDatabase.SPLITTER + 
                            prof.Title + MyFilesDatabase.SPLITTER + 
                            prof.Username + Environment.NewLine;
            }
            File.WriteAllText(mFilePath,fileText);
        }

        public void FillList()
        {
            if (!Directory.Exists(mDirToProjects)) return;
            if (!File.Exists(mFilePath)) return;
            SavedFtbSites.Clear();

            foreach (string line in File.ReadAllLines(mFilePath))
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
                string[] allInfo = line.Split(new string[] { MyFilesDatabase.SPLITTER }, StringSplitOptions.None);
                if (allInfo == null || allInfo.Length == 0) continue;
                SavedFtbSites.Add(new FTPprofile()
                {
                    Link = allInfo[0],
                    Homelink = allInfo[1],
                    Password = allInfo[2],
                    Title = allInfo[3],
                    Username = allInfo[4]
                });
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
