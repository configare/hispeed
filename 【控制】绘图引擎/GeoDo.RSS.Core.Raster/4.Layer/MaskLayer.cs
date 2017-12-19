using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class MaskLayer : Layer, IRenderLayer, IMaskLayer
    {
        protected bool _visible = true;
        protected Color _maskColor;
        protected CoordEnvelope _coordEnvelope;
        protected Size _rasterSize;
        protected Size _bitmapSize = new Size(1024, 768);
        protected Bitmap _bitmap;
        private ImageAttributes _ia = new ImageAttributes();
        private bool _isGeoCoord;

        public MaskLayer()
        {
            _name = _alias = "蒙板层";
            CreateImageAttributes();
        }

        private void CreateImageAttributes()
        {
            _ia = new ImageAttributes();
            ColorMap cm1 = new ColorMap();
            cm1.OldColor = Color.Red; //visible region
            cm1.NewColor = Color.Transparent;
            _ia.SetRemapTable(new ColorMap[] { cm1 }, ColorAdjustType.Bitmap);
        }

        private void BuildBitmap()
        {
            if (_bitmap == null || _bitmap.Size != _bitmapSize)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = new Bitmap(_bitmapSize.Width, _bitmapSize.Height, PixelFormat.Format8bppIndexed);
                ColorPalette cp = _bitmap.Palette;
                cp.Entries[1] = Color.Red;//visible region
                cp.Entries[0] = _maskColor;
                for (int i = 2; i < 256; i++)
                    cp.Entries[i] = _maskColor;
                _bitmap.Palette = cp;
            }
        }

        public unsafe void Update(Color maskColor, Size rasterSize, CoordEnvelope coordEnvelope, bool isGeoCoord, int[] visibleRegion)
        {
            if (rasterSize.Width < _bitmapSize.Width)
                _bitmapSize.Width = rasterSize.Width;
            if (rasterSize.Height < _bitmapSize.Height)
                _bitmapSize.Height = rasterSize.Height;
            _maskColor = maskColor;
            _rasterSize = rasterSize;
            _coordEnvelope = coordEnvelope;
            _isGeoCoord = isGeoCoord;
            BuildBitmap();
            float scaleX = _bitmap.Width / (float)_rasterSize.Width;
            float scaleY = _bitmap.Height / (float)_rasterSize.Height;
            float scale = Math.Min(scaleX, scaleY);
            CorrectCoordEnvelope(scale);
            BitmapData bdata = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            try
            {
                IntPtr scan0 = bdata.Scan0;
                int stride = bdata.Stride;
                Reset(bdata);
                byte* ptr0 = (byte*)scan0.ToPointer();
                byte* ptr = ptr0;
                int count = visibleRegion.Length;
                int r = 0, c = 0;
                int w = _rasterSize.Width;
                for (int i = 0; i < count; i++)
                {
                    r = visibleRegion[i] / w;
                    c = visibleRegion[i] % w;
                    r = (int)(r * scale);
                    c = (int)(c * scale);
                    ptr = ptr0 + r * stride + c;
                    *ptr = 1;//true value         
                }
            }
            finally
            {
                _bitmap.UnlockBits(bdata);
            }
        }

        private void CorrectCoordEnvelope(float scale)
        {
            double resX = _coordEnvelope.Width / (scale * _rasterSize.Width);
            double resY = _coordEnvelope.Height / (scale * _rasterSize.Height);
            double minX = _coordEnvelope.MinX;
            double maxY = _coordEnvelope.MaxY;
            double maxX = minX + _bitmap.Width * resX;
            double minY = maxY - _bitmap.Height * resY;
            _coordEnvelope = new CoordEnvelope(minX, maxX , minY , maxY );
        }

        private unsafe void Reset(BitmapData bdata)
        {
            IntPtr scan0 = bdata.Scan0;
            int stride = bdata.Stride;
            byte* ptr0 = (byte*)scan0.ToPointer();
            int count = stride * _bitmap.Height;
            for (int i = 0; i < count; i++, ptr0++)
                *ptr0 = 0;
        }

        [DisplayName("是否可见"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_bitmap == null || _coordEnvelope == null)
                return;
            CoordEnvelope evp = _coordEnvelope.Clone();
            if (_isGeoCoord)
            {
                (sender as ICanvas).CoordTransform.Geo2Prj(evp);
            }
            double x1 = evp.MinX;
            double y1 = evp.MinY;
            double x2 = evp.MaxX;
            double y2 = evp.MaxY;
            drawArgs.QuickTransformArgs.Transform(ref x1, ref y1);
            drawArgs.QuickTransformArgs.Transform(ref x2, ref y2);
            /*
             * 以下两句为了解决：
             * 在缩放过程中因浮点取整导致边界1-2个像元没有被遮住的问题
             */
            x1 -= 1;
            y2 -= 1;
            Graphics g = drawArgs.Graphics as Graphics;
            //g.DrawImage(_bitmap,
            //    RectangleF.FromLTRB((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Max(x1, x2), (float)Math.Max(y1, y2)));
            g.DrawImage(_bitmap, new PointF[] { new PointF((float)x1, (float)y2), new PointF((float)x2, (float)y2), new PointF((float)x1, (float)y1) },
                new RectangleF(0, 0, _bitmap.Width, _bitmap.Height), GraphicsUnit.Pixel, _ia);
        }

        public override void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            if (_ia != null)
            {
                _ia.Dispose();
                _ia = null;
            }
            base.Dispose();
        }
    }
}
