using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DST
{
    internal class DstFeatureFY3
    {
        //public float SolarZenith;
        //public short Height;
        //public short IDDI;
        public short Ref650;
        public short TBB11;
        public short TBB37;
        public int BTD11_12;
        public int BTD11_37;
        public float R47_64;
        public float NDVI;
        public float NDSI;
        public float NDDI;
        public short Ref470;
        public double IDSI;
        public short Ref1380;

        private const string INFO_FORMAT =
           //"    太阳天顶角 :  {0}\n" +
           //"    海拔高度 ：  {1}\n" +
           "    可见光0.65 :  {0}\n" +
           "    远红外11 :  {1}\n" +
           "    中红外3.7  :  {2}\n" +
           "    远红外11 - 远红外12 :  {3}\n" +
           "    远红外11 - 中红外3.7 :  {4}\n" +
           "    可见光0.47 / 可见光0.64 :  {5}\n" +
           "    NDVI :  {6}\n" +
           "    NDSI :  {7}\n" +
           "    NDDI :  {8}\n" +
           "    可见光0.47 :  {9}\n" +
           "    IDSI :  {10}\n" +
           "    可见光1.38 :  {11}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                //SolarZenith,
                //Height,
                Ref650,
                TBB11,
                TBB37,
                BTD11_12,
                BTD11_37,
                R47_64,
                NDVI,
                NDSI,
                NDDI,
                Ref470,
                IDSI,
                Ref1380);
        }
    }
}
