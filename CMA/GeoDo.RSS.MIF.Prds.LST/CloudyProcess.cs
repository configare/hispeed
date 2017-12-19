using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public class CloudyProcess
    {
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
    }
}
