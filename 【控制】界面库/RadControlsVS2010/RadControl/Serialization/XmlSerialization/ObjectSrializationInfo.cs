using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls.Styles.PropertySettings;

namespace Telerik.WinControls.XmlSerialization
{
    internal class ObjectSerializationInfo
    {
        private Type objectType;
        private bool isSerializedAsString;
        private RadProperty radProperty;
        private SerializationConverter converter;
        private TypeConverter typeConverter;
        private object propertyOwner;
        private PropertyDescriptor property;

        private bool shouldWaitForAttribute = false;

        private object currentPropertyValue;
        private string tempStringValueFromDeserialization;

        private static Dictionary<string, Type> builtInExtensionMap = null;
        private ComponentXmlSerializer serializer;

        private ObjectSerializationInfo(ComponentXmlSerializer serializer)
        {
            this.serializer = serializer;
        }

        public ObjectSerializationInfo(ComponentXmlSerializer serializer, SerializationConverter converter, object propertyOwner, PropertyDescriptor property)
            : this(serializer)
        {
            this.converter = converter;
            this.propertyOwner = propertyOwner;
            this.property = property;

            if (converter != null)
            {
                InitializeConverters();
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        private void InitializeConverters()
        {
            //call this to let special converters like the one for XmlPropertySetting to get the actual converter from 
            //the actaul serialized RadPoperty
            this.objectType = this.converter.GetActualPropertyType(this.propertyOwner, this.Property);

            //since "Property" attribute carries information how "Value" attribute should be processed, 
            //this fix ensures that order of attributes does not brake the parser
            if (objectType == null)
            {
                if (!shouldWaitForAttribute)
                {
                    shouldWaitForAttribute = true;
                    return;
                }
                else //wait only once
                {
                    throw new InvalidOperationException("Attribute PropertyName of a PropertySetting not found during deserialization.");
                }
            }

            //test if there is RadProperty
            this.radProperty = this.converter.GetRadProperty(this.propertyOwner, this.Property);

            TypeConverter newTypeConverter = null;

            if (this.radProperty != null)
            {
                PropertyDescriptor clrProperty = this.radProperty.FindClrProperty();
                if (clrProperty != null)
                {
                    newTypeConverter = clrProperty.Converter;
                }
            }

            if (newTypeConverter == null)
            {
                newTypeConverter = TypeDescriptor.GetConverter(objectType);
            }

            if (newTypeConverter.CanConvertFrom(typeof(string)) && newTypeConverter.CanConvertTo(typeof(string)))
            {
                this.isSerializedAsString = true;
                this.typeConverter = newTypeConverter;
                //mark to use typeConverter instaed of given converter
                this.converter = null;
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        public ObjectSerializationInfo(ComponentXmlSerializer serializer, TypeConverter typeConverter, object propertyOwner, PropertyDescriptor property)
            : this(serializer)
        {
            if (typeConverter != null)
            {
                currentPropertyValue = this.serializer.GetPropertyValue(property, propertyOwner);

                this.objectType = currentPropertyValue.GetType();
                this.isSerializedAsString = true;
                this.radProperty = null;
                this.typeConverter = typeConverter;
                this.propertyOwner = propertyOwner;
                this.property = property;
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        public ObjectSerializationInfo(ComponentXmlSerializer serializer, Type objectType, TypeConverter typeConverter, object propertyOwner, PropertyDescriptor property)
            : this(serializer)
        {
            if (objectType != null)
            {
                this.objectType = objectType;
                this.isSerializedAsString = typeConverter != null || objectType == typeof (string);
                this.radProperty = null;
                this.typeConverter = typeConverter;
                this.propertyOwner = propertyOwner;
                this.property = property;
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        public ObjectSerializationInfo(ComponentXmlSerializer serializer, Type objectType, TypeConverter typeConverter, object currentValue)
            : this(serializer)
        {
            if (objectType != null)
            {
                this.objectType = objectType;
                this.isSerializedAsString = typeConverter != null || objectType == typeof(string);
                this.radProperty = null;
                this.typeConverter = typeConverter;
                this.currentPropertyValue = currentValue;
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        public ObjectSerializationInfo(ComponentXmlSerializer serializer, Type objectType, object currentValue)
            : this(serializer)
        {
            if (objectType != null)
            {
                this.objectType = objectType;
                this.isSerializedAsString = objectType == typeof(string);
                this.radProperty = null;
                this.currentPropertyValue = currentValue;
            }
            else
            {
                this.isSerializedAsString = false;
            }
        }

        public Type ObjectType
        {
            get { return objectType; }
        }

        public bool IsSerializedAsString
        {
            get { return isSerializedAsString; }
        }

        public RadProperty RadProperty
        {
            get { return radProperty; }
        }

        public bool ShouldWaitForAttribute
        {
            get { return shouldWaitForAttribute; }
        }

        public PropertyDescriptor Property
        {
            get { return property; }
        }

        public string ConvertToString()
        {
            object propertyValue = this.GetCurrPropertyValue();

            //Fix for converting value srom serialization extensions
            //TODO - change this to use Extension Map, similar to DelegateParsingToExtender and so on
            IValueProvider valueProvider = propertyValue as IValueProvider;
            if (valueProvider != null)
            {
                propertyValue = valueProvider.GetValue();
            }

            if (converter != null)
            {
                return converter.ConvertToString(this.propertyOwner, this.Property, propertyValue);
            }
            else
                if (typeConverter != null)
                {
                    return typeConverter.ConvertToString(null, CultureInfo.InvariantCulture, propertyValue);
                }
                else
                    if (objectType == typeof(string))
                    {
                        return (string)propertyValue;
                    }

            return null;
        }

        public object GetCurrPropertyValue()
        {
            if (this.currentPropertyValue == null)
            {
                return this.serializer.GetPropertyValue(Property, this.propertyOwner);
            }

            return this.currentPropertyValue;
        }

        public object ConvertFromString(string value)
        {
            //Delegate parsing to extender if available
            if (!string.IsNullOrEmpty(value) && value.Length > 2)
            {
                string trimmed = value.Trim();

                if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
                {
                    object retValue = null;
                    bool parsingDelegatedToExtender = this.DelegateParsingToExtender(trimmed, this.propertyOwner, objectType, typeConverter, out retValue);
                    if (parsingDelegatedToExtender)
                    {
                        return retValue;
                    }
                }
            }

            if (converter != null)
            {
                return converter.ConvertFromString(this.propertyOwner, this.Property, value);
            }
            else
                if (typeConverter != null)
                {
                    return typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                }
                else
                    if (objectType == typeof(string))
                    {
                        return value;
                    }

            return null;
        }        

        private static Dictionary<string, Type> GetBuiltInExtensionMap()
        {
            if (builtInExtensionMap == null)
            {
                builtInExtensionMap = new Dictionary<string, Type>();
                builtInExtensionMap.Add("RelativeColor", typeof(ColorBlendExtension));
                builtInExtensionMap.Add("ColorBlendExtension", typeof(ColorBlendExtension));
                builtInExtensionMap.Add("ParamRef", typeof(ParameterReferenceExtension));
                builtInExtensionMap.Add("ParameterReferenceExtension", typeof(ParameterReferenceExtension));
            }

            return builtInExtensionMap;
        }
            
        private bool DelegateParsingToExtender(string sourceValue, object targetObject, Type targetObjectType, TypeConverter originalTypeConverter, out object retValue)
        {
            //sample extender strings
            //  {RelativeColor: BaseColor, 0, 10, -10, +30 }
            //  {ParamRef: GradientPercentage}

            retValue = null;

            int separatorIndex = sourceValue.IndexOf(":");
            if (separatorIndex < 2)
            {
                return false;
            }

            string extension = sourceValue.Substring(1, separatorIndex - 1);

            //extract only the value part, excluding extention name and {} brackets: BaseColor, 0, 10, -10, +30
            string valueToParse = sourceValue.Substring(separatorIndex + 1, sourceValue.Length - separatorIndex - 2);

            //MessageBox.Show("extender found: '" + extension + "'");
            //MessageBox.Show("value found: '" + valueToParse + "'");

            Type extensionType;
            GetBuiltInExtensionMap().TryGetValue(extension, out extensionType);

            if (extensionType == null)
            {
                return false;
            }

            if (!typeof(XmlSerializerExtention).IsAssignableFrom(extensionType))
            {
                throw new NotSupportedException(string.Format("Xml extension type not supported {0}. Extension type must inherit {1}.", extensionType, typeof(XmlSerializerExtention)));
            }

            XmlSerializerExtention targetExtention = (XmlSerializerExtention)Activator.CreateInstance(extensionType);

            XmlSerializerExtensionServiceProvider serviceProvider = new XmlSerializerExtensionServiceProvider(
                this.serializer.PropertiesProvider,
                targetObject,
                property,
                valueToParse
                );

            retValue = targetExtention.ProvideValue(serviceProvider);            

            return true;
        }

        public void SetTempStringValueFromDeserialization(string value)
        {
            this.tempStringValueFromDeserialization = value;
        }

        public object ConvertFromDelayedDeserialization()
        {
            this.InitializeConverters();
            return this.ConvertFromString(this.tempStringValueFromDeserialization);
        }
    }    
}
