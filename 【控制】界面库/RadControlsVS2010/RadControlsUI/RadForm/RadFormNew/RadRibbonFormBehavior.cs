using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadRibbonFormBehavior : ThemedFormBehavior
    {
        #region Delegates

        private delegate void DwmExtendFrameIntoClientAreaDelegate(IntPtr wndHandle, ref NativeMethods.MARGINS margins);
        private DwmExtendFrameIntoClientAreaDelegate DwmExtendFrameIntoClientArea;

        private delegate bool DwmDefWindowProcDelegate(IntPtr wndHandle, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr result);
        private DwmDefWindowProcDelegate DwmDefWindowProc;

        #endregion

        #region Fields
        private bool allowTheming = true;
        private RadRibbonBar radRibbonBar;
        private RibbonFormElement formElement;
        private bool compositionEnabled;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the <typeparamref name="RadRibbonFormBehavior"/> class.
        /// This instance is not associated with an <typeparamref name="IComponentTreeHandler"/> implementation.
        /// </summary>
        public RadRibbonFormBehavior()
            : base()
        {
            DllWrapper dwmapi = new DllWrapper("dwmapi.dll");

            DwmExtendFrameIntoClientArea =
                (DwmExtendFrameIntoClientAreaDelegate)dwmapi.GetFunctionAsDelegate("DwmExtendFrameIntoClientArea", typeof(DwmExtendFrameIntoClientAreaDelegate));

            DwmDefWindowProc =
                (DwmDefWindowProcDelegate)dwmapi.GetFunctionAsDelegate("DwmDefWindowProc", typeof(DwmDefWindowProcDelegate));
        }

        /// <summary>
        /// Creates an instance of the <typeparamref name="RadRibbonFormBehavior"/> class.
        /// <param name="treeHandler">The <typeparamref name="IComponentTreeHandler"/> implementation
        /// which this behavior is associated with.</param>
        /// </summary>
        public RadRibbonFormBehavior(IComponentTreeHandler treeHandler)
            : base(treeHandler)
        {
            DllWrapper dwmapi = new DllWrapper("dwmapi.dll");

            DwmExtendFrameIntoClientArea =
                (DwmExtendFrameIntoClientAreaDelegate)dwmapi.GetFunctionAsDelegate("DwmExtendFrameIntoClientArea", typeof(DwmExtendFrameIntoClientAreaDelegate));

            DwmDefWindowProc =
                (DwmDefWindowProcDelegate)dwmapi.GetFunctionAsDelegate("DwmDefWindowProc", typeof(DwmDefWindowProcDelegate));
        }

        /// <summary>
        /// Creates an instance of the <typeparamref name="RadRibbonFormBehavior"/> class.
        /// </summary>
        /// <param name="treeHandler">The associated <typeparamref name="IComponentTreeHandler"/> implementation.</param>
        /// <param name="shouldHandleCreateChildItems">Determines whether the behavior
        /// handles the CreateChildItems call.</param>
        public RadRibbonFormBehavior(IComponentTreeHandler treeHandler, bool shouldHandleCreateChildItems)
            : base(treeHandler, shouldHandleCreateChildItems)
        {
            DllWrapper dwmapi = new DllWrapper("dwmapi.dll");

            DwmExtendFrameIntoClientArea =
                (DwmExtendFrameIntoClientAreaDelegate)dwmapi.GetFunctionAsDelegate("DwmExtendFrameIntoClientArea", typeof(DwmExtendFrameIntoClientAreaDelegate));

            DwmDefWindowProc =
                (DwmDefWindowProcDelegate)dwmapi.GetFunctionAsDelegate("DwmDefWindowProc", typeof(DwmDefWindowProcDelegate));
        }

        #endregion

        #region Properties

        protected virtual RadRibbonBar RibbonBar
        {
            get
            {
                if (this.radRibbonBar != null
                    && object.ReferenceEquals(this.Form, this.radRibbonBar.Parent)
                    && !(this.radRibbonBar.Disposing || this.radRibbonBar.IsDisposed))
                {
                    return this.radRibbonBar;
                }

                this.radRibbonBar = null;

                foreach (Control ctrl in this.Form.Controls)
                {
                    if (ctrl is RadRibbonBar)
                        return this.radRibbonBar = ctrl as RadRibbonBar;
                }

                return null;
            }
        }

        public override RadElement FormElement
        {
            get
            {
                return this.formElement;
            }
        }

        public override Padding BorderWidth
        {
            get
            {
                return (this.FormElement as RibbonFormElement).BorderThickness;
            }
        }

        public override Padding ClientMargin
        {
            get
            {
                if (!this.CompositionEffectsEnabled
                    || this.Form.IsDesignMode)
                {
                    return this.BorderWidth;
                }
                else
                {
                    return new Padding(
                        SystemInformation.FrameBorderSize.Height,
                        0,
                        SystemInformation.FrameBorderSize.Height,
                        SystemInformation.FrameBorderSize.Height);
                }
            }
        }

        /// <summary>
        /// Returns a zero for the caption height.
        /// </summary>
        public override int CaptionHeight
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets an integer representing the top client margin of 
        /// the form when composition is enabled.
        /// </summary>
        public int TopCompositionMargin
        {
            get
            {
                if (!this.IsMaximized)
                {
                    return SystemInformation.CaptionHeight + SystemInformation.FrameBorderSize.Height;
                }
                else
                {
                    return SystemInformation.CaptionHeight + (2 * SystemInformation.FrameBorderSize.Height);
                }
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether composition effects
        /// are enabled for the form.
        /// </summary>
        public bool CompositionEffectsEnabled
        {
            get
            {
                return this.CompositionEnabled
                    && this.AllowTheming;
            }
        }

        /// <summary>
        /// Gets a value indicating whether composition effects are enabled for the Operating System.
        /// </summary>
        private bool CompositionEnabled
        {
            get
            {
                return this.compositionEnabled;
            }
        }

        /// <summary>
        /// Gets or sets value indicating wether the RadRibbonBar
        /// is drawn over the Aero glass under Vista or later 
        /// versions of Windows.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AllowTheming
        {
            get
            {
                return this.allowTheming;
            }
            set
            {
                this.allowTheming = value;

                if (this.allowTheming)
                {
                    if (this.CompositionEnabled)
                    {
                        this.UpdateFormForThemingState(true);
                    }
                }
                else
                {
                    if (this.CompositionEnabled)
                    {
                        this.UpdateFormForThemingState(false);
                    }
                }
            }
        }

        protected virtual void UpdateFormForThemingState(bool showAero)
        {

            if (this.RibbonBar != null)
            {
                this.RibbonBar.CompositionEnabled = showAero;
            }

            if (this.Form != null)
            {
                this.AdjustBehaviorForCompositionSate();
                this.Form.CallUpdateStyles();
                if (this.CompositionEffectsEnabled)
                {
                    this.ExtendFrameIntoClientArea();
                }
            }
        }

        public override Rectangle MenuBounds
        {
            get
            {
                return Rectangle.Empty;
            }
        }

        public override Rectangle CaptionTextBounds
        {
            get
            {
                if (this.RibbonBar != null)
                {
                    return this.RibbonBar.RibbonBarElement.RibbonCaption.CaptionLayout.CaptionTextElement.ControlBoundingRectangle;
                }
                return Rectangle.Empty;
            }
        }

        public override Rectangle IconBounds
        {
            get
            {
                return Rectangle.Empty;
            }
        }

        public override Rectangle SystemButtonsBounds
        {
            get
            {
                if (this.RibbonBar != null)
                {
                    return this.RibbonBar.RibbonBarElement.RibbonCaption.SystemButtons.ControlBoundingRectangle;
                }
                return Rectangle.Empty;
            }
        }

        #endregion

        #region Methods

        #region Wnd Proc handlers

        private void OnWMActivate(ref Message m)
        {
            base.HandleWndProc(ref m);

            if (this.RibbonBar == null)
            {
                return;
            }

            if ((int)m.WParam == NativeMethods.WA_ACTIVE || (int)m.WParam == NativeMethods.WA_CLICKACTIVE)
            {
                this.RibbonBar.RibbonBarElement.SetValue(RadRibbonBarElement.IsRibbonFormActiveProperty, true);
                this.FormElement.SetValue(RibbonFormElement.IsFormActiveProperty, true);
            }
            else
            {
                this.RibbonBar.RibbonBarElement.SetValue(RadRibbonBarElement.IsRibbonFormActiveProperty, false);
                this.FormElement.SetValue(RibbonFormElement.IsFormActiveProperty, false);
            }
        }

        private void OnWMHitTest(ref Message m)
        {

            if (!this.Form.IsHandleCreated)
            {
                this.CallBaseWndProc(ref m);
                return;
            }
            IntPtr result = new IntPtr();
            bool hittested = DwmDefWindowProc(this.Form.Handle, m.Msg, m.WParam, m.LParam, ref result);

            if (!hittested)
            {
                Point screenPoint = new Point(m.LParam.ToInt32());
                Point hitTestLoc = this.GetMappedWindowPoint(screenPoint);

                if (this.TopResizeFrame.Contains(hitTestLoc))
                {
                    m.Result = new IntPtr(NativeMethods.HTTOP);
                }
                else if (this.RibbonBar != null &&
                    new Rectangle(this.TopLeftResizeFrame.Right, 0, this.RibbonBar.Width, SystemInformation.CaptionHeight).Contains(hitTestLoc))
                {
                    m.Result = new IntPtr(NativeMethods.HTCAPTION);
                }
                else
                {
                    this.CallBaseWndProc(ref m);
                }
            }
            else
            {
                m.Result = result;
            }

        }

        private void UpdateMDIButtonsAndRibbonCaptionText()
        {
            RadRibbonBar ribbonBar = this.RibbonBar;
            if (ribbonBar == null)
            {
                return;
            }

            if (this.IsMdiChildMaximized)
            {
                if (ribbonBar.RibbonBarElement.MDIbutton.Visibility != ElementVisibility.Visible)
                {
                    ribbonBar.RibbonBarElement.MDIbutton.Visibility = ElementVisibility.Visible;
                    ribbonBar.RibbonBarElement.InvalidateMeasure();
                    ribbonBar.RibbonBarElement.InvalidateArrange();
                    ribbonBar.RibbonBarElement.UpdateLayout();
                    ribbonBar.Text = this.Form.Text;
                }
            }
            else
            {
                if (ribbonBar.RibbonBarElement.MDIbutton.Visibility != ElementVisibility.Hidden)
                {
                    ribbonBar.RibbonBarElement.MDIbutton.Visibility = ElementVisibility.Hidden;
                    ribbonBar.RibbonBarElement.InvalidateMeasure();
                    ribbonBar.RibbonBarElement.InvalidateArrange();
                    ribbonBar.RibbonBarElement.UpdateLayout();
                    ribbonBar.Text = this.Form.Text;
                }
            }
        }

        private void OnWMDWMCompositionChanged(ref Message m)
        {
            this.CallBaseWndProc(ref m);
            this.compositionEnabled = DWMAPI.IsCompositionEnabled;

            if (this.RibbonBar != null)
            {
                this.RibbonBar.CompositionEnabled = this.CompositionEffectsEnabled;
            }

            this.AdjustBehaviorForCompositionSate();

            this.Form.CallUpdateStyles();

            if (this.CompositionEffectsEnabled)
            {
                this.ExtendFrameIntoClientArea();
            }
        }


        public override bool HandleWndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == NativeMethods.WM_DWMCOMPOSITIONCHANGED)
            {
                this.OnWMDWMCompositionChanged(ref m);
                return true;
            }
            else if (m.Msg == NativeMethods.WM_ACTIVATE)
            {
                this.OnWMActivate(ref m);
                return true;
            }
            else
            {
                if (this.CompositionEffectsEnabled && !this.Form.IsDesignMode)
                {

                    switch (m.Msg)
                    {
                        case NativeMethods.WM_NCCALCSIZE:
                            {
                                this.UpdateMDIButtonsAndRibbonCaptionText();
                                return base.HandleWndProc(ref m);
                            }
                        case NativeMethods.WM_NCHITTEST:
                            {
                                this.OnWMHitTest(ref m);
                                return true;
                            }
                    }

                    return false;
                }
                else
                {
                    if (m.Msg == NativeMethods.WM_NCCALCSIZE)
                    {
                        this.UpdateMDIButtonsAndRibbonCaptionText();
                    }

                    return base.HandleWndProc(ref m);
                }
            }
        }

        #endregion

        #region Helper methods

        private void InitializeDummyMenuStrip()
        {
            if (this.Form.MainMenuStrip == null && !this.Form.IsDesignMode)
            {
                this.Form.MainMenuStrip = new RadRibbonFormMainMenuStrip();
            }
        }

        private List<object> oldRadRibbonBarElementSettings;

        private void PrepareRibbonBarForCompositionState()
        {
            RadRibbonBar ribbonBar = this.RibbonBar;
            if (ribbonBar == null)
                return;

            if (oldRadRibbonBarElementSettings == null)
            {
                oldRadRibbonBarElementSettings = new List<object>();
            }

            if (this.CompositionEffectsEnabled)
            {
                if (this.oldRadRibbonBarElementSettings.Count == 0)
                {
                    this.oldRadRibbonBarElementSettings.Add(ribbonBar.RibbonBarElement.CaptionFill.Visibility);
                    this.oldRadRibbonBarElementSettings.Add(ribbonBar.RootElement.BackColor);
                    this.oldRadRibbonBarElementSettings.Add(ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Visibility);
                    this.oldRadRibbonBarElementSettings.Add(ribbonBar.QuickAccessToolBarHeight);
                    this.oldRadRibbonBarElementSettings.Add(ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Margin);
                }

                ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Margin =
                    Padding.Add(ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Margin, new Padding(20, 0, 0, 0));
                ribbonBar.RibbonBarElement.CaptionFill.Visibility = ElementVisibility.Hidden;
                ribbonBar.RootElement.BackColor = Color.Transparent;
                ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Visibility = ElementVisibility.Hidden;
                ribbonBar.QuickAccessToolBarHeight = 27;

                if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
                {
                    ribbonBar.RibbonBarElement.RibbonTabStripHolder.Children[0].PositionOffset = new Size(0, 3);
                }
                else
                {
                    ribbonBar.RibbonBarElement.RibbonTabStripHolder.Children[0].PositionOffset = new Size(0, 1);
                }
            }
            else
            {
                if (this.oldRadRibbonBarElementSettings.Count > 0)
                {
                    ribbonBar.RibbonBarElement.CaptionFill.Visibility = (ElementVisibility)this.oldRadRibbonBarElementSettings[0];
                    ribbonBar.RootElement.BackColor = (Color)this.oldRadRibbonBarElementSettings[1];
                    ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Visibility = (ElementVisibility)this.oldRadRibbonBarElementSettings[2];
                    ribbonBar.QuickAccessToolBarHeight = (int)this.oldRadRibbonBarElementSettings[3];
                    ribbonBar.RibbonBarElement.RibbonCaption.SystemButtons.Margin = (Padding)oldRadRibbonBarElementSettings[4];
                    ribbonBar.RibbonBarElement.RibbonTabStripHolder.Children[0].PositionOffset = new Size(0, 0);
                }
            }
        }

        #endregion

        #region Overriden methods

        public override CreateParams CreateParams(CreateParams parameters)
        {
            CreateParams cp = base.CreateParams(parameters);

            if (!this.CompositionEnabled)
            {
                cp.Style &= ~NativeMethods.WS_CAPTION;
            }

            return cp;
        }

        protected override void OnGetMinMaxInfo(MinMaxInfo minMaxInfo)
        {
            minMaxInfo.MinTrackSize = new Size(SystemInformation.MinimizedWindowSize.Width, 100);

            base.OnGetMinMaxInfo(minMaxInfo);
        }

        protected override void OnActiveMDIChildTextChanged()
        {
            base.OnActiveMDIChildTextChanged();

            if (this.RibbonBar != null)
            {
                this.RibbonBar.Text = this.Form.Text;
            }
        }

        //public override bool OnAssociatedFormPaint(PaintEventArgs args)
        //{
        //    if (this.radRibbonBar == null)
        //    {
        //        base.OnAssociatedFormPaint(args);
        //    }

        //    if (!this.CompositionEffectsEnabled)
        //    {
        //        base.OnAssociatedFormPaint(args);
        //    }

        //    int topCompositionMargin = this.TopCompositionMargin;
        //    args.Graphics.Clear(Color.Transparent);
        //    args.Graphics.SetClip(new Rectangle(0, 0, this.Form.Width, topCompositionMargin), System.Drawing.Drawing2D.CombineMode.Exclude);

        //    //using (SolidBrush brush = new SolidBrush(this.Form.BackColor))
        //    //{
        //    //    args.Graphics.FillRectangle(brush, new Rectangle(0, topCompositionMargin, this.Form.Width, this.Form.Height - topCompositionMargin));
        //    //}

        //    return false;
        //}

        public override bool OnAssociatedFormPaintBackground(PaintEventArgs args)
        {
            if (this.radRibbonBar == null)
            {
                base.OnAssociatedFormPaintBackground(args);
            }

            if (!this.CompositionEffectsEnabled)
            {
                base.OnAssociatedFormPaintBackground(args);
            }

            int topCompositionMargin = this.TopCompositionMargin;
            //args.Graphics.Clear(Color.Transparent);
            args.Graphics.SetClip(new Rectangle(0, 0, this.Form.Width, topCompositionMargin), System.Drawing.Drawing2D.CombineMode.Exclude);

            //using (SolidBrush brush = new SolidBrush(this.Form.BackColor))
            //{
            //    args.Graphics.FillRectangle(brush, new Rectangle(0, topCompositionMargin, this.Form.Width, this.Form.Height - topCompositionMargin));
            //}

            return false;
        }

        protected override void OnFormAssociated()
        {
            base.OnFormAssociated();

            this.Form.SizeChanged += new EventHandler(Form_SizeChanged);
            this.Form.ThemeNameChanged += new ThemeNameChangedEventHandler(Form_ThemeNameChanged);

            this.InitializeDummyMenuStrip();

            this.compositionEnabled = DWMAPI.IsCompositionEnabled;

            if (!this.Form.IsHandleCreated)
            {
                this.Form.Load += new EventHandler(Form_Load);
            }
            else
            {
                this.InitializeRibbon();

                if (!this.Form.IsDesignMode && this.FormElement != null)
                {
                    this.AdjustBehaviorForCompositionSate();

                    if (this.CompositionEffectsEnabled)
                    {
                        this.ExtendFrameIntoClientArea();
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Form.SizeChanged -= new EventHandler(Form_SizeChanged);
                this.Form.Load -= new EventHandler(Form_Load);

                if (this.Form.MainMenuStrip is RadRibbonFormMainMenuStrip)
                {
                    this.Form.MainMenuStrip.Dispose();
                    this.Form.MainMenuStrip = null;
                }
            }

            base.Dispose(disposing);
        }


        public override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.formElement = new RibbonFormElement();

            parent.Children.Add(this.formElement);
        }


        #endregion

        private void ResetFormRegion()
        {
            if (!this.Form.IsHandleCreated && !this.Form.IsDesignMode)
                return;

            if (this.Form.WindowState == FormWindowState.Maximized
                && this.Form.MaximumSize == Size.Empty)
            {
                int formOffset;

                if (!this.CompositionEnabled)
                {
                    formOffset = SystemInformation.FixedFrameBorderSize.Height;
                }
                else
                {
                    formOffset = SystemInformation.FrameBorderSize.Height;
                }

                Rectangle boundsRect = new Rectangle(
                    new Point(formOffset, formOffset),
                    new Size(this.Form.Size.Width - (formOffset * 2), this.Form.Size.Height - (formOffset * 2)));

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddRectangle(boundsRect);
                    this.Form.Region = new Region(path);
                }
            }
            else
            {

                Region region = NativeMethods.CreateRoundRectRgn(
                    new Rectangle(0, 0, this.Form.Width, this.Form.Height), 9);
                this.Form.Region = region;

            }
        }

        private void ExtendFrameIntoClientArea()
        {
            if (!this.Form.IsHandleCreated)
            {
                return;
            }

            NativeMethods.MARGINS margins = new NativeMethods.MARGINS();

            margins.cyTopHeight = this.TopCompositionMargin;
            DwmExtendFrameIntoClientArea(this.Form.Handle, ref margins);
        }

        private void InitializeRibbon()
        {
            foreach (Control control in this.Form.Controls)
            {
                if (control is RadRibbonBar)
                {
                    this.radRibbonBar = control as RadRibbonBar;

                    this.radRibbonBar.CompositionEnabled = this.CompositionEffectsEnabled;

                    break;
                }
            }
        }

        /// <summary>
        /// This method adjusts the form's element tree according to the composition state
        /// of MS Windows. If Composition is enabled, the element tree is hidden and the glass
        /// effects of the form are visible.
        /// </summary>
        private void AdjustBehaviorForCompositionSate()
        {
            RibbonFormElement ribbonFormElement = this.FormElement as RibbonFormElement;

            if (this.CompositionEffectsEnabled && !this.Form.IsDesignMode && this.Form.TopLevel)
            {
                if (ribbonFormElement.Visibility != ElementVisibility.Collapsed)
                {
                    ribbonFormElement.Visibility = ElementVisibility.Collapsed;
                    this.PrepareRibbonBarForCompositionState();
                    this.Form.RootElement.Shape = null;
                    this.Form.Region = null;
                }
            }
            else
            {
                if (ribbonFormElement.Visibility != ElementVisibility.Visible)
                {
                    ribbonFormElement.Visibility = ElementVisibility.Visible;
                    this.Form.Padding = Padding.Empty;
                    this.PrepareRibbonBarForCompositionState();
                    this.Form.RootElement.ResetValue(RadItem.ShapeProperty, ValueResetFlags.Local);
                }
            }


        }

        #endregion

        #region Event handling

        private void Form_Load(object sender, EventArgs e)
        {
            this.InitializeRibbon();
            this.AdjustBehaviorForCompositionSate();
            if (this.CompositionEffectsEnabled)
            {
                this.ExtendFrameIntoClientArea();
            }
        }


        private FormWindowState oldWindowState;

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            if (this.CompositionEnabled)
            {
                if (this.IsMaximized)
                {
                    if (this.AllowTheming)
                    {
                        this.ExtendFrameIntoClientArea();
                    }

                    this.Form.Padding = new Padding(0, SystemInformation.FrameBorderSize.Height, 0, 0);
                }
                else if (oldWindowState != this.Form.WindowState)
                {

                    if (this.AllowTheming)
                    {
                        this.ExtendFrameIntoClientArea();
                    }
                    this.Form.Padding = Padding.Empty;
                }
            }

            if (oldWindowState != this.Form.WindowState)
            {

                if (this.RibbonBar != null)
                {
                    this.RibbonBar.RibbonBarElement.SetValue(RadRibbonBarElement.RibbonFormWindowStateProperty, this.Form.WindowState);
                }
            }

            this.oldWindowState = this.Form.WindowState;

            if (!this.CompositionEffectsEnabled || this.Form.IsDesignMode)
            {
                this.ResetFormRegion();
            }
        }

        private void Form_ThemeNameChanged(object source, ThemeNameChangedEventArgs args)
        {
            this.RefreshNC();
        }

        #endregion
    }
}
