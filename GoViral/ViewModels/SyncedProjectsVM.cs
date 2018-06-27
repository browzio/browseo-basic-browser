using GoViral.Models;
using GoViral.Windows;
using Organiser.Common.Classes;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GoViral.ViewModels
{
    public class SyncedProjectsVM : ViewModelBase
    {
        #region props events and Ctor
        public const string TypeOfGoViral = "GoViral";
        public const string TypeOfSEO = "SocialEngagerOptimizer";
        public const string TypeOfInsteo = "InsteoOptimizer";
        public const string TypeOfSystemBrowSERLauncher = "System BrowSER Launcher";
        public string typeOfSyncerPath = "";

        public ICommand SelectFolderSelect_Click { get; set; }
        public ICommand OnBtnClick { get; set; }

        private ObservableCollection<SavedSyncProject> savedProjectsList;
        public ObservableCollection<SavedSyncProject> SavedProjectsList
        {
            get { return savedProjectsList; }
            set { savedProjectsList = value; }
        }
        private int sISavedProjectsList;
        public int SISavedProjectsList
        {
            get { return sISavedProjectsList; }
            set { sISavedProjectsList = value; RaisePropertyChanged("SISavedProjectsList"); }
        }


        //
        private Visibility visiblityHasLinksOption = Visibility.Visible;
        public Visibility VisiblityHasLinksOption
        {
            get { return visiblityHasLinksOption; }
            set { visiblityHasLinksOption = value; RaisePropertyChanged("VisiblityHasLinksOption"); }
        }
        private Visibility visiblityDoesntHasLinksOption = Visibility.Collapsed;
        public Visibility VisiblityDoesntHasLinksOption
        {
            get { return visiblityDoesntHasLinksOption; }
            set { visiblityDoesntHasLinksOption = value; RaisePropertyChanged("VisiblityDoesntHasLinksOption"); }
        }


        private object mLock = new object();

        public SyncedProjectsVM(string type)
        {
            OnBtnClick = new RelayCommand(OnBtnClick_Clicked);

            SelectFolderSelect_Click = new RelayCommand(SelectFolderSelect_BtnClick);

            SavedProjectsList = new ObservableCollection<SavedSyncProject>();

            new Thread(LoadSyncedProjectsList).Start();

            typeOfSyncerPath = type;
            if(typeOfSyncerPath == TypeOfSystemBrowSERLauncher)
            {
                VisiblityHasLinksOption = Visibility.Collapsed;
                VisiblityDoesntHasLinksOption = Visibility.Visible;
            }
        }

        #endregion

        internal void SetCorrectSI(SyncedProjectData data)
        {
            foreach (SavedSyncProject p in SavedProjectsList)
            {
                bool found = false;

                foreach (var d in p.SyndicatedPostsList)
                {
                    if (d == data)
                    {
                        SISavedProjectsList = SavedProjectsList.IndexOf(p);
                        p.SISyndicatedPostsList = p.SyndicatedPostsList.IndexOf(d);
                        found = true;
                        break;
                    }
                }

                if (found)
                    break;
            }
        }

        private void LoadSyncedProjectsList()
        {
            lock (mLock)
            {
                try
                {
                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), typeOfSyncerPath, GloableProfData.PData.ProjectName);
                    if (Directory.Exists(saveToDir))
                    {
                        string allSavedFilePath = Path.Combine(saveToDir, "Sorter");
                        if (File.Exists(allSavedFilePath))
                        {
                            ObservableCollection<SavedSyncProject> allSaved = File.ReadAllText(allSavedFilePath).XmlDeserializeFromString<ObservableCollection<SavedSyncProject>>();
                            foreach (SavedSyncProject p in allSaved)
                            {
                                p.TypeOfSync = typeOfSyncerPath;
                                Application.Current.Dispatcher.Invoke(new Action<int, SavedSyncProject>(addToSavedProjectsList), DispatcherPriority.Normal, -1, p);
                            }
                        }

                        if (typeOfSyncerPath != TypeOfSystemBrowSERLauncher && (SavedProjectsList.Count == 0 || !SavedProjectsList.Any(p => p.Name == GloableProfData.PData.ProjectName)))
                        {
                            Application.Current.Dispatcher.Invoke(new Action<int, SavedSyncProject>(addToSavedProjectsList), DispatcherPriority.Normal, 0,
                                new SavedSyncProject()
                                {
                                    IsSyncedMessage = "Synced",
                                    ProjectName = GloableProfData.PData.ProjectName,
                                    SISyndicatedPostsList = 0,
                                    TypeOfSync = typeOfSyncerPath
                                });
                        }

                        foreach (FileInfo fi in new DirectoryInfo(saveToDir).GetFiles())
                        {
                            if (fi.Name == "info" || fi.Name == "Sorter" || fi.Name == "FBSerchInfo" || fi.Name.Trim().ToLower() == GloableProfData.PData.ProfileName) continue;
                            SavedSyncProject p = File.ReadAllText(fi.FullName).XmlDeserializeFromString<SavedSyncProject>();
                            SavedProjectsList[0].SyndicatedPostsList.RemoveAllThese(sp => sp.FromProject == fi.Name);
                            foreach (var d in p.SyndicatedPostsList)
                            {
                                if (SavedProjectsList[0].SyndicatedPostsList.Any(spd => spd.Url == d.Url)) continue;

                                Application.Current.Dispatcher.Invoke(new Action<int, SyncedProjectData>(addSyndicatedPostToSavedProjectsList), DispatcherPriority.Normal, 0, d);
                            }

                            fi.Delete();
                        }

                        string deleteLinksDir = Path.Combine(saveToDir, "ToDelete");
                        if (Directory.Exists(deleteLinksDir))
                        {
                            foreach (FileInfo fi in new DirectoryInfo(deleteLinksDir).GetFiles())
                            {
                                SavedSyncProject project = File.ReadAllText(fi.FullName).XmlDeserializeFromString<SavedSyncProject>();
                                SavedSyncProject projectToRemoveFrom = SavedProjectsList.FirstOrDefault(p => p.Name == fi.Name);
                                if (projectToRemoveFrom != null)
                                {
                                    Application.Current.Dispatcher.Invoke(new Action<SavedSyncProject, SavedSyncProject>(removeDeletedLinks), DispatcherPriority.Normal, project, projectToRemoveFrom);
                                }
                            }

                            Directory.Delete(deleteLinksDir, true);
                        }

                        new Thread(Saved).Start();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void removeDeletedLinks(SavedSyncProject project, SavedSyncProject projectToRemoveFrom)
        {
            foreach (var data in project.SyndicatedPostsList)
            {
                SyncedProjectData dataToRemove = projectToRemoveFrom.SyndicatedPostsList.FirstOrDefault(d => d.Url == data.Url);
                if (dataToRemove != null)
                {
                    projectToRemoveFrom.SyndicatedPostsList.Remove(dataToRemove);
                }
            }

            //if (projectToRemoveFrom.SyndicatedPostsList.Count == 0)
            //    SavedProjectsList.Remove(projectToRemoveFrom);
        }

        private void addToSavedProjectsList(int index, SavedSyncProject proj)
        {
            if (index == -1)
            {
                SavedProjectsList.Add(proj);
            }
            else
            {
                SavedProjectsList.Insert(index, proj);
            }
        }

        private void addSyndicatedPostToSavedProjectsList(int index, SyncedProjectData data)
        {
            SavedProjectsList[index].SyndicatedPostsList.Add(data);
        }

        private void Saved(object param)
        {
            lock (mLock)
            {
                try
                {
                    foreach (SavedSyncProject project in SavedProjectsList)
                    {
                        if (project.ProjectName == GloableProfData.PData.ProjectName) continue;

                        string saveToPtojectDir = Path.Combine(MyFilesDatabase.GetBaseDir(), typeOfSyncerPath, project.ProjectName);
                        if (!Directory.Exists(saveToPtojectDir)) Directory.CreateDirectory(saveToPtojectDir);

                        string saveToFilePath = Path.Combine(saveToPtojectDir, GloableProfData.PData.ProjectName);
                        File.WriteAllText(saveToFilePath, project.XmlSerializeToString());
                    }

                    string saveToDir = Path.Combine(MyFilesDatabase.GetBaseDir(), typeOfSyncerPath, GloableProfData.PData.ProjectName);
                    if (!Directory.Exists(saveToDir)) Directory.CreateDirectory(saveToDir);
                    File.WriteAllText(Path.Combine(saveToDir, "Sorter"), SavedProjectsList.XmlSerializeToString());

                    Dictionary<string, SavedSyncProject> dictionaryForDelete = param as Dictionary<string, SavedSyncProject>;
                    if (dictionaryForDelete != null)
                    {
                        foreach (var kv in dictionaryForDelete)
                        {
                            string deleteLinksDir = Path.Combine(MyFilesDatabase.GetBaseDir(), typeOfSyncerPath, kv.Key, "ToDelete");
                            if (!Directory.Exists(deleteLinksDir)) Directory.CreateDirectory(deleteLinksDir);

                            File.WriteAllText(Path.Combine(deleteLinksDir, GloableProfData.PData.ProjectName), kv.Value.XmlSerializeToString());
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Save Failed!");
                }
            }
        }
        bool isrefreshing = false;
        private async void OnBtnClick_Clicked(object obj)
        {
            try
            {
                if (isrefreshing) return;
                isrefreshing = true;
                Mouse.OverrideCursor = Cursors.Wait;
                // lock (mLock)
                // {
                Mouse.OverrideCursor = null;

                switch ((string)obj)
                {
                    case "ClearLinksAllProjects":
                        if (!"Are you sure you want to clear all the links in the projects?".Show(true)) return;
                        foreach (var p in SavedProjectsList)
                        {
                            p.SyndicatedPostsList.Clear();
                        }
                        break;

                    case "AddProject":
                        await AddProjects("", "");
                        break;

                    case "AddLinks":
                        RssFeedsLinksMultiWindow mlw = new RssFeedsLinksMultiWindow();
                        mlw.tbInputedText.Text = "";
                        mlw.Title = "Name , Url";
                        mlw.Closed += LinksWindow_Closed;
                        mlw.Show();
                        break;

                    case "Save":
                        await Task.Run(() => Saved(null));
                        //new Thread(Saved).Start();
                        break;

                    case "Refresh":
                        SavedProjectsList.Clear();
                        //new Thread(LoadSyncedProjectsList).Start();
                        await Task.Run(() => LoadSyncedProjectsList());
                        break;

                    #region context menu
                    case "CMDelete":
                        if (!"Are you sure?".Show(true)) return;
                        if (SISavedProjectsList > -1 && SavedProjectsList.Count > 0)
                        {
                            Dictionary<string, SavedSyncProject> deleteFromDictionary = new Dictionary<string, SavedSyncProject>();
                            foreach (var d in SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Where(dt => dt.FromProject != GloableProfData.PData.ProjectName))
                            {
                                if (d.FromProject == null) continue;
                                if (!deleteFromDictionary.ContainsKey(d.FromProject))
                                {
                                    deleteFromDictionary.Add(d.FromProject, new SavedSyncProject() { ProjectName = d.FromProject });
                                }

                                deleteFromDictionary[d.FromProject].SyndicatedPostsList.Add(d);
                            }

                            if (SavedProjectsList[SISavedProjectsList].ProjectName == GloableProfData.PData.ProjectName)
                            {
                                SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Clear();
                            }
                            else
                            {
                                SavedProjectsList.RemoveAt(SISavedProjectsList);
                            }

                            await Task.Run(() => Saved(deleteFromDictionary));

                            //new Thread(Saved).Start(deleteFromDictionary);
                        }
                        break;

                    case "CMEdit":
                        if (SISavedProjectsList > -1 && SavedProjectsList.Count > 0)
                        {
                            string links = "";

                            foreach (var d in SavedProjectsList[SISavedProjectsList].SyndicatedPostsList)
                            {
                                links += d.PageName + " , " + d.Url + Environment.NewLine;
                            }

                            RssFeedsLinksMultiWindow linksWindow = new RssFeedsLinksMultiWindow();
                            linksWindow.tbInputedText.Text = links;
                            linksWindow.Title = "Name , Url";
                            linksWindow.ShowDialog();
                            if (linksWindow.ButtonLeftClicked)
                            {
                                //if (string.IsNullOrEmpty(linksWindow.tbInputedText.Text) && string.IsNullOrWhiteSpace(linksWindow.tbInputedText.Text))
                                //{
                                //    SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Clear();
                                //    new Thread(Saved).Start();
                                //    return;
                                //}


                                if (linksWindow.tbInputedText.Text != links)
                                {
                                    Dictionary<string, SavedSyncProject> deleteFromDictionary = new Dictionary<string, SavedSyncProject>();

                                    string[] restOfLinksArr = linksWindow.tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                                    if (SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Count > restOfLinksArr.Length)
                                    {
                                        List<SyncedProjectData> tspdl = new List<SyncedProjectData>();
                                        foreach (var d in SavedProjectsList[SISavedProjectsList].SyndicatedPostsList)
                                        {
                                            bool delete = true;
                                            foreach (string linData in restOfLinksArr)
                                            {
                                                if (linData.Contains(","))
                                                {
                                                    string[] nameLinkDate = linData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                    string name = nameLinkDate[0];
                                                    string url = nameLinkDate[1];
                                                    if (url.Trim().ToLower() == d.Url.Trim().ToLower())
                                                    {
                                                        delete = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (delete)
                                            {
                                                tspdl.Add(d);
                                            }
                                        }
                                        if (tspdl.Count > 0)
                                        {

                                            foreach (var d in tspdl)
                                            {
                                                SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Remove(d);
                                                if (d.FromProject != GloableProfData.PData.ProjectName)
                                                {
                                                    if (!deleteFromDictionary.ContainsKey(d.FromProject))
                                                    {
                                                        deleteFromDictionary.Add(d.FromProject, new SavedSyncProject() { ProjectName = d.FromProject });
                                                    }

                                                    deleteFromDictionary[d.FromProject].SyndicatedPostsList.Add(d);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string linData in restOfLinksArr)
                                        {
                                            string name = "";
                                            string url = "";
                                            if (linData.Contains(","))
                                            {
                                                string[] nameLinkDate = linData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                name = nameLinkDate[0];
                                                url = nameLinkDate[1];
                                            }
                                            else
                                            {
                                                url = linData;
                                            }


                                            if (SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Any(spd => spd.Url.Trim().ToLower() == url.Trim().ToLower())) continue;

                                            SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Add(new SyncedProjectData()
                                            {
                                                PageName = name.Trim(),
                                                Url = url.Trim(),
                                            });
                                        }
                                    }

                                    await Task.Run(() => Saved(deleteFromDictionary));
                                }
                            }
                        }
                        break;

                    case "CMClear":
                        if (!"Are you sure?".Show(true)) return;
                        if (SISavedProjectsList > -1 && SavedProjectsList.Count > 0)
                        {
                            SavedProjectsList[SISavedProjectsList].SyndicatedPostsList.Clear();
                        }
                        break;
                    #endregion

                    default:
                        break;
                }
                // }
            }
            catch { }
            isrefreshing = false;
        }

        private void LinksWindow_Closed(object sender, EventArgs e)
        {
            RssFeedsLinksMultiWindow win = sender as RssFeedsLinksMultiWindow;

            if (win.ButtonLeftClicked)
            {
                AddUrlToSavedProjectList("", "", win.tbInputedText.Text);
            }
        }

        private async Task<bool> AddProjects(string url, string pageName, string mulltyLinks = null)
        {
            ChooseProjectsVM cpvm = new ChooseProjectsVM();
            await cpvm.InitProjectsWindowList();
            if (!cpvm.ShowListWindowDialog()) return false;

            foreach (var sp in cpvm.SavedProjectsListAdded)
            {
                if (!sp.IsChecked || sp.IsFolder) continue;

                var projtoaddTo = new SavedSyncProject()
                {
                    IsSyncedMessage = "Synced",
                    ProjectName = sp.Name,
                    SISyndicatedPostsList = 0,
                    TypeOfSync = typeOfSyncerPath,
                };
                var projExists = SavedProjectsList.FirstOrDefault(p => p.ProjectName.Trim().ToLower() == sp.Name.Trim().ToLower());
                if (projExists != null) projtoaddTo = projExists;

                projtoaddTo.IsChecked = true;

                addToProj(pageName, url, mulltyLinks, projtoaddTo);

                if (projExists == null)
                    SavedProjectsList.Add(projtoaddTo);
            }

            new Thread(Saved).Start();
            return true;
        }

        private void addToProj(string pageName, string url, string multilinks, SavedSyncProject proj)
        {
            if (multilinks == null && url != "")
            {
                if (proj.SyndicatedPostsList.Any(d => d.Url == url)) return;
                proj.SyndicatedPostsList.Add(getNewProjData(pageName, url, GloableProfData.PData.ProjectName));
            }
            else if (multilinks != null)
            {
                addByMulty(multilinks, proj);
            }
        }

        private void addByMulty(string mulltyLinks, SavedSyncProject projtoaddTo)
        {
            string[] links = mulltyLinks.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var link in links)
            {
                string name = "";
                string murl = "";

                if (link.Contains(","))
                {
                    string[] nameAndLink = link.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameAndLink.Length == 2)
                    {
                        name = nameAndLink[0].Trim();
                        murl = nameAndLink[1].Trim();
                    }
                }
                else
                {
                    murl = link;
                    name = link.Replace("http://", "");
                    name = name.Replace("https://", "");
                    name = name.Replace("www.", "");
                    if (name.Contains("."))
                    {
                        name = name.Remove(name.IndexOf("."));
                    }
                }

                if (projtoaddTo.SyndicatedPostsList.Any(d => d.Url == murl)) continue;
                projtoaddTo.SyndicatedPostsList.Add(getNewProjData(name, murl, GloableProfData.PData.ProjectName));
            }
        }

        private SyncedProjectData getNewProjData(string pageName, string url, string projectName)
        {
            return new SyncedProjectData() { PageName = pageName, Url = url, FromProject = projectName };
        }


        public void AddUrlToSavedProjectList(string pageName, string url, string multilinks)
        {
            ChooseFolderWindow cfw = new ChooseFolderWindow();
            cfw.DataContext = this;
            cfw.lstItems.ItemsSource = SavedProjectsList;
            cfw.Title = "Sellect Project";
            cfw.btnLoadAllProjects.Visibility = Visibility.Visible;
            cfw.ShowDialog();
            if (!cfw.OkClicked) return;
            if (cfw.LoadAllClicked)
            {
                AddProjects(url, pageName, multilinks);
                return;
            }

            bool needsSave = false;
            foreach (SavedSyncProject proj in SavedProjectsList)
            {
                if (!proj.IsChecked) continue;

                if (!needsSave) needsSave = true;

                addToProj(pageName, url, multilinks, proj);
            }

            if (needsSave)
                new Thread(Saved).Start();
        }


        private void SelectFolderSelect_BtnClick(object param)
        {
            foreach (var folder in SavedProjectsList)
            {
                folder.IsChecked = (string)param == "All";
            }
        }
    }
}
