using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Gecko.DOM;
using Gecko;
using Gecko.Interop;

namespace BrowseoFX_WPF.Core.BrowserListeners.Base
{
    public abstract class nsDOMEventListenerBase : nsIDOMEventListener
    {
        private string eventName;
        private GeckoDOMEventTarget eventTarget;
        private GeckoXULElement xulElement;

        public string EventName
        {
            get
            {
                return eventName;
            }
        }
        public GeckoXULElement XulElement
        {
            get
            {
                return xulElement;
            }
        }
        public GeckoDOMEventTarget EventTarget
        {
            get
            {
                if (eventTarget == null) eventTarget = Xpcom.QueryInterface<nsIDOMEventTarget>(XulElement.Instance).Wrap(GeckoDOMEventTarget.Create);
                return eventTarget;
            }
        }


        public nsDOMEventListenerBase(GeckoXULElement xulElement, string eventName)
        {
            this.xulElement = xulElement;
            this.eventName = eventName;
            AddEventListener();
        }

        public void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create);
            OnHandleGeckoDomEvent(args);
        }

        public abstract void OnHandleGeckoDomEvent(GeckoDOMEventArgs args);

        public void AddEventListener()
        {
            EventTarget.AddEventListener(eventName, this, false, true, 2);
        }
    }
}
