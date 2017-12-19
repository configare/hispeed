using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Runtime.InteropServices;
using Telerik.WinControls.UI;
using Telerik.WinControls.Styles;
namespace Telerik.WinControls.UI
{
	[ToolboxItem(false), ComVisible(false)]
	[Designer(DesignerConsts.RadPanelBarGroupDesignerString)]
	public class RadPanelBarGroupElement : RadItem
    {
        #region Constructors
        static RadPanelBarGroupElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadPanelBarGroupElementStateManagerFactory(), typeof(RadPanelBarGroupElement));
        }
        #endregion

        #region Events

        /// <summary>
		/// Occurs when the Expanded property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the Expanded property value changes.")]
		public event EventHandler GroupExpandedChanged;

		/// <summary>
		/// Occurs when the Selected property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the Selected property value changes.")]
		public event EventHandler GroupSelectedChanged;

		/// <summary>
		/// Occurs when the  Caption property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the  Caption property value changes.")]
		public event EventHandler CaptionChanged;

		/// <summary>
		/// Occurs when the CaptionButtonPosition property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the CaptionButtonPosition property value changes.")]
		public event EventHandler CaptionButtonPositionChanged;

		/// <summary>
		/// Occurs when the CaptionButtonRightOffset property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the CaptionButtonRightOffset property value changes.")]

		public event EventHandler CaptionButtonRightOffsetChanged;

		/// <summary>
		/// Occurs when the Image property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the Image property value changes.")]
		public event EventHandler ImageChanged;

		/// <summary>
		/// Occurs when the ImageIndex property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the ImageIndex property value changes.")]
		public event EventHandler ImageIndexChanged;

		/// <summary>
		/// Occurs when the ImageKey property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the ImageKey property value changes.")]
		public event EventHandler ImageKeyChanged;


		/// <summary>
		///		Raises the <see cref="CaptionChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCaptionChanged(EventArgs args)
		{
			if (this.CaptionChanged != null)
			{
				this.CaptionChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="CaptionButtonPositionChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCaptionButtonPositionChanged(EventArgs args)
		{
			if (this.CaptionButtonPositionChanged != null)
			{
				this.CaptionButtonPositionChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="CaptionButtonRightOffsetChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCaptionButtonRightOffsetChanged(EventArgs args)
		{
			if (this.CaptionButtonRightOffsetChanged != null)
			{
				this.CaptionButtonRightOffsetChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="ImageChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnImageChanged(EventArgs args)
		{
			if (this.ImageChanged != null)
			{
				this.ImageChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="ImageIndexChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnImageIndexChanged(EventArgs args)
		{
			if (this.ImageIndexChanged != null)
			{
				this.ImageIndexChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="ImageKeyChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnImageKeyChanged(EventArgs args)
		{
			if (this.ImageKeyChanged != null)
			{
				this.ImageKeyChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="GroupSelectedChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnGroupSelectedChanged(EventArgs args)
		{
			if (this.GroupSelectedChanged != null)
			{
				this.GroupSelectedChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="GroupExpandedChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnGroupExpandedChanged(EventArgs args)
		{
			if (this.GroupExpandedChanged != null)
			{
				this.GroupExpandedChanged(this, args);
			}
		}
		#endregion

		private RadPanelBarVisualElement captionFillPrimitive;
		private RadItemOwnerCollection items;
		internal BoxLayout verticalLayout;
		internal BoxLayout verticalGroupLayout;
		internal BoxLayout horizontalLayout;
		private bool expandOnDbClick;
		private RadPanelBarElement panelBarElement;
		private RadPanelBarCaptionButton captionButton;
		private RadHostItem contentPanelHost;
		private LightVisualElement groupBackGround;
   
		public static RadProperty ImageAlignmentProperty = RadProperty.Register(
	 "ImageAlignment",
	 typeof(ContentAlignment),
	 typeof(RadPanelBarGroupElement),
	 new RadElementPropertyMetadata(
		 ContentAlignment.MiddleLeft,
		 ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TextAlignmentProperty = RadProperty.Register(
			"TextAlignment",
			typeof(ContentAlignment),
			typeof(RadPanelBarGroupElement),
			new RadElementPropertyMetadata(
				ContentAlignment.MiddleCenter,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));


		public static RadProperty CaptionProperty = RadProperty.Register(
			"Caption", typeof(string), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
            "Default Caption", ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty ExpandedProperty = RadProperty.Register(
			"Expanded", typeof(bool), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty SelectedProperty = RadProperty.Register(
			"Selected", typeof(bool), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.None));

		public static RadProperty CaptionButtonPositionProperty = RadProperty.Register(
			"CaptionButtonPosition", typeof(CaptionButtonPositions), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			CaptionButtonPositions.Left, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty InitialOffsetProperty = RadProperty.Register(
			"InitialOffset", typeof(int), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SpacingBetweenItemsProperty = RadProperty.Register(
			"SpacingBetweenItems", typeof(int), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty CaptionButtonRightOffsetProperty = RadProperty.Register(
			"CaptionButtonRightOffset", typeof(int), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty PanelBarStyleProperty = RadProperty.Register(
		"PanelBarStyle", typeof(PanelBarStyles), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			PanelBarStyles.ListBar, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageProperty = RadProperty.Register(
		 "Image", typeof(Image), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
			 null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageIndexProperty = RadProperty.Register(
			"ImageIndex", typeof(int), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
				-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyProperty = RadProperty.Register(
			"ImageKey", typeof(string), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
				string.Empty, ElementPropertyOptions.None));

        private int? contentPanelSize = null;

        /// <summary>
        /// Gets or sets the size of the content panel.
        /// </summary>
        /// <value>The size of the content panel.</value>
        [Description("Gets or sets the size of the content panel.")]
        [Category("Layout")]
        [DefaultValue(null)]
        public int? ContentPanelSize
        {
            get
            {
                return this.contentPanelSize;
            }
            set
            {
                if (value != this.ContentPanelSize)
                {
                    this.contentPanelSize = value;
                    RadPanelBarElement panelBarElement = this.GetPanelBarElement();

                    if (value != null)
                    {
                        if (value.Value >= 0)
                        {
                            this.ApplyContentSize(value.Value);
                        }
                    }
                    else
                    {
                        this.ResetContentPanelProperties();

                        if (this.EnableHostControlMode && panelBarElement != null)
                        {
                            panelBarElement.CurrentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { this },
                                this.EnableHostControlMode);
                        }

                    }

                    if (panelBarElement != null)
                    {
                        panelBarElement.UpdateScrollbars();
                    }

                    this.OnNotifyPropertyChanged("ContentPanelSize");
                }
            }
        }

        internal void ApplyContentSize(int value)
        {
            if (this.ContentPanelSize == null)
            {
                return;
            }

            RadPanelBarElement panelBarElement = this.GetPanelBarElement();

            if (this.ContentPanel == null)
            {
                return;
            }

            if (this.ContentPanelHost == null)
            {
                return;
            }

            if (panelBarElement == null)
            {
                return;
            }

            if (panelBarElement.PanelBarStyle == PanelBarStyles.OutlookNavPane || panelBarElement.PanelBarStyle == PanelBarStyles.ListBar)
            {
                this.ContentPanelHost.StretchVertically = true;
                this.ContentPanelHost.StretchHorizontally = true;
                this.ContentPanelHost.MinSize = Size.Empty;
                this.ContentPanelHost.MaxSize = Size.Empty;
                this.ContentPanel.MaximumSize = Size.Empty;
                this.ContentPanel.MinimumSize = Size.Empty;
                this.ContentPanelHost.Size = Size.Empty;
                this.ContentPanel.Size = Size.Empty;
            }
            else
            {
                this.ContentPanel.AutoSize = false;

                this.ContentPanelHost.StretchVertically = false;
                this.ContentPanelHost.StretchHorizontally = true;
                
                this.ContentPanelHost.MaxSize = Size.Empty;
                this.ContentPanelHost.MinSize = new Size(0, this.ContentPanelSize.Value);

                this.ContentPanel.MinimumSize = new Size(0, this.ContentPanelSize.Value);
                this.ContentPanel.MaximumSize = Size.Empty;

                this.ContentPanelHost.Size = new Size(0, this.ContentPanelSize.Value);
                this.ContentPanel.Size = new Size(0, this.ContentPanelSize.Value);
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
            this.contentPanel = new RadTabStripContentPanel();
            this.contentPanelHost = new RadHostItem(this.contentPanel);

            if (this.ContentPanelSize != null)
            {
                this.ApplyContentSize(this.ContentPanelSize.Value);
            }
            
            this.SaveContentPanelProperties();
        }

        private bool panelAutoSize;
        private bool panelAutoScroll;

        internal void SaveContentPanelProperties()
        {
            this.panelAutoSize = this.contentPanel.AutoSize;
            this.panelAutoScroll = this.contentPanel.AutoScroll;
		}

		internal void ResetContentPanelProperties()
		{
            this.contentPanel.AutoSize = this.panelAutoSize;
            this.contentPanel.AutoScroll = !this.contentPanel.AutoScroll;
            this.contentPanel.MinimumSize = Size.Empty;
            this.contentPanelHost.MinSize = Size.Empty;
            this.contentPanel.MaximumSize = Size.Empty;
            this.contentPanelHost.MaxSize = Size.Empty;
            this.contentPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            this.contentPanel.AutoScroll = !this.contentPanel.AutoScroll;
        }


        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
            if (target != null)
            {
                    if (target.StretchHorizontally == true && target.StretchVertically == true)
                    {
                     //   target.StretchHorizontally = false;
                        target.StretchVertically = false;
                        if (this.Items.Owner != this.verticalGroupLayout)
                        {
                            RadElement owner = this.Items.Owner;
                            this.Items.Owner = this.verticalGroupLayout;
                            this.Items.Owner = owner;
                        }

                    }
                
                

                if (!this.Expanded)
                {
                    this.CollapseChildren(false);
                }
            }
		}


        /// <summary>
		///		Gets a collection of items which are children of the PanelBarGroup element.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
		[Description("Gets the collection of items which are children of the PanelBarGroup element.")]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		/// <summary>
		/// Expands the group on double Click
		/// </summary>
		[Description("Expands the group on double Click")]
		[Category("Behavior")]
		[DefaultValue(false)]
		[Browsable(false)]
		[Obsolete("This property will be removed in the next version")]
		public bool ExpandOnDoubleClick
		{
			get
			{
				return this.expandOnDbClick;
			}
			set
			{
				this.expandOnDbClick = value;
			}
		}

		

		private bool allowAngleTransformAnimation;

		/// <summary>
		///		Enables or disables the angleTransformAnimation	
		/// </summary>
		[DefaultValue(false)]
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Enables or disables the angleTransformAnimation")]
		[Obsolete("This property will be removed in the next version")]
		public bool AllowAngleTransformAnimation
		{
			get
			{
				return this.allowAngleTransformAnimation;
			}

			set
			{
				this.allowAngleTransformAnimation = value;
			}
		}

		private bool enableHostControlMode;

		/// <summary>
    	///Defines whether the group will appear in ControlMode	///
        ///</summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Defines whether the group will appear in ControlMode")]
		[DefaultValue(false)]
		public bool EnableHostControlMode
		{
			get
			{
				return this.enableHostControlMode;
			}
			set
			{
				this.enableHostControlMode = value;

				if (this.GetPanelBarElement() != null)
				{
					this.ResetContentPanelProperties();
					this.GetPanelBarElement().CurrentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { this }, value);
				}
			}
		}


		/// <summary>
		/// Gets or sets the alignment of image content on the drawing surface. 
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of image content on the drawing surface.")]
		public virtual System.Drawing.ContentAlignment ImageAlignment
		{
			get
			{
				return (ContentAlignment)this.GetValue(ImageAlignmentProperty);
			}
			set
			{
				this.SetValue(ImageAlignmentProperty, value);
			}
		}
		/// <summary>
		/// Gets or sets the alignment of text content on the drawing surface. 
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint),
		Localizable(true),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of text content on the drawing surface.")]
		public virtual System.Drawing.ContentAlignment TextAlignment
		{
			get
			{
				return (ContentAlignment)this.GetValue(TextAlignmentProperty);
			}
			set
			{
				this.SetValue(TextAlignmentProperty, value);
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
			internal set
			{
				this.contentPanel = value;
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
			internal set
			{
				this.contentPanelHost = value;
			}
		}

		/// <summary>
		/// Gets or sets whether the group is expanded
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the group is expanded.")]
		[DefaultValue(false)]
        public bool Expanded
		{
			get
			{
				return (bool)this.GetValue(ExpandedProperty);
			}
			set
			{
				this.SetValue(ExpandedProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets whether the group is selected
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets whether the group is selected.")]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
                return (bool)this.GetValue(SelectedProperty);
			}
			set
			{
				this.SetValue(SelectedProperty, value);
            }
		}


		[Description("Gets or sets the image that is displayed on a button element."),
		 Category(RadDesignCategory.AppearanceCategory),
		 Localizable(true),
		 TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
		 RefreshProperties(RefreshProperties.All),
		RadPropertyDefaultValue("Image", typeof(RadPanelBarGroupElement))]
	    [Browsable(false)]
        public virtual Image Image
		{
			get
			{
				return (Image)this.GetValue(ImageProperty);
			}
			set
			{
				if (this.Image != null && value != this.Image && (this.ImageIndex >= 0 || !String.IsNullOrEmpty(this.ImageKey)))
				{
					this.ImageIndex = -1;
					this.ImageKey = "";
				}

				this.SetValue(ImageProperty, value);
				this.OnImageChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the image list index value of the image displayed on the button control.
		/// </summary>
		[RadPropertyDefaultValue("ImageIndex", typeof(RadPanelBarGroupElement)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image list index value of the image displayed on the button control."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
		public virtual int ImageIndex
		{
			get
			{
				return (int)this.GetValue(ImageIndexProperty);
			}
			set
			{
				if (this.ImageKey != "")
				{
					this.ImageKey = "";
				}

				this.SetValue(ImageIndexProperty, value);
				this.OnImageIndexChanged(EventArgs.Empty);
			}
		}

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
		{
            base.OnTunnelEvent(sender, args);

            if (!this.IsInValidState(true))
            {
                return;
            }

			if (args.RoutedEvent == RootRadElement.OnRoutedImageListChanged)
			{
				if (this.ElementTree.ComponentTreeHandler.ImageList == null)
				{
					this.ImageIndex = -1;
					this.ImageKey = "";
				}
			}
		}


		/// <summary>
		/// Gets or sets the key accessor for the image in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("ImageKey", typeof(RadPanelBarGroupElement)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the key accessor for the image in the ImageList."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
		Localizable(true),
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKey
		{
			get
			{
				return (string)this.GetValue(ImageKeyProperty);
			}
			set
			{
				if (this.ImageIndex != -1)
				{
					this.ImageIndex = -1;
				}
				
				this.SetValue(ImageKeyProperty, value);
				this.OnImageKeyChanged(EventArgs.Empty);


			}
		}

		/// <summary>
		/// Gets or sets the right offset of the caption button which is part of the caption of every group
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the right offset of the caption button which is part of the caption of every group")]
		public int CaptionButtonRightOffset
		{
			get
			{
				return (int)this.GetValue(CaptionButtonRightOffsetProperty);
			}
			set
			{
				this.SetValue(CaptionButtonRightOffsetProperty, value);

				if (this.captionButton != null)
				{
					this.captionButton.Margin = new Padding(this.captionButton.Margin.Left,
						this.captionButton.Margin.Top, this.captionButton.Margin.Right + value, this.captionButton.Margin.Bottom);
				}

				this.OnCaptionButtonRightOffsetChanged(EventArgs.Empty);
			}
		}


		/// <summary>
		/// Gets or sets the Spacing between caption elements
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the Spacing between caption elements")]
		[DefaultValue(0)]
		[Obsolete("The property will be removed with the next version")]
		public int SpacingBetweenItems
		{
			get
			{
				return (int)this.GetValue(SpacingBetweenItemsProperty);
			}
			set
			{
				this.SetValue(SpacingBetweenItemsProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the initial offset of caption elements
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the initial offset of caption elements")]
		[DefaultValue(0)]
		public int InitialOffset
		{
			get
			{
				return (int)this.GetValue(InitialOffsetProperty);
			}
			set
			{
				this.SetValue(InitialOffsetProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets CaptionButton's position
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets CaptionButton's position.")]
		[DefaultValue(typeof(CaptionButtonPositions), "Right")]
		public CaptionButtonPositions CaptionButtonPosition
		{
			get
			{

				return (CaptionButtonPositions)this.GetValue(CaptionButtonPositionProperty);
			}
			set
			{
				this.SetValue(CaptionButtonPositionProperty, value);
				this.OnCaptionButtonPositionChanged(EventArgs.Empty);

				if (this.GetCaptionButton() != null)
				{
					if (value == CaptionButtonPositions.Left)
					{
						this.GetCaptionButton().Alignment = ContentAlignment.MiddleLeft;
					}
					else
					{
						this.GetCaptionButton().Alignment = ContentAlignment.MiddleRight;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets caption's height.
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets caption's height.")]
		[DefaultValue(0.0)]
		public float CaptionHeight
		{
			get
			{
				return this.captionFillPrimitive.DesiredSize.Height;
			}
		}


		/// <summary>
		/// Gets or sets the Caption
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the caption")]
		[DefaultValue("Default Caption"), Localizable(true)]
		public string Caption
		{
			get
			{
				return (string)this.GetValue(CaptionProperty);
			}
			set
			{
				this.SetValue(CaptionProperty, value);
				this.OnCaptionChanged(EventArgs.Empty);
			}
		}


		public RadPanelBarVisualElement GetCaptionElement()
		{
			return this.captionFillPrimitive;
		}

		public RadPanelBarCaptionButton GetCaptionButton()	
		{
			return this.captionButton;
		}

		public RadPanelBarElement GetPanelBarElement()
		{
			if (this.panelBarElement == null)
			{
				for (RadElement res = this.Parent; res != null && panelBarElement == null; res = res.Parent)
				{
					panelBarElement = res as RadPanelBarElement;
				}
			}

			return this.panelBarElement;
		}

		public void ShowCaptionButton(bool show)
		{
			if (!show)
				this.captionButton.Visibility = ElementVisibility.Collapsed;
			else
				this.captionButton.Visibility = ElementVisibility.Visible;

		}

        public static RadProperty ExpandExplorerBarImageProperty = RadProperty.Register(
"GroupExpandExplorerBarImage", typeof(Image), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
 null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image GroupExpandExplorerBarImage
        {
            get
            {
                return (Image)this.GetValue(ExpandExplorerBarImageProperty);
            }
            set
            {
                this.SetValue(ExpandExplorerBarImageProperty, value);
            }
        }

        public static RadProperty CollapseExplorerBarImageProperty = RadProperty.Register(
        "GroupCollapseExplorerBarImage", typeof(Image), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
            null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image GroupCollapseExplorerBarImage
        {
            get
            {
                return (Image)this.GetValue(CollapseExplorerBarImageProperty);
            }
            set
            {
                this.SetValue(CollapseExplorerBarImageProperty, value);
            }
        }


        public static RadProperty ExpandVSImageProperty = RadProperty.Register(
    "GroupExpandVSImage", typeof(Image), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
        null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image GroupExpandVSImage
        {
            get
            {
                return (Image)this.GetValue(ExpandVSImageProperty);
            }
            set
            {
                this.SetValue(ExpandVSImageProperty, value);
            }
        }

        public static RadProperty CollapseVSImageProperty = RadProperty.Register(
        "GroupCollapseVSImage", typeof(Image), typeof(RadPanelBarGroupElement), new RadElementPropertyMetadata(
            null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image GroupCollapseVSImage
        {
            get
            {
                return (Image)this.GetValue(CollapseVSImageProperty);
            }
            set
            {
                this.SetValue(CollapseVSImageProperty, value);
            }
        }

        public class RadPanelBarGroupCaptionFill : RadPanelBarVisualElement
        {
            static RadPanelBarGroupCaptionFill()
            {
                ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadPanelBarGroupCaptionFillStateManager(), typeof(RadPanelBarGroupCaptionFill));
            }
        }

        public class RadPanelBarGroupCaptionFillStateManager : ItemStateManagerFactory
        {
            protected override StateNodeBase CreateSpecificStates()
            {
                StateNodeWithCondition selectedState = new StateNodeWithCondition("Selected", new SimpleCondition(RadPanelBarGroupElement.SelectedProperty, true));
                return selectedState;
            }

            protected override void AddDefaultVisibleStates(ItemStateManager sm)
            {
                sm.AddDefaultVisibleState("Selected");
                base.AddDefaultVisibleStates(sm);
            }
        }

		protected override void CreateChildElements()
		{
            this.items.ItemTypes = new Type[] 
            {
                typeof(RadToggleButtonElement), typeof(RadCheckBoxElement), 
                typeof(RadRadioButtonElement), typeof(RadLabelElement), 
                typeof(RadButtonElement)
            };

			this.captionButton = new RadPanelBarCaptionButton(this);
			this.captionButton.Text = "";
			this.captionButton.Click += new EventHandler(button_Click);
			this.captionButton.Class = "CaptionButton";
			this.captionButton.Visibility = ElementVisibility.Collapsed;
			this.captionButton.StretchHorizontally = false;
			this.captionButton.StretchVertically = false;
            this.captionButton.SetDefaultImages();

			this.verticalLayout = new BoxLayout();
			this.verticalLayout.Orientation = Orientation.Vertical;
			this.verticalLayout.Class = "VerticalLayout";

			this.captionFillPrimitive = new RadPanelBarGroupCaptionFill();
			this.captionFillPrimitive.StretchHorizontally = true;
			this.captionFillPrimitive.StretchVertically = true;
			this.captionFillPrimitive.Text = "";
			this.captionFillPrimitive.DrawFill = true;
			this.captionFillPrimitive.Padding = new Padding(1, 1, 1, 1);
			this.captionFillPrimitive.Class = "GroupCaptionFill";
			this.captionFillPrimitive.ShouldHandleMouseInput = true;
			this.captionFillPrimitive.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.captionFillPrimitive.MouseDown += new MouseEventHandler(captionFillPrimitive_MouseDown);
            this.captionFillPrimitive.MouseUp += new MouseEventHandler(captionFillPrimitive_MouseUp);
            this.captionFillPrimitive.Click += new EventHandler(captionFillPrimitive_Click);
            this.captionFillPrimitive.MouseEnter += new EventHandler(captionFillPrimitive_MouseEnter);
            this.captionFillPrimitive.MouseLeave += new EventHandler(captionFillPrimitive_MouseLeave);
            this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.SelectedProperty, this, RadPanelBarGroupElement.SelectedProperty, PropertyBindingOptions.TwoWay);
            this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.ImageIndexProperty, this, RadPanelBarGroupElement.ImageIndexProperty, PropertyBindingOptions.TwoWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.ImageProperty, this, RadPanelBarGroupElement.ImageProperty, PropertyBindingOptions.TwoWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.ImageKeyProperty, this, RadPanelBarGroupElement.ImageKeyProperty, PropertyBindingOptions.TwoWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.TextProperty, this, RadPanelBarGroupElement.CaptionProperty, PropertyBindingOptions.OneWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.TextAlignmentProperty, this, RadPanelBarGroupElement.TextAlignmentProperty, PropertyBindingOptions.TwoWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.ImageAlignmentProperty, this, RadPanelBarGroupElement.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
			this.captionFillPrimitive.BindProperty(RadPanelBarVisualElement.InitialOffsetProperty, this, RadPanelBarGroupElement.InitialOffsetProperty, PropertyBindingOptions.OneWay);
			this.captionFillPrimitive.RadPropertyChanged += new RadPropertyChangedEventHandler(captionFillPrimitive_RadPropertyChanged);	
			this.captionFillPrimitive.Children.Add(this.captionButton);

			this.horizontalLayout = new BoxLayout();
			this.horizontalLayout.Children.Add(this.captionFillPrimitive);
			this.horizontalLayout.StretchHorizontally = false;
			this.horizontalLayout.StretchVertically = false;

			this.groupBackGround = new LightVisualElement();
			this.groupBackGround.Class = "BackGroundFill";
			this.groupBackGround.DrawFill = true;
			this.groupBackGround.DrawBorder = true;

			this.verticalLayout.Children.Add(this.horizontalLayout);
			this.verticalLayout.StretchHorizontally = false;
			this.verticalLayout.StretchVertically = false;

			this.verticalGroupLayout = new BoxLayout();
			this.verticalGroupLayout.Orientation = Orientation.Vertical;
			this.verticalGroupLayout.Class = "GroupLayout";
			this.verticalGroupLayout.StretchHorizontally = false;
			this.verticalGroupLayout.StretchVertically = false;
            this.verticalGroupLayout.Visibility = ElementVisibility.Collapsed;

			this.verticalLayout.Children.Add(this.verticalGroupLayout);

			this.groupBackGround.Children.Add(this.verticalLayout);

			this.Children.Add(this.groupBackGround);

			this.StretchHorizontally = false;
			this.StretchVertically = false;

			this.Items.Owner = this.verticalGroupLayout;

			this.verticalGroupLayout.RightToLeft = false;
			this.verticalLayout.RightToLeft = false;
			this.horizontalLayout.RightToLeft = false;
   		}

        void captionFillPrimitive_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        void captionFillPrimitive_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        private void captionFillPrimitive_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void captionFillPrimitive_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }	

		protected override void OnNotifyPropertyChanged(string propertyName)
		{
			if (propertyName == "ToolTipText")
			{
				if (this.captionFillPrimitive != null)
				{
					this.captionFillPrimitive.ToolTipText = this.ToolTipText;
				}
			}

			if (propertyName == "AutoToolTip")
			{
				if (this.captionFillPrimitive != null)
				{
					this.captionFillPrimitive.AutoToolTip = this.AutoToolTip;
				}
			}

			base.OnNotifyPropertyChanged(propertyName);
		}

        private void captionFillPrimitive_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{		
			if (e.Property == PaddingProperty)
			{
				if (this.GetPanelBarElement().CurrentStyle != null)
					this.GetPanelBarElement().CurrentStyle.GetBaseLayout().UpdateLayout();
			}
		}

		private void button_Click(object sender, EventArgs e)
		{
			this.Expanded = !this.Expanded;
		}

		internal void CollapseGroup()
		{
			if (this.GetPanelBarElement() == null)
				return;

			if (this.GetCaptionButton() == null)
				return;

			this.GetPanelBarElement().CurrentStyle.PerformAction(new RadPanelBarGroupElement[] { this }, GroupAction.Collapse);
		}

		internal void SelectGroup()
		{
			if (this.GetPanelBarElement() == null)
				return;

			this.GetPanelBarElement().CurrentStyle.PerformAction(new RadPanelBarGroupElement[] { this }, GroupAction.Select);

		}

		internal void UnSelectGroup()
		{
			if (this.GetPanelBarElement() == null)
				return;

			this.GetPanelBarElement().CurrentStyle.PerformAction(new RadPanelBarGroupElement[] { this }, GroupAction.UnSelect);

		}

		internal void ExpandGroup()
		{
			if (this.GetPanelBarElement() == null)
				return;

			if (this.GetCaptionButton() == null)
				return;

			this.GetPanelBarElement().CurrentStyle.PerformAction(new RadPanelBarGroupElement[] { this }, GroupAction.Expand);
		}

		public void PerformExpand()
		{
			if (this.Expanded)
			{
				this.ExpandGroup();
			}
			else
			{
				this.CollapseGroup();
			}
		}

		public void PerformSelect()
		{
			if (this.Selected)
			{
				this.SelectGroup();
			}
			else
			{
				this.UnSelectGroup();
			}
		}

		private void captionFillPrimitive_MouseDown(object sender, MouseEventArgs e)
		{
            this.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Right)
                return;

			if (this.GetPanelBarElement().Enabled)
			{
				this.SetValue(ExpandedProperty, !((bool)this.GetValue(ExpandedProperty)));
				this.SetValue(SelectedProperty, !((bool)this.GetValue(SelectedProperty)));
	            
            }

     	}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			RadPanelBarElement panelBarElement = this.GetPanelBarElement();
			if (panelBarElement == null)
			{
                base.OnPropertyChanged(e);
				return;
			}

			if (e.Property == ImageIndexProperty)
			{
				if (this.captionFillPrimitive != null)
				{
					this.captionFillPrimitive.ImageIndex = this.ImageIndex;
				}
			}

			if (e.Property == ImageKeyProperty)
			{
				if (this.captionFillPrimitive != null)
				{
					this.captionFillPrimitive.ImageKey = this.ImageKey;
				}
			}

			if (e.Property == ImageProperty)
			{
				if (this.captionFillPrimitive != null)
				{
					if (!(this.ImageIndex > -1 || !String.IsNullOrEmpty(this.ImageKey)))
					{
						this.captionFillPrimitive.Image = this.Image;
					}
				}

			}

			if (e.Property == ExpandedProperty)
			{	
				if ((bool)e.NewValue)
				{
					this.GetCaptionButton().ChangeStyle(GroupStatePrimitive.GroupState.Expanded);
				}
				else
				{
					this.GetCaptionButton().ChangeStyle(GroupStatePrimitive.GroupState.Collapsed);
				}

				if (!panelBarElement.IsUpdating)
				{
					this.PerformExpand();
				}
				this.OnGroupExpandedChanged(EventArgs.Empty);
			}

			if (e.Property == SelectedProperty)
			{
				if (!panelBarElement.IsUpdating)
				{
					this.PerformSelect();
				}

                this.captionFillPrimitive.SetValue(RadPanelBarGroupElement.SelectedProperty, e.NewValue);
				this.OnGroupSelectedChanged(EventArgs.Empty);
			}

			if (e.Property == RightToLeftProperty)
			{
				AutoSetCaptionButtonPosition();

				this.horizontalLayout.RightToLeft = false;
				this.verticalGroupLayout.RightToLeft = false;
				this.verticalLayout.RightToLeft = false;
			}

			base.OnPropertyChanged(e);
		}

		internal void AutoSetCaptionButtonPosition()
		{
            RadPanelBarElement panelBar = this.GetPanelBarElement();
            if (panelBar == null || !panelBar.IsInValidState(true))
				return;

            if (panelBar.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
			{
				if (panelBar.PanelBarStyle == PanelBarStyles.ExplorerBarStyle)
				{
					this.CaptionButtonPosition = CaptionButtonPositions.Left;
				}
				else
				{
					this.CaptionButtonPosition = CaptionButtonPositions.Right;
				}
			}
			else
			{
                if (panelBar.PanelBarStyle == PanelBarStyles.ExplorerBarStyle)
				{
					this.CaptionButtonPosition = CaptionButtonPositions.Right;
				}
				else
				{
					this.CaptionButtonPosition = CaptionButtonPositions.Left;
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
            if (e.Button != MouseButtons.Left)
                return;

            if (this.GetPanelBarElement().Enabled && this.DesignMode)
            {
                this.Expanded = !this.Expanded;
                this.Selected = !this.Selected;
            }
			
			base.OnMouseDown(e);
		}

		public void CollapseChildren(bool collapse)
		{
			if (collapse)
			{
				foreach (RadElement element in this.verticalGroupLayout.Children)
					element.Visibility = ElementVisibility.Visible;

				this.verticalGroupLayout.Visibility = ElementVisibility.Visible;
			}
			else
			{
				foreach (RadElement element in this.verticalGroupLayout.Children)
					element.Visibility = ElementVisibility.Collapsed;

				this.verticalGroupLayout.Visibility = ElementVisibility.Collapsed;

			}
		}

		public void ShowChildren(bool show)
		{
			if (show)
			{
				foreach (RadElement element in this.verticalGroupLayout.Children)
					element.Visibility = ElementVisibility.Visible;

				this.verticalGroupLayout.Visibility = ElementVisibility.Visible;
			}
			else
			{
				foreach (RadElement element in this.verticalGroupLayout.Children)
					element.Visibility = ElementVisibility.Hidden;

				this.verticalGroupLayout.Visibility = ElementVisibility.Hidden;

			}
		}

		
		private AnimatedPropertySetting animatedExpand = new AnimatedPropertySetting(
		RadElement.PaddingProperty,
		new Padding(0, 0, 0, 0),
		new Padding(0, 0, 0, 0),
		8,
		40);

		private AnimatedPropertySetting animatedAngle = new AnimatedPropertySetting(
		RadElement.AngleTransformProperty,
		0f,
		90f,
		16,
		40);



		internal void Expand(bool expand)
		{
			// TO DO 
			// ANIMATIONS ON EXPAND/COLLAPSE
			if (this.verticalGroupLayout != null)
			{
				if (this.GetPanelBarElement().PanelBarStyle != PanelBarStyles.OutlookNavPane)
				{
					if (this.captionButton != null)
					{
						//	this.captionButton.ChangeStyle();
					}

					if (this.EnableHostControlMode)
					{
						this.GetPanelBarElement().CurrentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { this },
							this.EnableHostControlMode);
					}
					{
						if (expand)
						{
							this.CollapseChildren(true);
						}
						else
						{
							this.CollapseChildren(false);
						}
					}
				}

			}
		}
	}

	public enum CaptionButtonPositions
	{
		Right,
		Left,

	}
	

}
	
