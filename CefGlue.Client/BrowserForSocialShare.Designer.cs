namespace Xilium.CefGlue.Client
{
    partial class BrowserForSocialShare
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.socialButtonsUserControl1 = new Organiser.Common.Controlls.SocialButtonsUserControl();
            this.browserCntrl1 = new Xilium.CefGlue.Client.BrowserCntrl();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Top;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(932, 45);
            this.elementHost1.TabIndex = 1;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Visible = false;
            this.elementHost1.Child = this.socialButtonsUserControl1;
            // 
            // browserCntrl1
            // 
            this.browserCntrl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.browserCntrl1.CBrowser = null;
            this.browserCntrl1.CurrAddress = null;
            this.browserCntrl1.Location = new System.Drawing.Point(0, 51);
            this.browserCntrl1.Name = "browserCntrl1";
            this.browserCntrl1.Size = new System.Drawing.Size(932, 602);
            this.browserCntrl1.TabIndex = 0;
            this.browserCntrl1.OnBrowserStatusChanged += new System.Action<string>(this.browserCntrl1_OnBrowserStatusChanged);
            // 
            // BrowserForSocialShare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 653);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.browserCntrl1);
            this.Name = "BrowserForSocialShare";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BrowserForSocialShare";
            this.ResumeLayout(false);

        }

        #endregion

        public BrowserCntrl browserCntrl1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private Organiser.Common.Controlls.SocialButtonsUserControl socialButtonsUserControl1;
    }
}