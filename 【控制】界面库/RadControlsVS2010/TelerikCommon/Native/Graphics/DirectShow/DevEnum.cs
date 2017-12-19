using System;
using System.Runtime.InteropServices;

#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From CDEF_CLASS_* defines
    /// </summary>
    [Flags]
    internal enum CDef
    {
        None = 0,
        ClassDefault = 0x0001,
        BypassClassManager = 0x0002,
        ClassLegacy = 0x0004,
        MeritAboveDoNotUse = 0x0008,
        DevmonCMGRDevice = 0x0010,
        DevmonDMO = 0x0020,
        DevmonPNPDevice = 0x0040,
        DevmonFilter = 0x0080,
        DevmonSelectiveMask = 0x00f0
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
#if USING_NET11
            [Out] out UCOMIEnumMoniker ppEnumMoniker,
#else
            [Out] out IEnumMoniker ppEnumMoniker,
#endif
            [In] CDef dwFlags);
    }

    #endregion
}
