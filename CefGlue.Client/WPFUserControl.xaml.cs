using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xilium.CefGlue.Client
{
    /// <summary>
    /// Interaction logic for WPFUserControl.xaml
    /// </summary>
    public partial class WPFUserControl : UserControl
    {
        public WPFUserControl()
        {
            InitializeComponent();

            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            TabbedBrowser cntrl = new TabbedBrowser();
            host.Child = cntrl;
            brwserGrd.Children.Add(host);
        }
    }
}
