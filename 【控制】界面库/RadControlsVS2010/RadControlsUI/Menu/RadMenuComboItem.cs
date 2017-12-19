using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///		Represents a menu item which has a combobox placed inside.
    /// </summary>
    public class RadMenuComboItem : RadMenuItemBase
    {
        // Fields
        private ImagePrimitive imagePrimitive;
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;
        private RadComboBoxElement comboBoxElement;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuComboItem";
        }

        #region Properties

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadMenuItem);
            }
        }

        /// <summary>
        ///		Provides a reference to the ComboBox element in the menu item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Provides a reference to the ComboBox element in the menu item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadComboBoxElement ComboBoxElement
        {
            get
            {
                return this.comboBoxElement;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory),
        Browsable(false)]
        [Description("Gets a collection representing the items contained in this ComboBox.")]
        public new RadItemCollection Items
        {
            get
            {
                return this.comboBoxElement.Items;
            }
        }

        #endregion

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
            this.borderPrimitive.Visibility = ElementVisibility.Collapsed;
            this.borderPrimitive.Class = "RadMenuComboItemBorderPrimitive";
            this.borderPrimitive.Name = "MenuComboItemBorder";
            this.Children.Add(this.borderPrimitive);

            // image
            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.Class = "RadMenuComboItemImagePrimitive";
            this.Children.Add(this.imagePrimitive);

            // combo box
            this.comboBoxElement = new RadComboBoxElement();
            this.comboBoxElement.ArrowButton.Arrow.AutoSize = true;
            this.comboBoxElement.MinSize = new Size(100, 20);
            this.Children.Add(this.comboBoxElement);

            // Disable the showing of the ComboBox's popup in design-time
            if (this.DesignMode)
            {
                this.comboBoxElement.ArrowButton.RoutedEventBehaviors.Add(new CancelMouseBehavior());
            }
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
                if (object.ReferenceEquals(child, this.comboBoxElement))
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
                if (element == this.imagePrimitive)
                {
                    float leftColumnWidth = menuLayout != null ? menuLayout.LeftColumnWidth : 0;
                    RectangleF finalRect = new RectangleF(clientRect.Left, clientRect.Top, leftColumnWidth, clientRect.Height);
                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
                }
                else if (element == this.comboBoxElement)
                {
                    RectangleF finalRect = new RectangleF(clientRect.Left + leftColumn, clientRect.Top,
                        clientRect.Width - (leftColumn + rightColumn), element.DesiredSize.Height);
                    if (this.RightToLeft)
                    {
                        finalRect = LayoutUtils.RTLTranslateNonRelative(finalRect, clientRect);
                    }

                    element.Arrange(finalRect);
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