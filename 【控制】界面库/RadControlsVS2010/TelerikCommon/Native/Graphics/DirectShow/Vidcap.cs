using System;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Multimedia
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    /// <summary>
    /// From KSTOPOLOGY_CONNECTION
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KSTopologyConnection
    {
        public int FromNode;
        public int FromNodePin;
        public int ToNode;
        public int ToNodePin;
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport,
    Guid("720D4AC0-7533-11D0-A5D6-28DB04C10000"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKsTopologyInfo
    {
        [PreserveSig]
        int get_NumCategories(
            [Out] out int pdwNumCategories
            );

        [PreserveSig]
        int get_Category(
            [In] int dwIndex,
            [Out] out Guid pCategory
            );

        [PreserveSig]
        int get_NumConnections(
            [Out] out int pdwNumConnections
            );

        [PreserveSig]
        int get_ConnectionInfo(
            [In] int dwIndex,
            [Out] out KSTopologyConnection pConnectionInfo
            );

        [PreserveSig]
        int get_NodeName(
            [In] int dwNodeId,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string pwchNodeName,
            [In] int dwBufSize,
            [Out] out int pdwNameLen
            );

        [PreserveSig]
        int get_NumNodes(
            [Out] out int pdwNumNodes
            );

        [PreserveSig]
        int get_NodeType(
            [In] int dwNodeId,
            [Out] out Guid pNodeType
            );

        [PreserveSig]
        int CreateNodeInstance( 
            [In] int dwNodeId, 
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid iid, 
            [Out, MarshalAs(UnmanagedType.IUnknown)] out Object ppvObject  
            ); 
    }

    [ComImport,
    Guid("1ABDAECA-68B6-4F83-9371-B413907C7B9F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISelector
    {
        [PreserveSig]
        int get_NumSources([Out] out int pdwNumSources);

        [PreserveSig]
        int get_SourceNodeId([Out] out int pdwPinId);

        [PreserveSig]
        int put_SourceNodeId([In] int dwPinId);
    }

    [ComImport,
    Guid("11737C14-24A7-4bb5-81A0-0D003813B0C4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKsNodeControl
    {
        [PreserveSig]
        int put_NodeId([In] int dwNodeId);

        [PreserveSig]
        int put_KsControl([In] IntPtr pKsControl); // PVOID
    }

#endif

    #endregion
}
