using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
	public class RadGalleryGroupFilter : RadItem
	{
		private RadItemOwnerCollection items;
		private bool selected = false;		

		public RadGalleryGroupFilter()
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadGalleryGroupItem) };
        }

		public RadGalleryGroupFilter(string text)
		{
			this.Text = text;
		}

		/// <summary>
		/// Gets a collection representing the group items contained in this gallery filter.
		/// </summary>
		[RadEditItemsAction]
		[Category(RadDesignCategory.DataCategory),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
	    Description("Gets a collection representing the group items contained in this gallery filter.")]
		[Editor(typeof(FilteredGalleryGroupsEditor), typeof(UITypeEditor))]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
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
		/// Returns whether the filter is currently selected.
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description("Returns whether the filter is currently selected.")]
		public bool Selected
		{
			get { return this.selected; }
			set { this.selected = value; }
		}

		public override string ToString()
		{
			if (this.Site != null)
			{
				return this.Site.Name;
			}
			return base.ToString();
		}		
	}
}
