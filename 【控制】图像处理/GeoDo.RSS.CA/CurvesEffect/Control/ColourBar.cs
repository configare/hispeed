using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.CA
{
    public class ColourBar : UserControl
    {
        private LinearGradientMode _linearGradientMode = LinearGradientMode.Horizontal;
        private Color _color1 = Color.Black;
        private Color _color2 = Color.White;
        private Pen _rectPen = new Pen(Color.FromArgb(255, 128, 128, 128), 1f);

        public ColourBar()
        { }

        public LinearGradientMode LinearGradientMode
        {
            get { return _linearGradientMode; }
            set
            {
                _linearGradientMode = value;
                this.Invalidate();
            }
        }

        public Color Color1
        {
            get { return _color1; }
            set
            {
                _color1 = value;
                this.Invalidate();
            }
        }

        public Color Color2
        {
            get { return _color2; }
            set 
            {
                _color2 = value;
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Rectangle _workRect = ClientRectangle; //e.ClipRectangle;
            LinearGradientBrush leftBrush = new LinearGradientBrush(_workRect, _color1, _color2, _linearGradientMode);
            g.FillRectangle(leftBrush, _workRect);
            g.DrawRectangle(_rectPen, _workRect);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if(_rectPen!=null)
                _rectPen.Dispose();
            base.Dispose(disposing);
        }
    }
}
