using Delimon.Win32.IO;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Organiser.Common.Classes
{
    public class MacroSettings
    {
        public static bool IsScrollWhenFoundChecked = true, IsHighlightWhenFoundChecked = false,
                           IsReplaySpeedFastChecked = false, IsReplaySpeedMediumChecked = true, IsReplaySpeedSlowChecked = false;
        public static int TimeoutLimit = 60;
        public static string DefaulFoldertMacros = "", DefaulFoldertDataSources = "", DefaultFolderDownloads = "", TwoCaptchaKey="";

        public static async Task InitMacrosSettings()
        {
            await Task.Run(() =>
            {
                DefaulFoldertMacros = MyFilesDatabase.GetBaseMacroScriptsDir();
                DefaulFoldertDataSources = MyFilesDatabase.GetBaseMacroDatasourcesDir();
                DefaultFolderDownloads = MyFilesDatabase.GetBaseMacroDownloadDir();

                try
                {
                    string settingsFile = MyFilesDatabase.GetBaseMacroSettingsDir();
                    settingsFile = settingsFile + "\\defaultsettings.txt";
                    if (File.Exists(settingsFile))
                    {
                        string[] fileLines = File.ReadAllText(settingsFile).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                        if (fileLines != null && fileLines.Length >= 9)
                        {
                            IsReplaySpeedFastChecked = Convert.ToBoolean(fileLines[0]);
                            IsReplaySpeedMediumChecked = Convert.ToBoolean(fileLines[1]);
                            IsReplaySpeedSlowChecked = Convert.ToBoolean(fileLines[2]);

                            IsScrollWhenFoundChecked = Convert.ToBoolean(fileLines[3]);
                            IsHighlightWhenFoundChecked = Convert.ToBoolean(fileLines[4]);

                            TimeoutLimit = Convert.ToInt32(fileLines[5]);

                            DefaulFoldertMacros = fileLines[6];
                            DefaulFoldertDataSources = fileLines[7];
                            DefaultFolderDownloads = fileLines[8];
                        }
                        if (fileLines.Length == 10)
                        {
                            TwoCaptchaKey = fileLines[9];
                        }
                    }
                }
                catch { "Failed to read settings file".Show(); }
            });
        }

        public static string GetBuitInMacrosBaseDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory + "\\BrowSEO IA Scripts";
        }

        public static bool IsIgnorableDirectoryOrFile(string pathToDirectory)
        {
            string bdir = MacroSettings.GetBuitInMacrosBaseDir();
            bdir = bdir.Replace("\\\\", "\\");

            return pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Common") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "IFTTT Connect", "IIM") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "BitLy.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Blogger.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Delicious.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Diigo.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Facebook.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "LinkedIn.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Medium.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Tumblr.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Twitter.js") ||
                   pathToDirectory == MyFilesDatabase.Path.Combine(bdir, "IFTT", "Wordpress.js");
        }
    }














    public class MacroOnDownload
    {
        public static string FOLDER = "*", FILE = "*", WAIT = "YES", SIZE = "";
    }
    public class MacroEventTypes
    {
        public const string MOUSEDOWN = "MOUSEDOWN";
        public const string MOUSEMOVE = "MOUSEMOVE";
        public const string MOUSEUP = "MOUSEUP";
        public const string CLICK = "CLICK";
        public const string DBLCLICK = "DBLCLICK";

        public const string KEYDOWN = "KEYDOWN";
        public const string KEYUP = "KEYUP";
        public const string KEYPRESS = "KEYPRESS";
    }
    public class COLn
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Value { get; set; }
    }
    public class MacroVariables
    {
        public class MacroDatasourceValues
        {
            public const string DATASOURCE_SLIDEOUT = "SLIDEOUT";
            public const string DATASOURCE_MAINPROJECTPROFILE = "PROJECTPROFILE";
            public const string DATASOURCE_SELECTPROFILE = "SELECTPROFILE";
            public const string DATASOURCE_PROFILENAME = "PROFILENAME=";
        }
        public class MacroProjectValues
        {
            public const string COLn_PROJECTNAME = "PROJECTNAME";
            public const string COLn_PROFILENAME = "PROFILENAME";

            public const string COLn_PROXYIP = "PROXYIP";
            public const string COLn_PROXYPORT = "PROXYPORT";
            public const string COLn_PROXYUSER = "PROXYUSER";
            public const string COLn_PROXYPASS = "PROXYPASS";

            public const string COLn_FIRSTNAME = "FIRSTNAME";
            public const string COLn_LASTNAME = "LASTNAME";
            public const string COLn_PHONE = "PHONE";

            public const string COLn_USERNAME = "USERNAME";
            public const string COLn_EMAIL = "EMAIL";
            public const string COLn_PASS = "PASSWORD";

            public const string COLn_SEX = "SEX";
            public const string COLn_BIRTHDAY = "BIRTHDAY";
            public const string COLn_BIRTHDAYFULL = "BIRTHDAYFULL";
            public const string COLn_BIRTHMONTH = "BIRTHMONTH";
            public const string COLn_BIRTHYEAR = "BIRTHYEAR";

            public const string COLn_STREET = "STREET";
            public const string COLn_CITY = "CITY";
            public const string COLn_STATE = "STATE";
            public const string COLn_ZIP = "ZIP";
            public const string COLn_COUNTRY = "COUNTRY";

            public const string COLn_WEBSITE = "WEBSITE";
            public const string COLn_NOTES = "NOTES";

            //{{!COL_PROJECTNAME}}
            //{{!COL_PROFILENAME}}

            //{{!COL_FIRSTNAME}} 
            //{{!COL_LASTNAME}} 
            //{{!COL_PHONE}} 
            //{{!COL_USERNAME}} 
            //{{!COL_EMAIL}} 
            //{{!COL_PASSWORD}} 

            //{{!COL_SEX}} 
            //{{!COL_BIRTHDAY}} 
            //{{!COL_BIRTHMONTH}}
            //{{!COL_BIRTHYEAR}}
            //{{!COL_STREET}} 
            //{{!COL_CITY}} 
            //{{!COL_STATE}} 
            //{{!COL_ZIP}}  
            //{{!COL_COUNTRY}} 
            //{{!COL_WEBSITE}} 
            //{{!COL_NOTES}}  
            //{{!COL_BIRTHDAYFULL}}
        }
        public const string PHONENUMBER = "!PHONENUMBER";
        public const string CLIPBOARD = "!CLIPBOARD";
        public const string CLOSEONERROR = "!CLOSEONERROR";

        public const string DATASOURCE = "!DATASOURCE";
        public const string DATASOURCE_COLUMNS = "!DATASOURCE_COLUMNS";
        public const string DATASOURCE_DELIMITER = "!DATASOURCE_DELIMITER";
        public const string DATASOURCE_LINE = "!DATASOURCE_LINE";
        //public const string DOWNLOADED_FILE_NAME = "!DOWNLOADED_FILE_NAME";
        //public const string DOWNLOADED_SIZE = "!DOWNLOADED_SIZE";
        //public const string ENCRYPTION = "!ENCRYPTION";
        //public const string ENDOFPAGE = "!ENDOFPAGE";
        public const string ERRORIGNORE = "!ERRORIGNORE";
        public const string EXTRACT = "!EXTRACT";
        public const string EXTRACT_TEST_POPUP = "!EXTRACT_TEST_POPUP";
        //public const string EXTRACTDIALOG = "!EXTRACTDIALOG";
        //public const string FAIL_ON_ALL_NAVIGATEERRORS = "!FAIL_ON_ALL_NAVIGATEERRORS";
        //public const string FILELOG = "!FILELOG";
        public const string FILESTOPWATCH = "!FILESTOPWATCH";
        public const string FILE_PROFILER = "!FILE_PROFILER";
        public const string FOLDER_DATASOURCE = "!FOLDER_DATASOURCE";
        public const string FOLDER_STOPWATCH = "!FOLDER_STOPWATCH";
        //public const string IMAGEX = "!IMAGEX";
        //public const string IMAGEY = "!IMAGEY";
        public const string LOOP = "!LOOP";
        //public const string MARKOBJECT = "!MARKOBJECT";
        public const string NOW = "!NOW";
        //public const string PLAYBACKDELAY = "!PLAYBACKDELAY";
        //public const string POPUP_ALLOWED = "!POPUP_ALLOWED";
        public const string REPLAYSPEED = "!REPLAYSPEED";
        //public const string REGION_BOTTOM = "!REGION_BOTTOM";
        //public const string REGION_LEFT = "!REGION_LEFT";
        //public const string REGION_RIGHT = "!REGION_RIGHT";
        //public const string REGION_TOP = "!REGION_TOP";
        public const string SINGLESTEP = "!SINGLESTEP";
        public const string STOPWATCHTIME = "!STOPWATCHTIME";
        //public const string STOPWATCH_HEADER = "!STOPWATCH_HEADER";
        //public const string TAGSOURCEINDEX = "!TAGSOURCEINDEX";
        //public const string TAGX = "!TAGX";
        //public const string TAGY = "!TAGY";
        //public const string TIMEOUT = "!TIMEOUT";
        //public const string TIMEOUT_DOWNLOAD = "!TIMEOUT_DOWNLOAD";
        public const string TIMEOUT_MACRO = "!TIMEOUT_MACRO";
        public const string TIMEOUT_PAGE = "!TIMEOUT_PAGE";
        public const string TIMEOUT_STEP = "!TIMEOUT_STEP";
        public const string URLCURRENT = "!URLCURRENT";
        public const string USERAGENT = "!USERAGENT";
        public const string VAR0 = "!VAR0";
        public const string VAR1 = "!VAR1";
        public const string VAR2 = "!VAR2";
        public const string VAR3 = "!VAR3";
        public const string VAR4 = "!VAR4";
        public const string VAR5 = "!VAR5";
        public const string VAR6 = "!VAR6";
        public const string VAR7 = "!VAR7";
        public const string VAR8 = "!VAR8";
        public const string VAR9 = "!VAR9";
        //public const string WAITPAGECOMPLETE = "!WAITPAGECOMPLETE";

        public Dictionary<string, string> MacroVariablesValues = new Dictionary<string, string>();
        public List<COLn> Columns = new List<COLn>();
        public event Action OnSetExtract = delegate { };

        public MacroVariables()
        {
            MacroVariablesValues.Add("!CLIPBOARD", "");
            MacroVariablesValues.Add("!CLOSEONERROR", "YES");
            // MacroVariablesValues.Add("!COLn", "");
            MacroVariablesValues.Add("!DATASOURCE", "");
            MacroVariablesValues.Add("!DATASOURCE_COLUMNS", "");
            MacroVariablesValues.Add("!DATASOURCE_DELIMITER", ",");
            MacroVariablesValues.Add("!DATASOURCE_LINE", "1");
            //MacroVariablesValues.Add("!DOWNLOADED_FILE_NAME", "");
            //MacroVariablesValues.Add("!DOWNLOADED_SIZE ", "");
            //MacroVariablesValues.Add("!ENCRYPTION", "");
            //MacroVariablesValues.Add("!ENDOFPAGE", "");
            MacroVariablesValues.Add("!ERRORIGNORE", "NO");
            MacroVariablesValues.Add("!EXTRACT", "NULL");
            MacroVariablesValues.Add("!EXTRACT_TEST_POPUP", "YES");
            //MacroVariablesValues.Add("!EXTRACTDIALOG", "");
            //MacroVariablesValues.Add("!FAIL_ON_ALL_NAVIGATEERRORS", "");
            //MacroVariablesValues.Add("!FILELOG", "");
            MacroVariablesValues.Add("!FILESTOPWATCH", "");
            MacroVariablesValues.Add("!FILE_PROFILER", "");
            MacroVariablesValues.Add("!FOLDER_DATASOURCE", "");
            MacroVariablesValues.Add("!FOLDER_STOPWATCH", "");
            //MacroVariablesValues.Add("!IMAGEX", "");
            //MacroVariablesValues.Add("!IMAGEY", "");
            MacroVariablesValues.Add("!LOOP", "");
            //MacroVariablesValues.Add("!MARKOBJECT", "");
            MacroVariablesValues.Add("!NOW", DateTime.Now.ToString());
            //MacroVariablesValues.Add("!PLAYBACKDELAY", "");
            //MacroVariablesValues.Add("!POPUP_ALLOWED", "");
            if(MacroSettings.IsReplaySpeedFastChecked) MacroVariablesValues.Add("!REPLAYSPEED", "FAST");
            if(MacroSettings.IsReplaySpeedSlowChecked) MacroVariablesValues.Add("!REPLAYSPEED", "SLOW");
            if(MacroSettings.IsReplaySpeedMediumChecked) MacroVariablesValues.Add("!REPLAYSPEED", "MEDIUM");
            //MacroVariablesValues.Add("!REGION_BOTTOM", "");
            //MacroVariablesValues.Add("!REGION_LEFT", "");
            //MacroVariablesValues.Add("!REGION_RIGHT", "");
            //MacroVariablesValues.Add("!REGION_TOP", "");
            MacroVariablesValues.Add("!SINGLESTEP", "NO");
            MacroVariablesValues.Add("!STOPWATCHTIME", "");
            //MacroVariablesValues.Add("!STOPWATCH_HEADER", "");
            //MacroVariablesValues.Add("!TAGSOURCEINDEX", "");
            //MacroVariablesValues.Add("!TAGX", "");
            //MacroVariablesValues.Add("!TAGY", "");
            //MacroVariablesValues.Add("!TIMEOUT", "");
            //MacroVariablesValues.Add("!TIMEOUT_DOWNLOAD ", "");
            MacroVariablesValues.Add("!TIMEOUT_MACRO", "");
            MacroVariablesValues.Add("!TIMEOUT_PAGE", "60");
            MacroVariablesValues.Add("!TIMEOUT_STEP", "6");
            MacroVariablesValues.Add("!URLCURRENT", "");
            MacroVariablesValues.Add("!USERAGENT", BrowserSettimgs.UserAgentFF);
            MacroVariablesValues.Add("!VAR0", "");
            MacroVariablesValues.Add("!VAR1", "");
            MacroVariablesValues.Add("!VAR2", "");
            MacroVariablesValues.Add("!VAR3", "");
            MacroVariablesValues.Add("!VAR4", "");
            MacroVariablesValues.Add("!VAR5", "");
            MacroVariablesValues.Add("!VAR6", "");
            MacroVariablesValues.Add("!VAR7", "");
            MacroVariablesValues.Add("!VAR8", "");
            MacroVariablesValues.Add("!VAR9", "");
            //MacroVariablesValues.Add("!WAITPAGECOMPLETE", "");
        }

        public string this[string key]
        {
            get
            {
                if (key.ToUpper().Contains("NOW:"))
                {
                    return MacroVariablesValues["!NOW"];
                }
                else
                {
                    return MacroVariablesValues.ContainsKey(key.ToUpper()) ? MacroVariablesValues[key.ToUpper()] : null;
                }
            }
            set
            {
                if (key == EXTRACT && value != "NULL")
                {
                    if (MacroVariablesValues[EXTRACT] == "NULL" || MacroVariablesValues[EXTRACT] == "") MacroVariablesValues[EXTRACT] = value;
                    else MacroVariablesValues[EXTRACT] = MacroVariablesValues[EXTRACT] + "," + value;
                }
                else
                {
                    MacroVariablesValues[key.ToUpper()] = value;
                }

                if (key == EXTRACT)
                {
                    OnSetExtract();
                }
            }
        }
        public void SetValue(string key, string value)
        {
            MacroVariablesValues[key] = value;
        }

        public void SetDSColVariablesValues(int row, int col, string value)
        {
            COLn clmn = Columns.FirstOrDefault(c => c.Row == row && c.Column == col);
            if (clmn == null)
            {
                clmn = new COLn() { Row = row, Column = col };
                Columns.Add(clmn);
            }
            clmn.Value = value;
        }
        public string GetColumnValue(int row, int col)
        {
            COLn clmn = Columns.FirstOrDefault(c => c.Row == row && c.Column == col);
            if (clmn == null) return "";
            return clmn.Value;
        }
    }
    public class MacroCommands
    {
        public const string Comment = "'";//'
        public const string ADD = "ADD";
        public const string BACK = "BACK";
        public const string CLEAR = "CLEAR";
        public const string CLICK = "CLICK";
        public const string DS = "DS";
        public const string EVAL = "EVAL";
        public const string EVENT = "EVENT";
        public const string EVENTS = "EVENTS";
        public const string EXTRACT = "EXTRACT";// (Part of the TAG command)
        public const string FILEDELETE = "FILEDELETE";
        public const string FILTER = "FILTER";
        public const string FRAME = "FRAME";
        public const string IMAGECLICK = "IMAGECLICK";
        public const string IMAGESEARCH = "IMAGESEARCH";

        public const string ONCERTIFICATEDIALOG = "ONCERTIFICATEDIALOG";
        public const string ONDIALOG = "ONDIALOG";
        public const string ONDOWNLOAD = "ONDOWNLOAD";
        public const string ONERRORDIALOG = "ONERRORDIALOG";
        public const string ONINSECURECONNECTION = "ONINSECURECONNECTION";// Version 10.3 and above
        public const string ONLOGIN = "ONLOGIN";
        public const string ONPRINT = "ONPRINT";
        public const string ONSECURITYDIALOG = "ONSECURITYDIALOG";
        public const string ONWEBPAGEDIALOG = "ONWEBPAGEDIALOG";

        public const string PAUSE = "PAUSE";
        public const string PRINT = "PRINT";
        public const string PROMPT = "PROMPT";
        public const string PROXY = "PROXY";
        public const string REFRESH = "REFRESH";
        public const string SAVEAS = "SAVEAS";
        public const string SAVEITEM = "SAVEITEM";
        public const string SCREENSHOT = "SCREENSHOT";
        public const string SEARCH = "SEARCH";
        public const string SET = "SET";
        public const string CMDLINE = "CMDLINE";
        public const string SIZE = "SIZE";
        public const string STOPWATCH = "STOPWATCH";
        public const string TAB = "TAB";
        public const string TAG = "TAG";
        public const string TRAY = "TRAY";
        public const string URL = "URL";
        public const string VERSION = "VERSION";
        public const string WAIT = "WAIT";
        public const string PASTE = "RAISEPASTE";
        public const string OPENFILE = "OPENFILE";

        public const string SOLVE = "SOLVE";
    }

    public enum IIMPlayType
    {
        macro,
        js,
        macroFromjs
    }
    public class Macro : ViewModelBase
    {
        public string Command { get; set; }
        public string Value { get; set; }
        private string line;
        public string Line
        {
            get { return line; }
            set { line = value; RaisePropertyChanged("Line"); }
        }


        private System.Windows.Media.Brush bGRunning;
        public System.Windows.Media.Brush BGRunning
        {
            get { return bGRunning; }
            set { bGRunning = value; RaisePropertyChanged("BGRunning"); }
        }

        public Macro()
        {
            SetTransparent(); 
        }

        public void SetTransparent()
        {
            BGRunning = System.Windows.Media.Brushes.White;
        }

        public void SetGreen()
        {
            BGRunning = System.Windows.Media.Brushes.LightGreen;
        }
    }
    public class MacroPlayer : ViewModelBase
    {
        public ObservableCollection<Macro> Macros { get; set; }
        private int sIMacroCommand;
        public int SIMacroCommand
        {
            get { return sIMacroCommand; }
            set { sIMacroCommand = value; RaisePropertyChanged("SIMacroCommand"); }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                RaisePropertyChanged("IsRunning");
            }
        }


        public MacroPlayer()
        {
            Macros = new ObservableCollection<Macro>();
        }

        public void InitMacroCommandsList(string macroCode)
        {
            Macros.Clear();

            string[] macroLines = macroCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (macroLines.Length == 1) macroLines = macroCode.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var macroLine in macroLines)
            {
                string line = macroLine.Trim();
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line) || line.Length <= 1 || line.StartsWith("'")) continue;

                if (line.Contains("=") || line.Contains(" "))
                {
                    //string[] macro = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    string command = line.Remove(line.IndexOf(' '));
                    string value = line.Substring(line.IndexOf(' ') + 1);
                    //var regex = new Regex(Regex.Escape(line));
                    //string value = regex.Replace(line, command, 1);
                    //string value = line.Replace(command, "").Trim();
                    Macros.Add(new Macro() { Command = command, Value = value, Line = macroLine });
                }
                else
                {
                    Macros.Add(new Macro() { Command = line, Value = "" , Line = macroLine });
                }
            }
        }

        public bool HasNext { get { return Macros.Any(); } }

        public Macro GetNextMacro()
        {
            if (Macros.Any())
            {
                Macro m = Macros[0];
                Macros.RemoveAt(0);
                return m;
            }
            else
            {
                return null;
            }
        }
    }
    public class MacroFile : ViewModelBase
    {
        public event Action<MacroFile> OnRunThisMacro = delegate { };

        public ICommand OnCommandFromView { get; set; }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; RaisePropertyChanged("FileName"); }
        }
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; RaisePropertyChanged("FilePath"); }
        }
        private string tooltipType;
        public string TooltipType
        {
            get { return tooltipType; }
            set { tooltipType = value; RaisePropertyChanged("TooltipType"); }
        }
        private string tooltipText;
        public string TooltipText
        {
            get { return tooltipText; }
            set { tooltipText = value; RaisePropertyChanged("TooltipText"); }
        }

        private bool isFolder;
        public bool IsFolder
        {
            get { return isFolder; }
            set { isFolder = value; RaisePropertyChanged("IsFolder"); }
        }


        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; RaisePropertyChanged("IsSelected"); }
        }
        public bool DontLoadFromExpanded { get; set; }
        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                if (!DontLoadFromExpanded && isExpanded && IsFolder)
                {
                    var mac = NextMacros.FirstOrDefault(m => m.FilePath == "DummyForExpandingToHaveToggle");
                    if (mac != null) NextMacros.Remove(mac);
                    if (NextMacros.Count == 0) LoadNextMacrosAsync();
                }
                RaisePropertyChanged("IsExpanded");
            }
        }
        private bool isMacroChecked;
        public bool IsMacroChecked
        {
            get { return isMacroChecked; }
            set { isMacroChecked = value; RaisePropertyChanged("IsMacroChecked"); }
        }

        public MacroFile ParentMacro { get; set; }
        public ObservableCollection<MacroFile> NextMacros { get; set; }

        public MacroFile()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            NextMacros = new ObservableCollection<MacroFile>();
        }

        public async void OnCommandFromView_Raised(object obj)
        {
            try
            {
                string param = obj as string;
                if (param == null) return;
                switch (param)
                {
                    case "FolderNew":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        SetNameAndDataWindow folderNewSetNameWindow = new SetNameAndDataWindow();
                        folderNewSetNameWindow.Title = "Rename";
                        folderNewSetNameWindow.tblockInfo.Text = "Choose A New Name.";
                        folderNewSetNameWindow.tbInputText.Text = string.Empty;
                        folderNewSetNameWindow.ShowDialog();
                        if (!folderNewSetNameWindow.OkClicked) return;
                        if (folderNewSetNameWindow.tbInputText.Text.IsNullOrEmpty()) return;

                        string newDirName = FilePath + "\\" + folderNewSetNameWindow.tbInputText.Text;
                        await Task.Run(() => Directory.CreateDirectory(newDirName));
                        var newmacrofolder = new MacroFile()
                        {
                            FilePath = newDirName,
                            FileName = folderNewSetNameWindow.tbInputText.Text,
                            IsFolder = true
                        };
                        newmacrofolder.OnRunThisMacro += OnRunThisMacro;
                        NextMacros.Add(newmacrofolder);
                        break;

                    case "FolderAddMacros":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        System.Windows.Forms.OpenFileDialog addMacroFiles = new System.Windows.Forms.OpenFileDialog();
                        addMacroFiles.Multiselect = true;
                        addMacroFiles.RestoreDirectory = true;
                        addMacroFiles.Title = "Select Macro Files";
                        if (addMacroFiles.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (addMacroFiles.FileNames == null || addMacroFiles.FileNames.Length == 0) return;

                            // Read the files
                            foreach (string file in addMacroFiles.FileNames)
                            {
                                FileInfo fi = new FileInfo(file);
                                string topath = FilePath + "\\" + fi.Name;
                                if (File.Exists(topath)) continue;

                                await Task.Run(() => fi.CopyTo(topath, true));

                                MacroFile macrofile = new MacroFile()
                                {
                                    IsFolder = false,
                                    FilePath = fi.FullName,
                                    FileName = fi.Name,
                                    ParentMacro = this,
                                };
                                macrofile.OnRunThisMacro += OnRunThisMacro;
                                NextMacros.Add(macrofile);
                            }

                            IsExpanded = true;
                        }
                        break;

                    case "FolderRename":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        if (ParentMacro == null)
                        {
                            "Cant Rename this folder".Show();
                            return;
                        }
                        SetNameAndDataWindow folderSetNameWindow = new SetNameAndDataWindow();
                        folderSetNameWindow.Title = "Rename";
                        folderSetNameWindow.tblockInfo.Text = "Enter A New Name.";
                        folderSetNameWindow.tbInputText.Text = FileName;
                        folderSetNameWindow.ShowDialog();
                        if (!folderSetNameWindow.OkClicked) return;
                        if (folderSetNameWindow.tbInputText.Text.IsNullOrEmpty() || folderSetNameWindow.tbInputText.Text.Trim() == FileName.Trim()) return;
                        string newName = folderSetNameWindow.tbInputText.Text.Trim();
                        string newPath = FilePath.Remove(FilePath.LastIndexOf("\\"));
                        newPath = newPath + "\\" + newName;
                        await Task.Run(() =>
                        {
                            // if (!Directory.Exists(newPath)) MyFilesDatabase.Directory.CreateDirectory(newPath);
                            Directory.Move(FilePath, newPath);
                            // if (Directory.Exists(FilePath)) Directory.Delete(FilePath);
                        });
                        FileName = newName;
                        FilePath = newPath;
                        Reset();
                        break;

                    case "FolderDelete":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        if (ParentMacro == null)
                        {
                            "Cant Delete this folder".Show();
                            return;
                        }
                        if (MessageBox.Show("Are you sure you want to delete " + FileName, "Are you Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

                        await Task.Run(() => Directory.Delete(FilePath, true));
                        NextMacros.Clear();
                        ParentMacro.NextMacros.Remove(this);
                        break;


                    case "MacroRun":
                        if (isFolder) return;
                        OnRunThisMacro(this);
                        break;

                    case "MacroEdit":
                        if (System.IO.File.Exists(FilePath))
                        {
                            MacroEditsWindow med = new MacroEditsWindow();
                            med.LoadFile(FilePath, FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory));
                            med.Show();
                        }
                        break;

                    case "MacroRename":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        SetNameAndDataWindow macroSetNameWindow = new SetNameAndDataWindow();
                        macroSetNameWindow.Title = "Rename";
                        macroSetNameWindow.tblockInfo.Text = "Enter A New Name.";
                        macroSetNameWindow.tbInputText.Text = FileName;
                        macroSetNameWindow.ShowDialog();
                        if (!macroSetNameWindow.OkClicked) return;
                        if (macroSetNameWindow.tbInputText.Text.IsNullOrEmpty() || macroSetNameWindow.tbInputText.Text.Trim() == FileName.Trim()) return;

                        string newNameMacro = macroSetNameWindow.tbInputText.Text.Trim();
                        string newPathMacro = FilePath.Remove(FilePath.LastIndexOf("\\"));
                        newPathMacro = newPathMacro + "\\" + newNameMacro;

                        FileInfo renameFI = new FileInfo(FilePath);
                        await Task.Run(() => renameFI.MoveTo(newPathMacro));

                        FileName = newNameMacro;
                        FilePath = newPathMacro;
                        break;

                    case "MacroDelete":
                        if (FilePath.Contains(AppDomain.CurrentDomain.BaseDirectory))
                        {
                            "Cant Change Pre Built Scripts".Show();
                            return;
                        }
                        if (ParentMacro == null)
                        {
                            "Cant Delete Base Folder Macros".Show();
                            return;
                        }
                        if (MessageBox.Show("Are you sure you want to delete " + FileName, "Are you Sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

                        if (!File.Exists(FilePath)) return;
                        await Task.Run(() => File.Delete(FilePath));
                        ParentMacro.NextMacros.Remove(this);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                ex.Message.Show();
            }
        }


        public void Reset()
        {
            NextMacros.Clear();
            IsExpanded = false;
            NextMacros.Add(new MacroFile() { FilePath = "DummyForExpandingToHaveToggle" });
            IsExpanded = true;
        }

        public void ThreadSafeAddMacro(MacroFile fileToAdd)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                NextMacros.Add(fileToAdd);
            });
        }

        public async Task LoadNextMacrosAsync()
        {
            await Task.Run(() =>
            {
                var basedirmacros = new DirectoryInfo(FilePath);
                if (basedirmacros.Exists) LoadAllChildMacros(basedirmacros);
            });
        }
        internal void LoadAllChildMacros(DirectoryInfo basedirmacros, bool isBrowseoIA = false)
        {
            try
            {
                var bases = basedirmacros.GetDirectories();
                if (bases != null)
                {
                    foreach (var dir in bases)
                    {
                        if (MacroSettings.IsIgnorableDirectoryOrFile(dir.FullName)) continue;
                        MacroFile macroDir = new MacroFile()
                        {
                            IsFolder = true,
                            FilePath = dir.FullName,
                            FileName = dir.Name == "IFTT"?"IFTTT":dir.Name,
                            ParentMacro = this,
                        };
                        macroDir.OnRunThisMacro += OnRunThisMacro;
                        ThreadSafeAddMacro(macroDir);

                        var bases2 = dir.GetDirectories();
                        if (bases2 != null)
                        {
                            foreach (var ddir in bases2)
                            {

                                if (MacroSettings.IsIgnorableDirectoryOrFile(ddir.FullName)) continue;
                                MacroFile macronextDir = new MacroFile()
                                {
                                    IsFolder = true,
                                    FilePath = ddir.FullName,
                                    FileName = ddir.Name,
                                    ParentMacro = macroDir,
                                };
                                macronextDir.OnRunThisMacro += OnRunThisMacro;
                                macronextDir.ThreadSafeAddMacro(new MacroFile() { FilePath = "DummyForExpandingToHaveToggle" });
                                macroDir.ThreadSafeAddMacro(macronextDir);
                            }
                        }

                        var files = dir.GetFiles();
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                if (MacroSettings.IsIgnorableDirectoryOrFile(file.FullName)) continue;
                                MacroFile macrofile = new MacroFile()
                                {
                                    IsFolder = false,
                                    FilePath = file.FullName,
                                    FileName = file.Name,
                                    ParentMacro = macroDir,
                                };
                                macrofile.TooltipType = GetTooltipType(file.Name, macrofile);
                                macrofile.OnRunThisMacro += OnRunThisMacro;
                                macroDir.ThreadSafeAddMacro(macrofile);
                            }
                        }
                    }
                }
            }
            catch { }

            try
            {
                var files1 = basedirmacros.GetFiles();
                if (files1 != null)
                {
                    foreach (var file in files1)
                    {
                        if (MacroSettings.IsIgnorableDirectoryOrFile(file.FullName)) continue;
                        MacroFile macrofile = new MacroFile()
                        {
                            IsFolder = false,
                            FilePath = file.FullName,
                            FileName = file.Name,
                            ParentMacro = this,
                        };
                        macrofile.TooltipType = GetTooltipType(file.Name, macrofile);
                        macrofile.OnRunThisMacro += OnRunThisMacro;
                        ThreadSafeAddMacro(macrofile);
                    }
                }
            }
            catch { }
        }

        private string GetTooltipType(string name, MacroFile mfile)
        {
            if (name.Contains(".")) name = name.Remove(name.IndexOf("."));
            switch (name)
            {
                case "YouTube Video Upload":
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltipvideoUpload.png";

                case "Twitter Follow":
                case "Reddit Save":
                case "Reddit Subscribe To Subreddit":
                case "Reddit Random Vote":
                case "Pinterest Follow By Keyword":
                case "Google+ Repost By Keyword":
                case "Google+ Like Post By Keyword":
                case "Google+ Join Communities":
                case "Google+ Follow By KW":
                case "Google+ Join Commuities By KW":
                case "Google+ Like By KW":
                case "Google+ Share By KW":
                case "FB Like Pages By KW":
                case "FB Join Goups By KW":
                case "Twitter Retweet By Kw":
                case "Twitter Like By Kw":
                case "Tumblr Follow Top":
                case "Tumblr Follow":
                case "Tumblr Like":
                    mfile.TooltipText = "Keywords,times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "wordpress IA":
                    mfile.TooltipText = "KW,Sort Feed (relevance=1 or date=2),Like Posts (1=yes or 2=no),Follow (1=yes or no=2, How many Actions (1-99)";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";


                case "Reddit Subreddit Scrape":
                    mfile.TooltipText = "Keyword, limit, sort";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Reddit Subreddit 3 in 1":
                case "Reddit Comment":
                    mfile.TooltipText = "Keyword, comment, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Pinterest Repin and Like From Search":
                    //case "Reddit Save":
                    mfile.TooltipText = "Keyword, board, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Pinterest Create Board":
                    mfile.TooltipText = "Board name, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "FB Photo Upload":
                    mfile.TooltipText = "Local filepath, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "FB Like Posts By URL":
                    mfile.TooltipText = "URL, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Facebook Post":
                case "Google+ Post":
                case "Twitter Post":
                    mfile.TooltipText = "Post Text, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Twitter Create Twitter List":
                    mfile.TooltipText = "Keyword ,list name,description";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Twitter Reply By Kw":
                    mfile.TooltipText = "Keyword,reply text, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Google+ Post With URL":
                    mfile.TooltipText = "url, post text, times to run";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                //case "Diigo_via_Gmail":
                //    mfile.TooltipText = "email";
                //    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Youtube like video after wait":
                    mfile.TooltipText = "x time to wait,video url";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Youtube subscribe by KW":
                    mfile.TooltipText = "keywords...";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "FB Page Cover Upload":
                    mfile.TooltipText = @"facebook.com/pagename,C:\filepath\to\image";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "FB Profile Cover Upload":
                case "Pinterest Change Image":
                    mfile.TooltipText = @"C:\filepath\to\image";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Buffer Connect G+ Page":
                    mfile.TooltipText = "Page Name";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";
                    
                case "URL Shortener":
                    mfile.TooltipText = "URLs";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Pinterest Edit Url":
                    mfile.TooltipText = "URL";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "LinkedIn Join Groups By KW":
                    mfile.TooltipText = "Keywords";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Pinterest Edit Description":
                    mfile.TooltipText = "Description";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "LinkedIn Share Post By KW then comment":
                    mfile.TooltipText = "Keyword,comment";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Google Webmasters Indexer":
                    mfile.TooltipText = "url";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Tumblr Upload Image [FILE]":
                case "Tumblr Upload Video [FILE]":
                    mfile.TooltipText = "filepath,post text,tags";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Tumblr Upload Image [URL]":
                case "Tumblr Upload Video [URL]":
                    mfile.TooltipText = "url,post text,tags";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";


                case "Bit":
                case "Blog":
                case "Blogger":
                case "Bufferapp":
                case "Delicious":
                case "Diigo":
                case "Diigo_via_Gmail":
                case "Evernote":
                case "Facebook_Page":
                case "G_Drive":
                case "GetPocket":
                case "Instapaper":
                case "Medium":
                case "OneNote":
                case "Pinboard":
                case "Tumblr":
                case "Twitter":
                case "RSS-to-Blogger-T2":
                case "RSS-to-Medium-T2":
                case "RSS-to-Tumblr-T2":
                case "RSS-to-WP-T2":
                    if (mfile.FilePath.Contains("Video Recipe URL") || mfile.FilePath.Contains("YouTube Like Recipes")) return "";

                    mfile.TooltipText = "RSS feed Url...";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Blogger-editable":
                case "Drive-editable":
                case "Evernote-editable":
                case "OneNote-editable":
                case "WordPress-editable":
                case "Delicious-editable":
                case "Diigo-editable":
                case "Twitter-editable":
                case "Tumblr-editable":
                    mfile.TooltipText = "Copy, Paste & Edit from first line in file...";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "FB Photo Upload & Description":
                    mfile.TooltipText = @"C:\full\path\to\file.png,description";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Tumblr Reblog":
                    mfile.TooltipText = "keyword,comment,tag1 tag2 tag3 ...";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Bing Crawl":
                case "Google Crawl":
                case "Youtube Crawl":
                    mfile.TooltipText = "# of pages,keyword";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";
                case "Bing Image Crawl":
                case "Google Image Crawl":
                case "Bing Image Crawl and Download":
                case "Bing Image Downloader":
                case "Google Image Crawl and Download":
                case "Google Image Downloader":
                    mfile.TooltipText = "keyword, # of images";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Instagram Hashtag":
                    mfile.TooltipText = "# of posts,keyword";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Follow From Follow Window":
                    mfile.TooltipText = "# of people to follow";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Invite From Liked Window":
                    mfile.TooltipText = "# of people to invite";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Follow From Suggestions (Auto)":
                    mfile.TooltipText = "# of times to scroll,page url";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Follow And Like From Link":
                case "Follow From Link":
                case "Like From Link":
                    mfile.TooltipText = "url of post";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";

                case "Hashtag Search Follow And Like":
                case "Hashtag Search Follow":
                case "Hashtag Search Like":
                    mfile.TooltipText = "# of times, keyword";
                    return "pack://application:,,,/Organiser.Common;component/Image/tooltip_blank.png";
                default:
                    return "";
            }
        }
    }
    public class MacroManger : ViewModelBase
    {
        //private static MacroManger instance;
        //public static MacroManger Instance
        //{
        //    get
        //    {
        //        if (instance == null) instance = new MacroManger();
        //        return instance;
        //    }
        //}

        public event Action<MacroManger, IIMPlayType, int> OnPlayMacro = delegate { };
        public event Action OnStopRequested = delegate { };
        public event Action OnMacroDone = delegate { };

        public ICommand OnCommandFromView { get; set; }

        public ObservableCollection<MacroFile> MacroFilesBase { get; set; }

        private string currentLoopPos;
        public string CurrentLoopPos
        {
            get { return currentLoopPos; }
            set { currentLoopPos = value; RaisePropertyChanged("CurrentLoopPos"); }
        }
        private int maxLoop;
        public int MaxLoop
        {
            get { return maxLoop; }
            set { maxLoop = value;
                if (value <= 0) MaxLoop = 1;
                RaisePropertyChanged("MaxLoop"); }
        }

        private string updateText;
        public string UpdateText
        {
            get { return updateText; }
            set { updateText = value; RaisePropertyChanged("UpdateText"); }
        }

        private MacroPlayer macroPlayer;
        public MacroPlayer MacroPlayer
        {
            get { return macroPlayer; }
            set { macroPlayer = value; RaisePropertyChanged("MacroPlayer"); }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                AnyRunning = value;
                MacroPlayer.IsRunning = value;
                IsPlayEnabled = !isRunning;
                if (!isRunning)
                {
                    //StopRequested = false;
                    UpdateText = "";
                    ContentForPaused = "Pause";
                }
                RaisePropertyChanged("IsRunning");
            }
        }
        public static bool AnyRunning = false;
        private bool isPlayEnabled;
        public bool IsPlayEnabled
        {
            get { return isPlayEnabled; }
            set { isPlayEnabled = value; RaisePropertyChanged("IsPlayEnabled"); }
        }

        private string contentForPaused;
        public string ContentForPaused
        {
            get { return contentForPaused; }
            set { contentForPaused = value; RaisePropertyChanged("ContentForPaused"); }
        }
        public bool Paused { get; set; }

        private bool stopRequested;
        public bool StopRequested
        {
            get { return stopRequested; }
            set
            {
                stopRequested = value;
                if (value)
                {
                    Paused = false;
                   // IsRunning = false;
                    OnStopRequested();
                }
                RaisePropertyChanged("StopRequested");
            }
        }
        public bool Skipped { get; set; }

        private string dataSourceSlideoutText;
        public string DataSourceSlideoutText
        {
            get { return dataSourceSlideoutText; }
            set { dataSourceSlideoutText = value; RaisePropertyChanged("DataSourceSlideoutText"); }
        }
        public string FileText { get; set; }
        public string FileDirectory { get; set; }
        public int CurrentJSDatasourceLoopPos { get; set; }
        public int DatasourceMaxLoop { get; set; }
        public int JSLoopPos { get; set; }
        public string SelectedMacroPlayingFileName { get; set; }
        public string SelectedMacroPlayingFilePath { get; set; }
        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        public SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public MacroManger()
        {
            OnCommandFromView = new RelayCommand(OnCommandFromView_Raised);
            MacroFilesBase = new ObservableCollection<MacroFile>();
            MaxLoop = 1;
            MacroPlayer = new MacroPlayer();
            IsPlayEnabled = true;
            ContentForPaused = "Pause";
            DataSourceSlideoutText = "";
        }

        public async Task recursivePlaymacro(string param)
        {
            switch (param)
            {
                case "MacroPlayChecked":
                case "MacroPlayLoopChecked":
                    var checkedMAcros = new List<MacroFile>();
                    GetCheckedMAcros(MacroFilesBase, checkedMAcros);
                    foreach (var mac in checkedMAcros)
                    {
                        if (StopRequested && !Skipped) break;
                        else if (Skipped && StopRequested) StopRequested = Skipped = false;
                        mac.IsSelected = true;
                        await recursivePlaymacro(param == "MacroPlayChecked" ? "MacroPlay" : "MacroPlayLoop");
                        //OnCommandFromView_Raised(type == "MacroPlayChecked" ? "MacroPlay" : "MacroPlayLoop");
                        mac.IsSelected = false;
                    }
                    break;

                case "MacroPlay":
                case "MacroPlayLoop":
                    var selectedMacro = await GetSelectedMacro(MacroFilesBase, "");
                    if (selectedMacro != null)
                    {
                        await semaphoreSlim.WaitAsync();

                        if (!StopRequested)
                        {
                            if (!File.Exists(selectedMacro.FilePath))
                            {
                                "Selected file not found".Show();
                                return;
                            }
                            IsRunning = true;
                            FileText = File.ReadAllText(selectedMacro.FilePath);
                            var fi = new FileInfo(selectedMacro.FilePath);
                            FileDirectory = fi.Directory.FullName;
                            SelectedMacroPlayingFileName = selectedMacro.FileName;
                            SelectedMacroPlayingFilePath = selectedMacro.FilePath;
                            IIMPlayType iimOrJs = fi.Extension == ".iim" ? IIMPlayType.macro : IIMPlayType.js;
                            //MacroPlayer.InitMacroCommandsList();
                            if (iimOrJs == IIMPlayType.macro)
                            {
                                OnPlayMacro(this, iimOrJs, param == "MacroPlay" ? 1 : MaxLoop);
                            }
                            else
                            {
                                if (MaxLoop <= 0) MaxLoop = 1;
                                DatasourceMaxLoop = 1;
                                for (CurrentJSDatasourceLoopPos = 0; CurrentJSDatasourceLoopPos < DatasourceMaxLoop; CurrentJSDatasourceLoopPos++)
                                {
                                    if (StopRequested) break;
                                    for (JSLoopPos = 0; JSLoopPos < MaxLoop; JSLoopPos++)
                                    {
                                        if (StopRequested) break;
                                        IsRunning = true;
                                        OnPlayMacro(this, iimOrJs, param == "MacroPlay" || iimOrJs == IIMPlayType.js ? 1 : MaxLoop);
                                    }
                                }
                            }
                        }

                        await semaphoreSlim.WaitAsync();
                        OnMacroDone();
                        SafeReleaseSemephore();
                    }
                    break;

                default: break;
            }
        }

        public async void OnCommandFromView_Raised(object obj)
        {
            try
            {
                string param = obj as string;
                if (param == null) return;
                switch (param)
                {
                    case "MacroPlay":
                    case "MacroPlayLoop":
                    case "MacroPlayChecked":
                    case "MacroPlayLoopChecked":
                        DataSourceSlideoutText = DataSourceSlideoutText.Trim();
                        StopRequested = false;
                        Skipped = false;
                        SafeReleaseSemephore();
                        await recursivePlaymacro(param);
                        IsRunning = false;
                        break;

                    case "MacroPause":
                        switch (ContentForPaused)
                        {
                            case "Pause":
                                Paused = true;
                                ContentForPaused = "Continue";
                                break;

                            case "Continue":
                                Paused = false;
                                ContentForPaused = "Pause";
                                break;

                            default:
                                break;
                        }
                        break;

                    case "MacroStop":
                        StopRequested = true;
                        break;

                    case "MacroSkip":
                        StopRequested = Skipped = true;
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                if(ex.Message != "The semaphore has been disposed.")ex.Message.Show();
                StopRequested = true;
                SafeReleaseSemephore();
                OnMacroDone();
            }
        }

        public void SafeReleaseSemephore()
        {
            try
            {
                semaphoreSlim.Release();
            }
            catch { }
        }

        public void GetCheckedMAcros(ObservableCollection<MacroFile> macroFilesBase, List<MacroFile> checkedMAcros)
        {
            foreach (var mac in macroFilesBase)
            {
                if (mac.IsMacroChecked)
                {
                    checkedMAcros.Add(mac);
                }

                if(mac.NextMacros != null)
                {
                    GetCheckedMAcros(mac.NextMacros, checkedMAcros);
                }
            }
        }

        private async Task<MacroFile> GetSelectedMacro(ObservableCollection<MacroFile> macFiles, string path, MacroFile toReturn = null)
        {
            if (toReturn != null) return toReturn;

            foreach (var mac in macFiles)
            {
                if (path == "")
                {
                    if (mac.IsSelected)
                    {
                        toReturn = mac;
                        return toReturn;
                    }
                }
                else
                {
                    if (mac.FilePath == path)
                    {
                        toReturn = mac;
                        return toReturn;
                    }
                }

                if (path != "" && mac.IsFolder)
                {
                    if(!mac.IsExpanded)
                    {
                        mac.DontLoadFromExpanded = true;
                        if (mac.NextMacros != null)
                        {
                            var nmac = mac.NextMacros.FirstOrDefault(m => m.FilePath == "DummyForExpandingToHaveToggle");
                            if (nmac != null) mac.NextMacros.Remove(nmac);
                        }
                        if (mac.NextMacros == null || mac.NextMacros.Count == 0) await mac.LoadNextMacrosAsync();
                        mac.DontLoadFromExpanded = false;
                    }
                }
                if (mac.NextMacros != null)
                {
                    toReturn = await GetSelectedMacro(mac.NextMacros, path, toReturn);
                }
            }

            return toReturn;
        }

        public async Task LoadIMacros(bool force)
        {
            if (force) MacroFilesBase.Clear();

            if (MacroFilesBase.Count > 0) return;

            await Task.Run(() =>
            {
                string bdir = MacroSettings.GetBuitInMacrosBaseDir();
                bdir = bdir.Replace("\\\\", "\\");
                var baseDirMain = new DirectoryInfo(bdir);
                MacroFile baseMacroDir1 = new MacroFile()
                {
                    IsFolder = true,
                    FilePath = MacroSettings.DefaulFoldertMacros,
                    FileName = baseDirMain.Name,
                    ParentMacro = null,
                };
                baseMacroDir1.OnRunThisMacro += BaseMacroDir_OnRunThisMacro;
                Application.Current.Dispatcher.Invoke(() => { MacroFilesBase.Add(baseMacroDir1); });
                baseMacroDir1.LoadAllChildMacros(baseDirMain);

                var basedirmacros = new DirectoryInfo(MacroSettings.DefaulFoldertMacros);

                MacroFile baseMacroDir = new MacroFile()
                {
                    IsFolder = true,
                    FilePath = MacroSettings.DefaulFoldertMacros,
                    FileName = basedirmacros.Name,
                    ParentMacro = null,
                };
                baseMacroDir.OnRunThisMacro += BaseMacroDir_OnRunThisMacro;
                Application.Current.Dispatcher.Invoke(() => { MacroFilesBase.Add(baseMacroDir); });
                baseMacroDir.LoadAllChildMacros(basedirmacros,true);
            });
        }

        private void BaseMacroDir_OnRunThisMacro(MacroFile macro)
        {
            macro.IsSelected = true;
            OnCommandFromView_Raised("MacroPlay");
        }

        //public async Task SetMacroActiveByPath(string macroPathorurl)
        //{
        //    var toSelectedMacro = await GetSelectedMacro(MacroFilesBase, macroPathorurl);
        //    if (toSelectedMacro != null)
        //    {
        //        toSelectedMacro.IsSelected = true;
        //        OnCommandFromView_Raised("MacroPlayLoop");
        //    }
        //}

        public async Task SetMacroActiveByPaths(List<string> paths)
        {
            uncheckAll(MacroFilesBase);
            foreach (var path in paths)
            {
                var toSelectedMacro = await GetSelectedMacro(MacroFilesBase, path);
                if (toSelectedMacro != null)
                {
                    toSelectedMacro.IsMacroChecked = true;
                }
            }

            OnCommandFromView_Raised("MacroPlayLoopChecked");
        }

        private void uncheckAll(ObservableCollection<MacroFile> macFiles)
        {
            foreach (var mac in macFiles)
            {
                mac.IsMacroChecked = false;

                if (mac.NextMacros != null)
                {
                    uncheckAll(mac.NextMacros);
                }
            }
        }
    }
}
