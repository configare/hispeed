using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace GeoDo.RasterProject
{
    /// <summary>
    /// 描述一个具有更高精度的地理坐标范围,
    /// 矩形区域的边平行于坐标轴。
    /// 左下角是x，y的最小值。
    /// </summary>
    [Serializable]
    public class PrjEnvelope : ICloneable
    {
        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;

        public PrjEnvelope()
        {
            _minX = double.MaxValue;
            _maxX = double.MinValue;
            _minY = double.MaxValue;
            _maxY = double.MinValue;
        }

        public PrjEnvelope(double minX, double maxX, double minY, double maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public PrjEnvelope(float minX, float maxX, float minY, float maxY)
        {
            _minX = double.Parse(minX.ToString());
            _maxX = double.Parse(maxX.ToString());
            _minY = double.Parse(minY.ToString());
            _maxY = double.Parse(maxY.ToString());
        }

        public static PrjEnvelope CreateByCenter(double centerX, double centerY, double width, double height)
        {
            double minX = centerX - width * 0.5d;
            double maxX = centerX + width * 0.5d;
            double minY = centerY - height * 0.5d;
            double maxY = centerY + height * 0.5d;
            return new PrjEnvelope(minX, maxX, minY, maxY);
        }

        public static PrjEnvelope CreateByLeftBottom(double leftBottomX, double leftBottomY, double width, double height)
        {
            double minX = leftBottomX;
            double maxX = leftBottomX + width;
            double minY = leftBottomY;
            double maxY = leftBottomY + height;
            return new PrjEnvelope(minX, maxX, minY, maxY);
        }

        public static PrjEnvelope CreateByLeftTop(double leftTopX, double leftTopY, double width, double height)
        {
            double minX = leftTopX;
            double maxX = leftTopX + width;
            double minY = leftTopY - height;
            double maxY = leftTopY;
            return new PrjEnvelope(minX, maxX, minY, maxY);
        }

        public static PrjEnvelope Empty
        {
            get { return new PrjEnvelope(0.0d, 0.0d, 0.0d, 0.0d); }
        }

        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public double Width
        {
            get { return _maxX - _minX; }
        }

        public double Height
        {
            get { return _maxY - _minY; }
        }

        public double CenterX
        {
            get { return (_maxX + _minX) * 0.5d; }
        }

        public double CenterY
        {
            get { return (_maxY + _minY) * 0.5d; }
        }

        public PrjPoint LeftTop
        {
            get { return new PrjPoint(_minX, _maxY); }
        }

        public PrjPoint RightBottom
        {
            get { return new PrjPoint(_maxX, _minY); }
        }

        /// <summary>
        /// 宽度/高度 的比值
        /// </summary>
        public double AspectRatio
        {
            get
            {
                return Width / Height;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (double.IsNaN(MinX) || double.IsNaN(MaxX))
                    return true;
                if (double.IsNaN(MinY) || double.IsNaN(MaxY))
                    return true;
                if (MinX >= MaxX || MinY >= MaxY)
                    return true;
                if (Math.Abs(MaxX - MinX) < 1.0e-10 || Math.Abs(MaxY - MinY) < 1.0e-10)
                    return true;
                return false;
            }
        }

        public virtual object Clone()
        {
            return new PrjEnvelope(_minX, _maxX, _minY, _maxY);
        }

        public static bool operator ==(PrjEnvelope left, PrjEnvelope right)
        {
            if (((object)left) == null) return (((object)right) == null);
            return left.Equals(right);
        }

        public static bool operator !=(PrjEnvelope left, PrjEnvelope right)
        {
            if (((object)left) == null) return (((object)right) != null);
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            // Check the identity case for reference equality
            if (base.Equals(obj))
            {
                return true;
            }
            PrjEnvelope other = obj as PrjEnvelope;
            if (other == null)
            {
                return false;
            }
            if (MinX != other.MinX)
            {
                return false;
            }
            if (MaxX != other.MaxX)
            {
                return false;
            }
            if (MinY != other.MinY)
            {
                return false;
            }
            if (MaxY != other.MaxY)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Contains(PrjEnvelope ext)
        {
            Contract.Requires(ext != null);
            return (this.MinX <= ext.MinX) &&
            ((ext.MaxX) <= (this.MaxX)) &&
            (this.MinY <= ext.MinY) &&
            ((ext.MaxY) <= (this.MaxY));
        }

        public void Intersect(PrjEnvelope rect)
        {
            PrjEnvelope result = PrjEnvelope.Intersect(rect, this);
            this.MinX = result.MinX;
            this.MinY = result.MinY;
            this.MaxX = result.MaxX;
            this.MaxY = result.MaxY;
        }

        public static PrjEnvelope Intersect(PrjEnvelope a, PrjEnvelope b)
        {
            Contract.Requires(a != null && b != null);
            double minx = a.MinX > b.MinX ? a.MinX : b.MinX;//Math.Max(a.MinX, b.MinX);
            double maxx = a.MaxX < b.MaxX ? a.MaxX : b.MaxX;//Math.Min(a.MaxX, b.MaxX);
            double miny = a.MinY > b.MinY ? a.MinY : b.MinY;//Math.Max(a.MinY, b.MinY);
            double maxy = a.MaxY < b.MaxY ? a.MaxY : b.MaxY;//Math.Min(a.MaxY, b.MaxY);
            if (minx > maxx)
                return null;
            if (miny > maxy)
                return null;
            return new PrjEnvelope(minx, maxx, miny, maxy);
        }

        public bool IntersectsWith(PrjEnvelope rect)
        {
            return _minX < rect.MaxX &&
                    _maxX > rect.MinX &&
                    _minY < rect.MaxY &&
                    _maxY > rect.MinY;
        }

        public static PrjEnvelope Union(PrjEnvelope a, PrjEnvelope b)
        {
            if (a == null) return b;
            if (b == null) return a;
            double minx = a.MinX < b.MinX ? a.MinX : b.MinX;
            double maxx = a.MaxX > b.MaxX ? a.MaxX : b.MaxX;
            double miny = a.MinY < b.MinY ? a.MinY : b.MinY;
            double maxy = a.MaxY > b.MaxY ? a.MaxY : b.MaxY;
            return new PrjEnvelope(minx, maxx, miny, maxy);
        }

        public bool Contains(double x, double y)
        {
            return this.MinX <= x && x < this.MaxX && this.MinY <= y && y < this.MaxY;
        }

        /// <summary>
        /// 四舍五入获得像素 高和宽
        /// </summary>
        public Size GetSize(double resolutionX, double resolutionY)
        {
            return new Size((int)(Width / resolutionX + 0.5d), (int)(Height / resolutionY + 0.5d));
        }

        /// <summary>
        /// 计算范围
        /// 添加是否跨越边缘限制，
        /// 设置边缘后，则如果跨越了边缘，则最大最小值即为两个边缘
        /// </summary>
        /// <param name="srcXs"></param>
        /// <param name="srcYs"></param>
        /// <param name="validEnvelope"></param>
        /// <param name="xFringe"></param>
        /// <param name="yFringe"></param>
        /// <returns></returns>
        public static PrjEnvelope GetEnvelope(double[] srcXs, double[] srcYs, PrjEnvelope validEnvelope, double xFringe, double yFringe)
        {
            double MaxX = double.MaxValue;
            double MinX = double.MinValue;
            double MaxY = double.MaxValue;
            double MinY = double.MinValue;
            if (validEnvelope != null)
            {
                MaxX = validEnvelope.MaxX;
                MinX = validEnvelope.MinX;
                MaxY = validEnvelope.MaxY;
                MinY = validEnvelope.MinY;
            }
            int length = srcXs.Length;
            double curMinX = srcXs[0];
            double curMaxX = srcXs[0];
            double curMinY = srcYs[0];
            double curMaxY = srcYs[0];
            //设置初始值
            for (int i = 0; i < length; i++)
            {
                if (srcXs[i] < MinX || srcXs[i] > MaxX || srcYs[i] < MinY || srcYs[i] > MaxY)
                    continue;
                curMinX = srcXs[i];
                curMaxX = srcXs[i];
                curMinY = srcYs[i];
                curMaxY = srcYs[i];
                break;
            }
            bool overXFringe = false;
            bool overYFringe = false;
            //开始遍历并计算最大最小值
            unsafe
            {
                fixed (double* xP = srcXs, yP = srcYs)
                {
                    double* px = xP;
                    double* py = yP;
                    for (int i = 0; i < length; i++, px++, py++)
                    {
                        //if (Math.Abs(*px - (-90d)) < 0.0000001)
                        //    Console.WriteLine(*px + "索引" + i);
                        if (*px < MinX || *px > MaxX || *py < MinY || *py > MaxY)
                            continue;
                        if (curMaxX < *px)
                            curMaxX = *px;
                        else if (curMinX > *px)
                            curMinX = *px;

                        if (curMaxY < *py)
                            curMaxY = *py;
                        else if (curMinY > *py)
                            curMinY = *py;
                    }
                }
            }
            PrjEnvelope dstEnvelope = new PrjEnvelope(curMinX, curMaxX, curMinY, curMaxY);
            return dstEnvelope;
        }

        public static PrjEnvelope GetEnvelope(double[] srcXs, double[] srcYs, PrjEnvelope validEnvelope)
        {
            double MaxX = double.MaxValue;
            double MinX = double.MinValue;
            double MaxY = double.MaxValue;
            double MinY = double.MinValue;
            if (validEnvelope != null)
            {
                MaxX = validEnvelope.MaxX;
                MinX = validEnvelope.MinX;
                MaxY = validEnvelope.MaxY;
                MinY = validEnvelope.MinY;
            }
            int length = srcXs.Length;
            double curMinX = MinX;
            double curMaxX = MaxX;
            double curMinY = MinY;
            double curMaxY = MaxY;
            //设置初始值
            for (int i = 0; i < length; i++)
            {
                if (srcXs[i] < MinX || srcXs[i] > MaxX || srcYs[i] < MinY || srcYs[i] > MaxY)
                    continue;
                curMinX = srcXs[i];
                curMaxX = srcXs[i];
                curMinY = srcYs[i];
                curMaxY = srcYs[i];
                break;
            }
            //开始遍历并计算最大最小值
            unsafe
            {
                fixed (double* xP = srcXs, yP = srcYs)
                {
                    double* px = xP;
                    double* py = yP;
                    for (int i = 0; i < length; i++, px++, py++)
                    {
                        //if (Math.Abs(*px - (-90d)) < 0.0000001)
                        //    Console.WriteLine(*px + "索引" + i);
                        if (*px < MinX || *px > MaxX || *py < MinY || *py > MaxY)
                            continue;
                        if (curMaxX < *px)
                            curMaxX = *px;
                        else if (curMinX > *px)
                            curMinX = *px;

                        if (curMaxY < *py)
                            curMaxY = *py;
                        else if (curMinY > *py)
                            curMinY = *py;
                    }
                }
            }
            PrjEnvelope dstEnvelope = new PrjEnvelope(curMinX, curMaxX, curMinY, curMaxY);
            return dstEnvelope;
        }

        public static PrjEnvelope GetEnvelope(float[] srcXs, float[] srcYs, PrjEnvelope validEnvelope)
        {
            float MaxX = float.MaxValue;
            float MinX = float.MinValue;
            float MaxY = float.MaxValue;
            float MinY = float.MinValue;
            if (validEnvelope != null)
            {
                MaxX = (float)validEnvelope.MaxX;
                MinX = (float)validEnvelope.MinX;
                MaxY = (float)validEnvelope.MaxY;
                MinY = (float)validEnvelope.MinY;
            }
            float curMinX = MaxX;
            float curMaxX = MinX;
            float curMinY = MaxY;
            float curMaxY = MinY;
            unsafe
            {
                fixed (float* xP = srcXs, yP = srcYs)
                {
                    float* px = xP;
                    float* py = yP;
                    int length = srcXs.Length;
                    for (int i = 0; i < length; i++, px++, py++)
                    {
                        if (*px < MinX || *px > MaxX || *py < MinY || *py > MaxY)
                            continue;
                        if (curMaxX < *px)
                            curMaxX = *px;
                        else if (curMinX > *px)
                            curMinX = *px;
                        if (curMaxY < *py)
                            curMaxY = *py;
                        else if (curMinY > *py)
                            curMinY = *py;
                    }
                }
            }
            PrjEnvelope dstEnvelope = new PrjEnvelope(curMinX, curMaxX, curMinY, curMaxY);
            return dstEnvelope;
        }

        public static bool ValidEnvelope(double[] srcXs, double[] srcYs, PrjEnvelope validEnv)
        {
            if (validEnv == null || validEnv.IsEmpty)
                throw new ArgumentNullException("validEnv", "参数[有效范围]不能为空");
            double MaxX = validEnv.MaxX;
            double MinX = validEnv.MinX;
            double MaxY = validEnv.MaxY;
            double MinY = validEnv.MinY;
            double curMinX = srcXs[0];
            double curMaxX = srcXs[0];
            double curMinY = srcYs[0];
            double curMaxY = srcYs[0];
            int length = srcXs.Length;
            for (int i = 0; i < length; i++)
            {
                if (srcXs[i] < MinX || srcXs[i] > MaxX || srcYs[i] < MinY || srcYs[i] > MaxY)//当前坐标不在指定
                    continue;
                else
                    return true;
            }
            return false;
        }

        public static bool HasValidEnvelope(double[] srcXs, double[] srcYs, PrjEnvelope validEnv, out double validRate, out PrjEnvelope outEnv)
        {
            if (validEnv == null || validEnv.IsEmpty)
                throw new ArgumentNullException("validEnv", "参数[有效范围]不能为空");
            bool hasValid = false;
            double MaxX = validEnv.MaxX;
            double MinX = validEnv.MinX;
            double MaxY = validEnv.MaxY;
            double MinY = validEnv.MinY;
            double curMinX = srcXs[0];
            double curMaxX = srcXs[0];
            double curMinY = srcYs[0];
            double curMaxY = srcYs[0];
            int length = srcXs.Length;
            for (int i = 0; i < length; i++)
            {
                if (srcXs[i] < MinX || srcXs[i] > MaxX || srcYs[i] < MinY || srcYs[i] > MaxY)   //当前坐标点不在指定有效范围
                    continue;
                curMinX = srcXs[i];
                curMaxX = srcXs[i];
                curMinY = srcYs[i];
                curMaxY = srcYs[i];
                hasValid = true;
                break;
            }
            if (!hasValid)
            {
                validRate = 0;
                outEnv = null;
                return false;
            }
            int validCount = 0;
            unsafe
            {
                fixed (double* xP = srcXs, yP = srcYs)
                {
                    double* px = xP;
                    double* py = yP;
                    for (int i = 0; i < length; i++, px++, py++)
                    {
                        if (*px < MinX || *px > MaxX || *py < MinY || *py > MaxY)
                            continue;
                        validCount++;
                        if (curMaxX < *px)
                            curMaxX = *px;
                        else if (curMinX > *px)
                            curMinX = *px;
                        if (curMaxY < *py)
                            curMaxY = *py;
                        else if (curMinY > *py)
                            curMinY = *py;
                    }
                }
            }
            validRate = validCount * 1.0 / length;
            outEnv = new PrjEnvelope(curMinX, curMaxX, curMinY, curMaxY);
            return true;
        }

        public override string ToString()
        {
            return string.Format("X[{0}|{1}],Y[{2}|{3}],Width[{4}],Height[{5}]",
                _minX.ToString(CultureInfo.CurrentCulture),
                _maxX.ToString(CultureInfo.CurrentCulture),
                _minY.ToString(CultureInfo.CurrentCulture),
                _maxY.ToString(CultureInfo.CurrentCulture),
                Width.ToString(CultureInfo.CurrentCulture),
                Height.ToString(CultureInfo.CurrentCulture));
        }

        public void Extend(float x, float y)
        {
            _minX -= x;
            _maxX += x;
            _minY -= y;
            _maxY += y;
        }

        /// <summary>
        /// 计算相交百分比
        /// </summary>
        /// <param name="validEnv"></param>
        /// <returns></returns>
        public bool IntersectsRate(PrjEnvelope validEnv, out double validRate)
        {
            PrjEnvelope intEnv = PrjEnvelope.Intersect(this, validEnv);
            if (intEnv.IsEmpty)
            {
                validRate = 0;
                return false;
            }
            validRate = (this.Height * this.Width * 1.0) / (intEnv.Height * intEnv.Width);
            return true;
        }
    }
}
