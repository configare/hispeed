using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public partial class LayoutControl : UserControl
    {
        private IScaleRulerHelper _helper;
        internal const int RULER_HEIGHT = 20;
        private UserControl _actualLayoutControl = new ActualLayoutControl();
        private ScaleRulerSettings _settings = new ScaleRulerSettings();
        private Font _font = new Font("微软雅黑", 6);
        private PointF _currentPoint = new PointF();

        public LayoutControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Paint += new PaintEventHandler(LayoutControl_Paint);
            Load += new EventHandler(LayoutControl_Load);
            SizeChanged += new EventHandler(LayoutControl_SizeChanged);
            Disposed += new EventHandler(LayoutControl_Disposed);
        }

        void LayoutControl_Disposed(object sender, EventArgs e)
        {
            _font.Dispose();
        }

        public UserControl ActualContainer
        {
            get { return _actualLayoutControl; }
        }

        void LayoutControl_SizeChanged(object sender, EventArgs e)
        {
            _actualLayoutControl.Width = Width - RULER_HEIGHT;
            _actualLayoutControl.Height = Height - RULER_HEIGHT;
        }

        void LayoutControl_Load(object sender, EventArgs e)
        {
            CreateActualContainer();
        }

        private void CreateActualContainer()
        {
            _actualLayoutControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            _actualLayoutControl.Left = RULER_HEIGHT;
            _actualLayoutControl.Top = RULER_HEIGHT;
            _actualLayoutControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            Controls.Add(_actualLayoutControl);
            _actualLayoutControl.SizeChanged += new EventHandler(c_SizeChanged);
            //遮住左上角的交叉线
            Panel p = new Panel();
            int offsetX = _actualLayoutControl.Size.Width - _actualLayoutControl.ClientSize.Width;
            int offsetY = _actualLayoutControl.Size.Height - _actualLayoutControl.ClientSize.Height;
            p.Width = RULER_HEIGHT + 1;
            p.Height = RULER_HEIGHT + 1;
            p.Location = new Point(0, 0);
            p.BackColor = SystemColors.Control;
            Controls.Add(p);
            //
            _actualLayoutControl.MouseMove += new MouseEventHandler(_actualLayoutControl_MouseMove);
        }


        void _actualLayoutControl_MouseMove(object sender, MouseEventArgs e)
        {
            _currentPoint.X = e.X;
            _currentPoint.Y = e.Y;
            Invalidate();
        }

        void c_SizeChanged(object sender, EventArgs e)
        {
            _actualLayoutControl.Invalidate();
        }

        public void SetScaleRulerHelper(IScaleRulerHelper helper)
        {
            _helper = helper;
        }

        void LayoutControl_Paint(object sender, PaintEventArgs e)
        {
            if (_helper == null)
                return;
            e.Graphics.Clear(SystemColors.Control);
            float scale = _helper.LayoutRuntime.Scale;
            float x = 0, y = 0;
            if (_helper.Layout == null)
                return;
            float w = _helper.Layout.Size.Width * scale;
            float h = _helper.Layout.Size.Height * scale;
            _helper.LayoutRuntime.Layout2Pixel(ref w, ref h);
            int offsetX = _actualLayoutControl.Size.Width - _actualLayoutControl.ClientSize.Width;
            int offsetY = _actualLayoutControl.Size.Height - _actualLayoutControl.ClientSize.Height;
            _helper.LayoutRuntime.Layout2Screen(ref x, ref y);
            x += RULER_HEIGHT;
            y += RULER_HEIGHT;
            x += offsetX / 2f;
            y += offsetY / 2f;
            e.Graphics.FillRectangle(Brushes.White, x, 0, w, RULER_HEIGHT);
            e.Graphics.FillRectangle(Brushes.White, 0, y, RULER_HEIGHT, h);
            e.Graphics.DrawRectangle(Pens.Gray, x, 0, w, RULER_HEIGHT);
            e.Graphics.DrawRectangle(Pens.Gray, 0, y, RULER_HEIGHT, h);
            //
            float fontWidth = e.Graphics.MeasureString("000", _font).Width;
            int major = _settings.MajorInterleave;
            int minor = _settings.MinorInterleave;
            bool isDrawMajor = scale * major > 2;
            bool isDrawMinor = scale * minor > 2;
            bool isLabel = scale * major > fontWidth;
            float pX = x, pY = y;
            //
            int idx = 0;
            float labelY = RULER_HEIGHT / 4f;
            float ty = RULER_HEIGHT / 2f;
            if (isDrawMajor)
            {
                while (pX <= w + x)
                {
                    e.Graphics.DrawLine(Pens.Black, pX, ty, pX, RULER_HEIGHT);
                    if (isLabel)
                        e.Graphics.DrawString((idx * major).ToString(), _font, Brushes.Black, pX, labelY);
                    pX += major * scale;
                    idx++;
                }
            }
            if (isDrawMinor)
            {
                pX = x;
                ty = RULER_HEIGHT / 1.4f;
                while (pX <= w + x)
                {
                    e.Graphics.DrawLine(Pens.Black, pX, ty, pX, RULER_HEIGHT);
                    pX += minor * scale;
                }
            }
            //
            idx = 0;
            float labelX = RULER_HEIGHT / 5f;
            float tx = RULER_HEIGHT / 2f;
            if (isDrawMajor)
            {
                while (pY <= h + y)
                {
                    e.Graphics.DrawLine(Pens.Black, 0, pY, tx, pY);
                    if (isLabel)
                        e.Graphics.DrawString((idx * major).ToString(), _font, Brushes.Black, labelX, pY);
                    pY += major * scale;
                    idx++;
                }
            }
            if (isDrawMinor)
            {
                pY = y;
                tx = RULER_HEIGHT / 4f;
                while (pY <= h + y)
                {
                    e.Graphics.DrawLine(Pens.Black, 0, pY, tx, pY);
                    pY += minor * scale;
                }
            }
            //
            x = _currentPoint.X + RULER_HEIGHT + offsetX / 2f;
            y = _currentPoint.Y + RULER_HEIGHT + offsetY / 2f;
            e.Graphics.DrawLine(Pens.Red, x, 0, x, RULER_HEIGHT);
            e.Graphics.DrawLine(Pens.Red, 0, y, RULER_HEIGHT, y);
        }
    }
}
