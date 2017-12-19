using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using CodeCell.Bricks.Runtime;
using CodeCell.Bricks.Serial;


namespace CodeCell.AgileMap.Core
{
    public enum masSimpleMarkerStyle
    {
        Triangle,
        Rectangle,
        Circle
    }

    public interface IMarkerSymbol : ISymbol, ISymbolCacheable
    {
        Size Size { get; set; }
        PointF PixelLocation { get;  }
    }

    [Serializable]
    public abstract class MarkerSymbol : Symbol, IMarkerSymbol, ISymbolCacheable
    {
        protected Size _size = new Size(2, 2);
        protected bool _needCache = true;
        [NonSerialized]
        protected Image _cachedImage = null;
        protected PointF _pixelLocation = PointF.Empty;

        public MarkerSymbol()
        {
            _brush = new SolidBrush(_color);
            ComputeSymbolSize();
        }

        public MarkerSymbol(Color color)
            : base(color)
        {
            _brush = new SolidBrush(_color);
            ComputeSymbolSize();
        }

        #region IMarkerSymbol 成员

        [DisplayName("颜色")]
        public override Color Color
        {
            get
            {
                return base.Color;
            }
            set
            {
                base.Color = value;
                ClearCache();
            }
        }

        [DisplayName("大小")]
        public Size Size
        {
            get { return _size; }
            set
            { 
                _size = value;
                ComputeSymbolSize();
            }
        }

        private void ComputeSymbolSize()
        {
            _symbolSize = _size;
            _symbolHalfWidth = _size.Width / 2f;
            _symbolHalfHeight = _size.Height / 2f;
        }

        [Browsable(false)]
        public PointF PixelLocation
        {
            get { return _pixelLocation; }
        }

        #endregion

        #region ISymbolCacheable Members

        [Browsable(false)]
        public bool NeedCache
        {
            get
            {
                return _needCache;
            }
            set
            {
                _needCache = false;
            }
        }

        public Image GetCacheSymbol()
        {
            return _cachedImage;
        }

        public void ClearCache()
        {
            if (_cachedImage != null)
            {
                _cachedImage.Dispose();
                _cachedImage = null;
            }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            if (_cachedImage != null)
            {
                _cachedImage.Dispose();
                _cachedImage = null;
            }
        }
    }

    public interface ISimpleMarkerSymbol : IMarkerSymbol
    {
        masSimpleMarkerStyle Style { get; set; }
    }

    public interface ITrueTypeFontSymbol : IMarkerSymbol
    {
        CharItem CharItem { get; set; }
    }

    public interface IPictureMarkerSymbol : IMarkerSymbol
    {
        Bitmap Bitmap { get; set; }
    }

    /// <summary>
    /// 简单标注
    /// 默认样式：直径为2像素的红色圆点
    /// </summary>
    [Serializable]
    public class SimpleMarkerSymbol : MarkerSymbol, ISimpleMarkerSymbol
    {
        protected masSimpleMarkerStyle _style = masSimpleMarkerStyle.Circle;
        private PointF[] pointFs = new PointF[3];

        public SimpleMarkerSymbol()
        {
            _color = Color.Red;
            _pen = Pens.Red;
            _brush = Brushes.Red;
        }

        public SimpleMarkerSymbol(masSimpleMarkerStyle style)
            : this()
        {
            _style = style;
        }

        public override void Draw(Graphics g, PointF point)
        {
            switch (_style)
            {
                case masSimpleMarkerStyle.Circle:
                    DrawEllipse(g, point);
                    break;
                case masSimpleMarkerStyle.Rectangle:
                    DrawRectangle(g, point);
                    break;
                case masSimpleMarkerStyle.Triangle:
                    DrewTriangle(g, point);
                    break;
                default:
                    return;
            }
        }

        private void DrawEllipse(Graphics g, PointF point)
        {
            point.X = point.X - _size.Width / 2;
            point.Y = point.Y - _size.Height / 2;
            _pixelLocation.X = point.X;
            _pixelLocation.Y = point.Y;
            if (_pen == null)
                _pen = Pens.Black;
            //g.DrawEllipse(_pen, point.X, point.Y, (float)_size.Width, (float)_size.Height);
            g.FillEllipse(_brush, point.X, point.Y, (float)_size.Width, (float)_size.Height);
        }

        private void DrawRectangle(Graphics g, PointF point)
        {
            point.X = point.X - _size.Width / 2;
            point.Y = point.Y - _size.Height / 2;
            _pixelLocation.X = point.X;
            _pixelLocation.Y = point.Y;
            g.DrawRectangle(_pen, point.X, point.Y, (float)_size.Width, (float)_size.Height);
        }

        private void DrewTriangle(Graphics g, PointF point)
        {
            pointFs[0].X = point.X + _size.Width / 2;
            pointFs[0].Y = point.Y;
            pointFs[1].X = point.X - _size.Width / 2;
            pointFs[1].Y = point.Y;
            pointFs[2].X = point.X;
            pointFs[2].Y = point.Y - _size.Height;
            _pixelLocation.X = point.X - _size.Width / 2;
            _pixelLocation.Y = point.Y - _size.Height;
            g.DrawPolygon(_pen, pointFs);
        }

        #region ISimpleMarkerSymbol 成员

        [DisplayName("简单符号样式")]
        public masSimpleMarkerStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        #endregion

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("style", _style.ToString());
            obj.AddAttribute("size", _size.Width.ToString() + "," + _size.Height.ToString());
            obj.AddAttribute("color", ColorHelper.ColorToString(_color));
            return obj;
        }

        public static IMarkerSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            masSimpleMarkerStyle style = masSimpleMarkerStyle.Circle;
            string strStyle = ele.Attribute("style").Value;
            foreach (masSimpleMarkerStyle istyle in Enum.GetValues(typeof(masSimpleMarkerStyle)))
            {
                if (istyle.ToString() == strStyle)
                {
                    style = istyle;
                    break;
                }
            }
            Size size = new Size();
            string sizeString = ele.Attribute("size").Value;
            string[] sizeStrings = sizeString.Split(',');
            size.Width = int.Parse(sizeStrings[0]);
            size.Height = int.Parse(sizeStrings[1]);
            //
            Color color = ColorHelper.StringToColor(ele.Attribute("color").Value);
            //
            ISimpleMarkerSymbol sym = new SimpleMarkerSymbol(style);
            sym.Size = size;
            sym.Color = color;
            //
            return sym;
        }
    }

    [Serializable]
    public class FontNameEditorUI : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (frmTrueTypeFontBrowser dlg = new frmTrueTypeFontBrowser())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.SelectedCharItem;
                }
            }
            return base.EditValue(context, provider, value);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }

    /// <summary>
    /// 字体样式标注
    /// 默认样式：默认字体，9号，黑色
    /// </summary>
    [Serializable]
    public class TrueTypeFontSymbol : MarkerSymbol, ITrueTypeFontSymbol
    {
        protected string _fontName = "";
        protected int _fontSize = 9;
        protected CharItem _charItem = null;

        public TrueTypeFontSymbol()
            : base()
        {
            _color = Color.Black;
        }

        public TrueTypeFontSymbol(Color color)
            : base(color)
        { }

        public TrueTypeFontSymbol(Color color, CharItem charItem)
            : base(color)
        {
            _charItem = charItem;
        }

        public override void Draw(Graphics g, PointF point)
        {
            if (g == null || _charItem == null)
                return;
            if (_brush == null)
                ReBuildBrush();
            float x = 0;
            float y = 0;
            if (_needCache)
            {
                if (_cachedImage == null)
                {
                    _symbolSize = g.MeasureString(_charItem.Text, _charItem.Font);
                    _symbolHalfWidth = _symbolSize.Width / 2f;
                    _symbolHalfHeight = _symbolSize.Height / 2f;
                    _cachedImage = new Bitmap((int)_symbolSize.Width, (int)_symbolSize.Height);
                    using (Graphics imgg = Graphics.FromImage(_cachedImage))
                    {
                        imgg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;//防止Win7下ClearType时候出现黑色杂点
                        //imgg.Clear(Color.White);
                        imgg.DrawString(_charItem.Text, _charItem.Font, _brush, 0, 0);
                    }
                    //(_cachedImage as Bitmap).MakeTransparent(Color.White);
                }
                x = point.X - _symbolHalfWidth;
                y = point.Y - _symbolHalfHeight;
                g.DrawImageUnscaled(_cachedImage, (int)x, (int)y);
            }
            else
            {
                
                _symbolSize = g.MeasureString(_charItem.Text, _charItem.Font);
                _symbolHalfWidth = _symbolSize.Width / 2f;
                _symbolHalfHeight = _symbolSize.Height / 2f;
                x = point.X - _symbolHalfWidth;
                y = point.Y - _symbolHalfHeight;
                g.DrawString(_charItem.Text, _charItem.Font, _brush, x, y);
            }
            _pixelLocation.X = x;
            _pixelLocation.Y = y;
        }

        #region ITrueTypeFontSymbol 成员

        [DisplayName("大小")]
        public new int Size
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                if (_charItem != null)
                    _charItem.Font = new Font(_charItem.Font.FontFamily, value);
                ClearCache();
            }
        }

        [DisplayName("符号")]
        [EditorAttribute(typeof(FontNameEditorUI), typeof(UITypeEditor))]
        public CharItem CharItem
        {
            get
            {
                return _charItem;
            }
            set
            {
                _charItem = value;
                ClearCache();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("size", _fontSize.ToString());
            if (_charItem != null)
            {
                obj.AddAttribute("font", FontHelper.FontToString(_charItem.Font));
                obj.AddAttribute("code", _charItem.Code.ToString());
            }
            obj.AddAttribute("color", ColorHelper.ColorToString(_color));
            return obj;
        }

        public static ITrueTypeFontSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            int size = int.Parse(ele.Attribute("size").Value);
            XAttribute att = ele.Attribute("font");
            Font font = null;
            Color color = Color.Red;
            int code = 0;
            if (att != null)
                font = FontHelper.StringToFont(att.Value);
            else
                font = new Font("宋体",9);
            att = ele.Attribute("code");
            if (att != null)
                code = int.Parse(att.Value);
            att = ele.Attribute("color");
            if (att != null)
                color = ColorHelper.StringToColor(att.Value);
            //
            return new TrueTypeFontSymbol(color, new CharItem((byte)code, font));
        }
    }


    [Serializable]
    internal class PictureEditorUI : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (frmPictureBrowser dlg = new frmPictureBrowser())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    return dlg.GetSelectedImage();
                }
            }
            return base.EditValue(context, provider, value);
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
    /// <summary>
    /// Picture标注
    /// </summary>
    [Serializable]
    public class PictureMarkerSymbol : MarkerSymbol, IPictureMarkerSymbol
    {
        private Bitmap _bitmap = null;
        private string _imageUrl = null;

        public PictureMarkerSymbol()
        { }

        public PictureMarkerSymbol(string imageUrl)
        {
            _imageUrl = imageUrl;
            if (_imageUrl == null)
                return;
            try
            {
                if (!_imageUrl.Contains(Path.VolumeSeparatorChar.ToString()))
                {
                    if (_imageUrl.Substring(0, 1) != Path.DirectorySeparatorChar.ToString())
                        _imageUrl = Path.DirectorySeparatorChar + _imageUrl;
                    _imageUrl = Constants.GetMapResourceDir() + _imageUrl;
                }
                _bitmap = (Bitmap)Bitmap.FromFile(_imageUrl);
                ComputeSymbolSize();
            }
            catch (Exception ex)
            {
                Log.WriterException("MarkerSymbol", "ImageUrl:set", ex);
            }
        }

        public PictureMarkerSymbol(Bitmap bitmap,string imageUrl)
        {
            _bitmap = bitmap;
            _imageUrl = imageUrl;
        }

        public override void Draw(Graphics g, PointF point)
        {
            if (_bitmap == null)
                return;
            float x = point.X - _symbolHalfWidth;
            float y = point.Y - _symbolHalfHeight;
            _pixelLocation.X = x;
            _pixelLocation.Y = y;
            g.DrawImage(_bitmap, x, y,_bitmap.Width,_bitmap.Height);
        }

        private void ComputeSymbolSize()
        {
            _symbolSize = _bitmap.Size;
            _symbolHalfWidth = _symbolSize.Width / 2f;
            _symbolHalfHeight = _symbolSize.Height / 2f;
        }

        #region IPictureMarkerSymbol 成员

        [Browsable(false)]
        public new Size Size
        {
            get { return Size.Empty; }
            set { ;}
        }

        [Browsable(false)]
        public new Color Color
        {
            get { return Color.Red; }
            set { ;}
        }

        [Browsable(false)]
        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set 
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = value;
                if(_bitmap != null)
                    ComputeSymbolSize();
            }
        }

        [DisplayName("标注图片")]
        [EditorAttribute(typeof(PictureEditorUI), typeof(UITypeEditor))]
        public ImageItem ImageItem
        {
            get
            {
                return new ImageItem(_imageUrl,false);
            }
            set
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _imageUrl = value.Filename;
                _bitmap =  value.Image as Bitmap;
                if (_bitmap != null)
                    ComputeSymbolSize();
                return;
            }
        }

        [DisplayName("标注图片"),Browsable(false)]
        public string ImageUrl
        {
            get { return _imageUrl; }
            set 
            {
                if (value == null || value != _imageUrl)
                    return;
                try
                {
                    _imageUrl = value;
                    _bitmap = (Bitmap)Bitmap.FromFile(_imageUrl);
                }
                catch (Exception ex)
                {
                    Log.WriterException("MarkerSymbol", "ImageUrl:set", ex);
                }
            }
        }

        #endregion

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            string fileurl = _imageUrl != null ? _imageUrl.Replace(Constants.GetMapResourceDir(), string.Empty) : string.Empty;
            obj.AddAttribute("imageurl", fileurl);
            return obj;
        }

        public static IPictureMarkerSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            return new PictureMarkerSymbol(ele.Attribute("imageurl").Value);
        }
    }
}
