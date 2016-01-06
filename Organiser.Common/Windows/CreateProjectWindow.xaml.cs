using Organiser.Common.Classes;
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
using System.Windows.Shapes;

namespace Organiser.Common.Windows
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public bool IsReadOnly;

        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void projName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                (sender as TextBox).IsReadOnly = true;
                (sender as TextBox).SelectAll();
                MyFilesDatabase.SetClipboardText((sender as TextBox).Text);
            }
        }
    }
}