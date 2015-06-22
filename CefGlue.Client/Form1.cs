using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xilium.CefGlue.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           BrowserCntrl b = new BrowserCntrl();
           b.init("https://google.com");
           b.Dock = DockStyle.Fill;
            this.Controls.Add(b);
        }
    }
}
