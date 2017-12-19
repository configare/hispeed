using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class FilePrdMap
    {
        public string Filename;
        public IRasterDataProvider Prd;
        public int BandCount = 0;
        public int StartBand = 1;
        public int[] BandNums = null;
        public double Zoom = 1;
        public VaildPra VaildValue;
        public bool SameRegion = true;

        public FilePrdMap(string filename, double zoom, VaildPra vaild, int[] bandNums)
        {
            Filename = filename;
            Zoom = zoom;
            VaildValue = vaild;
            if (bandNums != null || bandNums.Length != 0)
            {
                BandNums = bandNums;
                BandCount = bandNums.Length;
            }
        }
    }

    public class VaildPra
    {
        public float Min;
        public float Max;

        public VaildPra(float min, float max)
        {
            Max = max;
            Min = min;
        }
    }
}
