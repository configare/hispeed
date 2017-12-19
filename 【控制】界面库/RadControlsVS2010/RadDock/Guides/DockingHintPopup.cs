using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a RadLayeredWindow, used by a DragDropService to 
    /// display a semi-transparent hint about where a dock pane is about to be dropped.
    /// </summary>
    internal class DockingHintPopup : RadLayeredWindow
    {
        #region Constructor

        public DockingHintPopup()
        {
            this.TopMost = true;
            this.HitTestable = false;
        }

        #endregion

        #region Implementation

        public void Initialize(IDockingGuidesTemplate template)
        {
            this.fillColor = template.DockingHintBackColor;
            this.borderColor = template.DockingHintBorderColor;
            this.borderWidth = template.DockingHintBorderWidth;
        }

        protected override void PaintWindow(Graphics g, Bitmap graphicsBitmap)
        {
            //we will display a semi-transparent rectangle with outline
            Rectangle client = new Rectangle(0, 0, Width, Height);

            //apply filling
            SolidBrush fill = new SolidBrush(this.fillColor);
            g.FillRectangle(fill, client);
            fill.Dispose();

            Pen pen = new Pen(this.borderColor, this.borderWidth);
            pen.Alignment = PenAlignment.Inset;
            g.DrawRectangle(pen, client);
            pen.Dispose();

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Color to be used to fill the docking hint rectangle.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                if (this.borderColor == value)
                {
                    return;
                }

                this.borderColor = value;
                if (this.Updated)
                {
                    this.UpdateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Color to be used to fill the docking hint rectangle.
        /// </summary>
        public Color FillColor
        {
            get
            {
                return this.fillColor;
            }
            set
            {
                if (this.fillColor == value)
                {
                    return;
                }

                this.fillColor = value;
                if (this.Updated)
                {
                    this.UpdateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the border, drawn around the window. Defaults to 2 pixels.
        /// </summary>
        public int BorderWidth
        {
            get
            {
                return this.borderWidth;
            }
            set
            {
                if (this.borderWidth == value)
                {
                    return;
                }

                this.borderWidth = Math.Max(1, value);
                if (this.Updated)
                {
                    this.UpdateWindow();
                }
            }
        }

        #endregion

        #region Fields

        private Color fillColor;
        private Color borderColor;
        private int borderWidth;

        #endregion
    }
}
