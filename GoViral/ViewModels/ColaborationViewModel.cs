using Organiser.Common.Classes;
using Organiser.Common.Models;
using Organiser.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Organiser.Common.Classes.MyFilesDatabase;

namespace GoViral.ViewModels
{
    public class ColaborationViewModel : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<ProjectImported> ImportedPrjects { get; set; }

        public GoViralVM ownerGViralVM { get; set; }

        public ColaborationViewModel(GoViralVM mainvm)
        {
            ownerGViralVM = mainvm;
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            ImportedPrjects = new ObservableCollection<ProjectImported>();
        }

        public async void OnCommandFromView_Raised(object obj)
        {
            var param = obj as string;
            if (param == null) return;

            try
            {
                switch (param)
                {
                    case "ImportProjects":
                        ChooseProjectsVM cpvm = new ChooseProjectsVM();
                        await cpvm.InitProjectsWindowList();
                        if (cpvm.ShowListWindowDialog())
                        {
                            foreach (var sp in cpvm.SavedProjectsListAdded)
                            {
                                if (!sp.IsChecked || sp.IsFolder) continue;

                                var projExists = ImportedPrjects.FirstOrDefault(p => p.Name.Trim().ToLower() == sp.Name.Trim().ToLower());
                                if (projExists != null)
                                {
                                    projExists.FilePath = sp.FilePath;
                                    continue;
                                }

                                sp.IsChecked = false;
                                ImportedPrjects.Add(sp);
                            }

                            if (ImportedPrjects.Count == 0) return;

                            await Task.Run(() => SaveImportedProjects());
                        }
                        break;

                    case "CheckUncheckAll":
                        bool AnnyUnchecked = ImportedPrjects.Any(p => !p.IsChecked);
                        foreach (var proj in ImportedPrjects) proj.IsChecked = AnnyUnchecked;
                        break;

                    case "RemoveChecked":
                        if (MessageBox.Show("Are you sure?", "Are you sure you wan to remove these projects from this projects area?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                        for (int i = ImportedPrjects.Count - 1; i >= 0; i--)
                        {
                            var proj = ImportedPrjects[i];
                            if (!proj.IsChecked) continue;

                            ImportedPrjects.ElementAt(i);
                        }

                        await Task.Run(() => SaveImportedProjects());
                        break;

                    case "LoadCheckedIntoDominator":
                        foreach (var proj in ImportedPrjects)
                        {
                            if (!proj.IsChecked || proj.Name == GloableProfData.PData.ProjectName) continue;

                            ownerGViralVM.PopulatList(proj.Name);
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var error = "Colaboration VM " + ex.Message;
                error.Show();
            }
        }

        private void SaveImportedProjects()
        {
            string ddir = Path.Combine(GetBaseDir(), "CollaborationFB", GloableProfData.PData.ProjectName);
            if (!System.IO.Directory.Exists(ddir)) System.IO.Directory.CreateDirectory(ddir);

            string fPath = Path.Combine(ddir, "savedProjects");
            System.IO.File.WriteAllText(fPath, ImportedPrjects.XmlSerializeToString());
        }

        public async void LoadColabiratedProjects()
        {
            string ddir = Path.Combine(GetBaseDir(), "CollaborationFB", GloableProfData.PData.ProjectName);
            if (!System.IO.Directory.Exists(ddir)) return;

            string fPath = Path.Combine(ddir, "savedProjects");
            if (!System.IO.File.Exists(fPath)) return;
            try
            {
                ProjectImported[] imported = null;
                await Task.Run(() =>
                {
                    string data = System.IO.File.ReadAllText(fPath);
                    if (data == "") return;

                    imported = data.XmlDeserializeFromString<ProjectImported[]>();
                });

                if (imported != null && imported.Length > 0)
                {
                    ImportedPrjects.Clear();
                    foreach (var proj in imported)
                    {
                        ImportedPrjects.Add(proj);
                    }

                    LoadCheckedProjects();
                }
            }
            catch { "Colaboration VM Failed To Load Imported Projects.".Show(); }
        }

        public void SaveImportedCheckedProjects()
        {
            string ddir = Path.Combine(GetBaseDir(), "CollaborationFB", GloableProfData.PData.ProjectName);
            if (!System.IO.Directory.Exists(ddir)) System.IO.Directory.CreateDirectory(ddir);

            string fPath = Path.Combine(ddir, "savedCheckedProjects");
            var saveString = "";
            foreach (var proj in ImportedPrjects)
            {
                if (!proj.IsChecked) continue;
                saveString += proj.Name + Environment.NewLine;
            }

            System.IO.File.WriteAllText(fPath, saveString);
        }
        public void LoadCheckedProjects()
        {
            string ddir = Path.Combine(GetBaseDir(), "CollaborationFB", GloableProfData.PData.ProjectName);
            if (!System.IO.Directory.Exists(ddir)) System.IO.Directory.CreateDirectory(ddir);

            string fPath = Path.Combine(ddir, "savedCheckedProjects");
            if (!System.IO.File.Exists(fPath)) return;

                foreach (var proj in ImportedPrjects)
                {
                    foreach (var projName in System.IO.File.ReadAllLines(fPath))
                    {
                        if (projName == proj.Name)
                            proj.IsChecked = true;
                    }
                }

        }

    }
}
