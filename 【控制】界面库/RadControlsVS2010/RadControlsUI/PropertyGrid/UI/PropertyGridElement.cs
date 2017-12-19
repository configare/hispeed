using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;

namespace Telerik.WinControls.UI
{
    public class PropertyGridElement : StackLayoutElement
    {
        #region Fields

        private PropertyGridSplitElement splitElement;
        private PropertyGridToolbarElement toolbarElement;

        #endregion

        #region Constructors & Initialization

        static PropertyGridElement()
        {
            new Themes.ControlDefault.PrpertyGrid().DeserializeTheme();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.splitElement = new PropertyGridSplitElement();
            this.splitElement.StretchHorizontally = true;
            this.splitElement.StretchVertically = true;
            this.toolbarElement = new PropertyGridToolbarElement();
            this.toolbarElement.StretchHorizontally = true;
            this.toolbarElement.Visibility = ElementVisibility.Collapsed;
            this.Children.Add(this.toolbarElement);
            this.Children.Add(this.splitElement);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.Orientation = Orientation.Vertical;
            this.FitInAvailableSize = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="PropertyGridSplitElement"/>.
        /// </summary>
        public PropertyGridSplitElement SplitElement
        {
        	get
            {
                return this.splitElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyTableElement"/>.
        /// </summary>
        public PropertyGridTableElement PropertyTableElement
        {
            get
            {
                return this.splitElement.PropertyTableElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridToolbarElement"/>
        /// </summary>
        public PropertyGridToolbarElement ToolbarElement
        {
        	get
            {
                return this.toolbarElement;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PropertyGridToolbarElement"/> should be visible.
        /// </summary>
        public bool ToolbarVisible
        {
            get
            {
                return this.toolbarElement.Visibility == ElementVisibility.Visible;
            }
            set
            {
                if (value)
                {
                    this.toolbarElement.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.toolbarElement.Visibility = ElementVisibility.Collapsed;
                }

                this.OnNotifyPropertyChanged("SearchBarVisible");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting is enabled.
        /// </summary>
        public bool EnableSorting
        {
            get
            {
                return this.PropertyTableElement.CollectionView.CanSort;
            }
            set
            {
                if (EnableSorting != value)
                {
                    this.PropertyTableElement.CollectionView.CanSort = value;
                    if (value)
                    {
                        this.ToolbarElement.AlphabeticalToggleButton.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        this.ToolbarElement.AlphabeticalToggleButton.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grouping is enabled.
        /// </summary>
        public bool EnableGrouping
        {
            get
            {
                return this.PropertyTableElement.CollectionView.CanGroup;
            }
            set
            {
                if (EnableGrouping != value)
                {
                    this.PropertyTableElement.CollectionView.CanGroup = value;
                    if (value)
                    {
                        this.ToolbarElement.CategorizedAlphabeticalToggleButton.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        this.ToolbarElement.CategorizedAlphabeticalToggleButton.Visibility = ElementVisibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled.
        /// </summary>
        public bool EnableFiltering
        {
            get
            {
                return this.PropertyTableElement.CollectionView.CanFilter;
            }
            set
            {
                if (EnableFiltering != value)
                {
                    this.PropertyTableElement.CollectionView.CanFilter = value;
                    ((TextBox)this.ToolbarElement.SearchTextBoxElement.TextBoxItem.HostedControl).ReadOnly = !value;
                }
            }
        }

        #endregion
        
        #region Methods

        /// <summary>
    /// Expands all the categories in the <see cref="Telerik.WinControls.UI.PropertyGridElement"/>.
        /// </summary>
        public void ExpandAllGridItems()
        {
            this.PropertyTableElement.BeginUpdate();
            foreach (PropertyGridGroupItem group in this.PropertyTableElement.Groups)
            {
                group.Expanded = true;
                SetExpandedState(group.GridItems, true);
            }
            if (this.PropertyTableElement.Groups.Count == 0)
            {
                foreach (PropertyGridItem item in this.PropertyTableElement.CollectionView)
                {
                    if (item.Expandable)
                    {
                        item.Expanded = true;
                        SetExpandedState(item.GridItems, true);
                    }
                }
            }
            this.PropertyTableElement.EndUpdate(true, PropertyGridTableElement.UpdateActions.ExpandedChanged);
        }

        /// <summary>
        /// Collapses all the categories in the <see cref="Telerik.WinControls.UI.PropertyGridElement"/>.
        /// </summary>
        public void CollapseAllGridItems()
        {
            this.PropertyTableElement.BeginUpdate();
            foreach (PropertyGridGroupItem group in this.PropertyTableElement.Groups)
            {
                group.Expanded = false;
                SetExpandedState(group.GridItems, false);
            }
            if (this.PropertyTableElement.Groups.Count == 0)
            {
                foreach (PropertyGridItem item in this.PropertyTableElement.CollectionView)
                {
                    if (item.Expandable)
                    {
                        item.Expanded = false;
                        SetExpandedState(item.GridItems, false);
                    }
                }
            }
            this.PropertyTableElement.EndUpdate(true, PropertyGridTableElement.UpdateActions.ExpandedChanged);
        }

        private void SetExpandedState(IList<PropertyGridItem> items, bool newState)
        {
            foreach (PropertyGridItem item in items)
            {
                if (item.Expandable)
                {
                    item.Expanded = newState;
                    SetExpandedState(item.GridItems, newState);
                }
            }
        }

        /// <summary>
        /// Resets the selected property to its default value.
        /// </summary>
        public void ResetSelectedProperty()
        {
            PropertyGridItem item = this.PropertyTableElement.SelectedGridItem as PropertyGridItem;
            if (item != null)
            {
                item.ResetValue();
            }
        }
        
        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);
            desiredSize.Height = Math.Min(availableSize.Height, desiredSize.Height);
            return desiredSize;
        }

        protected override void OnStyleChanged(RadPropertyChangedEventArgs e)
        {
            base.OnStyleChanged(e);

            if (this.PropertyTableElement.IsEditing)
            {
                this.PropertyTableElement.CancelEdit();
            }

            if (e.NewValue != null)
            {
                this.PropertyTableElement.ViewElement.DisposeChildren();
                this.PropertyTableElement.ViewElement.ElementProvider.ClearCache();
                this.PropertyTableElement.ViewElement.InvalidateMeasure();
            }
        }

        #endregion
    }
}
