using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Themes.GradientDialog
{
    /// <exclude/>
	[ToolboxItemAttribute(false)]
    public partial class GradientAngleControl : UserControl
    {
        private double m_gradientAngle;
        public GradientAngleControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        }

        private void GradientAngleControl_Load(object sender, EventArgs e)
        {

        }

        public double GradientAngle
        {
            get
            {
                return m_gradientAngle;  
            }
            set
            {
                m_gradientAngle = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rec = new Rectangle(ClientRectangle.X + ClientRectangle.Width / 4 + ClientRectangle.Width / 8, ClientRectangle.Y + ClientRectangle.Height / 4 + ClientRectangle.Height / 8, ClientRectangle.Width / 4, ClientRectangle.Height / 4);
            using (Pen pen = new Pen(this.ForeColor))
            {
                using (Brush brush = new SolidBrush(this.BackColor))
                {
                    g.FillEllipse(brush, rec);
                    g.DrawEllipse(pen, new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1));
                    double angle = GradientAngle * Math.PI / 180;
                    int length = this.Width * 45 / 100;
                    int x1 = this.Width / 2;
                    int y1 = this.Height / 2;
                    int x2 = (int)(x1 + length * Math.Sin(angle));
                    int y2 = (int)(y1 - length * Math.Cos(angle));
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
            base.OnPaint(e);
        }
    }
}
