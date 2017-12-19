using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using System.Collections;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
	public class RadQuickAccessToolBar : RadItem
	{
		private RadItemOwnerCollection items;
        private StackLayoutPanel baseLayout;
		private FillPrimitive stripFill;
		private BorderPrimitive border;
		private RadToolStripOverFlowButtonElement overFlowButton;
        private RadRibbonBarElement ribbonBarElement;
        private RadMenuItem toolbarPositionMenuItem = new RadMenuItem();
        private RadMenuItem minimizeRibonMenuItem = new RadMenuItem();
        private SizeF lastMeasureSize = Size.Empty;

        public RadQuickAccessToolBar(RadRibbonBarElement ribbonBar)
		{
		    this.ribbonBarElement = ribbonBar;
		}

        static RadQuickAccessToolBar()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new QuickAccessToolbarStateManagerFactory(), typeof(RadQuickAccessToolBar));
        }

        internal static RadProperty IsCollapsedByUserProperty = RadProperty.Register("IsCollapsedByUser", 
            typeof(bool), typeof(RadQuickAccessToolBar), new RadElementPropertyMetadata(false, ElementPropertyOptions.None));

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] 
            { 
                typeof(RadButtonElement), typeof(RadToggleButtonElement), 
                typeof(RadRepeatButtonElement), typeof(RadCheckBoxElement), 
				typeof(RadImageButtonElement), typeof(RadRadioButtonElement),  
                typeof(RadDropDownButtonElement), typeof(RadSplitButtonElement), 
                typeof(RadToolStripSeparatorItem)
            };

            this.items.DefaultType = typeof(RadButtonElement);
            this.items.ItemsChanged += new ItemChangedDelegate(OnRadQuickAccessBar_ItemsChanged);

            this.MaxSize = new Size(0, 30);
        }

        public RadRibbonBarElement ParentRibbonBar
        {
            get
            {
                return this.ribbonBarElement;
            }
        }

		/// <summary>Refreshes the inner items Z-index.</summary>
		public void RefreshItems()
		{
			this.SuspendLayout();

			int i = 1;
			//changing Zedindex, when item is added
			foreach (RadElement item in this.Items)
			{
				item.ZIndex = this.Items.Count - i;
				i++;
			}

			this.ResumeLayout(true);
		}

       
		/// <summary>Gets the items in the tabstrip.</summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public class InnerItem : RadItem
		{
            static InnerItem()
            {
                ItemStateManagerFactoryRegistry.AddStateManagerFactory(new InnerItemStateManagerFactory(), typeof(RadQuickAccessToolBar.InnerItem));
            }

            private StackLayoutPanel stackLayout;
			private RadQuickAccessToolBar toolBar;
			private FillPrimitive fill;
			private BorderPrimitive border;

			protected override void InitializeFields()
            {
                base.InitializeFields();

                this.MinSize = new Size(0, 23);
                this.MaxSize = new Size(0, 23);
            }

			public StackLayoutPanel StripLayout
			{
				get
				{
					return this.stackLayout;
				}
			}

            public void ShowFillAndBorder(bool value)
            {
                if (this.fill != null)
                    this.fill.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);

                if (this.border != null)
                    this.border.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);


                if (!value)
                {
                    if (this.fill != null)
                        this.border.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);

                    if (this.border != null)
                        this.border.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Hidden);
                }
            }

			public RadQuickAccessToolBar ParentToolBar
			{
				get
				{
					if (this.toolBar == null)
					{
						for (RadElement res = this.Parent; res != null && this.toolBar == null; res = res.Parent)
						{
							this.toolBar = res as RadQuickAccessToolBar;
						}
					}
					return this.toolBar;
				}
			}

			protected override void CreateChildElements()
			{
				this.fill = new FillPrimitive();
				this.fill.Class = "InnerItemFill";
				this.fill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

				this.border = new BorderPrimitive();
				this.border.Class = "InnerItemBorder";
				this.border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

                this.stackLayout = new StackLayoutPanel();
                this.stackLayout.Orientation = Orientation.Horizontal;
                this.stackLayout.EqualChildrenHeight = true;
                this.stackLayout.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
                this.stackLayout.Class = "QuickAccessToolBarStripLayout";
                this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				this.Children.Add(this.fill);
                this.Children.Add(this.stackLayout);
				this.Children.Add(this.border);
			}           

		}

        
		/// <summary>
		/// ///////////////////
		/// </summary>

		private InnerItem quickAccessItemsPanel;

		public InnerItem GetInnerItem()
		{
			if (this.quickAccessItemsPanel != null)
				return quickAccessItemsPanel;
			return null;
		}

		protected override void CreateChildElements()
		{
			this.quickAccessItemsPanel = new InnerItem();
			this.quickAccessItemsPanel.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			
			this.baseLayout = new StackLayoutPanel();
			this.baseLayout.Orientation = Orientation.Horizontal;
			this.baseLayout.EqualChildrenHeight = true;
            this.baseLayout.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.baseLayout.Class = "QuickAccessToolBarBaseStripLayout";

			this.overFlowButton = new RadToolStripOverFlowButtonElement();
			this.overFlowButton.Class = "QuickAccessToolBarOverFlow";
			this.overFlowButton.Click += new EventHandler(overFlowButton_Click);
			this.overFlowButton.Margin = new Padding(3, 0, 0, 0 );
			this.overFlowButton.Visibility = ElementVisibility.Collapsed;
			this.overFlowButton.MaxSize = new Size(0, 16);
			this.overFlowButton.OverFlowPrimitive.Alignment = ContentAlignment.MiddleCenter;
			this.overFlowButton.Alignment = ContentAlignment.MiddleLeft;
            this.overFlowButton.ThemeRole = "QuickAccessOverflowButton";

			this.stripFill = new FillPrimitive();
			this.stripFill.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.stripFill.Class = "QuickAccessToolBarFill";

			this.border = new BorderPrimitive();
			this.border.Class = "QuickAccesstBorder";
			this.border.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

			this.baseLayout.Children.Add(this.quickAccessItemsPanel);
			this.baseLayout.Children.Add(this.overFlowButton);
			
			this.Children.Add(this.stripFill);
			this.Children.Add(this.baseLayout);
			this.Children.Add(this.border);

			this.items.Owner = quickAccessItemsPanel.StripLayout;
			this.RefreshItems();
		}

		internal class RadMenuAssociatedItem : RadMenuItem
		{
			private RadItem associatedItem;

			public RadMenuAssociatedItem(RadItem associatedItem)
			{
				this.associatedItem = associatedItem;
			}

			public RadItem AssociatedItem
			{
				get
				{
					return this.associatedItem;
				}
			}

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadMenuItem);
                }
            }
		}

        private void UnwireEvents()
        {
            this.overFlowButton.Items.Remove(this.toolbarPositionMenuItem);
            this.overFlowButton.Items.Remove(this.minimizeRibonMenuItem);

            while (this.overFlowButton.Items.Count > 0)
            {
                RadItem item = this.overFlowButton.Items[0];
                item.Click -= OnMenuItemShowHideItem_Click;
                item.Dispose();
            }

            this.toolbarPositionMenuItem.Click -= new EventHandler(OnMenuItemQuickAccessPosition_Click);
            this.minimizeRibonMenuItem.Click -= new EventHandler(OnMinimizeItem_Click);
        }

        private void PrepareDropDownItems()
        {
            this.UnwireEvents();

            foreach (RadItem item in this.Items)
            {
                if (item is RadToolStripSeparatorItem)
                    continue;
                RadMenuAssociatedItem menuItem = new RadMenuAssociatedItem(item);
                menuItem.Text = item.Text;

                if (!(bool)item.GetValue(IsCollapsedByUserProperty))
                    menuItem.IsChecked = true;
                else
                    menuItem.IsChecked = false;

                menuItem.Click += OnMenuItemShowHideItem_Click;
                this.overFlowButton.MinSize = new Size(this.overFlowButton.MinSize.Width,
                    Math.Max(this.overFlowButton.MinSize.Height, item.Size.Height));
                this.overFlowButton.Items.Add(menuItem);
            }

            this.overFlowButton.Items.Add(new RadMenuSeparatorItem());

            this.toolbarPositionMenuItem.Text =
                this.ribbonBarElement.QuickAccessToolbarBelowRibbon ? this.ribbonBarElement.LocalizationSettings.ShowQuickAccessMenuAboveItemText :
                this.ribbonBarElement.LocalizationSettings.ShowQuickAccessMenuBelowItemText;

            this.overFlowButton.Items.Add(this.toolbarPositionMenuItem);
            this.overFlowButton.Items.Add(this.minimizeRibonMenuItem);

            RadRibbonBar ribbon = this.ParentRibbonBar.ElementTree.Control as RadRibbonBar;
            if (ribbon != null)
            {
                if (ribbon.Expanded)
                    this.minimizeRibonMenuItem.Text = this.ribbonBarElement.LocalizationSettings.MinimizeRibbonItemText;
                else
                    this.minimizeRibonMenuItem.Text = this.ribbonBarElement.LocalizationSettings.MaximizeRibbonItemText;
            }

            this.toolbarPositionMenuItem.Click += new EventHandler(OnMenuItemQuickAccessPosition_Click);
            this.minimizeRibonMenuItem.Click += new EventHandler(OnMinimizeItem_Click);
        }

        private void ResetQuickAccessItemVisibility(RadMenuAssociatedItem item, bool visible)
        {
            if (item.AssociatedItem == null)
            {
                return;
            }

            item.IsChecked = visible;

            this.SetItemVisibility(item.AssociatedItem, visible);
        }

        /// <summary>
        /// This method defines whether a Quick Access Toolbar item is visible or not.
        /// If the method is called to hide an item, its Visibility property is set to Collapsed
        /// and the corresponding menu item in the overflow button is unchecked.
        /// The method throws an InvalidOperationException if the item does not below
        /// to the current QAT collection.
        /// </summary>
        /// <param name="item">The item which visibility will be modified.</param>
        /// <param name="isVisible">True to show an item, false to collapse it.</param>
        public void SetItemVisibility(RadItem item, bool isVisible)
        {
            if (!this.items.Contains(item))
            {
                throw new InvalidOperationException("Item not found in the target collection.");
            }

            if (!isVisible)
            {
                item.Visibility = ElementVisibility.Collapsed;
            }
            
            item.SetValue(IsCollapsedByUserProperty, !isVisible);

            this.InvalidateMeasure();
            this.InvalidateArrange();
            this.UpdateLayout();
            this.quickAccessItemsPanel.InvalidateMeasure();
            this.quickAccessItemsPanel.InvalidateArrange();
            this.quickAccessItemsPanel.UpdateLayout();
        }

        #region New layouts

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            this.lastMeasureSize = availableSize;

            bool isDesignMode = this.IsDesignMode;
            this.quickAccessItemsPanel.Measure(availableSize);

            base.MeasureOverride(availableSize);
            float currentWidth = 
                + this.overFlowButton.DesiredSize.Width + 25;
            RadItem previousVisibleItem = null;
            foreach (RadItem item in this.Items)
            {
                if ((bool)item.GetValue(IsCollapsedByUserProperty))
                {
                    continue;
                }

                SizeF itemSize = item.GetDesiredSize(false);
                if (isDesignMode)
                {
                    currentWidth += itemSize.Width;
                    continue;
                }

                if (currentWidth + itemSize.Width >= availableSize.Width)
                {
                    item.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    if (item is RadToolStripSeparatorItem)
                    {
                        if (previousVisibleItem == null || 
                            previousVisibleItem is RadToolStripSeparatorItem)
                        {
                            item.Visibility = ElementVisibility.Collapsed;
                            continue;
                        }
                    }
                    item.Visibility = ElementVisibility.Visible;
                    currentWidth += itemSize.Width;
                    previousVisibleItem = item;
                }
            }

            if (previousVisibleItem is RadToolStripSeparatorItem)
            {
                SizeF itemSize = previousVisibleItem.GetDesiredSize(false);
                previousVisibleItem.Visibility = ElementVisibility.Collapsed;
                currentWidth -= itemSize.Width;
            }

            return new SizeF(currentWidth, availableSize.Height);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.RightToLeft)
            {
                float yCoord = Math.Abs(this.DesiredSize.Height - this.overFlowButton.DesiredSize.Height) / 2;
                float xCoord = finalSize.Width - this.quickAccessItemsPanel.DesiredSize.Width - this.overFlowButton.DesiredSize.Width;
                if (xCoord < 0)
                {
                    xCoord = 0;
                }

                PointF overFlowButtonLocation = new PointF(xCoord, yCoord);
                this.overFlowButton.Arrange(new RectangleF(overFlowButtonLocation, this.overFlowButton.DesiredSize));

                xCoord = finalSize.Width - this.quickAccessItemsPanel.DesiredSize.Width;

                if (xCoord < 0)
                {
                    xCoord = this.overFlowButton.DesiredSize.Width;
                }
                this.quickAccessItemsPanel.Arrange(new RectangleF(new PointF(xCoord, 0), this.quickAccessItemsPanel.DesiredSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        #endregion

        #region Helper methods

        public RadItem GetLastVisibleItem()
        {
            for (int i = this.Items.Count - 1; i > -1 ; i--)
            {
                RadItem currentItem = this.Items[i];

                if (currentItem.Visibility == ElementVisibility.Visible)
                {
                    return currentItem;
                }
            }

            return null;
        }

        public RadItem GetFirstCollapsedItem()
        {
            foreach (RadItem item in this.Items)
            {
                if (item.Visibility == ElementVisibility.Collapsed && !(bool)item.GetValue(IsCollapsedByUserProperty))
                {
                    return item;
                }
            }

            return null;
        }

        #endregion

        #region Properties

        private int CurrentWidth
        {
            get
            {
                int width = 0;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    RadItem currentItem = this.Items[i];
                    if (currentItem.Visibility == ElementVisibility.Visible)
                    {
                        width += currentItem.FullBoundingRectangle.Width;
                    }
                }

                width += this.Margin.Left + this.Margin.Right;
                width += this.Padding.Left + this.Padding.Right;

                return width;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadMenuItem ToolbarPositionMenuItem
        {
            get { return toolbarPositionMenuItem; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadMenuItem MinimizeRibonMenuItem
        {
            get { return minimizeRibonMenuItem; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripOverFlowButtonElement OverflowButtonElement
        {
            get
            {
                return this.overFlowButton;
            }
        }

        #endregion

        #region Event handling

        private void overFlowButton_Click(object sender, EventArgs e)
        {
            this.PrepareDropDownItems();
            this.overFlowButton.ShowDropDown();
        }

        private void OnRadQuickAccessBar_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                if (this.overFlowButton != null)
                {
                    if (!this.IsDesignMode)
                    {
                        this.overFlowButton.Visibility = ElementVisibility.Visible;
                    }
                    else
                    {
                        this.overFlowButton.Visibility = ElementVisibility.Hidden;
                    }
                }

                target.MaxSize = new Size(0, 18);
                target.SetDefaultValueOverride( RadItem.PaddingProperty,new Padding(2, 1, 2, 2));

                if (target.GetType() == typeof(RadButtonElement))
                {
                    RadButtonElement buttonElement = target as RadButtonElement;

                    buttonElement.Class = "RibbonBarButtonElement";
                    buttonElement.BorderElement.Class = "ButtonInRibbonBorder";
                    buttonElement.ButtonFillElement.Class = "ButtonInRibbonFill";
                }
            }

            this.RefreshItems();
        }


        private void OnMinimizeItem_Click(object sender, EventArgs e)
        {
            if (this.ParentRibbonBar.ElementTree != null)
            {
                RadRibbonBar ribbon = (RadRibbonBar)this.ParentRibbonBar.ElementTree.Control;

                if (ribbon.CommandTabs.Count > 0)
                {
                    ribbon.Expanded = !ribbon.Expanded;
                }
            }
        }

        private void OnMenuItemQuickAccessPosition_Click(object sender, EventArgs e)
        {
            this.toolbarPositionMenuItem.Text = this.ribbonBarElement.LocalizationSettings.ShowQuickAccessMenuBelowItemText;

            this.ParentRibbonBar.QuickAccessToolbarBelowRibbon = !this.ParentRibbonBar.QuickAccessToolbarBelowRibbon;

            if (this.ParentRibbonBar.QuickAccessToolbarBelowRibbon)
            {
                this.toolbarPositionMenuItem.Text = this.ribbonBarElement.LocalizationSettings.ShowQuickAccessMenuAboveItemText;

            }

            this.ribbonBarElement.InvalidateMeasure();
            this.ribbonBarElement.InvalidateArrange();
            this.ribbonBarElement.UpdateLayout();
        }

        private void OnMenuItemShowHideItem_Click(object sender, EventArgs e)
        {
            RadItem item = (sender as RadMenuAssociatedItem).AssociatedItem;
            this.ResetQuickAccessItemVisibility(sender as RadMenuAssociatedItem, (bool)item.GetValue(IsCollapsedByUserProperty));
        }

        #endregion
    }

    #region State managers

    public class QuickAccessToolbarStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode nodes = new CompositeStateNode("QAToolbar states");
            StateNodeWithCondition stateNode = new StateNodeWithCondition("IsBelowRibbon", new SimpleCondition(RadRibbonBarElement.QuickAccessToolbarBelowRibbonProperty, true));
            nodes.AddState(stateNode);
            stateNode = new StateNodeWithCondition("IsBackstageMode", new SimpleCondition(RadRibbonBarElement.IsBackstageModeProperty, true));
            nodes.AddState(stateNode);
            stateNode = new StateNodeWithCondition("RightToLeft", new SimpleCondition(RadRibbonBarElement.RightToLeftProperty, true));
            nodes.AddState(stateNode);
            return nodes;
        }
    }

    public class InnerItemStateManagerFactory : ItemStateManagerFactory
    {
        protected override StateNodeBase CreateSpecificStates()
        {
            CompositeStateNode nodes = new CompositeStateNode("InnerItem states");
            StateNodeWithCondition stateNode = new StateNodeWithCondition("IsBackstageMode", new SimpleCondition(RadRibbonBarElement.IsBackstageModeProperty, true));
            nodes.AddState(stateNode);
            stateNode = new StateNodeWithCondition("RightToLeft", new SimpleCondition(RadRibbonBarElement.RightToLeftProperty, true));
            nodes.AddState(stateNode);
            return nodes;
        }
    }

    #endregion
}
