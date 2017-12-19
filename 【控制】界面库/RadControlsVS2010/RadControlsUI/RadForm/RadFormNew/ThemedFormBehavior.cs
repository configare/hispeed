using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a base class for all behaviors that
    /// modify the non-client area of the form and enable custom painting
    /// in it.
    /// </summary>
    public abstract class ThemedFormBehavior : FormControlBehavior
    {
        #region Fields

        private const int HIT_TEST_RADIUS = 8;
        private MdiClient mdiClient;
        private Form maximizedMdiChild;
        private Form activeMdiChild;
        private CreateParams currentFormParams;
        private int? currentFormState;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the ThemedFormBehavior class.
        /// </summary>
        public ThemedFormBehavior()
        {
        }

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// </summary>
        /// <param name="treeHandler">An IComponentTreeHandler instance.</param>
        public ThemedFormBehavior(IComponentTreeHandler treeHandler) :
            base(treeHandler)
        {
        }

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// </summary>
        /// <param name="treeHandler">An IComponentTreeHandler instance.</param>
        /// <param name="shouldCreateChildren">A flag that determines whether the CreateChildItems
        /// call is rerouted to the behavior.</param>
        public ThemedFormBehavior(IComponentTreeHandler treeHandler, bool shouldCreateChildren) :
            base(treeHandler, shouldCreateChildren)
        {
        }

        #endregion

        #region Properties

        #region Form state
        /// <summary>
        /// Gets a bool value that determines whether the Form's window state is maximized.
        /// </summary>
        protected virtual bool IsMaximized
        {
            get
            {
                if (this.Form.IsHandleCreated)
                {
                    return NativeMethods.IsZoomed(new HandleRef(this, this.Form.Handle));
                }
                return this.Form.WindowState == FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Gets a bool value that determines whether the Form's window state is minimized.
        /// </summary>
        protected virtual bool IsMinimized
        {
            get
            {
                if (this.Form.IsHandleCreated)
                {
                    return NativeMethods.IsIconic(new HandleRef(this, this.Form.Handle));
                }
                return this.Form.WindowState == FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// Gets a boolean value that determines whether the Form's window state is normal.
        /// </summary>
        protected virtual bool IsNormal
        {
            get
            {
                return this.Form.WindowState == FormWindowState.Normal;
            }
        }

        /// <summary>
        /// Gets an integer value that determines the current Form state:
        /// Possible values come from the SIZE_RESTORED, SIZE_MAXIMIZED, SIZE_MINIMIZED  win32 constants.
        /// </summary>
        protected int CurrentFormState
        {
            get
            {
                int formState = this.Form.WindowState == FormWindowState.Maximized ?
                    NativeMethods.SIZE_MAXIMIZED : this.Form.WindowState == FormWindowState.Normal ? NativeMethods.SIZE_RESTORED : NativeMethods.SIZE_MINIMIZED;
                return formState;
            }
        }

        /// <summary>
        /// Gets a boolean value showing whether a MDI child form is maximized.
        /// </summary>
        protected bool IsMdiChildMaximized
        {
            get
            {
                if (this.Form.IsMdiContainer)
                {
                    if (this.mdiClient != null)
                    {
                        if (this.Form.ActiveMdiChild != null && this.Form.ActiveMdiChild.IsHandleCreated)
                        {
                            return NativeMethods.IsZoomed(new HandleRef(this.Form.ActiveMdiChild, this.Form.ActiveMdiChild.Handle));
                        }

                        foreach (Form form in this.mdiClient.Controls)
                        {
                            if (form.Disposing || form.IsDisposed)
                            {
                                return false;
                            }

                            if (form.IsHandleCreated && NativeMethods.IsZoomed(new HandleRef(form, form.Handle)))
                            {
                                this.maximizedMdiChild = form;
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the MdiClient control of the Form.
        /// Returns null if the Form is not MdiContainer.
        /// </summary>
        protected MdiClient FormMdiClient
        {
            get
            {
                if (this.mdiClient == null && this.Form.IsMdiContainer)
                {
                    for (int i = 0; i < this.Form.Controls.Count; i++)
                    {
                        if (this.Form.Controls[i] is MdiClient)
                        {
                            return this.mdiClient = this.Form.Controls[i] as MdiClient;
                        }
                    }
                }
                return this.mdiClient;
            }
        }

        /// <summary>
        /// Gets a boolean value determining whether there is a Menu in the Form.
        /// </summary>
        protected bool IsMenuInForm
        {
            get
            {
                if (this.Form.MainMenuStrip != null
                    && !(this.Form.MainMenuStrip is RadRibbonFormMainMenuStrip))
                {
                    return true;
                }

                foreach (Control control in this.Form.Controls)
                {
                    if (control is RadMenu)
                    {
                        RadMenu menu = control as RadMenu;

                        if (menu.IsMainMenu)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the maximized MDI child if any.
        /// </summary>
        protected Form MaximizedMDIChild
        {
            get
            {
                Form mdiChild = this.Form.ActiveMdiChild;
                if (mdiChild != null && mdiChild.WindowState == FormWindowState.Maximized)
                {
                    return mdiChild;
                }
                return null;
            }
        }

        #endregion

        #region Hit testing frames

        /// <summary>
        /// This rectangle represents the top sizing frame of a window.
        /// </summary>
        public virtual Rectangle TopResizeFrame
        {
            get
            {
                return new Rectangle(HIT_TEST_RADIUS, 0, this.Form.Width - ( 2 * HIT_TEST_RADIUS ), HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the topleft sizing corner of a window.
        /// </summary>
        public virtual Rectangle TopLeftResizeFrame
        {
            get
            {
                return new Rectangle(0, 0, HIT_TEST_RADIUS, HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the left sizing frame of a window.
        /// </summary>
        public virtual Rectangle LeftResizeFrame
        {
            get
            {
                return new Rectangle(0, HIT_TEST_RADIUS, HIT_TEST_RADIUS, this.Form.Height - 2 * HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the bottomleft sizing corner of a window.
        /// </summary>
        public virtual Rectangle BottomLeftResizeFrame
        {
            get
            {
                return new Rectangle(0, this.Form.Height - HIT_TEST_RADIUS, HIT_TEST_RADIUS, HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the bottom sizing frame of a window.
        /// </summary>
        public virtual Rectangle BottomResizeFrame
        {
            get
            {
                return new Rectangle(HIT_TEST_RADIUS, this.Form.Height - HIT_TEST_RADIUS, this.Form.Width - ( 2 * HIT_TEST_RADIUS ), HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the bottomright sizing corner of a window.
        /// </summary>
        public virtual Rectangle BottomRightResizeFrame
        {
            get
            {
                return new Rectangle(this.Form.Width - HIT_TEST_RADIUS, this.Form.Height - HIT_TEST_RADIUS, HIT_TEST_RADIUS, HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the right sizing frame of a window.
        /// </summary>
        public virtual Rectangle RightResizeFrame
        {
            get
            {
                return new Rectangle(this.Form.Width - HIT_TEST_RADIUS, HIT_TEST_RADIUS, HIT_TEST_RADIUS, this.Form.Height - ( 2 * HIT_TEST_RADIUS ) );
            }
        }

        /// <summary>
        /// This rectangle represents the topright sizing corner of a window.
        /// </summary>
        public virtual Rectangle TopRightResizeFrame
        {
            get
            {
                return new Rectangle(this.Form.Width - HIT_TEST_RADIUS, 0, HIT_TEST_RADIUS, HIT_TEST_RADIUS);
            }
        }

        /// <summary>
        /// This rectangle represents the caption frame of a window.
        /// </summary>
        public virtual Rectangle CaptionFrame
        {
            get
            {
                return new Rectangle(0, 0, this.Form.Width, this.ClientMargin.Top);
            }
        }

        /// <summary>
        /// This rectangle represents the left border frame of a window.
        /// </summary>
        public virtual Rectangle LeftBorderFrame
        {
            get
            {
                return new Rectangle(
                    0,
                    this.ClientMargin.Top,
                    this.ClientMargin.Left,
                    this.Form.Height - (this.ClientMargin.Top + this.ClientMargin.Bottom));
            }
        }

        /// <summary>
        /// This rectangle represents the bottom border frame of a window.
        /// </summary>
        public virtual Rectangle BottomBorderFrame
        {
            get
            {
                return new Rectangle(
                    0,
                    this.Form.Height - this.ClientMargin.Bottom,
                    this.Form.Width,
                    this.ClientMargin.Bottom);
            }
        }

        /// <summary>
        /// This rectangle represents the right border frame of a window.
        /// </summary>
        public virtual Rectangle RightBorderFrame
        {
            get
            {
                return new Rectangle(
                    this.Form.Width - this.ClientMargin.Right,
                    this.ClientMargin.Top,
                    this.ClientMargin.Right,
                    this.Form.Height - (this.ClientMargin.Top + this.ClientMargin.Bottom));
            }
        }

        /// <summary>
        /// This rectangle represents the client rectangle of a window.
        /// </summary>
        public virtual Rectangle ClientFrame
        {
            get
            {
                return new Rectangle(this.ClientMargin.Left, this.ClientMargin.Top, this.Form.ClientRectangle.Width, this.Form.ClientRectangle.Height);
            }
        }

        #endregion
        

        /// <summary>
        /// Gets the rectangle that contains the menu of the form.
        /// </summary>
        public abstract Rectangle MenuBounds
        {
            get;
        }

        /// <summary>
        /// Gets the rectangle that contains the system buttons of the form.
        /// </summary>
        public abstract Rectangle SystemButtonsBounds
        {
            get;
        }

        /// <summary>
        /// Gets the rectangle that contains the form's icon.
        /// </summary>
        public abstract Rectangle IconBounds
        {
            get;
        }

        /// <summary>
        /// Gets the rectangle that contains the form's caption text.
        /// </summary>
        public abstract Rectangle CaptionTextBounds
        {
            get;
        }

        /// <summary>
        /// Gets or sets a bool value indiciating
        /// whether the behavior applies NC theming 
        /// to the form.
        /// </summary>
        public abstract bool AllowTheming
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current form CreateParams settings.
        /// </summary>
        public virtual CreateParams CurrentFormParams
        {
            get
            {
                return this.currentFormParams;
            }
        }

        /// <summary>
        /// Gets or sets the form associated with this behavior.
        /// Used only in design-time.
        /// IMPORTANT: This property can be assigned only one time.
        /// An InvalidaOperationException is thrown when the property
        /// is assigned more than once.
        /// </summary>
        public RadFormControlBase Form
        {
            get
            {
                return base.targetHandler as RadFormControlBase;
            }
            set
            {
                if (value is IComponentTreeHandler)
                {
                    if (base.targetHandler != null)
                    {
                        throw new InvalidOperationException("A Form is already associated.");
                    }

                    base.targetHandler = value;

                    this.OnFormAssociated();
                }
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// This method transforms screen coordinates
        /// into local coordinates.
        /// </summary>
        /// <param name="screenPoint">The screen point to transform.</param>
        /// <returns>The transformed point. If the handle of the associated Form is
        /// not created, the method returns the input.</returns>
        protected virtual Point GetMappedWindowPoint(Point screenPoint)
        {
            NativeMethods.POINT inputPoint = new NativeMethods.POINT();

            inputPoint.x = screenPoint.X;
            inputPoint.y = screenPoint.Y;

            if (this.Form.IsHandleCreated)
            {
                NativeMethods.MapWindowPoints(new HandleRef(this, IntPtr.Zero), new HandleRef(this, this.Form.Handle), inputPoint, 1);
            }

            return new Point(inputPoint.x + this.ClientMargin.Left, inputPoint.y + this.ClientMargin.Top);
        }

        /// <summary>
        /// This method returns the maximum available height according
        /// to the current position of the form in multi-monitor setup.
        /// </summary>
        /// <returns></returns>
        protected int GetMaximumFormHeightAccordingToCurrentScreen()
        {
            int result = Screen.PrimaryScreen.WorkingArea.Height;

            Screen currentScreen = Screen.FromControl(this.Form);

            if (currentScreen != null)
            {
                result = currentScreen.WorkingArea.Height;
            }
            result += SystemInformation.FrameBorderSize.Height * 2;

            return result;
        }

        #endregion

        #region Overriden methods

        public override CreateParams CreateParams(CreateParams parameters)
        {
            CreateParams cp = base.CreateParams(parameters);

            cp.Style |= (
                NativeMethods.WS_THICKFRAME
                | NativeMethods.WS_SYSMENU
                | NativeMethods.WS_CAPTION
                | NativeMethods.WS_MINIMIZEBOX
                | NativeMethods.WS_MAXIMIZEBOX
                );

            if (!DWMAPI.IsCompositionEnabled
                && this.AllowTheming || this.Form.IsDesignMode)
            {
                cp.Style &= ~NativeMethods.WS_CAPTION;
            }

            return this.currentFormParams = cp;
        }

        protected override void OnFormAssociated()
        {
            base.OnFormAssociated();
            this.Form.ControlAdded += new ControlEventHandler(Form_ControlAdded);
            this.Form.ControlRemoved += new ControlEventHandler(Form_ControlRemoved);
            this.Form.MdiChildActivate += new EventHandler(Form_MdiChildActivate);
        }

        protected override void Dispose(bool disposing)
        {
            this.Form.ControlAdded -= new ControlEventHandler(Form_ControlAdded);
            this.Form.ControlRemoved -= new ControlEventHandler(Form_ControlRemoved);
            this.Form.MdiChildActivate -= new EventHandler(Form_MdiChildActivate);

            if (this.activeMdiChild != null)
            {
                this.activeMdiChild.TextChanged -= new EventHandler(this.ActiveMDIChild_TextChanged);
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Protected and virtual methods

        protected virtual void OnActiveMDIChildTextChanged()
        {
        }

        /// <summary>
        /// Fires when the window state of the form is changed. Uses the SIZE_* values
        /// to define the window states.
        /// </summary>
        /// <param name="currentFormState">The old window state of the form.</param>
        /// <param name="newFormState">The new window state of the form</param>
        protected virtual void OnWindowStateChanged(int currentFormState, int newFormState)
        {
        }

        /// <summary>
        /// Immediately refreshes the whole non-client area of the form
        /// which this behavior is associated with.
        /// </summary>
        protected virtual void RefreshNC()
        {
            if (this.Form.IsHandleCreated && this.Form.ClientSize.Height > 0 && this.Form.ClientSize.Width > 0)
            {
                NativeMethods.RedrawWindow(
                    new HandleRef(null, this.Form.Handle),
                    IntPtr.Zero,
                    new HandleRef(null, IntPtr.Zero),
                    NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW);
            }
        }

        /// <summary>
        /// Invalidates the specified bounds in the non-client area
        /// of the form this behavior is associated with.
        /// </summary>
        /// <param name="bounds"></param>
        protected virtual void InvalidateNC(Rectangle bounds)
        {
            if (this.Form.IsHandleCreated)
            {
                NativeMethods.RECT rc = new NativeMethods.RECT(bounds.Left - this.ClientMargin.Left, bounds.Top - this.ClientMargin.Top, bounds.Right, bounds.Bottom);

                NativeMethods.RedrawWindow(
                    new HandleRef(this, this.Form.Handle),
                    ref rc,
                    new HandleRef(null, IntPtr.Zero),
                    NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
            }
        }


        /// <summary>
        /// This event is fired when the WM_GETMINMAXINFO message is sent to the form.
        /// </summary>
        /// <param name="minMaxInfo">Contains information about the position, maximum/minimum size of the form etc.
        /// Can be modified to adjust the settings applied to the form.</param>
        protected virtual void OnGetMinMaxInfo(MinMaxInfo minMaxInfo)
        {
        }

        /// <summary>
        /// This event is fired when the WM_NCPAINT message is sent to the form.
        /// </summary>
        /// <param name="graphics">The NC Graphics.</param>
        protected virtual void OnNCPaint(Graphics graphics)
        {
            this.PaintTitleBar(graphics);
            this.PaintBorders(graphics);
        }

        #endregion

        #region Painting

        private void PaintTitleBar(Graphics hDCGraphics)
        {
            if (this.ClientMargin.Top <= 0 || this.Form.Width <= 0)
            {
                return;
            }

            Bitmap offscreenBitMap = new Bitmap(this.Form.Width, this.ClientMargin.Top);
            Rectangle clipRectangle = new Rectangle(0, 0, offscreenBitMap.Width, offscreenBitMap.Height);

            using (Graphics g = Graphics.FromImage(offscreenBitMap))
            {
                RadGdiGraphics radGraphics = new RadGdiGraphics(g);

                this.PaintElement(radGraphics, clipRectangle, this.Form.RootElement);
            }

            hDCGraphics.DrawImage(offscreenBitMap, clipRectangle);
            offscreenBitMap.Dispose();
        }

        private void PaintBorders(Graphics hDCGraphics)
        {
            Rectangle leftBorderRect = new Rectangle(
                0,
                this.ClientMargin.Top,
                this.ClientMargin.Left,
                this.Form.Height - this.ClientMargin.Top
                );

            Rectangle rightBorderRect = new Rectangle(
                this.Form.Width - (this.ClientMargin.Right),
                this.ClientMargin.Top,
                this.ClientMargin.Right,
                this.Form.Height - this.ClientMargin.Top);

            Rectangle bottomBorderRect = new Rectangle(
                this.ClientMargin.Left,
                this.Form.Height - (this.ClientMargin.Bottom ),
                this.Form.Width - (this.ClientMargin.Left + this.ClientMargin.Right),
                this.ClientMargin.Bottom);


            if (leftBorderRect.Width > 0 && leftBorderRect.Height > 0)
            {

                Bitmap leftBorderOffscreen = new Bitmap(leftBorderRect.Width, leftBorderRect.Height);

                using (Graphics g = Graphics.FromImage(leftBorderOffscreen))
                {
                    RadGdiGraphics radGraphics = new RadGdiGraphics(g);
                    radGraphics.TranslateTransform(-leftBorderRect.X, -leftBorderRect.Y);
                    radGraphics.FillRectangle(leftBorderRect, this.Form.RootElement.BackColor);
                    PaintElement(radGraphics, leftBorderRect, this.Form.RootElement);
                }


                hDCGraphics.DrawImage(leftBorderOffscreen, leftBorderRect);
                leftBorderOffscreen.Dispose();
            }

            if (rightBorderRect.Width > 0 && rightBorderRect.Height > 0)
            {

                Bitmap rightBorderOffscreen = new Bitmap(rightBorderRect.Width, rightBorderRect.Height);

                using (Graphics g = Graphics.FromImage(rightBorderOffscreen))
                {

                    RadGdiGraphics radGraphics = new RadGdiGraphics(g);
                    radGraphics.TranslateTransform(-rightBorderRect.X, -rightBorderRect.Y);
                    radGraphics.FillRectangle(rightBorderRect, this.Form.RootElement.BackColor);
                    PaintElement(radGraphics, rightBorderRect, this.Form.RootElement);
                }

                hDCGraphics.DrawImage(rightBorderOffscreen, rightBorderRect);
                rightBorderOffscreen.Dispose();
            }


            if (bottomBorderRect.Width > 0 && bottomBorderRect.Height > 0)
            {
                Bitmap bottomBorderOffscreen = new Bitmap(bottomBorderRect.Width, bottomBorderRect.Height);

                using (Graphics g = Graphics.FromImage(bottomBorderOffscreen))
                {
                    RadGdiGraphics radGraphics = new RadGdiGraphics(g);
                    radGraphics.TranslateTransform(-bottomBorderRect.X, -bottomBorderRect.Y);
                    radGraphics.FillRectangle(bottomBorderRect, this.Form.RootElement.BackColor);
                    PaintElement(radGraphics, bottomBorderRect, this.Form.RootElement);
                }

                hDCGraphics.DrawImage(bottomBorderOffscreen, bottomBorderRect);
                bottomBorderOffscreen.Dispose();
            }
        }

        /// <summary>
        /// Paints an element on a specified graphics.
        /// </summary>
        /// <param name="graphics">The Graphics object to paint on.</param>
        /// <param name="clipRectangle">The clipping rectangle.</param>
        /// <param name="element">The element to paint.</param>
        protected virtual void PaintElement(IGraphics graphics, Rectangle clipRectangle, VisualElement element)
        {
            if (element is RootRadElement)
            {
                ((RootRadElement)element).Paint(graphics, clipRectangle, 0, new SizeF(1.0f, 1.0f), true);
            }
            else
            {
                element.Paint(graphics, clipRectangle,
                                0.0f, new SizeF(1.0f, 1.0f), false);
            }
        }

        #endregion

        #region Wnd Proc Handlers

        private void OnWMGetMinMaxInfo(ref Message m)
        {
            Telerik.WinControls.NativeMethods.MINMAXINFO info =
                           (Telerik.WinControls.NativeMethods.MINMAXINFO)
                           Marshal.PtrToStructure(m.LParam, typeof(Telerik.WinControls.NativeMethods.MINMAXINFO));

            if (this.Form.IsMdiChild)
            {
                info.ptMaxTrackSize.x = Math.Max(info.ptMaxTrackSize.x, info.ptMaxSize.x);
            }
            else if (!this.Form.IsMdiChild && this.IsMaximized
                && !DWMAPI.IsCompositionEnabled)
            {
                info.ptMaxTrackSize.y = this.GetMaximumFormHeightAccordingToCurrentScreen();

                if (Environment.OSVersion.Version.Major == 5 &&
                    Environment.OSVersion.Version.Minor == 1)
                {
                    Screen currentScreen = Screen.FromControl(this.Form);

                    if (currentScreen != null)
                    {
                        info.ptMaxPosition.y = currentScreen.WorkingArea.Y - SystemInformation.FrameBorderSize.Height;
                    }
                }
            }

            info.ptMinTrackSize.x = SystemInformation.MinimumWindowSize.Width;
            info.ptMinTrackSize.y = this.ClientMargin.Vertical;


            MinMaxInfo result = new MinMaxInfo();

            result.MaxPosition = new Point(info.ptMaxPosition.x, info.ptMaxPosition.y);
            result.MaxTrackSize = new Size(info.ptMaxTrackSize.x, info.ptMaxTrackSize.y);
            result.MinTrackSize = new Size(info.ptMinTrackSize.x, info.ptMinTrackSize.y);
            result.MaxSize = new Size(info.ptMaxSize.x, info.ptMaxSize.y);
            result.SizeReserved = new Size(info.ptReserved.x, info.ptReserved.y);


            this.OnGetMinMaxInfo(result);


            info.ptMaxPosition.x = result.MaxPosition.X;
            info.ptMaxPosition.y = result.MaxPosition.Y;

            info.ptMaxTrackSize.x = result.MaxTrackSize.Width;
            info.ptMaxTrackSize.y = result.MaxTrackSize.Height;

            info.ptMinTrackSize.x = result.MinTrackSize.Width;
            info.ptMinTrackSize.y = result.MinTrackSize.Height;

            info.ptMaxSize.x = result.MaxSize.Width;
            info.ptMaxSize.y = result.MaxSize.Height;

            info.ptReserved.x = result.SizeReserved.Width;
            info.ptReserved.y = result.SizeReserved.Height;

            Marshal.StructureToPtr(info, m.LParam, true);

            this.CallBaseWndProc(ref m);
        }


        private void OnWmActivate(ref Message m)
        {
            m.Result = new IntPtr(0);
            this.CallBaseWndProc(ref m);
            this.RefreshNC();
        }

        private void OnWMNCHitTest(ref Message m)
        {
            Point location = new Point(m.LParam.ToInt32());

            IntPtr result = new IntPtr(NativeMethods.HTNOWHERE);

            location = this.GetMappedWindowPoint(location);

            if (this.CaptionFrame.Contains(location))
            {
                result = new IntPtr(NativeMethods.HTCAPTION);
            }

            if (this.MenuBounds.Contains(location))
            {
                result = new IntPtr(NativeMethods.HTMENU);
            }

            if (this.Form.FormBorderStyle != FormBorderStyle.FixedToolWindow
                && this.Form.FormBorderStyle != FormBorderStyle.FixedSingle
                && this.Form.FormBorderStyle != FormBorderStyle.Fixed3D
                && this.Form.FormBorderStyle != FormBorderStyle.FixedDialog)
            {
                if (this.TopResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTTOP);
                }

                if (this.TopLeftResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTTOPLEFT);
                }

                if (this.LeftResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTLEFT);
                }

                if (this.BottomLeftResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTBOTTOMLEFT);
                }

                if (this.BottomResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTBOTTOM);
                }

                if (this.BottomRightResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTBOTTOMRIGHT);
                }

                if (this.RightResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTRIGHT);
                }

                if (this.TopRightResizeFrame.Contains(location))
                {
                    result = new IntPtr(NativeMethods.HTTOPRIGHT);
                }

            }

            if (this.ClientFrame.Contains(location))
            {
                result = new IntPtr(NativeMethods.HTCLIENT);
            }

            m.Result = result;
        }

        private void OnWMNCCalcSize(ref Message m)
        {
            if (m.WParam == new IntPtr(1))
            {
                NativeMethods.NCCALCSIZE_PARAMS ncCalcSizeParams = new NativeMethods.NCCALCSIZE_PARAMS();
                ncCalcSizeParams = (NativeMethods.NCCALCSIZE_PARAMS)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));

                Padding calculatedClientMargin = this.ClientMargin;

                //This code fixes the issue with the wrong positioning of a MDI child when 
                //composition is off and the child is maximized.
                if (this.Form.IsMdiChild && this.IsMaximized
                    && !DWMAPI.IsCompositionEnabled)
                {
                   ncCalcSizeParams.rgrc[0].top = -SystemInformation.FixedFrameBorderSize.Height;
                }

                ncCalcSizeParams.rgrc[0].top += calculatedClientMargin.Top;
                ncCalcSizeParams.rgrc[0].left += calculatedClientMargin.Left;
                ncCalcSizeParams.rgrc[0].right -= calculatedClientMargin.Right;
                ncCalcSizeParams.rgrc[0].bottom -= calculatedClientMargin.Bottom;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);

                m.Result = IntPtr.Zero;
            }
            else
            {
                this.CallBaseWndProc(ref m);

                NativeMethods.RECT ncCalcSizeParams = new NativeMethods.RECT();
                ncCalcSizeParams = (NativeMethods.RECT)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));


                Padding calculatedClientMargin = this.ClientMargin;

                //This code fixes the issue with the wrong positioning of a MDI child when 
                //composition is off and the child is maximized.
                if (this.Form.IsMdiChild && this.IsMaximized
                    && !DWMAPI.IsCompositionEnabled)
                {
                    ncCalcSizeParams.top = -SystemInformation.FixedFrameBorderSize.Height;
                }

                ncCalcSizeParams.top += calculatedClientMargin.Top;
                ncCalcSizeParams.left += calculatedClientMargin.Left;
                ncCalcSizeParams.right -= calculatedClientMargin.Right;
                ncCalcSizeParams.bottom -= calculatedClientMargin.Bottom;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);
                m.Result = IntPtr.Zero;
            }
        }

        private void OnWMNCPaint(ref Message m)
        {
            if (!this.Form.IsHandleCreated)
            {
                return;
            }

            HandleRef hWnd = new HandleRef(this, this.Form.Handle);
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

        private void OnWMNCActivate(ref Message m)
        {
            m.LParam = new IntPtr(-1);
            if (m.WParam == IntPtr.Zero)
            {
                m.Result = IntPtr.Zero;
            }

            this.CallDefWndProc(ref m);

            this.RefreshNC();
        }

        #endregion

        #region FormControlBehavior implementation

        public override void InvalidateElement(RadElement element, System.Drawing.Rectangle bounds)
        {
            this.InvalidateNC(bounds);
        }

        public override bool HandleWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCUAHDRAWCAPTION:
                case NativeMethods.WM_NCUAHDRAWFRAME:
                    {
                        m.Result = new IntPtr(0);
                        return true;
                    }
                
                case NativeMethods.WM_SIZE:
                    {
                        this.OnWMSize(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCCALCSIZE:
                    {
                        this.OnWMNCCalcSize(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCPAINT:
                    {
                        this.OnWMNCPaint(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCACTIVATE:
                    {
                        this.OnWMNCActivate(ref m);
                        return true;
                    }
                case NativeMethods.WM_ACTIVATE:
                    {
                        this.OnWmActivate(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCHITTEST:
                    {
                        this.OnWMNCHitTest(ref m);
                        return true;
                    }
                case NativeMethods.WM_GETMINMAXINFO:
                    {
                        this.OnWMGetMinMaxInfo(ref m);
                       
                        return true;
                    }
            }

            return false;
        }

        private void OnWMSize(ref Message m)
        {
            int newFormState = (int)m.WParam;

            if (this.currentFormState.HasValue)
            {
                if (this.currentFormState != newFormState)
                {
                    this.OnWindowStateChanged(this.currentFormState.Value, newFormState);

                    this.currentFormState = newFormState;
                }
            }
            else
            {
                this.currentFormState = newFormState;
            }
            this.CallBaseWndProc(ref m);
        }

        #endregion

        #region Event handling

        private void Form_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.activeMdiChild != null)
            {
                this.activeMdiChild.TextChanged -= new EventHandler(ActiveMDIChild_TextChanged);
                this.activeMdiChild = null;
            }

            this.activeMdiChild = this.Form.ActiveMdiChild;

            if (this.activeMdiChild != null)
            {
                this.activeMdiChild.TextChanged += new EventHandler(ActiveMDIChild_TextChanged);
            }
        }

        private void ActiveMDIChild_TextChanged(object sender, EventArgs e)
        {
            this.OnActiveMDIChildTextChanged();  
        }

        private void Form_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is MdiClient)
            {
                this.mdiClient = null;
            }
        }

        private void Form_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is MdiClient)
            {
                this.Form.Update();
                this.mdiClient = e.Control as MdiClient;
            }
        }

        #endregion
    }
}
