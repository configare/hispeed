using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.CA;
using System.ComponentModel.Composition;
using System.Xml;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    /*
    本库实现气象局国家气象卫星中心遥感应用室提出的指数增强公式：
    l=255*(lc-lmin)²/(lmax-lmin)²
    其中lc为待增强灰度值
    lmax、lmin为增强范围的上下限
    lmin = (x/2)-(y/4)
    lmax = (x/2)+(y/4)
    这里x、y为在一个255*255大小的图像上移动鼠标时候的x、y位置
    */
    [Export(typeof(IRgbProcessor))]
    public class NSMCExpEnhanceProcessor : RgbProcessorByPixel
    {
        NSMCExpEnhanceArg _expArg;

        public NSMCExpEnhanceProcessor():
            base()
        {
            _name = "NSMC指数增强";
        }

        public NSMCExpEnhanceProcessor(RgbProcessorArg arg)
            : base(arg)
        {
            _name = "NSMC指数增强";
        }

        public NSMCExpEnhanceProcessor(string name, RgbProcessorArg arg)
            : base(name, arg)
        {
            _name = "NSMC指数增强";
        }
        
        protected override void BeforeProcess()
        {
            base.BeforeProcess();
            _expArg = _arg as NSMCExpEnhanceArg;

        }

        protected override void Process(ref byte pixelValue)
        {
            if (_expArg != null)
            {
                pixelValue = _expArg.RedAdjustedValues[pixelValue];
            }
        }

        protected override void Process(ref byte pixelBlue, ref byte pixelGreen, ref byte pixelRed)
        {
            if (_expArg != null)
            {
                if (_expArg.BlueAdjustedValues != null)
                    pixelBlue = _expArg.BlueAdjustedValues[pixelBlue];
                if (_expArg.GreenAdjustedValues != null)
                    pixelGreen = _expArg.GreenAdjustedValues[pixelGreen];
                if (_expArg.RedAdjustedValues != null)
                    pixelRed = _expArg.RedAdjustedValues[pixelRed];
            }
        }

        public override System.Xml.XmlElement ToXML(System.Xml.XmlDocument xmldoc)
        {
            if (_expArg == null)
                _expArg = new NSMCExpEnhanceArg();
            return _expArg.ToXML(xmldoc);
        }

        public static RgbProcessor FromXML(XmlElement ele)
        {
            NSMCExpEnhanceArg arg = new NSMCExpEnhanceArg();
            for(int i=0;i<ele.ChildNodes.Count;i++)
            {
                XmlNode node = ele.ChildNodes[i];
                switch (node.Name)
                {
                    case "Red":
                        arg.RedControlPoint = GetPoint(node.InnerText);
                        break;
                    case "Green":
                        arg.GreenControlPoint = GetPoint(node.InnerText);
                        break;
                    case "Blue":
                        arg.BlueControlPoint = GetPoint(node.InnerText);
                        break;
		            default:
                        break;
	            }
            }
            NSMCExpEnhanceProcessor pro = new NSMCExpEnhanceProcessor(arg);
            return pro;
        }

        private static System.Drawing.Point GetPoint(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                return Point.Empty;
            string[] pts = p.Split(',');
            if (pts.Length != 2)
                return Point.Empty;
            int x, y;
            if (int.TryParse(pts[0], out x) && int.TryParse(pts[1], out y))
                return new Point(x, y);
            else
                return Point.Empty;
        }

        public override void CreateDefaultArguments()
        {
            _arg = new NSMCExpEnhanceArg();
        }
    }
}
