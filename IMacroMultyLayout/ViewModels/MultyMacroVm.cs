using Delimon.Win32.IO;
using IMacroMultyLayout.Models;
using Organiser.Common.Classes;
using Organiser.Common.Models;
using Organiser.Common.ViewModels;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IMacroMultyLayout.ViewModels
{
    public class MultyMacroVm : ViewModelBase
    {
        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<ProjectImported> ImportedPrjects { get; set; }

        private MacroManger MacroMangerimpl;
        public MacroManger MacroMangerImpl
        {
            get { return MacroMangerimpl; }
            set { MacroMangerimpl = value; RaisePropertyChanged("MacroMangerImpl"); }
        }


        private int timesToPlay;
        public int TimesToPlay
        {
            get { return timesToPlay; }
            set
            {
                timesToPlay = value;
                //if (TimesToPlayMax < value)
                //{
                //    TimesToPlayMax = value;
                //}
                RaisePropertyChanged("TimesToPlay");
            }
        }
        //TimesToPlayMax
        private int timesToPlayMax;
        public int TimesToPlayMax
        {
            get { return timesToPlayMax; }
            set { timesToPlayMax = value; RaisePropertyChanged("TimesToPlayMax"); }
        }

        private bool closeOnComplete;
        public bool CloseOnComplete
        {
            get { return closeOnComplete; }
            set { closeOnComplete = value; RaisePropertyChanged("CloseOnComplete"); }
        }

        private bool eachOnSeperateProcessChecked;
        public bool EachOnSeperateProcessChecked
        {
            get { return eachOnSeperateProcessChecked; }
            set { eachOnSeperateProcessChecked = value; RaisePropertyChanged("EachOnSeperateProcessChecked"); }
        }

        private string dataSource;
        public string DataSource
        {
            get { return dataSource; }
            set { dataSource = value; RaisePropertyChanged("DataSource"); }
        }

        public MultyMacroVm()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            ImportedPrjects = new ObservableCollection<ProjectImported>();
            GetSavedProjects();
            MacroMangerImpl = new MacroManger();
            TimesToPlay = TimesToPlayMax = 1;

        }

        private async void GetSavedProjects()
        {
            await Task.Run(() =>
            {
                try
                {
                    string data = MyFilesDatabase.GEtImportedProjectsForMultyMacroData(GloableProfData.PData.ProjectName);
                    if (data == "") return;

                    var imported = data.XmlDeserializeFromString<ProjectImported[]>();
                    if (imported != null && imported.Length > 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ImportedPrjects.Clear();
                            foreach (var proj in imported)
                            {
                                ImportedPrjects.Add(proj);
                            }

                            bool anyRemoved = false;
                            for (int i = ImportedPrjects.Count - 1; i >= 0; i--)
                            {
                                var proj = ImportedPrjects[i];
                                if (MyFilesDatabase.File.Exists(proj.FilePath)) continue;
                                anyRemoved = true;
                                ImportedPrjects.RemoveAt(i);
                            }
                            if (anyRemoved)
                            {
                                "Projects that hae either been moved or deleted have been removed from the imacro runner projects list".Show();
                            }
                        });
                    }
                }
                catch { "Failed To Load Imported Projects.".Show(); }
            });
        }

        private void SaveImportedProjects()
        {
            MyFilesDatabase.SaveImportedProjectsForMultyMacro(GloableProfData.PData.ProjectName, ImportedPrjects.XmlSerializeToString());
        }

        private async void OnCommandFromView_Raised(object obj)
        {
            string param = obj as string;
            if (param == null) return;

            try
            {
                switch (param)
                {
                    case "MacrosReload":
                        await MacroSettings.InitMacrosSettings();
                        await MacroMangerImpl.LoadIMacros(true);
                        break;

                    case "ProjectsCheckUnAll":
                        bool AnnyUnchecked = ImportedPrjects.Any(p => !p.IsChecked);
                        foreach (var proj in ImportedPrjects) proj.IsChecked = AnnyUnchecked;
                        break;

                    case "ProjectsImport":
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

                                ImportedPrjects.Add(sp);
                            }

                            await Task.Run(() => SaveImportedProjects());
                        }
                        break;

                    case "ProjectsRemove":
                        if (MessageBox.Show("Are you sure?", "Are you sure you wan to remove these projects from this projects area?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                        for (int i = ImportedPrjects.Count - 1; i >= 0; i--)
                        {
                            var proj = ImportedPrjects[i];
                            if (!proj.IsChecked) continue;

                            ImportedPrjects.RemoveAt(i);
                        }
                        await Task.Run(() => SaveImportedProjects());
                        break;

                    case "Run":
                        var checkedMAcros = new List<MacroFile>();
                        MacroMangerImpl.GetCheckedMAcros(MacroMangerImpl.MacroFilesBase, checkedMAcros);

                        int windowsLaunched = 1;
                        foreach (var proj in ImportedPrjects)
                        {
                            if (!proj.IsChecked) continue;

                            if (TimesToPlay < 1) TimesToPlay = 1;
                            if (TimesToPlayMax < 1) TimesToPlayMax = 1;
                            if (TimesToPlayMax < TimesToPlay) TimesToPlayMax = TimesToPlay;
                             var randTimesToPlay = new Random().Next(TimesToPlay, TimesToPlayMax);

                            string macroPaths = "";
                            foreach (var mac in checkedMAcros)
                            {
                                if (EachOnSeperateProcessChecked)
                                {
                                    randTimesToPlay = new Random().Next(TimesToPlay, TimesToPlayMax);
                                    //for (int i = 0; i < TimesToPlay; i++)
                                    //{
                                    var info = new ProcessStartInfo
                                    {
                                        Arguments = "\"" + proj.FilePath + "\"" + " " + CloseOnComplete + " " + "\"" + DataSource + "\"" + " " + "\"" + mac.FilePath + "\"" + " " + windowsLaunched + " " + randTimesToPlay,
                                        CreateNoWindow = true,
                                        UseShellExecute = false,
                                        FileName = "AnyProjFFProcess.exe"
                                    };


                                    Process p = Process.Start(info);
                                    ProcessManager.Instance.AddProcess(p);
                                    windowsLaunched++;
                                    // p.WaitForExit();
                                    //}
                                }
                                else
                                {
                                    macroPaths += mac.FilePath + MyFilesDatabase.SPLITTER;
                                }
                            }

                            if (macroPaths != "")
                            {
                                var info = new ProcessStartInfo
                                {
                                    Arguments = "\"" + proj.FilePath + "\"" + " " + CloseOnComplete + " " + "\"" + DataSource + "\"" + " " + "\"" + macroPaths + "\"" + " " + windowsLaunched + " " + randTimesToPlay,
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    FileName = "AnyProjFFProcess.exe"
                                };


                                Process p = Process.Start(info);
                                ProcessManager.Instance.AddProcess(p);
                                windowsLaunched++;
                            }
                        }
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                ex.Message.Show();
            }
        }
    }
}
