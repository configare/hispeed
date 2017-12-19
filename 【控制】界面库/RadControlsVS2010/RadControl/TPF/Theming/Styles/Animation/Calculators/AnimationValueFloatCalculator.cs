using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Calculates float values for the property animation.
    /// </summary>
    public class AnimationValueFloatCalculator : AnimationValueCalculator
    {
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
			if (endValue == null)
			{
				return (float)currValue + (float)step;
			}

			float startDouble = (float)startValue;
			float endDouble = (float)endValue;

			return calc.CalculateCurrentValue(startDouble, endDouble, currFrameNum, totalFrameNum);
        }
      
        public override object CalculateInversedStep(object step)
        {
            float value = (float)step;
            return -value;
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
            return base.CalculateFloatStep((float)animationStartValue, (float)animationEndValue, numFrames);
        }
    }
}
