using Delimon.Win32.IO;
using Organiser.Common.Classes;
using Organiser.Common.Models;
using Organiser.Common.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Organiser.Common.ViewModels
{
    public class ChooseProjectsVM : ViewModelBase
    {
        public ICommand SelectFolderSelect_Click { get; set; }
        public ObservableCollection<ProjectImported> SavedProjectsListAdded { get; set; }

        public ChooseProjectsVM()
        {
            SelectFolderSelect_Click = new RelayCommand(SelectFolderSelect_ClickIsChecked);
            SavedProjectsListAdded = new ObservableCollection<ProjectImported>();
        }

        public async Task InitProjectsWindowList()
        {
            List<KeyValuePair<string, string>> allprojectNames = await Task.Run(()=> 
            {
                 return MyFilesDatabase.GetAllProjectsAndDirs(true);
            });

            SavedProjectsListAdded.Clear();

            foreach (var pro in allprojectNames)
            {
                var proj = new ProjectImported()
                {
                    Name = pro.Key,
                    FilePath = pro.Value,
                    IsFolder = File.Exists(MyFilesDatabase.Path.Combine(pro.Value, "FolderData.ini")),
                };

                var dirs = new DirectoryInfo(proj.FilePath).GetDirectories();
                if (proj.IsFolder ||
                    (dirs != null && dirs.Count(d => d.Name.Contains("_tier_")) > 0)) proj.VisibleHasNext = Visibility.Visible;

                var deep = proj.FilePath.Replace(MyFilesDatabase.GetBaseProjectsDir(), "").Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                proj.TabMargin = new Thickness(deep.Length <= 1 ? 5 : deep.Length * 15, 5, 5, 5);

                if (proj.TabMargin.Left > 5) proj.ProjVisible = Visibility.Collapsed;



                proj.OnCheckedFolder += Proj_OnCheckedFolder;
                proj.OnClickedExpand += Proj_OnClickedExpand;
                SavedProjectsListAdded.Add(proj);
            }

            var importedToMove = new ObservableCollection<ProjectImported>();
            for (int i = SavedProjectsListAdded.Count - 1; i >= 0; i--)
            {
                var pro = SavedProjectsListAdded[i];
                if (pro.IsFolder && pro.TabMargin.Left == 5)
                {
                    for (int j = i; j < SavedProjectsListAdded.Count; j++)
                    {
                        var pro2 = SavedProjectsListAdded[j];
                        if (pro2 != pro && pro2.IsFolder && pro2.TabMargin.Left == 5)
                            break;

                        importedToMove.Add(pro2);
                    }
                }
            }
            int indexForInsert = 0;
            foreach (var p in importedToMove)
            {
                if (p.IsFolder && p.TabMargin.Left == 5) indexForInsert = 0;

                SavedProjectsListAdded.Remove(p);
                SavedProjectsListAdded.Insert(indexForInsert, p);
                indexForInsert++;
            }
        }


        private async void Proj_OnClickedExpand(ProjectImported proj, bool expand)
        {
            await ExpandProj(proj, expand, true);
        }

        private async Task ExpandProj(ProjectImported proj, bool expand, bool animate)
        {
            for (int i = SavedProjectsListAdded.IndexOf(proj); i < SavedProjectsListAdded.Count; i++)
            {
                var pro = SavedProjectsListAdded[i];
                if (pro == proj) continue;

                if (expand)
                {
                    if (proj.TabMargin.Left == 5 && pro.TabMargin.Left > 30 ||
                        (proj.TabMargin.Left != 5 && pro.TabMargin.Left > proj.TabMargin.Left + 15)) continue;
                    if (pro.TabMargin.Left == proj.TabMargin.Left ||
                        (proj.ProjVisible == Visibility.Visible && pro.TabMargin.Left < proj.TabMargin.Left)) break;

                    pro.ProjVisible = Visibility.Visible;
                }
                else
                {
                    if (pro.ProjVisible != Visibility.Visible) continue;
                    if (pro.TabMargin.Left == proj.TabMargin.Left || pro.TabMargin.Left < proj.TabMargin.Left) break;

                    pro.AngleTransformImage = 0;
                    pro.ProjVisible = Visibility.Collapsed;
                }

                if (animate) await Task.Delay(20);
            }
        }

        private void Proj_OnCheckedFolder(ProjectImported proj, bool isChecked)
        {
           // await ExpandProj(proj, true, false);
            for (int i = SavedProjectsListAdded.IndexOf(proj); i < SavedProjectsListAdded.Count; i++)
            {
                var pro = SavedProjectsListAdded[i];
                if (pro == proj) continue;
                if (pro.TabMargin.Left == proj.TabMargin.Left) break;

                pro.RaiseChecked = false;
                pro.IsChecked = isChecked;
                pro.RaiseChecked = true;
            }
        }

        private void SelectFolderSelect_ClickIsChecked(object obj)
        {
            string param = obj as string;
            if (param == null) return;

            foreach (var p in SavedProjectsListAdded)
            {
                p.RaiseChecked = false;
                p.IsChecked = param == "All" ? true : false;
                p.RaiseChecked = true;
            }
        }

        public bool ShowListWindowDialog()
        {
            ChooseFolderWindow cfw = new ChooseFolderWindow();
            cfw.DataContext = this;
            cfw.lstItems.ItemsSource = SavedProjectsListAdded;
            cfw.Title = "Select Project";
            cfw.ShowDialog();
            if (!cfw.OkClicked) return false;

            return true;
        }
    }
}
