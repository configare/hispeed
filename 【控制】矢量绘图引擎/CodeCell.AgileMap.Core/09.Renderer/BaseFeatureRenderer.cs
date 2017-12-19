using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Runtime.InteropServices;
using CodeCell.Bricks.Runtime;
using System.Diagnostics;


namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public abstract class BaseFeatureRenderer : IFeatureRenderer, IDisposable,IPersistable
    {
        [NonSerialized]
        internal IFeatureRenderEnvironment _environment = null;
        [NonSerialized]
        protected GraphicsPath _path = new GraphicsPath();
        protected ISymbol _currentSymbol = null;
        protected RotateFieldDef _rotateFieldDef = null;
        [NonSerialized]
        protected QuickTransformArgs _quickTransformArgs = null;
        internal string _name = string.Empty;
        //记下本次绘制要素的矩形区域
        internal Dictionary<Feature, RectangleF> _currentFeatureRects = new Dictionary<Feature, RectangleF>();

        public BaseFeatureRenderer()
        {
            _path = new GraphicsPath(); ;
        }

        internal void SetFeatureLayerEnvironment(IFeatureRenderEnvironment environment)
        {
            _environment = environment;
        }


        #region IFeatureRenderer Members

        [DisplayName("角度字段设置"), Description("本系统采用X轴正方向顺时针方向为0度,offset用于修正0度不同的情况。"), TypeConverter(typeof(ExpandableObjectConverter))]
        public RotateFieldDef RotateFieldDef
        {
            get { return _rotateFieldDef; }
            set { _rotateFieldDef = value; }
        }

        [Browsable(false)]
        public ISymbol CurrentSymbol
        {
            get { return _currentSymbol; }
        }

        public void Render(bool enableDisplayLevel, QuickTransformArgs quickTransformArgs, IGrid gd, Graphics g, Envelope rect, RepeatFeatureRecorder recorder)
        {
            if (g == null)
                return;
            _currentFeatureRects.Clear();
            List<Feature> features = gd.VectorFeatures;
            if (features == null || features.Count == 0)
                return;
            _quickTransformArgs = quickTransformArgs;
            int fetCount = features.Count;
            Feature feature = null;
            IOutsideIndicator outsideIndicator = null;
            bool isfullInside = (gd as Grid)._isfullInternal;
            for (int i = 0; i < fetCount; i++)
            {
                feature = features[i];
                if (feature == null || feature.Geometry == null)
                    continue;
                outsideIndicator = feature.OutsideIndicator;// (feature as ISupportOutsideIndicator).OutsideIndicator;
                //判断是否在该级别显示
                if (enableDisplayLevel && feature.DisplayLevel != -1)
                {
                    if (feature.DisplayLevel > _environment.CurrentLevel)
                    {
                        outsideIndicator.SetOutside(true);
                        continue;
                    }
                }
                //可见性计算
                if (!isfullInside)
                {
                    if (feature.Geometry is ShapePoint)
                        outsideIndicator.SetOutside(!IsPointInCurrentExtent(feature, rect));
                    else //Polyline,Polygon
                        outsideIndicator.SetOutside(!IsPolygonInCurrentExtent(feature, rect));
                    if (outsideIndicator.IsOutside)
                        continue;
                }
                else//整个网格都在可视区域内部
                {
                    outsideIndicator.SetOutside(false);
                }
                //是否已经在其他网格绘制过了
                if (feature.IsRepeatedOverGrids)
                {
                    if (recorder.IsRendered(feature.OID))
                        continue;
                    else
                        recorder.AddRenderedOID(feature.OID);
                }
                //为要素设置绘制符号
                SetCurrentSymbolFromFeature(feature);
                if (_currentSymbol == null)
                    continue;
                //为符号设置角度
                SetAngleOfSymbol(feature);
                //画几何形状
                try
                {
                    if (feature.Geometry is ShapePoint)
                    {
                        PointF pt = ShapePoint2PixelPoint(feature.Geometry as ShapePoint);
                        //位置冲突检测
                        if (_environment != null)
                        {
                            IConflictor conflictorForSymbol = _environment.ConflictorForSymbol;
                            if (conflictorForSymbol != null && conflictorForSymbol.Enabled)
                            {
                                //符号冲突
                                if (conflictorForSymbol.IsConflicted(pt.X - _currentSymbol.SymbolHalfWidth,
                                                                        pt.Y - _currentSymbol.SymbolHalfHeight,
                                                                        _currentSymbol.SymbolSize))
                                {
                                    outsideIndicator.SetOutside(true);
                                    continue;
                                }
                            }
                        }
                        //画符号
                        DrawPointUseCurrentSymbol(g, pt);
                        //记下要素符号矩形区域
                        _currentFeatureRects.Add(feature, new RectangleF((_currentSymbol as IMarkerSymbol).PixelLocation, _currentSymbol.SymbolSize));
                        //符号不冲突，在标注冲突检测网格中占位
                        #region debug
                        //符号矩形
                        //PointF tpt = new PointF((pt.X - _currentSymbol.SymbolSize.Width / 2f),
                        //                                      (pt.Y - _currentSymbol.SymbolSize.Height / 2f));
                        //(_environment.ConflictorForLabel as PixelConflictor).DrawTestRectangleF(tpt, _currentSymbol.SymbolSize, Color.Yellow);
                        #endregion 
                        if (_environment != null)
                        {
                            if (_environment.ConflictorForLabel != null && _environment.ConflictorForLabel.Enabled)
                            {
                                _environment.ConflictorForLabel.HoldPosition((pt.X - _currentSymbol.SymbolSize.Width / 2f),
                                                                                (pt.Y - _currentSymbol.SymbolSize.Height / 2f),
                                                                                _currentSymbol.SymbolSize);
                            }
                        }
                    }
                    else //Polyline,Polygon
                    {
                        SetGraphicsPathFromGeometry(feature.Geometry);
                        if (_path.PointCount > 0)
                        {
                            DrawPathUseCurrentSymbol(g);
                            _currentFeatureRects.Add(feature,RectangleF.Empty);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.WriterException("BaseFeatureRenderer", "Render(Layer:"+(_name != null ?_name : string.Empty)+",OID:"+feature.OID.ToString()+")", ex);
                }
                finally
                {
                    _currentSymbol.Angle = 0;
                }
            }
        }

        protected virtual void DrawPathUseCurrentSymbol(Graphics g)
        {
            (_currentSymbol as Symbol).Render(g, _path);
        }

        protected virtual void DrawPointUseCurrentSymbol(Graphics g, PointF pt)
        {
            (_currentSymbol as Symbol).Render(g, pt);
        }

        private void SetAngleOfSymbol(Feature feature)
        {
            if (_rotateFieldDef == null || _rotateFieldDef.RotateField == null)
                return;
            string strAngle = feature.GetFieldValue(_rotateFieldDef.RotateField);
            float angle = 0;
            if (float.TryParse(strAngle, out angle))
            {
                _currentSymbol.Angle = angle + _rotateFieldDef.Offset;
            }
        }

        protected abstract void SetCurrentSymbolFromFeature(Feature feature);

        private void SetGraphicsPathFromGeometry(Shape shape)
        {
            if (_path == null)
                _path = new GraphicsPath();
            _path.Reset();
            if (shape is ShapePolyline)
            {
                PolylineToPath(shape as ShapePolyline);
            }
            else if (shape is ShapePolygon)
            {
                PolygonToPath(shape as ShapePolygon);
            }
            else
            {
                throw new NotSupportedException("暂时不支持几何类型\"" + shape.ToString() + "\"的绘制!");
            }
        }

        #endregion

        private bool IsPointInCurrentExtent(Feature fet, Envelope rect)
        {
            ShapePoint pt = fet.Geometry as ShapePoint;
            return !(pt.X < rect.MinX || pt.X > rect.MaxX ||
                        pt.Y < rect.MinY || pt.Y > rect.MaxY);
        }

        private PointF ShapePoint2PixelPoint(ShapePoint shapePoint)
        {
            PointF ptf = new PointF();
            ptf.X = (float)(_quickTransformArgs.kLon * shapePoint.X + _quickTransformArgs.bLon);
            ptf.Y = (float)(_quickTransformArgs.kLat * shapePoint.Y + _quickTransformArgs.bLat);
            return ptf;
        }

        private bool IsPolygonInCurrentExtent(Feature fet, Envelope rect)
        {
            Envelope ep = fet.Geometry.Envelope;
            return !(ep.MaxX < rect.MinX || ep.MinX > rect.MaxX ||
                        ep.MaxY < rect.MinY || ep.MinY > rect.MaxY);
        }

        private unsafe void PolylineToPath(ShapePolyline ply)
        {
            int partCount = ply.Parts.Length;
            for (int pi = 0; pi < partCount; pi++)
            {
                ShapePoint[] prjPts = ply.Parts[pi].Points;
                PointF[] ptfs = new PointF[prjPts.Length];
                for (int i = 0; i < ptfs.Length; i++)
                {
                    ptfs[i].X = (float)(_quickTransformArgs.kLon * prjPts[i].X + _quickTransformArgs.bLon);
                    ptfs[i].Y = (float)(_quickTransformArgs.kLat * prjPts[i].Y + _quickTransformArgs.bLat);
                }
                _path.AddLines(ptfs);
                _path.StartFigure();
            }
        }

        /// <summary>
        /// 使用斜率截距快速算法
        /// 以全球国家为例：220毫秒
        /// </summary>
        /// <param name="ply"></param>
        private unsafe void PolygonToPath(ShapePolygon ply)
        {
            float kLon = _quickTransformArgs.kLon;
            float kLat = _quickTransformArgs.kLat;
            float bLon = _quickTransformArgs.bLon;
            float bLat = _quickTransformArgs.bLat;
            int ringCount = ply.Rings.Length;
            for (int ri = 0; ri < ringCount; ri++)
            {
                ShapePoint[] prjPts = ply.Rings[ri].Points;
                int nCount = prjPts.Length ;
                PointF[] pts = new PointF[nCount];
                fixed (PointF* ptr0 = pts)
                {
                    PointF* ptr = ptr0;
                    for (int i = 0; i < nCount; i++,ptr++)
                    {
                        ptr->X = (float)(kLon * prjPts[i].X + bLon);
                        ptr->Y = (float)(kLat * prjPts[i].Y + bLat);
                    }
                }
                _path.AddPolygon(pts);
            }
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (_path != null)
            {
                _path.Dispose();
                _path = null;
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Empty;
        }

        #region IPersistable Members

        public abstract PersistObject ToPersistObject();

        #endregion
    }
}
