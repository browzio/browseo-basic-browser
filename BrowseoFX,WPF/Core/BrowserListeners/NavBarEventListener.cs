using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using BrowseoFX_WPF.Core.DataAccess;
using Gecko;
using Gecko.DOM;
using Gecko.Interop;
using BrowseoFX_WPF.Core.BrowserListeners.Base;
using WindowsInput.Native;
using Browseo.SocialBirdEye.ViewModels;
using System.Diagnostics;

namespace BrowseoFX_WPF.Core.BrowserListeners
{
    public class NavBarEventListener : nsDOMEventListenerBase
    {
        public NavBarListenerStates State { get; set; }

        public NavBarEventListener(NavBarListenerStates state, GeckoXULElement xulElement, string eventName):
            base(xulElement, eventName)
        {
            State = state;
        }

        public override void OnHandleGeckoDomEvent(GeckoDOMEventArgs args)
        {
            switch (args.Type)
            {
                case "command":
                    switch (State)
                    {
                        case NavBarListenerStates.Default:
                            break;

                        case NavBarListenerStates.navbar_ButtonBrowseoIA:
                            BrowseoFXManager.Instance.OpenIAMacros();
                            break;

                        case NavBarListenerStates.navbar_ButtonCP:
                            BrowseoFXManager.Instance.OpenCP();
                            break;

                        case NavBarListenerStates.navbar_Button_SocialStats:
                            BrowseoFXManager.Instance.OpenSocialStats();
                            break;

                        case NavBarListenerStates.navbar_ButtonFormFiller:
                            BrowseoFXManager.Instance.TabbrowserHandler.FillForm(args.CurrentTarget.CastToGeckoElement().GetAttribute("label"));
                            //BrowseoFXManager.Instance.TabbrowserHandler.OpenCP();
                            break;

                        case NavBarListenerStates.navbar_DominateAll:
                            if (RanTutLink("https://browz.io/fbconverseo-lite", BrowseoFXManager.Instance.PanelUIHandler.isEnabledForFree)) return;
                            BrowseoFXManager.Instance.TabbrowserHandler.DominateAll();
                            break;

                        case NavBarListenerStates.navbar_FbConverseo:
                            if (RanTutLink("https://browz.io/fbconverseo-lite", BrowseoFXManager.Instance.PanelUIHandler.isEnabledForFree)) return;
                            BrowseoFXManager.Instance.OpenFbConverseo();
                            break;

                        case NavBarListenerStates.navbar_LSB:
                            if (RanTutLink("https://browz.io/lsb-lite", BrowseoFXManager.Instance.PanelUIHandler.isEnabledForFree)) return;
                            BrowseoFXManager.Instance.OpenLSB();
                            break;


                        case NavBarListenerStates.navbar_SEO:
                            if (RanTutLink("https://browz.io/seo-lite", BrowseoFXManager.Instance.PanelUIHandler.isEnabledForFree)) return;
                            BrowseoFXManager.Instance.OpenSEO();
                            break;

                        case NavBarListenerStates.navbar_BirdsEye:
                            BirdsEyeDashboardViewModel.Instance.ShowWindow();
                            break;

                        case NavBarListenerStates.panelUi_ctlSK_CP:
                            if (RanTutLink("https://browz.io/consoleaccess", BrowseoFXManager.Instance.PanelUIHandler.isEnabledForFree) ||
                                RanTutLink("https://browz.io/consoleaccess", BrowseoFXManager.Instance.PanelUIHandler.IsEnabledForKK)) return;
                            System.Windows.Forms.SendKeys.SendWait("^+k");
                            break;

                        case NavBarListenerStates.panelUi_ctlSK_AcceptFriends:
                            // System.Windows.Forms.SendKeys.SendWait("javascript:var inputs = document.getElementsByClassName('_42ft 4jy0 FriendRequestAdd addButton 4jy3 _517h')); for(var i=0; i<inputs.length;i++) { inputs[i].click(); }");
                            Autotype("javascript:var inputs = document.getElementsByClassName('_42ft _4jy0 FriendRequestAdd addButton _4jy3 _517h _51sy'); for(var i=0; i<inputs.length;i++) { inputs[i].click(); }");
                            break;

                        case NavBarListenerStates.panelUi_ctlSK_LikePages:
                            Autotype("javascript:var inputs = document.getElementsByClassName('_42ft _4jy0 PageLikeButton _4jy3 _517h _51sy'); for(var i=0; i<inputs.length;i++) { inputs[i].click(); }");
                            break;

                        case NavBarListenerStates.panelUi_ctlSK_LikeGroups:
                            Autotype("javascript:var inputs = document.getElementsByClassName('_42ft _4jy0 _4jy3 _517h _51sy'); for(var i=0; i<inputs.length;i++) { inputs[i].click(); }");
                            break;

                        case NavBarListenerStates.panelUi_ctlSK_LikePosts:
                            Autotype("javascript:var inputs = document.getElementsByClassName('UFILikeLink _4x9- _4x9_ _48-k'); for(var i=0; i<inputs.length;i++) { inputs[i].click(); }");
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
        private bool RanTutLink(string tutUrl, bool trueForFalse)
        {
            if (trueForFalse) return false;
            else
            {
                Process.Start(tutUrl);
                return true;
            }
        }

        public void Autotype(string text)
        {
            var sim = new WindowsInput.InputSimulator();
            sim.Keyboard.Sleep(500)
               .TextEntry(text)
               .Sleep(1000)
               .KeyPress(VirtualKeyCode.RETURN);
        }
    }
}
