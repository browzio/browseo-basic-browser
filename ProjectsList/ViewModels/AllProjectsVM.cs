using BrowserAndFeatures;
using Organiser.Common;
using ProjectsList.Helpers;
using ProjectsList.Models;
using ProjectsList.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectsList.ViewModels
{
    public class AllProjectsVM : INotifyPropertyChanged
    {
        #region projects region

        private ICommand createNewProject;
        public ICommand CreateNewProject
        {
            get { return createNewProject; }
            set { createNewProject = value; }
        }

        private ICommand contextMenuClick;
        public ICommand ContextMenuClick
        {
            get { return contextMenuClick; }
            set { contextMenuClick = value; }
        }

        private ICommand save;
        public ICommand Save
        {
            get { return save; }
            set { save = value; }
        }

        public ICommand CloseTabCommand { get; private set; }

        private ObservableCollection<PluginBrowser> browsers;
        public ObservableCollection<PluginBrowser> Browsers
        {
            get { return browsers; }
            set { browsers = value; }
        }

        private PluginBrowser selectedBrowser;
        public PluginBrowser SelectedBrowser
        {
            get { return selectedBrowser; }
            set { selectedBrowser = value;
            PropertyChanged(this, new PropertyChangedEventArgs("SelectedBrowser"));
            }
        }


        private ObservableCollection<ProjectData> _projects;
        public ObservableCollection<ProjectData> Projects
        {
            get { return _projects; }
            set { _projects = value; }
        }

        private int sIProjects;
        public int SIProjects
        {
            get { return sIProjects; }
            set { sIProjects = value; }
        }


        private PersonData currentProject;
        public PersonData CurrentProject
        {
            get { return currentProject; }
            set
            {
                if (currentProject != value)
                {
                    currentProject = value;
                    //FilesDatabase.CreateBrowserPersonData(currentProject);
                    //LoadPluginCommand.Execute(new PluginCatalogEntry()
                    //{
                    //    Name = currentProject.ProjectName,
                    //    Version="1.0",
                    //    Description = currentProject.ProxyIP,
                    //    AssemblyPath = @"..\..\..\Plugins\BrowserOutputPath\NichAndBrowser.dll",
                    //    MainClass = "NichAndBrowser.Plugin"
                    //});
                }
            }
        }

        public AllProjectsVM()
        {
            Projects = new ObservableCollection<ProjectData>();
            Browsers = new ObservableCollection<PluginBrowser>();

            CreateNewProject = new RelayCommand(NewProject);
            ContextMenuClick = new RelayCommand(OnContextMenuItemClicked);
            Save = new RelayCommand(SaveProjectClicked);
            CloseTabCommand = new DelegateCommand<PluginBrowser>(CloseTab);

            loadUpProjects();
        }

        private void CloseTab(PluginBrowser plugin)
        {
            try
            {
                bool changeSelection = (plugin == SelectedBrowser);
                int selectedIndex = Browsers.IndexOf(plugin);
                Browsers.Remove(plugin);
                if (changeSelection)
                {
                    int count = Browsers.Count;

                    if (count == 0)
                    {
                        SelectedBrowser = null;
                    }
                    else
                    {
                        if (selectedIndex >= count) selectedIndex = count - 1;
                        SelectedBrowser = Browsers[selectedIndex];
                    }
                }
            }
            catch { }
        }

        public void loadUpProjects()
        {
            Projects.Clear();
            foreach (ProjectData proj in FilesDatabase.GetProjects())
            {
                Projects.Add(proj);
            }
        }

        public void NewProject(object param)
        {
            PersonData pdata = new PersonData();
            CreateProjectWindow projWindow = new CreateProjectWindow();
            projWindow.DataContext = pdata;
            projWindow.ShowDialog();
            if (!projWindow.isSave) return;
            FilesDatabase.CreatProject(pdata);
            loadUpProjects();
        }

        public void SaveProjectClicked(object param)
        {
            switch (param.ToString())
            {
                case "Save":
                    try
                    {
                        FilesDatabase.CreatProject(Projects[SIProjects].PersonData);
                    }
                    catch
                    {
                        MessageBox.Show("select project to save");
                    }
                    break;

                case "SaveAll":
                    foreach (ProjectData proj in Projects)
                    {
                        FilesDatabase.CreatProject(proj.PersonData);
                    }
                    break;

                default:
                    return;
            }

            MessageBox.Show("Save Success.");
        }

        public void OnContextMenuItemClicked(object param)
        {
            switch (param.ToString())
            {
                case "Delete":
                    FilesDatabase.DeleteProject(Projects[SIProjects]);
                    Projects.Remove(Projects[SIProjects]);
                    break;

                case "ShowProfileData":
                    if (FilesDatabase.HasMultipleProfiles(Projects[SIProjects].ProjectName))
                    {
                        SelectProfileWindow selectProfile = new SelectProfileWindow(Projects[SIProjects].ProjectName);
                        selectProfile.ShowDialog();
                        if (!selectProfile.OkClicked) return;
                        CreateProjectWindow projWindow = new CreateProjectWindow();
                        PersonData pd = FilesDatabase.GetSubProjectPersonData(selectProfile.SelectedProfileFilePath);
                        projWindow.DataContext = pd;
                        if (selectProfile.SelectedProfileFilePath.Contains("ProjectData.ini"))
                            projWindow.DataContext = Projects[SIProjects].PersonData;
                        projWindow.init("Save", pd.ProjectName, true, selectProfile.SelectedProfileFilePath.Contains("ProjectData.ini"));
                        projWindow.Show();
                    }
                    else
                    {
                        CreateProjectWindow projWindow = new CreateProjectWindow();
                        projWindow.DataContext = Projects[SIProjects].PersonData;
                        projWindow.init("Save", Projects[SIProjects].ProjectName, false, false);
                        projWindow.Show();
                    }
                    break;

                case "AddProfile":
                    PersonData pdata = new PersonData();
                    pdata.ProjectName = Projects[SIProjects].PersonData.ProjectName;
                    pdata.FirstName = Projects[SIProjects].PersonData.FirstName;
                    pdata.LastName = Projects[SIProjects].PersonData.LastName;
                    pdata.Email = Projects[SIProjects].PersonData.Email;
                    pdata.Password = Projects[SIProjects].PersonData.Password;
                    pdata.Username = Projects[SIProjects].PersonData.Username;
                    pdata.ProxyIP = Projects[SIProjects].PersonData.ProxyIP;
                    pdata.ProxyPort = Projects[SIProjects].PersonData.ProxyPort;
                    pdata.ProxyUsername = Projects[SIProjects].PersonData.ProxyUsername;
                    pdata.ProxyPassword = Projects[SIProjects].PersonData.ProxyPassword;
                    pdata.PhoneNumber = Projects[SIProjects].PersonData.PhoneNumber;
                    pdata.CmbSelectedIndexSex = Projects[SIProjects].PersonData.CmbSelectedIndexSex;
                    pdata.CmbSelectedIndexDay = Projects[SIProjects].PersonData.CmbSelectedIndexDay;
                    pdata.CmbSelectedIndexMonth = Projects[SIProjects].PersonData.CmbSelectedIndexMonth;
                    pdata.BirthdayYear = Projects[SIProjects].PersonData.BirthdayYear;
                    pdata.Address = Projects[SIProjects].PersonData.Address;
                    pdata.Notes = Projects[SIProjects].PersonData.Notes;
                    CreateProjectWindow addProjWindow = new CreateProjectWindow();
                    addProjWindow.DataContext = pdata;
                    addProjWindow.init("Save Profile", Projects[SIProjects].ProjectName, false,false);
                    addProjWindow.projName.IsEnabled = false;
                    addProjWindow.ShowDialog();
                    if (!addProjWindow.isSave) return;
                    FilesDatabase.CreatSubProjectUser(pdata);
                    break;

                case "LaunchInBrowser":
                    //CurrentProject = Projects[SIProjects].PersonData;
                    if (Browsers.Count > 0)
                        //{
                        //    if (SelectedBrowser.PData.ProjectName != Projects[SIProjects].ProjectName)
                        //    {
                        //        FilesDatabase.FlipCache(SelectedBrowser.PData.ProjectName, Projects[SIProjects].ProjectName, true);
                        //    }
                        CloseTab(Browsers[0]);
                    //}
                    //else
                    //{
                    //    FilesDatabase.FlipCache("", Projects[SIProjects].ProjectName, false);
                    //}
                    //DynamicBrowser.IECache.ClearCache();
                    SelectedBrowser = new PluginBrowser(Projects[SIProjects].PersonData);

                    Browsers.Add(SelectedBrowser);
                    break;

                default:
                    break;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        internal void Closed()
        {
           // DynamicBrowser.Browser.KillWebCore();
        }
    }
}
