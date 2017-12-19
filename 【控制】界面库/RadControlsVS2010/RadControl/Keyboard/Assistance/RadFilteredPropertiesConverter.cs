using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls.Assistance
{
	class RadFilteredPropertiesConverter : ExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
			PropertyDescriptorCollection newProperties = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor property in properties)
			{
				if (property.Name == "TipItems" &&
					property.PropertyType == typeof(RadItemReadOnlyCollection))
				{
					newProperties.Add(property);
				}
			}
			return newProperties;
		}
	}
}
