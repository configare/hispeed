using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

    /// <summary>
    /// From AMINTERLACE_*
    /// </summary>
    [Flags]
    internal enum AMInterlace
    {
        None = 0,
        IsInterlaced          = 0x00000001,
        OneFieldPerSample     = 0x00000002,
        Field1First           = 0x00000004,
        Unused                = 0x00000008,
        FieldPatternMask      = 0x00000030,
        FieldPatField1Only    = 0x00000000,
        FieldPatField2Only    = 0x00000010,
        FieldPatBothRegular   = 0x00000020,
        FieldPatBothIrregular = 0x00000030,
        DisplayModeMask       = 0x000000c0,
        DisplayModeBobOnly    = 0x00000000,
        DisplayModeWeaveOnly  = 0x00000040,
        DisplayModeBobOrWeave = 0x00000080,
    }

    /// <summary>
    /// From AMCOPYPROTECT_*
    /// </summary>
    internal enum AMCopyProtect
    {
        None = 0,
        RestrictDuplication = 0x00000001
    }

    /// <summary>
    /// From AMCONTROL_*
    /// </summary>
    [Flags]
    internal enum AMControl
    {
        None = 0,
        Used      = 0x00000001,
        PadTo4x3  = 0x00000002,
        PadTo16x9 = 0x00000004,
    }

    /// <summary>
    /// From VIDEOINFOHEADER
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class VideoInfoHeader
    {
        public DsRect SrcRect;
        public DsRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;
    }

    /// <summary>
    /// From VIDEOINFOHEADER2
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class VideoInfoHeader2
    {
        public DsRect SrcRect;
        public DsRect TargetRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public AMInterlace InterlaceFlags;
        public AMCopyProtect CopyProtectFlags;
        public int PictAspectRatioX;
        public int PictAspectRatioY;
        public AMControl ControlFlags;
        public int Reserved2;
        public BitmapInfoHeader BmiHeader;
    }

    /// <summary>
    /// From WAVEFORMATEX
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack=2)]
    internal class WaveFormatEx
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;
    }

    #endregion

    #region Interfaces

    [ComImport,
    Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabber
    {
        [PreserveSig]
        int SetOneShot(
            [In, MarshalAs(UnmanagedType.Bool)] bool OneShot);

        [PreserveSig]
        int SetMediaType(
            [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int GetConnectedMediaType(
            [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

        [PreserveSig]
        int SetBufferSamples(
            [In, MarshalAs(UnmanagedType.Bool)] bool BufferThem);

        [PreserveSig]
        int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);

        [PreserveSig]
        int GetCurrentSample(out IMediaSample ppSample);

        [PreserveSig]
        int SetCallback(ISampleGrabberCB pCallback, int WhichMethodToCallback);
    }

    [ComImport,
    Guid("0579154A-2B53-4994-B0D0-E773148EFF85"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabberCB
    {
        /// <summary>
        /// When called, callee must release pSample
        /// </summary>
        [PreserveSig]
        int SampleCB(double SampleTime, IMediaSample pSample);

        [PreserveSig]
        int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen);
    }

    #endregion
}
