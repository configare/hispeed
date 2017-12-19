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
using Telerik.WinControls.Styles;


namespace Telerik.WinControls.UI
{

    /// <summary>
    /// Represents an arrow button element. Each telerik control has a 
    /// corresponding tree of RadElements; the RadArrowButtonElement can be nested 
    /// in other telerik controls. 
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadCommandBarArrowButton : LightVisualElement
    {
        private ArrowPrimitive arrow;

        private static Size defaultSize = new Size(16, 16);

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
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.SetValue(RadItem.IsMouseDownProperty, false);
            this.SetValue(RadItem.IsMouseOverProperty, false);
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

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadCommandBarArrowButton);
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.NotifyParentOnMouseInput = true;
        }
           

        protected override void CreateChildElements()
        {
            this.arrow = new ArrowPrimitive(ArrowDirection.Down);
            this.arrow.Class = "RadArrowButtonArrow";
            this.arrow.AutoSize = false;
            this.arrow.Alignment = ContentAlignment.MiddleCenter;
            this.Children.Add(this.arrow);

            this.NotifyParentOnMouseInput = true;
        }


        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            return RadCommandBarArrowButton.defaultSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF arrowRect = new RectangleF((finalSize.Width - arrow.DesiredSize.Width) / 2f,
                                                  (finalSize.Height - arrow.DesiredSize.Height) / 2f,
                                                  arrow.DesiredSize.Width,
                                                  arrow.DesiredSize.Height);
            arrow.Arrange(arrowRect);

            return finalSize;
        }

        #region System skinning

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            this.arrow.Visibility = ElementVisibility.Hidden;
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();

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
