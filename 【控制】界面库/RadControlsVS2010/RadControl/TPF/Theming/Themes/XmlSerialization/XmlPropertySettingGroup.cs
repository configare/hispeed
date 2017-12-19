using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a group of property settings.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XmlPropertySettingGroup
    {
        private XmlPropertySettingCollection propertySettings;
        private XmlSelectorCollection selectors;
        private string basedOn;

        /// <summary>
        /// Gets or sets the collection of properties.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XmlPropertySettingCollection PropertySettings
        {
            get
            {
                if (propertySettings == null)
                {
                    propertySettings = new XmlPropertySettingCollection();
                }

                return propertySettings;
            }
            //set { propertySettings = value; }
        }

        /// <summary>
        /// Gets or sets the collection of selectors.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XmlSelectorCollection Selectors
        {
            get
            {
                if (selectors == null)
                {
                    selectors = new XmlSelectorCollection(1);
                }

                return selectors;
            }
            //set { selectors = value; }
        }

        /// <summary>
        /// Retrieves the string representation of the instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PropertySettingGroup";
        }

        /// <summary>
        /// Retrieve the name of the group.
        /// </summary>
		public string GroupName
		{
			get
			{
				if (this.Selectors == null || this.Selectors.Count == 0)
				{
					return "Initial State";
				}

				if (Selectors[0] is XmlSelectorBase)
				{
					XmlCondition condition = ((XmlSelectorBase)Selectors[0]).Condition;
					if (condition != null)
					{
						return condition.BuildExpressionString();
					}
					else
					{
						return "Initial State";
					}
				}

				return "Unknown State";
			}
		}      

        /// <summary>
        /// Gets or sets value indicating the key of a repository item which this group is based on.
        /// </summary>
        [DefaultValue("")]
        public string BasedOn
        {
            get
            {
                return this.basedOn;
            }
            set
            {
                this.basedOn = value;
            }
        }

        /// <summary>
        /// Determines whether the BasedOn property should be serialized.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBasedOn()
        {
            return !string.IsNullOrEmpty(this.basedOn);
        }

        public PropertySettingGroup Deserialize()
        {            
            PropertySettingGroup group = new PropertySettingGroup();

            group.BasedOn = this.basedOn;

            if (this.propertySettings != null)
            {
                for (int settingIndex = 0; settingIndex < this.PropertySettings.Count; settingIndex++)
                {
                    XmlPropertySetting xmlPropertySetting = this.PropertySettings[settingIndex];
                    IPropertySetting setting = xmlPropertySetting.Deserialize();
                    group.PropertySettings.Add(setting);
                }
            }

            if (this.selectors != null)
            {
                for (int selectorIndex = 0; selectorIndex < this.Selectors.Count; selectorIndex++)
                {
                    XmlElementSelector xmlSelector = this.Selectors[selectorIndex];
                    IElementSelector selector = xmlSelector.Deserialize();
                    group.Selectors.Add(selector);
                }
            }

            return group;
        }
    }
}

