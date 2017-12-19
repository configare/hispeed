using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.WinControls
{

    /// <summary>
    /// Defines a set of IPropertySetting objects that form a group that 
    /// can be applied to a RadElement. Used by StyleSheets system.
    /// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertySettingGroup
    {
        private SelectorCollection selectors = new SelectorCollection();
        private PropertySettingCollection propertySettings = new PropertySettingCollection();

        private bool isStyleSettingApplied;
        private string basedOn;
        public const char BasedOnDelimiter = ',';

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroup class.
        /// </summary>
        public PropertySettingGroup()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroup class.
        /// </summary>
        public PropertySettingGroup(string className, params IPropertySetting[] propertySettings)
        {
            this.Selectors.Add(new ClassSelector(className));
            this.PropertySettings.AddRange(propertySettings);
        }

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroup class.
        /// </summary>
        public PropertySettingGroup(string className, Condition condition, params IPropertySetting[] propertySettings)
        {
            ClassSelector selector = new ClassSelector(className);
            selector.Condition = condition;
            this.Selectors.Add(selector);
            this.PropertySettings.AddRange(propertySettings);
        }

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroup class.
        /// </summary>
        public PropertySettingGroup(Type type, params IPropertySetting[] propertySettings)
        {
            this.Selectors.Add(new TypeSelector(type));
            this.PropertySettings.AddRange(propertySettings);
        }

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroup class.
        /// </summary>
        public PropertySettingGroup(Type type, Condition condition, params IPropertySetting[] propertySettings)
        {
            TypeSelector selector = new TypeSelector(type);
            selector.Condition = condition;
            this.Selectors.Add(selector);
            this.PropertySettings.AddRange(propertySettings);
        }

        public void AssociateWithRepositoryItem(string key)
        {
            if (string.IsNullOrEmpty(key) || this.IsBasedOnRepositoryItem(key))
            {
                return;
            }

            if (string.IsNullOrEmpty(this.basedOn))
            {
                this.basedOn = key;
            }
            else
            {
                this.basedOn += (BasedOnDelimiter + key);
            }
        }

        public void ResetRepositoryItemAssociation(string itemKey)
        {
            this.ResetRepositoryItemAssociation(itemKey, true);
        }

        //TODO: Write tests to verify string parsing
        public void ResetRepositoryItemAssociation(string itemKey, bool clearInheritedSettings)
        {
            if (string.IsNullOrEmpty(this.basedOn))
            {
                return;
            }

            //passing null or empty string as item key will reset entire BaseOn member
            if (string.IsNullOrEmpty(itemKey))
            {
                this.basedOn = string.Empty;
                if (clearInheritedSettings)
                {
                    this.propertySettings.ResetInheritedProperties();
                }

                return;
            }

            string[] keys = this.GetBasedOnRepositoryItems();
            int keyIndex = -1;
            int indexInBaseOn = 0;

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == itemKey)
                {
                    keyIndex = i;
                    break;
                }

                indexInBaseOn += (keys[i].Length + 1);
            }

            //item key is not found
            if (keyIndex == -1)
            {
                return;
            }

            int removeLength = itemKey.Length;
            //remove item as well as delimeter if we have more than one key
            if (keys.Length > 1)
            {
                if (keyIndex == keys.Length - 1)
                {
                    indexInBaseOn--;
                    removeLength++;
                }
                else
                {
                    removeLength++;
                }
            }

            this.basedOn = this.basedOn.Remove(indexInBaseOn, removeLength);
            if (clearInheritedSettings && string.IsNullOrEmpty(this.basedOn))
            {
                this.propertySettings.ResetInheritedProperties();
            }
        }

        public string[] GetBasedOnRepositoryItems()
        {
            if (string.IsNullOrEmpty(this.basedOn))
            {
                return new string[] { };
            }

            string[] keys = this.basedOn.Trim().Split(PropertySettingGroup.BasedOnDelimiter);

            int length = keys.Length;
            for (int i = 0; i < length; i++)
            {
                keys[i] = keys[i].Trim();
            }

            return keys;
        }

        public bool IsBasedOnRepositoryItem(string itemKey)
        {
            foreach (string key in this.GetBasedOnRepositoryItems())
            {
                if (key == itemKey)
                {
                    return true;
                }
            }

            return false;
        }

        internal void SetIsFromStyleSheet()
        {
            if(this.isStyleSettingApplied)
            {
                return;
            }

            foreach (IPropertySetting setting in this.propertySettings)
            {
                AnimatedPropertySetting animation = setting as AnimatedPropertySetting;
                if (animation != null)
                {
                    animation.IsStyleSetting = true;
                }
            }

            this.isStyleSettingApplied = true;
        }

        /// <summary>
        /// Gets a collection of the selectors for this property setting group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SelectorCollection Selectors
        {
            get
            {
                return this.selectors;
            }
        }

        /// <summary>
        /// Gets a collection of the property settings for the property setting group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PropertySettingCollection PropertySettings
        {
            get
            {
                return this.propertySettings;
            }
        }

        /// <summary>
        /// Gets or sets value indicating the key of a repository item which this group is based on.
        /// </summary>
        public string BasedOn
        {
            get
            {
                return this.basedOn;
            }
            set
            {
                if (this.basedOn == value)
                {
                    return;
                }

                this.basedOn = this.NormalizeBasedOn(value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBasedOn()
        {
            return !string.IsNullOrEmpty(this.basedOn);
        }

        /// <summary>
        /// Trims all leading and trailing whitespaces per item key basis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string NormalizeBasedOn(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            string trimmed = value.Trim();
            string[] keys = trimmed.Split(BasedOnDelimiter);

            int keyLength = keys.Length;
            StringBuilder builder = new StringBuilder(value.Length);

            for (int i = 0; i < keyLength; i++)
            {
                if (i > 0)
                {
                    builder.Append(BasedOnDelimiter);
                }
                builder.Append(keys[i].Trim());
            }

            return builder.ToString();
        }

        public XmlPropertySettingGroup Serialize()
        {
            XmlPropertySettingGroup xmlGroup = new XmlPropertySettingGroup();

            xmlGroup.BasedOn = this.basedOn;

            for (int settingIndex = 0; settingIndex < this.PropertySettings.OriginalPropertySettings.Count; settingIndex++)
            {
                IPropertySetting setting = this.PropertySettings.OriginalPropertySettings[settingIndex];
                //serialize value;
                XmlPropertySetting xmlSetting = setting.Serialize();
                xmlGroup.PropertySettings.Add(xmlSetting);
            }

            if (this.Selectors.Count > 0)
            {
                for (int selectorIndex = 0; selectorIndex < this.Selectors.Count; selectorIndex++)
                {
                    IElementSelector selector = this.Selectors[selectorIndex];
                    xmlGroup.Selectors.Add(selector.Serialize());
                }
            }

            return xmlGroup;
        }

        private void UpdateInheritedProperties()
        {
            if (string.IsNullOrEmpty(this.basedOn))
            {
                this.propertySettings.ResetInheritedProperties();
            }
        }

        public void InheritRepositoryItem(Telerik.WinControls.Styles.XmlRepositoryItem newItem)
        {
            for (int i = 0; i < newItem.DeserializedPropertySettings.Count; i++)
            {
                IPropertySetting newSetting = newItem.DeserializedPropertySettings[i];

                bool found = false;
                for (int k = 0; k < this.propertySettings.OriginalPropertySettings.Count; k++)
                {
                    PropertySetting existingSetting = this.propertySettings.OriginalPropertySettings[k] as PropertySetting;

                    if (existingSetting != null &&
                        existingSetting.Property == newSetting.Property)
                    {
                        //Settingd defined in inherited groups beat those from repository, so no value copy
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    this.propertySettings.AddInheritedPropertySetting(newSetting, newItem.ItemType);
                }
            }
        }
    }
}
