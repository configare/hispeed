using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class SizeGripItem : RadItem
    {
        public enum SizingModes
        {
            Horizontal,
            Vertical,
            Both
        };

        internal static SizeGripItem activeGrip;
        private bool allowSizing = true;
        private FillPrimitive fill;
        private BorderPrimitive border;
        private ImagePrimitive image;
        private Point mouseDown;
        private Point oldMousePos;
        private Size downSize;
        private Point downPos;
        private Rectangle downRect;
        private SizingModes sizingMode = SizingModes.Both;
        private bool shouldAspectRootElement = true;

        

        public event ValueChangingEventHandler Sized;
        public event ValueChangingEventHandler Sizing;

        protected virtual void OnSized(object sender, ValueChangingEventArgs args)
        {
            if (this.Sized != null)
            {
                this.Sized(this, args);
            }
        }

        protected virtual void OnSizing(object sender, ValueChangingEventArgs args)
        {
            if (this.Sizing != null)
            {
                this.Sizing(this, args);
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that
        /// determines whether the SizeGripItem
        /// can resize the hosting control.
        /// </summary>
        public bool AllowSizing
        {
            get
            {
                return this.allowSizing;
            }
            set
            {
                this.allowSizing = value;
            }
        }

        public ImagePrimitive Image
        {
            get
            {
                return this.image;
            }
        }

        [DefaultValue(SizeGripItem.SizingModes.Both)]
        public SizingModes SizingMode
        {
            get
            {
                return sizingMode;
            }
            set
            {
                sizingMode = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool ShouldAspectRootElement
        {
            get { return shouldAspectRootElement; }
            set { shouldAspectRootElement = value; }
        }

        protected override void CreateChildElements()
        {			
            fill = new FillPrimitive();
            fill.Class = "GripItemFill";
            this.Children.Add(fill);

            border = new BorderPrimitive();
            border.Class = "GripItemBorder";
            this.Children.Add(border);

            image = new ImagePrimitive();
            image.MinSize = new Size(8, 7);
            this.Children.Add(image);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!this.allowSizing)
            {
                return;
            }

            mouseDown = Control.MousePosition;            
            this.OnSizing(this, new ValueChangingEventArgs(mouseDown));            
            oldMousePos = mouseDown;
            downPos = this.ElementTree.Control.PointToScreen(Point.Empty);
            downSize = this.ElementTree.Control.Size;
            downRect = new Rectangle(downPos, downSize);
            this.Capture = true;
            activeGrip = this;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!this.allowSizing)
            {
                return;
            }

            if (e.Button != MouseButtons.Left || activeGrip != this)
            {
                return;
            }

            Point mousePos = Control.MousePosition;
            if (oldMousePos == mousePos)
            {
                return;
            }

            int dx = sizingMode == SizingModes.Vertical ? 0 : Control.MousePosition.X - mouseDown.X;
            int dy = sizingMode == SizingModes.Horizontal ? 0 : Control.MousePosition.Y - mouseDown.Y;

            if (IsSizerAtBottom())
            {
                if (this.RightToLeft)
                {
                    SizeControlBottomLeft(dx, dy);
                }
                else
                {
                    SizeControlBottomRight(dx, dy);
                }
            }
            else
            {
                if (this.RightToLeft)
                {
                    SizeControlTopLeft(dx, dy);
                }
                else
                {
                    SizeControlTopRight(dx, dy);
                }
            }

            oldMousePos = Control.MousePosition;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.ElementTree.Control.Cursor = Cursors.Arrow;
            this.Capture = false;
            if (!this.allowSizing)
            {
                return;
            }

            if (activeGrip == this)
            {
                activeGrip = null;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (sizingMode == SizingModes.Vertical)
            {
                this.ElementTree.Control.Cursor = Cursors.SizeNS;
            }
            else if (IsSizerAtBottom())
            {
                this.ElementTree.Control.Cursor = !this.RightToLeft ? Cursors.SizeNWSE : Cursors.SizeNESW;
            }
            else
            {
                this.ElementTree.Control.Cursor = !this.RightToLeft ? Cursors.SizeNESW : Cursors.SizeNWSE;
            }

            if (!this.allowSizing)
            {
                base.OnMouseEnter(e);
                return;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.ElementTree.Control.Cursor = Cursors.Arrow;
            
            if (!this.allowSizing)
            {
                base.OnMouseLeave(e);
                return;
            }
			
            base.OnMouseLeave(e);
        }

        public override bool RightToLeft
        {
            get
            {                
                return base.RightToLeft;
            }
            set
            {
                //base.RightToLeft = value;
                //this.Alignment = !value ? ContentAlignment.BottomRight : ContentAlignment.BottomLeft;               
                this.AngleTransform = !value ? 0 : 90;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the sizing element is at the bottom of the window.
        /// If true, the size of the popup should increase. If false, the size should decrease. 
        /// </summary>
        /// <returns></returns>
        private bool IsSizerAtBottom()
        {
            if (this.UseNewLayoutSystem)
                return DockLayoutPanel.GetDock(this.Parent) == Dock.Bottom;
            else
                return this.Parent.Alignment == ContentAlignment.BottomCenter;
        }

        private Size GetMinControlSize()
        {
            if (!this.IsInValidState(true))
            {
                return Size.Empty;
            }
            // TODO: use DesiredSize when ONLY the new layout system is used
            Size res = this.Parent.Children[3].Size;
            if (this.UseNewLayoutSystem && shouldAspectRootElement)
            {
                res = Size.Round(this.ElementTree.RootElement.DesiredSize); //this.Parent.DesiredSize
            }

            res.Width = Math.Max(res.Width, this.ElementTree.Control.MinimumSize.Width);
            res.Width = Math.Max(res.Width, 0);
            res.Height = Math.Max(res.Height, this.ElementTree.Control.MinimumSize.Height);
            res.Height = Math.Max(res.Height, 0);

            return res;
        }

        private void SizeControlTopLeft(int dx, int dy)
        {
            Size testSize = this.downSize;
            testSize.Width -= dx;
            testSize.Height -= dy;

            Size minSize = GetMinControlSize();
            Size maxSize = this.ElementTree.Control.MaximumSize;

            if (minSize.Width > 0 && testSize.Width < minSize.Width)
                dx = this.downRect.Width - minSize.Width;
            if (maxSize.Width > 0 && testSize.Width > maxSize.Width)
                dx = this.downRect.Width - maxSize.Width;

            if (minSize.Height > 0 && testSize.Height < minSize.Height)
                dy = this.downRect.Height - minSize.Height;
            if (maxSize.Height > 0 && testSize.Height > maxSize.Height)
                dy = this.downRect.Height - maxSize.Height;

            Rectangle newRect = this.downRect;
            newRect.X += dx;
            newRect.Y += dy;
            newRect.Width -= dx;
            newRect.Height -= dy;

            this.ElementTree.Control.Bounds = newRect;
            this.OnSized(this, new ValueChangingEventArgs(new SizeF(dx, dy)));
        }

        private void SizeControlTopRight(int dx, int dy)
        {
            Size testSize = this.downSize;
            testSize.Width += dx;
            testSize.Height -= dy;

            Size minSize = GetMinControlSize();
            Size maxSize = this.ElementTree.Control.MaximumSize;

            if (minSize.Width > 0 && testSize.Width < minSize.Width)
                testSize.Width = minSize.Width;
            if (maxSize.Width > 0 && testSize.Width > maxSize.Width)
                testSize.Width = maxSize.Width;

            if (minSize.Height > 0 && testSize.Height < minSize.Height)
                dy = this.downRect.Height - minSize.Height;
            if (maxSize.Height > 0 && testSize.Height > maxSize.Height)
                dy = this.downRect.Height - maxSize.Height;

            Rectangle newRect = this.downRect;
            newRect.Y += dy;
            newRect.Width = testSize.Width;
            newRect.Height -= dy;

            this.ElementTree.Control.Bounds = newRect;
            this.OnSized(this, new ValueChangingEventArgs(new SizeF(dx, dy)));
        }

        private void SizeControlBottomLeft(int dx, int dy)
        {
            Size testSize = this.downSize;
            testSize.Width -= dx;
            testSize.Height += dy;

            Size minSize = GetMinControlSize();
            Size maxSize = this.ElementTree.Control.MaximumSize;

            if (minSize.Width > 0 && testSize.Width < minSize.Width)
                dx = this.downRect.Width - minSize.Width;
            if (maxSize.Width > 0 && testSize.Width > maxSize.Width)
                dx = this.downRect.Width - maxSize.Width;

            if (minSize.Height > 0 && testSize.Height < minSize.Height)
                testSize.Height = minSize.Height;
            if (maxSize.Height > 0 && testSize.Height > maxSize.Height)
                testSize.Height = maxSize.Height;

            Rectangle newRect = this.downRect;
            newRect.X += dx;
            newRect.Width -= dx;
            newRect.Height = testSize.Height;

            this.ElementTree.Control.Bounds = newRect;
            this.OnSized(this, new ValueChangingEventArgs(new SizeF(dx, dy)));
        }

        private void SizeControlBottomRight(int dx, int dy)
        {
            Rectangle newRect = this.downRect;
            newRect.Width += dx;
            newRect.Height += dy;

            Size minSize = GetMinControlSize();
            Size maxSize = this.ElementTree.Control.MaximumSize;

            if (minSize.Width > 0 && newRect.Width < minSize.Width)
                newRect.Width = minSize.Width;
            if (maxSize.Width > 0 && newRect.Width > maxSize.Width)
                newRect.Width = maxSize.Width;

            if (minSize.Height > 0 && newRect.Height < minSize.Height)
                newRect.Height = minSize.Height;
            if (maxSize.Height > 0 && newRect.Height > maxSize.Height)
                newRect.Height = maxSize.Height;

            this.ElementTree.Control.Bounds = newRect;
            this.OnSized(this, new ValueChangingEventArgs(new SizeF(dx, dy)));
        }
    }
}
