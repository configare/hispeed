using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Primitives;
using System.Reflection;

namespace Telerik.WinControls
{
    /// <exclude/>
	public partial class RadGradientDialog : Form
	{


		public RadGradientDialog()
		{
			InitializeComponent();
	

        }
        //IFillElement
		public FillPrimitive Fill
		{
			get
			{
				return this.editorControl.Fill;
			}
		}

	}
}