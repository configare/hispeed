using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.DST
{
    internal class DstFeature
    {
        public UInt16 Vis;
        public UInt16 ShortInfrared;
        public UInt16 NearInfrared;
        public UInt16 MidInfrared;
        public UInt16 FarInfrared;
        public Int16 ShortVis;
        public Int16 ShortNearInfrared;
        public Int16 ShortFarInfrared;
        public Int16 MidFarInfrared;

        private const string INFO_FORMAT =
           "    可见光   :  {0}\n" +
           "    短波红外 :  {1}\n" +
           "    近红外   :  {2}\n" +
           "    中红外   :  {3}\n" +
           "    远红外   :  {4}\n" +
           "    短波可见光差值 :  {5}\n" +
           "    短波近红外差值 :  {6}\n" +
           "    短波远红外差值 :  {7}\n" +
           "    中远红外差值   :  {8}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                Vis == 9999 ? "" : Vis.ToString(),
                ShortInfrared == 9999 ? "" : ShortInfrared.ToString(),
                NearInfrared == 9999 ? "" : NearInfrared.ToString(),
                MidInfrared == 9999 ? "" : MidInfrared.ToString(),
                FarInfrared == 9999 ? "" : FarInfrared.ToString(),
                ShortVis == 9999 ? "" : ShortVis.ToString(),
                ShortNearInfrared == 9999 ? "" : ShortNearInfrared.ToString(),
                ShortFarInfrared == 9999 ? "" : ShortFarInfrared.ToString(),
                MidFarInfrared == 9999 ? "" : MidFarInfrared.ToString());
        }
    }
}
