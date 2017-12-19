using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace GeoDo.RSS.Layout
{
    public class SizableElementGroup : ElementGroup, ISizableElementGroup, IPersitable
    {
        protected SizeF _size;
        protected PointF _location;
        protected enumEditType _editType = enumEditType.Move;
        protected float _angle;

        public SizableElementGroup()
            : base()
        {
            Init();
        }

        public SizableElementGroup(IElement[] elements)
            : base()
        {
            Init();
            if (elements == null || elements.Length == 0)
                return;
            foreach (IElement ele in elements)
                if (ele is ISizableElement && !(ele is ISizableElementGroup))
                    _elements.Add(ele);
            SetGroupLocationFromElements();
            SetGroupSizeFromElements();
        }

        private void Init()
        {
            _name = "组合";
        }

        [Persist(enumAttType.UnValueType), DisplayName("尺寸"), Category("布局")]
        public SizeF Size
        {
            get { return _size; }
            set { _size = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("左上角坐标"), Category("布局")]
        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
        }

        [DisplayName("角度"), Category("布局")]
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        [DisplayName("支持的编辑类型"), Category("设计")]
        public enumEditType SupportedEditType
        {
            get { return _editType; }
        }

        private void SetGroupLocationFromElements()
        {
            ISizableElement sizeEle;
            IElement[] eles = _elements.ToArray();
            if (eles == null)
                return;
            if ((eles[0] as ISizableElement) == null)
                return;
            float minX = (eles[0] as ISizableElement).Location.X;
            float minY = (eles[0] as ISizableElement).Location.Y;
            for (int i = 1; i < eles.Length; i++)
            {
                sizeEle = eles[i] as ISizableElement;
                if (sizeEle == null)
                    continue;
                if (sizeEle.Location.X < minX)
                    minX = sizeEle.Location.X;
                if (sizeEle.Location.Y < minY)
                    minY = sizeEle.Location.Y;
            }
            _location.X = minX;
            _location.Y = minY;
        }

        private void SetGroupSizeFromElements()
        {
            ISizableElement sizeEle;
            float maxRX = 0f;
            float maxRY = 0f;
            foreach (IElement element in _elements)
            {
                sizeEle = element as ISizableElement;
                if (sizeEle == null)
                    continue;
                if (sizeEle.Location.X + sizeEle.Size.Width > maxRX)
                    maxRX = sizeEle.Location.X + sizeEle.Size.Width;
                if (sizeEle.Location.Y + sizeEle.Size.Height > maxRY)
                    maxRY = sizeEle.Location.Y + sizeEle.Size.Height;
            }
            _size.Width = maxRX - _location.X;
            _size.Height = maxRY - _location.Y;
        }

        public virtual void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
        }

        public virtual void ApplyRotate(float angleDegree)
        {
            throw new NotImplementedException();
        }

        public virtual void ApplyLocation(float layoutOffsetX, float layoutOffsetY)
        {
            IElement[] elements = _elements.ToArray();
            if (elements == null || elements.Length == 0)
                return;

            ISizableElement sizeEle;
            for (int i = 0; i < elements.Length; i++)
            {
                sizeEle = elements[i] as ISizableElement;
                if (sizeEle == null || sizeEle.IsSelected == true)
                    continue;
                sizeEle.ApplyLocation(layoutOffsetX, layoutOffsetY);
            }
            SetGroupLocationFromElements();
            SetGroupSizeFromElements();
            //_location.X += layoutOffsetX;
            //_location.Y += layoutOffsetY;
        }

        public virtual void ApplyLocationByItemSelected(float layoutOffsetX, float layoutOffsetY)
        {
            SetGroupLocationFromElements();
            SetGroupSizeFromElements();
        }

        public virtual void ApplySize(float layoutXSize, float layoutYSize)
        {
            IElement[] elements = _elements.ToArray();
            if (elements == null || elements.Length == 0)
                return;
            ISizableElement sizeEle;
            for (int i = 0; i < elements.Length; i++)
            {
                sizeEle = elements[i] as ISizableElement;
                if (sizeEle == null)
                    continue;
                sizeEle.ApplySize(layoutXSize, layoutYSize);
            }
            SetGroupLocationFromElements();
            SetGroupSizeFromElements();
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

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            foreach (ISizableElement ele in _elements)
                ele.Render(sender, drawArgs);
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("size") != null)
                _size = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("size").Value);
            if (xml.Attribute("location") != null)
                _location = (PointF)LayoutFromFile.Base64StringToObject(xml.Attribute("location").Value);
            base.InitByXml(xml);
        }
    }
}
