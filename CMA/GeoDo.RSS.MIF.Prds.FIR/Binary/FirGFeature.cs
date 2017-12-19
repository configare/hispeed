using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class FirGFeature
    {
        public Int16 Ndvi;
        public float NdviZoom = 1000f;
        public UInt16 Visible;
        public UInt16 FarInfrared;
        public UInt16 NearInfrared;

        private const string INFO_FORMAT =
           "    NDVI :  {0}\n" +
           "    可见光 :  {1}\n" +
           "    近红外 :  {2}\n" +
           "    远红外 :  {3}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                Ndvi / NdviZoom,
                Visible,
                NearInfrared,
                FarInfrared);
        }
    }
}
