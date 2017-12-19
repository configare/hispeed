using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Telerik.WinControls.Design
{
    /// <exclude/>
	public class RadPropertyDefaultValueAttribute : DefaultValueAttribute
	{
		private string propertyName;
        // Type of the object that ownes the RadProperty which provides the default value.
		private Type ownerType;

        /// <summary>
        /// RadPropertyDefaultValueAttribute constructor
        /// </summary>
        /// <param name="propertyName">The name of the property which provides the default value.</param>
        /// <param name="ownerType">Type of the object that ownes the RadProperty which provides the default value.</param>
		public RadPropertyDefaultValueAttribute(string propertyName, Type ownerType)
			: base((string)null)
		{
			this.propertyName = propertyName;
			this.ownerType = ownerType;
		}

		public override object Value
		{
			get
			{
				try
				{
					RadObjectType dependencyType = RadObjectType.FromSystemType(this.ownerType);
					RadProperty dependencyProperty = RadTypeResolver.Instance.GetRegisteredRadProperty(this.ownerType, this.propertyName);
					if (dependencyProperty != null && dependencyType != null)
					{
						return dependencyProperty.GetMetadata(dependencyType).DefaultValue;
					}
				}
				catch (Exception)
				{
				}

				return base.Value;
			}
		}
	}
}