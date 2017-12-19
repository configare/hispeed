using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a button element. The button element could be placed in each control's
    ///     Items collection. It encapsulates all the necessary logic related to the user
    ///     interaction and UI. The <see cref="RadButton">RadButton</see> class is a simple
    ///     wrapper for the RadButtonElement class. The <see cref="RadButton">RadButton</see>
    ///     acts to transfer events to and from its corresponding RadButtonElement instance.
    ///     The RadButtonElement which is essentially the RadButton control may be nested in
    ///     other telerik controls.
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
    public class RadButtonElement : RadButtonItem
	{
        /// <summary>
        /// Initializes a new instance of the RadButtonElement class.
        /// </summary>
		public RadButtonElement()
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.CanFocus = true;
        }
        
		public RadButtonElement(string text)
			: base(text)
		{
		}

		public RadButtonElement(string text, Image image)
			: base(text, image)
		{
		}

		static RadButtonElement()
		{
		    RadElement.CanFocusProperty.OverrideMetadata(
                typeof (RadButtonElement), new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay)
                );

			new Themes.ControlDefault.Button().DeserializeTheme();
		}

        public static RadProperty LargeImageProperty = RadProperty.Register(
			"LargeImage", typeof(Image), typeof(RadButtonElement), new RadElementPropertyMetadata(
				null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty LargeImageIndexProperty = RadProperty.Register(
			"LargeImageIndex", typeof(int), typeof(RadButtonElement), new RadElementPropertyMetadata(
				-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty LargeImageKeyProperty = RadProperty.Register(
			"LargeImageKey", typeof(string), typeof(RadButtonElement), new RadElementPropertyMetadata(
				string.Empty, ElementPropertyOptions.None));

		public static RadProperty SmallImageProperty = RadProperty.Register(
			"SmallImage", typeof(Image), typeof(RadButtonElement), new RadElementPropertyMetadata(
				null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SmallImageIndexProperty = RadProperty.Register(
			"SmallImageIndex", typeof(int), typeof(RadButtonElement), new RadElementPropertyMetadata(
				-1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SmallImageKeyProperty = RadProperty.Register(
			"SmallImageKey", typeof(string), typeof(RadButtonElement), new RadElementPropertyMetadata(
				string.Empty, ElementPropertyOptions.None));

		public static RadProperty UseSmallImageListProperty = RadProperty.Register(
			"UseSmallImageList", typeof(bool), typeof(RadButtonElement), new RadElementPropertyMetadata(
				false, ElementPropertyOptions.None));

		///<summary>
		/// Gets the large image that is displayed on a button element.
		/// </summary>
		[RadPropertyDefaultValue("LargeImage", typeof(RadButtonElement)),
	    Category(RadDesignCategory.AppearanceCategory),
		Description("Gets the large image that is displayed on a button element."),
		Browsable(false),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Image LargeImage
		{
			get
			{
				return (Image)this.GetValue(LargeImageProperty);
			}
		}

		/// <summary>
		/// Gets the large image list index value of the image displayed on the button control.
		/// </summary>
		[RadPropertyDefaultValue("LargeImageIndex", typeof(RadButtonElement)),		
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets the large image list index value of the image displayed on the button control."),
	    //RefreshProperties(RefreshProperties.Repaint), 
		Browsable(false),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString),		
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int LargeImageIndex
		{
			get
			{
				return (int)this.GetValue(LargeImageIndexProperty);
			}
		}

		/// <summary>
		/// Gets the large key accessor for the image in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("LargeImageKey", typeof(RadButtonElement)),		
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets the large key accessor for the image in the ImageList."),
	    //RefreshProperties(RefreshProperties.Repaint),
	    Browsable(false),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),		
		TypeConverter(DesignerConsts.RadImageKeyConverterString),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string LargeImageKey
		{
			get
			{
				return (string)this.GetValue(LargeImageKeyProperty);
			}
		}

		///<summary>
		/// Gets or sets the large image that is displayed on a button element.
		/// </summary>
		[RadPropertyDefaultValue("SmallImage", typeof(RadButtonElement)),
		Category(RadDesignCategory.AppearanceCategory),
	    Description("Gets or sets the large image that is displayed on a button element."),		
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
		RefreshProperties(RefreshProperties.All)]
		public virtual Image SmallImage
		{
			get
			{
				return (Image)this.GetValue(SmallImageProperty);
			}
			set
			{
				this.SetValue(SmallImageProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the small image list index value of the image displayed on the button control.
		/// </summary>
		[RadPropertyDefaultValue("SmallImageIndex", typeof(RadButtonElement)),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the small image list index value of the image displayed on the button control."),
	    RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.SmallImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
		public virtual int SmallImageIndex
		{
			get
			{
				return (int)this.GetValue(SmallImageIndexProperty);
			}
			set
			{
				this.SetValue(SmallImageIndexProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the small key accessor for the image in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("SmallImageKey", typeof(RadButtonElement)),		
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the small key accessor for the image in the ImageList."),
	    RefreshProperties(RefreshProperties.All), 
		RelatedImageList("ElementTree.Control.SmallImageList"),		
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),		
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string SmallImageKey
		{
			get
			{
				return (string)this.GetValue(SmallImageKeyProperty);
			}
			set
			{
				this.SetValue(SmallImageKeyProperty, value);
			}
		}

		/// <summary>
		///		Specifies whether the button should use the original image list or the small image list.
		/// </summary>
		[Browsable(false)]
		public virtual bool UseSmallImageList
		{
			get
			{
				return (bool) this.GetValue(UseSmallImageListProperty);
			}
			set
			{
				this.SetValue(UseSmallImageListProperty, value);
			}
		}

        // Fields
		protected FillPrimitive fillPrimitive;
		protected TextPrimitive textPrimitive;
        
		protected BorderPrimitive borderPrimitive;
		protected ImageAndTextLayoutPanel layoutPanel;
		protected ImagePrimitive imagePrimitive;

        /// <summary>
        /// Angle of rotation for the button image.
        /// Unlike AngleTransform the property ImagePrimitiveAngleTransform rotates the image only.
        /// AngleTransform rotates the whole button
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Angle of rotation for the button image")]
        [RadDefaultValue("AngleTransform", typeof(ImagePrimitive))]
		public float ImagePrimitiveAngleTransform
		{
			get
			{
				if ( this.imagePrimitive != null )
					return this.imagePrimitive.AngleTransform;
				return 0.0f;
			}
			set
			{
				if ( this.imagePrimitive != null )
					this.imagePrimitive.AngleTransform = value;
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

		protected override void CreateChildElements()
		{
			this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "ButtonFill";
			this.fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Class = "ButtonBorder";
			this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.textPrimitive = new TextPrimitive();
			this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
			this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadButtonItem.TextProperty, PropertyBindingOptions.OneWay);
			
			this.imagePrimitive = new ImagePrimitive();
			this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.UseSmallImageListProperty, this, RadButtonElement.UseSmallImageListProperty, PropertyBindingOptions.TwoWay);

			this.layoutPanel = new ImageAndTextLayoutPanel();
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.AutoSizeModeProperty, this, RadButtonItem.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(RadElement.StretchHorizontallyProperty, this, RadElement.StretchHorizontallyProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(RadElement.StretchVerticallyProperty, this, RadElement.StretchVerticallyProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadButtonItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);

			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadButtonItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadButtonItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadButtonItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);
						
			this.layoutPanel.Children.Add(this.imagePrimitive);
			this.layoutPanel.Children.Add(this.textPrimitive);
            //p.p. 6.4.2009 fix bug with text layout
            this.textPrimitive.BindProperty(RadElement.AlignmentProperty, this, RadButtonItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.imagePrimitive.BindProperty(RadElement.AlignmentProperty, this, RadButtonItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
			//end add
			this.borderPrimitive.ZIndex = 2;
			this.borderPrimitive.BindProperty(RadElement.IsItemFocusedProperty, this, RadElement.IsFocusedProperty, PropertyBindingOptions.OneWay);
			
			FocusPrimitive focusPrimitive = new FocusPrimitive(this.borderPrimitive);
			focusPrimitive.ZIndex = 4;
			focusPrimitive.Visibility = ElementVisibility.Hidden;
			focusPrimitive.Class = "ButtonFocus";
			this.layoutPanel.ZIndex = 3;

			focusPrimitive.BindProperty(RadElement.IsItemFocusedProperty, this, RadElement.IsFocusedProperty, PropertyBindingOptions.OneWay);

            this.Children.Add(this.fillPrimitive);
			this.Children.Add(this.layoutPanel);
			this.Children.Add(this.borderPrimitive);
			this.Children.Add(focusPrimitive);
            
		}

		/// <summary>
        /// Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement.
        /// </summary>
        [DefaultValue(true),
        Description("Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement."),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool MeasureTrailingSpaces
        {
            get
            {
                return this.textPrimitive.MeasureTrailingSpaces;
            }
            set
            {
                this.textPrimitive.MeasureTrailingSpaces = value;
            }
        }        


        /// <summary>
        /// Gets the FillPrimitive element that is reponsible for painting of the background of the control
        /// </summary>
		[Browsable(true),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FillPrimitive ButtonFillElement
    	{
    		get 
			{
				return this.fillPrimitive;
			}
    	}

        /// <summary>        
        /// Gets the BorderPrimitive element that is reponsible for painting of the border of the control        
        /// </summary>
		[Browsable(true),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BorderPrimitive BorderElement
		{
			get
			{
				return this.borderPrimitive;
			}
		}

        /// <summary>        
        /// Gets the TextPrimitive element that is reponsible for painting of the border of the control        
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual TextPrimitive TextElement
        {
            get
            {
                return this.textPrimitive;
            }
        }

		/// <summary>Gets or sets a value indicating whether the border is shown.</summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether the border is shown.")]
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get
			{
				return this.borderPrimitive.Visibility == ElementVisibility.Visible;
			}
			set
			{
				this.borderPrimitive.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
			}
		}

        /// <summary>
        /// Gets a reference to the ImagePrimitive of the RadButtonElement.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImagePrimitive ImagePrimitive
        {
            get
            {
                return imagePrimitive;
            }
        }


        #region ISupportSystemSkin

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override VisualStyleElement GetXPVisualStyle()
        {
            if (!Enabled)
            {
                return VisualStyleElement.Button.PushButton.Disabled;
            }

            if (IsMouseDown)
            {
                return this.IsMouseOver ? VisualStyleElement.Button.PushButton.Pressed : VisualStyleElement.Button.PushButton.Hot;
            }

            if (IsMouseOver)
            {
                return VisualStyleElement.Button.PushButton.Hot;
            }

            return IsDefault ? VisualStyleElement.Button.PushButton.Default : VisualStyleElement.Button.PushButton.Normal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override VisualStyleElement GetVistaVisualStyle()
        {
            return GetXPVisualStyle();
        }

        protected override bool ShouldPaintChild(RadElement element)
        {
            if (this.paintSystemSkin == true)
            {
                if (element == this.fillPrimitive || element == this.borderPrimitive)
                {
                    return false;
                }
            }

            return base.ShouldPaintChild(element);
        }

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            //update the forecolor for the text
            this.textPrimitive.ForeColor = SystemSkinManager.Instance.Renderer.GetColor(ColorProperty.TextColor);
        }

        #endregion
    }

	public class RibbonButtonBorderBehavior : PropertyChangeBehavior
	{
		private bool showAlwaysBorder = false;

		public RibbonButtonBorderBehavior()
			: this(false)
		{
		}

		public RibbonButtonBorderBehavior(bool showAlwaysBorder)
			: base(RadElement.IsMouseOverProperty)
		{
			this.showAlwaysBorder = showAlwaysBorder;
		}

		public override void OnPropertyChange(RadElement element, RadPropertyChangedEventArgs e)
		{
            //RadButtonElement button = (RadButtonElement) element;

            //if (button.ShowBorder || this.showAlwaysBorder)
            //    button.BorderElement.Visibility = (bool) e.NewValue ? ElementVisibility.Visible : ElementVisibility.Hidden;
		}
	}
}