using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From MIXER_DATA_* defines
    /// </summary>
    [Flags]
    internal enum MixerData
    {
        None = 0,
        AspectRatio = 0x00000001, // picture aspect ratio changed
        NativeSize = 0x00000002, // native size of video changed
        Palette = 0x00000004 // palette of video changed
    }

    /// <summary>
    /// #define MIXER_STATE_* defines
    /// </summary>
    internal enum MixerState
    {
        Mask = 0x00000003, // use this mask with state status bits
        Unconnected = 0x00000000, // mixer is unconnected and stopped
        ConnectedStopped = 0x00000001, // mixer is connected and stopped
        ConnectedPaused = 0x00000002, // mixer is connected and paused
        ConnectedPlaying = 0x00000003 // mixer is connected and playing
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("81A3BD31-DEE1-11d1-8508-00A0C91F9CA0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMixerOCXNotify
    {
        [PreserveSig]
        int OnInvalidateRect([In] DsRect lpcRect);

        [PreserveSig]
        int OnStatusChange([In] MixerState ulStatusFlags);

        [PreserveSig]
        int OnDataChange([In] MixerData ulDataFlags);
    }

    [ComImport,
    Guid("81A3BD32-DEE1-11d1-8508-00A0C91F9CA0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMixerOCX
    {
        [PreserveSig]
        int OnDisplayChange(
            [In] int ulBitsPerPixel,
            [In] int ulScreenWidth,
            [In] int ulScreenHeight
            );

        [PreserveSig]
        int GetAspectRatio(
            [Out] out int pdwPictAspectRatioX,
            [Out] out int pdwPictAspectRatioY
            );

        [PreserveSig]
        int GetVideoSize(
            [Out] out int pdwVideoWidth,
            [Out] out int pdwVideoHeight
            );

        [PreserveSig]
        int GetStatus([Out] out int pdwStatus);

        [PreserveSig]
        int OnDraw(
            [In] IntPtr hdcDraw, // HDC
            [In] DsRect prcDraw
            );

        [PreserveSig]
        int SetDrawRegion(
            // While in theory this takes an LPPOINT, in practice
            // it must be NULL.
            [In] IntPtr lpptTopLeftSC,
            [In] DsRect prcDrawCC,
            [In] DsRect lprcClip
            );

        [PreserveSig]
        int Advise([In] IMixerOCXNotify pmdns);

        [PreserveSig]
        int UnAdvise();
    }

    #endregion
}
