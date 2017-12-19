using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class BandHelper
    {
        public static void GetBandFromBandName(PrjBand[] bands, string bandName, out string dsName, out int dsIndex)
        {
            dsName = "";
            dsIndex = -1;
            if (bands == null)
                return;
            foreach (PrjBand band in bands)
            {
                if (band.BandName == bandName)
                {
                    dsName = band.DataSetName;
                    dsIndex = band.DataSetIndex;
                }
            }
        }

        /// <summary>
        /// 根据波段序号获取波段信息
        /// </summary>
        /// <param name="bandNos">从1开始的波段号</param>
        /// <returns></returns>
        private static PrjBand[] GetPrjBandsVIRR(int[] bandNos)
        {
            PrjBand[] prjBand = OrbitBandDefCollection.VIRR_1000_OrbitDefCollecges();
            List<PrjBand> bands = new List<PrjBand>();
            foreach (int bandNo in bandNos)
            {
                if (bandNo <= 0 || bandNo > prjBand.Length)
                    continue;
                bands.Add(prjBand[bandNo - 1]);
            }
            return bands.ToArray();
        }
    }
}
