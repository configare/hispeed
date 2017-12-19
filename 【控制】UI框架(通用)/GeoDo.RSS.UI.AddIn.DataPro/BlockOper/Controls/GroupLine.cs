using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class GroupLine : UserControl
    {
        private static int _idx = 0;
        private Image _image = null;
        private string _caption = "GroupLine" + _idx.ToString();
        private Pen _linePen = new Pen(Color.Black);
        private RadioButton _radioButton = new RadioButton();

        public GroupLine()
        {
            InitializeComponent();
            _radioButton.Text = string.Empty;
            _radioButton.Width = 12;
            Controls.Add(_radioButton);
        }

        [Category("自定义属性")]
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }

        [Category("自定义属性")]
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        [Category("自定义属性")]
        public Color LineColor
        {
            get { return _linePen.Color; }
            set
            {
                _linePen = new Pen(value);
            }
        }

        [Category("自定义属性")]
        public bool Checked
        {
            get { return _radioButton.Checked; }
            set { _radioButton.Checked = value; }
        }

        [Category("自定义属性")]
        public bool RadioButtonVisible
        {
            get { return _radioButton.Visible; }
            set { _radioButton.Visible = value; }
        }

        [Category("自定义属性")]
        public Size RadioButtonSize
        {
            get { return _radioButton.Size; }
            set { _radioButton.Size = value; }
        }

        public RadioButton RadioButton
        {
            get { return _radioButton; }
        }

        protected override void OnResize(EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int top = Height / 2;
            int left = 0;
            _radioButton.Top = top - _radioButton.Height / 2;
            left = _radioButton.Width;
            left = DrawImage(e.Graphics, left, top);
            left = DrawText(e.Graphics, left, top);
            DrawLine(e.Graphics, left, top);
        }

        private void DrawLine(Graphics graphics, int left, int top)
        {
            if (Width < left)
                return;
            graphics.DrawLine(_linePen, left, top, Width, top);
        }

        private int DrawText(Graphics graphics, int left, int top)
        {
            if (_caption != null)
            {
                if (Width < left)
                    return left;
                SizeF size = graphics.MeasureString(_caption, Font);
                using (SolidBrush b = new SolidBrush(ForeColor))
                {
                    graphics.DrawString(_caption, Font, b, left, top - size.Height / 2);
                }
                return left + (int)Math.Ceiling(size.Width);
            }
            return left;
        }

        private int DrawImage(Graphics graphics, int left, int top)
        {
            if (_image != null)
            {
                graphics.DrawImageUnscaled(_image, left, top - _image.Height / 2);
                return _image.Width;
            }
            return left;
        }
    }
}
