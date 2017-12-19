using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ValueMapper
    {
        public interface MapperRange
        {
            bool InRange(double value);

            double From
            {
                get;
            }

            double To
            {
                get;
            }

            double Length
            {
                get;
            }

            double MappedValue
            {
                get;
            }

            double MapToUnit(double value);
        }

        public class RangePair
        {
            public MapperRange source;
            public MapperRange target;

            public RangePair(MapperRange source, MapperRange target)
            {
                this.source = source;
                this.target = target;
            }
        }

        public class Range : MapperRange
        {
            public int includeBorders = 0;
            public double from;
            public double to;
            public double value = double.NaN;

            public Range()
            {
            }

            public Range(bool includeLeft, double from, double to, bool includeRight)
            {
                Set(includeLeft, from, to, includeRight);
            }

            public Range(bool includeLeft, double from, double to, bool includeRight, double defaultValue)
                : this(includeLeft, from, to, includeRight)
            {
                this.value = defaultValue;
            }

            public void Set(bool includeLeft, double from, double to, bool includeRight)
            {
                this.includeBorders = 0;

                if (includeLeft)
                    this.includeBorders |= 0x01;

                if (includeRight)
                    this.includeBorders |= 0x02;

                this.from = from;
                this.to = to;

            }

            public bool InRange(double value)
            {
                bool left = this.from < value;
                left |= (includeBorders & 0x01) == 0x01 && this.from == value;

                bool right = value < this.to;
                right |= (includeBorders & 0x02) == 0x02 && this.to == value;

                return left && right;
            }

            public double From
            {
                get { return this.from; }
            }

            public double To
            {
                get { return this.to; }
            }

            public double Length
            {
                get { return this.to - this.from; }
            }

            public double MappedValue
            {
                get { return this.value; }
            }

            public double MapToUnit(double value)
            {
                if (value == this.to) return 1.0;

                double r = value - this.from;

                r %= this.Length;
                if (r < 0) r += this.Length;

                return r / this.Length;
            }
        }

        public class Value : MapperRange
        {
            public double value;

            public Value()
            {
            }

            public Value(double value)
            {
                this.value = value;
            }

            public bool InRange(double value)
            {
                return double.Equals(this.value, value);
            }

            public double From
            {
                get { return this.value; }
            }

            public double To
            {
                get { return this.value; }
            }

            public double Length
            {
                get { return 0; }
            }

            public double MappedValue
            {
                get { return this.value; }
            }

            public double MapToUnit(double value)
            {
                return this.value;
            }
        }

        private List<RangePair> ranges = new List<RangePair>();

        public RangePair this[int index]
        {
            get { return this.ranges[index]; }
        }

        public int GetIndexFromSource(double value)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].source.InRange(value)) return i;
            }

            return -1;
        }

        public int GetIndexFromTarget(double value)
        {
            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].target.InRange(value)) return i;
            }

            return -1;
        }

        public MapperRange GetTarget(double value)
        {
            int index = GetIndexFromSource(value);

            return index == -1 ? null : this.ranges[index].target;
        }

        public MapperRange GetSource(double value)
        {
            int index = GetIndexFromTarget(value);

            return index == -1 ? null : this.ranges[index].source;
        }

        public void Add(MapperRange source, MapperRange target)
        {
            this.ranges.Add(new RangePair(source, target));
        }

        public double MapInTarget(double value)
        {
            int index = GetIndexFromSource(value);
            
            if (index == -1) return double.NaN;

            RangePair p = this.ranges[index];

            if (p.target.Length == 0) return p.target.From;

            if (p.source.Length == 0)
            {
                return double.IsNaN(p.target.MappedValue) ?
                    (p.target.From + p.target.To) / 2 : p.target.MappedValue;
            }

            double t = (value - p.source.From) / p.source.Length;

            return p.target.From + ( t * p.target.Length );
        }

        public double MapInSource(double value)
        {
            int index = GetIndexFromTarget(value);

            if (index == -1) return double.NaN;

            RangePair p = this.ranges[index];

            if (p.source.Length == 0) return p.source.From;

            if (p.target.Length == 0)
            {
                return double.IsNaN(p.source.MappedValue) ?
                    (p.source.From + p.source.To) / 2 : p.source.MappedValue;
            }

            double t = (value - p.target.From) / p.target.Length;

            return p.source.From + (t * p.source.Length);
        }

        public double MapTargetToUnit(double value)
        {
            int index = GetIndexFromTarget(value);

            if (index == -1) return double.NaN;

            MapperRange range = this.ranges[index].target;

            return range.MapToUnit(value);

            //return (value - range.From) / range.Length;
        }
    }

    public abstract class RangeMapper
    {
        protected int itemsCount;

        public int ItemsCount
        {
            get { return this.itemsCount; }
            set { this.itemsCount = value; }
        }

        public abstract int MapFromRangeIndex(int index);

        public abstract Range Normalize(Range source);

        public abstract int Normalize(Range range, int index);

        public Range CreateLeft(Range target, Range source)
        {
            return this.Normalize(Range.Intersection(target, int.MinValue, source.From));
        }

        public Range CreateRight(Range target, Range source)
        {
            return this.Normalize(Range.Intersection(target, source.To, int.MaxValue));
        }

        public abstract int Closest(int curVal, int newValue);

        public abstract bool IsInRange(Range range, int index);
    }

    public class CircularRangeMapper : RangeMapper
    {
        public override int MapFromRangeIndex(int index)
        {
            if (itemsCount < 1)
                return 0;

            index %= itemsCount;

            return index < 0 ? index + itemsCount : index;
        }

        public override Range Normalize(Range source)
        {
            if (this.itemsCount < 1) return Range.Create(0, 0);

            int length = (int)source.Length;
            int f = source.From % this.itemsCount;

            return new Range(f, f + Math.Min(length, this.itemsCount));
            //return new Range(f, f + length);
            //Point from = new Point(source.From / itemsCount, source.From % itemsCount);
            //Point to = new Point(source.To / itemsCount, source.To % itemsCount);

            //bool fs = from.X < 0;
            //bool ts = to.X < 0;

            //if (from.X == to.X || fs != ts)
            //{
            //    from.X = to.X = 0;
            //}
            //else
            //{
            //    from.X = 0;
            //    to.X = 1;
            //}

            //return new Range(from.X * itemsCount + from.Y, to.X * itemsCount + to.Y);
        }

        public override int Normalize(Range range, int index)
        {
            if (itemsCount < 1 || range.Length < 1) return 0;
            int res = index % itemsCount;

            if (res < range.From) res += itemsCount;
            if (res >= range.To) res -= itemsCount;

            return res;
        }

        public override int Closest(int curVal, int newValue)
        {
            if (itemsCount == 0) return newValue;

            int f = curVal / itemsCount;

            int t1 = newValue % itemsCount + f * itemsCount;
            int t2 = newValue % itemsCount + (f - 1) * itemsCount;
            int t3 = newValue % itemsCount + (f + 1) * itemsCount;

            int d1 = Math.Abs(curVal - t1);
            int d2 = Math.Abs(curVal - t2);
            int d3 = Math.Abs(curVal - t3);

            if (d1 < d2)
            {
                return d1 < d3 ? t1 : t3;
            }
            else
            {
                return d2 < d3 ? t2 : t3;
            }
        }

        public override bool IsInRange(Range range, int index)
        {
            if (this.itemsCount < 1) return false;

            int r = Math.Min((int)range.Length, this.itemsCount);

            int f = range.From % this.itemsCount;
            

            if (f < 0) f += this.itemsCount;

            int t = (f + r) % this.itemsCount;

            if (f < t)
                return f <= index && index < t;
            else
                return (0 <= index && index < t) || (f <= index && index < this.itemsCount);
        }
    }

    public class RestrictedRangeMapper : RangeMapper
    {
        public override int MapFromRangeIndex(int index)
        {
            return this.Normalize(Range.Create(0, this.itemsCount),  index);
        }

        public override Range Normalize(Range source)
        {
            return Range.Intersection(source, 0, this.itemsCount);
        }

        public override int Normalize(Range source, int index)
        {
            if (index < source.From) return source.From;
            if (index >= source.To) return Math.Max(0, source.To - 1);

            return index;
        }

        public override int Closest(int curVal, int newValue)
        {
            return newValue;
        }

        public override bool IsInRange(Range range, int index)
        {
            return range.From <= index && index < range.To;
        }
    }

}
