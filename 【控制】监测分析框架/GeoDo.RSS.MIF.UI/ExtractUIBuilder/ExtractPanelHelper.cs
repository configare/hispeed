using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.UI
{
    internal class ExtractPanelHelper
    {
        public Dictionary<string, string> BandDes;

        public ExtractPanelHelper()
        {
            BandDes = new Dictionary<string, string>();
            BandDes.Add("Visible", "可见光");
            BandDes.Add("Visiblerdd", "可见光(红光)");
            BandDes.Add("NearInfrared", "近红外");
            BandDes.Add("MiddleInfrared", "中红外");
            BandDes.Add("FarInfrared", "远红外");
            BandDes.Add("ShortInfrared", "短波红外");
        }
    }
}