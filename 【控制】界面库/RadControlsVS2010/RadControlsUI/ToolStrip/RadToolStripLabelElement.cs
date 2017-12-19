using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a tool strip label element. You can use this element to include labels 
    /// in the toolstrip.  
    /// </summary>
	public class RadToolStripLabelElement : RadItem
	{
		private TextPrimitive textPrimitive;

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == TextProperty)
			{
				this.textPrimitive.Text = (string)e.NewValue;
			}
			base.OnPropertyChanged(e);
		}

		protected override void CreateChildElements()
		{
			this.textPrimitive = new TextPrimitive();
			this.textPrimitive.Text = "RadToolStrip";

			this.Children.Add(this.textPrimitive);
		}	
	}
}
