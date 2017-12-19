using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Paint;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.Primitives
{
	[Editor(typeof(RadFillEditor), typeof(UITypeEditor))]
	public class GripPrimitive : BasePrimitive
	{
		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
        }

        [DefaultValue(false)]
        public override bool StretchHorizontally
        {
            get { return base.StretchHorizontally; }
            set { base.StretchHorizontally = value; }
        }

        [DefaultValue(false)]
        public override bool StretchVertically
        {
            get { return base.StretchVertically; }
            set { base.StretchVertically = value; }
        }

		public static RadProperty NumberOfDotsProperty = RadProperty.Register(
			"NumberOfDots", typeof(int), typeof(GripPrimitive), new RadElementPropertyMetadata(
				4, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty BackColor2Property = RadProperty.Register(
			"BackColor2", typeof(Color), typeof(GripPrimitive), new RadElementPropertyMetadata(
				Color.FromKnownColor(KnownColor.Control), ElementPropertyOptions.AffectsDisplay));

		[Description("Second color component when gradient style is other than solid")]
		[RadPropertyDefaultValue("BackColor2", typeof(GripPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
		[Editor(typeof(RadColorEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(RadColorEditorConverter))]
		public virtual Color BackColor2
		{
			get
			{
				return(Color)this.GetValue(BackColor2Property);
			}
			set
			{
				this.SetValue(BackColor2Property, value);
			}
		}

		public virtual int NumberOfDots
		{
			get
			{
				return (int)GetValue(NumberOfDotsProperty);
			}
			set
			{
				SetValue(NumberOfDotsProperty, value);
			}
		}

		public override void PaintPrimitive(IGraphics g, float angle, SizeF scale)
		{

			Rectangle rect = new Rectangle(Point.Empty, this.Size);

			int dotsOffset = (this.Size.Height) / NumberOfDots;

			RectangleF chunkbar = new RectangleF(0, 4, 2F, 2F);

			for (int i = 0; ; i++)
			{
				RectangleF shadowRect = new RectangleF(chunkbar.X + 0.1F, chunkbar.Y + 0.1F, 2F, 2F);

				g.FillRectangle(shadowRect, BackColor2);		
				g.FillRectangle(chunkbar, BackColor);
				chunkbar.Y += 4;
				if (chunkbar.Y + 6 > this.Size.Height) break;
			}

		}
	}
}
