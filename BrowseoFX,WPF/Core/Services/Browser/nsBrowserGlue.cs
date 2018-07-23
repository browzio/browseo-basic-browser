using Gecko;
using Gecko.CustomMarshalers;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.Browser
{
    public class nsBrowserGlue :
        nsIBrowserGlue,
        nsIObserver,
        nsISupportsWeakReference

    {
        private static nsBrowserGlue _instancs;
        public static nsBrowserGlue Instance
        {
            get
            {
                if (_instancs == null) _instancs = new nsBrowserGlue();
                return _instancs;
            }
        }


        private nsBrowserGlue()
        {
            RegisterObservers();
        }



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


        public void RegisterObservers()
        {
            var os = FXServices.ObserverService;
            os.AddObserver(this, "notifications-open-settings", false);
            os.AddObserver(this, "prefservice:after-app-defaults", false);
            os.AddObserver(this, "final-ui-startup", false);
            os.AddObserver(this, "browser-delayed-startup-finished", false);
            os.AddObserver(this, "sessionstore-windows-restored", false);
            os.AddObserver(this, "browser:purge-session-history", false);
            os.AddObserver(this, "quit-application-requested", false);
            os.AddObserver(this, "quit-application-granted", false);
            //if (OBSERVE_LASTWINDOW_CLOSE_TOPICS)
            //{
            //    os.addObserver(this, "browser-lastwindow-close-requested", false);
            //    os.addObserver(this, "browser-lastwindow-close-granted", false);
            //}
            os.AddObserver(this, "weave:service:ready", false);
            os.AddObserver(this, "fxaccounts:onverified", false);
            os.AddObserver(this, "fxaccounts:device_disconnected", false);
            os.AddObserver(this, "weave:engine:clients:display-uris", false);
            os.AddObserver(this, "session-save", false);
            os.AddObserver(this, "places-init-complete", false);
            //this._isPlacesInitObserver = true;
            os.AddObserver(this, "places-database-locked", false);
            // this._isPlacesLockedObserver = true;
            os.AddObserver(this, "distribution-customization-complete", false);
            os.AddObserver(this, "handle-xul-text-link", false);
            os.AddObserver(this, "profile-before-change", false);
            //if (AppConstants.MOZ_TELEMETRY_REPORTING)
            //{
            //    os.addObserver(this, "keyword-search", false);
            //}
            os.AddObserver(this, "browser-search-engine-modified", false);
            os.AddObserver(this, "restart-in-safe-mode", false);
            os.AddObserver(this, "flash-plugin-hang", false);
            os.AddObserver(this, "xpi-signature-changed", false);
            os.AddObserver(this, "autocomplete-did-enter-text", false);

            //if (AppConstants.NIGHTLY_BUILD)
            //{
            //    os.addObserver(this, AddonWatcher.TOPIC_SLOW_ADDON_DETECTED, false);
            //}

            //this._flashHangCount = 0;
            //this._firstWindowReady = new Promise(resolve => this._firstWindowLoaded = resolve);

            //if (AppConstants.platform == "win" ||
            //    AppConstants.platform == "macosx")
            //{
            //    // Handles prompting to inform about incompatibilites when accessibility
            //    // and e10s are active together.
            //    E10SAccessibilityCheck.init();
            //}
        }

        public void Observe([MarshalAs(UnmanagedType.Interface)] nsISupports aSubject, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aTopic, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(WStringMarshaler))] string aData)
        {
            switch (aTopic)
            {
                case "places-init-complete":
                case "places-database-locked":
                case "distribution-customization-complete":
                case "browser-delayed-startup-finished":
                case "quit-application-granted":
                    FXServices.ObserverService.RemoveObserver(this, aTopic);
                    break;

                case "profile-before-change":
                    // Any component depending on Places should be finalized in
                    // _onPlacesShutdown.  Any component that doesn't need to act after
                    // the UI has gone should be finalized in _onQuitApplicationGranted.
                    dispose();
                    break;

                default:
                    break;
            }

            using (var browserClassInitializer = Xpcom.GetService2<Gecko.Interfaces.nsISupports>("@mozilla.browseo/browser/browserClassInitializer;1"))
            {
                    var obs = browserClassInitializer.QueryInterface<nsIObserver>();
                    obs.Observe(aSubject, aTopic, aData);
            }
        }

        private void dispose()
        {
        }

        public void Sanitize([MarshalAs(UnmanagedType.Interface)] nsIDOMWindow aParentWindow)
        {

        }
    }
}
