using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.StateManagers;
using System.ComponentModel;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public class PropertyGridExpanderElement : PropertyGridContentElement
    {
        #region Dependancy Properties

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(PropertyGridExpanderElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded", typeof(bool), typeof(PropertyGridExpanderElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsControlInactiveProperty = RadProperty.Register(
            "IsInactive", typeof(bool), typeof(PropertyGridExpanderElement),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsInEditModeProperty = RadProperty.Register(
            "IsInEditMode", typeof(bool), typeof(PropertyGridExpanderElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private bool signImageSet;
        private ExpanderItem expanderItem;

        #endregion

        #region Initialization

        static PropertyGridExpanderElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridExpanderElementStateManager(), typeof(PropertyGridExpanderElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = false;
            this.DrawBorder = false;
            this.NotifyParentOnMouseInput = true;
            this.ClipDrawing = true;
            this.Class = "PropertyGridItemExpanderItem";
            this.StretchHorizontally = false;
            this.StretchVertically = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.expanderItem = new ExpanderItem();
            this.expanderItem.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
            this.expanderItem.DrawSignBorder = true;
            this.expanderItem.DrawSignFill = false;
            this.expanderItem.SignSize = new Size(9, 9);
            this.expanderItem.SignPadding = new Padding(1, 1, 1, 1);
            this.expanderItem.SignBorderColor = Color.LightGray;
            this.expanderItem.SignBorderWidth = 1;
            this.expanderItem.ForeColor = Color.Black;
            this.expanderItem.SignStyle = ExpanderItem.SignStyles.PlusMinus;
            this.expanderItem.SignBorderColor = Color.Gray;
            this.expanderItem.ShowHorizontalLine = false;
            this.expanderItem.NotifyParentOnMouseInput = true;
            this.expanderItem.DrawFill = false;
            this.expanderItem.DrawBorder = false;
            this.Children.Add(this.expanderItem);
        }

        #endregion

        #region Properties

        public ExpanderItem ExpanderItem
        {
        	get
            {
                return this.expanderItem;
            }
        }

        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.expanderItem.Expanded;
            }
            set
            {
                this.expanderItem.Expanded = value;
                this.SetValue(IsExpandedProperty, value);
            }
        }

        public bool IsInactive
        {
            get
            {
                return (bool)this.GetValue(IsControlInactiveProperty);
            }
            set
            {
                this.SetValue(IsControlInactiveProperty, value);
            }
        }
        
        #endregion

        #region Public methods

        public virtual void Synchronize()
        {
            if (this.VisualItem is PropertyGridItemElement)
            {
                PropertyGridItemElement visualItem = VisualItem as PropertyGridItemElement;

                if (visualItem != null)
                {
                    this.IsExpanded = visualItem.Data.Expanded;
                    this.IsSelected = visualItem.IsSelected;

                    PropertyGridItem dataItem = visualItem.Data as PropertyGridItem;

                    if (dataItem.Expandable)
                    {                        
                        this.expanderItem.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        this.expanderItem.Visibility = ElementVisibility.Hidden;
                    }
                }
                
                bool isInEditMode = visualItem.IsInEditMode;
                if (isInEditMode && visualItem.Data.Level == 0 && visualItem.Data.GridItems.Count > 0)
                {
                    isInEditMode = false;
                }

                this.SetValue(IsInEditModeProperty, isInEditMode);
            }

            if (this.VisualItem is PropertyGridGroupElement)
            {
                PropertyGridGroupElement visualItem = VisualItem as PropertyGridGroupElement;

                if (visualItem != null)
                {
                    this.IsExpanded = visualItem.Data.Expanded;
                    this.IsSelected = visualItem.IsSelected;

                    PropertyGridGroupItem dataItem = visualItem.Data as PropertyGridGroupItem;

                    if (dataItem.GridItems.Count == 0)
                    {
                        this.expanderItem.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        this.expanderItem.Visibility = ElementVisibility.Visible;
                    }
                }
            }

            this.UpdateSignImage();
        }

        #endregion

        #region Implementation

        protected virtual void UpdateSignImage()
        {
            PropertyGridTableElement propertyGridTableElement = PropertyGridTableElement;
            if (propertyGridTableElement == null)
                return;

            if (this.expanderItem.Expanded)
            {
                if (SetSignImage(propertyGridTableElement, PropertyGridTableElement.HoveredExpandImageProperty))
                    return;

                if (SetSignImage(propertyGridTableElement, PropertyGridTableElement.ExpandImageProperty))
                    return;
            }

            if (SetSignImage(propertyGridTableElement, PropertyGridTableElement.HoveredCollapseImageProperty))
                return;

            if (SetSignImage(propertyGridTableElement, PropertyGridTableElement.CollapseImageProperty))
                return;

            if (this.signImageSet)
            {
                this.signImageSet = false;
                this.ResetValue(ExpanderItem.SignImageProperty, ValueResetFlags.Local);
                this.ResetValue(ExpanderItem.DrawSignBorderProperty, ValueResetFlags.Local);
                this.ResetValue(ExpanderItem.DrawSignFillProperty, ValueResetFlags.Local);
            }
        }

        protected virtual bool SetSignImage(RadElement element, RadProperty property)
        {
            ValueSource valueSource = element.GetValueSource(property);
            Image img = (Image)element.GetValue(property);
            if (valueSource != ValueSource.DefaultValue && img != null)
            {
                this.expanderItem.SignStyle = ExpanderItem.SignStyles.Image;
                this.expanderItem.SignImage = img;
                this.expanderItem.DrawSignBorder = false;
                this.expanderItem.DrawSignFill = false;
                this.signImageSet = true;
                return true;
            }

            return false;
        }

        #endregion

        #region Event handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsExpandedProperty)
            {
                this.UpdateSignImage();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.expanderItem.Visibility == ElementVisibility.Visible && e.Button == MouseButtons.Left)
            {
                this.VisualItem.Data.Expanded = !this.VisualItem.Data.Expanded;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.ElementTree.Control.Cursor = Cursors.Default;
        }

        #endregion
        
        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = Size.Empty;
            PropertyGridItemElement visualItem = this.VisualItem as PropertyGridItemElement;
            PropertyGridTableElement propertyGridTableElement = this.PropertyGridTableElement;
            if (propertyGridTableElement != null && visualItem != null)
            {
                PropertyGridItem dataItem = visualItem.Data as PropertyGridItem;
                if (dataItem != null)
                {
                    desiredSize.Width = propertyGridTableElement.ItemIndent;
                    if (float.IsPositiveInfinity(availableSize.Height))
                    {
                        desiredSize.Height = propertyGridTableElement.ItemHeight;
                    }
                    else
                    {
                        desiredSize.Height = availableSize.Height;
                    }
                }
            }

            return desiredSize;
        }

        #endregion  
    }
}
