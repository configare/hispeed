using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SnwFeature
    {
        public short Ndsi;
        public float NdsiZoom = 1000f;
        public UInt16 Visible;
        public UInt16 ShortInfrared;
        public UInt16 FarInfrared;

        private const string INFO_FORMAT =
           "    NDSI :  {0}\n" +
           "    可见光 :  {1}\n" +
           "    远红外 :  {2}\n" +
           "    短红外 :  {3}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                Ndsi / NdsiZoom,
                Visible,
                FarInfrared,
                ShortInfrared);
        }
    }
}
