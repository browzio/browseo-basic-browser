using Browser.Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Browser.Common.Windows
{
    /// <summary>
    /// Interaction logic for RemindersWindow.xaml
    /// </summary>
    public partial class RemindersWindow : Window
    {
        public RemindersWindow()
        {
            InitializeComponent();
        }

        private readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
        public bool IsHyperlink(string word)
        {
            // First check to make sure the word has at least one of the characters we need to make a hyperlink
            if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
            {
                if (Uri.IsWellFormedUriString(word, UriKind.Absolute))
                {
                    // The string is an Absolute URI
                    return true;
                }
                else if (UrlRegex.IsMatch(word))
                {
                    Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        // rebuild it it with http to turn it into an Absolute URI
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri.IsAbsoluteUri)
                    {
                        return true;
                    }
                }
                else
                {
                    Uri wordUri = new Uri(word);

                    // Check to see if URL is a network path
                    if (wordUri.IsUnc || wordUri.IsFile)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void DetectURLs(Paragraph par)
        {
            string paragraphText = new TextRange(par.ContentStart, par.ContentEnd).Text;
            if (paragraphText == "") return;

            // Split the paragraph by words
            foreach (string word in paragraphText.Split(' ').ToList())
            {
                if (IsHyperlink(word))
                {
                    Uri uri = new Uri(word, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri)
                    {
                        // Prepend it with http
                        uri = new Uri(@"http://" + word, UriKind.Absolute);
                    }

                    if (uri != null)
                    {
                        TextPointer position = par.ContentStart;

                        try
                        {
                            // Find the word in the paragraph
                            while (position != null)
                            {
                                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                                {
                                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                                    // Find the starting index of any substring that matches "word".
                                    int indexInRun = textRun.IndexOf(word);
                                    if (indexInRun > 0)
                                    {
                                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                                        TextPointer end = start.GetPositionAtOffset(word.Length);
                                        var link = new Hyperlink(start, end)
                                        {
                                            NavigateUri = uri,

                                        };
                                        link.Click += Hyperlink_Click;
                                    }
                                    else if (indexInRun == -1)
                                    {
                                    }
                                }

                                position = position.GetNextContextPosition(LogicalDirection.Forward);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public void Hyperlink_Click(object sender, EventArgs e)
        {
            (this.DataContext as RemindersVM).Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }

        private void myRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Paragraph paragraph = new Paragraph();
            //paragraph.Inlines.Add(new Run("www.google.com google.com http://www.google.com"));

            RichTextBox tb = sender as RichTextBox;
            if (tb.Document.Blocks.FirstBlock == null) return;

            try
            {
                tb.TextChanged -= myRichTextBox_TextChanged;
                DetectURLs(tb.Document.Blocks.FirstBlock as Paragraph);
                tb.TextChanged += myRichTextBox_TextChanged;
            }
            catch { }

            //tb.Document.Blocks.Clear();
            //tb.Document.Blocks.Add(paragraph);
        }
    }
}
