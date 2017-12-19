using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using VisualStyles = System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an arrow button element. Each telerik control has a 
    /// corresponding tree of RadElements; the RadArrowButtonElement can be nested 
    /// in other telerik controls. 
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadArrowButtonElement : RadButtonItem
    {
        private ArrowPrimitive arrow;
        private OverflowPrimitive overflowArrow;
        private BorderPrimitive borderPrimitive;
        private FillPrimitive fillPrimitive;
        private ImagePrimitive imagePrimitive;

        private static Size defaultSize = new Size(16, 16);

        /// <summary>Gets the default size of the <see cref="RadArrowButtonElement"/></summary>		
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Size RadArrowButtonDefaultSize
        {
            get { return defaultSize; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size ArrowFullSize
        {
            get
            {
                return new Size(this.arrow.Size.Width + (int)this.borderPrimitive.HorizontalWidth,
                    this.arrow.Size.Height + (int)this.borderPrimitive.VerticalWidth);
            }
        }

        /// <summary>Gets or sets the 
        /// %arrow direction:Telerik.WinControls.Primitives.ArrowPrimitive.ArrowDirection%.</summary>
        public ArrowDirection Direction
        {
            get
            {
                return this.arrow.Direction;
            }
            set
            {
                this.arrow.Direction = value;
                this.overflowArrow.Direction = value;
            }
        }

        /// <summary>Gets the BorderPrimitive object.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BorderPrimitive Border
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        /// <summary>Gets the FillPrimitive object.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FillPrimitive Fill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>Gets the ArrowPrimitive object.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ArrowPrimitive Arrow
        {
            get
            {
                return this.arrow;
            }
        }

        static RadArrowButtonElement()
        {
            DefaultSizeProperty.OverrideMetadata(typeof(RadArrowButtonElement),
                new RadElementPropertyMetadata(defaultSize,
                ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsMeasure));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.NotifyParentOnMouseInput = true;
        }

        /// <summary>
        /// If set to true shows and OverflowPrimitive instead of an ArrowPrimitive.
        /// </summary>
        [DefaultValue(true)]
        public bool OverflowMode
        {
            get
            {
                return this.overflowArrow.Visibility == ElementVisibility.Collapsed;
            }
            set
            {
                if (value)
                {
                    this.Arrow.Visibility = ElementVisibility.Collapsed;
                    this.overflowArrow.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.Arrow.Visibility = ElementVisibility.Visible;
                    this.overflowArrow.Visibility = ElementVisibility.Collapsed;
                }
                this.InvalidateMeasure();
            }
        }

        internal void SetFillClass(string fillClass)
        {
            this.fillPrimitive.Class = fillClass;
        }

        protected override void CreateChildElements()
        {
            this.arrow = new ArrowPrimitive(ArrowDirection.Down);
            this.arrow.Class = "RadArrowButtonArrow";
            this.arrow.AutoSize = false;
            this.arrow.Alignment = ContentAlignment.MiddleCenter;

            this.overflowArrow = new OverflowPrimitive(ArrowDirection.Down);
            this.overflowArrow.Class = "RadArrowButtonOverflowArrow";
            this.overflowArrow.AutoSize = false;
            this.overflowArrow.Alignment = ContentAlignment.MiddleCenter;
            this.overflowArrow.Visibility = ElementVisibility.Collapsed;

            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "RadArrowButtonFill";
            this.fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RadArrowButtonBorder";
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.Class = "RadArrowButtonImage";
            this.imagePrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.imagePrimitive.Alignment = ContentAlignment.MiddleCenter;

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.arrow);
            this.Children.Add(this.overflowArrow);
            this.Children.Add(this.imagePrimitive);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF arrowRect = new RectangleF((finalSize.Width - arrow.DesiredSize.Width) / 2f,
                                                  (finalSize.Height - arrow.DesiredSize.Height) / 2f,
                                                  arrow.DesiredSize.Width,
                                                  arrow.DesiredSize.Height);
            arrow.Arrange(arrowRect);
            arrowRect = new RectangleF((finalSize.Width - overflowArrow.DesiredSize.Width) / 2f,
                                                 (finalSize.Height - overflowArrow.DesiredSize.Height) / 2f,
                                                 overflowArrow.DesiredSize.Width,
                                                 overflowArrow.DesiredSize.Height);
            overflowArrow.Arrange(arrowRect);
            return finalSize;
        }

        #region System skinning

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            this.fillPrimitive.Visibility = ElementVisibility.Hidden;
            this.borderPrimitive.Visibility = ElementVisibility.Hidden;
            this.arrow.Visibility = ElementVisibility.Hidden;
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();

            this.fillPrimitive.Visibility = ElementVisibility.Visible;
            this.borderPrimitive.Visibility = ElementVisibility.Visible;
            this.arrow.Visibility = ElementVisibility.Visible;
        }

        protected override void PaintElementSkin(Telerik.WinControls.Paint.IGraphics graphics)
        {
            if (object.ReferenceEquals(SystemSkinManager.DefaultElement, SystemSkinManager.Instance.CurrentElement))
            {
                return;
            }

            base.PaintElementSkin(graphics);
        }

        private void PaintElementSkin(Graphics g, VisualStyles.VisualStyleElement element, Rectangle bounds)
        {
            if (SystemSkinManager.Instance.SetCurrentElement(element))
            {
                SystemSkinManager.Instance.PaintCurrentElement(g, bounds);
            }
        }

        public override VisualStyles.VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        public override VisualStyles.VisualStyleElement GetXPVisualStyle()
        {
            if (!this.Enabled)
            {
                return VisualStyles.VisualStyleElement.ComboBox.DropDownButton.Disabled;
            }
            else
            {
                if (!this.IsMouseOver && !this.IsMouseDown)
                {
                    return VisualStyles.VisualStyleElement.ComboBox.DropDownButton.Normal;
                }
                else if (this.IsMouseOver && !this.IsMouseDown)
                {
                    return VisualStyles.VisualStyleElement.ComboBox.DropDownButton.Hot;
                }
                else if (this.IsMouseOver && this.IsMouseDown)
                {
                    return VisualStyles.VisualStyleElement.ComboBox.DropDownButton.Hot;
                }
            }

            return SystemSkinManager.DefaultElement;
        }

        #endregion
    }
}
