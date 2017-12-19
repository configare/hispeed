using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.RadColorPicker
{
	/// <summary>
	/// A transparent color box where semi-transparent colors can be shown
	/// </summary>
	[ToolboxItem(false), ComVisible(false)]
	public class TransparentColorBox : RadControl
	{
		private TransparentColorBoxElement element;
		public TransparentColorBox()
		{
		}

		/// <summary>
		/// Gets or sets the color shown in the box
		/// </summary>
		public override System.Drawing.Color BackColor
		{
			get
			{
                return this.element.BackColor;
			}
			set
			{
				this.element.BackColor = value;
			}

		}

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            element = new TransparentColorBoxElement();
            element.StretchHorizontally = true;
            element.StretchVertically = true;
            this.RootElement.Children.Add(element);
        }
	}
	/// <summary>
	/// A transparent color box where semi-transparent colors can be shown
	/// </summary>
	public class TransparentColorBoxElement : RadElement
	{
		LightVisualElement colorElement = new LightVisualElement();

		protected override void CreateChildElements()
		{
			base.CreateChildElements();

			LightVisualElement imageBackgroundElement = new LightVisualElement();
			imageBackgroundElement.Image = Telerik.WinControls.UI.Properties.Resources.Checkerboard;
			imageBackgroundElement.Alignment = System.Drawing.ContentAlignment.TopLeft;
			imageBackgroundElement.ImageLayout = System.Windows.Forms.ImageLayout.Tile;
			imageBackgroundElement.StretchVertically = true;
			imageBackgroundElement.StretchHorizontally = true;
			imageBackgroundElement.ZIndex = 1;

			colorElement.DrawFill = true;
			colorElement.NumberOfColors = 1;
			colorElement.BackColor = System.Drawing.Color.Red;
			colorElement.StretchHorizontally = true;
			colorElement.StretchVertically = true;
			colorElement.ZIndex = 2;

			this.Children.Add(imageBackgroundElement);
			this.Children.Add(colorElement);
		}
		/// <summary>
		/// Gets or set the color shown in the box
		/// </summary>
		public Color BackColor
		{
			get
			{
				return colorElement.BackColor;
			}
			set
			{
				colorElement.BackColor = value;
			}
		}
	}
}
