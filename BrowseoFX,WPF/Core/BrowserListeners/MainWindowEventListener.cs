using BrowseoFX_WPF.Core.BrowserListeners.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko.DOM;
using BrowseoFX_WPF.Core.DataAccess;
using Gecko.Interfaces;
using System.Runtime.InteropServices;
using Gecko;

namespace BrowseoFX_WPF.Core.BrowserListeners
{
    public class MainWindowEventListener : nsDOMEventListenerBase
    {
        public MainWindowListenerStates State { get; set; }

        public MainWindowEventListener(MainWindowListenerStates state, GeckoXULElement xulElement, string eventName):
            base(xulElement, eventName)
        {
            State = state;
        }

        public override void OnHandleGeckoDomEvent(GeckoDOMEventArgs args)
        {

            switch (args.Type)
            {
                case "dblclick":
                    args.StopPropagation();
                    args.StopImmediatePropagation();
                    args.PreventDefault();
                    break;

                case "onload":

                    break;

                default:
                    break;
            }
            //switch (State)
            //{
            //    case MainWindowListenerStates.Default:
            //        break;

            //    case MainWindowListenerStates.mainwindow_onload:
            //        break;

            //    case MainWindowListenerStates.mainwindow_dblclick:
            //        break;

            //    default:
            //        break;
            //}
        }
    }
}
