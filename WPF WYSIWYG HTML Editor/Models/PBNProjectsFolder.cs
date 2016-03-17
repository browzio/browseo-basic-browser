using Organiser.Common.Classes;
using Organiser.Common.Windows;
using SocialOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_WYSIWYG_HTML_Editor.MVVM;
using WPF_WYSIWYG_HTML_Editor.XAML;

namespace WPF_WYSIWYG_HTML_Editor.Models
{
    public class PBNProjectsFolder : ViewModelBase
    {
        private string folderName;
        public string FolderName
        {
            get { return folderName; }
            set { folderName = value; RaisePropertyChanged("FolderName"); }
        }

        public int SIPBN { get; set; }

        //RefreshPBNVault
        public ICommand VautContextMenu { get; set; }
        public event Action<object> OnVautContextMenuClick_Clicked = delegate { };

        public ObservableCollection<PBNProject> PBNProjects { get; set; }

        public PBNProjectsFolder()
        {
            PBNProjects = new ObservableCollection<PBNProject>();


            VautContextMenu = new RelayCommand(OnVautContextMenuClick);
        }

        private async void OnVautContextMenuClick(object obj)
        {
            try
            {
                string param = Convert.ToString(obj);
                if (param == "CopyLinkPBN" || param == "CopyLinkMoney")
                {
                    PersonData profile = MyFilesDatabase.GetSubProjectPersonData(PBNProjects[SIPBN].FilePath);
                    if (profile == null || string.IsNullOrEmpty(profile.WebAddress) || string.IsNullOrWhiteSpace(profile.WebAddress)) return;
                    MyFilesDatabase.SetClipboardText(profile.WebAddress);
                }
                else if (param == "BACKLINK_HISTORY")
                {
                    try
                    {
                        BacklinksHistoryWindow bhw = new BacklinksHistoryWindow();
                        BacklinksHistoryVM vm = new BacklinksHistoryVM();
                        vm.FillHistoryList(MyFilesDatabase.GetSubProjectPersonData(PBNProjects[SIPBN].FilePath));
                        bhw.DataContext = vm;
                        bhw.Show();
                    }
                    catch { }
                }
                else if (param == "RenameFolderVault" || param == "RenameFolderMoney")
                {
                    try
                    {
                        SetNameAndDataWindow setNameWindow = new SetNameAndDataWindow();
                        setNameWindow.Title = "Rename";
                        setNameWindow.tblockInfo.Text = "Enter a new name for folder " + FolderName;
                        setNameWindow.ShowDialog();
                        if (!setNameWindow.OkClicked) return;
                        string inputtext = setNameWindow.tbInputText.Text;
                        if (inputtext.IsNullOrEmpty()) return;

                        await Task.Run(() =>
                        {
                            string pathForNewFolder = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", inputtext);
                            if (inputtext.ToLower().Trim() == FolderName.ToLower().Trim() ||
                                Directory.Exists(pathForNewFolder))
                            {
                                "A name like that already exists in your vault".Show();
                                return;
                            }

                            Directory.CreateDirectory(pathForNewFolder);
                            if (param == "RenameFolderVault")
                            {
                                string file = Path.Combine(pathForNewFolder, XmlRpcVM.PBNCONFIG);
                                string oldFile = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", FolderName, XmlRpcVM.PBNCONFIG);
                                if (!File.Exists(oldFile)) return;

                                string vaultText = File.ReadAllText(oldFile);
                                File.WriteAllText(file, vaultText);
                                File.Delete(oldFile);
                            }
                            else
                            {
                                string file = Path.Combine(pathForNewFolder, XmlRpcVM.PBNCONFIGMONEY);
                                string oldFile = Path.Combine(MyFilesDatabase.GetBaseDir(), "PBNVault", FolderName, XmlRpcVM.PBNCONFIGMONEY);
                                if (!File.Exists(oldFile)) return;

                                string vaultText = File.ReadAllText(oldFile);
                                File.WriteAllText(file, vaultText);
                                File.Delete(oldFile);
                            }
                            FolderName = inputtext;
                        });
                    }
                    catch
                    {
                        "Rename Failed".Show();
                    }
                }
                else if (param == "RemovePBN" || param == "RemoveMoney")
                {
                    PersonData profilep = MyFilesDatabase.GetSubProjectPersonData(PBNProjects[SIPBN].FilePath);
                    profilep.InMonney = false;
                    profilep.InPBNVault = false;
                    MyFilesDatabase.ReWrightProjData(profilep, PBNProjects[SIPBN].FilePath);
                    PBNProjects.RemoveAt(SIPBN);
                }
                else
                {
                    OnVautContextMenuClick_Clicked(obj);
                }
            }
            catch
            {

            }
        }
    }
}
