using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
	[ToolboxItem(false)]
	public class RadPanelBarContentControl : RadControl
	{
		private ItemsControlElement itemsControlElement;

		public RadPanelBarContentControl()
		{
			this.AutoSize = true;
            this.ThemeName = "ControlDefault";
        }

        public void ShowChildren()
		{
			foreach (RadElement element in this.itemsControlElement.GetBoxLayout().Children)
			{
				element.Visibility = ElementVisibility.Visible;
			}
		}

        public void HideChildren()
        {
            foreach (RadElement element in this.itemsControlElement.GetBoxLayout().Children)
            {
                element.Visibility = ElementVisibility.Collapsed;
            }
        }

		internal ItemsControlElement ItemsControlElement
		{
			get
			{
				return this.itemsControlElement;
			}
		}

        protected override void CreateChildItems(RadElement parent)
        {
            this.itemsControlElement = new ItemsControlElement();
            parent.Children.Add(this.itemsControlElement);
            base.CreateChildItems(parent);
        }

	}

	public class ItemsControlElement : RadItem
	{
		private BoxLayout verticalLayout;
	
		internal BoxLayout GetBoxLayout()
		{
			return this.verticalLayout;
		}

		protected override void CreateChildElements()
		{
			this.verticalLayout = new BoxLayout();
			this.verticalLayout.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.Children.Add(this.verticalLayout);
		}
	}
}
