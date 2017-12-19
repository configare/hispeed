using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.IO;
using System.Media;
using System.ComponentModel.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a Desktop Alert component which can be used to
    /// display a small window on the screen to notify the user that an
    /// event occurred. The location of the window and the way it appears
    /// can be customized.
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    [Description("This class represents a Desktop Alert component which can be used to" + 
            " display a small window on the screen to notify the user that an " + 
            "event occurred. The location of the window, the way it appears," +
            "as well as its content can be customized.")]
    [Designer(DesignerConsts.RadDesktopAlertDesignerString)]
    public class RadDesktopAlert : RadComponent
    {
        #region Fields

        private DesktopAlertPopup popup;
        internal readonly static Size DefaultSize = new Size(329, 95);
        private Size fixedSize = Size.Empty;
        private AlertScreenPosition screenPosition = AlertScreenPosition.BottomRight;
        private bool playSound = false;
        private SystemSound soundToPlay;

        #endregion

        #region Events

        private static readonly object OpeningEventKey = new object();
        private static readonly object OpenedEventKey = new object();
        private static readonly object ClosingEventKey = new object();
        private static readonly object ClosedEventKey = new object();

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="RadDesktopAlert"/> class.
        /// </summary>
        public RadDesktopAlert()
        {
            this.popup = this.CreatePopup();
            this.WireEvents();
        }

        /// <summary>
        /// Creates an instance of the <see cref="RadDesktopAlert"/> class.
        /// </summary>
        /// <param name="container">An implementation of the <see cref="IContainer"/> interface
        /// that holds this instance.</param>
        public RadDesktopAlert(IContainer container)
            : this()
        {
            container.Add(this);
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether control's elements are aligned
        /// to support locales using right-to-left fonts.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Forms.RightToLeft"/> values.
        /// The default is <see cref="F:System.Windows.Forms.RightToLeft.Inherit"/>.</returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The assigned
        /// value is not one of the <see cref="T:System.Windows.Forms.RightToLeft"/> values.
        /// </exception>
        [System.ComponentModel.LocalizableAttribute(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts.")]
        [System.ComponentModel.AmbientValueAttribute(0)]
        public virtual System.Windows.Forms.RightToLeft RightToLeft
        {
            get 
            {
                return this.popup.RightToLeft; 
            }
            set 
            {
                this.popup.RightToLeft = value; 
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether a sound is played
        /// when the alert's popup is shown.
        /// </summary>
        //[Description("Gets or sets a boolean value determining whether a sound is played when the alert's popup is shown.")]
        //[DefaultValue(false)]
        //[Category(RadDesignCategory.BehaviorCategory)]
        [Browsable(false)]
        public bool PlaySound
        {
            get
            {
                return this.playSound;
            }
            set
            {
                if (this.playSound != value)
                {
                    this.playSound = value;
                    this.OnNotifyPropertyChanged("PlaySound");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sound which is played when the alert's popup is shown
        /// and the PlaySound property is set to true.
        /// </summary>
        //[Description("Gets or sets the sound played when the alert's popup is shown.")]
        //[DefaultValue(typeof(SystemSounds), "Exclamation")]
        //[Category(RadDesignCategory.BehaviorCategory)]
        //[Editor(typeof(SoundToPlayEditor), typeof(UITypeEditor))]
        [Browsable(false)]
        public SystemSound SoundToPlay
        {
            get
            {
                return this.soundToPlay;
            }
            set
            {
                if (this.soundToPlay != value)
                {
                    this.soundToPlay = value;
                    this.OnNotifyPropertyChanged("SoundToPlay");
                }
            }
        }

        /// <summary>
        /// Gets or sets the initial opacity of the alert's popup.
        /// </summary>
        [DefaultValue(0.8f)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the initial opacity of the alert's popup.")]
        public float Opacity
        {
            get
            {
                return this.popup.Opacity;
            }
            set
            {
                this.popup.Opacity = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the options button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the options button is shown.")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowOptionsButton
        {
            get
            {
                return this.popup.ShowOptionsButton;
            }
            set
            {
                this.popup.ShowOptionsButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the pin button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the pin button is shown.")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowPinButton
        {
            get
            {
                return this.popup.ShowPinButton;
            }
            set
            {
                this.popup.ShowPinButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the close button is shown.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the close button is shown.")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public bool ShowCloseButton
        {
            get
            {
                return this.popup.ShowCloseButton;
            }
            set
            {
                this.popup.ShowCloseButton = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the alert's
        /// popup will be pinned on the screen. If pinned, the alert's popup
        /// will not be automatically closed upon mouse click outside its bounds
        /// or if the AutoClose property is set to true.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the alert's "
            +"popup will be pinned on the screen. If pinned, the alert's popup "
            +"will not be automatically closed upon mouse click outside its bounds "+
            "or if the AutoClose property is set to true.")]
        [DefaultValue(false)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool IsPinned
        {
            get
            {
                return this.popup.IsPinned;
            }
            set
            {
                this.popup.IsPinned = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the popup
        /// can be moved by dragging the caption grip.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the popup can be moved by dragging the caption grip.")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool CanMove
        {
            get
            {
                return this.popup.CanMove;
            }
            set
            {
                this.popup.CanMove = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value determining whether the alert's popup
        /// will be animated.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets a boolean value determining whether the alert's popup will be animated.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool PopupAnimation
        {
            get
            {
                return this.popup.AnimationEnabled;
            }
            set
            {
                this.popup.AnimationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value determining the direction of the alert's popup animation.
        /// </summary>
        [DefaultValue(typeof(RadDirection), "Down")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value determining the direction of the alert's popup animation.")]
        public RadDirection PopupAnimationDirection
        {
            get
            {
                return this.popup.DropDownAnimationDirection;
            }
            set
            {
                this.popup.DropDownAnimationDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of the alert's drop-down animation frames.
        /// </summary>
        [Description("Gets or sets the count of the alert's drop-down animation frames.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(50)]
        public int PopupAnimationFrames
        {
            get
            {
                return this.popup.AnimationFrames;
            }
            set
            {
                this.popup.AnimationFrames = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the drop-down animation easing.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the type of the drop-down animation easing.")]
        [DefaultValue(typeof(RadEasingType), "Default")]
        public RadEasingType PopupAnimationEasing
        {
            get
            {
                return this.popup.EasingType;
            }
            set
            {
                this.popup.EasingType = value;
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="FadeAnimationType"/>
        /// enumerator that determines the type of fade animation performed
        /// when the alert's popup is opened/closed.
        /// </summary>
        [DefaultValue(typeof(FadeAnimationType), "FadeIn, FadeOut")]
        [Description("Gets or sets a value from the FadeAnimationType enumerator that determines the type of fade animation performed when the alert's popup is opened/closed")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Editor(typeof(FadeAnimationTypeEditor), typeof(UITypeEditor))]
        public FadeAnimationType FadeAnimationType
        {
            get
            {
                return this.popup.FadeAnimationType;
            }
            set
            {
                this.popup.FadeAnimationType = value;
            }
        }

        /// <summary>
        /// Gets or sets the interval in milliseconds between two animation frames.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the interval in milliseconds between two animation frames.")]
        [DefaultValue(10)]
        public int FadeAnimationSpeed
        {
            get
            {
                return this.popup.FadeAnimationSpeed;
            }
            set
            {
                this.popup.FadeAnimationSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the count of animation frames for the fade animation.
        /// </summary>
        [Description("Gets or sets the count of animation frames for the fade animation.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(80)]
        public int FadeAnimationFrames
        {
            get
            {
                return this.popup.FadeAnimationFrames;
            }
            set
            {
                this.popup.FadeAnimationFrames = value;
            }
        }

        /// <summary>
        /// Gets a sets a boolean value determining whether the alert popup will be automatically 
        /// closed after a given amount of time.
        /// </summary>
        [Description("Gets or sets a boolean value determining whether the alert's popup is automatically closed.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        public bool AutoClose
        {
            get
            {
                return this.popup.AutoClose;
            }
            set
            {
                this.popup.AutoClose = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds after
        /// which the alert will be automatically closed.
        /// </summary>
        [Description("Gets or sets the amount of time in seconds after which"
            +"the alert will be automatically closed.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(10)]
        public int AutoCloseDelay
        {
            get
            {
                return this.popup.AutoCloseDelay;
            }
            set
            {
                this.popup.AutoCloseDelay = value;
            }
        }

        /// <summary>
        /// Gets or sets a value of the <see cref="AlertScreenPosition"/>
        /// enum which defines the position of the alert popup
        /// on the working area of the active screen.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(typeof(AlertScreenPosition), "BottomRight")]
        [Description("Gets or sets the position of the alert popup on the working area of the active screen.")]
        public virtual AlertScreenPosition ScreenPosition
        {
            get
            {
                return this.screenPosition;
            }
            set
            {
                if (this.screenPosition != value)
                {
                    this.screenPosition = value;
                    this.OnNotifyPropertyChanged("ScreenPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Size"/>struct
        /// which defines fixed size for the alert's popup. The default
        /// value is an empty size. In this case the popup adjusts its
        /// size according to its content. Otherwise the value of this property is
        /// considered.
        /// </summary>
        [DefaultValue(typeof(Size), "0, 0")]
        [Description("Gets or sets the fixed size for the alert's popup. If the value is Size.Empty, the size of the popup is dynamically adjusted according to its content.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public Size FixedSize
        {
            get
            {
                return this.fixedSize;
            }
            set
            {
                if (value.Height < 0 || value.Width < 0)
                {
                    throw new ArgumentException("The fixed size cannot have negative metrics.");
                }

                if (this.fixedSize != value)
                {
                    this.fixedSize = value;
                    this.OnNotifyPropertyChanged("FixedSize");
                }
            }
        }

        /// <summary>
        /// Gets or sets the content image of the <see cref="RadDesktopAlert"/>.
        /// </summary>
        [Description("Gets or sets the content image of the alert.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public Image ContentImage
        {
            get
            {
                return this.popup.Image;
            }
            set
            {
                this.popup.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed in the alert popup. This text
        /// can be additinally HTML formatted to achieve better appearance.
        /// </summary>
        [Description("Gets or sets the alert's content text. This text can be HTML formatted for better appearance.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue("")]
        [Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public string ContentText
        {
            get
            {
                return this.popup.ContentText;
            }
            set
            {
                this.popup.ContentText = value;
            }
        }

        /// <summary>
        /// Gets or sets the alert's caption text.
        /// The caption text is displayed below the moving grip of the alert's popup.
        /// </summary>
        [Description("Gets or sets the alert's caption text.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue("")]
        [Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public string CaptionText
        {
            get
            {
                return this.popup.CaptionText;
            }
            set
            {
                this.popup.CaptionText = value;
            }
        }

        /// <summary>
        /// Gets or sets the items collection containing the button items shown at the bottom
        /// part of the desktop alert's popup.
        /// </summary>
        [Description("Gets the collection that holds the button items added to the alert.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemCollection ButtonItems
        {
            get
            {
                return this.popup.ButtonItems;
            }
        }

        /// <summary>
        /// Gets the items collection containing the items added to the options drop-down button
        /// of the desktop alert's popup.
        /// </summary>
        [Description("Gets the collection that holds the option items added to the alert's options button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemCollection OptionItems
        {
            get
            {
                return this.popup.OptionItems;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="DesktopAlertPopup"/>class
        /// that represents the popup of the desktop alert.
        /// </summary>
        [Browsable(false)]
        public DesktopAlertPopup Popup
        {
            get
            {
               return this.popup;
            }
        }

        #endregion

        #region Methods

        private void WireEvents()
        {
            this.popup.PopupClosing += this.OnPopup_Closing;
            this.popup.PopupClosed += this.OnPopup_Closed;
            this.popup.PopupOpened += this.OnPopup_Opened;
            this.popup.VisibleChanged += this.OnPopup_VisibleChanged;
        }
        
        private void UnwireEvents()
        {
            this.popup.PopupClosing -= this.OnPopup_Closing;
            this.popup.PopupClosed -= this.OnPopup_Closed;
            this.popup.PopupOpened -= this.OnPopup_Opened;
            this.popup.VisibleChanged -= this.OnPopup_VisibleChanged;
        }

        /// <summary>
        /// Returns an instance of the <see cref="DesktopAlertPopup"/> class
        /// that represents the alert's popup
        /// </summary>
        protected virtual DesktopAlertPopup CreatePopup()
        {
            return new DesktopAlertPopup(this);
        }

        /// <summary>
        /// Displays the alert popup on the screen at the specified location.
        /// </summary>
        public void Show()
        {
            if (DesktopAlertManager.Instance.ContainsAlert(this))
            {
                return;
            }

            RadPopupOpeningEventArgs args = new RadPopupOpeningEventArgs(this.popup.Location);
            this.OnPopupOpening(this, args);

            if (args.Cancel)
            {
                return;
            }

            this.popup.Size = this.GetPopupSize();
            DesktopAlertManager.Instance.AddAlert(this);
            this.popup.Show(this.popup.Location);

            if (!PopupManager.Default.ContainsPopup(this.popup))
            {
                return;
            }

            if (this.playSound && this.soundToPlay != null)
            {
                this.soundToPlay.Play();
            }
        }

        #region Desktop Alert Manager related

        protected internal virtual void OnLocationChangeRequested(Point alertLocation)
        {
            if (!this.popup.locationModifiedByUser)
            {
                if (this.popup.Location != alertLocation)
                {
                    this.popup.Location = alertLocation;
                }
            }
        }

        protected internal virtual bool GetLocationModifiedByUser()
        {
            return this.popup.locationModifiedByUser;
        }

        /// <summary>
        /// Resets the explicit location modifier. In other words, if the user
        /// has modified the location of the alert's popup, the <see cref="DesktopAlertManager"/>
        /// will not consider it when rearranging the visible alerts. This method
        /// will reset the explicit location modifier and thus the <see cref="DesktopAlertManager"/>
        /// will continue managing the location of the alert according to its location settings.
        /// </summary>
        public void ResetLocationModifier()
        {
            this.popup.locationModifiedByUser = false;
        }

        #endregion

        /// <summary>
        /// Hides the alert popup from the screen.
        /// </summary>
        public void Hide()
        {
            this.popup.ClosePopup(RadPopupCloseReason.CloseCalled);
        }

        protected virtual Size GetPopupSize()
        {
            if (this.fixedSize != Size.Empty)
            {
                return this.fixedSize;
            }

            SizeF result;
            Size defaultSize = RadDesktopAlert.DefaultSize;
            if (!this.popup.IsLoaded)
            {
                this.popup.LoadElementTree(new Size(defaultSize.Width, defaultSize.Height));
                result = new SizeF(defaultSize.Width, this.popup.AlertElement.DesiredSize.Height);
            }
            else
            {
                result = MeasurementControl.ThreadInstance.GetDesiredSize(
                    this.popup.AlertElement,
                    new Size(defaultSize.Width, defaultSize.Height));
                result = new SizeF(defaultSize.Width, result.Height);
            }

            return result.ToSize();
        }

        public override IComponentTreeHandler GetOwnedTreeHandler()
        {
            return this.popup;
        }

        #endregion

        #region Events and event handling

        private void OnPopup_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.popup.Visible)
            {
                DesktopAlertManager.Instance.RemoveAlert(this);
            }
        }

        protected virtual void OnPopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            RadPopupClosingEventHandler handler = this.Events[ClosingEventKey] as RadPopupClosingEventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnPopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            RadPopupClosedEventHandler handler = this.Events[ClosedEventKey] as RadPopupClosedEventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnPopupOpening(object sender, RadPopupOpeningEventArgs args)
        {
            RadPopupOpeningEventHandler handler = this.Events[OpeningEventKey] as RadPopupOpeningEventHandler;
            if (handler != null)
            {
                handler(this, args);
            }

            if (args.Cancel)
            {
                DesktopAlertManager.Instance.RemoveAlert(this);
            }
        }

        protected virtual void OnPopupOpened(object sender, EventArgs args)
        {
            RadPopupOpenedEventHandler handler = this.Events[OpenedEventKey] as RadPopupOpenedEventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Fires when the alert's popup is about to be opened. The opening
        /// action can be canceled by modifying the arguments of this event.
        /// </summary>
        public event RadPopupOpeningEventHandler Opening
        {
            add
            {
                this.Events.AddHandler(OpeningEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(OpeningEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the alert's popup was opened.
        /// </summary>
        public event RadPopupOpenedEventHandler Opened
        {
            add
            {
                this.Events.AddHandler(OpenedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(OpenedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the alert's popup is about to be closed.
        /// The closing action can be canceled by modifying the
        /// arguments of this event..
        /// </summary>
        public event RadPopupClosingEventHandler Closing
        {
            add
            {
                this.Events.AddHandler(ClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ClosingEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the alert's popup was closed.
        /// </summary>
        public event RadPopupClosedEventHandler Closed
        {
            add
            {
                this.Events.AddHandler(ClosedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ClosedEventKey, value);
            }
        }

        private void OnPopup_Closing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnPopupClosing(sender, args);
        }

        private void OnPopup_Opened(object sender, EventArgs args)
        {
            this.OnPopupOpened(sender, args);
        }

        private void OnPopup_Closed(object sender, RadPopupClosedEventArgs args)
        {
            this.OnPopupClosed(sender, args);
        }

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            if (propertyName == "ThemeName")
            {
                this.popup.ThemeName = this.ThemeName;
            }

            base.OnNotifyPropertyChanged(propertyName);
        }

        #endregion

        #region Cleanup

        protected override void DisposeManagedResources()
        {
            //If the alert is disposed while shown, remove it from the alert manager.
            if (DesktopAlertManager.Instance.ContainsAlert(this))
            {
                DesktopAlertManager.Instance.RemoveAlert(this);
            }

            this.UnwireEvents();
            this.Hide();
            this.popup.Dispose();
            base.DisposeManagedResources();
        }

        #endregion
    }
}
