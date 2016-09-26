using GoViral.Instagram.InstModels;
using GoViral.Windows;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Xilium.CefGlue.Client;

namespace GoViral.Instagram.InstViewModels
{
    public class InstaDominateVM : ViewModelBase
    {
        public event Action<string> OnSendContentToSorter = delegate { };

        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<InstaFolder> Folders { get; set; }
        private InstaFolder selectedFolder;
        public InstaFolder SelectedFolder
        {
            get { return selectedFolder; }
            set { selectedFolder = value; RaisePropertyChanged("SelectedFolder"); }
        }


        private WindowsFormsHost wfh;
        public WindowsFormsHost WebBrowserHost
        {
            get
            {
                if (wfh == null)
                {
                    RefreshBrowser();
                }
                return wfh;
            }
            set
            {
                wfh = value; RaisePropertyChanged("WebBrowserHost");
            }
        }

        public BrowserCntrl WebBrowser { get; set; }
        private string browserPreviewStatus;
        public string BrowserPreviewStatus
        {
            get { return browserPreviewStatus; }
            set { browserPreviewStatus = value; RaisePropertyChanged("BrowserPreviewStatus"); }
        }


        public InstaDominateVM()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            Folders = new ObservableCollection<InstaFolder>();

            LoadSavedList();
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            if (Mouse.OverrideCursor == Cursors.Wait) return;

            using (new WaitCursor())
            {
                try
                {
                    string param = obj as string;
                    switch (param)
                    {
                        case "NEWFOLDER":
                            Mouse.OverrideCursor = null;
                            await AddNewFolder();
                            break;

                        case "MULTILINKS":
                            break;

                        case "DEDUPE":
                            break;

                        case "SAVE":
                            await SaveList();
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        ex.InnerException.Message.Show();
                    else
                        ex.Message.Show();
                }
            }
        }

        private async Task<bool> AddNewFolder()
        {
            SetNameAndDataWindow setFolderNAmeWindow = new SetNameAndDataWindow();
            setFolderNAmeWindow.Title = "Create Folder";
            setFolderNAmeWindow.tblockInfo.Text = "Write in the name for the folder you want to create.";
            setFolderNAmeWindow.ShowDialog();
            if (setFolderNAmeWindow.OkClicked && !setFolderNAmeWindow.tbInputText.Text.IsNullOrEmpty())
            {
                if (Folders.Any(f => f.FolderTitle.ToLower().Trim() == setFolderNAmeWindow.tbInputText.Text.ToLower().Trim()))
                {
                    (setFolderNAmeWindow.tbInputText.Text + " Already exists, use a different name.").Show();
                    await AddNewFolder();
                }
                else
                {

                    InstaFolder folder = new InstaFolder() { FolderTitle = setFolderNAmeWindow.tbInputText.Text };
                    folder.OnSendContentToSorter -= SelectedFolder_OnSendContentToSorter;
                    folder.OnSendContentToSorter += SelectedFolder_OnSendContentToSorter;
                    folder.SetResponsesEvents();
                    Folders.Add(folder);

                    await SaveList();

                    return true;
                }
            }

            return false;
        }

        private async Task SaveList()
        {
            await Task.Run(() => 
            {
                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "insteo", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                foreach (var folder in Folders)
                {
                    string folderPath = Path.Combine(dirPath, folder.FolderTitle);
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    string filePath = Path.Combine(folderPath, InstaVM.PATHDOMINATEFILE);

                    string json = JsonConvert.SerializeObject(folder);
                    File.WriteAllText(filePath, json);
                }

            });
        }

        private Task<List<string>> LoadJsonList()
        {
            return Task.Run(() =>
            {
                string dirPath = Path.Combine(MyFilesDatabase.GetBaseDir(), "insteo", GloableProfData.PData.ProjectName);
                if (!Directory.Exists(dirPath)) return null;

                List<string> jsonList = new List<string>();
                foreach (var folder in new DirectoryInfo(dirPath).GetDirectories())
                {
                    string filePath = Path.Combine(folder.FullName, InstaVM.PATHDOMINATEFILE);
                    if (!File.Exists(filePath)) continue;

                    jsonList.Add(File.ReadAllText(filePath));
                }
                return jsonList;
            });
        }

        private async void LoadSavedList()
        {
            List<string> jsonList = await LoadJsonList();
            if (jsonList == null) return;

            //JsonSerializerSettings jsSettings = new JsonSerializerSettings();
            //jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            foreach (var folder in jsonList)
            {
                InstaFolder loadedFolder = JsonConvert.DeserializeObject<InstaFolder>(folder);
                if (loadedFolder == null) return;

                foreach (var l in loadedFolder.Links)
                {
                    l.OnRaisedCommandToViewModel += L_OnRaisedCommandToViewModel;
                }
                Folders.Add(loadedFolder);
                loadedFolder.SetResponsesEvents();
                loadedFolder.OnSendContentToSorter -= SelectedFolder_OnSendContentToSorter;
                loadedFolder.OnSendContentToSorter += SelectedFolder_OnSendContentToSorter;
                if (loadedFolder.InstaResponseLists != null)
                {
                    loadedFolder.InstaResponseLists.SetAllChildEvents();
                    loadedFolder.InstaResponseLists.RasieAllCountsChanged();
                }
            }

            if (Folders.Count > 0) SelectedFolder = Folders[0];
        }

        internal async void OnReceivedFromSearch(InstaResponseLists responseList)
        {
            if (Folders.Count > 0)
            {
                ToFolderChooserWindow fcw = new ToFolderChooserWindow() { DataContext = this };
                fcw.dpName.Visibility = Visibility.Collapsed;
                fcw.dpUrl.Visibility = Visibility.Collapsed;
                if (fcw.ShowDialog() == false) return;
                if (SelectedFolder == null) return;
                if (SelectedFolder.InstaResponseLists == null) SelectedFolder.InstaResponseLists = new InstaResponseLists("");

                foreach (var u in responseList.UserSearchResponse)
                {
                    SelectedFolder.InstaResponseLists.UserSearchResponse.Add(u);

                    InstalLink l = new InstalLink() { Name = u.Username, Url = Convert.ToString(u.Id), Id = Convert.ToString(u.Id) };
                    l.OnRaisedCommandToViewModel += L_OnRaisedCommandToViewModel;
                    SelectedFolder.Links.Add(l);
                }

                foreach (var t in responseList.TagsSearchResponse)
                {
                    SelectedFolder.InstaResponseLists.TagsSearchResponse.Add(t);

                    InstalLink l = new InstalLink() { Name = t.Name, Url = Convert.ToString(t.MediaCount), Id = "IsATag" };
                    l.OnRaisedCommandToViewModel += L_OnRaisedCommandToViewModel;
                    SelectedFolder.Links.Add(l);
                }

                foreach (var m in responseList.MediaSearchResponse)
                {
                    SelectedFolder.InstaResponseLists.MediaSearchResponse.Add(m);

                    InstalLink l = new InstalLink() { Name = m.User.Username, Url = m.Link , Id = m.Id};
                    l.OnRaisedCommandToViewModel += L_OnRaisedCommandToViewModel;
                    SelectedFolder.Links.Add(l);
                }
                SelectedFolder.SetResponsesEvents();
                SelectedFolder.OnSendContentToSorter -= SelectedFolder_OnSendContentToSorter;
                SelectedFolder.OnSendContentToSorter += SelectedFolder_OnSendContentToSorter;
                //SelectedFolder.InstaResponseLists = responseList;
            }
            else
            {
                bool didCreate = await AddNewFolder();
                if (didCreate) OnReceivedFromSearch(responseList);
                return;
            }
        }

        private void SelectedFolder_OnSendContentToSorter(string content)
        {
            OnSendContentToSorter(content);
        }

        private void L_OnRaisedCommandToViewModel(InstalLink savedLink, string command)
        {
            switch (command)
            {
                case "DoubleClickMediaItem":
                    string link = "";
                    if(savedLink.Url.StartsWith("http://") || savedLink.Url.StartsWith("https://"))
                    {
                        link = savedLink.Url;
                    }
                    else
                    {
                        link = InstaVM.LINK_INSTAGRAM + savedLink.Name;
                    }


                    WebBrowser.Navigate(link);
                    break;

                default:
                    break;
            }
        }

        #region browser load and events
        void Folder_OnLoadInBrowser(string url)
        {
            WebBrowser.Navigate(url);
        }

        void WebBrowser_OnBrowserLoadingChanged(bool isLoading)
        {
            if (!isLoading)
            {
                BrowserPreviewStatus = "Loaded.";
            }
            else
            {
                BrowserPreviewStatus = "Loading...";
            }
        }


        public void RefreshBrowser()
        {
            string url = "https://www.instagram.com/";
            if (WebBrowser != null)
            {
                try
                {
                    if (WebBrowser.CBrowser != null && WebBrowser.GetBrowser() != null && WebBrowser.GetTheMainFrame() != null)
                    {
                        url = WebBrowser.GetTheMainFrame().Url;
                    }
                }
                catch { }
                WebBrowser.DisposeBrowserComponents();
            }

            if (wfh != null)
            {
                wfh.Child.Dispose();
            }

            WebBrowser = new Xilium.CefGlue.Client.BrowserCntrl();
            WebBrowser.OnBrowserLoadingChanged += WebBrowser_OnBrowserLoadingChanged;
            WebBrowser.init(url, BrowserSettimgs.FlashEnabled, BrowserSettimgs.JavascriptEnabled, BrowserSettimgs.JavaEnabled);
            if (wfh == null)
                wfh = new WindowsFormsHost();

            wfh.Child = WebBrowser;
            RaisePropertyChanged("WebBrowserHost");

            //WebBrowser.Reload();
        }

        public void DisposeBrowser()
        {
            if (WebBrowser != null)
                WebBrowser.DisposeBrowserComponents();
        }

        #endregion
    }
}
