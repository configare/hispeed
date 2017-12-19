using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using System.Security;

namespace Telerik.WinControls
{
    [SuppressUnmanagedCodeSecurity]
    internal class UXTheme
    {
        private const string NativeDll = "uxtheme.dll";

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenThemeData(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszClassList);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern int CloseThemeData(IntPtr hTheme);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern int SetWindowTheme(IntPtr handle, string windowName, string ids);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref NativeMethods.RECT rect, IntPtr clip);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern bool IsThemePartDefined(IntPtr hTheme, int iPartId, int iStateId);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

        [DllImport(NativeDll, CharSet = CharSet.Auto)]
        public static extern int GetThemePartSize(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, IntPtr rect, ThemeSizeType eSize, [Out] NativeMethods.SIZE psz);

        [DllImport(NativeDll)]
        public static extern IntPtr BeginBufferedPaint(IntPtr pHdcTarget, IntPtr lpRect, IntPtr bufferFormat, IntPtr bpPaintParams, ref IntPtr pHdc);

        [DllImport(NativeDll)]
        public static extern IntPtr BufferedPaintSetAlpha(IntPtr hBufferedPaint, IntPtr prc, byte alpha);

        [DllImport(NativeDll)]
        public static extern IntPtr EndBufferedPaint(IntPtr hBufferedPaint, IntPtr fUpdateTarget);

        [DllImport(NativeDll, CharSet = CharSet.Unicode)]
        public static extern int DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, string text, int iCharCount, int dwFlags, ref NativeMethods.RECT pRect, ref DWMAPI.DTTOPTS pOptions);
    }
}
