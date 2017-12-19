using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Primitives;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Telerik.WinControls.UI
{
    ///<exclude/>
    /// <summary>Represents a button item.</summary>
    [Designer(DesignerConsts.RadButtonItemDesignerString)]
    public class RadButtonItem : RadItem, IButtonControl, IImageElement
    {
        #region BitState Keys

        internal const ulong IsSharedImageStateKey = RadItemLastStateKey << 1;
        internal const ulong CaptureOnMouseDownStateKey = IsSharedImageStateKey << 1;

        #endregion

        #region Fields

        private DialogResult dialogResult = DialogResult.None;

        #endregion

        #region Property mapping

        private static Dictionary<RadProperty, RadProperty> propertiesForMapping;

        internal static Dictionary<RadProperty, RadProperty> PropertiesForMapping
        {
            get
            {
                return propertiesForMapping;
            }
        }

        internal override RadProperty MapStyleProperty(RadProperty propertyToMap, string settingType)
        {
            RadProperty result;
            RadButtonItem.PropertiesForMapping.TryGetValue(propertyToMap, out result);

            return result;
        }

        #endregion

        static RadButtonItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ButtonItemStateManagerFactory(), typeof(RadButtonItem));
            propertiesForMapping = new Dictionary<RadProperty, RadProperty>(1);
            AddPropertyMaps();
        }

        /// <summary>
        /// Initializes a new instance of the RadButtonItem class.
        /// </summary>
        public RadButtonItem()
		{            
		}

        /// <summary>
        /// Initializes a new instance of the RadButtonItem class and sets it's Text property to
        /// the provided string.
        /// </summary>
        /// <param name="text"></param>
		public RadButtonItem(string text)
		{
			this.Text = text;
		}

        /// <summary>
        /// Initializes a new instance of the RadButtonItem class, sets it's Text and Image
        /// properties to the provided string and Image.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="image"></param>
		public RadButtonItem(string text, Image image)
		{
			this.Text = text;
			this.Image = image;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[IsSharedImageStateKey] = true;
            this.BitState[CaptureOnMouseDownStateKey] = true;
        }

        #region Events
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler DoubleClick
        {
            add
            {
                base.DoubleClick += value;
            }
            remove
            {
                base.DoubleClick -= value;
            }
        }

        #endregion

        #region Properties

        #region Dependency Properties

        public static RadProperty ImageProperty = RadProperty.Register(
            "Image",
            typeof(Image),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
                null,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DisplayStyleProperty = RadProperty.Register(
            "DisplayStyle",
            typeof(DisplayStyle),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
				DisplayStyle.ImageAndText,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));
		
        public static RadProperty ImageIndexProperty = RadProperty.Register(
            "ImageIndex",
            typeof(int),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
				-1,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageKeyProperty = RadProperty.Register(
            "ImageKey",
            typeof(string),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
                string.Empty,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ImageAlignmentProperty = RadProperty.Register(
            "ImageAlignment",
            typeof(ContentAlignment),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
				ContentAlignment.MiddleLeft,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty TextAlignmentProperty = RadProperty.Register(
            "TextAlignment",
            typeof(ContentAlignment),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
				ContentAlignment.MiddleCenter,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsParentArrange));

        public static RadProperty TextImageRelationProperty = RadProperty.Register(
            "TextImageRelation",
            typeof(TextImageRelation),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
                TextImageRelation.Overlay,
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty IsDefaultProperty = RadProperty.Register(
            "IsDefault",
            typeof(bool),
            typeof(RadButtonItem),
            new RadElementPropertyMetadata(
                false,
                ElementPropertyOptions.None));

		public static RadProperty IsPressedProperty = RadProperty.Register(
			"IsPressed",
			typeof(bool),
			typeof(RadButtonItem),
			new RadElementPropertyMetadata(
				false,
				ElementPropertyOptions.None)); 

        #endregion

        ///<summary>
        /// Gets or sets the image that is displayed on a button element.
        /// </summary>
        [RadPropertyDefaultValue("Image", typeof(RadButtonItem)),
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
		[RadPropertyDefaultValue("ImageIndex", typeof(RadButtonItem)),        
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
				return (int) this.GetValue(ImageIndexProperty);
			}
			set
			{
				this.SetValue(ImageIndexProperty, value);
			}
		}

        /// <summary>
        /// Gets or sets the key accessor for the image in the ImageList.
        /// </summary>
		[RadPropertyDefaultValue("ImageKey", typeof(RadButtonItem)),         
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
		RadPropertyDefaultValue("TextImageRelation", typeof(RadButtonItem))]
        public virtual TextImageRelation TextImageRelation
		{
			get
			{
				return (TextImageRelation) this.GetValue(TextImageRelationProperty);
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
		[RadPropertyDefaultValue("ImageAlignment", typeof(RadButtonItem))]
        public virtual System.Drawing.ContentAlignment ImageAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue(ImageAlignmentProperty);
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
        Description("Gets or sets the alignment of text content on the drawing surface."),
		RadPropertyDefaultValue("TextAlignment", typeof(RadButtonItem))]
        public virtual System.Drawing.ContentAlignment TextAlignment
		{
			get
			{
				return (ContentAlignment) this.GetValue(TextAlignmentProperty);
			}
			set
			{
				this.SetValue(TextAlignmentProperty, value);
			}
		}
                
        /// <summary>
        /// Specifies the options for display of image and text primitives in the element.
        /// </summary>
		[Browsable(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Category(RadDesignCategory.AppearanceCategory)]
		[Description("Specifies the options for display of image and text primitives in the element.")]
		[RadPropertyDefaultValue("DisplayStyle", typeof(RadButtonItem))]
		public virtual DisplayStyle DisplayStyle
		{
			get
			{
				return (DisplayStyle) this.GetValue(DisplayStyleProperty);
			}
			set
			{
				this.SetValue(DisplayStyleProperty, value);
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the button item is in the pressed state.
		/// </summary>
        [ReadOnly(true), 
        Browsable(false), 
        Category(RadDesignCategory.AppearanceCategory),
	    Description("Gets a value indicating whether the button item is in the pressed state.")]
        public bool IsPressed
        {
            get
            {                
                return (bool)this.GetValue(RadButtonItem.IsPressedProperty);
            }
        }

        /// <summary>
        /// Determines if this button is the default button for the form it is on.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDefault
        {
            get
            {
                return (bool)base.GetValue(RadButtonItem.IsDefaultProperty);
            }
            set
            {
                base.SetValue(RadButtonItem.IsDefaultProperty, value);
            }
        }

        /// <summary>
        /// Determines whether the Image value of the current item is shared (reused by ither items).
        /// This flag is true by default. If it is set to false, then the item itselft will dispose the Image upon its disposal.
        /// </summary>
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether the Image value of the current item is shared (reused by other items). This flag is true by default; if it is set to false, then the item itselft will dispose the Image upon its disposal.")]
        public bool IsSharedImage
        {
            get
            {
                return this.GetBitState(IsSharedImageStateKey);
            }
            set
            {
                this.BitState[IsSharedImageStateKey] = value;
            }
        }
		
	    #endregion

        private static void AddPropertyMaps()
        {
            propertiesForMapping.Add(ImagePrimitive.ImageProperty, RadButtonItem.ImageProperty);
        }
        
        private bool HandleIsMouseOverChanged()
        {
            if (this.ClickMode != ClickMode.Hover)
            {
                return false;
            }
            if (base.IsMouseOver)
            {
				this.SetValue(RadButtonItem.IsPressedProperty, true);
            }
            else
            {
				this.SetValue(RadButtonItem.IsPressedProperty, false);
            }
            return true;
        }

        protected override void DisposeManagedResources()
        {
            if (!this.GetBitState(IsSharedImageStateKey))
            {
                Image image = this.Image;
                if (image != null)
                {
                    image.Dispose();
                }
            }
            base.DisposeManagedResources();
        }

		protected override bool IsItemHovered
		{
			get
			{
				bool res = base.IsItemHovered && this.IsPressed;
				
				return res;
			}
		}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.ClickMode != ClickMode.Hover && e.Button == MouseButtons.Left)
            {
				if (this.GetBitState(CaptureOnMouseDownStateKey))
				{
					this.Capture = true;
				}
                if (!this.IsPressed)
                {
					this.SetValue(RadButtonItem.IsPressedProperty, true);
                }
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
			if (this.ClickMode != ClickMode.Hover)
			{
				this.SetValue(RadButtonItem.IsPressedProperty, false);
				if (this.GetBitState(CaptureOnMouseDownStateKey) && base.Capture)
				{
					base.Capture = false;
				}
			}
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.HandleIsMouseOverChanged();
			if (base.Capture)
			{
				this.SetValue(RadButtonItem.IsPressedProperty, true);
			}
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.HandleIsMouseOverChanged();
			this.SetValue(RadButtonItem.IsPressedProperty, false);
        }

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape && this.IsPressed)
			{				
				this.SetValue(RadButtonItem.IsPressedProperty, false);
				return;
			}

			if (!this.Enabled)
				return;

			if (e.KeyCode == Keys.Space && !this.IsPressed)
			{
				this.SetValue(RadButtonItem.IsPressedProperty, true);
			}
			else if (e.KeyCode == Keys.Enter)
			{
				this.SetValue(RadButtonItem.IsPressedProperty, true);
				try
				{
					this.RaiseClick(System.EventArgs.Empty);
				}
				finally
				{
					this.SetValue(RadButtonItem.IsPressedProperty, false);
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (!this.Enabled)
				return;
			
			if (e.KeyCode == Keys.Space && this.IsPressed)
			{
				try
				{
					this.RaiseClick(EventArgs.Empty);
				}
				finally
				{
					this.SetValue(RadButtonItem.IsPressedProperty, false);
				}
			}
		}

        protected override void OnDoubleClick(EventArgs e)
        {
            this.OnClick(e);
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == RadElement.EnabledProperty && !(bool)e.NewValue)
			{
				this.SetValue(RadButtonItem.IsPressedProperty, false);
			}
			else if (e.Property == RadButtonItem.IsPressedProperty)
			{
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(RadButtonItem.IsPressedProperty, e.NewValue);
				}            
            }
		}

        /// <summary>
        /// implementing IButtonControl allows our button elemnts to act as standard
        /// default buttons used by the Form class
        /// </summary>
        #region IButtonControl Members

        DialogResult IButtonControl.DialogResult
        {
            get
            {
                return this.dialogResult;
            }
            set
            {
                this.dialogResult = value;
            }
        }

        void IButtonControl.NotifyDefault(bool value)
        {
            if (this.IsDefault != value)
            {
                this.IsDefault = value;
            }
        }

		protected override void OnClick(EventArgs e)
		{
			MouseEventArgs mouseEventArgs = e as MouseEventArgs;
			if (this.ElementState == ElementState.Loaded && 
                (e == EventArgs.Empty || (mouseEventArgs != null && mouseEventArgs.Button == MouseButtons.Left)))
			{
				Control control = this.ElementTree.Control;
				IButtonControl buttonControl = control as IButtonControl;
				if (buttonControl != null && buttonControl.DialogResult != DialogResult.None)
				{
					Form form = control.FindForm();
					if (form != null)
					{
						form.DialogResult = buttonControl.DialogResult;
					}
				}

               
            }

            if (this.Enabled)
            {
                base.OnClick(e);
            }
		}

        void IButtonControl.PerformClick()
        {
            this.OnClick(EventArgs.Empty);
        }

        #endregion
    }
}
