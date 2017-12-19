using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Drawing.Text;
using VisualStyles = System.Windows.Forms.VisualStyles;
using Telerik.WinControls.UI.Data;
using System.Diagnostics;


namespace Telerik.WinControls.UI
{
    /// <summary>Represents a list box item used in the RadListBox control.</summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadListBoxItem : RadItem, IComparable, IDataBoundItem
    {
        #region BitState Keys

        internal const ulong ChildElementsCreatedStateKey = RadItemLastStateKey << 1;
        internal const ulong CreatingChildElementsStateKey = ChildElementsCreatedStateKey << 1;
        internal const ulong AllowCreateChildElementsStateKey = CreatingChildElementsStateKey << 1;

        internal const ulong RadListBoxItemLastStateKey = AllowCreateChildElementsStateKey;

        #endregion

        #region Fields

        private BorderPrimitive borderElement;
		private ImageAndTextLayoutPanel layoutPanel;
		private ImagePrimitive imagePrimitive;
        private FillPrimitive fillPrimitive;

		private TextPrimitive textPrimitive;
		private TextPrimitive descriptionTextPrimitive;
		private RadMenuSeparatorItem textSeparator;
		private ElementVisibility textSeparatorVisibility = ElementVisibility.Collapsed;
		private object value = null;
        private object dataItem;
		private RadListBoxElement ownerElement;

        int index = -1;

        #endregion

		#region Constructors

        static RadListBoxItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadListBoxStateManagerFactory(), typeof(RadListBoxItem));
        }

		/// <summary>Initializes a new instance of the RadListBoxItem class.</summary>
		public RadListBoxItem()
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.CanFocus = true;
            this.NotifyParentOnMouseInput = true;
        }

		/// <summary>Initializes a new instance of the RadListBoxItem class using the item's text.</summary>
		public RadListBoxItem(string text)
		{
			Text = text;
		}

        /// <summary>
        /// Initializes a new instance of the RadListBoxItem class using item's dataItem, text and
        /// value.
        /// </summary>
        public RadListBoxItem(object dataItem, string text, object value)
            : this(text)
        {
            Value = value;
            this.dataItem = dataItem;
        }

		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text and
		/// value.
		/// </summary>
		public RadListBoxItem(string text, object value)
			: this(text)
		{
			Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text and 
		/// an onClick handler.
		/// </summary>
		public RadListBoxItem(string text, MouseEventHandler OnClick)
			: this(text)
		{
			MouseUp += OnClick;
		}
				
		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text, 
		/// it's description and value.
		/// </summary>
		public RadListBoxItem(string text, string descriptionText, object value)
			: this(text, descriptionText)
		{
			this.DescriptionText = descriptionText;
			Value = value;
		}
		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text, 
		/// it's description and an onClick handler.
		/// </summary>
		public RadListBoxItem(string text, string descriptionText, MouseEventHandler OnClick)
			: this(text, descriptionText)
		{
			this.DescriptionText = descriptionText;
			MouseUp += OnClick;
		}
		#endregion

		#region Dependency properties
        /// <summary>
        /// The instance of RadProperty that contains key and metadata for the Active property
        /// </summary>
		public static RadProperty ActiveProperty = RadProperty.Register("Active", typeof(bool), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ImageProperty = RadProperty.Register("Image", typeof(Image), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageIndexProperty = RadProperty.Register("ImageIndex", typeof(int), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyProperty = RadProperty.Register("ImageKey", typeof(string), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty DisplayStyleProperty = RadProperty.Register("DisplayStyle", typeof(DisplayStyle),	typeof(RadListBoxItem),
			new RadElementPropertyMetadata(DisplayStyle.ImageAndText, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageAlignmentProperty = RadProperty.Register("ImageAlignment", typeof(ContentAlignment),	typeof(RadListBoxItem),
			new RadElementPropertyMetadata(ContentAlignment.MiddleLeft,	ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TextAlignmentProperty = RadProperty.Register("TextAlignment",	typeof(ContentAlignment), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TextImageRelationProperty = RadProperty.Register("TextImageRelation",	typeof(TextImageRelation), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(TextImageRelation.Overlay, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty ValueProperty = RadProperty.Register(
            "Value", typeof(object), typeof(RadListBoxItem), new RadElementPropertyMetadata(
            null, ElementPropertyOptions.None));

		public static RadProperty DescriptionFontProperty =	RadProperty.Register(
			"DescriptionFont", typeof(Font), typeof(RadListBoxItem), new RadElementPropertyMetadata(
			Control.DefaultFont, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

		public static RadProperty SelectedProperty = RadProperty.Register("Selected", typeof(bool), typeof(RadListBoxItem),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));
		#endregion
		
		#region Properties

        public FillPrimitive ItemFill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        public BorderPrimitive ItemBorder
        {
            get
            {
                return this.borderElement;
            }
        }

        public TextPrimitive ItemText
        {
            get
            {
                return this.textPrimitive;
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

		internal bool Active
		{
			get { return (bool)GetValue(ActiveProperty); }
			set { SetValue(ActiveProperty, value); }
		}

		internal bool Selected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Index
        {
            get
            {
                return this.index;
            }

            internal set
            {
                this.index = value;
            }
        }

		/// <summary>
		/// Gets or sets the font of the descrition text of the RadListBoxItem. 
		/// </summary>
		[Description("DescriptionFont - ex. of the descritpion text of an RadListBoxItem. The property is inheritable through the element tree.")]
		[RadPropertyDefaultValue("DescriptionFont", typeof(RadListBoxItem)), 
		Category(RadDesignCategory.AppearanceCategory)]
		public virtual Font DescriptionFont
		{
			get
			{
				return (Font)this.GetValue(DescriptionFontProperty);
			}
			set
			{
				this.SetValue(DescriptionFontProperty, value);
			}
		}

		/// <summary>
		///		Gets or sets the description text associated with this item. 
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory),
	    Description("Gets or sets the description text associated with this item. "),
		Bindable(true),
		SettingsBindable(true),
		Editor(DesignerConsts.MultilineStringEditorString, typeof(UITypeEditor)),
	    RadPropertyDefaultValue("Text", typeof(TextPrimitive))]
		public string DescriptionText
		{
			get
			{
                this.EnsureChildElements();
				return this.descriptionTextPrimitive.Text;
			}
			set
			{
                this.EnsureChildElements();
				this.descriptionTextPrimitive.Text = value;
				if (value != string.Empty)
					this.textSeparator.Visibility = this.textSeparatorVisibility;
				else
					this.textSeparator.Visibility = ElementVisibility.Collapsed;
			}
		}

		/// <summary>
		/// Specifies the options for display of image and text primitives in the element.
		/// </summary>
		[Browsable(true), Localizable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Specifies the options for display of image and text primitives in the element.")]
		public DisplayStyle DisplayStyle
		{
			get
			{
				return (DisplayStyle)this.GetValue(DisplayStyleProperty);
			}
			set
			{
				this.SetValue(DisplayStyleProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the image that is displayed on a button element.
		/// </summary>
		[RadPropertyDefaultValue("Image", typeof(RadListBoxItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image that is displayed on a button element."),
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
		public virtual Image Image
		{
			get
			{
				return (Image)this.GetValue(ImageProperty);
			}
			set
			{
				this.SetValue(ImageProperty, value);				
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
		/// Gets or sets the image list index value of the image displayed on the button control.
		/// </summary>		
		[RadPropertyDefaultValue("ImageIndex", typeof(RadListBoxItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the image list index value of the image displayed on the button control."),
		RefreshProperties(RefreshProperties.All),
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
				this.SetValue(ImageIndexProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the key accessor for the image in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("ImageKey", typeof(RadListBoxItem)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the key accessor for the image in the ImageList."),
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKey
		{
			get
			{
				return (string)this.GetValue(ImageKeyProperty);
			}
			set
			{
				this.SetValue(ImageKeyProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the position of text and image relative to each other. 
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the position of text and image relative to each other.")]
		public virtual TextImageRelation TextImageRelation
		{
			get
			{
				return (TextImageRelation)this.GetValue(TextImageRelationProperty);
			}
			set
			{
				this.SetValue(TextImageRelationProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the alignment of text content on the drawing surface. 
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint),
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

		/// <summary>
		/// Gets or sets the visibility of the separator element between the text and the description text
		/// </summary>
		[DefaultValue(ElementVisibility.Collapsed), 
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the visibility of the separator element between the text and the description text.")]
		public virtual ElementVisibility TextSeparatorVisibility
		{
			get
			{
				return this.textSeparatorVisibility;
			}
			set
			{
                this.EnsureChildElements();
				this.textSeparatorVisibility = value;
				if (this.DescriptionText != string.Empty)
				{					
					this.textSeparator.Visibility = value;
				}
				else
				{
					this.textSeparator.Visibility = ElementVisibility.Collapsed;
				}
			}
		}

        /// <summary>
        /// Gets or sets data about this item.  
        /// </summary>
		[Browsable(false),//Category(RadDesignCategory.DataCategory),
        Localizable(false), 
        Bindable(true), 
        TypeConverter(typeof(StringConverter)),
        Description("Gets or sets data about this item."), 
        DefaultValue((string)null)]
        public object Value
        {
            get
            {
				return this.value;
            }
            set
            {
				this.value = value;
            }
        }

		/// <summary>
		/// Gets or sets the owner of the item. 
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadListBoxElement OwnerElement
		{
			get
			{
				return this.ownerElement;
			}
			set
			{
				this.ownerElement = value;
			}
		}

        public bool IsDataBound
        {
            get
            {
                return this.dataItem != null;
            }
        }

        /// <summary>
        /// Gets the original item from the DataSource
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem
		{
			get
			{
				return this.dataItem;
			}
		}

		void IDataBoundItem.SetDataItem(object dataItem)
		{
			this.dataItem = dataItem;
		}

		#endregion		

        #region Public Methods

		public Size GetDesiredSize()
        {
            Size res = Size.Empty;

            if (this.UseNewLayoutSystem)
            {
                if (this.GetBitState(NeverMeasuredStateKey))
                    this.Measure(new SizeF(float.PositiveInfinity, float.PositiveInfinity));
                res = Size.Round(this.DesiredSize);
            }
            else
            {
                //this.AssureElements();
                Size borderSize = Size.Round(this.borderElement.BorderSize);
                Size ownOffset = Size.Add(this.Padding.Size, borderSize);
                ownOffset = Size.Add(this.Margin.Size, ownOffset);
                res = Size.Add(this.layoutPanel.FullSize, ownOffset);
            }
            
            return res;
        }

        #endregion

        #region Lazy Loading

        protected override void CallCreateChildElements()
        {
            if (this.GetBitState(AllowCreateChildElementsStateKey))
            {
                base.CallCreateChildElements();
            }
        }

        public override RadElementCollection Children
        {
            get
            {
                this.EnsureChildElements();
                return base.Children;
            }
        }

        /// <summary>
        /// Ensures that all child elements are created.
        /// </summary>
        protected void EnsureChildElements()
        {
            if (this.GetBitState(ChildElementsCreatedStateKey) ||
                this.GetBitState(CreatingChildElementsStateKey))
            {
                return;
            }

            this.BitState[CreatingChildElementsStateKey] = true;
            this.BitState[AllowCreateChildElementsStateKey] = true;

            this.CallCreateChildElements();

            this.BitState[AllowCreateChildElementsStateKey] = false;
            this.BitState[CreatingChildElementsStateKey] = false;
            this.BitState[ChildElementsCreatedStateKey] = true;
        }

        protected override void LoadCore()
        {
            //child elements must be created upon element loading
            this.EnsureChildElements();

            base.LoadCore();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public override bool CanFocus
        {
            get
            {
                return base.CanFocus;
            }
            set
            {
                base.CanFocus = value;
            }
        }

        protected override void ApplyTheme()
        {
            this.EnsureChildElements();
            base.ApplyTheme();
        }

		protected override void CreateChildElements()
		{
            this.fillPrimitive = new FillPrimitive();
			fillPrimitive.Class = "ListBoxItemSelectionFill";
			fillPrimitive.Visibility = ElementVisibility.Hidden;
			fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.borderElement = new BorderPrimitive();
			this.borderElement.Class = "ListBoxItemSelectionBorder";
			this.borderElement.Visibility = ElementVisibility.Hidden;
			this.borderElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadListBoxItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadListBoxItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadListBoxItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);

			this.textPrimitive = new TextPrimitive();
            this.textPrimitive.Class = "ListBoxItemText";
            this.textPrimitive.UseMnemonic = false;
            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadListBoxItem.TextProperty, PropertyBindingOptions.OneWay);
            this.textPrimitive.BindProperty(TextPrimitive.AlignmentProperty, this, RadListBoxItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);

			this.descriptionTextPrimitive = new TextPrimitive();
			this.descriptionTextPrimitive.Class = "ListBoxItemDescriptionText";
            this.descriptionTextPrimitive.UseMnemonic = false;
			this.descriptionTextPrimitive.BindProperty(TextPrimitive.FontProperty, this, RadListBoxItem.DescriptionFontProperty, PropertyBindingOptions.OneWay);

            StackLayoutPanel textPanel = new StackLayoutPanel();
			textPanel.Orientation = Orientation.Vertical;
			textPanel.EqualChildrenWidth = true;
            textPanel.Children.Add(this.textPrimitive);

			textSeparator = new RadMenuSeparatorItem();
			textSeparator.NotifyParentOnMouseInput = true;
			textSeparator.Class = "ListBoxItemTextSeparator";
			textSeparator.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			textSeparator.Visibility = ElementVisibility.Collapsed;

			textPanel.Children.Add(textSeparator);
			textPanel.Children.Add(this.descriptionTextPrimitive);
			textPanel.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);

			this.layoutPanel = new ImageAndTextLayoutPanel();
            this.layoutPanel.StretchHorizontally = false;
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadListBoxItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadListBoxItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadListBoxItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadListBoxItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.Children.Add(this.imagePrimitive);
			this.layoutPanel.Children.Add(textPanel);

			this.Children.Add(fillPrimitive);
			this.Children.Add(this.borderElement);
			this.Children.Add(this.layoutPanel);
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == RadListBoxItem.ActiveProperty)
			{
                this.EnsureChildElements();
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(ActiveProperty, e.NewValue);
				}
			}
			else if (e.Property == RadListBoxItem.SelectedProperty)
			{
                this.EnsureChildElements();
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(SelectedProperty, e.NewValue);
				}
			}
        }
		
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			if (this.ownerElement != null)
			{
				this.ownerElement.ProcessItemTextChanged(e);
			}
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF res = base.MeasureOverride(availableSize);
            // If the item is empty...
            int emptyHeight = this.BorderThickness.Vertical + this.Padding.Vertical;
            if (res.Height <= emptyHeight)
            {
                Size textSize = TextRenderer.MeasureText("W", this.Font);
                res.Height = textSize.Height + emptyHeight;
            }
            return res;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!(e is MouseEventArgs))
            {
                return;
            }

            if (this.ownerElement == null)
            {
                return;
            }

            if (((MouseEventArgs)e).Button == MouseButtons.Left && this.ownerElement.SelectionMode == SelectionMode.MultiExtended)
            {
                // TODO: Uncomment once RadItem's Click event works correctly.
                //DispatchMouseNotification(MouseNotification.MouseDrag);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                DispatchMouseNotification(MouseNotification.MouseUp);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                DispatchMouseNotification(MouseNotification.MouseDown);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            DispatchMouseNotification(MouseNotification.Click);
        }

        private void DispatchMouseNotification(MouseNotification notification)
        {
            if (ownerElement != null)
            {
                ownerElement.ProcessMouseSelection(this, notification);
            }
        }

        #endregion

        #region IComparable Members
        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Compares the
        /// current instance with another object of the same type.</span>
        /// </summary>
		public int CompareTo(RadListBoxItem item)
		{
			if (item == null)
				return 1;			
			return CultureInfo.InvariantCulture.CompareInfo.Compare(this.Text, item.Text, CompareOptions.None);
		}

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Compares the
        /// current instance with another object of the same type.</span>
        /// </summary>
		public int CompareTo(object obj)
		{			
			return this.CompareTo((RadListBoxItem)obj);
		}
		#endregion

        #region System skinning

        

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            this.fillPrimitive.SetValue(RadItem.VisibilityProperty, ElementVisibility.Collapsed);
            this.borderElement.SetValue(RadItem.VisibilityProperty, ElementVisibility.Collapsed);
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();

            this.fillPrimitive.ResetValue(RadItem.VisibilityProperty, ValueResetFlags.Local);
            this.borderElement.ResetValue(RadItem.VisibilityProperty, ValueResetFlags.Local);
        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement  GetVistaVisualStyle()
        {
 	        return this.GetXPVisualStyle();
        }

        protected override void PaintElementSkin(Telerik.WinControls.Paint.IGraphics graphics)
        {

            if (SystemSkinManager.Instance.CurrentElement == SystemSkinManager.DefaultElement)
            {
                return;
            }

            base.PaintElementSkin(graphics);
        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement  GetXPVisualStyle()
        {
 	         if (!this.Enabled)
             {
                 return VisualStyles.VisualStyleElement.ListView.Item.Disabled;
             }
             else
             {
                 if (this.IsMouseOver && !this.Selected)
                 {
                     return VisualStyles.VisualStyleElement.ListView.Item.Hot;
                 }
                 else if (this.Selected && this.ElementTree.Control.Focused)
                 {
                     return VisualStyles.VisualStyleElement.ListView.Item.Selected;
                 }
                 else if (this.Selected && !this.ElementTree.Control.Focused)
                 {
                     return VisualStyles.VisualStyleElement.ListView.Item.SelectedNotFocus;
                 }
             }

             return SystemSkinManager.DefaultElement;
        }

        #endregion
    }
}
