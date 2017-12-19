using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class DropDownPopupForm : RadEditorPopupControlBase
    {
        #region Fields
        private RadDropDownListElement ownerDropDownListElement;

        #endregion

        #region Cstor

        public DropDownPopupForm(RadDropDownListElement ownerDropDownListElement)
            : base(ownerDropDownListElement)
        {
            this.ownerDropDownListElement = ownerDropDownListElement;
            this.SizingGripDockLayout.Children.Add(this.ownerDropDownListElement.ListElement);
            this.SizingGrip.ShouldAspectRootElement = false;
            this.TabStop = false;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadDropDownPopupFormAccessibleObject(this);
        }

        #endregion

        #region Overrides

        protected override bool ProcessFocusRequested(RadElement element)
        {
            return false;
        }

        public override void ClosePopup(RadPopupCloseReason reason)
        {
            base.ClosePopup(reason);
            this.ownerDropDownListElement.ClosePopupCore();
        }

        public override bool OnMouseWheel(Control target, int delta)
        {
            if (!this.ownerDropDownListElement.EnableMouseWheel)
            {
                return true;
            }

            base.OnMouseWheel(target, delta);
            this.ownerDropDownListElement.ListElement.OnMouseWheel(delta);
            return true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (this.CanClosePopupCore(e.Button, RadPopupCloseReason.Mouse))
            {
                this.ClosePopup(RadPopupCloseReason.Mouse);
            }
        }

        public override bool CanClosePopup(RadPopupCloseReason reason)
        {
            MouseButtons mouseButtons = Control.MouseButtons;
            return this.CanClosePopupCore(mouseButtons, reason);
        }

        public override bool OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                RadDropDownListElement mainDropDown = this.GetMainDropDownListElement();
                Debug.Assert(mainDropDown != null);
                mainDropDown.ClosePopup(RadPopupCloseReason.Keyboard);

                if (this.ownerDropDownListElement.isSuggestMode)
                {
                    this.ownerDropDownListElement.ClosePopup(RadPopupCloseReason.Keyboard);
                }

                return false;
            }

            if (keyData == Keys.Back)
            {
                return false;
            }

            RadListElement listElement = this.OwnerDropDownListElement.ListElement;

            if (keyData == Keys.Home)
            {
                listElement.SelectedItem = this.OwnerDropDownListElement.Items.First;
            }

            if (keyData == Keys.End)
            {
                listElement.SelectedItem = this.OwnerDropDownListElement.Items.Last;
            }

            if (keyData == Keys.PageUp)
            {
                listElement.ScrollByPage(-1);
            }

            if (keyData == Keys.PageDown)
            {
                listElement.ScrollByPage(1);
            }

            if (keyData != Keys.Enter)
            {
                return base.OnKeyDown(keyData);
            }

            RadListDataItem hoveredItem = this.GetSelectedOrHoveredItem();
            if (hoveredItem != null)
            {
                int indexOfHoveredItem = hoveredItem.RowIndex;
                this.ownerDropDownListElement.SelectedIndex = indexOfHoveredItem;
                this.ownerDropDownListElement.ClosePopup(RadPopupCloseReason.Keyboard);
            }
            else
            {
                RadDropDownListElement owner = this.GetMainDropDownListElement();
                string text = owner.TextBox.Text;
                owner.SelectItemFromText(text);
                owner.ClosePopup(RadPopupCloseReason.Keyboard);
            }

            bool baseResult = base.OnKeyDown(keyData);
            return baseResult;
        }

        protected virtual bool CanClosePopupCore(MouseButtons mouseButtons, RadPopupCloseReason reason)
        {
            if (reason == RadPopupCloseReason.Mouse
                && this.ownerDropDownListElement != null
                && !this.ownerDropDownListElement.IsDisposed
                && !this.ownerDropDownListElement.IsDisposing
                && this.ownerDropDownListElement.ElementTree!=null
                && this.ownerDropDownListElement.ElementTree.Control!=null
                && this.ownerDropDownListElement.ListElement!=null
                && this.ownerDropDownListElement.ListElement.ElementTree!=null
                && this.ownerDropDownListElement.ListElement.ElementTree.Control != null
                && this.ownerDropDownListElement.ArrowButton!=null)
            {
                Point relativeMousePosition = this.ownerDropDownListElement.ElementTree.Control.PointToClient(Control.MousePosition);
                Point relativeMousePositionInPopup = this.ownerDropDownListElement.ListElement.ElementTree.Control.PointToClient(Control.MousePosition);
                Rectangle arrowButtonRectangle = this.ownerDropDownListElement.ArrowButton.ControlBoundingRectangle;

                if (this.ownerDropDownListElement.DropDownStyle == RadDropDownStyle.DropDownList
                    && (this.ownerDropDownListElement.ContainsMouse || this.ownerDropDownListElement.Bounds.Contains(relativeMousePosition)))
                {
                    return false;
                }

                if (mouseButtons != MouseButtons.Left)
                {
                    return true;
                }

                if (arrowButtonRectangle.Contains(relativeMousePosition))
                {
                    return false;
                }

                if (this.ownerDropDownListElement.ListElement.HScrollBar!=null && this.ownerDropDownListElement.ListElement.HScrollBar.ContainsMouse)
                {
                    return false;
                }

                if (this.ownerDropDownListElement.ListElement.VScrollBar!=null && this.ownerDropDownListElement.ListElement.VScrollBar.ContainsMouse)
                {
                    return false;
                }

                if (this.SizingGrip!=null && this.SizingGrip.ControlBoundingRectangle.Contains(relativeMousePositionInPopup))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Properties
        public RadDropDownListElement OwnerDropDownListElement
        {
            get
            {
                return ownerDropDownListElement;
            }
        }

        #endregion

        #region Helpers

        private RadListDataItem GetSelectedOrHoveredItem()
        {
            RadListDataItem hoveredItem = null;
            RadElementCollection items = this.ownerDropDownListElement.ListElement.ViewElement.Children;
            foreach (RadListVisualItem item in items)
            {
                if (item.Selected && item.Data.RowIndex != this.ownerDropDownListElement.selectedIndexOnPopupOpen)
                {
                    return item.Data;
                }

                if (item.IsMouseOver)
                {
                    hoveredItem = item.Data;
                }
            }

            return hoveredItem;
        }

        //Refactor this
        private RadDropDownListElement GetMainDropDownListElement() //get the non suggest drop down list 
        {
            if (!this.ownerDropDownListElement.isSuggestMode)
            {
                return this.ownerDropDownListElement;
            }

            RadDropDownListElement owner = this.ownerDropDownListElement.Parent as RadDropDownListElement;
            Debug.Assert(owner != null, "Cannot find the non suggest owner");
            return owner;
        }

        #endregion
    }
}
