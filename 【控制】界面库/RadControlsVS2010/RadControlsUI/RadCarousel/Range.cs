using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public class Range
    {
        private int from = 0;
        private int to = 0;

        public static Range Empty;
        public static Range Full = new Range(int.MinValue, int.MaxValue);

        #region Constructors & Initialization

        public Range()
        {
        }

        public Range(int from, int to)
        {
            SetRange(from, to);
        }

        public static Range Copy(Range range)
        {
            return new Range(range.from, range.to);
        }

        public static Range Create(int from, int to)
        {
            return new Range(from, to);
        }

        #endregion

        #region Properties

        public int From
        {
            get { return this.from; }
            set
            {
                if (this.from != value)
                {
                    this.from = value;
                    this.Update();
                }
            }
        }

        public int To
        {
            get { return this.to; }
            set
            {
                if (this.to != value)
                {
                    this.to = value;
                    this.Update();
                }
            }
        }

        public uint Length
        {
            get { return (uint)(this.to - this.from); }
        }

        public int Center
        {
            get { return (this.from + this.to) / 2; }
        }

        #endregion

        #region Intersect, Intersection

        public void Intersect(Range range)
        {
            this.from = Math.Max(this.from, Math.Min(this.to, range.from));
            this.to = Math.Min(this.to, Math.Max(this.from, range.to));
        }

        public void Intersect(int from, int to)
        {
            Intersect(new Range(from, to));
        }

        public Range Intersection(Range range)
        {
            return Range.Intersection(this, range);
        }

        public Range Intersection(int from, int to)
        {
            return Range.Intersection(this, from, to);
        }

        public static void Intersect(Range target, Range source)
        {
            target.Intersect(source);
        }

        public static void Intersect(Range target, int srcFrom, int srcTo)
        {
            Range.Intersect(target, new Range(srcFrom, srcTo));
        }

        public static Range Intersection(Range set1, Range set2)
        {
            Range t = new Range(set1.from, set1.to);
            t.Intersect(set2);

            return t;
        }

        public static Range Intersection(Range set1, int set2from, int set2to)
        {
            return Range.Intersection(set1, new Range(set2from, set2to));
        }

        public static Range Intersection(int set1from, int set1to, int set2from, int set2to)
        {
            return Range.Intersection(new Range(set1from, set1to), new Range(set2from, set2to));
        }

        #endregion

        #region Extend & Shrink

        public bool Extend(int left, int right)
        {
            bool extended = true;

            extended &= left > 0 ? ExtendLeft(left) : ShrinkLeft(-left);
            extended &= right > 0 ? ExtendRight(right) : ShrinkRight(-right);

            return extended;
        }

        public bool ExtendLeft(int left)
        {
            if (int.MinValue + left > this.from)
            {
                this.from = int.MinValue;
                return false;
            }

            this.from -= left;
            return true;
        }

        public bool ExtendRight(int right)
        {
            if (int.MaxValue - right < this.to)
            {
                this.to = int.MaxValue;
                return false;
            }

            this.to += right;
            return true;
        }

        public static Range Extend(Range range, int left, int right)
        {
            Range r = Range.Copy(range);

            r.Extend(left, right);

            return r;
        }

        public bool Shrink(int left, int right)
        {
            if (this.from + left > this.to - right)
            {
                this.to = this.from = (this.from + this.to) / 2;
                return false;
            }

            this.from += left;
            this.to -= right;

            return true;
        }

        public bool ShrinkLeft(int left)
        {
            if (this.from + left > this.to)
            {
                this.from = this.to;
                return false;
            }

            this.from += left;
            return true;
        }

        public bool ShrinkRight(int right)
        {
            if (this.to - right < this.from)
            {
                this.to = this.from;
                return false;
            }

            this.to -= right;
            return true;
        }

        public static Range Shrink(Range range, int left, int right)
        {
            Range r = Range.Copy(range);

            r.Shrink(left, right);

            return r;
        }

        #endregion

        #region Shift region

        public int Shift(int positions)
        {
            int realPositions = positions;

            switch (positions.CompareTo(0))
            {
                case -1:
                    return -ShiftLeft(-positions);

                case 1:
                    return ShiftRight(positions);
            }

            return 0;
        }

        public int ShiftLeft(int positions)
        {
            if (positions <= 0)
                return 0;

            if (int.MinValue + positions > this.from)
                positions = this.from - int.MinValue;

            this.from -= positions;
            this.to -= positions;

            return positions;
        }

        public int ShiftRight(int positions)
        {
            if (positions <= 0)
                return 0;

            if (int.MaxValue - positions < this.to)
                positions = int.MaxValue - this.to;

            this.from += positions;
            this.to += positions;

            return positions;
        }

        #endregion

        #region CenterAt

        public int CenterAt(int center)
        {
            int currentCenter = this.Center;
            int currentDiff = this.Shift(center - currentCenter);

            return currentCenter + currentDiff;
        }

        public static Range CenterAt(Range range, int center)
        {
            Range r = Range.Copy(range);
            r.CenterAt(center);
            return r;
        }

        public static Range CenterAt(int from, int to, int center)
        {
            Range r = new Range(from, to);
            r.CenterAt(center);

            return r;
        }

        #endregion

        public void SetRange(int from, int to)
        {
            if (from <= to)
                Set(from, to);
            else
                Set(to, from);
        }

        public int FromRangeIndex(int index)
        {
            return this.IsInRange(index) ? index - this.from : -1;
        }

        public int FromRangeIndexRestricted(int index)
        {
            if (index < this.from) return 0;
            if (index >= this.to) return this.to - this.from - 1;
            return index - this.from;
        }

        public int ToRangeIndex(int index)
        {
            int res = this.from + index;

            return res < this.to ? res : -1;
        }

        public bool IsInRange(int index)
        {
            return this.from <= index && index < this.to;
        }

        public static bool IsInRange(Range range, int index)
        {
            return range != null ? range.IsInRange(index) : false;
        }


        private void Update()
        {
            if (this.from > this.to)
                Set(this.to, this.from);
        }

        private void Set(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public override string ToString()
        {
            return this.from == this.to ?
                string.Format(" {0} ", this.from) :
                string.Format(" [{0}, {1}) ", this.from, this.to);
        }
    }
}
