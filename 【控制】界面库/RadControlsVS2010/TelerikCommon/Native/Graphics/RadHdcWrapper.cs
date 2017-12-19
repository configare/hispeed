using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a wrapper for native GDI HDC handle.
    /// Transforms a GDI+ graphics to its HDC counterpart, preserving any clipping and transformations applied.
    /// </summary>
    public class RadHdcWrapper
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="useTransfrom"></param>
        public RadHdcWrapper(Graphics g, bool useTransfrom)
        {
            this.graphics = g;

            Region rg = g.Clip;
            this.clipRegion = rg.GetHrgn(g);
            rg.Dispose();

            this.useTransform = useTransfrom;
            if (this.useTransform)
            {
                Matrix m = g.Transform;
                transform = XFORM.FromMatrix(m);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IntPtr GetHdc()
        {
            this.hdc = this.graphics.GetHdc();
            this.dcState = SaveDC(this.hdc);

            if (this.useTransform)
            {
                this.graphicsMode = SetGraphicsMode(this.hdc, GM_ADVANCED);
                GetWorldTransform(this.hdc, ref this.oldTransform);
                ModifyWorldTransform(this.hdc, ref this.transform, MWT_LEFTMULTIPLY);
            }

            if (this.clipRegion != IntPtr.Zero)
            {
                this.origRegion = CreateRectRgn(0, 0, 0, 0);
                int result = GetClipRgn(this.hdc, this.origRegion);

                if (result == 1)
                {
                    CombineRgn(this.clipRegion, this.origRegion, this.clipRegion, 1);
                }

                SelectClipRgn(this.hdc, this.clipRegion);
            }

            return this.hdc;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (this.hdc == IntPtr.Zero)
                return;

            RestoreDC(this.hdc, this.dcState);

            if (this.useTransform)
            {
                SetGraphicsMode(this.hdc, this.graphicsMode);
                SetWorldTransform(this.hdc, ref this.oldTransform);
            }

            if (this.clipRegion != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(new HandleRef(null, this.clipRegion));
                NativeMethods.DeleteObject(new HandleRef(null, this.origRegion));
            }

            this.graphics.ReleaseHdc(this.hdc);

            this.graphics = null;
            this.hdc = IntPtr.Zero;
            this.clipRegion = IntPtr.Zero;
            this.origRegion = IntPtr.Zero;
        }


        #endregion

        #region Fields

        private IntPtr hdc;
        private IntPtr clipRegion;
        private IntPtr origRegion;
        private int dcState;
        private int graphicsMode;
        private bool useTransform;
        private XFORM transform;
        private XFORM oldTransform;

        private Graphics graphics;

        #endregion

        #region Interop

        private const string NativeLibrary = "gdi32";
        private const int GM_COMPATIBLE = 1;
        private const int GM_ADVANCED = 2;
        private const int MWT_IDENTITY = 1;
        private const int MWT_LEFTMULTIPLY = 2;
        private const int MWT_RIGHTMULTIPLY = 3;

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int SaveDC(IntPtr hDC);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int SetGraphicsMode(IntPtr hdc, int iMode);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int GetWorldTransform(IntPtr hdc, ref XFORM xForm);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int SetWorldTransform(IntPtr hdc, ref XFORM xForm);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int ModifyWorldTransform(IntPtr hdc, ref XFORM xForm, int mode);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateRectRgn(int X1, int Y1, int X2, int Y2);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int GetClipRgn(IntPtr hDC, IntPtr hrgn);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int CombineRgn(IntPtr hDestRgn, IntPtr hSrcRgn1, IntPtr hSrcRgn2, int nCombineMode);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn);

        [DllImport(NativeLibrary, CharSet = CharSet.Auto)]
        private static extern IntPtr RestoreDC(IntPtr hDC, int savedDC);

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct XFORM
        {
            public XFORM(float m11, float m12, float m21, float m22, float dx, float dy)
            {
                eM11 = m11;
                eM12 = m12;
                eM21 = m21;
                eM22 = m22;
                eDx = dx;
                eDy = dy;
            }

            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;

            public static XFORM FromMatrix(Matrix m)
            {
                float[] elements = m.Elements;
                float m11 = elements[0];
                float m12 = elements[1];
                float m21 = elements[2];
                float m22 = elements[3];
                float dx = elements[4];
                float dy = elements[5];

                return new XFORM(m11, m12, m21, m22, dx, dy);
            }

            public static XFORM Identity = new XFORM(1, 0, 0, 1, 0, 0);
        }

        #endregion
    }
}
