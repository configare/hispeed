using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    /// <summary>
    /// Attribute for telerik theme serialization.
    /// </summary>
    public class SerializationConverterAttribute : Attribute
    {
        private Type converterType = null;

        public SerializationConverterAttribute(Type converterType)
        {
            if (typeof(SerializationConverter).IsAssignableFrom(converterType))
            {
                this.converterType = converterType;
            }
        }

        public Type ConverterType
        {
            get { return converterType; }
        }

        public SerializationConverter GetConverterInstance()
        {
            if (this.converterType == null)
            {
                throw new InvalidOperationException(
                    string.Format("converterType not specified. Please make sure the SerializationConverterAttribute attribute specifies a type that inherits from {0}", typeof(SerializationConverter)));
            }

            return (SerializationConverter)Activator.CreateInstance(converterType);
        }
    }
}
