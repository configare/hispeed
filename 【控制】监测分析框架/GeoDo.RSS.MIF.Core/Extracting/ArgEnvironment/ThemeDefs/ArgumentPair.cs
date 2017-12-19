using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class ArgumentPair : ArgumentBase
    {
        public ArgumentDef ArgumentMin;
        public ArgumentDef ArgumentMax;
        public string FineTuning = "1";

        public int ArgPairCount
        {
            get
            {
                int i = 0;
                if (ArgumentMin != null)
                    i++;
                if (ArgumentMax != null)
                    i++;
                return i;
            }
        }

        public override string ToString()
        {
            if (ArgumentMin == null && ArgumentMax == null)
                return "";
            else
            {
                return ArgumentMin.Defaultvalue + " " + ArgumentMax.Defaultvalue;
            }
        }
    }
}
