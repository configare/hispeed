using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Calculates int values for property animation.
    /// </summary>
    public class AnimationValueIntCalculator : AnimationValueCalculator
    {      
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
			if (endValue == null)
			{
				return (int)currValue + (int)step;
			}

			int startDouble = (int)startValue;
			int endDouble = (int)endValue;

			return calc.CalculateCurrentValue(startDouble, endDouble, currFrameNum, totalFrameNum);
        }
      
        public override object CalculateInversedStep(object step)
        {
            int value = (int)step;
            return -value;
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
            return base.CalculateIntStep((int)animationStartValue, (int)animationEndValue, numFrames);
        }
    }    
}
