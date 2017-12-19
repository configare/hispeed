using System;
using System.Runtime.InteropServices;

#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From KSMULTIPLE_ITEM - Note that data is returned in the memory IMMEDIATELY following this struct.
    /// The Size parm indicates ths size of the KSMultipleItem plus the extra bytes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class KSMultipleItem
    {
        public int Size;
        public int Count;
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("00000109-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersistStream : IPersist
    {
        #region IPersist Methods

        [PreserveSig]
        new int GetClassID([Out] out Guid pClassID);

        #endregion

        [PreserveSig]
        int IsDirty();

        [PreserveSig]
#if USING_NET11
        int Load([In] UCOMIStream pStm);
#else
        int Load([In] IStream pStm);
#endif

        [PreserveSig]
        int Save(
#if USING_NET11
            [In] UCOMIStream pStm,
#else
            [In] IStream pStm,
#endif
            [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

        [PreserveSig]
        int GetSizeMax([Out] out long pcbSize);
    }

    [ComImport,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPersist
    {
        [PreserveSig]
        int GetClassID([Out] out Guid pClassID);
    }

    [ComImport,
    Guid("b61178d1-a2d9-11cf-9e53-00aa00a216a1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKsPin
    {
        /// <summary>
        /// The caller must free the returned structures, using the CoTaskMemFree function
        /// </summary>
        [PreserveSig]
        int KsQueryMediums(
            out IntPtr ip);
    }

    [ComImport,
    Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISpecifyPropertyPages
    {
        [PreserveSig]
        int GetPages(out DsCAUUID pPages);
    }

    [ComImport,
    Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyBag
    {
        [PreserveSig]
        int Read(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
            [In] IErrorLog pErrorLog
            );

        [PreserveSig]
        int Write(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
            [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
            );
    }

    [ComImport,
    Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IErrorLog
    {
        [PreserveSig]
        int AddError(
            [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
#if USING_NET11
            [In] EXCEPINFO pExcepInfo);
#else
            [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
#endif
    }

    #endregion
}
