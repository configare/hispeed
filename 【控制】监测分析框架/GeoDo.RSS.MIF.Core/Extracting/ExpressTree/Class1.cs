using System;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Core
{
    public class FuncBuilderX : IExtractFuncProvider<UInt16>
    {
        /*
        public Func<int, UInt16[], bool> GetBoolFunc()
        {
            return (idx, values) => { return (values[0] > 300) && (values[1] > 230); };
        }
		*/

        public Func<int, UInt16[], bool> GetBoolFunc()
        {
            return (idx, values) =>
            {
                return (values[1] / 10f > 0) && (values[1] / 10f < 15) &&
                     (values[2] / 10f < 280) && (values[2] / 10f > 244) && (values[0] / 10f > 25) &&
                     ((float)(values[0] / 10f - values[1] / 10f) / (values[0] / 10f + values[1] / 10f) > 0.5) &&
                     ((float)(values[0] / 10f - values[1] / 10f) / (values[0] / 10f + values[1] / 10f) < 1) &&
                     (values[0] / 10f < 100);
            };
        }


        public float NDVI(UInt16 b1, UInt16 b2)
        {
            return (b1 + b2) == 0 ? 0f : (b1 - b2) / (float)(b1 + b2);
        }
    }
}