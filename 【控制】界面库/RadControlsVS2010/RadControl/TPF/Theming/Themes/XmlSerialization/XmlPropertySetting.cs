using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Telerik.WinControls.Themes.Serialization;
using Telerik.WinControls.Themes.XmlSerialization;
using Telerik.WinControls.XmlSerialization;
using System.Diagnostics;

namespace Telerik.WinControls
{	
    /// <summary>
    /// Represents a property setting. Each property of Telerik controls can be 
    /// serialized and deserialized through an instance of this class. The 
    /// XMLPropertySetting instance describes the affected control, its property, and
    /// the current value. XmlPropertySetting is very similar to CSS style properties.
    /// </summary>
    //[Serializable]
    //[XmlInclude(typeof(XmlAnimatedPropertySetting))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XmlPropertySetting: ISerializationValidatable
    {
        private string property;
        private object value;
        private string stringValue;

        private bool serializeValueAsString = false;

        public XmlPropertySetting()
        { }

        public XmlPropertySetting(RadProperty property, object value)
        {
            this.Property = property.FullName;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets a string value indicating the property. 
        /// For example, Telerik.WinControls.VisualElement.ForeColor.
        /// </summary>
        [XmlAttribute]
        [Editor(typeof(SettingPropertyEditor), typeof(UITypeEditor))]
		[Description("RadProperty string representation, consisting of property owner type full name, and property name")]
        public string Property
        {
            get { return property; }
            set
            {
                this.InvalidateTempValues();
				if (string.IsNullOrEmpty(value))
				{
					throw new InvalidOperationException("Property cannot be null or empty");
				}
                property = value; 				
            }
        }

        /// <summary>
        /// Gets or sets an object value indicating the value of the property. For example,
        /// the value of Telerik.WinControls.VisualElement.ForeColor property 
        /// could be "navy" or "128, 0, 255, 63".
        /// </summary>
        /// <remarks>
        /// Here is how XmlPropertySetting determines whether to serialize Value or ValueString property when used in 
        /// ThemeComponent with CodeDom serialization.
        /// 
        /// If the property belongs to an object from System, Telerik.WinControl or Telerik.WinCotnrols.UI assembly
        /// then Value will be serialized (the actual object). For values that are defined in other assemblies ValueString 
        /// will be serialized. Tthis is Value, serialized as string using the TypeConverter specified by the corresponing RadProperty.
        /// This is important for late-binding for types declared in different assemblies: egg. Docking
        /// Otherwise a problem will occur when adding a ThemeComponent on the Form in a project which does not 
        /// reference Docking or Grid, etc, or custom controls assembly.
        /// 
        /// For xml serializtion, property serialize always as string using the TypeConverter specified by the corresponing RadProperty.
        /// </remarks>
        [Editor(typeof(SettingValueEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(SettingValueConverter))]
        [SerializationConverter(typeof(XmlPropertySettingValueConverter))]
		[Description("RadProperty value to use when assigning or comparing")]
        //[DefaultValue(null)]
        public object Value
        {
            get
            {
                if (value == null && this.stringValue != null)
                {
                    RadProperty prop = this.GetDeserializedProperty();
                    if (prop != null)
                    {
                        value = DeserializeValue(prop, this.stringValue);
                    }
                    else
                    {
                        throw new InvalidOperationException("Deserialized XmlProperty setting has no property assigned");
                    }
                }

                return value;
            }
            set
            {
                this.InvalidateTempValues();
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the value serialized to string using the corresponding property TypeConverter. Generally used in rear cases by CodeDom 
        /// Serializer, if theme is serializaed to CodeDom
        /// </summary>
        [Browsable(false)]
        public string ValueString
        {
            get
            {
                RadProperty prop = this.GetDeserializedProperty();
                if (prop != null)
                {
                    return SerializeValue(prop, this.value);
                }

                return null;
            }
            set
            {
                this.stringValue = value;
            }
        }

        private bool ShouldSerializeValue()
        {
            bool res = true;

            if (string.IsNullOrEmpty(this.property))
            {
                return true;
            }

            res = !serializeValueAsString;

            return res;
        }

        private bool ShouldSerializeValueString()
        {
            bool res = true;

            if (string.IsNullOrEmpty(this.property))
            {
                return false;
            }

            res = serializeValueAsString;

            return res;
        }

        static bool duplicateTelerikAssembliesDetected = false;
        private static Type RadObjectType = typeof(RadObject);

        /// <summary>
        /// Deserializes the property given as a string. For example, 
        /// Telerik.WinControls.VisualElement.ForeColor.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static RadProperty DeserializeProperty(string property)
        {
            return DeserializePropertyCore(property, true, true);
        }

        /// <summary>
        /// Deserializes the property given as a string. For example, 
        /// Telerik.WinControls.VisualElement.ForeColor.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
		public static RadProperty DeserializePropertySafe(string property)
		{
			return DeserializePropertyCore(property, true, false);
		}

        private static RadProperty DeserializePropertyCore(string property, bool fallback, bool throwOnError)
        {
            if (string.IsNullOrEmpty(property))
            {
                return null;
            }

            string[] propertyParts = property.Split('.');
            string propertyName;
            string className;
            if (propertyParts.Length > 1)
            {
                propertyName = propertyParts[propertyParts.Length - 1];
                className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
            }
            else
            {
                if (throwOnError)
                {
                    throw new Exception("Invalid property parts");
                }
                return null;
            }

            Type currentType = RadTypeResolver.Instance.GetTypeByName(className);
            if (currentType == null)
            {
                if (throwOnError)
                {
                    throw new InvalidOperationException("Could not look-up type '" + className + "'");
                }
                return null;
            }

            RadProperty radProperty = FindProperty(currentType, propertyName, fallback);
            if (radProperty != null)
            {
                return radProperty;
            }

            if (!throwOnError)
            {
                return null;
            }

            return ProcessDuplicateAssemblies(propertyName, className);
        }

        private static RadProperty FindProperty(Type currentType, string propertyName, bool fallback)
        {
            RadProperty radProperty = RadProperty.FindSafe(currentType, propertyName);
            if (radProperty == null && fallback)
            {
                //Provide Fallback mechanism - e.g. TreeNodeUI.BackColor should fallback to VisualElement.BackColor
                currentType = currentType.BaseType;
                while (currentType != null && currentType != RadObjectType)
                {
                    radProperty = RadProperty.FindSafe(currentType, propertyName);
                    if (radProperty != null)
                    {
                        break;
                    }

                    currentType = currentType.BaseType;
                }
            }

            return radProperty;
        }

        private static RadProperty ProcessDuplicateAssemblies(string propertyName, string className)
        {
            if (duplicateTelerikAssembliesDetected)
            {
                return null;
            }

            Hashtable telerikAssemblies = new Hashtable();

            foreach (RadTypeResolver.LoadedAssembly loadedAssembly in RadTypeResolver.Instance.LoadedAssemblies)
            {
                if (loadedAssembly.isTelerik)
                {
                    Assembly duplicatedAssembly = telerikAssemblies[loadedAssembly.assembly.FullName] as Assembly;
                    if (duplicatedAssembly != null)
                    {
                        MessageBox.Show(
                            string.Format("Visual Studio is attempting to load Telerik assemblies that have different versions: \n {0} \n and \n {1} \n Please remove references to assemblies with incorrect version and restart Visual Studio.",
                            loadedAssembly.assembly.Location,
                            duplicatedAssembly.Location));

                        duplicateTelerikAssembliesDetected = true;
                        return null;
                    }
                    else
                    {
                        telerikAssemblies[loadedAssembly.assembly.FullName] = loadedAssembly.assembly;
                    }
                }
            }

            throw new RadPropertyNotFoundException(propertyName, className);
        }

        /// <summary>
        /// Serializes the given dependency property with the given value. 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeValue(RadProperty property, object value)
        {
            if (value == null)
            {
                return null;
            }

            string res;
            PropertyDescriptor prop = TypeDescriptor.GetProperties(property.OwnerType).Find(property.Name, true);
            if (prop != null)
            {
                res = SerializeValue(prop, value, property.FullName);
            }
            else
            {
                //Attached property
                TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                res = ConvertValueToString(converter, value, property.FullName);
            }

            return res;
        }

        public static string SerializeValue(PropertyDescriptor prop, object value, string propertyDisplayName)
        {
            string res;
            TypeConverter converter = prop.Converter;
            res = ConvertValueToString(converter, value, propertyDisplayName);
            return res;
        }

        private static string ConvertValueToString(TypeConverter converter, object value, string propertyName)
        {
            string res = null;
            if (converter != null)
            {
                //if (!(converter is InnerObjectConverter) && converter.CanConvertTo(typeof(string)))
                if (converter.CanConvertTo(typeof(string)))
                {
                    try
                    {
                        res = (string)converter.ConvertTo(null, AnimationValueCalculatorFactory.SerializationCulture, value, typeof(string));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error setting value: " + ex.Message);
                    }
                }
                else
                {
                    if (value is System.String)
                    {
                        return (string)value;
                    }
                    
                    MessageBox.Show("Can't find TypeConverter to string for property " + propertyName);                    
                }
            }
            else
            {
                MessageBox.Show("Can't find any TypeConverter for property " + propertyName);
            }

            return res;
        }

        private static object ConvertValueFromString(TypeConverter converter, string value, string propertyName, Type propertyType, bool throwOnError)
        {
            object res = null;
            if (converter != null)
            {
                //if (!(converter is InnerObjectConverter) && converter.CanConvertFrom(typeof(string)))
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        res = converter.ConvertFrom(null, AnimationValueCalculatorFactory.SerializationCulture, value);
                    }
                    catch (Exception ex)
                    {
                        if (throwOnError)
                        {
                            throw;
                        }
                        MessageBox.Show("Error setting value: " + ex.Message);
                    }
                }
                else
                {
                    if (propertyType == typeof(string))
                    {
                        return value;
                    }
                    
                    string message = "Can't find TypeConverter from string for property " + propertyName + " of type " + propertyType;
                    if (throwOnError)
                    {
                        throw new InvalidOperationException(message);
                    }

                    MessageBox.Show(message);
                }
            }
            else
            {
                string message = "Can't find any TypeConverter for property " + propertyName;
                if (throwOnError)
                {
                    throw new InvalidOperationException(message);
                }

                MessageBox.Show(message);
            }

            return res;
        }
        /// <summary>
        /// Deserializes the given dependency property with the given value.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object DeserializeValue(RadProperty property, string value)
        {
            return DeserializeValue(property, value, false);
        }

        public static object DeserializeValue(RadProperty property, string value, bool throwOnError)
        {
            if (value == null)
            {
                return null;
            }

            object res;
            PropertyDescriptor prop = TypeDescriptor.GetProperties(property.OwnerType).Find(property.Name, true);
            if (prop != null)
            {
                res = DeserializeValue(prop, value, property.FullName, throwOnError);
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                res = ConvertValueFromString(converter, value, property.FullName, property.PropertyType, throwOnError);
            }

            return res;
        }

        public static object DeserializeValue(PropertyDescriptor prop, string value, string propertyDisplayName, bool throwOnError)
        {
            object res;
            TypeConverter converter = prop.Converter;
            res = ConvertValueFromString(converter, value, propertyDisplayName, prop.PropertyType, throwOnError);
            return res;
        }

        public static object DeserializeValue(Type propertyType, string value, string propertyDisplayName)
        {
            object res;
            TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
            res = ConvertValueFromString(converter, value, propertyDisplayName, propertyType, false);
            return res;
        }

        protected object GetConvertedValue(RadProperty property, object value)
        {
            if (value != null &&
                !property.PropertyType.IsAssignableFrom(value.GetType()) &&
                !(value is Telerik.WinControls.Styles.PropertySettings.IValueProvider))
            {
                try
                {
                    return Convert.ChangeType(value, property.PropertyType);
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        string.Format(
                        "Error changing object type during deserialization. Property {0} of type: {1}, value type: {2}",
                        property.FullName,
                        property.PropertyType,
                        value.GetType().Name
                        ),
                        ex);
                }
            }
            else
            {
                return value;
            }
        }

        public virtual IPropertySetting Deserialize()
        {
			if (string.IsNullOrEmpty(this.Property))
			{
				throw new InvalidOperationException("Property to deserialize is null or empty");
			}

            PropertySetting setting = new PropertySetting();
            setting.Property = DeserializeProperty(this.Property);

            //In previous verions of this class, Value used to be stored as string and deserialized upon retrieval
            //Now Value is the actual object, not string, and deserialization is no longer needed, at least for value types that are stored in System or Telerik assemblies
            //For Types in other assemblies ValueAsString should be used
            setting.Value = this.GetConvertedValue(setting.Property, this.Value); //DeserializeValue(setting.Property, this.Value);

            return setting;
        }

        /// <summary>
        /// Retrieves a string representation of the class.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PropertySetting";
        }
        /// <summary>
        /// Retrieves the name of the property.
        /// </summary>
        /// <returns></returns>
        public string GetPropertyName()
        {
            if(string.IsNullOrEmpty(Property))
                return "unknown";
                
            string[] tokens = Property.Split('.');
            return tokens[tokens.Length - 1];
        }

        private RadProperty tempPoroperty = null;        
        /// <summary>
        /// Retrieves the deserialized property.
        /// </summary>
        /// <returns></returns>
        public RadProperty GetDeserializedProperty()
        {
            if (tempPoroperty == null)
            {
                tempPoroperty = XmlPropertySetting.DeserializeProperty(this.Property);
            }

            return tempPoroperty;
        }

        private void InvalidateTempValues()
        {
            this.tempPoroperty = null;
        }

		#region ISerializationValidatable Members

		void ISerializationValidatable.Validate()
		{
			if (string.IsNullOrEmpty(this.property))
			{
				throw new InvalidOperationException("PropertySetting Property or Value cannot be null");
			}
		}

		#endregion

        public void SetSerializeValueAsString(bool value)
        {
            this.serializeValueAsString = value;
        }
    }
    
    public class SettingValueEditor : UITypeEditor
    {
        private UITypeEditor actualEditor = null;
        private TypeConverter actualConverter = null;
        private RadProperty currProperty;
        private Type actualPropertyType;

        public Type GetActualPropertyType()
        {
            return this.actualPropertyType;
        }

        public RadProperty GetRadProperty()
        {
            return this.currProperty;
        }

        private UITypeEditor GetActualEditor(ITypeDescriptorContext context)
        {
            if (context == null)
            {
                return actualEditor;
            }

            XmlPropertySetting setting = (XmlPropertySetting)context.Instance;
            //Find property
            //setting.Property
            //Find type converter
            //Convert value
            //Find corresponding UITypeEditor
            //Edit converted value                

            if (setting.Property == null)
            {
                this.actualEditor = null;
                return null;
            }

            string[] propertyParts = setting.Property.Split('.');
            string propertyName;
            string className;
            if (propertyParts.Length > 1)
            {
                propertyName = propertyParts[propertyParts.Length - 1];
                className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
            }
            else
            {
                MessageBox.Show("Invalid property name. Property consist of type FullName\".\"PropertyName.");
                this.actualEditor = null;
                return null;
            }

            RadProperty prop = RadProperty.FindSafe(className, propertyName);
            this.currProperty = prop;

            TypeConverter converter;

            if (prop != null)
            {
                converter = TypeDescriptor.GetConverter(prop.PropertyType);
            }
            else
            {
                MessageBox.Show("Can't find property " + setting.Property + ". Property " + propertyName + "not registered for RadObject" + className);
                this.actualEditor = null;
                return null;
            }

            this.actualPropertyType = prop.PropertyType;

            if (converter == null ||
                !converter.CanConvertFrom(typeof(string)) ||
                !converter.CanConvertTo(typeof(string)))
            {
                if (!converter.CanConvertFrom(typeof(string)))
                {
                    MessageBox.Show("Converter can't convert from string");
                }
                else
                    if (!converter.CanConvertTo(typeof(string)))
                    {
                        MessageBox.Show("Converter can't convert to string");
                    }
                    else
                    {
                        MessageBox.Show("Converter for type not found");
                    }

                this.actualEditor = null;
                return null;
            }
            
            this.actualConverter = converter;

            PropertyDescriptor actualProperty = TypeDescriptor.GetProperties(prop.OwnerType).Find(prop.Name, false);

            if (actualProperty != null)
                this.actualEditor = (UITypeEditor)actualProperty.GetEditor(typeof(UITypeEditor));
            else
                this.actualEditor = (UITypeEditor)TypeDescriptor.GetEditor(prop.PropertyType, typeof(UITypeEditor));


            return actualEditor;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            UITypeEditor editor = this.GetActualEditor(context);
            TypeConverter converter = this.actualConverter;

            if (editor == null || converter == null )
            {
                return value;
            }

            if (currProperty == null)
            {
                return value;
            }


            object convertedValue;

            if (value == null || value.GetType() != currProperty.PropertyType)
            {
                if (currProperty.PropertyType.IsValueType)
                {
                    convertedValue = Activator.CreateInstance(currProperty.PropertyType);
                }
                else
                {
                    convertedValue = null;
                }
            }
            else
            {
                convertedValue = converter.ConvertFrom(value);
            }

            object res = value;

            if (editor != null)
            {
                res = editor.EditValue(provider, convertedValue);
                res = converter.ConvertTo(res, typeof(string));
            }

            return res;
        }

        public bool CanEditValue(ITypeDescriptorContext context)
        {
            return this.GetActualEditor(context) != null;
        }
    }

    public class SettingValueConverter: TypeConverter
    {
        private ITypeDescriptorContext currContext;
        private TypeConverter underlayingConverter;
        //private RadProperty actualProperty;
        private bool hasEditor;
    
        private TypeConverter GetUnderlayingConverter(ITypeDescriptorContext context)
        {
            if (underlayingConverter == null || context != currContext)
            {
                currContext = context;
                if (context == null)
                {
                    return underlayingConverter;
                }
                XmlPropertySetting setting = (XmlPropertySetting)context.Instance;
                //Find property
                //setting.Property
                //Find type converter
                //Convert value
                //Find corresponding UITypeEditor
                //Edit converted value

                if (string.IsNullOrEmpty(setting.Property))
                {
                    this.underlayingConverter = null;
                    return null;
                }

                string[] propertyParts = setting.Property.Split('.');
                string propertyName;
                string className;
                if (propertyParts.Length > 1)
                {
                    propertyName = propertyParts[propertyParts.Length - 1];
                    className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
                }
                else
                {
                    throw new Exception("Invalid property name. Property consist of type FullName \".\" and property name.");
                }

                RadProperty prop = RadProperty.FindSafe(className, propertyName);

                //this.actualProperty = prop;

                TypeConverter converter;

                if (prop != null)
                {
                    converter = TypeDescriptor.GetConverter(prop.PropertyType);
                }
                else
                {
                    MessageBox.Show("Can't find property " + setting.Property + ". Property " + propertyName + " not registered for " + className);
                    this.underlayingConverter = null;
                    return null;
                }


                if (converter == null ||
                    !converter.CanConvertFrom(typeof(string)) ||
                    !converter.CanConvertTo(typeof(string)))
                {
                    if (!converter.CanConvertFrom(typeof(string)))
                    {
                        MessageBox.Show("Converter can't convert from string");
                    }
                    else
                        if (!converter.CanConvertTo(typeof(string)))
                        {
                            MessageBox.Show("Converter can't convert to string");
                        }
                        else
                        {
                            MessageBox.Show("Converter for type not found");
                        }
                }


                UITypeEditor editor = (UITypeEditor)TypeDescriptor.GetEditor(prop.PropertyType, typeof(UITypeEditor));
                hasEditor = editor != null;

                underlayingConverter = converter;
            }

            return underlayingConverter;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null)
            {
                return false;
            }

            return actual.CanConvertFrom(sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null)
            {
                return false;
            }

            return actual.CanConvertTo(destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null || hasEditor)
            {
                return false;
            }

            return actual.GetStandardValuesSupported();
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null)
            {
                return base.GetStandardValues(context);
            }

            return (StandardValuesCollection)actual.GetStandardValues();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null)
            {
                return null;
            }

            /*if (value != null && value.GetType() == typeof(string))
            {
                return value;
                //object actualValue = actual.ConvertTo(context, culture, value, GetActualProperty(context).PropertyType);
            }*/

            return actual.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            TypeConverter actual = this.GetUnderlayingConverter(context);
            if (actual == null)
            {
                return null;
            }

            /*if (value != null && value.GetType() == destinationType)
            {
                return value;
                //object actualValue = actual.ConvertFrom(context, culture, value);
            }*/

            return actual.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class SettingPropertyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            TypeTreeForm form = new TypeTreeForm(true, value);
            if (form.ShowDialog() == DialogResult.OK)
                if (form.SelectedTag != null)
                    return (form.ParentSelectedTag as Type).FullName + "." + 
                        (form.SelectedTag as RadProperty).Name;

            return value;
        }
    }
}
