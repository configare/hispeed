using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Telerik.WinControls.UI
{   
    /// <exclude/>
    public enum ColorModes
	{
		Red, Green, Blue, Hue, Saturation, Luminance,
	}

	/// <summary>
	/// Provides different sets of colors
	/// </summary>
	public class ColorProvider
	{
		#region Basic colors

        private static string[] basicColorNames = new string[]
		{
			"#ff8080", "#ffff80", "#80ff80", "#00ff80", "#80ffff", "#0080ff", "#ff80c0", "#ff80ff",
			"#ff0000", "#ffff00", "#80ff00", "#00ff40", "#00ffff", "#0080c0", "#8080c0", "#ff00ff",
			"#804040", "#ff8040", "#00ff00", "#008080", "#004080", "#8080ff", "#800040", "#ff0080",
			"#800000", "#ff8000", "#008000", "#008040", "#0000ff", "#0000a0", "#800080", "#8000ff",
			"#400000", "#804000", "#004000", "#004040", "#000080", "#000040", "#400040", "#400080",
			"#000000", "#808000", "#808040", "#808080", "#408080", "#c0c0c0", "#400040", "#ffffff"
		};

		#endregion
		/// <summary>
		/// Gets the set of basic colors
		/// </summary>
		public static Color[] BasicColors
		{
			get
			{
				Color[] colors = new Color[basicColorNames.Length];
				for (int i = 0; i < colors.Length; i++)
					colors[i] = HexToColor(basicColorNames[i]);
				return colors;
			}
		}

		/// <summary>
		/// Gets the set of system colors
		/// </summary>
		public static Color[] SystemColors
		{
			get
			{
				return (Color[])GetColorsFromType(typeof(SystemColors)).ToArray(typeof(Color));
			}
		}

		/// <summary>
		/// Gets the set of named colors
		/// </summary>
		public static Color[] NamedColors
		{
			get
			{
				return (Color[])GetColorsFromType(typeof(Color)).ToArray(typeof(Color));
			}
		}

        private static Regex parseHex = new Regex(@"^\s*#?(?<value>([a-fA-F0-9]{6})|([a-fA-F0-9]{8}))\s*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

		/// <summary>
		/// Gets the color correspoding to a hex value
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color HexToColor(string color)
		{

            Match m = parseHex.Match(color);

            if (!m.Success || !m.Groups["value"].Success)
            {
                return Color.Empty;
            }

            string parsedValue = m.Groups["value"].ToString();

            UInt32 uintColor = UInt32.Parse(
                parsedValue,
                System.Globalization.NumberStyles.HexNumber
                );

            if (parsedValue.Length == 6)
                uintColor |= 0xFF000000;

            return Color.FromArgb((int)uintColor);

		}

		/// <summary>
		/// Gets the hex value for the color
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static string ColorToHex(Color color)
		{
            return color.A == 255 ?
                string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B) :
                string.Format("{0:X2}{1:X2}{2:X2}{2:X2}", color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Gets the rounded value
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static int Round(double val)
		{
			int ret_val = (int)val;

			int temp = (int)(val * 100);

			if ((temp % 100) >= 50)
				ret_val += 1;

			return ret_val;
		}

        static ArrayList GetColorsFromType(Type type)
		{
			ArrayList colors = new ArrayList();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

			foreach (PropertyInfo property in properties)
				if (property.PropertyType.Equals(typeof(Color)))
				{
					Color color = (Color)property.GetValue(type, null);
					colors.Add(color);
				}

			return colors;
		}
	}
}
