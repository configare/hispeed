using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;
using VisualStyles = System.Windows.Forms.VisualStyles;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    [Flags]
    public enum SizingMode
    {
        None = 0, 
        RightBottom = 1, 
        UpDown = 2, 
        UpDownAndRightBottom = RightBottom | UpDown
    }

	public class SizeGripElement : RadItem
	{
        private FillPrimitive fill;
        private BorderPrimitive border;
        private SizeGripItem gripItem;
        private SizeGripItem gripItem2;

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
            this.Visibility = ElementVisibility.Collapsed;
        }

		public SizingMode SizingMode
		{
			get 
			{
                if (gripItem.Visibility == ElementVisibility.Visible && gripItem2.Visibility == ElementVisibility.Visible)
					return SizingMode.UpDownAndRightBottom;
				else if (gripItem.Visibility == ElementVisibility.Visible)
					return SizingMode.UpDown;
				else if (gripItem2.Visibility == ElementVisibility.Visible)
					return SizingMode.RightBottom;
				else
					return SizingMode.None;
			}
			set
			{
                if ((value & SizingMode.RightBottom) ==	SizingMode.RightBottom)
					gripItem2.Visibility = ElementVisibility.Visible;
				else
					gripItem2.Visibility = ElementVisibility.Collapsed;

				if ((value & SizingMode.UpDown) == SizingMode.UpDown)
					gripItem.Visibility = ElementVisibility.Visible;
				else
					gripItem.Visibility = ElementVisibility.Collapsed;

				if (value == SizingMode.None)
					this.Visibility = ElementVisibility.Collapsed;
				else
					this.Visibility = ElementVisibility.Visible;
			}
		}


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool ShouldAspectRootElement
        {
            get
            {
                return this.GripItemNS.ShouldAspectRootElement && this.GripItemNSEW.ShouldAspectRootElement;
            }
            set
            {
                this.GripItemNS.ShouldAspectRootElement = this.GripItemNSEW.ShouldAspectRootElement = value;
            }
        }

		protected override void CreateChildElements()
		{
			fill = new FillPrimitive();
			fill.Class = "GripFill";
			this.Children.Add(fill);

			border = new BorderPrimitive();
			border.Class = "GripBorder";
			border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.Children.Add(border);

			gripItem = new SizeGripItem();
			gripItem.StretchHorizontally = false;
			gripItem.StretchVertically = false;
			gripItem.Class = "GripNS";
			gripItem.Image.Class = "GripNSImage";
			gripItem.SizingMode = SizeGripItem.SizingModes.Vertical;
			gripItem.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			gripItem.Alignment = ContentAlignment.BottomCenter;
			this.Children.Add(gripItem);

			gripItem2 = new SizeGripItem();
			gripItem2.StretchHorizontally = false;
			gripItem2.StretchVertically = false;
			gripItem2.Class = "GripNSEW";
			gripItem2.Image.Class = "GripNSEWImage";
			gripItem2.SizingMode = SizeGripItem.SizingModes.Both;
			gripItem2.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            gripItem2.RightToLeft = this.RightToLeft;
            gripItem2.Alignment = ContentAlignment.MiddleRight;
			this.Children.Add(gripItem2);
		}

        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
                this.gripItem2.RightToLeft = value;               
            }
        }

		public SizeGripItem GripItemNS
		{
			get
			{
				return this.gripItem;
			}
		}

		public SizeGripItem GripItemNSEW
		{
			get
			{
				return this.gripItem2;
			}
        }

        #region System skinning

        protected override bool ShouldPaintChild(RadElement element)
        {
            if ((object.ReferenceEquals(element, this.fill)
                || object.ReferenceEquals(element, this.border))
                && this.paintSystemSkin == true)
            {
                return false;
            }

            return base.ShouldPaintChild(element);
        }

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            this.fill.Visibility = ElementVisibility.Hidden;
            this.border.Visibility = ElementVisibility.Hidden;
        }

        protected override void UnitializeSystemSkinPaint()
        {
            base.UnitializeSystemSkinPaint();

            this.fill.Visibility = ElementVisibility.Visible;
            this.border.Visibility = ElementVisibility.Visible;
        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        public override System.Windows.Forms.VisualStyles.VisualStyleElement GetXPVisualStyle()
        {
            return VisualStyles.VisualStyleElement.Status.Bar.Normal;
        }

        #endregion
    }
}
