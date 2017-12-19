using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Styles;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class RadListVisualItem : LightVisualElement, IVirtualizedElement<RadListDataItem>
    {
        #region RadProperties

        public static RadProperty ActiveProperty = RadProperty.Register("Active", typeof(bool), typeof(RadListVisualItem), new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));
        public static RadProperty SelectedProperty = RadProperty.Register("Selected", typeof(bool), typeof(RadListVisualItem), new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private RadListDataItem item;
        private static List<RadProperty> synchronizationProperties;

        #endregion

        #region Initialization

        static RadListVisualItem()
        {
            ElementPropertyOptions flags = ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay;
            Type dataItemType = typeof(RadListVisualItem);
            LightVisualElement.TextImageRelationProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(TextImageRelation.ImageBeforeText, flags));
            LightVisualElement.ImageAlignmentProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, flags));
            LightVisualElement.TextAlignmentProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, flags));

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadListVisualItemStateManager(), typeof(RadListVisualItem));
            RadListVisualItem.InitializeSynchronizationProperties();
        }

        private static void InitializeSynchronizationProperties()
        {
            RadListVisualItem.SynchronizationProperties.Add(LightVisualElement.TextImageRelationProperty);
            RadListVisualItem.SynchronizationProperties.Add(LightVisualElement.ImageAlignmentProperty);
            RadListVisualItem.SynchronizationProperties.Add(LightVisualElement.TextAlignmentProperty);
            RadListVisualItem.SynchronizationProperties.Add(LightVisualElement.ImageProperty);
            RadListVisualItem.SynchronizationProperties.Add(LightVisualElement.TextWrapProperty);
            RadListVisualItem.SynchronizationProperties.Add(RadItem.TextOrientationProperty);
            RadListVisualItem.SynchronizationProperties.Add(VisualElement.FontProperty);
            RadListVisualItem.SynchronizationProperties.Add(VisualElement.ForeColorProperty);
            RadListVisualItem.SynchronizationProperties.Add(RadElement.EnabledProperty);
        }

        #endregion

        #region Properties

        protected static List<RadProperty> SynchronizationProperties
        {
            get
            {
                if (RadListVisualItem.synchronizationProperties == null)
                {
                    RadListVisualItem.synchronizationProperties = new List<RadProperty>();
                }

                return RadListVisualItem.synchronizationProperties;
            }
        }

        public bool Selected
        {
            get
            {
                return (bool)this.GetValue(RadListVisualItem.SelectedProperty);
            }
        }

        public bool Active
        {
            get
            {
                return (bool)this.GetValue(RadListVisualItem.ActiveProperty);
            }
        }

        #endregion

        #region Overrides

        //protected override SizeF ArrangeOverride(SizeF finalSize)
        //{
        //    if (  this.Data.Owner != null  && this.Data.Owner.ShowGroups && this.Data.Group != this.Data.Owner.groupFactory.DefaultGroup && !this.Data.Group.Collapsible)
        //    {
        //        RectangleF arrangeRect = this.GetClientRectangle(finalSize);
        //        float offset = this.Data.Owner.NonCollapsibleGroupItemsOffset;
        //        this.layoutManagerPart.Arrange(new RectangleF(arrangeRect.X + offset, arrangeRect.Y, arrangeRect.Width - offset, arrangeRect.Height));

        //        return finalSize;
        //    }
        //    else
        //    {
        //        return base.ArrangeOverride(finalSize);
        //    }
        //}

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.DrawBorder = true;
            this.BypassLayoutPolicies = true;
            this.NotifyParentOnMouseInput = true;
            this.AutoEllipsis = true;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.AllowDrag = true;
        }

        #endregion

        #region IVirtualizedElement<ListItem> Members

        public RadListDataItem Data
        {
            get 
            { 
                return item; 
            }

            protected set
            {
                this.item = value;
            }
        }

        public virtual void Attach(RadListDataItem data, object context)
        {
            this.item = data;

            Debug.Assert(this.item != null, "Data item must never be null at this point.");

            this.Text = this.Data.Text;
            this.BindProperty(RadListVisualItem.ActiveProperty, data, RadListDataItem.ActiveProperty, PropertyBindingOptions.OneWay);
            this.BindProperty(RadListVisualItem.SelectedProperty, data, RadListDataItem.SelectedProperty, PropertyBindingOptions.OneWay);

            this.item.RadPropertyChanged += this.DataPropertyChanged;

            this.Synchronize();
        }
        
        public virtual void Detach()
        {
            this.UnbindProperty(RadListVisualItem.ActiveProperty);
            this.UnbindProperty(RadListVisualItem.SelectedProperty);

            Debug.Assert(this.item != null, "Data item must never be null at this point.");
            this.item.RadPropertyChanged -= this.DataPropertyChanged;
            this.item = null;
        }

        public virtual void Synchronize()
        {
            if (this.IsDesignMode)
            {
                return;
            }

            if (this.item.Owner == null)
            {
                // the item owner will be null when an item is removed from the RadListElements Items collection and also from the SelectedItems.
                // When removed from the selected items its SelectedProperty will change but it will already be removed from Items so it will have no owner.
                return;
            }

            this.SynchronizeProperties();

            Debug.Assert(this.item != null, "Data item must never be null at this point.");
            this.item.Owner.OnVisualItemFormatting(this);
        }

        protected void SynchronizeProperties()
        {
            this.Text = this.Data.Text;

            foreach(RadProperty prop in RadListVisualItem.SynchronizationProperties)
            {
                this.PropertySynchronizing(prop);
                if (Data.GetValueSource(prop) < ValueSource.Local)
                {
                    this.ResetValue(prop, ValueResetFlags.Local);
                }
                else
                {
                    this.SetValue(prop, this.Data.GetValue(prop));
                }
                this.PropertySynchronized(prop);
            }
        }

        protected virtual void PropertySynchronizing(RadProperty property)
        {
        }

        protected virtual void PropertySynchronized(RadProperty property)
        {
        }

        protected virtual void DataPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (this.IsDesignMode)
            {
                return;
            }

            if (this.item.Owner == null)
            {
                // the item owner will be null when an item is removed from the RadListElements Items collection and also from the SelectedItems.
                // When removed from the selected items its SelectedProperty will change but it will already be removed from Items so it will have no owner.
                return;
            }

            this.SynchronizeProperties();

            this.item.Owner.OnVisualItemFormatting(this);
        }

        public virtual bool IsCompatible(RadListDataItem data, object context)
        {
            return !(data is RadListDataGroupItem);
        }

        #endregion
    }
}
