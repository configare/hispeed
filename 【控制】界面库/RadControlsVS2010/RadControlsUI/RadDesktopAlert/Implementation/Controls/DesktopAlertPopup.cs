using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the popup of a <see cref="RadDesktopAlert"/>component.
    /// This popup hosts an instance of the <see cref="RadDesktopAlertElement"/>class which
    /// represents the element hierarchy of the alert.
    /// </summary>
    [ToolboxItem(false)]
    public class DesktopAlertPopup : RadPopupControlBase
    {
        #region Fields

        private RadDesktopAlertElement alertElement;
        private bool canMove = true;
        private bool isPinned = false;
        private int autoCloseSuspendCount = 0;

        private Point oldMousePosition;
        private float? oldOpacity;
        private int autoCloseDelayInSeconds = 10;
        private bool autoClose = true;
        private Timer autoCloseTimer;

        private RadDesktopAlert ownerAlert;
        internal bool locationModifiedByUser = false;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="DesktopAlertPopup"/>class.
        /// </summary>
        /// <param name="owner">An instance of the <see cref="RadDesktopAlert"/>class that
        /// represents the owner alert of the <see cref="DesktopAlertPopup"/></param>
        public DesktopAlertPopup(RadDesktopAlert owner)
            : this(null, owner)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="DesktopAlertPopup"/>class
        /// with specified owner.
        /// </summary>
        /// <param name="element">An instance of the <see cref="RadElement"/>class that
        /// represents the owner element of the <see cref="DesktopAlertPopup"/></param>
        /// <param name="owner">An instance of the <see cref="RadDesktopAlert"/>class that
        /// represents the owner alert of the <see cref="DesktopAlertPopup"/></param>
        public DesktopAlertPopup(RadElement element, RadDesktopAlert owner)
            : base(element)
        {
            this.ownerAlert = owner;
            this.FadeAnimationType = FadeAnimationType.FadeIn | FadeAnimationType.FadeOut;
            this.Opacity = 0.8f;
            this.FadeAnimationFrames = 80;
            this.AnimationFrames = 50;
            this.EasingType = RadEasingType.Default;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a sets a boolean value determining whether the alert popup will be automatically 
        /// closed after a given amount of time.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the alert's popup is automatically closed.")]
        [DefaultValue(true)]
        public bool AutoClose
        {
            get
            {
                return this.autoClose;
            }
            set
            {
                if (this.autoClose != value)
                {
                    this.autoClose = value;
                    this.OnNotifyPropertyChanged("AutoClose");
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds after
        /// which the alert will be automatically closed.
        /// </summary>
        [Description("Gets or sets the amount of time in seconds after which"
            + "the alert will be automatically closed.")]
        [DefaultValue(10)]
        public int AutoCloseDelay
        {
            get
            {
                return this.autoCloseDelayInSeconds;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The auto-close delay must be non-negative.");
                }

                if (this.autoCloseDelayInSeconds != value)
                {
                    this.autoCloseDelayInSeconds = value;
                    this.OnNotifyPropertyChanged("AutoCloseDelay");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the options button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the options button is shown.")]
        [DefaultValue(true)]
        public bool ShowOptionsButton
        {
            get
            {
                return this.alertElement.ShowOptionsButton;
            }
            set
            {
                this.alertElement.ShowOptionsButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the pin button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the pin button is shown.")]
        [DefaultValue(true)]
        public bool ShowPinButton
        {
            get
            {
                return this.alertElement.ShowPinButton;
            }
            set
            {
                this.alertElement.ShowPinButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the close button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the close button is shown.")]
        [DefaultValue(true)]
        public bool ShowCloseButton
        {
            get
            {
                return this.alertElement.ShowCloseButton;
            }
            set
            {
                this.alertElement.ShowCloseButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the popup is pinned on the screen.
        /// </summary>
        public bool IsPinned
        {
            get
            {
                return this.isPinned;
            }
            set
            {
                if (this.isPinned != value)
                {
                    this.isPinned = value;
                    this.OnNotifyPropertyChanged("IsPinned");
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the popup
        /// can be moved by dragging it by the grip.
        /// </summary>
        public bool CanMove
        {
            get
            {
                return this.canMove;
            }
            set
            {
                if (this.canMove != value)
                {
                    this.canMove = value;
                    this.OnNotifyPropertyChanged("CanMove");
                }
            }
        }

        public override string ThemeClassName
        {
            get
            {
                return typeof(DesktopAlertPopup).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadDesktopAlertElement"/>which
        /// represents the main alert element.
        /// </summary>
        public RadDesktopAlertElement AlertElement
        {
            get
            {
                return this.alertElement;
            }
        }

        #endregion

        #region Virtual properties

        /// <summary>
        /// Gets or sets the caption text of the alert.
        /// </summary>
        [Description("Gets or sets the caption text of the desktop alert.")]
        public virtual string CaptionText
        {
            get
            {
                return this.alertElement.CaptionText;
            }
            set
            {
                this.alertElement.CaptionText = value;
            }
        }

        /// <summary>
        /// Gets or sets the content text of the alert.
        /// </summary>
        [Description("Gets or sets the content text of the desktop alert.")]
        public virtual string ContentText
        {
            get
            {
                return this.alertElement.ContentText;
            }
            set
            {
                this.alertElement.ContentText = value;
            }
        }

        /// <summary>
        /// Gets or sets the content image of the alert.
        /// </summary>
        [Description("Gets or sets the content image of the alert.")]
        public virtual System.Drawing.Image Image
        {
            get
            {
                return this.alertElement.ContentImage;
            }
            set
            {
                this.alertElement.ContentImage = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadItemCollection"/>that
        /// holds the buttons items added to the alert component.
        /// </summary>
        public RadItemCollection ButtonItems
        {
            get
            {
                return this.alertElement.ButtonsPanel.Items;
            }
        }

        public RadItemCollection OptionItems
        {
            get
            {
                return this.alertElement.CaptionElement.TextAndButtonsElement.OptionsButton.Items;
            }
        }

        #endregion

        #region Methods

        #region Auto close

        private void EvaluateAutoCloseParameters()
        {
            if (!this.autoClose)
            {
                return;
            }

            if (this.autoCloseTimer == null)
            {
                this.autoCloseTimer = new Timer();
                this.autoCloseTimer.Tick += new EventHandler(OnAutoCloseTimer_Tick);
            }

            this.autoCloseTimer.Interval = this.autoCloseDelayInSeconds * 1000;
            this.autoCloseTimer.Stop();
            this.autoCloseTimer.Start();
        }

        /// <summary>
        /// Stops the auto-close timer.
        /// </summary>
        public void SuspendAutoClose()
        {
            if (!PopupManager.Default.ContainsPopup(this))
            {
                return;
            }

            if (!this.autoClose || this.autoCloseTimer == null)
            {
                return;
            }

            if (this.autoCloseSuspendCount == 0)
            {
                this.autoCloseTimer.Stop();
            }
            this.autoCloseSuspendCount++;
        }

        /// <summary>
        /// Restarts the auto-close timer.
        /// </summary>
        public void ResumeAutoClose()
        {
            if (!PopupManager.Default.ContainsPopup(this))
            {
                return;
            }

            if (!this.autoClose || this.autoCloseTimer == null)
            {
                return;
            }

            if (this.autoCloseSuspendCount > 0)
            {
                this.autoCloseSuspendCount--;
            }

            if (this.autoCloseSuspendCount == 0)
            {
                this.autoCloseTimer.Start();
            }
        }

        #endregion

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType.Equals(typeof(RadButtonElement)))
            {
                return true;
            }

            if (elementType.Equals(typeof(RadDropDownButtonElement)))
            {
                return true;
            }

            if (elementType.Equals(typeof(RadToggleButtonElement)))
            {
                return true;
            }
            return base.ControlDefinesThemeForElement(element);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.alertElement = new RadDesktopAlertElement();
            this.alertElement.CaptionElement.TextAndButtonsElement.CloseButton.Click += this.OnAlertCloseButton_Click;
            this.alertElement.CaptionElement.TextAndButtonsElement.PinButton.ToggleStateChanged += this.PinButton_ToggleStateChanged;
            this.alertElement.ThemeRole = "DesktopAlertElement";
            parent.Children.Add(this.alertElement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.autoCloseTimer != null)
                {
                    this.autoCloseTimer.Tick -= this.OnAutoCloseTimer_Tick;
                    this.autoCloseTimer.Dispose();
                    this.autoCloseTimer = null;
                }

                this.alertElement.CaptionElement.TextAndButtonsElement.CloseButton.Click -= this.OnAlertCloseButton_Click;
                this.alertElement.CaptionElement.TextAndButtonsElement.PinButton.ToggleStateChanged -= this.PinButton_ToggleStateChanged;
            }

            base.Dispose(disposing);
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.canMove)
            {
                base.OnMouseMove(e);
                return;
            }

            if (Control.MouseButtons == MouseButtons.Left)
            {
                bool canMove = this.alertElement.CaptionElement.CaptionGrip.ControlBoundingRectangle.Contains(this.oldMousePosition);
                if (canMove)
                {
                    Point locationToSet = Point.Subtract(Control.MousePosition, new Size(oldMousePosition));
                    if (!this.locationModifiedByUser && locationToSet != this.Location)
                    {
                        this.locationModifiedByUser = true;
                        DesktopAlertManager.Instance.UpdateAlertsOrder();
                    }
                    this.Location = locationToSet;
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            oldMousePosition = e.Location;
            base.OnMouseDown(e);
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            return false;
        }

        public override bool OnKeyDown(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                case Keys.Escape:
                case Keys.Back:
                    {
                        return false;
                    }
            }

            return base.OnKeyDown(keyData);
        }

        #endregion

        #region Event handling

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            base.OnNotifyPropertyChanged(propertyName);

            if (propertyName == "IsPinned")
            {
                this.alertElement.CaptionElement.TextAndButtonsElement.PinButton.ToggleState = 
                    this.isPinned ? ToggleState.On : ToggleState.Off;
            }
        }

        private void PinButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                this.isPinned = true;
                this.SuspendAutoClose();
            }
            else
            {
                this.isPinned = false;
                this.ResumeAutoClose();
            }
        }

        protected override void OnPopupOpening(CancelEventArgs args)
        {
            base.OnPopupOpening(args);
            this.autoCloseSuspendCount = 0;
            this.EvaluateAutoCloseParameters();
        }

        private void OnAutoCloseTimer_Tick(object sender, EventArgs e)
        {
            this.autoCloseTimer.Stop();

            if (!this.isPinned)
            {
                this.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            //If the popup is still registered in the PopupManager
            //we will change its opacity on mouse enter. Otherwise,
            //no changes are performed. Scenario: the popup has a
            //fade-out animation defined. When the popup closes,
            //it will still be visible until it fades out - in this case
            //we do not need to set its opacity.
            if (PopupManager.Default.ContainsPopup(this))
            {
                this.SuspendAutoClose();
                this.oldOpacity = this.Opacity;
                this.Opacity = 1.0f;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (PopupManager.Default.ContainsPopup(this))
            {
                this.ResumeAutoClose();

                if (this.oldOpacity.HasValue)
                {
                    this.Opacity = this.oldOpacity.Value;
                    this.oldOpacity = null;
                }
            }
            base.OnMouseLeave(e);
        }

        private void OnAlertCloseButton_Click(object sender, EventArgs e)
        {
            this.ClosePopup(RadPopupCloseReason.CloseCalled);
        }

        #endregion
    }
}
