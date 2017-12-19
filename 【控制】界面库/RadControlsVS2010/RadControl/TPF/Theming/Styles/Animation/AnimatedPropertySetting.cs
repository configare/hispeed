using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using Telerik.WinControls.Collections;
using Telerik.WinControls.Styles.PropertySettings;
using Telerik.WinControls.Primitives;
using System.Collections.Specialized;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the animation type.
    /// </summary>
    public enum RadAnimationType
    {   
        /// <summary>
        /// 
        /// </summary>
        ByStep = 0,
        /// <summary>
        /// 
        /// </summary>
        ByStartEndValues
    }
	
    public class AnimatedPropertySetting : PropertySettingBase
    {
        #region Constructors

        public AnimatedPropertySetting()
        {
        }

        public AnimatedPropertySetting(RadProperty property, object startValue, int numFrames, int interval, object step, object reverseStep)
        {
            base.Property = property;
            this.startValue = startValue;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.step = step;
            this.reverseStep = reverseStep;
            this.animationType = RadAnimationType.ByStep;
            this.skipToEndValueOnReplace = true;
        }

        public AnimatedPropertySetting(RadProperty property, object startValue, int numFrames, int interval, object step)
        {
            base.Property = property;
            this.startValue = startValue;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.step = step;
            this.animatorStyle = AnimatorStyles.AnimateWhenApply;
            this.animationType = RadAnimationType.ByStep;
            this.skipToEndValueOnReplace = true;
        }

        public AnimatedPropertySetting(RadProperty property, int numFrames, int interval, object step, object reverseStep)
        {
            base.Property = property;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.step = step;
            this.reverseStep = reverseStep;
            this.animationType = RadAnimationType.ByStep;
            this.skipToEndValueOnReplace = true;
        }

        public AnimatedPropertySetting(RadProperty property, int numFrames, int interval, object step)
        {
            base.Property = property;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.step = step;
            this.animationType = RadAnimationType.ByStep;
            this.skipToEndValueOnReplace = true;
        }

        public AnimatedPropertySetting(RadProperty property, object animationStartValue, object animationEndValue, int numFrames, int interval)
        {
            base.Property = property;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.StartValue = animationStartValue;
            this.EndValue = animationEndValue;
            this.animationType = RadAnimationType.ByStartEndValues;
            this.skipToEndValueOnReplace = true;

            this.calculator = AnimationValueCalculatorFactory.GetCalculator(base.Property.PropertyType);

            if (calculator != null)
            {
                this.step = calculator.CalculateAnimationStep(this.StartValue, this.EndValue, numFrames);
                this.reverseStep = calculator.CalculateInversedStep(step);
            }
            else
            {
                throw new Exception("Error calculating animation step because there is not any calculator for type '" + base.Property.PropertyType + "' registered.");
            }
        }

        public AnimatedPropertySetting(RadProperty property, AnimationValueCalculator calculator, object animationStartValue, object animationEndValue, int numFrames, int interval)
        {
            base.Property = property;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.StartValue = animationStartValue;
            this.EndValue = animationEndValue;
            this.animationType = RadAnimationType.ByStartEndValues;
            this.skipToEndValueOnReplace = true;

            this.calculator = calculator;


            if (calculator != null)
            {
                this.step = calculator.CalculateAnimationStep(this.StartValue, this.EndValue, numFrames);
                this.reverseStep = calculator.CalculateInversedStep(step);
            }
            else
            {
                throw new Exception("Error calculating animation step because there is not any calculator for type '" + base.Property.PropertyType + "' registered.");
            }
        }

        public AnimatedPropertySetting(RadProperty property, object animationEndValue, int numFrames, int interval)
        {
            base.Property = property;
            this.interval = interval;
            this.NumFrames = numFrames;
            this.StartValue = null;
            this.skipToEndValueOnReplace = true;

            this.EndValue = animationEndValue;

            this.animationType = RadAnimationType.ByStartEndValues;
        }

        #endregion

        #region Events

        public event AnimationFinishedEventHandler AnimationFinished;
        //check whether the event has to be called when the animations are disabled
        public event AnimationStartedEventHandler AnimationStarted;

        #endregion

        #region Public Implementation

        public bool IsAnimating(RadElement element)
        {
            ElementValuesAnimator animator = GetExistingAnimator(element);
            return animator != null ? animator.IsRunning : false;
        }

        public void SetParameters(int interval, object step, object reverseStep, int numRepetitions)
        {
            this.Interval = interval;
            this.NumFrames = numRepetitions;

            this.Step = step;
            this.ReverseStep = reverseStep;
        }

        public void RestoreValue(RadElement element)
        {
            this.OnAnimationFinished(new AnimationStatusEventArgs(element, false, false));
        }

        public virtual void OnValueApplied(RadElement element)
        {
            if (nextPropertySetting != null)
            {
                nextPropertySetting.ApplyValue(element);
            }

            this.OnAnimationFinished(new AnimationStatusEventArgs(element, false, !this.restoreValueAfterApply));
        }

        public void Cancel(RadElement element)
        {
            this.Cancel(element, true);
        }

        public void Cancel(RadElement element, bool reverseAnimation)
        {
            if (this.RemoveDelayTimer(element))
            {
                this.OnAnimationFinished(new AnimationStatusEventArgs(element, true, false));
            }
            else
            {
                ElementValuesAnimator animator = this.GetExistingAnimator(element);
                if (animator != null && animator.IsRunning)
                {
                    animator.Stop();
                    this.OnAnimationFinished(new AnimationStatusEventArgs(element, true, false));
                }
            }
        }

        public void Stop(RadElement element)
        {
            if (this.RemoveDelayTimer(element))
            {
                this.OnAnimationFinished(new AnimationStatusEventArgs(element, true, true));
            }
            else
            {
                ElementValuesAnimator animator = this.GetExistingAnimator(element);
                if (animator != null && animator.IsRunning)
                {
                    animator.Stop();
                    this.OnAnimationFinished(new AnimationStatusEventArgs(element, true, true));
                }
            }
        }

        #endregion

        #region Overrides

        public override object Clone()
        {
            AnimatedPropertySetting setting = new AnimatedPropertySetting();
            setting.animationLoopType = this.animationLoopType;
            setting.animationType = this.animationType;
            setting.animatorStyle = this.animatorStyle;
            setting.applyDelay = this.applyDelay;
            setting.applyEasingType = this.applyEasingType;
            setting.calculator = this.calculator;
            setting.delayTimerForElement = this.delayTimerForElement;
            setting.endValueProviderHelper = this.endValueProviderHelper;
            setting.interval = this.interval;
            setting.isStyleSetting = this.isStyleSetting;
            setting.nextPropertySetting = this.nextPropertySetting;
            setting.numFrames = this.numFrames;
            setting.removeAfterApply = this.removeAfterApply;
            setting.Property = this.Property;
            setting.restoreValueAfterApply = this.restoreValueAfterApply;
            setting.reverseStep = this.reverseStep;
            setting.skipToEndValueOnReplace = this.skipToEndValueOnReplace;
            setting.startValue = this.startValue;
            setting.startValueProviderHelper = this.startValueProviderHelper;
            setting.unApplyEasingType = this.unApplyEasingType;

            return setting;
        }

        public override void ApplyValue(RadElement element)
        {
            if (this.ApplyDelay > 0)
            {
                //we have an initial delay that we should reflect
                Timer timer = new Timer();
                timer.Interval = this.ApplyDelay;
                timer.Tick += new EventHandler(delayTimer_Tick);
                timer.Tag = element;

                delayTimerForElement[element.GetHashCode()] = timer;
                timer.Start();

                object animatedValue = this.StartValue;
                if (animatedValue != null)
                {
                    ElementValuesAnimator animator = this.GetAnimator(element);
                    animator.SetCurrentValue(animatedValue);
                    this.OnAnimationStarted(new AnimationStatusEventArgs(element, false));
                }
            }
            else
            {
                ApplyValueInternal(element);
            }
        }

        public override void UnapplyValue(RadElement element)
        {
            if (this.IsAnimationEnabled(element) && (this.AnimatorStyle & AnimatorStyles.AnimateWhenUnapply) == AnimatorStyles.AnimateWhenUnapply)
            {
                //Undo animation only if already aplied
                ElementValuesAnimator animator = element.ValuesAnimators[this.GetHashCode()] as ElementValuesAnimator;
                if (animator != null)
                {
                    animator.Start(AnimationState.Reversing);
                }
            }
            else
            {
                OnAnimationFinished(new AnimationStatusEventArgs(element, false, false));
            }
        }

        protected override void PropertySettingRemoving(RadObject targetRadObject)
        {
            ElementValuesAnimator animator = ((RadElement)targetRadObject).ValuesAnimators[this.GetHashCode()] as ElementValuesAnimator;
            if (animator != null)
            {
                animator.SettingRemoving();
            }
        }

        public override void UnregisterValue(RadElement element)
        {
            int key = this.GetHashCode();

            ElementValuesAnimator animator = element.ValuesAnimators[key] as ElementValuesAnimator;
            if (animator != null)
            {
                animator.Stop();
                element.ValuesAnimators.Remove(key);
            }

            this.OnAnimationFinished(new AnimationStatusEventArgs(element, false, true));
        }

        public override object GetCurrentValue(RadObject forObject)
        {
            ElementValuesAnimator animator = this.GetAnimator((RadElement)forObject);
            return animator.GetCurrentValue();
        }

        protected override XmlPropertySetting Serialize()
        {
            XmlAnimatedPropertySetting xmlPropertySetting = new XmlAnimatedPropertySetting();
            xmlPropertySetting.AnimationLoopType = this.AnimationLoopType;
            xmlPropertySetting.Property = this.Property.FullName;
            xmlPropertySetting.Value = this.StartValue; //XmlPropertySetting.SerializeValue(this.Property, this.StartValue);
            xmlPropertySetting.EndValue = this.EndValue; //XmlPropertySetting.SerializeValue(this.Property, this.EndValue);

            xmlPropertySetting.Step = null;
            if (this.Step != null)
            {
                xmlPropertySetting.Step = new XmlAnimationStep(this.Step);
            }

            if (this.ReverseStep != null)
            {
                xmlPropertySetting.ReverseStep = new XmlAnimationStep(this.ReverseStep);
            }

            xmlPropertySetting.ApplyEasingType = this.ApplyEasingType;
            xmlPropertySetting.UnapplyEasingType = this.UnapplyEasingType;
            xmlPropertySetting.Interval = this.Interval;
            xmlPropertySetting.NumFrames = this.NumFrames;
            xmlPropertySetting.AnimationType = this.AnimationType;
            xmlPropertySetting.StartValueIsCurrentValue = this.StartValueIsCurrentValue;

            return xmlPropertySetting;
        }

        #endregion

        #region Protected Methods

        protected AnimatedPropertySetting GetAnimatedSetting(RadElement element)
        {
            return (AnimatedPropertySetting)element.GetCurrentAnimation(this.Property);
        }

        protected void OnAnimationFinished(AnimationStatusEventArgs e)
        {
            //notify the animated element for operation completion
            e.Element.OnAnimationFinished(this);

            if (AnimationFinished != null)
                AnimationFinished(this, e);
        }

        protected void OnAnimationStarted(AnimationStatusEventArgs e)
        {
            //notify the animated element for operation start
            e.Element.OnAnimationStarted(this);

            if (AnimationStarted != null)
                AnimationStarted(this, e);
        }

        #endregion

        #region Internal Implementation

        internal object GetInitialValue(RadElement element)
        {
            if (this.startValueProviderHelper.Value != null)
            {
                return this.startValueProviderHelper.Value;
            }

            return element.GetValue(this.Property);
        }

        private ElementValuesAnimator GetExistingAnimator(RadElement element)
        {
            return element.ValuesAnimators[this.GetHashCode()] as ElementValuesAnimator;
        }

        private ElementValuesAnimator GetAnimator(RadElement element)
        {
            ElementValuesAnimator result = element.ValuesAnimators[this.GetHashCode()] as ElementValuesAnimator;

            if (result == null)
            {
                result = CreateAnimator(element);
            }

            return result;
        }

        private ElementValuesAnimator CreateAnimator(RadElement element)
        {
            ElementValuesAnimator result = new ElementValuesAnimator(element, this);
            element.ValuesAnimators.Add(this.GetHashCode(), result);

            return result;
        }

        internal void RemovePreviousAnimation(RadElement element, AnimatedPropertySetting setting)
        {
            // Evtim: Added code to throw OnAnimationFinished when animation is changed with another one
            ElementValuesAnimator prevAnimator = setting != null ? setting.GetExistingAnimator(element) : null;
            if (prevAnimator != null && prevAnimator.IsRunning)
            {
                setting.PropertySettingRemoving(element);
                setting.OnAnimationFinished(new AnimationStatusEventArgs(element, true, false));
            }
        }

        private void ApplyValueInternal(RadElement element)
        {
            RemovePreviousAnimation(element, this.GetAnimatedSetting(element));

            ElementValuesAnimator animator = this.GetAnimator(element);
            animator.ReInitialize(element, this.GetInitialValue(element));

            //notify attached element for animation start event
            if (this.applyDelay <= 0)
            {
                this.OnAnimationStarted(new AnimationStatusEventArgs(element, false));
            }

            if (this.IsAnimationEnabled(element) && (this.AnimatorStyle & AnimatorStyles.AnimateWhenApply) == AnimatorStyles.AnimateWhenApply &&
                element.Visibility == ElementVisibility.Visible && element.ElementTree != null && element.ElementTree.Control.Visible)
            {
                animator.Start(AnimationState.Applying);
                return;
            }

            //we should animate the value, simply calculate the EndValue and apply it to the element.
            AnimationValueCalculator calculator = AnimationValueCalculatorFactory.GetCalculator(this.Property.PropertyType);
            object animatedValue;

            if (this.EndValue == null)
            {
                animatedValue = this.StartValue;
                if (animatedValue == null)
                {
                    animatedValue = animator.CachedStartValue;
                }
                for (int i = 1; i <= this.NumFrames; i++)
                {
                    animatedValue = calculator.CalculateAnimatedValue(this.StartValue, this.EndValue, animatedValue, this.Step, i, this.NumFrames, new StandardEasingCalculator(RadEasingType.InQuad));
                }
            }
            else
            {
                animatedValue = this.EndValue;
            }

            animator.SetCurrentValue(animatedValue);
            //notify for the value change
            this.OnValueApplied(element);
        }

        internal bool RemoveDelayTimer(RadElement element)
        {
            int elementID = element.GetHashCode();
            Timer delayTimer = (Timer)this.delayTimerForElement[elementID];

            if (delayTimer == null)
            {
                return false;
            }

            this.delayTimerForElement.Remove(elementID);
            delayTimer.Stop();
            delayTimer.Dispose();

            return true;
        }

        internal void OnSettingRemoving(RadElement element)
        {
            this.RemoveDelayTimer(element);
        }

        /// <summary>
        /// This method supports internal TPF infrastructure and is not intended for use elsewhere
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IValueProvider GetStartValueProvider()
        {
            return this.startValueProviderHelper.ValueProvider;
        }

        /// <summary>
        /// This method supports internal TPF infrastructure and is not intended for use elsewhere
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IValueProvider GetEndValueProvider()
        {
            return this.endValueProviderHelper.ValueProvider;
        }

        #endregion

        #region Event Handlers

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            timer.Stop();
            timer.Dispose();
            RadElement element = (RadElement)timer.Tag;

            int elementID = element.GetHashCode();
            object res = delayTimerForElement[elementID];
            if (res != null)
            {
                delayTimerForElement.Remove(elementID);
                this.ApplyValueInternal(element);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether this setting is created from a StyleSheet.
        /// It is used internally by the framework to determine whether the value
        /// is treated as Animated (with highest precedence) or as a Style one.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsStyleSetting
        {
            get
            {
                return this.isStyleSetting;
            }
            internal set
            {
                this.isStyleSetting = value;
            }
        }

        public RadAnimationType AnimationType
        {
            get { return animationType; }
            set { animationType = value; }
        }

        public LoopType AnimationLoopType
        {
            get
            {
                return this.animationLoopType;
            }
            set
            {
                this.animationLoopType = value;
            }
        }

        public AnimatorStyles AnimatorStyle
        {
            get { return animatorStyle; }
            set { animatorStyle = value; }
        }

        public bool StartValueIsCurrentValue
        {
            get { return startValueIsCurrentValue; }
            set { startValueIsCurrentValue = value; }
        }

        public RadEasingType ApplyEasingType
        {
            get { return applyEasingType; }
            set { applyEasingType = value; }
        }

        public AnimationValueCalculator Calculator
        {
            get { return this.calculator; }

            set { this.calculator = value; }
        }

        public RadEasingType UnapplyEasingType
        {
            get { return unApplyEasingType; }
            set { unApplyEasingType = value; }
        }

        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }

        public object Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
            }
        }

        public object ReverseStep
        {
            get
            {
                return reverseStep;
            }
            set
            {
                reverseStep = value;
            }
        }

        public int NumFrames
        {
            get
            {
                return numFrames;
            }
            set
            {
                numFrames = value;
            }
        }

        public object EndValue
        {
            get
            {
                return endValueProviderHelper.Value;
            }
            set
            {
                endValueProviderHelper.Value = value;
            }
        }

        public bool RemoveAfterApply
        {
            get
            {
                return removeAfterApply;
            }
            set
            {
                removeAfterApply = value;
            }
        }

        /// <summary>
        /// Determines whether a currently running animation should reach its EndValue upon
        /// replacemenet with another one.
        /// </summary>
        [DefaultValue(true)]
        public bool SkipToEndValueOnReplace
        {
            get
            {
                return skipToEndValueOnReplace;
            }
            set
            {
                skipToEndValueOnReplace = value;
            }
        }

        public object StartValue
        {
            get
            {
                return this.startValueProviderHelper.Value;
            }
            set
            {
                this.startValueProviderHelper.Value = value;
            }
        }

        public static bool AnimationsEnabled
        {
            get
            {
                return AnimatedPropertySetting.animationsEnabled;
            }
            set
            {
                AnimatedPropertySetting.animationsEnabled = value;
            }
        }

        public bool IsAnimationEnabled(RadElement element)
        {
            return ThemeResolutionService.AllowAnimations &&
                AnimatedPropertySetting.AnimationsEnabled && 
                element.ElementTree != null &&
                element.ElementTree.AnimationsEnabled;
        }

        /// <summary>
        /// Gets or sets animation start Delay in milliseconds
        /// </summary>
        public int ApplyDelay
        {
            get
            {
                return applyDelay;
            }
            set
            {
                applyDelay = value;
            }
        }

        public IPropertySetting NextPropertySetting
        {
            get
            {
                return nextPropertySetting;
            }
            set
            {
                nextPropertySetting = value;
            }
        }

        public bool RestoreValueAfterApply
        {
            get
            {
                return restoreValueAfterApply;
            }
            set
            {
                restoreValueAfterApply = value;
            }
        }

        #endregion

        #region Fields

        private static bool animationsEnabled = true;

        private bool isStyleSetting;
        private bool removeAfterApply = false;
        private bool restoreValueAfterApply = false;

        IPropertySetting nextPropertySetting = null;

        private bool skipToEndValueOnReplace = true;

        private AnimationValueCalculator calculator = null;

        private bool startValueIsCurrentValue;
        private int applyDelay = 0;

        private int numFrames = 5;
        private int interval = 50;
        private object step = null;
        private object reverseStep = null;

        private object startValue = null;

        private HybridDictionary delayTimerForElement = new HybridDictionary(0);

        private AnimatorStyles animatorStyle = AnimatorStyles.AnimateWhenApply;
        private RadAnimationType animationType = RadAnimationType.ByStartEndValues;
        private LoopType animationLoopType = LoopType.None;

        private RadEasingType applyEasingType = RadEasingType.OutQuad;
        private RadEasingType unApplyEasingType = RadEasingType.OutQuad;

        private ValueProviderHelper endValueProviderHelper = new ValueProviderHelper();
        private ValueProviderHelper startValueProviderHelper = new ValueProviderHelper();

        #endregion
    }    
}
