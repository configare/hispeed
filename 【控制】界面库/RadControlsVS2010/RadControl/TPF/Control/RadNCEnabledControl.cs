using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// This class represents a <see cref="RadControl"/> that allows for non-client area modification and paiting.
    /// </summary>
    [ToolboxItem(false)]
    public class RadNCEnabledControl : RadControl
    {
        #region Properties

        protected virtual bool EnableNCPainting
        {
            get
            {
                return false;
            }
        }

        protected virtual bool EnableNCModification
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region WndProc handling

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCPAINT:
                    this.OnWMNCPaint(ref m);
                    break;
                case NativeMethods.WM_NCCALCSIZE:
                    this.OnWMNCCalcSize(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void OnWMNCPaint(ref System.Windows.Forms.Message m)
        {
            if (!this.EnableNCPainting)
            {
                base.WndProc(ref m);
                return;
            }

            if (!this.IsHandleCreated)
            {
                return;
            }

            HandleRef hWnd = new HandleRef(this, this.Handle);
            NativeMethods.RECT windowRect = new NativeMethods.RECT();
            if (NativeMethods.GetWindowRect(hWnd, ref windowRect) == false)
                return;

            Rectangle bounds = new Rectangle(0, 0,
                windowRect.right - windowRect.left,
                windowRect.bottom - windowRect.top);

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return;


            IntPtr hDC = IntPtr.Zero;
            int getDCEXFlags = NativeMethods.DCX_WINDOW | NativeMethods.DCX_CACHE | NativeMethods.DCX_CLIPSIBLINGS;
            IntPtr hRegion = IntPtr.Zero;

            if (m.WParam != (IntPtr)1)
            {
                getDCEXFlags |= NativeMethods.DCX_INTERSECTRGN;
                hRegion = m.WParam;
            }
            HandleRef windowHRgnRef = new HandleRef(this, hRegion);
            hDC = NativeMethods.GetDCEx(hWnd, windowHRgnRef,
                   getDCEXFlags);
            try
            {
                if (hDC != IntPtr.Zero)
                {
                    using (Graphics drawingSurface = Graphics.FromHdc(hDC))
                    {
                        this.OnNCPaint(drawingSurface);
                    }
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(new HandleRef(this, m.HWnd), new HandleRef(null, hDC));
            }
        }

        private void OnWMNCCalcSize(ref System.Windows.Forms.Message m)
        {
            if (!this.EnableNCModification)
            {
                base.WndProc(ref m);
                return;
            }

            if (m.WParam == new IntPtr(1))
            {
                NativeMethods.NCCALCSIZE_PARAMS ncCalcSizeParams = new NativeMethods.NCCALCSIZE_PARAMS();
                ncCalcSizeParams = (NativeMethods.NCCALCSIZE_PARAMS)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));

                Padding calculatedClientMargin = this.GetNCMetrics();

                ncCalcSizeParams.rgrc[0].top += calculatedClientMargin.Top;
                ncCalcSizeParams.rgrc[0].left += calculatedClientMargin.Left;
                ncCalcSizeParams.rgrc[0].right -= calculatedClientMargin.Right;
                ncCalcSizeParams.rgrc[0].bottom -= calculatedClientMargin.Bottom;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);

                m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc(ref m);

                NativeMethods.RECT ncCalcSizeParams = new NativeMethods.RECT();
                ncCalcSizeParams = (NativeMethods.RECT)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));


                Padding calculatedClientMargin = this.GetNCMetrics();

                ncCalcSizeParams.top += calculatedClientMargin.Top;
                ncCalcSizeParams.left += calculatedClientMargin.Left;
                ncCalcSizeParams.right -= calculatedClientMargin.Right;
                ncCalcSizeParams.bottom -= calculatedClientMargin.Bottom;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);
                m.Result = IntPtr.Zero;
            }
        }

        #endregion

        #region Virtual methods

        protected virtual void OnNCPaint(Graphics g)
        {

        }

        protected virtual Padding GetNCMetrics()
        {
            return Padding.Empty;
        }

        #endregion
    }
}
