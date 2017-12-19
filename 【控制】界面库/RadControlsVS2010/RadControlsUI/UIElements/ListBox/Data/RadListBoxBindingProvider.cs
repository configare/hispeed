using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI.Data;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI.UIElements.ListBox.Data
{
    public class RadListBoxBindingProvider : BindingProviderBase<RadListBoxItem>
    {
        PropertyMappingInfo mappings = new PropertyMappingInfo();
        public RadListBoxBindingProvider(IBindableComponent owner) : base(owner)
        {
            this.PropertyMappings = mappings;
        }

        protected override RadListBoxItem CreateInstance()
        {
            RadListBoxItem newItem = new RadListBoxItem();

            return newItem;
        }

        public void UpdatePropertyMappings(ConvertCallback DataToString, string displayMember, string valueMember)
        {
            if (this.mappings == null)
            {
                return;
            }

            PropertyMapping displayValueMapping = this.mappings.FindByLogicalItemProperty("Text");
            if (displayValueMapping == null)
            {
                displayValueMapping = new PropertyMapping("Text", displayMember);
                displayValueMapping.ConvertToLogicalValue = DataToString;
                this.mappings.Mappings.Add(displayValueMapping);
            }
            else
            {
                displayValueMapping.DataSourceItemProperty = displayMember;
            }

            PropertyMapping valueMapping = this.mappings.FindByLogicalItemProperty("Value");
            if (valueMapping == null)
            {
                valueMapping = new PropertyMapping("Value", valueMember);
                this.mappings.Mappings.Add(valueMapping);

            }
            else
            {
                valueMapping.DataSourceItemProperty = valueMember;
            }
        }

        public void UpdateItems()
        {
            if (this.logicalItems == null)
            {
                return;
            }

            if(this.logicalItemProperties == null)
            {
                return;
            }

            if(this.dataItemProperties == null)
            {
                return;
            }

            for(int i = 0; i < this.logicalItems.Count; ++i)
            {
                RadListBoxItem currentItem = this.logicalItems[i];

                for (int j = 0; j < this.mappings.Mappings.Count; ++j)
                {
                    PropertyMapping currentMapping = this.mappings.Mappings[j];

                    PropertyDescriptor currentLogicalProperty = this.logicalItemProperties.Find(currentMapping.LogicalItemProperty, false);
                    PropertyDescriptor currentDataProperty = this.dataItemProperties.Find(currentMapping.DataSourceItemProperty, false);

                    if (currentLogicalProperty != null && currentDataProperty != null)
                    {
                        if (currentMapping.LogicalItemProperty == "Text")
                        {
                            this.logicalItems[i].Text = (string)currentMapping.ConvertToLogicalValue(currentDataProperty.GetValue(currentItem.DataItem), currentDataProperty.Converter);
                        }

                        if(currentMapping.LogicalItemProperty == "Value")
                        {
                            this.logicalItems[i].Value = currentDataProperty.GetValue(this.logicalItems[i].DataItem);
                        }
                    }

                    if (currentDataProperty == null)
                    {
                        if (currentMapping.LogicalItemProperty == "Text" && currentMapping.DataSourceItemProperty == "" && currentMapping.ConvertToLogicalValue != null)
                        {
                            this.logicalItems[i].Text = (string)currentMapping.ConvertToLogicalValue(this.logicalItems[i].DataItem, TypeDescriptor.GetConverter(this.logicalItems[i].DataItem.GetType()));
                        }

                        if (currentMapping.LogicalItemProperty == "Value" && currentMapping.DataSourceItemProperty == "")
                        {
                            this.logicalItems[i].Value = this.logicalItems[i].DataItem;
                        }
                    }
                }
            }
        }

        protected override void OnSourcePropertyNotFound(object dataItem, PropertyMapping mapping, PropertyDescriptor targetProp, object logicalItem)
        {
            if (dataItem != null)
            {
                if (mapping.LogicalItemProperty == "Text")
                {
                    targetProp.SetValue(logicalItem, mapping.ConvertToLogicalValue(dataItem, TypeDescriptor.GetConverter(dataItem.GetType())));
                }

                if (mapping.LogicalItemProperty == "Value")
                {
                    targetProp.SetValue(logicalItem, dataItem);
                }
            }
        }
    }
}
