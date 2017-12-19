using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines the types of registrations of a StyleSheet in the ThemeResolutionService.
    /// </summary>
    [Serializable]
    public enum BuilderRegistrationType
    {
		ElementTypeGlobal,
        ElementNameControlName,
        ElementNameControlType,
        ElementTypeControlName,
        ElementTypeControlType,
        ElementTypeDefault
    }

	public class XmlObsoleteOnlyRead : Attribute
	{
	}

    public class StyleBuilderRegistration
    {
		private List<RadStylesheetRelation> stylesheetRelations;
        private StyleBuilder builder;

		public StyleBuilderRegistration(
			StyleBuilder builder)
		{
			this.builder = builder;
		}

        public StyleBuilderRegistration(
            BuilderRegistrationType registrationType,
            string elementType,
			string controlType,
            string elementName,
            string controlName,
            StyleBuilder builder)
        {
            this.builder = builder;

			this.stylesheetRelations = new List<RadStylesheetRelation>(1);
			RadStylesheetRelation relation = new RadStylesheetRelation();
			this.stylesheetRelations.Add(relation);

			relation.RegistrationType = registrationType;
			relation.ElementName = elementName;
			relation.ControlName = controlName;
			relation.ControlType = controlType;
			relation.ElementType = elementType;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    	public List<RadStylesheetRelation> StylesheetRelations
    	{
    		get 
			{
				if (this.stylesheetRelations == null)
				{
					this.stylesheetRelations = new List<RadStylesheetRelation>();
				}
				return this.stylesheetRelations; 
			}
    	}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    	public StyleBuilder Builder
    	{
    		get { return builder; }
    	}

    	public void AddStylesheetRelation(BuilderRegistrationType type, string elementType, string controlType, string elementName, string controlName)
    	{
			RadStylesheetRelation relation = new RadStylesheetRelation(type, elementType, controlType, elementName, controlName);

			this.StylesheetRelations.Add(relation);
    	}
    }
}
