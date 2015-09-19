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

namespace WPF_WYSIWYG_HTML_Editor.XAML
{
    /// <summary>
    /// Interaction logic for WysiwigControl.xaml
    /// </summary>
    public partial class WysiwigControl : UserControl
    {
        FindReplaceWindow frw;

        public WysiwigControl()
        {
            InitializeComponent();
            //lineResizer.DataContext = this;
            //MouseY = 0;
            //lineResizer.Visibility = System.Windows.Visibility.Hidden;
        }
        private void SettingsBold_Click(object sender, RoutedEventArgs e)
        {
            Format.bold();
        }

        private void SettingsItalic_Click(object sender, RoutedEventArgs e)
        {
            Format.Italic();
        }

        private void SettingsUnderLine_Click(object sender, RoutedEventArgs e)
        {
            Format.Underline();
        }

        private void SettingsRightAlign_Click(object sender, RoutedEventArgs e)
        {
            Format.Underline();
        }

        private void SettingsLeftAlign_Click(object sender, RoutedEventArgs e)
        {
            Format.JustifyLeft();
        }

        private void SettingsCenter2_Click(object sender, RoutedEventArgs e)
        {
            Format.JustifyCenter();
        }

        private void SettingsJustifyRight_Click(object sender, RoutedEventArgs e)
        {
            Format.JustifyRight();
        }

        private void SettingsJustifyFull_Click(object sender, RoutedEventArgs e)
        {
            Format.JustifyFull();
        }

        private void SettingsInsertOrderedList_Click(object sender, RoutedEventArgs e)
        {
            Format.InsertOrderedList();
        }

        private void SettingsBullets_Click(object sender, RoutedEventArgs e)
        {
            Format.InsertUnorderedList();
        }

        private void SettingsOutIdent_Click(object sender, RoutedEventArgs e)
        {
            Format.Outdent();
        }

        private void SettingsIdent_Click(object sender, RoutedEventArgs e)
        {
            Format.Indent();
        }

        private void RibbonButtonNew_Click(object sender, RoutedEventArgs e)
        {
            Gui.newdocument();
        }

        private void RibbonButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Gui.newdocumentFile();
        }

        private void RibbonButtonOpenweb_Click(object sender, RoutedEventArgs e)
        {
            webBrowserEditor.newWb(@"http://www.codeproject.com/");
        }

        private void SettingsFontColor_Click(object sender, RoutedEventArgs e)
        {
            Gui.SettingsFontColor();
        }

        private void SettingsBackColor_Click(object sender, RoutedEventArgs e)
        {
            Gui.SettingsBackColor();
        }

        private void SettingsAddLink_Click(object sender, RoutedEventArgs e)
        {
            Gui.SettingsAddLink();
        }

        private void SettingsAddImage_Click(object sender, RoutedEventArgs e)
        {
            Gui.SettingsAddImage();
        }

        private void RibbonButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Gui.RibbonButtonSave();
        }

        private void RibbonComboboxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Gui.RibbonComboboxFonts(RibbonComboboxFonts);
        }

        private void RibbonComboboxFontHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Gui.RibbonComboboxFontHeight(RibbonComboboxFontHeight);
        }

        private void RibbonComboboxFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Gui.RibbonComboboxFormat(RibbonComboboxFormat);
        }

        private void EditWeb_Click(object sender, RoutedEventArgs e)
        {
            Gui.EditWeb();
        }

        private void ViewHTML_Click(object sender, RoutedEventArgs e)
        {
            Gui.ViewHTML();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Gui.webBrowser = webBrowserEditor;
            Gui.htmlEditor = HtmlEditor1;
            Initialisation.webeditor = this;
            Gui.newdocument();

            Initialisation.RibbonComboboxFontsInitialisation();
            Initialisation.RibbonComboboxFontSizeInitialisation();
            Initialisation.RibbonComboboxFormatInitionalisation();

           // if (ribon.SelectedIndex != 2)
            //grdFtpProjects.Visibility = System.Windows.Visibility.Collapsed;
        }



        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is XmlRpcVM)
            {
                XmlRpcVM vm = DataContext as XmlRpcVM;
                vm.OnPublishClick(webBrowserEditor.doc.body.innerHTML);
            }
        }

        private void pbnPubfromVault_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is XmlRpcVM)
            {
                XmlRpcVM vm = DataContext as XmlRpcVM;
                vm.OnPubFromVaultClick(webBrowserEditor.doc.body.innerHTML);
            }
        }

        public void SetProfileData(SocialOrganizer.Models.PersonData profile)
        {
            //ftpProjectList.SetProfile(profile);
            //ftpProjectList.SetProfile(profile);
        }
        double preHeight = 0;
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ribon.SelectedIndex == 2)
            //{
            //    if (preHeight < ribon.ActualHeight)
            //    preHeight = ribon.ActualHeight;
            //    grdFtpProjects.Visibility = System.Windows.Visibility.Visible;
            //    if (ribon.Height != 50.0)
            //    ribon.Height = 50;
            //    webBrowserEditor.unhook();
            //}
            //else
            //{
            //    if (preHeight > 0)
            //        ribon.Height = preHeight;
            //    grdFtpProjects.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private void btnSpin_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is XmlRpcVM)
            {
                XmlRpcVM vm = DataContext as XmlRpcVM;
                //tbContrl.Visibility = System.Windows.Visibility.Visible;
                vm.Spin(webBrowserEditor.doc.body.innerText, tbPostTitle.Text);
            }
        }

        private void btnClearAllTabs_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext != null && DataContext is XmlRpcVM)
            {
                XmlRpcVM vm = DataContext as XmlRpcVM;
                vm.ClearSpunTabs();
                //tbContrl.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region custom windows
        private void btnSpinWindow_Click(object sender, RoutedEventArgs e)
        {
            SpinWindow sw = new SpinWindow();
            sw.OnClickedSpin += sw_OnClickedSpin;
            sw.Show();
        }

        void sw_OnClickedSpin(string spunText)
        {
            webBrowserEditor.doc.body.innerText = spunText;
        }

        private void btnFindReplace_Click(object sender, RoutedEventArgs e)
        {
            if (frw == null)
            {
                frw = new FindReplaceWindow();
                frw.OnClickedFind += frw_OnClickedFind;
                frw.OnClickedFindReplace += frw_OnClickedFindReplace;
                frw.OnClickedReplaceAll += frw_OnClickedReplaceAll;
                frw.Closed += frw_Closed;
                frw.Show();
            }
        }

        void frw_OnClickedFind(string findText, bool clickedOnce)
        {
            webBrowserEditor.Find(findText, clickedOnce);
        }

        void frw_OnClickedFindReplace(string findText, string replaceText, bool clickedOnce)
        {
            webBrowserEditor.FindReplace(findText, replaceText, clickedOnce);
        }

        void frw_OnClickedReplaceAll(string findText, string replaceText)
        {
            webBrowserEditor.ReplaceAll(findText, replaceText, false);
        }

        void frw_Closed(object sender, EventArgs e)
        {
            frw = null;
        }

        #endregion

        #region Resize



        //public int MouseY
        //{
        //    get { return (int)GetValue(MouseYProperty); }
        //    set { SetValue(MouseYProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MouseX.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MouseYProperty =
        //    DependencyProperty.Register("MouseY", typeof(int), typeof(WysiwigControl), new UIPropertyMetadata(0));


        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);

        //    MouseY = (int)Mouse.GetPosition(grdTasks).Y;
        //    double y = Mouse.GetPosition(tbContrl).Y;
        //    if (Mouse.GetPosition(tbContrl).Y < tbContrl.ActualHeight)
        //    {
        //        Mouse.SetCursor(Cursors.Cross);

        //        if (e.LeftButton == MouseButtonState.Pressed)
        //        {
        //            lineResizer.Visibility = System.Windows.Visibility.Visible;
        //            IsResizeIng = true;
        //            Mouse.SetCursor(Cursors.Cross);
        //        }
        //    }

        //    if (IsResizeIng && System.Windows.Forms.Cursor.Current != System.Windows.Forms.Cursors.Cross)
        //        Mouse.SetCursor(Cursors.Cross);
        //}

        //public bool IsResizeIng { get; set; }

        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonUp(e);

        //    if (IsResizeIng)
        //    {
        //        double width = Mouse.GetPosition(tbContrl).Y;
        //        if (width > 145)
        //        {
        //            tbContrl.Height = width + 5;
        //            //cmbOrdering.Width = width + 5;
        //        }
        //        else // if width < 145
        //        {
        //            tbContrl.Height = 150;
        //            //cmbOrdering.Width = 150;
        //        }
        //    }

        //    IsResizeIng = false;
        //    lineResizer.Visibility = System.Windows.Visibility.Hidden; 
        //}

        #endregion
    }
}