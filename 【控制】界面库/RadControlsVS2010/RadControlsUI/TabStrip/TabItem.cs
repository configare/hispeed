using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public enum EditMode
    {
        Name,
        Tag,
        Value
    }

    /// <summary>Represents a tab item.</summary>
	[ToolboxItem(false), ComVisible(false), RadItemEditText]
    [Designer(DesignerConsts.RadTabItemDesignerString)]
	public class TabItem : RadButtonItem
	{
        private TextPrimitive textPrimitive;
        private FillPrimitive fill;
        private BorderPrimitive tabBorder;
        private ImagePrimitive image;
        private RadTabStripElement parentTabStrip;
     	private RadHostItem contentPanelHost;
		internal EditMode editMode = EditMode.Name;
        private SizeF textPreferredSize;

        /// <summary>Fires when the visibility is changed.</summary>
		public event EventHandler VisibilityChanged;

        /// <summary>Initializes a new instance of the TabItem class.</summary>
        public TabItem()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.Class = "TabItem";
            this.ThemeRole = "TabItem";
            this.BitState[CaptureOnMouseDownStateKey] = false;
            this.CanFocus = true;
        }

        static TabItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TabItemStateManagerFactory(), typeof(TabItem));

            TextAlignmentProperty.OverrideMetadata(typeof(TabItem), new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, 
                                                                    ElementPropertyOptions.AffectsLayout | 
                                                                    ElementPropertyOptions.InvalidatesLayout));
            TextImageRelationProperty.OverrideMetadata(typeof(TabItem), new RadElementPropertyMetadata(TextImageRelation.ImageBeforeText,
                                                                    ElementPropertyOptions.AffectsLayout | 
                                                                    ElementPropertyOptions.InvalidatesLayout | 
                                                                    ElementPropertyOptions.AffectsMeasure));
        }

        
        /// <summary>Initializes a new instance of TabItem class using tab text.</summary>
		public TabItem(string text)
		{
            this.Text = text;
        }

        /// <summary>
        /// Gets or sets the alignment of text content on the drawing surface. 
        /// </summary>
        [RadPropertyDefaultValue("TextAlignment", typeof(TabItem))]
        public override System.Drawing.ContentAlignment TextAlignment
        {
            get
            {
                return base.TextAlignment;
            }
            set
            {
                base.TextAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of text and image relative to each other. 
        /// </summary>
        [RadPropertyDefaultValue("TextImageRelation", typeof(TabItem))]
        public override TextImageRelation TextImageRelation
        {
            get
            {
                return base.TextImageRelation;
            }
            set
            {
                base.TextImageRelation = value;
            }
        }		

        /// <summary>
        /// Gets the <see cref="Telerik.WinControls.Primitives.ImagePrimitive"/> instance which is used by
        /// the TabItem to display its image.
        /// </summary>
        public ImagePrimitive ImagePrimitive
        {
            get 
            {
                return this.image; 
            }
        }

        /// <summary>
        /// Gets the <see cref="Telerik.WinControls.Primitives.TextPrimitive"/> instance which is used by
        /// the TabItem to display its text.
        /// </summary>
		public TextPrimitive TextPrimitive
		{
			get
			{
                return this.textPrimitive;
			}
		}

        /// <summary>
        /// Gets the <see cref="Telerik.WinControls.UI.RadTabStripElement"/> instance which the TabItem belongs to.
        /// </summary>
		public RadTabStripElement ParentTabStrip
		{
			get
			{
                //condition removed, due to problem with moving items from one tabstrip to another - egg. drag-drop
				//if (parentTabStrip == null)
				//{
					for (RadElement res = this.Parent; res != null && parentTabStrip == null; res = res.Parent)
					{
						parentTabStrip = res as RadTabStripElement;
					}
				//}

				return this.parentTabStrip;
			}
		}

        /// <summary>Gets or sets a value indicating whether the tab item is selected.</summary>
	//	[Browsable(false)]
		[RadPropertyDefaultValue("IsSelected", typeof(RadTabStripElement))]
		public bool IsSelected
		{
			get
			{
				return (bool) this.GetValue(RadTabStripElement.IsSelectedProperty);
			}
			set
			{
				if (this.ParentTabStrip != null)
				{
					this.ParentTabStrip.SelectedTab = this;
				}
				else
				{
					this.SetValue(RadTabStripElement.IsSelectedProperty, value);
				}
			}
		}


        [Browsable(false)]
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

		/// <summary>Gets or sets a value indicating whether the tab item is hovered.</summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[RadPropertyDefaultValue("IsSelected", typeof(RadTabStripElement))]
		public bool IsHovered
		{
			get
			{
				return (bool)this.GetValue(RadTabStripElement.IsHoveredProperty);
			}
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            //TODO: The place for this code is not here. Probably the TextPrimitive itself may expose PreferredSize property.
            //TODO: Refactor along with the RadTabStripElement
            if (this.parentTabStrip != null && this.parentTabStrip.OverflowMode == TabStripItemOverflowMode.Shrink)
            {
                TextParams textParams = this.textPrimitive.CreateTextParams();
                this.textPreferredSize = this.textPrimitive.MeasureOverride(LayoutUtils.InfinitySize, textParams);
            }

            return base.MeasureOverride(availableSize);
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == VisibilityProperty)
			{
				OnVisibilityChanged(EventArgs.Empty);

                if (!this.IsInValidState(true))
                {
                    return;
                }

                if (this.ElementTree.Control as RadTabStrip != null)
                {
                    RadTabStrip tabStrip = this.ElementTree.Control as RadTabStrip;
                    if (tabStrip != null)
                    {
                        if (tabStrip.EnableTabControlMode && this.ContentPanel != null)
                        {
                            ElementVisibility newValue = (ElementVisibility)e.NewValue;
                            if (newValue == ElementVisibility.Visible)
                                this.ContentPanelHost.Visibility = ElementVisibility.Visible;
                            else
                                this.ContentPanelHost.Visibility = ElementVisibility.Hidden;
                        }
                    }
                }
			}
			else if (e.Property == IsMouseOverProperty)
			{
				this.ParentTabStrip.CallOnTabHovered(new TabEventArgs(this));
			}
			else if (e.Property == EnabledProperty)
			{
                this.ContentPanel.Enabled = (bool)e.NewValue;
			}
			else if (e.Property == TextProperty || e.Property == ImageProperty)
			{
				// PATCH
				if (this.layout != null)
				{
					this.layout.PerformLayout();

					if (this.ParentTabStrip != null)
					{
						if (this.ParentTabStrip.BoxLayout != null)
							this.ParentTabStrip.BoxLayout.PerformLayout();
					}
				}
			}
            else if (e.Property == RadTabStripElement.IsSelectedProperty)
            {
                this.UpdateMargins();
            }

            if (this.UseNewLayoutSystem)
            {
                RadTabStripElement tabStripElement = this.ParentTabStrip;
                if (tabStripElement != null)
                {
                    if (e.Property == RadElement.StretchHorizontallyProperty)
                    {
                        if (tabStripElement.TabsPosition == TabPositions.Top || tabStripElement.TabsPosition == TabPositions.Bottom)
                        {
                            bool stretchText = (bool)e.NewValue;
                            this.layout.StretchHorizontally = stretchText;
                            //TODO: 
                            //this.TextPrimitive.StretchHorizontally = stretchText;
                        }
                    }
                    if (e.Property == RadElement.StretchVerticallyProperty)
                    {
                        if (tabStripElement.TabsPosition == TabPositions.Left || tabStripElement.TabsPosition == TabPositions.Right)
                        {
                            bool stretchText = (bool)e.NewValue;
                            this.layout.StretchHorizontally = stretchText;
                        }
                    }
                }
            }

			base.OnPropertyChanged(e);
		}

        internal void UpdateMargins()
        {
            if (this.parentTabStrip == null)
            {
                return;
            }

            Padding margins = this.IsSelected ? this.parentTabStrip.SelectedItemMargins : this.parentTabStrip.ItemMargins;
            switch(this.parentTabStrip.TabsPosition)
            {
                case TabPositions.Left:
                    margins = LayoutUtils.RotateMargin(margins, 270);
                    break;
                case TabPositions.Right:
                    margins = LayoutUtils.RotateMargin(margins, 90);
                    break;
                case TabPositions.Bottom:
                    margins = LayoutUtils.RotateMargin(margins, 180);
                    break;
            }

            this.SetDefaultValueOverride(RadElement.MarginProperty, margins);
        }

		protected virtual void OnVisibilityChanged(EventArgs e)
		{
			if (VisibilityChanged != null)
			{
				VisibilityChanged(this, e);
			}
		}

		internal ImageAndTextLayoutPanel layout;

		protected override void CreateChildElements()
		{
            this.contentPanel = new RadTabStripContentPanel();
            this.contentPanel.Visible = false;
            this.contentPanel.SetAssociatedItem(this);
            this.contentPanelHost = new RadHostItem(contentPanel);
            this.contentPanelHost.SetDefaultValueOverride(RadElement.AutoSizeModeProperty, RadAutoSizeMode.FitToAvailableSize);

            // When the docking is being shown for the first time an exception in TabItem was thrown.
            this.textPrimitive = new TextPrimitive();
            this.image = new ImagePrimitive();

            this.fill = new FillPrimitive();
            this.fill.Class = "TabFill";
			this.Children.Add(this.fill);

			this.tabBorder = new BorderPrimitive();
            this.tabBorder.Class = "TabItemBorder";
            this.tabBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.Children.Add(this.tabBorder);

			this.layout = new ImageAndTextLayoutPanel();
            this.layout.UseNewLayoutSystem = this.UseNewLayoutSystem;
            this.layout.StretchHorizontally = true;
            this.layout.StretchVertically = true;

            this.layout.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, TabItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            this.layout.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, TabItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layout.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, TabItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layout.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, TabItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);

            this.Children.Add(this.layout);

            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.OneWay);
            this.textPrimitive.Class = "TabItemText";
            this.textPrimitive.AutoSize = true;
            this.layout.Children.Add(this.textPrimitive);

            this.image.Class = "TabItemImage";
            this.image.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);

            this.image.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.image.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.image.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);

			this.layout.Children.Add(image);
        }        

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			ParentTabStrip.CallOnMouseDown(e);
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			ParentTabStrip.CallOnMouseMove(e);
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);
			ParentTabStrip.CallOnMouseUp(e);
		}

        /// <summary>
        /// Raises tunnel event (down the heirarchy) using the element that raises the event
        /// and the routed event arguments.
        /// </summary>
		public override void RaiseTunnelEvent(RadElement sender, RoutedEventArgs args)
		{

			base.RaiseTunnelEvent(sender, args);

			//TODO fix hardcoded selected state of the children
			if (args.RoutedEvent == RadTabStripElement.OnRoutedTabSelected)
			{
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(RadTabStripElement.IsSelectedProperty, true);
				}
			}
			else
				if (args.RoutedEvent == RadTabStripElement.OnTabDeselected)
				{
					foreach (RadElement child in this.ChildrenHierarchy)
					{
						child.SetValue(RadTabStripElement.IsSelectedProperty, false);
					}
				}
		}


        public Rectangle GetItemRectangleToTabStrip()
        {
            Rectangle rec = new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            return rec;
        }

        /// <summary>Retrieve the item if it occupies the given point.</summary>
        public TabItem GetItemByPoint(Point pt)
        {
            if (GetItemRectangleToTabStrip().Contains(pt))
                return this;
            return null;
        }

        /// <summary>Gets a size structure indicating text size.</summary>
		public Size GetTextSize()
		{
            return textPrimitive.GetTextSize().ToSize();
		}

        /// <summary>Gets the item as a bitmap image.</summary>
		public Bitmap GetItemBitmap()
		{
			Size size = GetTextSize();
			Bitmap bmp = new Bitmap(Size.Width, Size.Height);
			Graphics g = Graphics.FromImage(bmp);
			Telerik.WinControls.Paint.RadGdiGraphics gfx = new Telerik.WinControls.Paint.RadGdiGraphics(g);
			this.fill.PaintPrimitive(gfx, this.AngleTransform, this.ScaleTransform);
			this.image.PaintPrimitive(gfx, this.AngleTransform, this.ScaleTransform);
			this.textPrimitive.PaintPrimitive(gfx, this.AngleTransform, this.ScaleTransform);
			this.tabBorder.PaintPrimitive(gfx, this.AngleTransform, this.ScaleTransform);
            return bmp;
		}

        /// <summary>
        /// Gets the preferred size of the hosted TextPrimitive.
        /// </summary>
        [Browsable(false)]
        internal SizeF TextPreferredSize
        {
            get
            {
                return this.textPreferredSize;
            }
        }

		private RadTabStripContentPanel contentPanel;

		/// <summary>
		/// Gets the Panel that holds tab content when in tab-control mode.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[Description("Gets the Panel that holds tab content when in tab-control mode.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadTabStripContentPanel ContentPanel
		{
			get
			{
				return contentPanel;
			}
		}

		/// <summary>
		/// Gets the RadItemHost that holds tab ContentPanel when in tab-control mode.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public RadHostItem ContentPanelHost
		{
			get
			{
                return contentPanelHost;
			}
		}

        /// <commentsfrom cref="Telerik.WinControls.RadElement.ZIndex" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ZIndex
		{
			get
			{
				return base.ZIndex;
			}
			set
			{
				base.ZIndex = value;
			}
		}
	}
}
