using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Telerik.WinControls
{

    /// <summary>
    /// Represents a serializable correspondence to the SimpleCondtion class.
    /// </summary>
    //[Serializable]
    //[XmlInclude(typeof(UnaryOperator))]
    public class XmlSimpleCondition : XmlCondition
    {
        private UnaryOperator unaryOperator = UnaryOperator.None;
        private XmlPropertySetting setting = new XmlPropertySetting();
        /// <summary>
        /// Gets or sets a value indicating the UnaryOperator used in the condition.
        /// </summary>
		[Description("Unary operator to apply when comparing property with value given")]
        [DefaultValue(UnaryOperator.None)]
        public UnaryOperator UnaryOperator
        {
            get { return unaryOperator; }
            set { unaryOperator = value; }
        }
        /// <summary>
        /// Gets or sets the XML property setting for the instance.
        /// </summary>
		[Description("Property and value to compare")]
        public XmlPropertySetting Setting
        {
            get { return setting; }
            set { setting = value; }
        }
        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is XmlSimpleCondition)
                return this.setting == (obj as XmlSimpleCondition).Setting &&
                       this.unaryOperator == (obj as XmlSimpleCondition).UnaryOperator;
            
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void DeserializeProperties(Condition selector)
        {
            //
        }

        protected override Condition CreateInstance()
        {
            IPropertySetting setting = this.Setting.Deserialize();
            return new SimpleCondition(setting, this.UnaryOperator);
        }
    }
}
