using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Diagnostics;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents an item that contains external control. There is no limitation for the control type - could
    /// be any descedant of the class Control.
    /// </summary>
    public class RadHostItem : RadItem
    {
        #region BitState Keys

        internal const ulong RouteMessagesStateKey = RadItemLastStateKey << 1;
        internal const ulong RadHostItemLastStateKey = RouteMessagesStateKey;

        #endregion

        // Fields
        private Control hostedControl;
        private SizeSettingDirection sizeSettingDirection = SizeSettingDirection.None;
		internal static readonly object EventValidated;
		internal static readonly object EventValidating;
        internal static readonly object GotFocusEventKey;
        internal static readonly object LostFocusEventKey;

        /// <summary>
        /// Gets the instance of the hosted control.
        /// </summary>
        public Control HostedControl
        {
            get 
            { 
                return this.hostedControl; 
            }
        }

		static RadHostItem() 
		{
			RadHostItem.EventValidated = new object();
			RadHostItem.EventValidating = new object();
            RadHostItem.GotFocusEventKey = new object();
            RadHostItem.LostFocusEventKey = new object();
		}

		public RadHostItem(Control c)
		{
			this.hostedControl = c;

            //Synch inheritable properties
            this.hostedControl.ForeColor = this.ForeColor;
            this.hostedControl.BackColor = this.BackColor;
            this.hostedControl.Font = this.Font;

            this.WireEvents();
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[RouteMessagesStateKey] = true;
            this.NotifyParentOnMouseInput = true;
            this.CanFocus = true;
        }

        protected override void DisposeManagedResources()
        {
            if (!(this.hostedControl.Disposing || this.hostedControl.IsDisposed))
            {
                this.hostedControl.Dispose();
            }

            base.DisposeManagedResources();
        }

        /// <summary>
        /// Corresponds to the hosted control's Validated event
        /// </summary>
		[Category("Validation")]
		public event EventHandler Validated
		{
			add
			{
				base.Events.AddHandler(RadHostItem.EventValidated, value);
			}
			remove
			{
				base.Events.RemoveHandler(RadHostItem.EventValidated, value);
			}
		}

        /// <summary>
        /// Corresponds to the hosted control's Validating event
        /// </summary>
        [Category("Validation")]
		public event CancelEventHandler Validating
		{
			add
			{
				base.Events.AddHandler(RadHostItem.EventValidating, value);
			}
			remove
			{
				base.Events.RemoveHandler(RadHostItem.EventValidating, value);
			}
		}

        /// <summary>
        /// 	<para>Gets or sets whether the mouse and keyboard messages from the hosted control
        ///     can be routed to the owner control.</para>
        /// </summary>
        /// <remarks>
        /// 	<para>
        ///         You can use <see cref="RadElementTree.Control">ElementTree.Control</see> to get
        ///         the owner control.
        ///     </para>
        /// 	<para>
        ///         To get the hosted control use <see cref="HostedControl">HostedControl</see>
        ///         property.
        ///     </para>
        /// </remarks>
        [Description("Allow routing mouse messages to owner control")]
		[Category(RadDesignCategory.BehaviorCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(true)]
		public bool RouteMessages
		{
            get
            {
                return this.GetBitState(RouteMessagesStateKey);
            }
            set
            {
                this.SetBitState(RouteMessagesStateKey, value);
            }
		}

		private void HandleValidated(object sender, EventArgs e)
		{
			this.OnValidated(e);
		}

		private void HandleValidating(object sender, CancelEventArgs e)
		{
			this.OnValidating(e);
		}

		protected virtual void OnValidated(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)base.Events[RadHostItem.EventValidated];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		protected virtual void OnValidating(CancelEventArgs e)
		{
			CancelEventHandler handler1 = (CancelEventHandler)base.Events[RadHostItem.EventValidating];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		public override bool Focus()
		{
			this.Focus(false);
            //Debug.WriteLine("hostitem is focused");
            if (this.HostedControl is IComponentTreeHandler)
            {
                return (this.HostedControl as IComponentTreeHandler).OnFocusRequested(this);
            }

            return false;
		}

		///////////////////////////
        /// <summary>
        /// Occurs when the element recieves focus.
        /// </summary>
        [Browsable(false),
        Category("Property Changed"),
        Description("Occurs when the element recieves focus.")]
        public event EventHandler GotFocus
        {
            add
            {
                this.Events.AddHandler( GotFocusEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(GotFocusEventKey, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnGotFocus(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[GotFocusEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the element loses focus.
        /// </summary>
        [Browsable(false),
        Category("Property Changed"),
        Description("Occurs when the element loses focus.")]
        public event EventHandler LostFocus
        {
            add
            {
                this.Events.AddHandler(LostFocusEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(LostFocusEventKey, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLostFocus(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)this.Events[LostFocusEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }
        
        void hostedControl_GotFocus(object sender, EventArgs e)
        {
            this.OnGotFocus(e);            
        }

        void hostedControl_LostFocus(object sender, EventArgs e)
        {
            this.OnLostFocus(e);
        }

        void hostedControl_SizeChanged(object sender, EventArgs e)
        {
            if (this.UseNewLayoutSystem)
            {
                if (this.AutoSize)
                {
                    SizeSettingDirection direction = this.sizeSettingDirection;
                    if (direction == SizeSettingDirection.None || direction == SizeSettingDirection.FromHostedControl)
                    {
                        this.sizeSettingDirection = SizeSettingDirection.FromHostedControl;

                        this.InvalidateMeasure();
                        //this.UpdateLayout();

                        if (direction == SizeSettingDirection.None)
                            this.sizeSettingDirection = SizeSettingDirection.None;
                    }
                }
            }
            else
            {
                if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
                {
                    Size newSize = this.Size;
                    if (!this.StretchHorizontally)
                        newSize.Width = this.hostedControl.Size.Width;
                    if (!this.StretchVertically)
                        newSize.Height = this.hostedControl.Size.Height;

                    if (!this.StretchHorizontally || !this.StretchVertically)
                    {
                        this.Size = newSize;
                    }
                }
            }
        }

        private void hostedControl_Disposed(object sender, EventArgs e)
		{
            this.UnwireEvents();
		}
		
		public override bool OverridesDefaultLayout
		{
			get
			{
				return true;
			}
		}


        /// <summary>
        /// Gets or sets the <strong>CausesValidation</strong> property of the hosted
        /// control.
        /// </summary>
        /// <remarks>
        ///     Using this property is equivalent to using
        ///     <see cref="HostedControl">HostedControl</see>.CausesValidation
        /// </remarks>
        [Category("Validation"), DefaultValue(true)]
		public bool CausesValidation
		{
			get
			{
				return this.hostedControl.CausesValidation;
			}
			set
			{
				this.hostedControl.CausesValidation = value;
			}
		}

		protected void EnsureHostedControl()
		{
            if (this.ElementState == ElementState.Loaded)
            {
                this.ElementTree.ComponentTreeHandler.RegisterHostedControl(this);
            }
		}

        public override void PerformLayoutCore(RadElement affectedElement)
        {
            if (this.hostedControl != null)
                this.hostedControl.PerformLayout();

            base.PerformLayoutCore(affectedElement);

            if (this.hostedControl != null)
            {
                Size newSize = this.hostedControl.Size;
                if (this.StretchHorizontally || this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                    newSize.Width = this.Size.Width;
                if (this.StretchVertically || this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                    newSize.Height = this.Size.Height;

                this.hostedControl.Size = newSize;
            }
        }
      
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
            if (this.ElementState != ElementState.Loaded)
            {
                return proposedSize;
            }

			if (this.AutoSize && this.hostedControl != null)
            {
                if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
                {
                    Size res = this.LayoutEngine.CheckSize(/*proposedSize*/this.Parent.Size);
                    return res;
                }
                else
                {
                    Size res = this.hostedControl.Size;

                    if (this.StretchHorizontally)
                        res.Width = proposedSize.Width - this.Margin.Horizontal;
                    if (this.StretchVertically)
                        res.Height = proposedSize.Height - this.Margin.Vertical;                    
                    return res;
                }
            }

            return base.GetPreferredSizeCore(proposedSize);
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);

            if (this.AutoSize && this.hostedControl != null)
            {
                res = this.hostedControl.Size;

                if (this.StretchHorizontally)
                {
                    res.Width = 0;
                }
                if (this.StretchVertically)
                {
                    res.Height = 0;
                }
            }

            return res;
        }



        protected override void ArrangeCore(RectangleF finalRect)
        {
            base.ArrangeCore(finalRect);

            if (this.sizeSettingDirection == SizeSettingDirection.FromHostedControl)
            {
                return;
            }

            this.sizeSettingDirection = SizeSettingDirection.ToHostedControl;

            Size finalSize = Size.Round(finalRect.Size);
            Size hostedSize = this.hostedControl.Size;
            Size newSize = new Size(this.StretchHorizontally ? finalSize.Width : hostedSize.Width,
                                    this.StretchVertically ? finalSize.Height : hostedSize.Height);

            this.SetControlBounds(new Rectangle(this.LocationToControl(), newSize));

            this.sizeSettingDirection = SizeSettingDirection.None;
        }

        private void SetControlBounds(Rectangle bounds)
        {
            if (!this.IsInValidState(true))
            {
                return;
            }
            Control host = this.ElementTree.Control;
            host.SuspendLayout();
            this.hostedControl.Bounds = bounds;
            host.ResumeLayout(false);
        }

		private void SyncRTL(bool rtl)
        {
            if (rtl)
            {
                this.hostedControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            }
            else
            {
                this.hostedControl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            }
        }

        private void SyncFont(Font font)
        {
            bool isInValidState =this.IsInValidState(true);

            if (isInValidState)
            {
                this.ElementTree.Control.SuspendLayout();
            }

            this.hostedControl.SuspendLayout();
            this.hostedControl.Font = font;
            this.hostedControl.ResumeLayout(false);

            if (isInValidState)
            {
                this.ElementTree.Control.ResumeLayout(false);
            }
        }

        private void SyncForeColor(Color foreColor)
        {
            if ((this.hostedControl is TextBox) && (foreColor.A < 255))
            {
                foreColor = Color.FromArgb(255, foreColor);
            }

            this.hostedControl.ForeColor = foreColor;
        }

		private void SyncBackColor(Color backColor)
		{
            //Remove the color transparency
            //in case the hosted control does not support it
            if (backColor.A < 255)
            {
                if (!ControlHelper.GetControlStyle(this.hostedControl, ControlStyles.SupportsTransparentBackColor))
                {
                    backColor = Color.FromArgb(255, backColor);
                }
            }

            this.hostedControl.BackColor = backColor;
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
            base.OnPropertyChanged(e);

            if (this.hostedControl == null)
            {
                return;
            }

            if (e.Property == VisualElement.ForeColorProperty)
            {
                this.SyncForeColor((Color)e.NewValue);
            }
            else if (e.Property == VisualElement.BackColorProperty)
            {
                SyncBackColor((Color)e.NewValue);
            }
            else if (e.Property == FontProperty)
            {
                this.SyncFont((Font)e.NewValue);
            }
            else if (e.Property == RightToLeftProperty)
            {
                this.SyncRTL((bool)e.NewValue);
            }
            else if( e.Property == RadElement.EnabledProperty)
            {
                this.hostedControl.Enabled = (bool)e.NewValue;
            }
		}

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;

                if (this.HostedControl != null)
                {
                    this.HostedControl.Enabled = value;
                }
            }
        }

		private RoutedEventArgs pendingRoutedEventArgs = null;
		private RadElement pendingSender = null;

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnTunnelEvent(sender, args);

            if (this.UseNewLayoutSystem)
            {
                if (args.RoutedEvent == RootRadElement.RootLayoutSuspendedEvent)
                {
                    if (this.HostedControl != null)
                        this.HostedControl.SuspendLayout();
                }
                else if (args.RoutedEvent == RootRadElement.RootLayoutResumedEvent)
                {
                    if (this.HostedControl != null)
                        this.HostedControl.ResumeLayout(false);
                }
                PerformRoutedEventAction(sender, args);
            }
            else
            {
                if (args.RoutedEvent == RadElement.BoundsChangedEvent || args.RoutedEvent == RadElement.VisibilityChangingEvent)
                {
                    if (this.IsLayoutSuspended && (this.SuspendedParent != null))
                    {
                        this.pendingSender = sender;
                        this.pendingRoutedEventArgs = args;
                    }
                    else
                    {
                        PerformRoutedEventAction(sender, args);
                    }
                }
                else if ((args.RoutedEvent == RootRadElement.RootLayoutResumedEvent) && (this.pendingRoutedEventArgs != null) &&
                    (!this.IsLayoutSuspended && (this.SuspendedParent == null)))
                {
                    RoutedEventArgs pendingArgs = this.pendingRoutedEventArgs;
                    RadElement tunnelSender = this.pendingSender;
                    this.pendingRoutedEventArgs = null;
                    this.pendingSender = null;
                    PerformRoutedEventAction(tunnelSender, pendingArgs);
                }
            }
		}

		protected override void OnTransformationInvalidated()
		{
			SyncBoundsWithHostedControl();
		}

        private void PerformRoutedEventAction(RadElement sender, RoutedEventArgs args)
		{
            // The routed event is used instead of using OnPropertyChanged() because Visibility of
            // some of the parent elements in the hierarchy could be modified...
			if (args.RoutedEvent == RadElement.VisibilityChangingEvent)
			{
                this.UpdateControlVisibility();
			}
            else if (args.RoutedEvent == RadElement.BoundsChangedEvent)
            {
                this.SyncBoundsWithHostedControl();
            }
		}

        /// <summary>
        /// Updates the visibility, which is bound to the item's current IsVisible state, of the hosted control.
        /// </summary>
		public void UpdateControlVisibility()
		{
            if (this.ElementState != ElementState.Loaded)
            {
                return;
            }

            bool visible = this.Visibility == ElementVisibility.Visible;
            if (visible)
            {
                visible = !this.HasInvisibleAncestor();
            }
            this.hostedControl.Visible = visible;
		}

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.UpdateControlVisibility();
        }

        
     

        protected override void OnElementTreeChanged(ComponentThemableElementTree previousTree)
        {
            base.OnElementTreeChanged(previousTree);

            if (previousTree != null)
            {
                previousTree.ComponentTreeHandler.UnregisterHostedControl(this, true);
            }

            if (this.ElementTree != null)
            {
                this.ElementTree.ComponentTreeHandler.RegisterHostedControl(this);
            }
        }

        private void UnwireEvents()
        {
            this.hostedControl.MouseUp -= hostedControl_MouseUp;
            this.hostedControl.MouseDown -= hostedControl_MouseDown;
            this.hostedControl.MouseMove -= hostedControl_MouseMove;
            this.hostedControl.MouseHover -= hostedControl_MouseHover;
            this.hostedControl.Click -= hostedControl_Click;
            this.hostedControl.DoubleClick -= hostedControl_DoubleClick;
            this.hostedControl.MouseEnter -= hostedControl_MouseEnter;
            this.hostedControl.MouseLeave -= hostedControl_MouseLeave;
            this.hostedControl.SizeChanged -= hostedControl_SizeChanged;
            this.hostedControl.Validating -= HandleValidating;
            this.hostedControl.Validated -= HandleValidated;
            this.hostedControl.Disposed -= hostedControl_Disposed;
            this.hostedControl.LostFocus -= hostedControl_LostFocus;
            this.hostedControl.GotFocus -= hostedControl_GotFocus;
        }

        private void WireEvents()
        {
            this.hostedControl.MouseUp += hostedControl_MouseUp;
            this.hostedControl.MouseDown += hostedControl_MouseDown;
            this.hostedControl.MouseMove += hostedControl_MouseMove;
            this.hostedControl.MouseHover += hostedControl_MouseHover;
            this.hostedControl.Click += hostedControl_Click;
            this.hostedControl.DoubleClick += hostedControl_DoubleClick;
            this.hostedControl.MouseEnter += hostedControl_MouseEnter;
            this.hostedControl.MouseLeave += hostedControl_MouseLeave;
            this.hostedControl.SizeChanged += hostedControl_SizeChanged;
            this.hostedControl.Validating += HandleValidating;
            this.hostedControl.Validated += HandleValidated;
            this.hostedControl.Disposed += hostedControl_Disposed;
            this.hostedControl.LostFocus += hostedControl_LostFocus;
            this.hostedControl.GotFocus += hostedControl_GotFocus;
        }

        protected virtual void SyncBoundsWithHostedControl()
		{
            if (!this.IsInValidState(true))
            {
                return;
            }

            Control host = this.ElementTree.Control;
            host.SuspendLayout();
            this.hostedControl.Location = this.LocationToControl();
            host.ResumeLayout(false);
		}

		private void hostedControl_MouseHover(object sender, EventArgs e)
		{
            if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseHover(e);
            }
		}

		private void hostedControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseMove(TranslateMouseEventArgsToControl(e));
            }
		}

		private void hostedControl_MouseLeave(object sender, EventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseLeave(e);
            }
		}

		private void hostedControl_MouseEnter(object sender, EventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseEnter(e);
            }
		}

		private void hostedControl_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseDown(TranslateMouseEventArgsToControl(e));
            }
		}

		private void hostedControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnMouseUp(TranslateMouseEventArgsToControl(e));
            }
		}

		private void hostedControl_Click(object sender, EventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnClick(e);
            }
		}

		private void hostedControl_DoubleClick(object sender, EventArgs e)
		{
			if (this.ElementState == ElementState.Loaded &&
                this.GetBitState(RouteMessagesStateKey))
            {
                this.ElementTree.ComponentTreeHandler.Behavior.OnDoubleClick(e);
            }
		}

		private MouseEventArgs TranslateMouseEventArgsToControl(MouseEventArgs e)
		{
			Point newLocation = this.ElementTree.Control.PointToClient(this.hostedControl.PointToScreen(e.Location));
			return new MouseEventArgs(
				e.Button,
				e.Clicks,
				newLocation.X,
				newLocation.Y,
				e.Delta);
		}


        private enum SizeSettingDirection
        {
            None,
            ToHostedControl,
            FromHostedControl
        }
	}
}
