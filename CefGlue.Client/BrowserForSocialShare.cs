using BrowserHost.ViewModels;
using Organiser.Common.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Xilium.CefGlue.Client
{
    public partial class BrowserForSocialShare : Form
    {
        private string startUrl = "";

        public BrowserForSocialShare()
        {
            InitializeComponent();
        }

        private void browserCntrl1_OnBrowserStatusChanged(string obj)
        {
            Text = Text.Trim();
            Text = Text.Replace("Loading...", "Loaded.");
           if(!elementHost1.Visible) browserCntrl1.Dock = DockStyle.Fill;
        }

        public void SetSocialButtonsVisable(string type)
        {
            if(type == "SocialEngagerOptimizer")
            {
                elementHost1.Visible = true;
                socialButtonsUserControl1.OnClickedSocialButtons += SocialButtonsUserControl1_OnClickedSocialButtons;
            }
        }

        private void SocialButtonsUserControl1_OnClickedSocialButtons(string typeOfSocialbtn)
        {
            try
            {
                string fullUrl = Social.GetShareUrl(typeOfSocialbtn, startUrl);

                Text = Text.Replace("Loaded.", "Loading...");

                if (fullUrl != "" && fullUrl != "pin")
                    browserCntrl1.Navigate(fullUrl);
                else if (fullUrl == "pin")
                {
                   // System.Diagnostics.Debugger.Launch();
                    browserCntrl1.OnBrowserLoadingChanged += BrowserCntrl1_OnBrowserLoadingChanged;
                    browserCntrl1.Navigate(startUrl);
                }
            }
            catch { }
        }

        private void BrowserCntrl1_OnBrowserLoadingChanged(bool loaded)
        {
            if (loaded)
            {
                browserCntrl1.OnBrowserLoadingChanged -= BrowserCntrl1_OnBrowserLoadingChanged;

                PinterestImagePickerVM pinterestImagePicker = new PinterestImagePickerVM();
                pinterestImagePicker.OnLaunchSharePopup += PinterestImagePicker_OnLaunchSharePopup;
                pinterestImagePicker.VisitSource(browserCntrl1.CBrowser.Browser.GetMainFrame().Url);
                browserCntrl1.CBrowser.Browser.GetMainFrame().GetSource(pinterestImagePicker.Visitor);
            }
        }

        private void PinterestImagePicker_OnLaunchSharePopup(string fullUrl)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(PinterestImagePicker_OnLaunchSharePopup), fullUrl);
                return;
            }
            browserCntrl1.Navigate(fullUrl);
        }

        public void SetStartUrl(string url)
        {
            startUrl = url;
        }
    }
}
