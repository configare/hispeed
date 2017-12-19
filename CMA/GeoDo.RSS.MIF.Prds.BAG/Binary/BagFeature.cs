using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    internal class BagFeature
    {
        private const string INFO_FORMAT =
            "    NDVI :  {0}\n";

        public float Ndvi;

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,Ndvi);
        }
    }
}
