using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using ContentAlignment = System.Drawing.ContentAlignment;
using System.Runtime.InteropServices;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a toolstrip overflow button element. 
    /// </summary>
	[RadToolboxItem(false), ComVisible(false)]
	public class RadToolStripOverFlowButtonElement : RadDropDownButtonElement
	{
		private OverflowPrimitive overFlowPrimitive;
		private FillPrimitive fillPrimitive;
		private BorderPrimitive borderPrimitive;

        public RadToolStripOverFlowButtonElement()
        {
        }
        /// <summary>
        /// Gets the drop down button arrow position.
        /// </summary>
		public override DropDownButtonArrowPosition ArrowPosition
		{
			get
			{
				return DropDownButtonArrowPosition.Left;
			}
		}
		private class HiddenItemsPrimitive : BasePrimitive
		{
			protected override void InitializeFields()
            {
                base.InitializeFields();

                this.MinSize = new Size(10, 10);
            }

			public static RadProperty BackColor2Property = RadProperty.Register(
				"BackColor2", typeof(Color), typeof(HiddenItemsPrimitive), new RadElementPropertyMetadata(
					SystemColors.Control, ElementPropertyOptions.AffectsDisplay));

			[Description("Second color component when gradient style is other than solid")]
			[RadPropertyDefaultValue("BackColor2", typeof(GripPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
			[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
			[TypeConverter(typeof(RadColorEditorConverter))]
			public virtual Color BackColor2
			{
				get
				{
					return (Color)this.GetValue(BackColor2Property);
				}
				set
				{
					this.SetValue(BackColor2Property, value);
				}
			}

			public override void PaintPrimitive(IGraphics g, float angle, SizeF scale)
			{
				int x = 1;
				int y = 5;

				g.DrawLine(Color.Black, x, y, x, y+2);
				g.DrawLine(Color.Black, x, y + 1.5f, x + 1, y + 1.5f);

				g.DrawLine(Color.White, x+1, y+2, x+2, y+2);
				g.DrawLine(Color.White, x + 1, y + 2, x + 1, y + 3);

				// Draws The second arrow with its shadow                  
				g.DrawLine(Color.Black, x+4, y, x+4, y + 2);
				g.DrawLine(Color.Black, x + 4, y + 1.5f, x + 5, y + 1.5f);

				g.DrawLine(Color.White, x + 5, y + 2, x + 6, y + 2);
				g.DrawLine(Color.White, x + 5, y + 2, x + 5, y + 3);

			}

			public static readonly Size MinVerticalSize;

			static HiddenItemsPrimitive()
			{
				MinVerticalSize = new Size(10, 7);
			}
		}

		public override bool ShowArrow
		{
			get
			{
				return true;
			}
		}
        /// <summary>
        /// Shows small arrows.
        /// </summary>
        /// <param name="show"></param>
		public void ShowSmallArrows(bool show)
		{
			if ( show )
				this.hiddenItems.Visibility = ElementVisibility.Visible;
			else
				this.hiddenItems.Visibility = ElementVisibility.Hidden;
	
		}

        /// <summary>
        /// Gets the overflow primitive.
        /// </summary>
		public OverflowPrimitive OverFlowPrimitive
		{
			get
			{
				return this.overFlowPrimitive;
			}
		}

		private HiddenItemsPrimitive hiddenItems;

        /// <summary>
        /// Creates child elements.
        /// </summary>
		protected override void CreateChildElements()
		{
			this.overFlowPrimitive = new OverflowPrimitive(ArrowDirection.Down);	
			this.overFlowPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.overFlowPrimitive.AutoSize = false;
			this.overFlowPrimitive.Alignment = ContentAlignment.BottomLeft;
			this.overFlowPrimitive.Class = "overFlowButton";			
		
			this.fillPrimitive = new FillPrimitive();
			this.fillPrimitive.Class = "overFlowButtonFill";

			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Class = "overFlowBorder";
			this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.borderPrimitive.Visibility = ElementVisibility.Hidden;

			this.hiddenItems = new HiddenItemsPrimitive();
			this.hiddenItems.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.hiddenItems.AutoSize = true;
			this.hiddenItems.Class = "OverFlowArrows";
			this.hiddenItems.Visibility = ElementVisibility.Hidden;
			this.hiddenItems.BackColor = Color.White;
			this.hiddenItems.BackColor2 = Color.Black;

			this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
		
			this.Children.Add(this.hiddenItems);
			this.Children.Add(this.overFlowPrimitive);
		}

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            base.ArrangeOverride(finalSize);
            RectangleF arrowRect = new RectangleF((finalSize.Width - overFlowPrimitive.DesiredSize.Width) / 2f,
                                                  (finalSize.Height - overFlowPrimitive.DesiredSize.Height) / 2f,
                                                  overFlowPrimitive.DesiredSize.Width,
                                                  overFlowPrimitive.DesiredSize.Height);
            overFlowPrimitive.Arrange(arrowRect);
            return finalSize;
        }
	}
}
