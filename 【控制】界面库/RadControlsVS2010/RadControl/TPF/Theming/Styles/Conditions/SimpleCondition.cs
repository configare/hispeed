using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// SimpleCondition evaluates when a property of an Element equals a certain value.
    /// </summary>
    public class SimpleCondition : Condition
    {
        private IPropertySetting setting;

        private UnaryOperator unaryOperator = UnaryOperator.None;
        /// <summary>
        /// Gets or sets the unary operator of the simple condition.
        /// </summary>
        public UnaryOperator UnaryOperator
        {
            get { return unaryOperator; }
            set { unaryOperator = value; }
        }
        /// <summary>
        /// Initializes a new instance of the SimpleCondition class.
        /// </summary>
        /// <param name="settingToCheck"></param>
        public SimpleCondition(IPropertySetting settingToCheck)
        {
            this.setting = settingToCheck;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleCondition class from the setting to check, and the
        /// unary operator to use.
        /// </summary>
        /// <param name="settingToCheck"></param>
        /// <param name="unaryOperator"></param>
        public SimpleCondition(IPropertySetting settingToCheck, UnaryOperator unaryOperator)
        {
            this.setting = settingToCheck;
            this.unaryOperator = unaryOperator;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleCondition class from the property, value and unary operator
        /// </summary>        
        public SimpleCondition(RadProperty property, object value, UnaryOperator unaryOperator)
        {
            this.setting = new PropertySetting(property, value);
            this.unaryOperator = unaryOperator;
        }

        /// <summary>
        /// Initializes a new instance of the SimpleCondition class from the property and value
        /// </summary>        
        public SimpleCondition(RadProperty property, object value)
        {
            this.setting = new PropertySetting(property, value);
        }

        /// <summary>
        /// Gets or sets the setting of the current property.
        /// </summary>
        public IPropertySetting Setting
        {
            get { return setting; }
            set { setting = value; }
        }
        /// <summary>
        /// Evaluates the target RadElement using the unary operator.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool Evaluate(RadElement target)
        {
            if (target == null)
            {
                return false;
            }

            switch (this.UnaryOperator)
            {
                case UnaryOperator.None:
                    return (target.GetValue(this.Setting.Property).Equals(this.Setting.GetCurrentValue(target)));
                case UnaryOperator.NotOperator:
                    return !((target.GetValue(this.Setting.Property).Equals(this.Setting.GetCurrentValue(target))));
            }

            return false;
        }

        protected override void FillAffectedProperties(List<RadProperty> inList)
        {
            inList.Add(setting.Property);
        }

        protected override void SerializeProperties(XmlCondition res)
        {
            base.SerializeProperties(res);

            XmlSimpleCondition instance = (XmlSimpleCondition)res;

            instance.Setting = this.Setting.Serialize();
            instance.UnaryOperator = this.UnaryOperator;
        }
        /// <summary>
        /// Creates a Serializable instance of the SimpleCondition class.
        /// </summary>
        /// <returns></returns>
        public override XmlCondition CreateSerializableInstance()
        {
            return new XmlSimpleCondition();
        }
        /// <summary>
        /// Retrieves the string representation of the current instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Setting != null)
            {
                string op = (this.UnaryOperator == UnaryOperator.NotOperator) ? "!" : "";

                return op + this.Setting.Property.FullName;
            }

            return base.ToString();
        }
    }
}
