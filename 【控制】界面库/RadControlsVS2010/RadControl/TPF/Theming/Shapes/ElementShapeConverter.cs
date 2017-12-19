using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
	/// <summary>Represents element shape converter.</summary>
	public class ElementShapeConverter : ComponentConverter
	{
		private const string noneString = "(none)";
		static bool shownError = false;

        public ElementShapeConverter(): base(typeof(ElementShape))
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (context != null && context.Container != null)
                return base.ConvertFrom(context, culture, value);

            if (value is string)
            {
                if (noneString.Equals(value))
                    return null;

                try
                {
                    string[] tokens = (value as string).Split('|');

                    if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
                    {
                        return null;
                    }
                    if (shownError)
                    {
                        return null;
                    }
                    ElementShape elementShape = null;
                    Type shapeType = RadTypeResolver.Instance.GetTypeByName(tokens[0].Trim());
                    if (shapeType == null)
                    {
                        return null;
                    }
                    if (!(typeof(ElementShape)).IsAssignableFrom(shapeType))
                    {
                        VSCacheError.ShowVSCacheError((typeof(ElementShape)).Assembly, shapeType.Assembly);
                        shownError = true;
                    }
                    else
                    {
                        elementShape = (ElementShape)Activator.CreateInstance(shapeType);
                        if (tokens.Length > 1)
                        {
                            elementShape.DeserializeProperties(tokens[1]);
                        }
                    }
                    return elementShape;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deserializing custom shape: " + ex.ToString());
                    return null;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (context != null && context.Container != null)
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (value == null && destinationType == typeof(string))
            {
                return noneString;
            }

            if (value == null)
            {
                return null;
            }

            if (destinationType == typeof(string))
            {
                if (typeof(ElementShape).IsAssignableFrom(value.GetType()))
                {
                    string serializedProperties = ((ElementShape)value).SerializeProperties();
                    return value.GetType().FullName + (string.IsNullOrEmpty(serializedProperties) ? "" : "|" + serializedProperties);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
	}
}
