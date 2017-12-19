using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.WindowAnimation;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a base class for all popup-forms used throughout
    /// the RadControls for WinForms suite.
    /// </summary>
    [ToolboxItem(false)]
    public class RadPopupControlBase : RadControl, IPopupControl, ITooltipOwner
    {
        #region Nested classes

        public class PopupAnimationProperties : INotifyPropertyChanged
        {
            private WindowAnimationEngine animationEngine;
            private RadDirection animationDirection = RadDirection.Down;

            /// <summary>
            /// Creates an instance of the PopupAnimationProperties class.
            /// This class encapsulates a WindowAnimationEngine instance
            /// and exposes its properties.
            /// </summary>
            /// <param name="animationEngine">The WindowAnimationEngine instance.</param>
            internal PopupAnimationProperties(WindowAnimationEngine animationEngine)
            {
                this.animationEngine = animationEngine;
            }

            /// <summary>
            /// Gets or sets the direction of the drop-down animation.
            /// </summary>
            public RadDirection AnimationDirection
            {
                get
                {
                    return this.animationDirection;
                }
                set
                {
                    if (value != this.animationDirection)
                    {
                        this.animationDirection = value;
                        this.OnNotifyPropertyChanged("AnimationDirection");
                    }
                }
            }

            /// <summary>
            /// Gets or sets the count of the frames of the animation.
            /// </summary>
            public int AnimationFrames
            {
                get
                {
                    return animationEngine.AnimationFrames;
                }
                set
                {
                    if (this.animationEngine.AnimationFrames != value)
                    {
                        this.animationEngine.AnimationFrames = value;
                        this.OnNotifyPropertyChanged("AnimationFrames");
                    }
                }
            }

            /// <summary>
            /// Gets or sets the easing type of the animation.
            /// </summary>
            public RadEasingType EasingType
            {
                get
                {
                    return this.animationEngine.EasingType;
                }
                set
                {
                    if (this.animationEngine.EasingType != value)
                    {
                        this.animationEngine.EasingType = value;
                        this.OnNotifyPropertyChanged("EasingType");
                    }
                }
            }

            /// <summary>
            /// Gets an integer value representing the animation
            /// step.
            /// </summary>
            public int AnimationStep
            {
                get
                {
                    return this.animationEngine.AnimationStep;
                }
            }

            /// <summary>
            /// Gets the <see cref="Telerik.WinControls.WindowAnimation.WindowAnimationEngine"/>
            /// instance associated with the AnimationProperties instance.
            /// </summary>
            public WindowAnimationEngine AnimationEngine
            {
                get
                {
                    return this.animationEngine;
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnNotifyPropertyChanged(string propertyName)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private object FadeAnimationFinishedKey = new object();
        private object PopupOpeningEventKey = new object();
        private object PopupOpenedEventKey = new object();
        private object PopupClosingEventKey = new object();
        private object PopupClosedEventKey = new object();

        private RadElement ownerElement = null;
        private IPopupControl ownerPopupInternal = null;
        private List<IPopupControl> children;

        //Popup settings
        private AlignmentCorrectionMode horizontalAlignmentCorrectionMode = AlignmentCorrectionMode.None;
        private AlignmentCorrectionMode verticalAlignmentCorrectionMode = AlignmentCorrectionMode.None;
        private VerticalPopupAlignment verticalPopupAlignment = VerticalPopupAlignment.TopToBottom;
        private HorizontalPopupAlignment horizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
        private FitToScreenModes fitToScreenMode = FitToScreenModes.FitWidth | FitToScreenModes.FitHeight;
        private ScreenSpaceMode screenSpaceMode = ScreenSpaceMode.WorkingArea;
        private AlternativeCorrectionMode alignmentRectangleOverlapMode = AlternativeCorrectionMode.Flip;
        private FadeAnimationType fadeAnimationType;
        private MethodInvoker performFadeInStepInvoker;
        private MethodInvoker performFadeOutStepInvoker;
        private bool dropShadow = false;
        private bool aeroEnabled = false;
        private float opacity = 1.0F;
        private bool isLayeredWindow = false;
        private Timer fadeAnimationTimer;
        private int fadeAnimationFrames = 10;
        private int fadeAnimationSpeed = 10;
        private bool animationStateOpening = true;
        private float calculatedAnimationStep = 0;
        private RadPopupCloseReason lastCloseReason;
        private bool dropDownAnimating = false;
        private float opacityInternal = 1.0F;
        protected Rectangle lastAlignmentRectangle = Rectangle.Empty;

        //Drop down animations
        private WindowAnimationEngine animationEngine;
        public PopupAnimationProperties AnimationProperties;
        private NotifyAnimationCallback callbackAnimating;
        private NotifyAnimationCallback callbackAnimationFinished;

        private bool shouldRestoreAutoSize = false;
        private PopupAnimationTypes animationType = PopupAnimationTypes.Fade;
        private Size backupSize = Size.Empty;

        #endregion

        #region Constructors & Initialization

        /// <summary>
        /// Creates an instance of the RadPopupFormBase class.
        /// </summary>
        public RadPopupControlBase(RadElement owner)
        {
            this.ownerElement = owner;            
            this.animationEngine = new WindowAnimationEngine();
            this.AnimationProperties = new PopupAnimationProperties(this.animationEngine);
            this.callbackAnimating = new NotifyAnimationCallback(this.OnAnimating);
            this.callbackAnimationFinished = new NotifyAnimationCallback(this.OnAnimationFinished);
            this.animationEngine.Animating += new AnimationEventHandler(OnAnimatingEvent);
            this.animationEngine.AnimationFinished += new AnimationEventHandler(OnAnimationFinishedEvent);
            //popups have white BackColor by default
            this.RootElement.SetDefaultValueOverride(VisualElement.BackColorProperty, SystemColors.Window);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the direction of the drop-down
        /// animation.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public RadDirection DropDownAnimationDirection
        {
            get
            {
                return this.AnimationProperties.AnimationDirection;
            }
            set
            {
                this.AnimationProperties.AnimationDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the easing type for the drop down animations.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public RadEasingType EasingType
        {
            get
            {
                return this.AnimationProperties.EasingType;
            }
            set
            {
                this.AnimationProperties.EasingType = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of the frames of the drop down animation.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public int AnimationFrames
        {
            get
            {
                return this.AnimationProperties.AnimationFrames;
            }
            set
            {
                this.AnimationProperties.AnimationFrames = value;
            }
        }

        /// <summary>
        /// Gets or sets a bool value determining
        /// whether popup animation is enabled.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool AnimationEnabled
        {
            get
            {
                return animationType != PopupAnimationTypes.None;
            }
            set
            {
                if (value != this.AnimationEnabled)
                {
                    if (value)
                    {
                        this.animationType = PopupAnimationTypes.Fade;
                    }
                    else
                    {
                        this.animationType = PopupAnimationTypes.None;
                    }
                    this.OnNotifyPropertyChanged("AnimationEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value determining what animation type to use when showing the popup.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public PopupAnimationTypes AnimationType
        {
            get
            {
                return animationType; 
            }
            set
            {
                if (animationType != value)
                {
                    animationType = value;
                    OnNotifyPropertyChanged("AnimationType");
                }
            }
        }

        internal Size NonAnimatedSize
        {
            get
            {
                if (this.dropDownAnimating)
                {
                    return this.backupSize;
                }

                return this.Size;
            }
        }

        private float OpacityInternal
        {
            get
            {
                return this.opacityInternal;
            }
            set
            {
                if (value != this.opacityInternal)
                {
                    this.opacityInternal = value;
                    this.UpdateOpacitySettings(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the frame count
        /// for the fade animation.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public int FadeAnimationFrames
        {
            get
            {
                return this.fadeAnimationFrames;
            }
            set
            {
                if (value != this.fadeAnimationFrames && value > 0)
                {
                    this.fadeAnimationFrames = value;
                    this.OnNotifyPropertyChanged("FadeAnimationFrames");
                    this.UpdateFadeAnimationSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets the time interval for each fade animation frame.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        public int FadeAnimationSpeed
        {
            get
            {
                return this.fadeAnimationSpeed;
            }
            set
            {
                if (value != this.fadeAnimationSpeed)
                {
                    this.fadeAnimationSpeed = value;
                    this.OnNotifyPropertyChanged("FadeAnimationSpeed");
                    this.UpdateFadeAnimationSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets a float value that determines the opacity of the popup.
        /// This property accepts values from 0.0 to 1.0. For example,
        /// to make the popup semi-transparent, set the property to 0.5.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        public float Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                if (value != this.opacity)
                {
                    if (value < 0.0)
                    {
                        this.opacity = 0.0F;
                    }
                    else if (value > 1.0)
                    {
                        this.opacity = 1.0F;
                    }
                    else if (OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
                    {
                        this.opacity = value;
                    }
                    this.opacityInternal = this.opacity;
                    this.OnNotifyPropertyChanged("Opacity");
                    this.UpdateOpacitySettings(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value which determines
        /// whether the popup drops a shadow.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool DropShadow
        {
            get
            {
                return this.dropShadow;
            }
            set
            {
                if (value != this.dropShadow)
                {
                    this.dropShadow = value;
                    this.OnNotifyPropertyChanged("DropShadow");
                    this.UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Enables the support for Windows Vista DWM effects.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool EnableAeroEffects
        {
            get
            {
                return this.aeroEnabled;
            }
            set
            {
                if (value != this.aeroEnabled)
                {
                    this.aeroEnabled = value;
                    this.OnNotifyPropertyChanged("EnableAeroEffects");
                    //this.UpdateStyles();
                    this.UpdateAeroEffectState();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the type
        /// of the fade animation.
        /// </summary>
        [Editor(typeof(Telerik.WinControls.UI.FadeAnimationTypeEditor), typeof(UITypeEditor))]
        [Category(RadDesignCategory.BehaviorCategory)]
        public FadeAnimationType FadeAnimationType
        {
            get
            {
                return this.fadeAnimationType;
            }
            set
            {
                if (value != this.fadeAnimationType)
                {
                    this.fadeAnimationType = value;
                    this.OnNotifyPropertyChanged("FadeAnimationType");
                    this.UpdateFadeAnimationSettings();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="FitToScreenModes"/> enum
        /// which defines how the size of the popup is fit to the currently active screen.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value that determines how the size of the popup is adjusted according to the available screen space.")]
        [DefaultValue(typeof(FitToScreenModes), "FitWidth | FitHeight")]
        public FitToScreenModes FitToScreenMode
        {
            get
            {
                return this.fitToScreenMode;
            }
            set
            {
                if (value != this.fitToScreenMode)
                {
                    this.fitToScreenMode = value;
                    this.OnNotifyPropertyChanged("FitToScreenMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="ScreenSpaceMode"/> enum
        /// which determines what part of the screen is considered when positioning the popup.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value that determines what part of the screen is considered when positioning the popup.")]
        [DefaultValue(typeof(ScreenSpaceMode), "WorkingArea")]
        public ScreenSpaceMode ScreenSpaceMode
        {
            get
            {
                return this.screenSpaceMode;
            }
            set
            {
                if (value != this.screenSpaceMode)
                {
                    this.screenSpaceMode = value;
                    this.OnNotifyPropertyChanged("ScreenSpaceMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="AlignmentRectangleOverlapMode"/> which defines how the popup will be positioned according to the
        /// alignment rectangle when its location cannot be adjusted so that it meets all popup alignment and alignment correction mode requirements.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value that defines how the popup will be positioned according to the alignment rectangle when its location cannot be"
            + " adjusted so that it meets all popup alignment and alignment correction mode requirements.")]
        [DefaultValue(typeof(AlternativeCorrectionMode), "Flip")]
        public AlternativeCorrectionMode AlignmentRectangleOverlapMode
        {
            get
            {
                return this.alignmentRectangleOverlapMode;
            }
            set
            {
                if (value != this.alignmentRectangleOverlapMode)
                {
                    this.alignmentRectangleOverlapMode = value;
                    this.OnNotifyPropertyChanged("AlignmentRectangleOverlapMode");
                }
            }
        }

        /// <summary>
        /// Defines how the popup will be horizontally aligned in case of lack of 
        /// screen space.
        /// </summary>
        [DefaultValue(AlignmentCorrectionMode.None)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public AlignmentCorrectionMode HorizontalAlignmentCorrectionMode
        {
            get
            {
                return this.horizontalAlignmentCorrectionMode;
            }
            set
            {
                if (value != this.horizontalAlignmentCorrectionMode)
                {
                    this.horizontalAlignmentCorrectionMode = value;
                    this.OnNotifyPropertyChanged("HorizontalAlignmentCorrectionMode");
                }
            }
        }

        /// <summary>
        /// Defines how the popup will be vertically aligned in case of lack of 
        /// screen space.
        /// </summary>
        [DefaultValue(AlignmentCorrectionMode.None)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public AlignmentCorrectionMode VerticalAlignmentCorrectionMode
        {
            get
            {
                return this.verticalAlignmentCorrectionMode;
            }
            set
            {
                if (value != this.verticalAlignmentCorrectionMode)
                {
                    this.verticalAlignmentCorrectionMode = value;
                    this.OnNotifyPropertyChanged("VerticalAlignmentCorrectionMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that defines the vertical alignment
        /// of the popup based on the alignment rectangle passed
        /// in the ShowPopup method.
        /// </summary>
        [DefaultValue(VerticalPopupAlignment.TopToBottom)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public VerticalPopupAlignment VerticalPopupAlignment
        {
            get
            {
                return this.verticalPopupAlignment;
            }
            set
            {
                if (value != this.verticalPopupAlignment)
                {
                    this.verticalPopupAlignment = value;
                    this.OnNotifyPropertyChanged("VerticalPopupAlignment");
                    this.UpdateLocation(this.lastAlignmentRectangle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that defines the horizontal alignment
        /// of the popup based on the alignment rectangle passed
        /// in the ShowPopup method.
        /// </summary>
        [DefaultValue(HorizontalPopupAlignment.LeftToLeft)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public HorizontalPopupAlignment HorizontalPopupAlignment
        {
            get
            {
                return this.horizontalPopupAlignment;
            }
            set
            {
                if (value != this.horizontalPopupAlignment)
                {
                    this.horizontalPopupAlignment = value;
                    this.OnNotifyPropertyChanged("HorizontalPopupAlignment");
                    this.UpdateLocation(this.lastAlignmentRectangle);
                }
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = new CreateParams();
                cp.Style = NativeMethods.WS_POPUP;
                cp.Style |= NativeMethods.WS_CLIPCHILDREN;
                cp.ExStyle = NativeMethods.WS_EX_TOOLWINDOW;
                cp.ExStyle |= NativeMethods.WS_EX_TOPMOST;

                if (this.isLayeredWindow)
                {
                    cp.ExStyle |= NativeMethods.WS_EX_LAYERED;
                }

                if (this.dropShadow && OSFeature.IsPresent(SystemParameter.DropShadow))
                {
                    cp.ClassStyle |= NativeMethods.CS_DROPSHADOW;
                }

                cp.Width = this.Size.Width;
                cp.Height = this.Size.Height;
                cp.X = this.Location.X;
                cp.Y = this.Location.Y;
                return cp;
            }
        }

        /// <summary>
        /// Gets the RadElement that owns this popup.
        /// </summary>
        [Browsable(false)]
        public RadElement OwnerElement
        {
            get
            {
                return this.ownerElement;
            }
        }

        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.OwnerPopup"/>
        /// </summary>
        [Browsable(false)]
        public IPopupControl OwnerPopup
        {
            get
            {
                if (this.ownerElement == null
                    || !this.ownerElement.IsInValidState(true))
                    return null;

                Control ownerControl = this.ownerElement.ElementTree.Control;

                while (ownerControl != null)
                {
                    if (ownerControl is IPopupControl)
                    {
                        return ownerControl as IPopupControl;
                    }

                    ownerControl = ownerControl.Parent;
                }

                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the popup based on the value
        /// set to its Location property.
        /// </summary>
        public new void Show()
        {
            this.ShowPopup(new Rectangle(this.Location, Size.Empty));
        }

        /// <summary>
        /// Shows the popup at the location passed
        /// as a parameter. The location is in screen coordinates
        /// </summary>
        /// <param name="screenLocation">An instance of the <see cref="System.Drawing.Point"/> struct that represents the location.</param>
        public void Show(Point screenLocation)
        {
            this.ShowPopup(new Rectangle(screenLocation, Size.Empty));
        }

        /// <summary>
        /// Shows the control based on the screen rectangle
        /// of a given control.
        /// </summary>
        /// <param name="control">The control which defines the location of the popup.</param>
        public void Show(Control control)
        {
            Point location = control.PointToScreen(Point.Empty);
            this.ShowPopup(new Rectangle(location, control.Size));
        }

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public new void Hide()
        {
            this.ClosePopup(RadPopupCloseReason.CloseCalled);
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            if (element == null)
            {
                return false;
            }

            return base.ProcessFocusRequested(element);
        }

        protected override void WndProc(ref Message m)
        {
            //do not handle input events such as Mouse and Keyboard while animating
            if (this.dropDownAnimating && this.IsInputMessage(m.Msg))
            {
                return;
            }

            switch (m.Msg)
            {
                case NativeMethods.WM_MOUSEACTIVATE:
                    {
                        this.OnWMMouseActivate(ref m);
                        return;
                    }
                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        private bool IsInputMessage(int message)
        {
            if (message >= NativeMethods.WM_MOUSEFIRST && message <= NativeMethods.WM_MOUSELAST)
            {
                return true;
            }

            if (message >= NativeMethods.WM_KEYFIRST && message <= NativeMethods.WM_KEYLAST)
            {
                return true;
            }

            return message == NativeMethods.WM_MOUSEENTER || message == NativeMethods.WM_MOUSELEAVE;
        }

        private void OnWMMouseActivate(ref Message m)
        {
            m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
        }

        private void UpdateOpacitySettings(float opacity)
        {
            if (!this.isLayeredWindow && opacity < 1.0)
            {
                this.isLayeredWindow = true;
                this.UpdateStyles();
            }

            if (this.isLayeredWindow)
            {
                byte opacityValue = (byte)(opacity * 255);

                NativeMethods.SetLayeredWindowAttributes(new HandleRef(this, this.Handle), 0, opacityValue, NativeMethods.LWA_ALPHA);
            }
            else
            {
                this.isLayeredWindow = false;
                this.UpdateStyles();
            }
        }

        internal void CallOnPopupOpened()
        {
            RadPopupOpenedEventHandler popupOpenedHandler = this.Events[this.PopupOpenedEventKey] as RadPopupOpenedEventHandler;

            if (popupOpenedHandler != null)
            {
                EventArgs args = new EventArgs();

                popupOpenedHandler(this, args);
            }

            this.ownerPopupInternal = this.OwnerPopup;
            this.OnPopupOpened();
        }

        /// <summary>
        /// Fires when the popup is opened.
        /// </summary>
        protected virtual void OnPopupOpened()
        {
        }

        internal void CallOnPopupOpening(CancelEventArgs args)
        {
            RadPopupOpeningEventHandler popupOpeningHandler = this.Events[this.PopupOpeningEventKey] as RadPopupOpeningEventHandler;

            if (popupOpeningHandler != null)
            {
                popupOpeningHandler(this, args);
            }

            this.OnPopupOpening(args);
        }

        /// <summary>
        /// Fires when the popup is about to open.
        /// </summary>
        /// <param name="args">A CancelEventArgs object that contains information about the event</param>
        protected virtual void OnPopupOpening(CancelEventArgs args)
        {

        }

        internal void CallOnPopupClosed(RadPopupClosedEventArgs args)
        {
            RadPopupClosedEventHandler popupClosedHandler = this.Events[this.PopupClosedEventKey] as RadPopupClosedEventHandler;

            if (popupClosedHandler != null)
            {
                popupClosedHandler(this, args);
            }

            this.OnPopupClosed(args);
        }

        /// <summary>
        /// Fires when the popup is closed.
        /// </summary>
        /// <param name="args">A RadPopupClosedEventArgs instance
        /// that contains information about what caused the popup to close.</param>
        protected virtual void OnPopupClosed(RadPopupClosedEventArgs args)
        {

        }

        internal void CallOnPopupClosing(RadPopupClosingEventArgs args)
        {
            RadPopupClosingEventHandler popupClosingHandler = this.Events[this.PopupClosingEventKey] as RadPopupClosingEventHandler;

            if (popupClosingHandler != null)
            {
                popupClosingHandler(this, args);
            }

            this.OnPopupClosing(args);
        }

        /// <summary>
        /// Fires when the popup is about to close.
        /// </summary>
        /// <param name="args">A RadPopupClosingEventArgs instance that
        /// contains information about the event</param>
        protected virtual void OnPopupClosing(RadPopupClosingEventArgs args)
        {
        }

        #endregion

        #region Animation

        private void UpdateFadeAnimationSettings()
        {
            if (this.fadeAnimationType == UI.FadeAnimationType.None)
            {
                return;
            }
            if (this.fadeAnimationTimer == null)
            {
                this.fadeAnimationTimer = new Timer();
                this.fadeAnimationTimer.Tick += new EventHandler(this.OnFadeAnimationTimer_Tick);
                this.fadeAnimationTimer.Interval = this.fadeAnimationSpeed;
                this.performFadeInStepInvoker = new MethodInvoker(this.PerformFadeInStep);
                this.performFadeOutStepInvoker = new MethodInvoker(this.PerformFadeOutStep);
            }

            this.fadeAnimationTimer.Interval = this.fadeAnimationSpeed;
            this.calculatedAnimationStep = (float)(1.0 / this.fadeAnimationFrames);
        }

        private void OnFadeAnimationTimer_Tick(object sender, EventArgs e)
        {





            if (this.animationStateOpening)
            {
                if (this.IsHandleCreated && !(this.Disposing || this.IsDisposed))
                {
                    this.Invoke(this.performFadeInStepInvoker);
                }
                else
                {
                    this.fadeAnimationTimer.Stop();
                    this.OnFadeAnimationFinished(this.animationStateOpening);
                }
            }
            else
            {
                if (this.IsHandleCreated && !(this.Disposing || this.IsDisposed))
                {
                    this.Invoke(this.performFadeOutStepInvoker);
                }
                else
                {
                    this.fadeAnimationTimer.Stop();
                    this.OnFadeAnimationFinished(this.animationStateOpening);
                }
            }
        }

        private void PerformFadeOutStep()
        {





            if (this.OpacityInternal - this.calculatedAnimationStep > 0)
            {
                this.OpacityInternal -= this.calculatedAnimationStep;
            }
            else
            {
                this.OpacityInternal = 0;
                this.fadeAnimationTimer.Stop();

                this.OnFadeAnimationFinished(this.animationStateOpening);
            }
        }

        private void PerformFadeInStep()
        {





            if (this.OpacityInternal + this.calculatedAnimationStep < this.Opacity)
            {
                this.OpacityInternal += this.calculatedAnimationStep;
            }
            else
            {
                this.fadeAnimationTimer.Stop();
                this.OpacityInternal = this.Opacity;
                this.OnFadeAnimationFinished(this.animationStateOpening);
            }
        }

        private void OnFadeAnimationFinished(bool isFadingIn)
        {
            if (!isFadingIn)
            {
                this.ClosePopupCore();
            }

            RadPopupFadeAnimationFinishedEventHandler handler = this.Events[this.FadeAnimationFinishedKey] as RadPopupFadeAnimationFinishedEventHandler;

            if (handler != null)
            {
                handler(this, new FadeAnimationEventArgs(isFadingIn));
            }
        }

        #endregion

        #region Service methods

        protected virtual void InitializeDropDownAnimation(Point location)
        {
            this.backupSize = this.Size;
            this.animationEngine.AnimateMinimumToMaximum = true;
            Rectangle startRectangle = new Rectangle(location, new Size(this.Width, 0));
            Rectangle endRectangle = new Rectangle(location, this.Size);
            switch (this.AnimationProperties.AnimationDirection)
            {
                case RadDirection.Up:
                    {
                        startRectangle = new Rectangle(new Point(location.X, location.Y + this.Height), new Size(this.Width, 0));
                        endRectangle = new Rectangle(new Point(location.X, location.Y), this.Size);
                        break;
                    }
                case RadDirection.Left:
                    {
                        startRectangle = new Rectangle(new Point(location.X + this.Width, location.Y), new Size(0, this.Height));
                        endRectangle = new Rectangle(new Point(location.X, location.Y), this.Size);
                        break;
                    }
                case RadDirection.Right:
                    {
                        startRectangle = new Rectangle(new Point(location.X, location.Y), new Size(0, this.Height));
                        endRectangle = new Rectangle(new Point(location.X, location.Y), this.Size);
                        break;
                    }
            }


            this.animationEngine.Minimum = startRectangle;
            this.animationEngine.Maximum = endRectangle;

            if (this.AutoSize)
            {
                this.AutoSize = false;
                this.shouldRestoreAutoSize = true;
            }

            this.Size = startRectangle.Size;
            this.Location = startRectangle.Location;
        }

        /// <summary>
        /// Updates the Aero effects support upon property change.
        /// </summary>
        private void UpdateAeroEffectState()
        {
            bool isCompositionEnabled = DWMAPI.IsCompositionEnabled;
            DWMAPI.DWMBLURBEHIND blurInfo = new DWMAPI.DWMBLURBEHIND();

            if (isCompositionEnabled && this.aeroEnabled)
            {
                blurInfo.dwFlags = NativeMethods.DWM_BB_ENABLE;
                blurInfo.fEnable = true;
            }
            else if (isCompositionEnabled && !this.aeroEnabled)
            {
                blurInfo.dwFlags = NativeMethods.DWM_BB_ENABLE;
                blurInfo.fEnable = false;
            }

            if (isCompositionEnabled)
            {
                DWMAPI.DwmEnableBlurBehindWindow(this.Handle, ref blurInfo);
            }
        }

        /// <summary>
        /// Updates the location of the popup based on the
        /// alignment rectangle and the current alignment settings.
        /// You can adjust the alignment settings by using the
        /// VerticalPopupAlignment and HorizontalPopupAlignment properties.
        /// </summary>
        /// <param name="alignmentRectangle">The alignment rectangle based on which the popup is positioned.</param>
        public void UpdateLocation(Rectangle alignmentRectangle)
        {
            this.lastAlignmentRectangle = alignmentRectangle;
            this.Location = this.GetCorrectedLocation(lastAlignmentRectangle);
        }

        /// <summary>
        /// Updates the location of the popup based on the last used
        /// alignment rectangle and the current alignment settings.
        /// You can adjust the alignment settings by using the
        /// VerticalPopupAlignment and HorizontalPopupAlignment properties.
        /// </summary>
        public void UpdateLocation()
        {
            this.Location = this.GetCorrectedLocation(this.lastAlignmentRectangle);
        }

        /// <summary>
        /// This method returns a point which defines the position of the popup.
        /// By default, aligns the popup based on the <paramref name="alignmentRectangle"/>
        /// and the current alignment settings. You can adjust the alignment settings
        /// by settin the HorizontalPopupAlignment and VerticalPopupAlignment properties.
        /// </summary>
        /// <param name="alignmentRectangle">The alignment rectangle based on which
        /// the popup is aligned.</param>
        /// <returns>An instance of the <see cref="System.Drawing.Point"/> struct
        /// that represents the calculated position of the popup.</returns>
        protected virtual Point GetCorrectedLocation(Rectangle alignmentRectangle)
        {
            return this.GetCorrectedLocation(this.GetCurrentScreen(alignmentRectangle), alignmentRectangle);
        }

        /// <summary>
        /// This method returns a point which defines the position of the popup.
        /// By default, aligns the popup based on the <paramref name="alignmentRectangle"/>
        /// and the current alignment settings. You can adjust the alignment settings
        /// by settin the HorizontalPopupAlignment and VerticalPopupAlignment properties.
        /// </summary>
        /// <param name="currentScreen">An instance of the <see cref="Screen"/> class
        /// that represents the screen where the popup is about to be positioned.</param>
        /// <param name="alignmentRectangle">The alignment rectangle based on which
        /// the popup is aligned.</param>
        /// <returns>An instance of the <see cref="System.Drawing.Point"/> struct
        /// that represents the calculated position of the popup.</returns>
        protected virtual Point GetCorrectedLocation(Screen currentScreen, Rectangle alignmentRectangle)
        {
            Point proposedPoint = new Point(
                    this.GetHorizontalPopupLocation(alignmentRectangle),
                    this.GetVerticalPopupLocation(alignmentRectangle));

            Point correctedPoint = proposedPoint;

            //These two methods try to find reasonable screen location for the popup according to the popup alignment and
            //the alignment correction mode settings.
            correctedPoint = this.GetCorrectedHorizontalLocation(currentScreen, alignmentRectangle, correctedPoint);
            correctedPoint = this.GetCorrectedVerticalLocation(currentScreen, alignmentRectangle, correctedPoint);

            //This method checks whether the above location correction routines have found a valid screen location
            //according to the popup alignment and the alignment correction mode settings. If not, the method
            //uses another approach to find a reasonable location for the popup and does not obey the popup alignment
            //and the alignment correction mode settings.
            correctedPoint = this.CheckMakeLastLocationCorrection(
                alignmentRectangle,
                correctedPoint,
                this.GetAvailableBoundsFromScreen(currentScreen));

            return correctedPoint;
        }

        protected internal virtual Size ApplySizingConstraints(Size availableSize, Screen currentScreen)
        {
            int width = availableSize.Width;
            int height = availableSize.Height;
            Rectangle screenBounds = this.GetAvailableBoundsFromScreen(currentScreen);

            if ((this.fitToScreenMode & FitToScreenModes.FitHeight) != 0)
            {
                if (height > screenBounds.Height)
                {
                    height = screenBounds.Height;
                }
            }

            if ((this.fitToScreenMode & FitToScreenModes.FitWidth) != 0)
            {
                if (width > screenBounds.Width)
                {
                    width = screenBounds.Width;
                }
            }

            return new Size(width, height);
        }

        /// <summary>
        /// Gets the screen on which the popup will be displayed.
        /// </summary>
        /// <param name="alignmentRectangle">The alignment rectangle for the popup.</param>
        /// <returns>An instance of the <see cref="Screen"/> class that represents
        /// the screen where the popup will be displayed.</returns>
        protected internal virtual Screen GetCurrentScreen(Rectangle alignmentRectangle)
        {
            return Screen.FromRectangle(alignmentRectangle);
        }

        /// <summary>
        /// Gets an instance of the <see cref="Screen"/> class
        /// that represents the screen where the popup is displayed.
        /// </summary>
        public Screen GetCurrentScreen()
        {
            return this.GetCurrentScreen(this.lastAlignmentRectangle);
        }

        /// <summary>
        /// Gets a <see cref="Rectangle"/> which represents the available bounds for the popup to show.
        /// By default this method returns the bounds of the screen.
        /// </summary>
        /// <param name="screen">An instance of the <see cref="Screen"/> class that represents
        /// the active screen where the popup is about to be shown.</param>
        /// <returns>An instance of the <see cref="Rectangle"/> struct that represents the
        /// available bounds for the popup based on the active screen.</returns>
        protected internal virtual Rectangle GetAvailableBoundsFromScreen(Screen screen)
        {
            if (this.screenSpaceMode == ScreenSpaceMode.WorkingArea)
            {
                return screen.WorkingArea;
            }

            return screen.Bounds;
        }

        /// <summary>
        /// Calculates the horizontal position of the popup
        /// according to the current <see cref="Telerik.WinControls.UI.HorizontalPopupAlignment"/>
        /// and <see cref="Telerik.WinControls.UI.AlignmentCorrectionMode"/>.
        /// </summary>
        /// <param name="currentScreen">The screen in which the popup will be aligned.</param>
        /// <param name="alignmentRectangle">The alignment rectangle of the popup.</param>
        /// <param name="calculatedLocation">The calculated location that will be corrected if needed.</param>
        /// <returns>An instance of the <see cref="Point"/> struct that represents the corrected location of the popup</returns>
        protected virtual Point GetCorrectedHorizontalLocation(Screen currentScreen, Rectangle alignmentRectangle, Point calculatedLocation)
        {
            Rectangle availableBounds = this.GetAvailableBoundsFromScreen(currentScreen);
            switch (this.horizontalAlignmentCorrectionMode)
            {
                case AlignmentCorrectionMode.Smooth:
                    {

                        return this.AlignHorizontallyWithoutSnapping(calculatedLocation, alignmentRectangle, availableBounds);
                    }
                case AlignmentCorrectionMode.SnapToEdges:
                    {
                        return this.SnapToEdgeHorizontally(calculatedLocation, alignmentRectangle, availableBounds);
                    }
                case AlignmentCorrectionMode.SnapToOuterEdges:
                    {
                        return this.SnapToOuterEdgeHorizontally(calculatedLocation, alignmentRectangle, availableBounds);
                    }
            }

            return calculatedLocation;
        }

        private Point AlignHorizontallyWithoutSnapping(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            int proposedXCoord = proposedLocation.X;
            Size popupSize = this.Size;
            int leftAvailableSpace = proposedXCoord - bounds.Left;
            int rightAvailableSpace = bounds.Right - (proposedXCoord + popupSize.Width);

            if (leftAvailableSpace > 0 && rightAvailableSpace < 0)
            {
                if (Math.Abs(rightAvailableSpace) <= leftAvailableSpace)
                {
                    proposedXCoord -= Math.Abs(rightAvailableSpace);
                    return new Point(proposedXCoord, proposedLocation.Y);
                }
            }
            else if (leftAvailableSpace < 0 && rightAvailableSpace > 0)
            {
                if (rightAvailableSpace >= Math.Abs(leftAvailableSpace))
                {
                    proposedXCoord += Math.Abs(leftAvailableSpace);
                    return new Point(proposedXCoord, proposedLocation.Y);
                }
            }

            return proposedLocation;
        }

        private Point SnapToOuterEdgeHorizontally(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            Rectangle alignmentRectInScreen = Rectangle.Intersect(alignmentRectangle, bounds);
            Size popupSize = this.Size;

            if (alignmentRectInScreen != Rectangle.Empty &&
                alignmentRectangle.Width != alignmentRectInScreen.Width)
            {
                if (alignmentRectangle.Right > bounds.Right
                    && alignmentRectangle.Left > bounds.Left)
                {
                    if (alignmentRectangle.Left - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Left - popupSize.Width, proposedLocation.Y);
                    }
                }
                else if (alignmentRectangle.Left < bounds.Left
                    && alignmentRectangle.Right <= bounds.Right)
                {
                    if (alignmentRectangle.Right + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Right, proposedLocation.Y);
                    }
                }
            }
            else
            {
                if (proposedLocation.X < bounds.Left)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(bounds.Left, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Right + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Right, proposedLocation.Y);
                    }
                }
                else if (proposedLocation.X + popupSize.Width > bounds.Right)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(bounds.Right - popupSize.Width, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Left - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Left - popupSize.Width, proposedLocation.Y);
                    }
                }
            }

            return proposedLocation;
        }

        private Point SnapToEdgeHorizontally(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            int xCoord = proposedLocation.X;
            Rectangle alignmentRectInScreen = Rectangle.Intersect(alignmentRectangle, bounds);
            Size popupSize = this.Size;

            if (alignmentRectInScreen != Rectangle.Empty &&
                alignmentRectangle.Width != alignmentRectInScreen.Width)
            {
                if (alignmentRectangle.Right > bounds.Right
                    && alignmentRectangle.Left >= bounds.Left)
                {
                    if (alignmentRectangle.Left + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Left, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Left <= bounds.Right &&
                        alignmentRectangle.Left - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Left - popupSize.Width, proposedLocation.Y);
                    }
                    else if (bounds.Right - popupSize.Width >= bounds.Left)
                    {
                        return new Point(bounds.Right - popupSize.Width, proposedLocation.Y);
                    }
                }
                else if (alignmentRectangle.Left < bounds.Left
                    && alignmentRectangle.Right <= bounds.Right)
                {
                    if (alignmentRectangle.Right - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Right - popupSize.Width, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Right >= bounds.Left &&
                        alignmentRectangle.Right + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Right, proposedLocation.Y);
                    }
                    else if (bounds.Left - popupSize.Width <= bounds.Right)
                    {
                        return new Point(bounds.Left - popupSize.Width, proposedLocation.Y);
                    }
                }
            }
            else
            {
                if (xCoord < bounds.Left)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(bounds.Left, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Right - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Right - popupSize.Width, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Left + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Left, proposedLocation.Y);
                    }

                }
                else if (xCoord + popupSize.Width > bounds.Right)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(bounds.Right - popupSize.Width, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Left + popupSize.Width <= bounds.Right)
                    {
                        return new Point(alignmentRectangle.Left, proposedLocation.Y);
                    }
                    else if (alignmentRectangle.Right - popupSize.Width >= bounds.Left)
                    {
                        return new Point(alignmentRectangle.Right - popupSize.Width, proposedLocation.Y);
                    }
                }
            }

            return proposedLocation;
        }

        /// <summary>
        /// Calculates the vertical position of the popup
        /// according to the current <see cref="Telerik.WinControls.UI.VerticalPopupAlignment"/>
        /// and <see cref="Telerik.WinControls.UI.AlignmentCorrectionMode"/>.
        /// </summary>
        /// <param name="currentScreen">The screen in which the popup will be aligned.</param>
        /// <param name="alignmentRectangle">The alignment rectangle of the popup.</param>
        /// <param name="calculatedLocation">The calculated location that will be corrected if needed.</param>
        /// <returns>An integer that represents the corrected vertical location of the popup</returns>
        protected virtual Point GetCorrectedVerticalLocation(Screen currentScreen, Rectangle alignmentRectangle, Point calculatedLocation)
        {
            Rectangle availableBounds = this.GetAvailableBoundsFromScreen(currentScreen);
            switch (this.verticalAlignmentCorrectionMode)
            {
                case AlignmentCorrectionMode.Smooth:
                    {
                        return this.AlignVerticallyWithoutSnapping(calculatedLocation, alignmentRectangle, availableBounds);
                    }
                case AlignmentCorrectionMode.SnapToEdges:
                    {
                        return this.SnapToEdgeVertically(calculatedLocation, alignmentRectangle, availableBounds);
                    }
                case AlignmentCorrectionMode.SnapToOuterEdges:
                    {
                        return this.SnapToOuterEdgeVertically(calculatedLocation, alignmentRectangle, availableBounds);
                    }
            }

            return calculatedLocation;
        }

        private Point AlignVerticallyWithoutSnapping(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            Size popupSize = this.Size;

            int bottomAvailableSpace = bounds.Bottom - (proposedLocation.Y + popupSize.Height);
            int topAvailableSpace = proposedLocation.Y - bounds.Top;

            if (bottomAvailableSpace > 0 && topAvailableSpace < 0)
            {
                if (Math.Abs(topAvailableSpace) <= bottomAvailableSpace)
                {
                    return new Point(proposedLocation.X, Math.Abs(topAvailableSpace) + proposedLocation.Y);
                }
            }
            else if (bottomAvailableSpace < 0 && topAvailableSpace > 0)
            {
                if (topAvailableSpace >= Math.Abs(bottomAvailableSpace))
                {
                    return new Point(proposedLocation.X, proposedLocation.Y - Math.Abs(bottomAvailableSpace));
                }
            }

            return proposedLocation;
        }

        private Point SnapToOuterEdgeVertically(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            Rectangle alignmentRectInScreen = Rectangle.Intersect(alignmentRectangle, bounds);
            Size popupSize = this.Size;
            if (alignmentRectInScreen != Rectangle.Empty &&
                alignmentRectangle.Height != alignmentRectInScreen.Height)
            {
                if (alignmentRectangle.Bottom > bounds.Bottom
                    && !(alignmentRectangle.Top < bounds.Top))
                {
                    if (alignmentRectangle.Top - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top - popupSize.Height);
                    }
                }
                else if (alignmentRectangle.Top < bounds.Top
                    && !(alignmentRectangle.Bottom > bounds.Bottom))
                {
                    if (alignmentRectangle.Bottom + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom);
                    }
                }
            }
            else
            {
                if (proposedLocation.Y < bounds.Top)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(proposedLocation.X, bounds.Top);
                    }
                    else if (alignmentRectangle.Bottom + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom);
                    }
                }
                else if (proposedLocation.Y + popupSize.Height > bounds.Bottom)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(proposedLocation.X, bounds.Bottom - popupSize.Height);
                    }
                    else if (alignmentRectangle.Top - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top - popupSize.Height);
                    }
                }
            }

            return proposedLocation;
        }

        private Point SnapToEdgeVertically(Point proposedLocation, Rectangle alignmentRectangle, Rectangle bounds)
        {
            Rectangle alignmentRectInScreen = Rectangle.Intersect(alignmentRectangle, bounds);
            Size popupSize = this.Size;
            if (alignmentRectInScreen != Rectangle.Empty &&
                alignmentRectangle.Height != alignmentRectInScreen.Height)
            {
                if (alignmentRectangle.Bottom > bounds.Bottom
                    && alignmentRectangle.Top >= bounds.Top)
                {
                    if (alignmentRectangle.Top + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top);
                    }
                    else if (alignmentRectangle.Top <= bounds.Bottom &&
                        alignmentRectangle.Top - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top - popupSize.Height);
                    }
                    else if (bounds.Bottom - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, bounds.Bottom - popupSize.Height);
                    }
                }
                else if (alignmentRectangle.Top < bounds.Top
                    && alignmentRectangle.Bottom <= bounds.Bottom)
                {
                    if (alignmentRectangle.Bottom - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom - popupSize.Height);
                    }
                    else if (alignmentRectangle.Bottom >= bounds.Top &&
                        alignmentRectangle.Bottom + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom);
                    }
                    else if (bounds.Top + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, bounds.Top);
                    }
                }
            }
            else
            {
                if (proposedLocation.Y < bounds.Top)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(proposedLocation.X, bounds.Top);
                    }
                    else if (alignmentRectangle.Bottom - popupSize.Height >= bounds.Top)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom - popupSize.Height);
                    }
                    else if (alignmentRectangle.Top + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top);
                    }
                }
                else if (proposedLocation.Y + popupSize.Height > bounds.Bottom)
                {
                    if (alignmentRectInScreen == Rectangle.Empty)
                    {
                        return new Point(proposedLocation.X, bounds.Bottom - popupSize.Height);
                    }
                    else if (alignmentRectangle.Top + popupSize.Height <= bounds.Bottom)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Top);
                    }
                    else if (alignmentRectangle.Bottom - bounds.Top >= popupSize.Height)
                    {
                        return new Point(proposedLocation.X, alignmentRectangle.Bottom - popupSize.Height);
                    }
                }
            }

            return proposedLocation;
        }

        /// <summary>
        /// Calculates the horizontal popup location based on the <paramref name="alignmentRectangle"/>.
        /// This method uses the HorizontalPopupAlignment property setting.
        /// </summary>
        /// <param name="alignmentRectangle">An instance of the <see cref="System.Drawing.Rectangle"/> struct
        /// that represents the alignment rectangle.</param>
        /// <returns>Returns an integer that represents the X coordinate of the popup.</returns>
        protected virtual int GetHorizontalPopupLocation(Rectangle alignmentRectangle)
        {
            int result = 0;

            switch (this.horizontalPopupAlignment)
            {
                case HorizontalPopupAlignment.LeftToLeft:
                    {
                        result = alignmentRectangle.Left;
                        break;
                    }
                case HorizontalPopupAlignment.LeftToRight:
                    {
                        result = alignmentRectangle.Right;
                        break;
                    }
                case HorizontalPopupAlignment.RightToLeft:
                    {
                        result = alignmentRectangle.Left - this.Width;
                        break;
                    }
                case HorizontalPopupAlignment.RightToRight:
                    {
                        result = alignmentRectangle.Right - this.Width;
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// Calculates the vertical popup location based on the <paramref name="alignmentRectangle"/>.
        /// This method uses the VerticalPopupAlignment property setting.
        /// </summary>
        /// <param name="alignmentRectangle">An instance of the <see cref="System.Drawing.Rectangle"/> struct
        /// that represents the alignment rectangle.</param>
        /// <returns>Returns an integer that represents the Y coordinate of the popup.</returns>
        protected virtual int GetVerticalPopupLocation(Rectangle alignmentRectangle)
        {
            int result = 0;

            switch (this.verticalPopupAlignment)
            {
                case VerticalPopupAlignment.TopToBottom:
                    {
                        result = alignmentRectangle.Bottom;
                        break;
                    }
                case VerticalPopupAlignment.BottomToBottom:
                    {
                        result = alignmentRectangle.Bottom - this.Height;
                        break;
                    }
                case VerticalPopupAlignment.BottomToTop:
                    {
                        result = alignmentRectangle.Top - this.Height;
                        break;
                    }
                case VerticalPopupAlignment.TopToTop:
                    {
                        result = alignmentRectangle.Top;
                        break;
                    }
            }

            return result;
        }

        /// <summary>
        /// Fires when a drop-down animation is about to begin.
        /// </summary>
        protected virtual void AnimationStarting()
        {
            this.dropDownAnimating = true;
        }

        /// <summary>
        /// This method is executed when the popup needs to receive manual horizontal alignment.
        /// This can happen when there is no reasonable possibility for the
        /// alignment routines to define a proper horizontal position for the popup.
        /// In this way the developer is enabled to define a horizontal position
        /// according to their preferences.
        /// </summary>
        /// <param name="alignmentRectangle">The proposed alignment rectangle with screen coordinates.</param>.
        /// <param name="proposedLocation">The proposed coordinates</param>.
        /// <param name="availableBounds">The proposed available space for the popup.</param>.
        /// <returns>An instance of the <see cref="Point"/> struct that represents the location of the popup.</returns>
        protected virtual Point OnAlternativeXLocationNeeded(Rectangle alignmentRectangle, Point proposedLocation, Rectangle availableBounds)
        {
            if (this.alignmentRectangleOverlapMode == AlternativeCorrectionMode.Overlap)
            {
                Rectangle popupBounds = new Rectangle(proposedLocation, this.Size);
                if (popupBounds.Left < availableBounds.Left)
                {
                    return new Point(availableBounds.Left, proposedLocation.Y);
                }
                else if (popupBounds.Right > availableBounds.Right)
                {
                    return new Point(availableBounds.Right - this.Size.Width, proposedLocation.Y);
                }

                return proposedLocation;
            }

            Size popupSize = this.Size;

            if (availableBounds.Bottom - alignmentRectangle.Bottom >= popupSize.Height)
            {
                return new Point(availableBounds.Right - popupSize.Width, alignmentRectangle.Bottom);
            }
            else if (alignmentRectangle.Top - availableBounds.Top >= popupSize.Height)
            {
                return new Point(availableBounds.Right - popupSize.Width, alignmentRectangle.Top - popupSize.Height);
            }

            return proposedLocation;
        }

        /// <summary>
        /// Checks whether the current alignment rectangle intersects with the popup's bounds
        /// according to a given popup location.
        /// </summary>
        /// <param name="alignmentRectangle">An instance of the <see cref="Rectangle"/> struct that represents
        /// the current alignment rectangle.</param>
        /// <param name="proposedLocation">An instance of the <see cref="Point"/> struct that represents the proposed popup location.</param>
        /// <param name="availableBounds">An instance of the <see cref="Rectangle"/> struct that represents the available bounds on the screen.</param>
        /// <returns>An instance of the <see cref="Point"/> struct that represents the result of the operation.</returns>
        protected virtual Point CheckMakeLastLocationCorrection(Rectangle alignmentRectangle, Point proposedLocation, Rectangle availableBounds)
        {
            Rectangle popupBounds = new Rectangle(proposedLocation, this.Size);
            Rectangle intersectedPopupBounds = Rectangle.Intersect(availableBounds, popupBounds);

            if (popupBounds.Width != intersectedPopupBounds.Width)
            {
                return this.OnAlternativeXLocationNeeded(alignmentRectangle, proposedLocation, availableBounds);
            }
            else if (popupBounds.Height != intersectedPopupBounds.Height)
            {
                return this.OnAlternativeYLocationNeeded(alignmentRectangle, proposedLocation, availableBounds);
            }

            return proposedLocation;
        }

        /// <summary>
        /// This method is executed when the popup needs to receive manual vertical alignment.
        /// This can happen when there is no reasonable possibility for the
        /// alignment routines to define a proper vertical position for the popup.
        /// In this way the developer is enabled to define a vertical position
        /// according to their preferences.
        /// </summary>
        /// <param name="alignmentRectangle">The proposed alignment rectangle with screen coordinates.</param>.
        /// <param name="proposedLocation">The proposed coordinates</param>.
        /// <param name="availableBounds">The proposed available space for the popup.</param>.
        /// <returns>An instance of the <see cref="Point"/> struct that represents the location of the popup.</returns>
        protected virtual Point OnAlternativeYLocationNeeded(Rectangle alignmentRectangle, Point proposedLocation, Rectangle availableBounds)
        {
            if (this.alignmentRectangleOverlapMode == AlternativeCorrectionMode.Overlap)
            {
                Rectangle popupBounds = new Rectangle(proposedLocation, this.Size);
                if (popupBounds.Top < availableBounds.Top)
                {
                    return new Point(proposedLocation.X, availableBounds.Top);
                }
                else if (popupBounds.Bottom > availableBounds.Bottom)
                {
                    return new Point(proposedLocation.X, availableBounds.Bottom - this.Size.Height);
                }
                return proposedLocation;
            }

            Size popupSize = this.Size;

            int newYCoord = proposedLocation.Y < availableBounds.Top ?
                availableBounds.Top : availableBounds.Bottom - popupSize.Height;
            if (availableBounds.Right - alignmentRectangle.Right >= popupSize.Width)
            {
                return new Point(alignmentRectangle.Right, newYCoord);
            }
            else if (alignmentRectangle.Left - availableBounds.Left >= popupSize.Width)
            {
                return new Point(alignmentRectangle.Left - popupSize.Width, availableBounds.Bottom - popupSize.Height);
            }

            return proposedLocation;
        }

        private void ClosePopupCore()
        {
            this.SetVisibleCore(false);
            this.dropDownAnimating = false;
        }

        private void ShowPopupCore(Size size, Point location)
        {
            this.SetBoundsCore(location.X, location.Y, size.Width, size.Height, BoundsSpecified.All);
            NativeMethods.ShowWindow(this.Handle, NativeMethods.SW_SHOWNOACTIVATE);

            if (DWMAPI.IsCompositionEnabled && this.aeroEnabled)
            {
                this.UpdateAeroEffectState();
            }

            ControlHelper.BringToFront(this.Handle, false);
        }

        #endregion

        #region IPopupControlMembers

        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.Children"/>
        /// </summary>
        public List<IPopupControl> Children
        {
            get
            {
                if (this.children == null)
                {
                    this.children = new List<IPopupControl>();
                }

                return this.children;
            }
        }

        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.ShowPopup"/>
        /// </summary>
        public virtual void ShowPopup(Rectangle alignmentRectangle)
        {
            if (PopupManager.Default.ContainsPopup(this))
            {
                return;
            }

            this.lastAlignmentRectangle = alignmentRectangle;
            Screen currentScreen = this.GetCurrentScreen(alignmentRectangle);
            Point location = this.GetCorrectedLocation(alignmentRectangle);
            Size popupSize = this.ApplySizingConstraints(this.Size, currentScreen);
            RadPopupOpeningEventArgs args = new RadPopupOpeningEventArgs(location);

            this.CallOnPopupOpening(args);

            if (args.Cancel)
            {
                return;
            }
            location = args.CustomLocation;
            PopupManager.Default.AddPopup(this);

            if ((this.animationType & PopupAnimationTypes.Fade) != 0 && ThemeResolutionService.AllowAnimations)          
            {
                this.InitializeDropDownAnimation(location);
                this.dropDownAnimating = true;
                this.animationEngine.Start();

                this.AnimationStarting();
            }

            bool hasFadeInAnimation = (this.fadeAnimationType & FadeAnimationType.FadeIn) != 0 && (this.animationType & PopupAnimationTypes.Fade) != 0;
            if (hasFadeInAnimation && ThemeResolutionService.AllowAnimations)
            {
                this.animationStateOpening = true;

                this.OpacityInternal = 0;

                this.ShowPopupCore(popupSize, location);
                this.CallOnPopupOpened();

                if (!this.fadeAnimationTimer.Enabled)
                {
                    this.fadeAnimationTimer.Start();
                }
            }
            else
            {
                //If there is no fade in animation,
                //assure that the initial opacity is 100%;
                if (this.opacityInternal != this.Opacity)
                {
                    this.OpacityInternal = this.Opacity;
                }

                this.ShowPopupCore(popupSize, location);
                this.CallOnPopupOpened();
            }
        }

        public virtual void ClosePopup(PopupCloseInfo info)
        {
            if (!PopupManager.Default.ContainsPopup(this))
            {
                return;
            }

            RadPopupClosingEventArgs closingArgs = new RadPopupClosingEventArgs(info.CloseReason);

            this.CallOnPopupClosing(closingArgs);

            if (closingArgs.Cancel)
            {
                info.Closed = false;
                return;
            }

            if ((this.fadeAnimationType & FadeAnimationType.FadeOut) != 0 
                && (this.animationType & PopupAnimationTypes.Fade) != 0
                && ThemeResolutionService.AllowAnimations)
            {
                this.lastCloseReason = info.CloseReason;
                this.animationStateOpening = false;
                this.OpacityInternal = this.opacity;

                if (!this.fadeAnimationTimer.Enabled)
                {
                    this.UpdateStyles();
                    PopupManager.Default.RemovePopup(this);
                    RadPopupClosedEventArgs closedArgs = new RadPopupClosedEventArgs(info.CloseReason);
                    this.CallOnPopupClosed(closedArgs);
                    this.fadeAnimationTimer.Start();
                }
                else
                {
                    info.Closed = false;
                }

            }
            else
            {
                PopupManager.Default.RemovePopup(this);
                this.ClosePopupCore();
                RadPopupClosedEventArgs closedArgs = new RadPopupClosedEventArgs(info.CloseReason);
                this.CallOnPopupClosed(closedArgs);
            }
        }

        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.ClosePopup(RadPopupCloseReason)"/>
        /// </summary>
        public virtual void ClosePopup(RadPopupCloseReason reason)
        {
            PopupCloseInfo info = new PopupCloseInfo(reason, null);
            this.ClosePopup(info);
        }

        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.CanClosePopup"/>
        /// </summary>
        public virtual bool CanClosePopup(RadPopupCloseReason reason)
        {
            return true;
        }


        /// <summary>
        /// <see cref="Telerik.WinControls.UI.IPopupControl.OnKeyDown"/>
        /// </summary>
        /// <param name="keyData"></param>
        public virtual bool OnKeyDown(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                case Keys.Enter:
                case Keys.Back:
                    {
                        this.ClosePopup(RadPopupCloseReason.Keyboard);
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Determines whether the MouseWheel event is handled by the popup.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public virtual bool OnMouseWheel(Control target, int delta)
        {
            return target == this;
        }

        #endregion

        #region Events

        private void OnAnimatingEvent(object sender, AnimationEventArgs e)
        {
            //do double-checking since animating events arive from another thread
            if (!this.Disposing && !this.IsDisposed && this.IsHandleCreated)
            {
                lock (this)
                {
                    if (!this.Disposing && !this.IsDisposed && this.IsHandleCreated)
                    {
                        //use the asynchronous invocation otherwise we will encounter dead lock
                        this.BeginInvoke(this.callbackAnimating, e);
                    }
                }
            }
        }

        private void OnAnimationFinishedEvent(object sender, AnimationEventArgs e)
        {
            //do double-checking since animating events arive from another thread
            if (!this.Disposing && !this.IsDisposed && this.IsHandleCreated)
            {
                lock (this)
                {
                    if (!this.Disposing && !this.IsDisposed && this.IsHandleCreated)
                    {
                        //use the asynchronous invocation otherwise we will encounter dead lock
                        this.BeginInvoke(this.callbackAnimationFinished, e);
                    }
                }
            }
        }

        protected virtual void OnAnimationFinished(AnimationEventArgs args)
        {
            if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
            {
                return;
            }

            this.dropDownAnimating = false;
            if (this.shouldRestoreAutoSize)
            {
                this.AutoSize = true;
            }
            this.Size = this.backupSize;
        }

        protected virtual void OnAnimating(AnimationEventArgs e)
        {
            if (this.Disposing || this.IsDisposed || !this.IsHandleCreated)
            {
                return;
            }

            Rectangle frameRectangle = ((Rectangle)e.AnimationValue);

            this.Location = frameRectangle.Location;
            this.Size = frameRectangle.Size;
        }

        /// <summary>
        /// Fires when a fade animation has finished. The
        /// event args contain information about the type of the animation.
        /// </summary>
        public event RadPopupFadeAnimationFinishedEventHandler FadeAnimationFinished
        {
            add
            {
                this.Events.AddHandler(this.FadeAnimationFinishedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(this.FadeAnimationFinishedKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup-form is about to be opened.
        /// </summary>
        public event RadPopupOpeningEventHandler PopupOpening
        {
            add
            {
                this.Events.AddHandler(this.PopupOpeningEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(this.PopupOpeningEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup-form is opened.
        /// </summary>
        public event RadPopupOpenedEventHandler PopupOpened
        {
            add
            {
                this.Events.AddHandler(this.PopupOpenedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(this.PopupOpenedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup is about to be closed.
        /// </summary>
        public event RadPopupClosingEventHandler PopupClosing
        {
            add
            {
                this.Events.AddHandler(this.PopupClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(this.PopupClosingEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the popup is closed.
        /// </summary>
        public event RadPopupClosedEventHandler PopupClosed
        {
            add
            {
                this.Events.AddHandler(this.PopupClosedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(this.PopupClosedEventKey, value);
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.animationEngine.Animating -= new AnimationEventHandler(OnAnimatingEvent);
                this.animationEngine.AnimationFinished -= new AnimationEventHandler(OnAnimationFinishedEvent);
                this.animationEngine.Dispose();

                if (this.ownerPopupInternal != null)
                {
                    if (this.ownerPopupInternal.Children.Contains(this))
                    {
                        this.ownerPopupInternal.Children.Remove(this);
                    }
                }

                PopupManager.Default.RemovePopup(this);

                if (this.fadeAnimationTimer != null)
                {
                    this.fadeAnimationTimer.Stop();
                    this.fadeAnimationTimer.Tick -= new EventHandler(this.OnFadeAnimationTimer_Tick);
                    this.fadeAnimationTimer.Dispose();
                }

            }
            base.Dispose(disposing);
        }
        #endregion

        #region ITooltipOwner
       
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return this.ownerElement;       
            }
            set
            {
                this.ownerElement = (RadElement)value;
            }
        }
        #endregion
    }
}

