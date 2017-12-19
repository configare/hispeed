using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;
using System.Xml;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /*
    本库实现气象局国家气象卫星中心遥感应用室提出的指数增强公式：
    l=255*(lc-lmin)²/(lmax-lmin)²
    其中lc为待增强灰度值
    lmax、lmin为增强范围的上下限
    lmin = (x/2)-(y/4)
    lmax = (x/2)+(y/4)
    这里x、y为在一个255*255大小的图像上移动鼠标时候的x、y位置[0,255]
    */
    /// </summary>
    internal class NSMCExpEnhanceArg : RgbProcessorArg
    {
        private Point _rPoint = new Point();
        private Point _gPoint = new Point();
        private Point _bPoint = new Point();
        private int _maxValue = 512;
        private byte[] _r = new byte[256];
        private byte[] _g = new byte[256];
        private byte[] _b = new byte[256];

        public NSMCExpEnhanceArg()
            : base()
        {
            _r = new byte[256];
            _g = new byte[256];
            _b = new byte[256];
            for (int i = 0; i <= 255; i++)
            {
                _r[i] = (byte)i;
                _g[i] = (byte)i;
                _b[i] = (byte)i;
            }
        }

        public Point RedControlPoint
        {
            get { return _rPoint; }
            set
            {
                _rPoint = value;
                _r = UpdateRgbProcess(_rPoint);
            }
        }

        public Point GreenControlPoint
        {
            get { return _gPoint; }
            set
            {
                _gPoint = value;
                _g = UpdateRgbProcess(_gPoint);
            }
        }

        public Point BlueControlPoint
        {
            get { return _bPoint; }
            set
            {
                _bPoint = value;
                _b = UpdateRgbProcess(_bPoint);
            }
        }

        public byte[] RedAdjustedValues
        {
            get
            {
                return _r;
            }
        }

        public byte[] GreenAdjustedValues
        {
            get
            {
                return _g;
            }
        }

        public byte[] BlueAdjustedValues
        {
            get
            {
                return _b;
            }
        }

        private byte[] UpdateRgbProcess(Point point)
        {
            if (point.Y == 0)
                return null;
            int lmin = (point.X / 2) - (point.Y / 4);
            int lmax = (point.X / 2) + (point.Y / 4);
            if (lmax == lmin)
                return null;
            if (lmin < 0)
                lmin = 0;
            if (lmax > _maxValue - 1)
                lmax = _maxValue - 1;
            byte[] r = new byte[_maxValue];
            for (int i = 0; i <= 255; i++)
            {
                double val = (255f * ((i - lmin) * (i - lmin)) / ((lmax - lmin) * (lmax - lmin)));
                if (val < 0)
                    r[i] = 0;
                else if (val > 255)
                    r[i] = 255;
                else
                    r[i] = (byte)val;
            }
            return r;
        }

        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            XmlElement xmlElem = xmldoc.CreateElement("Arguments");
            XmlElement subRElem = xmldoc.CreateElement("Red");
            subRElem.InnerText = string.Format("{0},{1}", _rPoint.X, _rPoint.Y);
            XmlElement subGElem = xmldoc.CreateElement("Green");
            subGElem.InnerText = string.Format("{0},{1}", _gPoint.X, _gPoint.Y);
            XmlElement subBElem = xmldoc.CreateElement("Blue");
            subBElem.InnerText = string.Format("{0},{1}", _bPoint.X, _bPoint.Y);
            xmlElem.AppendChild(subRElem);
            xmlElem.AppendChild(subGElem);
            xmlElem.AppendChild(subBElem);
            return xmlElem;
        }

        public override RgbProcessorArg Clone()
        {
            NSMCExpEnhanceArg arg = new NSMCExpEnhanceArg();
            arg.RedControlPoint = _rPoint;
            arg.GreenControlPoint = _gPoint;
            arg.BlueControlPoint = _bPoint;
            return arg;
        }
    }
}
