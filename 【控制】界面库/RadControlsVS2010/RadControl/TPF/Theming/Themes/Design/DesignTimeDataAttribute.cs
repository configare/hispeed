using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes.Design
{
    ///<exclude/>
    /// <summary>Represents Theme Designer Data Attribute.</summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class RadThemeDesignerDataAttribute : Attribute
	{
		private Type designTimeDataType;

        /// <summary>Initializes a new instance of the RadThemeDesignerDataAttribute.</summary>
		public RadThemeDesignerDataAttribute(Type designTimeDataType)
		{
			this.designTimeDataType = designTimeDataType;
		}

        /// <summary>Gets or sets data data type.</summary>
		public Type DesignTimeDataType
		{
			get { return designTimeDataType; }
			set { designTimeDataType = value; }
		}
	}  
}
