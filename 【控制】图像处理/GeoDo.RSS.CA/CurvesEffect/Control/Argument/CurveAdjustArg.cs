using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 用于预设曲线调整方案
    /// </summary>
    public class CurveAdjustArg
    {
        public CurveAdjustArg(string text)
        {
            Text = text;
        }

        public string Text;
        /// <summary>
        /// 曲线类型
        /// 曲线：Spline
        /// 折线：Line
        /// </summary>
        public LineType LineType;

        public IDictionary<string, Point[]> ControlPoint;

        public static CurveAdjustArg[] DefaultCurves
        {
            get
            {
                CurveAdjustArg fx = new CurveAdjustArg("反相");
                fx.LineType = LineType.Line;
                fx.ControlPoint.Add("Rgb", new Point[] { new Point(0, 255), new Point(255, 0) });
                return new CurveAdjustArg[] { fx };
            }
        }

        public XElement ToXml()
        {
            return new XElement("CurveAdjustArg",
                new XElement("Text", Text),
                new XElement("LineType", LineType),
                ControlPointToEle());
        }

        /*
        <CurveAdjustArg>
            <Text>反相</Text>
            <LineType>Line</LineType>
            <Line>
                <Rgb>
                    <ControlPoint>0,255</ControlPoint>
                    <ControlPoint>255,0</ControlPoint>
                </Rgb>
            </Line>
        </CurveAdjustArg>
         */
        private XElement ControlPointToEle()
        {
            XElement x = new XElement("Line");
            foreach (string key in ControlPoint.Keys)
            {
                Point[] pts = ControlPoint[key];
                List<XElement> ptsX = new List<XElement>();
                foreach (Point pt in pts)
                {
                    ptsX.Add(new XElement("ControlPoint", string.Format("{0},{1}", pt.X, pt.Y)));
                }
                x.Add(new XElement("Key", key), ptsX.ToArray());
            }
            return x;
        }
    }

    public enum LineType
    {
        Line,
        Spline
    }
}
