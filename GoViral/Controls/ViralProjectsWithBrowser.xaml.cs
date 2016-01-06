using Organiser.Common.Classes;
using Organiser.Common.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for ProjectsWithBrowser.xaml
    /// </summary>
    public partial class ProjectsWithBrowser : UserControl
    {
        public ProjectsWithBrowser()
        {
            InitializeComponent();
        }

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                try
                {
                    (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(((sender as Grid).DataContext as GoViral.Models.ListOption).Url);
                }
                catch
                {

                }
            }
        }

        private void btnLoadMoreLikes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
                //ListBox foundlb = FindChild<ListBox>(this, "lbsavedList");
               // vm.LoadAllLikes(((sender as Button).DataContext as FacebookGraphPostResult), (sender as Button).Tag.ToString());
            }
            catch
            {
                MessageBox.Show("No more likes found");
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            setCorrectSI(sender);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            setCorrectSI(sender);
        }

        private void lbsavedList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //setCorrectSI(sender);
        }

        private void spsavedList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            setCorrectSI(sender);
        }

        private void setCorrectSI(object sender)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    lock (sender)
            //    {
            try
            {
                ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
                Models.ListOption loToFind = null;
                if (sender is Expander)
                {
                    loToFind = ((sender as Expander).DataContext as Models.ListOption);
                }
                else if (sender is TextBox)
                {
                    loToFind = ((sender as TextBox).DataContext as Models.ListOption);
                }
                else if (sender is StackPanel)
                {
                    loToFind = ((sender as StackPanel).DataContext as Models.ListOption);
                }

                if (loToFind == null) return;
                Task.Factory.StartNew(() =>
                {
                    lock (loToFind)
                    {
                        foreach (Models.Folder f in vm.Folders)
                        {
                            Models.ListOption lo = f.SavedLinksList.SingleOrDefault(s => s == loToFind);
                            if (lo != null)
                            {
                                vm.SIFolders = vm.Folders.IndexOf(f);
                                f.SISavedLinks = f.SavedLinksList.IndexOf(lo);
                                break;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
            }
            //    }
            //});
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        private ScrollViewer mouseWithinChildScroll;
        private void lbsavedList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //ListView lv = FindChild<ListView>(sender as ListView, "lvPosts");
            //Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
            //ScrollViewer scrollViewer = border.Child as ScrollViewer;

            //int MouseX = (int)Mouse.GetPosition(lv).X;
            //int MouseY = (int)Mouse.GetPosition(lv).Y;

            //if (MouseY > 0 && MouseY < lv.ActualHeight && (
            //    (scrollViewer.ContentVerticalOffset != 0 && scrollViewer.ContentVerticalOffset != scrollViewer.ScrollableHeight) ||
            //    (scrollViewer.ContentVerticalOffset == 0 && e.Delta<0) || (scrollViewer.ContentVerticalOffset == scrollViewer.ScrollableHeight && e.Delta > 0)
            //        ))
            //{
            //if (e.Delta > 0)
            //{
            //    if (lv.SelectedIndex < lv.Items.Count - 2)
            //        lv.SelectedIndex += 1;
            //}
            //else
            //{
            //    if (lv.SelectedIndex > 1)
            //        lv.SelectedIndex -= 1;
            //}

            //scrollViewer.ScrollToVerticalOffset(e.Delta > 0 ? scrollViewer.VerticalOffset - 1 : scrollViewer.VerticalOffset + 1);
            //e.Handled = true;
            //if (e.Delta > 0)
            //{
            //    //scrollViewer.ScrollToVerticalOffset(e.Delta);
            //    //ScrollBar.LineDownCommand.Execute(null, scrollViewer as IInputElement);
            //}
            //if (e.Delta < 0)
            //{
            //    ScrollBar.LineUpCommand.Execute(null, scrollViewer as IInputElement);
            //}
            // }
            //else
            //{                                                       
            if (mouseWithinChildScroll == null   ||
                (mouseWithinChildScroll.ContentVerticalOffset == 0 && e.Delta > 0) ||
                (mouseWithinChildScroll.ContentVerticalOffset == mouseWithinChildScroll.ScrollableHeight && e.Delta > 0))
            {
                MyScrollViewer.ScrollToVerticalOffset(MyScrollViewer.VerticalOffset - e.Delta);
               // e.Handled = true;
            }
            //else
            //    e.Handled = true;
            //}
        }

        private void lvPosts_MouseEnter(object sender, MouseEventArgs e)
        { 
            Decorator border = VisualTreeHelper.GetChild((sender as ListView), 0) as Decorator;
            mouseWithinChildScroll = border.Child as ScrollViewer; 
        }

        private void lvPosts_MouseLeave(object sender, MouseEventArgs e)
        { 
            mouseWithinChildScroll = null;
        }

        private void lvPosts_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView lv = (sender as ListView);
            Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (e.Delta < 0)
            {
                scrollViewer.ScrollToVerticalOffset(e.Delta > 0 ? scrollViewer.VerticalOffset - 1 : scrollViewer.VerticalOffset + 1);
            }
            else
            {
                scrollViewer.ScrollToVerticalOffset(e.Delta > 0 ? scrollViewer.VerticalOffset - 1 : scrollViewer.VerticalOffset + 1);
            }

            e.Handled = true;
        }

        private void fbPostImages_Click(object sender, RoutedEventArgs e)
        {
            string full_picture = Convert.ToString((sender as MenuItem).Tag);
            if (string.IsNullOrEmpty(full_picture) || string.IsNullOrWhiteSpace(full_picture)) return;
            full_picture = full_picture.Replace("&amp;", "&");
            (this.DataContext as ViewModels.GoViralVM).BeginImageDownload(full_picture);
        }

        private void btnPostLink_Click(object sender, RoutedEventArgs e)
        {
            string url = Convert.ToString((sender as Button).Tag);
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) return;
            (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(url);
        }

        private void tbPostLink_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
            string url = Convert.ToString((sender as TextBlock).Text);
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url)) return;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                linkTextForCopy = url;
                return;
            }
            
            (this.DataContext as GoViral.ViewModels.GoViralVM).WebBrowser.Navigate(url);
        }

        string linkTextForCopy = "";

        private void miCopyLink_Click(object sender, RoutedEventArgs e)
        {
            string url = Convert.ToString((sender as MenuItem).Tag);
            if (string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url))
            {
                url = linkTextForCopy;
            }
            MyFilesDatabase.SetClipboardText(url);
        }
    }
}
