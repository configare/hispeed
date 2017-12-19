using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.Design
{
	/// <exclude/>
	public class RadDescriptionAttribute : DescriptionAttribute
	{
		private string name;
		private Type objectType;

		public RadDescriptionAttribute(string name, Type objectType)
			: base((string)null)
		{
			this.name = name;
			this.objectType = objectType;
		}

		public override string Description
		{
			get
			{
				try
				{
					DescriptionAttribute description = null;
					MemberInfo info = null;

					info = objectType.GetProperty(this.name);

					if (info == null)
					{
						info = objectType.GetEvent(this.name);

						if (info == null)
							info = objectType.GetMethod(this.name);
					}

					if (info != null)
						description = (DescriptionAttribute) Attribute.GetCustomAttribute(info, typeof(DescriptionAttribute));

					if (description != null)
						return description.Description;
				}
				catch (Exception)
				{
				}

				return base.Description;
			}
		}
	}
}
