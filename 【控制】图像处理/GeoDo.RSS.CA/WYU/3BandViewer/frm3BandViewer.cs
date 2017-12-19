using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace GeoDo.RSS.CA
{
    public partial class frm3BandViewer : TopFormBase, I3BandViewer
    {
        private Bitmap[] _bitmaps = new Bitmap[3];
        private Size _preSize = Size.Empty;
        private BandSpliter _bandSpliter = new BandSpliter();

        public frm3BandViewer()
        {
            InitializeComponent();
            Disposed += new EventHandler(frm3BandViewer_Disposed);
            SizeChanged += new EventHandler(frm3BandViewer_SizeChanged);
            DoubleBuffered = true;
        }

        void frm3BandViewer_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        void frm3BandViewer_Disposed(object sender, EventArgs e)
        {
            TryFreeBitmaps();
        }

        public void UpdateView()
        {
            _bitmaps = null;
            try
            {
                if (this.Owner == null)
                    return;
                Bitmap bm = null;
                if (this.Owner is frmCurveProcessorArgEditor)
                    bm = (this.Owner as frmCurveProcessorArgEditor).ActiveDrawing;
                else if (this.Owner is frmReversalColorEditor)
                    bm = (this.Owner as frmReversalColorEditor).ActiveDrawing;
                else if (this.Owner is IActiveDrawing)
                    bm = (this.Owner as IActiveDrawing).ActiveDrawing;
                else
                    return;
                if (bm == null)
                    return;
                if (bm == null || bm.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    TryFreeBitmaps();
                    _bitmaps = null;
                    return;
                }
                if (_preSize != bm.Size)
                    ReBuildBitmaps(bm);
                SplitBitmaps(bm);
            }
            finally
            {
                Invalidate();
            }
        }

        private void ReBuildBitmaps(Bitmap bm)
        {
            TryFreeBitmaps();
            Bitmap bmR = new Bitmap(bm.Width, bm.Height, PixelFormat.Format8bppIndexed);
            bmR.Palette = _bandSpliter.DefaultColorPalette;
            Bitmap bmG = new Bitmap(bm.Width, bm.Height, PixelFormat.Format8bppIndexed);
            bmG.Palette = _bandSpliter.DefaultColorPalette;
            Bitmap bmB = new Bitmap(bm.Width, bm.Height, PixelFormat.Format8bppIndexed);
            bmB.Palette = _bandSpliter.DefaultColorPalette;
            _bitmaps = new Bitmap[3];
            _bitmaps[0] = bmR;
            _bitmaps[1] = bmG;
            _bitmaps[2] = bmB;
        }

        void TryFreeBitmaps()
        {
            if (_bitmaps == null)
                return;
            foreach (Bitmap b in _bitmaps)
            {
                if (b != null)
                    b.Dispose();
            }
        }

        private void SplitBitmaps(Bitmap bm)
        {
            _bandSpliter.SplitBitmaps(bm, ref _bitmaps[0], ref _bitmaps[1], ref _bitmaps[2]);
        }

        private Font _font = new Font("黑体", 12);
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_bitmaps == null)
                return;
            float sizeByWidth = (Width) / _bitmaps.Length;
            float size = Math.Min(sizeByWidth, Height);
            size = Math.Min(Math.Min(size, _bitmaps[0].Width), _bitmaps[0].Height);
            float margin = (Width - _bitmaps.Length * size) / 4f;
            float x = margin;
            e.Graphics.DrawString("Red", _font, Brushes.Red, 4, 4);
            e.Graphics.DrawString("Green", _font, Brushes.Green, 8 + size, 4);
            e.Graphics.DrawString("Blue", _font, Brushes.Blue, 12 + size * 2, 4);
            float factor = _bitmaps[0].Width / (float)_bitmaps[0].Height;
            float w = size;
            float h = size / factor;
            float y = 22;
            for (int i = 0; i < _bitmaps.Length; i++)
            {
                e.Graphics.DrawImage(_bitmaps[i], x, y, w, h);
                x += (size + margin);
            }
        }
    }
}
