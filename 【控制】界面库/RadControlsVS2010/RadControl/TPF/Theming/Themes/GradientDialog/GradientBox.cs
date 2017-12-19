using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{   
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
    partial class GradientBox : RadControl
	{
		FillPrimitive fill;
		BorderPrimitive border;

        public GradientBox()
        {
            this.Behavior.BitmapRepository.DisableBitmapCache = true;
        }

		public FillPrimitive Fill
		{
			get { return fill; }
		}

		public BorderPrimitive Border
		{
			get { return border; }
		}

		public GradientStyles FillStyle
		{
			get
			{
				return this.fill.GradientStyle;
			}
			set
			{
                this.fill.GradientStyle = value;
			}
		}

		public float FillAngle
		{
			get
			{
                return this.fill.GradientAngle;
			}
			set
			{
                this.fill.GradientAngle = value;
			}
		}

		protected override void CreateChildItems(RadElement parent)
        {
            fill = new FillPrimitive();
            fill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            border = new BorderPrimitive();
            border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.RootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.RootElement.Children.Add(this.fill);
            this.RootElement.Children.Add(this.border);
        }
	}
}
