using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layout;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	public class RadGalleryMenuLayoutPanel : LayoutPanel
	{
		private FillPrimitive leftFillPrimitive;
		private FillPrimitive fillPrimitive;
		private BorderPrimitive borderPrimitive;
		private StackLayoutPanel leftLayoutPanel;
        private StackLayoutPanel bottomStripLayoutPanel;
        private WrapLayoutPanel wrapperLayoutPanel;

		internal static readonly int minImageWidth = 20;

		private const int shortcutTextInternalMargin = 4;


        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }
		
        public RadElement LayoutPanel
		{
			get
			{
				return this.leftLayoutPanel;
			}
		}

		public BorderPrimitive Border
		{
			get
			{
				return this.borderPrimitive;
			}
		}

		private bool defaultBehavior = true;
		public bool DefaultBehavior
		{
			get { return this.defaultBehavior; }
			set { this.defaultBehavior = value; } 
		}

		protected override void CreateChildElements()
		{
			this.leftFillPrimitive = new FillPrimitive();
			this.leftFillPrimitive.Class = "RadSubMenuPanelFillPrimitive";
			this.leftFillPrimitive.ZIndex = 1;

			this.leftLayoutPanel = new StackLayoutPanel();
			this.leftLayoutPanel.Orientation = Orientation.Vertical;			
			this.leftLayoutPanel.EqualChildrenWidth = true;
			ForceSizeLayoutPanel force1 = new ForceSizeLayoutPanel();
			force1.ForceChildWidthToParent = true;
			force1.Children.Add(leftLayoutPanel);

			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.borderPrimitive.Class = "RadSubMenuPanelBorderPrimitive";;

			this.fillPrimitive = new FillPrimitive();
			this.fillPrimitive.Class = "RadSubMenuPanelBackFillPrimitive";;
			this.fillPrimitive.BackColor = Color.White;
			this.fillPrimitive.GradientStyle = GradientStyles.Solid;

            this.wrapperLayoutPanel = new WrapLayoutPanel();
			this.wrapperLayoutPanel.Orientation = Orientation.Vertical;
			this.wrapperLayoutPanel.ZIndex = 2;

            this.bottomStripLayoutPanel = new StackLayoutPanel();
			this.bottomStripLayoutPanel.MinSize = new Size(0, 10);
			this.bottomStripLayoutPanel.Visibility = ElementVisibility.Collapsed;
			this.bottomStripLayoutPanel.Orientation = Orientation.Horizontal;
			ForceSizeLayoutPanel force2 = new ForceSizeLayoutPanel();
			force2.ForceChildWidthToParent = true;
			force2.Children.Add(bottomStripLayoutPanel);

			this.wrapperLayoutPanel.Children.Add(force1);
			this.wrapperLayoutPanel.Children.Add(force2);

			this.Children.Add(this.fillPrimitive);
			this.Children.Add(this.leftFillPrimitive);
			this.Children.Add(wrapperLayoutPanel);
			this.Children.Add(this.borderPrimitive);
		}

		public override void PerformLayoutCore(RadElement affectedElement)
		{
			base.PerformLayoutCore(affectedElement);

			if (this.defaultBehavior)
			{
				Size prefSize = this.wrapperLayoutPanel.GetPreferredSize(this.AvailableSize);
				this.wrapperLayoutPanel.Size = new Size(this.Parent.FieldSize.Width - (int)this.borderPrimitive.BorderSize.Width, prefSize.Height);
				this.leftLayoutPanel.ChildrenForcedSize = new Size(this.wrapperLayoutPanel.Size.Width, this.leftLayoutPanel.Size.Height);
			}
		}
	}
}
