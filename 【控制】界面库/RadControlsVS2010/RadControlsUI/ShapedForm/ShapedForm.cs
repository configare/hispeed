using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public partial class ShapedForm : Form
    {
        private FormWindowState oldWindowState;
        private Color borderColor = Color.Black;
        private int borderWidth = 1;
        private ElementShape shape = null;
        private GraphicsPath borderPath = null;
        private GraphicsPath outerPath = null;
        private string themeName;
        private bool active;
        private bool enableCompositionOnVista;
        static private bool isCompositionAvailable;
        private Color trasnparencyCompositionKey = Color.Magenta;
        private bool allowResize = true;

        private static DllWrapper dllWrapper;

        static ShapedForm()
        {
        }

        public ShapedForm()
        {
            if (dllWrapper == null)
            {
                dllWrapper = new DllWrapper(@"dwmapi.dll");
                if (dllWrapper.IsDllLoaded)
                {
                    dwmIsCompositionEnabled = (DwmIsCompositionEnabled)dllWrapper.GetFunctionAsDelegate(
                        "DwmIsCompositionEnabled", typeof(DwmIsCompositionEnabled));
                    if (dwmIsCompositionEnabled != null)
                    {
                        bool enabled = false;
                        dwmIsCompositionEnabled(ref enabled);
                        if (enabled)
                        {
                            dwmEnableBlurBehindWindow = (DwmEnableBlurBehindWindow)dllWrapper.GetFunctionAsDelegate(
                                "DwmEnableBlurBehindWindow", typeof(DwmEnableBlurBehindWindow));
                            isCompositionAvailable = true;
                        }
                    }
                }
            }

            InitializeComponent();
        }

        #region Win32

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public uint dwFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnable;
            public IntPtr hRegionBlur;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fTransitionOnMaximized;

            public const uint DWM_BB_ENABLE = 0x00000001;
            public const uint DWM_BB_BLURREGION = 0x00000002;
            public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int DwmIsCompositionEnabled(ref bool isEnabled);
        static private DwmIsCompositionEnabled dwmIsCompositionEnabled;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);
        static private DwmEnableBlurBehindWindow dwmEnableBlurBehindWindow;
        private PenAlignment penAlignment = PenAlignment.Left;

        #endregion

        #region Properties

        /// <summary>
        /// Allow form's resize
        /// </summary>
        [Browsable(true), Category("Appearance")]
        [Description("Allow form's resize")]
        [DefaultValue(true)]
        public bool AllowResize
        {
            get { return this.allowResize; }
            set { this.allowResize = value; }
        }

        /// <summary>
        /// Gets or sets the form's border color
        /// </summary>
        [Browsable(true), Category("Appearance")]
        [Description("Gets or sets the form's border color")]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }

        /// <summary>
        /// Gets or sets the form's border width
        /// </summary>
        [Browsable(true), Category("Appearance")]
        [Description("Gets or sets the form's border width")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }

        /// <summary>
        /// Gets or sets an instance of the Shape object of a form. The shape of the
        /// form is responsible for providing its' border(s) with custom shape.
        /// </summary>
        /// <remarks>
        /// Some predefined shapes are available, like <see cref="RoundRectShape"/> or <see cref="EllipseShape"/>.
        /// <see cref="CustomShape"/> offers a way to specify element's shape with a sequance of points and curves using code 
        /// or the design time <see cref="ElementShapeEditor"/>
        /// 	<see cref="UITypeEditor"/>.
        /// </remarks>
        [Browsable(true), Category("Appearance")]
        [Description("Represents the shape of the form. Setting shape affects also border form painting. Shape is considered when painting, clipping and hit-testing an element.")]
        [DefaultValue(null)]
        [Editor(typeof(ElementShapeEditor), typeof(UITypeEditor))]
        public ElementShape Shape
        {
            get { return this.shape; }
            set
            {
                if (this.shape != null)
                    this.shape.Dispose();

                this.shape = value;
                ApplyShape();
            }
        }

        /// <summary>Gets or sets theme name.</summary>
        [Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
        [Description("Gets or sets theme name.")]
        [DefaultValue((string)"")]
        [Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
        public string ThemeName
        {
            get { return themeName; }
            set
            {
                themeName = value;
                ApplyTheme(themeName);
            }
        }

        /// <summary>Enables or disables transparent background on Vista</summary>
        [Browsable(true), Category("Appearance")]
        [Description("Enables or disables transparent background on Vista.")]
        [DefaultValue(false)]
        public bool EnableCompositionOnVista
        {
            get { return this.enableCompositionOnVista; }
            set { this.enableCompositionOnVista = value; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x00020000; // Set the WS_MINIMIZEBOX flag so that it can be minimized and maximized from the taskbar.
                return cp;
            }
        }

        /*
        /// <summary>Gets or sets the color used as transparent color when composition is enabled</summary>
        [Browsable(true), Category("Appearance")]
        [Description("Gets or sets the color used as transparent color when composition is enabled.")]
        [DefaultValue("Magenta")]
        public Color TrasnparencyCompositionKey
        {
            get { return this.trasnparencyCompositionKey; }
            set { this.trasnparencyCompositionKey = value; }
        }
        */

        #endregion

        #region Event handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!DesignMode && enableCompositionOnVista && isCompositionAvailable)
            {
                bool r = NativeMethods.SetLayeredWindowAttributes(
                    new HandleRef(this, this.Handle), 100, 0, NativeMethods.LWA_COLORKEY);


                DWM_BLURBEHIND blurBehind = new DWM_BLURBEHIND();

                blurBehind.dwFlags = DWM_BLURBEHIND.DWM_BB_ENABLE | DWM_BLURBEHIND.DWM_BB_TRANSITIONONMAXIMIZED;
                blurBehind.fEnable = true;
                blurBehind.fTransitionOnMaximized = false;

                dwmEnableBlurBehindWindow(this.Handle, ref blurBehind);
                ThemeResolutionService.ApplicationThemeChanged += new ThemeChangedHandler(ApplicationThemeChanged);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            PaintFrame(e.Graphics);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.WindowState != FormWindowState.Minimized)
            {
                this.oldWindowState = this.WindowState;
            }

            ApplyShape();
            if (this.Visible)
            {
                using (Graphics g = CreateGraphics())
                {
                    PaintFrame(g);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (this.shape != null ||
                this.IsMdiContainer)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_NCCALCSIZE:
                        m.Result = IntPtr.Zero;
                        return;

                    case NativeMethods.WM_NCPAINT:
                        m.Result = IntPtr.Zero;
                        return;

                    case NativeMethods.WM_NCACTIVATE:
                        active = Convert.ToBoolean(m.WParam.ToInt32());
                        Invalidate();
                        m.Result = (IntPtr)1;
                        return;

                    case 0xAE:
                    case 0xAF:
                        m.Result = IntPtr.Zero;
                        return;
                }
            }

            switch (m.Msg)
            {
                case NativeMethods.WM_POPUPSYSTEMMENU:
                    WmTaskBarMenu();
                    break;

                case NativeMethods.WM_NCHITTEST:
                    if (this.FormBorderStyle == FormBorderStyle.None && this.AllowResize)
                    {
                        Point pt = this.PointToClient(new Point(m.LParam.ToInt32()));
                        m.Result = (IntPtr)GetHitTest(pt);
                        return;
                    }
                    break;

                case NativeMethods.WM_WINDOWPOSCHANGED:
                    if (this.IsMdiContainer)
                        OnLayout(new LayoutEventArgs(this, "Bounds"));
                    break;

                case NativeMethods.WM_GETMINMAXINFO:
                    {
                        NativeMethods.MINMAXINFO info = (NativeMethods.MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.MINMAXINFO));

                        if (this.Parent == null)
                        {
                            // Get the working area of the screen that hodls the biggest part of the form.
                            // The maximization logic will put the maximized form on this screen.
                            Screen scr = Screen.FromControl(this);
                            Rectangle workingArea = scr.WorkingArea;
                            Rectangle screenRect = scr.Bounds;

                            info.ptMaxSize.x = workingArea.Width;
                            info.ptMaxSize.y = workingArea.Height;
                            info.ptMaxPosition.x = workingArea.X - screenRect.X;
                            info.ptMaxPosition.y = workingArea.Y - screenRect.Y;
                            Marshal.StructureToPtr(info, m.LParam, false);
                        }
                        //else
                        //{
                        //    NativeMethods.POINT maxSize = new NativeMethods.POINT();
                        //    maxSize.x = this.Parent.Width;
                        //    maxSize.y = this.Parent.Height;

                        //    NativeMethods.POINT maxPos = new NativeMethods.POINT();
                        //    maxPos.x = this.Parent.Bounds.X;
                        //    maxPos.y = this.Parent.Bounds.Y;

                        //    info.ptMaxSize = maxSize;
                        //    info.ptMaxPosition = maxPos;
                        //    Marshal.StructureToPtr(info, m.LParam, false);
                        //}

                        base.WndProc(ref m);
                    }
                    return;
            }

            base.WndProc(ref m);
        }

        protected virtual void ApplicationThemeChanged(object sender, ThemeChangedEventArgs args)
        {
            if (args.ThemeName != null)
                this.ThemeName = args.ThemeName;
        }

        private void WmTaskBarMenu()
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem item = new MenuItem("Restore");
                item.Click += new EventHandler(item_RestoreClick);
                menu.MenuItems.Add(item);

                item = new MenuItem("Move");
                item.Enabled = false;
                menu.MenuItems.Add(item);

                item = new MenuItem("Size");
                item.Enabled = false;
                menu.MenuItems.Add(item);

                item = new MenuItem("Minimize");
                item.Click += new EventHandler(item_MinimizeClick);
                item.Enabled = (this.WindowState != FormWindowState.Minimized && this.MinimizeBox);
                menu.MenuItems.Add(item);

                item = new MenuItem("Maximize");
                item.Click += new EventHandler(item_MaximizeClick);
                item.Enabled = (this.WindowState != FormWindowState.Maximized && this.MaximizeBox);
                menu.MenuItems.Add(item);

                menu.MenuItems.Add(new MenuItem("-"));

                item = new MenuItem("Close    Alt+F4");
                item.Click += new EventHandler(item_CloseClick);
                menu.MenuItems.Add(item);

                menu.Show(this, PointToClient(Control.MousePosition));
            }
        }

        private void item_RestoreClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = this.oldWindowState;
            }
        }

        private void item_MinimizeClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void item_MaximizeClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void item_CloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        protected virtual void PaintFrame(Graphics g)
        {
            if (!DesignMode && enableCompositionOnVista && isCompositionAvailable)
            {
                g.Clear(Color.Black);
                if (this.FormBorderStyle != FormBorderStyle.None)
                    return;
            }
            else
            {
                if (this.BackgroundImage != null)
                    base.OnPaintBackground(new PaintEventArgs(g, this.ClientRectangle));
                else
                {
                    try
                    {
                        g.Clear(this.BackColor);
                    }
                    catch (Exception)
                    {
                        // There is a knwon issue in GDI+ on a remote desktop and the connection 
                        // window is minimized:
                        // The reason why GDI+ error occurs is that GDI+ fail to render 
                        // to the disconnected desktop
                        // As suggested in msdn forums, this exception may be ignorred
                    }
                }
            }

            using (Pen pen = new Pen(this.borderColor, this.BorderWidth))
            {
                pen.Alignment = penAlignment;

                SmoothingMode originalSmoothingMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (this.borderPath != null)
                {
                    g.DrawPath(pen, borderPath);
                }
                else
                {
                    g.DrawRectangle(pen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                }

                g.SmoothingMode = originalSmoothingMode;
            }
        }

        public PenAlignment BorderAlignment
        {
            get { return this.penAlignment; }
            set { this.penAlignment = value; }
        }

        protected virtual int GetHitTest(Point point)
        {
            int w = 3;
            int tw = 24;

            if (this.WindowState == FormWindowState.Maximized)
                w = -1;

            if (point.X <= w * 2 && point.Y <= w * 2)
                return NativeMethods.HTTOPLEFT;
            else if (point.X >= this.Bounds.Width - w * 2 && point.Y <= w * 2)
                return NativeMethods.HTTOPRIGHT;
            else if (point.X <= w * 2 && point.Y >= this.Bounds.Height - w * 2)
                return NativeMethods.HTBOTTOMLEFT;
            else if (point.X >= this.Bounds.Width - w * 2 && point.Y >= this.Bounds.Height - w * 2)
                return NativeMethods.HTBOTTOMRIGHT;
            else if (point.X <= w)
                return NativeMethods.HTLEFT;
            else if (point.X >= this.Bounds.Width - w)
                return NativeMethods.HTRIGHT;
            else if (point.Y <= w)
                return NativeMethods.HTTOP;
            else if (point.Y >= this.Bounds.Height - w)
                return NativeMethods.HTBOTTOM;
            else if (point.Y <= tw)
                return NativeMethods.HTCAPTION;

            return NativeMethods.HTCLIENT;
        }

        private void ApplyTheme(string themeName)
        {
            switch (themeName)
            {
                case "ControlDefault":
                    this.BorderWidth = 1;
                    this.BackColor = Color.FromArgb(191, 219, 254);
                    this.BorderColor = Color.FromArgb(59, 90, 130);
                    break;

                case "Office2007Black":
                    this.BorderWidth = 1;
                    this.BackColor = Color.FromArgb(83, 83, 83);
                    this.BorderColor = Color.FromArgb(47, 47, 47);
                    break;

                case "Office2007Silver":
                    this.BorderWidth = 1;
                    this.BackColor = Color.FromArgb(208, 212, 221);
                    this.BorderColor = Color.FromArgb(152, 152, 152);
                    break;

                case "Telerik":
                    this.BorderWidth = 1;
                    this.BackColor = Color.FromArgb(254, 254, 254);
                    this.BorderColor = Color.FromArgb(152, 152, 152);
                    break;
            }

            NativeMethods.SetWindowPos(new HandleRef(null, base.Handle), new HandleRef(null, IntPtr.Zero), 0, 0, 0, 0, 0x237);
            NativeMethods.RedrawWindow(new HandleRef(null, base.Handle), IntPtr.Zero, new HandleRef(null, IntPtr.Zero), 0x501);
        }

        private void ApplyShape()
        {
            if (this.shape != null)
            {
                if (this.Region != null)
                    this.Region.Dispose();

                if (this.borderPath != null)
                    this.borderPath.Dispose();

                if (this.outerPath != null)
                    this.outerPath.Dispose();

                //there is grd API for creating the round rect region, which should be used only in RoundRectShape case
                if (shape.GetType() == typeof(RoundRectShape))
                {
                    borderPath = this.shape.CreatePath(new RectangleF(0, 0, this.Bounds.Width - 1f, this.Bounds.Height - 1f));
                    outerPath = shape.CreatePath(new RectangleF(0, 0, this.Bounds.Width, this.Bounds.Height));
                    int shapreRadius = ((RoundRectShape)shape).Radius;
                    int radius;
                    radius = 2 * shapreRadius;

                    this.Region = NativeMethods.CreateRoundRectRgn(new Rectangle(Point.Empty, this.Bounds.Size), radius);
                }
                else
                {
                    borderPath = this.shape.CreatePath(new RectangleF(0, 0, this.Bounds.Width - 1f, this.Bounds.Height - 1f));
                    outerPath = shape.CreatePath(new RectangleF(0, 0, this.Bounds.Width, this.Bounds.Height + borderWidth));
                    this.Region = new Region(outerPath);
                }
            }
        }
    }
}