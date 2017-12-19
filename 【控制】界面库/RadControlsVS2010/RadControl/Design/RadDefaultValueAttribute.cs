using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Telerik.WinControls.Design
{
    /// <exclude/>
	public class RadDefaultValueAttribute : DefaultValueAttribute
	{
		private string propertyName;
		private Type objectType;

		public RadDefaultValueAttribute(string propertyName, Type objectType)
			: base((string)null)
		{
			this.propertyName = propertyName;
			this.objectType = objectType;
		}

		public override object Value
		{
			get
			{
				try
				{
					DefaultValueAttribute defaultValue = null;
					PropertyInfo info = objectType.GetProperty(this.propertyName);

					if (info != null)
					{
						defaultValue = (DefaultValueAttribute)Attribute.GetCustomAttribute(info, typeof(DefaultValueAttribute), false);
					}

					if (defaultValue != null)
					{
						if (defaultValue != this) 
						{ 
							return defaultValue.Value;
						}
						else
						{
							throw new ArgumentException("Incorrect use of RadDefaultValueAttribute.", info.Name, null);
						}
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