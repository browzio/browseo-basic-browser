using Gecko;
using Gecko.CustomMarshalers;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class nsObserverAny : nsIObserver
    {
        public void Observe([MarshalAs(UnmanagedType.Interface)] nsISupports aSubject, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aTopic, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(WStringMarshaler))] string aData)
        {
            Console.WriteLine(aTopic);

            switch (aTopic)
            {
                case "browser-delayed-startup-finished":
                 
                    break;

                case "browserClassInitializer_12345":
                    break;

                case "xul-window-registered":
                    break;

                default:
                    break;
            }
        }
    }
}
