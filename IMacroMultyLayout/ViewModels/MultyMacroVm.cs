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

        public ObservableCollection<ProjectDataLinesSetting> CheckedProjects { get; set; }

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


        private int minWaitTimeBetweenLaunches = 1;
        public int MinWaitTimeBetweenLaunches
        {
            get { return minWaitTimeBetweenLaunches; }
            set { minWaitTimeBetweenLaunches = value; RaisePropertyChanged("TimesToPlayMax"); }
        }
        
        private int maxWaitTimeBetweenLaunches = 1;
        public int MaxWaitTimeBetweenLaunches
        {
            get { return maxWaitTimeBetweenLaunches; }
            set { maxWaitTimeBetweenLaunches = value; RaisePropertyChanged("MaxWaitTimeBetweenLaunches"); }
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
            set
            {
                dataSource = value;
                if (!value.IsNullOrEmpty()) DataLinesTotal = DataSource.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length; 
                RaisePropertyChanged("DataSource");
            }
        }

        private int dataLinesTotal;
        public int DataLinesTotal
        {
            get { return dataLinesTotal; }
            set { dataLinesTotal = value; RaisePropertyChanged("DataLinesTotal"); }
        }


        private int datasourceSettingsStartLine;
        public int DatasourceSettingsStartLine
        {
            get { return datasourceSettingsStartLine; }
            set { datasourceSettingsStartLine = value; RaisePropertyChanged("DatasourceSettingsStartLine"); }
        }

        private int datasourceSettingsEndLine;
        public int DatasourceSettingsEndLine
        {
            get { return datasourceSettingsEndLine; }
            set { datasourceSettingsEndLine = value; RaisePropertyChanged("DatasourceSettingsEndLine"); }
        }

        private bool stopRequested = false;

        private bool running;
        public bool Running
        {
            get { return running; }
            set { running = value; RaisePropertyChanged("Running"); }
        }


        public MultyMacroVm()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            ImportedPrjects = new ObservableCollection<ProjectImported>();
            CheckedProjects = new ObservableCollection<ProjectDataLinesSetting>();
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
                                AddImportedProject(proj);
                            }

                            bool anyRemoved = false;
                            for (int i = ImportedPrjects.Count - 1; i >= 0; i--)
                            {
                                var proj = ImportedPrjects[i];
                                if (MyFilesDatabase.File.Exists(proj.FilePath)) continue;
                                anyRemoved = true;
                                RemoveImportedProject(ImportedPrjects.ElementAt(i));
                            }
                            if (anyRemoved)
                            {
                                Task.Run(() => SaveImportedProjects());
                                "Projects that have either been moved or deleted, have been removed from the imacro runner projects list".Show();
                            }
                        });
                    }
                }
                catch { "Failed To Load Imported Projects.".Show(); }
            });
        }

        private void AddImportedProject(ProjectImported proj)
        {
            proj.IsChecked = false;
            proj.OnCheckedChanged += Proj_OnCheckedChanged;
            ImportedPrjects.Add(proj);
        }
        private void RemoveImportedProject(ProjectImported proj)
        {
            var projChecked = CheckedProjects.FirstOrDefault(p => p.Name == proj.Name);
            if (projChecked != null) CheckedProjects.Remove(projChecked);
            ImportedPrjects.Remove(proj);
        }

        private void Proj_OnCheckedChanged(ProjectImported proj, bool isChecked)
        {
            if (isChecked)
            {
                if (!CheckedProjects.Any(p => p.Name == proj.Name))
                {
                    CheckedProjects.Add(new ProjectDataLinesSetting()
                    {
                        Name = proj.Name,
                        DataSourceLinesFrom = 0,
                        DataSourceLinesTo = 0
                    });
                }
            }
            else
            {
                var projChecked = CheckedProjects.FirstOrDefault(p => p.Name == proj.Name);
                if (projChecked != null) CheckedProjects.Remove(projChecked);
            }
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
                                AddImportedProject(sp);
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
                            
                            RemoveImportedProject(ImportedPrjects.ElementAt(i));
                        }
                        await Task.Run(() => SaveImportedProjects());
                        break;

                    case "Apply":
                        var increment = 0;
                        foreach (var dsp in CheckedProjects)
                        {
                            if (increment + DatasourceSettingsEndLine > DataLinesTotal) increment = 0;

                            dsp.DataSourceLinesFrom = increment == 0 ? 1 : increment+1;
                            increment += DatasourceSettingsEndLine;
                            dsp.DataSourceLinesTo = increment;
                        }
                        break;

                    case "UseAll":
                        foreach (var dsp in CheckedProjects)
                        {
                            dsp.DataSourceLinesFrom = 1;
                            dsp.DataSourceLinesTo = DataLinesTotal;
                        }
                        break;

                    case "Random":
                        var randIncrement = DataLinesTotal > CheckedProjects.Count ? DataLinesTotal / CheckedProjects.Count : CheckedProjects.Count / DataLinesTotal;
                        var lastRandMin = 1;
                        var lastRandIncrementmax = new Random().Next(1, randIncrement);
                        if (lastRandMin == lastRandIncrementmax) lastRandIncrementmax += 1;
                        foreach (var dsp in CheckedProjects)
                        {
                            if(lastRandIncrementmax > DataLinesTotal)
                            {
                                lastRandMin = 1;
                                lastRandIncrementmax = new Random().Next(1, randIncrement);
                                if (lastRandMin == lastRandIncrementmax) lastRandIncrementmax += 1;
                            }
                            dsp.DataSourceLinesFrom = lastRandMin;
                            dsp.DataSourceLinesTo = lastRandIncrementmax;

                            //re randomize increment
                            lastRandMin = lastRandIncrementmax + 1;
                            lastRandIncrementmax = new Random().Next(lastRandMin + 1, lastRandMin + 1 + randIncrement);
                        }
                        break;

                    case "SharedRandom":
                        foreach (var dsp in CheckedProjects)
                        {
                            dsp.DataSourceLinesFrom = 0;
                            dsp.DataSourceLinesTo = 0;
                            dsp.DataSourceLinesFrom = new Random().Next(1, DataLinesTotal - 1);
                            dsp.DataSourceLinesTo = new Random().Next(dsp.DataSourceLinesFrom + 1, DataLinesTotal);
                            await Task.Delay(50);
                        }
                        break;

                    case "Stop":
                        stopRequested = true;
                        Running = false;
                        break;

                    case "Run":
                        Running = true;
                        var checkedMAcros = new List<MacroFile>();
                        MacroMangerImpl.GetCheckedMAcros(MacroMangerImpl.MacroFilesBase, checkedMAcros);

                        int windowsLaunched = 1;
                        foreach (var proj in ImportedPrjects)
                        {
                            if (stopRequested)
                            {
                                Running = false;
                                stopRequested = false;
                                return;
                            }
                            if (!proj.IsChecked) continue;

                            if (TimesToPlay < 1) TimesToPlay = 1;
                            if (TimesToPlayMax < 1) TimesToPlayMax = 1;
                            if (TimesToPlayMax < TimesToPlay) TimesToPlayMax = TimesToPlay;

                            if (DataSource.IsNullOrEmpty()) DataSource = "";

                             var randTimesToPlay = new Random().Next(TimesToPlay, TimesToPlayMax);
                            string macroPaths = "";

                            var datasourceForProj = DataSource;
                            var dsLines = datasourceForProj.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                            var projSettings = CheckedProjects.FirstOrDefault(p => p.Name == proj.Name);
                            if (projSettings != null)
                            {
                                datasourceForProj = "";
                                for (int i = 0; i < dsLines.Length; i++)
                                {
                                    if (i >= projSettings.DataSourceLinesFrom - 1 && i <= projSettings.DataSourceLinesTo -1)
                                        datasourceForProj += dsLines[i] + Environment.NewLine; 
                                }
                            }
                            //foreach (var mac in checkedMAcros)
                            //{
                            //    if (EachOnSeperateProcessChecked)
                            //    {
                            //        randTimesToPlay = new Random().Next(TimesToPlay, TimesToPlayMax);
                            //        //for (int i = 0; i < TimesToPlay; i++)
                            //        //{

                            //        //"C:\Program Files\Mozilla Firefox\firefox.exe" imacros://run/?m="_My Macro.IIM"
                            //        var info = new ProcessStartInfo
                            //        {
                            //            Arguments = "\"" + proj.FilePath + "\"" + " " + CloseOnComplete + " " + "\"" + datasourceForProj + "\"" + " " + "\"" + mac.FilePath + "\"" + " " + windowsLaunched + " " + randTimesToPlay,
                            //            CreateNoWindow = true,
                            //            UseShellExecute = false,
                            //            FileName = "AnyProjFFProcess.exe"
                            //        };


                            //        Process p = Process.Start(info);
                            //        ProcessManager.Instance.AddProcess(p);
                            //        windowsLaunched++;
                            //        // p.WaitForExit();
                            //        //}
                            //    }
                            //    else
                            //    {
                            //        macroPaths += mac.FilePath + MyFilesDatabase.SPLITTER;
                            //    }
                            //}

                            //if (macroPaths != "")
                            //{
                            //    var info = new ProcessStartInfo
                            //    {
                            //        Arguments = "\"" + proj.FilePath + "\"" + " " + CloseOnComplete + " " + "\"" + datasourceForProj + "\"" + " " + "\"" + macroPaths + "\"" + " " + windowsLaunched + " " + randTimesToPlay,
                            //        CreateNoWindow = true,
                            //        UseShellExecute = false,
                            //        FileName = "AnyProjFFProcess.exe"
                            //    };


                            //    Process p = Process.Start(info);
                            //    ProcessManager.Instance.AddProcess(p);
                            //    windowsLaunched++;
                            //}

                            var PersonData = MyFilesDatabase.GetUpPdaaFromPath(proj.FilePath);
                           string ffpath = Path.Combine(MyFilesDatabase.GetBaseDir(), "CachesFF\\" + proj.Name);
                                if (!Directory.Exists(ffpath)) Directory.CreateDirectory(ffpath);

                            foreach (var mac in checkedMAcros)
                            {
                                //MyFilesDatabase.LaunchToSystemFF("-new-instance -no-remote -new-tab -url about:home -new-tab -url " + url + " -profile \"" + ffpath + "\"", ffpath, PersonData);
                                //MyFilesDatabase.LaunchToSystemFF("-new-instance -no-remote -profile \"" + ffpath + "\" -url \"imacros://run/?m=" + mac.FilePath + "\"", ffpath, PersonData,true,PersonData.ProjectName);

                                var info = new ProcessStartInfo
                                {
                                    Arguments = "\"" + proj.FilePath + "\"" + " " + "\"imacros://run/?m=" + mac.FilePath + "\" "+ windowsLaunched + " " + "\"" + datasourceForProj + "\"",
                                    CreateNoWindow = true,
                                    UseShellExecute = false,
                                    FileName = "BrowseoFX.CMD.exe"
                                };
                                Process p = Process.Start(info);
                                ProcessManager.Instance.AddProcess(p);

                                int randLaunchWait = new Random().Next(MinWaitTimeBetweenLaunches, MaxWaitTimeBetweenLaunches) * 1000;
                                await Task.Delay(randLaunchWait < 5000 ? randLaunchWait + 5000 : randLaunchWait);
                                windowsLaunched++;
                            }
                        }
                        Running = false;
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Running = false;
                ex.Message.Show();
            }
        }
    }
}
