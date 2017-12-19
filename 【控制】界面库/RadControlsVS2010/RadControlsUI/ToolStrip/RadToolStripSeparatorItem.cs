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
    /// <summary>Represents a <see cref="RadToolStripItem">rad tool strip</see> separator item.</summary>
	public class RadToolStripSeparatorItem : RadItem
	{
        /// <summary>Initializes a new instance of the RadToolStripSeparatorItem class.</summary>
		public RadToolStripSeparatorItem()
        {
       		
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.StretchVertically = true;
            this.StretchHorizontally = true;
            this.MinSize = new Size(2, 17);
        }
        
		public static RadProperty SweepAngleProperty = RadProperty.Register("SweepAngle", typeof(int),
			typeof(RadToolStripSeparatorItem),
			 new RadElementPropertyMetadata(0, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty OrientationProperty = RadProperty.Register("SeparatorOrientation", typeof(SepOrientation),
		   typeof(RadToolStripSeparatorItem),
			new RadElementPropertyMetadata(SepOrientation.Vertical, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty LineWidthProperty = RadProperty.Register("LineWidth", typeof(int),
		 typeof(RadToolStripSeparatorItem),
		  new RadElementPropertyMetadata(2, ElementPropertyOptions.AffectsDisplay));


        /// <summary>
        ///     Gets or sets separator orientation. Possible values are members of
        ///     <see cref="Telerik.WinControls.SepOrientation">SepOrientation</see> enumeration.
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

		/// <summary>Gets or sets line width in pixels.</summary>
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

        /// <summary>Gets or set the separator angle in degrees.</summary>
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

		protected override void CreateChildElements()
		{
			FillPrimitive LineFill = new FillPrimitive();
			LineFill.BackColor = Color.Silver;
			LineFill.BackColor2 = Color.Silver;

			LineFill.MinSize = this.MinSize;
			LineFill.Class = "LineFill";
		
			this.Children.Add(LineFill);
        }
	}
}
