using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    //[Serializable]
	public class RadStylesheetRelation
	{
		private BuilderRegistrationType registrationType = BuilderRegistrationType.ElementTypeControlType;
		
		private string controlType = string.Empty;
        private string elementType = string.Empty;
        private string controlName = string.Empty;
        private string elementName = string.Empty;

		/// <summary>
		/// Initializes a new instance of the RadStylesheetRelation class.
		/// </summary>
		public RadStylesheetRelation()
		{
			//
		}

		public RadStylesheetRelation(BuilderRegistrationType type, string elementType, string controlType, string elementName, string controlName)
		{
			this.registrationType = type;
			this.elementType = elementType ?? string.Empty;
            this.controlType = controlType ?? string.Empty;
            this.elementName = elementName ?? string.Empty;
            this.controlName = controlName ?? string.Empty;
		}

        /// <summary>
        /// Determines whether the specified relation is equal to this one.
        /// </summary>
        /// <param name="relation"></param>
        /// <returns></returns>
        public bool Equals(RadStylesheetRelation relation)
        {
            return this.registrationType == relation.registrationType &&
                string.CompareOrdinal(this.elementType, relation.elementType) == 0 &&
                string.CompareOrdinal(this.controlType, relation.controlType) == 0 &&
                string.CompareOrdinal(this.elementName, relation.elementName) == 0 &&
                string.CompareOrdinal(this.controlName, relation.controlName) == 0;
        }

	    /// <summary>
	    /// Gets or sets a value indicating the builder registration type.
	    /// </summary>
	    [XmlAttribute]
	    [DefaultValueAttribute(BuilderRegistrationType.ElementTypeControlType)]
		public BuilderRegistrationType RegistrationType
		{
			get { return registrationType; }
			set { registrationType = value; }
		}
		
		/// <summary>
		/// Gets or sets a string value indicating the control type.
		/// </summary>
		[XmlAttribute]
        [DefaultValue("")]
		public string ControlType
		{
			get { return controlType; }
			set { controlType = value; }
		}
		/// <summary>
		/// Gets or sets a string value indicating the element type.
		/// </summary>
		[XmlAttribute]
        [DefaultValue("")]
		public string ElementType
		{
			get { return elementType; }
			set { elementType = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating the control name.
		/// </summary>
		[XmlAttribute]
        [DefaultValue("")]
		public String ControlName
		{
			get { return controlName; }
			set { controlName = value; }
		}
		/// <summary>
		/// Gets or sets a string value indicating the element name.
		/// </summary>
		[XmlAttribute]
        [DefaultValue("")]
		public String ElementName
		{
			get { return elementName; }
			set { elementName = value; }
		}
	}

    //[Serializable]
    public class RadStyleSheetRelationList : List<RadStylesheetRelation>
    {
        public RadStyleSheetRelationList()
        {
        }

        public RadStyleSheetRelationList(int capacity) : base(capacity)
        {
        }
    }

    /// <summary>
    /// Represents a registration for the Style Builder. The class is responsible 
    /// for the serialization and deserialization of a group of <em>t</em>elerik controls.
    /// </summary>
    //[Serializable]
    //[XmlInclude(typeof(BuilderRegistrationType))]
    //[XmlInclude(typeof(XmlBuilderData))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XmlStyleBuilderRegistration
    {
		private RadStyleSheetRelationList stylesheetRelations;

		private Type builderType;
        private XmlBuilderData builderData;

        /// <summary>
        /// Initializes a new instance of the XmlStyleBuilderRegistration class.
        /// </summary>
        public XmlStyleBuilderRegistration()
        {
			this.builderType = typeof(DefaultStyleBuilder);	
        }

        public XmlStyleBuilderRegistration(StyleBuilderRegistration toCopyFrom)
        {
            this.BuilderType = toCopyFrom.Builder.GetType();
            this.BuilderData = toCopyFrom.Builder.BuilderData;

            foreach (RadStylesheetRelation relationToCopy in toCopyFrom.StylesheetRelations)
            {
                RadStylesheetRelation relation = new RadStylesheetRelation();

                relation.RegistrationType = relationToCopy.RegistrationType;
                relation.ControlName = relationToCopy.ControlName;
                relation.ControlType = relationToCopy.ControlType;
                relation.ElementName = relationToCopy.ElementName;
                relation.ElementType = relationToCopy.ElementType;

                this.StylesheetRelations.Add(relation);
            }
        }

        /// <summary>
        /// Initializes a new instance of the XmlStyleBuilderRegistration class
        /// from xml style sheet, control type, and element type.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="controlType"></param>
        /// <param name="elementType"></param>
        public XmlStyleBuilderRegistration(XmlStyleSheet style, string controlType, string elementType)
        {
			this.builderData = style;

            this.stylesheetRelations = new RadStyleSheetRelationList(1);
        	RadStylesheetRelation relation = new RadStylesheetRelation();
        	this.StylesheetRelations.Add(relation);

			relation.RegistrationType = BuilderRegistrationType.ElementTypeControlType;			
			relation.ElementType = elementType ?? string.Empty;
            relation.ControlType = controlType ?? string.Empty;
        }

		/// <summary>
		/// Gets or sets a string value indicating the builder type.
		/// </summary>
		[XmlAttribute]
		public Type BuilderType
		{
			get { return builderType; }
			set { builderType = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating the xml builder data.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public XmlBuilderData BuilderData
        {
            get { return builderData; }
            set { builderData = value; }
        }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    	public RadStyleSheetRelationList StylesheetRelations
    	{
    		get
			{
				if (this.stylesheetRelations == null)
				{
					this.stylesheetRelations = new RadStyleSheetRelationList(1);
					/*if (this.controlName != null |
						this.elementName != null ||
						this.controlType != null ||
						this.elementType != null)
					{
						this.stylesheetRelations.Add(
							new RadStylesheetRelation(
							this.RegistrationType,
							this.ElementType,
							this.ControlType,
							this.ElementName,
							this.ControlName));
					}*/
				}

				return stylesheetRelations;
			}
    	}

    	/// <summary>
        /// Retrieves the style builder registration.
        /// </summary>
        /// <returns></returns>
        public StyleBuilderRegistration GetRegistration()
        {
			Type actualBuilderType = typeof(DefaultStyleBuilder);

            if (this.BuilderType != null)
            {
                actualBuilderType = this.BuilderType;
            }

			StyleBuilder builder = (StyleBuilder)Activator.CreateInstance(actualBuilderType);
            builder.BuilderData = this.BuilderData;

            StyleBuilderRegistration reg = new StyleBuilderRegistration(
                builder
                );

			foreach( RadStylesheetRelation relation in this.StylesheetRelations )
			{
				string actualElementType = relation.ElementType;

				if (relation.RegistrationType == BuilderRegistrationType.ElementTypeControlType &&
					relation.ElementType == null)
				{
					actualElementType = typeof(RootRadElement).FullName;
				}

				reg.AddStylesheetRelation(relation.RegistrationType,
										  actualElementType,
										  relation.ControlType,
										  relation.ElementName,
										  relation.ControlName);
			}

            return reg;
        }
    }
}
