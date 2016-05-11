using Browser.Common.ViewModels;
using DragDropListview;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Browser.Common.Views
{
    /// <summary>
    /// Interaction logic for BrowserHostCntrl.xaml
    /// </summary>
    public partial class BrowserHostCntrl : UserControl
    {
        public event Action OnOpenNewTab = delegate { };
        public event Action<ExecutedRoutedEventArgs> OnCloseTab = delegate { };
        public event Action OnContentRenderd = delegate { };

        public BrowserHostCntrl()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OpenNewTab));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab));

            TabControl.Loaded += TabControl_Loaded;
        }

        private void TabControl_Loaded(object sender, RoutedEventArgs e)
        {
            TabControl.Loaded -= TabControl_Loaded;

            PresentationSource presentationSource = PresentationSource.FromVisual((Visual)sender);
            if (presentationSource == null)
            {
                OnContentRenderd();
                return;
            }
            presentationSource.ContentRendered += TabControl_ContentRendered;
        }

        void TabControl_ContentRendered(object sender, EventArgs e)
        {
            // Don't forget to unsubscribe from the event
            ((PresentationSource)sender).ContentRendered -= TabControl_ContentRendered;
            OnContentRenderd();
            // ..
        }

        private void CloseTab(object sender, ExecutedRoutedEventArgs e)
        {
            OnCloseTab(e);
        }

        private void OpenNewTab(object sender, ExecutedRoutedEventArgs e)
        {
            OnOpenNewTab();
            TabControl.SelectedIndex = TabControl.Items.Count - 1;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tabControl = sender as TabControl;

                tabControl.Dispatcher.BeginInvoke(
                    new Action(() => UpdateZIndex(sender as TabControl)),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        private void UpdateZIndex(TabControl tabControl)
        {
            ItemContainerGenerator icg = tabControl.ItemContainerGenerator;

            if (icg.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                foreach (object o in tabControl.Items)
                {
                    UIElement tabItem = icg.ContainerFromItem(o) as UIElement;
                    if (tabItem != null)
                    {
                        // Set ZIndex
                        Panel.SetZIndex(tabItem, (o == tabControl.SelectedItem ? 100 :
                            90 - tabControl.Items.IndexOf(o)));
                    }
                }
            }

            //Action emptyAction = delegate { };
            //TabControl.Dispatcher.Invoke(DispatcherPriority.Render, emptyAction);
        }

        private void Sviewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            //this.InvalidateVisual();
            //this.UpdateLayout();
            //this.Dispatcher.Invoke(emptyDelegate, DispatcherPriority.Render);
        }


    }
}
