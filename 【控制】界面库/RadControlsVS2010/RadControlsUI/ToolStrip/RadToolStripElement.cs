using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;
//using Telerik.WinControls.UI.ToolStrip;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls.UI
{
	/// <summary>Represents a tool strip element -  a bar in RadToolStrip in which 
	/// RadToolStripItems can be placed.</summary>
    [Designer(DesignerConsts.RadToolStripElementDesignerString)]
    public class RadToolStripElement : RadItem, IItemsOwner
	{
        private RadItemOwnerCollection items;
		private ToolStripElementLayout stripLayout;
		private FillPrimitive stripFill;
		private Stack<RadItem> collapsedItems;
		private RadToolStripManager parentToolStripManager;
		private BorderPrimitive border;
     

        /// <summary>
        /// Gets or sets orientation - it could be horizontal or vertical.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [RadDefaultValue("Orientation", typeof(ToolStripElementLayout))]
        internal Orientation Orientation
        {
            get
            {
                return this.stripLayout.Orientation;
            }
            set
            {
                if (value != this.stripLayout.Orientation)
                {
                    Size temp = this.MinSize;
                    this.MinSize = new Size(temp.Height, temp.Width);
                }
                this.stripLayout.Orientation = value;
            
			}
        }

	
		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.MinSize = new Size(0, 21);
            this.collapsedItems = new Stack<RadItem>();
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
		
			if (operation == ItemsChangeOperation.Inserted)
			{
                ((RadToolStripItem)target).Orientation = this.Orientation;

                if ( this.ParentToolStripManager != null )
                {
                    if (this.ParentToolStripManager.ShowOverFlowButton == false)
                    {
                        ((RadToolStripItem)target).OverflowManager.DropDownButton.Visibility = ElementVisibility.Hidden;
                    }
                    else
                    {
                        ((RadToolStripItem)target).OverflowManager.DropDownButton.Visibility = ElementVisibility.Visible;
                    }
                }

                target.SetValue(RadToolStripManager.ToolStripOrientationProperty, this.Orientation);
				foreach (RadElement childElement in target.Children)
				{
					childElement.SetValue(RadToolStripManager.ToolStripOrientationProperty, this.Orientation);
				}
			}
		}

		/// <summary>Gets the items in the tabstrip.</summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RadEditItemsAction]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        [RadNewItem("", false, false, false)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		/// <summary>Gets the toolstrip parent (a ToolStripManager instnace).</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripManager ParentToolStripManager
		{
			get
			{
				if (this.parentToolStripManager == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripManager == null; res = res.Parent)
					{
						this.parentToolStripManager = res as RadToolStripManager;
					}
				}
				return this.parentToolStripManager;
			}
		}

		internal bool HasBorder()
		{
			return this.border.Visibility != ElementVisibility.Collapsed;
		}
        /// <summary>
        /// Creates child elements
        /// </summary>
		protected override void CreateChildElements()
		{
			this.stripLayout = new ToolStripElementLayout();
			this.stripLayout.Orientation = Orientation.Horizontal;
			this.stripLayout.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.stripLayout.Class = "ToolStripElementLayout";
		
			this.border = new BorderPrimitive();

			this.stripFill = new FillPrimitive();
			this.stripFill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.stripFill.Class = "ToolStripElementFill";

			this.Margin = new Padding(this.Margin.Left, 1, this.Margin.Right, this.Margin.Bottom);
			this.Children.Add(this.stripLayout);

			border.Class = "ToolStripElementBorder";

			border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.Children.Add(border);

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadToolStripItem) };
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
			this.items.Owner = stripLayout;
		}

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = base.GetPreferredSizeCore(proposedSize);
            
            Orientation orientation = this.Orientation;
            RadToolStripManager tsm = this.ParentToolStripManager;
            if (tsm != null)
                orientation = tsm.Orientation;
            
            if (orientation == Orientation.Horizontal)
                res.Width = proposedSize.Width - this.LayoutEngine.GetBorderSize().Width - this.Margin.Horizontal;
            else
                res.Height = proposedSize.Height - this.LayoutEngine.GetBorderSize().Height - this.Margin.Vertical;

            return res;
        }
	}
}
