using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Styles.PropertySettings;

namespace Telerik.WinControls.XmlSerialization
{
    public class ParameterReferenceExtension : XmlSerializerExtention, IValueProvider
    {
        private string themePropertyName;
        private IPropertiesProvider styleParameterSerivce;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideTargetValue valueService = IServiceProviderHelper<IProvideTargetValue>.GetService(serviceProvider, typeof(ParameterReferenceExtension).ToString());

            styleParameterSerivce = IServiceProviderHelper<IPropertiesProvider>.GetService(serviceProvider, typeof(ParameterReferenceExtension).ToString());

            this.themePropertyName = valueService.SourceValue.Trim();
            if (string.IsNullOrEmpty(themePropertyName))
            {
                throw new InvalidOperationException("The first argument of RelativeColor exptrssion should be the name of the ThemeProperty");
            }

            return this;
        }

        public object OriginalValue
        {
            get
            {
                return styleParameterSerivce.GetPropertyValue(this.themePropertyName);
            }
        }

        #region IValueProvider Members

        public object GetValue()
        {
            return styleParameterSerivce.GetPropertyValue(this.themePropertyName);
        }

        #endregion
    }
}
