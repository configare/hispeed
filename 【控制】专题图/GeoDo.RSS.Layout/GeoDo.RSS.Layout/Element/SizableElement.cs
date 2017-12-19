using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace GeoDo.RSS.Layout
{
    public abstract class SizableElement : Element, ISizableElement, ICanvasEvent, IInteractiveEditable, IPersitable
    {
        protected SizeF _size;
        protected PointF _location;
        protected enumEditType _editType = enumEditType.Move;
        protected float _angle = 0;
        protected Matrix _matrix;
        protected static Image _lockImage;
        protected SizeF _inSize = new Size(20, 20);
        protected bool _isDoubleBorderLine = false;

        public SizableElement()
            : base()
        {
        }

        [Persist(enumAttType.UnValueType), DisplayName("尺寸"), Category("布局")]
        public virtual SizeF Size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("左上角坐标"), Category("布局")]
        public PointF Location
        {
            get { return _location; }
            set
            {
                _location = value;
            }
        }

        [PersistAttribute(), DisplayName("旋转角度"), Category("布局")]
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        [Persist(enumAttType.ValueType), DisplayName("使用双边框"), Category("双线框")]
        public bool IsDoubleBorderLine
        {
            get { return _isDoubleBorderLine; }
            set { _isDoubleBorderLine = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("内框尺寸"), Category("双线框")]
        public virtual SizeF InSize
        {
            get { return _inSize; }
            set
            {
                _inSize = value;
            }
        }

        public override bool IsHited(float layoutX, float layoutY)
        {
            return layoutX > _location.X && layoutX < _location.X + _size.Width
                && layoutY > _location.Y && layoutY < _location.Y + _size.Height;
        }

        public override bool IsHited(RectangleF layoutRect)
        {
            return !(_location.X < layoutRect.Left ||
                     _location.X > layoutRect.Right ||
                     _location.Y < layoutRect.Top ||
                     _location.Y > layoutRect.Bottom);
        }

        public virtual void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
        }

        [Browsable(false)]
        public virtual enumEditType SupportedEditType
        {
            get { return _editType; }
        }

        public virtual void ApplyLocation(float layoutOffsetX, float layoutOffsetY)
        {
            if (_isLocked)
                return;
            _location.X += layoutOffsetX;
            _location.Y += layoutOffsetY;
        }

        public virtual void ApplySize(float layoutXSize, float layoutYSize)
        {
            if (_isLocked)
                return;
            if ((_size.Width + layoutXSize) <= 0 || (_size.Height + layoutYSize) <= 0)
                return;
            _size.Width += layoutXSize;
            _size.Height += layoutYSize;
        }

        public virtual void ApplyRotate(float angleDegree)
        {
            _angle = angleDegree;
        }

        Matrix _oldMatrix;
        protected virtual void BeginRotate(IDrawArgs drawArgs)
        {
            if (Math.Abs(_angle) < float.Epsilon)
                return;
            while (_angle > 360)
            {
                _angle -= 360;
            };
            while (_angle < 0)
            {
                _angle += 360;
            };
            Graphics g = drawArgs.Graphics as Graphics;
            ILayoutRuntime runtime = drawArgs.Runtime;
        setAgleLine:
            if (_matrix != null)
            {
                _matrix.Reset();
                float x = _location.X + _size.Width / 2;
                float y = _location.Y + _size.Height / 2;
                runtime.Layout2Screen(ref x, ref y);
                _matrix.RotateAt(_angle, new PointF(x, y));
            }
            else
            {
                _matrix = new Matrix();
                goto setAgleLine;
            }
            _oldMatrix = g.Transform;
            Matrix c = _matrix.Clone();
            c.Multiply(_oldMatrix, MatrixOrder.Append);
            g.Transform = c;
        }

        public void DrawLockIcon(IDrawArgs drawArgs)
        {
            if (_isLocked && !DrawArgs.IsExporting)
            {
                if (_lockImage == null)
                    _lockImage = Bitmap.FromStream((typeof(SizableElement)).Assembly.GetManifestResourceStream("GeoDo.RSS.Layout.Lock.png"));
                float x = _location.X, y = _location.Y, w = _size.Width, h = _size.Height;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                (drawArgs.Graphics as Graphics).DrawImage(_lockImage, x, y);
            }
        }

        public virtual void EndRotate(IDrawArgs drawArgs)
        {
            DrawLockIcon(drawArgs);
            if (_angle < float.Epsilon)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            g.Transform.Dispose();
            g.Transform = _oldMatrix;
            _oldMatrix = null;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("angle") != null)
            {
                att = xml.Attribute("angle").Value;
                if (!String.IsNullOrEmpty(att))
                    _angle = float.Parse(att);
            }
            if (xml.Attribute("size") != null)
            {
                _size = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("size").Value);
            }
            if (xml.Attribute("location") != null)
                _location = (PointF)LayoutFromFile.Base64StringToObject(xml.Attribute("location").Value);
            if (xml.Attribute("isdoubleborderline") != null)
            {
                att = xml.Attribute("isdoubleborderline").Value;
                if (att != null)
                    _isDoubleBorderLine = bool.Parse(att);
            }
            if (xml.Attribute("interlineSize") != null)
            {
                _inSize = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("interlineSize").Value);
            }
            base.InitByXml(xml);
        }

        private void ChangeSizeToPixel()
        {
            if (_size == SizeF.Empty)
                return;
            if (_runtime == null)
                return;
            if (_runtime.Layout == null)
                return;
            if (_runtime.Layout.Unit == enumLayoutUnit.Centimeter)
                return;
            _size = new SizeF(_runtime.Centimeter2Pixel(_size.Width), _runtime.Centimeter2Pixel(_size.Height));
        }

        public override void Dispose()
        {
            if (_matrix != null)
            {
                _matrix.Dispose();
                _matrix = null;
            }
            base.Dispose();
        }
    }
}
