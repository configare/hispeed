using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class MemoryBitmapLayer<T>:Layer,IMemoryBitmapLayer
    {
        private bool _visible = false;
        private IMemoryRaster<T> _raster;
        private Dictionary<T, Color> _colorMap;
        private Bitmap _bitmap;
        private bool _coordIsTransformed = false;

        public MemoryBitmapLayer(IMemoryRaster<T> raster,Dictionary<T,Color> colorMap)
        {
            _name = raster.Identify;
            _raster = raster;
            _colorMap = colorMap;
            CreateBitmap();
        }

        private void CreateBitmap()
        {
            Update();
        }

        [Browsable(false)]
        public object Raster
        {
            get { return _raster; }
        }

        public void UpdateColorMap(object colorMap)
        {
            Dictionary<T, Color> cm = colorMap as Dictionary<T, Color>;
            if (cm != null)
            {
                _colorMap = cm;
                Update();
            }
        }

        public void Update()
        {
            fLine:
            if (_bitmap == null)
                _bitmap = new Bitmap(_raster.Size.Width, _raster.Size.Height, PixelFormat.Format32bppArgb);
            else if (_raster.Size != _bitmap.Size)
            {
                _bitmap.Dispose();
                goto fLine;
            }
            using (IArray2Bitmap<T> b = new Array2Bitmap<T>())
            {
                b.ToBitmap(_raster.Data, _raster.Size, _colorMap, ref _bitmap);
            }
        }

        [DisplayName("是否可见"),Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_bitmap == null || _raster == null)
                return;
            CoordEnvelope evp = _raster.Envelope;
            if (!_coordIsTransformed && _raster.IsGeoCoordinate)
            {
                (sender as ICanvas).CoordTransform.Geo2Prj(evp);
                _coordIsTransformed = true;
            }
            double x1 = evp.MinX;
            double y1 = evp.MinY;
            double x2 = evp.MaxX;
            double y2 = evp.MaxY;
            drawArgs.QuickTransformArgs.Transform(ref x1, ref y1);
            drawArgs.QuickTransformArgs.Transform(ref x2, ref y2);
            //(drawArgs.Graphics as Graphics).DrawImage(_bitmap,
            //    RectangleF.FromLTRB((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Max(x1, x2), (float)Math.Max(y1, y2))
            //);

            ImageAttributes ia = SetTransparent(drawArgs.Graphics as Graphics);
            //(drawArgs.Graphics as Graphics).DrawImage(_bitmap,
            //    RectangleF.FromLTRB((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Max(x1, x2), (float)Math.Max(y1, y2))
            //);
            (drawArgs.Graphics as Graphics).DrawImage(_bitmap,
                new PointF[] { new PointF((float)x1, (float)y2), new PointF((float)x2, (float)y2), new PointF((float)x2, (float)y1) },
                new RectangleF(0, 0, (float)evp.Width, (float)evp.Height), GraphicsUnit.Pixel, ia);
        }

        private ImageAttributes SetTransparent(Graphics graphics)
        {
            ImageAttributes ia = new ImageAttributes();
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = 0f;
            ia.SetColorMatrix(colorMatrix, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            return ia;
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            _raster = null;
        }
    }
}
