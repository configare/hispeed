using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class HdrProjectionInfo
    {
        public string Datum = "WGS-84";
        public string Name = string.Empty;
        public string Units = "Meters";
        public int ProjectionID = 0;//Geographic Latitude/Longitude
        public float[] PrjArguments = null;

        public HdrProjectionInfo()
        {
        }

        public override string ToString()
        {
            if (PrjArguments == null || ProjectionID == 0)
                return string.Empty;
            string str = "projection info = {";
            str += (ProjectionID.ToString() + ",");
            for (int i = 0; i < PrjArguments.Length; i++)
                str += (PrjArguments[i].ToString() + ",");
            str += (Datum + ",");
            str += (Name + ",");
            str += ("units = " + Units + "}");
            return str;
        }

        public string ToArgsString(char split)
        {
            string argsString = ProjectionID.ToString();
            if (PrjArguments != null)
                foreach (float args in PrjArguments)
                {
                    argsString += (split) + args.ToString();
                }
            return argsString;
        }
    }
}
