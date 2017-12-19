using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing;
using Telerik.WinControls.Themes.XmlSerialization;
using Telerik.WinControls.XmlSerialization;

namespace Telerik.WinControls
{
    //[Serializable]
    //[XmlInclude(typeof(AnimatorStyles))]
    public class XmlAnimatedPropertySetting : XmlPropertySetting
    {
        private AnimatorStyles animatorStyle = AnimatorStyles.AnimateAlways;
        private LoopType animationLoopType = LoopType.None;
        private int numFrames = 5;
        private XmlAnimationStep step;
        private XmlAnimationStep reverseStep;
        private int interval = 50;
        private RadAnimationType animationType = RadAnimationType.ByStartEndValues;
        private object endValue;
		private bool startValueIsCurrentValue;

		private RadEasingType applyEasingType = RadEasingType.OutQuad;
		private RadEasingType unApplyEasingType = RadEasingType.OutQuad;

        [XmlAttribute]
        [DefaultValue(LoopType.None)]
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

        [XmlAttribute]
        [DefaultValue(AnimatorStyles.AnimateAlways)]
        public AnimatorStyles AnimatorStyle
        {
            get { return animatorStyle; }
            set { animatorStyle = value; }
        }

		[XmlAttribute]
		public bool StartValueIsCurrentValue
		{
			get { return startValueIsCurrentValue; }
			set { startValueIsCurrentValue = value; }
		}

        private bool ShouldSerializeStartValueIsCurrentValue()
        {
            return (this.startValueIsCurrentValue && this.Value != null) || (this.Value == null && !startValueIsCurrentValue);
        }

        [DefaultValue(null)]
        public XmlAnimationStep Step
        {
            get{ return this.step; }
            set { this.step = value; }
        }

        [DefaultValue(null)]
        public XmlAnimationStep ReverseStep
        {
            get { return reverseStep; }
            set { reverseStep = value; }
        }

        [DefaultValue(RadEasingType.OutQuad)]
		public RadEasingType ApplyEasingType
		{
			get { return applyEasingType; }
			set { applyEasingType = value; }
		}

        [DefaultValue(RadEasingType.OutQuad)]
		public RadEasingType UnapplyEasingType
		{
			get { return unApplyEasingType; }
			set { unApplyEasingType = value; }
		}

        [Editor(typeof(SettingValueEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(SettingValueConverter))]
        [SerializationConverter(typeof(XmlPropertySettingValueConverter))]
        public object EndValue
        {
            get { return endValue; }
            set { endValue = value; }
        }

        [XmlAttribute]
        [DefaultValue(50)]
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        [XmlAttribute]
        [DefaultValue(5)]
        public int NumFrames
        {
            get { return numFrames; }
            set { numFrames = value; }
        }

        [XmlAttribute]
        [DefaultValue(RadAnimationType.ByStartEndValues)]
        public RadAnimationType AnimationType
        {
            get { return animationType; }
            set { animationType = value; }
        }

        public static string ConvertValueToString(object value)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType = value.GetType();

            //convert value using calculator
            AnimationValueCalculator calc = AnimationValueCalculatorFactory.GetCalculator(valueType);
            return calc.ConvertAnimationStepToString(value);
        }

        private static object ConvertValueFromString(Type valueType, string value)
        {
            if (value == null)
            {
                return null;
            }

            //convert value using calculator
            AnimationValueCalculator calc = AnimationValueCalculatorFactory.GetCalculator(valueType);
            return calc.ConvertToAnimationStepFromString(value);
        }

        public override IPropertySetting Deserialize()
        {
            AnimatedPropertySetting setting = new AnimatedPropertySetting();
            setting.Property = DeserializeProperty(this.Property);
			setting.StartValueIsCurrentValue = this.StartValueIsCurrentValue | this.Value == null;
			if (!this.StartValueIsCurrentValue)
			{
                setting.StartValue = base.GetConvertedValue(setting.Property, this.Value); //DeserializeValue(setting.Property, this.Value);                
			}			

            setting.Interval = this.Interval;
            setting.NumFrames = this.NumFrames;
            setting.AnimationType = this.AnimationType;
            setting.AnimationLoopType = this.AnimationLoopType;
			setting.ApplyEasingType = this.ApplyEasingType;
			setting.UnapplyEasingType = this.UnapplyEasingType;

            setting.EndValue = base.GetConvertedValue(setting.Property, this.EndValue); //DeserializeValue(setting.Property, this.EndValue);            

            if (this.Step != null)
            {
                setting.Step = DeserializeStep(this.Step);
            }

            if (this.ReverseStep != null)
            {
                setting.ReverseStep = DeserializeStep(this.ReverseStep);
            }

            return setting;
        }


        public static object DeserializeStep(XmlAnimationStep step)
        {
            Type type = RadTypeResolver.Instance.GetTypeByName(step.StepType, true);

            return ConvertValueFromString(type, step.Value);
        }

        public static XmlAnimationStep SerializeStep(object step)
        {
            Type type = step.GetType();

            XmlAnimationStep res = new XmlAnimationStep();
            res.StepType = type.FullName;

            res.Value = ConvertValueToString(step);

            return res;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
