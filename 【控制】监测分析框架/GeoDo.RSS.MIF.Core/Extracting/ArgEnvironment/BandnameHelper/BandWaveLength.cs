using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 波长范围,单位um
    /// </summary>
    public class BandWaveLength
    {
        public float Min = 0;
        public float Max = 0;
        public string WaveLengthType;

        public BandWaveLength(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public bool IsContains(float waveLength)
        {
            return waveLength >= Min && waveLength <= Max;
        }

        public override string ToString()
        {
            return Min.ToString().PadRight(6,' ')+ "um ~ " + Max.ToString().PadRight(6,' ') + "um";
        }
    }
}
