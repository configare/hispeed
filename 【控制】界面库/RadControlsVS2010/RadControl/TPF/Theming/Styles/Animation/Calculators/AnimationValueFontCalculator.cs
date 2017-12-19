using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Calculates Font values for property animation, using FontAnimationStep values.
    /// </summary>
    public class AnimationValueFontCalculator : AnimationValueCalculator
    {

		public override object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc)
        {
            Font origial = (Font)currValue;
            FontAnimationStep fontStep = (FontAnimationStep)step;

			return new Font(origial.FontFamily, origial.Size + fontStep.SizeStep);
        }
        
        public override object ConvertToAnimationStepFromString(string value)
        {
            float fontSizeDelta = string.IsNullOrEmpty(value) ? 0f : float.Parse(value);
            return new FontAnimationStep(fontSizeDelta);
        }
       
        public override string ConvertAnimationStepToString(object value)
        {
            FontAnimationStep step = value as FontAnimationStep;

            if (step != null)
            {
                return step.SizeStep.ToString(AnimationValueCalculatorFactory.SerializationCulture);
            }

            return null;
        }

        public override object CalculateInversedStep(object step)
        {
            FontAnimationStep fontStep = (FontAnimationStep)step;
            return new FontAnimationStep(-fontStep.SizeStep);
        }

        public override object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames)
        {
            Font fontStart = (Font)animationStartValue;
            Font fontEnd = (Font)animationEndValue;

            return new FontAnimationStep(
                base.CalculateFloatStep(fontStart.Size, fontEnd.Size, numFrames));
        }
    }    
}
