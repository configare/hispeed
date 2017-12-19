using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <exclude/>
	public class GradientColorValue
	{
		public Color ColorValue;
		public float ColorPosition;

		public GradientColorValue()
		{
			ColorValue = Color.White;
			ColorPosition = 0.0f;
		}
		public GradientColorValue(Color color, float point)
		{
			ColorValue = color;
			ColorPosition = point;
		}
	}
}
