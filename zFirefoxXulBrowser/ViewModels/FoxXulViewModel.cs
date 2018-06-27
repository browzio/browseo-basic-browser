using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SocialOrganizer.Models;
using Gecko;
using Gecko.DOM;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using Organiser.Common.Windows;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Diagnostics;
using Gecko.Windows;
using zFirefoxXulBrowser.Controls;
using Gecko.Interfaces;

namespace zFirefoxXulBrowser.ViewModels
{
    public class FoxXulViewModel : ViewModelBase
    {
        public static WebView WebBrowser
        {
            get { return FFXulBrowserUserControl.pageView; }
        }
        public FoxXulViewModel()
        {
        }
    }
}
