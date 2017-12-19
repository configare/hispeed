using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
	public class RadImageItem : RadItem
	{
		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
        }

		public static RadProperty BorderVisibleProperty = RadProperty.Register(
			"BorderVisible", typeof(bool), typeof(RadImageItem), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ImageProperty = RadProperty.Register(
			"Image", typeof(Image), typeof(RadImageItem), new RadElementPropertyMetadata(
			null, ElementPropertyOptions.AffectsDisplay));

		public static RadProperty ImageIndexProperty = RadProperty.Register(
		   "ImageIndex", typeof(int), typeof(RadImageItem), new RadElementPropertyMetadata(
			   -1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ImageKeyProperty = RadProperty.Register(
			"ImageKey", typeof(string), typeof(RadImageItem), new RadElementPropertyMetadata(
				string.Empty, ElementPropertyOptions.None));


		[RadPropertyDefaultValue("BorderVisible", typeof(RadImageItem))]
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

		[RadPropertyDefaultValue("Image", typeof(RadImageItem)),		
		RefreshProperties(RefreshProperties.All),
		TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
		public Image Image
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
		
		[RadPropertyDefaultValue("ImageIndex", typeof(RadImageItem)),		
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ElementTree.Control.ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
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

		[RadPropertyDefaultValue("ImageKey", typeof(RadImageItem)),                 
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

		private ImagePrimitive imagePrimitive;
		private BorderPrimitive borderPrimitive;

		protected override void CreateChildElements()
		{
			this.imagePrimitive = new ImagePrimitive();
			this.imagePrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, 
				RadImageItem.ImageProperty, PropertyBindingOptions.TwoWay);

			this.Children.Add(this.imagePrimitive);
			
				
			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Visibility = ElementVisibility.Hidden;
			this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.Children.Add(this.borderPrimitive);
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadImageItem.BorderVisibleProperty)
			{
				bool borderVisible = (bool)e.NewValue;

				this.borderPrimitive.Visibility = (borderVisible) ? ElementVisibility.Visible :
					ElementVisibility.Hidden;
			}

			base.OnPropertyChanged(e);
		}
	}
}
