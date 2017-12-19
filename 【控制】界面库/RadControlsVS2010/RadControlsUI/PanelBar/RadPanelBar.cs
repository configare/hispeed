using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI;
using Telerik.WinControls.Design;
namespace Telerik.WinControls.UI
{
	[RadThemeDesignerData(typeof(RadPanelBarDesignerData))]
	[ToolboxItem(false)]
    [Designer(DesignerConsts.RadPanelBarDesignerString)]
	[Description("Builds collapsible side-menu systems and Outlook-type panels")]
	[DefaultProperty("Items")]
    [Docking(DockingBehavior.Ask)]
    [Obsolete("This control is obsolete. Use RadPageView instead.")]
	public class RadPanelBar : RadControl
	{
        private bool canScroll = true;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("CaptionChanged", typeof(RadPanelBarElement))]
		public event EventHandler CaptionChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("SpacingBetweenItemsChanged", typeof(RadPanelBarElement))]
		public event EventHandler SpacingBetweenItemsChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("PanelBarStyleChanged", typeof(RadPanelBarElement))]
		public event EventHandler PanelBarStyleChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("TopOffsetChanged", typeof(RadPanelBarElement))]
		public event EventHandler TopOffsetChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("BottomOffsetChanged", typeof(RadPanelBarElement))]
		public event EventHandler BottomOffsetChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("RightOffsetChanged", typeof(RadPanelBarElement))]
		public event EventHandler RightOffsetChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("LeftOffsetChanged", typeof(RadPanelBarElement))]
		public event EventHandler LeftOffsetChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("SpacingBetweenGroupsChanged", typeof(RadPanelBarElement))]
		public event EventHandler SpacingBetweenGroupsChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("SpacingBetweenColumnsChanged", typeof(RadPanelBarElement))]
		public event EventHandler SpacingBetweenColumnsChanged;

		[Category(RadDesignCategory.PropertyChangedCategory)]
		[RadDescription("NumberOfColumnsChanged", typeof(RadPanelBarElement))]
		public event EventHandler NumberOfColumnsChanged;

		/// <summary>
		/// Occurs when a group has been expanded
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been expanded")]
		public event PanelBarGroupEventHandler PanelBarGroupExpanded;

		/// <summary>
		/// Occurs when a group is going to be expanded
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be expanded")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupExpanding;

		/// <summary>
		/// Occurs when a group has been selected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been selected")]
		public event PanelBarGroupEventHandler PanelBarGroupSelected;

		/// <summary>
		/// Occurs when a group is going to be selected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be selected")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupSelecting;


		/// <summary>
		/// Occurs when a group has been collapsed
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been collapsed")]
		public event PanelBarGroupEventHandler PanelBarGroupCollapsed;

		/// <summary>
		/// Occurs when a group is going to be collapsed
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be collapsed")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupCollapsing;

		/// <summary>
		/// Occurs when a group has been unselected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been unselected")]
		public event PanelBarGroupEventHandler PanelBarGroupUnSelected;

		/// <summary>
		/// Occurs when a group is going to be unselected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be unselected")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupUnSelecting;
		static RadPanelBar()
		{
		}

        private RadPanelBarElement panelBarElement;

        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 150);
            }
        }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(true), Category(RadDesignCategory.DataCategory)] 
		[Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
		[RadEditItemsAction]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.panelBarElement.Items;
			}
		}

		/// <summary>
		/// Gets or sets caption's height.
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets caption's height.")]
		[DefaultValue(25)]
		[Obsolete("This property will be removed in the next version")]
		public int CaptionHeight
		{
			get
			{
				return this.panelBarElement.CaptionHeight;
			}
			set
			{
				this.panelBarElement.CaptionHeight = value;
			}
		}

		/// <summary>
		/// Gets or sets whether groups should show images in their caption
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets whether groups should show images in their caption.")]
		[DefaultValue(true)]
		[Obsolete("This property will be removed in the next version")]
		public bool ShowCaptionImages
		{
			get
			{
				return this.panelBarElement.ShowCaptionImages;
			}
			set
			{
				this.panelBarElement.ShowCaptionImages = value;
			}
		}

		/// <summary>
		/// Gets or sets the font of the OutLookStyle's caption.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the font of the OutLookStyle's caption.")]
		[DefaultValue("")]
		public Font CaptionTextFont
		{
			get
			{
				return this.panelBarElement.CaptionTextFont;
			}
			set
			{
				this.panelBarElement.CaptionTextFont = value;
			}
		}

		/// <summary>
		/// Gets or sets the Caption
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("Caption", typeof(RadPanelBarElement))]
		[RadDefaultValue("Default Caption", typeof(RadPanelBarElement))]
		[Localizable(true)]
		public string Caption
		{
			get
			{
				return this.panelBarElement.Caption;
			}
			set
			{
				this.panelBarElement.Caption = value;
			}
		}

		/// <summary>
		/// Gets or sets the right offset of the caption button which is part of the caption of every group
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("CaptionButtonRightOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("CaptionButtonRightOffset", typeof(RadPanelBarElement))]
		[Obsolete("This property will be removed in the next version")]
		public int CaptionButtonRightOffset
		{
			get
			{
				return this.panelBarElement.CaptionButtonRightOffset;
			}
			set
			{
				this.panelBarElement.CaptionButtonRightOffset = value;
			}
		}


		/// <summary>
		/// Gets or sets the Spacing between caption elements
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("SpacingBetweenItems", typeof(RadPanelBarElement))]
		[RadDefaultValue("SpacingBetweenItems", typeof(RadPanelBarElement))]
		[Obsolete("This property will be removed in the next version")]
		public int SpacingBetweenItems
		{
			get
			{
				return this.panelBarElement.SpacingBetweenItems;
			}
			set
			{
				this.panelBarElement.SpacingBetweenItems = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial offset of caption elements
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("InitialOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("InitialOffset", typeof(RadPanelBarElement))]
		public int InitialOffset
		{
			get
			{
				return this.panelBarElement.InitialOffset;
			}
			set
			{
				this.panelBarElement.InitialOffset = value;
			}
		}


		/// <summary>
		/// Gets or sets the RightOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("RightOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("RightOffset", typeof(RadPanelBarElement))]
		public int RightOffset
		{
			get
			{
				return this.panelBarElement.RightOffset;
			}
			set
			{
				this.panelBarElement.RightOffset = value;
			}
		}

		/// <summary>
		/// Gets or sets the BottomOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("BottomOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("BottomOffset", typeof(RadPanelBarElement))]
		public int BottomOffset
		{
			get
			{
				return this.panelBarElement.BottomOffset;
			}
			set
			{
				this.panelBarElement.BottomOffset = value;
			}
		}

		/// <summary>
		/// Gets or sets the TopOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("TopOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("TopOffset", typeof(RadPanelBarElement))]
		public int TopOffset
		{
			get
			{
				return this.panelBarElement.TopOffset;
			}
			set
			{
				this.panelBarElement.TopOffset = value;
			}
		}

		/// <summary>
		/// Gets or sets the LeftOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("LeftOffset", typeof(RadPanelBarElement))]
		[RadDefaultValue("LeftOffset", typeof(RadPanelBarElement))]
		public int LeftOffset
		{
			get
			{
				return this.panelBarElement.LeftOffset;
			}
			set
			{
				this.panelBarElement.LeftOffset = value;
			}
		}


		/// <summary>
		/// Gets or sets the number of columns
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("NumberOfColumns", typeof(RadPanelBarElement))]
		[RadDefaultValue("NumberOfColumns", typeof(RadPanelBarElement))]
		public int NumberOfColumns
		{
			get
			{
				return this.panelBarElement.NumberOfColumns;
			}
			set
			{
				this.panelBarElement.NumberOfColumns = value;
			}
		}

		/// <summary>
		/// Gets or sets the spacing between columns
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("SpacingBetweenColumns", typeof(RadPanelBarElement))]
		[RadDefaultValue("SpacingBetweenColumns", typeof(RadPanelBarElement))]
		public int SpacingBetweenColumns
		{
			get
			{
				return this.panelBarElement.SpacingBetweenColumns;
			}
			set
			{
				this.panelBarElement.SpacingBetweenColumns = value;
			}
		}

		/// <summary>
		/// Gets or sets the spacing between groups
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[RadDescription("SpacingBetweenGroups", typeof(RadPanelBarElement))]
		[RadDefaultValue("SpacingBetweenGroups", typeof(RadPanelBarElement))]
		public int SpacingBetweenGroups
		{
			get
			{
				return this.panelBarElement.SpacingBetweenGroups;
			}
			set
			{
				this.panelBarElement.SpacingBetweenGroups = value;
			}
		}

		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("PanelBarStyle", typeof(RadPanelBarElement))]
		[RadDefaultValue("PanelBarStyle", typeof(RadPanelBarElement))]
		public PanelBarStyles GroupStyle
		{
			get
			{
				return this.panelBarElement.PanelBarStyle;
			}
			set
			{
				this.panelBarElement.PanelBarStyle = value;
			}
		}


		public RadPanelBar()
		{
            this.vScrollBar = new RadVScrollBar();

            this.Controls.Add(this.vScrollBar);

            this.vScrollBar.Visible = false;
            this.vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);

			this.SizeChanged += new EventHandler(RadPanelBar_SizeChanged);

		
			this.CausesValidation = false;
		}

   
        /// <summary>
        /// Gets or sets a value whether the panel bar can scroll
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Gets or sets a value whether the panel bar can scroll")]
        [Browsable(true)]
        public bool CanScroll
        {
            get
            {
                return this.canScroll;
            }
            set
            {
                this.canScroll = value;
                if (!value)
                {
                    this.vScrollBar.Visible = false;
                }
                else
                {
                    this.vScrollBar.Visible = true;
                }

                if (this.PanelBarElement != null)
                {
                    this.PanelBarElement.MaxSize = Size.Empty;
                    this.PanelBarElement.InvalidateMeasure();
                    this.PanelBarElement.InvalidateArrange();
                    this.PanelBarElement.UpdateLayout();

                    this.PanelBarElement.Children[0].InvalidateMeasure();
                    this.PanelBarElement.Children[0].InvalidateArrange();
                    this.PanelBarElement.Children[0].UpdateLayout();
                }
            }
        }

        [Browsable(false)]
        public new bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }
		[DefaultValue(false), Browsable(false)]
		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
			}
		}

        private void RadPanelBar_SizeChanged(object sender, EventArgs e)
		{
            if (this.panelBarElement == null)
                return;

            if (this.panelBarElement.CurrentStyle.GetBaseLayout() != null)
            {
                this.panelBarElement.CurrentStyle.GetBaseLayout().InvalidateMeasure();
                this.panelBarElement.CurrentStyle.GetBaseLayout().UpdateLayout();
            }         
		}

		/// <commentsfrom cref="RadPanelBarElement.OnTopOffsetChanged" filter=""/>
		protected virtual void OnTopOffsetChanged(EventArgs args)
		{
			if (this.TopOffsetChanged != null)
			{
				this.TopOffsetChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnBottomOffsetChanged" filter=""/>
		protected virtual void OnBottomOffsetChanged(EventArgs args)
		{
			if (this.BottomOffsetChanged != null)
			{
				this.BottomOffsetChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnRightOffsetChanged" filter=""/>
		protected virtual void OnRightOffsetChanged(EventArgs args)
		{
			if (this.RightOffsetChanged != null)
			{
				this.RightOffsetChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnLeftOffsetChanged" filter=""/>
		protected virtual void OnLeftOffsetChanged(EventArgs args)
		{
			if (this.LeftOffsetChanged != null)
			{
				this.LeftOffsetChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnSpacingBetweenColumnsChanged" filter=""/>
		protected virtual void OnSpacingBetweenColumnsChanged(EventArgs args)
		{
			if (this.SpacingBetweenColumnsChanged != null)
			{
				this.SpacingBetweenColumnsChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnSpacingBetweenGroupsChanged" filter=""/>
		protected virtual void OnSpacingBetweenGroupsChanged(EventArgs args)
		{
			if (this.SpacingBetweenGroupsChanged != null)
			{
				this.SpacingBetweenGroupsChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnNumberOfColumnsChanged" filter=""/>
		protected virtual void OnNumberOfColumnsChanged(EventArgs args)
		{
			if (this.NumberOfColumnsChanged != null)
			{
				this.NumberOfColumnsChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnCaptionChanged" filter=""/>
		protected virtual void OnCaptionChanged(EventArgs args)
		{
			if (this.CaptionChanged != null)
			{
				this.CaptionChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnSpacingBetweenItemsChanged" filter=""/>
		protected virtual void OnSpacingBetweenItemsChanged(EventArgs args)
		{
			if (this.SpacingBetweenItemsChanged != null)
			{
				this.SpacingBetweenItemsChanged(this, args);
			}
		}

		/// <commentsfrom cref="RadPanelBarElement.OnPanelBarStyleChanged" filter=""/>
		protected virtual void OnPanelBarStyleChanged(EventArgs args)
		{
			if (this.PanelBarStyleChanged != null)
			{
				this.PanelBarStyleChanged(this, args);
			}
		}

		protected virtual void OnPanelBarGroupSelected(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupSelected != null)
			{
				this.PanelBarGroupSelected(this, args);
			}
		}

		protected virtual void OnPanelBarGroupExpanded(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupExpanded != null)
			{
				this.PanelBarGroupExpanded(this, args);
			}
		}

		
		protected virtual void OnPanelBarGroupUnSelected(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupUnSelected != null)
			{
				this.PanelBarGroupUnSelected(this, args);
			}
		}

		protected virtual void OnPanelBarGroupCollapsed(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupCollapsed != null)
			{
				this.PanelBarGroupCollapsed(this, args);
			}
		}

		protected virtual void OnPanelBarGroupUnSelecting(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupUnSelecting != null)
			{
				this.PanelBarGroupUnSelecting(this, args);
			}
		}

		protected virtual void OnPanelBarGroupCollapsing(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupCollapsing != null)
			{
				this.PanelBarGroupCollapsing(this, args);
			}
		}

		protected virtual void OnPanelBarGroupExpanding(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupExpanding != null)
			{
				this.PanelBarGroupExpanding(this, args);
			}
		}

		protected virtual void OnPanelBarGroupSelecting(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupSelecting != null)
			{
				this.PanelBarGroupSelecting(this, args);
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			if (!this.vScrollBar.Visible)
				return;

			if (e.Delta < 0)
			{
				if (this.vScrollBar.Value + 20 <= this.vScrollBar.Maximum)
				{
					this.vScrollBar.Value += 20;
				}
				else
				{
					this.vScrollBar.Value = this.vScrollBar.Maximum; 
				}
			}
			else
				if (this.vScrollBar.Value >= 20)
				{
					this.vScrollBar.Value -= 20;
				}
				else
				{
					this.vScrollBar.Value = 0;
				}
		}

		internal RadVScrollBar vScrollBar;

        private void RadPanelBar_ThemeNameChanged(object source, ThemeNameChangedEventArgs args)
		{
            if (this.PanelBarElement != null)
            {
                if (this.PanelBarElement.CurrentStyle != null)
                {
                    this.PanelBarElement.CurrentStyle.UpdateGroupsUI();
                }
            }
		}


        /// <summary>
        /// Gets the instance of RadPanelBarElement wrapped by this control. RadPanelBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadPanelBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadPanelBarElement PanelBarElement
		{
			get
			{
				return this.panelBarElement;
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if ( this.PanelBarElement.Children.Count > 0 )
			{
				this.PanelBarElement.Children[0].PositionOffset = new SizeF( 
					this.PanelBarElement.Children[0].PositionOffset.Width,
					-this.vScrollBar.Value);

                foreach (RadPanelBarGroupElement group in this.PanelBarElement.Items)
				{
					if (group.EnableHostControlMode)
					{
						Point location = group.ContentPanelHost.LocationToControl();
						group.ContentPanel.Location = location;
					}
				}

				this.Refresh();
			}
		}


        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);

            rootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadScrollViewer))
                return true;

            return false;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.panelBarElement = new RadPanelBarElement();
            parent.Children.Add(this.panelBarElement);
            this.panelBarElement.CaptionChanged += delegate(object sender, EventArgs args) { OnCaptionChanged(args); };
            this.panelBarElement.BottomOffsetChanged += delegate(object sender, EventArgs args) { OnBottomOffsetChanged(args); };
            this.panelBarElement.TopOffsetChanged += delegate(object sender, EventArgs args) { OnTopOffsetChanged(args); };
            this.panelBarElement.LeftOffsetChanged += delegate(object sender, EventArgs args) { OnLeftOffsetChanged(args); };
            this.panelBarElement.RightOffsetChanged += delegate(object sender, EventArgs args) { OnRightOffsetChanged(args); };
            this.panelBarElement.SpacingBetweenColumnsChanged += delegate(object sender, EventArgs args) { OnSpacingBetweenColumnsChanged(args); };
            this.panelBarElement.SpacingBetweenItemsChanged += delegate(object sender, EventArgs args) { OnSpacingBetweenItemsChanged(args); };
            this.panelBarElement.SpacingBetweenGroupsChanged += delegate(object sender, EventArgs args) { OnSpacingBetweenGroupsChanged(args); };
            this.panelBarElement.NumberOfColumnsChanged += delegate(object sender, EventArgs args) { OnNumberOfColumnsChanged(args); };
            this.panelBarElement.PanelBarStyleChanged += delegate(object sender, EventArgs args) { OnPanelBarStyleChanged(args); };

            this.panelBarElement.PanelBarGroupCollapsed += delegate(object sender, PanelBarGroupEventArgs args) { OnPanelBarGroupCollapsed(args); };
            this.panelBarElement.PanelBarGroupExpanded += delegate(object sender, PanelBarGroupEventArgs args) { OnPanelBarGroupExpanded(args); };
            this.panelBarElement.PanelBarGroupExpanding += delegate(object sender, PanelBarGroupCancelEventArgs args) { OnPanelBarGroupExpanding(args); };
            this.panelBarElement.PanelBarGroupCollapsing += delegate(object sender, PanelBarGroupCancelEventArgs args) { OnPanelBarGroupCollapsing(args); };
            this.panelBarElement.PanelBarGroupUnSelected += delegate(object sender, PanelBarGroupEventArgs args) { OnPanelBarGroupUnSelected(args); };
            this.panelBarElement.PanelBarGroupSelected += delegate(object sender, PanelBarGroupEventArgs args) { OnPanelBarGroupSelected(args); };
            this.panelBarElement.PanelBarGroupSelecting += delegate(object sender, PanelBarGroupCancelEventArgs args) { OnPanelBarGroupSelecting(args); };
            this.panelBarElement.PanelBarGroupUnSelecting += delegate(object sender, PanelBarGroupCancelEventArgs args) { OnPanelBarGroupUnSelecting(args); };

            this.ThemeNameChanged += new ThemeNameChangedEventHandler(RadPanelBar_ThemeNameChanged);

            this.ElementTree.PerformInnerLayout(true, 0, 0, this.DefaultSize.Width, this.DefaultSize.Height);
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            this.vScrollBar.ThemeName = this.ThemeName;
        }
	}
}
