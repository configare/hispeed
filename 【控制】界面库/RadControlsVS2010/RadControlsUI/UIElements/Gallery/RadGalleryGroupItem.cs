using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	public class RadGalleryGroupItem : RadItem
	{
		private RadItemOwnerCollection items;

		private RadElement captionElement;
		private RadElement bodyElement;
		private ElementWithCaptionLayoutPanel groupLayoutPanel;
		private WrapLayoutPanel itemsLayoutPanel;		
		private TextPrimitive textPrimitive;

		public RadGalleryGroupItem()
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.itemsLayoutPanel = new WrapLayoutPanel();
            this.textPrimitive = new TextPrimitive();
            this.captionElement = new FillPrimitive();
            this.bodyElement = new FillPrimitive();
            this.groupLayoutPanel = new ElementWithCaptionLayoutPanel();

            this.NotifyParentOnMouseInput = true;
            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadGalleryItem) };
        }

		public RadGalleryGroupItem(string text)
		{
			this.Text = text;
		}

		[RadEditItemsAction]
		[Category(RadDesignCategory.DataCategory),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(GroupedGalleryItemsEditor), typeof(UITypeEditor))]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		[Browsable(false),
	    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WrapLayoutPanel ItemsLayoutPanel
		{
			get 
			{
				return this.itemsLayoutPanel; 
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the caption of the group is shown.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets a value indicating whether the caption of the group is shown."),
		RadDefaultValue("ShowCaption", typeof(ElementWithCaptionLayoutPanel))]
		public bool ShowCaption
		{
			get
			{
				return this.groupLayoutPanel.ShowCaption;
			}
			set
			{
				this.groupLayoutPanel.ShowCaption = value;
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
			this.textPrimitive.Class = "GalleryGroupCaption";
			this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.OneWay);			

			this.captionElement = new FillPrimitive();
			this.captionElement.AutoSizeMode = RadAutoSizeMode.Auto;
			this.captionElement.SetValue(ElementWithCaptionLayoutPanel.CaptionElementProperty, true);
			this.captionElement.Class = "GalleryGroupCaptionFill";
			this.captionElement.Children.Add(textPrimitive);
			

			this.bodyElement = new FillPrimitive();
			this.bodyElement.AutoSizeMode = RadAutoSizeMode.Auto;
			this.bodyElement.Class = "GalleryGroupBodyFill";
			this.bodyElement.Padding = new Padding(2, 2, 2, 0);
			this.bodyElement.Children.Add(this.itemsLayoutPanel);

			this.groupLayoutPanel = new ElementWithCaptionLayoutPanel();
			this.groupLayoutPanel.CaptionOnTop = true;
			this.groupLayoutPanel.Children.Add(captionElement);
			this.groupLayoutPanel.Children.Add(bodyElement);

			this.Children.Add(this.groupLayoutPanel);
		}

		public override void PerformLayoutCore(RadElement affectedElement)
		{
			Size availableSize = this.AvailableSize;
			Size preferredSize = this.groupLayoutPanel.GetPreferredSize(availableSize);

			this.groupLayoutPanel.Bounds = new Rectangle(Point.Empty, new Size(availableSize.Width, preferredSize.Height));
			this.bodyElement.Size = new Size(availableSize.Width, this.bodyElement.Size.Height);
			this.captionElement.Size = new Size(availableSize.Width, this.captionElement.Size.Height);
		}
	}
}
