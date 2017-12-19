using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents VS like tab shape. Shapes are series of connected lines and curves.</summary>
	[ToolboxItem(false), ComVisible(false)]
	public class TabVsShape : ElementShape
	{
        private bool rightToLeft = false;
        private bool closeFigure;
        private const char Separator = ';';

		/// <summary><para>Gets or sets the orientation of this shape.</para></summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
		public bool RightToLeft
		{
			get { return rightToLeft; }
			set { rightToLeft = value; }
		}

        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CloseFigure
        {
            get
            {
                return this.closeFigure;
            }
            set
            {
                this.closeFigure = value;
            }
        }

        /// <summary>
        /// Creates VS like tab shape. Overrides CreatePath method in its base class -
        /// ElementShape.
        /// </summary>
		public override GraphicsPath CreatePath(Rectangle bounds)
		{
            GraphicsPath path = new GraphicsPath();

            int roundLength;
            if (bounds.Width >= bounds.Height * 2)
            {
                roundLength = bounds.Height;
            }
            else
            {
                roundLength = bounds.Width / 2;
            }

			if (rightToLeft)
			{
				path.AddLine(new Point(bounds.X, bounds.Y + bounds.Height), new Point(bounds.X, bounds.Y + 2));
				path.AddLine(new Point(bounds.X + 2, bounds.Y), new Point(bounds.X + bounds.Width - roundLength - 2, bounds.Y));
				path.AddLine(new Point(bounds.X + bounds.Width - roundLength + 2, bounds.Y + 2), 
					new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height));
			}
			else
			{
				path.AddLine(new Point(bounds.X, bounds.Y + bounds.Height), new Point(bounds.X + roundLength - 2, bounds.Y + 2));
				path.AddLine(new Point(bounds.X + roundLength + 2, bounds.Y), new Point(bounds.X + bounds.Width - 2, bounds.Y));
				path.AddLine(new Point(bounds.X + bounds.Width, bounds.Y + 2), new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height));
			}

            if (this.closeFigure)
            {
                path.CloseAllFigures();
            }

			return path;
		}

		        /// <summary>Serializes properties. Required for telerik serialization mechanism.</summary>
        public override string SerializeProperties()
        {
            return this.rightToLeft.ToString() + Separator + this.closeFigure.ToString();
        }

        /// <summary>Deserializes properties. Required for telerik deserialization mechanism.</summary>
        public override void DeserializeProperties(string propertiesString)
        {
            if (string.IsNullOrEmpty(propertiesString))
            {
				return;
            }

            string[] tokens = propertiesString.Split(Separator);
            if (tokens.Length > 0)
            {
                this.rightToLeft = bool.Parse(tokens[0]);
            }

            if (tokens.Length > 1)
            {
                this.closeFigure = bool.Parse(tokens[1]);
            }
        }
	}
}