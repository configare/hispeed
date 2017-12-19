using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public class ExtractResultSmoother
    {
        /// <summary>
        /// 对判识结果进行平滑处理
        /// </summary>
        /// <param name="width">原始影像宽度</param>
        /// <param name="height">原始影像高度</param>
        /// <param name="result">待平滑判识结果（有沙尘索引号数组）</param>
        /// <returns>平滑后结果</returns>
        public int[] Smooth(int width, int height, int[] result)
        {
            int length = result.Length;
            List<int> visMarkList = new List<int>();//存放平滑掉的像元索引
            List<int> smoothResult = new List<int>();
            for (int i = 0; i < length; i++)
            {
                smoothResult.Add(result[i]);
            }
            for (int i = 0; i < length; i++)
            {
                //若为边缘两行两列，则不处理
                int xIndex = result[i] / width;
                int yIndex = result[i] % width;
                if (xIndex < 2 || yIndex < 2 || xIndex >= height - 2 || yIndex >= width - 2)
                    continue;
                int iaround = 0;
                for (int m = 0; m < 5; m++)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        int index = (xIndex + m - 2) * width + yIndex + n - 2;
                        if (isNotExistInSet(index, smoothResult) && isNotExistInSet(index, visMarkList))
                            iaround++;
                    }
                }
                if (iaround >= 15)
                {
                    smoothResult.Remove(result[i]);
                    visMarkList.Add(result[i]);
                }
            }
            return smoothResult.ToArray();
        }

        private bool isNotExistInSet(int element,List<int> set)
        {
            if (set.Count < 1)
                return true;
            int mid;//中间位置  
            int beg = 0;
            int last = set.Count - 1;
            while (beg <= last)
            {
                mid = (beg + last) / 2;
                if (element == set[mid])
                {
                    return false;
                }
                else if (set[mid] < element)
                {
                    beg = mid + 1;
                }
                else if (set[mid] > element)
                {
                    last = mid - 1;
                }
            }
            return true;
        }

        public string Smooth(string rasterIdentify, IRasterDataProvider dataPrd)
        {

            return null;
        }
    }
}
