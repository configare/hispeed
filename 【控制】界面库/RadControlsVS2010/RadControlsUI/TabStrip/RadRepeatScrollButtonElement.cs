using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.UI.TabStrip;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents a repeat scroll button element, and like all elements, 
    /// can be nested in other telerik controls. The RadRepeatScrollButton is a 
    /// simple wrapper for the RadRepeatScrollButtonElement class. The former acts to
    /// transfer events to and from its RadRepeatScrollButtonElement instance. 
    /// All logical and graphical functionality is implemented in the 
    /// RadRepeatScrollButtonElement class.</summary>
	[RadToolboxItem(false)]
    public class RadRepeatScrollButtonElement : RadRepeatButtonElement
    {
        public ArrowPrimitive arrowPrimitive;

        /// <summary>Initializes a new instance of the RadRepeatButtonElement class.</summary>
        public RadRepeatScrollButtonElement()
		{
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ClipDrawing = true;
        }

        /// <summary>
        /// Initializes a new instance of the RadRepeatButtonElement class using scroll
        /// button direction.
        /// </summary>
        public RadRepeatScrollButtonElement(ScrollButtonDirection direction)
		{
            this.Direction = direction;
		}

        /// <summary>
        ///     Gets or sets the <see cref="ScrollButtonDirection">scroll button direction</see>:
        ///     Left, Up, Right, and Down.
        /// </summary>
        public ScrollButtonDirection Direction
        {
            get
            {
                return (ScrollButtonDirection)this.arrowPrimitive.Direction;
            }
            set
            {
                this.arrowPrimitive.Direction = (ArrowDirection)value;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
                     
            Rectangle rangeRectangle = new Rectangle(-100,
                -100,
                 200,
                 200 );

            if (rangeRectangle.Contains(e.Location) && e.Button == MouseButtons.Left)
            {
                this.OnMouseDown(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Capture = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.Capture = false;
        }

        private void FillList(List<PreferredSizeData> list)
        {
            foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
            {
                list.Add(new PreferredSizeData(child, this.Size));
            }
        }

        protected override void OnTimeout(object sender, EventArgs e)
        {
            base.OnTimeout(sender, e);
            if (base.IsPressed)
            {
                MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 1,
                    0, 0, 0);

                this.DoMouseDown(args);
            }
        }

        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.PerformLayout" filter=""/>
        public override void PerformLayoutCore(RadElement affectedElement)
        {
            List<PreferredSizeData> list = new List<PreferredSizeData>();
            FillList(list);

            foreach (PreferredSizeData data in list)
            {
                if (data.Element != this.arrowPrimitive)
                {
                    data.Element.Size = this.Size;
                    continue;
                }

                int sizeValue = Math.Min(this.Size.Width, this.Size.Height);
                int newSize = sizeValue / 3;
                this.arrowPrimitive.Size = new Size(newSize, newSize);
            }

        }
        /// <commentsfrom cref="Telerik.WinControls.Layouts.IRadLayoutElement.GetPreferredSize" filter=""/>
        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            return new Size(this.arrowPrimitive.Size.Width * 3, this.Parent.Size.Height);
        }

        protected override void CreateChildElements()
        {
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.ZIndex = 0;
            this.fillPrimitive.Class = "ScrollButtonFill";
            this.Children.Add(this.fillPrimitive);

            this.arrowPrimitive = new ArrowPrimitive();
            this.arrowPrimitive.Alignment = ContentAlignment.MiddleCenter;
            this.arrowPrimitive.ZIndex = 1;
            this.arrowPrimitive.Class = "ScrollButtonArrow";
            this.Children.Add(this.arrowPrimitive);

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RepeatScrollButtonBorder";
            this.borderPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.Children.Add(this.borderPrimitive);
        }
    }
}
