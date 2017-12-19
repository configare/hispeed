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
    public class PrjEnvelopeF : ICloneable
    {
        private float _minX;
        private float _maxX;
        private float _minY;
        private float _maxY;

        public PrjEnvelopeF()
        {
            _minX = float.MaxValue;
            _maxX = float.MinValue;
            _minY = float.MaxValue;
            _maxY = float.MinValue;
        }

        public PrjEnvelopeF(float minX, float maxX, float minY, float maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public PrjEnvelopeF(double minX, double maxX, double minY, double maxY)
        {
            _minX = (float)minX;
            _maxX = (float)maxX;
            _minY = (float)minY;
            _maxY = (float)maxY;
        }

        public static PrjEnvelopeF CreateByCenter(float centerX, float centerY, float width, float height)
        {
            float minX = centerX - width * 0.5f;
            float maxX = centerX + width * 0.5f;
            float minY = centerY - height * 0.5f;
            float maxY = centerY + height * 0.5f;
            return new PrjEnvelopeF(minX, maxX, minY, maxY);
        }

        public static PrjEnvelopeF CreateByLeftBottom(float leftBottomX, float leftBottomY, float width, float height)
        {
            float minX = leftBottomX;
            float maxX = leftBottomX + width;
            float minY = leftBottomY;
            float maxY = leftBottomY + height;
            return new PrjEnvelopeF(minX, maxX, minY, maxY);
        }

        public static PrjEnvelopeF CreateByLeftTop(float leftTopX, float leftTopY, float width, float height)
        {
            float minX = leftTopX;
            float maxX = leftTopX + width;
            float minY = leftTopY - height;
            float maxY = leftTopY;
            return new PrjEnvelopeF(minX, maxX, minY, maxY);
        }

        public static PrjEnvelopeF Empty
        {
            get { return new PrjEnvelopeF(0.0f, 0.0f, 0.0f, 0.0f); }
        }
        
        public float MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }

        public float MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public float MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public float MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public float Width
        {
            get { return _maxX - _minX; }
        }

        public float Height
        {
            get { return _maxY - _minY; }
        }

        public float CenterX
        {
            get { return (_maxX + _minX) * 0.5f; }
        }

        public float CenterY
        {
            get { return (_maxY + _minY) * 0.5f; }
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
        public float AspectRatio
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
                {
                    return true;
                }
                if (double.IsNaN(MinY) || double.IsNaN(MaxY))
                {
                    return true;
                }
                if (MinX > MaxX || MinY > MaxY)
                {
                    return true;
                }
                return false;
            }
        }
        
        public virtual object Clone()
        {
            return new PrjEnvelopeF(_minX, _maxX, _minY, _maxY);
        }

        public static bool operator ==(PrjEnvelopeF left, PrjEnvelopeF right)
        {
            if (((object)left) == null) return (((object)right) == null);
            return left.Equals(right);
        }

        public static bool operator !=(PrjEnvelopeF left, PrjEnvelopeF right)
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
            PrjEnvelopeF other = obj as PrjEnvelopeF;
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

        public bool Contains(PrjEnvelopeF ext)
        {
            Contract.Requires(ext != null);
            return (this.MinX <= ext.MinX) &&
            ((ext.MaxX) <= (this.MaxX)) &&
            (this.MinY <= ext.MinY) &&
            ((ext.MaxY) <= (this.MaxY));
        }
        
        public void Intersect(PrjEnvelopeF rect)
        {
            PrjEnvelopeF result = PrjEnvelopeF.Intersect(rect, this);
            this.MinX = result.MinX;
            this.MinY = result.MinY;
            this.MaxX = result.MaxX;
            this.MaxY = result.MaxY;
        }

        public static PrjEnvelopeF Intersect(PrjEnvelopeF a, PrjEnvelopeF b)
        {
            Contract.Requires(a != null && b != null);
            float minx = a.MinX > b.MinX ? a.MinX : b.MinX;//Math.Max(a.MinX, b.MinX);
            float maxx = a.MaxX < b.MaxX ? a.MaxX : b.MaxX;//Math.Min(a.MaxX, b.MaxX);
            float miny = a.MinY > b.MinY ? a.MinY : b.MinY;//Math.Max(a.MinY, b.MinY);
            float maxy = a.MaxY < b.MaxY ? a.MaxY : b.MaxY;//Math.Min(a.MaxY, b.MaxY);
            if (minx > maxx)
                return null;
            if (miny > maxy)
                return null;
            return new PrjEnvelopeF(minx, maxx, miny, maxy);
        }

        public bool IntersectsWith(PrjEnvelopeF rect)
        {
            return _minX < rect.MaxX &&
                    _maxX > rect.MinX &&
                    _minY < rect.MaxY &&
                    _maxY > rect.MinY;
        }

        public static PrjEnvelopeF Union(PrjEnvelopeF a, PrjEnvelopeF b)
        {
            if (a == null) return b;
            if (b == null) return a;
            float minx = a.MinX < b.MinX ? a.MinX : b.MinX;
            float maxx = a.MaxX > b.MaxX ? a.MaxX : b.MaxX;
            float miny = a.MinY < b.MinY ? a.MinY : b.MinY;
            float maxy = a.MaxY > b.MaxY ? a.MaxY : b.MaxY;
            return new PrjEnvelopeF(minx, maxx, miny, maxy);
        }

        public bool Contains(double x, double y)
        {
            return this.MinX <= x && x < this.MaxX &&
                this.MinY <= y && y < this.MaxY;
        }

        /// <summary>
        /// 四舍五入获得像素 高和宽
        /// </summary>
        public Size GetSize(float resolutionX, float resolutionY)
        {
            return new Size((int)(Width / resolutionX + 0.5f), (int)(Height / resolutionY + 0.5f));
        }

        public static PrjEnvelopeF GetEnvelope(float[] srcXs, float[] srcYs, PrjEnvelopeF maskEnvelope)
        {
            double MaxX = float.MaxValue;
            double MinX = float.MinValue;
            double MaxY = float.MaxValue;
            double MinY = float.MinValue;
            if (maskEnvelope != null)
            {
                MaxX = maskEnvelope.MaxX;
                MinX = maskEnvelope.MinX;
                MaxY = maskEnvelope.MaxY;
                MinY = maskEnvelope.MinY;
            }
            int length = srcXs.Length;
            float curMinX = srcXs[0];
            float curMaxX = srcXs[0];
            float curMinY = srcYs[0];
            float curMaxY = srcYs[0];
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
            unsafe
            {
                fixed (float* xP = srcXs, yP = srcYs)
                {
                    float* px = xP;
                    float* py = yP;
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
            PrjEnvelopeF dstEnvelope = new PrjEnvelopeF(curMinX, curMaxX, curMinY, curMaxY);
            return dstEnvelope;
        }

        public static PrjEnvelopeF GetEnvelope(double[] srcXs, double[] srcYs, PrjEnvelopeF maskEnvelope)
        {
            float MaxX = float.MaxValue;
            float MinX = float.MinValue;
            float MaxY = float.MaxValue;
            float MinY = float.MinValue;
            if (maskEnvelope != null)
            {
                MaxX = (float)maskEnvelope.MaxX;
                MinX = (float)maskEnvelope.MinX;
                MaxY = (float)maskEnvelope.MaxY;
                MinY = (float)maskEnvelope.MinY;
            }
            double curMinX = MaxX;
            double curMaxX = MinX;
            double curMinY = MaxY;
            double curMaxY = MinY;
            unsafe
            {
                fixed (double* xP = srcXs, yP = srcYs)
                {
                    double* px = xP;
                    double* py = yP;
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
            PrjEnvelopeF dstEnvelope = new PrjEnvelopeF(curMinX, curMaxX, curMinY, curMaxY);
            return dstEnvelope;
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
    }
}
