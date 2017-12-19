using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    public class PropertySerializationMetadata
    {
        private string propertyName;
        private string typeFullName;
        private Type type;
        private AttributeCollection attributes;

        private bool? shouldSerializeProperty = null;

        public PropertySerializationMetadata(string typeFullName, string propertyName, params Attribute[] attributes)
            :
            this(typeFullName, propertyName, new AttributeCollection(attributes))
        {
        }

        public PropertySerializationMetadata(Type targetType, string propertyName, params Attribute[] attributes)
            :
            this(targetType, propertyName, new AttributeCollection(attributes))
        {
        }

        public PropertySerializationMetadata(string targetTypeFullName, string propertyName, AttributeCollection attributes)
        {
            this.propertyName = propertyName;
            this.attributes = attributes;
            this.typeFullName = targetTypeFullName;
        }

        public PropertySerializationMetadata(string targetTypeFullName, string propertyName, bool shouldSerialize)
        {
            this.propertyName = propertyName;            
            this.typeFullName = targetTypeFullName;
            this.shouldSerializeProperty = shouldSerialize;
        }

        public PropertySerializationMetadata(Type targetType, string propertyName, AttributeCollection attributes)
        {
            this.propertyName = propertyName;
            this.attributes = attributes;
            this.type = targetType;
        }

        public PropertySerializationMetadata(Type targetType, string propertyName, bool shouldSerialize)
        {
            this.propertyName = propertyName;
            this.type = targetType;
            this.shouldSerializeProperty = shouldSerialize;
        }

        public string PropertyName
        {
            get { return propertyName; }
        }

        public string TypeFullName
        {
            get
            {
                if (type != null)
                {
                    return this.type.FullName;
                }

                return typeFullName;
            }
        }

        public Type Type
        {
            get { return type; }
        }

        public AttributeCollection Attributes
        {
            get
            {
                if (attributes == null)
                {
                    attributes = new AttributeCollection();
                }

                return attributes;
            }
            set
            {
                this.attributes = value;
            }
        }

        public virtual bool? ShouldSerializeProperty
        {
            get { return shouldSerializeProperty; }
            set { shouldSerializeProperty = value; }
        }
    }
}
