using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
	public class RadLabelElement : RadItem
	{
        static RadLabelElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(RadLabelElement));
        }

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }
		
		public static RadProperty ImageProperty = RadProperty.Register(
           "Image",
           typeof(Image),
           typeof(RadLabelElement),
           new RadElementPropertyMetadata(
               null,
               ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DisplayStyleProperty = RadProperty.Register(
            "DisplayStyle",
            typeof(DisplayStyle),
            typeof(RadLabelElement),
            new RadElementPropertyMetadata(
                DisplayStyle.ImageAndText,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty ImageIndexProperty = RadProperty.Register(
            "ImageIndex",
            typeof(int),
            typeof(RadLabelElement),
            new RadElementPropertyMetadata(
                -1,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageKeyProperty = RadProperty.Register(
            "ImageKey",
            typeof(string),
            typeof(RadLabelElement),
            new RadElementPropertyMetadata(
                string.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageAlignmentProperty = RadProperty.Register(
            "ImageAlignment",
            typeof(ContentAlignment),
            typeof(RadLabelElement),
            new RadElementPropertyMetadata(
                ContentAlignment.MiddleLeft,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

  
        public static RadProperty TextImageRelationProperty = RadProperty.Register(
            "TextImageRelation",
            typeof(TextImageRelation),
            typeof(RadLabelElement),
            new RadElementPropertyMetadata(
                TextImageRelation.Overlay,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));



		public static RadProperty BorderVisibleProperty = RadProperty.Register(
			"BorderVisible",
			typeof(bool),
			typeof(RadLabelElement),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));
		
		public static RadProperty TextAlignmentProperty = RadProperty.Register(
			"TextAlignment",
			typeof(ContentAlignment),
			typeof(RadLabelElement),
			new RadElementPropertyMetadata(
				ContentAlignment.TopLeft,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		[RadPropertyDefaultValue("BorderVisible", typeof(RadLabelElement))]
		public bool BorderVisible
		{
			get
			{
				return (bool)this.GetValue(BorderVisibleProperty);
			}
			set
			{
				this.SetValue(BorderVisibleProperty, value);
			}
		}
      
        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [RadPropertyDefaultValue("TextWrap", typeof(TextPrimitive))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("True if the text should wrap to the available layout rectangle otherwise, false.")]
        public bool TextWrap
        {
            get
            {
                return this.textPrimitive.TextWrap;
            }
            set
            {
                this.textPrimitive.TextWrap = value;
            }
        }

		/// <summary>
		/// Gets or sets the alignment of text content on the drawing surface. 
		/// </summary>
		[RefreshProperties(RefreshProperties.Repaint),
		Localizable(true),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of text content on the drawing surface."),
		RadPropertyDefaultValue("TextAlignment", typeof(RadLabelElement))]
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

        /// <commentsfrom cref="TextPrimitive.UseMnemonic" filter=""/>
        [RadDefaultValue("UseMnemonic", typeof(TextPrimitive))]
        [RadDescription("UseMnemonic", typeof(TextPrimitive))]
        [Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool UseMnemonic
        {
            get
            {
                return this.textPrimitive.UseMnemonic;
            }
            set
            {
                this.textPrimitive.UseMnemonic = value;
            }
        }

        ///<summary>
        /// Gets or sets the image that is displayed on a button element.
        /// </summary>
        [RadPropertyDefaultValue("Image", typeof(RadLabelElement)),
        Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the image that is displayed on a button element."),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        //[Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
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
        /// Gets or sets the image list index value of the image displayed on the button control.
        /// </summary>
        [RadPropertyDefaultValue("ImageIndex", typeof(RadLabelElement)),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true),
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
        [RadPropertyDefaultValue("ImageKey", typeof(RadLabelElement)),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true),
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
        Description("Gets or sets the position of text and image relative to each other."),
       RadPropertyDefaultValue("TextImageRelation", typeof(RadLabelElement))]
        public virtual System.Windows.Forms.TextImageRelation TextImageRelation
        {
            get
            {
                return (System.Windows.Forms.TextImageRelation)this.GetValue(TextImageRelationProperty);
            }
            set
            {
                this.SetValue(TextImageRelationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of image content on the drawing surface. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the alignment of image content on the drawing surface.")]
        [RadPropertyDefaultValue("ImageAlignment", typeof(RadLabelElement))]
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
        /// Gets the element responsible for painting the background of the label
        /// </summary>
        [Description("Gets the element responsible for painting the background of the label")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	    public FillPrimitive LabelFill
	    {
	        get
	        {
	            return this.fillPrimitive;
	        }
	    }


        /// <summary>
        /// Gets the element responsible for painting the text of the label
        /// </summary>
        [Description("Gets the element responsible for painting the text of the label")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPrimitive LabelText
        {
            get
            {
                return this.textPrimitive;
            }
        }

        internal BorderPrimitive borderPrimitive;
	    internal FillPrimitive fillPrimitive;
		internal TextPrimitive textPrimitive;
        internal ImagePrimitive imagePrimitive;
        internal ImageAndTextLayoutPanel layoutPanel;

        protected override void CreateChildElements()
		{
			this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.FitToSizeMode = RadFitToSizeMode.FitToParentContent;
			this.borderPrimitive.Visibility = ElementVisibility.Hidden;
			
			this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.BackColor = Color.Transparent;
            this.fillPrimitive.BackColor2 = Color.Transparent;
            this.fillPrimitive.BackColor3 = Color.Transparent;
            this.fillPrimitive.BackColor4 = Color.Transparent;
			
			this.textPrimitive = new TextPrimitive();
			this.textPrimitive.Alignment = ContentAlignment.TopLeft;
			this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadLabelElement.TextProperty, PropertyBindingOptions.TwoWay);
            this.textPrimitive.BindProperty(AlignmentProperty, this, RadLabelElement.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.textPrimitive.AutoEllipsis = true;
            this.textPrimitive.TextWrap = true;

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadLabelElement.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadLabelElement.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadLabelElement.ImageKeyProperty, PropertyBindingOptions.TwoWay);
      
            this.layoutPanel = new ImageAndTextLayoutPanel();
            this.layoutPanel.UseNewLayoutSystem = this.UseNewLayoutSystem;
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.AutoSizeModeProperty, this, RadLabelElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(RadElement.StretchHorizontallyProperty, this, RadElement.StretchHorizontallyProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(RadElement.StretchVerticallyProperty, this, RadElement.StretchVerticallyProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadLabelElement.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.TextAlignment = (this.RightToLeft) ? ContentAlignment.TopRight : ContentAlignment.TopLeft;

            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadLabelElement.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadLabelElement.TextImageRelationProperty, PropertyBindingOptions.OneWay);
          
            this.layoutPanel.Children.Add(this.imagePrimitive);
            this.layoutPanel.Children.Add(this.textPrimitive);

            this.borderPrimitive.ZIndex = 2;
            this.borderPrimitive.BindProperty(RadElement.IsItemFocusedProperty, this, RadElement.IsFocusedProperty, PropertyBindingOptions.OneWay);
          
            this.layoutPanel.ZIndex = 3;

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.borderPrimitive);
            this.Children.Add(this.layoutPanel);
  		}

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if(e.Property == RadLabelElement.BorderVisibleProperty)
			{
				bool borderVisible = (bool) e.NewValue;
				
				this.borderPrimitive.Visibility = (borderVisible) ? ElementVisibility.Visible :
					ElementVisibility.Hidden;
			}
			else if (e.Property == RadElement.AutoSizeModeProperty)
			{
				this.textPrimitive.AutoSizeMode = (RadAutoSizeMode)e.NewValue;
				
				if((RadAutoSizeMode)e.NewValue == RadAutoSizeMode.FitToAvailableSize)
				{
					this.textPrimitive.TextAlignment = ContentAlignment.MiddleCenter;
				}
				else
				{
					this.textPrimitive.TextAlignment = ContentAlignment.MiddleLeft;
				}
			}
            else if (e.Property == TextAlignmentProperty &&
                this.ElementTree != null && this.ElementTree.Control as RadLabel != null)
            {
                if ((this.ElementTree.Control as RadLabel).AutoSize)
                {
                    this.layoutPanel.TextAlignment = (this.RightToLeft) ? ContentAlignment.TopRight : ContentAlignment.TopLeft;
                }
                else
                {
                    this.layoutPanel.TextAlignment = this.TextAlignment;
                }
            }
            else if (e.Property == RightToLeftProperty)
            {
                this.TextAlignment = (this.RightToLeft) ? ContentAlignment.TopRight : ContentAlignment.TopLeft;
            }
			
			base.OnPropertyChanged(e);
		}
	}
}
