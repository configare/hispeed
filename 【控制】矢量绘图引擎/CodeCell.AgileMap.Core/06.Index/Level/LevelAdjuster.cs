using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;


namespace CodeCell.AgileMap.Core
{
    public class LevelAdjuster:IDisposable
    {
        protected List<Feature> _features = null;
        protected int _gridSize = 20;
        protected int _beginLevel = 0;
        internal const string cstLevelField = "DisplayLevel";
        protected const float cstMetersPerInch = 2.54f / 100f;
        protected const int cstDPI = 96;
        protected const int cstMinGridSize = 5;
        protected List<Shape> _geometries = null;
        private int _uncheckedCount = 0;
        private int _fieldIndexOfLevel = -1;
        private QuickTransformArgs _quickTranArgs = null;

        public LevelAdjuster()
        {
        }

        public Feature[] Features
        {
            get { return _features != null ?_features.ToArray() : null; }
            set
            {
                if(value != null && value.Length>0)
                    _features = new List<Feature>(value); 
            }
        }

        public int GridSize
        {
            get { return _gridSize; }
            set 
            {
                _gridSize = value; 
            }
        }

        public int BeginLevel
        {
            get { return _beginLevel; }
            set 
            {
                if (value < 1 || value > 15)
                    return;
                _beginLevel = value; 
            }
        }

        public  bool Do()
        {
            Envelope fullEnvelope = null;
            //检查是否有DisplayLevel字段
            CheckAndAddLevelField(out fullEnvelope);
            _fieldIndexOfLevel = _features[0].IndexOfField(cstLevelField);
            //如果是地理坐标，则转换为投影坐标
            TryToProjectCoord(ref fullEnvelope);
            //
            Matrix transform = new Matrix();
            Size size = Size.Empty;
            try
            {
                _uncheckedCount = _features.Count;
                for (int iLevel = _beginLevel; iLevel < LevelDefinition.LevelItems.Length; iLevel++)
                {
                    if (_uncheckedCount == 0)
                        return true;
                    float scale = LevelDefinition.GetScaleByLevel(iLevel);
                    bool isOK = UpdateTransfrom(scale, fullEnvelope, ref transform,ref size);
                    if (!isOK)
                        continue;
                    isOK = CheckByPerGrid(iLevel,size, transform);
                    if (!isOK)
                        break;
                }
                //已经到达最后一级仍然剩余的
                if (_uncheckedCount > 0)
                {
                    foreach (Feature fet in _features)
                    {
                        if(!fet.TempFlag)
                        {
                            fet.SetFieldValue(cstLevelField, LevelDefinition.LevelItems[LevelDefinition.LevelItems.Length - 1].Level.ToString());
                            fet.ResetDisplayLevel();
                            fet.TempFlag = true;
                        }
                    }
                }
                //
                return true;
            }
            finally 
            {
                transform.Dispose();
                //如果对几何形状进行过重投影，则恢复
                if (_geometries != null)
                {
                    for (int i = 0; i < _features.Count; i++)
                        _features[i].Geometry = _geometries[i];
                }
            }
        }

        private bool CheckByPerGrid(int iLevel,Size size, Matrix transform)
        {
            int rowCount = (int)Math.Ceiling((float)size.Height / _gridSize);
            int colCount = (int)Math.Ceiling((float)size.Width / _gridSize);
            bool[,] grids = null;
            try
            {
                grids = new bool[rowCount, colCount];
            }
            catch 
            {
                return false;
            }
            //逐个要素计算所在的网格
            PointF screenPt = new PointF();
            Feature fet = null;
            ComputeQuickArgs(transform);
            for (int i = _features.Count - 1; i >= 0; i--)
            {
                fet = _features[i];
                ShapePoint pt = fet.Geometry.Centroid;
                screenPt.X = (float)(pt.X * _quickTranArgs.kLon + _quickTranArgs.bLon);
                screenPt.Y = (float)(pt.Y * _quickTranArgs.kLat + _quickTranArgs.bLat);
                //
                int c = Math.Min((int)Math.Ceiling(screenPt.X / _gridSize), colCount - 1);
                int r = Math.Min((int)Math.Ceiling(screenPt.Y / _gridSize), rowCount - 1);
                //
                #region debug
                //string name = fet.GetFieldValue(0);
                //if ((name == "嘉峪关市" || name == "酒泉市"))
                //{
                //    Console.WriteLine("");
                //}
                #endregion
                //如果某个网内只有一个要素,则该要素的显示级别为iLevel,否则可以按优先字段决定(暂时不实现)
                //目前实现，只要有一个已经占了该网格，其余的不允许占
                if (grids[r, c])
                    continue;
                else
                {
                    grids[r, c] = true;
                    if (!fet.TempFlag)
                    {
                        fet.SetFieldValue(_fieldIndexOfLevel, iLevel.ToString());
                        fet.ResetDisplayLevel();
                        fet.TempFlag = true;
                        _uncheckedCount--;
                    }
                }
            }
            return true;
        }

        private void ComputeQuickArgs(Matrix transform)
        {
            if (_quickTranArgs == null)
                _quickTranArgs = new QuickTransformArgs();
            ShapePoint geoPt1 = _features[0].Geometry.Centroid;
            ShapePoint geoPt2 = _features[_features.Count - 1].Geometry.Centroid;
            PointF prjPt1 = geoPt1.ToPointF();
            PointF prjPt2 = geoPt2.ToPointF();
            PointF[] screenPts = new PointF[] { prjPt1, prjPt2 };
            transform.TransformPoints(screenPts);
            //
            _quickTranArgs.kLon = (screenPts[0].X - screenPts[1].X) / (prjPt1.X - prjPt2.X);
            _quickTranArgs.kLat = (screenPts[0].Y - screenPts[1].Y) / (prjPt1.Y - prjPt2.Y);
            _quickTranArgs.bLon = screenPts[0].X - _quickTranArgs.kLon * prjPt1.X;
            _quickTranArgs.bLat = screenPts[0].Y - _quickTranArgs.kLat * prjPt1.Y;
        }

        private void TryToProjectCoord(ref Envelope fullEnvelope)
        {
            if (fullEnvelope.MinX>=-180 && fullEnvelope.MaxX <=180 && fullEnvelope.MinY>=-90 && fullEnvelope.MaxY<=90 && fullEnvelope.Width <= 360 && fullEnvelope.Height <= 180)
            {
                fullEnvelope = null;
                IProjectionTransform prj = ProjectionTransformFactory.GetDefault();
                _geometries = new List<Shape>(_features.Count);
                foreach (Feature fet in _features)
                {
                    //保留未投影前的几何形状对象
                    _geometries.Add(fet.Geometry.Clone() as Shape);
                    //重投影
                    fet.Geometry.Project(prj);
                    //计算外接矩形
                    if (fullEnvelope == null)
                        fullEnvelope = fet.Geometry.Envelope.Clone() as Envelope;
                    else
                        fullEnvelope.UnionWith(fet.Geometry.Envelope);
                }
            }
        }

        private bool UpdateTransfrom(float scale,Envelope fullEnvelope, ref Matrix transform,ref Size size)
        {
            transform.Reset();
            double metersPerPixel = (1f / cstDPI) * cstMetersPerInch;
            double dwidth = (1f / scale) * fullEnvelope.Width / metersPerPixel;
            double dheight = (1f / scale) * fullEnvelope.Height / metersPerPixel;
            size.Width = (int)dwidth;
            size.Height = (int)dheight;
            if (size.Width < _gridSize || size.Height < _gridSize)
                return false;
            double xs = size.Width / fullEnvelope.Width;
            double ys = size.Height / fullEnvelope.Height;
            double s = Math.Min(xs, ys);
            transform.Scale((float)s, -(float)s);
            transform.Translate(-(float)fullEnvelope.MinX, -(float)fullEnvelope.MaxY);
            return true;
        }

        private void CheckAndAddLevelField(out Envelope envelope)
        {
            envelope = _features[0].Geometry.Envelope.Clone() as Envelope;
            string fld = cstLevelField.ToUpper();
            foreach (Feature fet in _features)
            {
                fet.TempFlag = false;
                envelope.UnionWith(fet.Geometry.Envelope);
                if (fet.FieldNames == null || fet.FieldNames.Length == 0)
                {
                    fet.SetFieldAndValues(new string[] { fld }, new string[] { "-1" });
                    continue;
                }
                bool exist = false;
                for (int i = 0; i < fet.FieldNames.Length; i++)
                {
                    if (fet.FieldNames[i].ToUpper() == fld)
                    {
                        fet.SetFieldValue(i, "-1");
                        exist = true;
                        break;
                    }
                }
                if(!exist)
                {
                    string[] flds = new string[fet.FieldNames.Length + 1];
                    string[] vals = new string[fet.FieldValues.Length + 1];
                    Array.Copy(fet.FieldNames, flds, fet.FieldNames.Length);
                    Array.Copy(fet.FieldValues, vals, fet.FieldValues.Length);
                    flds[flds.Length - 1] = fld;
                    vals[vals.Length - 1] = "-1";
                    fet.SetFieldAndValues(flds, vals);
                }
            }
        }

        public void Dispose()
        {
            if (_features != null)
            {
                _features.Clear();
                _features = null;
            }
        }
    }
}
