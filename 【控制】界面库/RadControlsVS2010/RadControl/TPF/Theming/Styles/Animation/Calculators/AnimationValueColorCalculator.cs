using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Animates color values using ColorAnimationStep objects.
    /// </summary>
    public class AnimationValueColorCalculator : AnimationValueCalculator
    {
		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
			Color currColor = (Color)currValue;
			ColorAnimationStep colorStep = (ColorAnimationStep)step;

            int newA;
			int newR;
			int newG;
			int newB;

			if (endValue == null)
			{
				newA = Math.Max(0, Math.Min(255, currColor.A + colorStep.A));
				newR = Math.Max(0, Math.Min(255, currColor.R + colorStep.R));
				newG = Math.Max(0, Math.Min(255, currColor.G + colorStep.G));
				newB = Math.Max(0, Math.Min(255, currColor.B + colorStep.B));

				return Color.FromArgb(newA, newR, newG, newB);
			}

			Color startColor = (Color)startValue;
			Color endColor = (Color)endValue;

			newA = Math.Max(0, Math.Min(255, calc.CalculateCurrentValue(startColor.A, endColor.A, currFrameNum, totalFrameNum)));
			newR = Math.Max(0, Math.Min(255, calc.CalculateCurrentValue(startColor.R, endColor.R, currFrameNum, totalFrameNum)));
			newG = Math.Max(0, Math.Min(255, calc.CalculateCurrentValue(startColor.G, endColor.G, currFrameNum, totalFrameNum)));
			newB = Math.Max(0, Math.Min(255, calc.CalculateCurrentValue(startColor.B, endColor.B, currFrameNum, totalFrameNum)));

            return Color.FromArgb(newA, newR, newG, newB);
        }

        public override object CalculateInversedStep(object step)
        {
            ColorAnimationStep colorBase = (ColorAnimationStep)step;
            return new ColorAnimationStep(-colorBase.A, -colorBase.R, -colorBase.G, -colorBase.B);
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
            Color colorStart = (Color)animationStartValue;
            Color colorEnd = (Color)animationEndValue;

            return new ColorAnimationStep(
                base.CalculateIntStep(colorStart.A, colorEnd.A, numFrames, -255, 255),
                base.CalculateIntStep(colorStart.R, colorEnd.R, numFrames, -255, 255),
                base.CalculateIntStep(colorStart.G, colorEnd.G, numFrames, -255, 255),
                base.CalculateIntStep(colorStart.B, colorEnd.B, numFrames, -255, 255));
        }
    }
}
