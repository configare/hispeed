using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telerik.WinControls.Paint;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls;
using Telerik.WinControls.UI;
namespace Telerik.WinControls.Primitives
{
	public class GroupStatePrimitive : BasePrimitive
	{
		public enum GroupState
		{
			Expanded,
			Collapsed
		}

        //TODO - remove usage ot minSize static var, in favor of the MinSize property
		static readonly Size minSize = new Size(10, 10);

		#region Contructors

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
        }

		#endregion

		#region Properties
		///////////////////////////////////////////////////////////////////////
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

		public static readonly RadProperty GroupStyleProperty =
			RadProperty.Register("PanelBarStyle", typeof(PanelBarStyles), typeof(GroupStatePrimitive),
				new RadElementPropertyMetadata(PanelBarStyles.ExplorerBarStyle, ElementPropertyOptions.AffectsDisplay));

		[RadPropertyDefaultValue("PanelBarStyle", typeof(GroupStatePrimitive)), Description(RadDesignCategory.AppearanceCategory)]
		public PanelBarStyles PanelBarStyle
		{
			get
			{
				return (PanelBarStyles)this.GetValue(GroupStyleProperty);
			}
			set
			{
				this.SetValue(GroupStyleProperty, value);
			}
		}


		public static readonly RadProperty GroupStateProperty =
			RadProperty.Register("State", typeof(GroupState), typeof(GroupStatePrimitive),
				new RadElementPropertyMetadata(GroupState.Expanded, ElementPropertyOptions.AffectsDisplay));

		[RadPropertyDefaultValue("State", typeof(GroupStatePrimitive)), Description(RadDesignCategory.AppearanceCategory)]
		public GroupState State
		{
			get
			{
				return (GroupState)this.GetValue(GroupStateProperty);
			}
			set
			{
				this.SetValue(GroupStateProperty, value);
			}
		}

		#endregion


		public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
		{
			Rectangle rect = new Rectangle(Point.Empty, this.Size);

			graphics.ChangeSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.AntiAlias);
			switch (this.PanelBarStyle)
			{
				case PanelBarStyles.VisualStudio2005ToolBox:
					if (this.State == GroupState.Expanded)
					{
						graphics.DrawLine(Color.Black, 0, rect.Height / 2, rect.Width, rect.Height / 2);
					}
					else
					{
						graphics.DrawLine(Color.Black, 0, rect.Height / 2, rect.Width, rect.Height / 2);
						graphics.DrawLine(Color.Black, rect.Width / 2, 0, rect.Width / 2, rect.Height);
					}
					break;
				case PanelBarStyles.ExplorerBarStyle:
					int arrowOffset = 4;
					if (this.State == GroupState.Expanded)
					{
						graphics.FillPolygon(this.ForeColor,
							new Point[]
							{
								new Point(0, rect.Height / 2),
								new Point(rect.Width / 2, 0),
								new Point(rect.Width, rect.Height / 2),
								new Point(rect.Width - 2, rect.Height / 2),
								new Point(rect.Width / 2, 2),
								new Point(2, rect.Height / 2)
							});

						graphics.FillPolygon(this.ForeColor,
							new Point[]
							{
								new Point(0, rect.Height / 2 + arrowOffset),
								new Point(rect.Width / 2, arrowOffset),
								new Point(rect.Width, rect.Height / 2 + arrowOffset),
								new Point(rect.Width - 2, rect.Height / 2 + arrowOffset),
								new Point(rect.Width / 2, 2 + arrowOffset),
								new Point(2, rect.Height / 2 + arrowOffset)
							});
					}
					else
					{
						graphics.FillPolygon(this.ForeColor,
							new Point[]
							{
								new Point(0, rect.Height / 2),
								new Point(rect.Width / 2, rect.Height),
								new Point(rect.Width, rect.Height / 2),
								new Point(rect.Width - 2, rect.Height / 2),
								new Point(rect.Width / 2, rect.Height -  2),
								new Point(2, rect.Height / 2)
							});

						graphics.FillPolygon(this.ForeColor,
							new Point[]
							{
								new Point(0, rect.Height / 2 - arrowOffset),
								new Point(rect.Width / 2, rect.Height - arrowOffset),
								new Point(rect.Width, rect.Height / 2 - arrowOffset),
								new Point(rect.Width - 2, rect.Height / 2 - arrowOffset),
								new Point(rect.Width / 2, rect.Height - 2 - arrowOffset),
								new Point(2, rect.Height / 2 - arrowOffset)
							});
					}
					break;
			}
			graphics.RestoreSmoothingMode();
		}

		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			return minSize;
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            // Ensure that Measure() is called for all children
            base.MeasureOverride(availableSize);

            return minSize;
        }
	}
}
