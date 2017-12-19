using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents a menu separation item. 
    /// Use it to separate logically unrelated items in the menu.</summary>
	public class RadMenuSeparatorItem : RadMenuItemBase
	{
		private LinePrimitive linePrimitive;
        private float cachedLineOffset;
        /// <summary><para>Initializes a new instance of the RadMenuSeparatorItem class.</para></summary>
        public RadMenuSeparatorItem()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuSeparatorItem";
        }

        #region Properties

        public static RadProperty SweepAngleProperty = RadProperty.Register(
			"SweepAngle", typeof(int), typeof(RadMenuSeparatorItem), new RadElementPropertyMetadata(
				0, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or set the sweep angle in degrees.</summary>
        [DefaultValue(0)]
        public virtual int SweepAngle
        {
            get
            {
                return (int)GetValue(SweepAngleProperty);
            }
            set
            {
                SetValue(SweepAngleProperty, value);
            }
        }

		public static RadProperty OrientationProperty = RadProperty.Register(
			"SeparatorOrientation", typeof(SepOrientation), typeof(RadMenuSeparatorItem), new RadElementPropertyMetadata(
				SepOrientation.Horizontal, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        ///     Gets or sets the <see cref="Telerik.WinControls.SepOrientation">separator
        ///     orientation</see>. Possible values are members of SepOrientation enumeration.
        /// </summary>
        public virtual SepOrientation SeparatorOrientation
        {
            get
            {
                return (SepOrientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public static RadProperty LineWidthProperty = RadProperty.Register(
            "LineWidth", typeof(int), typeof(RadMenuSeparatorItem), new RadElementPropertyMetadata(
                1, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LineOffsetProperty = RadProperty.Register(
            "LineOffset", typeof(float), typeof(RadMenuSeparatorItem), new RadElementPropertyMetadata(
                3.0f, ElementPropertyOptions.AffectsDisplay));

        /// <summary>Gets or sets separators width in pixels.</summary>
        public virtual int LineWidth
        {
            get
            {
                return (int)GetValue(LineWidthProperty);
            }
            set
            {
                SetValue(LineWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the location where the draw of the line should start
        /// </summary>
        public virtual float LineOffset
        {
            get
            {
                return cachedLineOffset;
            }
            set
            {
                cachedLineOffset = value;
                SetValue(LineOffsetProperty, value);
            }
        }

        public static RadProperty ShowFillProperty = RadProperty.Register(
			"ShowFill", typeof(bool), typeof(RadMenuSeparatorItem), new RadElementPropertyMetadata(
				true, ElementPropertyOptions.AffectsDisplay));

		//private TextPrimitive textPrimitive;
		//private FillPrimitive fillPrimitive;
		//private BorderPrimitive borderPrimitive;

        /// <summary>Gets a value indicating whether the RadMenuSeparator can be selected.</summary>
		public override bool Selectable
		{
			get
			{
				return false;
			}
        }

        #endregion

        #region Event handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == ShowFillProperty)
			{
				bool showFill = (bool)e.NewValue;
				this.linePrimitive.Visibility = showFill ? ElementVisibility.Visible : ElementVisibility.Hidden;
			}
        }

        #endregion

        protected override void CreateChildElements()
        {
            this.linePrimitive = new LinePrimitive();
            this.linePrimitive.Class = "LineFill";
            this.linePrimitive.SetDefaultValueOverride(RadElement.MarginProperty,new Padding(0, 2, 0, 2));
            this.linePrimitive.BindProperty(LinePrimitive.SweepAngleProperty, this, SweepAngleProperty, PropertyBindingOptions.TwoWay);
            this.linePrimitive.BindProperty(LinePrimitive.LineWidthProperty, this, LineWidthProperty, PropertyBindingOptions.TwoWay);
            this.linePrimitive.BindProperty(LinePrimitive.OrientationProperty, this, OrientationProperty, PropertyBindingOptions.TwoWay);
            this.Children.Add(this.linePrimitive);

            cachedLineOffset =  (float)GetValue(LineOffsetProperty);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RadDropDownMenuLayout menuLayout = FindAncestor<RadDropDownMenuLayout>();
            if (menuLayout != null && this.linePrimitive != null)
            {
                RectangleF rect = new RectangleF(menuLayout.LeftColumnWidth + menuLayout.LeftColumnMaxPadding + this.cachedLineOffset, 0f,
                    finalSize.Width - (menuLayout.LeftColumnWidth + menuLayout.LeftColumnMaxPadding), finalSize.Height);
                if (this.RightToLeft)
                {
                    rect.X = finalSize.Width - rect.X - rect.Width;
                }

                this.linePrimitive.Arrange(rect);
            }
            return finalSize;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "Class")
            {
                return this.Class != "RadMenuSeparatorItem";
            }

            return base.ShouldSerializeProperty(property);
        }
    }
}