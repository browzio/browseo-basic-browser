using BrowseoFX_WPF.Core.Base;
using Gecko;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services
{
    public class FXServices //: SingletonBase<BrowseoFXServicesManager>
    {
        private static BFXpcom.BFXDirectoryServiceProvider _directoryServiceprovider;
        public  static BFXpcom.BFXDirectoryServiceProvider DirectoryServiceProvider
        {
            get
            {
                if (_directoryServiceprovider == null) _directoryServiceprovider = new BFXpcom.BFXDirectoryServiceProvider();
                return _directoryServiceprovider;
            }
        }
        public static void RegisterDirectoryService()
        {
            using (var directoryService = Xpcom.GetService2<nsIDirectoryService>(Gecko.Contracts.DirectoryService))
            {
                if (directoryService != null)
                    directoryService.Instance.RegisterProvider(DirectoryServiceProvider);
            }
        }

        private static BFXpcom.SessionStoreService _sessionStoreService;
        public static BFXpcom.SessionStoreService SessionStoreService
        {
            get
            {
                if (_sessionStoreService == null) _sessionStoreService = new BFXpcom.SessionStoreService();
                return _sessionStoreService;
            }
        }

        private static BFXpcom.BFXPreferencesService _preferencesService;
        public static BFXpcom.BFXPreferencesService PreferencesService
        {
            get
            {
                if (_preferencesService == null) _preferencesService = new BFXpcom.BFXPreferencesService();
                return _preferencesService;
            }
        }
        

        //private static Browseo.BrowserProfilesManagerService _browserProfilesManagerService;
        //public  static Browseo.BrowserProfilesManagerService BrowserProfilesManagerService
        //{
        //    get
        //    {
        //        if (_browserProfilesManagerService == null) _browserProfilesManagerService = new Browseo.BrowserProfilesManagerService();
        //        return _browserProfilesManagerService;
        //    }
        //}

        private static nsIObserverService observerService;
        public static nsIObserverService ObserverService
        {
            get
            {
                if (observerService == null)
                {
                    observerService = Xpcom.GetService<Gecko.Interfaces.nsIObserverService>(Gecko.Contracts.ObserverService);
                }
                return observerService;
            }
        }


        private static BFXpcom.BFXWindowCreator _windowCreator;
        public static BFXpcom.BFXWindowCreator WindowCreator
        {
            get
            {
                if (_windowCreator == null) _windowCreator = new BFXpcom.BFXWindowCreator();
                return _windowCreator;
            }
        }

        private static BFXpcom.BFXMacroPlayerService _bFXMacroPlayerService;
        public static BFXpcom.BFXMacroPlayerService BFXMacroPlayerService
        {
            get
            {
                if (_bFXMacroPlayerService == null) _bFXMacroPlayerService = new BFXpcom.BFXMacroPlayerService();
                return _bFXMacroPlayerService;
            }
        }
    }
}
