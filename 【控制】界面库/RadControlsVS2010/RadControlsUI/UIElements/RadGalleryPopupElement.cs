using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;


namespace Telerik.WinControls.UI
{
    public class RadGalleryPopupElement : RadItem
    {
        #region fields
        private RadScrollViewer scrollViewer = new RadScrollViewer();
        private StackLayoutPanel groupHolderStackLayout = new StackLayoutPanel();
        private SizeGripElement gripElement = new SizeGripElement();
        private DockLayoutPanel mainLayout = new DockLayoutPanel();
        private RadMenuElement menuElement = new RadMenuElement();
        private RadDropDownButtonElement filtersDropDownButton = new RadDropDownButtonElement();
       
        private RadGalleryGroupFilter selectedFilter;


        private RadItemOwnerCollection galleryItems;
        private RadItemOwnerCollection groups;
        private RadItemOwnerCollection menuItems;
        private RadItemOwnerCollection filters;

        private SizeF actualSize = SizeF.Empty;
        private SizeF newSize;
        private SizeF initialSize;
        private SizeF minimumSize;
        private bool filterEnabled = true;
        private SizingMode dropDownSizingMode = SizingMode.UpDownAndRightBottom;

   
        #endregion

        #region cstors
        public RadGalleryPopupElement(RadItemOwnerCollection items, 
                                      RadItemOwnerCollection groups,
                                      RadItemOwnerCollection filters,
                                      RadItemOwnerCollection menuItems,
                                      SizeF initialSize,
                                      SizeF minimumSize

            ): this(                  items, 
                                      groups,
                                      filters,
                                      menuItems,
                                      initialSize,
                                      minimumSize,
                                      SizingMode.UpDownAndRightBottom)
        {

        }

        public RadGalleryPopupElement(RadItemOwnerCollection items,
                                     RadItemOwnerCollection groups,
                                     RadItemOwnerCollection filters,
                                     RadItemOwnerCollection menuItems)

        {
            this.galleryItems = items;
            this.groups = groups;
            this.menuItems = menuItems;
            this.newSize = initialSize;
            this.filters = filters;
            this.BuildPopupContent();//read groups if any
            this.menuElement.Items.AddRange(this.menuItems);
        }

        public RadGalleryPopupElement(RadItemOwnerCollection items, 
                                      RadItemOwnerCollection groups,
                                      RadItemOwnerCollection filters,
                                      RadItemOwnerCollection menuItems,
                                      SizeF initialSize,
                                      SizeF minimumSize,
                                      SizingMode dropDownSizingMode

            )
        {
            this.galleryItems = items;
            this.groups = groups;
            this.menuItems = menuItems;
            this.newSize = initialSize;
            this.actualSize = initialSize;
            this.initialSize = initialSize;
            this.minimumSize = minimumSize;
            this.filters = filters;
            this.dropDownSizingMode = dropDownSizingMode;

            this.BuildPopupContent();//read groups if any

            this.menuElement.MinSize = new Size((int)initialSize.Width, 0);
            this.menuElement.Items.AddRange(this.menuItems);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="RadScrollViewer"/>
        /// class that represents the scrollable panel that holds
        /// the gallery items when the popup is shown.
        /// </summary>
        public RadScrollViewer ScrollViewer
        {
            get
            {
                return this.scrollViewer;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadMenuElement"/>class
        /// that represents the element holding the buttons that represent
        /// the different filters and groups.
        /// </summary>
        public RadMenuElement MenuElement
        {
            get
            {
                return this.menuElement;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="SizeGripElement"/>class
        /// that represents the sizing grip of the dropdown.
        /// </summary>
        public SizeGripElement SizingGrip
        {
            get
            {
                return this.gripElement;
            }
        }

        internal SizeF ActualSize
        {
            get
            {
                return actualSize;
            }
            set
            {
                actualSize = value;
            }
        }


        internal SizeF NewSize
        {
            get
            {
                return newSize;
            }
            set
            {
                newSize = value;
            }
        }


        internal SizeF InitialSize
        {
            get
            {
                return initialSize;
            }
            set
            {
                initialSize = value;
            }
        }


        internal SizeF MinimumSize
        {
            get
            {
                return minimumSize;
            }
            set
            {
                minimumSize = value;
            }
        }



        internal SizingMode DropDownSizingMode
        {
            get
            {
                return dropDownSizingMode;
            }
            set
            {
                dropDownSizingMode = value;
            }
        }


        internal RadDropDownButtonElement FiltersButton
        {
            get
            {
                return this.filtersDropDownButton;
            }
        }

        //TODO: refactor this function - get as is from OLD Gallery
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         Description("Gets or sets the currently selected group filter."),
         Browsable(false)]
        public RadGalleryGroupFilter SelectedFilter
        {
            get
            {
                return this.selectedFilter;
            }
            set
            {
                if (this.selectedFilter != value)
                {
                    if (this.selectedFilter != null)
                    {
                        this.selectedFilter.Selected = false;
                    }
                    this.selectedFilter = value;
                    this.selectedFilter.Selected = true;
                    this.filtersDropDownButton.Text = this.selectedFilter.Text;

                    foreach (RadGalleryGroupItem group in this.groups)
                    {
                        if (!this.selectedFilter.Items.Contains(group))
                        {
                            group.Visibility = ElementVisibility.Collapsed;
                            foreach (RadItem item in group.Items)
                            {
                                item.Visibility = ElementVisibility.Collapsed;
                            }
                        }
                        else
                        {
                            group.Visibility = ElementVisibility.Visible;
                            foreach (RadItem item in group.Items)
                            {
                                item.Visibility = ElementVisibility.Visible;
                                InvalidateAndUpdate(item);
                            }
                        }
                    }

                    foreach (RadMenuItem item in this.filtersDropDownButton.Items)
                    {
                        if (this.selectedFilter != (RadGalleryGroupFilter)item.Tag)
                            item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
                        else
                            item.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
                    }
                }
            }
        }

        public StackLayoutPanel GroupHolderStackLayout
        {
            get
            {
                return this.groupHolderStackLayout;
            }
        }


        #endregion

        #region overrides
        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            //init section
            this.GroupHolderStackLayout.Orientation = Orientation.Vertical;
            this.gripElement.SizingMode = SizingMode.UpDownAndRightBottom;
            this.scrollViewer.Viewport = this.GroupHolderStackLayout;
            this.scrollViewer.ShowFill = true;
            this.scrollViewer.ShowBorder = false;
            this.scrollViewer.HorizontalScrollState = ScrollState.AlwaysHide;
            this.scrollViewer.ScrollLayoutPanel.MeasureWithAvaibleSize = true;
            this.mainLayout.LastChildFill = true;

            this.menuElement.AllItemsEqualHeight = true;

            //dock
            DockLayoutPanel.SetDock(this.filtersDropDownButton, Dock.Top);
            DockLayoutPanel.SetDock(this.gripElement, Dock.Bottom);
            DockLayoutPanel.SetDock(this.menuElement, Dock.Bottom);
            this.scrollViewer.Class = "RadGalleryPopupScrollViewer";
            this.scrollViewer.FillElement.Class = "RadGalleryPopupScrollViewerFill";

            //children section
            this.gripElement.SizingMode = this.dropDownSizingMode;
            this.mainLayout.Children.Add(this.filtersDropDownButton);
            this.mainLayout.Children.Add(this.gripElement);
            this.mainLayout.Children.Add(this.menuElement);
            this.mainLayout.Children.Add(this.scrollViewer);
            this.Children.Add(this.mainLayout);

            this.WireEvents();//resize events
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(this.newSize);
            this.SetMenuItemsVertically();
            return this.newSize;
        }

        private void SetMenuItemsVertically()
        {
            int maxLen = 0;
            foreach(RadElement element in this.menuItems)
            {
                maxLen = Math.Max(maxLen, (int)element.DesiredSize.Width);
            }
            this.menuElement.MaxSize = new Size(maxLen, 0);

            if (this.ElementTree != null)
            {
                this.ElementTree.Control.BackColor = filtersDropDownButton.BackColor;
            }
        }

        internal void ClearCollections()
        {
            foreach (RadItem item in this.filtersDropDownButton.Items)
            {
                item.Click -= this.filterItem_Click;
            }

            this.filtersDropDownButton.Items.Clear();
            this.GroupHolderStackLayout.Children.Clear();
            this.menuElement.Items.Clear();
        }

             
        #endregion

        #region helpers

        protected void WireEvents()
        {
            this.gripElement.GripItemNS.Sizing += new ValueChangingEventHandler(this.GripItemNS_Sizing);
            this.gripElement.GripItemNSEW.Sizing += new ValueChangingEventHandler(this.GripItemNS_Sizing);

            this.gripElement.GripItemNS.Sized += new ValueChangingEventHandler(this.GripItemNS_Sized);
            this.gripElement.GripItemNSEW.Sized += new ValueChangingEventHandler(this.GripItemNS_Sized);
        }

        protected void UnWireEvents()
        {
            this.gripElement.GripItemNS.Sizing -= new ValueChangingEventHandler(this.GripItemNS_Sizing);
            this.gripElement.GripItemNSEW.Sizing -= new ValueChangingEventHandler(this.GripItemNS_Sizing);

            this.gripElement.GripItemNS.Sized -= new ValueChangingEventHandler(this.GripItemNS_Sized);
            this.gripElement.GripItemNSEW.Sized -= new ValueChangingEventHandler(this.GripItemNS_Sized);
        }

       
        internal void BuildPopupContent()
        {   
            this.BuildGroup();
            this.BuildFilters();
        }

        protected void BuildGroup()
        {
            this.galleryItems.Owner = null;

            if (this.groups.Count == 0)
            {
                RadGalleryGroupItem groupItem = new RadGalleryGroupItem(string.Empty);
                groupItem.ShowCaption = false;

                groupItem.Items.AddRange(this.galleryItems);
                groupItem.Items.Owner = groupItem.ItemsLayoutPanel;
                this.GroupHolderStackLayout.Children.Add(groupItem);
                this.InvalidateAndUpdate(groupItem.ItemsLayoutPanel);
            }
            else
            {
                foreach (RadGalleryGroupItem groupItem in this.groups)
                {
                    groupItem.Items.Owner = groupItem.ItemsLayoutPanel;
                    this.GroupHolderStackLayout.Children.Add(groupItem);
                    this.InvalidateAndUpdate(groupItem.ItemsLayoutPanel);
                }
            }

            this.InvalidateAndUpdate(this.GroupHolderStackLayout);
        }


        private void InvalidateAndUpdate( RadElement element)
        {
            element.InvalidateMeasure();
            element.InvalidateArrange();
            element.UpdateLayout();
        }

        //TODO: refactor this function - get as is from OLD Gallery
        private void BuildFilters()
        {
            if (this.filterEnabled && this.filters.Count > 0 && this.groups.Count > 0)
            {
                foreach (RadGalleryGroupFilter filter in this.filters)
                {
                    RadMenuItem item = new RadMenuItem(filter.Text);
                    item.Click += new EventHandler(filterItem_Click);
                    item.Tag = filter;
                    if (filter.Selected)
                    {
                        this.SelectedFilter = filter;
                        item.IsChecked = true;
                    }
                    this.filtersDropDownButton.Items.Add(item);
                }
                if (this.SelectedFilter == null)
                    this.SelectedFilter = this.filters[0] as RadGalleryGroupFilter;
                filtersDropDownButton.Visibility = ElementVisibility.Visible;
            }
            else
                filtersDropDownButton.Visibility = ElementVisibility.Collapsed;
        }

        private void filterItem_Click(object sender, EventArgs e)
        {
            RadGalleryGroupFilter filter = ((RadMenuItem)sender).Tag as RadGalleryGroupFilter;
            this.SelectedFilter = filter;
        }
        
        [Obsolete("This method should not be used. It will be removed in Q2 2010.")]
        public void SetChildrenOwner(RadElement owner)
        {
            this.galleryItems.Owner = owner;
            this.groups.Owner = owner;
            this.menuItems.Owner = owner;
            this.filters.Owner = owner;
        }

        #endregion

        #region Event handling

        void GripItemNS_Sized(object sender, ValueChangingEventArgs e)
        {
            this.newSize = this.actualSize + (SizeF)e.NewValue;

            if (this.newSize.Width < this.minimumSize.Width)
            {
                this.newSize.Width = this.minimumSize.Width;
            }

            if (this.newSize.Height < this.minimumSize.Height)
            {
                this.newSize.Height = this.minimumSize.Height;
            }

            if (this.newSize.Width > this.ElementTree.RootElement.MaxSize.Width && this.ElementTree.RootElement.MaxSize.Width > 0)
            {
                this.newSize.Width = this.ElementTree.RootElement.MaxSize.Width;
            }

            if (this.newSize.Height > this.ElementTree.RootElement.MaxSize.Height && this.ElementTree.RootElement.MaxSize.Height > 0)
            {
                this.newSize.Height = this.ElementTree.RootElement.MaxSize.Height;
            }

            this.InvalidateMeasure();
            this.ElementTree.RootElement.UpdateLayout();
            this.ElementTree.Control.Size = this.ElementTree.RootElement.DesiredSize.ToSize();
        }

        void GripItemNS_Sizing(object sender, ValueChangingEventArgs e)
        {
            this.actualSize = this.Size;
        }


        #endregion
    }
}

