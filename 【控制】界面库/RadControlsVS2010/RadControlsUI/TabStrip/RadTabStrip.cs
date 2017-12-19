using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a tab strip. The RadTabStrip class is a simple wrapper for the
    ///     <see cref="RadTabStripElement">RadTabStripElement</see> class. All UI and logic
    ///     functionality is implemented in
    ///     <see cref="RadTabStripElement">RadTabStripElement</see>. The RadTabStrip acts to
    ///     transfer events to and from its corresponding
    ///     <see cref="RadTabStripElement">RadTabStripElement</see> instance. The
    ///     <see cref="RadTabStripElement">RadTabStripElement</see> which is essentially the
    ///     RadTabStrip control may be nested in other telerik controls.
    /// </summary>
	[Designer( DesignerConsts.RadTabStripDesignerString)]
    [RadThemeDesignerData(typeof(RadTabStripDesignTimeData))]
	[ToolboxItem(false)]
	[Description("Builds tabbed interfaces with rich formatting and behavior")]
	[DefaultProperty("Items"), DefaultEvent("TabSelected")]
    [Obsolete("This control is obsolete. Use RadPageView instead.")]
	public class RadTabStrip : RadItemsControl
	{
    	private RadTabStripElement tabStripElement;
		private bool enableTabControlMode;

                protected override System.Drawing.Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        protected override void OnLoad(Size desiredSize)
        {
            base.OnLoad(desiredSize);

            this.ProcessKeyboard = true;
        }

		/// <commentsfrom cref="RadTabStripElement.Items" filter=""/>
		[RadEditItemsAction]
        [RadNewItem("Type here", true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[RadDescription("Items", typeof(RadTabStripElement))]
		public override RadItemOwnerCollection Items
		{
			get
			{
				return this.tabStripElement.Items;
			}
		}




        /// <summary>
        /// Gets the instance of RadTabStripElement wrapped by this control. RadTabStripElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadTabStrip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadTabStripElement TabStripElement
		{
			get
			{
				return this.tabStripElement;
			}
		}

        private bool autoScroll; 

		/// <commentsfrom cref="RadTabStripElement.AutoScroll" filter=""/>
		[Browsable(false), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("AutoScroll", typeof(RadTabStripElement))]
		[RadDefaultValue("AutoScroll", typeof(RadTabStripElement))]
		public override bool AutoScroll
		{
			get
			{
				return autoScroll;
			}
			set
			{
				this.autoScroll = value;
                this.tabStripElement.AutoScroll = value;
			}
		}

        [Browsable(false)]
        public new Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }

            set
            {
                base.AutoScrollMargin = value;
            }
        }

        [Browsable(false)]
        public new Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }

            set
            {
                base.AutoScrollMinSize = value;
            }
        }

		/// <summary>
		///Gets or sets a value indicating whether the user can edit the text of  tab items.
		/// </summary>        
		[Category("Behavior"), DefaultValue(false)]
		[Description("Gets or sets a value indicating whether the user can edit the text of tab items.")]
		public bool AllowEdit
		{
			get
			{
				return this.tabStripElement.AllowEdit;
			}
			set
			{
				this.tabStripElement.AllowEdit = value;
			}
		}
			
		/// <commentsfrom cref="RadTabStripElement.ShrinkMode" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ShrinkMode", typeof(RadTabStripElement))]
		[RadDefaultValue("ShrinkMode", typeof(RadTabStripElement))]
		public bool ShrinkMode
		{
			get
			{
				return this.tabStripElement.ShrinkMode;
			}

			set
			{
				this.tabStripElement.ShrinkMode = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.AllowDragDrop" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("AllowDragDrop", typeof(RadTabStripElement))]
		[RadDefaultValue("AllowDragDrop", typeof(RadTabStripElement))]
		public bool AllowDragDrop
		{
			get
			{
				return this.tabStripElement.AllowDragDrop;
			}
			set
			{
				this.tabStripElement.AllowDragDrop = value;
			}
		}

        /// <summary>Turns on and off the overflow button.</summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ShowOverFlowButton", typeof(RadTabStripElement))]
		[RadDefaultValue("ShowOverFlowButton", typeof(RadTabStripElement))]
		public bool ShowOverFlowButton
		{
			get
			{
				return tabStripElement.ShowOverFlowButton;
			}
			set
			{
				tabStripElement.ShowOverFlowButton = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.SelectedTab" filter=""/>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadElement SelectedTab
		{
			get
			{
				return this.tabStripElement.SelectedTab;
			}
			set
			{
				this.tabStripElement.SelectedTab = value;
			}
		}

		/// <summary>
		///		Defines whether the control will appear in TabControlMode (it's allowed other controls to be added in the base area and associated with the relevant tab item)
		///		or whether the control will consist only of tab items and the relevant elements (scroll buttons, overflow button, etc).
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
		[Description("Defines whether the control will appear in TabControlMode (it's allowed other controls to be added in the base area and associated with the relevant tab item) or whether the control will consist only of tab items and the relevant elements (scroll buttons, overflow button, etc).")]
		public bool EnableTabControlMode
		{
			get
			{
				return this.enableTabControlMode;
			}
			set
			{
                if (this.enableTabControlMode == value)
                    return;
				this.enableTabControlMode = value;
				if (value)
				{
                    foreach (RadItem item in this.Items)
                    {
                        if (!(item is TabItem))
                        {
                            continue;
                        }

                        TabItem tabItem = item as TabItem;

                        if (tabItem.ContentPanel == null)
                            continue;

                        tabItem.ContentPanel.SetAssociatedItem(item);

                        if (tabItem.IsSelected)
                            tabItem.ContentPanelHost.Visibility = ElementVisibility.Visible;
                        else
                            tabItem.ContentPanelHost.Visibility = ElementVisibility.Hidden;
                    }
				}
				else
				{
					foreach (RadElement child in this.TabStripElement.TabContent.Children)
					{
						if (child is RadHostItem)
						{
							child.Visibility = ElementVisibility.Collapsed;
						}
					}
				}
			
				this.OnEnableTabControlModeChanged(EventArgs.Empty);

				this.TabStripElement.StretchBaseMode = this.enableTabControlMode ? TabBaseStretchMode.StretchToRemainingSpace : TabBaseStretchMode.Default;
			}
		}

		/// <summary>
		/// Fires when EnableTabControlMode changes
		/// </summary>
		public event EventHandler EnableTabControlModeChanged;


		/// <summary>
		/// Fires the EnableTabControlModeChange event
		/// </summary>
		protected virtual void OnEnableTabControlModeChanged(EventArgs e)
		{
			if (this.EnableTabControlModeChanged != null)
			{
				this.EnableTabControlModeChanged(this, e);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.TabsPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabsPosition", typeof(RadTabStripElement))]
		[RadDefaultValue("TabsPosition", typeof(RadTabStripElement))]
		public TabPositions TabsPosition
		{
			get
			{
				return this.tabStripElement.TabsPosition;
			}
			set
			{
                this.tabStripElement.TabsPosition = value;
            }
		}

        /// <commentsfrom cref="Telerik.WinControls.RadItem.TextOrientation" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TextOrientation", typeof(RadTabStripElement))]
		[RadDefaultValue("TextOrientation", typeof(RadTabStripElement))]
		public Orientation TextOrientation
		{
			get
			{
				return this.tabStripElement.TextOrientation;
			}
			set
			{
				this.tabStripElement.TextOrientation = value;
			}
		}



		/// <commentsfrom cref="RadTabStripElement.TabScrollStyle" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabScrollStyle", typeof(RadTabStripElement))]
		[RadDefaultValue("TabScrollStyle", typeof(RadTabStripElement))]
		public TabStripScrollStyle TabScrollStyle
		{
			get
			{
				return this.tabStripElement.TabScrollStyle;
			}
			set
			{
				this.tabStripElement.TabScrollStyle = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.ScrollOffsetStep" filter=""/>
		[DefaultValue(8), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ScrollOffsetStep", typeof(RadTabStripElement))]
		[RadDefaultValue("ScrollOffsetStep", typeof(RadTabStripElement))]
		public int ScrollOffsetStep
		{
			get
			{
				return this.tabStripElement.ScrollOffsetStep;
			}
			set
			{
				this.tabStripElement.ScrollOffsetStep = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.TabDropDownButtonPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabDropDownButtonPosition", typeof(RadTabStripElement))]
		[RadDefaultValue("TabDropDownButtonPosition", typeof(RadTabStripElement))]
		public TabItemsDropDownButtonPosition TabDropDownButtonPosition
		{
			get
			{
				return this.tabStripElement.TabDropDownButtonPosition;
			}
			set
			{
				this.tabStripElement.TabDropDownButtonPosition = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.TabScrollButtonsPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabScrollButtonsPosition", typeof(RadTabStripElement))]
		[RadDefaultValue("TabScrollButtonsPosition", typeof(RadTabStripElement))]
		public TabScrollButtonsPosition TabScrollButtonsPosition
		{
			get
			{
				return this.tabStripElement.TabScrollButtonsPosition;
			}
			
			set
			{
				this.tabStripElement.TabScrollButtonsPosition = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.TabScrollButtonsPosition" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("AllTabsEqualHeight", typeof(RadTabStripElement))]
		[RadDefaultValue("AllTabsEqualHeight", typeof(RadTabStripElement))]
		public bool AllTabsEqualHeight
		{
			get
			{
				return this.tabStripElement.AllTabsEqualHeight;
			}
			set
			{
				this.tabStripElement.AllTabsEqualHeight = value;
			}
		}

		/// <commentsfrom cref="RadTabStripElement.ItemsOffset" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("ItemsOffset", typeof(RadTabStripElement))]
		[RadDefaultValue("ItemsOffset", typeof(RadTabStripElement))]
		public int ItemsOffset
		{
			get
			{
				return this.tabStripElement.ItemsOffset;
			}
			set
			{
				this.tabStripElement.ItemsOffset = value;
			}
		}

		/// <summary>Gets or set a value indicating whether autosize is turned on.</summary>
		[DefaultValue(false)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}


		/// <commentsfrom cref="RadTabStripElement.TabSelecting" filter=""/>
		[Category(RadDesignCategory.ActionCategory)]
		[RadDescription("TabSelecting", typeof(RadTabStripElement))]
		public event TabCancelEventHandler TabSelecting;

		/// <commentsfrom cref="RadTabStripElement.TabSelected" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("TabSelected", typeof(RadTabStripElement))]
		public event TabEventHandler TabSelected;

		/// <commentsfrom cref="RadTabStripElement.TabHovered" filter=""/>
		[Category(RadDesignCategory.MouseCategory)]
		[RadDescription("TabHovered", typeof(RadTabStripElement))]
		public event TabEventHandler TabHovered;

		/// <commentsfrom cref="RadTabStripElement.TabDragStarting" filter=""/>
		[Category(RadDesignCategory.DragDropCategory)]
		[RadDescription("TabDragStarting", typeof(RadTabStripElement))]
		public event TabDragCancelEventHandler TabDragStarting;
		/// <commentsfrom cref="RadTabStripElement.TabDragStarted" filter=""/>
		[Category(RadDesignCategory.DragDropCategory)]
		[RadDescription("TabDragStarted", typeof(RadTabStripElement))]
		public event TabDragEventHandler TabDragStarted;

		/// <commentsfrom cref="RadTabStripElement.TabDragEnding" filter=""/>
		[Category(RadDesignCategory.DragDropCategory)]
		[RadDescription("TabDragEnding", typeof(RadTabStripElement))]
		public event TabDragCancelEventHandler TabDragEnding;
		/// <commentsfrom cref="RadTabStripElement.TabDragEnded" filter=""/>
		[Category(RadDesignCategory.DragDropCategory)]
		[RadDescription("TabDragEnded", typeof(RadTabStripElement))]
		public event TabDragEventHandler TabDragEnded;

		/// <commentsfrom cref="RadTabStripElement.TabsPositionChanged" filter=""/>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("TabsPositionChanged", typeof(RadTabStripElement))]
		public event EventHandler TabsPositionChanged;

		/// <commentsfrom cref="RadTabStripElement.AllTabsEqualHeightChanged" filter=""/>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("AllTabsEqualHeightChanged", typeof(RadTabStripElement))]
		public event EventHandler AllTabsEqualHeightChanged;

		/// <commentsfrom cref="RadItem.TextOrientationChanged" filter=""/>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		public event EventHandler TextOrientationChanged;

        /// <summary>Initializes a new instance of the RadTabStrip class.</summary>
		public RadTabStrip()
            : this(true)
		{
            base.TabStop = true;
            base.SetStyle(ControlStyles.Selectable, true);
		}

		public RadTabStrip(bool useNewLayout)
		{
			this.UseNewLayoutSystem = useNewLayout;
            if (!useNewLayout)
            {
                base.AutoSize = false;
            }
			this.AllowDrop = false;
			this.TabStop = true;
			this.EnableTabControlMode = true;
		}

        /// <summary>Retrieves the selected item in the tab strip.</summary>
		public override RadItem GetSelectedItem()
		{
			return (RadItem) this.tabStripElement.SelectedTab;
		}


		protected override bool ProcessCmdKey(ref Message m, Keys keyData)
		{

			if (keyData == Keys.F2)
			{	
				this.TabStripElement.BeginEdit();	
			}

			if (keyData == Keys.Escape)
			{
				this.TabStripElement.EndEdit(true);
			}


		
			return base.ProcessCmdKey(ref m, keyData);

		}


        protected override void OnItemSelected(ItemSelectedEventArgs args)
        {
            base.OnItemSelected(args);
            RadItem item = args.Item;
            if (!this.UseNewLayoutSystem)
            {
                int lastSelectedTabIndex = this.tabStripElement.Items.IndexOf((RadItem)this.tabStripElement.SelectedTab);
                int index = this.tabStripElement.Items.IndexOf(item);
                this.tabStripElement.SetSelectedTab(item);

                if (!this.ShrinkMode)
                    this.tabStripElement.scrollingManager.PerformScroll(item as TabItem, lastSelectedTabIndex, index);
            }
            else
            {

                int lastSelectedTabIndex = this.tabStripElement.BoxLayout.Children.IndexOf((RadItem)this.tabStripElement.SelectedTab);
                int index = this.tabStripElement.Items.IndexOf(item);
                this.tabStripElement.SetSelectedTab(item);

                if (!this.ShrinkMode)
                    this.tabStripElement.scrollingManager.PerformScroll(item as TabItem, lastSelectedTabIndex, index);
            }
        }

        private void tabStripElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadTabStripElement.BackColorProperty)
			{
				this.RootElement.BackColor = this.tabStripElement.BackColor;
			}
		}

		/// <commentsfrom cref="RadItem.OnTextOrientationChanged" filter=""/>
		protected virtual void OnTextOrientationChanged(EventArgs args)
		{
			if (this.TextOrientationChanged != null)
			{
				this.TextOrientationChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnAllTabsEqualHeightChanged" filter=""/>
		protected virtual void OnAllTabsEqualHeightChanged(EventArgs args)
		{
			if (this.AllTabsEqualHeightChanged != null)
			{
				this.AllTabsEqualHeightChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabsPositionChanged" filter=""/>
		protected virtual void OnTabsPositionChanged(EventArgs args)
		{
			if (this.TabsPositionChanged != null)
			{
				this.TabsPositionChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabSelected" filter=""/>
		protected virtual void OnTabSelected(TabEventArgs args)
		{
			if (this.TabSelected != null)
			{
				this.TabSelected(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabHovered" filter=""/>
		protected virtual void OnTabHovered(TabEventArgs args)
		{
			if (this.TabHovered != null)
			{
				this.TabHovered(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabDragStarting" filter=""/>
		protected virtual void OnTabDragStarting(TabDragCancelEventArgs args)
		{
			if (this.TabDragStarting != null)
			{
				this.TabDragStarting(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabDragStarted" filter=""/>
		protected virtual void OnTabDragStarted(TabDragEventArgs args)
		{
			if (this.TabDragStarted != null)
			{
				this.TabDragStarted(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabDragEnding" filter=""/>
		protected virtual void OnTabDragEnding(TabDragCancelEventArgs args)
		{
			if (this.TabDragEnding != null)
			{
				this.TabDragEnding(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabSelecting" filter=""/>
		protected virtual void OnTabSelecting(TabCancelEventArgs args)
		{
			if (this.TabSelecting != null)
			{
				this.TabSelecting(this, args);
			}
		}

		/// <commentsfrom cref="RadTabStripElement.OnTabDragEnded" filter=""/>
		protected virtual void OnTabDragEnded(TabDragEventArgs args)
		{
			if (this.TabDragEnded != null)
			{
				this.TabDragEnded(this, args);
			}
		}

        private void tabStripElement_TabSelected(object sender, TabEventArgs args)
		{
			if (this.enableTabControlMode)
			{
				foreach (RadItem item in this.Items)
				{
					TabItem tabItem = item as TabItem;
					if (tabItem == null)
						continue;

					if (tabItem == args.TabItem)
					{
						tabItem.ContentPanelHost.Visibility = ElementVisibility.Visible;
					}
					else
					{
						tabItem.ContentPanelHost.Visibility = ElementVisibility.Hidden;
					}
				}
			}
		}

		[DefaultValue(false)]
		internal bool RTL
		{
			get
			{
				return
					 this.rightToLeft;
			}

			set
			{
				if (this.rightToLeft != value)
				{
                    if (this.TabStripElement != null)
                    {
                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            this.TabStripElement.RightToLeft = true;
                        }
                        else
                        {
                            this.TabStripElement.RightToLeft = false;
                        }
                    }
				

					this.rightToLeft = value;
				}
			}
		}

        private bool rightToLeft = false;

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            System.Windows.Forms.RightToLeft value = base.RightToLeft;
            if (value == RightToLeft.Inherit)
            {
                Control rtlSource = this.Parent;
                while (rtlSource != null && rtlSource.RightToLeft == RightToLeft.Inherit)
                {
                    rtlSource = rtlSource.Parent;
                }
                value = (rtlSource != null) ? rtlSource.RightToLeft : RightToLeft.No;

                if (value == RightToLeft.Yes)
                    RTL = true;
                else
                    RTL = false;
            }
            if (value == RightToLeft.No)
            {
                RTL = false;
            }
            else if (value == RightToLeft.Yes)
            {
                RTL = true;
            }

        }

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			if (this.enableTabControlMode)
			{
                TabItem tabItem = target as TabItem;
                switch (operation)
                {
                    case ItemsChangeOperation.Inserted:
                        if (tabItem != null && tabItem.ContentPanel != null)
                        {
                            tabItem.ContentPanel.SetAssociatedItem(tabItem);
                            this.TabStripElement.TabContent.Children.Add(tabItem.ContentPanelHost);
                            if (tabItem.IsSelected)
                                tabItem.ContentPanelHost.Visibility = ElementVisibility.Visible;
                            else
                                tabItem.ContentPanelHost.Visibility = ElementVisibility.Hidden;
                        }
                        break;
                    case ItemsChangeOperation.Removing:
                        if (tabItem != null && tabItem.ContentPanelHost != null &&
                            this.TabStripElement.TabContent.Children.Contains(tabItem.ContentPanelHost))
                        {
                            this.TabStripElement.TabContent.Children.Remove(tabItem.ContentPanelHost);
                        }
                        break;
                    case ItemsChangeOperation.Clearing:
                        while (this.TabStripElement.TabContent.Children.Count > 1)
                        {
                            RadElement child = this.TabStripElement.TabContent.Children[1];
                            if (child is RadHostItem)
                                this.TabStripElement.TabContent.Children.Remove(child);
                        }
                        break;
                    case ItemsChangeOperation.Set:
                        if (tabItem != null && tabItem.ContentPanel != null)
                        {
                            tabItem.ContentPanel.SetAssociatedItem(tabItem);
                            if (!this.TabStripElement.TabContent.Children.Contains(tabItem.ContentPanelHost))
                                this.TabStripElement.TabContent.Children.Add(tabItem.ContentPanelHost);
                            if (tabItem.IsSelected)
                                tabItem.ContentPanelHost.Visibility = ElementVisibility.Visible;
                            else
                                tabItem.ContentPanelHost.Visibility = ElementVisibility.Hidden;
                        }
                        break;
                }
			}
		}

	

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (element == this.tabStripElement.OverflowManager.DropDownButton)
                return true;

            if (elementType == typeof(RadTabStripElement))
                return true;

            return false;
        }


        protected override void CreateChildItems(RadElement parent)
        {
            this.tabStripElement = new RadTabStripElement();
            this.RootElement.Children.Add(this.tabStripElement);

            this.tabStripElement.Items.ItemsChanged += new ItemChangedDelegate(this.Items_ItemsChanged);
            this.tabStripElement.TabSelected += new TabEventHandler(this.tabStripElement_TabSelected);

            this.tabStripElement.TabSelecting += delegate(object sender, TabCancelEventArgs args) { this.OnTabSelecting(args); };
            this.tabStripElement.TabDragEnded += delegate(object sender, TabDragEventArgs args) { this.OnTabDragEnded(args); };
            this.tabStripElement.TabDragEnding += delegate(object sender, TabDragCancelEventArgs args) { this.OnTabDragEnding(args); };
            this.tabStripElement.TabDragStarted += delegate(object sender, TabDragEventArgs args) { this.OnTabDragStarted(args); };
            this.tabStripElement.TabDragStarting += delegate(object sender, TabDragCancelEventArgs args) { this.OnTabDragStarting(args); };

            this.tabStripElement.TabSelected += delegate(object sender, TabEventArgs args) { this.OnTabSelected(args); };
            this.tabStripElement.TabHovered += delegate(object sender, TabEventArgs args) { this.OnTabHovered(args); };

            this.tabStripElement.TabsPositionChanged += delegate(object sender, EventArgs args) { this.OnTabsPositionChanged(args); };
            this.tabStripElement.AllTabsEqualHeightChanged += delegate(object sender, EventArgs args) { this.OnAllTabsEqualHeightChanged(args); };
            this.tabStripElement.TextOrientationChanged += delegate(object sender, EventArgs args) { this.OnTextOrientationChanged(args); };

            this.tabStripElement.RadPropertyChanged += new RadPropertyChangedEventHandler(this.tabStripElement_RadPropertyChanged);

            this.ElementTree.PerformInnerLayout(true, 0, 0, this.DefaultSize.Width, this.DefaultSize.Height);
        }
	}
}
