using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko.Windows;
using Gecko;
using Gecko.Interfaces;
using Gecko.Interop;
using Gecko.DOM;
using System.Runtime.InteropServices;
using System.ComponentModel;
using BrowseoFX_WPF.Core.DataAccess;
using BrowseoFX_WPF.Core.BrowserListeners;
using System.IO;
using Organiser.Common.Classes;

namespace BrowseoFX_WPF.Core.BrowserHandlers.GUI
{
    public class PanelUIHandler
    {
        public GeckoXULElement NaveBarContents { get; set; }
        public GeckoXULElement PanelUIcontents { get; set; }
        public GeckoXULElement PanelUImultiView { get; set; }
        public GeckoXULElement PanelUIpopup { get; set; }

        public bool NoFB { get; internal set; }
        public bool IsEnabledForKK { get; set; }

        public bool isEnabledForFree = true;

        //public GeckoXULElement PanelUISub_vboxSecurity { get; set; }
        //public GeckoXULElement PanelUISub_vboxTimeSync { get; set; }

        public PanelUIHandler()
        {
            IsEnabledForKK = true;
        }
        public void Init()
        {
            //create default styles for the edited window
            CreateDefaultWindowView(BrowseoFXManager.Instance.GloableWebView);

            //navigation bar content
            NaveBarContents = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("nav-bar-customization-target") as GeckoXULElement;

            //contents in the panelui popup
            PanelUIcontents = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("PanelUI-contents") as GeckoXULElement;

            //multi view qithin the popup panel ui
            PanelUImultiView = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("PanelUI-multiView") as GeckoXULElement;

            //open menu button PanelUI inside nav-bar PanelUI-popup PanelUI-button PanelUI-menu-button
            PanelUIpopup = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("PanelUI-popup") as GeckoXULElement;
            AddEventListenerFor(PanelUIpopup, "popupshown", PanelUIListenerState.PanelUIPopUp, PanelUIListenerState.PanelUIPopUp.GetDescription());

            //added functionality to the navigation bar

            //if (isEnabledForFree)
            //{
            //FbConverseo Button
            if (!NoFB)
            {
                CreateBirdsEyeButton();
                CreateCheckpointCheck();
                CreateLSBButton();
                CreateSEOButton();
                CreateFbConverseoButton();
            }

            //Social social-share-button
            //CreateSocialButton();

            //browseoIA
            //CreateBrowseoIAUI();

            //Added Functionality to the popup ui\\
            //Anonymity panel
            CreateAnonymitySettingsPanelUI();

            //TimeSync panel
            CreateTimeSyncPanelUI();

            //fontswitcherLink
            CreateFontSwitcherLink();

            //dominate Button
            //CreateDominateAllButton();

            //Social StatsButton 
            CreateSocialStatsButton();

            //CP Button
            CreateCPButton();

            //Formfiller
            CreateFormFillerButton();


            //console shortcut options
           CreateCtlSKButton();

            ////Additional context mmenu features
            CreateContextMenueAdditions();
            //  }
        }

        bool wtfman;
        private async void CreateCheckpointCheck()
        {
#if DEBUG
            int wtf = 0;
#else
            if (!wtfman)
            {
                wtfman = true;
                await Task.Run(() =>
                {
                    string tmpdir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp";
                    int waited = 0;
                    while (!Directory.Exists(tmpdir))
                    {
                        Task.Delay(500); waited++;
                        if (waited >= 10)
                        {
                            BadImageException ex = new BadImageException("Un Known");
                            ex.Source = "IMG";
                            throw ex;
                        }
                    }
                    waited = 0;
                    string fpat = System.IO.Path.Combine(tmpdir, "669dbe86-9b83-4848-a4e8-ab3f86a69740");
                    while (!File.Exists(fpat))
                    {
                        Task.Delay(500); waited++;
                        if (waited >= 10)
                        {
                            BadImageException ex = new BadImageException("Un Known");
                            ex.Source = "IMG";
                            throw ex;
                        }
                    }
                    if (File.ReadAllText(fpat).Trim() != "5a73dee9-8b23-4b50-9ad4-8678a5b983ac")
                    {
                        BadImageException ex = new BadImageException("Un Known");
                        ex.Source = "IMG";
                        throw ex;
                    }
                    else
                        File.Delete(fpat);


                    waited = 0;
                    fpat = System.IO.Path.Combine(tmpdir, "46e0290a-2219-47e4-9e20-d21a8787ccfe");
                    while (!File.Exists(fpat))
                    {
                        Task.Delay(500); waited++;
                        if (waited >= 10)
                        {
                            BadImageException ex = new BadImageException("Un Known");
                            ex.Source = "IMG";
                            throw ex;
                        }
                    }
                    if (File.ReadAllText(fpat).Trim() != "15516b40-a0ec-43a3-8a30-729c12494598")
                    {
                        BadImageException ex = new BadImageException("Un Known");
                        ex.Source = "IMG";
                        throw ex;
                    }
                    else
                        File.Delete(fpat);
                });
            }
#endif
        }

        public void CreateContextMenueAdditions()
        {
            //<menupopup id="contentAreaContextMenu" pagemenu="#page-menu-separator"
            var menugroup = BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementById("contentAreaContextMenu") as GeckoXULElement;
            if (menugroup != null)
            {
                new ContextMenuListener(ContextMenuListenerStates.Default, menugroup, "popupshown");
                
                //<menuseparator id="spell-suggestions-separator"/>
                var menuseparator = CreateXULElement("menuseparator", "id", "spell-suggestions-separator");
                menugroup.AppendChild(menuseparator);

                //  < menuitem id = "context-openlinkincurrent"
                //label = "&openLinkCmdInCurrent.label;"
                //accesskey = "&openLinkCmdInCurrent.accesskey;"
                //oncommand = "gContextMenu.openLinkInCurrent();" />

                var menuitem_ToSocialEngager = CreateXULElement("menuitem",
                    "id", "menuitem_ToSocialEngager",
                    "consumeanchor", "contentAreaContextMenu",
                    "label", "To Social Engager");
                menugroup.AppendChild(menuitem_ToSocialEngager);
                new ContextMenuListener(ContextMenuListenerStates.menuitem_ToSocialEngager, menuitem_ToSocialEngager, "command");

                var menuitem_curate = CreateXULElement("menuitem",
                    "id", "menuitem_curate",
                    "consumeanchor", "contentAreaContextMenu",
                    "label", "Curate");
                menugroup.AppendChild(menuitem_curate);
                new ContextMenuListener(ContextMenuListenerStates.menuitem_curate, menuitem_curate, "command");

                var menuitem_curaste = CreateXULElement("menuitem",
                    "id", "menuitem_curaste",
                    "consumeanchor", "contentAreaContextMenu",
                    "label", "Curaste");
                menugroup.AppendChild(menuitem_curaste);
                new ContextMenuListener(ContextMenuListenerStates.menuitem_curaste, menuitem_curaste, "command");
            }
        }

        public void CreateDefaultWindowView(WebView webview)
        {
            var toolbars = webview.Widget.View.Document.GetElementsByTagName("toolbar");
            int times = 0;
            //foreach (var tBar in toolbars)
            //{
            //    tBar.SetAttribute("style",
            //        "background-color:#f5f5f5; !important;");
            //    times++;
            //    if (times == 2) break;
            //}

            var mainWindow = webview.Widget.View.Document.GetElementById("main-window") as GeckoXULElement;
            if (mainWindow != null)
            {
                mainWindow.SetAttribute("fullscreenbutton", "false");
                mainWindow.SetAttribute("sizemode", "maximized");
                mainWindow.SetAttribute("resizable", "no");
                new MainWindowEventListener(MainWindowListenerStates.mainwindow_dblclick, mainWindow, "dblclick");
                new MainWindowEventListener(MainWindowListenerStates.mainwindow_onload, mainWindow, "onload");
            }

            using (var thisWindow = Xpcom.QueryInterface2<mozIDOMWindowProxy>(webview.Widget.View.Instance))
            {
                using (var domchromeWindow = Xpcom.QueryInterface2<nsIDOMChromeWindow>(thisWindow.Instance))
                {
                    domchromeWindow.Instance.Maximize();
                }
            }

            //titlebar-placeholder
            var titlebar = webview.Widget.View.Document.GetElementById("titlebar") as GeckoXULElement;
            if (titlebar != null)
            {
                titlebar.Hidden = true;
            }
            //btns
            var btn1 = webview.Widget.View.Document.GetElementById("restore-button") as GeckoXULElement;
            if (btn1 != null)
            {
                btn1.Hidden = true;
            }
            var btn2 = webview.Widget.View.Document.GetElementById("close-button") as GeckoXULElement;
            if (btn2 != null)
            {
                btn2.Hidden = true;
            }
            var btn3 = webview.Widget.View.Document.GetElementById("minimize-button") as GeckoXULElement;
            if (btn3 != null)
            {
                btn3.Hidden = true;
            }

            //tab-view-deck
            var toolbarMenubar = webview.Widget.View.Document.GetElementById("toolbar-menubar") as GeckoXULElement;
            if (toolbarMenubar != null)
            {
                toolbarMenubar.Hidden = true;
            }

            //foreach (var key in BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.GetElementsByTagName("key"))
            //{
            //    var curMods = key.GetAttribute("modifiers");
            //    curMods = curMods.Replace("accel", "control");
            //    key.SetAttribute("modifiers", curMods);
            //}
        }

        #region navbar addon region
        private void CreateBirdsEyeButton()
        {
            var navBar_toolbarlitem_BirdsEye = CreateXULElement("toolbaritem",
            "id", "navBar_toolbarlitem_BirdsEye");
            NaveBarContents.AppendChild(navBar_toolbarlitem_BirdsEye);

            var navBar_toolbarbutton_BirdsEye = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_BirdsEye",
                "consumeanchor", "navBar_toolbarlitem_BirdsEye",
                "tooltiptext", "Birds Eye Dashboard",
                "label", "Birds Eye DAshboard",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/baseline_dashboard_black_18dp.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_BirdsEye.AppendChild(navBar_toolbarbutton_BirdsEye);

            new NavBarEventListener(NavBarListenerStates.navbar_BirdsEye, navBar_toolbarbutton_BirdsEye, "command");
        }

        private void CreateSEOButton()
        {
            var navBar_toolbarlitem_SEO = CreateXULElement("toolbaritem",
                "id", "navBar_toolbarlitem_SEO");
            NaveBarContents.AppendChild(navBar_toolbarlitem_SEO);

            var navBar_toolbarbutton_SEO = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_SEO",
                "consumeanchor", "navBar_toolbarlitem_SEO",
                "tooltiptext", "S·E·O",
                "label", "S·E·O",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/Social_Media.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_SEO.AppendChild(navBar_toolbarbutton_SEO);

            new NavBarEventListener(NavBarListenerStates.navbar_SEO, navBar_toolbarbutton_SEO, "command");
        }

        private void CreateLSBButton()
        {
            var navBar_toolbarlitem_LSB = CreateXULElement("toolbaritem",
     "id", "navBar_toolbarlitem_LSB");
            NaveBarContents.AppendChild(navBar_toolbarlitem_LSB);

            var navBar_toolbarbutton_LSB = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_LSB",
                "consumeanchor", "navBar_toolbarlitem_LSB",
                "tooltiptext", "L·S·B",
                "label", "L·S·B",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/ic_open_in_browser_black_24dp_2x.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_LSB.AppendChild(navBar_toolbarbutton_LSB);

            new NavBarEventListener(NavBarListenerStates.navbar_LSB, navBar_toolbarbutton_LSB, "command");
        }

        private void CreateFbConverseoButton()
        {
            var navBar_toolbarlitem_FbConverseo = CreateXULElement("toolbaritem",
                "id", "navBar_toolbarlitem_FbConverseo");
            NaveBarContents.AppendChild(navBar_toolbarlitem_FbConverseo);

            var navBar_toolbarbutton_FbConverseo = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_FbConverseo",
                "consumeanchor", "navBar_toolbarlitem_FbConverseo",
                "tooltiptext", "FB Conver·SEO",
                "label", "FB Conver·SEO",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/fb_black_circle.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_FbConverseo.AppendChild(navBar_toolbarbutton_FbConverseo);

            new NavBarEventListener(NavBarListenerStates.navbar_FbConverseo, navBar_toolbarbutton_FbConverseo, "command");
        }

        /// <summary>
        /// Social Share
        /// </summary>
        private void CreateSocialButton()
        {
            var navBar_toolbarlitem_Social = CreateXULElement("toolbaritem",
            "id", "navBar_toolbarlitem_Social");
            NaveBarContents.AppendChild(navBar_toolbarlitem_Social);

            var navBar_toolbarbutton_Social = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_Social",
                "consumeanchor", "navBar_toolbarlitem_Social",
                "tooltiptext", "Social Share",
                "label", "Social Share",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "oncommand", "SocialShare.sharePage();",
                "style", "list-style-image: url(\"chrome://xulfx/skin/browseo.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_Social.AppendChild(navBar_toolbarbutton_Social);

            //new NavBarEventListener(NavBarListenerStates.navbar_ButtonBrowseoIA, navBar_toolbarbutton_BrowseoIA, "command");
        }

        /// <summary>
        /// Browseo IA
        /// </summary>
        private void CreateBrowseoIAUI()
        {
            var navBar_toolbarlitem_BrowseoIA = CreateXULElement("toolbaritem",
                "id", "navBar_toolbarlitem_BrowseoIA");
            NaveBarContents.AppendChild(navBar_toolbarlitem_BrowseoIA);

            var navBar_toolbarbutton_BrowseoIA = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_BrowseoIA",
                "consumeanchor", "NavBar_toolbarlitem_BrowseoIA",
                "tooltiptext", "Open Browseo IA",
                "label", "Browseo IA",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/browseo.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_BrowseoIA.AppendChild(navBar_toolbarbutton_BrowseoIA);
            
            new NavBarEventListener(NavBarListenerStates.navbar_ButtonBrowseoIA, navBar_toolbarbutton_BrowseoIA, "command");
        }

        #endregion

        #region PanelUI region

        private void CreateFontSwitcherLink()
        {
            //placeholder in panelUi popup for the button
            var panelUI_toolbaritemFontSwitch = CreateXULElement("toolbaritem",
                "id", "panelUI_toolbaritemFontSwitch");
            PanelUIcontents.AppendChild(panelUI_toolbaritemFontSwitch);

            //timesync settings button
            var panelUI_toolbarbuttonFontSwitch = CreateXULElement("toolbarbutton",
             "id", "panelUI_toolbarbuttonFontSwitch",
             "consumeanchor", "panelUI_toolbaritemFontSwitch",
             "tooltiptext", "Open Font Switch Settings",
             "label", "Edit Fonts",
             "class", "toolbarbutton-1 chromeclass-toolbar-additional",
             "style", "list-style-image: url(\"chrome://xulfx/skin/ic_format_size_black_24dp_2x.png\"); -moz-image-region: auto;",
             //"oncommand", "gBrowser.addTab(\"http://no.google.anymore/\")");
            "oncommand", "openLinkIn(\"about:preferences#content\", \"tab\", { charset: undefined, referrerURI: undefined, inBackground: false });");// "gBrowser.loadURI(\"about:preferences#content\")");
            panelUI_toolbaritemFontSwitch.AppendChild(panelUI_toolbarbuttonFontSwitch);
           // AddEventListenerFor(panelUI_toolbarbuttonFontSwitch, "command", PanelUIListenerState.panelUI_toolbarbuttonTimeSync, "panelUI_toolbaritemFontSwitch");
        }

        private void CreateTimeSyncPanelUI()
        {
            //placeholder in panelUi popup for the button
            var panelUI_toolbaritemTimeSync = CreateXULElement("toolbaritem",
                "id", "panelUI_toolbaritemTimeSync");
            PanelUIcontents.AppendChild(panelUI_toolbaritemTimeSync);

            //timesync settings button
            var panelUI_toolbarbuttonTimeSync = CreateXULElement("toolbarbutton",
             "id", "panelUI_toolbarbuttonTimeSync",
             "consumeanchor", "panelUI_toolbaritemTimeSync",
             "tooltiptext", "Open Time Sync Settings",
             "label", "Time Sync",
             "class", "toolbarbutton-1 chromeclass-toolbar-additional",
             "style", "list-style-image: url(\"chrome://xulfx/skin/ic_access_time_black_24dp_2x.png\"); -moz-image-region: auto;",
             "oncommand", "PanelUI.showSubView('panelUI_panelviewTimeSync', this, 'PanelUI-contents')",
             "closemenu", "none");
            panelUI_toolbaritemTimeSync.AppendChild(panelUI_toolbarbuttonTimeSync);

            //timesync slideout\\

            //panel view for the settings options
            var panelUI_panelviewTimeSync = CreateXULElement("panelview",
            "id", "panelUI_panelviewTimeSync",
            "flex", "1",
            "class", "panel-subview-body",
            "flip", "slide",
            "animate", "true");
            PanelUImultiView.AppendChild(panelUI_panelviewTimeSync);

            //label for the panel slideout
            var PanelUI_labelTimeSync = CreateXULElement("label",
            "id", "PanelUI_labelTimeSync",
            "class", "panel-subview-header",
            "value", "Time Sync Settings");
            panelUI_panelviewTimeSync.AppendChild(PanelUI_labelTimeSync);

            //viewbox to hold the options
            var PanelUISub_vboxTimeSync = CreateXULElement("vbox",
            "id", "PanelUISub_vboxTimeSync",
            "class", "panel-subview-body");
            panelUI_panelviewTimeSync.AppendChild(PanelUISub_vboxTimeSync);

            foreach (var timezone in BrowseoFXManager.Instance.SettingsHandler.TimeZones)
            {
                AddCBOptionTo(timezone, timezone,
                    (timezone == BrowseoFXManager.Instance.SettingsHandler.SystemTimeZone_Current).ToLowerString(),
                    PanelUIListenerState.PanelUImenuitem_TimeZone, PanelUISub_vboxTimeSync);
            }
        }

        private void CreateCtlSKButton()
        {
            var panelUi_toolbarlitem_CP = CreateXULElement("toolbaritem",
                "id", "panelUi_toolbarlitem_CP");
            PanelUIcontents.AppendChild(panelUi_toolbarlitem_CP);

            var panelUi_toolbarbutton_CP = CreateXULElement("toolbarbutton",
                "id", "panelUi_toolbarbutton_CP",
                "consumeanchor", "panelUi_toolbarlitem_CP",
                "tooltiptext", "Open Dev Console",
                "label", "Open Dev Console",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "oncommand", "PanelUI.showSubView('panelUI_ctlsk', this, 'PanelUI-contents')",
                "style", "list-style-image: url(\"chrome://xulfx/skin/baseline_keyboard_black_18dp.png\"); -moz-image-region: auto;",
                "closemenu", "none");
            panelUi_toolbarlitem_CP.AppendChild(panelUi_toolbarbutton_CP);

            //panel view for the settings options
            var panelUI_ctlsk = CreateXULElement("panelview",
            "id", "panelUI_ctlsk",
            "flex", "1",
            "class", "panel-subview-body",
            "flip", "slide",
            "animate", "true");
            PanelUImultiView.AppendChild(panelUI_ctlsk);

            //label for the panel slideout
            var PanelUI_ctlsk = CreateXULElement("label",
            "id", "PanelUI_ctlsk",
            "class", "panel-subview-header",
            "value", "Console Shortcuts");
            panelUI_ctlsk.AppendChild(PanelUI_ctlsk);

            //viewbox to hold the options
            var PanelUISub_ctlsk_vbox = CreateXULElement("vbox",
            "id", "PanelUISub_ctlsk_vbox",
            "class", "panel-subview-body");
            panelUI_ctlsk.AppendChild(PanelUISub_ctlsk_vbox);

            var PanelUISubBtn_ctlsk = CreateXULElement("toolbarbutton",
            "id", "PanelUISubBtn_ctlsk",
            "label", "Facebook Freind Requests",
            "tooltiptext", "Accepts friends in the open friend requests window",
            "closemenu", "none",
            "class", "subviewbutton");
            PanelUISub_ctlsk_vbox.AppendChild(PanelUISubBtn_ctlsk);

            var PanelUIfbLikePagesBtn_ctlsk = CreateXULElement("toolbarbutton",
           "id", "PanelUIfbLikePagesBtn_ctlsk",
           "label", "Facebook Like Pages",
           "tooltiptext", "Like Pages From A Facebook Search",
           "closemenu", "none",
           "class", "subviewbutton");
            PanelUISub_ctlsk_vbox.AppendChild(PanelUIfbLikePagesBtn_ctlsk);

            var PanelUIfbLikeGroupsBtn_ctlsk = CreateXULElement("toolbarbutton",
            "id", "PanelUIfbLikeGroupsBtn_ctlsk",
            "label", "Facebook Join Groups",
            "tooltiptext", "Join Groups From A Facebook Search",
            "closemenu", "none",
            "class", "subviewbutton");
            PanelUISub_ctlsk_vbox.AppendChild(PanelUIfbLikeGroupsBtn_ctlsk);

            var PanelUIfbLikePostsBtn_ctlsk = CreateXULElement("toolbarbutton",
            "id", "PanelUIfbLikePostsBtn_ctlsk",
            "label", "Facebook Like Posts",
            "tooltiptext", "Like Posts On The Page",
            "closemenu", "none",
            "class", "subviewbutton");
            PanelUISub_ctlsk_vbox.AppendChild(PanelUIfbLikePostsBtn_ctlsk);

            new NavBarEventListener(NavBarListenerStates.panelUi_ctlSK_CP, panelUi_toolbarbutton_CP, "command");
            new NavBarEventListener(NavBarListenerStates.panelUi_ctlSK_AcceptFriends, PanelUISubBtn_ctlsk, "command");
            new NavBarEventListener(NavBarListenerStates.panelUi_ctlSK_LikePages, PanelUIfbLikePagesBtn_ctlsk, "command");
            new NavBarEventListener(NavBarListenerStates.panelUi_ctlSK_LikeGroups, PanelUIfbLikeGroupsBtn_ctlsk, "command");
            new NavBarEventListener(NavBarListenerStates.panelUi_ctlSK_LikePosts, PanelUIfbLikePostsBtn_ctlsk, "command");
        }

        /// <summary>
        /// CP On Top
        /// </summary>
        private void CreateCPButton()
        {
            var navBar_toolbarlitem_CP = CreateXULElement("toolbaritem",
            "id", "navBar_toolbarlitem_CP");
            NaveBarContents.AppendChild(navBar_toolbarlitem_CP);

            var navBar_toolbarbutton_CP = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_CP",
                "consumeanchor", "navBar_toolbarlitem_CP",
                "tooltiptext", "Open Copy Paste Window",
                "label", "C/P Window",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/ic_content_paste_black_24dp_2x.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_CP.AppendChild(navBar_toolbarbutton_CP);

            new NavBarEventListener(NavBarListenerStates.navbar_ButtonCP, navBar_toolbarbutton_CP, "command");
        }


        /// <summary>
        /// Form Filler
        /// </summary>
        private void CreateFormFillerButton()
        {
            var FormFiller_navBar_toolbarlitem = CreateXULElement("toolbaritem",
                "id", "FormFiller_navBar_toolbarlitem");
            NaveBarContents.AppendChild(FormFiller_navBar_toolbarlitem);

            var FormFiller_navBar_toolbarbutton = CreateXULElement("toolbarbutton",
                "id", "FormFiller_navBar_toolbarbutton",
                "consumeanchor", "FormFiller_navBar_toolbarlitem",
                "tooltiptext", "Fill Form With Profile Data",
                "label", "Form Filler",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/ic_person_black_24dp_2x.png\"); -moz-image-region: auto;");
            FormFiller_navBar_toolbarlitem.AppendChild(FormFiller_navBar_toolbarbutton);

            //var FormFiller_toolbarbutton_menupopup = CreateXULElement("menupopup",
            //   "id", "FormFiller_toolbarbutton_menupopup");
            //FormFiller_navBar_toolbarbutton.AppendChild(FormFiller_toolbarbutton_menupopup);

            //foreach (var profile in BrowseoFXManager.Instance.Project.Profiles)
            //{
            //    var FormFiller_menupopup_menuitem = CreateXULElement("menuitem", 
            //        "id", "FormFiller_menuitem_" + profile.ProfOrProjName,
            //        "label", profile.ProfOrProjName);
            //    FormFiller_toolbarbutton_menupopup.AppendChild(FormFiller_menupopup_menuitem);
            new NavBarEventListener(NavBarListenerStates.navbar_ButtonFormFiller, FormFiller_navBar_toolbarbutton, "command");
            //}
        }

        private void CreateDominateAllButton()
        {
            var navBar_toolbarlitem_DominateAll = CreateXULElement("toolbaritem",
                "id", "navBar_toolbarlitem_DominateAll");
            PanelUIcontents.AppendChild(navBar_toolbarlitem_DominateAll);

            var navBar_toolbarbutton_DominateAll = CreateXULElement("toolbarbutton",
                "id", "navBar_toolbarbutton_DominateAll",
                "consumeanchor", "navBar_toolbarlitem_DominateAll",
                "tooltiptext", "(From Facebook Search) Dominate All",
                "label", "Dominate All",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
                "style", "list-style-image: url(\"chrome://xulfx/skin/ic_present_to_all_black_24dp_2x.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_DominateAll.AppendChild(navBar_toolbarbutton_DominateAll);

            new NavBarEventListener(NavBarListenerStates.navbar_DominateAll, navBar_toolbarbutton_DominateAll, "command");
        }

        //navbar_Button_SocialStats
        /// <summary>
        /// CP On Top
        /// </summary>
        private void CreateSocialStatsButton()
        {
            var navBar_toolbarlitem_SS = CreateXULElement("toolbaritem",
            "id", "navBar_toolbarlitem_SS");
            PanelUIcontents.AppendChild(navBar_toolbarlitem_SS);

            var navbar_Button_SocialStats = CreateXULElement("toolbarbutton",
                "id", "navbar_Button_SocialStats",
                "consumeanchor", "navBar_toolbarlitem_SS",
                "tooltiptext", "Social Stats Of Urls On Page",
                "label", "Social Stats",
                "class", "toolbarbutton-1 chromeclass-toolbar-additional",
            "style", "list-style-image: url(\"chrome://xulfx/skin/ic_assessment_black_24dp_2x.png\"); -moz-image-region: auto;");
            navBar_toolbarlitem_SS.AppendChild(navbar_Button_SocialStats);

            new NavBarEventListener(NavBarListenerStates.navbar_Button_SocialStats, navbar_Button_SocialStats, "command");
        }

        private void CreateAnonymitySettingsPanelUI()
        {
            //PanelUI\\

            //placeholder in panelUi popup for the button
            var panelUI_toolbaritemSecurity = CreateXULElement("toolbaritem",
                "id", "panelUI_toolbaritemSecurity");
            PanelUIcontents.AppendChild(panelUI_toolbaritemSecurity);

            //anonymity settings button
            var panelUI_toolbarbuttonSecurity = CreateXULElement("toolbarbutton",
             "id", "panelUI_toolbarbuttonSecurity",
             "consumeanchor", "panelUI_toolbaritemSecurity",
             "tooltiptext", "Open Browser Anonymity Settings",
             "label", "Anonymity",
             "class", "toolbarbutton-1 chromeclass-toolbar-additional",
             "style", "list-style-image: url(\"chrome://xulfx/skin/ic_security_black_24dp_2x.png\"); -moz-image-region: auto;",
             "oncommand", "PanelUI.showSubView('panelUI_panelviewSecurity', this, 'PanelUI-contents')",
             "closemenu", "none");
            panelUI_toolbaritemSecurity.AppendChild(panelUI_toolbarbuttonSecurity);
            //AddEventListenerFor(panelUI_toolbarbuttonSecurity, "command", PanelUIListenerState.Default, "panelUI_toolbarbuttonSecurity");

            //anonymity slideout\\

            //panel view for the settings options
            var panelUI_panelviewSecurity = CreateXULElement("panelview",
            "id", "panelUI_panelviewSecurity",
            "flex", "1",
            "class", "panel-subview-body",
            "flip", "slide",
            "animate", "true");
            PanelUImultiView.AppendChild(panelUI_panelviewSecurity);

            //label for the panel slideout
            var PanelUI_labelSecurity = CreateXULElement("label",
            "id", "PanelUI_labelSecurity",
            "class", "panel-subview-header",
            "value", "Browser Anonymity Settings");
            panelUI_panelviewSecurity.AppendChild(PanelUI_labelSecurity);

            //viewbox to hold the options
            var PanelUISub_vboxSecurity = CreateXULElement("vbox",
            "id", "PanelUISub_vboxSecurity",
            "class", "panel-subview-body");
            panelUI_panelviewSecurity.AppendChild(PanelUISub_vboxSecurity);

            //Anonimity Options\\

            //flash
            AddCBOptionTo("Flash", PanelUIListenerState.PanelUImenuitem_Flash.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetFlashEnabledPref.ToLowerString(),
                PanelUIListenerState.PanelUImenuitem_Flash, PanelUISub_vboxSecurity);

            //java
            //AddCBOptionTo("Java", PanelUIListenerState.PanelUImenuitem_Java.GetDescription(),
            //    BrowseoFXManager.Instance.SettingsHandler.JavaEnabled.ToLowerString(),
            //    PanelUIListenerState.PanelUImenuitem_Java, PanelUISub_vboxSecurity);

            //plugins
            AddCBOptionTo("Plugins", PanelUIListenerState.PanelUImenuitem_Plugins.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetPluginsEnabledPref.ToLowerString(),
                PanelUIListenerState.PanelUImenuitem_Plugins, PanelUISub_vboxSecurity);

            //webGL
            AddCBOptionTo("WebGL", PanelUIListenerState.PanelUImenuitem_WebGL.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetWebGLEnabledPref.ToLowerString(),
                 PanelUIListenerState.PanelUImenuitem_WebGL, PanelUISub_vboxSecurity);

            //webRTC
            AddCBOptionTo("WebRTC", PanelUIListenerState.PanelUImenuitem_WebRtc.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetWebRTCEnabledPref.ToLowerString(),
                PanelUIListenerState.PanelUImenuitem_WebRtc, PanelUISub_vboxSecurity);

            //DNT
            AddCBOptionTo("DNT", PanelUIListenerState.PanelUImenuitem_DNT.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetDoNotTrackEnabledPref.ToLowerString(),
                PanelUIListenerState.PanelUImenuitem_DNT, PanelUISub_vboxSecurity);

            //javascript
            AddCBOptionTo("Javascript", PanelUIListenerState.PanelUImenuitem_Javascript.GetDescription(),
                BrowseoFXManager.Instance.SettingsHandler.GetJavascriptEnabledPref.ToLowerString(),
                PanelUIListenerState.PanelUImenuitem_Javascript, PanelUISub_vboxSecurity);

            //useragent todo
            PanelUISub_vboxSecurity.AppendChild(
                CreateXULElement("label",
                "id", "useragentHeader",
                "class", "panel-subview-header",
                "value", "Common User Agents"));
            foreach (var uaStringValue in BrowseoFXManager.Instance.SettingsHandler.UserAgents)
            {
                AddCBOptionTo(uaStringValue, uaStringValue,
                    (uaStringValue == BrowseoFXManager.Instance.SettingsHandler.UserAgent_Current).ToLowerString(),
                    PanelUIListenerState.PanelUImenuitem_Useragent, PanelUISub_vboxSecurity);
            }
        }

        private void AddCBOptionTo(string label, string id,string isChecked, PanelUIListenerState paneluiListenerType, GeckoXULElement elementToAppend)
        {
            var PanelUISub_toolbarbutton = CreateXULElement("toolbarbutton",
            "id", id,
            "label", label,
            "checked", isChecked,
            "checkState",isChecked == "true"? "1" : "0",
            "tooltiptext", label,
            "type", "checkbox",
            "closemenu", "none",
            "class", "subviewbutton");
            AddEventListenerFor(PanelUISub_toolbarbutton, "command", paneluiListenerType, id);
            elementToAppend.AppendChild(PanelUISub_toolbarbutton);
        }

        private void AddEventListenerFor(GeckoXULElement xulElement, string eventName, PanelUIListenerState listenerType, string elementID)
        {
            Xpcom.QueryInterface<nsIDOMEventTarget>(xulElement.Instance).Wrap(GeckoDOMEventTarget.Create)
                .AddEventListener(eventName, new PanelUIListener(BrowseoFXManager.Instance.GloableWebView.Widget.View, BrowseoFXManager.Instance.GloableWebView.Widget.View.Document, listenerType, elementID, xulElement), false, true, 2);
        }

        #endregion

        public GeckoXULElement CreateXULElement(string elementType, params string[] properys)
        {
            var xulElement = (GeckoXULElement)BrowseoFXManager.Instance.GloableWebView.Widget.View.Document.CreateElementNS("http://www.mozilla.org/keymaster/gatekeeper/there.is.only.xul", elementType);
            for (int i = 0; i < properys.Length; i++)
            {
                xulElement.SetAttribute(properys[i], properys[++i]);
            }
            return xulElement;
        }
    }

    public class PanelUIListener : nsIDOMEventListener
    {
        private GeckoXULElement element;
        private GeckoDocument xulDocument;
        private GeckoWindow view;
        private PanelUIListenerState state;
        private string elementID = "";
        private bool firstClick = true;

        public PanelUIListener(GeckoWindow view, GeckoDocument document, PanelUIListenerState state, string elementID, GeckoXULElement element)
        {
            this.xulDocument = document;
            this.view = view;
            this.state = state;
            this.elementID = elementID;
            this.element = element;
        }

        public async void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create);

            switch (args.Type)
            {
                case "popupshown":
                    switch (state)
                    {
                        case PanelUIListenerState.PanelUIPopUp:
                            var fullscreenBtn = xulDocument.GetElementById("fullscreen-button") as GeckoXULElement;
                            if (!fullscreenBtn.Hidden)
                            {
                                fullscreenBtn.Hidden = true;
                            }
                            //var fullscreenBtn = xulDocument.GetElementById("fullscreen-button") as GeckoXULElement;
                            break;

                        default:
                            break;
                    }
                    break;

                case "command":
                    var sender = xulDocument.GetElementById(elementID) as GeckoXULElement;
                    if (sender != null)
                    {
                        var label = sender.GetAttribute("label");
                        //var id = sender.GetAttribute("id");
                        var isChecked = sender.GetAttribute("checked") == "true";
                        //var checkedVal = sender.GetAttribute("checked");
                        //var checkedState = sender.GetAttribute("checkState");

                        //TODO:clean better
                       // var setCheckedState = false;


                        switch (state)
                        {
                            case PanelUIListenerState.panelUI_toolbarbuttonTimeSync:
                                BrowseoFXManager.Instance.OpenTimeSwitchPage();
                                return;

                            case PanelUIListenerState.PanelUImenuitem_Flash:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetFlashEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_Java:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetJavaEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_Plugins:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetPluginsEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_WebGL:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetWebGLEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_WebRtc:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetWebRTCEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_DNT:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetDoNotTrackEnabledPref;
                                break;
                            case PanelUIListenerState.PanelUImenuitem_Javascript:
                                isChecked = !BrowseoFXManager.Instance.SettingsHandler.GetJavascriptEnabledPref;
                                break;

                            case PanelUIListenerState.PanelUImenuitem_TimeZone:
                            case PanelUIListenerState.PanelUImenuitem_Useragent:
                                sender.SetAttribute("checked", "true");
                                sender.SetAttribute("checkState", "1");

                                var listToUncheck = BrowseoFXManager.Instance.SettingsHandler.UserAgents;
                                var currentToCheck = label;
                                if (state == PanelUIListenerState.PanelUImenuitem_TimeZone)
                                {
                                    listToUncheck = BrowseoFXManager.Instance.SettingsHandler.TimeZones;
                                }

                                foreach (var itemValue in listToUncheck)
                                {
                                    if (itemValue == currentToCheck) continue;
                                    var otherUAoption = xulDocument.GetElementById(itemValue);
                                    otherUAoption.SetAttribute("checked", "false");
                                    otherUAoption.SetAttribute("checkState", "0");
                                }

                                break;

                            case PanelUIListenerState.Default:
                                break;

                            default:
                                break;
                        }

                        //firstClick = false;
                        //if (setCheckedState)
                        //{
                        //    isChecked = true;
                        //    sender.SetAttribute("checked", "true");
                        //    sender.SetAttribute("checkState", "1");
                        //}
                        BrowseoFXManager.Instance.SettingsHandler.SetPrefenceSettings(isChecked, state, label);
                       // await BrowseoFXManager.Instance.SettingsHandler.SavePreferenceSettingsAsync();
                        BrowseoFXManager.Instance.RefreshBrowserTabs();
                    }
                    break;

                case "TabClose":
                    args.StopPropagation();
                    args.StopImmediatePropagation();
                    args.PreventDefault();
                    break;

                default:
                    break;
            }
        }
    }
}
