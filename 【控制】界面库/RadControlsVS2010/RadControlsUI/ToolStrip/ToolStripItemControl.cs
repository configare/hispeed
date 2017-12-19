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
using Telerik.WinControls.UI;
using System.Collections;
namespace Telerik.WinControls.UI
{
	[RadToolboxItem(false)]
    [ToolboxItem(false)]
	public class RadToolStripItemControl : RadControl
	{
		private RadToolStripContainterElement containerElement;
		private RadItemOwnerCollection items;

        protected override bool GetUseNewLayout()
        {
            return false;
        }

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
            if (width == 0)
            {
                width = this.containerElement.ContainerForm.Bounds.Width - 6;
            }

            if (height == 0)
            {
                height = this.containerElement.ContainerForm.Bounds.Height - 23;
            }

			base.SetBoundsCore(x, y, width, height, specified);

		}

		public RadToolStripItemControl(RadToolStripContainterElement container)
		{
			this.containerElement = container;
			this.containerElement.ContainerForm.SizeChanged += new EventHandler(ContainerForm_SizeChanged);
			this.items = container.Items;

			this.CausesValidation = false;

            this.CreateChildItems(this.RootElement);
		}

		[DefaultValue(false), Browsable(false)]
		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
			}
		}

		public override string ThemeClassName
		{
			get
			{
				return typeof(RadToolStrip).FullName;
			}
			set
			{
				base.ThemeClassName = value;
			}
		}

		private void ContainerForm_SizeChanged(object sender, EventArgs e)
		{
			this.SetBoundsCore(this.Location.X, this.Location.Y, (sender as Form).Bounds.Width - 6,
				(sender as Form).Bounds.Height - 23, BoundsSpecified.All);
		}

		public RadToolStripContainterElement ContainerElement
		{
			get
			{
				return this.containerElement;
			}
		}

		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[RadNewItem("Add New Item", false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadButtonElement))
                return true;
            else
                if (elementType == typeof(RadDropDownButtonElement))
                    return true;

            return base.ControlDefinesThemeForElement(element);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            if (this.containerElement == null)
            {
                return;
            }

            parent.Children.Add(this.containerElement);
            this.containerElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

        }
	}

	public class RadToolStripContainterElement : RadItem
	{
		private RadItemOwnerCollection items;
		private ToolStripFlowLayout parentPanel;
		private FillPrimitive fill;

		private Form containerForm;

		public FillPrimitive ToolStripContainterElementFill
		{
			get
			{
				return this.fill;
			}
		}

		public Form ContainerForm
		{
			get
			{
				return this.containerForm;
			}
		}

		internal ToolStripFlowLayout StackLayoutPanel
		{
			get
			{
				return this.parentPanel;
			}
		}

		public RadToolStripContainterElement(Form containerForm)
		{
			this.containerForm = containerForm;
            this.CreateChildElements();
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadButtonElement), typeof(RadTextBoxElement), typeof(RadComboBoxElement), 
				 typeof(RadToolStripLabelElement), typeof(RadToolStripSeparatorItem), typeof(RadDropDownButtonElement),		
					};
            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			target.Visibility = ElementVisibility.Visible;
		}

		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			Size preferredSize = base.GetPreferredSizeCore(proposedSize);

			preferredSize.Width = Math.Max(preferredSize.Width, this.AvailableSize.Width);

			return preferredSize;
		}

		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[RadNewItem("Add New Item", false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public void RefreshItems()
		{
			this.SuspendLayout();

			int i = 1;
			//changing Zedindex, when item is added
			foreach (RadElement item in this.Items)
			{
				item.ZIndex = this.items.Count - i;
				item.Visibility = ElementVisibility.Visible;
				i++;
			}
			this.ResumeLayout(true);

		}

		public new void Invalidate()
		{
			if (this.parentPanel != null && this.parentPanel.ElementState == ElementState.Loaded)
			{
				this.parentPanel.LayoutEngine.SetLayoutInvalidated(true);
				this.parentPanel.LayoutEngine.PerformParentLayout();
			}
		}

		protected override void CreateChildElements()
		{
            if (this.containerForm == null)
            {
                return;
            }

			this.parentPanel = new ToolStripFlowLayout(containerForm);
			this.parentPanel.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

			this.fill = new FillPrimitive();
			this.fill.AutoSize = false;

			this.Children.Add(fill);

			this.Children.Add(this.parentPanel);
			this.items.Owner = this.parentPanel;
		
			RefreshItems();
		}
	}
}
