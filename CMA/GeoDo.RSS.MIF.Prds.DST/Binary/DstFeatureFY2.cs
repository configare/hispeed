using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DST
{
    internal class DstFeatureFY2
    {
        public float TBD12;
        public float TBDBK12;
        public float IDDI;
        public float TBD13;
        public float TBD14;
        public float TBD2;
        public float TBD3;
        public float TBD4;
        public float RefMidIR;
        public float Visref;

        private const string INFO_FORMAT =
           "    远红外11 - 远红外12   :  {0}\n" +
           "    背景亮温远红外11 -远红外12   ：  {1}\n" +
           "    远红外11 - 中红外6.7  :  {2}\n" +
           "    远红外11 - 中红外3.8  :  {3}\n" +
           "    远红外12  :  {4}\n" +
           "    中红外6.7 :  {5}\n" +
           "    中红外3.8 :  {6}\n" +
           "    IDDI :  {7}\n" +
           "    RefMidIR :  {8}\n" +
           "    Visref :  {9}\n";

        public override string ToString()
        {
            return string.Format(INFO_FORMAT,
                TBD12,
                TBDBK12,
                TBD13,
                TBD14,
                TBD2,
                TBD3,
                TBD4,
                IDDI,
                RefMidIR,
                Visref);
        }
    }
}