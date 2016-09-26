using Organiser.Common.Classes;
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

namespace Organiser.Common.Windows
{
    class MacroSyntaxProvider
    {
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        #region ctor
        static MacroSyntaxProvider()
        {
            string[] strs = {
            //MacroCommands.Comment,
            MacroCommands.ADD,
            MacroCommands.BACK, 
            MacroCommands.CLEAR, 
            MacroCommands.CLICK, 
            MacroCommands.DS, 
            MacroCommands.EVAL, 
            MacroCommands.EVENT,
            MacroCommands.EVENTS,
            MacroCommands.EXTRACT,
            MacroCommands.FILEDELETE, 
            MacroCommands.FILTER, 
            MacroCommands.FRAME, 
            MacroCommands.IMAGECLICK, 
            MacroCommands.IMAGESEARCH, 
            MacroCommands.ONCERTIFICATEDIALOG,
            MacroCommands.ONDIALOG,
            MacroCommands.ONDOWNLOAD,
            MacroCommands.ONERRORDIALOG, 
            MacroCommands.ONINSECURECONNECTION, 
            MacroCommands.ONLOGIN,
            MacroCommands.ONPRINT,   
            MacroCommands.ONSECURITYDIALOG,
            MacroCommands.ONWEBPAGEDIALOG, 
            MacroCommands.PAUSE, 
            MacroCommands.PRINT, 
            MacroCommands.PROMPT,
            MacroCommands.PROXY, 
            MacroCommands.REFRESH,
            MacroCommands.SAVEAS,
            MacroCommands.SAVEITEM,
            MacroCommands.SCREENSHOT, 
            MacroCommands.SEARCH,
            MacroCommands.SET, 
            MacroCommands.SIZE,
            MacroCommands.STOPWATCH,
            MacroCommands.TAB, 
            MacroCommands.TAG, 
            MacroCommands.TRAY,
            MacroCommands.URL, 
            MacroCommands.VERSION, 
            MacroCommands.WAIT, 
        };
            tags = new List<string>(strs);

            char[] chrs = {
                //'.',
                //')',
                //'(',
                //'[',
                //']',
                //'>',
                //'<',
                //':',
                //';',
                //'\n',
                //'\t'
                '!',
                '"',
                '=',
            };
            specials = new List<char>(chrs);
        }
        #endregion
        public static List<char> GetSpecials
        {
            get { return specials; }
        }
        public static List<string> GetTags
        {
            get { return tags; }
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate (string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate (string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }
    }
    /// <summary>
    /// Interaction logic for MacroEditsWindow.xaml
    /// </summary>
    public partial class MacroEditsWindow : Window
    {
        public bool NeedsSave { get; set; }

        public MacroEditsWindow()
        {
            InitializeComponent();
            this.Closing += MacroEditsWindow_Closing;
            TextInput.Document.PageWidth = 2000;
        }

        private void MacroEditsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (NeedsSave && !IsPreBuils)
            {
                MessageBoxResult res = MessageBox.Show("You have unsaved changes, Would you like to save them before closing?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        break;
                    case MessageBoxResult.No:
                        NeedsSave = false;
                        return;

                    default: break;
                }
            }
        }

        #region textbox
        bool wastext = false;
        //bool wasEnter = false;
        Key keypressed = Key.A;
        private void TextInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            wastext = true;
            NeedsSave = true;
            keypressed = e.Key;
            if (e.Key == Key.Space || e.Key == Key.Enter) wastext = false;
            if (e.Key == Key.V)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                { wastext = false; }
            }
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (TextInput.Document == null)
                return;
            if (e.Changes.Count <= 1 && (wastext || keypressed == Key.Delete || keypressed == Key.Back)) return;
            //if (wastext && keypressed != Key.Space && keypressed != Key.Enter)
            //{
            //    wastext = false;
            //    return;
            //}

            if (keypressed == Key.Space || keypressed == Key.Enter)
            {
                TextPointer caretPos = TextInput.CaretPosition;
                 TextPointer start = caretPos.GetLineStartPosition(0);
                if (keypressed == Key.Enter) start = caretPos.GetLineStartPosition(-1);
                TextPointer end = (caretPos.GetLineStartPosition(1) != null ? caretPos.GetLineStartPosition(1) : caretPos.DocumentEnd);

                TextRange tr = new TextRange(start, end);
                tr.ClearAllProperties();

                TextPointer navigator = start;
                while (navigator.CompareTo(end) < 0)
                {
                    TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                    if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                    {
                        CheckWordsInRun((Run)navigator.Parent);
                    }
                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            else
            {
                TextRange documentRange = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
                documentRange.ClearAllProperties();

                TextPointer navigator = TextInput.Document.ContentStart;
                while (navigator.CompareTo(TextInput.Document.ContentEnd) < 0)
                {
                    TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                    if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                    {
                        CheckWordsInRun((Run)navigator.Parent);
                    }
                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
            }

            Format();
        }
        new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;
        }
        List<Tag> m_tags = new List<Tag>();
        List<Tag> m_tagCommands = new List<Tag>();
        List<Tag> m_tagStrings = new List<Tag>();
        List<Tag> m_tagVariable = new List<Tag>();
        void Format()
        {
            TextInput.TextChanged -= this.TextChangedEventHandler;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Medium);
            }
            m_tags.Clear();

            for (int i = 0; i < m_tagCommands.Count; i++)
            {
                TextRange range = new TextRange(m_tagCommands[i].StartPosition, m_tagCommands[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.DarkRed));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Medium);
            }
            m_tagCommands.Clear();

            for (int i = 0; i < m_tagStrings.Count; i++)
            {
                TextRange range = new TextRange(m_tagStrings[i].StartPosition, m_tagStrings[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.BlueViolet));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Medium);
            }
            m_tagStrings.Clear();

            for (int i = 0; i < m_tagVariable.Count; i++)
            {
                TextRange range = new TextRange(m_tagVariable[i].StartPosition, m_tagVariable[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.BlueViolet));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Medium);
            }
            m_tagVariable.Clear();

            TextInput.TextChanged += this.TextChangedEventHandler;
        }

        void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            int pqIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | MacroSyntaxProvider.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | text[i - 1] == '"' | text[i - 1] == '=' | text[i - 1] == '!'))
                    {
                        eIndex = i - 1;
                        if (text[i] == '"' && pqIndex != 0)
                        {
                            sIndex = pqIndex;
                            pqIndex = 0;
                        }
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);


                        Tag t = new Tag();
                        t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                        t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                        t.Word = word;
                        if (MacroSyntaxProvider.IsKnownTag(word))
                        {
                            m_tags.Add(t);
                        }
                        else if (text[i] == '=')
                        {
                            m_tagCommands.Add(t);
                        }
                        else if (text[i] == '"' && text[i - 1] != ':')
                        {
                            m_tagStrings.Add(t);
                        }
                        //else if (text[i] == '!')
                        //{
                        //    m_tagVariable.Add(t);
                        //}
                    }
                    if (text[i] == '"') pqIndex = i;

                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (MacroSyntaxProvider.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }
        #endregion

        public string FilePath { get; set; }
        public bool IsPreBuils { get; set; }
        internal void LoadFile(string filePath, bool isBase)
        {
            IsPreBuils = isBase;
            using (System.IO.FileStream stream = Delimon.Win32.IO.File.OpenRead(filePath))
            {
                try
                {
                    TextRange range = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
                    range.Load(stream, DataFormats.Text);
                    FilePath = filePath;
                }
                catch
                {
                    "Couldnt load file".Show();
                    NeedsSave = false;
                    this.Close();
                }
                stream.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (IsPreBuils)
            {
                "Cant save changes to pre built macros".Show();
                return;

            }
            Save();
            this.Close();
        }

        private void Save()
        {
            if (TextInput.Document == null) return;
            try
            {
                string filetext = "";
                var textLines = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd).Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                foreach (var line in textLines)
                {
                    filetext += line + Environment.NewLine;
                }

                if (Delimon.Win32.IO.File.Exists(FilePath)) Delimon.Win32.IO.File.Delete(FilePath);
                Delimon.Win32.IO.File.WriteAllText(FilePath, filetext);
            }
            catch
            {
                "Couldnt save file".Show();
            }

            NeedsSave = false;

            //using (System.IO.FileStream stream = Delimon.Win32.IO.File.OpenWrite(FilePath))
            //{
            //    try
            //    {
            //        TextRange range = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
            //        range.Save(stream, DataFormats.Text);
            //    }
            //    catch
            //    {
            //        "Couldnt save file".Show();
            //    }

            //    stream.Close();
            //}

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            NeedsSave = false;
            this.Close();
        }
    }
}
