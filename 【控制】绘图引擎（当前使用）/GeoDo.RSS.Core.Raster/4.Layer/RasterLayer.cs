using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Drawing.Imaging;
using System.ComponentModel;
using GeoDo.Core;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class RasterLayer : Layer, IRasterLayer
    {
        protected bool _visible = true;
        protected IRasterDrawing _drawing = null;
        protected ICanvas _canvas = null;
        protected ImageAttributes _ia;
        protected bool _isNeedReplaceColor = true;
        protected Color _newColor = Color.Transparent;
        protected Color _oldColor = Color.Black;

        public RasterLayer()
        {
        }

        public RasterLayer(IRasterDrawing drawing)
            : base()
        {
            _name = Path.GetFileNameWithoutExtension(drawing.DataProvider.fileName);
            _alias = Path.GetFileName(drawing.DataProvider.fileName);
            _drawing = drawing;
            CreateImageAttributes();
        }

        public RasterLayer(string name, string alias, IRasterDrawing drawing)
            : base()
        {
            _name = name;
            _alias = alias;
            _drawing = drawing;
            CreateImageAttributes();
        }

        [Browsable(false), GeoDo.Core.XmlPersist(typeof(DrawingPropertyConverter))]
        public object Drawing
        {
            get { return _drawing; }
            set { _drawing = value as IRasterDrawing; }
        }

        [DisplayName("是否可见"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private int _trans = 0;

        [DisplayName("透明度(%)"), Category("状态")]
        public int Opacity
        {
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 100)
                    value = 100;
                if (_trans != value)
                {
                    _trans = value;
                    UpdateTransalte();
                }
            }
            get
            {
                return _trans;
            }
        }

        [DisplayName("启用替换"), Category("颜色替换")]
        public bool IsNeedReplaceColor
        {
            get { return _isNeedReplaceColor; }
            set 
            {
                _isNeedReplaceColor = value; 
                if(value)
                    _ia = null;
            }
        }

        [DisplayName("旧颜色"), Category("颜色替换"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color OldColor
        {
            get { return _oldColor; }
            set 
            {
                _oldColor = value;
                _ia = null;
            }
        }

        [DisplayName("新颜色"), Category("颜色替换"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color NewColor
        {
            get { return _newColor; }
            set 
            {
                _newColor = value;
                _ia = null;
            }
        }

        public void UpdateDrawing(IRasterDrawing drawing)
        {
            _drawing = drawing;
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_canvas == null)
                _canvas = sender as ICanvas;
            if (_drawing == null)
                return;
            //Console.WriteLine("RenderRasterLayer:" + _canvas.CurrentEnvelope.ToString());
            Bitmap bitmap = GetBitmap(drawArgs);
            DrawBitmap(bitmap, drawArgs);
        }

        private void DrawBitmap(Bitmap bitmap, IDrawArgs drawArgs)
        {
            if (bitmap == null)
                return;
            CoordEnvelope evp = _drawing.Envelope;
            //TryApplyAOIMask(bitmap, evp);
            double x1 = evp.MinX;
            double y1 = evp.MinY;
            double x2 = evp.MaxX;
            double y2 = evp.MaxY;
            drawArgs.QuickTransformArgs.Transform(ref x1, ref y1);
            drawArgs.QuickTransformArgs.Transform(ref x2, ref y2);
            if (!_isNeedReplaceColor)
            {
                (drawArgs.Graphics as Graphics).DrawImage(bitmap,
                    RectangleF.FromLTRB((float)Math.Min(x1, x2),
                    (float)Math.Min(y1, y2),
                    (float)Math.Max(x1, x2),
                    (float)Math.Max(y1, y2))
                );
            }
            else
            {
                CreateImageAttributes();
                (drawArgs.Graphics as Graphics).DrawImage(bitmap, new PointF[] { new PointF((float)x1, (float)y2), new PointF((float)x2, (float)y2), new PointF((float)x1, (float)y1) },
                new RectangleF(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel, _ia);
            }
        }

        private unsafe void TryApplyAOIMask(Bitmap bitmap, CoordEnvelope evp)
        {
            int[] maskAOI = _canvas.MaskGetter();
            if (maskAOI == null)
                return;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            try
            {
                IntPtr scan0 = bdata.Scan0;
                int stride = bdata.Stride;
                byte* ptr0 = (byte*)scan0.ToPointer();
                byte* ptr = ptr0;
                int count = maskAOI.Length;
                int r = 0, c = 0;
                int w = bdata.Width;
                for (int i = 0; i < count; i++)
                {
                    r = maskAOI[i] / w;
                    c = maskAOI[i] % w;
                    ptr = ptr0 + r * stride + c * 3;
                    for (int b = 0; b < 3; b++, ptr++)
                        *ptr = 0;//black
                }
            }
            finally
            {
                bitmap.UnlockBits(bdata);
            }
        }

        private void CreateImageAttributes()
        {
            if (_ia == null)
                _ia = new ImageAttributes();
            ColorMap cm1 = new ColorMap();
            cm1.OldColor = _oldColor;
            cm1.NewColor = _newColor;
            _ia.SetRemapTable(new ColorMap[] { cm1 }, ColorAdjustType.Bitmap);
        }

        private void UpdateTransalte()
        {
            //以下测试使用矩阵方式调整透明度，效率估计不高，日后再说
            if (_ia == null)
                _ia = new ImageAttributes();
            float opacity = (1 - _trans / 100f);    //这就是透明度了,1表示不透明
            float[][] ptsArray ={ 
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, opacity, 0},  //注意：此处为0.1f，图像为强透明
                    new float[] {0, 0, 0, 0, 1}};
            ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
            _ia.SetColorMatrix(clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        private Bitmap GetBitmap(IDrawArgs drawArgs)
        {
            return _drawing.GetBitmap(drawArgs);
        }

        public override void Dispose()
        {
            if (_drawing != null)
            {
                _drawing.Dispose();
                _drawing = null;
            }
            if (_ia != null)
            {
                _ia.Dispose();
                _ia = null;
            }
            base.Dispose();
            _canvas = null;
        }
    }
}
