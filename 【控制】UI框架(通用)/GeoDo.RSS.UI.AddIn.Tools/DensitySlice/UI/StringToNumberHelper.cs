using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public static class StringToNumberHelper
    {
        //判断number参数是否是整数
        public static bool isIntegerNumber(String number)
        {
            number = number.Trim();
            String intNumRegex = "\\-{0,1}\\d+";//整数的正则表达式  
            if (Regex.Match(number, intNumRegex).Success)
                return true;
            else
                return false;
        }

        // 判断number参数是否是浮点数表示方式 
        public static bool isFloatPointNumber(String number)
        {
            number = number.Trim();
            String pointPrefix = "(\\-|\\+){0,1}\\d*\\.\\d+";//浮点数的正则表达式-小数点在中间与前面  
            String pointSuffix = "(\\-|\\+){0,1}\\d+\\.";//浮点数的正则表达式-小数点在后面  
            if (Regex.Match(number, pointPrefix).Success || Regex.Match(number,pointSuffix).Success)
                return true;
            else
                return false;
        }  
    }
}
