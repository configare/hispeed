using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A standard behavior for RadForm.
    /// </summary>
    public class RadFormBehavior : ThemedFormBehavior
    {
        #region Form elements
        private RadFormElement formElement;

        #endregion

        #region Fields

        private bool allowTheming = true;
        private bool isMdiStripMeasured = false;
        private ScrollBarThumb scrollThumbCapture = null;

        private bool isMouseTracking = false;
        private System.Diagnostics.Stopwatch dblClickStopwatch;

        private MdiClient mdiClient;
        private RadElement capturedNCItem;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// This instance has no Form associated with it.
        /// </summary>
        public RadFormBehavior()
        : base()
        {
        }

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// </summary>
        /// <param name="treeHandler">An IComponentTreeHandler instance.</param>
        public RadFormBehavior(IComponentTreeHandler treeHandler) :
        base(treeHandler)
        {
        }

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// </summary>
        /// <param name="treeHandler">An IComponentTreeHandler instance.</param>
        /// <param name="shouldCreateChildren">A flag that determines whether the CreateChildItems
        /// call is rerouted to the behavior.</param>
        public RadFormBehavior(IComponentTreeHandler treeHandler, bool shouldCreateChildren) :
        base(treeHandler, shouldCreateChildren)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounding rectangle of the sizing grip that appears
        /// when both scrollbars are visible.
        /// </summary>
        protected virtual Rectangle ScrollbarSizingGripBounds
        {
            get
            {
                return this.formElement.ScrollBarsFormSizingGrip.ControlBoundingRectangle;
            }
        }

        /// <summary>
        /// Gets the bounding rectangle of the horizontal scrollbar.
        /// Returns an empty rectangle if the scrollbar is not visible.
        /// </summary>
        protected virtual Rectangle HorizontalScrollbarBounds
        {
            get
            {
                return this.SynchronizeHorizontalScrollbarState();
            }
        }

        /// <summary>
        /// Gets the bounding rectangle of the vertical scrollbar.
        /// Returns an empty rectangle if the scrollbar is not visible.
        /// </summary>
        protected virtual Rectangle VerticalScrollbarBounds
        {
            get
            {
                return this.SynchronizeVerticalScrollbarState();
            }
        }

        public override Rectangle CaptionTextBounds
        {
            get
            {
                return this.formElement.TitleBar.TitlePrimitive.ControlBoundingRectangle;
            }
        }

        public override Rectangle IconBounds
        {
            get
            {
                return this.formElement.TitleBar.IconPrimitive.ControlBoundingRectangle;
            }
        }

        public override Rectangle SystemButtonsBounds
        {
            get
            {
                return this.formElement.TitleBar.SystemButtons.ControlBoundingRectangle;
            }
        }

        public override Rectangle MenuBounds
        {
            get
            {
                return this.formElement.MdiControlStrip.ControlBoundingRectangle;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating
        /// whether the behavior paints in the NC area of the
        /// Form when OS is Vista or later and Composition is enabled.
        /// </summary>
        public override bool AllowTheming
        {
            get
            {
                return this.allowTheming;
            }
            set
            {
                if (value != this.allowTheming)
                {
                    this.allowTheming = value;

                    if (this.FormElement != null)
                    {
                        this.AdjustFormForThemingState();
                    }
                }
            }
        }

        public override RadElement FormElement
        {
            get
            {
                return this.formElement;
            }
        }

        /// <summary>
        /// Gets the Caption Height of the Form.
        /// </summary>
        public override int CaptionHeight
        {
            get
            {
                if (this.Form.IsMdiChild && !this.IsMaximized
                    && (this.FormElement as RadFormElement).TitleBar.Visibility != ElementVisibility.Visible)
                {
                    (this.FormElement as RadFormElement).TitleBar.Visibility = ElementVisibility.Visible;
                }

                return (int)(this.FormElement as RadFormElement).TitleBar.DesiredSize.Height;
            }
        }

        /// <summary>
        /// Gets the Border width of the Form.
        /// </summary>
        public override Padding BorderWidth
        {
            get
            {
                return(this.FormElement as RadFormElement).BorderWidth;
            }
        }

        /// <summary>
        /// Gets the margin that determines the position and size of the client
        /// area of the Form.
        /// </summary>
        public override Padding ClientMargin
        {
            get
            {
                Padding clientMargin = this.CalculateDynamicClientMargin();

                if (this.Form.RightToLeft == RightToLeft.Yes)
                {
                    clientMargin = new Padding(
                        clientMargin.Left + this.VerticalScrollbarBounds.Width,
                        clientMargin.Top,
                        clientMargin.Right,
                        clientMargin.Bottom);
                }
                else if (this.Form.RightToLeft == RightToLeft.No)
                {
                    clientMargin = new Padding(
                        clientMargin.Left,
                        clientMargin.Top,
                        clientMargin.Right + this.VerticalScrollbarBounds.Width,
                        clientMargin.Bottom);
                }

                clientMargin = Padding.Add(clientMargin, new Padding(0, 0, 0, this.HorizontalScrollbarBounds.Height));

                return clientMargin;
            }
        }

        #endregion

        #region Methods

        #region Scrolling

        private void SynchronizeSizeGrip()
        {
            RadFormElement formElement = this.FormElement as RadFormElement;
            if (formElement == null)
                return;
            if (this.Form.VerticalScroll.Visible && this.Form.HorizontalScroll.Visible)
            {
                if (formElement.ScrollBarsFormSizingGrip.Visibility != ElementVisibility.Visible)
                {
                    formElement.ScrollBarsFormSizingGrip.Visibility = ElementVisibility.Visible;
                }
            }
            else
            {
                if (formElement.ScrollBarsFormSizingGrip.Visibility != ElementVisibility.Collapsed)
                {
                    formElement.ScrollBarsFormSizingGrip.Visibility = ElementVisibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle of the vertical scrollbar.
        /// </summary>
        /// <returns>An instance of the Rectangle struct that represents
        /// the position and the size of the vertical scrollbar</returns>
        protected virtual Rectangle SynchronizeVerticalScrollbarState()
        {
            RadScrollBarElement scrollBar = (this.FormElement as RadFormElement).VerticalScrollbar;

            if (this.Form.VerticalScroll.Visible)
            {
                if (scrollBar.Visibility != ElementVisibility.Visible)
                {
                    scrollBar.Visibility = ElementVisibility.Visible;
                }
                return scrollBar.ControlBoundingRectangle;
            }
            else
            {
                if (scrollBar.Visibility == ElementVisibility.Visible)
                {
                    scrollBar.Visibility = ElementVisibility.Collapsed;
                }
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Calculates the bounding rectangle of the horizontal scrollbar.
        /// </summary>
        /// <returns>An instance of the Rectangle struct that represents
        /// the position and the size of the horizontal scrollbar.</returns>
        protected virtual Rectangle SynchronizeHorizontalScrollbarState()
        {
            RadScrollBarElement scrollBar = (this.FormElement as RadFormElement).HorizontalScrollbar;

            if (this.Form.HorizontalScroll.Visible)
            {
                if (scrollBar.Visibility != ElementVisibility.Visible)
                {
                    scrollBar.Visibility = ElementVisibility.Visible;
                }
                return scrollBar.ControlBoundingRectangle;
            }
            else
            {
                if (scrollBar.Visibility == ElementVisibility.Visible)
                {
                    scrollBar.Visibility = ElementVisibility.Collapsed;
                }
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// This method synchronizes the values of the standard vertical scrollbar
        /// with the RadScrollBarElement in the Form's element hierarchy.
        /// </summary>
        protected virtual void SynchronizeVerticalScrollbarValues()
        {
            if (!this.Form.VerticalScroll.Visible)
                return;

            RadScrollBarElement scrollBar = (this.FormElement as RadFormElement).VerticalScrollbar;
            scrollBar.Minimum = this.Form.VerticalScroll.Minimum;
            scrollBar.Maximum = this.Form.VerticalScroll.Maximum;
            
            if ( this.Form.VerticalScroll.SmallChange < 0 )
                scrollBar.SmallChange = 0;
            else
                scrollBar.SmallChange = this.Form.VerticalScroll.SmallChange;

            if (this.Form.VerticalScroll.LargeChange < 0)
                scrollBar.LargeChange = 0;
            else
                scrollBar.LargeChange = this.Form.VerticalScroll.LargeChange;

            scrollBar.Value = this.Form.VerticalScroll.Value;
        }

        /// <summary>
        /// This method synchronizes the values of the standard horizontal scrollbar
        /// with the RadScrollBarElement in the Form's element hierarchy.
        /// </summary>
        protected virtual void SynchronizeHorizontalScrollbarValues()
        {
            if (!this.Form.HorizontalScroll.Visible)
                return;

            RadScrollBarElement scrollBar = (this.FormElement as RadFormElement).HorizontalScrollbar;
            scrollBar.Minimum = this.Form.HorizontalScroll.Minimum;
            scrollBar.Maximum = this.Form.HorizontalScroll.Maximum;
            scrollBar.SmallChange = this.Form.HorizontalScroll.SmallChange;
            scrollBar.LargeChange = this.Form.HorizontalScroll.LargeChange;
            scrollBar.Value = this.Form.HorizontalScroll.Value;
        }

        #region Scrolling events

        private void VerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.Form.VerticalScroll.Value = e.NewValue;
        }

        private void HorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.Form.HorizontalScroll.Value = e.NewValue;
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets the Caption Height according to the Styles applied and
        /// the current state of the form.
        /// </summary>
        /// <returns>A value representing the height of the caption.</returns>
        protected virtual int GetDynamicCaptionHeight()
        {
            int result = 0;

            if (!this.IsMaximized)
            {
                if (this.Form.FormBorderStyle != FormBorderStyle.None)
                {
                    result = (this.CaptionHeight
                              + this.BorderWidth.Top
                              + (this.FormElement as RadFormElement).TitleBar.Margin.Vertical);
                }
            }
            else
            {
                if (!DWMAPI.IsCompositionEnabled)
                {
                    //If there is a MaximumSize set to the Form,
                    //do not add the FrameBorderSize to the Caption since the
                    //Form should appear as if in normal state when maximized
                    result = (this.CaptionHeight + 
                              (this.Form.MaximumSize == Size.Empty ?
                               SystemInformation.FixedFrameBorderSize.Height : 0));
                }
                else
                {
                    //If there is a MaximumSize set to the Form,
                    //do not add the FrameBorderSize to the Caption since the
                    //Form should appear as if in normal state when maximized
                    result = (this.CaptionHeight + 
                              (this.Form.MaximumSize == Size.Empty ?
                               SystemInformation.FrameBorderSize.Height : 0));
                }
            }
            //If MDI menu should be displayed, include its height.
            result += this.GetAdjustedMDIStripHeight();

            return result;
        }

        /// <summary>
        /// Gets a Padding object which represents the window NC margin according
        /// to the currently set styles.
        /// </summary>
        /// <returns>The form NC margin.</returns>
        protected virtual Padding GetWindowRealNCMargin()
        {
            NativeMethods.RECT boundsRect = new NativeMethods.RECT();
            Rectangle screenClient = this.Form.RectangleToScreen(this.Form.ClientRectangle);
            boundsRect.left = screenClient.Left;
            boundsRect.top = screenClient.Top;
            boundsRect.right = screenClient.Right;
            boundsRect.bottom = screenClient.Bottom;
            CreateParams cp = this.CurrentFormParams;

            bool res = NativeMethods.AdjustWindowRectEx(ref boundsRect,
                                                            cp.Style,
                                                            this.Form.MainMenuStrip != null,
                                                            cp.ExStyle);

            Padding result = Padding.Empty;

            if (res)
            {
                result = new Padding(
                    screenClient.Left - boundsRect.left,
                    screenClient.Top - boundsRect.top,
                    boundsRect.right - screenClient.Right,
                    boundsRect.bottom - screenClient.Bottom);
            }

            return result;
        }

        private int GetAdjustedMDIStripHeight()
        {
            int result = 0;

            if (this.IsMdiChildMaximized)
            {
                if (!this.IsMenuInForm)
                {
                    if ((this.FormElement as RadFormElement).MdiControlStrip.Visibility != ElementVisibility.Visible
                        || !this.isMdiStripMeasured)
                    {
                        (this.FormElement as RadFormElement).MdiControlStrip.Visibility = ElementVisibility.Visible;

                        (this.FormElement as RadFormElement).MdiControlStrip.ActiveMDIChild = this.MaximizedMDIChild;
                        (this.FormElement as RadFormElement).MdiControlStrip.Measure(this.Form.Size);
                        this.isMdiStripMeasured = true;
                    }

                    result += (int)(this.FormElement as RadFormElement).MdiControlStrip.DesiredSize.Height;
                }

                if ((this.FormElement as RadFormElement).TitleBar.Text != this.Form.Text)
                {
                    (this.FormElement as RadFormElement).TitleBar.Text = this.Form.Text;
                }

                if ((this.FormElement as RadFormElement).TitleBar.Text != this.Form.Text)
                {
                    (this.FormElement as RadFormElement).TitleBar.Text = this.Form.Text;
                }
            }
            else
            {
                if ((this.FormElement as RadFormElement).MdiControlStrip.Visibility == ElementVisibility.Visible)
                {
                    (this.FormElement as RadFormElement).MdiControlStrip.Visibility = ElementVisibility.Collapsed;
                }

                if ((this.FormElement as RadFormElement).TitleBar.Text != this.Form.Text)
                {
                    (this.FormElement as RadFormElement).TitleBar.Text = this.Form.Text;
                }

                this.isMdiStripMeasured = false;
            }

            return result;
        }

        /// <summary>
        /// Calculates the client margin based on the current form and form element settings
        /// </summary>
        /// <returns></returns>
        public virtual Padding CalculateDynamicClientMargin()
        {
            Padding result = new Padding();
          
            if (!this.IsMaximized || (this.IsMaximized && this.Form.MaximumSize != Size.Empty))
            {
                if (this.Form.FormBorderStyle != FormBorderStyle.None)
                {
                    result = new Padding(
                        this.BorderWidth.Left,
                        this.GetDynamicCaptionHeight(),
                        this.BorderWidth.Right,
                        this.BorderWidth.Bottom);

                    if (this.Form.IsMdiChild && this.IsMinimized)
                    {
                        result = new Padding(
                            this.BorderWidth.Left,
                            this.Form.Size.Height,
                            this.BorderWidth.Right,
                            this.BorderWidth.Bottom);
                    }
                }
                else
                {
                    result = Padding.Empty;
                }
            }
            else
            {
                Padding realNCMargin = this.GetWindowRealNCMargin();

                result = new Padding(
                    realNCMargin.Left,
                    this.GetDynamicCaptionHeight(),
                    realNCMargin.Right,
                    realNCMargin.Bottom);

                if (this.Form.IsMdiChild)
                {
                    int topMargin = realNCMargin.Top;

                    //If the MDI Child has a MainMenuStrip assigned, removed the height
                    //of a standard windows menu from the top margin otherwise
                    //the mdi child is wrongly positioned when maximized.
                    if (this.Form.MainMenuStrip != null)
                    {
                        topMargin -= SystemInformation.MenuHeight;
                    }

                    result = new Padding(result.Left, topMargin, result.Right, result.Bottom);
                }
            }
            return result;
        }

        private void TrackMouseLeaveMessage()
        {
            if (this.isMouseTracking || !this.Form.IsHandleCreated)
            {
                return;
            }
            NativeMethods.TRACKMOUSEEVENT track = new NativeMethods.TRACKMOUSEEVENT();
            track.dwFlags = NativeMethods.TME_HOVER | NativeMethods.TME_LEAVE | NativeMethods.TME_NONCLIENT; //0x13; 
            track.hwndTrack = this.Form.Handle;
            if (!NativeMethods._TrackMouseEvent(track))
            {
                return;
            }
            this.isMouseTracking = true;
        }

        /// <summary>
        /// This method translates a point which is in client coordinates
        /// to a point in NC coordinates. Used when the Form has captured the mouse
        /// and NC mouse events have to be processed.
        /// </summary>
        /// <param name="clientCoordinates">A point in client coordinates.</param>
        /// <returns>The point in NC coordinates.</returns>
        protected virtual Point NCFromClientCoordinates(Point clientCoordinates)
        {
            clientCoordinates.Offset(this.ClientMargin.Left, this.ClientMargin.Top);

            return clientCoordinates;
        }

        //This method returns true for each two mouse clicks
        //which are in a time span of 500 milliseconds.
        //Call the method once to mark the start of a double click
        //operation. The return value of the second call shows whether a double click
        //has been marked.
        //The method returns null upon first call, otherwise true or false;
        private bool? MarkDoubleClickStart()
        {
            if (!this.dblClickStopwatch.IsRunning)
            {
                this.dblClickStopwatch.Reset();
                this.dblClickStopwatch.Start();
            }
            else
            {
                this.dblClickStopwatch.Stop();
                if (this.dblClickStopwatch.ElapsedMilliseconds < SystemInformation.DoubleClickTime)
                {
                    return true;
                }
            }

            return null;
        }

        /// <summary>
        /// Fires when a Form has been associated with the behavior.
        /// </summary>
        protected override void OnFormAssociated()
        {
            base.OnFormAssociated();

            if (this.formElement == null && this.Form.RootElement.Children.Count > 0)
            {
                this.formElement = this.Form.RootElement.Children[0] as RadFormElement;
            }

            if (!this.Form.IsHandleCreated)
            {
                this.Form.Load += new EventHandler(Form_Load);
                this.Form.ControlAdded += new ControlEventHandler(OnForm_ControlAdded);
                this.Form.ControlRemoved += new ControlEventHandler(OnForm_ControlRemoved);
                this.Form.RightToLeftChanged += new EventHandler(OnForm_RightToLeftChanged);
                this.Form.ThemeNameChanged += new ThemeNameChangedEventHandler(OnForm_ThemeNameChanged);
                this.Form.SizeChanged += new EventHandler(Form_SizeChanged);
            }
            else
            {
                foreach (Control control in this.Form.Controls)
                {
                    if (control is MdiClient)
                    {
                        this.mdiClient = control as MdiClient;
                        return;
                    }
                }
            }

            this.dblClickStopwatch = new System.Diagnostics.Stopwatch();
        }

        /// <summary>
        /// This method adjusts the Element tree according
        /// to the current AllowTheming property value.
        /// If AllowTheming is true, the Element tree is painted,
        /// otherwise not.
        /// </summary>
        private void AdjustFormForThemingState()
        {
            if (!this.AllowTheming)
            {
                this.FormElement.Visibility = ElementVisibility.Collapsed;
                this.Form.RootElement.SetValue(RootRadElement.ApplyShapeToControlProperty, false);
                this.Form.Region = null;
                this.Form.CallUpdateStyles();
            }
            else
            {
                this.FormElement.Visibility = ElementVisibility.Visible;
                this.Form.RootElement.SetValue(RootRadElement.ApplyShapeToControlProperty, true);
                this.Form.CallUpdateStyles();
            }
        }

        /// <summary>
        /// Changes the visibility of the separate items within the RadFormElement's element tree
        /// according to the current Form state.
        /// </summary>
        /// <param name="formState">The state of the Form as it comes from the WM_SIZE message</param>
        protected void AdjustFormElementForFormState(int? formState)
        {
            RadFormElement radFormElement = this.FormElement as RadFormElement;
            //Change the FormElement settings only when no maximum size is set.
            if (formState == NativeMethods.SIZE_MAXIMIZED)
            {
                radFormElement.SetValue(RadFormElement.FormWindowStateProperty, FormWindowState.Maximized);

                if (this.Form.MaximumSize == Size.Empty)
                {
                    if (!this.Form.IsMdiChild)
                    {
                        radFormElement.Border.Visibility = ElementVisibility.Collapsed;
                        radFormElement.ImageBorder.Visibility = ElementVisibility.Collapsed;
                    }

                    int result;

                    if (!DWMAPI.IsCompositionEnabled)
                    {
                        result = SystemInformation.FixedFrameBorderSize.Height;
                    }
                    else
                    {
                        result = SystemInformation.FrameBorderSize.Height;
                    }
                    
                    if (this.Form.IsMdiChild)
                    {
                        if (this.Form.AutoScroll && DWMAPI.IsCompositionEnabled)
                        {
                            radFormElement.Margin = new Padding(
                                radFormElement.BorderWidth.Left,
                                -radFormElement.BorderWidth.Top,
                                radFormElement.BorderWidth.Right,
                                radFormElement.BorderWidth.Bottom);
                        }
                        else
                        {
                            radFormElement.Margin = new Padding(
                                0,
                                -radFormElement.TitleBar.ControlBoundingRectangle.Height,
                                -1,
                                -1);
                        }
                    }
                    else
                    {
                        radFormElement.Margin = new Padding(result);
                    }
                }
            }

            if (formState == NativeMethods.SIZE_MINIMIZED)
            {
                radFormElement.SetValue(RadFormElement.FormWindowStateProperty, FormWindowState.Minimized);
                radFormElement.Border.Visibility = ElementVisibility.Visible;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Visible;

                if (this.Form.IsMdiChild)
                {
                    radFormElement.TitleBar.Visibility = ElementVisibility.Visible;
                }

                radFormElement.Margin = Padding.Empty;
            }

            if (formState == NativeMethods.SIZE_RESTORED)
            {
                radFormElement.SetValue(RadFormElement.FormWindowStateProperty, FormWindowState.Normal);
                radFormElement.Border.Visibility = ElementVisibility.Visible;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Visible;

                radFormElement.Margin = Padding.Empty;

                if (this.Form.IsMdiChild)
                {
                    radFormElement.TitleBar.Visibility = ElementVisibility.Visible;
                }
            }
        }

        /// <summary>
        /// This method adjusts the FormElement according to the styles currently applied to the Form.
        /// For example, the MinimizeButton, MaximizeButton and CloseButton in the Title Bar are enabled/disabled
        /// according to whether the corresponding window style is applied.
        /// </summary>
        protected void AdjustFormElementForCurrentStyles()
        {
            RadFormElement radFormElement = this.FormElement as RadFormElement;

            this.AdjustSystemButtonsForStyle();

            if (this.Form.FormBorderStyle == FormBorderStyle.FixedToolWindow
                || this.Form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                radFormElement.TitleBar.Visibility = ElementVisibility.Visible;
                radFormElement.Border.Visibility = ElementVisibility.Visible;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Visible;

                this.Form.RootElement.ResetValue(RadItem.ShapeProperty, ValueResetFlags.Local);
            }
            else if (this.Form.FormBorderStyle == FormBorderStyle.Sizable)
            {
                radFormElement.TitleBar.Visibility = ElementVisibility.Visible;
                radFormElement.Border.Visibility = ElementVisibility.Visible;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Visible;

                this.Form.RootElement.ResetValue(RadItem.ShapeProperty, ValueResetFlags.Local);
            }
            else if (this.Form.FormBorderStyle == FormBorderStyle.FixedDialog
                     || this.Form.FormBorderStyle == FormBorderStyle.FixedSingle
                     || this.Form.FormBorderStyle == FormBorderStyle.Fixed3D)
            {
                radFormElement.TitleBar.Visibility = ElementVisibility.Visible;
                radFormElement.Border.Visibility = ElementVisibility.Visible;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Visible;

                this.Form.RootElement.ResetValue(RadItem.ShapeProperty, ValueResetFlags.Local);
            }
            else if (this.Form.FormBorderStyle == FormBorderStyle.None)
            {
                radFormElement.TitleBar.Visibility = ElementVisibility.Collapsed;
                radFormElement.Border.Visibility = ElementVisibility.Collapsed;
                radFormElement.ImageBorder.Visibility = ElementVisibility.Collapsed;
                this.Form.RootElement.SetValue(RadItem.ShapeProperty, null);
                this.Form.Region = null;

                this.Form.Invalidate();
            }
        }

        private void AdjustSystemButtonsForStyle()
        {
            RadFormElement radFormElement = this.FormElement as RadFormElement;

            if (this.Form.FormBorderStyle == FormBorderStyle.FixedToolWindow
                || this.Form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                radFormElement.TitleBar.MinimizeButton.Visibility = ElementVisibility.Hidden;
                radFormElement.TitleBar.MaximizeButton.Visibility = ElementVisibility.Hidden;
                radFormElement.TitleBar.IconPrimitive.Visibility = ElementVisibility.Collapsed;
            }
            else if (this.Form.FormBorderStyle == FormBorderStyle.Sizable
                     || this.Form.FormBorderStyle == FormBorderStyle.FixedDialog
                     || this.Form.FormBorderStyle == FormBorderStyle.FixedSingle
                     || this.Form.FormBorderStyle == FormBorderStyle.Fixed3D)
            {
                radFormElement.TitleBar.SystemButtons.Visibility = 
                this.Form.ControlBox ? ElementVisibility.Visible : ElementVisibility.Hidden;
                radFormElement.TitleBar.IconPrimitive.Visibility = this.Form.ShowIcon && this.Form.ControlBox
                                                                   ? ElementVisibility.Visible : ElementVisibility.Collapsed;

                if (this.Form.ControlBox)
                {
                    radFormElement.TitleBar.MaximizeButton.Enabled = this.Form.MaximizeBox;
                    radFormElement.TitleBar.MaximizeButton.Visibility =
                    this.Form.MinimizeBox || this.Form.MaximizeBox ? ElementVisibility.Visible : ElementVisibility.Hidden;

                    radFormElement.TitleBar.MinimizeButton.Enabled = this.Form.MinimizeBox;
                    radFormElement.TitleBar.MinimizeButton.Visibility =
                    (this.Form.MaximizeBox || this.Form.MinimizeBox) ? ElementVisibility.Visible : ElementVisibility.Hidden;

                    if (this.Form.FormBorderStyle == FormBorderStyle.FixedDialog)
                    {
                        radFormElement.TitleBar.IconPrimitive.Visibility = ElementVisibility.Collapsed;
                    }
                    else
                    {
                        radFormElement.TitleBar.IconPrimitive.Visibility =
                        this.Form.ShowIcon ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                    }
                }
            }
        }

        #region Overriden methods

        public override CreateParams CreateParams(CreateParams parameters)
        {
            CreateParams cp = base.CreateParams(parameters);

            if (!this.Form.MinimizeBox)
            {
                cp.Style &= ~NativeMethods.WS_MINIMIZEBOX;
            }

            if (!this.Form.MaximizeBox)
            {
                cp.Style &= ~NativeMethods.WS_MAXIMIZEBOX;
            }

            if (!this.Form.ControlBox)
            {
                cp.Style &= ~NativeMethods.WS_MINIMIZEBOX;
                cp.Style &= ~NativeMethods.WS_MAXIMIZEBOX;
            }

            if (this.Form.ShowInTaskbar)
            {
                cp.ExStyle |= NativeMethods.WS_EX_APPWINDOW;
            }
            else
            {
                cp.ExStyle &= ~NativeMethods.WS_EX_APPWINDOW;
            }

            if (this.Form.FormBorderStyle == FormBorderStyle.None)
            {
                cp.Style &= ~NativeMethods.WS_CAPTION;
                cp.Style &= ~NativeMethods.WS_THICKFRAME;
                cp.Style &= ~NativeMethods.WS_BORDER;
                cp.Style &= ~NativeMethods.WS_MINIMIZEBOX;
                cp.Style &= ~NativeMethods.WS_MAXIMIZEBOX;
            }

            cp.Style &= ~NativeMethods.WS_HSCROLL;
            cp.Style &= ~NativeMethods.WS_VSCROLL;

            return cp;
        }

        protected override void OnActiveMDIChildTextChanged()
        {
            base.OnActiveMDIChildTextChanged();

            (this.FormElement as RadFormElement).TitleBar.Text = this.Form.Text;
        }

        protected override void OnWindowStateChanged(int currentFormState, int newFormState)
        {
            base.OnWindowStateChanged(currentFormState, newFormState);
            this.AdjustFormElementForFormState(newFormState);
            this.stateChanged1 = true;
        }

        public override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.formElement = new RadFormElement();
            parent.Children.Add(this.formElement);
            this.formElement.TitleBar.Text = this.Form.Text;
            this.formElement.MdiControlStrip.MinimizeButton.Click += new EventHandler(MinimizeButton_Click);
            this.formElement.MdiControlStrip.MaximizeButton.Click += new EventHandler(MaximizeButton_Click);
            this.formElement.MdiControlStrip.CloseButton.Click += new EventHandler(CloseButton_Click);
            //The icon of the title bar handles mouse input since when clicked, shows
            //the system menu of the form.
            this.formElement.TitleBar.IconPrimitive.ShouldHandleMouseInput = true;

            this.formElement.HorizontalScrollbar.Scroll += new ScrollEventHandler(HorizontalScrollbar_Scroll);
            this.formElement.VerticalScrollbar.Scroll += new ScrollEventHandler(VerticalScrollbar_Scroll);
            this.formElement.MdiControlStrip.MaximizedMdiIcon.ShouldHandleMouseInput = true;
        }

        public override void InvalidateElement(RadElement element, Rectangle bounds)
        {
            this.InvalidateNC(bounds);
        }

        public override bool HandleWndProc(ref System.Windows.Forms.Message m)
        {
            if (!this.AllowTheming)
            {
                return false;
            }

            switch (m.Msg)
            {
                case NativeMethods.WM_MOUSEWHEEL:
                    {
                        this.OnWMMouseWheel(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCHITTEST:
                    {
                        this.OnWMNCHittest(ref m);
                        return true;
                    }
                case NativeMethods.WM_ACTIVATE:
                    {
                        this.OnWMActivate(ref m);
                        return true;
                    }
                case NativeMethods.WM_MDIACTIVATE:
                    {
                        this.OnWmMDIActivate(ref m);
                        return true;
                    }
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                    {
                        this.CallBaseWndProc(ref m);
                        this.OnWMDWMCompositionChanged(ref m);
                        break;
                    }
                case NativeMethods.WM_STYLECHANGED:
                    {
                        this.OnWMStyleChanged(ref m);
                        return true;
                    }
                case NativeMethods.WM_SETICON:
                    {
                        this.OnWmSetIcon(ref m);
                        return true;
                    }
                case NativeMethods.WM_SETTEXT:
                    {
                        this.OnWmSetText(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCMOUSELEAVE:
                    {
                        this.OnWMNCMouseLeave(ref m);
                        break;
                    }
                case NativeMethods.WM_NCMOUSEMOVE:
                    {
                        this.OnWMNCMouseMove(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCLBUTTONDOWN:
                    {
                        this.OnWmNCLeftMouseButtonDown(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCLBUTTONDBLCLK:
                    {
                        this.OnWMNCLButtonDblClk(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCLBUTTONUP:
                    {
                        this.OnWmNCLeftMouseButtonUp(ref m);
                        return true;
                    }
                case NativeMethods.WM_NCRBUTTONUP:
                    {
                        this.OnWMNCRightButtonUp(ref m);
                        return true;
                    }
                case NativeMethods.WM_MOUSEMOVE:
                    {
                        this.OnWmMouseMove(ref m);
                        return true;
                    }
                case NativeMethods.WM_LBUTTONUP:
                    {
                        this.OnWMLButtonUp(ref m);
                        return true;
                    }
                case NativeMethods.WM_MOVE:
                    {
                        this.OnWMMove(ref m);
                        return true;
                    }
            }
            return base.HandleWndProc(ref m);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Form.ControlAdded -= new ControlEventHandler(OnForm_ControlAdded);
                this.Form.ControlRemoved -= new ControlEventHandler(OnForm_ControlRemoved);
                this.Form.ThemeNameChanged -= new ThemeNameChangedEventHandler(OnForm_ThemeNameChanged);
                this.Form.RightToLeftChanged -= new EventHandler(OnForm_RightToLeftChanged);
                this.Form.SizeChanged -= new EventHandler(Form_SizeChanged);
            }
            base.Dispose(disposing);
        }

        #endregion

        #region WM Handlers
        //It seems that when restoring the form from minimized state to normal
        //the custom handling of client/non-client calculations causes some visual glitches
        //due to missing layout passes. These bolean flags allow for additional layout calls
        //in the Form_SizeChanged event handlers only when the window state has been
        //changed from minimized to normal.
        private bool stateChanged1 = false;
        private bool stateChanged2 = false;
        private FormWindowState oldWindowState = FormWindowState.Normal;

        private void OnWMMove(ref Message m)
        {
            this.CallBaseWndProc(ref m);

            if (stateChanged1)
            {
                stateChanged2 = true;
            }

            if (oldWindowState != this.Form.WindowState)
            {
                stateChanged1 = true;
            }
        }

        private void OnWMLButtonUp(ref Message m)
        {
            Point location = new Point(m.LParam.ToInt32());
            location = this.NCFromClientCoordinates(location);
            MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 0, location.X, location.Y, 1);
            if (this.scrollThumbCapture != null)
            {
                this.scrollThumbCapture.CallDoMouseUp(args);
                this.scrollThumbCapture = null;
            }

            if (this.capturedNCItem != null)
            {
                this.capturedNCItem.CallDoMouseUp(args);
            }

            this.CallBaseWndProc(ref m);
        }

        private void OnWmMouseMove(ref Message m)
        {
            if (this.scrollThumbCapture != null)
            {
                Point location = new Point(m.LParam.ToInt32());
                location = this.NCFromClientCoordinates(location);

                MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 0, location.X, location.Y, 1);
               
                this.scrollThumbCapture.CallDoMouseMove(args);

                return;
            }

            this.CallBaseWndProc(ref m);
        }

        private void OnWMMouseWheel(ref Message m)
        {
            this.CallBaseWndProc(ref m);

            RadFormElement formElement = this.FormElement as RadFormElement;
            if (this.Form.VerticalScroll.Visible
                && this.Form.VerticalScroll.Value != formElement.VerticalScrollbar.Value)
            {
                if (this.Form.VerticalScroll.Value >= formElement.VerticalScrollbar.Minimum
                    && this.Form.VerticalScroll.Value <= formElement.VerticalScrollbar.Maximum)
                {
                    formElement.VerticalScrollbar.Value = this.Form.VerticalScroll.Value;
                }
            }

            if (this.Form.HorizontalScroll.Visible
                && this.Form.HorizontalScroll.Value != formElement.HorizontalScrollbar.Value)
            {
                if (this.Form.HorizontalScroll.Value >= formElement.HorizontalScrollbar.Minimum
                    && this.Form.HorizontalScroll.Value <= formElement.HorizontalScrollbar.Maximum)
                {
                    formElement.HorizontalScrollbar.Value = this.Form.HorizontalScroll.Value;
                }
            }
        }

        private void OnWMNCHittest(ref Message m)
        {
            Point location = new Point(m.LParam.ToInt32());

            int result = NativeMethods.HTNOWHERE;

            location = this.GetMappedWindowPoint(location);

            if ((this.Form.HorizontalScroll.Visible &&
                 this.Form.VerticalScroll.Visible)
                 && this.ScrollbarSizingGripBounds.Contains(location))
            {
                result = NativeMethods.HTBOTTOMRIGHT;
            }

            if (this.Form.VerticalScroll.Visible && this.VerticalScrollbarBounds.Contains(location))
            {
                result = NativeMethods.HTOBJECT;
            }

            if (this.Form.HorizontalScroll.Visible && this.HorizontalScrollbarBounds.Contains(location))
            {
                result = NativeMethods.HTOBJECT;
            }

            if (result != NativeMethods.HTNOWHERE)
            {
                m.Result = new IntPtr(result);
            }
            else
            {
                base.HandleWndProc(ref m);
            }
        }
        
        private void OnWMActivate(ref Message m)
        {
            base.HandleWndProc(ref m);

            if ((int)m.WParam == NativeMethods.WA_ACTIVE || (int)m.WParam == NativeMethods.WA_CLICKACTIVE)
            {
                this.formElement.SetIsFormActiveInternal(true);
            }
            else
            {
                this.formElement.SetIsFormActiveInternal(false);
            }
        }

        private void OnWmMDIActivate(ref Message m)
        {
            if (this.Form.IsMdiChild && this.Form.IsHandleCreated)
            {
                if (m.WParam == this.Form.Handle)
                {
                    this.formElement.SetIsFormActiveInternal(false);
                }
                else if (m.LParam == this.Form.Handle)
                {
                    this.formElement.SetIsFormActiveInternal(true);
                } 

                m.Result = IntPtr.Zero;
            }
            this.CallBaseWndProc(ref m);
        }

        private void OnWMDWMCompositionChanged(ref Message m)
        {
            this.Form.CallUpdateStyles();

            //If MDI Container, update the styles of all
            //MDI children that inherit from RadFormControlBase
            if (this.Form.IsMdiContainer)
            {
                foreach (Form form in this.Form.MdiChildren)
                {
                    if (form is RadFormControlBase)
                    {
                        (form as RadFormControlBase).CallUpdateStyles();
                    }
                }
            }
        }

        private void OnWMNCMouseLeave(ref Message m)
        {
            this.isMouseTracking = false;
            Point location = new Point(m.LParam.ToInt32());
            Point clientLocation = this.GetMappedWindowPoint(location);
            MouseEventArgs args = new MouseEventArgs(MouseButtons.None, 0, clientLocation.X, clientLocation.Y, 0);

            if (this.capturedNCItem != null)
            {
                this.capturedNCItem.CallDoMouseUp(args);
            }

            this.targetHandler.Behavior.OnMouseLeave(args);
        }

        private void OnWMNCRightButtonUp(ref Message m)
        {
            Point hitLocation = new Point(m.LParam.ToInt32());
            Point mappedhitLocation = this.GetMappedWindowPoint(hitLocation);

            MouseEventArgs args = new MouseEventArgs(MouseButtons.Right, 1, mappedhitLocation.X, mappedhitLocation.Y, 0);

            this.Form.Behavior.OnMouseUp(args);

            RadElement itemUnderMouse = this.Form.ElementTree.GetElementAtPoint(mappedhitLocation);

            if (itemUnderMouse is RadFormTitleBarElement)
            {
                int menuLocation = 0;
                menuLocation |= hitLocation.Y;
                menuLocation <<= 16;
                menuLocation |= hitLocation.X;
                IntPtr ptrLocation = new IntPtr(menuLocation);
                NativeMethods.PostMessage(new HandleRef(this.Form, this.Form.Handle),
                                                NativeMethods.WM_POPUPSYSTEMMENU, IntPtr.Zero, ptrLocation);
            }
        }

        private void OnWmNCLeftMouseButtonUp(ref Message m)
        {
            Point hitLocation = new Point(m.LParam.ToInt32());
            hitLocation = this.GetMappedWindowPoint(hitLocation);
            MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1, hitLocation.X, hitLocation.Y, 0);

            if (this.scrollThumbCapture != null)
            {
                this.scrollThumbCapture.CallDoMouseUp(args);
                this.scrollThumbCapture = null;
            }

            if (this.capturedNCItem != null)
            {
                this.capturedNCItem.CallDoMouseUp(args);
            }

            RadElement itemUnderMouse = this.Form.ElementTree.GetElementAtPoint(hitLocation);
            this.Form.Behavior.OnMouseUp(args);

            int cmdType = 0;
            if (itemUnderMouse != null && itemUnderMouse.Enabled)
            {
                RadFormElement formElement = this.FormElement as RadFormElement;
                if (object.ReferenceEquals(itemUnderMouse, formElement.TitleBar.MaximizeButton))
                {
                    if (this.Form.IsHandleCreated && NativeMethods.IsZoomed(new HandleRef(null, this.Form.Handle)))
                    {
                        cmdType = NativeMethods.SC_RESTORE;
                    }
                    else
                    {
                        cmdType = NativeMethods.SC_MAXIMIZE;
                    }
                }
                else if (object.ReferenceEquals(itemUnderMouse, formElement.TitleBar.CloseButton))
                {
                    cmdType = NativeMethods.SC_CLOSE;
                }
                else if (object.ReferenceEquals(itemUnderMouse, formElement.TitleBar.MinimizeButton))
                {
                    if (this.Form.IsHandleCreated && NativeMethods.IsIconic(new HandleRef(null, this.Form.Handle)))
                    {
                        cmdType = NativeMethods.SC_RESTORE;
                    }
                    else
                    {
                        cmdType = NativeMethods.SC_MINIMIZE;
                    }
                }
                else if (object.ReferenceEquals(itemUnderMouse, formElement.TitleBar.IconPrimitive))
                {
                    bool? dblClickResult = this.MarkDoubleClickStart();

                    if (dblClickResult != null && dblClickResult.Value)
                    {
                        cmdType = NativeMethods.SC_CLOSE;
                        if (this.Form.IsHandleCreated)
                        {
                            NativeMethods.PostMessage(new HandleRef(this, this.Form.Handle),
                                                            NativeMethods.WM_SYSCOMMAND, new IntPtr(cmdType), IntPtr.Zero);
                            return;
                        }
                    }
                    //Show the system menu if the user has clicked on the icon of the title bar.
                    int menuLocation = 0;

                    Rectangle clientRectInScreenCoord =
                    this.Form.RectangleToScreen(this.Form.ClientRectangle);

                    menuLocation |= clientRectInScreenCoord.Y;
                    menuLocation <<= 16;
                    menuLocation |= clientRectInScreenCoord.X;

                    IntPtr ptrLocation = new IntPtr(menuLocation);
                    NativeMethods.PostMessage(new HandleRef(this, this.Form.Handle),
                                                    NativeMethods.WM_POPUPSYSTEMMENU, IntPtr.Zero, ptrLocation);
                    return;
                }
                else if (object.ReferenceEquals(itemUnderMouse, formElement.MdiControlStrip.MaximizedMdiIcon))
                {
                    //Show the system menu if the user has clicked on the icon of the MDI menu.
                    Form mdiChild = this.MaximizedMDIChild;

                    if (mdiChild != null && mdiChild.IsHandleCreated)
                    {
                        bool? dblClickResult = this.MarkDoubleClickStart();

                        if (dblClickResult != null && dblClickResult.Value)
                        {
                            cmdType = NativeMethods.SC_CLOSE;
                            NativeMethods.PostMessage(new HandleRef(this, mdiChild.Handle),
                                                            NativeMethods.WM_SYSCOMMAND, new IntPtr(cmdType), IntPtr.Zero);
                            return;
                        }

                        int menuLocation = 0;

                        Rectangle clientRectInScreenCoord =
                        mdiChild.RectangleToScreen(mdiChild.ClientRectangle);

                        menuLocation |= clientRectInScreenCoord.Y;
                        menuLocation <<= 16;
                        menuLocation |= clientRectInScreenCoord.X;
                        IntPtr ptrLocation = new IntPtr(menuLocation);
                        NativeMethods.PostMessage(new HandleRef(this, mdiChild.Handle),
                                                        NativeMethods.WM_POPUPSYSTEMMENU, IntPtr.Zero, ptrLocation);
                    }
                    return;
                }
            }

            if (cmdType != 0 && this.Form.IsHandleCreated)
            {
                NativeMethods.PostMessage(new HandleRef(this, this.Form.Handle),
                                                NativeMethods.WM_SYSCOMMAND, new IntPtr(cmdType), IntPtr.Zero);
            }
        }


        private void OnWMNCLButtonDblClk(ref Message m)
        {
            Point screenLocation = new Point(m.LParam.ToInt32());
            Point location = this.GetMappedWindowPoint(screenLocation);

            RadElement itemUnderMouse = this.Form.ElementTree.GetElementAtPoint(location);
            MouseEventArgs args = new MouseEventArgs(Control.MouseButtons, 1, location.X, location.Y, 0);
            if (itemUnderMouse != null)
            {
                itemUnderMouse.CallDoDoubleClick(args);
            }

            this.CallBaseWndProc(ref m);
        }

        private void OnWmNCLeftMouseButtonDown(ref Message m)
        {
            if (!this.Form.IsHandleCreated)
            {
                this.CallBaseWndProc(ref m);
                return;
            }

            Point screenLocation = new Point(m.LParam.ToInt32());
            Point location = this.GetMappedWindowPoint(screenLocation);

            RadElement itemUnderMouse = this.Form.ElementTree.GetElementAtPoint(location);
            MouseEventArgs args = new MouseEventArgs(Control.MouseButtons, 1, location.X, location.Y, 0);
            this.Form.Behavior.OnMouseDown(args);

            this.capturedNCItem = itemUnderMouse;
            if (itemUnderMouse is ScrollBarThumb)
            {
                this.scrollThumbCapture = itemUnderMouse as ScrollBarThumb;
            }

            if (itemUnderMouse != null && itemUnderMouse.Enabled
                && !object.ReferenceEquals(itemUnderMouse, this.formElement.TitleBar)
                && !object.ReferenceEquals(itemUnderMouse, this.formElement)
                && !object.ReferenceEquals(itemUnderMouse, this.formElement.ScrollBarsFormSizingGrip))
            {
                
                return;
            }
            this.CallBaseWndProc(ref m);
        }

        private void OnWmSetIcon(ref Message m)
        {
            this.CallBaseWndProc(ref m);
            if ((int)m.WParam == NativeMethods.ICON_SMALL
                && m.LParam != IntPtr.Zero)
            {
                Icon titleBarIcon = Icon.FromHandle(m.LParam);
                
                if (titleBarIcon != null)
                {
                    RadFormElement formElement = this.FormElement as RadFormElement;

                    if (formElement != null)
                    {
                        Bitmap bitmapIcon  = titleBarIcon.ToBitmap();
                        formElement.SetFormElementIconInternal(bitmapIcon);
                        titleBarIcon.Dispose();
                        this.Form.CallSetClientSizeCore(
                            this.Form.ClientSize.Width,
                            this.Form.ClientSize.Height);
                    }
                }
            }
            else if ((int)m.WParam == NativeMethods.ICON_SMALL 
                     && m.LParam == IntPtr.Zero)
            {
                RadFormElement formElement = this.FormElement as RadFormElement;

                if (formElement != null)
                {
                    formElement.SetFormElementIconInternal(null);
                }
            }
        }

        private void OnWmSetText(ref Message m)
        {
            this.CallBaseWndProc(ref m);
            string formText = Marshal.PtrToStringAuto(m.LParam);
            (this.FormElement as RadFormElement).TitleBar.Text = formText;

            this.RefreshNC();
        }

        private void OnWMStyleChanged(ref Message m)
        {
            this.CallBaseWndProc(ref m);
            this.AdjustFormElementForCurrentStyles();
        }

        private void OnWMNCMouseMove(ref Message m)
        {
            this.CallBaseWndProc(ref m);
            this.TrackMouseLeaveMessage();
            Point screenPoint = new Point(m.LParam.ToInt32());
            
            Point location = this.GetMappedWindowPoint(screenPoint);

            MouseEventArgs args = new MouseEventArgs(Control.MouseButtons, 1, location.X, location.Y, 1);

            if (this.scrollThumbCapture != null)
            {
                this.scrollThumbCapture.CallDoMouseMove(args);
                return;
            }

            this.Form.Behavior.OnMouseMove(args);
        }

        #endregion

        #region Event handling

        private void CloseButton_Click(object sender, EventArgs e)
        {
            (this.FormElement as RadFormElement).MdiControlStrip.Visibility = ElementVisibility.Collapsed;

            for (int i = 0; i < this.Form.MdiChildren.Length; i++)
            {
                Form mdiChild = this.Form.MdiChildren[i];

                if (mdiChild.WindowState == FormWindowState.Maximized)
                {
                    mdiChild.Close();
                    break;
                }
            }

            if (this.Form.IsHandleCreated)
            {
                NativeMethods.SetWindowPos(
                    new HandleRef(null, this.Form.Handle), 
                    new HandleRef(null, IntPtr.Zero), 
                    0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_DRAWFRAME);
            }
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Form.MdiChildren.Length; i++)
            {
                Form mdiChild = this.Form.MdiChildren[i];

                if (mdiChild.WindowState == FormWindowState.Maximized)
                {
                    mdiChild.WindowState = FormWindowState.Normal;
                    break;
                }
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Form.MdiChildren.Length; i++)
            {
                Form mdiChild = this.Form.MdiChildren[i];

                if (mdiChild.WindowState == FormWindowState.Maximized)
                {
                    mdiChild.WindowState = FormWindowState.Minimized;
                    break;
                }
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Form.Load -= new EventHandler(Form_Load);
            if (this.Form.IsDesignMode)
            {
                this.formElement.SetDefaultValueOverride(RadFormElement.IsFormActiveProperty, true);
            }
            this.AdjustFormElementForFormState(this.CurrentFormState);
            this.SynchronizeVerticalScrollbarValues();
            this.SynchronizeHorizontalScrollbarValues();
        }

        private void OnForm_RightToLeftChanged(object sender, EventArgs e)
        {
            this.FormElement.SetValue(RadItem.RightToLeftProperty, this.Form.RightToLeft == RightToLeft.Yes);
        }

        private void OnForm_ThemeNameChanged(object source, ThemeNameChangedEventArgs args)
        {
            RadFormElement radFormElement = this.FormElement as RadFormElement;
            if (radFormElement != null)
            {
                radFormElement.InvalidateMeasure();
                radFormElement.InvalidateArrange();
                radFormElement.UpdateLayout();
                this.Form.Update();
                this.Form.CallUpdateStyles();
                this.AdjustFormElementForFormState(this.CurrentFormState);
            }
        }

        private void Form_SizeChanged(object sender, EventArgs e)
        {
            this.SynchronizeSizeGrip();
            this.SynchronizeHorizontalScrollbarValues();
            this.SynchronizeVerticalScrollbarValues();

            if (this.stateChanged1 || this.stateChanged2)
            {
                if (this.stateChanged2)
                {
                    this.stateChanged1 = false;
                    this.stateChanged2 = false;
                }

                this.Form.PerformLayout(this.Form, "Bounds");
            }
        }

        private void OnForm_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.SynchronizeSizeGrip();
            this.SynchronizeHorizontalScrollbarValues();
            this.SynchronizeVerticalScrollbarValues();
        }

        private void OnForm_ControlAdded(object sender, ControlEventArgs e)
        {
            this.SynchronizeSizeGrip();
            this.SynchronizeHorizontalScrollbarValues();
            this.SynchronizeVerticalScrollbarValues();
        }
        #endregion
        #endregion
    }
}
