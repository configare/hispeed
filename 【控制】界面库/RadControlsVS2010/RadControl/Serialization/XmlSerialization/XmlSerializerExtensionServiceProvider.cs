using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public class XmlSerializerExtensionServiceProvider : IServiceProvider, IProvideTargetValue
    {
        private IPropertiesProvider propertiesProvider;
        private object targetObject;
        private object targetProperty;
        private string sourceValue;

        public XmlSerializerExtensionServiceProvider(IPropertiesProvider propertiesProvider, object targetObject, object targetProperty, string sourceValue)
        {
            this.propertiesProvider = propertiesProvider;
            this.targetObject = targetObject;
            this.sourceValue = sourceValue;
            this.targetProperty = targetProperty;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IProvideTargetValue))
            {
                return this;
            }
            else if (serviceType == typeof(IPropertiesProvider))
            {
                return this.propertiesProvider;
            }

            return null;
        }

        #region IProvideTargetValue Members

        public object TargetObject
        {
            get { return targetObject; }
        }

        public object TargetProperty
        {
            get { return targetProperty; }
        }

        public string SourceValue
        {
            get { return sourceValue; }
        }

        #endregion
    }
}
