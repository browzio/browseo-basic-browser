using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.DataAccess
{
    public enum PanelUIListenerState
    {
        [Description("Default")]
        Default,

        [Description("PanelUI-popup")]
        PanelUIPopUp,

        [Description("panelUI_toolbarbuttonTimeSync")]
        panelUI_toolbarbuttonTimeSync,

        [Description("PanelUISub_toolbarbutton_flash")]
        PanelUImenuitem_Flash,

        [Description("PanelUISub_toolbarbutton_java")]
        PanelUImenuitem_Java,

        [Description("PanelUISub_toolbarbutton_plugins")]
        PanelUImenuitem_Plugins,

        [Description("PanelUISub_toolbarbutton_webgl")]
        PanelUImenuitem_WebGL,

        [Description("PanelUISub_toolbarbutton_webrtc")]
        PanelUImenuitem_WebRtc,

        [Description("PanelUISub_toolbarbutton_dnt")]
        PanelUImenuitem_DNT,

        [Description("PanelUISub_toolbarbutton_javascript")]
        PanelUImenuitem_Javascript,

        [Description("PanelUISub_toolbarbutton_useragent")]
        PanelUImenuitem_Useragent,

        [Description("PanelUISub_toolbarbutton_timeZone")]
        PanelUImenuitem_TimeZone,
    }

    public enum NavBarListenerStates
    {
        [Description("Default")]
        Default,

        [Description("navBar_toolbarbutton_BrowseoIA")]
        navbar_ButtonBrowseoIA,

        [Description("navBar_toolbarlitem_FormFiller")]
        navbar_ButtonFormFiller,

        [Description("navBar_toolbarbutton_CP")]
        navbar_ButtonCP,

        [Description("navbar_DominateAll")]
        navbar_DominateAll,

        [Description("navbar_FbConverseo")]
        navbar_FbConverseo,

        [Description("navbar_LSB")]
        navbar_LSB,

        [Description("navbar_SEO")]
        navbar_SEO,

        [Description("navbar_BirdsEye")]
        navbar_BirdsEye,

        [Description("navbar_Button_SocialStats")]
        navbar_Button_SocialStats,

        [Description("panelUi_ctlSK_CP")]
        panelUi_ctlSK_CP,

        [Description("panelUi_ctlSK_AcceptFriends")]
        panelUi_ctlSK_AcceptFriends,

        [Description("panelUi_ctlSK_LikePages")]
        panelUi_ctlSK_LikePages,

        [Description("panelUi_ctlSK_LikeGroups")]
        panelUi_ctlSK_LikeGroups,

        [Description("panelUi_ctlSK_LikePosts")]
        panelUi_ctlSK_LikePosts,
    }


    public enum ContextMenuListenerStates
    {
        [Description("Default")]
        Default,

        [Description("menuitem_ToSocialEngager")]
        menuitem_ToSocialEngager,

        [Description("menuitem_curaste")]
        menuitem_curaste,

        [Description("menuitem_curate")]
        menuitem_curate,
    }

    public enum MainWindowListenerStates
    {
        [Description("Default")]
        Default,

        [Description("main-window")]
        mainwindow_onload,

        [Description("main-window")]
        mainwindow_dblclick,
    }
}
