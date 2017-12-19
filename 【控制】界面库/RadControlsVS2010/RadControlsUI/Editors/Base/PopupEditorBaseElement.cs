using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public abstract class PopupEditorBaseElement : EditorBaseElement //EditorBase
    {
        #region BitState Keys

        internal const ulong ReadOnlyStateKey = EditorBaseElementLastStateKey << 1;
        internal const ulong PopupEditorBaseElementLastStateKey = ReadOnlyStateKey;

        #endregion

        // Fields
        private RadPopupControlBase popupForm = null;
        private int ownerOffset = 0;
        internal static PopupManager manager = PopupManager.Default;

        #region Constructors & Dispose

        static PopupEditorBaseElement()
        {
            new Themes.ControlDefault.ComboBox().DeserializeTheme();
        }

        protected override void DisposeManagedResources()
        {
            this.ClosePopup(RadPopupCloseReason.CloseCalled);
            this.DisposePopupForm();

            base.DisposeManagedResources();
        }

        protected void DisposePopupForm()
        {
            this.DisposePopupFormCore(true);
        }

        protected virtual void DisposePopupFormCore(bool dispose)
        {
            if (this.popupForm != null)
            {
                this.UnwirePopupFormEvents(this.PopupForm);
                this.popupForm.Dispose();
                this.popupForm = null;
            }
        }

        #endregion

        #region Properties

        public int OwnerOffset
        {
            get 
            {
                return this.ownerOffset; 
            }
            set 
            {
				if (this.ownerOffset != value)
				{
					this.ownerOffset = value;
					this.OnNotifyPropertyChanged("OwnerOffset");
				}
            }
        }

        protected internal virtual RadPopupControlBase PopupForm
        {
            get
            {
                return this.GetPopupForm();
            }
        }

        [Browsable(false)]
        public virtual bool IsPopupOpen
        {
            get
            {
                return this.popupForm != null &&
                   PopupManager.Default.ContainsPopup(this.popupForm);
            }
        } 

        [Browsable(false)]
        public bool EditorContainsFocus
        {
            get
            {
                if (this.ElementTree == null)
                {
                    return false;
                }

                return this.ElementTree.Control.ContainsFocus || (this.IsPopupOpen && PopupForm.ContainsFocus);
            }
        }

        /// <summary>
        /// Enables or disables the ReadOnly mode of RadComboBox. The default value is false.
        /// </summary>
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property is not used anymore and will be removed in a future release.")]
        [Browsable(false)]
        public virtual bool ReadOnly
        {
            get
            {
                return this.GetBitState(ReadOnlyStateKey);
            }
            set
            {
                this.SetBitState(ReadOnlyStateKey, value);
            }
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == ReadOnlyStateKey)
            {
                this.OnNotifyPropertyChanged("ReadOnly");
            }
        }


        #endregion

        #region Create & Open/Close
        /// <summary>
        /// Closes the popup if it is open, or shows the popup if it is closed.
        /// </summary>
        protected virtual void TooglePopupState()
        {
            if (this.IsPopupOpen)
            {
                this.ClosePopup();
            }
            else
            {
                this.ShowPopup();
            }
        }

        /// <summary>
        /// Closes the popup with a RadPopupCloseReason.CloseCalled reason.
        /// </summary>
        public void ClosePopup()
        {
            ClosePopup(RadPopupCloseReason.CloseCalled);
        }

        /// <summary>
        /// Closes the popup with the provided reason for closing.
        /// </summary>
        /// <param name="reason">the reason for the close operation as specified through RadPopupCloseReason enumeration.</param>
        protected internal virtual void ClosePopup(RadPopupCloseReason reason)
        {
            if (reason != RadPopupCloseReason.CloseCalled)
            {
                CancelEventArgs e = new CancelEventArgs();
                this.OnQueryValue(e);
                if (e.Cancel)
                {
                    return;
                }
            }
            if (this.popupForm != null)
            {
                this.popupForm.ClosePopup(reason);
            }
        }

        /// <summary>
        /// Displays the popup on the screen.
        /// </summary>
        public virtual void ShowPopup()
        {
            if (!this.CanDisplayPopup())
            {
                return;
            }

            RadPopupControlBase popup = this.PopupForm;
            if (popup == null)
            {
                return;
            }


           

            //explicitly load the element tree as the layout is needed in order to perform size calculations
            if (popup.ElementTree.RootElement.ElementState != ElementState.Loaded)
            {
                Size size = this.GetInitialPopupSize();
                this.PopupForm.LoadElementTree(size);
            }

           //explicitly syncronize the theme with our RadControl host

            this.UpdatePopupMinMaxSize(popup);
            this.ApplyThemeToPopup(this.PopupForm);

            //update popup's bounds on the screen
            Point location = this.GetPopupLocation(popup);

            popup.Size = this.GetPopupSize(popup, popup.RootElement.GetBitState(MeasureDirtyStateKey) || popup.RootElement.GetBitState(NeverMeasuredStateKey));
            popup.HorizontalPopupAlignment = HorizontalPopupAlignment.LeftToLeft;
            popup.VerticalPopupAlignment = VerticalPopupAlignment.TopToBottom;
            
            if (this.RightToLeft)
            {
                //The right edge of the popup form is aligned to the right
                //edge of the Combo Box control.
                popupForm.HorizontalPopupAlignment = HorizontalPopupAlignment.RightToRight;
            }
            this.ShowPopupCore(popup);
            popup.ShowPopup(new Rectangle(location, this.ControlBoundingRectangle.Size));
        }

        /// <summary>
        /// Used to initialize the size of the popup
        /// when it is initially opened and the
        /// element tree is loaded.
        /// </summary>
        protected virtual Size GetInitialPopupSize()
        {
            return Size.Empty;
        }

        /// <summary>
        /// Performs the core popup display logic.
        /// </summary>
        /// <param name="popup">The popup form that is about to be displayed.</param>
        protected virtual void ShowPopupCore(RadPopupControlBase popup)
        {
        }

        /// <summary>
        /// Gets the screen coordinated where the popup should be displayed.
        /// </summary>
        /// <param name="popup"></param>
        /// <returns></returns>
        protected virtual Point GetPopupLocation(RadPopupControlBase popup)
        {
            Point location = 
                this.ElementTree.Control.PointToScreen(this.ControlBoundingRectangle.Location);

            return location;
        }

        /// <summary>
        /// Gets the display size for the popup.
        /// </summary>
        /// <param name="popup">The popup which size should beretrieved.</param>
        /// <param name="measure">True to perform explicit measure, false otherwise.</param>
        /// <returns></returns>
        protected virtual Size GetPopupSize(RadPopupControlBase popup, bool measure)
        {
            return Size.Empty;
        }

        /// <summary>
        /// Applies any Min/Max size restrictions to the popup form.
        /// </summary>
        /// <param name="popup"></param>
        protected virtual void UpdatePopupMinMaxSize(RadPopupControlBase popup)
        {
        }

        /// <summary>
        /// Syncronizes the theme of the editor itself with the popup that is about to be displayed.
        /// </summary>
        /// <param name="popup"></param>
        protected virtual void ApplyThemeToPopup(RadPopupControlBase popup)
        {
            string popupThemeName = "ControlDefault";
            if (this.ElementTree != null && this.ElementTree.ComponentTreeHandler != null &&
                !string.IsNullOrEmpty(this.ElementTree.ComponentTreeHandler.ThemeName))
            {
                popupThemeName = this.ElementTree.ComponentTreeHandler.ThemeName;
            }

            if (popup.ThemeName != popupThemeName)
            {
                popup.ThemeName = popupThemeName;
                //force layout update if the popup's root element is already loaded
                if (popup.RootElement.ElementState == ElementState.Loaded)
                {
                    popup.RootElement.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Determines whether the popup form may be displayed.
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanDisplayPopup()
        {
            if (this.IsPopupOpen)
            {
                return false;
            }

            return this.ElementState == ElementState.Loaded;
        }

        /// <summary>
        /// Creates the popup instance. You have to override this method in order to provide a popup 
        /// that is specialized by its content. Example: In a combo box you have to override and provide a specialized class
        /// that contains and handles the listbox element.
        /// </summary>
        /// <returns>The popup instance.</returns>
        protected virtual RadPopupControlBase CreatePopupForm()
        {
            RadEditorPopupControlBase popup = new RadEditorPopupControlBase(this);
            popup.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
            this.WirePopupFormEvents(popup);           
            return popup;
        }

        /// <summary>
        /// Gets a valid instance of the popup, that is properly
        /// initialized to work with the PopupEditorBaseElement.
        /// </summary>
        /// <returns>The popup instance.</returns>
        internal protected virtual RadPopupControlBase GetPopupForm()
        {
            if (this.popupForm == null)
            {
                this.popupForm = this.CreatePopupForm();

                this.popupForm.PopupOpening += new RadPopupOpeningEventHandler(OnPopupForm_Opening);
                this.popupForm.PopupOpened += new RadPopupOpenedEventHandler(OnPopupForm_Opened);
                this.popupForm.PopupClosing += new RadPopupClosingEventHandler(OnPopupForm_Closing);
                this.popupForm.PopupClosed += new RadPopupClosedEventHandler(OnPopupForm_Closed);
            }

            return this.popupForm;
        }

        #endregion

        //protected void OnEditorKeyPress(KeyPressEventArgs e)
        //{
        //    //if (!e.Handled && this.IsPopupOpen)
        //    //{
        //    //    PopupForm.ProcessKeyPress(e);
        //    //}
        //}

        //protected void OnEditorKeyUp(KeyEventArgs e)
        //{
        //    //if (!e.Handled && this.IsPopupOpen)
        //    //{
        //    //    PopupForm.ProcessKeyUp(e);
        //    //}
        //}

        protected void OnEditorKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.KeyData == (Keys.Down | Keys.Alt))
            {
                if (this.IsPopupOpen)
                {
                    this.ClosePopup(RadPopupCloseReason.Keyboard);
                }
                else
                {
                    this.ShowPopup();
                }
                e.Handled = true;
            }

            //if (!e.Handled)
            //{
            //    //if (this.IsPopupOpen)
            //    //{
            //    //    PopupForm.ProcessKeyDown(e);
            //    //}
            //}
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
            }
            base.OnMouseDown(e);
        }

        protected internal virtual void ProcessPopupTabKey(KeyEventArgs e)
        {
            this.ClosePopup(RadPopupCloseReason.Keyboard);
            if (this.IsPopupOpen)
            {
                return;
            }
            base.ProcessDialogKey(e.KeyData);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadObject.BindingContextProperty)
            {
                this.PopupForm.BindingContext = this.BindingContext;
            }
        }

        #region Events

        #region Popup events


        private void OnPopupForm_Closed(object sender, RadPopupClosedEventArgs args)
        {
            this.OnPopupClosed(args);
        }

        private void OnPopupForm_Closing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnPopupClosing(args);
        }

        private void OnPopupForm_Opened(object sender, EventArgs args)
        {
            this.OnPopupOpened(args);
        }

        private void OnPopupForm_Opening(object sender, CancelEventArgs args)
        {
            this.OnPopupOpening(args);
        } 

        protected virtual void WirePopupFormEvents(RadPopupControlBase popup)
        {
        }

        protected virtual void UnwirePopupFormEvents(RadPopupControlBase popup)
        {
        }

        #endregion

        protected virtual void OnPopupOpening(CancelEventArgs e)
        {
            if (PopupOpening != null)
            {
                PopupOpening(this, e);
            }
        }

        protected virtual void OnPopupOpened(EventArgs args)
        {
            if (PopupOpened != null)
            {
                PopupOpened(this, args);
            }
        }

        protected virtual void OnPopupClosing(RadPopupClosingEventArgs e)
        {
            if (PopupClosing != null)
            {
                PopupClosing(this, e);
            }
        }

        protected virtual void OnPopupClosed(RadPopupClosedEventArgs e)
        {
            if (PopupClosed != null)
            {
                PopupClosed(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler PopupOpened;

        /// <summary>
        /// 
        /// </summary>
        public event CancelEventHandler PopupOpening;

        /// <summary>
        /// 
        /// </summary>
        public event RadPopupClosingEventHandler PopupClosing;

        /// <summary>
        /// 
        /// </summary>
        public event RadPopupClosedEventHandler PopupClosed; 

        #endregion
    }
}