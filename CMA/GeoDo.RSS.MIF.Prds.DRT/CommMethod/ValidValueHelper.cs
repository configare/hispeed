using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class ValidValueHelper
    {
        internal static bool IsVaild(float key, ArgumentItem argItem)
        {
            if (key == (float)argItem.Cloudy / argItem.Zoom)
                return false;
            if (argItem.Invaild != null && argItem.Invaild.Length != 0)
            {
                foreach (int item in argItem.Invaild)
                {
                    if (key == (float)item / argItem.Zoom)
                        return false;
                }
            }
            return true;
        }
    }
}
