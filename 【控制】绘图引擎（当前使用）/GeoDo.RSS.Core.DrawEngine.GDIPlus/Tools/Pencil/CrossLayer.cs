using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    /// <summary>
    /// 几何精校正 GCP点
    /// </summary>
    public class CrossLayer : Layer
    {
        protected bool _visible = false;
        protected bool _isMoved = false;
        protected List<PointF> _vertexts = new List<PointF>();
        private   Color _penColor = Color.Red;
        private   Pen _pen = new Pen(Color.Red);

        public CrossLayer()
        {
        }

        public bool IsMoved
        {
            get { return _isMoved; }
            set { _isMoved = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Reset()
        {
            _vertexts.Clear();
        }

        public void SetVertext(PointF pt)
        {
            if (_isMoved)
            {
                _vertexts.Add(new PointF(pt.X - 50, pt.Y));
                _vertexts.Add(new PointF(pt.X, pt.Y - 50));
                _vertexts.Add(new PointF(pt.X + 50, pt.Y));
                _vertexts.Add(new PointF(pt.X, pt.Y + 50));
            }
            else
            {
                _vertexts.Add(new PointF(pt.X - 15, pt.Y));
                _vertexts.Add(new PointF(pt.X, pt.Y - 15));
                _vertexts.Add(new PointF(pt.X + 15, pt.Y));
                _vertexts.Add(new PointF(pt.X, pt.Y + 15));
            }
        }

        public void SetVertext(float centerX, float centerY, float halfWidth, float halfHeight)
        {
            if (_isMoved)
            {
                _vertexts.Add(new PointF(centerX - halfWidth, centerY));
                _vertexts.Add(new PointF(centerX, centerY - halfHeight));
                _vertexts.Add(new PointF(centerX + halfWidth, centerY));
                _vertexts.Add(new PointF(centerX, centerY + halfHeight));
            }
            else
            {
                _vertexts.Add(new PointF(centerX - halfWidth / 4, centerY));
                _vertexts.Add(new PointF(centerX, centerY - halfHeight / 4));
                _vertexts.Add(new PointF(centerX + halfWidth / 4, centerY));
                _vertexts.Add(new PointF(centerX, centerY + halfHeight / 4));
            }
        }

        public void SetVertext(float centerX, float centerY)
        {
             _vertexts.Add(new PointF(centerX, centerY));
        }

        public GeometryOfDrawed AddGeometry(bool hasPrimaryDrawObject)
        {
            using (GraphicsPath path = GetGraphicsPath())
            {
                if (path == null)
                    return null;

                GeometryOfDrawed geometry = new GeometryOfDrawed();
                geometry.ShapeType = enumPencilType.Cross.ToString();
                if (_vertexts.Count() == 4)
                {
                    geometry.RasterPoints = new PointF[] { new PointF(_vertexts[0].X, _vertexts[0].Y), new PointF(_vertexts[1].X, _vertexts[1].Y), 
                                                       new PointF(_vertexts[2].X, _vertexts[2].Y), new PointF(_vertexts[3].X, _vertexts[3].Y)};
                }
                else
                {
                    geometry.RasterPoints = new PointF[] { new PointF(_vertexts[0].X, _vertexts[0].Y)};
                }
                geometry.Types = path.PathData.Types;
                if (!hasPrimaryDrawObject)
                {
                    geometry.IsPrjCoord = true;
                }
                if (IsMoved)
                {
                    geometry.ControlRasterPoints = new PointF[1];
                }
                else
                {
                    geometry.ControlRasterPoints = new PointF[0];
                }
                return geometry;
            }
        }

        public GeometryOfDrawed AddGeometry(ICanvas canvas)
        {
            using (GraphicsPath path = GetGraphicsPath())
            {
                if (path == null)
                    return null;

                GeometryOfDrawed geometry = new GeometryOfDrawed();
                geometry.ShapeType = enumPencilType.Cross.ToString();
                geometry.RasterPoints = new PointF[] { new PointF(_vertexts[0].X, _vertexts[0].Y), new PointF(_vertexts[1].X, _vertexts[1].Y), 
                                                       new PointF(_vertexts[2].X, _vertexts[2].Y), new PointF(_vertexts[3].X, _vertexts[3].Y)};

                float row, col;
                double prjY, prjX;
                for (int i = 0; i < geometry.RasterPoints.Length; i++)
                {
                    if (canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
                    {
                        canvas.CoordTransform.Screen2Raster(geometry.RasterPoints[i].X, geometry.RasterPoints[i].Y, out row, out col);
                        geometry.RasterPoints[i].X = col;
                        geometry.RasterPoints[i].Y = row;
                    }
                    else//无活动影像时返回投影坐标
                    {
                        canvas.CoordTransform.Screen2Prj(geometry.RasterPoints[i].X, geometry.RasterPoints[i].Y, out prjX, out prjY);
                        geometry.RasterPoints[i].X = (float)prjX;
                        geometry.RasterPoints[i].Y = (float)prjY;
                        geometry.IsPrjCoord = true;
                    }
                }

                geometry.Types = path.PathData.Types;

                return geometry;
            }
        }


        private GraphicsPath GetGraphicsPath()
        {
            GraphicsPath path = new GraphicsPath();

            if (_vertexts.Count() == 4)
            {
                path.AddLine(_vertexts[0], _vertexts[2]);
                path.AddLine(_vertexts[1], _vertexts[3]);
            }
            else
            {
                path.AddLine(new PointF(_vertexts[0].X - 15, _vertexts[0].Y), new PointF(_vertexts[0].X + 15, _vertexts[0].Y));
                path.AddLine(new PointF(_vertexts[0].X, _vertexts[0].Y-15), new PointF(_vertexts[0].X, _vertexts[0].Y+15));
            }
            
            return path;
        }

        private Size GetSize(ICanvas canvas)
        {
            if (canvas == null || canvas.PrimaryDrawObject == null)
                return Size.Empty;
            return canvas.PrimaryDrawObject.Size;
        }

        [Browsable(false)]
        public Color PenColor
        {
            get { return _penColor; }
            set
            {
                if (_penColor != value)
                {
                    _penColor = value;
                    _pen = new Pen(value);
                }
            }
        }
    }
}
