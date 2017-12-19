using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    /// <summary>
    /// Calculates animation rectangle values.
    /// </summary>
    public class AnimationValuePaddingCalculator : AnimationValueCalculator
    {      
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
			if (endValue == null)
			{
				Padding rect = (Padding)step;
				Padding currRect = (Padding)currValue;

				return new Padding(
					currRect.Left + rect.Left,
					currRect.Top + rect.Top,
					currRect.Right + rect.Right,
					currRect.Bottom + rect.Bottom);
			}

			Padding startPadding = (Padding)startValue;
			Padding endPadding = (Padding)endValue;

			Padding res = new Padding(
					calc.CalculateCurrentValue(startPadding.Left, endPadding.Left, currFrameNum, totalFrameNum),
					calc.CalculateCurrentValue(startPadding.Top, endPadding.Top, currFrameNum, totalFrameNum),
					calc.CalculateCurrentValue(startPadding.Right, endPadding.Right, currFrameNum, totalFrameNum),
					calc.CalculateCurrentValue(startPadding.Bottom, endPadding.Bottom, currFrameNum, totalFrameNum)
			);

			return res;
        }

        public override object CalculateInversedStep(object step)
        {
            Padding rect = (Padding)step;

            return new Padding(
                -rect.Left,
                -rect.Top,
                -rect.Right,
                -rect.Bottom);
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
            Padding sizeStart = (Padding)animationStartValue;
            Padding sizeEnd = (Padding)animationEndValue;

            return new Padding(
                base.CalculateIntStep(sizeStart.Left, sizeEnd.Left, numFrames),
                base.CalculateIntStep(sizeStart.Top, sizeEnd.Top, numFrames),
                base.CalculateIntStep(sizeStart.Right, sizeEnd.Right, numFrames),
                base.CalculateIntStep(sizeStart.Bottom, sizeEnd.Bottom, numFrames));
        }
    }
}
