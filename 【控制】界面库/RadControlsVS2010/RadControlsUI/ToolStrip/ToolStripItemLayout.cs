using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
	public class ToolStripItemLayout : StripLayoutPanel
	{
		private Stack<RadElement> collapsedItems;

		private RadToolStripItem parentToolStripItem;
		private RadToolStripElement parentToolStripElement;
		private RadToolStripManager parentToolStripManager;


		public static RadProperty IsGripElementProperty = RadProperty.Register(
		  "IsGripElementProperty", typeof(bool), typeof(ToolStripItemLayout), new RadElementPropertyMetadata(
			  false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty IsOverFlowButtonElementProperty = RadProperty.Register(
		  "IsOverFlowButtonElementProperty", typeof(bool), typeof(ToolStripItemLayout), new RadElementPropertyMetadata(
			  false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		protected override void InitializeFields()
        {
            base.InitializeFields();

            collapsedItems = new Stack<RadElement>();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Stack<RadElement> CollapsedItems
		{
			get
			{
				return this.collapsedItems;
			}
		}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RadToolStripElement ParentToolStripElement
		{
			get
			{
				if (this.parentToolStripElement == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripElement == null; res = res.Parent)
					{
						this.parentToolStripElement = res as RadToolStripElement;
					}
				}
				return this.parentToolStripElement;
			}
			set
			{
				this.parentToolStripElement = value;

			}
		}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal RadToolStripManager ParentToolStripManager
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
			set
			{
				if (value != null)
				{
					if ((this.parentToolStripManager == null)
						&& (value != null))
					{
						
					}

					this.parentToolStripManager = value;
				}
			}
		}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal RadToolStripItem ParentToolStripItem
		{
			get
			{
				if (this.parentToolStripItem == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripItem == null; res = res.Parent)
					{
						this.parentToolStripItem = res as RadToolStripItem;
					}
				}
				return this.parentToolStripItem;
			}
			set
			{
				this.parentToolStripItem = value;
			}
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == OrientationProperty)
			{
				if ((Orientation)e.NewValue == Orientation.Vertical)
				{
					foreach (RadElement child in this.Children)
					{
                        child.Alignment = ContentAlignment.TopLeft;
                        child.AngleTransform = 90;
                        SetImagePrimitivesAngleTransform(child, -90);

                        if (child as RadComboBoxElement != null)
                            child.Visibility = ElementVisibility.Collapsed;
                        if (child as RadDropDownButtonElement != null)
                            child.Visibility = ElementVisibility.Collapsed;
                        if (child as RadTextBoxElement != null)
                            child.Visibility = ElementVisibility.Collapsed;
					}
				}
				else
				{
					foreach (RadElement child in this.Children)
					{
                        child.Alignment = ContentAlignment.MiddleLeft;
                        child.AngleTransform = 0;
                        SetImagePrimitivesAngleTransform(child, 0);

                        if (child as RadComboBoxElement != null)
                            child.Visibility = ElementVisibility.Visible;
                        if (child as RadDropDownButtonElement != null)
                            child.Visibility = ElementVisibility.Visible;
                        if (child as RadTextBoxElement != null)
                            child.Visibility = ElementVisibility.Visible;
					}
				}
			}

			base.OnPropertyChanged(e);
		}

        private void SetImagePrimitivesAngleTransform(RadElement item, float angleTransform)
        {
            if (item.Children.Count > 0)
            {
                foreach (RadElement element in item.Children)
                {
                    if (element is ImagePrimitive)
                        element.AngleTransform = angleTransform;
                    else
                        SetImagePrimitivesAngleTransform(element, angleTransform);
                }
            }
        }

		internal bool HasAnyCollapsedItems()
		{
			if (this.ParentToolStripElement != null)
			{
				foreach (RadToolStripItem item in this.ParentToolStripElement.Items)
				{
					foreach (RadItem childItem in item.Items)
					{
						if (childItem.Visibility == ElementVisibility.Collapsed)
							return true;
					}
				}
			}
			return false;
		}
	}
}
