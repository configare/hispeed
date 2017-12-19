using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.ValueComposite
{
    public class VaildValueProcess
    {
        public static bool ProcessMaxNanValues(Int16[] values, Int16[] cloudValue, Int16[] waterValue, Int16 defCloudy, out Int16 result)
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
                    //if (!isInValidRange(maxValue, minValue, values[0]))
                    //    return false;
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

        public static bool ProcessMinNanValues(Int16[] values, Int16[] cloudValue, Int16[] waterValue, Int16 defCloudy, out Int16 result)
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
                    //if (!isInValidRange(maxValue, minValue, values[0]))
                    //    return false;
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
