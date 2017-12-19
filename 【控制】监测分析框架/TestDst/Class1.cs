using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestDst
{
    class Class1
    {
        /*
       public Func<int, UInt16[], bool> GetBoolFunc()
       {
           return (idx, values) => { return (values[0] > 300) && (values[1] > 230); };
       }
       */

        public Func<int, UInt16[], bool> GetBoolFunc()
        {
            return (idx, values) => { return ((values[0] / 10f) > 28) && (values[0] / 10f < 78) && (values[1] / 10f > 245) && (values[1] / 10f < 293) && (values[2] / 10f > 28) && ((values[2] / 10f - values[0] / 10f) > 0) && ((values[2] / 10f - values[1] / 10f + 250) > 15); };
        }


        public float NDVI(UInt16 b1, UInt16 b2)
        {
            return (b1 + b2) == 0 ? 0f : (b1 - b2) / (float)(b1 + b2);
        }
    }
}
