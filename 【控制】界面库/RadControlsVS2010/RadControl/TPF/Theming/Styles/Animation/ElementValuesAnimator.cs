using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls
{
    public class ElementValuesAnimator
    {
        #region Nested

        /// <summary>
        /// This class is a base for all classes that implement
        /// the logic which is executed when a certain action
        /// in the <see cref="ElementValuesAnimator"/> should be performed.
        /// </summary>
        public abstract class ValuesAnimatorState
        {

            #region Fields

            protected ElementValuesAnimator animator;

            #endregion

            #region Ctor

            /// <summary>
            /// Creates an instance of the <see cref="ValuesAnimatorState"/> class.
            /// </summary>
            /// <param name="animator"></param>
            public ValuesAnimatorState(ElementValuesAnimator animator)
            {
                this.animator = animator;
            }

            #endregion

            #region Methods

            /// <summary>
            /// This method executes the action which is to be performed.
            /// </summary>
            /// <param name="newState">An instance of the <see cref="AnimationState"/>
            /// enum which represents the state change which triggered the action.</param>
            /// <returns>If a state change is performed, the method returns an instance of the new state.
            /// Otherwise the method returns null.</returns>
            protected abstract ValuesAnimatorState PerformActionOverride(AnimationState newState);

            /// <summary>
            /// This method executes an action according to the <see cref="AnimationState"/>
            /// provided.
            /// <param name="newState">The new animation state based on which
            /// an action is performed.</param>
            /// </summary>
            public virtual ValuesAnimatorState PerformAction(AnimationState newState)
            {
                ValuesAnimatorState state = this.PerformActionOverride(newState);

                if (state == null)
                {
                    this.PerformDefaultAction(newState);
                    return null;
                }

                return state;
            }

            /// <summary>
            /// This method is executed when the state is not changed after
            /// a PerformAction call.
            /// </summary>
            /// <param name="state">The state which triggered the PerformAction call.</param>
            protected virtual void PerformDefaultAction(AnimationState state)
            {
                this.animator.numFrames = this.animator.originalNumFrames;
                this.animator.easingCalculator.EasingType = this.animator.setting.ApplyEasingType;
                this.animator.currentFrame = 0;
                this.animator.currentStep = this.animator.step;
                this.animator.currentState = state;
                this.animator.StartTimer();
            }

            #endregion
        }

        public class ValuesAnimatorReversingState : ValuesAnimatorState
        {
            #region Ctor

            public ValuesAnimatorReversingState(ElementValuesAnimator animator)
                : base(animator)
            {
            }

            #endregion

            #region Methods

            protected override ValuesAnimatorState PerformActionOverride(AnimationState newState)
            {
                switch (newState)
                {
                    case AnimationState.NotRunning:
                        {
                            this.animator.setting.RestoreValue(animator.element);
                            this.animator.value = null;
                            this.animator.currentState = newState;

                            return new ValuesAnimatorNotRunningState(this.animator);
                        }
                    case AnimationState.Applying:
                        {
                            //Animation has not fiished, but canceled 
                            if (this.animator.currentFrame < this.animator.numFrames)
                            {
                                //So jump to the first frame
                                this.animator.PerformChangeValue(this.animator.numFrames);
                            }

                            //this.numFrames = originalNumFrames - this.currentFrame + 1;
                            this.animator.numFrames = this.animator.originalNumFrames;
                            this.animator.currentFrame = 0;
                            this.animator.currentStep = this.animator.step;
                            this.animator.easingCalculator.EasingType = this.animator.setting.ApplyEasingType;

                            this.animator.currentState = newState;
                            this.animator.StartTimer();
                            return new ValuesAnimatorApplyingState(this.animator);
                        }
                    default:
                        return null;
                }
            }

            #endregion
        }

        public class ValuesAnimatorApplyingState : ValuesAnimatorState
        {
            #region Ctor

            public ValuesAnimatorApplyingState(ElementValuesAnimator animator)
                : base(animator)
            {
            }

            #endregion

            #region Methods

            private void PerformAnimationLoop()
            {
                switch(this.animator.animationLoopType)
                {
                    case LoopType.Standard:
                        {
                            this.animator.currentFrame = 0;
                            this.animator.StartTimer();
                            break;
                        }
                    case LoopType.Reversed:
                        {
                            object startValue = this.animator.cachedStartValue;
                            this.animator.cachedStartValue = this.animator.cachedEndValue;
                            this.animator.cachedEndValue = startValue;
                            this.animator.currentFrame = 0;
                            this.animator.StartTimer();
                            break;
                        }
                }
            }

            protected override ValuesAnimatorState PerformActionOverride(AnimationState newState)
            {
                switch(newState)
                {
                    case AnimationState.NotRunning:
                        {
                            if (this.animator.AnimationLoopType == LoopType.None)
                            {
                                //Animation has not fiished, but canceled 
                                if (this.animator.currentFrame < this.animator.numFrames)
                                {
                                    //So jump to the last frame
                                    this.animator.PerformChangeValue(this.animator.numFrames);
                                }

                                this.animator.currentState = newState;
                                this.animator.setting.OnValueApplied(this.animator.element);

                                return new ValuesAnimatorNotRunningState(this.animator);
                            }
                            else
                            {
                                this.PerformAnimationLoop();
                                return this;
                            }
                        }
                    case AnimationState.Reversing:
                        {
                            //Animation has not fiished, but canceled 
                            if (this.animator.currentFrame <= this.animator.numFrames)
                            {
                                //So jump to the last frame
                                this.animator.PerformChangeValue(this.animator.numFrames);
                            }

                            this.animator.numFrames = this.animator.originalNumFrames;//this.currentFrame;
                            this.animator.currentFrame = 0;
                            this.animator.currentStep = this.animator.reverseStep;
                            this.animator.easingCalculator.EasingType = this.animator.setting.UnapplyEasingType;

                            this.animator.currentState = newState;
                            this.animator.StartTimer();
                            return new ValuesAnimatorReversingState(this.animator);
                        }
                    default:
                        return null;
                }

            }



            #endregion
        }

        public class ValuesAnimatorNotRunningState : ValuesAnimatorState
        {
            #region Ctor

            public ValuesAnimatorNotRunningState(ElementValuesAnimator animator)
                : base(animator)
            {
            }

            #endregion

            #region Methods

            protected override ValuesAnimatorState PerformActionOverride(AnimationState newState)
            {
                switch(newState)
                {
                    case AnimationState.Applying:
                        {
                            this.animator.numFrames = this.animator.originalNumFrames;
                            this.animator.easingCalculator.EasingType = this.animator.setting.ApplyEasingType;
                            this.animator.currentFrame = 0;
                            this.animator.currentStep = this.animator.step;
                            this.animator.currentState = newState;
                            this.animator.StartTimer();
                            return new ValuesAnimatorApplyingState(this.animator);
                        }
                    case AnimationState.Reversing:
                        {
                            this.animator.numFrames = this.animator.originalNumFrames;
                            this.animator.currentFrame = 0;
                            this.animator.currentStep = this.animator.reverseStep;
                            this.animator.easingCalculator.EasingType = this.animator.setting.UnapplyEasingType;

                            this.animator.currentState = newState;
                            this.animator.StartTimer();
                            return new ValuesAnimatorReversingState(this.animator);
                        }
                    default:
                        return null;
                }
            }

            #endregion
        }

        #endregion

        private ValuesAnimatorState currentAnimatorState;
        private object cachedEndValue = null;

        private RadElement element;
		private RadControlAnimationTimer timer;

        private LoopType animationLoopType = LoopType.None;

        private int currentFrame = 0;
        private int originalNumFrames = 5;
        private int numFrames = 5;
        private int interval = 40;		

        private AnimationState currentState = AnimationState.NotRunning;

        private object cachedStartValue;

        private object step = null;
        private object reverseStep = null;

        private object currentStep = null;

        private AnimatedPropertySetting setting;

		private StandardEasingCalculator easingCalculator = new StandardEasingCalculator();

        public ElementValuesAnimator(RadElement element, AnimatedPropertySetting setting)
        {
            this.element = element;
            this.setting = setting;
            this.currentAnimatorState = new ValuesAnimatorNotRunningState(this);
        }

    	private object initialValueSetFromPreviousAnimatedPropertySetting = null;

        public object CachedStartValue
        {
            get
			{
				return this.cachedStartValue;
			}
        }

        public object CachedEndValue
        {
            get
            {
                return this.cachedEndValue;
            }
        }

        public void Start(AnimationState newState)
        {
            ChangedCurrentAction(newState);
        }

        internal void OnElementDisposed()
        {
            //stop timer
            this.StopTimer();
            //ask the setting to remove the delay timer for the element
            this.setting.RemoveDelayTimer(element);

            //clear references
            this.setting = null;
            this.element = null;
        }

        private void StartTimer()
        {
			if (this.element.ElementTree != null)
			{
				DetermineChildControlsVisible(this.element.ElementTree.Control);
			}

			this.timer = new RadControlAnimationTimer();
            this.timer.Interval = this.interval < 5? 40 : this.interval;

            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }		

		private void StopTimer()
		{
			if (this.timer != null)
			{
				if (this.element.ElementTree != null)
				{
					RestoreChildControlsVisible(this.element.ElementTree.Control);
				}

				this.timer.Stop();
				this.timer = null;
			}
        }

        #region Hide child controls when animating size/location of an element
        //the following was used to fix issues when animating elements outside the bounds of the parent element
        //If this case is not handled any child controls would appear over the parent element
        //Example - RadRibbonBar expand/collapse animation


        static Hashtable alteredVisibleChildControls = new Hashtable();
		static Hashtable savedChildVisibility = new Hashtable();

		private void DetermineChildControlsVisible(/*RadControl*/ Control control)
		{
			if (!PropertyAnimationAffectsChildControlsBounds())
				return;

			int controlHashCode = control.GetHashCode();
			object objCounter = alteredVisibleChildControls[controlHashCode];
			int counter = 0;
			bool startCounter = true;
			if (objCounter != null)
			{
				counter = (int)objCounter;
			}
			else
			{
				startCounter = false;
				foreach (Control childControl in control.Controls)
				{
					int childControlHashCode = childControl.GetHashCode();

					if (childControl.Visible)
					{
						savedChildVisibility[childControlHashCode] = childControl.Visible;
						childControl.Visible = false;
					}

					startCounter = true;
				}
			}

			counter++;

			if (startCounter)
				alteredVisibleChildControls[controlHashCode] = counter;
		}

		private void RestoreChildControlsVisible(Control control)
		{
			if (!PropertyAnimationAffectsChildControlsBounds())
				return;

			int controlHashCode = control.GetHashCode();
			object objCounter = alteredVisibleChildControls[controlHashCode];
			int counter = 0;
			if (objCounter != null)
			{
				counter = (int)objCounter;
			}

			counter--;
			//System.Diagnostics.Debug.WriteLine("" + counter);			

			if (counter <= 0)
			{
				alteredVisibleChildControls[controlHashCode] = null;
				foreach (Control childControl in control.Controls)
				{
					int childControlHashCode = childControl.GetHashCode();

					object childControlVisible = savedChildVisibility[childControlHashCode];
					if (childControlVisible != null)
					{
						childControl.Visible = (bool)childControlVisible;
						savedChildVisibility[childControlHashCode] = null;
					}
				}
			}
			else
			{				
				alteredVisibleChildControls[controlHashCode] = counter;
			}
		}

		private bool? propertyAnimationAffectsChildControlsBoundsCache = null;

		private bool PropertyAnimationAffectsChildControlsBounds()
		{
			if (propertyAnimationAffectsChildControlsBoundsCache == null ||
				!propertyAnimationAffectsChildControlsBoundsCache.HasValue)
			{
				if (this.setting.Property == RadElement.PositionOffsetProperty)
					propertyAnimationAffectsChildControlsBoundsCache = true;
				else
					propertyAnimationAffectsChildControlsBoundsCache = false;

				//TODO: should this be considered?
				//RadPropertyMetadata metadata = this.setting.Property.GetMetadata(this.element);
				//(metadata as RadElementPropertyMetadata).AffectsLayout...				
			}

			return propertyAnimationAffectsChildControlsBoundsCache.Value;
        }
        #endregion

        private object value = null;

        void timer_Tick(object sender, EventArgs e)
        {
            this.currentFrame++;

            if (this.currentFrame > numFrames)
            {
                ChangedCurrentAction(AnimationState.NotRunning);
                return;
            }

            this.PerformChangeValue(this.currentFrame);
        }

		private void PerformChangeValue(int frame)
		{
			if (this.value == null)
			{
				return;
			}


			if (this.currentState == AnimationState.Applying)
			{
                this.value = this.Calculator.CalculateAnimatedValue(this.cachedStartValue, this.cachedEndValue, this.value, currentStep, frame, this.originalNumFrames, this.easingCalculator);
			}
			else
			{
				this.value = this.Calculator.CalculateAnimatedValue(this.setting.EndValue, this.setting.StartValue, this.value, currentStep, frame, this.originalNumFrames, this.easingCalculator);
            }

			if (element.IsInValidState(true))
			{
				//temp fix of bounds-animation problem
                if (this.setting.Property == RadElement.BoundsProperty)
                {
                    element.SetBounds((Rectangle)value);
                }
                else
                    element.OnAnimatedPropertyValueChanged(this.setting);
			}
		}

        private AnimationValueCalculator calculator;

        protected virtual AnimationValueCalculator Calculator
        {
            get
            {

                if (this.setting != null && this.setting.Calculator != null)
                {
                    return this.calculator = this.setting.Calculator;
                }

                if (this.calculator == null)
                {
                    this.calculator = AnimationValueCalculatorFactory.GetCalculator(this.setting.Property.PropertyType);
                }

                return this.calculator;
            }
        }

		public void StopAndReverse(bool animateReverse)
		{
			if (animateReverse && this.currentState == AnimationState.Applying)
			{
				this.ChangedCurrentAction(AnimationState.Reversing);
			}
			else if (this.currentState != AnimationState.NotRunning)
			{
				this.ChangedCurrentAction(AnimationState.NotRunning);
			}
		}

        private void ChangedCurrentAction(AnimationState newState)
        {
            StopTimer();

            if (this.currentAnimatorState != null)
            {
                ValuesAnimatorState state = this.currentAnimatorState.PerformAction(newState);

                if (state != null)
                {
                    this.currentAnimatorState = state;
                }
            }
        }      

        public object GetCurrentValue()
        {
            return this.value; 
        }

        public void ResetValue()
        {
            this.value = null;
        }

        public void SetCurrentValue(object startValue)
        {
            this.value = startValue;
        }

		internal object InitialValueSetFromPreviousAnimatedPropertySetting
		{
			get
			{ 
				return initialValueSetFromPreviousAnimatedPropertySetting; 
			}
			set
			{ 
				initialValueSetFromPreviousAnimatedPropertySetting = value;
				
				this.cachedStartValue = value;
				this.value = value;
			}
		}

		public void ReInitialize(RadElement element, object initialValue)
		{
            this.cachedStartValue = initialValue;
            this.cachedEndValue = this.setting.EndValue;
			this.value = this.cachedStartValue;
			this.originalNumFrames = this.setting.NumFrames;
			this.numFrames = this.setting.NumFrames;
			this.interval = setting.Interval;
			this.step = this.setting.Step;
			this.reverseStep = this.setting.ReverseStep;
            this.animationLoopType = this.setting.AnimationLoopType;

			if (this.setting.AnimationType == RadAnimationType.ByStartEndValues)
			{
				if (this.step == null ||
					 this.setting.AnimationType == RadAnimationType.ByStartEndValues &&
					(this.setting.StartValue == null ||
					this.setting.StartValueIsCurrentValue)
					)
				{
					if (this.setting.EndValue == null)
					{
						throw new InvalidOperationException("Error calculating animation step: EndValue is not specified for property '" + this.setting.Property.FullName + "' ");
					}

					object startValue = this.CachedStartValue;

					AnimationValueCalculator calculator = AnimationValueCalculatorFactory.GetCalculator(this.setting.Property.PropertyType);
					if (calculator != null)
					{
						this.step = calculator.CalculateAnimationStep(startValue, this.setting.EndValue, this.setting.NumFrames);
						this.reverseStep = calculator.CalculateInversedStep(step);
					}
					else
					{
						throw new Exception("Error calculating animation step because there is not any calculator for type '" + this.setting.Property.PropertyType + "' registered.");
					}
				}
			}

			if (this.reverseStep == null)
			{
				AnimationValueCalculator calculator = AnimationValueCalculatorFactory.GetCalculator(this.setting.Property.PropertyType);
				if (calculator != null)
				{
					this.reverseStep = calculator.CalculateInversedStep(step);
				}
			}			
		}

		public void Stop()
		{
			this.StopTimer();
		}

		public void SettingRemoving()
		{
			if (this.timer != null)
			{
				this.StopTimer();
				if (this.currentState != AnimationState.NotRunning)
				{
                    if (setting.SkipToEndValueOnReplace)
                    {
                        setting.OnSettingRemoving(element);
                        if (this.currentFrame < numFrames)
                            //    //Jump to the last frame
                            this.PerformChangeValue(numFrames);
                    }
				}
			}
		}

        public bool IsRunning
        {
            get { return timer != null && timer.IsRunning; }
        }

        internal LoopType AnimationLoopType
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
	}
}
