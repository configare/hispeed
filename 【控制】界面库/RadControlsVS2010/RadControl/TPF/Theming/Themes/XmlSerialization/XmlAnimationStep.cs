using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Telerik.WinControls
{
    //[Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XmlAnimationStep
    {
        private string stepType;
        private string value;

        [XmlAttribute]
        public string StepType
        {
            get { return this.stepType; }
            set { this.stepType = value; }
        }

        public XmlAnimationStep()
        {
        }

        public XmlAnimationStep(object actualStep)
        {
            this.StepType = XmlTheme.SerializeType(actualStep.GetType());
            this.Value = XmlAnimatedPropertySetting.ConvertValueToString(actualStep);
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
