using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading.Tasks;

namespace GeoDo.RSS.RasterTools
{
    internal abstract class ContourGenerator<T> : IContourGenerator, IDisposable
    {
        internal enum enumEdgeType
        {
            A,
            B,
            C
        }

        protected double _noData = 0;
        protected T[] _bandValues;
        protected int _gridWidth;
        protected int _gridHeight;
        protected int _triWidth;  //每行三角形个数
        protected int _width;
        protected int _height;
        protected double[] _contourValues;
        protected int _triangleCount;
        protected int[] _borderTriangles;
        protected int _edgeCount;
        protected IRasterBand _raster;
        protected StatusRecorder _triangleIsUsed;
        protected StatusRecorder _edgeIsUsed;
        protected List<ContourLine> _contourLines;
        protected bool _isOutputUncompleted = false;
        protected int _sample = 1;
        protected bool _isParallel = false;

        public ContourGenerator()
        {
        }

        public bool IsOutputUncompleted
        {
            get { return _isOutputUncompleted; }
            set { _isOutputUncompleted = value; }
        }

        public double NoDataForOutsideAOI
        {
            get { return _noData; }
            set { _noData = value; }
        }

        public int Sample
        {
            get { return _sample; }
            set { _sample = value; }
        }

        public double[] ContourValues
        {
            get { return _contourValues; }
        }

        public ContourLine[] Generate(IRasterBand raster, double[] contourValues, int[] aoi, Action<int, string> tracker)
        {
            Size oSize = new Size(raster.Width, raster.Height);
            bool isNewRaster = false;
            int offsetRow = 0, offsetCol = 0;
            if (aoi != null && aoi.Length > 0)
            {
                //感兴趣区域不采样
                _sample = 1;
                Rectangle rect = AOIHelper.ComputeAOIRect(aoi, oSize);
                offsetRow = rect.Top;
                offsetCol = rect.Left;
                raster = GetAOIRaster(raster, aoi, rect, oSize);
                isNewRaster = true;
            }
            _raster = raster;
            _width = raster.Width / _sample;
            _height = raster.Height / _sample;
            _gridWidth = _width - 1;
            _gridHeight = _height - 1;
            _triWidth = _gridWidth * 2;
            _contourValues = contourValues;
            _triangleCount = 2 * _gridWidth * _gridHeight;
            _triangleIsUsed = new StatusRecorder(_triangleCount);
            _edgeCount = _gridWidth * _height + _gridHeight * _width + _gridWidth * _gridHeight;
            _edgeIsUsed = new StatusRecorder(_edgeCount);
            try
            {
                if (tracker != null)
                    tracker(0, "正在准备数据...");
                ReadBandValues();
                if (tracker != null)
                    tracker(10, "正在提取边界三角形...");
                GetBorderTriangles();
                if (tracker != null)
                    tracker(12, "正在生成等值线...");
                _contourLines = new List<ContourLine>();
                GenerateContours(tracker);
                if (tracker != null)
                    tracker(100, "等值线生成完毕!");
            }
            finally
            {
                if (isNewRaster)
                {
                    OffsetToFull(_contourLines, offsetRow, offsetCol);
                    raster.Dispose();
                }
            }
            return _contourLines != null && _contourLines.Count > 0 ? _contourLines.ToArray() : null;
        }

        private void GenerateContours(Action<int, string> tracker)
        {
            int span = (100 - 17) / _contourValues.Length;
            for (int i = 0; i < _contourValues.Length; i++)
            {
                if (i != 0)
                {
                    _triangleIsUsed.Reset();
                    _edgeIsUsed.Reset();
                }
                if (tracker != null)
                    tracker(12 + i * span, "正在生成值为[" + _contourValues[i].ToString() + "]的开放式等值线...");
                GenerateUnclosedContours(_contourValues[i]);
                if (tracker != null)
                    tracker(12 + i * span + 5, "正在生成值为[" + _contourValues[i].ToString() + "]的封闭式等值线...");
                GenerateClosedContours(_contourValues[i]);
            }
        }

        Dictionary<int, VTriangle> crtCandidateTriansForUnclosed;
        double crtContourValue;
        private void GenerateUnclosedContours(double contourValue)
        {
            crtContourValue = contourValue;
            int tCount = _borderTriangles.Length;
            VTriangle tri = new VTriangle();
            crtCandidateTriansForUnclosed = new Dictionary<int, VTriangle>();
            for (int i = 0; i < tCount; i++)
            {
                tri = new VTriangle();
                ValIsContainedByTriangle(_borderTriangles[i], ToTValue(contourValue), ref tri);
                if (!tri.Is2Contained())
                    continue;
                if (crtCandidateTriansForUnclosed.ContainsKey(_borderTriangles[i]))//左下角和右上角的三角形是重复的
                    continue;
                crtCandidateTriansForUnclosed.Add(_borderTriangles[i], tri);
            }
            //并行算法
            if (_isParallel)
                Parallel.ForEach<int>(crtCandidateTriansForUnclosed.Keys, new Action<int>(ActionTrackUnclosedContour));
            else//串行算法
                foreach (int key in crtCandidateTriansForUnclosed.Keys)
                    ActionTrackUnclosedContour(key);
        }

        private void ActionTrackUnclosedContour(int idx)
        {
            ContourLine contLine = null;
            if (idx % _triWidth == 0)                 //left border
            {
                if (!crtCandidateTriansForUnclosed[idx].IsBContained)
                    return;
                contLine = TrackUnclosedContour(idx, enumEdgeType.B, crtContourValue);
            }
            else if ((idx + 1) % _triWidth == 0)      //right border
            {
                if (!crtCandidateTriansForUnclosed[idx].IsBContained)
                    return;
                contLine = TrackUnclosedContour(idx, enumEdgeType.B, crtContourValue);
            }
            else if (idx / _triWidth == 0)            //top border
            {
                if (!crtCandidateTriansForUnclosed[idx].IsAContained)
                    return;
                contLine = TrackUnclosedContour(idx, enumEdgeType.A, crtContourValue);
            }
            else if (idx / _triWidth == _gridHeight - 1)//bottom border
            {
                if (!crtCandidateTriansForUnclosed[idx].IsAContained)
                    return;
                contLine = TrackUnclosedContour(idx, enumEdgeType.A, crtContourValue);
            }
            if (contLine != null)
                _contourLines.Add(contLine);
        }

        private void GenerateClosedContours(double contourValue)
        {
            //串行算法
            if (_isParallel)
            {
                crtContourValue = contourValue;
                Parallel.For(0, _triangleCount, new Action<int>(ActionGenClosedContour));
            }
            else//并行算法
            {
                for (int i = 0; i < _triangleCount; i++)
                {
                    if (IsBorderTriangle(i))
                        continue;
                    ContourLine cLine = TrackClosedContour(i, enumEdgeType.A, contourValue);
                    if (cLine != null)
                        _contourLines.Add(cLine);
                }
            }
        }

        private void ActionGenClosedContour(int idx)
        {
            if (IsBorderTriangle(idx))
            {
                _triangleIsUsed.SetStatus(idx, true);
                return;
            }
            ContourLine cLine = TrackClosedContour(idx, enumEdgeType.A, crtContourValue);
            if (cLine != null)
                _contourLines.Add(cLine);
        }

        private ContourLine TrackUnclosedContour(int crtTriangle, enumEdgeType crtEdge, double contValue)
        {
            if (_triangleIsUsed.IsTrue(crtTriangle))
                return null;
            ContourLine cntLine = new ContourLine(contValue);
            UpdateClassIndex(cntLine);
            enumEdgeType firstEdge = crtEdge;
            int firstTriangle = crtTriangle;
            enumEdgeType[] otherEdges = new enumEdgeType[2];
            int edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
            bool edgeUsed = _edgeIsUsed.IsTrue(edgeIdx);
            PointF vpt = TryGetVPoint(crtTriangle, crtEdge, contValue);
            if (vpt.IsEmpty || edgeUsed)
                return null;
            _edgeIsUsed.SetStatus(edgeIdx, true);
            return TrackClosedContourFromEdge(crtTriangle, crtEdge, contValue, vpt,
                (idx) => { return IsBorderTriangle(idx); }, 2);
        }

        private ContourLine TrackClosedContour(int crtTriangle, enumEdgeType crtEdge, double contValue)
        {
            if (_triangleIsUsed.IsTrue(crtTriangle))
                return null;
            int edgeIdx = 0;
            enumEdgeType firstEdge = crtEdge;
            int firstTriangle = crtTriangle;
            enumEdgeType[] otherEdges = new enumEdgeType[2] { enumEdgeType.B, enumEdgeType.C };
            PointF vpt = PointF.Empty;
            vpt = TryGetVPoint(crtTriangle, crtEdge, contValue);
            edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
            if (vpt.IsEmpty || _edgeIsUsed.IsTrue(edgeIdx))
            {
                for (int i = 0; i < 2; i++)
                {
                    crtEdge = otherEdges[i];
                    vpt = TryGetVPoint(crtTriangle, crtEdge, contValue);
                    edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
                    if (!vpt.IsEmpty && !_edgeIsUsed.IsTrue(edgeIdx))
                        break;
                }
            }
            if (vpt.IsEmpty || _edgeIsUsed.IsTrue(edgeIdx))
            {
                //当前三角形三条边均不存在等值点，标记为已使用
                _triangleIsUsed.SetStatus(crtTriangle, true);
                return null;
            }
            return TrackClosedContourFromEdge(crtTriangle, crtEdge, contValue, vpt,
                (idx) => { return idx == firstTriangle || IsBorderTriangle(idx); }, 6);
        }

        private void UpdateClassIndex(ContourLine cntLine)
        {
            if (_contourValues == null || _contourValues.Length == 0)
                return;
            for (int i = 0; i < _contourValues.Length; i++)
            {
                if (Math.Abs(_contourValues[i] - cntLine.ContourValue) < double.Epsilon)
                {
                    cntLine.ClassIndex = i;
                    break;
                }
            }
        }

        private ContourLine TrackClosedContourFromEdge(int crtTriangle, enumEdgeType crtEdge, double contValue, PointF vpt, Func<int, bool> isStop, int minPoints)
        {
            bool isSharePointed = false;
            bool edgeUsed = false;
            ContourLine cntLine = new ContourLine(contValue);
            UpdateClassIndex(cntLine);
            enumEdgeType[] otherEdges = new enumEdgeType[2];
            int edgeIdx = 0;
            bool isClosed = false;
            while (true)
            {
                //等值线进入当前三角形
                if (!isSharePointed)
                    cntLine.AddPoint(vpt);
                //查找等值线离开当前三角形的出口点
                SetOthterEdges(crtEdge, ref otherEdges);
                crtEdge = otherEdges[0];
                edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
                edgeUsed = _edgeIsUsed.IsTrue(edgeIdx);
                vpt = !edgeUsed ? TryGetVPoint(crtTriangle, crtEdge, contValue) : PointF.Empty;
                if (vpt.IsEmpty)//该边不包含等值点或者已被使用
                {
                    crtEdge = otherEdges[1];
                    edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
                    edgeUsed = _edgeIsUsed.IsTrue(edgeIdx);
                    vpt = !edgeUsed ? TryGetVPoint(crtTriangle, crtEdge, contValue) : PointF.Empty;
                    if (vpt.IsEmpty)//没有找到出口
                        break;
                    else
                    {
                        cntLine.AddPoint(vpt);
                        _edgeIsUsed.SetStatus(edgeIdx, true);
                    }
                }
                else
                {
                    cntLine.AddPoint(vpt);
                    _edgeIsUsed.SetStatus(edgeIdx, true);
                }
                //标记当前三角形已经使用
                _triangleIsUsed.SetStatus(crtTriangle, true);
                //查找当前三角形相连的三角形作为当前三角形
                crtTriangle = GetNextTriangle(crtTriangle, ref crtEdge);
                //退出条件
                if (crtTriangle == -1)
                {
                    break;
                }
                edgeIdx = ToEdgeIndex(crtTriangle, crtEdge);
                _edgeIsUsed.SetStatus(edgeIdx, false);//两个三角形的共享边标记为未使用
                if (isStop(crtTriangle))
                {
                    isClosed = true;
                    _edgeIsUsed.SetStatus(edgeIdx, true);
                    break;
                }
                if (_triangleIsUsed.IsTrue(crtTriangle))
                    break;
                isSharePointed = true;
            }
            if (_isOutputUncompleted)
                return cntLine.Count >= minPoints ? cntLine : null;
            else
            {
                if (isClosed)
                    return cntLine.Count >= minPoints ? cntLine : null;
                return null;
            }
        }

        private bool IsBorderTriangle(int idxTriangle)
        {
            if (idxTriangle % _triWidth == 0)                      //left
                return true;
            else if ((idxTriangle + 1) % _triWidth == 0)           //right
                return true;
            else if (idxTriangle / _triWidth == 0 && idxTriangle % 2 != 0)                         //top
                return true;
            else if (idxTriangle / _triWidth == _gridHeight - 1)   //bottom
                return true;
            return false;
        }

        private bool IsBorderEdge(int idxTriangle, enumEdgeType edge)
        {
            if (idxTriangle % _triWidth == 0 && edge == enumEdgeType.B)                      //left
                return true;
            else if ((idxTriangle + 1) % _triWidth == 0 && edge == enumEdgeType.B)           //right
                return true;
            else if (idxTriangle / _triWidth == 0 && edge == enumEdgeType.A)                 //top
                return true;
            else if (idxTriangle / _triWidth == _gridHeight - 1 && edge == enumEdgeType.A)   //bottom
                return true;
            return false;
        }

        private int ToEdgeIndex(int crtTriangle, enumEdgeType crtEdge)
        {
            int pixelRow = crtTriangle / _triWidth;
            int pixelCol = crtTriangle % _triWidth / 2;
            bool isLeftBottom = crtTriangle % 2 == 0;
            int idx = 0;
            if (pixelRow == 0)
                idx = _gridWidth + pixelCol * 3;
            else
                idx = pixelRow * (_gridWidth * 3 + 1) + _gridWidth + pixelCol * (_gridWidth - 1);
            if (isLeftBottom)
            {
                switch (crtEdge)
                {
                    case enumEdgeType.A:
                        return idx + 1;
                    case enumEdgeType.B:
                        return idx;
                    case enumEdgeType.C:
                        return idx + 2;
                }
            }
            else
            {
                switch (crtEdge)
                {
                    case enumEdgeType.A:
                        if (pixelRow == 0)
                            return pixelCol;
                        else
                            return idx - _gridWidth * 3;
                    case enumEdgeType.B:
                        return idx + 3;
                    case enumEdgeType.C:
                        return idx + 2;
                }
            }
            return -1;
        }

        private int GetNextTriangle(int crtTriangle, ref enumEdgeType crtEdge)
        {
            bool isRightTop = crtTriangle % 2 != 0;
            int span = (_gridWidth * 2 + 1);
            try
            {
                switch (crtEdge)
                {
                    case enumEdgeType.A:
                        if (isRightTop)
                            crtTriangle = crtTriangle - span;
                        else
                            crtTriangle = crtTriangle + span;
                        break;
                    case enumEdgeType.B:
                        if (isRightTop)
                            crtTriangle = crtTriangle + 1;
                        else
                            crtTriangle = crtTriangle - 1;
                        break;
                    case enumEdgeType.C:
                        if (isRightTop)
                            crtTriangle = crtTriangle - 1;
                        else
                            crtTriangle = crtTriangle + 1;
                        break;
                }
            }
            finally
            {
                if (crtTriangle > _triangleCount - 1 || crtTriangle < 0)
                    crtTriangle = -1;
            }
            return crtTriangle;
        }

        private void SetOthterEdges(enumEdgeType crtEdge, ref enumEdgeType[] otherEdges)
        {
            if (crtEdge == enumEdgeType.A)
            {
                otherEdges[0] = enumEdgeType.B;
                otherEdges[1] = enumEdgeType.C;
            }
            else if (crtEdge == enumEdgeType.B)
            {
                otherEdges[0] = enumEdgeType.A;
                otherEdges[1] = enumEdgeType.C;
            }
            else if (crtEdge == enumEdgeType.C)
            {
                otherEdges[0] = enumEdgeType.A;
                otherEdges[1] = enumEdgeType.B;
            }
        }


        private PointF TryGetVPoint(int crtTriangle, enumEdgeType crtEdge, double contValue)
        {
            int pixelRow = crtTriangle / _triWidth;
            int pixelCol = crtTriangle % _triWidth / 2;
            bool isRightTop = crtTriangle % 2 != 0;//是右上三角形
            if (isRightTop)
                pixelCol += 1;
            else
                pixelRow += 1;
            int pixelIdx = pixelRow * _width + pixelCol;
            int pixelIdxV1 = pixelIdx, pixelIdxV2 = pixelIdx;
            switch (crtEdge)
            {
                case enumEdgeType.A:
                    if (isRightTop)
                        pixelIdxV1 -= 1;
                    else
                        pixelIdxV2 += 1;
                    break;
                case enumEdgeType.B:
                    if (isRightTop)
                        pixelIdxV2 += _width;
                    else
                        pixelIdxV1 -= _width;
                    break;
                case enumEdgeType.C:
                    if (isRightTop)
                    {
                        pixelIdxV1 -= 1;
                        pixelIdxV2 += _width;
                    }
                    else
                    {
                        pixelIdxV1 -= _width;
                        pixelIdxV2 += 1;
                    }
                    break;
            }
            if (!CheckIsContained(_bandValues[pixelIdxV1], _bandValues[pixelIdxV2], ToTValue(contValue)))
                return PointF.Empty;
            PointF vpt = GetVPoint(crtEdge, pixelIdxV1, pixelIdxV2, contValue);
            //经过采样
            if (_sample > 1)
            {
                vpt.X *= _sample;
                vpt.Y *= _sample;
            }
            return vpt;
        }

        protected abstract PointF GetVPoint(enumEdgeType edgeType, int pixelIdxV1, int pixelIdxV2, double contValue);

        public void ValIsContainedByTriangle(int idx, T val, ref VTriangle triangle)
        {
            int pixelRow = idx / _triWidth;
            int pixelCol = idx % _triWidth / 2;
            bool isContained = false;
            int pixelIdx = 0;
            if (idx % 2 == 0)//偶数，左下三角形
            {
                pixelRow += 1;
                pixelIdx = pixelRow * _width + pixelCol;
                isContained = CheckIsContained(_bandValues[pixelIdx], _bandValues[pixelIdx + 1], val);          //left -> right
                if (isContained)
                    triangle.SetAContained();
                isContained = CheckIsContained(_bandValues[pixelIdx - _width], _bandValues[pixelIdx], val);     //top -> bottom
                if (isContained)
                    triangle.SetBContained();
                isContained = CheckIsContained(_bandValues[pixelIdx - _width], _bandValues[pixelIdx + 1], val); //lefttop -> rightbottom
                if (isContained)
                    triangle.SetCContained();
            }
            else//奇数，右上三角形
            {
                pixelCol += 1;
                pixelIdx = pixelRow * _width + pixelCol;
                isContained = CheckIsContained(_bandValues[pixelIdx - 1], _bandValues[pixelIdx], val);         //left -> right
                if (isContained)
                    triangle.SetAContained();
                isContained = CheckIsContained(_bandValues[pixelIdx], _bandValues[pixelIdx + _width], val);    //top -> bottom
                if (isContained)
                    triangle.SetBContained();
                isContained = CheckIsContained(_bandValues[pixelIdx - 1], _bandValues[pixelIdx + _width], val);//lefttop -> rightbottom
                if (isContained)
                    triangle.SetCContained();
            }
        }

        protected abstract T ToTValue(double v);

        protected abstract bool CheckIsContained(T v1, T v2, T val);

        /// <summary>
        /// 得到边框处的三角形
        /// </summary>
        private void GetBorderTriangles()
        {
            _borderTriangles = new int[_gridWidth * 2 + _gridHeight * 2];//left + right + top + bottom
            int idx = 0;
            //left
            for (int i = 0; i < _gridHeight; i++)
                _borderTriangles[idx++] = i * _gridWidth * 2;
            //right
            for (int i = 0; i < _gridHeight; i++)
                _borderTriangles[idx++] = i * _gridWidth * 2 + _gridWidth * 2 - 1;
            //top
            for (int i = 0; i < _gridWidth; i++)
                _borderTriangles[idx++] = i * 2 + 1;
            //bottom
            for (int i = 0; i < _gridWidth; i++)
                _borderTriangles[idx++] = (_gridHeight - 1) * (_gridWidth * 2) + i * 2;
        }

        /// <summary>
        /// 读取栅格数据到内存
        /// </summary>
        private void ReadBandValues()
        {
            if (_raster is IArrayRasterBand<T>)
            {
                _bandValues = (_raster as IArrayRasterBand<T>).BandValues;
            }
            else
            {
                _bandValues = new T[_width * _height];
                GCHandle handle = GCHandle.Alloc(_bandValues, GCHandleType.Pinned);
                try
                {
                    _raster.Read(0, 0, _raster.Width, _raster.Height, handle.AddrOfPinnedObject(), _raster.DataType, _width, _height);
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        protected abstract T GetNoDataValueOutsideAOI();

        /// <summary>
        /// 将基于AOI计算出的等值线还原到全图
        /// </summary>
        /// <param name="contourLines"></param>
        /// <param name="aoi"></param>
        private unsafe void OffsetToFull(List<ContourLine> contourLines, int offsetRow, int offsetCol)
        {
            if (contourLines == null || contourLines.Count == 0)
                return;
            int ptCount = 0;
            foreach (ContourLine cl in contourLines)
            {
                if (cl == null || cl.Count == 0)
                    continue;
                ptCount = cl.Count;
                fixed (PointF* ptr0 = cl.Points)
                {
                    PointF* ptr = ptr0;
                    for (int i = 0; i < ptCount; i++, ptr++)
                    {
                        ptr->X = ptr->X + offsetCol;
                        ptr->Y = ptr->Y + offsetRow;
                    }
                }
            }
        }

        /// <summary>
        /// 获取栅格数据的感兴趣区域部分
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private unsafe IRasterBand GetAOIRaster(IRasterBand raster, int[] aoi, Rectangle rect, Size oSize)
        {
            int offsetRow = rect.Top;
            int offsetCol = rect.Left;
            int oWidth = oSize.Width;
            int aoiWidth = rect.Width;
            T nodata = GetNoDataValueOutsideAOI();
            T[] buffer = new T[rect.Width * rect.Height];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                raster.Read(rect.Left, rect.Top, rect.Width, rect.Height, handle.AddrOfPinnedObject(), raster.DataType, rect.Width, rect.Height);
            }
            finally
            {
                handle.Free();
            }
            //
            if (aoi != null && aoi.Length > 0)
            {
                //将全图AOI索引转换到AOI外包矩形
                fixed (int* ptr0 = aoi)
                {
                    int* ptr = ptr0;
                    int aoiCount = aoi.Length;
                    for (int i = 0; i < aoiCount; i++, ptr++)
                        *ptr = (*ptr / oWidth - offsetRow) * aoiWidth + (*ptr % oWidth - offsetCol);
                }
                //逆转AOI，将AOI外的像元替换为无效值
                aoi = AOIHelper.Reversal(aoi, new Size(rect.Width, rect.Height));
                if (aoi != null && aoi.Length > 0)
                {
                    fixed (int* ptr0 = aoi)
                    {
                        int* ptr = ptr0;
                        int aoiCount = aoi.Length;
                        int idx = 0;
                        for (int i = 0; i < aoiCount; i++, ptr++)
                        {
                            idx = (*ptr / aoiWidth) * aoiWidth + *ptr % aoiWidth;
                            buffer[idx] = nodata;
                        }
                    }
                }
            }
            return new ArrayRasterBand<T>(1, buffer, rect.Width, rect.Height, null);
        }

        //Begin   空间插值
        public void SetDataValue(double[] rasPointValue, int width, int height)
        {
            _bandValues = new T[width * height];
            for (int i = 0; i < width * height; i++)
            {
                _bandValues[i] = ToTValue(rasPointValue[i]);
            }
        }

        public ContourLine[] Generate(int width, int height, double[] contourValues, Action<int, string> tracker)
        {
            bool isNewRaster = false;
            int offsetRow = 0, offsetCol = 0;

            _width = width;
            _height = height;
            _gridWidth = _width - 1;
            _gridHeight = _height - 1;
            _triWidth = _gridWidth * 2;
            _contourValues = contourValues;
            _triangleCount = 2 * _gridWidth * _gridHeight;
            _triangleIsUsed = new StatusRecorder(_triangleCount);
            _edgeCount = _gridWidth * _height + _gridHeight * _width + _gridWidth * _gridHeight;
            _edgeIsUsed = new StatusRecorder(_edgeCount);

            try
            {
                if (tracker != null)
                    tracker(10, "正在提取边界三角形...");
                GetBorderTriangles();
                if (tracker != null)
                    tracker(12, "正在生成等值线...");
                _contourLines = new List<ContourLine>();
                GenerateContours(tracker);
                if (tracker != null)
                    tracker(100, "等值线生成完毕!");
            }
            finally
            {
                if (isNewRaster)
                {
                    OffsetToFull(_contourLines, offsetRow, offsetCol);
                }
            }
            return _contourLines != null && _contourLines.Count > 0 ? _contourLines.ToArray() : null;
        }
        //End    空间插值
        public virtual void Dispose()
        {
            _bandValues = null;
            _borderTriangles = null;
            _triangleIsUsed.Dispose();
            GC.Collect();
        }
    }
}
