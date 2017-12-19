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
    public class SimpleBitmapLayer : Layer, IRasterLayer
    {
        private bool _visible = true;
        private Bitmap _bitmap;
        private CoordEnvelope _coordEnvelope;
        private bool _isGeoCoord;
        private ImageAttributes _ia = new ImageAttributes();
        protected bool _isNeedReplaceColor = false;
        protected Color _newColor;
        protected Color _oldColor;

        public SimpleBitmapLayer(string name, Bitmap bitmap, CoordEnvelope coordEnvelope, bool isGeoCoord)
        {
            _name = name;
            _bitmap = bitmap;
            _coordEnvelope = coordEnvelope;
            _isGeoCoord = isGeoCoord;
            CreateImageAttributes();
        }

        private void CreateImageAttributes()
        {
            _ia = new ImageAttributes();
            ColorMap cm1 = new ColorMap();
            cm1.OldColor = Color.Black;
            cm1.NewColor = Color.Transparent;
            _ia.SetRemapTable(new ColorMap[] { cm1 }, ColorAdjustType.Bitmap);
        }

        [DisplayName("是否可见"),Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("启用替换"), Category("颜色替换")]
        public bool IsNeedReplaceColor
        {
            get { return _isNeedReplaceColor; }
            set { _isNeedReplaceColor = value; }
        }

        [DisplayName("旧颜色"), Category("颜色替换")]
        public Color OldColor
        {
            get { return _oldColor; }
            set { _oldColor = value; }
        }

        [DisplayName("新颜色"), Category("颜色替换")]
        public Color NewColor
        {
            get { return _newColor; }
            set { _newColor = value; }
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
            Graphics g = drawArgs.Graphics as Graphics;
            //g.DrawImage(_bitmap,
            //    RectangleF.FromLTRB((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Max(x1, x2), (float)Math.Max(y1, y2)));
            //                       
            g.DrawImage(_bitmap,new PointF[]{ new PointF((float)x1,(float)y2),new PointF((float)x2,(float)y2),new PointF((float)x1,(float)y1)},
                new RectangleF(0,0,_bitmap.Width,_bitmap.Height),GraphicsUnit.Pixel,_ia);
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        }

        [Browsable(false)]
        public object Drawing
        {
            get { return null; }
        }

        public override void Dispose()
        {
            base.Dispose();
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
        }
    }
}
