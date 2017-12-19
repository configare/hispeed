using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls
{
    //[Serializable]
    //[XmlInclude(typeof(XmlPropertySettingGroup))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class XmlStyleSheet : XmlBuilderData
    {
        private XmlPropertySettingGroupCollection propertySettingGroups;
        private string partiallyLoadedXmlData = null;
        private string themeLocation;
        private string themeName;

        public XmlStyleSheet()
        {
        }

        public XmlStyleSheet(StyleSheet style)
        {
            //Create xmlstyel from stylesheet
            this.propertySettingGroups = new XmlPropertySettingGroupCollection(style.PropertySettingGroups.Count);

            for (int i = 0; i < style.PropertySettingGroups.Count; i++ )
            {
                this.propertySettingGroups.Add(style.PropertySettingGroups[i].Serialize());
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XmlPropertySettingGroupCollection PropertySettingGroups
        {
            get
            {
                if (propertySettingGroups == null)
                {
                    propertySettingGroups = new XmlPropertySettingGroupCollection();
                }

                return propertySettingGroups;
            }
            //set { propertySettingGroups = value; }
        }

        /// <summary>
        /// Gets value indicating the location of the theme file that the XmlStyleSheet has been loaded from.
        /// </summary>
        public string ThemeLocation
        {
            get { return themeLocation; }
        }

        public StyleSheet GetStyleSheet()
        {
            if (this.partiallyLoadedXmlData != null)
            {
                ThemeResolutionService.EnsureThemeRegistered(themeName);
                Theme theme = ThemeResolutionService.GetTheme(themeName);
                return new PartiallyLoadableStyleSheet(theme, this.partiallyLoadedXmlData, this.ThemeLocation);
            }
            else
            {
                return new PartiallyLoadableStyleSheet(this, this.ThemeLocation);
            }

            /*
            StyleSheet res = new StyleSheet();
			if (this.PropertySettingGroups == null)
			{
				return res;
			}

            PropertySettingGroupCollection resGroups = res.PropertySettingGroups;
            XmlPropertySettingGroupCollection xmlGroups = this.PropertySettingGroups;

            DeserializePropertySettingGroups(res.PropertySettingGroups);

            return res;*/
        }

        public void DeserializePropertySettingGroups(PropertySettingGroupCollection resGroups)
        {
            for (int i = 0; i < this.PropertySettingGroups.Count; i++)
            {
                XmlPropertySettingGroup xmlGroup = this.PropertySettingGroups[i];
                
                if (xmlGroup.PropertySettings == null  ||
                    xmlGroup.Selectors == null )
                {
                    continue;
                }

                try
                {
                    PropertySettingGroup group = xmlGroup.Deserialize();
                    resGroups.Add(group);
                }
                catch (Exception ex)
                {
                    string message = "Exception occured while deserializing PropertySettingGroup: " + ex.ToString();
                    Trace.WriteLine(message);
                    Debug.Fail(message);
                }
            }
        }


        /// <summary>
        /// This method supports TPF infrastructure and is intended for internal use only.
        /// </summary>
        /// <param name="applyUnconditional"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
		public StyleSheet GetStyleSheet(XmlSelectorBase applyUnconditional, XmlPropertySettingGroup[] groups)
		{
			StyleSheet res = new StyleSheet();
			SelectorBase applySelector = null;

			if (this.PropertySettingGroups == null)
			{
				return res;
			}

            for (int i = 0; i < this.PropertySettingGroups.Count; i++)
            {
                XmlPropertySettingGroup xmlGroup = this.PropertySettingGroups[i];
                if (xmlGroup.Selectors != null)
                {
                    for (int selectorIndex = 0; selectorIndex < xmlGroup.Selectors.Count; selectorIndex++)
                    {
                        XmlElementSelector xmlSelector = xmlGroup.Selectors[selectorIndex];
                        if (xmlSelector == applyUnconditional)
                        {
                            applySelector = (SelectorBase) xmlSelector.Deserialize();
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < this.PropertySettingGroups.Count; i++)
            {
                XmlPropertySettingGroup xmlGroup = this.PropertySettingGroups[i];
                PropertySettingGroup group = new PropertySettingGroup();

                if (xmlGroup.PropertySettings == null ||
                    xmlGroup.Selectors == null)
                {
                    continue;
                }

                bool disable = false;
                if (applyUnconditional != null)
                {
                    for (int groupIndex = 0; groupIndex < groups.Length; groupIndex++)
                    {
                        XmlPropertySettingGroup g = groups[groupIndex];
                        if (g == xmlGroup)
                        {
                            disable = true;
                            break;
                        }
                    }
                }

                for (int settingIndex = 0; settingIndex < xmlGroup.PropertySettings.Count; settingIndex++)
                {
                    XmlPropertySetting xmlPropertySetting = xmlGroup.PropertySettings[settingIndex];
                    IPropertySetting setting = xmlPropertySetting.Deserialize();
                    group.PropertySettings.Add(setting);
                }

                //IElementSelector activeSelector = null;

                for (int selectorIndex = 0; selectorIndex < xmlGroup.Selectors.Count; selectorIndex++)
                {
                    XmlElementSelector xmlSelector = xmlGroup.Selectors[selectorIndex];
                    IElementSelector selector = null;
                    selector = xmlSelector.Deserialize();

                    if (xmlSelector == applyUnconditional)
                    {
                        applySelector.IsActiveSelectorInStyleBuilder = true;
                        selector = applySelector;
                        //continue;
                    }
                    else if (disable)
                    {
                        ((SelectorBase) selector).DisableStyle = true;
                        //((SelectorBase)selector).ExcludeSelector = applySelector;
                    }

                    group.Selectors.Add(selector);
                }

                //if (activeSelector != null)
                //	group.Selectors.Insert(0, activeSelector);

                res.PropertySettingGroups.Add(group);
            }

            return res;
		}

        internal void SetPartiallyLoadedXmlData(string partiallyLoadedXmlData)
        {
            this.partiallyLoadedXmlData = partiallyLoadedXmlData;
        }

        internal void SetThemeLocation(string themeLocation)
        {
            this.themeLocation = themeLocation;
        }

        internal void SetThemeName(string themeName)
        {
            this.themeName = themeName;
        }
    }
}
