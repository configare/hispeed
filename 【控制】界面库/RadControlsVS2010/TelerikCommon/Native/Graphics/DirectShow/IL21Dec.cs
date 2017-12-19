using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From AM_LINE21_CCLEVEL
    /// </summary>
    internal enum AMLine21CCLevel
    {
        TC2 = 0,
    }

    /// <summary>
    /// From AM_LINE21_CCSERVICE
    /// </summary>
    internal enum AMLine21CCService
    {
        None = 0,
        Caption1,
        Caption2,
        Text1,
        Text2,
        XDS,
        DefChannel = 10,
        Invalid
    }

    /// <summary>
    /// From AM_LINE21_CCSTATE
    /// </summary>
    internal enum AMLine21CCState
    {
        Off = 0,
        On
    }

    /// <summary>
    /// From AM_LINE21_DRAWBGMODE
    /// </summary>
    internal enum AMLine21DrawBGMode
    {
        Opaque,
        Transparent
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("6E8D4A21-310C-11d0-B79A-00AA003767A7"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAMLine21Decoder
    {
        [PreserveSig]
        int GetDecoderLevel([Out] out AMLine21CCLevel lpLevel);

        [PreserveSig]
        int GetCurrentService([Out] out AMLine21CCService lpService);

        [PreserveSig]
        int SetCurrentService([In] AMLine21CCService Service);

        [PreserveSig]
        int GetServiceState([Out] out AMLine21CCState lpState);

        [PreserveSig]
        int SetServiceState([In] AMLine21CCState State);

        [PreserveSig]
        int GetOutputFormat([Out] BitmapInfoHeader lpbmih);

        [PreserveSig]
        int SetOutputFormat([In] BitmapInfoHeader lpbmih);

        [PreserveSig]
        int GetBackgroundColor([Out] out int pdwPhysColor);

        [PreserveSig]
        int SetBackgroundColor([In] int dwPhysColor);

        [PreserveSig]
        int GetRedrawAlways([Out, MarshalAs(UnmanagedType.Bool)] out bool lpbOption);

        [PreserveSig]
        int SetRedrawAlways([In, MarshalAs(UnmanagedType.Bool)] bool bOption);

        [PreserveSig]
        int GetDrawBackgroundMode([Out] out AMLine21DrawBGMode lpMode);

        [PreserveSig]
        int SetDrawBackgroundMode([In] AMLine21DrawBGMode Mode);
    }

    #endregion
}
