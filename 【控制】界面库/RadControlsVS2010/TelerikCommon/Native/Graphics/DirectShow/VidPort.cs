using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES
    
    /// <summary>
    /// From AMVPSIZE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AMVPSize
    {
        public int           dwWidth;                // the width
        public int           dwHeight;               // the height
    }

    /// <summary>
    /// From DDVIDEOPORTCONNECT
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DDVideoPortConnect
    {
        public int dwSize;
        public int dwPortWidth;
        public Guid guidTypeID;
        public int dwFlags;
        public IntPtr dwReserved1;
    }

    /// <summary>
    /// From AMVPDATAINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct VPDataInfo
    {
        public int dwSize;
        public int dwMicrosecondsPerField;
        public AMVPDimInfo amvpDimInfo;
        public int dwPictAspectRatioX;
        public int dwPictAspectRatioY;
        public bool bEnableDoubleClock;
        public bool bEnableVACT;
        public bool bDataIsInterlaced;
        public int lHalfLinesOdd;
        public bool bFieldPolarityInverted;
        public int dwNumLinesInVREF;
        public int lHalfLinesEven;
        public int dwReserved1;
    }

    /// <summary>
    /// From AMVPDIMINFO
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct AMVPDimInfo
    {
        public int dwFieldWidth;
        public int dwFieldHeight;
        public int dwVBIWidth;
        public int dwVBIHeight;
        public Rectangle rcValidRegion;
    }

#endif

    /// <summary>
    /// From AMVP_MODE
    /// </summary>
    internal enum AMVP_Mode
    {
        Weave,
        BobInterleaved,
        BobNonInterleaved,
        SkipEven,
        SkipOdd
    }

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPBaseConfig
    {
        [PreserveSig]
        int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        int SetInvertPolarity(
            );

        [PreserveSig]
        int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );
    }

    [ComImport,
    Guid("BC29A660-30E3-11d0-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPConfig : IVPBaseConfig
    {
    #region IVPBaseConfig Methods

        [PreserveSig]
        new int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        new int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        new int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        new int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        new int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        new int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        new int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        new int SetInvertPolarity(
            );

        [PreserveSig]
        new int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        new int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        new int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        new int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        new int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );

        #endregion

        [PreserveSig]
        int IsVPDecimationAllowed(
            [MarshalAs(UnmanagedType.Bool)] out bool pbIsDecimationAllowed
            );

        [PreserveSig]
        int SetScalingFactors(
            AMVPSize pamvpSize
            );
    }

    [ComImport,
    Guid("EC529B00-1A1F-11D1-BAD9-00609744111A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPVBIConfig : IVPBaseConfig
    {
    #region IVPBaseConfig Methods

        [PreserveSig]
        new int GetConnectInfo(
            out int pdwNumConnectInfo,
            out DDVideoPortConnect pddVPConnectInfo
            );

        [PreserveSig]
        new int SetConnectInfo(
            int dwChosenEntry
            );

        [PreserveSig]
        new int GetVPDataInfo(
            out VPDataInfo pamvpDataInfo
            );

        [PreserveSig]
        new int GetMaxPixelRate(
            out AMVPSize pamvpSize,
            out int pdwMaxPixelsPerSecond
            );

        [PreserveSig]
        new int InformVPInputFormats(
            int dwNumFormats,
            DDPixelFormat pDDPixelFormats
            );

        [PreserveSig]
        new int GetVideoFormats(
            out int pdwNumFormats,
            out DDPixelFormat pddPixelFormats
            );

        [PreserveSig]
        new int SetVideoFormat(
            int dwChosenEntry
            );

        [PreserveSig]
        new int SetInvertPolarity(
            );

        [PreserveSig]
        new int GetOverlaySurface(
            out IntPtr ppddOverlaySurface // IDirectDrawSurface
            );

        [PreserveSig]
        new int SetDirectDrawKernelHandle(
            IntPtr dwDDKernelHandle
            );

        [PreserveSig]
        new int SetVideoPortID(
            int dwVideoPortID
            );

        [PreserveSig]
        new int SetDDSurfaceKernelHandles(
            int cHandles,
            IntPtr rgDDKernelHandles
            );

        [PreserveSig]
        new int SetSurfaceParameters(
            int dwPitch,
            int dwXOrigin,
            int dwYOrigin
            );

        #endregion

    }

    [ComImport,
    Guid("EC529B01-1A1F-11D1-BAD9-00609744111A"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPVBINotify : IVPBaseNotify
    {
    #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion

    }

#endif

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPBaseNotify
    {
        [PreserveSig]
        int RenegotiateVPParameters();
    }

    [ComImport,
    Guid("C76794A1-D6C5-11d0-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPNotify : IVPBaseNotify
    {
        #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion

        [PreserveSig]
        int SetDeinterlaceMode(
            AMVP_Mode mode
            );

        [PreserveSig]
        int GetDeinterlaceMode(
            out AMVP_Mode pMode
            );
    }

    [ComImport,
    Guid("EBF47183-8764-11d1-9E69-00C04FD7C15B"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IVPNotify2 : IVPNotify
    {
        #region IVPBaseNotify

        [PreserveSig]
        new int RenegotiateVPParameters();

        #endregion

        #region IVPNotify Methods

        [PreserveSig]
        new int SetDeinterlaceMode(
            AMVP_Mode mode
            );

        [PreserveSig]
        new int GetDeinterlaceMode(
            out AMVP_Mode pMode
            );

        #endregion

        [PreserveSig]
        int SetVPSyncMaster(
            [MarshalAs(UnmanagedType.Bool)] bool bVPSyncMaster
            );

        [PreserveSig]
        int GetVPSyncMaster(
            [MarshalAs(UnmanagedType.Bool)] out bool pbVPSyncMaster
            );

    }

    #endregion
}
