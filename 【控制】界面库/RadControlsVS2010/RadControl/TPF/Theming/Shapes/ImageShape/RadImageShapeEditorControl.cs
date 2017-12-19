using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System.ComponentModel;

namespace Telerik.WinControls
{
    [ToolboxItem(false)]
    public class RadImageShapeEditorControl : Control
    {
        #region Fields

        private RadImageShape shape;
        private static Size gripSize = new Size(17, 17);

        #endregion

        #region Constructor

        public RadImageShapeEditorControl()
            : this(null)
        {
        }

        public RadImageShapeEditorControl(RadImageShape shape)
        {
            this.shape = shape;
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
        }

        #endregion

        #region Properties

        public RadImageShape Shape
        {
            get
            {
                return this.shape;
            }
        }

        #endregion

        #region Overrides

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle client = this.GetClientRect();

            if (this.shape != null)
            {
                //apply 5 pixels padding for the shape
                Rectangle shapeRect = client;
                shapeRect.Inflate(-5, -5);
                shapeRect.Width -= gripSize.Width;
                shapeRect.Height -= gripSize.Height;

                this.shape.Paint(e.Graphics, shapeRect);
            }

            ControlPaint.DrawSizeGrip(e.Graphics, Color.Gray, this.GetGripRect());

            //draw border around the control
            //GDI+ DrawRectangle fix
            client.Width -= 1;
            client.Height -= 1;
            e.Graphics.DrawRectangle(Pens.Black, client);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    Point mousePos = this.PointToClient(Control.MousePosition);
                    if (this.GetGripRect().Contains(mousePos))
                    {
                        m.Result = (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    }
                    break;
            }
        }

        #endregion

        #region Private Implementation

        private Rectangle GetClientRect()
        {
            Rectangle client = this.ClientRectangle;
            return LayoutUtils.DeflateRect(client, this.Padding);
        }

        private Rectangle GetGripRect()
        {
            Rectangle client = this.GetClientRect();
            return new Rectangle(client.Right - gripSize.Width, client.Bottom - gripSize.Height, gripSize.Width, gripSize.Height);
        }

        #endregion
    }
}
