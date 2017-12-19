using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class Datum:ISpatialRefFormat
    {
        public string Name = "D_WGS_1984";
        public Spheroid Spheroid = new Spheroid(); //WGS-84

        public Datum()//WGS-84
        { 
        }

        //DATUM["D_WGS_1984",
        //             SPHEROID["WGS_1984",6378137.0,298.257223563]
        //            ]
        public Datum(WktItem wktItem)
        {
            Spheroid = new Spheroid(wktItem.GetWktItem("SPHEROID"));
            Name = (wktItem.Value.Split(',')[0]).Replace("\"", string.Empty);
        }

        public Datum(string name, Spheroid spheroid)
        {
            Name = name;
            Spheroid = spheroid;
        }

        public bool IsSame(Datum datum)
        {
            try
            {
                return datum != null && datum.Spheroid.IsSame(Spheroid);
            }
            catch { return false; }
        }

        /*
         * DATUM[ "D_WGS_1984",
                        SPHEROID["WGS_1984",6378137.0,298.257223563]
                      ]
         */
        public override string ToString()
        {
            return "DATUM[\"" + Name + "\"," + Spheroid.ToString() + "]";
        }

        #region ISpatialRefFormat Members

        public string FormatToString()
        {
            return "DATUM[\"" + Name + "\",\n" +
                     "                     " + Spheroid.ToString() + "\n                    ]";
        }

        #endregion
    }
}
