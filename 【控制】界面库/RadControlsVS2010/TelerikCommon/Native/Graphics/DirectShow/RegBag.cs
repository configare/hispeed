using System;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Interfaces

    [ComImport,
    Guid("8A674B48-1F63-11d3-B64C-00C04F79498E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICreatePropBagOnRegKey
    {
        [PreserveSig]
        int Create(
            [In] IntPtr hkey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string subkey,
            [In] int ulOptions,
            [In] int samDesired,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [Out, MarshalAs(UnmanagedType.IUnknown)] out object ppBag
            );
    }

    #endregion
}
