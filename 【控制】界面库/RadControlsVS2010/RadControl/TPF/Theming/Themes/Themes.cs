using System;
using System.Collections.Generic;
using System.Drawing;
using Telerik.WinControls.Styles.PropertySettings;
using Telerik.WinControls.Themes;
using Telerik.WinControls.XmlSerialization;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls
{
    public interface IPropertiesProvider
    {
        object GetPropertyValue(string propertyName);
        void SetPropertyValue(string propertyName, object value);
    }

    /// <summary>
    /// Represents a Theme for some of the Telerik controls.
    /// </summary>
    public class Theme : IComparable, IPropertiesProvider
    {
        private string themeName;
        private ThemePropertyCollection themeProperties;
        private Dictionary<string, HslColor> colorBlendParameterNames = new Dictionary<string, HslColor>();
        Dictionary<PartiallyLoadableStyleSheet, PartiallyLoadedStyleSheetHelper> partiallyLoadedHelpers = new Dictionary<PartiallyLoadableStyleSheet, PartiallyLoadedStyleSheetHelper>();

        /// <summary>
        /// Initializes a new instance of the Theme class using theme name.
        /// </summary>
        /// <param name="themeName"></param>
        public Theme(string themeName)
        {
            this.themeName = themeName;
        }

        /// <summary>
        /// Gets or sets a theme name.
        /// </summary>
        public string ThemeName
        {
            get { return themeName; }
            set { themeName = value; }
        }

        /// <summary>
        /// Collectino of theme parameters. See <see cref="AddColorBlend(string, HslColor)"/> for details.
        /// </summary>
        public ThemePropertyCollection ThemeProperties
        {
            get
            {
                if (themeProperties == null)
                {
                    themeProperties = new ThemePropertyCollection(this);
                }

                return themeProperties;
            }
        }


        /// <summary>
        /// Gets the XmlStyleRepository of the theme, if any 
        /// </summary>
        public XmlStyleRepository Repository
        {
            get
            {
                return ThemeResolutionService.GetThemeRepository(this.themeName);
            } 
        }

        /// <summary>
        /// Gets the registered style builders.
        /// </summary>
        /// <returns></returns>
        public StyleBuilderRegistration[] GetRegisteredStyleBuilders()
        {
            return ThemeResolutionService.GetStyleSheetBuilders(this.ThemeName);
        }

        public bool HasBuildersFor(RadControl control)
        {
            return ThemeResolutionService.HasBuildersFor(control, this.ThemeName);
        }

        /// <summary>
        /// Deserializes a theme from a given %XmlTheme:Telerik.WinControls.XmlTheme%
        /// instance. Resulting theme is registered within ThemeReolutionService and can be obtained using the following 
		/// ThemeResolutionService.GetTheme(xmlTheme.ThemeName)
        /// </summary>
        /// <param name="xmlTheme">An instance of the <see cref="XmlTheme"/>class
        /// which is deserialized.</param>
        public static void Deserialize(XmlTheme xmlTheme)
        {
            Deserialize(xmlTheme, true);
        }

        /// <summary>
        /// Deserializes a theme from a given %XmlTheme:Telerik.WinControls.XmlTheme%
        /// instance. Resulting theme is registered within ThemeReolutionService and can be obtained using the following 
        /// ThemeResolutionService.GetTheme(xmlTheme.ThemeName)
        /// </summary>
        /// <param name="xmlTheme">An instance of the <see cref="XmlTheme"/>class
        /// which is deserialized.</param>
        /// <param name="registerBuildersWithThemeResService">Defines whether new <see cref="StyleBuilderRegistration"/>
        /// instances are added in the <see cref="ThemeResolutionService"/>.</param>
        public static void Deserialize(XmlTheme xmlTheme, bool registerBuildersWithThemeResService)
        {
            //ThemeResolutionService.BeginEditTheme(xmlTheme.ThemeName);

            if (string.IsNullOrEmpty(xmlTheme.ThemeName))
                return;

            ThemeResolutionService.SuspendThemeChange();

            try
            {
                //Copy theme parameters
                ThemeResolutionService.EnsureThemeRegistered(xmlTheme.ThemeName);
                Theme themeInstance = ThemeResolutionService.GetTheme(xmlTheme.ThemeName);
                foreach (KeyValuePair<string, object> dictionaryEntry in xmlTheme.ThemeProperties)
                {
                    themeInstance.ThemeProperties[dictionaryEntry.Key] = dictionaryEntry.Value;
                }

                string[] themeNames = xmlTheme.ThemeName.Split(',', ';');

                if (xmlTheme.HasRepository)
                {
                    foreach (string themeName in themeNames)
                    {
                        //If a repository for this theme already exists, both are merged.
                        ThemeResolutionService.RegisterThemeRepository(xmlTheme.StyleRepository, themeName.Trim());
                    }
                }

                //Register StyleBuilders
                if (registerBuildersWithThemeResService && xmlTheme.BuilderRegistrations != null)
                {

                    foreach (XmlStyleBuilderRegistration registration in xmlTheme.BuilderRegistrations)
                    {
                        foreach (string themeName in themeNames)
                        {
                            ThemeResolutionService.RegisterStyleBuilder(registration.GetRegistration(), themeName.Trim());
                        }
                    }
                }
            }
            finally
            {
                ThemeResolutionService.ResumeThemeChange();
            }
        }

        /// <summary>
        /// Find a specific style builder
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public StyleBuilder FindStyleSheetRegistration(string controlType, string elementType)
        {
            return ThemeResolutionService.GetRegisteredControlStyleBuilder(controlType, elementType, themeName);
        }

        
        #region IComparable Members

		public int CompareTo(object obj)
		{
			return this.ThemeName.CompareTo(((Theme)obj).ThemeName);
		}

		#endregion

        #region IPropertiesProvider Members

        /// <summary>
        /// IPropertiesProvider implementation
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(string propertyName)
        {
            object res;
            if (this.ThemeProperties.TryGetValue(propertyName, out res))
            {
                return res;
            }

            return null;
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            this.ThemeProperties[propertyName] = value;
        }

        #endregion        

        /// <summary>
        /// Applys color blend to all colors in the current theme, based on the given baseColor. ThemeNameParameter will be 
        /// automatically added to the <see cref="ThemeProperties"/> collection.
        /// </summary>
        /// <remarks>
        /// When AddColorBlend is used, themeparamter will be added to ThemeProperties collection. Then the theme will be processed and 
        /// color values will be substituted by the value of ThemeParameter specified. 
        /// Changing the value of the theme parameter will allow you to dynamically modify the HSL components 
        /// of the corresponding color values.
        /// </remarks>
        /// <param name="themeParameterName">theme parameter name to be populated</param>
        /// <param name="baseColor">THe Hue setting of the base color will be used to determine wich colours will be affected by the colorblend. L and S values of the color will be used to calculate new color values</param>
        /// <param name="notifyThemeChanged">Specify whether to notify notify contorls that theme has changed</param>
        public void AddColorBlend(string themeParameterName, HslColor baseColor, bool notifyThemeChanged)
        {
            //TODO: change behavior to not remove all but add color blends
            //RemoveAllColorBlends();
            if (this.colorBlendParameterNames.ContainsKey(themeParameterName))
            {
                this.RemoveColorBlend(themeParameterName);
            }

            this.ThemeProperties[themeParameterName] = baseColor;

            foreach (StyleBuilderRegistration registration in this.GetRegisteredStyleBuilders())
            {
                ApplyColorBlendToStyle(themeParameterName, baseColor, registration);
            }

            colorBlendParameterNames.Add(themeParameterName, baseColor);

            if (notifyThemeChanged)
            {
                this.NotifyThemeChanged();
            }
        }

        /// <summary>
        /// Notifies all controls that currently use this theme, to let them refresh their appearance.
        /// </summary>
        public void NotifyThemeChanged()
        {
            ThemeResolutionService.RaiseThemeChanged(this.ThemeName);
        }

        /// <summary>
        /// Applys color blend to all colors in the current theme, based on the given baseColor and notifies all controls that theme has changed. ThemeNameParameter will be 
        /// automatically added to the <see cref="ThemeProperties"/> collection.
        /// </summary>
        /// <remarks>
        /// When AddColorBlend is used, themeparamter will be added to ThemeProperties collection. Then the theme will be processed and 
        /// color values will be substituted by the value of ThemeParameter specified. 
        /// Changing the value of the theme parameter will allow you to dynamically modify the HSL components 
        /// of the corresponding color values.
        /// </remarks>
        /// <param name="themeParameterName">theme parameter name to be populated</param>
        /// <param name="baseColor">THe Hue setting of the base color will be used to determine wich colours will be affected by the colorblend. L and S values of the color will be used to calculate new color values</param>
        public void AddColorBlend(string themeParameterName, HslColor baseColor)
        {
            this.AddColorBlend(themeParameterName, baseColor, true);
        }

        internal void StyleBuilderRegistered(StyleBuilderRegistration styleBuilder)
        {
            if (this.colorBlendParameterNames.Count > 0)
            {
                foreach (KeyValuePair<string, HslColor> value in this.colorBlendParameterNames)
                {
                    this.ApplyColorBlendToStyle(value.Key, value.Value, styleBuilder);
                }
            }
        }

        public void RemoveAllColorBlends()
        {
            RemoveAllColorBlends(true);
        }

        /// <summary>
        /// Removes all color blends created by using <see cref="AddColorBlend(string, Telerik.WinControls.HslColor)"/>
        /// </summary>
        //[EditorBrowsable(EditorBrowsableState.Advanced)]
        public void RemoveAllColorBlends(bool notifyThemeChanged)
        {
            //no color blends, do nothing
            if (this.colorBlendParameterNames.Count == 0)
            {
                return;
            }
            foreach (KeyValuePair<string, HslColor> entry in this.colorBlendParameterNames)
            {
                this.RemoveColorBlendInternal(entry.Key);
                this.ThemeProperties.Remove(entry.Key);
            }

            this.colorBlendParameterNames.Clear();

            if (notifyThemeChanged)
            {
                this.NotifyThemeChanged();
            }
        }

        /// <summary>
        /// Removes specific color blend, previously added using <see cref="AddColorBlend(string, HslColor)"/> with the same themeParameterName argument
        /// </summary>
        public void RemoveColorBlend(string themeParameterName)
        {
            this.RemoveColorBlendInternal(themeParameterName);
            this.colorBlendParameterNames.Remove(themeParameterName);
        }

        private void RemoveColorBlendInternal(string themeParameterName)
        {
            foreach (StyleBuilderRegistration registration in this.GetRegisteredStyleBuilders())
            {
                PartiallyLoadableStyleSheet partiallyLoaded =
                        ((DefaultStyleBuilder)registration.Builder).Style as PartiallyLoadableStyleSheet;

                //Partially loaded stylesheets support
                if (partiallyLoaded != null && !partiallyLoaded.IsLoadedCompletely)
                {
                    PartiallyLoadedStyleSheetHelper helper = this.partiallyLoadedHelpers[partiallyLoaded];
                    if (helper != null)
                    {
                        helper.UnsubscribeFromLoadedEvent(partiallyLoaded);
                    }
                    continue;
                }

                foreach (
                    PropertySettingGroup group in
                        ((DefaultStyleBuilder)registration.Builder).Style.PropertySettingGroups)
                {
                    foreach (IPropertySetting setting in group.PropertySettings)
                    {
                        RadProperty prop = setting.Property;
                        if (prop.PropertyType == typeof(Color))
                        {
                            if (setting is AnimatedPropertySetting)
                            {
                                ColorBlendExtension startValueExtension =
                                    ((AnimatedPropertySetting)setting).GetStartValueProvider() as ColorBlendExtension;
                                if (startValueExtension != null && startValueExtension.ThemePropertyName == themeParameterName)
                                {
                                    ((AnimatedPropertySetting)setting).StartValue = startValueExtension.OriginalColor;
                                }

                                ColorBlendExtension endValueExtension =
                                    ((AnimatedPropertySetting)setting).GetEndValueProvider() as ColorBlendExtension;
                                if (endValueExtension != null && endValueExtension.ThemePropertyName == themeParameterName)
                                {
                                    ((AnimatedPropertySetting)setting).EndValue = endValueExtension.OriginalColor;
                                }
                            }
                            else //actualSetting is PropertySetting
                            {
                                ColorBlendExtension valueExtension =
                                    ((PropertySetting)setting).GetValueProvider() as ColorBlendExtension;
                                if (valueExtension != null && valueExtension.ThemePropertyName == themeParameterName)
                                {
                                    ((PropertySetting)setting).Value = valueExtension.OriginalColor;
                                }
                            }
                        }
                    }
                }
            }
        }        

        private void ApplyColorBlendToStyle(string themeParameterName, HslColor baseColor, StyleBuilderRegistration registration)
        {
            PartiallyLoadableStyleSheet partiallyLoaded = ((DefaultStyleBuilder) registration.Builder).Style as PartiallyLoadableStyleSheet;
            //delay applying of color blend, to the moment stylesheet is requested by a control            
            if (partiallyLoaded != null && !partiallyLoaded.IsLoadedCompletely)
            {
                PartiallyLoadedStyleSheetHelper helper =
                    new PartiallyLoadedStyleSheetHelper(partiallyLoaded, this, baseColor, themeParameterName);

                this.partiallyLoadedHelpers[partiallyLoaded] = helper;

                return;
            }

            StyleSheet style = ((DefaultStyleBuilder) registration.Builder).Style;

            ApplyColorBlendToStyleSheet(baseColor, themeParameterName, this, style);
        }

        public static void ApplyColorBlendToStyleSheet(HslColor baseColor, string themeParameterName, IPropertiesProvider propertiesProvider, StyleSheet style)
        {
            PropertySettingGroupCollection groups = style.PropertySettingGroups;
            ApplyColorBlendToGroups(groups, baseColor, themeParameterName, propertiesProvider);
        }

        //public static void ApplyColorBlendToStyle(HslColor baseColor, string themeParameterName, StyleSheet style)
        //{
        //    PropertySettingGroupCollection groups = style.PropertySettingGroups;
        //    ApplyColorBlendToGroups(groups, baseColor, themeParameterName);
        //}

        public void ApplyColorBlendToGroups(PropertySettingGroupCollection groups, HslColor baseColor, string themeParameterName)
        {
            ApplyColorBlendToGroups(groups, baseColor, themeParameterName, this);
        }

        public static void ApplyColorBlendToGroups(PropertySettingGroupCollection groups, HslColor baseColor, string themeParameterName, IPropertiesProvider propertiesProvider)
        {
            foreach ( PropertySettingGroup group in groups)
            {
                foreach (IPropertySetting setting in group.PropertySettings)
                {
                    RadProperty prop = setting.Property;
                    if (prop.PropertyType == typeof(Color))
                    {
                        if (setting is AnimatedPropertySetting)
                        {
                            AnimatedPropertySetting animatedSetting = (AnimatedPropertySetting) setting;

                            object value = animatedSetting.StartValue;
                            if (value != null)
                            {
                                Color color = (Color) value;
                                if (ShouldApplyBaseColor(baseColor, color))
                                {
                                    object relativeColorDef =
                                        GetRelativeColorDef(propertiesProvider, themeParameterName, baseColor, color,
                                                            animatedSetting.GetStartValueProvider());
                                    animatedSetting.StartValue = relativeColorDef;
                                }
                            }

                            object endValue = animatedSetting.EndValue;
                            if (endValue != null)
                            {
                                Color color = (Color) endValue;
                                if (ShouldApplyBaseColor(baseColor, color))
                                {
                                    animatedSetting.EndValue =
                                        GetRelativeColorDef(propertiesProvider, themeParameterName, baseColor, color,
                                                            animatedSetting.GetEndValueProvider());
                                }
                            }
                        }
                        else //actualSetting is PropertySetting
                        {
                            PropertySetting propertySetting = (PropertySetting) setting;
                            object value = propertySetting.Value;
                            if (value != null)
                            {
                                Color color = (Color) value;
                                if (ShouldApplyBaseColor(baseColor, color))
                                {
                                    propertySetting.Value =
                                        GetRelativeColorDef(propertiesProvider, themeParameterName, baseColor, color,
                                                            propertySetting.GetValueProvider());
                                }
                            }
                        }
                    }
                }
            }
        }

        private class PartiallyLoadedStyleSheetHelper
        {
            private readonly Theme theme;
            private readonly HslColor baseColor;
            private readonly string themeParameterName;

            public PartiallyLoadedStyleSheetHelper(PartiallyLoadableStyleSheet plSS, Theme theme, HslColor baseColor, string themeParameterName)
            {
                this.theme = theme;
                this.baseColor = baseColor;
                this.themeParameterName = themeParameterName;

                plSS.LoadedCompletely += plSS_LoadedCompletely;
            }

            void plSS_LoadedCompletely(object sender, EventArgs e)
            {            
                PartiallyLoadableStyleSheet plSS = (PartiallyLoadableStyleSheet) sender;
                plSS.LoadedCompletely -= plSS_LoadedCompletely;

                this.theme.partiallyLoadedHelpers.Remove(plSS);

                theme.ApplyColorBlendToGroups(plSS.PropertySettingGroups, baseColor, themeParameterName);
            }

            public void UnsubscribeFromLoadedEvent(PartiallyLoadableStyleSheet partiallyLoaded)
            {
                partiallyLoaded.LoadedCompletely -= plSS_LoadedCompletely;
            }
        }

        private static bool ShouldApplyBaseColor(HslColor baseColor, Color actualColor)
        {
            HslColor hslColor = HslColor.FromColor(actualColor);


            return Math.Abs(baseColor.H - hslColor.H) < (1d / 14d);
        }

        private static object GetRelativeColorDef(IPropertiesProvider propertiesProvider, string themeParameterName, HslColor baseHslColor, Color currentColor, IValueProvider originalValueProvider)
        {
            ColorBlendExtension extension = originalValueProvider as ColorBlendExtension;
            if (extension == null)
            {
                return new ColorBlendExtension(propertiesProvider, themeParameterName, baseHslColor, currentColor, currentColor);
            }
            else
            {                
                return new ColorBlendExtension(propertiesProvider, themeParameterName, baseHslColor, currentColor, extension.OriginalColor);
            }
        }
    }    

    /// <summary>
    /// Represents a list of %Themes:Theme%.
    /// </summary>
    public class ThemeList : List<Theme>
    {
	}
  
}
