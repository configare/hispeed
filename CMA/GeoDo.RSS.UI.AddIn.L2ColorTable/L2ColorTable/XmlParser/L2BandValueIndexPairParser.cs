using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public class L2BandValueIndexPairParser
    {
        public L2BandValueIndexPairParser()
        { 
        }

        public BandValueIndexPair[] Parse(string values, string indexes)
        {
            List<BandValueIndexPair> valueIndexPairs = new List<BandValueIndexPair>();
            int v = -1;
            if (int.TryParse(values, out v))
            {
                values = v.ToString() + "~" + v.ToString();
            }
            if (int.TryParse(indexes, out v))
            {
                indexes = v.ToString() + "~" + v.ToString();
            }
            //
            int v1 = -1, v2 = -1;
            int idx1 = -1, idx2 = -1;
            string[] parts = values.Split('~');
            v1 = int.Parse(parts[0]);
            v2 = int.Parse(parts[1]);
            //
            parts = indexes.Split('~');
            idx1 = int.Parse(parts[0]);
            idx2 = int.Parse(parts[1]);
            //
            if (idx1 == idx2)
            {
                valueIndexPairs.Add(new BandValueIndexPair(v1, v2, idx1));
                return valueIndexPairs.ToArray();
            }
            //
            if (v1 == v2 && idx1 != idx2)
            {
                valueIndexPairs.Add(new BandValueIndexPair(v1, v2, idx1));
                return valueIndexPairs.ToArray();
            }
            //
            int nV = (v2 - v1) + 1;
            int nIdx = (idx2 - idx1) + 1;
            //
            int span = nV / nIdx;
            if (nV < nIdx)
                span = 1;
            //
            for (int i = idx1, pv = v1; i <= idx2; i++, pv += (span+1))
            {
                valueIndexPairs.Add(new BandValueIndexPair(pv, pv + span, i));
            }
            return valueIndexPairs.ToArray();
        }
        
        public BandValueIndexPair[] Parse(string values, string indexes, Int16 transValue, bool isDisplay, Color transRGB, string description)
        {
            List<BandValueIndexPair> valueIndexPairs = new List<BandValueIndexPair>();
            int v = -1;
            if (int.TryParse(values, out v))
            {
                values = v.ToString() + "~" + v.ToString();
            }
            if (int.TryParse(indexes, out v))
            {
                indexes = v.ToString() + "~" + v.ToString();
            }
            //
            int v1 = -1, v2 = -1;
            int idx1 = -1, idx2 = -1;
            string[] parts = values.Split('~');
            v1 = int.Parse(parts[0]);
            v2 = int.Parse(parts[1]);
            //
            parts = indexes.Split('~');
            idx1 = int.Parse(parts[0]);
            idx2 = int.Parse(parts[1]);
            //
            if (idx1 == idx2)
            {
                valueIndexPairs.Add(new BandValueIndexPair(v1, v2, idx1, transValue, isDisplay, transRGB, description));
                return valueIndexPairs.ToArray();
            }
            //
            if (v1 == v2 && idx1 != idx2)
            {
                valueIndexPairs.Add(new BandValueIndexPair(v1, v2, idx1, transValue, isDisplay, transRGB, description));
                return valueIndexPairs.ToArray();
            }
            //
            int nV = (v2 - v1) + 1;
            int nIdx = (idx2 - idx1) + 1;
            //
            int span = nV / nIdx;
            if (nV < nIdx)
                span = 1;
            //
            for (int i = idx1, pv = v1; i <= idx2; i++, pv += (span + 1))
            {
                valueIndexPairs.Add(new BandValueIndexPair(pv, pv + span, i, transValue, isDisplay, transRGB, description));
            }
            return valueIndexPairs.ToArray();
        }
    }
}
