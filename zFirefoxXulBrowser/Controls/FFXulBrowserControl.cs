using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko.Windows;

namespace zFirefoxXulBrowser.Controls
{
    public partial class FFXulBrowserControl : UserControl
    {
        public WebView Browser { get; private set; }

        public FFXulBrowserControl()
        {
            InitializeComponent();
        }

        public async void initBrowser(string url)
        {
            Browser = new WebView();
            Browser.Dock = DockStyle.Fill;
            Browser.Navigate(url);

            
            Browser.Navigating += (s, ee) =>
            {

            };

            Browser.DocumentCompleted += (s, ee) =>
            {
            };


            Browser.DocumentTitleChanged += (s, e) =>
            {
            };

            Browser.StatusTextChanged += (s, e) =>
            {

            };

            //TODO
            //Browser.ShowContextMenu += (s, e) =>
            //{

            //    if (!e.AssociatedLink.IsNullOrEmpty())
            //    {
            //        e.ContextMenu.MenuItems.Add("-");

            //        MenuItem nt = new MenuItem() { Name = "1", Text = "Open In New Tab" };
            //        nt.Click += (ss, ee) => { OnCreateNewTab(e.AssociatedLink); };
            //        e.ContextMenu.MenuItems.Add(nt);
            //    }

            //    e.ContextMenu.MenuItems.Add("-");

            //    //model.AddItem(333, "To Social Enagager");
            //    MenuItem tse = new MenuItem() { Name = "888", Text = "To Social Enagager" };
            //    tse.Click += Tse_Click;
            //    e.ContextMenu.MenuItems.Add(tse);

            //    e.ContextMenu.MenuItems.Add("-");

            //    //model.AddItem(222, "Curaste...");
            //    MenuItem cur = new MenuItem() { Name = "222", Text = "Curaste..." };
            //    cur.Click += Tse_Click;
            //    e.ContextMenu.MenuItems.Add(cur);

            //    //model.AddItem(666, "Curate It");
            //    MenuItem ci = new MenuItem() { Name = "666", Text = "Curate It" };
            //    ci.Click += Tse_Click;
            //    e.ContextMenu.MenuItems.Add(ci);

            //    if (Browser.Url != null &&
            //    Browser.Url.ToString().ToLower().Contains("www.facebook.com/search") || Browser.Url.ToString().ToLower().Contains("facebook.com/groups/?category=membership"))
            //    {
            //        e.ContextMenu.MenuItems.Add("-");

            //        //model.AddItem(555, "Dominate");
            //        MenuItem d = new MenuItem() { Name = "555", Text = "Dominate" };
            //        d.Click += Tse_Click;
            //        e.ContextMenu.MenuItems.Add(d);

            //        //model.AddItem(444, "Dominate All");
            //        MenuItem da = new MenuItem() { Name = "444", Text = "Dominate All" };
            //        da.Click += Tse_Click;
            //        e.ContextMenu.MenuItems.Add(da);
            //    }

            //    if (Browser.Url != null && Browser.Url.ToString().ToLower().Contains("https://www.facebook.com/bookmarks/groups"))
            //    {
            //        e.ContextMenu.MenuItems.Add("-");

            //        //model.AddItem(555, "Dominate");
            //        MenuItem d = new MenuItem() { Name = "2", Text = "Dominate All" };
            //        d.Click += Tse_Click;
            //        e.ContextMenu.MenuItems.Add(d);
            //    }
            //};

            this.SuspendLayout();
            this.Controls.Add(Browser);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
