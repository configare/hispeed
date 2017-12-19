using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using Telerik.WinControls.Styles.PropertySettings;

namespace Telerik.WinControls
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertySetting : PropertySettingBase
    {        
        ValueProviderHelper valueProviderHelper = new ValueProviderHelper();

        public object Value
        {
            get
            {
                return valueProviderHelper.Value;
            }
            set
            {
                valueProviderHelper.Value = value;
            }
        }

        /// <summary>
        /// Empties the ValuesPerThread cache.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CleanValuesPerThread()
        {
            this.valueProviderHelper.CleanValuesPerThread();
        }

        public PropertySetting()
        {
        }

        public PropertySetting(RadProperty property, object value): base(property)
        {
            this.Value = value;
        }

        public override object GetCurrentValue(RadObject forObject)
        {
            return this.Value;
        }
        
        public override void ApplyValue(RadElement element)
        {
            base.RegisterStyleValueBase(element);
        }

        public override void UnapplyValue(RadElement element)
        {
            base.UnapplyStyleValueBase(element);
        }

        public override void UnregisterValue(RadElement element)
        {
            base.UnregisterStyleValueBase(element);
        }

        protected override XmlPropertySetting Serialize()
        {
            XmlPropertySetting xmlPropertySetting = new XmlPropertySetting();

            xmlPropertySetting.Property = this.Property.FullName;
            xmlPropertySetting.Value = this.valueProviderHelper.UnderlayingValue;//XmlPropertySetting.SerializeValue(this.Property, this.Value);

            return xmlPropertySetting;
        }

        /// <summary>
        /// This method supports internal TPF infrastructure and is not intended for use elsewhere
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
	    public IValueProvider GetValueProvider()
	    {
            return this.valueProviderHelper.ValueProvider;
            //if (this.valueProviderHelper.ValueProvider != null)
            //{
            //    this.valueProviderHelper.Value = this.valueProviderHelper.ValueProvider.OriginalValue;
            //}
	    }

        public override string ToString()
        {
            return this.Property.OwnerType.Name + "." + this.Property.Name + " = " + (this.Value != null ? this.Value.ToString() : "null");
        }

        #region Clone support

        public override object Clone()
        {
            PropertySetting result = new PropertySetting(this.Property, this.Value);
            result.valueProviderHelper = this.valueProviderHelper;

            return result;
        }

        #endregion
    }
}
