using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class ComputeArgument
    {
        public int[] BandNos;
        public string Express;

        public ComputeArgument(int[] bandNos, string express)
        {
            BandNos = bandNos;
            Express = express;
        }
    }
}
