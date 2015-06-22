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

namespace NichResearch
{
    /// <summary>
    /// Interaction logic for NichResearchControl.xaml
    /// </summary>
    public partial class NichResearchControl : UserControl
    {
        public NichResearchControl()
        {
            InitializeComponent();
            DataContext = new ResearchVM();
        }
    }
}
