using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class RadDateTimePickerDropDown : RadEditorPopupControlBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when the drop down is opened
        /// </summary>
        [Description("Occurs when the drop down is opened")]
        [Category("Action")]
        public event EventHandler Opened;

        /// <summary>
        /// Occurs when the drop down is opening
        /// </summary>
        [Description("Occurs when the drop down is opening")]
        [Category("Action")]
        public event CancelEventHandler Opening;


        /// <summary>
        /// Occurs when the drop down is closing
        /// </summary>
        [Description("Occurs when the drop down is closing")]
        [Category("Action")]
        public event RadPopupClosingEventHandler Closing;

        /// <summary>
        /// Occurs when the drop down is closed
        /// </summary>
        [Description("Occurs when the drop down is closed")]
        [Category("Action")]
        public event RadPopupClosedEventHandler Closed;

        public RadDateTimePickerDropDown(RadItem ownerElement)
            : base(ownerElement)
        {
            this.PopupClosed += new RadPopupClosedEventHandler(RadDateTimePickerDropDown_PopupClosed);
            this.PopupClosing += new RadPopupClosingEventHandler(RadDateTimePickerDropDown_PopupClosing);
            this.PopupOpened += new RadPopupOpenedEventHandler(RadDateTimePickerDropDown_PopupOpened);
            this.PopupOpening += new RadPopupOpeningEventHandler(RadDateTimePickerDropDown_PopupOpening);
        }



        private RadControl hostedControl;

        /// <summary>
        /// Gets or sets the hosted control in the popup.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or sets the hosted control in the popup.")]
        [Category("Behavior")]
        public RadControl HostedControl
        {
            get
            {
                return this.hostedControl;
            }

            set
            {
                if (this.hostedControl != value)
                {
                    this.hostedControl = value;

                    if (this.hostedControl == null)
                    {
                        this.Controls.Clear();
                    }
                    else
                    {

                        this.Controls.Add(value);
                    }


                    this.OnNotifyPropertyChanged(new PropertyChangedEventArgs("HostedControl"));
                }
            }
        }

        internal bool calendarLostFocus;

        /// <summary>
        /// Gets the resizing grip element
        /// </summary>
        [Description("Gets the resizing grip element")]
        [Browsable(false)]
        [Category("Behavior")]
        [Obsolete("This property is obsolete. Please, use the SizingGrip property instead.")]
        public RadElement ResizeGrip
        {
            get
            {
                return this.SizingGrip;
            }
        }

        /// <summary>
        /// The owner control of the popup
        /// </summary>
        [Description("The owner control of the popup")]
        public RadControl OwnerControl;

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.SizingGripDockLayout.LastChildFill = false;
        }

        /// <summary>
        /// Shows the popup control with a specified popup direction and offset by the owner
        /// </summary>
        /// <param name="popupDirection"></param>
        /// <param name="ownerOffset"></param>
        public Point ShowControl(RadDirection popupDirection, int ownerOffset)
        {
            bool corected = false;
            this.DropDownAnimationDirection = popupDirection;
            if (this.OwnerElement as RadDateTimePickerElement != null)
            {
                this.SizingGrip.MinSize = new Size(10, 10);
                this.SizingGrip.Visibility = ElementVisibility.Visible;
                this.SizingGrip.GripItemNSEW.Alignment = ContentAlignment.BottomRight;
            }

            if (this.SizingGrip != null)
            {
                if (corected)
                {
                    DockLayoutPanel.SetDock(this.SizingGrip, Telerik.WinControls.Layouts.Dock.Top);
                }
                else
                {

                    DockLayoutPanel.SetDock(this.SizingGrip, Telerik.WinControls.Layouts.Dock.Bottom);
                }
            }

            if (!(this.OwnerElement as RadDateTimePickerElement).RightToLeft)
            {
                this.SizingGrip.RightToLeft = false;
            }
            else
            {
                this.SizingGrip.RightToLeft = true;
            }

            this.AutoUpdateBounds();
            Rectangle screenRect = this.OwnerControl.RectangleToScreen(this.OwnerElement.ControlBoundingRectangle);
            this.ShowPopup(screenRect);

            return this.Location;
        }

        /// <summary>
        /// Hides the popup
        /// </summary>
        public void HideControl()
        {
            this.ClosePopup(RadPopupCloseReason.CloseCalled);
        }


        #region Animation

        private int backupHeight = 0;
        private int backupWidth = 0;
        private Rectangle backupBounds = Rectangle.Empty;

        private Size minimum = Size.Empty;
        private Size maximum = Size.Empty;

        /// <summary>
        /// Get/Set minimum value allowed for size
        /// </summary>	
        internal Size Minimum
        {
            get
            {
                return minimum;
            }

            set
            {
                minimum = value;
            }
        }

        /// <summary>
        /// Get/Set maximum value allowed for size
        /// </summary>
        internal Size Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        protected void AutoUpdateBounds()
        {
            this.minimum = new Size(this.Width, 0);
            this.maximum = this.Size;
        }

        protected virtual void BackupBounds()
        {
            backupBounds = this.Bounds;
            if (backupHeight == 0)
            {
                backupHeight = this.Height;
            }
            if (backupWidth == 0)
            {
                backupWidth = this.Width;
            }
        }

        #endregion

        #region Event handling

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (this.SizingGrip != null)
            {
                this.SizingGrip.RightToLeft = this.RightToLeft == RightToLeft.Yes ? true : false;
            }
        }

        private Size lastSize;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.HostedControl != null)
            {
                int height = this.SizingGrip.Visibility == ElementVisibility.Visible ? (int)this.SizingGrip.DesiredSize.Height : 0;

                this.HostedControl.Size = new Size(this.Size.Width, this.Size.Height - height);

                this.lastSize = this.Size;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.PopupClosed -= new RadPopupClosedEventHandler(RadDateTimePickerDropDown_PopupClosed);
            this.PopupClosing -= new RadPopupClosingEventHandler(RadDateTimePickerDropDown_PopupClosing);
            this.PopupOpened -= new RadPopupOpenedEventHandler(RadDateTimePickerDropDown_PopupOpened);
            this.PopupOpening -= new RadPopupOpeningEventHandler(RadDateTimePickerDropDown_PopupOpening);
        }

        private void RadDateTimePickerDropDown_PopupOpening(object sender, CancelEventArgs args)
        {
            if (this.Opening != null)
            {
                this.Opening(sender, args);
            }
        }

        private void RadDateTimePickerDropDown_PopupOpened(object sender, EventArgs args)
        {
            if (this.Opened != null)
            {
                this.Opened(sender, args);
            }
        }

        private void RadDateTimePickerDropDown_PopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            if (this.Closing != null)
            {
                this.Closing(sender, args);
            }
        }

        private void RadDateTimePickerDropDown_PopupClosed(object sender, RadPopupClosedEventArgs args)
        {
            if (this.Closed != null)
            {
                this.Closed(sender, args);
            }
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            if (reason == RadPopupCloseReason.Mouse)
            {
                if (!this.OwnerElement.IsInValidState(true) || !(this.OwnerElement is RadDateTimePickerElement))
                {
                    return base.CanClosePopup(reason);
                }

                RadDateTimePickerElement dateTimePickerElement = this.OwnerElement as RadDateTimePickerElement;

                if (dateTimePickerElement.ArrowButton == null)
                    return base.CanClosePopup(reason);

                return !dateTimePickerElement.ArrowButton.IsMouseOver;
            }

            return base.CanClosePopup(reason);
        }

        #endregion
    }




}
