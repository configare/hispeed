using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Themes;

namespace Telerik.WinControls.Styles
{
    public class XmlRepositoryItem : List<XmlPropertySetting>
    {
        private string key;
        private ElementVisibility? visibilityModifierValue;
        private string itemType;
        private string displayName;
        List<IPropertySetting> deserializedPropertySettings;
        private static object syncRoot = new object();

        public string ItemType
        {
            get
            {
                return this.itemType;
            }
            set
            {
                this.itemType = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating the key of the group uised to identfy the group when referenced 
        /// by other groups when <see cref="Telerik.WinControls.PropertySettingGroup.BasedOn"/> specified.
        /// </summary>
        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public void Reset()
        {
            this.deserializedPropertySettings = null;
            this.Clear();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<IPropertySetting> DeserializedPropertySettings
        {
            get
            {
                if (this.deserializedPropertySettings == null)
                {
                    lock (syncRoot)
                    {
                        if (this.deserializedPropertySettings == null)
                        {
                            this.deserializedPropertySettings = new List<IPropertySetting>();
                            foreach (XmlPropertySetting xmlPs in this)
                            {
                                IPropertySetting setting = xmlPs.Deserialize();
                                AnimatedPropertySetting animation = setting as AnimatedPropertySetting;
                                if (animation != null)
                                {
                                    animation.IsStyleSetting = true;
                                }

                                if (object.ReferenceEquals(setting.Property, RadElement.VisibilityProperty))
                                {
                                    this.InitializeVisibilityModifierValue(setting);
                                }

                                this.deserializedPropertySettings.Add(setting);
                            }

                            this.EnsureLightVisualElementProperties();
                        }
                    }
                }

                return this.deserializedPropertySettings;
            }
        }

        private void InitializeVisibilityModifierValue(IPropertySetting setting)
        {
            PropertySetting propSetting = setting as PropertySetting;
            if (propSetting == null || !(propSetting.Value is ElementVisibility))
            {
                return;
            }
            this.visibilityModifierValue = (ElementVisibility)propSetting.Value;
        }

        private void EnsureLightVisualElementProperties()
        {
            //If there is a property setting in this repository item that modifies the visibility property
            //we need to map the value of the modifier to bool and pass it to the DrawFill/DrawBorder settings.
            bool lightVisualElementVisibilityModifierValue = true;
            if (this.visibilityModifierValue.HasValue)
            {
                lightVisualElementVisibilityModifierValue = this.visibilityModifierValue.Value == ElementVisibility.Visible ? true : false;
            }

            //check the type of the repository in order to add the DrawFill and DrawBorder properties of LightVisualElement
            if (this.itemType == RepositoryItemTypes.Gradient)
            {
                RadProperty drawFillProperty = XmlPropertySetting.DeserializePropertySafe("Telerik.WinControls.UI.LightVisualElement.DrawFill");
                if (drawFillProperty != null)
                {
                    this.deserializedPropertySettings.Add(new PropertySetting(drawFillProperty, lightVisualElementVisibilityModifierValue));
                }
            }
            else if (this.itemType == RepositoryItemTypes.Border)
            {
                RadProperty drawBorderProperty = XmlPropertySetting.DeserializePropertySafe("Telerik.WinControls.UI.LightVisualElement.DrawBorder");
                if (drawBorderProperty != null)
                {
                    this.deserializedPropertySettings.Add(new PropertySetting(drawBorderProperty, lightVisualElementVisibilityModifierValue));
                }
            }
        }
    }
}
