using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.RibbonBar
{
    /// <summary>
    /// This class represents the popup which is displayed when a collapsed tab
    /// is selected in the RadRibbonBar control.
    /// </summary>
    [ToolboxItem(false)]
    public class RibbonBarPopup : RadPopupControlBase
    {
        #region Fields

        private bool isPopupShown = false;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the RibbonBarPopup class.
        /// </summary>
        /// <param name="ownerRibbon"></param>
        public RibbonBarPopup(RadRibbonBarElement ownerRibbon)
            : base(ownerRibbon)
        {
            this.FadeAnimationType = FadeAnimationType.FadeOut;
            this.DropShadow = true;
            this.FadeAnimationFrames = 30;
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadRibbonBar).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating
        /// whether the ribbon popup is shown.
        /// </summary>
        public bool IsPopupShown
        {
            get
            {
                return this.isPopupShown;
            }
        }

        /// <summary>
        /// Gets the owner RadRibbonBarElement.
        /// </summary>
        internal RadRibbonBarElement RibbonBarElement
        {
            get
            {
                return this.OwnerElement as RadRibbonBarElement;
            }
        }

        #endregion

        #region Methods

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadButtonElement) ||
                elementType == typeof(RadRibbonBarButtonGroup) ||
                elementType == typeof(RadRibbonBarElement) ||
                elementType == typeof(RadScrollViewer) ||
                elementType == typeof(RadCheckBoxElement) ||
                elementType == typeof(RadToggleButtonElement) ||
                elementType == typeof(RadDropDownButtonElement) ||
                elementType == typeof(RadRepeatButtonElement)
                )
                return true;

            if (elementType.Equals(typeof(RadTextBoxElement)))
            {
                if (element.FindAncestorByThemeEffectiveType(typeof(RadComboBoxElement)) != null)
                {
                    return true;
                }
            }
            else if (elementType.Equals(typeof(RadMaskedEditBoxElement)))
            {
                if (element.FindAncestor<RadDateTimePickerElement>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected override Point GetCorrectedLocation(Screen currentScreen, Rectangle alignmentRectangle)
        {
            return alignmentRectangle.Location;
        }

        public override void ShowPopup(System.Drawing.Rectangle alignmentRectangle)
        {
            base.ShowPopup(alignmentRectangle);
            this.isPopupShown = true;
        }

        public override void ClosePopup(RadPopupCloseReason reason)
        {
            base.ClosePopup(reason);
            this.isPopupShown = false;
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            return false;
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            RadRibbonBar ribbonBar = this.RibbonBarElement.ElementTree.Control as RadRibbonBar;

            Point pointInRibbon = ribbonBar.PointToClient(System.Windows.Forms.Cursor.Position);

            if (this.RibbonBarElement.TabStripElement.ItemContainer.ControlBoundingRectangle.Contains(pointInRibbon))
            {
                return false;
            }

            NativeMethods.POINTSTRUCT point = new NativeMethods.POINTSTRUCT();
            point.x = Cursor.Position.X;
            point.y = Cursor.Position.Y;
            IntPtr handle = NativeMethods._WindowFromPoint(point);

            Control controlUnderMouse = Control.FromHandle(handle);

            if (controlUnderMouse is ZoomPopup)
            {
                return false;
            }

            if (controlUnderMouse is RadDropDownMenu)
            {
                return false;
            }



            return base.CanClosePopup(reason);
        }

        /// <summary>
        /// Close the popup upon mouse click unless
        /// the user has clicked on a RadElement
        /// that opens another popup.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            RadElement elementUnderMouse = this.RootElement.ElementTree.GetElementAtPoint(e.Location);

            if (elementUnderMouse != null)
            {
                if (elementUnderMouse.FindAncestor<PopupEditorBaseElement>() == null
                    && elementUnderMouse.FindAncestor<RadGalleryElement>() == null
                    && !(elementUnderMouse is RadGalleryElement)
                    && elementUnderMouse.FindAncestor<RadDropDownButtonElement>() == null
                    && !(elementUnderMouse is RadDropDownButtonElement)
                    && !(elementUnderMouse is ActionButtonElement))
                {
                    this.ClosePopup(RadPopupCloseReason.Mouse);
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            IntPtr hRgn = NativeMethods.CreateRoundRectRgn(
                0,
                0,
                this.Bounds.Right - this.Bounds.Left + 1,
                this.Bounds.Bottom - this.Bounds.Top + 1, 4, 4);

            this.Region = Region.FromHrgn(hRgn);

        }

        #endregion
    }
}

