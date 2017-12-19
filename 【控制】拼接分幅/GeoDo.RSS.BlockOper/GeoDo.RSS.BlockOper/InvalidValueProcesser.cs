using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.BlockOper
{
    public static class InvalidValueProcesser<T> where T : IComparable
    {
        /// <summary>
        /// 无效值处理
        /// </summary>
        /// <param name="srcImage">源数据块</param>
        /// <param name="dstImage">待拼接数据块</param>
        /// <param name="rowColRange">行列开始结束序列号</param>
        /// <param name="invalidValue">无效值数组</param>
        public static void ProcessInvalidValue(T[] srcImage, ref T[] dstImage, int[] rowColRange, T[] invalidValue)
        {
            int beginRow = rowColRange[0];
            int endRow = rowColRange[1];
            int beginCol = rowColRange[2];
            int endCol = rowColRange[3];
            int dstwidth = rowColRange[4];
            int width = endCol - beginCol;
            int height = endRow - beginRow;
            for (int i = 0; i < height; i++)
            {
                int index = (beginRow + i) * dstwidth + beginCol;
                for (int j = 0; j < width; j++, index++)
                {
                    foreach (T value in invalidValue)
                    {
                        if (dstImage[index].CompareTo(value) == 0)
                        {
                            dstImage[index] = srcImage[i * width + j];
                            break;
                        }
                    }
                }
            }
        }

        public static void ProcessInvalidValue(ushort[] srcImage, ref ushort[] dstImage, int[] rowColRange, ushort[] invalidValue, bool smooth)
        {
            int beginRow = rowColRange[0];
            int endRow = rowColRange[1];
            int beginCol = rowColRange[2];
            int endCol = rowColRange[3];
            int dstwidth = rowColRange[4];
            int width = endCol - beginCol;
            int height = endRow - beginRow;
            bool isInvalid = false;
            for (int i = 0; i < height; i++)
            {
                int index = (beginRow + i) * dstwidth + beginCol;
                for (int j = 0; j < width; j++, index++)
                {
                    isInvalid = false;
                    foreach (ushort value in invalidValue)
                    {
                        if (dstImage[index]== value)
                        {
                            isInvalid = true;
                            dstImage[index] = srcImage[i * width + j];
                            break;
                        }
                    }
                    foreach (ushort value in invalidValue)
                    {
                        if (srcImage[i * width + j] ==value)
                        {
                            isInvalid = true;
                            break;
                        }
                    }
                    if (!isInvalid && smooth)  //做数据平滑，取平均值
                    {
                        dstImage[index] = (ushort)((dstImage[index] + srcImage[i * width + j]) / 2);
                    }
                }
            }
        }

        public static void ProcessInvalidValue(T[] tImage, ref T[] oImage, int[] rowColRange, T[] invalidValue, Func<long, bool> validFuncExt,Action<int> ChangeDstValue)
        {
            int beginRow = rowColRange[0];
            int endRow = rowColRange[1];
            int beginCol = rowColRange[2];
            int endCol = rowColRange[3];
            int dstwidth = rowColRange[4];
            int width = endCol - beginCol;
            int height = endRow - beginRow;
            bool valued = false;
            for (int i = 0; i < height; i++)
            {
                int index = (beginRow + i) * dstwidth + beginCol;
                for (int j = 0; j < width; j++, index++)
                {
                    valued = false;
                    foreach (T value in invalidValue)
                    {
                        if (oImage[index].CompareTo(value) == 0)
                        {
                            valued = true;
                            oImage[index] = tImage[i * width + j];
                            if (ChangeDstValue != null)
                                ChangeDstValue(index);
                            break;
                        }
                    }
                    if (!valued && validFuncExt != null && validFuncExt(index))
                    {
                        if (ChangeDstValue != null)
                            ChangeDstValue(index);
                        oImage[index] = tImage[i * width + j];
                    }
                }
            }
        }

        /// <summary>
        /// 无效值处理（重叠处进行特殊处理（最大值、最小值、平均值））
        /// </summary>
        /// <param name="srcImage">源数据块</param>
        /// <param name="dstImage">待拼接数据块</param>
        /// <param name="rowColRange">行列开始结束序列号</param>
        /// <param name="invalidValue">无效值数组</param>
        /// <param name="processId">交集处处理方法（取最大值，最小值，平均值）</param>
        /// <param name="extensionAction">均值函数</param>
        public static void ProcessInvalidValue(T[] srcImage, ref T[] dstImage, int[] rowColRange, T[] invalidValue, string processId,Func<T,T,T> extensionAction)
        {
            int beginRow = rowColRange[0];
            int endRow = rowColRange[1];
            int beginCol = rowColRange[2];
            int endCol = rowColRange[3];
            int dstwidth = rowColRange[4];
            int width = endCol - beginCol;
            int height = endRow - beginRow;
            switch (processId.ToUpper())
            {
                case "AVG":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            int index = (beginRow + i) * dstwidth + beginCol;
                            for (int j = 0; j < width; j++, index++)
                            {
                                foreach (T value in invalidValue)
                                {
                                    if (dstImage[index].CompareTo(value) == 0)
                                    {
                                        dstImage[index] = srcImage[i * width + j];
                                        break;
                                    }
                                    if (srcImage[i * width + j].CompareTo(value) == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        dstImage[index] = extensionAction(dstImage[index],srcImage[i * width + j]);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case "MAX":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            int index = (beginRow + i) * dstwidth + beginCol;
                            for (int j = 0; j < width; j++, index++)
                            {
                                foreach (T value in invalidValue)
                                {
                                    if (dstImage[index].CompareTo(value) == 0)
                                    {
                                        dstImage[index] = srcImage[i * width + j];
                                        break;
                                    }
                                    if (srcImage[i * width + j].CompareTo(value) == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (dstImage[index].CompareTo(srcImage[i * width + j]) > 0)
                                            break;
                                        else
                                            dstImage[index] = srcImage[i * width + j];
                                    }
                                }
                            }
                        }
                        break;
                    }
                case "MIN":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            int index = (beginRow + i) * dstwidth + beginCol;
                            for (int j = 0; j < width; j++, index++)
                            {
                                foreach (T value in invalidValue)
                                {
                                    if (dstImage[index].CompareTo(value) == 0)
                                    {
                                        dstImage[index] = srcImage[i * width + j];
                                        break;
                                    }
                                    if (srcImage[i * width + j].CompareTo(value) == 0)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        if (dstImage[index].CompareTo(srcImage[i * width + j]) < 0)
                                            break;
                                        else
                                            dstImage[index] = srcImage[i * width + j];
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        public static void ProcessInvalidValue(T[] srcImage, ref T[] dstImage, Size imageSize, T[] invalidValue)
        {
            int width = imageSize.Width;
            int height = imageSize.Height;
            int index = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++, index++)
                {

                    foreach (T value in invalidValue)
                    {
                        if (dstImage[index].CompareTo(value) == 0)
                        {
                            dstImage[index] = srcImage[index];
                            break;
                        }
                    }
                }
            }
        }
    }
}
