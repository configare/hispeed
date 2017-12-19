using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using GeoDo.RSS.UI.AddIn.L2ColorTable;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public partial class UCColorRampUseBlocks : UserControl
    {
        private BandValueColorPair[] _valueColors = null;
        private Bitmap _bitmap = null;
        private int _samples = 10;

        public UCColorRampUseBlocks()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public void Apply(BandValueColorPair[] valueColors)
        {
            _valueColors = valueColors;
            Invalidate();
        }

        private int Samples
        {
            get { return _samples; }
            set { _samples = value; }
        }

        public static Bitmap GetLegentBitmap(BandValueColorPair[] valueColors,int count, Size size)
        {
            float samplesSpan = -1;
            BandValueColorPair[] samples = SamplesBandValueColors(valueColors, count, out samplesSpan);
            if (samplesSpan < 1)
                samples = null;
            Bitmap bm = new Bitmap(size.Width, size.Height);
            Font font = new Font("宋体", 9);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(Color.White);
                SizeF fontSize = g.MeasureString("333", font);
                float x = 0;
                float span = bm.Width / (float)valueColors.Length;
                using (Pen pen = new Pen(Brushes.Black))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                    //
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        for (int i = 0; i < valueColors.Length; i++)
                        {
                            brush.Color = valueColors[i].Color;
                            g.FillRectangle(brush, x, 0, span, bm.Height - fontSize.Height - 4);
                            x += span;
                        }
                    }
                    x = 0;
                    //label
                    for (int i = 0; i < samples.Length; i++)
                    {
                        x = i * span * samplesSpan;
                        g.DrawLine(pen, x, 0, x, bm.Height - fontSize.Height - 4);
                        if (i == samples.Length - 1)
                        {
                            fontSize = g.MeasureString(samples[i].MinValue.ToString(), font);
                            x -= fontSize.Width;
                        }
                        g.DrawString(samples[i].MinValue.ToString(), font, Brushes.Black, x, bm.Height - fontSize.Height);
                    }
                }
                g.DrawRectangle(Pens.Gray, 0, 0, bm.Width - 1, bm.Height - 1);
            }
            return bm;
        }

        public static BandValueColorPair[] SamplesBandValueColors(BandValueColorPair[] allValueColors, int count, out float span)
        {
            span = -1;
            if (allValueColors == null || allValueColors.Length == 0)
                return null;
            if (count > allValueColors.Length)
                count = allValueColors.Length;
            BandValueColorPair[] rets = new BandValueColorPair[count];
            if (count == 1)
            {
                rets[0] = allValueColors[0];
                return rets;
            }
            span = allValueColors.Length / (float)(count - 1);
            for (int i = 0; i < count; i++)
            {
                int idx = (int)(i * span);
                if (idx >= allValueColors.Length)
                    idx = allValueColors.Length - 1;
                rets[i] = allValueColors[idx];
            }
            return rets;
        }

        private BandValueColorPair[] SamplesBandValueColors(int count,out float span)
        {
            return SamplesBandValueColors(_valueColors, count, out span);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_valueColors == null || _valueColors.Length == 0)
                return;
            if (_bitmap == null || _bitmap.Width != Width)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = new Bitmap(Width, Height);
            }
            float samplesSpan = -1;
            BandValueColorPair[] samples = SamplesBandValueColors(_samples, out samplesSpan);
            if (samplesSpan < 1)
                samples = null;
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                g.Clear(Color.Transparent);
                SizeF fontSize = g.MeasureString("300", Font);
                float span = _bitmap.Width / (float)_valueColors.Length;
                float x = 0;
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    for (int i = 0; i < _valueColors.Length; i++)
                    {
                        brush.Color = _valueColors[i].Color;
                        g.FillRectangle(brush, x, 0, span, _bitmap.Height - fontSize.Height - 4);
                        x += span;
                    }
                }
                if (samples != null)
                {
                    using (Pen pen = new Pen(Brushes.Black))
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        //label
                        x = 0;
                        for (int i = 0; i < samples.Length; i++)
                        {
                            x = i * span * samplesSpan;
                            g.DrawLine(pen, x, 0, x, _bitmap.Height - fontSize.Height - 4);
                            if (i == samples.Length - 1)
                            {
                                fontSize = g.MeasureString(samples[i].MinValue.ToString(), Font);
                                x -= fontSize.Width;
                            }
                            g.DrawString(samples[i].MinValue.ToString(), Font, Brushes.Black, x, _bitmap.Height - fontSize.Height);
                        }
                    }
                }
            }
            e.Graphics.DrawImage(_bitmap, 0, 0);
        }
    }
}
