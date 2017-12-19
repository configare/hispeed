using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.DF.GDAL.H4BandPrd
{
    internal class DefaultBandNameParser:IBandNameParser
    {
        private const string EXP_DIGITAL = @"^(?<bNO>\d*)$";
        private const string EXP_RANGE = @"^(?<bNO>\d*)~(?<eNO>\d*)$";
        private const string EXP_HI = @"^(?<bNO>\d*)hi$";
        private const string EXP_LO = @"^(?<bNO>\d*)lo$";

        public BandName[] Parse(string bandName)
        {
            if (string.IsNullOrEmpty(bandName))
                return null;
            List<BandName> bandNames = new List<BandName>();
            string[] parts = bandName.Split(',');
            foreach (string prt in parts)
            {
                BandName[] bNames = GetBandName(prt);
                //不支持的波段名称格式
                if (bNames == null)
                    return null;
                bandNames.AddRange(bNames);
            }
            TryHandleHI_LO(bandNames);
            return bandNames.Count > 0 ? bandNames.ToArray() : null;
        }

        private void TryHandleHI_LO(List<BandName> bandNames)
        {
            for (int i = 0; i < bandNames.Count; i++)
            {
                if (bandNames[i].Name.EndsWith("lo"))
                    IncreaseBandNo(i+1, bandNames);
            }
        }

        private void IncreaseBandNo(int bIndex, List<BandName> bandNames)
        {
            for (int i = bIndex; i < bandNames.Count; i++)
                bandNames[i].Index++;
        }

        private BandName[] GetBandName(string prt)
        {
            Match match = null;
            match = Regex.Match(prt, EXP_DIGITAL);
            if (match.Success)
                return new BandName[] { new BandName(int.Parse(match.Groups["bNO"].Value)) };
            match = Regex.Match(prt, EXP_RANGE);
            if (match.Success)
            {
                int bNO = int.Parse(match.Groups["bNO"].Value);
                int eNO = int.Parse(match.Groups["eNO"].Value);
                List<BandName> bnames = new List<BandName>();
                for (int b = bNO; b <= eNO; b++)
                    bnames.Add(new BandName(b));
                return bnames.ToArray();
            }
            match = Regex.Match(prt, EXP_LO);
            if (match.Success)
                return new BandName[] { new BandName(int.Parse(match.Groups["bNO"].Value), match.Groups["bNO"].Value + "lo") };
            match = Regex.Match(prt, EXP_HI);
            if(match.Success)
                return new BandName[] { new BandName(int.Parse(match.Groups["bNO"].Value), match.Groups["bNO"].Value + "hi") };
            return null;
        }
    }
}
