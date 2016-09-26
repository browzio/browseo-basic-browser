using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
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

namespace Organiser.Common.Controlls
{
    /// <summary>
    /// Interaction logic for TrvMacrosUserControl.xaml
    /// </summary>
    public partial class TrvMacrosUserControl : UserControl
    {
        public TrvMacrosUserControl()
        {
            InitializeComponent();
        }

        private void spMacros_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var mac = (sender as StackPanel).DataContext as MacroFile;
                if (mac != null)
                {
                    mac.OnCommandFromView_Raised("MacroRun");
                }
            }
        }

        private void spMacros_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mac = (sender as StackPanel).DataContext as MacroFile;
            if (mac != null)
            {
                mac.IsSelected = true;
            }
        }

        #region dragdrop
        bool cmopened = false;
        Point _lastMouseDown;
        MacroFile draggedItem, _target;
        object oldSource;

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (cmopened) return;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point currentPosition = e.GetPosition(trvMacros);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (MacroFile)trvMacros.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(trvMacros, trvMacros.SelectedValue, DragDropEffects.Move);

                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted
                                if (CheckDropTarget(draggedItem, _target))
                                {
                                    CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
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
                if (cmopened) return;
                Point currentPosition = e.GetPosition(trvMacros);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    if (oldSource != e.OriginalSource && oldSource != null)
                    {
                        SetBGFromSource(oldSource, "#00FFFFFF", 15.96);
                    }

                    // Verify that this is a valid drop and then store the drop target
                    MacroFile item = GetProjectDataVMFromSource(e.OriginalSource);
                    if (item != null && CheckDropTarget(draggedItem, item))
                    {
                        oldSource = e.OriginalSource;
                        SetBGFromSource(oldSource, "#E0E0E0", 25);
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

        private void SetBGFromSource(object originalSource, string hexColor, double height)
        {
            StackPanel sp = null;
            if (originalSource is TextBlock) sp = (originalSource as TextBlock).Parent as StackPanel;
            else if (originalSource is Image) sp = (originalSource as Image).Parent as StackPanel;

            if (sp == null) return;

            sp.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(hexColor));
            sp.Height = height;
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (cmopened) return;
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                MacroFile TargetItem = GetProjectDataVMFromSource(e.OriginalSource);
                if (TargetItem == null || TargetItem == draggedItem || draggedItem == null) return;

                _target = TargetItem;
                e.Effects = DragDropEffects.Move;
            }
            catch (Exception)
            {
            }
        }

        private MacroFile GetProjectDataVMFromSource(object originalSource)
        {
            if (originalSource is TextBlock)
            {
                return (originalSource as TextBlock).DataContext as MacroFile;
            }
            //else if (originalSource is Border)
            //{
            //    return (originalSource as TextBlock).DataContext as ProjectDataVM; ;
            //}
            else if (originalSource is Image)
            {
                return (originalSource as Image).DataContext as MacroFile;
            }

            return null;
        }
        private bool CheckDropTarget(MacroFile _sourceItem, MacroFile _targetItem)
        {
            if (!_targetItem.IsFolder && _sourceItem.IsFolder) return false;
            //Check whether the target item is meeting your condition
            bool _isEqual = false;
            if (!_sourceItem.FileName.ToString().Equals(_targetItem.FileName.ToString()) &&
                !_targetItem.FilePath.Contains(_sourceItem.FilePath) &&
                _targetItem.IsFolder)
            {
                _isEqual = true;
            }
            return _isEqual;

        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            cmopened = true;

            draggedItem = null;
            _target = null;
            if (oldSource != null)
            {
                SetBGFromSource(oldSource, "#00FFFFFF", 15.96);
            }
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            cmopened = false;
        }

        private void CopyItem(MacroFile _sourceItem, MacroFile _targetItem)
        {
            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop " + _sourceItem.FileName.ToString() + " into " + _targetItem.FileName.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    //adding dragged TreeViewItem in target TreeViewItem
                    string folderName = _sourceItem.FileName;

                    //finding Parent TreeViewItem of dragged TreeViewItem 
                    string toFolderDir = _targetItem.FilePath + "\\" + folderName;

                    //move directory
                    if (Delimon.Win32.IO.File.Exists(_sourceItem.FilePath))
                    {
                        Delimon.Win32.IO.File.Move(_sourceItem.FilePath, toFolderDir);
                        _targetItem.Reset();
                        if (_sourceItem.ParentMacro != null)
                        {
                            _sourceItem.ParentMacro.NextMacros.Remove(_sourceItem);
                        }
                    }
                }
                catch
                {

                }
            }

            if (oldSource != null)
            {
                SetBGFromSource(oldSource, "#00FFFFFF", 15.96);
            }
        }
        #endregion
    }
}
