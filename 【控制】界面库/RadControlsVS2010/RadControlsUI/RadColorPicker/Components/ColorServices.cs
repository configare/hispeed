using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.RadColorPicker
{
	/// <summary>
	/// Provides common services for color transformations
	/// </summary>
	public class ColorServices
	{
		/// <summary>
		/// Gets a color from RGB ratios
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <param name="value3"></param>
		/// <returns></returns>
		public static Color ColorFromRGBRatios(double value1, double value2, double value3)
		{
			int rValue;
			int gValue;
			int bValue;
			if (value3 == 0)
			{
				rValue = gValue = bValue = (int)(value2 * 255);
			}
			else
			{
				float value4;
				if (value2 <= 0.5)
				{
					value4 = (float)(value2 + (value2 * value3));
				}
				else
				{
					value4 = (float)((value2 + value3) - (value2 * value3));
				}
				float value5 = ((float)(2 * value2)) - value4;
				rValue = ColorServices.GetColorChannelValue(value5, value4, (float)(value1 + 120));
				gValue = ColorServices.GetColorChannelValue(value5, value4, (float)value1);
				bValue = ColorServices.GetColorChannelValue(value5, value4, (float)(value1 - 120));
			}
			return Color.FromArgb(rValue, gValue, bValue);
		}

		private static int GetColorChannelValue(float value1, float value2, float value3)
		{
			if (value3 > 360f)
				value3 -= 360f;
			else if (value3 < 0f)
				value3 += 360f;
			if (value3 < 60f)
				value1 += ((value2 - value1) * value3) / 60f;
			else if (value3 < 180f)
				value1 = value2;
			else if (value3 < 240f)
				value1 += ((value2 - value1) * (240f - value3)) / 60f;

			return (int)(value1 * 255f);
		}

		/// <summary>
		/// Gets a color quotient
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		public static float GetColorQuotient(float value1, float value2)
		{
			return (float)((Math.Atan2((double)value2, (double)value1) * 180) / Math.PI);
		}
	}
}
