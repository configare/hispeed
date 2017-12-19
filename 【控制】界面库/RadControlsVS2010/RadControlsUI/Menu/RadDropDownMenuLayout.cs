using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadDropDownMenuLayout : StackLayoutPanel
    {
        private float leftColumnWidth;
        private float rightColumnWidth;
        private float leftColumnMaxPadding;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Orientation = Orientation.Vertical;
            this.EqualChildrenWidth = true;
            this.UseNewLayoutSystem = true;
        }

        #region Properties

        public static RadProperty LeftColumnMinWidthProperty = RadProperty.Register(
            "LeftColumnMinWidth", typeof(int), typeof(RadDropDownMenuLayout), new RadElementPropertyMetadata(
            21, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        /// <summary>
        /// Gets or sets the left column minimal width.
        /// </summary>
        [DefaultValue(false),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the left column minimal width.")]
        public virtual int LeftColumnMinWidth
        {
            get
            {
                return (int)GetValue(LeftColumnMinWidthProperty);
            }
            set
            {
                SetValue(LeftColumnMinWidthProperty, value);
            }
        }

        public virtual float LeftColumnWidth
        {
            get
            {
                return this.leftColumnWidth;
            }
        }

        public virtual float RightColumnWidth
        {
            get
            {
                return this.rightColumnWidth;
            }
        }

        public virtual float LeftColumnMaxPadding
        {
            get
            {
                return leftColumnMaxPadding;
            }
        }

        #endregion

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.ElementTree == null)
            {
                return base.MeasureOverride(availableSize);
            }

            RadDropDownMenu menu = this.ElementTree.Control as RadDropDownMenu;
            RadApplicationMenuDropDown appMenu = null;
            if (menu != null && menu.OwnerElement != null && menu.OwnerElement.IsInValidState(true))
            {
                appMenu = menu.OwnerElement.ElementTree.Control as RadApplicationMenuDropDown;
            }

            if (appMenu != null)
            {
                availableSize.Width = appMenu.RightColumnWidth - 5;
            }
            
            foreach (RadElement element in this.Children)
            {
                RadMenuItem menuItem = element as RadMenuItem;
                if (menuItem != null)
                {
                    menuItem.Measure(availableSize);
                    leftColumnWidth = Math.Max(leftColumnWidth, menuItem.LeftColumnElement.DesiredSize.Width);
                    rightColumnWidth = Math.Max(rightColumnWidth, menuItem.RightColumnElement.DesiredSize.Width);
                    leftColumnMaxPadding = Math.Max(leftColumnMaxPadding, menuItem.Padding.Left + menuItem.BorderThickness.Left + menuItem.Margin.Left);
                }
            }
            leftColumnWidth = Math.Max(leftColumnWidth, LeftColumnMinWidth);

            SizeF preferredSize = base.MeasureOverride(availableSize);
            preferredSize.Width += leftColumnWidth;
            preferredSize.Width += this.Padding.Horizontal;
            preferredSize.Height += this.Padding.Vertical;

            if (appMenu != null)
            {
                preferredSize.Width = appMenu.RightColumnWidth - 5;
            }

            return preferredSize;
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            base.OnChildrenChanged(child, changeOperation);

            RadDropDownMenuElement menuElement = this.FindAncestor<RadDropDownMenuElement>();
            if (menuElement == null)
            {
                return;
            }

            if (changeOperation == ItemsChangeOperation.Inserted || changeOperation == ItemsChangeOperation.Set)
            {
                DropDownPosition position = (DropDownPosition)menuElement.GetValue(RadDropDownMenuElement.DropDownPositionProperty);
                foreach (RadElement element in child.ChildrenHierarchy)
                {
                    element.SetValue(RadDropDownMenuElement.DropDownPositionProperty, position);
                }
            }

            //This is needed after adding the scrolling support in RadDropDownMenuElement.
            //since when adding items the scroll layout panel does not invalidate its parent's layout
            //and thus wrong size of the RadDropDownMenu is calculated, we need to invalidate the
            //main element's layout explicitly to force recalculation of the popup's size.
            //This behavior is needed only when adding items in design mode.
            menuElement.InvalidateMeasure();
        }
    }
}
