using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles.Animation.Calculators
{
	/// <summary>
	/// Calculates double values for the property animation.
	/// </summary>
	public class AnimationValueDoubleCalculator : AnimationValueCalculator
	{
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
		{
			if (endValue == null)
			{
				return (double)currValue + (double)step;
			}

			double startDouble = (double)startValue;
			double endDouble = (double)endValue;

			return calc.CalculateCurrentValue(startDouble, endDouble, currFrameNum, totalFrameNum);
		}

		public override object CalculateInversedStep(object step)
		{
			double value = (double)step;
			return -value;
		}
       
		public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
		{
			return base.CalculateDoubleStep((double)animationStartValue, (double)animationEndValue, numFrames);
		}
	}
}
