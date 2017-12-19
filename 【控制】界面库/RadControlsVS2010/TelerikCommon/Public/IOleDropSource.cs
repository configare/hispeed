using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
	[ComImport, Guid("00000121-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleDropSource
	{
		[PreserveSig]
		int OleQueryContinueDrag(int fEscapePressed, [In, MarshalAs(UnmanagedType.U4)] int grfKeyState);
		[PreserveSig]
		int OleGiveFeedback([In, MarshalAs(UnmanagedType.U4)] int dwEffect);
	}
}