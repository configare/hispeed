using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class CLMFeature
    {
        public Int16 Ndsi;
        public Int16 Ndvi;
        public float NdsiZoom = 1000f;
        public float NdviZoom = 1000f;
        public UInt16 Visible;
        public UInt16 FarInfrared;
        public UInt16 nearInfrared;
        public UInt16 shortInfrared;
        public bool UseNDSI = true;
        public bool UseNearVisiable = true;
        public bool UseFarInfrared = true;
        public bool UseNDVI = false;
        public bool UseNearShort = false;
        public bool UseNearInfrared = false;

        private const string INFO_FORMAT =
           "{0}" +
           "{1}" +
           "{2}" +
           "{3}" +
           "{4}" +
           "{5}" +
           "{6}";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                                  "    可见光 :  " + Visible + "\n",
                UseFarInfrared ?  "    远红外 :  " + FarInfrared.ToString() + "\n" : "",
                UseNDSI ?         "    NDSI   :  " + (Ndsi / NdsiZoom).ToString() + "\n" : "",
                UseNearVisiable ? "    近红外/可见光 :  " + ((float)nearInfrared / Visible).ToString() + "\n" : "",
                UseNearInfrared ? "    近红外 :  " + nearInfrared.ToString() + "\n" : "",
                UseNearShort ?    "    近红外/短波红外 :  " + ((float)nearInfrared / shortInfrared).ToString() + "\n" : "",
                UseNDVI ?         "    NDVI   :  " + (Ndvi / NdviZoom).ToString() + "\n" : "");
        }
    }
}
