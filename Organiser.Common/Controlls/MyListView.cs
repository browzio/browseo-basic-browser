using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Organiser.Common.Controlls
{
    public class MyListView : ListView
    {
        public MyListView() 
        {
            this.MouseEnter += MyListView_MouseEnter;
            this.MouseLeave += MyListView_MouseLeave; 
        }

        private void MyListView_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MyIsMouseDirectlyOver = false;
        }

        private void MyListView_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MyIsMouseDirectlyOver = true;
        }

        public bool MyIsMouseDirectlyOver
        {
            get { return (bool)GetValue(MyIsMouseDirectlyOverProperty); }
            set { SetValue(MyIsMouseDirectlyOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyIsMouseDirectlyOver.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyIsMouseDirectlyOverProperty =
            DependencyProperty.Register("MyIsMouseDirectlyOver", typeof(bool), typeof(MyListView), new PropertyMetadata(0));


    }
}
