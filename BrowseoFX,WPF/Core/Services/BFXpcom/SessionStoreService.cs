using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko;
using Gecko.CustomMarshalers;
using SpiderMonkey;
using System.Runtime.InteropServices;
using Gecko.Interop;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class SessionStoreService : nsISessionStore,
        nsIMessageListener,
        nsISupportsWeakReference,
        nsIObserver,
        nsIDOMEventListener
    {

        #region Initialize Instance
        public SessionStoreService()
        {
            _sessionStore = Xpcom.GetService<Gecko.Interfaces.nsISessionStore>("@mozilla.org/browser/sessionstore;1");
            foreach (var topic in Obserinvg)
            {
                FXServices.ObserverService.AddObserver(this, topic, true);
            }
        }
        #endregion

        #region supports weak ref
        private GeckoWeakReference _weakRef;
        public GeckoWeakReference WeakReference
        {
            get { return _weakRef ?? (_weakRef = new GeckoWeakReference(this)); }
            protected set { _weakRef = value; }
        }
        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIWeakReference GetWeakReference()
        {
            return WeakReference;
        }
        #endregion

        private static nsISessionStore _sessionStore;
        public static nsISessionStore SessionStore
        {
            get
            {
                if (_sessionStore == null)
                    _sessionStore = new SessionStoreService();
                return _sessionStore;
            }
        }


        #region nsIDOMEventListener
        // These are tab events that we listen to.
        public string[] TabEvents = {
          "TabOpen", "TabBrowserInserted", "TabClose", "TabSelect", "TabShow", "TabHide", "TabPinned", "TabUnpinned",
          "XULFrameLoaderCreated"
        };

        public void HandleEvent([MarshalAs(UnmanagedType.Interface)] nsIDOMEvent @event)
        {
            using (var args = Xpcom.QueryInterface<nsIDOMEvent>(@event).Wrap(GeckoDOMEventArgs.Create))
            {

                var evntListener = Xpcom.QueryInterface<nsIDOMEventListener>(SessionStore);
                evntListener.HandleEvent(@event);

                //let win = aEvent.currentTarget.ownerGlobal;
                //let target = aEvent.originalTarget;
                switch (args.Type)
                {
                    case "TabOpen":
                        //evntListener.HandleEvent(@event);
                        // this.onTabAdd(win);
                        break;
                    case "TabBrowserInserted":
                        // var evntL = Xpcom.QueryInterface<nsIDOMEventListener>(SessionStore);
                        evntListener.HandleEvent(@event);
                        //this.onTabBrowserInserted(win, target);
                        break;
                    case "TabClose":
                        // `adoptedBy` will be set if the tab was closed because it is being
                        // moved to a new window.
                        //  if (!aEvent.detail.adoptedBy)
                        // this.onTabClose(win, target);
                        //  this.onTabRemove(win, target);
                        //evntListener.HandleEvent(@event);
                        break;
                    case "TabSelect":
                        //evntListener.HandleEvent(@event);
                        // this.onTabSelect(win);
                        break;
                    case "TabShow":
                        //  this.onTabShow(win, target);
                        break;
                    case "TabHide":
                        //  this.onTabHide(win, target);
                        break;
                    case "TabPinned":
                    case "TabUnpinned":
                    case "SwapDocShells":
                        // this.saveStateDelayed(win);
                        break;
                    case "oop-browser-crashed":
                        // this.onBrowserCrashed(target);
                        break;
                    case "XULFrameLoaderCreated":
                        //if (target.namespaceURI == NS_XUL &&
                        //    target.localName == "browser" &&
                        //    target.frameLoader &&
                        //    target.permanentKey)
                        //{
                        //    this._lastKnownFrameLoader.set(target.permanentKey, target.frameLoader);
                        //    this.resetEpoch(target);
                        //}
                        break;
                    default:
                        break;
                        //throw new Error(`unhandled event ${ aEvent.type}?`);
                }
            }
            
  //  this._clearRestoringWindows();
    }


        #endregion

        #region messages
        // Messages that will be received via the Frame Message Manager.
        public string[] Messages = {
          // The content script sends us data that has been invalidated and needs to
          // be saved to disk.
          "SessionStore:update",

          // The restoreHistory code has run. This is a good time to run SSTabRestoring.
          "SessionStore:restoreHistoryComplete",

          // The load for the restoring tab has begun. We update the URL bar at this
          // time; if we did it before, the load would overwrite it.
          "SessionStore:restoreTabContentStarted",

          // All network loads for a restoring tab are done, so we should
          // consider restoring another tab in the queue. The document has
          // been restored, and forms have been filled. We trigger
          // SSTabRestored at this time.
          "SessionStore:restoreTabContentComplete",

          // A crashed tab was revived by navigating to a different page. Remove its
          // browser from the list of crashed browsers to stop ignoring its messages.
          "SessionStore:crashedTabRevived",

          // The content script encountered an error.
          "SessionStore:error",
        };

        // The list of messages we accept from <xul:browser>s that have no tab
        // assigned, or whose windows have gone away. Those are for example the
        // ones that preload about:newtab pages, or from browsers where the window
        // has just been closed.
        public string[] NOTAB_MESSAGES = {
          // For a description see above.
          "SessionStore:crashedTabRevived",

          // For a description see above.
          "SessionStore:update",

          // For a description see above.
          "SessionStore:error",
        };

        // The list of messages we accept without an "epoch" parameter.
        // See getCurrentEpoch() and friends to find out what an "epoch" is.
        public string[] NOEPOCH_MESSAGES = {
          // For a description see above.
          "SessionStore:crashedTabRevived",

          // For a description see above.
          "SessionStore:error",
        };

        // The list of messages we want to receive even during the short period after a
        // frame has been removed from the DOM and before its frame script has finished
        // unloading.
        public string[] CLOSED_MESSAGES = {
          // For a description see above.
          "SessionStore:crashedTabRevived",

          // For a description see above.
          "SessionStore:update",

          // For a description see above.
          "SessionStore:error",
        };

        public void ReceiveMessage()
        {
            throw new NotImplementedException();
        }

        #endregion




        #region nsIObserver
        // global notifications observed
        public string[] Obserinvg = {
          "browser-window-before-show", "domwindowclosed",
          "quit-application-granted", "browser-lastwindow-close-granted",
          "quit-application", "browser:purge-session-history",
          "browser:purge-domain-data",
          "idle-daily",
        };
        public void Observe([MarshalAs(UnmanagedType.Interface)] nsISupports aSubject, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aTopic, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(WStringMarshaler))] string aData)
        {
            var obs = Xpcom.QueryInterface<nsIObserver>(SessionStore);
            obs.Observe(aSubject, aTopic, aData);

            //switch (aTopic)
            //{
            //    case "browser-window-before-show": // catch new windows
            //        var obs = Xpcom.QueryInterface<nsIObserver>(SessionStore);
            //        obs.Observe(aSubject, aTopic, aData);
            //        break;
            //    case "domwindowclosed": // catch closed windows
            //       // this.onClose(aSubject);
            //        break;
            //    case "quit-application-granted":
            //      //  let syncShutdown = aData == "syncShutdown";
            //      //  this.onQuitApplicationGranted(syncShutdown);
            //        break;
            //    case "browser-lastwindow-close-granted":
            //      //  this.onLastWindowCloseGranted();
            //        break;
            //    case "quit-application":
            //      //  this.onQuitApplication(aData);
            //        break;
            //    case "browser:purge-session-history": // catch sanitization
            //      //  this.onPurgeSessionHistory();
            //        break;
            //    case "browser:purge-domain-data":
            //       // this.onPurgeDomainData(aData);
            //        break;
            //    case "nsPref:changed": // catch pref changes
            //       // this.onPrefChange(aData);
            //        break;
            //    case "idle-daily":
            //       // this.onIdleDaily();
            //        break;
            //}
        }
        #endregion




        #region nsISessionStore
        public void DeleteGlobalValue([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey)
        {
            throw new NotImplementedException();
        }

        public void DeleteTabValue([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey)
        {
            throw new NotImplementedException();
        }

        public void DeleteWindowValue([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIDOMNode DuplicateTab([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, int aDelta)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIDOMNode ForgetClosedTab([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, uint aIndex)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIDOMNode ForgetClosedWindow(uint aIndex)
        {
            throw new NotImplementedException();
        }

        public void GetBrowserState([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            //using(var res = new nsAString())
            //{
            //    SessionStore.GetBrowserState(res);
            //    var resString = res.ToString();
                
            //}
            SessionStore.GetBrowserState(result);
            //throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public bool GetCanRestoreLastSessionAttribute()
        {
            throw new NotImplementedException();
        }

        public uint GetClosedTabCount([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow)
        {
            throw new NotImplementedException();
        }

        public void GetClosedTabData([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public uint GetClosedWindowCount()
        {
            throw new NotImplementedException();
        }

        public void GetClosedWindowData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void GetGlobalValue([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void GetTabState([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void GetTabValue([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void GetWindowState([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void GetWindowValue([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase result)
        {
            throw new NotImplementedException();
        }

        public void PersistTabAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aName)
        {
            throw new NotImplementedException();
        }

        public void RestoreLastSession()
        {
            throw new NotImplementedException();
        }

        public void SetBrowserState([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aState)
        {
            throw new NotImplementedException();
        }

        public void SetCanRestoreLastSessionAttribute([MarshalAs(UnmanagedType.U1)] bool value)
        {
            throw new NotImplementedException();
        }

        public void SetGlobalValue([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, ref JSVal aStringValue)
        {
            throw new NotImplementedException();
        }

        public void SetTabState([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aState)
        {
            throw new NotImplementedException();
        }

        public void SetTabValue([MarshalAs(UnmanagedType.Interface)] nsIDOMNode aTab, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, ref JSVal aStringValue)
        {
            throw new NotImplementedException();
        }

        public void SetWindowState([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aState, [MarshalAs(UnmanagedType.U1)] bool aOverwrite)
        {
            throw new NotImplementedException();
        }

        public void SetWindowValue([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(AStringMarshaler))] nsAStringBase aKey, ref JSVal aStringValue)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIDOMNode UndoCloseTab([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aWindow, uint aIndex)
        {
            throw new NotImplementedException();
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public nsIDOMWindow UndoCloseWindow(uint aIndex)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
