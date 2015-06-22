using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfDragDropTreeView.Windows;

namespace WpfDragDropTreeView.Controls
{
    /// <summary>
    /// Interaction logic for DraggableTreeview.xaml
    /// </summary>
    public partial class DraggableTreeview : UserControl
    {
        public event Action<string> OnLaunchSite = delegate { };
        public string ProjName { get; set; }

        private const string SPLITTER = "{[:]}";

        Point _lastMouseDown;
        TreeViewItem draggedItem, _target;

        //each profile proxy dont change

        string newFilePath;

        public static string GetBaseDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RAWSocialOrganizer";
        }

        public DraggableTreeview()
        {
            InitializeComponent();
            this.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        }


        public void initTreeview()
        {
            string filePath = System.IO.Path.Combine(GetBaseDir(), "Sites\\" + ProjName + ".txt");
            newFilePath = System.IO.Path.Combine(GetBaseDir(), "Sites\\"+ProjName+"\\" + ProjName + ".txt");

            string newDir = System.IO.Path.Combine(GetBaseDir(), "Sites\\" + ProjName);
            if (!Directory.Exists(newDir))
                Directory.CreateDirectory(newDir);

            if (!File.Exists(newFilePath))
            {
                if (File.Exists(filePath))
                {
                    foreach (var site in File.ReadAllLines(filePath))
                    {
                        File.AppendAllText(newFilePath, site + SPLITTER + site + Environment.NewLine);
                    }
                }
            }

            if (File.Exists(newFilePath))
            {
                foreach (var site in File.ReadAllLines(newFilePath))
                {
                    string[] siteNtag = site.Split(new string[] { SPLITTER }, StringSplitOptions.None);

                    trvBookmarks.Items.Add(new TreeViewItem
                    {
                        AllowDrop = true,
                        Tag = siteNtag[0],
                        Header = siteNtag.Length > 1 ? siteNtag[1] : "",
                        Name = "Site"
                    });
                }
            }

            foreach (string dir in Directory.GetDirectories(System.IO.Path.Combine(GetBaseDir(), "Sites\\" + ProjName)))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);

                TreeViewItem ParentItem = new TreeViewItem
                {
                    AllowDrop = true,
                    Tag = dirInfo.FullName,
                    Header = dirInfo.Name,
                    Name = "Dir",
                    FontWeight = FontWeights.Bold
                };
                trvBookmarks.Items.Add(ParentItem);

                string siteList = System.IO.Path.Combine(dirInfo.FullName, ProjName+".txt");
                if (File.Exists(siteList))
                {
                    foreach (var site in File.ReadAllLines(siteList))
                    {
                        string[] siteNtag = site.Split(new string[] { SPLITTER }, StringSplitOptions.None);
                        ParentItem.Items.Add(new TreeViewItem
                        {
                            AllowDrop = true,
                            Tag = siteNtag[0],
                            Header = siteNtag.Length > 1 ? siteNtag[1] : "",
                            Name = "Site",
                            FontWeight = FontWeights.Regular
                        });
                    }

                    ParentItem.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                }
            }

            trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  

        }

        StackPanel getSiteHeader(string headerText)
        {
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.AllowDrop = true;

            // create Image
            Image image = new Image();
            image.Source = new BitmapImage
                (new Uri("pack://application:,,/Images/new_document.png"));
            image.Width = 16;
            image.Height = 16;
            image.AllowDrop = true;
            // Label
            Label lbl = new Label();
            lbl.Content = headerText;
            lbl.AllowDrop = true;

            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            return stack;
        }

        StackPanel getDirHeader(string headerText)
        {
            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.AllowDrop = true;

            // create Image
            Image image = new Image();
            image.Source = new BitmapImage
                (new Uri("pack://application:,,/Images/closed_folder.png"));
            image.Width = 16;
            image.Height = 16;
            image.AllowDrop = true;
            // Label
            Label lbl = new Label();
            lbl.Content = headerText;
            lbl.AllowDrop = true;

            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            return stack;
        }

        #region dragdrop

        private void TreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(trvBookmarks);
            }

        }
        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(trvBookmarks);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (TreeViewItem)trvBookmarks.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(trvBookmarks, trvBookmarks.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target == null))
                            {
                                TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(draggedItem);
                                try
                                {
                                    try
                                    {
                                        ParentItem.Items.Remove(draggedItem);
                                        ParentItem.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                                    }
                                    catch
                                    {
                                        trvBookmarks.Items.Remove(draggedItem);
                                        trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                                    }
                                }
                                catch { }

                                trvBookmarks.Items.Add(draggedItem);
                                trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                            }
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null) && draggedItem.Name == "Site")
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Header.ToString().Equals(_target.Header.ToString()))
                                {
                                    if (_target.Name == "Dir")
                                    {
                                        CopyItem(draggedItem, _target);
                                    }
                                    else
                                    {
                                        TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(draggedItem);
                                        try
                                        {
                                            try
                                            {
                                                ParentItem.Items.Remove(draggedItem);
                                                ParentItem.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                                            }
                                            catch
                                            {
                                                trvBookmarks.Items.Remove(draggedItem);
                                                trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                                            }
                                        }
                                        catch { }

                                        trvBookmarks.Items.Add(draggedItem);
                                        trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
                                    }
                                    _target = null;
                                    draggedItem = null;

                                    save();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(trvBookmarks);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }
        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception)
            {
            }



        }
        private bool CheckDropTarget(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.Header.ToString().Equals(_targetItem.Header.ToString()))
            {
                _isEqual = true;
            }
            return _isEqual;

        }
        private void CopyItem(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            try
            {
                //adding dragged TreeViewItem in target TreeViewItem
                addChild(_sourceItem, _targetItem);

                //finding Parent TreeViewItem of dragged TreeViewItem 
                TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
                // if parent is null then remove from TreeView else remove from Parent TreeViewItem
                if (ParentItem == null)
                {
                    trvBookmarks.Items.Remove(_sourceItem);
                }
                else
                {
                    ParentItem.Items.Remove(_sourceItem);
                }
            }
            catch
            {

            }
        }

        public void addChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            // add item in target TreeViewItem 
            TreeViewItem item1 = new TreeViewItem();
            item1.Header = _sourceItem.Header;
            item1.Tag = _sourceItem.Tag;
            item1.Name = _sourceItem.Name;
            item1.FontWeight = FontWeight = FontWeights.Regular;
            _targetItem.Items.Add(item1);
            _targetItem.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));  
            foreach (TreeViewItem item in _sourceItem.Items)
            {
                addChild(item, item1);
            }
        }
        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        #endregion

        TreeViewItem selectedTreeViewItem;

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedTreeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (selectedTreeViewItem != null)
            {
                selectedTreeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTreeViewItem == null) return;

            if (MessageBox.Show("Are you sure you would like to delete " + selectedTreeViewItem.Tag + "?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (selectedTreeViewItem.Name == "Dir")
                {
                    Directory.Delete(selectedTreeViewItem.Tag.ToString(), true);
                }
                TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(selectedTreeViewItem);
                try
                {
                    try
                    {
                        ParentItem.Items.Remove(selectedTreeViewItem);
                    }
                    catch
                    {
                        trvBookmarks.Items.Remove(selectedTreeViewItem);
                    }
                }
                catch { }
            }

            save();
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTreeViewItem == null) return;

            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.SetValues(selectedTreeViewItem.Header.ToString(), selectedTreeViewItem.Tag.ToString());
            ebm.ShowDialog();
            if (ebm.SaveClicked)
            {
                selectedTreeViewItem.Header = ebm.tbName.Text;
                selectedTreeViewItem.Tag = ebm.tbURL.Text;
            }
            trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending)); 

            save();
        }

        private void addFolder_Click(object sender, RoutedEventArgs e)
        {
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.spUrl.Visibility = Visibility.Collapsed;
            ebm.Height = 140;
            ebm.ShowDialog();
            if (ebm.SaveClicked)
            {
                trvBookmarks.Items.Add(new TreeViewItem
                {
                    Header = ebm.tbName.Text,
                    Tag = System.IO.Path.Combine(GetBaseDir(), "Sites\\" + ProjName, ebm.tbName.Text),
                    FontWeight = FontWeights.Bold,
                    Name = "Dir"
                });
            }
            trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending)); 

            save();
        }

        private void save()
        {
            File.Delete(newFilePath);

            foreach (TreeViewItem parent in trvBookmarks.Items)
            {
                if (parent.Name == "Site")
                {
                    File.AppendAllText(newFilePath, parent.Tag + SPLITTER + parent.Header + Environment.NewLine);
                }

                if (parent.Name == "Dir")
                {
                    if (!Directory.Exists(parent.Tag.ToString()))
                    {
                        Directory.CreateDirectory(parent.Tag.ToString());
                    }

                    if (parent.Items != null)
                    {
                        string dirpath = System.IO.Path.Combine(parent.Tag.ToString(), ProjName+".txt");
                        if (File.Exists(dirpath)) File.Delete(dirpath);

                        foreach (TreeViewItem child in parent.Items)
                        {
                            File.AppendAllText(dirpath, child.Tag + SPLITTER + child.Header + Environment.NewLine);
                        }
                    }
                }
            }
        }

        public static List<string> GetSites(string path)
        {
            List<string> sites = new List<string>();
            if (File.Exists(path))
            {
                foreach (var item in File.ReadAllLines(path))
                {
                    sites.Add(item);
                }
            }

            return sites;
        }

        private void addSite_Click(object sender, RoutedEventArgs e)
        {
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.SetValues("", "");
            ebm.ShowDialog();

            if (ebm.SaveClicked)
            {
                trvBookmarks.Items.Add(new TreeViewItem
                {
                    AllowDrop = true,
                    Tag = ebm.tbURL.Text,
                    Header = ebm.tbName.Text,
                    Name = "Site"
                });
            }
            trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending)); 

            save();
        }

        private void trvBookmarks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedTreeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (selectedTreeViewItem == null || selectedTreeViewItem.Name == "Dir") return;

            OnLaunchSite(selectedTreeViewItem.Tag.ToString());
        }

        public void SaveSite(string site)
        {
            EditBookmarkWindow ebm = new EditBookmarkWindow();
            ebm.SetValues(site, site);
            ebm.ShowDialog();

            if (ebm.SaveClicked)
            {
                trvBookmarks.Items.Add(new TreeViewItem
                {
                    AllowDrop = true,
                    Tag = ebm.tbURL.Text,
                    Header = ebm.tbName.Text,
                    Name = "Site"
                });
            }
            trvBookmarks.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending)); 
            save();
        }
    }
}
