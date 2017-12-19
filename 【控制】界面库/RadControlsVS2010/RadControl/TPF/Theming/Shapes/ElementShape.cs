using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace Telerik.WinControls
{

    /// <summary>Represents element shape. Base class for specialized shapes such as 
    /// EllipseShape, RoundRectShape, Office12Shape, etc. </summary>
    [TypeConverter(typeof(ElementShapeConverter))]
	[Editor(typeof(ElementShapeEditor), typeof(UITypeEditor))]
    public abstract class ElementShape: Component
	{
		GraphicsPath shape;
		GraphicsPath contour;

        public ElementShape()
        {
        }

        public ElementShape(IContainer container)
        {
            container.Add(this);
        }

        /// <summary>
        /// Retrieves the shape of the element. GraphicsPath represents a series of connected
        /// lines and curves.
        /// </summary>
		public GraphicsPath GetElementShape(RadElement element)
		{
			Rectangle bounds = GetBounds(element);
			return CreatePath(bounds);
		}

        /// <summary>
        /// Retrieves the contour of the element0. GraphicsPath represents a series of
        /// connected lines and curves.
        /// </summary>
		public GraphicsPath GetElementContour(RadElement element)
		{
			Rectangle bounds = GetBounds(element);
			return CreateContour(bounds);
		}

        public GraphicsPath GetElementContour(Rectangle bounds)
        {
            return CreateContour(bounds);
        }

        public virtual Region CreateRegion(Rectangle bounds)
        {
            GraphicsPath path = this.CreatePath(bounds);
            Region region = new Region(path);
            path.Dispose();

            return region;
        }

		protected virtual Rectangle GetBounds(RadElement element)
		{
			Rectangle bounds = element.Bounds;
			bounds.Location = new Point(0, 0);
			bounds.Size = Size.Subtract(element.Size, new Size(1, 1));
			return bounds;
		}

        /// <summary>Creates path using a rectangle for bounds.</summary>
		public abstract GraphicsPath CreatePath(Rectangle bounds);

        /// <summary>Creates path using a rectangle for bounds.</summary>
        public virtual GraphicsPath CreatePath(RectangleF bounds)
        {
            return this.CreatePath(Rectangle.Round(bounds));
        }

        protected virtual GraphicsPath CreateContour(Rectangle bounds)
		{
			// TODO: hack for ribbonbar tabstrip!
			//bounds.Size = Size.Add(bounds.Size, new Size(1, 1));
			return CreatePath(bounds);
		}

        /// <summary>
        /// Serializes properties. Required for serialization mechanism of telerik
        /// framework.
        /// </summary>
        public virtual string SerializeProperties()
        {
            return string.Empty;
        }
        /// <summary>
        /// Deserializes properties. Required for the deserialization mechanism of telerik
        /// framework.
        /// </summary>
        public virtual void DeserializeProperties(string propertiesString)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (shape != null)
                {
                    shape.Dispose();
                    shape = null;
                }
                if (contour != null)
                {
                    contour.Dispose();
                    contour = null;
                }
            }

            base.Dispose(disposing);
        }
	}
}