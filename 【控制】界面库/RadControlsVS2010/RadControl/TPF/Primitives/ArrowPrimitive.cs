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
    /// <summary>Represents an arrow that is drawn on the screen.
    /// <para>
    /// Extends %BasePrimitive:Telerik.WinControls.Primitives.BasePrimitive%. 
    /// </para> 
    ///</summary>
    public class ArrowPrimitive : BasePrimitive
    {        
        public static readonly Size MinHorizontalSize;
        public static readonly Size MinVerticalSize;

        #region Contructors
        ///////////////////////////////////////////////////////////////////////

        /// <summary><para>Initializes a new instance of the ArrowPrimitive class.</para></summary>
        static ArrowPrimitive()
        {
            MinHorizontalSize = new Size(4, 8);
            MinVerticalSize = new Size(8, 4);
        }

        public ArrowPrimitive()
        {
        }

        /// <summary>
        /// 	<para>Initializes a new instance of the ArrowPrimitive class using the
        ///     ArrowDirection enumeration. Possible directions are up, bottom, left, and
        ///     right.</para>
        /// </summary>
        public ArrowPrimitive(ArrowDirection arrowDirection)
        {
            this.Direction = arrowDirection;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.Direction = ArrowDirection.Down;
            this.ResetSizesByDirection(ArrowDirection.Down, true);
        }

        #endregion

        #region Properties

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

		public static readonly RadProperty DirectionProperty =
			RadProperty.Register("Direction", typeof(ArrowDirection), typeof(ArrowPrimitive),
				new RadElementPropertyMetadata(ArrowDirection.Down, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout 
			| ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets the arrow direction. The possible values are contained in the
        /// ArrowDirection enumeration: up, left, right, and bottom.
        /// </summary>
        [RadPropertyDefaultValue("Direction", typeof(ArrowPrimitive)), Description(RadDesignCategory.AppearanceCategory)]
        public ArrowDirection Direction
        {
            get 
            { 
                return (ArrowDirection)this.GetValue(DirectionProperty); 
            }
            set 
            {
                 this.SetValue(DirectionProperty, value);
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////
        /// <summary>Draws the primitive on the screen.</summary>
        public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
        {
            Rectangle rect = new Rectangle(Point.Empty, this.Size);
            
			// Draw only in the four "clean" cases
            // Combinations of direction are not allowed
            graphics.ChangeSmoothingMode(System.Drawing.Drawing2D.SmoothingMode.None);
            
            //in Up and Down direction we need to add one-pixel correction to the GDI+ FillPolygon method
            switch (this.Direction)
            {
                case ArrowDirection.Left:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(rect.Width, rect.Top),
                            new Point(rect.Left, rect.Height / 2),
                            new Point(rect.Width, rect.Height)
                        });
                    break;
                case ArrowDirection.Right:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(rect.Left, rect.Top),
                            new Point(rect.Width, rect.Height / 2),
                            new Point(rect.Left, rect.Height)
                        });
                    break;
                case ArrowDirection.Up:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(rect.Left, rect.Height),
                            new Point(rect.Width / 2, rect.Top - 1),
                            new Point(rect.Width, rect.Height)
                        });
                    break;
                case ArrowDirection.Down:
                    graphics.FillPolygon(this.ForeColor,
                        new Point[] {
                            new Point(rect.Left + 1, rect.Top),
                            new Point(rect.Width / 2, rect.Height),
                            new Point(rect.Width, rect.Top)
                        });
                    break;
            }
            graphics.RestoreSmoothingMode();
        }

        /// <summary>
        /// 	<para>Retrieves the size of a rectangular area into which a primitive can be
        ///     fitted. If proposed size is too small, the minimal size is retrieved.</para>
        /// </summary>
		public override Size GetPreferredSizeCore(Size proposedSize)
        {
            int width;
            int height;
            if (this.Direction == ArrowDirection.Left ||
                this.Direction == ArrowDirection.Right)
            {
				width = Math.Max(MinHorizontalSize.Width, proposedSize.Width) ;
				height = Math.Max(MinHorizontalSize.Height, proposedSize.Height);
            }
            else
            {
				width = Math.Max(MinVerticalSize.Width, proposedSize.Width);
				height = Math.Max(MinVerticalSize.Height, proposedSize.Height);				
            }

			if (this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren)
			{
				width += this.Padding.Left + this.Padding.Right;
				height += this.Padding.Top + this.Padding.Bottom;
			}

        	return new Size(width, height);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            // Ensure that Measure() is called for all children
            base.MeasureOverride(availableSize);

            Size res = Size.Empty;
            if (this.Direction == ArrowDirection.Left ||
                this.Direction == ArrowDirection.Right)
            {
                res = MinHorizontalSize;
            }
            else
            {
                res = MinVerticalSize;
            }

            res = Size.Add(res, this.Padding.Size);
            return res;
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.AutoSizeProperty)
            {
                this.Size = this.MinSize;
            }
            else if (e.Property == ArrowPrimitive.DirectionProperty)
            {
                this.ResetSizesByDirection(this.Direction, this.AutoSize);
            }
		}

        private void ResetSizesByDirection(ArrowDirection newDirection, bool setMinSize)
        {
            this.SuspendPropertyNotifications();

            if (newDirection == ArrowDirection.Left ||
                newDirection == ArrowDirection.Right)
            {
                if (setMinSize)
                {
                    this.MinSize = MinHorizontalSize;
                }
                else
                {
                    this.Size = MinHorizontalSize;
                }
            }
            else
            {
                if (setMinSize)
                {
                    this.MinSize = MinVerticalSize;
                }
                else
                {
                    this.Size = MinVerticalSize;
                }
            }

            this.ResumePropertyNotifications();
        }

    }
}
