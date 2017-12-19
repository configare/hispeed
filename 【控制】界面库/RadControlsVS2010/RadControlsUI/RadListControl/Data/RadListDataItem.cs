using System;
using Telerik.WinControls.Data;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public class RadListDataItem : RadObject, IDataItem
    {
        #region Dependency properties
        public static readonly RadProperty HeightProperty = RadProperty.Register("Height", typeof(int), typeof(RadListDataItem), new RadElementPropertyMetadata(-1));
        public static readonly RadProperty ActiveProperty = RadProperty.Register("Active", typeof(bool), typeof(RadListDataItem), new RadElementPropertyMetadata(false));
        public static readonly RadProperty SelectedProperty = RadProperty.Register("Selected", typeof(bool), typeof(RadListDataItem), new RadElementPropertyMetadata(false));
        public static readonly RadProperty ValueProperty = RadProperty.Register("Value", typeof(object), typeof(RadListDataItem), new RadElementPropertyMetadata(null, ElementPropertyOptions.None));
      
        #endregion

        #region Fields

        protected object dataBoundItem;
        protected ListDataLayer dataLayer;
        protected RadListElement ownerElement;
        internal ListGroup group;
        // The cached text is an optimization during data binding as well as during sorting and filtering.
        // Binding or sortng thousands of items and using the normal Text property causes property value composition and formatting which is
        // very slow and unnecessary.
        private string cachedText = "";
        private bool changingOwner = false;
        private SizeF measuredSize;
        protected object tag;

        #endregion

        #region Initialization

        static RadListDataItem()
        {
            ElementPropertyOptions flags = ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay;
            Type dataItemType = typeof(RadListDataItem);
            LightVisualElement.TextImageRelationProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(TextImageRelation.ImageBeforeText, flags));
            LightVisualElement.ImageAlignmentProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, flags));
            LightVisualElement.TextAlignmentProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, flags));
            LightVisualElement.TextWrapProperty.OverrideMetadata(dataItemType, new RadElementPropertyMetadata(true, flags));            
        }

        public RadListDataItem(string text) : this()
        {
            this.Text = text;
        }

        public RadListDataItem(string text, object value) : this(text)
        {
            this.Value = value;
        }

        public RadListDataItem()
        {
        }

        #endregion

        #region Properties

        [Category("Data"),
        Localizable(false),
        Bindable(true),
        DefaultValue((string)null),
        TypeConverter(typeof(StringConverter)),
        Description("Tag object that can be used to store user data, corresponding to the element")]
        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }

        internal virtual ListGroup Group
        {
            get
            {
                return this.group ?? this.ownerElement.groupFactory.DefaultGroup;
            }
            set
            {
                if (this.group != value)
                {
                    this.group = value;
                    this.ownerElement.UpdateItemTraverser();
                    this.ownerElement.Scroller.UpdateScrollRange();
                    this.ownerElement.InvalidateMeasure(true);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this data item is data bound.
        /// </summary>
        [Browsable(false)]
        public bool IsDataBound
        {
            get
            {
                return this.dataLayer != null && this.dataLayer.DataSource != null;
            }
        }

        /// <summary>
        /// Gets a value that represents the ListDataLayer associated with this data item and its parent RadListControl.
        /// The ListDataLayer encapsulates the data operations provided by RadListControl which are sorting, filtering and currency synchronization.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal ListDataLayer DataLayer
        {
            get
            {
                return this.dataLayer;
            }
            set
            {
                if (this.DataLayer == value)
                {
                    return;
                }

                this.dataLayer = value;
                this.OnNotifyPropertyChanged("DataLayer");
            }
        }

        /// <summary>
        /// Gets a value represeting the owner RadListElement of this data item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListElement Owner
        {
            get
            {
                return this.ownerElement;
            }

            internal set
            {
                if (ownerElement == value)
                {
                    return;
                }

                if (this.changingOwner)
                {
                    return;
                }

                this.changingOwner = true;
                if (this.ownerElement != null && value != null)
                {
                    this.ownerElement.Items.Remove(this);
                }

                this.ownerElement = value;
                if (this.ownerElement != null)
                {
                    this.DataLayer = this.ownerElement.DataLayer;
                }
                this.OnNotifyPropertyChanged("Owner");
                this.changingOwner = false;
            }
        }

        /// <summary>
        /// Gets a value represeting the owner control of this data item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Control OwnerControl
        {
            get
            {
                return this.ownerElement.ElementTree.Control;
            }
        }

        /// <summary>
        /// Gets or sets the visual height of this item.
        /// This property can be set only when AutoSizeItems of the parent RadListControl is true.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(-1)]
        [Description("Gets or sets the visual height of this item. This property can be set only when AutoSizeItems of the parent RadListControl is true.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Height
        {
            get 
            {
                return (int)GetValue(HeightProperty); 
            }
            set 
            {
                //if (this.Owner != null && !this.Owner.AutoSizeItems)
                //{
                //    throw new InvalidOperationException("The Height property can not be set when AutoSizeItems of the parent RadListElement is false.");
                //}
                SetValue(HeightProperty, value);
                this.Owner.Scroller.UpdateScrollRange();
                this.Owner.ViewElement.UpdateItems();
            }
        }

        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [Description("Determines whether text wrap is enabled.")]
        [RadPropertyDefaultValue("TextWrap", typeof(RadListDataItem)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool TextWrap
        {
            get
            {
                return (bool)this.GetValue(LightVisualElement.TextWrapProperty);
            }
            set
            {
                this.SetValue(LightVisualElement.TextWrapProperty, value);                
            }
        }

        /// <summary>
        /// Gets the index of this data item in the Items collection of RadListControl.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RowIndex
        {
            get 
            {
                if (this.dataLayer == null)
                {
                    return -1;
                }

                return this.dataLayer.GetRowIndex(this);
            }
        }

        /// <summary>
        /// Gets a value that will be used in the visual representation of this item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DisplayValue
        {
            get 
            {
                if (this.dataLayer == null)
                {
                    return "";
                }

                return this.dataLayer.GetDisplayValue(this);
            }
        }

        /// <summary>
        /// Gets or sets a value for the property indicated by ValueMember if in bound mode, and private value in unbound mode.
        /// Trying to explicitly set this property in bound mode will result in an InvalidOperationException.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object Value
        {
            get 
            {
                if (this.IsDataBound)
                {
                    return this.GetBoundValue();
                }
                else
                {
                    return this.GetUnboundValue();
                }
            }

            set
            {
                if (this.IsDataBound)
                {
                    throw new InvalidOperationException("The Value property can not be set while in bound mode.");
                }
                else
                {
                    this.SetUnboundValue(value);
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Active
        {
            get
            {
                return (bool)this.GetValue(RadListDataItem.ActiveProperty);
            }

            set
            {
                this.SetValue(RadListDataItem.ActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this item is selected. Setting this property will cause the selection events of the owner list control to fire if there is one.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [Description("Gets or sets a value that indicates if this item is selected. Setting this property will cause the selection events of the owner list control to fire if there is one.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Selected
        {
            get
            {
                return (bool)this.GetValue(RadListDataItem.SelectedProperty);
            }

            set
            {
                if (value == this.Selected)
                {
                    return;
                }
                else
                {
                    this.SetValue(RadListDataItem.SelectedProperty, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this item responds to GUI events.
        /// </summary>
        [Browsable(true)]
        [RadPropertyDefaultValue("Enabled", typeof(RadElement))]
        [Description("Gets or sets whether this item responds to GUI events.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Enabled
        {
            get
            {
                return (bool)this.GetValue(RadElement.EnabledProperty);
            }

            set
            {
                this.SetValue(RadElement.EnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue("")]
        [Description("Gets or sets the text for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual string Text
        {
            get
            {
                // Allow the user to set the text herself/himself. If there is no local value we use the string that the data layer provides.
                if (this.GetValueSource(LightVisualElement.TextProperty) != ValueSource.Local)
                {
                    if (this.dataBoundItem != null)
                    {
                        object value = this.DisplayValue;
                        return value != null ? value.ToString() : this.dataBoundItem.ToString();
                    }
                }

                return (string)this.GetValue(LightVisualElement.TextProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a text value that is used for sorting. Creating a RadProperty during data binding is too slow, this is why
        /// this property is used instead and its value can be used for sorting.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal string CachedText
        {
            get
            {
                return this.cachedText;
            }

            set
            {
                if (this.cachedText == value)
                {
                    return;
                }

                this.cachedText = value;
                this.OnNotifyPropertyChanged("CachedText");
            }
        }

        /// <summary>
        /// Gets or sets an image for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(null)]
        [Description("Gets or sets an image for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Image
        {
            get
            {
                return (Image)this.GetValue(LightVisualElement.ImageProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.ImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text-image relation for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(TextImageRelation.ImageBeforeText)]
        [Description("Gets or sets the text-image relation for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return (TextImageRelation)this.GetValue(LightVisualElement.TextImageRelationProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.TextImageRelationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image alignment for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the image alignment for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment ImageAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(LightVisualElement.ImageAlignmentProperty);
            }
            set
            {
                this.SetValue(LightVisualElement.ImageAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text alignment for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the text alignment for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TextAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(LightVisualElement.TextAlignmentProperty);
            }
            set
            {
                this.SetValue(LightVisualElement.TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text orientation for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(Orientation.Horizontal)]
        [Description("Gets or sets the text orientation for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Orientation TextOrientation
        {
            get
            {
                return (Orientation)this.GetValue(LightVisualElement.TextOrientationProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.TextOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [RadPropertyDefaultValue("Font", typeof(VisualElement))]
        [Description("Gets or sets the font for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font Font
        {
            get
            {
                return (Font)this.GetValue(LightVisualElement.FontProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.FontProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text color for this RadListDataItem instance.
        /// </summary>
        [Browsable(true)]
        [RadPropertyDefaultValue("ForeColor", typeof(VisualElement))]
        [Description("Gets or sets the text color for this RadListDataItem instance.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ForeColor
        {
            get
            {
                return (Color)this.GetValue(LightVisualElement.ForeColorProperty);
            }

            set
            {
                this.SetValue(LightVisualElement.ForeColorProperty, value);
            }
        }

        /// <summary>
        /// Gets a value that indicates if this item is currently visible.
        /// </summary>
        [Browsable(false)]
        [Description("Gets a value that indicates if this item is currently visible.")]
        public bool IsVisible
        {
            get
            {
                return this.VisualItem != null;
            }
        }

        /// <summary>
        /// Gets a value that visually represents this data item. If the item is not visible, this property returns null.
        /// The visual item returned should be used only to get information about a particular item. Since visual items
        /// are shared between different data items, properties must not be set directly on the visual item in order 
        /// to avoid uncustomizable or unwanted behavior. For example if properties are set directly to the visual item
        /// the themes may not work correctly.
        /// </summary>
        [Browsable(false)]
        public RadListVisualItem VisualItem
        {
            get
            {
                if (this.Owner != null)
                {
                    foreach (RadElement element in this.Owner.ViewElement.Children)
                    {
                        if (element is RadListVisualItem)
                        {
                            RadListVisualItem visualElement = element as RadListVisualItem;
                            if (visualElement.Data == this)
                            {
                                return visualElement;
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the preferred size for the element which will present this item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the preferred size for the element which will present this item.")]
        public SizeF MeasuredSize
        {
            get
            {
                return this.measuredSize;
            }
            set
            {
                this.measuredSize = value;
            }
        }
            
        #endregion

        #region IDataItem Members

        /// <summary>
        /// Gets or sets a value that represents the raw data item that this RadListDataItem is associated with.
        /// This property will be non null when the item is created via RadListControl's data binding and will contain the underlaying data item. Setting this property explicitly will have no effect in unbound mode and will throw an InvalidOperationException in bound mode.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataBoundItem
        {
            get
            {
                return this.dataBoundItem;
            }
            set
            {
                this.SetDataBoundItem(false, value);
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int FieldCount
        {
            get 
            {
                return 1; 
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object this[int index]
        {
            get
            {
                if (this.dataLayer == null)
                {
                    return null;
                }

                return this.dataLayer.GetDisplayValue(this);
            }
            set
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object this[string name]
        {
            get
            {
                if (this.dataLayer == null)
                {
                    return null;
                }

                return this.dataLayer.GetDisplayValue(this);
            }
            set
            {
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int IndexOf(string name)
        {
            return 0;
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == LightVisualElement.TextProperty)
            {
                this.cachedText = (string)e.NewValue;
                if (this.ownerElement != null && this.dataLayer != null)
                {
                    int prevIndex = this.ownerElement.SelectedIndex;
                    RadListDataItem selectedItem = this.ownerElement.SelectedItem;
                    this.ownerElement.SuspendSelectionEvents = true;

                    this.dataLayer.ListSource.Refresh();

                    this.ownerElement.SelectedItem = selectedItem;
                    this.ownerElement.SuspendSelectionEvents = false;

                    if (prevIndex != this.ownerElement.SelectedIndex)
                    {
                        this.ownerElement.OnSelectedIndexChanged(this.ownerElement.SelectedIndex);
                    }
                }
            }

            if (this.Owner != null)
            {
                this.Owner.OnDataItemPropertyChanged(this, e);
            }

            base.OnPropertyChanged(e);
        }

        public override string ToString()
        {
            return this.Text;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets a value for the Value property while in bound mode.
        /// </summary>
        /// <returns>Gets an object reference pointing to the value of the Value property in bound mode.</returns>
        protected virtual object GetBoundValue()
        {
            if (this.dataLayer.ValueMember == "")
            {
                return this.dataBoundItem;
            }

            return this.dataLayer.GetValue(this);
        }

        /// <summary>
        /// Gets a value for the Value property in unbound mode.
        /// </summary>
        /// <returns>Returns an object reference pointing to the value of the Value property in unbound mode.</returns>
        protected virtual object GetUnboundValue()
        {
            return this.GetValue(RadListDataItem.ValueProperty);
        }

        /// <summary>
        /// This method is called when setting the Value property of a RadListDataItem when it is in unbound mode.
        /// </summary>
        /// <param name="value">The value to set the Value property to.</param>
        protected virtual void SetUnboundValue(object value)
        {
            this.SetValue(RadListDataItem.ValueProperty, value);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// This method is used to assign the DataBoundItem property of this RadListDataItem.
        /// If a user attempts to set DataBoundItem while in bound mode, an exception should be thrown.
        /// In unbound mode this property can be set to any value and will not affect the behavior of this RadListDataItem.
        /// </summary>
        /// <param name="dataBinding">A flag that indicates if the data bound item is being set from the data binding engine or by the user.
        /// true means it is being set by the data binding engine.</param>
        /// <param name="value">The value that will be assigned to the DataBoundItem property.</param>
        protected internal virtual void SetDataBoundItem(bool dataBinding, object value)
        {
            if (!dataBinding && this.dataLayer != null && this.dataLayer.DataSource != null)
            {
                throw new InvalidOperationException("DataBoundItem can not be set explicitly in bound mode.");
            }

            this.dataBoundItem = value;
            if (this.Owner != null && this.Owner.DataSource != null)
            {
                this.Owner.OnListItemDataBound(this);
            }
            this.cachedText = this.dataLayer.GetUnformattedValue(this);
            this.OnNotifyPropertyChanged("DataBoundItem");
        }
        #endregion
    }
}
