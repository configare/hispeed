using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telerik.WinControls.Paint;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Primitives
{
	public class OverflowPrimitive : BasePrimitive
	{
		      /// <summary>
        /// Specifies arrow directions for the arrow primitive: Up, Right, Down, and
        /// Left.
        /// </summary>

        public static readonly Size MinHorizontalSize = new Size(4, 8);
		public static readonly Size MinVerticalSize = new Size(8, 10);
        
        #region Contructors

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
        }


        /// <summary>
        /// 	<para>Initializes a new instance of the ArrowPrimitive class using the
        ///     ArrowDirection enumeration. Possible directions are up, bottom, left, and
        ///     right.</para>
        /// </summary>
		public OverflowPrimitive(ArrowDirection arrowDirection)
        {
            this.Direction = arrowDirection;

			if (this.Direction == ArrowDirection.Left ||
				this.Direction == ArrowDirection.Right)
			{
				this.Size = MinHorizontalSize;
			}
			else
			{
				this.Size = MinVerticalSize;
			}
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

		public static readonly RadProperty ArrowDirectionProperty =
			RadProperty.Register("ArrowDirection", typeof(ArrowDirection), typeof(OverflowPrimitive),
				new RadElementPropertyMetadata(ArrowDirection.Down, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        /// Gets or sets the arrow direction. The possible values are contained in the
        /// ArrowDirection enumeration: up, left, right, and bottom.
        /// </summary>
		[RadPropertyDefaultValue("ArrowDirection", typeof(OverflowPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public ArrowDirection Direction
        {
            get { return (ArrowDirection)this.GetValue(ArrowDirectionProperty); }
            set { this.SetValue(ArrowDirectionProperty, value); }
        }

		public static readonly RadProperty ShadowColorProperty =
			RadProperty.Register("ShadowColor", typeof(Color), typeof(OverflowPrimitive),
				new RadElementPropertyMetadata(Color.White, ElementPropertyOptions.AffectsDisplay));
		[RadPropertyDefaultValue("ShadowColor", typeof(OverflowPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
		public Color ShadowColor
		{
			get { return (Color)this.GetValue(ShadowColorProperty); }
			set { this.SetValue(ShadowColorProperty, value); }
		}

        public static readonly RadProperty DrawArrowProperty =
            RadProperty.Register("DrawArrow", typeof(bool), typeof(OverflowPrimitive),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));
        [RadPropertyDefaultValue("DrawArrow", typeof(OverflowPrimitive)), Category(RadDesignCategory.AppearanceCategory)]
        public bool DrawArrow
        {
            get { return (bool)this.GetValue(DrawArrowProperty); }
            set { this.SetValue(DrawArrowProperty, value); }
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            if (!this.DrawArrow)
            {
                return;
            }

            Rectangle rect = new Rectangle(Point.Empty, this.Size);

            // Draw only in the four "clean" cases
            // Combinations of direction are not allowed
            graphics.ChangeSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.AntiAlias);
            switch (this.Direction)
            {
                case ArrowDirection.Left:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(rect.Width - 3, 0),
                            new Point(0, rect.Height / 2),
                            new Point(rect.Width - 3, rect.Height)
                        });

					graphics.DrawLine(this.BackColor, rect.Width - 1, 0, rect.Width - 1, rect.Height - 1);
					graphics.DrawLine(this.ForeColor, rect.Width - 2, 0, rect.Width - 2, rect.Height);
                    break;
                case ArrowDirection.Right:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(3, 0),
                            new Point(rect.Width, rect.Height / 2),
                            new Point(3, rect.Height)
                        });

					graphics.DrawLine(this.ForeColor, 1, 0, 1, rect.Height);
                    break;
                case ArrowDirection.Up:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(0, rect.Height - 3),
                            new Point(rect.Width / 2, 0),
                            new Point(rect.Width, rect.Height - 3)
                        });

					graphics.DrawLine(this.ForeColor, rect.Width/2, rect.Height - 3, 0, rect.Width );
					graphics.DrawLine(this.ForeColor, 0, rect.Height - 1, rect.Width, rect.Height - 1);
					graphics.DrawLine(this.BackColor, 1, rect.Height - 2, rect.Width - 1, rect.Height - 2);
					break;
                case ArrowDirection.Down:
                    graphics.FillPolygon(this.ShadowColor,
                        new Point[] {
                            new Point(0, 5),
                            new Point((rect.Width - 1) / 2 + 1, rect.Height),
                            new Point(rect.Width, 5)
                        });

                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(0, 4),
                            new Point((rect.Width - 1) / 2, rect.Height - 1),
                            new Point(rect.Width - 1, 4)
                        });

					graphics.DrawLine(this.ForeColor, 0, 1, rect.Width - 1, 1);
                    graphics.DrawLine(this.ShadowColor, 1, 2, rect.Width, 2);
					break;
            }
            graphics.RestoreSmoothingMode();
        }

		public override Size GetPreferredSizeCore(Size proposedSize)
        {
            int width;
            int height;
			if (this.Direction == ArrowDirection.Left ||
				this.Direction == ArrowDirection.Right)
			{
				width = Math.Max(MinHorizontalSize.Width, proposedSize.Width);
				height = Math.Max(MinHorizontalSize.Height, proposedSize.Height);
			}
			else
			{
				width = Math.Max(MinVerticalSize.Width, proposedSize.Width);
				height = Math.Max(MinVerticalSize.Height, proposedSize.Height);
			}

			return new Size(width, height);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            // Ensure that Measure() is called for all children
            base.MeasureOverride(availableSize);

            int width;
            int height;
            
            if (this.Direction == ArrowDirection.Left ||
                this.Direction == ArrowDirection.Right)
            {
                width = (int)Math.Max(MinHorizontalSize.Width, availableSize.Width);
                height = (int)Math.Max(MinHorizontalSize.Height, availableSize.Height);
            }
            else
            {
                width = (int)Math.Max(MinVerticalSize.Width, availableSize.Width);
                height = (int)Math.Max(MinVerticalSize.Height, availableSize.Height);
            }

            return new Size(width, height);
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == ArrowPrimitive.DirectionProperty)
			{
				if (this.Direction == ArrowDirection.Left ||
					this.Direction == ArrowDirection.Right)
				{
					if (this.AutoSize)
						this.MinSize = MinHorizontalSize;
					else
						this.Size = MinHorizontalSize;
				}
				else
				{
					if (this.AutoSize)
						this.MinSize = MinVerticalSize;
					else
						this.Size = MinVerticalSize;
				}
			}
			base.OnPropertyChanged(e);
		}

	}
}
