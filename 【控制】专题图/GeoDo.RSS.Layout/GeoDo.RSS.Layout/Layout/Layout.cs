using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public class Layout : ElementGroup, ILayout, IPersitable
    {
        protected enumLayoutUnit _unit;
        private SizeF _size;

        public Layout()
            : base()
        {
            _unit = enumLayoutUnit.Pixel;
        }

        public Layout(enumLayoutUnit unit)
            : base()
        {
            _unit = unit;
        }

        [Persist()]
        public enumLayoutUnit Unit
        {
            get { return _unit; }
        }

        [Persist(enumAttType.UnValueType)]
        public SizeF Size
        {
            get
            {
                IBorder border = GetBorder();
                if (border != null)
                {
                    _size = border.Size;
                    return _size;
                }
                return SizeF.Empty;
            }
            set
            {
                IBorder border = GetBorder();
                if (border != null)
                {
                    if (_size != value)
                    {
                        //TryAdjustRelativeLocation(border, value);
                        _size = value;
                        border.Size = _size;
                    }
                }
            }
        }

        private void TryAdjustRelativeLocation(IBorder border, SizeF newSize)
        {
            float preX = border.Location.X + border.Size.Width;
            float preY = border.Location.Y + border.Size.Height;
            foreach (IElement ele in _elements)
            {
                if (ele is ISizableElement)
                    TryComputeElementLocation(ele as ISizableElement, preX, preY, newSize);
            }
        }

        private void TryComputeElementLocation(ISizableElement ele, float preX, float preY, SizeF newSize)
        {
            if (ele is IBorder)
                return;
            if (ele is IDataFrame)
                TryAdjustDataFrameSize(ele as IDataFrame, preX, preY, newSize);
            if (ele is ISizableElementGroup)
            {
                foreach (ISizableElement element in (ele as ISizableElementGroup).Elements)
                    TryComputeElementLocation(element, preX, preY, newSize);
            }
            if (ele.Location.X > (preX - ele.Location.X)) //右停靠
                ele.Location = new PointF(newSize.Width - (preX - ele.Location.X), ele.Location.Y);
            if (ele.Location.Y > (preY - ele.Location.Y)) //下停靠
                ele.Location = new PointF(ele.Location.X, newSize.Height - (preY - ele.Location.Y));
            if (Math.Abs(ele.Location.X - (preX - ele.Location.X - ele.Size.Width)) < 0.0001) //左右居中
                ele.Location = new PointF(newSize.Width / 2f - ele.Size.Width / 2f, ele.Location.Y);
            if (Math.Abs(ele.Location.Y - (preY - ele.Location.Y - ele.Size.Height)) < 0.0001) //上下居中
                ele.Location = new PointF(ele.Location.X, newSize.Height / 2f - ele.Size.Height / 2f);
        }

        private void TryAdjustDataFrameSize(IDataFrame df, float preX, float preY, SizeF newSize)
        {
            float blankX = preX - df.Location.X - df.Size.Width; // 右边距
            float blankY = preY - df.Location.Y - df.Size.Height; //下边距
            //
            float wid = newSize.Width - df.Location.X - blankX;
            float hei = newSize.Height - df.Location.Y - blankY;
            df.Size = new SizeF(wid, hei);
        }

        public IBorder GetBorder()
        {
            foreach (IElement ele in _elements)
                if (ele is IBorder)
                    return ele as IBorder;
            return null;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("unit") != null)
            {
                string s = xml.Attribute("unit").Value;
                switch (s.ToLower())
                {
                    case "pixel":
                        _unit = enumLayoutUnit.Pixel;
                        break;
                    case "centimeter":
                        _unit = enumLayoutUnit.Centimeter;
                        break;
                }
            }
            if (xml.Attribute("size") != null)
            {
                _size = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("size").Value);
            }
            base.InitByXml(xml);
        }

        public IElement[] QueryElements(Func<IElement, bool> filter)
        {
            List<IElement> eles = new List<IElement>();
            QueryElements(filter, this, eles);
            return eles.Count > 0 ? eles.ToArray() : null;
        }

        private void QueryElements(Func<IElement, bool> filter, IElement beginElement, List<IElement> eles)
        {
            if (beginElement is IElementGroup)
            {
                foreach (IElement sub in (beginElement as IElementGroup).Elements)
                    QueryElements(filter, sub, eles);
            }
            else
                if (filter(beginElement))
                    eles.Add(beginElement);
        }
    }
}
