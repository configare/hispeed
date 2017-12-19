using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using Telerik.WinControls.Keyboard;
using System.Reflection;
using Telerik.WinControls.Elements;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;

namespace Telerik.WinControls.Commands
{
    public class CommandContextConverter : CommandBaseConverter
    {
        public CommandContextConverter(System.Type type)
            : base(type)
        {
        }

        public override TypeConverter.StandardValuesCollection GetStandardValuesCore(ITypeDescriptorContext context, TypeConverter.StandardValuesCollection collection)
        {
            List<IComponent> list = CommandBaseConverter.DiscoverCommandsContexts(collection);
			if (context != null && list != null && (context.GetService(typeof(IDesignerHost)) != null))
			{
				for (int i = (list.Count-1); i >=0; i--)
				{
					if (list[i].Site == null)
					{
						 list.RemoveAt(i);
					}
				}
				list.Insert(0, null);
			}
            return new TypeConverter.StandardValuesCollection(list);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            //context.OnComponentChanged()
            if (context.Instance is InputBinding)
            {
                TypeConverter.StandardValuesCollection collection1 = base.GetComponentsReferences(context);
                return this.GetStandardValuesCore(context, collection1);
            }
            return base.GetStandardValues(context);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            string text1 = ((string)value).Trim();
            if (!string.Equals(text1, CommandBaseConverter.none) && (context != null))
            {
                List<IComponent> sourceList =
                    CommandBaseConverter.DiscoverCommandsContexts(base.GetComponentsReferences(context));
                if (sourceList != null &&
                    sourceList.Count > 0)
                {
                    for (int i = 0; i < sourceList.Count; i++)
                    {
                        string name = sourceList[i].ToString();
                        if (text1.Equals(name))
                        {
                            return sourceList[i];
                        }
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("Destination Type is not defined.");
            }
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            if (value == null)
            {
                return CommandBaseConverter.none;
            }
            if (context != null)
            {
                if (value is IComponent)
                {
                    try
                    {
                        return (value as IComponent).Site.Name;
                    }
                    catch (Exception)
                    {
                        return CommandBaseConverter.none;
                    }
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
            return string.Empty;
        }
    }
}