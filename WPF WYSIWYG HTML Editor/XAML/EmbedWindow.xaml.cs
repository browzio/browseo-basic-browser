using mshtml;
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
using System.Windows.Shapes;

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for EmbedWindow.xaml
    /// </summary>
    public partial class EmbedWindow : Window, IDisposable
    {
        public HTMLDocument doc;

        public EmbedWindow(HTMLDocument doc)
        {
            InitializeComponent();
            this.doc = doc;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Dispose();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (doc != null)
            {
                dynamic r = doc.selection.createRange();
                r.pasteHTML(string.Format(@"[embed]{0}[/embed]", link.Text));
                this.Hide();
            }
        }
    }
}
