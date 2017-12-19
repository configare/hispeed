using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.Permissions;
using System.Diagnostics;

namespace Telerik.WinControls
{

    public class TelerikPaintHelper
    {
        public static void CopyImageFromGraphics(Graphics graphics, Bitmap destinationImage)
        {
            Graphics grfxBitmap = Graphics.FromImage(destinationImage);
            IntPtr hdcScreen = graphics.GetHdc();
            IntPtr hdcBitmap = grfxBitmap.GetHdc();
            NativeMethods.BitBlt(hdcBitmap, 0, 0, destinationImage.Width, destinationImage.Height, hdcScreen, 0, 0, NativeMethods.SRCCOPY);
            grfxBitmap.ReleaseHdc(hdcBitmap);
            graphics.ReleaseHdc(hdcScreen);
        }

        public static IntPtr CreateHalftoneBrush()
        {
            short[] numArray1 = new short[8];
            for (int num1 = 0; num1 < 8; num1++)
            {
                numArray1[num1] = (short)(0x5555 << ((num1 & 1) & 0x1f));
            }
            IntPtr ptr1 = NativeMethods.CreateBitmap(8, 8, 1, 1, numArray1);
            NativeMethods.LOGBRUSH logbrush1 = new NativeMethods.LOGBRUSH();
            logbrush1.lbColor = ColorTranslator.ToWin32(Color.Black);
            logbrush1.lbStyle = 3;
            logbrush1.lbHatch = ptr1;
            IntPtr ptr2 = NativeMethods.CreateBrushIndirect(logbrush1);
            NativeMethods.DeleteObject(new HandleRef(null, ptr1));
            return ptr2;
        }

        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        public static void DrawHalftoneLine(Rectangle rectangle1)
        {
            IntPtr ptr1 = NativeMethods.GetDesktopWindow();
            DrawHalftoneLine(ptr1, rectangle1, null);
        }

        public static void DrawHalftoneLine(Control canvasControl, Rectangle rectangle1)
        {
            IntPtr ptr1 = canvasControl.Handle;
            DrawHalftoneLine(ptr1, rectangle1, canvasControl);
        }

        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        public static void DrawHalftoneLine(IntPtr windowHandle, Rectangle rectangle1, Control managedBase)
        {
            IntPtr ptr1 = NativeMethods.GetDCEx(new HandleRef(managedBase, windowHandle), NativeMethods.NullHandleRef, 0x402);
            IntPtr ptr2 = TelerikPaintHelper.CreateHalftoneBrush();
            IntPtr ptr3 = NativeMethods.SelectObject(new HandleRef(managedBase, ptr1), new HandleRef(null, ptr2));
            NativeMethods.PatBlt(new HandleRef(managedBase, ptr1), rectangle1.X, rectangle1.Y, rectangle1.Width, rectangle1.Height, 0x5a0049);
            NativeMethods.SelectObject(new HandleRef(managedBase, ptr1), new HandleRef(null, ptr3));
            NativeMethods.DeleteObject(new HandleRef(null, ptr2));
            NativeMethods.ReleaseDC(new HandleRef(managedBase, windowHandle), new HandleRef(null, ptr1));
        }

        public static void DrawGlowingText(Graphics graphics, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            if (Application.RenderWithVisualStyles)
            {
                DWMAPI.DrawTextOnGlass(graphics, text, font, bounds, color, flags, DWMAPI.DTT_GLOWSIZE);
            }
        }
    
        public static bool IsCompositionEnabled()
        {
            return DWMAPI.IsCompositionEnabled;
        }
    }
}