using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Gecko;
using Gecko.Interop;
using BrowseoFX_WPF.Core.Base;
using Gecko.DOM;

namespace BrowseoFX_WPF.Core.BrowserListeners.Base
{
    public class BasicEventListenerBase :   
        nsIDOMEventListener
    {
        public virtual void OnHandleGeckoDomEvent(GeckoDOMEventArgs args) { }

        public void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create);
            OnHandleGeckoDomEvent(args);
        }

        public void AddListener(object element, string eventName)
        {
            GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(element).Wrap(GeckoDOMEventTarget.Create);
            eventTarget.AddEventListener(eventName, this, false, true, 2);
        }

        public void AddListener(object element, List<string> events)
        {
            GeckoDOMEventTarget eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(element).Wrap(GeckoDOMEventTarget.Create);
            foreach (var eventName in events)
            {
                eventTarget.AddEventListener(eventName, this, false, true, 2);
            }
        }

        //internal void AddAllEventListeners(string elementName, GeckoDocument contentDocument, List<string> events)
        //{

        //}
    }
}
