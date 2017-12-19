using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public static class AOIHelper
    {
        internal class ColRangeOfRow
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

        public static int[] Scale(int[] aoi, Size size, Size newSize)
        {
            if (aoi == null || aoi.Length == 1 || size.IsEmpty || newSize.IsEmpty)
                return null;
            int nCount = aoi.Length;
            int row = 0;
            int col = 0;
            int width = size.Width;
            float scaleX = newSize.Width / (float)size.Width;
            float scaleY = newSize.Height / (float)size.Height;
            SortedList<int, ColRangeOfRow> rowRanges = new SortedList<int, ColRangeOfRow>();
            ColRangeOfRow range = null;
            for (int i = 0; i < nCount; i++)
            {
                row = (int)(scaleY * (aoi[i] / width));
                col = (int)(scaleX * (aoi[i] % width));
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
            //插值
            ColRangeOfRow[] rows = rowRanges.Values.ToArray();
            if (rows.Length == 0)
                return null;
            ColRangeOfRow crtRow = null;
            ColRangeOfRow nxtRow = null;
            int bColOffsetSpan = 0;
            int eColOffsetSpan = 0;
            List<int> retAOIs = new List<int>();
            int newWidth = newSize.Width;
            int bCol = 0;
            int eCol = 0;
            for (int i = 0; i < rows.Length - 1; i++)
            {
                crtRow = rows[i];
                nxtRow = rows[i + 1];
                bColOffsetSpan = (nxtRow.BeginCol - crtRow.BeginCol) / (nxtRow.Row - crtRow.Row);
                eColOffsetSpan = (nxtRow.EndCol - crtRow.EndCol) / (nxtRow.Row - crtRow.Row);
                bCol = crtRow.BeginCol ;
                eCol = crtRow.EndCol ;
                for (int r = crtRow.Row; r < nxtRow.Row; r++, bCol += bColOffsetSpan, eCol += eColOffsetSpan)
                    for (int c = bCol; c < eCol; c++)
                        retAOIs.Add(r * newWidth + c);
            }
            //最后一行
            crtRow = rows[rows.Length - 1];
            int lastRow = crtRow.Row;
            bCol = crtRow.BeginCol;
            eCol = crtRow.EndCol;
            for (int c = bCol; c <= eCol; c++)
                retAOIs.Add(lastRow * newWidth + c);
            //
            return retAOIs.Count > 0 ? retAOIs.ToArray() : null;
        }

        public static int[] Reverse(int[] aoi, Size size)
        {
            if (aoi == null || aoi.Length == 0 || size.Width == 0 || size.Height == 0)
                return null;
            using (IBinaryBitmapBuilder b = new BinaryBitmapBuilder())
            {
                Bitmap bm = b.CreateBinaryBitmap(size, Color.Red, Color.Black);
                try
                {
                    b.Fill(aoi, size, ref bm);
                    using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
                    {
                        return c.ToRaster(bm, Color.Black);
                    }
                }
                finally
                {
                    bm.Dispose();
                }
            }
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
