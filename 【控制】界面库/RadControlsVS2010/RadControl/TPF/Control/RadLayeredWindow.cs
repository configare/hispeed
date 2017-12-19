using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a Win2K+ layered window semantic, which allows for semi-transparent windows.
    /// </summary>
    [ToolboxItem(false)]
    public class RadLayeredWindow : Control
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RadLayeredWindow()
        {
            if (!OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
            {
                throw new InvalidOperationException("Current OS does not support Layered Windows");
            }

            this.updated = false;
            this.topMost = false;
            this.hitTestable = false;
            this.desiredSize = Size.Empty;
            this.alpha = 1F;
            this.recreateHandleOnSizeChanged = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = new CreateParams();
                //tell windows that Desktop is our the parent
                cp.Parent = IntPtr.Zero;

                cp.Style = NativeMethods.WS_POPUP;
                cp.ExStyle = NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_LAYERED;

                if (this.topMost)
                {
                    cp.ExStyle |= NativeMethods.WS_EX_TOPMOST;
                }

                if (!this.hitTestable)
                {
                    cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT;
                }

                return cp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        /// <summary>
        /// Provides special handling for the WM_MOUSEACTIVATE, WM_PAINT and WM_NCHITTEST messages.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_MOUSEACTIVATE:
                    //set the result to MA_NOACTIVATE
                    m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                    return;
                case NativeMethods.WM_NCHITTEST:
                    if (!this.hitTestable)
                    {
                        m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                        return;
                    }
                    break;
                case NativeMethods.WM_PAINT:
                case NativeMethods.WM_ERASEBKGND:
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets or sets the Image that represents the Layered window.
        /// </summary>
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
                this.updated = false;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Brings the window on top of the z-order.
        /// </summary>
        /// <param name="activate"></param>
        public void BringToFront(bool activate)
        {
            this.UpdateZOrder(NativeMethods.HWND_TOP, activate);
        }

        /// <summary>
        /// Sends the window to back of the z-order.
        /// </summary>
        /// <param name="activate"></param>
        public void SendToBack(bool activate)
        {
            this.UpdateZOrder(NativeMethods.HWND_BOTTOM, activate);
        }

        private void UpdateZOrder(HandleRef pos, bool activate)
        {
            int flags = 0x603;
            if (!activate)
            {
                flags |= 0x10;
            }
            NativeMethods.SetWindowPos(new HandleRef(this, this.Handle), pos, 0, 0, 0, 0, flags);
        }

        /// <summary>
        /// Suspends any Layered-related updates for the window.
        /// Useful for multiple properties set-up without sequential update for each property change.
        /// </summary>
        public void SuspendUpdates()
        {
            this.suspendUpdateCount++;
        }

        /// <summary>
        /// Resumes previously suspended updates and forces Layered update.
        /// </summary>
        public void ResumeUpdates()
        {
            this.ResumeUpdates(true);
        }

        /// <summary>
        /// Resumes previously suspended updates. Optionally preforms Layered update.
        /// </summary>
        /// <param name="update"></param>
        public void ResumeUpdates(bool update)
        {
            this.suspendUpdateCount--;
            this.suspendUpdateCount = Math.Max(0, this.suspendUpdateCount);

            if (this.suspendUpdateCount == 0 && update)
            {
                this.ShowWindow(this.Location);
            }
        }

        /// <summary>
        /// Displays the window to user using the specified location and current size.
        /// </summary>
        public virtual void ShowWindow(Point screenLocation)
        {
            if (this.suspendUpdateCount > 0)
            {
                return;
            }

            bool handleCreated = this.IsHandleCreated;
            if (!handleCreated)
            {
                NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
            }

            this.Location = screenLocation;

            if (!this.updated)
            {
                Size displaySize = this.DisplaySize;
                bool sizeChange = this.Size != displaySize;
                this.Size = displaySize;

                //determine whether we have a changed size and need to re-create the handle
                if (handleCreated)
                {
                    if (sizeChange && this.recreateHandleOnSizeChanged)
                    {
                        this.RecreateHandle();
                        NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);

                        //assign again size and location since we have recreated the Handle
                        this.Size = displaySize;
                        this.Location = screenLocation;
                    }
                }

                this.Size = displaySize;

                if (displaySize != Size.Empty)
                {
                    this.UpdateWindow();
                }
            }

            if (!Visible)
            {
                NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
            }
        }

        /// <summary>
        /// Performs painting of the window.
        /// Default implementation simply paints the BackgroundImage (if any).
        /// </summary>
        /// <param name="g">The graphics to use.</param>
        /// <param name="graphicsBitmap">The off-screen bitmap instance the graphics is created from.</param>
        protected virtual void PaintWindow(Graphics g, Bitmap graphicsBitmap)
        {
            Image img = BackgroundImage;
            if (img != null)
            {
                g.DrawImage(img, 0, 0, Width, Height);
            }
        }

        /// <summary>
        /// Gets the final Bitmap that represents the content of the Layered Window.
        /// </summary>
        protected Bitmap Content
        {
            get
            {
                return this.content;
            }
        }

        /// <summary>
        /// Updates the layered window.
        /// </summary>
        protected void UpdateWindow()
        {
            if (this.suspendUpdateCount > 0)
            {
                return;
            }

            if (this.content != null)
            {
                this.content.Dispose();
            }

            this.content = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(this.content);

            //perform window painting using this graphics surface
            //by default, the Background image will be painted.
            PaintWindow(g, this.content);

            NativeUpdateWindow(this.content);

            g.Dispose();

            this.updated = true;
        }
        /// <summary>
        /// Performs native layered window update, using the Win32 UpdateLayeredWindow API.
        /// </summary>
        /// <param name="bmp"></param>
        private void NativeUpdateWindow(Bitmap bmp)
        {
            Point location = Location;

            lock (MeasurementGraphics.SyncObject)
            {
                IntPtr hDC = NativeMethods.GetDC(new HandleRef(null, IntPtr.Zero));
                IntPtr memoryDC = NativeMethods.CreateCompatibleDC(new HandleRef(null, hDC));
                IntPtr hBitmap = bmp.GetHbitmap(Color.FromArgb(0));
                IntPtr oldBitmap = NativeMethods.SelectObject(new HandleRef(null, memoryDC), new HandleRef(null, hBitmap));

                NativeMethods.SIZESTRUCT ulwsize;
                ulwsize.cx = Width;
                ulwsize.cy = Height;

                NativeMethods.POINTSTRUCT topPos;
                topPos.x = Left;
                topPos.y = Top;

                NativeMethods.POINTSTRUCT pointSource;
                pointSource.x = 0;
                pointSource.y = 0;

                NativeMethods.BLENDFUNCTION blend;
                blend.BlendOp = (byte)AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)(this.alpha * 255);
                blend.AlphaFormat = (byte)AC_SRC_ALPHA;

                // Tell operating system to use our bitmap for painting
                NativeMethods.UpdateLayeredWindow(Handle, hDC, ref topPos, ref ulwsize,
                                                        memoryDC, ref pointSource, 0, ref blend, ULW_ALPHA);

                NativeMethods.SelectObject(new HandleRef(null, memoryDC), new HandleRef(null, oldBitmap));
                // Cleanup resources
                NativeMethods.ReleaseDC(new HandleRef(null, IntPtr.Zero), new HandleRef(null, hDC));
                NativeMethods.DeleteObject(new HandleRef(null, hBitmap));
                NativeMethods.DeleteDC(new HandleRef(null, memoryDC));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether window's handle will be re-created upon a Size change.
        /// If the window is large - e.g. 800*600 pixels, 
        /// applying new size may cause flicker due to the nature of Layered Windows semantic.
        /// </summary>
        public bool RecreateHandleOnSizeChanged
        {
            get
            {
                return this.recreateHandleOnSizeChanged;
            }
            set
            {
                if (this.recreateHandleOnSizeChanged == value)
                {
                    return;
                }

                this.recreateHandleOnSizeChanged = value;
            }
        }

        /// <summary>
        /// Determines whether the window is updated (used UpdateLayeredWindow API).
        /// </summary>
        public bool Updated
        {
            get
            {
                return this.updated;
            }
            protected set
            {
                this.updated = value;
            }
        }

        /// <summary>
        /// Gets or sets the Alpha (transparency) value - [0, 1] - for the window.
        /// </summary>
        public float Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                value = Math.Min(1, value);
                value = Math.Max(0, value);

                if (this.alpha == value)
                {
                    return;
                }

                this.alpha = value;

                if (this.updated)
                {
                    this.updated = false;
                    this.UpdateWindow();
                }
            }
        }

        /// <summary>
        /// Gets the current size used by the window to visualize itself.
        /// </summary>
        public Size DisplaySize
        {
            get
            {
                if (this.desiredSize != Size.Empty)
                {
                    return this.desiredSize;
                }

                if (this.BackgroundImage != null)
                {
                    return this.BackgroundImage.Size;
                }

                return Size.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the size of the window.
        /// </summary>
        public Size DesiredSize
        {
            get
            {
                return this.desiredSize;
            }
            set
            {
                value.Width = Math.Max(0, value.Width);
                value.Height = Math.Max(0, value.Height);

                if (value == this.desiredSize)
                {
                    return;
                }

                this.desiredSize = value;
                if (this.updated)
                {
                    this.updated = false;
                    this.ShowWindow(this.Location);
                }
            }
        }

        /// <summary>
        /// Determines whether the window is TopMost (above all floating windows).
        /// </summary>
        public bool TopMost
        {
            get
            {
                return this.topMost;
            }
            set
            {
                if (this.topMost == value)
                {
                    return;
                }

                this.topMost = value;
                if (this.IsHandleCreated)
                {
                    this.UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Determines whether the Control is visible for mouse input.
        /// </summary>
        public bool HitTestable
        {
            get
            {
                return this.hitTestable;
            }
            set
            {
                if (this.hitTestable == value)
                {
                    return;
                }

                this.hitTestable = value;
                if (this.IsHandleCreated)
                {
                    this.UpdateStyles();
                }
            }
        }

        #endregion

        #region Fields

        private Bitmap content;
        private int suspendUpdateCount;
        private Size desiredSize;
        private bool updated;
        private bool topMost;
        private bool hitTestable;
        private float alpha;
        private bool recreateHandleOnSizeChanged;

        #endregion

        #region Constants

        private const int AC_SRC_OVER = 0x00;
        private const int AC_SRC_ALPHA = 0x01;
        private const int ULW_COLORKEY = 0x00000001;
        private const int ULW_ALPHA = 0x00000002;
        private const int ULW_OPAQUE = 0x00000004;

        #endregion
    }
}
