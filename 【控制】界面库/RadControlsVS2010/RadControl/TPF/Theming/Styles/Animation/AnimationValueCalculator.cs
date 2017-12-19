using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using Telerik.WinControls.Styles.Animation.Calculators;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a map of CLR types and corresponding <see cref="AnimationValueCalculator"/> type using when property animation is running and
    /// for animations serialization in themes.
    /// </summary>
    public class AnimationValueCalculatorFactory
    {
        private static Hashtable registeredCalculators;
        private static Hashtable registeredStepsFromSystemType;
        private static Hashtable registeredStepsToSystemType;

        static AnimationValueCalculatorFactory()
        {
            registeredCalculators = new Hashtable();
            registeredStepsFromSystemType = new Hashtable();
            registeredStepsToSystemType = new Hashtable();

            RegisterAnimationValueCalculatorType(typeof(int), typeof(AnimationValueIntCalculator));
            RegisterAnimationValueCalculatorType(typeof(Rectangle), typeof(AnimationValueRectangleCalculator));
            RegisterAnimationValueCalculatorType(typeof(Color), typeof(AnimationValueColorCalculator));
            RegisterAnimationValueCalculatorType(typeof(Font), typeof(AnimationValueFontCalculator));            
            RegisterAnimationValueCalculatorType(typeof(float), typeof(AnimationValueFloatCalculator));
			RegisterAnimationValueCalculatorType(typeof(double), typeof(AnimationValueDoubleCalculator));
            RegisterAnimationValueCalculatorType(typeof(Padding), typeof(AnimationValuePaddingCalculator));

			RegisterAnimationValueCalculatorType(typeof(Size), typeof(AnimationValueSizeCalculator));
			RegisterAnimationValueCalculatorType(typeof(SizeF), typeof(AnimationValueSizeFCalculator));
			RegisterAnimationValueCalculatorType(typeof(Point), typeof(AnimationValuePointCalculator));
			RegisterAnimationValueCalculatorType(typeof(PointF), typeof(AnimationValuePointFCalculator));

            RegisterAnimationStep(typeof(Color), typeof(ColorAnimationStep));
            RegisterAnimationStep(typeof(Font), typeof(FontAnimationStep));            
        }

        public static Type GetRegisteredStepForType(Type systemType)
        {
            return registeredStepsFromSystemType[systemType] as Type;
        }

        public static Type GetRegisteredTypeFromStepType(object stepType)
        {
            return registeredStepsToSystemType[stepType] as Type;
        }

        public static void RegisterAnimationStep(Type systemType, Type stepType)
        {
            registeredStepsFromSystemType[systemType] = stepType;
            registeredStepsToSystemType[stepType] = systemType;
        }

        public static void RegisterAnimationValueCalculatorType(Type objectType, Type calculatorType)
        {
            if (!calculatorType.IsSubclassOf(typeof(AnimationValueCalculator)))
            {
                throw new InvalidOperationException("calculatorType should inherit from class " + typeof(AnimationValueCalculator).FullName);
            }

            registeredCalculators[objectType] = calculatorType;
        }

        private static CultureInfo serializationCulture = CultureInfo.GetCultureInfo("en-US");

        public static CultureInfo SerializationCulture
        {
            get
            {
                return serializationCulture;
            }
        }

        public static AnimationValueCalculator GetCalculator(Type objectType)
        {
            Type actualStepType = GetRegisteredTypeFromStepType(objectType);

            if (actualStepType == null)
            {
                actualStepType = objectType;
            }

            Type resType = (Type)registeredCalculators[actualStepType];

            if (resType == null)
            {
                return null;
            }

            AnimationValueCalculator res = (AnimationValueCalculator)Activator.CreateInstance(resType);
            res.AssociatedType = actualStepType;

            return res;
        }       
    }

    /// <summary>
    /// Calculates values used in each frame of property animation.
    /// Also supports converting animation step values to and from a string for 
    /// theme serialization.
    /// </summary>
    public abstract class AnimationValueCalculator
    {
        private Type associatedType = null;

        public virtual Type AssociatedType
        {
            get
            {
                return associatedType;
            }
            set
            {
                associatedType = value;
            }
        }
        /// <summary>
        /// Calculates the animated value from start value, end value, current value, 
        /// current frame, total number of frames and the specified animation calulator.
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="endValue"></param>
        /// <param name="currValue"></param>
        /// <param name="step"></param>
        /// <param name="currFrameNum"></param>
        /// <param name="totalFrameNum"></param>
        /// <param name="calc"></param>
        /// <returns></returns>
        public abstract object CalculateAnimatedValue(object startValue, object endValue, object currValue, object step, int currFrameNum, int totalFrameNum, EasingCalculator calc);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public abstract object CalculateInversedStep(object step);
        /// <summary>
        /// Retrieves the animation step as a string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual string ConvertAnimationStepToString(object value)
        {
            if (value == null)
            {
                return null;
            }

            string res = value.ToString();

            Type valueType = AnimationValueCalculatorFactory.GetRegisteredStepForType(this.AssociatedType);

            if (valueType == null)
            {
                valueType = this.AssociatedType;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(valueType);

            if (converter != null)
            {
                if (converter.CanConvertTo(typeof(string)))
                {
                    try
                    {
                        res = (string)converter.ConvertTo(value, typeof(string));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error converting animation value: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Can't find TypeConverter to string for animation step " + value.ToString());

                }
            }
            else
            {
                MessageBox.Show("Can't find any TypeConverter for animation property " + value.ToString());
            }

            return res;
        }
        /// <summary>
        /// Converts a string to an animation value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual object ConvertToAnimationStepFromString(string value)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType = AnimationValueCalculatorFactory.GetRegisteredStepForType(this.AssociatedType);

            if (valueType == null)
            {
                valueType = this.AssociatedType;
            }

            object res = null;

            TypeConverter converter = TypeDescriptor.GetConverter(valueType);
            if (converter != null)
            {
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        res = converter.ConvertFrom(value);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error setting animation value: " + ex.Message);
                    }
                }
                else
                {
                    //throw new InvalidOperationException(string.Format(
                    //    "The TypeConverter - {0} cannot convert from string, animation step of type {1}",
                    //    converter.ToString(), valueType.FullName));
                    MessageBox.Show(string.Format("The TypeConverter - {0} cannot convert from string, animation step of type {1}", converter.ToString(), valueType.FullName));
                }
            }
            else
            {
                MessageBox.Show("Can't find any TypeConverter for animation step of type " + valueType.FullName);
            }

            return res;
        }
        /// <summary>
        /// Calculates the animation step from start value, end value, and the total number of frames.
        /// </summary>
        /// <param name="animationStartValue"></param>
        /// <param name="animationEndValue"></param>
        /// <param name="numFrames"></param>
        /// <returns></returns>
        public abstract object CalculateAnimationStep(object animationStartValue, object animationEndValue, int numFrames);

        protected int CalculateIntStep(int animationStartValue, int animationEndValue, int numFrames)
        {
            return this.CalculateIntStep(animationStartValue, animationEndValue, numFrames, int.MinValue, int.MaxValue);
        }

        protected int CalculateIntStep(int animationStartValue, int animationEndValue, int numFrames, int minValue, int maxValue)
        {
            int res;
            if (numFrames <= 0)
            {
                return 0;
            }

            res = Math.Min(maxValue, Math.Max(minValue, animationEndValue - animationStartValue) / numFrames);

            return res;
        }

        protected float CalculateFloatStep(float animationStartValue, float animationEndValue, int numFrames)
        {
            return this.CalculateFloatStep(animationStartValue, animationEndValue, numFrames, float.MinValue, float.MaxValue);
        }

        protected float CalculateFloatStep(float animationStartValue, float animationEndValue, float numFrames, float minValue, float maxValue)
        {
            float res;
            if (numFrames <= 0)
            {
                return 0;
            }

            res = Math.Min(maxValue, Math.Max(minValue, animationEndValue - animationStartValue) / numFrames);

            return res;
        }

        protected double CalculateDoubleStep(double animationStartValue, double animationEndValue, int numFrames)
        {
            return this.CalculateDoubleStep(animationStartValue, animationEndValue, numFrames, double.MinValue, double.MaxValue);
        }

        protected double CalculateDoubleStep(double animationStartValue, double animationEndValue, double numFrames, double minValue, double maxValue)
        {
            double res;
            if (numFrames <= 0)
            {
                return 0;
            }

            res = Math.Min(maxValue, Math.Max(minValue, animationEndValue - animationStartValue) / numFrames);

            return res;
        }

        public virtual object GetStartValue(object animationStartValue)
        {
            return animationStartValue;
        }

        public virtual object GetEndValue(object animationEndValue)
        {
            return animationEndValue;
        }
    }
    /// <summary>
    /// Represents a numerical value calculator. It is used internally by StyleSheet 
    /// system to calculate the value changes when animating RadElement properties.
    /// </summary>
	public abstract class EasingCalculator
	{
        /// <summary>
        /// Calculates the current value of some property from the initial value, end value, current frame, and the numbers of frames.
        /// </summary>
        /// <param name="initialValue"></param>
        /// <param name="endValue"></param>
        /// <param name="currentFrame"></param>
        /// <param name="numFrames"></param>
        /// <returns></returns>
		public abstract int CalculateCurrentValue(int initialValue, int endValue, int currentFrame, int numFrames);
		/// <summary>
		/// Calculates the current value of some property from the initial value, end value, current frame, and the number of frames.
		/// </summary>
		/// <param name="initialValue"></param>
		/// <param name="endValue"></param>
		/// <param name="currentFrame"></param>
		/// <param name="numFrames"></param>
		/// <returns></returns>
        public abstract float CalculateCurrentValue(float initialValue, float endValue, int currentFrame, int numFrames);
		/// <summary>
		/// Caclulates the current value of a property from the initial value, end value, current frame, and the number of frames.
		/// </summary>
		/// <param name="initialValue"></param>
		/// <param name="endValue"></param>
		/// <param name="currentFrame"></param>
		/// <param name="numFrames"></param>
		/// <returns></returns>
        public abstract double CalculateCurrentValue(double initialValue, double endValue, int currentFrame, int numFrames);
	}
    /// <summary>
    /// Defines the easing equations for the easing animations.
    /// </summary>
	public enum RadEasingType
	{
		Linear = 0,

		InQuad,
		OutQuad,
		InOutQuad,

		InCubic,
		OutCubic,
		InOutCubic,

		InQuart,
		OutQuart,
		InOutQuart,

		InQuint,
		OutQuint,
		InOutQuint,

		InSine,
		OutSine,
		InOutSine,

		InExponential,
		OutExponential,
		InOutExponential,

		InCircular,
		OutCircular,
		InOutCircular,

		InElastic,
		OutElastic,
		InOutElastic,

		InBack,
		OutBack,
		InOutBack,

		InBounce,
		OutBounce,
		InOutBounce,

		Default
	}	

	public class StandardEasingCalculator: EasingCalculator
    {
        #region Pivate Data

        private RadEasingType easingType = RadEasingType.InQuad;

        #endregion

        #region Constructors

        public StandardEasingCalculator()
		{
		}

		public StandardEasingCalculator(RadEasingType easingType)
		{
			this.EasingType = easingType;
        }

        #endregion

        #region Accessors

        public RadEasingType EasingType
		{
			get { return easingType; }
			set { easingType = value; }
        }

        #endregion

        #region Easing Helper Methods

        private double OutBounce(double percentDone, double difference)
        {
            if (percentDone < (1 / 2.75))
                return difference * (7.5625 * percentDone * percentDone);
            else if (percentDone < (2 / 2.75))
                return difference * (7.5625 * (percentDone - 1.5 / 2.75) * (percentDone - 1.5 / 2.75) + 0.75);
            else if (percentDone < (2.5 / 2.75))
                return difference * (7.5625 * (percentDone - 2.25 / 2.75) * (percentDone - 2.25 / 2.75) + 0.9375);
            else
                return difference * (7.5625 * (percentDone - 2.625 / 2.75) * (percentDone - 2.625 / 2.75) + 0.984375);
        }

        private double Elastic(double percentDone, double difference, double period, double amplitude)
        {
            double s;

            if (period == 0) period = difference * 0.3;

            if ((amplitude == 0) || (amplitude < Math.Abs(difference)))
            {
                amplitude = difference;
                s = period / 4;
            }
            else
            {
                s = period / (2 * Math.PI) * Math.Asin(difference / amplitude);
            }

            return (difference * Math.Pow(2, 10 * percentDone) * Math.Sin((percentDone * difference - s) * (2 * Math.PI) / period));
        }

        // Calculates num to the power of pow faster than Math.Pow for small powers
        private double FastPow(double num, int pow)
        {
            double res = num;
            for (int i = 1; i < pow; i++)
                res *= num;

            return res;
        }

        #endregion


        #region CalculateCurrentValue methods (TODO: Add parameters for some easing types)

        public override int CalculateCurrentValue(int initialValue, int endValue, int currentFrame, int numFrames)
        {
            return (int)Math.Round(CalculateCurrentValue((double)initialValue, (double)endValue, currentFrame, numFrames));
        }

        public override float CalculateCurrentValue(float initialValue, float endValue, int currentFrame, int numFrames)
        {
            return (float)CalculateCurrentValue((double)initialValue, (double)endValue, currentFrame, numFrames);
        }

        public override double CalculateCurrentValue(double initialValue, double endValue, int currentFrame, int numFrames)
        {
            if (initialValue == endValue)
                return endValue;

            if (currentFrame <= 0) return initialValue;
            if (currentFrame >= numFrames) return endValue;

            double percentDone = (double)currentFrame / (double)numFrames;
            double difference = endValue - initialValue;

            /* 
             * TODO:
             * 
             * Add period and amplitude as parameters for InElastic,OutElastic and InOutElastic
             * Leave 0 for default values
             * 
             * Add backtrack parameter for the InBack, OutBack and InOutBack easings
             * Leave 1.70158 as default value
             */

            double period = 0;
            double amplitude = 0;
            double backTrack = 1.70158; 
            
            switch (this.EasingType)
            {

                #region Default and Linear (Default is Linear)

                default:
                case RadEasingType.Linear:
                    return difference * percentDone + initialValue;

                #endregion

                #region InQuad, OutQuad, InOutQuad (done)
                case RadEasingType.InQuad:
                    return difference * percentDone * percentDone + initialValue;

                case RadEasingType.OutQuad:
                    return -difference * percentDone * (percentDone - 2) + initialValue;

                case RadEasingType.InOutQuad:
                    if (percentDone < 0.5) return 2 * difference * percentDone * percentDone + initialValue;
                    else return -difference * (2 * (percentDone - 1) * (percentDone - 1) - 1) + initialValue;

                #endregion

                #region InCubic, OutCubic, InOutCubic (done)

                case RadEasingType.InCubic:
                    return difference * FastPow(percentDone, 3) + initialValue;

                case RadEasingType.OutCubic:
                    return difference * (FastPow(percentDone - 1, 3) + 1) + initialValue;

                case RadEasingType.InOutCubic:
                    if (percentDone < 0.5) return (4 * difference * FastPow(percentDone, 3)) + initialValue;
                    return (difference * (4 * FastPow(percentDone - 1, 3) + 1)) + initialValue;

                #endregion

                #region InQuart, OutQuart, InOutQuart (done)

                case RadEasingType.InQuart:
                    return (difference * FastPow(percentDone, 4)) + initialValue;

                case RadEasingType.OutQuart:
                    return (-difference * (FastPow(percentDone - 1, 4) - 1)) + initialValue;

                case RadEasingType.InOutQuart:
                    if (percentDone < 0.5) return (8 * difference * FastPow(percentDone, 4)) + initialValue;
                    return (-difference * (8 * FastPow(percentDone - 1, 4) - 1)) + initialValue;

                #endregion

                #region InQuint, OutQuint, InOutQuint (done)

                case RadEasingType.InQuint:
                    return (difference * FastPow(percentDone, 5)) + initialValue;

                case RadEasingType.OutQuint:
                    return (difference * (FastPow(percentDone - 1, 5) + 1)) + initialValue;

                case RadEasingType.InOutQuint:
                    if (percentDone < 0.5) return (8 * difference * FastPow(percentDone, 5)) + initialValue;
                    return (difference * (8 * FastPow(percentDone - 1, 5) + 1)) + initialValue;

                #endregion

                #region InSine, OutSine, InOutSine (done)

                case RadEasingType.InSine:
                    return (difference * (1 - Math.Cos(percentDone * Math.PI / 2))) + initialValue;

                case RadEasingType.OutSine:
                    return (difference * Math.Sin(percentDone * Math.PI / 2)) + initialValue;

                case RadEasingType.InOutSine:
                    return ((difference / 2) * (1 - Math.Cos(percentDone * Math.PI))) + initialValue;

                #endregion

                #region InExponential, OutExponential, InOutExponential (done)

                case RadEasingType.InExponential:
                    return (difference * Math.Pow(2, 10 * (percentDone - 1)) + initialValue);

                case RadEasingType.OutExponential:
                    return (difference * (1 - Math.Pow(2, -10 * percentDone)) + initialValue);

                case RadEasingType.InOutExponential:
                    if (percentDone < 0.5)
                        return (difference * Math.Pow(2, 20 * percentDone - 11)) + initialValue;
                    else
                        return (difference * (1 - Math.Pow(2, -20 * percentDone + 9)) + initialValue);

                #endregion

                #region InCircular, OutCircular, InOutCircular (done)

                case RadEasingType.InCircular:
                    return (difference * (1 - Math.Sqrt(1 - percentDone * percentDone)) + initialValue);

                case RadEasingType.OutCircular:
                    return (difference * Math.Sqrt(percentDone * (2 - percentDone))) + initialValue;

                case RadEasingType.InOutCircular:
                    if (percentDone < 0.5)
                        return ((difference / 2) * (1 - Math.Sqrt(1 - 4 * percentDone * percentDone)) + initialValue);
                    else
                        return ((difference / 2) * (1 + Math.Sqrt(1 - 4 * (percentDone - 1) * (percentDone - 1))) + initialValue);

                #endregion

                #region InElastic, OutElastic, InOutElastic (done)

                case RadEasingType.InElastic:
                    return initialValue - Elastic(percentDone - 1, difference, period, amplitude);

                case RadEasingType.OutElastic:
                    return endValue + Elastic(-percentDone, difference, period, amplitude);

                case RadEasingType.InOutElastic:
                    if (period == 0) period = difference * 0.45; // = 0.3 * 1.5 (taken from JS as is)
                    if (percentDone < 0.5) return (initialValue - 0.5 * Elastic(percentDone * 2 - 1, difference, period, amplitude));
                    else return (endValue + 0.5 * Elastic(1 - percentDone * 2, difference, period, amplitude));

                #endregion

                #region InBack, OutBack, InOutBack (done)

                case RadEasingType.InBack:
                    return (difference * percentDone * percentDone * ((backTrack + 1) * percentDone - backTrack) + initialValue);

                case RadEasingType.OutBack:
                    return (difference * (FastPow(percentDone - 1, 2) * ((backTrack + 1) * (percentDone - 1) + backTrack) + 1) + initialValue);

                case RadEasingType.InOutBack:
                    backTrack *= 1.525;
                    if (percentDone < 0.5)
                        return (2 * difference * FastPow(percentDone, 2) * ((backTrack + 1) * 2 * percentDone - backTrack)) + initialValue;
                    else
                        return (difference * (2 * FastPow(percentDone - 1, 2) * ((backTrack + 1) * 2 * (percentDone - 1) + backTrack) + 1)) + initialValue;

                #endregion

                #region InBounce, OutBounce, InOutBounce (done)

                case RadEasingType.InBounce:
                    return (difference - OutBounce(1 - percentDone, difference)) + initialValue;

                case RadEasingType.OutBounce:
                    return OutBounce(percentDone, difference) + initialValue;

                case RadEasingType.InOutBounce:
                    if (percentDone < 0.5)
                        return (0.5 * (difference - OutBounce(1 - 2 * percentDone, difference))) + initialValue;
                    else
                        return (0.5 * (difference + OutBounce(2 * percentDone - 1, difference))) + initialValue;

                #endregion

            }
        }


        #endregion
    }

	/*
  Easing Equations v1.5
  May 1, 2003
  (c) 2003 Robert Penner, all rights reserved. 
  This work is subject to the terms in http://www.robertpenner.com/easing_terms_of_use.html.  
  
  These tweening functions provide different flavors of 
  math-based motion under a consistent API. 
  
  Types of easing:
  
	  Linear
	  Quadratic
	  Cubic
	  Quartic
	  Quintic
	  Sinusoidal
	  Exponential
	  Circular
	  Elastic
	  Back
	  Bounce

  Changes:
  1.5 - added bounce easing
  1.4 - added elastic and back easing
  1.3 - tweaked the exponential easing functions to make endpoints exact
  1.2 - inline optimizations (changing t and multiplying in one step)--thanks to Tatsuo Kato for the idea
  
  Discussed in Chapter 7 of 
  Robert Penner's Programming Macromedia Flash MX
  (including graphs of the easing equations)
  
  http://www.robertpenner.com/profmx
  http://www.amazon.com/exec/obidos/ASIN/0072223561/robertpennerc-20
*/
/*

// simple linear tweening - no easing
// t: current time, b: beginning value, c: change in value, d: duration
var Penner = {};
Penner.Linear = function (t, b, c, d) {
	return c*t/d + b;
};


 ///////////// QUADRATIC EASING: t^2 ///////////////////

// quadratic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be in frames or seconds/milliseconds
Penner.InQuad = function (t, b, c, d) {
	return c*(t/=d)*t + b;
};

// quadratic easing out - decelerating to zero velocity
Penner.OutQuad = function (t, b, c, d) {
	return -c *(t/=d)*(t-2) + b;
};

// quadratic easing in/out - acceleration until halfway, then deceleration
Penner.InOutQuad = function (t, b, c, d) {
	if ((t/=d/2) < 1) return c/2*t*t + b;
	return -c/2 * ((--t)*(t-2) - 1) + b;
};


 ///////////// CUBIC EASING: t^3 ///////////////////////

// cubic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
Penner.InCubic = function (t, b, c, d) {
	return c*(t/=d)*t*t + b;
};

// cubic easing out - decelerating to zero velocity
Penner.OutCubic = function (t, b, c, d) {
	return c*((t=t/d-1)*t*t + 1) + b;
};

// cubic easing in/out - acceleration until halfway, then deceleration
Penner.InOutCubic = function (t, b, c, d) {
	if ((t/=d/2) < 1) return c/2*t*t*t + b;
	return c/2*((t-=2)*t*t + 2) + b;
};


 ///////////// QUARTIC EASING: t^4 /////////////////////

// quartic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
Penner.InQuart = function (t, b, c, d) {
	return c*(t/=d)*t*t*t + b;
};

// quartic easing out - decelerating to zero velocity
Penner.OutQuart = function (t, b, c, d) {
	return -c * ((t=t/d-1)*t*t*t - 1) + b;
};

// quartic easing in/out - acceleration until halfway, then deceleration
Penner.InOutQuart = function (t, b, c, d) {
	if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
	return -c/2 * ((t-=2)*t*t*t - 2) + b;
};


 ///////////// QUINTIC EASING: t^5  ////////////////////

// quintic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
Penner.InQuint = function (t, b, c, d) {
	return c*(t/=d)*t*t*t*t + b;
};

// quintic easing out - decelerating to zero velocity
Penner.OutQuint = function (t, b, c, d) {
	return c*((t=t/d-1)*t*t*t*t + 1) + b;
};

// quintic easing in/out - acceleration until halfway, then deceleration
Penner.InOutQuint = function (t, b, c, d) {
	if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
	return c/2*((t-=2)*t*t*t*t + 2) + b;
};



 ///////////// SINUSOIDAL EASING: sin(t) ///////////////

// sinusoidal easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
Penner.InSine = function (t, b, c, d) {
	return -c * Math.cos(t/d * (Math.PI/2)) + c + b;
};

// sinusoidal easing out - decelerating to zero velocity
Penner.OutSine = function (t, b, c, d) {
	return c * Math.sin(t/d * (Math.PI/2)) + b;
};

// sinusoidal easing in/out - accelerating until halfway, then decelerating
Penner.InOutSine = function (t, b, c, d) {
	return -c/2 * (Math.cos(Math.PI*t/d) - 1) + b;
};


 ///////////// EXPONENTIAL EASING: 2^t /////////////////

// exponential easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
Penner.InExpo = function (t, b, c, d) {
	return (t==0) ? b : c * Math.pow(2, 10 * (t/d - 1)) + b;
};

// exponential easing out - decelerating to zero velocity
Penner.OutExpo = function (t, b, c, d) {
	return (t==d) ? b+c : c * (-Math.pow(2, -10 * t/d) + 1) + b;
};

// exponential easing in/out - accelerating until halfway, then decelerating
Penner.InOutExpo = function (t, b, c, d) {
	if (t==0) return b;
	if (t==d) return b+c;
	if ((t/=d/2) < 1) return c/2 * Math.pow(2, 10 * (t - 1)) + b;
	return c/2 * (-Math.pow(2, -10 * --t) + 2) + b;
};


 /////////// CIRCULAR EASING: sqrt(1-t^2) //////////////

// circular easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
Penner.InCirc = function (t, b, c, d) {
	return -c * (Math.sqrt(1 - (t/=d)*t) - 1) + b;
};

// circular easing out - decelerating to zero velocity
Penner.OutCirc = function (t, b, c, d) {
	return c * Math.sqrt(1 - (t=t/d-1)*t) + b;
};

// circular easing in/out - acceleration until halfway, then deceleration
Penner.InOutCirc = function (t, b, c, d) {
	if ((t/=d/2) < 1) return -c/2 * (Math.sqrt(1 - t*t) - 1) + b;
	return c/2 * (Math.sqrt(1 - (t-=2)*t) + 1) + b;
};


 /////////// ELASTIC EASING: exponentially decaying sine wave  //////////////

// t: current time, b: beginning value, c: change in value, d: duration, a: amplitude (optional), p: period (optional)
// t and d can be in frames or seconds/milliseconds

Penner.InElastic = function (t, b, c, d, a, p) {
	if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
	if ((!a) || a < Math.abs(c)) { a=c; var s=p/4; }
	else var s = p/(2*Math.PI) * Math.asin (c/a);
	return -(a*Math.pow(2,10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )) + b;
};

Penner.OutElastic = function (t, b, c, d, a, p) {
	if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
	if ((!a) || a < Math.abs(c)) { a=c; var s=p/4; }
	else var s = p/(2*Math.PI) * Math.asin (c/a);
	return a*Math.pow(2,-10*t) * Math.sin( (t*d-s)*(2*Math.PI)/p ) + c + b;
};

Penner.InOutElastic = function (t, b, c, d, a, p) {
	if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (!p) p=d*(.3*1.5);
	if ((!a) || a < Math.abs(c)) { a=c; var s=p/4; }
	else var s = p/(2*Math.PI) * Math.asin (c/a);
	if (t < 1) return -.5*(a*Math.pow(2,10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )) + b;
	return a*Math.pow(2,-10*(t-=1)) * Math.sin( (t*d-s)*(2*Math.PI)/p )*.5 + c + b;
};


 /////////// BACK EASING: overshooting cubic easing: (s+1)*t^3 - s*t^2  //////////////

// back easing in - backtracking slightly, then reversing direction and moving to target
// t: current time, b: beginning value, c: change in value, d: duration, s: overshoot amount (optional)
// t and d can be in frames or seconds/milliseconds
// s controls the amount of overshoot: higher s means greater overshoot
// s has a default value of 1.70158, which produces an overshoot of 10 percent
// s==0 produces cubic easing with no overshoot
Penner.InBack = function (t, b, c, d, s) {
	if (s == undefined) s = 1.70158;
	return c*(t/=d)*t*((s+1)*t - s) + b;
};

// back easing out - moving towards target, overshooting it slightly, then reversing and coming back to target
Penner.OutBack = function (t, b, c, d, s) {
	if (s == undefined) s = 1.70158;
	return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
};

// back easing in/out - backtracking slightly, then reversing direction and moving to target,
// then overshooting target, reversing, and finally coming back to target
Penner.InOutBack = function (t, b, c, d, s) {
	if (s == undefined) s = 1.70158; 
	if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525))+1)*t - s)) + b;
	return c/2*((t-=2)*t*(((s*=(1.525))+1)*t + s) + 2) + b;
};


 /////////// BOUNCE EASING: exponentially decaying parabolic bounce  //////////////

// bounce easing in
// t: current time, b: beginning value, c: change in position, d: duration
Penner.InBounce = function (t, b, c, d) {
	return c - Penner.OutBounce (d-t, 0, c, d) + b;
};

// bounce easing out
Penner.OutBounce = function (t, b, c, d) {
	if ((t/=d) < (1/2.75)) {
		return c*(7.5625*t*t) + b;
	} else if (t < (2/2.75)) {
		return c*(7.5625*(t-=(1.5/2.75))*t + .75) + b;
	} else if (t < (2.5/2.75)) {
		return c*(7.5625*(t-=(2.25/2.75))*t + .9375) + b;
	} else {
		return c*(7.5625*(t-=(2.625/2.75))*t + .984375) + b;
	}
};

// bounce easing in/out
Penner.InOutBounce = function (t, b, c, d) {
	if (t < d/2) return Penner.InBounce (t*2, 0, c, d) * .5 + b;
	return Penner.OutBounce (t*2-d, 0, c, d) * .5 + c*.5 + b;
};
*/
}