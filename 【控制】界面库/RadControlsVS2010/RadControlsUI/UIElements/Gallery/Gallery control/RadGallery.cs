using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Displays a highly customizable scrollable list of items composed of images and/or text
    /// </summary>
	[ToolboxItem(false)]
	//[RadThemeDesignerData(typeof(RadGalleryDesignTimeData))]
	[Description("Displays a highly customizable scrollable list of items composed of images and/or text")]
    [Obsolete("This control is obsolete and must not be used.")]
	public class RadGallery : RadControl
	{
		private RadGalleryElement galleryElement;

        /// <summary>
        /// Gets the instance of RadGalleryElement wrapped by this control. RadGalleryElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadGallery.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadGalleryElement GalleryElement
        {
            get
            {
                return this.galleryElement;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.Items" filter=""/>
		[RadEditItemsAction]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		[RadDescription("Items", typeof(RadGalleryElement))]
		public RadItemCollection Items
		{
			get
			{
				return this.GalleryElement.Items;
			}
		}

		/// <summary>
		/// Gets a collection representing the groups contained in this gallery.
		/// </summary>
		[RadEditItemsAction]
		[Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Description("Gets a collection representing the groups contained in this gallery.")]
		public RadItemOwnerCollection Groups
		{
			get
			{
				return this.GalleryElement.Groups;
			}
		}

		/// <summary>
		/// Gets or sets the maximum number of columns to be shown in the in-ribbon portion of the gallery. 
		/// </summary>
		[RadEditItemsAction]
		[Category(RadDesignCategory.BehaviorCategory),
		RadDefaultValue("MaxColumns", typeof(RadGalleryElement)),
		Description("Gets or sets the maximum number of columns to be shown in the in-ribbon part of the gallery.")]
		public int MaxColumns
		{
			get
			{
				return this.GalleryElement.MaxColumns;
			}
			set
			{
				this.GalleryElement.MaxColumns = value;
			}
		}


		/// <summary>
		/// Gets or sets the maximum number of rows to be shown in the in-ribbon portion of the gallery. 
		/// </summary>
		[RadEditItemsAction]
		[Category(RadDesignCategory.BehaviorCategory),
		RadDefaultValue("MaxRows", typeof(RadGalleryElement)),
		Description("Gets or sets the maximum number of rows to be shown in the in-ribbon part of the gallery.")]
		public int MaxRows
		{
			get
			{
				return this.galleryElement.MaxRows;
			}
			set
			{
				this.galleryElement.MaxRows = value;
			}
		}

        /// <summary>
        /// Default constructor
        /// </summary>
		public RadGallery()
		{
			base.SetStyle(ControlStyles.Selectable, true);
		}

        public override ComponentThemableElementTree ElementTree
        {
            get
            {
                if (this.elementTree == null)
                {
                    this.elementTree = new ComponentOverrideElementTree(this);
                }
                return this.elementTree;
            }
        }

        //Nested types
        public class ComponentOverrideElementTree : ComponentThemableElementTree
        {
            private RadGallery owner = null;

            public ComponentOverrideElementTree(RadControl owner)
                : base(owner)
            {
                this.owner = owner as RadGallery;
            }

            /// <summary>
            /// create child items
            /// </summary>
            /// <param name="parent"></param>
            protected internal override void CreateChildItems(RadElement parent)
            {
                this.owner.galleryElement = new RadGalleryElement();
				this.owner.galleryElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
                this.RootElement.Children.Add(this.owner.galleryElement);
                base.CreateChildItems(parent);
            }
        }
	}
}
