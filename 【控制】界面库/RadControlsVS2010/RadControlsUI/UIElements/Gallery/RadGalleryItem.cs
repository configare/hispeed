using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    public class RadGalleryItem : RadButtonItem, ICloneable
    {
        static RadGalleryItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new GalleryItemStateManager(), typeof(RadGalleryItem));
        }

        public RadGalleryItem()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.NotifyParentOnMouseInput = true;
        }

        public RadGalleryItem(string text)
            : base(text)
        {
        }

        public RadGalleryItem(string text, Image image)
            : base(text, image)
        {
        }

        // Fields
        private FillPrimitive fillPrimitive;
        private TextPrimitive textPrimitive;
        private BorderPrimitive borderPrimitive;
        private ImageAndTextLayoutPanel layoutPanel;
        private ImagePrimitive imagePrimitive;
        private TextPrimitive descriptionTextPrimitive;
        private Font descriptionFontCache = null;

        private static RadProperty ActiveProperty = RadProperty.Register("Active", typeof(bool), typeof(RadGalleryItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty DescriptionFontProperty = RadProperty.Register(
            "DescriptionFont", typeof(Font), typeof(RadGalleryItem), new RadElementPropertyMetadata(
            Control.DefaultFont, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        internal static RadProperty IsSelectedProperty = RadProperty.Register("IsSelected", typeof(bool), typeof(RadGalleryItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        internal bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }

		/// <summary>
		/// Returns whether the gallery item is currently selected.
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
	    Description("Returns whether the gallery item is currently selected.")]
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font of the descrition text of the RadGalleryItem. 
        /// </summary>
        [Description("DescriptionFont - ex. of the descritpion text of an RadGalleryItem. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("DescriptionFont", typeof(RadGalleryItem)),
        Category(RadDesignCategory.AppearanceCategory)]
        public virtual Font DescriptionFont
        {
            get
            {
                if (this.descriptionFontCache == null)
                {
                    this.descriptionFontCache = (Font)this.GetValue(DescriptionFontProperty);
                }
                return this.descriptionFontCache;
            }
            set
            {
                this.SetValue(DescriptionFontProperty, value);
            }
        }

        private RadGalleryElement owner;

        internal RadGalleryElement Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
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
        RadPropertyDefaultValue("DescriptionText", typeof(RadGalleryItem))]
        public string DescriptionText
        {
            get
            {
                return this.descriptionTextPrimitive.Text;
            }
            set
            {
                this.descriptionTextPrimitive.Text = value;
            }
        }

        /// <summary>
        /// Angle of rotation for the button image.
        /// Unlike AngleTransform the property ImagePrimitiveAngleTransform rotates the image only.
        /// AngleTransform rotates the whole item
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Angle of rotation for the button image")]
        [RadDefaultValue("AngleTransform", typeof(ImagePrimitive))]
        public float ImagePrimitiveAngleTransform
        {
            get
            {
                return this.imagePrimitive.AngleTransform;
            }
            set
            {
                this.imagePrimitive.AngleTransform = value;
            }
        }

        public override string ToString()
		{
			if (this.Site != null) 
			{
				return this.Site.Name;
			}
			return base.ToString();
		}

        protected override void CreateChildElements()
        {
            fillPrimitive = new FillPrimitive();
            fillPrimitive.Class = "GalleryItemSelectionFill";
            fillPrimitive.Visibility = ElementVisibility.Hidden;
            fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            borderPrimitive = new BorderPrimitive();
            borderPrimitive.Class = "GalleryItemSelectionBorder";
            borderPrimitive.Visibility = ElementVisibility.Hidden;
            borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadGalleryItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadGalleryItem.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadGalleryItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);

            textPrimitive = new TextPrimitive();
            textPrimitive.Class = "GalleryItemText";
            textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadGalleryItem.TextProperty, PropertyBindingOptions.OneWay);
            textPrimitive.BindProperty(TextPrimitive.AlignmentProperty, this, RadGalleryItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);

            this.descriptionTextPrimitive = new TextPrimitive();
            this.descriptionTextPrimitive.Class = "GalleryItemDescriptionText";
            this.descriptionTextPrimitive.BindProperty(TextPrimitive.FontProperty, this, RadGalleryItem.DescriptionFontProperty, PropertyBindingOptions.OneWay);

            StackLayoutPanel textPanel = new StackLayoutPanel();
            textPanel.Orientation = Orientation.Vertical;
            textPanel.EqualChildrenWidth = true;
            textPanel.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            textPanel.Children.Add(textPrimitive);
            textPanel.Children.Add(this.descriptionTextPrimitive);


            this.layoutPanel = new ImageAndTextLayoutPanel();
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadGalleryItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadGalleryItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadGalleryItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadGalleryItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.Children.Add(imagePrimitive);
            this.layoutPanel.Children.Add(textPanel);

            this.Children.Add(fillPrimitive);
            this.Children.Add(borderPrimitive);
            this.Children.Add(this.layoutPanel);

        }


        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            return layoutPanel.DesiredSize + this.Padding.Size+new SizeF(borderPrimitive.Width, borderPrimitive.Width);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadGalleryItem.IsSelectedProperty)
            {
                foreach (RadElement child in this.ChildrenHierarchy)
                {
                    child.SetValue(RadGalleryItem.IsSelectedProperty, e.NewValue);
                }
            }
             
        }

        public bool IsZoomShown()
        {
            PropertyChangeBehaviorCollection behaviorCollection = this.GetBehaviors();

            if( behaviorCollection.Count==0 )
            {
                return false;
            }

            GalleryMouseOverBehavior galleryMouseOverBehavior = behaviorCollection[0] as GalleryMouseOverBehavior;
            if( galleryMouseOverBehavior == null )
            {
                return false;
            }
            return galleryMouseOverBehavior.IsPopupShown;
        }

        public void ZoomHide()
        {
            PropertyChangeBehaviorCollection behaviorCollection = this.GetBehaviors();

            if (behaviorCollection.Count == 0)
            {
                return;
            }

            GalleryMouseOverBehavior galleryMouseOverBehavior = behaviorCollection[0] as GalleryMouseOverBehavior;
            if (galleryMouseOverBehavior == null)
            {
                return;
            }
            galleryMouseOverBehavior.ClosePopup();
        }

        #region ICloneable Members

        public object Clone()
        {
            RadGalleryItem newItem = new RadGalleryItem(this.Text, this.Image);
            return newItem;
        }

        #endregion
    }
}
