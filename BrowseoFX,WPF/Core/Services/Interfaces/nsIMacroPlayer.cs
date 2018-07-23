namespace BrowseoFX_WPF.Core.Services.Interfaces
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Runtime.CompilerServices;
    using Gecko;
    using Gecko.Interfaces;

    [ComImport()]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("a58332c8-79a7-4cd6-ace7-813e43e207e1")]
    public interface nsIMacroPlayer
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void setVariableMessage([MarshalAs(UnmanagedType.LPStr)] string action, [MarshalAs(UnmanagedType.LPStr)] string message);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void playMacro([MarshalAs(UnmanagedType.Interface)]nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.Interface)] nsIDOMDocument aDocument, [MarshalAs(UnmanagedType.LPStr)] string text);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void macroDone([MarshalAs(UnmanagedType.Interface)]nsIDOMWindow aWindow, [MarshalAs(UnmanagedType.Interface)] nsIDOMDocument aDocument);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void setHandler([MarshalAs(UnmanagedType.Interface)]nsICommandHandler aHandler);
    }
}
