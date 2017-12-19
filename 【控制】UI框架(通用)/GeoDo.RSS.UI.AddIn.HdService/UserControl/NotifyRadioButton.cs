using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class NotifyRadioButton : RadioButton
    {
        private int? _notifyMessage = 0;
        private Font _textFont = new Font("微软雅黑", 15);
        private Color _textColor = Color.FromArgb(200, 255, 0, 0);
        private Color _textLinkedColor = Color.FromArgb(200, 0, 0, 255);
        private Color textColor = Color.FromArgb(200, 255, 0, 0);
        Rectangle imgRectangle;
        RectangleF textRectangle;

        public event EventHandler NotifyClicked;

        public NotifyRadioButton()
            : base()
        {
            this.Appearance = System.Windows.Forms.Appearance.Button;
            this.Paint += new PaintEventHandler(NotifyRadioButton_Paint);
            this.MouseMove += new MouseEventHandler(NotifyRadioButton_MouseMove);
            this.MouseClick += new MouseEventHandler(NotifyRadioButton_MouseClick);
        }

        void NotifyRadioButton_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            if (textRectangle.IntersectsWith(new Rectangle(p, new Size(1, 1))))
            {
                if (NotifyClicked != null)
                    NotifyClicked(sender, new EventArgs());
            }
        }

        void NotifyRadioButton_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            if (textRectangle.IntersectsWith(new Rectangle(p, new Size(1, 1))))
            {
                _textColor = Color.FromArgb(255, 255, 0, 0);
                _textLinkedColor = Color.FromArgb(255, 0, 0, 255);
                textColor = Color.FromArgb(255, 255, 0, 0);
                this.textColor = _textLinkedColor;
                this.Cursor = Cursors.Hand;
            }
            else
            {
                _textColor = Color.FromArgb(200, 255, 0, 0);
                _textLinkedColor = Color.FromArgb(200, 0, 0, 255);
                textColor = Color.FromArgb(200, 255, 0, 0);
                this.textColor = _textColor;
                this.Cursor = Cursors.Default;
            }
            Invalidate();
        }

        public Font NotifyFont
        { 
            get{return _textFont;}
            set
            {
                _textFont = value;
                Invalidate();
            }
        }

        public int? NotifyMessage
        {
            get
            {
                return _notifyMessage;
            }
            set
            {
                _notifyMessage = value;
                Invalidate();
            }
        }
        
        void NotifyRadioButton_Paint(object sender, PaintEventArgs e)
        {
            if (_notifyMessage==null)
                return;
            string dr = _notifyMessage.ToString();
            Graphics g = e.Graphics;
            SizeF txtSize = g.MeasureString(dr, _textFont);
            PointF txtPoint = new PointF(this.Width - txtSize.Width - this.Margin.Right - 1, this.Margin.Top);
            
            //g.FillRectangle(new SolidBrush(Color.FromArgb(200, 0, 0, 255)), new RectangleF(drPoint, drSize));
            //g.DrawRectangle(new Pen(Color.FromArgb(200, 0, 0, 255)), drPoint.X, drPoint.Y, drSize.Width, drSize.Height);

            //FillRoundRectangle(g, new SolidBrush(Color.FromArgb(125, 67, 147, 218)), new RectangleF(drPoint, drSize), 3);
            //DrawRoundRectangle(g, new Pen(Color.FromArgb(200, 0, 0, 255)), new RectangleF(drPoint, drSize), 3);

            Image img = GeoDo.RSS.UI.AddIn.HdService.Properties.Resources.bubble2;
            Size imgSize = new System.Drawing.Size((int)img.Width > txtSize.Width ? img.Width : (int)txtSize.Width, img.Height > txtSize.Height ? img.Height : (int)txtSize.Height);
            PointF imgPoint = new PointF(this.Width - imgSize.Width - this.Margin.Right - 1, this.Margin.Top);

            imgRectangle = new Rectangle((int)imgPoint.X, (int)imgPoint.Y, imgSize.Width, imgSize.Height);
            g.DrawImageUnscaledAndClipped(img, imgRectangle);

            textRectangle = new RectangleF((int)txtPoint.X, (int)txtPoint.Y, txtSize.Width, txtSize.Height);
            g.DrawString(dr, _textFont, new SolidBrush(textColor), textRectangle);
            g.Flush();
        }

        public void DrawRoundRectangle(Graphics g, Pen pen, RectangleF rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.DrawPath(pen, path);
            }
        }

        public void FillRoundRectangle(Graphics g, Brush brush, RectangleF rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        internal static GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
