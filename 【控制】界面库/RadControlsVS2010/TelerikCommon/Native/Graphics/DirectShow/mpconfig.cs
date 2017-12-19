using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From DDCOLOR_* defines
    /// </summary>
    [Flags]
    internal enum DDColor
    {
        None =                  0x00000000,
        Brightness =            0x00000001,
        Contrast =              0x00000002,
        Hue =                   0x00000004,
        Saturation =            0x00000008,
        Sharpness =             0x00000010,
        Gamma =                 0x00000020,
        ColorEnable =           0x00000040
    }

    /// <summary>
    /// From DDCOLORCONTROL
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class DDColorControl
    {
        public int  dwSize;
        public DDColor  dwFlags;
        public int  lBrightness;
        public int  lContrast;
        public int  lHue;
        public int  lSaturation;
        public int  lSharpness;
        public int  lGamma;
        public int  lColorEnable;
        public int  dwReserved1;
    }

    internal enum AspectRatioMode
    {
        Stretched,
        LetterBox,
        Crop,
        StretchedAsPrimary
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("EBF47182-8764-11d1-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMixerPinConfig2 : IMixerPinConfig
    {
        #region IMixerPinConfig Methods

        [PreserveSig]
        new int SetRelativePosition(
            int dwLeft,
            int dwTop,
            int dwRight,
            int dwBottom);

        [PreserveSig]
        new int GetRelativePosition(
            out int pdwLeft,
            out int pdwTop,
            out int pdwRight,
            out int pdwBottom
            );

        [PreserveSig]
        new int SetZOrder(
            int dwZOrder
            );

        [PreserveSig]
        new int GetZOrder(
            out int pdwZOrder
            );

        [PreserveSig]
        new int SetColorKey(
            [MarshalAs(UnmanagedType.LPStruct)]ColorKey pColorKey
            );

        [PreserveSig]
        new int GetColorKey(
            [Out, MarshalAs(UnmanagedType.LPStruct)] ColorKey pColorKey,
            out int pColor
            );

        [PreserveSig]
        new int SetBlendingParameter(
            int dwBlendingParameter
            );

        [PreserveSig]
        new int GetBlendingParameter(
            out int pdwBlendingParameter
            );

        [PreserveSig]
        new int SetAspectRatioMode(
            AspectRatioMode amAspectRatioMode
            );

        [PreserveSig]
        new int GetAspectRatioMode(
            out AspectRatioMode pamAspectRatioMode
            );

        [PreserveSig]
        new int SetStreamTransparent(
            [In, MarshalAs(UnmanagedType.Bool)] bool bStreamTransparent
            );

        [PreserveSig]
        new int GetStreamTransparent(
            [Out, MarshalAs(UnmanagedType.Bool)] out bool pbStreamTransparent
            );

        #endregion

        [PreserveSig]
        int SetOverlaySurfaceColorControls(
            DDColorControl pColorControl
            );

        [PreserveSig]
        int GetOverlaySurfaceColorControls(
            DDColorControl pColorControl
            );
    }

    [ComImport,
    Guid("593CDDE1-0759-11d1-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMixerPinConfig
    {
        [PreserveSig]
        int SetRelativePosition(
            int dwLeft,
            int dwTop,
            int dwRight,
            int dwBottom);

        [PreserveSig]
        int GetRelativePosition(
            out int pdwLeft,
            out int pdwTop,
            out int pdwRight,
            out int pdwBottom
            );

        [PreserveSig]
        int SetZOrder(
            int dwZOrder
            );

        [PreserveSig]
        int GetZOrder(
            out int pdwZOrder
            );

        [PreserveSig]
        int SetColorKey(
            [MarshalAs(UnmanagedType.LPStruct)] ColorKey pColorKey
            );

        [PreserveSig]
        int GetColorKey(
            [Out, MarshalAs(UnmanagedType.LPStruct)] ColorKey pColorKey,
            out int pColor
            );

        [PreserveSig]
        int SetBlendingParameter(
            int dwBlendingParameter
            );

        [PreserveSig]
        int GetBlendingParameter(
            out int pdwBlendingParameter
            );

        [PreserveSig]
        int SetAspectRatioMode(
            AspectRatioMode amAspectRatioMode
            );

        [PreserveSig]
        int GetAspectRatioMode(
            out AspectRatioMode pamAspectRatioMode
            );

        [PreserveSig]
        int SetStreamTransparent(
            [In, MarshalAs(UnmanagedType.Bool)] bool bStreamTransparent
            );

        [PreserveSig]
        int GetStreamTransparent(
            [Out, MarshalAs(UnmanagedType.Bool)] out bool pbStreamTransparent
            );
    }

    #endregion
}
