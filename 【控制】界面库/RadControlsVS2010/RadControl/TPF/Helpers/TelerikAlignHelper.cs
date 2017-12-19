using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls
{
	public class TelerikAlignHelper
	{
		private static readonly TextImageRelation[] imageAlignToRelation;

		static TelerikAlignHelper()
		{
			TextImageRelation[] relationArray = new TextImageRelation[11];
			relationArray[0] = TextImageRelation.ImageBeforeText | TextImageRelation.ImageAboveText;
			relationArray[1] = TextImageRelation.ImageAboveText;
			relationArray[2] = TextImageRelation.TextBeforeImage | TextImageRelation.ImageAboveText;
			relationArray[4] = TextImageRelation.ImageBeforeText;
			relationArray[6] = TextImageRelation.TextBeforeImage;
			relationArray[8] = TextImageRelation.ImageBeforeText | TextImageRelation.TextAboveImage;
			relationArray[9] = TextImageRelation.TextAboveImage;
			relationArray[10] = TextImageRelation.TextBeforeImage | TextImageRelation.TextAboveImage;
			TelerikAlignHelper.imageAlignToRelation = relationArray;
		}

		public static TextFormatFlags TranslateAlignmentForGDI(ContentAlignment align)
		{
			ContentAlignment anyBottom = System.Drawing.ContentAlignment.BottomRight | (System.Drawing.ContentAlignment.BottomCenter | System.Drawing.ContentAlignment.BottomLeft);
			ContentAlignment anyMiddle = System.Drawing.ContentAlignment.MiddleRight | (System.Drawing.ContentAlignment.MiddleCenter | System.Drawing.ContentAlignment.MiddleLeft);

			if ((align & anyBottom) != ((ContentAlignment)0))
			{
				return TextFormatFlags.Bottom;
			}
			if ((align & anyMiddle) != ((ContentAlignment)0))
			{
				return TextFormatFlags.VerticalCenter;
			}
			return TextFormatFlags.GlyphOverhangPadding;
		}

		public static TextFormatFlags TranslateLineAlignmentForGDI(ContentAlignment align)
		{
			ContentAlignment anyRight = System.Drawing.ContentAlignment.BottomRight | (System.Drawing.ContentAlignment.MiddleRight | System.Drawing.ContentAlignment.TopRight);
			ContentAlignment anyCenter = System.Drawing.ContentAlignment.BottomCenter | (System.Drawing.ContentAlignment.MiddleCenter | System.Drawing.ContentAlignment.TopCenter);

			if ((align & anyRight) != ((ContentAlignment)0))
			{
				return TextFormatFlags.Right;
			}
			if ((align & anyCenter) != ((ContentAlignment)0))
			{
				return TextFormatFlags.HorizontalCenter;
			}
			return TextFormatFlags.GlyphOverhangPadding;
		}

		public static StringAlignment TranslateAlignment(ContentAlignment align)
		{
			ContentAlignment anyRight = System.Drawing.ContentAlignment.BottomRight | (System.Drawing.ContentAlignment.MiddleRight | System.Drawing.ContentAlignment.TopRight);
			ContentAlignment anyCenter = System.Drawing.ContentAlignment.BottomCenter | (System.Drawing.ContentAlignment.MiddleCenter | System.Drawing.ContentAlignment.TopCenter);

			if ((align & anyRight) != ((ContentAlignment)0))
			{
				return StringAlignment.Far;
			}
			if ((align & anyCenter) != ((ContentAlignment)0))
			{
				return StringAlignment.Center;
			}
			return StringAlignment.Near;
		}

		public static StringAlignment TranslateLineAlignment(ContentAlignment align)
		{
			ContentAlignment anyBottom = anyBottom = System.Drawing.ContentAlignment.BottomRight | (System.Drawing.ContentAlignment.BottomCenter | System.Drawing.ContentAlignment.BottomLeft);
			ContentAlignment anyMiddle = System.Drawing.ContentAlignment.MiddleRight | (System.Drawing.ContentAlignment.MiddleCenter | System.Drawing.ContentAlignment.MiddleLeft);

			if ((align & anyBottom) != ((ContentAlignment)0))
			{
				return StringAlignment.Far;
			}
			if ((align & anyMiddle) != ((ContentAlignment)0))
			{
				return StringAlignment.Center;
			}
			return StringAlignment.Near;
		}

		public static ContentAlignment RtlTranslateContent(ContentAlignment align)
		{
			ContentAlignment[][] alignmentArrays = new ContentAlignment[3][];
			alignmentArrays[0] = new ContentAlignment[] { ContentAlignment.TopLeft, ContentAlignment.TopRight };
			alignmentArrays[1] = new ContentAlignment[] { ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight };
			alignmentArrays[2] = new ContentAlignment[] { ContentAlignment.BottomLeft, ContentAlignment.BottomRight };
			for (int num1 = 0; num1 < 3; num1++)
			{
				if (alignmentArrays[num1][0] == align)
				{
					return alignmentArrays[num1][1];
				}
				if (alignmentArrays[num1][1] == align)
				{
					return alignmentArrays[num1][0];
				}
			}
			return align;
		}

		public static TextImageRelation RtlTranslateRelation(TextImageRelation relation)
		{
			if (relation != TextImageRelation.ImageBeforeText)
			{
				if (relation == TextImageRelation.TextBeforeImage)
				{
					return TextImageRelation.ImageBeforeText;
				}
				return relation;
			}
			return TextImageRelation.TextBeforeImage;
		}

		public static TextImageRelation ImageAlignToRelation(ContentAlignment alignment)
		{
			return TelerikAlignHelper.imageAlignToRelation[LayoutUtils.ContentAlignmentToIndex(alignment)];
		}

		public static TextImageRelation TextAlignToRelation(ContentAlignment alignment)
		{
			return LayoutUtils.GetOppositeTextImageRelation(TelerikAlignHelper.ImageAlignToRelation(alignment));
		}
	}
}