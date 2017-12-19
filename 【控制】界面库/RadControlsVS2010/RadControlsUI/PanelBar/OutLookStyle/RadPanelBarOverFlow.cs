using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace Telerik.WinControls.UI
{
	public class RadPanelBarOverFlow : RadItem
	{
		internal BoxLayout stripLayout;
		private FillPrimitive overFlowFill;
		private BorderPrimitive overFlowBorder;
		private RadItemOwnerCollection items;
		private RadDropDownButtonElement dropDownButton;
		private RadPanelBarElement panelBarElement;

		public RadPanelBarElement ParentPanelBarElement
		{
			get
			{
				if (this.panelBarElement == null)
				{
					for (RadElement res = this.Parent; res != null && panelBarElement == null; res = res.Parent)
					{
						panelBarElement = res as RadPanelBarElement;
					}
				}
				return this.panelBarElement;
			}
		}

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);

            this.MinSize = new System.Drawing.Size(0, 20);
            this.UseNewLayoutSystem = true;
        }

		public RadDropDownButtonElement DropDownButton
		{
			get
			{
				return this.dropDownButton;
			}
		}

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			target.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			target.StretchHorizontally = false;
		}


		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public override System.Drawing.Size GetPreferredSizeCore(System.Drawing.Size proposedSize)
		{
			this.stripLayout.SetBounds(new Rectangle(this.stripLayout.Location, this.Size));
			return base.GetPreferredSizeCore(proposedSize);
		}


		protected override void CreateChildElements()
		{
			DockLayoutPanel parentStripLayout = new DockLayoutPanel();
		
			this.stripLayout = new BoxLayout();
			this.stripLayout.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.stripLayout.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.stripLayout.Class = "overFlowLayout";

			this.overFlowBorder = new BorderPrimitive();
			this.overFlowBorder.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.overFlowBorder.Class = "overFlowBorder";

			this.overFlowFill = new FillPrimitive();
			this.overFlowFill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.overFlowFill.Class = "overFlowFill";
			this.overFlowFill.MinSize = new System.Drawing.Size(20, 20);

			this.dropDownButton = new RadDropDownButtonElement();

			this.Children.Add(this.overFlowFill);
			this.Children.Add(this.overFlowBorder);
			this.Children.Add(parentStripLayout);
		
			this.dropDownButton.ArrowButton.Arrow.AutoSize = true;
			this.stripLayout.RightToLeft = true;
			this.dropDownButton.StretchHorizontally = false;
			this.dropDownButton.StretchVertically = false;
			this.dropDownButton.Alignment = ContentAlignment.MiddleRight;
			this.dropDownButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dropDownButton.MinSize = new Size(16, 16);
            this.dropDownButton.ArrowButton.MinSize = new Size(16, 16);
            this.dropDownButton.SetValue(DockLayoutPanel.DockProperty, Dock.Right);
			parentStripLayout.Children.Add(this.dropDownButton);
			parentStripLayout.Children.Add(this.stripLayout);

			this.items.Owner = this.stripLayout;
		}
	}
}
