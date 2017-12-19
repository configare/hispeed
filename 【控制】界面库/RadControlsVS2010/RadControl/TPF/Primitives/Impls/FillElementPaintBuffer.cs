using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.Primitives
{
	internal class FillElementPaintBuffer
	{
		private readonly IFillElement fillElement;
		private readonly FillRepository bitmapRepository;
		private Bitmap paintBuffer;
		private Graphics graphics = null;
		private int cachedPrimitiveHash = -1;
	    private int shapeHash;
        private static Dictionary<int, object> CacheRelatedPropertyNames;

        static FillElementPaintBuffer()
        {
            CacheRelatedPropertyNames = new Dictionary<int, object>(30);
            CacheRelatedPropertyNames.Add("Bounds".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("NumberOfColors".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("BackColor".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("BackColor2".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("BackColor3".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("BackColor4".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("GradientPercentage".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("GradientPercentage2".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("GradientStyle".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("GradientAngle".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("Shape".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("GradientAngleCorrection".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("AngleTransform".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("SmoothingMode".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("VisualState".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("FillPadding".GetHashCode(), null);
            CacheRelatedPropertyNames.Add("ItemContentOrientation".GetHashCode(), null);
        }

	    public FillElementPaintBuffer(IFillElement fillElement, FillRepository fillRepository)
		{
			this.fillElement = fillElement;
			this.bitmapRepository = fillRepository;
		}

		public void InvalidateCachedPrimitiveHash(RadProperty property)
		{
            if (CacheRelatedPropertyNames.ContainsKey(property.NameHash))
            {
                this.cachedPrimitiveHash = -1;
            }
		}

        public void InvalidateCachedPrimitiveHash()
        {
            this.cachedPrimitiveHash = -1;
        }

	    public virtual bool ShouldUsePaintBuffer()
		{
            int colorsNum = this.fillElement.NumberOfColors;
			//do not use paint buffer for solid fills
			if (this.fillElement.GradientStyle == GradientStyles.Solid ||
                colorsNum <= 1 || (colorsNum == 2 && this.fillElement.BackColor == this.fillElement.BackColor2))
			{
				return false;
			}

			return true;
		}

		public bool PaintFromBuffer(IGraphics g, SizeF scale, Size desired)
		{
			Size scaledSize = this.GetScaled(desired, scale);
			int primitiveHash = this.GetPrimitiveHash(desired);
			this.paintBuffer = this.bitmapRepository.GetBitmapBySizeAndHash(scaledSize, primitiveHash);

			if (this.paintBuffer == null)
			{
				if (scaledSize.Width != 0 && scaledSize.Height != 0)
				{
					this.paintBuffer = new Bitmap(scaledSize.Width, scaledSize.Height);
					this.bitmapRepository.AddNewBitmap(scaledSize, primitiveHash, this.paintBuffer);
					return false;
				}
				else
				{
					// Simulate that the bitmap is drawn (although it is null)
					return true;
				}
			}

            Graphics gdiGraphics = g.UnderlayGraphics as Graphics;
			Matrix original = null;
			if (scale.Width != 1f || scale.Height != 1f)
			{
                original = gdiGraphics.Transform;
                gdiGraphics.ScaleTransform(1 / scale.Width, 1 / scale.Height);
			}
			g.DrawBitmap(this.paintBuffer, 0, 0);
			if (original != null)
			{
                gdiGraphics.Transform = original;
			}

			return true;
		}

		public void SetGraphics(IGraphics g, SizeF scale)
		{
			RadGdiGraphics gdiGraphics = (RadGdiGraphics)g;
			this.graphics = gdiGraphics.Graphics;

			Graphics newGraphics = Graphics.FromImage(this.paintBuffer);
			newGraphics.ScaleTransform(scale.Width, scale.Height);

			//Fixed issue with smoothing mode and FillPath by changing SmoothingMode
			//TODO: should we copy other graphics state properties?

			//newGraphics.TextRenderingHint = gdiGraphics.Graphics.TextRenderingHint;
			//newGraphics.InterpolationMode = gdiGraphics.Graphics.InterpolationMode;
			//newGraphics.TextContrast = gdiGraphics.Graphics.TextContrast;

			newGraphics.SmoothingMode = gdiGraphics.Graphics.SmoothingMode;

			((RadGdiGraphics)g).Graphics = newGraphics;
		}

		public void ResetGraphics(IGraphics g, SizeF scale)
		{
			if (this.graphics == null)
			{
				return;
			}

			((RadGdiGraphics)g).Graphics.Dispose();
			((RadGdiGraphics)g).Graphics = graphics;
			//TelerikPaintHelper.CopyImageToGraphics(graphics, this.paintBuffer);

			Matrix original = null;
			//g.ChangeOpacity(1d);
			if (scale.Width != 1f || scale.Height != 1f)
			{
				original = ((RadGdiGraphics)g).Graphics.Transform;
				((RadGdiGraphics)g).Graphics.ScaleTransform(1 / scale.Width, 1 / scale.Height);
			}

			g.DrawBitmap(this.paintBuffer, 0, 0);

			if (original != null)
			{
				((RadGdiGraphics)g).Graphics.Transform = original;
			}
		}

		public void RemoveBitmapsBySize(Size size, SizeF scale)
		{
			this.bitmapRepository.RemoveBitmapsBySize(this.GetScaled(size, scale));
		}

		private int GetPrimitiveHash(Size desired)
		{
            if (this.cachedPrimitiveHash != -1)
            {
                return this.cachedPrimitiveHash;
            }

			//BackColor, BackColor2, BackColor3, BackColor4, this.GradientPercentage);
			//	g.FillGradientRectangle(rect, colorStops, colorOffsets, GradientStyle, GradientAngle, this.GradientPercentage, this.GradientPercentage2);
			StringBuilder hash = new StringBuilder();
			hash.Append(desired.Width);
			hash.Append(";");
            hash.Append(desired.Height);
			hash.Append(";");
			hash.Append(this.fillElement.NumberOfColors);
			hash.Append(";");
			hash.Append(this.fillElement.BackColor.ToArgb().ToString());
			hash.Append(this.fillElement.BackColor2.ToArgb().ToString());
			hash.Append(this.fillElement.BackColor3.ToArgb().ToString());
			hash.Append(this.fillElement.BackColor4.ToArgb().ToString());
			hash.Append(";");
			hash.Append(this.fillElement.GradientPercentage);
			hash.Append(";");
			hash.Append(this.fillElement.GradientPercentage2);
			hash.Append(";");
			hash.Append(this.GetGradientStyleAsString());
			hash.Append(";");
			hash.Append(this.fillElement.GradientAngle);
            hash.Append(this.shapeHash);

			this.cachedPrimitiveHash = hash.ToString().GetHashCode();

			return this.cachedPrimitiveHash;
		}

		private string GetGradientStyleAsString()
		{
			string gradientStyle = Enum.GetName(typeof(GradientStyles), this.fillElement.GradientStyle);
			if (gradientStyle == null)
			{
				gradientStyle = string.Empty;
			}

			return gradientStyle;
		}

		private Size GetScaled(Size original, SizeF scale)
		{
			return new Size((int)(original.Width * scale.Width), (int)(original.Height * scale.Height));
		}

	    public void SetShapeHash(int shapeHash)
	    {
            if (this.shapeHash != shapeHash)
            {
                this.shapeHash = shapeHash;
                this.InvalidateCachedPrimitiveHash();                
            }
	    }
	}
}
