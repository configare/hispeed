using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a menu item from drop down menu opened by the <see cref="RadCommandBarOverflowButton"/>.
    /// Has a coresponding item from the <see cref="CommandBarStripElement"/> Items collection and 
    /// controls its VisibleInStrip property.
    /// </summary>
    public class RadCommandBarOverflowMenuItem : RadMenuItemBase
    {

        #region Fields

        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private LightVisualElement menuElement;
        private RadMenuCheckmark menuCheckMark;
        private RadCommandBarBaseItem representedItem;
        private RadDropDownMenu ownerMenu;

        #endregion

        #region Ctor

        /// <summary>
        /// Create RadCommandBarOverflowMenuItem instance
        /// </summary>
        /// <param name="representedItem">Which item will be show in menu</param>
        /// <param name="ownerMenu">Menu that should be updated on representedItem visibility is changed</param>
        public RadCommandBarOverflowMenuItem(RadCommandBarBaseItem representedItem, RadDropDownMenu ownerMenu)
        {
            this.representedItem = representedItem;
            this.ownerMenu = ownerMenu;

            if (!string.IsNullOrEmpty(representedItem.DisplayName))
            {
                this.menuElement.Text = this.CheckForLongText(representedItem.DisplayName);
            }
            else
            {
                this.menuElement.Text = this.CheckForLongText( representedItem.Name);
            }

            Type t = this.representedItem.GetType();
            PropertyInfo p = t.GetProperty("Image");

            if (p != null)
            {
                this.menuCheckMark.CheckState = Enumerations.ToggleState.On;
                this.Image = (Image)(p.GetValue(this.representedItem, null));
            }

            if (this.menuElement.Image == null)
            {
                this.menuElement.Image = new Bitmap(16, 16);
            }
            else
            {
                this.Image = this.Image.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            }

            if (!this.representedItem.VisibleInStrip)
            {
                this.menuCheckMark.CheckState = Enumerations.ToggleState.Off;
            }
        }

        #endregion

        #region Properties

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadMenuItem);
            }
        }

        ///<summary>
        /// Gets or sets the image that is displayed on menu item element.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]        
        public new Image Image
        {
            get
            {
                return this.menuElement.Image;
            }
            set
            {
                this.menuElement.Image = value;
            }
        }

        ///<summary>
        /// Gets or sets the text that is displayed on menu item element.
        /// </summary>
        public new string Text
        {
            get
            {
                return this.menuElement.Text;
            }
            set
            {
                this.menuElement.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the item is in checked state.
        /// This property affects the VisibleInStrip property of the coresponding item in <see cref="CommandBarStripElement"/>.
        /// </summary>
        public bool Checked
        {
            get
            {
                return this.menuCheckMark.CheckState == Enumerations.ToggleState.On;
            }
            set
            {
                if (this.representedItem.VisibleInStrip == value)
                {
                    return;
                }

                this.representedItem.VisibleInStrip = value; 
               
                
                if (this.representedItem.VisibleInStrip)
                {
                    this.menuCheckMark.CheckState = Enumerations.ToggleState.On;
                }
                else
                {
                    this.menuCheckMark.CheckState = Enumerations.ToggleState.Off;
                }

                this.representedItem.Parent.InvalidateMeasure(true);
                this.representedItem.Parent.UpdateLayout();
                this.ownerMenu.PopupElement.InvalidateMeasure(true);
                this.ownerMenu.PopupElement.UpdateLayout();
            }
        }

        #endregion

        #region Helpers

        private string CheckForLongText(string text)
        {
            //special request by Mimi
            //if (text.Length > 20)
            //{
            //    return text.Substring(0, 20) + "...";
            //}

            return text;
        }

        #endregion
  
        #region Event Handlers

        protected void menuElement_MouseEnter(object sender, EventArgs e)
        {
            this.Select();
        }

        protected void menuElement_MouseLeave(object sender, EventArgs e)
        {
            this.Deselect();
        }

        protected void menuElement_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        #endregion

        #region Overrides

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuComboItem";
        }

        protected override void CreateChildElements()
        {
            // fill
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "RadMenuItemFillPrimitive";
            this.fillPrimitive.Name = "MenuComboItemFill";
            this.fillPrimitive.BackColor = Color.Empty;
            this.fillPrimitive.GradientStyle = GradientStyles.Solid;
            this.Children.Add(this.fillPrimitive);

            // border
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Visibility = ElementVisibility.Visible;
            this.borderPrimitive.Class = "RadMenuItemBorderPrimitive";
            this.borderPrimitive.Name = "MenuComboItemBorder";
            this.Children.Add(this.borderPrimitive);



            // light visual element
            this.menuElement = new LightVisualElement();
            this.menuElement.MinSize = new Size(100, 20);
            this.menuElement.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.menuElement.TextAlignment = ContentAlignment.MiddleLeft;
            this.menuElement.ImageAlignment = ContentAlignment.MiddleLeft;
            this.menuElement.ImageLayout = ImageLayout.Stretch;

            //  this.menuElement.
            this.menuElement.MaxSize = new Size(0, 16);
            this.menuElement.MinSize = new Size(0, 16);
            this.menuElement.MouseEnter += new EventHandler(menuElement_MouseEnter);
            this.menuElement.MouseLeave += new EventHandler(menuElement_MouseLeave);
            this.menuElement.Click += new EventHandler(menuElement_Click);
            this.Children.Add(this.menuElement);

            // Disable the showing of the ComboBox's popup in design-time
            if (this.DesignMode)
            {
                this.menuElement.RoutedEventBehaviors.Add(new CancelMouseBehavior());
            }

            // checkmark
            this.menuCheckMark = new RadMenuCheckmark();
            this.menuCheckMark.MinSize = new Size(13, 13);
            this.menuCheckMark.Alignment = ContentAlignment.MiddleCenter;
            this.menuCheckMark.CheckElement.Alignment = ContentAlignment.MiddleCenter;
            this.menuCheckMark.Class = "RadMenuItemCheckPrimitive";
            this.menuCheckMark.NotifyParentOnMouseInput = false;

            this.Children.Add(this.menuCheckMark);

        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = SizeF.Empty;
            float leftColumn = 0;
            float rightColumn = 0;
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            if (menuLayout != null)
            {
                leftColumn = menuLayout.LeftColumnMaxPadding + menuLayout.LeftColumnWidth;
                rightColumn = menuLayout.LeftColumnMaxPadding + menuLayout.RightColumnWidth;
            }
            for (int i = 0; i < this.Children.Count; i++)
            {
                RadElement child = this.Children[i];
                child.Measure(availableSize);
                res.Height = Math.Max(child.DesiredSize.Height, res.Height);
                if (object.ReferenceEquals(child, this.menuElement))
                {
                    res.Width += child.DesiredSize.Width;
                }
            }
            res.Width += leftColumn + rightColumn;
            res.Width += this.Padding.Horizontal + this.BorderThickness.Horizontal;
            res.Height += this.Padding.Vertical + this.BorderThickness.Vertical;
            return res;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            RadDropDownMenuLayout menuLayout = this.Parent as RadDropDownMenuLayout;
            float leftColumn = 0;
            float rightColumn = 0;
            if (menuLayout != null)
            {
                leftColumn = menuLayout.LeftColumnMaxPadding + menuLayout.LeftColumnWidth;
                rightColumn = menuLayout.LeftColumnMaxPadding + menuLayout.RightColumnWidth;
            }

            foreach (RadElement element in this.Children)
            {
                if (element == this.menuElement)
                {
                    if (this.RightToLeft)
                    {
                        element.Arrange(new RectangleF(clientRect.Right - leftColumn - clientRect.Width + (leftColumn + rightColumn), clientRect.Top,
                         clientRect.Width - (leftColumn + rightColumn), element.DesiredSize.Height));
                    }
                    else
                    {
                        element.Arrange(new RectangleF(clientRect.Left + leftColumn, clientRect.Top,
                            clientRect.Width - (leftColumn + rightColumn), element.DesiredSize.Height));
                 
                        }
                }
                else if (element == this.menuCheckMark)
                {
                    RectangleF rect = new RectangleF(clientRect.X, clientRect.Y, leftColumn, clientRect.Height);
                    if (this.RightToLeft)
                    {
                        rect.X = clientRect.Width - rect.Width;
                    }

                    element.Arrange(rect);
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentBounds)
                {
                    element.Arrange(new RectangleF(Point.Empty, finalSize));
                }
                else if (element.FitToSizeMode == RadFitToSizeMode.FitToParentPadding)
                {
                    element.Arrange(new RectangleF(this.BorderThickness.Left, this.BorderThickness.Top, finalSize.Width - this.BorderThickness.Horizontal, finalSize.Height - this.BorderThickness.Vertical));
                }
                else
                {
                    element.Arrange(clientRect);
                }
            }

            return finalSize;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Class")
            {
                return this.Class != "RadMenuComboItem";
            }

            return base.ShouldSerializeProperty(property);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Checked = !this.Checked;

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Select();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Deselect();
        }

        #endregion

        private class CancelMouseBehavior : RoutedEventBehavior
        {
            public CancelMouseBehavior()
                : base(new RaisedRoutedEvent(
                RadElement.MouseDownEvent, string.Empty, EventBehaviorSenderType.AnySender, RoutingDirection.Tunnel))
            {
            }

            public override void OnEventOccured(RadElement sender, RadElement element, RoutedEventArgs args)
            {
                args.Canceled = true;
                base.OnEventOccured(sender, element, args);
            }
        }

    }
}
