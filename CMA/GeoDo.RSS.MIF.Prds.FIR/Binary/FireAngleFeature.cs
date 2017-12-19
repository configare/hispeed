using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class FireAngleFeature
    {
        public double Glint;
        public double SunZ;
        public double SunA;
        public double SatZ;
        public double SatA;

        private const string INFO_FORMAT =
           "    耀斑角 :  {0}\n" +
           "    太阳天顶角 :  {1}\n" +
           "    太阳方位角 :  {2}\n" +
           "    卫星天顶角 :  {3}\n" +
           "    卫星方位角 ： {4}\n ";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                Glint,
                SunZ,
                SunA,
                SatZ,
                SatA
                );
        }
    }
}
