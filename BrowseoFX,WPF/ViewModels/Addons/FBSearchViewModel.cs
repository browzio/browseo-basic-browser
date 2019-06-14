using BrowseoFX_WPF.Core;
using BrowseoFX_WPF.Models.Addons;
using Delimon.Win32.IO;
using Newtonsoft.Json;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BrowseoFX_WPF.ViewModels.Addons
{

    public class UIDReaponse
    {
        public string friendlyname { get; set; }
        public string fb_uid { get; set; }
    }
    public class FBSearchViewModel : ViewModelBase
    {
        string ModuleName = "FBSearch";
        string FBSearchUrl = "https://www.facebook.com/search/";
        string UsernameToUidRequestUrl = "http://graph.tips/beta/controller.php?request=add-fb-item&var1=&var2=";

        string SavedFolderName = "FBOGSavedUsernames";
        string SavedFolderPath { get { return Path.Combine(MyFilesDatabase.GetBaseDir(), SavedFolderName); } }
        string SavedFileName = "Usernames.txt";
        string SavedFilePath { get { return Path.Combine(SavedFolderPath, SavedFileName); } }

        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<SavedModuleData<ObservableCollection<User>>> ProjectsSavedUsernamesList { get; set; }

        private SavedModuleData<ObservableCollection<User>> selectedProjectsSavedUsernamesList;
        public SavedModuleData<ObservableCollection<User>> SelectedProjectsSavedUsernamesList
        {
            get { return selectedProjectsSavedUsernamesList; }
            set { selectedProjectsSavedUsernamesList = value; NotifyOfPropertyChange(); }
        }

        public ObservableCollection<User> SavedUsernamesList { get; set; }
        
        private string usernameInput;
        public string UsernameInput
        {
            get { return usernameInput; }
            set { usernameInput = value; NotifyOfPropertyChange(); }
        }

        #region single search properties
        public ObservableCollection<KeyValuePair<string, string>> SingleOptionsList { get; set; }

        private KeyValuePair<string, string> selectedSingleOption;
        public KeyValuePair<string, string> SelectedSingleOption
        {
            get { return selectedSingleOption; }
            set { selectedSingleOption = value; NotifyOfPropertyChange(); }
        }

        private bool isFriendsOfChecked;
        public bool IsFriendsOfChecked
        {
            get { return isFriendsOfChecked; }
            set { isFriendsOfChecked = value; NotifyOfPropertyChange(); }
        }

        private User selectedSavedUsernameSingle;
        public User SelectedSavedUsernameSingle
        {
            get { return selectedSavedUsernameSingle; }
            set { selectedSavedUsernameSingle = value; NotifyOfPropertyChange(); }
        }

        #endregion
        
        #region in common search properties
        public ObservableCollection<KeyValuePair<string, string>> InCommonOptionsList { get; set; }

        private KeyValuePair<string, string> selectedInCommonOptions;
        public KeyValuePair<string, string> SelectedInCommonOptions
        {
            get { return selectedInCommonOptions; }
            set { selectedInCommonOptions = value; NotifyOfPropertyChange(); }
        }

        private User selectedSavedUsernameInCommonA;
        public User SelectedSavedUsernameInCommonA
        {
            get { return selectedSavedUsernameInCommonA; }
            set { selectedSavedUsernameInCommonA = value; NotifyOfPropertyChange(); }
        }

        private User selectedSavedUsernameInCommonB;
        public User SelectedSavedUsernameInCommonB
        {
            get { return selectedSavedUsernameInCommonB; }
            set { selectedSavedUsernameInCommonB = value; NotifyOfPropertyChange(); }
        }

        #endregion

        #region wall search properties

        private string wallSearchText;
        public string WallSearchText
        {
            get { return wallSearchText; }
            set { wallSearchText = value; NotifyOfPropertyChange(); }
        }

        private User selectedSavedUsernameWall;
        public User SelectedSavedUsernameWall
        {
            get { return selectedSavedUsernameWall; }
            set { selectedSavedUsernameWall = value; NotifyOfPropertyChange(); }
        }

        #endregion

        #region place properties

        private string placeSearchTextA;
        public string PlaceSearchTextA
        {
            get { return placeSearchTextA; }
            set { placeSearchTextA = value; NotifyOfPropertyChange(); }
        }

        private string placeSearchTextB;
        public string PlaceSearchTextB
        {
            get { return placeSearchTextB; }
            set { placeSearchTextB = value; NotifyOfPropertyChange(); }
        }

        #endregion

        public FBSearchViewModel()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);

            ProjectsSavedUsernamesList = new ObservableCollection<SavedModuleData<ObservableCollection<User>>>();
            SavedUsernamesList = new ObservableCollection<User>();
            SingleOptionsList = new ObservableCollection<KeyValuePair<string,string>>();
            InCommonOptionsList = new ObservableCollection<KeyValuePair<string, string>>();

            LoadInCommonList();
            LoadSingleOptionsList();
        }

        private void LoadSingleOptionsList()
        {
            SingleOptionsList.Add(new KeyValuePair<string, string>("Posts with", "/stories/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Posts by", "/stories-by/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Posts commented by", "/stories-commented/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Posts tagged with", "/stories-tagged/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Photos liked by", "/photos-liked/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Photos made of", "/photos-of/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Photos tagged with", "/photos-tagged/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Photos commented by", "/photos-commented/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Events invitations for", "/events/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Events joined by", "/events-joined/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Groups joined by", "/groups/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Places liked by", "/places-liked/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Places visited by", "/places-visited/"));

            SingleOptionsList.Add(new KeyValuePair<string, string>("Restaurants visited by", "/places-visited/1436055710025360/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Bars visited by", "/places-visited/856947674344242/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Cafes visited by", "/places-visited/1585092751741608/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Nightclubs visited by", "/places-visited/1588881098065695/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Shopping places visited by", "/places-visited/1631302203768899/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Hotels visited by", "/places-visited/739776352805342/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Landmarks visited by", "/places-visited/440293872820865/places/intersect/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Museums visited by", "/places-visited/197817313562497/places/intersect/"));

            SingleOptionsList.Add(new KeyValuePair<string, string>("Pages liked by", "/pages-liked/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Apps used by", "/apps-used/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Videos with", "/videos/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Videos by", "/videos-by/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Videos liked by", "/videos-liked/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Videos commented by", "/videos-commented/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Friends of", "/friends/"));
            SingleOptionsList.Add(new KeyValuePair<string, string>("Relatives of", "/relatives/"));
        }

        private void LoadInCommonList()
        {
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Places visited by", "/places-visited/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Pages liked by", "/pages-liked/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Photos liked by", "/photos-liked/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Photos commented by", "/photos-commented/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Photos tagged with", "/photos-of/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Posts commented by", "/stories-commented/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Common events among", "/events/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Common groups among", "/groups/"));
            InCommonOptionsList.Add(new KeyValuePair<string, string>("Apps used by", "/apps-used/"));
        }

        private void User_OnCommand_Raised(User user, object param)
        {
            if (SelectedProjectsSavedUsernamesList != null && SelectedProjectsSavedUsernamesList.SavedData != null)
            {
                SelectedProjectsSavedUsernamesList.SavedData.Remove(user);
            }
            
            SaveUsernames(false);
        }
        
        private async void OnCommandFromView_Raised(object obj)
        {
            try
            {
                switch (obj as string)
                {
                    case "Refresh":
                        LoadSavedUsernames();
                        break;

                    case "DeleteProjectFolder":
                        if (SelectedProjectsSavedUsernamesList != null)
                        {
                            ProjectsSavedUsernamesList.Remove(SelectedProjectsSavedUsernamesList);
                            string projéctFolderDir = Path.Combine(SavedFolderPath, SelectedProjectsSavedUsernamesList.ProjectName); 
                            await SelectedProjectsSavedUsernamesList.DeleteSavedDataFolder(projéctFolderDir);
                        }
                        break;

                    case "SAVE":
                        if(ProjectsSavedUsernamesList.Count == 0)
                            LoadSavedUsernames();

                        SaveUsernames(true);
                        break;

                    case "SearchSINGLE":
                        if (SelectedSingleOption.Equals(default(KeyValuePair<string, string>)) || SelectedSavedUsernameSingle == null)
                            return;

                        BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate
                        (
                            FBSearchUrl + SelectedSavedUsernameSingle.ID + SelectedSingleOption.Value
                        );
                        break;

                    case "SearchINCOMMON":
                        if (SelectedInCommonOptions.Equals(default(KeyValuePair<string, string>)) || 
                            SelectedSavedUsernameInCommonA == null ||
                            SelectedSavedUsernameInCommonB == null)
                            return;

                        //https://www.facebook.com/search/564872245/places-visited/564872245/places-visited/intersect
                        BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate
                        (

                            FBSearchUrl + SelectedSavedUsernameInCommonA.ID + SelectedInCommonOptions.Value + SelectedSavedUsernameInCommonB.ID + SelectedInCommonOptions.Value + "/intersect"
                        );
                        break;

                    case "SearchWALL":
                        //https://www.facebook.com/search/str/tech/stories-keyword/
                        BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate
                        (
                            FBSearchUrl + "str/" + WallSearchText + "/stories-keyword/"
                        );
                        break;

                    case "SearchWALLUSER":
                        if (SelectedSavedUsernameWall == null)
                            return;

                        //https://www.facebook.com/search/str/tech/stories-keyword/564872245/stories/intersect
                        BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate
                        (
                            FBSearchUrl + "str/" + WallSearchText + "/stories-keyword/" + SelectedSavedUsernameWall.ID + "/stories/intersect"
                        );
                        break;

                    case "SearchPLACE":
                        //https://www.facebook.com/search/str/berlin/pages-named/residents/present/intersect/str/barcelona/pages-named/visitors/intersect
                        BrowseoFXManager.Instance.TabbrowserHandler.SelectedTabNavigate
                        (
                            FBSearchUrl + "str/" + PlaceSearchTextA + "/pages-named/residents/present/intersect/str/" + PlaceSearchTextB + "/pages-named/visitors/intersect"
                        );
                        break;

                    default:
                        break;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("FBSearch Error: " + ex.Message);
            }
        }

        private async void SaveUsernames(bool request)
        {
            if (request)
            {
                if (UsernameInput.IsNullOrEmpty() ||
                    ProjectsSavedUsernamesList.Any(u => u.ProjectName == GloableProfData.PData.ProjectName && u.SavedData.Any(t => t.Username == UsernameInput)))
                    return;

                UIDReaponse uidResponse = null;

                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    await Task.Run(() =>
                    {
                        using (var client = new WebClient())
                        {
                            client.Proxy = MyFilesDatabase.GetRequestsProxy();

                            string uidJson = client.DownloadString(UsernameToUidRequestUrl + UsernameInput);
                            uidResponse = JsonConvert.DeserializeObject<UIDReaponse>(uidJson.ToLower());
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FBSearch Couldn't save username. " + ex.Message);
                }
                Mouse.OverrideCursor = null;

                if (uidResponse == null) return;

                AddUserToList(new User() { ID = uidResponse.fb_uid, Username = uidResponse.friendlyname });
            }

            if (!Directory.Exists(SavedFolderPath))
                Directory.CreateDirectory(SavedFolderPath);

            foreach (var savedData in ProjectsSavedUsernamesList)
            {
                var projectSaveDir = Path.Combine(SavedFolderPath, savedData.ProjectName);
                if (!Directory.Exists(projectSaveDir))
                    Directory.CreateDirectory(projectSaveDir);

                var projectsSaveFilePath = Path.Combine(projectSaveDir, SavedFileName);
                await savedData.Save(projectsSaveFilePath);
            }

            AddSavedUsernamesSingleLis();
        }

        private async void LoadSavedUsernames()
        {
            if (!Directory.Exists(SavedFolderPath)) return;
            ProjectsSavedUsernamesList.Clear();

            DirectoryInfo dInfo = new DirectoryInfo(SavedFolderPath);
            foreach (var folder in dInfo.GetDirectories())
            {
                var filepath = Path.Combine(folder.FullName, SavedFileName);
                if (!File.Exists(filepath)) continue;

                var fileData = new SavedModuleData<ObservableCollection<User>>(ModuleName, folder.Name);
                await fileData.Load(filepath);
                AddProjectFolderToList(fileData);
            }

            AddSavedUsernamesSingleLis();
        }

        private void AddProjectFolderToList(SavedModuleData<ObservableCollection<User>> fileData)
        {
            var projectsData = ProjectsSavedUsernamesList.FirstOrDefault(p => p.ProjectName == fileData.ProjectName);
            if(projectsData == null)
            {
                SetChildDataEvents(fileData.SavedData);

                ProjectsSavedUsernamesList.Add(fileData);
            }
            else
            {
                ProjectsSavedUsernamesList.Remove(fileData);

                SetChildDataEvents(fileData.SavedData);

                ProjectsSavedUsernamesList.Add(fileData);
            }
        }

        private void SetChildDataEvents(ObservableCollection<User> savedData)
        {
            foreach (var user in savedData)
            {
                user.OnCommand_Raised -= User_OnCommand_Raised;
                user.OnCommand_Raised += User_OnCommand_Raised;
            }
        }

        private void AddUserToList(User user)
        {
            user.OnCommand_Raised -= User_OnCommand_Raised;
            user.OnCommand_Raised += User_OnCommand_Raised;
            var projectsData = ProjectsSavedUsernamesList.FirstOrDefault(p => p.ProjectName == GloableProfData.PData.ProjectName);
            if (projectsData == null)
            {
                projectsData = new SavedModuleData<ObservableCollection<User>>(ModuleName, GloableProfData.PData.ProjectName);
                projectsData.SavedData = new ObservableCollection<User>();
                projectsData.SavedData.Add(user);
                ProjectsSavedUsernamesList.Add(projectsData);
            }
            else
            {
                if (projectsData.SavedData == null)
                    projectsData.SavedData = new ObservableCollection<User>();

                projectsData.SavedData.Add(user);
            }
        }

        private void AddSavedUsernamesSingleLis()
        {
            SavedUsernamesList.Clear();
            foreach (var item in ProjectsSavedUsernamesList)
            {
                if (item.SavedData == null) continue;

                foreach (var user in item.SavedData)
                {
                    SavedUsernamesList.Add(user);
                }
            }
        }
    }
}
