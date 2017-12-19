using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using Telerik.WinControls.UI;
using System.Drawing.Imaging;
using System.ComponentModel;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an image button.
    /// </summary>
    public class RadImageButtonElement : RadButtonElement
	{
		private Image normalStateImage;
        


		#region Dependency properties

		public static RadProperty ImageHoveredProperty = RadProperty.Register(
			"ImageHovered",
			typeof(Image),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				null,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageIndexHoveredProperty = RadProperty.Register(
			"ImageIndexHovered",
			typeof(int),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				-1,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyHoveredProperty = RadProperty.Register(
			"ImageKeyHovered",
			typeof(string),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				string.Empty,
				ElementPropertyOptions.None));

		public static RadProperty ImageClickedProperty = RadProperty.Register(
			"ImageClicked",
			typeof(Image),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				null,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageIndexClickedProperty = RadProperty.Register(
			"ImageIndexClicked",
			typeof(int),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				-1,
				ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyClickedProperty = RadProperty.Register(
			"ImageKeyClicked",
			typeof(string),
			typeof(RadImageButtonElement),
			new RadElementPropertyMetadata(
				string.Empty,
				ElementPropertyOptions.None));

		#endregion

		#region Properties

		///<summary>
		/// Gets or sets the image that is displayed on a button element when it is hovered.
		/// </summary>
		[Description("Gets or sets the image that is displayed on a button element when it is hovered."),
		Category(RadDesignCategory.AppearanceCategory),
		Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		RadPropertyDefaultValue("ImageHovered", typeof(RadButtonItem))]		
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
		public virtual Image ImageHovered
		{
			get
			{
				return (Image)this.GetValue(ImageHoveredProperty);
			}
			set
			{
				this.SetValue(ImageHoveredProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the image list index value of the image displayed on the button control when it is hovered.
		/// </summary>
		[RadPropertyDefaultValue("ImageIndexHovered", typeof(RadButtonItem)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
	   Description("Gets or sets the image list index value of the image displayed on the button control when it is hovered."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        
		public virtual int ImageIndexHovered
		{
			get
			{
				return (int)this.GetValue(ImageIndexHoveredProperty);
			}
			set
			{
				this.SetValue(ImageIndexHoveredProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the key accessor for the image for hovered state in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("ImageKeyHovered", typeof(RadButtonItem)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the key accessor for the image for hovered state in the ImageList."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
		Localizable(true),
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKeyHovered
		{
			get
			{
				return (string)this.GetValue(ImageKeyHoveredProperty);
			}
			set
			{
				this.SetValue(ImageKeyHoveredProperty, value);
			}
		}

		///<summary>
		/// Gets or sets the image that is displayed on a button element when it is clicked.
		/// </summary>
		[Description("Gets or sets the image that is displayed on a button element when it is clicked."),
		Category(RadDesignCategory.AppearanceCategory),
		Localizable(true),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		RadPropertyDefaultValue("ImageClicked", typeof(RadButtonItem))]
		[Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
		public virtual Image ImageClicked
		{
			get
			{
				return (Image)this.GetValue(ImageClickedProperty);
			}
			set
			{
				this.SetValue(ImageClickedProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the image list index value of the image displayed on the button control when it is clicked.
		/// </summary>
		[RadPropertyDefaultValue("ImageIndexClicked", typeof(RadButtonItem)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
	   Description("Gets or sets the image list index value of the image displayed on the button control when it is clicked."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
		public virtual int ImageIndexClicked
		{
			get
			{
				return (int)this.GetValue(ImageIndexClickedProperty);
			}
			set
			{
				this.SetValue(ImageIndexClickedProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the key accessor for the image for clicked state in the ImageList.
		/// </summary>
		[RadPropertyDefaultValue("ImageKeyClicked", typeof(RadButtonItem)),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the key accessor for the image for clicked state in the ImageList."),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
		Localizable(true),
		TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKeyClicked
		{
			get
			{
				return (string)this.GetValue(ImageKeyClickedProperty);
			}
			set
			{
				this.SetValue(ImageKeyClickedProperty, value);
			}
		}

		#endregion

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DisplayStyle = DisplayStyle.Image;
            this.ImageAlignment = ContentAlignment.MiddleCenter;
        }

        private void EnsureNormalStateImageCached()
        {
            if (this.normalStateImage == null)
            {
                this.normalStateImage = this.Image;
            }
        }

        private bool chaingingImageInternally;

        private void SetNewStateImage(Image newImage)
        {
            chaingingImageInternally = true;

            if (newImage != null)
            {
                //avoid setting property locally 
                //to be able to respond to changes from theme
                PropertySetting setting = new PropertySetting(
                    RadButtonItem.ImageProperty,
                    newImage
                    );

                setting.ApplyValue(this);
            }

            chaingingImageInternally = false;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsMouseOverProperty)
            {
                EnsureNormalStateImageCached();
                SetImageIfHovered((bool)e.NewValue);
            }
            else if (e.Property == IsMouseDownProperty)
            {
                EnsureNormalStateImageCached();
                if ((bool)e.NewValue)
                {
                    this.SetNewStateImage(this.ImageClicked);                    
                }
                else
                {
                    SetImageIfHovered(this.IsMouseOver);
                }
            }
            else if (e.Property == RadButtonItem.ImageProperty)
            {
                if (!chaingingImageInternally)
                {
                    this.normalStateImage = null;
                    EnsureNormalStateImageCached();
                }                
            }
        }

        private void SetImageIfHovered(bool hovered)
        {
            if (hovered)
            {
                SetNewStateImage(this.ImageHovered);
            }
            else
            {
                SetNewStateImage(normalStateImage);
            }
        }

        [DefaultValue(DisplayStyle.Image)]
        public override DisplayStyle DisplayStyle
        {
            get
            {
                return base.DisplayStyle;
            }
            set
            {
                base.DisplayStyle = value;
            }
        }
    }
}

