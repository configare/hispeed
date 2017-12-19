using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.Layouts
{
    ///<exclude/> 
    public class LayoutUtils
    {
        // Methods
        static LayoutUtils()
        {
            InfinitySize = new SizeF(float.PositiveInfinity, float.PositiveInfinity);
            MaxSizeF = new SizeF(float.MaxValue, float.MaxValue);
            LayoutUtils.MaxSize = new Size(0x7fffffff, 0x7fffffff);
            LayoutUtils.MaxRectangle = new Rectangle(0, 0, 0x7fffffff, 0x7fffffff);
            AnchorStyles[] stylesArray1 = new AnchorStyles[] { AnchorStyles.Left | AnchorStyles.Top, AnchorStyles.Right | (AnchorStyles.Left | AnchorStyles.Top), AnchorStyles.Right | (AnchorStyles.Left | AnchorStyles.Bottom), AnchorStyles.Left | (AnchorStyles.Bottom | AnchorStyles.Top), AnchorStyles.Right | (AnchorStyles.Bottom | AnchorStyles.Top), AnchorStyles.Right | (AnchorStyles.Left | (AnchorStyles.Bottom | AnchorStyles.Top)) } ;
            LayoutUtils.dockingToAnchor = stylesArray1;
            LayoutUtils.TestString = "j^";
        }

        public LayoutUtils()
        {
        }

        public static Padding RotateMargin(Padding margin, int angle)
        {
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            switch (angle)
            {
                case 90:
                    margin = new Padding(margin.Bottom, margin.Left, margin.Top, margin.Right);
                    break;
                case 180:
                    margin = new Padding(margin.Right, margin.Bottom, margin.Left, margin.Top);
                    break;
                case 270:
                    margin = new Padding(margin.Top, margin.Right, margin.Bottom, margin.Left);
                    break;
            }

            return margin;
        }

        public static Size AddAlignedRegion(Size textSize, Size imageSize, TextImageRelation relation)
        {
			if (relation == TextImageRelation.Overlay)
			{
				return new Size(Math.Max(textSize.Width, imageSize.Width), Math.Max(textSize.Height, imageSize.Height));
			}
			else
			{
				return LayoutUtils.AddAlignedRegionCore(textSize, imageSize, LayoutUtils.IsVerticalRelation(relation));
			}
        }

		public static SizeF AddAlignedRegion(SizeF textSize, SizeF imageSize, TextImageRelation relation)
		{
			if (relation == TextImageRelation.Overlay)
			{
				return new SizeF(Math.Max(textSize.Width, imageSize.Width), Math.Max(textSize.Height, imageSize.Height));
			}
			else
			{
				return LayoutUtils.AddAlignedRegionCore(textSize, imageSize, LayoutUtils.IsVerticalRelation(relation));
			}
		}

        public static Size AddAlignedRegionCore(Size currentSize, Size contentSize, bool vertical)
        {
            if (vertical)
            {
                currentSize.Width = Math.Max(currentSize.Width, contentSize.Width);
                currentSize.Height += contentSize.Height;
                return currentSize;
            }
            currentSize.Width += contentSize.Width;
            currentSize.Height = Math.Max(currentSize.Height, contentSize.Height);
            return currentSize;
        }

		public static SizeF AddAlignedRegionCore(SizeF currentSize, SizeF contentSize, bool vertical)
		{
			if (vertical)
			{
				currentSize.Width = Math.Max(currentSize.Width, contentSize.Width);
				currentSize.Height += contentSize.Height;
				return currentSize;
			}
			currentSize.Width += contentSize.Width;
			currentSize.Height = Math.Max(currentSize.Height, contentSize.Height);
			return currentSize;
		}

        public static Rectangle Align(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            return LayoutUtils.VAlign(alignThis, LayoutUtils.HAlign(alignThis, withinThis, align), align);
        }

		public static RectangleF Align(SizeF alignThis, RectangleF withinThis, ContentAlignment align)
		{
			return LayoutUtils.VAlign(alignThis, LayoutUtils.HAlign(alignThis, withinThis, align), align);
		}

        public static Rectangle Align(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
        {
            return LayoutUtils.VAlign(alignThis, LayoutUtils.HAlign(alignThis, withinThis, anchorStyles), anchorStyles);
        }

        public static Rectangle AlignAndStretch(Size fitThis, Rectangle withinThis, AnchorStyles anchorStyles)
        {
            return LayoutUtils.Align(LayoutUtils.Stretch(fitThis, withinThis.Size, anchorStyles), withinThis, anchorStyles);
        }

        public static bool AreWidthAndHeightLarger(Size size1, Size size2)
        {
            if (size1.Width >= size2.Width)
            {
                return (size1.Height >= size2.Height);
            }
            return false;
        }

        public static Padding ClampNegativePaddingToZero(Padding padding)
        {
            if (padding.All < 0)
            {
                padding.Left = Math.Max(0, padding.Left);
                padding.Top = Math.Max(0, padding.Top);
                padding.Right = Math.Max(0, padding.Right);
                padding.Bottom = Math.Max(0, padding.Bottom);
            }
            return padding;
        }

        public static int ContentAlignmentToIndex(ContentAlignment alignment)
        {
            int num1 = LayoutUtils.xContentAlignmentToIndex(((int) alignment) & 15);
            int num2 = LayoutUtils.xContentAlignmentToIndex((((int) alignment) >> 4) & 15);
            int num3 = LayoutUtils.xContentAlignmentToIndex((((int) alignment) >> 8) & 15);
            int num4 = (((((num2 != 0) ? 4 : 0) | ((num3 != 0) ? 8 : 0)) | num1) | num2) | num3;
            num4--;
            return num4;
        }

        public static Size ConvertZeroToUnbounded(Size size)
        {
            if (size.Width == 0)
            {
                size.Width = 0x7fffffff;
            }
            if (size.Height == 0)
            {
                size.Height = 0x7fffffff;
            }
            return size;
        }

        public static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

        public static RectangleF DeflateRect(RectangleF rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;

            return rect;
        }

        public static void ExpandRegionsToFillBounds(Rectangle bounds, AnchorStyles region1Align, ref Rectangle region1, ref Rectangle region2)
        {
            switch (region1Align)
            {
                case AnchorStyles.Top:
                {
                    region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Bottom);
                    region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Top);
                    return;
                }
                case AnchorStyles.Bottom:
                {
                    region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Top);
                    region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Bottom);
                    return;
                }
                case (AnchorStyles.Bottom | AnchorStyles.Top):
                {
                    return;
                }
                case AnchorStyles.Left:
                {
                    region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Right);
                    region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Left);
                    return;
                }
                case AnchorStyles.Right:
                {
                    region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Left);
                    region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Right);
                    return;
                }
            }
        }

		public static void ExpandRegionsToFillBounds(RectangleF bounds, AnchorStyles region1Align, ref RectangleF region1, ref RectangleF region2)
		{
			switch (region1Align)
			{
				case AnchorStyles.Top:
				{
					region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Bottom);
					region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Top);
					return;
				}
				case AnchorStyles.Bottom:
				{
					region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Top);
					region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Bottom);
					return;
				}
				case (AnchorStyles.Bottom | AnchorStyles.Top):
				{
					return;
				}
				case AnchorStyles.Left:
				{
					region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Right);
					region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Left);
					return;
				}
				case AnchorStyles.Right:
				{
					region1 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region1, AnchorStyles.Left);
					region2 = LayoutUtils.SubstituteSpecifiedBounds(bounds, region2, AnchorStyles.Right);
					return;
				}
			}
		}

        public static Padding FlipPadding(Padding padding)
        {
            if (padding.All == -1)
            {
                int num1 = padding.Top;
                padding.Top = padding.Left;
                padding.Left = num1;
                num1 = padding.Bottom;
                padding.Bottom = padding.Right;
                padding.Right = num1;
            }
            return padding;
        }

        public static Point FlipPoint(Point point)
        {
            int num1 = point.X;
            point.X = point.Y;
            point.Y = num1;
            return point;
        }

        public static Rectangle FlipRectangle(Rectangle rect)
        {
            rect.Location = LayoutUtils.FlipPoint(rect.Location);
            rect.Size = LayoutUtils.FlipSize(rect.Size);
            return rect;
        }

        public static Rectangle FlipRectangleIf(bool condition, Rectangle rect)
        {
            if (!condition)
            {
                return rect;
            }
            return LayoutUtils.FlipRectangle(rect);
        }

        public static Size FlipSize(Size size)
        {
            int num1 = size.Width;
            size.Width = size.Height;
            size.Height = num1;
            return size;
        }

		public static SizeF FlipSize(SizeF size)
		{
			float num1 = size.Width;
			size.Width = size.Height;
			size.Height = num1;
			return size;
		}

        public static Size FlipSizeIf(bool condition, Size size)
        {
            if (!condition)
            {
                return size;
            }
            return LayoutUtils.FlipSize(size);
        }

		public static SizeF FlipSizeIf(bool condition, SizeF size)
		{
			if (!condition)
			{
				return size;
			}
			return LayoutUtils.FlipSize(size);
		}

        private static AnchorStyles GetOppositeAnchor(AnchorStyles anchor)
        {
            AnchorStyles styles1 = AnchorStyles.None;
            if (anchor != AnchorStyles.None)
            {
                for (int num1 = 1; num1 <= 8; num1 = num1 << 1)
                {
                    switch ((anchor & ((AnchorStyles) num1)))
                    {
                        case AnchorStyles.None:
                        case (AnchorStyles.Bottom | AnchorStyles.Top):
                        case (AnchorStyles.Left | AnchorStyles.Top):
                        case (AnchorStyles.Left | AnchorStyles.Bottom):
                        case (AnchorStyles.Left | (AnchorStyles.Bottom | AnchorStyles.Top)):
                        {
                            goto Label_0051;
                        }
                        case AnchorStyles.Top:
                        {
                            styles1 |= AnchorStyles.Bottom;
                            goto Label_0051;
                        }
                        case AnchorStyles.Bottom:
                        {
                            break;
                        }
                        case AnchorStyles.Left:
                        {
                            styles1 |= AnchorStyles.Right;
                            goto Label_0051;
                        }
                        case AnchorStyles.Right:
                        {
                            styles1 |= AnchorStyles.Left;
                            goto Label_0051;
                        }
                        default:
                        {
                            goto Label_0051;
                        }
                    }
                    styles1 |= AnchorStyles.Top;
                Label_0051:;
                }
            }
            return styles1;
        }

        public static TextImageRelation GetOppositeTextImageRelation(TextImageRelation relation)
        {
            return (TextImageRelation) LayoutUtils.GetOppositeAnchor((AnchorStyles) relation);
        }

		/*
        internal static AnchorStyles GetUnifiedAnchor(ITelerikArrangedElement element)
        {
            DockStyle style1 = DefaultLayout.GetDock(element);
            if (style1 != DockStyle.None)
            {
                return LayoutUtils.dockingToAnchor[(int) style1];
            }
            return DefaultLayout.GetAnchor(element);
        }
		 */

		public static bool IsRightAlignment(ContentAlignment align)
		{
			return ((align & LayoutUtils.AnyRight) != ((ContentAlignment) 0));
		}

        public static Rectangle HAlign(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & LayoutUtils.AnyRight) != ((ContentAlignment) 0))
            {
                withinThis.X += (withinThis.Width - alignThis.Width);
            }
            else if ((align & LayoutUtils.AnyCenter) != ((ContentAlignment) 0))
            {
                withinThis.X += ((withinThis.Width - alignThis.Width) / 2);
            }
            withinThis.Width = alignThis.Width;
            return withinThis;
        }

		public static RectangleF HAlign(SizeF alignThis, RectangleF withinThis, ContentAlignment align)
		{
			if ((align & LayoutUtils.AnyRight) != ((ContentAlignment)0))
			{
				withinThis.X += (withinThis.Width - alignThis.Width);
			}
			else if ((align & LayoutUtils.AnyCenter) != ((ContentAlignment)0))
			{
				withinThis.X += ((withinThis.Width - alignThis.Width) / 2);
			}
			withinThis.Width = alignThis.Width;
			return withinThis;
		}

        public static Rectangle HAlign(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
        {
            if ((anchorStyles & AnchorStyles.Right) != AnchorStyles.None)
            {
                withinThis.X += (withinThis.Width - alignThis.Width);
            }
            else if ((anchorStyles == AnchorStyles.None) || ((anchorStyles & (AnchorStyles.Right | AnchorStyles.Left)) == AnchorStyles.None))
            {
                withinThis.X += ((withinThis.Width - alignThis.Width) / 2);
            }
            withinThis.Width = alignThis.Width;
            return withinThis;
        }

        public static Rectangle InflateRect(Rectangle rect, Padding padding)
        {
            rect.X -= padding.Left;
            rect.Y -= padding.Top;
            rect.Width += padding.Horizontal;
            rect.Height += padding.Vertical;
            return rect;
        }

        public static Size IntersectSizes(Size a, Size b)
        {
            return new Size(Math.Min(a.Width, b.Width), Math.Min(a.Height, b.Height));
        }

        public static bool IsHorizontalAlignment(ContentAlignment align)
        {
            return !LayoutUtils.IsVerticalAlignment(align);
        }

        public static bool IsHorizontalRelation(TextImageRelation relation)
        {
            return ((relation & (TextImageRelation.TextBeforeImage | TextImageRelation.ImageBeforeText)) != TextImageRelation.Overlay);
        }

        public static bool IsIntersectHorizontally(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.IntersectsWith(rect2))
            {
                if ((rect1.X <= rect2.X) && ((rect1.X + rect1.Width) >= (rect2.X + rect2.Width)))
                {
                    return true;
                }
                if ((rect2.X <= rect1.X) && ((rect2.X + rect2.Width) >= (rect1.X + rect1.Width)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsIntersectVertically(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.IntersectsWith(rect2))
            {
                if ((rect1.Y <= rect2.Y) && ((rect1.Y + rect1.Width) >= (rect2.Y + rect2.Width)))
                {
                    return true;
                }
                if ((rect2.Y <= rect1.Y) && ((rect2.Y + rect2.Width) >= (rect1.Y + rect1.Width)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsVerticalAlignment(ContentAlignment align)
        {
            return ((align & (ContentAlignment.BottomCenter | ContentAlignment.TopCenter)) != ((ContentAlignment) 0));
        }

        public static bool IsVerticalRelation(TextImageRelation relation)
        {
            return ((relation & (TextImageRelation.TextAboveImage | TextImageRelation.ImageAboveText)) != TextImageRelation.Overlay);
        }

        public static bool IsZeroWidthOrHeight(Rectangle rectangle)
        {
            if (rectangle.Width != 0)
            {
                return (rectangle.Height == 0);
            }
            return true;
        }

        public static bool IsZeroWidthOrHeight(Size size)
        {
            if (size.Width != 0)
            {
                return (size.Height == 0);
            }
            return true;
        }

        public static Size OldGetLargestStringSizeInCollection(Font font, ICollection objects)
        {
            Size size1 = Size.Empty;
            if (objects != null)
            {
                foreach (object obj1 in objects)
                {
                    Size size2 = TextRenderer.MeasureText(obj1.ToString(), font, new Size(0x7fff, 0x7fff), TextFormatFlags.SingleLine);
                    size1.Width = Math.Max(size1.Width, size2.Width);
                    size1.Height = Math.Max(size1.Height, size2.Height);
                }
            }
            return size1;
        }

        public static Rectangle RTLTranslate(Rectangle bounds, Rectangle withinBounds)
        {
            bounds.X = withinBounds.Width - bounds.Right;
            return bounds;
        }

        public static RectangleF RTLTranslate(RectangleF bounds, RectangleF withinBounds)
        {
            bounds.X = withinBounds.Width - bounds.Right;
            return bounds;
        }

        public static RectangleF RTLTranslateNonRelative(RectangleF bounds, RectangleF withinBounds)
        {
            bounds.X = withinBounds.Right - bounds.Right + withinBounds.X;
            return bounds;
        }

        public static Rectangle RTLTranslateNonRelative(Rectangle bounds, Rectangle withinBounds)
        {
            bounds.X = withinBounds.Width - bounds.Right + withinBounds.X;
            return bounds;
        }

        public static void SplitRegion(Rectangle bounds, Size specifiedContent, AnchorStyles region1Align, out Rectangle region1, out Rectangle region2)
        {
            Rectangle rectangle1;
            region2 = rectangle1 = bounds;
            region1 = rectangle1;
            switch (region1Align)
            {
                case AnchorStyles.Top:
                {
                    region1.Height = specifiedContent.Height;
                    region2.Y += specifiedContent.Height;
                    region2.Height -= specifiedContent.Height;
                    return;
                }
                case AnchorStyles.Bottom:
                {
                    region1.Y += (bounds.Height - specifiedContent.Height);
                    region1.Height = specifiedContent.Height;
                    region2.Height -= specifiedContent.Height;
                    return;
                }
                case (AnchorStyles.Bottom | AnchorStyles.Top):
                {
                    return;
                }
                case AnchorStyles.Left:
                {
                    region1.Width = specifiedContent.Width;
                    region2.X += specifiedContent.Width;
                    region2.Width -= specifiedContent.Width;
                    return;
                }
                case AnchorStyles.Right:
                {
                    region1.X += (bounds.Width - specifiedContent.Width);
                    region1.Width = specifiedContent.Width;
                    region2.Width -= specifiedContent.Width;
                    return;
                }
            }
        }

		public static void SplitRegion(RectangleF bounds, SizeF specifiedContent, AnchorStyles region1Align, out RectangleF region1, out RectangleF region2)
		{
			RectangleF rectangle1;
			region2 = rectangle1 = bounds;
			region1 = rectangle1;
			switch (region1Align)
			{
				case AnchorStyles.Top:
					{
						region1.Height = specifiedContent.Height;
						region2.Y += specifiedContent.Height;
						region2.Height -= specifiedContent.Height;
						return;
					}
				case AnchorStyles.Bottom:
					{
						region1.Y += (bounds.Height - specifiedContent.Height);
						region1.Height = specifiedContent.Height;
						region2.Height -= specifiedContent.Height;
						return;
					}
				case (AnchorStyles.Bottom | AnchorStyles.Top):
					{
						return;
					}
				case AnchorStyles.Left:
					{
						region1.Width = specifiedContent.Width;
						region2.X += specifiedContent.Width;
						region2.Width -= specifiedContent.Width;
						return;
					}
				case AnchorStyles.Right:
					{
						region1.X += (bounds.Width - specifiedContent.Width);
						region1.Width = specifiedContent.Width;
						region2.Width -= specifiedContent.Width;
						return;
					}
			}
		}

        public static Size Stretch(Size stretchThis, Size withinThis, AnchorStyles anchorStyles)
        {
            Size size1 = new Size(((anchorStyles & (AnchorStyles.Right | AnchorStyles.Left)) == (AnchorStyles.Right | AnchorStyles.Left)) ? withinThis.Width : stretchThis.Width, ((anchorStyles & (AnchorStyles.Bottom | AnchorStyles.Top)) == (AnchorStyles.Bottom | AnchorStyles.Top)) ? withinThis.Height : stretchThis.Height);
            if (size1.Width > withinThis.Width)
            {
                size1.Width = withinThis.Width;
            }
            if (size1.Height > withinThis.Height)
            {
                size1.Height = withinThis.Height;
            }
            return size1;
        }

        public static Size SubAlignedRegion(Size currentSize, Size contentSize, TextImageRelation relation)
        {
			if (relation == TextImageRelation.Overlay)
			{
				return currentSize;
			}
			else
			{
				return LayoutUtils.SubAlignedRegionCore(currentSize, contentSize, LayoutUtils.IsVerticalRelation(relation));
			}
        }

		public static SizeF SubAlignedRegion(SizeF currentSize, SizeF contentSize, TextImageRelation relation)
		{
			if (relation == TextImageRelation.Overlay)
			{
				return currentSize;
			}
			else
			{
				return LayoutUtils.SubAlignedRegionCore(currentSize, contentSize, LayoutUtils.IsVerticalRelation(relation));
			}
		}

        public static Size SubAlignedRegionCore(Size currentSize, Size contentSize, bool vertical)
        {
            if (vertical)
            {
                currentSize.Height -= contentSize.Height;
                return currentSize;
            }
            currentSize.Width -= contentSize.Width;
            return currentSize;
        }

		public static SizeF SubAlignedRegionCore(SizeF currentSize, SizeF contentSize, bool vertical)
		{
			if (vertical)
			{
				currentSize.Height -= contentSize.Height;
				return currentSize;
			}
			currentSize.Width -= contentSize.Width;
			return currentSize;
		}

        private static Rectangle SubstituteSpecifiedBounds(Rectangle originalBounds, Rectangle substitutionBounds, AnchorStyles specified)
        {
            int num1 = ((specified & AnchorStyles.Left) != AnchorStyles.None) ? substitutionBounds.Left : originalBounds.Left;
            int num2 = ((specified & AnchorStyles.Top) != AnchorStyles.None) ? substitutionBounds.Top : originalBounds.Top;
            int num3 = ((specified & AnchorStyles.Right) != AnchorStyles.None) ? substitutionBounds.Right : originalBounds.Right;
            int num4 = ((specified & AnchorStyles.Bottom) != AnchorStyles.None) ? substitutionBounds.Bottom : originalBounds.Bottom;
            return Rectangle.FromLTRB(num1, num2, num3, num4);
        }

		private static RectangleF SubstituteSpecifiedBounds(RectangleF originalBounds, RectangleF substitutionBounds, AnchorStyles specified)
		{
			float num1 = ((specified & AnchorStyles.Left) != AnchorStyles.None) ? substitutionBounds.Left : originalBounds.Left;
			float num2 = ((specified & AnchorStyles.Top) != AnchorStyles.None) ? substitutionBounds.Top : originalBounds.Top;
			float num3 = ((specified & AnchorStyles.Right) != AnchorStyles.None) ? substitutionBounds.Right : originalBounds.Right;
			float num4 = ((specified & AnchorStyles.Bottom) != AnchorStyles.None) ? substitutionBounds.Bottom : originalBounds.Bottom;
			return RectangleF.FromLTRB(num1, num2, num3, num4);
		}

        public static Size UnionSizes(Size a, Size b)
        {
            return new Size(Math.Max(a.Width, b.Width), Math.Max(a.Height, b.Height));
        }

		public static SizeF UnionSizes(SizeF a, SizeF b)
		{
			return new SizeF(Math.Max(a.Width, b.Width), Math.Max(a.Height, b.Height));
		}

        public static Rectangle VAlign(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & LayoutUtils.AnyBottom) != ((ContentAlignment) 0))
            {
                withinThis.Y += (withinThis.Height - alignThis.Height);
            }
			else if ((align & LayoutUtils.AnyMiddle) != ((ContentAlignment) 0))
            {
                withinThis.Y += ((withinThis.Height - alignThis.Height) / 2);
            }
            withinThis.Height = alignThis.Height;
            return withinThis;
        }

		public static RectangleF VAlign(SizeF alignThis, RectangleF withinThis, ContentAlignment align)
		{
			if ((align & LayoutUtils.AnyBottom) != ((ContentAlignment)0))
			{
				withinThis.Y += (withinThis.Height - alignThis.Height);
			}
			else if ((align & LayoutUtils.AnyMiddle) != ((ContentAlignment)0))
			{
				withinThis.Y += ((withinThis.Height - alignThis.Height) / 2);
			}
			withinThis.Height = alignThis.Height;
			return withinThis;
		}

        public static Rectangle VAlign(Size alignThis, Rectangle withinThis, AnchorStyles anchorStyles)
        {
            if ((anchorStyles & AnchorStyles.Bottom) != AnchorStyles.None)
            {
                withinThis.Y += (withinThis.Height - alignThis.Height);
            }
            else if ((anchorStyles == AnchorStyles.None) || ((anchorStyles & (AnchorStyles.Bottom | AnchorStyles.Top)) == AnchorStyles.None))
            {
                withinThis.Y += ((withinThis.Height - alignThis.Height) / 2);
            }
            withinThis.Height = alignThis.Height;
            return withinThis;
        }

        private static byte xContentAlignmentToIndex(int threeBitFlag)
        {
            return ((threeBitFlag == 4) ? ((byte) 3) : ((byte) threeBitFlag));
        }


        // Fields
        public const ContentAlignment AnyBottom = (ContentAlignment.BottomRight | (ContentAlignment.BottomCenter | ContentAlignment.BottomLeft));
        public const ContentAlignment AnyCenter = (ContentAlignment.BottomCenter | (ContentAlignment.MiddleCenter | ContentAlignment.TopCenter));
        public const ContentAlignment AnyLeft = (ContentAlignment.BottomLeft | (ContentAlignment.MiddleLeft | ContentAlignment.TopLeft));
        public const ContentAlignment AnyMiddle = (ContentAlignment.MiddleRight | (ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft));
        public const ContentAlignment AnyRight = (ContentAlignment.BottomRight | (ContentAlignment.MiddleRight | ContentAlignment.TopRight));
        public const ContentAlignment AnyTop = (ContentAlignment.TopRight | (ContentAlignment.TopCenter | ContentAlignment.TopLeft));
        private static readonly AnchorStyles[] dockingToAnchor;
        public const AnchorStyles HorizontalAnchorStyles = (AnchorStyles.Right | AnchorStyles.Left);
        public static readonly Rectangle MaxRectangle;
        public static readonly Size MaxSize;
        public static readonly SizeF InfinitySize;
        public static readonly SizeF MaxSizeF;
        public static readonly string TestString;
        public const AnchorStyles VerticalAnchorStyles = (AnchorStyles.Bottom | AnchorStyles.Top);

		public static Size InvalidSize = new Size(-2147483648, -2147483648);
		public static Padding InvalidPadding = new Padding(-2147483648, -2147483648, -2147483648, -2147483648);
		public static Rectangle InvalidBounds = new Rectangle(-2147483648, -2147483648, -2147483648, -2147483648);

        // Nested Types
        public sealed class MeasureTextCache
        {
            // Methods
            public MeasureTextCache()
            {
                this.unconstrainedPreferredSize = LayoutUtils.InvalidSize;
                this.nextCacheEntry = -1;
            }

            public Size GetTextSize(string text, Font font, Size proposedConstraints, TextFormatFlags flags)
            {
                if (!this.TextRequiresWordBreak(text, font, proposedConstraints, flags))
                {
                    return this.unconstrainedPreferredSize;
                }
                if (this.sizeCacheList == null)
                {
                    this.sizeCacheList = new PreferredSizeCache[6];
                }
                PreferredSizeCache[] cacheArray1 = this.sizeCacheList;
                for (int num1 = 0; num1 < cacheArray1.Length; num1++)
                {
                    PreferredSizeCache cache1 = cacheArray1[num1];
                    if (cache1.ConstrainingSize == proposedConstraints)
                    {
                        return cache1.PreferredSize;
                    }
                    if ((cache1.ConstrainingSize.Width == proposedConstraints.Width) && (cache1.PreferredSize.Height <= proposedConstraints.Height))
                    {
                        return cache1.PreferredSize;
                    }
                }
                Size size1 = TextRenderer.MeasureText(text, font, proposedConstraints, flags);
                this.nextCacheEntry = (this.nextCacheEntry + 1) % 6;
                this.sizeCacheList[this.nextCacheEntry] = new PreferredSizeCache(proposedConstraints, size1);
                return size1;
            }

            private Size GetUnconstrainedSize(string text, Font font, TextFormatFlags flags)
            {
                if (this.unconstrainedPreferredSize == LayoutUtils.InvalidSize)
                {
                    flags &= ((TextFormatFlags) (-17));
                    this.unconstrainedPreferredSize = TextRenderer.MeasureText(text, font, LayoutUtils.MaxSize, flags);
                }
                return this.unconstrainedPreferredSize;
            }

            public void InvalidateCache()
            {
                this.unconstrainedPreferredSize = LayoutUtils.InvalidSize;
                this.sizeCacheList = null;
            }

            public bool TextRequiresWordBreak(string text, Font font, Size size, TextFormatFlags flags)
            {
                Size size1 = this.GetUnconstrainedSize(text, font, flags);
                return (size1.Width > size.Width);
            }


            // Fields
            private const int MaxCacheSize = 6;
            private int nextCacheEntry;
            private PreferredSizeCache[] sizeCacheList;
            private Size unconstrainedPreferredSize;

            // Nested Types
            [StructLayout(LayoutKind.Sequential)]
            private struct PreferredSizeCache
            {
                public Size ConstrainingSize;
                public Size PreferredSize;
                public PreferredSizeCache(Size constrainingSize, Size preferredSize)
                {
                    this.ConstrainingSize = constrainingSize;
                    this.PreferredSize = preferredSize;
                }
            }
        }
    }
}