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

namespace Browmium.WPF.Views
{
    /// <summary>
    /// Interaction logic for BrowserTabView.xaml
    /// </summary>
    public partial class BrowserTabView : UserControl
    {
        public event Action<string, string, List<string>> OnAddedToGoViral = delegate { };//link,type,multi

        public BrowserTabView()
        {
            InitializeComponent();
        }

        public void CloseAllTabs()
        {
        }

        public void SetBookmarksEvents(bool v)
        {
        }

        public void CreateNewTab(string url)
        {

        }

        public void SetRemindersCount()
        {

        }

        public void SearchFor(string query)
        {

        }

        public void LaunchNewWindowToLink(string link, string rssLink)
        {

        }
    }
}
