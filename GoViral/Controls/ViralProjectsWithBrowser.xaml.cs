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
        ViewModels.GoViralVM ViewModel;


        public event Action<string, string, string> OnClickedSendSocialLink;

        public ProjectsWithBrowser()
        {
            InitializeComponent();

            ViewModel = new ViewModels.GoViralVM();
            //ViewModel.WebBrowserControler = BFXBrowser.BaseBrowser;
            DataContext = ViewModel;
        }

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                try
                {
                    NavigateToUrl(((sender as Grid).DataContext as GoViral.Models.ListOption).Url);
                }
                catch
                {

                }
            }
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
                Models.ListOption loToFind = ((sender as TextBox).DataContext as Models.ListOption);
                if (loToFind == null) return;
                Models.Folder nextFolder = ViewModel.Folders.SingleOrDefault(f => f.SavedLinksList.SingleOrDefault(lo => lo == loToFind) != null);
                if(nextFolder != null)
                {
                    ViewModel.SIFolders = ViewModel.Folders.IndexOf(nextFolder);
                    nextFolder.SISavedLinks = nextFolder.SavedLinksList.IndexOf(loToFind);    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private void lbFolders_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild((sender as ListView), 0) as Decorator;
            if (border == null) return;
            ScrollViewer sv = border.Child as ScrollViewer;
            if (sv == null) return;
            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
        }

        private void NavigateToUrl(string url)
        {
            //if (tbCntrl.SelectedIndex != 2) tbCntrl.SelectedIndex = 2;
            if (url.Contains("/?ref=br_rs")) url = url.Replace("/?ref=br_rs", "");

            if (url.Contains("facebook.com") && url.Contains("-"))
            {
                string urltillId = url.Remove(url.LastIndexOf("/"));
                string name = url.Substring(url.LastIndexOf("/") + 1);
                if (name.Contains("-"))
                {
                    string id = name.Substring(name.LastIndexOf("-") + 1);
                    //long tryparseResult = 0;
                    //if (Int64.TryParse(id, out tryparseResult))
                    //{
                        url = "https://www.facebook.com/" + id;
                    //}
                    //else
                    //{
                    //    url = "https://www.facebook.com/" + name;
                    //}
                }
            }
            ViewModel.RaiseOnSelectedTabNavigate(url);
            //ViewModel.WebBrowser.Navigate(url);
        }


        #region ucSelectedPageInfo
        private void selectedPageInfoUC_AddUrlToPostsSync(string pageName, string url)
        {
            ucSyncedPosts.ViewModel.AddUrlToSavedProjectList(pageName, url, null);
        }

        private void ucSelectedPageInfo_AddUrlForMulti(string text)
        {
            if (multiWindowForLinksAdd == null)
            {
                multiWindowForLinksAdd = new Organiser.Common.Windows.RssFeedsLinksMultiWindow();
                multiWindowForLinksAdd.Closed += MultiWindowForLinksAdd_Closed;
                multiWindowForLinksAdd.Title = "Page Name , Url";
                multiWindowForLinksAdd.Show();
            }
            text = text.RemoveAmps();
            multiWindowForLinksAdd.tbInputedText.Text += text;
        }

        private void ucSelectedPageInfo_OnNavigateToUrl(string url)
        {
            NavigateToUrl(url);
        }
        #endregion


        #region ucSyncedPosts
       // List<KeyValuePair<string, string>> ToSendSyncLinks = new List<KeyValuePair<string, string>>();


        Organiser.Common.Windows.RssFeedsLinksMultiWindow multiWindowForLinksAdd;

        private void MultiWindowForLinksAdd_Closed(object sender, EventArgs e)
        {
            if (multiWindowForLinksAdd.ButtonLeftClicked)
            {
                ucSyncedPosts.ViewModel.AddUrlToSavedProjectList("", "", multiWindowForLinksAdd.tbInputedText.Text);
                multiWindowForLinksAdd = null;
            }
        }

        private void UcSyncedPosts_OnBrowserNavigateToUrl(string url)
        {
            NavigateToUrl(url);
        }
        #endregion



        #region ucSearch
        private void ucSearch_OnOpenInBrowserRequested(string url)
        {
            NavigateToUrl(url);
        }

        private void ucSearch_OnStoreForDominationRequested(string link, List<string> multi)
        {
            ViewModel.AsyncAddLinkToList(link,"", multi, showLinksWindow: false);
        }

        private void ucSearch_OnOpenInBrowserForDownloadRequested(string source)
        {
            //BFXBrowser.BaseBrowser.StartDownload(source);
            //ViewModel.WebBrowser.GetBrowser().GetHost().StartDownload(source);
            // (this.DataContext as ViewModels.GoViralVM).WebBrowser.Navigate(source);
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ucSyncedPosts.ViewModel == null)
            {
                ucSyncedPosts.ViewModel = new ViewModels.SyncedProjectsVM(ViewModels.SyncedProjectsVM.TypeOfGoViral);
                ucSyncedPosts.DataContext = ucSyncedPosts.ViewModel;
            }
        }


        private Action EmptyDelegate = delegate () { };
        void updateLayouts()
        {
            UpdateLayout();
            //try
            //{
            //    BaseBrowser.MainWebView.Widget.BaseWindow.Instance.Repaint(true);
            //}
            //catch { }

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            updateLayouts();
        }

        private void Expander_Expanded_1(object sender, RoutedEventArgs e)
        {
            updateLayouts();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            updateLayouts();
        }

        private void Expander_Collapsed_1(object sender, RoutedEventArgs e)
        {
            updateLayouts();
        }

        private void ucSelectedPageInfo_OnClickedSendSocialLink(string param, string url, string imgLink)
        {
            OnClickedSendSocialLink?.Invoke(param, url, imgLink);
        }

        private void Expander_Drop(object sender, DragEventArgs e)
        {
            //string link = "https://www.facebook.com/" + type + "/" + name + "-" + id;

            if (e.Data.GetData("HTML Format") != null)
            {
                var currentFolder = (sender as Expander).DataContext as Models.Folder;
                var htmlFormat = e.Data.GetData("HTML Format").ToString();
                if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/groups/"))
                {
                    var urlSubstring = "<a href=\"";
                    var titleSubstring = "\">";

                    var url = e.Data.GetData("HTML Format").ToString();
                    url = url.Substring(url.IndexOf(urlSubstring) + urlSubstring.Length);
                    url = url.Remove(url.IndexOf("\""));
                    url = url.Replace("?ref=br_rs", "");
                    if (url.EndsWith("/")) url = url.Remove(url.Length - 1);

                    var id = url.Substring(url.LastIndexOf("/") + 1);

                    url = url.Remove(url.LastIndexOf("/") + 1);

                    var title = e.Data.GetData("HTML Format").ToString();
                    title = title.Substring(title.LastIndexOf(titleSubstring) + titleSubstring.Length);
                    title = title.Remove(title.IndexOf("</a>"));
                    title = title.Replace("&amp", "");


                    //if (currentFolder.SelectedFolder == null) currentFolder.SavedLinksList = new System.Collections.ObjectModel.ObservableCollection<Models.ListOption>();
                    currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = title, Url = url + title + "-" + id });
                }
                else if(e.Data.GetData("Text") != null)
                {
                    var text = e.Data.GetData("Text").ToString();
                    text = text.Replace("?ref=br_rs", "");
                    text = text.Remove(text.LastIndexOf("/"));
                    var id = text.Substring(text.LastIndexOf("/") + 1);
                    if (id.Contains("-")) id = id.Substring(id.LastIndexOf("-"));
                    id = id.Replace("-", "");

                    if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/pages/"))
                    {
                        //pages
                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = id, Url = "https://www.facebook.com/pages/" + id + "-" + id });
                    }
                    else if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/str") && !htmlFormat.Contains("photos-keyword"))
                    {

                        //places
                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = id, Url = "https://www.facebook.com/places/" + id + "-" + id });
                    }
                    else if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/videos"))
                    {
                        //videos
                        text = text.Replace("https://www.facebook.com/", "");
                        text = text.Remove(text.IndexOf("/"));

                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = text, Url = "https://www.facebook.com/videos/" + text + "-" + id });
                    }
                    else if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/str") && htmlFormat.Contains("photos-keyword"))
                    {
                        //photos
                        text = text.Replace("https://www.facebook.com/", "");
                        text = text.Remove(text.IndexOf("/"));

                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = text, Url = "https://www.facebook.com/photos/" + text + "-" + id });
                    }
                    else if (htmlFormat.Contains("SourceURL:https://www.facebook.com/search/events/"))
                    {
                        //events
                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = id, Url = "https://www.facebook.com/events/" + id + "-" + id });
                    }
                    else
                    {
                        //pages
                        text = e.Data.GetData("Text").ToString();
                        text = text.Remove(text.LastIndexOf("/"));
                        text = text.Substring(text.LastIndexOf("/") + 1);
                        text = text.Replace("-", " ");
                        text = text.Replace("-"+id, "");
                        currentFolder.SavedLinksList.Add(new Models.ListOption() { Name = text, Url = "https://www.facebook.com/pages/" + text + "-" + id });
                    }
                }
            }
           // 
        }
    }
}




//private void lbsavedList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
//{
//    //ListView lv = FindChild<ListView>(sender as ListView, "lvPosts");
//    //Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
//    //ScrollViewer scrollViewer = border.Child as ScrollViewer;

//    //int MouseX = (int)Mouse.GetPosition(lv).X;
//    //int MouseY = (int)Mouse.GetPosition(lv).Y;

//    //if (MouseY > 0 && MouseY < lv.ActualHeight && (
//    //    (scrollViewer.ContentVerticalOffset != 0 && scrollViewer.ContentVerticalOffset != scrollViewer.ScrollableHeight) ||
//    //    (scrollViewer.ContentVerticalOffset == 0 && e.Delta<0) || (scrollViewer.ContentVerticalOffset == scrollViewer.ScrollableHeight && e.Delta > 0)
//    //        ))
//    //{
//    //if (e.Delta > 0)
//    //{
//    //    if (lv.SelectedIndex < lv.Items.Count - 2)
//    //        lv.SelectedIndex += 1;
//    //}
//    //else
//    //{
//    //    if (lv.SelectedIndex > 1)
//    //        lv.SelectedIndex -= 1;
//    //}

//    //scrollViewer.ScrollToVerticalOffset(e.Delta > 0 ? scrollViewer.VerticalOffset - 1 : scrollViewer.VerticalOffset + 1);
//    //e.Handled = true;
//    //if (e.Delta > 0)
//    //{
//    //    //scrollViewer.ScrollToVerticalOffset(e.Delta);
//    //    //ScrollBar.LineDownCommand.Execute(null, scrollViewer as IInputElement);
//    //}
//    //if (e.Delta < 0)
//    //{
//    //    ScrollBar.LineUpCommand.Execute(null, scrollViewer as IInputElement);
//    //}
//    // }
//    //else
//    //{                                                       
//    //if (mouseWithinChildScroll == null   ||
//    //    (mouseWithinChildScroll.ContentVerticalOffset == 0 && e.Delta > 0) ||
//    //    (mouseWithinChildScroll.ContentVerticalOffset == mouseWithinChildScroll.ScrollableHeight && e.Delta > 0))
//    //{
//    if (mouseWithinChildScroll == null)
//    {
//        MyScrollViewer.ScrollToVerticalOffset(MyScrollViewer.VerticalOffset - e.Delta);
//    }
//    // e.Handled = true;
//    // }
//    //else
//    //    e.Handled = true;
//    //}
//}
//private void btnLoadMoreLikes_Click(object sender, RoutedEventArgs e)
//{
//    try
//    {
//        //ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
//        //ListBox foundlb = FindChild<ListBox>(this, "lbsavedList");
//        // vm.LoadAllLikes(((sender as Button).DataContext as FacebookGraphPostResult), (sender as Button).Tag.ToString());
//    }
//    catch
//    {
//        MessageBox.Show("No more likes found");
//    }
//}
//private void setCorrectSI(object sender)
//{
//    //Task.Factory.StartNew(() =>
//    //{
//    //    lock (sender)
//    //    {
//    try
//    {
//        ViewModels.GoViralVM vm = this.DataContext as ViewModels.GoViralVM;
//        Models.ListOption loToFind = null;
//        if (sender is Expander)
//        {
//            loToFind = ((sender as Expander).DataContext as Models.ListOption);
//        }
//        else if (sender is TextBox)
//        {
//            loToFind = ((sender as TextBox).DataContext as Models.ListOption);
//        }
//        else if (sender is StackPanel)
//        {
//            loToFind = ((sender as StackPanel).DataContext as Models.ListOption);
//        }

//        if (loToFind == null) return;
//        Task.Factory.StartNew(() =>
//        {
//            lock (loToFind)
//            {
//                foreach (Models.Folder f in vm.Folders)
//                {
//                    Models.ListOption lo = f.SavedLinksList.SingleOrDefault(s => s == loToFind);
//                    if (lo != null)
//                    {
//                        vm.SIFolders = vm.Folders.IndexOf(f);
//                        f.SISavedLinks = f.SavedLinksList.IndexOf(lo);
//                        break;
//                    }
//                }
//            }
//        });
//    }
//    catch (Exception ex)
//    {
//    }
//    //    }
//    //});
//}
