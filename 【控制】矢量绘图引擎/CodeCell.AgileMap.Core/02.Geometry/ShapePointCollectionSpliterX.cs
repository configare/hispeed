/*
 * 分割算法（以经度分割为例,X）:
 *     令分割线的坐标为X'
 *      一、计算点集与分割线的交点
 *            点集：可以是线的点集合，也可以是多边形中一个环的点集合
 *            从起点StartPoint开始:
 *                    (Point(i).X >= X' and Point(i+1).X<X' ) OR (Point(i).X < X' and Point(i+1).X >= X' ) 记为交点
 *            交点坐标为：(X',(Point(i).Y + Point(i+1).Y) / 2)
 *            交点数组为:Point[]
 *            按Y坐标升序排序，计作CrossPoints
 *      二、分割过程
 *            分割后的一小部分计作环：PartStatus{Point[],int[]}//第一个数组为点集合，第二个数组为与该部分相连的交点序号
 *            X'左侧的环集合计作:LeftParts{PartStatus[]}
 *            X'右侧的环集合计作:RightParts{PartStatus[]}
 *            1、如果CrossPoints.Length = 1 OR 2，则使用简单分割
 *                构造环:LeftPart与RightPart
 *                if Point(i).X >= X' then 
 *                   RightPart.Points.Add(point(i))
 *                else 
 *                   LeftPart.Points.Add(point(i))
 *                分割后为两个环
 *            2、如果CrossPoints.Length >2 则执行以下步骤
 *            3、从点集合中取点 Point[i],从第0个开始取
 *                 如果 Point[i]在 X'右侧    则构造 RightPart,并将点 i 加入 RightPart ,RightPart 加入 RightParts集合
 *                 如果 Point[i]在 X'左侧    则构造 LeftPart,  并将点 i 加入 LeftPart,    LeftPart   加入 LeftParts集合
 *                 记做 ActivePart
 *                 递增 i ++ 
 *                 直到 Point[i] 出现在X'的相反侧 ，跳4步
 *             4、查找到Point[i]与Point[i-1]的交点 ，从CrossPoints集合中   （此时Point[i]与Point[i-1]分别位于X'的左右侧)
 *                 for(int i=0;i<CrossPoints.Length;i++)
 *                 {
 *                     if(     (CrossPoints[i].Y >= Point[i].Y && CrossPoints[i].Y <= Point[i-1].y) OR
 *                             (CrossPoints[i].Y < Point[i].Y   && CrossPoints[i].Y >= Point[i-1].Y )
 *                      {
 *                              CurrentCrossPoint = CrossPoint[i]; 
 *                              CurrentCrossPointIndex = i;
 *                              break;
 *                      }
 *                 }
 *                 ActivePart.CurrentCrossPointIndexes.Add(CurrentCrossPointIndex)
 *             5、从Point[i]所在侧的环集合中查找活动环(活动环要发生改变),也就是从Point[i-1]所在的相反侧
 *                 令 Point[i]所在侧的环集合为 Parts  ,可能为 LeftParts或 RightParts
 *                 Case 1:如果 Parts.Length == 0,即为空，那么构造活动环 ActivePart,并加入 Parts 集合
 *                 case 2:如果 Parts.Length >0
 *                           Case 2-1 :  如果 CurrentCrossPointIndex 与 Parts集合中的所有环的交点都不相邻 
 *                                                  不相邻的算法 暂时考虑为 不连续，估计还要考虑方向？
 *                                            构造新的环 ActiviePart = new PartStatus(Point[i]),加入到 ActivePartCollection
 *                                            例如：例子中跨过交点9，构造 R3
 *                                                     此时左侧集合中有 R1,但是交点序号为1,与9不相邻
 *                           Case 2-2 : 如果 CurrentCrossPointIndex 与 Parts集合中某个环的交点序号相邻
 *                                           Case 2-2-1 : 与 CurrentCrossPointIndex 相邻的环可独立成环,则构造心环作为活动环。
 *                                                              判断独立成环的条件，只有两个交点且连续
 *                                                              例如：跨过交点7时，找到相邻环 R3,但是R3可独立成环，因此不能作为活动环。
 *                                                                       因此，构造环 R4,
 *                                           Case 2-2-2:  动环为与CurrentCrossPointIndex相邻的环
 *                                                              例如：例子中 跨过 交点8的情形，因为 8 与 R2的交点 9 相邻，因此活动环为 R2
 *                 递增 Point[i],i++
 *              6、重复 3 - 5步，指导所有点都加入完毕。
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 交点对象
    /// </summary>
    internal class CrossPoint:IComparer<CrossPoint>,IComparable
    {
        public ShapePoint Point = null;
        public int Index = -1;

        public CrossPoint()
        { 
        }

        public CrossPoint(ShapePoint pt, int index)
        {
            Point = pt;
            Index = index;
        }

        //判断输入的两个点是否跨过了交点的X坐标
        public bool IsCrossX(ShapePoint leftPoint, ShapePoint rightPoint)
        {
            return (leftPoint.X < Point.X && rightPoint.X >= Point.X) ||
                      (leftPoint.X >= Point.X && rightPoint.X < Point.X);
        }

        //判断输入的两个点是否跨过了交点的Y坐标
        public bool IsCrossY(ShapePoint leftPoint, ShapePoint rightPoint)
        {
            double Y = (rightPoint.Y + leftPoint.Y) / 2d;
            return Math.Abs(Y - Point.Y) < 0.0000001d;
            //return (leftPoint.Y < Point.Y && rightPoint.Y >= Point.Y) ||
            //          (leftPoint.Y >= Point.Y && rightPoint.Y < Point.Y);
        }

        #region IComparer<CrossPoint> 成员

        public int Compare(CrossPoint x, CrossPoint y)
        {
            double diff =  x.Point.Y - y.Point.Y;
            if (diff < 0)
                return -1;
            else if (diff < double.Epsilon)
                return 0;
            else
                return 1;
        }

        #endregion

        #region IComparable 成员

        public int CompareTo(object obj)
        {
            return Compare(this, obj as CrossPoint);
        }

        #endregion
    }

    /// <summary>
    /// 构造号的一个环
    /// </summary>
    internal class PartStatus
    {
        public List<ShapePoint> Points = new List<ShapePoint>();
        protected List<int> _crossPointIndexes = new List<int>();

        public PartStatus()
        { 
        }

        public PartStatus(ShapePoint startPoint)
        {
            Points.Add(startPoint);
        }

        public void AddLinkeCrossPointIndex(int crossPointIndex)
        {
            if(!_crossPointIndexes.Contains(crossPointIndex))
                _crossPointIndexes.Add(crossPointIndex);
        }

        //是不是独立成环，如果交点个数为2，且相关的交相邻
        public bool IsSinglePart
        {
            get 
            {
                return _crossPointIndexes.Count == 2 && Math.Abs(_crossPointIndexes[1] - _crossPointIndexes[0]) == 1;
            }
        }

        //判断一个交点是否与本环相邻
        public bool IsNear(int crossPointIndex,out bool isLessThan)
        {
            isLessThan = false;
            if (_crossPointIndexes.Count == 0)
                return false;
            _crossPointIndexes.Sort();
            if (_crossPointIndexes.Count > 2)
                Console.WriteLine("");
            int s = _crossPointIndexes[0] - 1;
            if (crossPointIndex == s)
            {
                isLessThan = true;
                return true;
            }
            int l = _crossPointIndexes[_crossPointIndexes.Count - 1] + 1;
            if (crossPointIndex == l)
            {
                isLessThan = false;
                return true;
            }
            return false;
        }
    }

    internal class ShapePointCollectionSpliterX
    {
        //原始坐标点(一般情况下来自于一个多边性的环或者一个多以线的线带)
        protected List<ShapePoint> _originalPoints = null;
        //分割号的左右侧环集合
        protected List<PartStatus> _leftParts = new List<PartStatus>();
        protected List<PartStatus> _rightParts = new List<PartStatus>();
        //交点集合
        protected List<CrossPoint> _crossPoints = new List<CrossPoint>();
        //分割线
        protected double _X = 0d;
        //活动集合(左侧还是右侧)
        protected List<PartStatus> _activePartCollection = null;
        //活动环
        protected PartStatus _activePart = null;
        //当前点索引，用于记录逐个点是否分割完毕
        protected int _currentPointIndex = -1;
        //前一个环
        private PartStatus _prePartStatus = null;

        public ShapePointCollectionSpliterX()
        { 
        }

        public ShapePointCollectionSpliterX(ShapePoint[] points)
        {
            if (_originalPoints == null)
                _originalPoints = new List<ShapePoint>(points);
            _originalPoints.AddRange(points);
        }

        public void Reset(ShapePoint[] points)
        {
             _originalPoints = new List<ShapePoint>(points);
            _crossPoints.Clear();
            _X = 0;
            _currentPointIndex = -1;
            _activePart = null;
            _activePartCollection = null;
            _prePartStatus = null;
        }

        public void Split(double splitX, out List<ShapePoint[]> leftResult, out List<ShapePoint[]> rightResult)
        {
            _X = splitX;
            leftResult = rightResult = null;
            if (_originalPoints == null || _originalPoints.Count == 0)
                throw new Exception("要分割的源点集没有输入,请使用Reset函数设置。");
            //计算分割线与点集合的交点
            ComputeCrossPoints();
            //与分割线不相交
            if (_crossPoints.Count == 0)
            {
                leftResult = new List<ShapePoint[]>();
                leftResult.Add(_originalPoints.ToArray());
                return;
            }
            //简单分割算法
            if (_crossPoints.Count == 1 || _crossPoints.Count == 2)
            {
                SimpleSplit();
            }
            else
            {
                ComplexSplit();
            }
            //准备分割结果
            if (_leftParts.Count > 0)
            {
                leftResult = new List<ShapePoint[]>();
                foreach (PartStatus part in _leftParts)
                    leftResult.Add(part.Points.ToArray());
            }
            if (_rightParts.Count > 0)
            {
                rightResult = new List<ShapePoint[]>();
                foreach (PartStatus part in _rightParts)
                    rightResult.Add(part.Points.ToArray());
            }
        }

        private void ComplexSplit()
        {
            _currentPointIndex = 0;
            int pointCount = _originalPoints.Count;
            ShapePoint pt = null,prePt = null;
            while (_currentPointIndex < pointCount)
            {
                pt = _originalPoints[_currentPointIndex];
                //确定活动环集合（即：在左侧还是在右侧）
                GetActivePartCollection(pt);
                //获取活动环
                if (prePt == null)//第一次
                    GetActivePart(pt,null);
                else
                {
                    bool isRight1 = pt.X >= _X;
                    bool isRight2 = prePt.X >= _X;
                    if (isRight1 ^ isRight2) //不在同侧
                    {
                        GetActivePart(pt, prePt);
                    }
                }
                //加入点到活动环
                _activePart.Points.Add(pt);
                //继续处理下一个点
                prePt = pt;
                _currentPointIndex++;
            }
        }

        private void GetActivePart(ShapePoint pt,ShapePoint prePoint)
        {
            CrossPoint crossPt  = null;
            //获取跨分割线的交点
            if (prePoint != null)
            {
                crossPt  = GetCrossPoint(pt, prePoint);
                if (crossPt == null)
                    throw new Exception("获取一对点的交点失败。"); //不会出现这样的错误。
            }
            //
            if (_activePartCollection.Count == 0)
            {
                _activePart = new PartStatus();
                _activePartCollection.Add(_activePart);
                goto endLine;
            }
            else
            {
                //获取相邻的环
                foreach (PartStatus part in _activePartCollection)
                {
                    bool isLessThan = false;
                    if (part.IsNear(crossPt.Index,out isLessThan) )//相邻
                    {
                        if (_activePartCollection.Equals(_leftParts) && isLessThan)
                        {
                            _activePart = part;
                            goto endLine;
                        }
                        if ((_activePartCollection.Equals(_rightParts) && !isLessThan))
                        {
                            _activePart = part;
                            goto endLine;
                        }
                    }
                }
                _activePart = new PartStatus();
                //将新建的环加入活动环集合
                _activePartCollection.Add(_activePart);
                goto endLine;
            }
        endLine:
            //将当前交点与活动环关联
            if (crossPt != null)
            {
                if (_prePartStatus != null)
                {
                    _prePartStatus.AddLinkeCrossPointIndex(crossPt.Index);
                }
            }
            _prePartStatus = _activePart;
        }

        private CrossPoint GetCrossPoint(ShapePoint pt, ShapePoint prePoint)
        {
            foreach (CrossPoint cpt in _crossPoints)
                if (cpt.IsCrossY(pt, prePoint))
                    return cpt;
            return null;
        }

        private void GetActivePartCollection(ShapePoint pt)
        {
            if (pt.X >= _X)
                _activePartCollection = _rightParts;
            else
                _activePartCollection = _leftParts;
        }

        private void SimpleSplit()
        {
            PartStatus left = new PartStatus();
            PartStatus right = new PartStatus();
            foreach (ShapePoint pt in _originalPoints)
            {
                if (pt.X >= _X)
                    right.Points.Add(pt);
                else
                    left.Points.Add(pt);
            }
            if(left.Points.Count>0)
                _leftParts.Add(left);
            if (right.Points.Count > 0)
                _rightParts.Add(right);
        }

        private void ComputeCrossPoints()
        {
            int preIdx = 0;
            bool preIsRight = _originalPoints[preIdx].X >= _X;
            bool isRight = false;
            for (int i = 1; i < _originalPoints.Count; i++)
            {
                preIsRight = _originalPoints[preIdx].X >= _X;
                isRight = _originalPoints[i].X >= _X;
                if (isRight ^ preIsRight)
                {
                    CrossPoint pt = new CrossPoint(new ShapePoint(_X, (_originalPoints[preIdx].Y + _originalPoints[i].Y) / 2d), -1);//unsort
                    _crossPoints.Add(pt);
                }
                preIdx = i;
            }
            if (_crossPoints.Count > 0)
            {
                _crossPoints.Sort();
                for (int i = 0; i < _crossPoints.Count; i++)
                    _crossPoints[i].Index = i;
            }
        }
    }
}
