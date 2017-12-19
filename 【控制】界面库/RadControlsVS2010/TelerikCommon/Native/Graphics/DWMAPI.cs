using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    [SuppressUnmanagedCodeSecurity]
    internal class DWMAPI
    {
        public const int DTT_COMPOSITED = 8192;
        public const int DTT_GLOWSIZE = 2048;
        public const int DTT_TEXTCOLOR = 1;
        public const int BPBF_TOPDOWNDIB = 2;

        [DllImport("dwmapi.dll")]
        public static extern void DwmIsCompositionEnabled(ref bool isEnabled);

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref NativeMethods.MARGINS pMargins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr result);

        [DllImport("dwmapi.dll")]
        public static extern bool DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWMBLURBEHIND blurInfo);

        public static void DrawTextOnGlass(Graphics graphics, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags, int rflags)
        {
            IntPtr primaryHdc = graphics.GetHdc();

            IntPtr memoryHdc = NativeMethods.CreateCompatibleDC(new HandleRef(null, primaryHdc));

            NativeMethods.BITMAPINFO info = new NativeMethods.BITMAPINFO();
            info.bmiHeader_biSize = Marshal.SizeOf(info);
            info.bmiHeader_biWidth = bounds.Width;
            info.bmiHeader_biHeight = -bounds.Height;
            info.bmiHeader_biPlanes = 1;
            info.bmiHeader_biBitCount = 32;
            info.bmiHeader_biCompression = 0;

            IntPtr ppbBits = IntPtr.Zero;

            IntPtr dib = NativeMethods.CreateDIBSection(primaryHdc, info, (uint)0, out ppbBits, IntPtr.Zero, (uint)0);

            NativeMethods.SelectObject(new HandleRef(null, memoryHdc), new HandleRef(null, dib));


            IntPtr fontHandle = font.ToHfont();
            NativeMethods.SelectObject(new HandleRef(null, memoryHdc), new HandleRef(null, fontHandle));


            System.Windows.Forms.VisualStyles.VisualStyleRenderer renderer = new System.Windows.Forms.VisualStyles.VisualStyleRenderer(System.Windows.Forms.VisualStyles.VisualStyleElement.Window.Caption.Active);
            DWMAPI.DTTOPTS dttOpts = new DWMAPI.DTTOPTS();
            dttOpts.dwSize = Marshal.SizeOf(typeof(DWMAPI.DTTOPTS));
            dttOpts.dwFlags = DWMAPI.DTT_COMPOSITED | rflags | DWMAPI.DTT_TEXTCOLOR;
            dttOpts.crText = ColorTranslator.ToWin32(color);
            dttOpts.iGlowSize = 10;
            NativeMethods.RECT textBounds = new NativeMethods.RECT(0, 0, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
            UXTheme.DrawThemeTextEx(renderer.Handle, memoryHdc, 0, 0, text, -1, (int)flags, ref textBounds, ref dttOpts);

            const int SRCCOPY = 0x00CC0020;
            NativeMethods.BitBlt(primaryHdc, bounds.Left, bounds.Top, bounds.Width, bounds.Height, memoryHdc, 0, 0, SRCCOPY);


            NativeMethods.DeleteObject(new HandleRef(null, fontHandle));
            NativeMethods.DeleteObject(new HandleRef(null, dib));
            NativeMethods.DeleteDC(new HandleRef(null, memoryHdc));

            graphics.ReleaseHdc(primaryHdc);
        }

        public static bool IsVista
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        public static bool IsCompositionEnabled
        {
            get
            {
                if (!IsVista)
                {
                    return false;
                }
                bool enabled = false;
                DwmIsCompositionEnabled(ref enabled);
                return enabled;
            }
        }

        public static bool IsAlphaBlendingSupported
        {
            get
            {
                if (Environment.OSVersion.Version.Major >= 5 && // Windows 2000 and up
                    System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]).ToLower() != "msaccess") // If not in MSAccess environment
                    return true;
                else
                    return false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DTTOPTS
        {
            public int dwSize;
            public int dwFlags;
            public int crText;
            public int crBorder;
            public int crShadow;
            public int iTextShadowType;
            public NativeMethods.POINT ptShadowOffset;
            public int iBorderSize;
            public int iFontPropId;
            public int iColorPropId;
            public int iStateId;
            public bool fApplyOverlay;
            public int iGlowSize;
            public int pfnDrawTextCallback;
            public IntPtr lParam;
        }

        public struct DWMBLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }
    }
}
