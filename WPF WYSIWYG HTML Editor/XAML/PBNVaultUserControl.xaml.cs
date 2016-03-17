using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for PBNVaultUserControl.xaml
    /// </summary>
    public partial class PBNVaultUserControl : UserControl
    {
        public PBNVaultUserControl()
        {
            InitializeComponent();
            
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                //Mouse.SetCursor(Cursors.Hand);
                e.UseDefaultCursors = true;
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                //Mouse.SetCursor(Cursors.Hand);
                e.UseDefaultCursors = true;
            }
            else
            {
                //Mouse.SetCursor(Cursors.No);
                e.UseDefaultCursors = false;
            }
            e.Handled = true;
        }

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lb = (sender as ListBox);
                if (lb.SelectedItem == null || lb.ItemsSource == null || lb.DataContext == null) return;

                // Package the data.
                DataObject data = new DataObject();
                data.SetData("Object", lb.SelectedItem);
                data.SetData("IsMoney", false);
                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
                e.Handled = true;
            }
        }

        private void Expander_Drop(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
               // if (e.Data.GetData("List") != null) return;

                var item = sender as Expander;

                Models.PBNProject p = e.Data.GetData("Object") as Models.PBNProject;
                if (p == null) return;

                Models.PBNProjectsFolder folder = (item.DataContext as Models.PBNProjectsFolder);
                Models.PBNProjectsFolder fsource = e.Data.GetData("DataContext") as Models.PBNProjectsFolder;
                if (fsource == folder) return;

                if (!folder.PBNProjects.Contains(p)) folder.PBNProjects.Add(p);
                if (fsource != null) fsource.PBNProjects.Remove(p);

                item.IsExpanded = true;

                (this.DataContext as XmlRpcVM).SavedMoneyProjects.Remove(p);
                (this.DataContext as XmlRpcVM).SavedPBNProjects.Remove(p);
                
                //var ip = (ItemsPresenter)sv.Content;
                var point = item.TranslatePoint(new Point() - (Vector)e.GetPosition(sv), sv);
                sv.ScrollToVerticalOffset(sv.VerticalOffset);
            }
            e.Handled = true;
        }

        private void ListBox_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBox lb = (sender as ListBox);
                if (lb.SelectedItem == null || lb.ItemsSource == null || lb.DataContext == null) return;

                // Package the data.
                DataObject data = new DataObject();
                data.SetData("Object", lb.SelectedItem);
                data.SetData("List", lb.ItemsSource);
                data.SetData("DataContext", lb.DataContext);

                // Inititate the drag-and-drop operation.
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void ListBox_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                bool listwasnull = false;
                if (e.Data.GetData("List") == null)
                {
                    listwasnull = true;
                    if(!Convert.ToBoolean(e.Data.GetData("IsMoney")))
                         return;
                }

                Models.PBNProject p = e.Data.GetData("Object") as Models.PBNProject;
                if (p == null) return;

                ((sender as ListBox).DataContext as XmlRpcVM).SavedPBNProjects.Add(p);
                if (listwasnull)
                {
                    ((sender as ListBox).DataContext as XmlRpcVM).SavedMoneyProjects.Remove(p);
                }
                else
                {
                    ObservableCollection<Models.PBNProject> f = e.Data.GetData("List") as ObservableCollection<Models.PBNProject>;
                    if (f != null)
                    {
                        f.Remove(p);
                    }
                }
            }
            e.Handled = true;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            (sender as Expander).IsExpanded = false;
            e.Handled = true;
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }
    }
}
