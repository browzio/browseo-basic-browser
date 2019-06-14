using Gecko;
using Gecko.CustomMarshalers;
using Gecko.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrowseoFX_WPF.Core.Services.BFXpcom
{
    public class nsObserverFingerPrint : nsIObserver
    {
        public void Observe([MarshalAs(UnmanagedType.Interface)] nsISupports aSubject, 
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string aTopic,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(WStringMarshaler))] string aData)
        {
            Console.WriteLine(aData);

            switch (aTopic)
            {
                case "browseoFX-fingerprint-current":
                    //MessageBox.Show(aData);
                    break;

                default:
                    break;
            }
        }
    }
}
