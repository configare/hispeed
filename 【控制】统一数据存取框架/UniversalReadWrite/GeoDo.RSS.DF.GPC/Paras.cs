using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GPC
{
    partial class Callatlon
    {
        private const int MAXBOX = 6596;
        private const byte MAXLAT = 72;
        private const byte MAXLON = 144;
        /// <summary>
        /// 每个纬度带中的cell个数
        /// </summary>
        private static UInt16[] ICELLS = new ushort[MAXLAT] {3,   9,  16,  22,  28,  34,  40,  46,  52,  58,             
                64,  69,  75,  80,  85,  90,  95, 100, 104, 108,             
                112, 116, 120, 123, 126, 129, 132, 134, 136, 138,             
                140, 141, 142, 143, 144, 144, 144, 144, 143, 142,             
                141, 140, 138, 136, 134, 132, 129, 126, 123, 120,             
                116, 112, 108, 104, 100,  95,  90,  85,  80,  75,             
                69,  64,  58,  52,  46,  40,  34,  28,  22,  16,             
                 9,   3 };
        /// <summary>
        /// 为获得cell在每个纬度带中的序号，需lonIndex减去对应值
        /// </summary>
        private static UInt16[] NCELLS = new ushort[MAXLAT] { 0,   3,  12,  28,  50,  78, 112, 152, 198, 250,             
            308, 372, 441, 516, 596, 681, 771, 866, 966,1070,             
            1178,1290,1406,1526,1649,1775,1904,2036,2170,2306,             
            2444,2584,2725,2867,3010,3154,3298,3442,3586,3729,             
            3871,4012,4152,4290,4426,4560,4692,4821,4947,5070,             
            5190,5306,5418,5526,5630,5730,5825,5915,6000,6080,             
            6155,6224,6288,6346,6398,6444,6484,6518,6546,6568,             
            6584,6593 };
    }
}
