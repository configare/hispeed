using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class VisualElementLayoutPart : IVisualLayoutPart
    {
        protected RectangleF bounds;
        protected SizeF desiredSize;
        protected LightVisualElement owner;
        protected Padding padding;
        protected Padding margin;

        public Padding Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        

        public VisualElementLayoutPart(LightVisualElement owner)
        {
            this.owner = owner;
        }

        public LightVisualElement Owner
        {
            get { return this.owner; }
        }

        public RectangleF Bounds
        {
            get { return this.bounds; }
        }

        public SizeF DesiredSize
        {
            get { return this.desiredSize; }
            set {  this.desiredSize=value; }
        }

        public virtual SizeF Measure(SizeF availableSize)
        {
            return SizeF.Empty;
        }

        public virtual SizeF Arrange(RectangleF bounds)
        {
            this.bounds = bounds;
            return new SizeF( this.bounds.Size );
        }
    }
}
