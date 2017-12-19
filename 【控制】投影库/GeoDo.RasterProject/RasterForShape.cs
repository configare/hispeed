using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RasterProject
{
    /// <summary>
    /// 后向映射计算矢量经纬度对应栅格坐标
    /// </summary>
    public class RasterForShape
    {
        public RasterForShape()
        { }

        /// <summary>
        /// 参考《极轨气象卫星图像的两种投影算法》的后向映射法
        /// </summary>
        /// <param name="srcDu"></param>
        /// <param name="srcSize"></param>
        /// <param name="needFindData"></param>
        /// <param name="colTables"></param>
        /// <param name="rowTables"></param>
        public void Find(double[] srcLongs, double[] srcLats, Size srcSize, double[] needFindLongs, double[] needFindLats, out int[] colTables, out int[] rowTables)
        {
            colTables = null;
            rowTables = null;
            int h = srcSize.Height;
            int w = srcSize.Width;
            int rOff = 0;
            int index = 0;
            int needFindDataLength = needFindLongs.Length;
            //第一步、粗查找
            int stride = 10;//(int)Math.Log(h * w / 4);
            int[] cursoryCols = new int[needFindDataLength];
            int[] cursoryRows = new int[needFindDataLength];
            double minLong = double.MaxValue;
            double minLat = double.MaxValue;
            double longCur;
            double latCur;
            double curFindLong = 0;
            double curFindLat = 0;
            int curFindRow = 0;
            int curFindCol = 0;
            double srcLong;
            double srcLat;
            for (int i = 0; i < needFindDataLength; i++)
            {
                curFindLong = needFindLongs[i];
                curFindLat = needFindLats[i];
                for (int row = 0; row < h; row += stride)
                {
                    rOff = row * w;
                    for (int col = 0; col < w; col += stride)
                    {
                        index = rOff + col;
                        srcLong = srcLongs[index];
                        srcLat = srcLats[index];
                        longCur = Math.Abs(curFindLong - srcLong);
                        latCur = Math.Abs(curFindLat - srcLat);
                        if (minLong > longCur && minLat > latCur)
                        {
                            minLong = longCur;
                            minLat = latCur;
                            curFindRow = row;
                            curFindCol = col;
                        }
                    }
                }
                cursoryCols[i] = curFindRow;
                cursoryRows[i] = curFindCol;
            }
            //精查找
            minLong = double.MaxValue;
            minLat = double.MaxValue;
            int cursoryCol, cursoryRow;
            int beginRow, endRow, beginCol, endCol;
            for (int i = 0; i < needFindDataLength; i++)
            {
                cursoryCol = cursoryCols[i];
                cursoryRow = cursoryRows[i];
                beginRow = cursoryRow - stride;
                endRow = cursoryRow + stride;
                beginCol = cursoryCol - stride;
                endCol = cursoryCol - stride;
                beginRow = beginRow < 0 ? 0 : beginRow;
                beginCol = beginCol < 0 ? 0 : beginCol;
                endRow = endRow > h ? h : endRow;
                endCol = endCol > w ? w : endCol;
                for (int row = beginRow; row < endRow; row++)
                {
                    rOff = row * w;
                    for (int col = beginCol; col < endCol; col++)
                    {
                        index = rOff + col;
                        srcLong = srcLongs[index];
                        srcLat = srcLats[index];
                        longCur = Math.Abs(curFindLong - srcLong);
                        latCur = Math.Abs(curFindLat - srcLat);
                        if (minLong > longCur && minLat > latCur)
                        {
                            minLong = longCur;
                            minLat = latCur;
                            curFindRow = row;
                            curFindCol = col;
                        }
                    }
                }
                cursoryCols[i] = curFindRow;
                cursoryRows[i] = curFindCol;
            }
        }
    }
}
