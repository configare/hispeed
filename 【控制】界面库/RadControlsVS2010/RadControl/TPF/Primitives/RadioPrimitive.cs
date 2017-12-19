using System;
using System.Collections.Generic;
using System.Text;
//using Telerik.WinControls.Design;
using Telerik.WinControls.Paint;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.Primitives
{
	/// <summary><para>Represents a check box primitive that is drawn on the screen.</para></summary>
	public class RadioPrimitive : FillPrimitive
    {
        #region Dependency properties

        public static RadProperty UseParentShapeProperty = RadProperty.Register("UseParentShape", typeof(bool),
           typeof(RadioPrimitive), new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.MinSize = new Size(13, 13);
            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.BackColor = Color.Black;
            this.BackColor2 = Color.Black;
            this.ForeColor = Color.Black;
        }

        #endregion

        #region Properties

        /// <summary>
		/// Gets the minimal size of the check primitive. Its default value is Size(13,
		/// 13).
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Please use the MinSize property instead.")]
		public Size MinimalSize
		{
			get
			{
				return this.MinSize;
			}
        }

        [RadPropertyDefaultValue("UseParentShape", typeof(RadioPrimitive))]
        public bool UseParentShape
        {
            get
            {
                return (bool)this.GetValue(UseParentShapeProperty);
            }
            set
            {
                this.SetValue(UseParentShapeProperty, value);
            }
        }

        #endregion

        #region Painting

		public override void PaintPrimitive(IGraphics graphics, float angle, SizeF scale)
		{
            if (!CheckForValidParameters())
            {
                return;
            }

            int colorsMaxValue = 4;
            Color[] colorStops = new Color[Math.Min(Math.Max(this.NumberOfColors, 1), colorsMaxValue)];
            float[] colorOffsets = new float[Math.Min(Math.Max(this.NumberOfColors, 1), colorsMaxValue)];
            Rectangle rect = GetClientRect();                
            GraphicsPath graphPath = CreatePath(rect);

            if (this.NumberOfColors > 0)
            {
                colorStops[0] = BackColor;
                colorOffsets[0] = 0f;
            }

            if (this.NumberOfColors > 1)
            {
                colorStops[1] = BackColor2;
                colorOffsets[colorStops.Length - 1] = 1f;
            }

            if (this.NumberOfColors > 2)
            {
                colorStops[2] = BackColor3;
                colorOffsets[1] = this.GradientPercentage;
            }

            if (this.NumberOfColors > 3)
            {
                colorStops[3] = BackColor4;
                colorOffsets[2] = this.GradientPercentage2;
            }

            PaintPrimitiveCore(graphics, rect, colorStops, colorOffsets, graphPath);            
        }

        protected virtual bool CheckForValidParameters()
        {
            if (BackColor.A == 0)
            {
                if (NumberOfColors <= 1)
                {
                    return false;
                }

                if (BackColor2.A == 0)
                {
                    if (NumberOfColors <= 2)
                    {
                        return false;
                    }

                    if (BackColor3.A == 0)
                    {
                        if (NumberOfColors <= 3)
                        {
                            return false;
                        }

                        if (BackColor4.A == 0)
                        {
                            return false;
                        }
                    }
                }
            }

            if ((this.Size.Width <= 0) || (this.Size.Height <= 0))
            {
                return false;
            }

            return true;
        }

        protected virtual GraphicsPath CreatePath(Rectangle rect)
        {
            if (this.UseParentShape && this.Parent != null && this.Parent.Shape != null)
            {
                return this.Parent.Shape.CreatePath(rect);
            }
            return null;
        }

        protected virtual void PaintPrimitiveCore(IGraphics g, Rectangle rect, Color[] colorStops, float[] colorOffsets, GraphicsPath graphPath)
        {
            if (graphPath != null)
            {
                g.FillPath(colorStops, colorOffsets,
                    GradientAngle, GradientPercentage, GradientPercentage2,
                    rect,
                    graphPath);
                g.DrawPath(graphPath, this.ForeColor, PenAlignment.Center, 1);
                graphPath.Dispose();
            }
            else
            {
                g.FillGradientRectangle(
                    rect,
                    colorStops, colorOffsets, this.GradientStyle,
                    GradientAngle, GradientPercentage, GradientPercentage2);
                g.DrawRectangle(rect, ForeColor);
            }
        }

        protected virtual Rectangle GetClientRect()
        {
            Rectangle rect = new Rectangle(Point.Empty, this.Size);
            return new Rectangle(rect.Width / 4, rect.Height / 4, rect.Width / 2, rect.Height / 2);
        }

        #endregion

        #region Layout

        public override Size GetPreferredSizeCore(Size proposedSize)
		{
			if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
			{
				return proposedSize;
			}
			else
			{
				return this.MinSize;
			}
		}
        
        #endregion
    }
}