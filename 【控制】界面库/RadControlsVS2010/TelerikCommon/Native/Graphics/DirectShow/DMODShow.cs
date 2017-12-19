using System;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Interfaces

    [ComImport,
    Guid("52d6f586-9f0f-4824-8fc8-e32ca04930c2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDMOWrapperFilter
    {
        [PreserveSig]
        int Init(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid clsidDMO,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid catDMO
            );
    }

    #endregion
}
