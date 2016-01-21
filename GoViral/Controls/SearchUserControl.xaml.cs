using GoViral.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace GoViral.Controls
{
    /// <summary>
    /// Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public SearchVM ViewModel { get; set; }

        public SearchUserControl()
        {
            InitializeComponent();

            if(ViewModel == null)
            {
                ViewModel = new SearchVM();
                this.DataContext = ViewModel;
            }
        }
        
        private void miCollpse_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
                return;

            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null)
                return;

            var listView = contextMenu.PlacementTarget as ListView;
            if (listView == null)
                return;

            var expander = listView.Parent as Expander;
            if (expander == null)
                return;

            expander.IsExpanded = false;
        }



        ListView lvInnerCurrent = null;

        private void LVOuterData_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ListView lv = sender as ListView;
            ScrollViewer sv = getScrollViewerFromLV(lv);

            if (sv.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden || sv.ScrollableHeight == 0)
            {
                e.Handled = true;

                if (lvInnerCurrent != null)
                {
                    if (e.Delta < 0)
                    {
                        if (lvInnerCurrent.SelectedIndex < lvInnerCurrent.Items.Count - 1)
                            lvInnerCurrent.SelectedIndex += 1;
                    }
                    else
                    {
                        if (lvInnerCurrent.SelectedIndex > 0)
                            lvInnerCurrent.SelectedIndex -= 1;
                    }

                    lvInnerCurrent.ScrollIntoView(lvInnerCurrent.SelectedItem);
                    //ScrollViewer svInner = getScrollViewerFromLV(lvInnerCurrent);
                    //svInner.ScrollToVerticalOffset(svInner.VerticalOffset - e.Delta);
                }
                else
                {
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                    eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                    eventArg.Source = sender;
                    var parent = ((Control)sender).Parent as UIElement;
                    parent.RaiseEvent(eventArg);
                }
            }
        }

        private void LVOuterData_ScrollChanged(object sender, RoutedEventArgs e)
        {
            if (lvInnerCurrent != null)
            {
                e.Handled = true;
                ListView lv = sender as ListView;
                lv.ScrollIntoView(lv.SelectedItem);
            }
        }

        private void LVOuterData_MouseLeave(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = getScrollViewerFromLV(sender as ListView);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            lvInnerCurrent = null;
        }


        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);

            Models.SearchResult selectedSR = ex.DataContext as Models.SearchResult;
            lvParent.SelectedIndex = ViewModel.SearchResultsWithKwList.SearchResultsList.IndexOf(selectedSR);
            scrollExpanderIntoView(lvParent, selectedSR);

            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            lvInnerCurrent = ex.Content as ListView;
            if (lvInnerCurrent.SelectedIndex == -1) lvInnerCurrent.SelectedIndex = 0;

           // lvPreview.ItemTemplate.Template = lvInnerCurrent.ItemTemplate.Template;
           // lvPreview.Items.Refresh();

            //string lvXaml = XamlWriter.Save(lvInnerCurrent);
            //StringReader stringReader = new StringReader(lvXaml);
            //XmlReader xmlReader = XmlReader.Create(stringReader);
            //lvPreview = (ListView)XamlReader.Load(xmlReader);
            //cpSelectedItemPreview.Content = (ListView)XamlReader.Load(xmlReader);
            //cpSelectedItemPreview. = lvInnerCurrent.Template;
            //ccPreviewSelectedItem.DataContext = lvInnerCurrent.DataContext;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);
            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void Expander_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Expander ex = sender as Expander;
            ListView lvParent = getLVparentByExpanderName(ex.Name);
            ScrollViewer sv = getScrollViewerFromLV(lvParent);
            ListView lvInner = (sender as Expander).Content as ListView;
            if (lvInnerCurrent == null && !(sender as Expander).IsExpanded) sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            else lvInnerCurrent = lvInner;

            if (lvInner != null && lvPreview.ItemsSource != lvInner.ItemsSource)
            {
                lvPreview.ItemsSource = lvInner.ItemsSource;
                lvPreview.ItemTemplate = lvInner.ItemTemplate;
            }
        }

        private ListView getLVparentByExpanderName(string name)
        {
            switch (name)
            {
                case "exPages":
                    return lvDataPages;

                case "exGroups":
                    return lvDataGroups;

                case "exEvents":
                    return lvDataEvents;

                case "exPlaces":
                    return lvDataPlaces;

                case "exPeople":
                    return lvDataPeople;

                default:
                    break;
            }

            return null;
        }

        private ScrollViewer getScrollViewerFromLV(object sender)
        {
            ListView lv = sender as ListView;
            if (lv == null) return null;
            Decorator border = VisualTreeHelper.GetChild(lv, 0) as Decorator;
            if (border == null) return null;
            ScrollViewer sv = border.Child as ScrollViewer;
            if (sv == null) return null;
            return sv;
        }

        private void scrollExpanderIntoView(ListView sender, Models.SearchResult selectedSR)
        {
            ScrollViewer sv = getScrollViewerFromLV(sender);

            sv.ScrollToTop();
            sender.ScrollIntoView(selectedSR);
        }



        private void lvPreview_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (lvPreview.SelectedIndex == -1) lvPreview.SelectedIndex = 0;

            if (e.Delta < 0)
            {
                if (lvPreview.SelectedIndex < lvPreview.Items.Count - 1)
                    lvPreview.SelectedIndex += 1;
            }
            else
            {
                if (lvPreview.SelectedIndex > 0)
                    lvPreview.SelectedIndex -= 1;
            }

            lvPreview.ScrollIntoView(lvPreview.SelectedItem);
        }
    }
}