using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemElementBase : LightVisualElement, IVirtualizedElement<PropertyGridItemBase>
    {
        #region Dependency properties

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(PropertyGridItemElementBase),new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded", typeof(bool), typeof(PropertyGridItemElementBase), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsControlInactiveProperty = RadProperty.Register(
            "IsControlInactive", typeof(bool), typeof(PropertyGridItemElementBase), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        #endregion

        #region Constructor

        static PropertyGridItemElementBase()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridItemElementBaseStateManager(), typeof(PropertyGridItemElementBase));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the item is selected.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that the item is selected")]
        public virtual bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(PropertyGridItemElementBase.IsSelectedProperty);
            }
            set
            {
                this.SetValue(PropertyGridItemElementBase.IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is expanded.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that the item is expanded")]
        public virtual bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(PropertyGridItemElementBase.IsExpandedProperty);
            }
            set
            {
                this.SetValue(PropertyGridItemElementBase.IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control contains the focus.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating whether the control contains the focus.")]
        public virtual bool IsControlFocused
        {
            get
            {
                return !(bool)this.GetValue(PropertyGridItemElementBase.IsControlInactiveProperty);
            }
            set
            {
                this.SetValue(PropertyGridItemElementBase.IsControlInactiveProperty, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridTableElement"/> that is parent to this item.
        /// </summary>
        public PropertyGridTableElement PropertyTableElement
        {
            get
            {
                return this.FindAncestor<PropertyGridTableElement>();
            }
        }

        #endregion

        #region Event handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.PropertyTableElement != null)
            {
                this.PropertyTableElement.OnItemMouseDown(new PropertyGridMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.PropertyTableElement != null)
            {
                this.PropertyTableElement.OnItemMouseMove(new PropertyGridMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.PropertyTableElement != null)
            {
                this.PropertyTableElement.OnItemMouseClick(new RadPropertyGridEventArgs(this.Data));
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (this.PropertyTableElement != null)
            {
                this.PropertyTableElement.OnItemMouseDoubleClick(new RadPropertyGridEventArgs(this.Data));
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsMouseOverElementProperty && this.IsInValidState(true))
            {
                OnFormatting();
            }
        }

        protected virtual void OnItemPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                this.SetValue(IsSelectedProperty, this.Data.Selected);
                OnFormatting();
            }
            else if (e.PropertyName == "IsExpanded")
            {
                this.SetValue(IsExpandedProperty, this.Data.Expanded);
                OnFormatting();
            }
            else if (e.PropertyName == "IsControlFocused")
            {
                this.SetValue(IsControlInactiveProperty, this.IsFocused);
                OnFormatting();
            }
        }

        protected virtual void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsInValidState(true))
            {
                OnItemPropertyChanged(e);
            }
        }
        
        protected virtual void OnFormatting()
        {
            this.PropertyTableElement.OnItemFormatting(new PropertyGridItemFormattingEventArgs(this));
        }

        #endregion

        #region IVirtualizedElement<PropertyGridDataItemBase> Memebers

        public virtual PropertyGridItemBase Data
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual void Attach(PropertyGridItemBase data, object context)
        {
            throw new NotImplementedException();
        }

        public virtual void Detach()
        {
            throw new NotImplementedException();
        }

        public virtual void Synchronize()
        {
            this.PropertyTableElement.OnItemFormatting(new PropertyGridItemFormattingEventArgs(this));
        }

        public virtual bool IsCompatible(PropertyGridItemBase data, object context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}