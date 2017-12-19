using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class PictureElement : SizableElement, IPersitable, IDisposable
    {
        protected Bitmap _bitmap;
        private const int MAX_SIZE = 512;
        protected Size _bitmapOSize;

        public PictureElement()
            : base()
        {
            _name = "图片";
        }

        public PictureElement(Bitmap bitmap)
            : base()
        {
            _name = "图片";
            _icon = ImageGetter.GetImageByName("PictureElement.png");
            _bitmap = bitmap;
            _bitmapOSize = new Size(_bitmap.Width, _bitmap.Height); 
        }

        public PictureElement(string fname)
            : base()
        {
            try
            {
                if (File.Exists(fname))
                {
                    _bitmap = Bitmap.FromFile(fname) as Bitmap;
                    _name = Path.GetFileNameWithoutExtension(fname);
                    _bitmapOSize = new Size(_bitmap.Width, _bitmap.Height); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ResetOSize()
        {
            _size = SizeF.Empty;
        }

        [DisplayName("恢复原始大小"), Category("设计")]
        public bool ResetingOSize
        {
            get { return false; }
            set 
            {
                if (value)
                {
                    ResetOSize();
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("原始大小"), Category("数据")]
        public Size OSize
        {
            get { return _bitmapOSize; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("图片信息"),Browsable(false)]
        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            BeginRotate(drawArgs);
            float w=0,h=0;
            if (_size.IsEmpty)
            {
                w = _bitmapOSize.Width;
                h = _bitmapOSize.Height;
                drawArgs.Runtime.Pixel2Layout(ref w, ref h);
                _size = new SizeF(w, h);
            }
            else
            {
                w = _size.Width;
                h = _size.Height;
            }
            float x = _location.X, y = _location.Y;
            drawArgs.Runtime.Layout2Screen(ref x, ref y);
            drawArgs.Runtime.Layout2Screen(ref w);
            drawArgs.Runtime.Layout2Screen(ref h);
            g.DrawImage(_bitmap, x, y, w, h);
            EndRotate(drawArgs);
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("bitmap") != null)
                _bitmap = (Bitmap)LayoutFromFile.Base64StringToObject(xml.Attribute("bitmap").Value);
            if (xml.Attribute("osize") != null)
            {
                _size = (Size)LayoutFromFile.Base64StringToObject(xml.Attribute("osize").Value);
            }
            base.InitByXml(xml);
        }

        public override void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            base.Dispose();
        }
    }
}
