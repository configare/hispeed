using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public static class AOIHelper
    {
        public class ColRangeOfRow
        {
            public int Row;
            public int BeginCol;
            public int EndCol;
            public ColRangeOfRow(int row, int beginCol, int endCol)
            {
                Row = row;
                BeginCol = beginCol;
                EndCol = endCol;
            }
        }

        public static SortedList<int, ColRangeOfRow> ComputeColRanges(int[] aoi, Size size)
        {
            SortedList<int, ColRangeOfRow> rowRanges = new SortedList<int, ColRangeOfRow>();
            ColRangeOfRow range = null;
            int nCount = aoi.Length;
            int row = 0, col = 0, width = size.Width;
            for (int i = 0; i < nCount; i++)
            {
                row = aoi[i] / width;
                col = aoi[i] % width;
                if (rowRanges.ContainsKey(row))
                {
                    range = rowRanges[row];
                    range.BeginCol = Math.Min(range.BeginCol, col);
                    range.EndCol = Math.Max(range.EndCol, col);
                }
                else
                {
                    range = new ColRangeOfRow(row, col, col);
                    rowRanges.Add(row, range);
                }
            }
            return rowRanges;
        }

        public class AOIRowRange
        {
            public int BeginIndex;
            public int EndIndex;
            public AOIRowRange(int beginIdx, int endIdx)
            {
                BeginIndex = beginIdx;
                EndIndex = endIdx;
            }
        }

        public static Dictionary<int, AOIRowRange> ComputeRowIndexRange(int[] aoi, Size size)
        {
            if (size.Width == 0 || size.Height == 0 || aoi == null || aoi.Length == 0)
                return null;
            int row = 0, prerow = 0;
            Dictionary<int, AOIRowRange> rowOffsets = new Dictionary<int, AOIRowRange>(size.Height);
            if (aoi != null)
            {
                int nCount = aoi.Length;
                for (int i = 0; i < nCount; i++)
                {
                    row = aoi[i] / size.Width;
                    if (!rowOffsets.ContainsKey(row))
                        rowOffsets.Add(row, new AOIRowRange(i, 0));//row beginIdx
                    if (row != prerow)
                        if (rowOffsets.ContainsKey(prerow))
                            rowOffsets[prerow].EndIndex = i;//row endIdx
                    prerow = row;
                }
                rowOffsets[row].EndIndex = nCount;
            }
            else
            {
                int offset = 0;
                for (int i = 0; i < size.Height; i++, offset += size.Width)
                    rowOffsets.Add(i, new AOIRowRange(offset, offset + size.Width));
            }
            return rowOffsets;
        }

        public static Rectangle ComputeAOIRect(int[] aoi, Size size)
        {
            int bRow = int.MaxValue;
            int bCol = int.MaxValue;
            int eRow = int.MinValue;
            int eCol = int.MinValue;
            if (aoi != null && aoi.Length > 0)
            {
                int nCount = aoi.Length;
                int row = 0, col = 0;
                int width = size.Width;
                int prerow = 0;
                for (int i = 0; i < nCount; i++)
                {
                    row = aoi[i] / width;
                    col = aoi[i] - row * width;
                    bRow = Math.Min(bRow, row);
                    eRow = Math.Max(eRow, row);
                    bCol = Math.Min(bCol, col);
                    eCol = Math.Max(eCol, col);
                    prerow = row;
                }
                eRow++;
                eCol++;
            }
            else
            {
                bRow = bCol = 0;
                eRow = size.Height;
                eCol = size.Width;
            }
            return Rectangle.FromLTRB(bCol, bRow, eCol, eRow);
        }

        public static int[] Reversal(int[] aoi, Size size)
        {
            int count = size.Width * size.Height ;
            StatusRecorder status = new StatusRecorder(count);
            int aoiCount = aoi.Length;
            for (int i = 0; i < aoiCount; i++)
                status.SetStatus(aoi[i], true);
            List<int> retAOI = new List<int>(count - aoiCount);
            for (int i = 0; i < count; i++)
                if (!status.IsTrue(i))
                    retAOI.Add(i);
            return retAOI.Count > 0 ? retAOI.ToArray() : null;
        }

        public static int[] Merge(int[][] aois)
        {
            if (aois == null || aois.Length == 0)
                return null;
            SortedSet<int> aoi = new SortedSet<int>();
            for (int i = 0; i < aois.Length; i++)
            {
                if (aois[i] == null || aois[i].Length == 0)
                    continue;
                for (int k = 0; k < aois[i].Length; k++)
                {
                    aoi.Add(aois[i][k]);
                }
            }
            return aoi.Count > 0 ? aoi.ToArray() : null;
        }

        public static int[] Intersect(int[] aoi1, int[] aoi2, Size size)
        {
            if (aoi1 == null && aoi2 == null)
                return null;
            if (aoi1 == null && aoi2 != null)
                return aoi2;
            if (aoi2 == null && aoi1 != null)
                return aoi1;
            byte[] buffer1 = new byte[size.Width * size.Height];
            byte[] buffer2 = new byte[size.Width * size.Height];
            for (int i = 0; i < aoi1.Length; i++)
                buffer1[aoi1[i]] = 1;
            for (int i = 0; i < aoi2.Length; i++)
                buffer2[aoi2[i]] = 1;
            SortedSet<int> aoi = new SortedSet<int>();
            int idx = 0;
            for (int r = 0; r < size.Height; r++)
            {
                for (int c = 0; c < size.Width; c++, idx++)
                {
                    if (buffer1[idx] == 1 && buffer2[idx] == 1)
                        aoi.Add(idx);
                }
            }
            return aoi.Count > 0 ? aoi.ToArray() : null;
        }
    }
}
