#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-10 18:48:18
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    /// <summary>
    /// 类名：SelectMaskLayer
    /// 属性描述：标记层，用于绘制标记的点
    /// 创建者：罗战克   创建日期：2013-09-10 18:48:18
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SelectMaskLayer : Layer, IRenderLayer, IControlLayer, IAOILayer, ISelectMaskLayer
    {
        private bool _changing = false;
        private bool _visible = true;
        private Dictionary<int, CoordPoint> _maskPoints = new Dictionary<int, CoordPoint>();
        private Pen _pen = new Pen(Color.Cyan, 3);
        private Brush _brush = Brushes.Cyan;

        public Dictionary<int, CoordPoint> MaskPoints
        {
            get { return _maskPoints; }
        }

        public void Add(Dictionary<int, CoordPoint> points)
        {
            try
            {
                _changing = true;
                List<CoordPoint> pts = new List<CoordPoint>();
                foreach (int index in points.Keys)
                {
                    if (!_maskPoints.ContainsKey(index))
                    {
                        _maskPoints.Add(index, points[index]);
                    }
                }
            }
            finally
            {
                _changing = false;
            }
        }

        public void Remove(Dictionary<int, CoordPoint> points)
        {
            try
            {
                _changing = true;
                List<CoordPoint> pts = new List<CoordPoint>();
                foreach (int index in points.Keys)
                {
                    if (_maskPoints.ContainsKey(index))
                    {
                        _maskPoints.Remove(index);
                    }
                }
            }
            finally
            {
                _changing = false;
            }
        }
        
        public void Clear()
        {
            try
            {
                _changing = true;
                _maskPoints.Clear();
            }
            finally
            {
                _changing = false;
            }
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_changing)
                return;
            if (_maskPoints.Count == 0)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            ICanvas canvas = sender as ICanvas;
            Point[] screenpts = RasterToScreen(_maskPoints.Values.ToArray(), canvas);
            foreach (PointF geo in screenpts)
            {
                //g.FillRectangle(_brush, geo.X - 2, geo.Y - 2, 4, 4);
                g.DrawRectangle(_pen, geo.X - 2, geo.Y - 2, 4, 4);
            }
        }

        private Point[] RasterToScreen(CoordPoint[] point, ICanvas canvas)
        {
            if (point == null || point.Length == 0)
                return null;
            double prjx, prjy;
            int x, y;
            int count = point.Length;
            Point[] pts = new Point[count];
            for (int i = 0; i < count; i++)
            {
                CoordPoint pt = point[i];
                canvas.CoordTransform.Geo2Prj(pt.X, pt.Y, out prjx, out prjy);
                canvas.CoordTransform.Prj2Screen(prjx, prjy, out x, out y);
                pts[i].X = x;
                pts[i].Y = y;
            }
            return pts;
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _maskPoints.Clear();
            _pen.Dispose();
            _brush.Dispose();
        }
    }
}
