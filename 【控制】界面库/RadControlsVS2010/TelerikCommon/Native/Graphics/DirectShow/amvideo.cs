using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From AMDDS_* defines
    /// </summary>
    [Flags]
    internal enum DirectDrawSwitches
    {
        None = 0x00,
        DCIPS = 0x01,
        PS = 0x02,
        RGBOVR = 0x04,
        YUVOVR = 0x08,
        RGBOFF = 0x10,
        YUVOFF = 0x20,
        RGBFLP = 0x40,
        YUVFLP = 0x80,
        All = 0xFF,
        YUV = (YUVOFF | YUVOVR | YUVFLP),
        RGB = (RGBOFF | RGBOVR | RGBFLP),
        Primary = (DCIPS | PS)
    }

    /// <summary>
    /// From AM_PROPERTY_FRAMESTEP
    /// </summary>
    internal enum PropertyFrameStep
    {
        Step   = 0x01,
        Cancel = 0x02,
        CanStep = 0x03,
        CanStepMultiple = 0x04
    }

    /// <summary>
    /// From AM_FRAMESTEP_STEP
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FrameStepStep
    {
        public int dwFramesToStep;
    }

    /// <summary>
    /// From MPEG1VIDEOINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MPEG1VideoInfo
    {
        public VideoInfoHeader hdr;
        public int dwStartTimeCode;
        public int           cbSequenceHeader;
        public byte            bSequenceHeader;
    }

    /// <summary>
    /// From ANALOGVIDEOINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AnalogVideoInfo
    {
        public Rectangle            rcSource;
        public Rectangle            rcTarget;
        public int            dwActiveWidth;
        public int            dwActiveHeight;
        public long  AvgTimePerFrame;
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport,
    Guid("36d39eb0-dd75-11ce-bf0e-00aa0055595a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDirectDrawVideo
    {
        [PreserveSig]
        int GetSwitches(out int pSwitches);

        [PreserveSig]
        int SetSwitches(int Switches);

        [PreserveSig]
        int GetCaps(out IntPtr pCaps); // DDCAPS

        [PreserveSig]
        int GetEmulatedCaps(out IntPtr pCaps); // DDCAPS

        [PreserveSig]
        int GetSurfaceDesc(out IntPtr pSurfaceDesc); // DDSURFACEDESC

        [PreserveSig]
        int GetFourCCCodes(out int pCount,out int pCodes);

        [PreserveSig]
        int SetDirectDraw(IntPtr pDirectDraw); // LPDIRECTDRAW

        [PreserveSig]
        int GetDirectDraw(out IntPtr ppDirectDraw); // LPDIRECTDRAW

        [PreserveSig]
        int GetSurfaceType(out DirectDrawSwitches pSurfaceType);

        [PreserveSig]
        int SetDefault();

        [PreserveSig]
        int UseScanLine(int UseScanLine);

        [PreserveSig]
        int CanUseScanLine(out int UseScanLine);

        [PreserveSig]
        int UseOverlayStretch(int UseOverlayStretch);

        [PreserveSig]
        int CanUseOverlayStretch(out int UseOverlayStretch);

        [PreserveSig]
        int UseWhenFullScreen(int UseWhenFullScreen);

        [PreserveSig]
        int WillUseFullScreen(out int UseWhenFullScreen);
    }

    [ComImport,
    Guid("dd1d7110-7836-11cf-bf47-00aa0055595a"),
    Obsolete("This interface has been deprecated.", false),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFullScreenVideo
    {
        [PreserveSig]
        int CountModes(out int pModes);

        [PreserveSig]
        int GetModeInfo(int Mode,out int pWidth,out int pHeight,out int pDepth);

        [PreserveSig]
        int GetCurrentMode(out int pMode);

        [PreserveSig]
        int IsModeAvailable(int Mode);

        [PreserveSig]
        int IsModeEnabled(int Mode);

        [PreserveSig]
        int SetEnabled(int Mode,int bEnabled);

        [PreserveSig]
        int GetClipFactor(out int pClipFactor);

        [PreserveSig]
        int SetClipFactor(int ClipFactor);

        [PreserveSig]
        int SetMessageDrain(IntPtr hwnd);

        [PreserveSig]
        int GetMessageDrain(out IntPtr hwnd);

        [PreserveSig]
        int SetMonitor(int Monitor);

        [PreserveSig]
        int GetMonitor(out int Monitor);

        [PreserveSig]
        int HideOnDeactivate(int Hide);

        [PreserveSig]
        int IsHideOnDeactivate();

        [PreserveSig]
        int SetCaption([MarshalAs(UnmanagedType.BStr)] string strCaption);

        [PreserveSig]
        int GetCaption([MarshalAs(UnmanagedType.BStr)] out string pstrCaption);

        [PreserveSig]
        int SetDefault();

    }

    [ComImport,
    Guid("53479470-f1dd-11cf-bc42-00aa00ac74f6"),
    Obsolete("This interface has been deprecated.", false),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFullScreenVideoEx : IFullScreenVideo
    {
        #region IFullScreenVideo methods

        [PreserveSig]
        new int CountModes(out int pModes);

        [PreserveSig]
        new int GetModeInfo(int Mode, out int pWidth, out int pHeight, out int pDepth);

        [PreserveSig]
        new int GetCurrentMode(out int pMode);

        [PreserveSig]
        new int IsModeAvailable(int Mode);

        [PreserveSig]
        new int IsModeEnabled(int Mode);

        [PreserveSig]
        new int SetEnabled(int Mode, int bEnabled);

        [PreserveSig]
        new int GetClipFactor(out int pClipFactor);

        [PreserveSig]
        new int SetClipFactor(int ClipFactor);

        [PreserveSig]
        new int SetMessageDrain(IntPtr hwnd);

        [PreserveSig]
        new int GetMessageDrain(out IntPtr hwnd);

        [PreserveSig]
        new int SetMonitor(int Monitor);

        [PreserveSig]
        new int GetMonitor(out int Monitor);

        [PreserveSig]
        new int HideOnDeactivate(int Hide);

        [PreserveSig]
        new int IsHideOnDeactivate();

        [PreserveSig]
        new int SetCaption([MarshalAs(UnmanagedType.BStr)] string strCaption);

        [PreserveSig]
        new int GetCaption([MarshalAs(UnmanagedType.BStr)] out string pstrCaption);

        [PreserveSig]
        new int SetDefault();
        #endregion

        [PreserveSig]
        int SetAcceleratorTable(IntPtr hwnd, IntPtr hAccel); // HACCEL

        [PreserveSig]
        int GetAcceleratorTable(out IntPtr phwnd, out IntPtr phAccel); // HACCEL

        [PreserveSig]
        int KeepPixelAspectRatio(int KeepAspect);

        [PreserveSig]
        int IsKeepPixelAspectRatio(out int pKeepAspect);

    }

    [ComImport,
    Guid("61ded640-e912-11ce-a099-00aa00479a58"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBaseVideoMixer
    {
        [PreserveSig]
        int SetLeadPin(int iPin);

        [PreserveSig]
        int GetLeadPin(out int piPin);

        [PreserveSig]
        int GetInputPinCount(out int piPinCount);

        [PreserveSig]
        int IsUsingClock(out int pbValue);

        [PreserveSig]
        int SetUsingClock(int bValue);

        [PreserveSig]
        int GetClockPeriod(out int pbValue);

        [PreserveSig]
        int SetClockPeriod(int bValue);
    }

#endif

    [ComImport,
    Guid("1bd0ecb0-f8e2-11ce-aac6-0020af0b99a3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQualProp
    {
        [PreserveSig]
        int get_FramesDroppedInRenderer(out int pcFrames);

        [PreserveSig]
        int get_FramesDrawn(out int pcFramesDrawn);

        [PreserveSig]
        int get_AvgFrameRate(out int piAvgFrameRate);

        [PreserveSig]
        int get_Jitter(out int iJitter);

        [PreserveSig]
        int get_AvgSyncOffset(out int piAvg);

        [PreserveSig]
        int get_DevSyncOffset(out int piDev);

    }

    #endregion
}
