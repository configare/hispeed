using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.Primitives
{
	public interface IFillElement
	{
		Color BackColor
		{
			get;
			set;
		}

		Color BackColor2
		{
			get;
			set;
		}

		Color BackColor3
		{
			get;
			set;
		}

		Color BackColor4
		{
			get;
			set;
		}

		int NumberOfColors
		{
			get;
			set;
		}

		float GradientAngle
		{
			get;
			set;
		}

		float GradientPercentage
		{
			get;
			set;
		}

		float GradientPercentage2
		{
			get;
			set;
		}

		GradientStyles GradientStyle
		{
			get;
			set;
		}

		Size Size
		{
			get;
			set;
		}
	}
}
