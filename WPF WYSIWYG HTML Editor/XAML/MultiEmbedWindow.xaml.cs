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
using Organiser.Common.Classes;

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for MultiEmbedWindow.xaml
    /// </summary>
    public partial class MultiEmbedWindow : Window, IDisposable
    {
        public HTMLDocument doc;

        public MultiEmbedWindow(HTMLDocument doc)
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
                if (tbInputedText.Text.IsNullOrEmpty()) return;

                string embendsinsyntax = string.Empty;
                string[] links = tbInputedText.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var link in links)
                {
                    if (link.IsNullOrEmpty()) continue;

                    embendsinsyntax += string.Format(@"[embed]{0}[/embed] | ", link);
                }
                embendsinsyntax = embendsinsyntax.Remove(embendsinsyntax.LastIndexOf("|"));
                embendsinsyntax = embendsinsyntax.Trim();
                embendsinsyntax = "{" + embendsinsyntax + "}";

                dynamic r = doc.selection.createRange();
                r.pasteHTML(embendsinsyntax);
                this.Hide();
            }
        }
    }
}
