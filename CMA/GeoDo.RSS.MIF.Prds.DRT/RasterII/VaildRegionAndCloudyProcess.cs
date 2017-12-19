using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class VaildRegionAndCloudyProcess
    {
        /// <summary>
        /// 比较两值，返回非云数据或云
        /// </summary>
        /// <param name="values"></param>
        /// <param name="cloudy"></param>
        /// <param name="result"></param>
        /// <returns>返回值为false 表示不是云，需进一步按最大值、最小值等处理
        ///          返回值为true 表示其中有云，比较结果采用result结果即可
        /// </returns>
        public static bool CompareProcessCloudy(Int16[] values, Int16 cloudy, out Int16 result)
        {
            result = Int16.MinValue;
            if (values == null || values.Length != 2)
                return false;
            if (values[0] == cloudy || values[1] == cloudy)
            {
                if (values[0] == cloudy)
                    result = values[1];
                else
                    result = values[0];
            }
            if (result == cloudy || result != Int16.MinValue)
                return true;
            return false;
        }

        public static bool CompareProcessVaildRegion(Int16[] values, double[] vaildRegion, Int16 invaildFlag, out Int16 result)
        {
            result = Int16.MinValue;
            if (values == null || values.Length != 2)
                return false;
            Int16[] compareTemp = new Int16[2];
            for (int i = 0; i < 2; i++)
            {
                if (values[i] >= vaildRegion[0] && values[i] <= vaildRegion[1])
                    compareTemp[i] = values[i];
                else
                    compareTemp[i] = invaildFlag;
            }
            if (compareTemp[0] == invaildFlag || compareTemp[1] == invaildFlag)
            {
                if (compareTemp[0] == invaildFlag)
                    result = values[1];
                else
                    result = values[0];
            }
            if (result == invaildFlag || result != Int16.MinValue)
                return true;
            return false;
        }

        public static bool AVG(Int16[] values, Int16 cloudy, Int16[] vaildRegion, Int16 invaildFlag, out Int16 result)
        {
            result = Int16.MinValue;
            if (values == null || values.Length == 0)
                return false;
            int length = values.Length;
            double sum = 0;
            double count = 0;
            for (int i = 0; i < length; i++)
            {
                if (values[i] == cloudy)
                    continue;
                if (values[i] < vaildRegion[0] || values[1] > vaildRegion[1])
                    continue;
                sum += values[i];
                count++;
            }
            if (count == 0)
                result = 0;
            result = (Int16)(sum / count);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="nanValue"></param>
        /// <param name="result"></param>
        /// <returns>是否将result值更新至maxdata</returns>
        public static bool ProcessMaxNanValues(Int16[] values, Int16[] cloudValue, Int16[] waterValue, double maxValue, double minValue, Int16 defCloudy, out Int16 result)
        {
            result = Int16.MinValue;
            if (values[0] == Int16.MinValue)
                return false;
            else
            {
                result = values[0];
                if (values[1] == Int16.MinValue)
                    return true;
                if (isNanValue(values[1], waterValue))
                    return false;
                if (values[0] == Int16.MinValue && isNanValue(values[0], cloudValue))
                {
                    result = defCloudy;
                    return true;
                }
                if (isNanValue(values[0], cloudValue))
                {
                    if (isNanValue(values[1], cloudValue))
                    {
                        result = defCloudy;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    if (isNanValue(values[0], waterValue))
                    {
                        return true;
                    }
                    if (!isInValidRange(maxValue, minValue, values[0]))
                        return false;
                    else
                    {
                        if (isNanValue(values[1], cloudValue))
                        {
                            return true;
                        }
                        else
                        {
                            if (values[1] < values[0])
                            {

                                result = values[0];
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                }
            }
        }

        public static bool ProcessMaxNanValues(Int16[] values, Int16[] cloudValue, Int16 defCloudy, out Int16 result)
        {
            result = Int16.MinValue;
            if (values[0] == Int16.MinValue)
                return false;
            else
            {
                result = values[0];
                if (values[1] == Int16.MinValue)
                    return true;
                if (values[0] == Int16.MinValue && isNanValue(values[0], cloudValue))
                {
                    result = defCloudy;
                    return true;
                }
                if (isNanValue(values[0], cloudValue))
                {
                    if (isNanValue(values[1], cloudValue))
                    {
                        result = defCloudy;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    if (isNanValue(values[1], cloudValue))
                    {
                        return true;
                    }
                    else
                    {
                        if (values[1] < values[0])
                        {

                            result = values[0];
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
        }

        public static bool isNanValue(Int16 value, Int16[] nanValues)
        {
            if (nanValues == null || nanValues.Length < 1)
                return false;
            foreach (Int16 item in nanValues)
            {
                if (value == item)
                    return true;
            }
            return false;
        }

        private static bool isInValidRange(double maxValue, double minValue, Int16 value)
        {
            if (value <= maxValue && value >= minValue)
                return true;
            else
                return false;
        }

        public static bool ProcessMinNanValues(Int16[] values, Int16[] cloudValue, Int16[] waterValue, double maxValue, double minValue, Int16 defCloudy, out Int16 result)
        {
            result = values[0];
            if (values[0] == Int16.MaxValue)
                return false;
            else
            {
                if (values[1] == Int16.MaxValue)
                    return true;
                if (isNanValue(values[1], waterValue))
                    return false;
                if (values[0] == Int16.MaxValue && isNanValue(values[0], cloudValue))
                {
                    result = defCloudy;
                    return true;
                }
                if (isNanValue(values[0], cloudValue))
                {
                    if (isNanValue(values[1], cloudValue))
                    {
                        result = defCloudy;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    if (isNanValue(values[0], waterValue))
                        return true;
                    if (!isInValidRange(maxValue, minValue, values[0]))
                        return false;
                    else
                    {
                        if (isNanValue(values[1], cloudValue))
                            return true;
                        else
                        {
                            if (values[1] > values[0])
                                return true;
                            else
                                return false;
                        }
                    }
                }
            }
        }
    }
}
