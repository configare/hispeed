using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RasterProject
{
    /// <summary>
    /// 实现插值算法
    /// </summary>
    public class Interpolation
    {
        const ushort InvalidValue = UInt16.MaxValue;

        /// <summary>
        /// 最邻近插值
        /// </summary>
        /// <param name="lootUpTableMask"></param>
        /// <param name="lutWidth"></param>
        /// <param name="lutHeight"></param>
        /// <param name="tmpRowLookUpTable"></param>
        /// <param name="tmpColLookUpTable"></param>
        public static void ChazhiNear(byte[] lootUpTableMask, int lutWidth, int lutHeight, UInt16[] tmpRowLookUpTable, UInt16[] tmpColLookUpTable)
        {
            byte[] m_lpPos2, m_lpPos3;
            m_lpPos2 = new byte[lutWidth * lutHeight];
            m_lpPos3 = new byte[lutWidth * lutHeight];
            int i, j, k, flag;
            int curIndex;
            int curRIndex;
            flag = 0;
            int tIndex = 0;
            lootUpTableMask.CopyTo(m_lpPos2, 0);
            int endRow = lutHeight - 1;
            int endCol = lutWidth - 1;
            for (k = 0; k < 3; k++)
            {
                m_lpPos2.CopyTo(m_lpPos3, 0);
                for (j = 1; j < endRow; j++)
                {
                    curRIndex = j * lutWidth;
                    for (i = 1; i < endCol; i++)
                    {
                        //九宫格差值
                        curIndex = curRIndex + i;
                        if (m_lpPos2[curIndex] == 0 || tmpRowLookUpTable[curIndex] == InvalidValue || tmpColLookUpTable[curIndex] == InvalidValue)
                        {
                            //右
                            tIndex = curIndex + 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//右
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            //左
                            tIndex = curIndex - 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//左
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            //上
                            tIndex = curIndex - lutWidth;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//上
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            //下
                            tIndex = curIndex + lutWidth;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//下
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            tIndex = curIndex - lutWidth - 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//左上
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            tIndex = curIndex - lutWidth + 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//右上
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            tIndex = curIndex + lutWidth - 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//左下
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            tIndex = curIndex + lutWidth + 1;
                            if (m_lpPos2[tIndex] == 1 && tmpRowLookUpTable[tIndex] != InvalidValue && tmpColLookUpTable[tIndex] != InvalidValue)//右下
                            {
                                tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                                tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                                m_lpPos3[curIndex] = 1;
                                continue;
                            }
                            flag = 1;
                        }
                    }
                }
                if (flag == 0)
                    break;
                else
                    m_lpPos3.CopyTo(m_lpPos2, 0);
            }
            //下面处理第一列，最后一列
            for (j = 0; j < lutHeight; j++)
            {
                i = 0;//第一列
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == InvalidValue)
                {
                    tIndex = curIndex + 1;
                    if (tmpRowLookUpTable[tIndex] != InvalidValue)
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];//取下一列的值(右侧)
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
                i = lutWidth - 1;//最后一列
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == InvalidValue)
                {
                    tIndex = curIndex - 1;
                    if (tmpRowLookUpTable[tIndex] != InvalidValue)
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];//取左边的值(左侧)
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
            }
            for (i = 0; i < lutWidth; i++) //第一行，最后一行
            {
                j = 0;//第一行
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == InvalidValue)
                {
                    tIndex = curIndex + lutWidth;// (j + 1) * width + i;
                    if (tmpRowLookUpTable[tIndex] != InvalidValue)//取下一行的值
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
                j = lutHeight - 1;//最后一行
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == InvalidValue)
                {
                    tIndex = curIndex - lutWidth;// (j - 1) * width + i;
                    if (tmpRowLookUpTable[tIndex] != InvalidValue)//取上一行的值
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
            }
            m_lpPos2 = null;
            m_lpPos3 = null;
            GC.Collect();
            GC.WaitForFullGCComplete();
        }

        public void ChazhiNearO(byte[] lootUpTableMask, int lutWidth, int lutHeight, UInt16[] tmpRowLookUpTable, UInt16[] tmpColLookUpTable)
        {
            int i, j;
            int curIndex;
            int curRIndex;
            UInt16 tmpRowValue = 0;
            UInt16 tmpColValue = 0;
            int tIndex = 0;
            int endRow = lutHeight - 1;
            int endCol = lutWidth - 1;
            int m, n, tRowIndex;
            bool hasValue = false;
            int rOffset = 0;
            for (j = 1; j < endRow; j++)
            {
                curRIndex = j * lutWidth;
                for (i = 1; i < endCol; i++)
                {
                    curIndex = curRIndex + i;
                    if (lootUpTableMask[curIndex] == 0 && tmpRowLookUpTable[curIndex] == 0)
                    {
                        //第一层
                        tIndex = curIndex + lutWidth;
                        if (lootUpTableMask[tIndex] == 1)//下
                        {
                            tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                            tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                            continue;
                        }
                        tIndex = curIndex - lutWidth;
                        if (lootUpTableMask[tIndex] == 1)//上
                        {
                            tmpRowValue = tmpRowLookUpTable[curIndex - lutWidth];
                            tmpColValue = tmpColLookUpTable[curIndex - lutWidth];
                            continue;
                        }
                        tIndex = curIndex + 1;
                        if (lootUpTableMask[tIndex] == 1)//右
                        {
                            tmpRowValue = tmpRowLookUpTable[curIndex - lutWidth];
                            tmpColValue = tmpColLookUpTable[curIndex - lutWidth];
                            continue;
                        }
                        tIndex = curIndex - 1;
                        if (lootUpTableMask[tIndex] == 1)//左
                        {
                            tmpRowValue = tmpRowLookUpTable[curIndex - lutWidth];
                            tmpColValue = tmpColLookUpTable[curIndex - lutWidth];
                            continue;
                        }
                        tIndex = curIndex - lutWidth - 1;
                        if (lootUpTableMask[tIndex] == 1)//左上
                        {
                            tmpRowValue = tmpRowLookUpTable[curIndex - lutWidth];
                            tmpColValue = tmpColLookUpTable[curIndex - lutWidth];
                            continue;
                        }
                        tIndex = curIndex - lutWidth + 1;
                        if (lootUpTableMask[tIndex] == 1)//右上
                        {
                            tmpRowValue = tmpRowLookUpTable[curIndex - lutWidth];
                            tmpColValue = tmpColLookUpTable[curIndex - lutWidth];
                            continue;
                        }
                        if (lootUpTableMask[curIndex + lutWidth - 1] == 1)//左下
                        {
                            tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[curIndex + lutWidth - 1];
                            tmpColLookUpTable[curIndex] = tmpColLookUpTable[curIndex + lutWidth - 1];
                            continue;
                        }
                        if (lootUpTableMask[curIndex + lutWidth + 1] == 1)//右下
                        {
                            tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[curIndex + lutWidth + 1];
                            tmpColLookUpTable[curIndex] = tmpColLookUpTable[curIndex + lutWidth + 1];
                            continue;
                        }
                        hasValue = false;
                        for (m = -1; m <= 1; m++)
                        {
                            if (hasValue)
                                break;
                            tRowIndex = (j + m) * lutWidth;
                            for (n = -1; n <= 1; n++)
                            {
                                if (m == 0 && n == 0)
                                    continue;
                                tIndex = tRowIndex + (i + n);
                                if (lootUpTableMask[tIndex] == 1 && tmpRowLookUpTable[tIndex] != 0)
                                {
                                    tmpRowValue = tmpRowLookUpTable[tIndex];
                                    tmpColValue = tmpColLookUpTable[tIndex];
                                    hasValue = true;
                                    break;
                                }
                            }
                        }
                        if (hasValue)
                        {
                            tmpColLookUpTable[curIndex] = tmpColValue;
                            tmpRowLookUpTable[curIndex] = tmpRowValue;
                            continue;
                        }
                        if (i <= 2 || j <= 2 || i >= lutWidth - 2 || j >= lutHeight - 2)
                            continue;
                        //2层
                        rOffset = 2 * lutWidth;
                        for (m = -2; m <= 2; m++)
                        {
                            tIndex = curIndex - rOffset + m; //行-2;
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                            tIndex = curIndex + rOffset + m; //行+2
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                        }
                        if (hasValue)
                        {
                            tmpColLookUpTable[curIndex] = tmpColValue;
                            tmpRowLookUpTable[curIndex] = tmpRowValue;
                            continue;
                        }
                        for (n = -1; n <= 1; n++)
                        {
                            tIndex = curIndex + n * lutWidth - 2; //列-2
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                            tIndex = curIndex + n * lutWidth + 2; //列+2
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                        }
                        if (hasValue)
                        {
                            tmpColLookUpTable[curIndex] = tmpColValue;
                            tmpRowLookUpTable[curIndex] = tmpRowValue;
                            continue;
                        }
                        if (i <= 3 || j <= 3 || i >= lutWidth - 3 || j >= lutHeight - 3)
                            continue;
                        //2层
                        rOffset = 3 * lutWidth;
                        for (m = -3; m <= 3; m++)
                        {
                            tIndex = curIndex - rOffset + m; //行-3;
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                            tIndex = curIndex + rOffset + m; //行+3
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                        }
                        if (hasValue)
                        {
                            tmpColLookUpTable[curIndex] = tmpColValue;
                            tmpRowLookUpTable[curIndex] = tmpRowValue;
                            continue;
                        }
                        for (n = -2; n <= 2; n++)
                        {
                            tIndex = curIndex + n * lutWidth - 3; //列-3
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                            tIndex = curIndex + n * lutWidth + 3; //列+3
                            if (lootUpTableMask[tIndex] == 1)
                            {
                                tmpRowValue = tmpRowLookUpTable[tIndex];
                                tmpColValue = tmpColLookUpTable[tIndex];
                                hasValue = true;
                                break;
                            }
                        }
                        if (hasValue)
                        {
                            tmpColLookUpTable[curIndex] = tmpColValue;
                            tmpRowLookUpTable[curIndex] = tmpRowValue;
                            continue;
                        }
                    }
                }
            }
            //下面处理第一列，最后一列
            for (j = 0; j < lutHeight; j++)
            {
                i = 0;//第一列
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tIndex = curIndex + 1;
                    if (tmpRowLookUpTable[tIndex] != 0)
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];//取下一列的值
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }

                i = lutWidth - 1;//最后一列
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tIndex = curIndex - 1;
                    if (tmpRowLookUpTable[tIndex] != 0)
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];//取左边的值
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
            }
            for (i = 0; i < lutWidth; i++) //第一行，最后一行
            {
                j = 0;//第一行
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tIndex = curIndex + lutWidth;// (j + 1) * width + i;
                    if (tmpRowLookUpTable[tIndex] != 0)//取下一行的值
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
                j = lutHeight - 1;//最后一行
                curIndex = j * lutWidth + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tIndex = curIndex - lutWidth;// (j - 1) * width + i;
                    if (tmpRowLookUpTable[tIndex] != 0)//取上一行的值
                    {
                        tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[tIndex];
                        tmpColLookUpTable[curIndex] = tmpColLookUpTable[tIndex];
                    }
                }
            }
            GC.Collect();
        }

        private void Chazhi(byte[] lootUpTableTag, int width, int height, UInt16[] tmpRowLookUpTable, UInt16[] tmpColLookUpTable)
        {//实现一期插值方法，取上下左右平均值,但是一期平均的是 实际值
            byte[] m_lpPos2, m_lpPos3;
            m_lpPos2 = new byte[width * height];
            m_lpPos3 = new byte[width * height];
            int i, j, k, m, n, m_iTempNum, flag;
            int sum, sum2;
            int curIndex;
            int tIndex;
            flag = 0;
            int rOff = 0;
            int mOff = 0;
            lootUpTableTag.CopyTo(m_lpPos2, 0);
            for (k = 0; k < 2; k++)
            {
                m_lpPos2.CopyTo(m_lpPos3, 0);
                for (j = 1; j < height - 1; j++)
                {
                    rOff = j * width;
                    for (i = 1; i < width - 1; i++)
                    {
                        //这里的差值方法是取周围九个点的均值,这里是一期的实现，用的是目标区域四周值来插值。
                        curIndex = rOff + i;
                        if (m_lpPos2[curIndex] == 0 || tmpRowLookUpTable[curIndex] == 0)
                        {
                            sum2 = 0;
                            sum = 0;
                            m_iTempNum = 0;
                            for (m = -1; m <= 1; m++)
                            {
                                mOff = (j + m) * width + i;
                                for (n = -1; n <= 1; n++)
                                {
                                    tIndex = mOff + n;
                                    if (m_lpPos2[tIndex] != 0 && tmpRowLookUpTable[tIndex] != 0)
                                    {
                                        m_iTempNum++;
                                        sum += tmpRowLookUpTable[tIndex];
                                        sum2 += tmpColLookUpTable[tIndex];
                                    }
                                }
                            }
                            if (m_iTempNum != 0)
                            {
                                tmpRowLookUpTable[curIndex] = (UInt16)(sum * 1.0 / m_iTempNum + 0.5);
                                tmpColLookUpTable[curIndex] = (UInt16)(sum2 * 1.0 / m_iTempNum + 0.5);
                                m_lpPos3[curIndex] = 1;
                            }
                            else
                                flag = 1;
                        }
                    }
                }
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    m_lpPos3.CopyTo(m_lpPos2, 0);
                }
            }
            //下面处理第一行，最后一行，第一列，最后一列
            for (j = 0; j < height; j++)
            {
                i = 0;
                curIndex = j * width + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[curIndex + 1];//取下一列的值
                    tmpColLookUpTable[curIndex] = tmpColLookUpTable[curIndex + 1];
                }
                i = width - 1;
                curIndex = j * width + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[curIndex - 1];//取左边的值
                    tmpColLookUpTable[curIndex] = tmpColLookUpTable[curIndex - 1];
                }
            }
            for (i = 0; i < width; i++)
            {
                j = 0;
                curIndex = j * width + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[(j + 1) * width + i];//取下一行的值
                    tmpColLookUpTable[curIndex] = tmpColLookUpTable[(j + 1) * width + i];
                }
                j = height - 1;
                curIndex = j * width + i;
                if (tmpRowLookUpTable[curIndex] == 0)
                {
                    tmpRowLookUpTable[curIndex] = tmpRowLookUpTable[(j - 1) * width + i];//取上一行的值
                    tmpColLookUpTable[curIndex] = tmpColLookUpTable[(j - 1) * width + i];
                }
            }
            m_lpPos2 = null;
            m_lpPos3 = null;  //l-h test
        }
        
    }
}
