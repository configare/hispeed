using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class Spheroid
    {
        public string Name = "WGS_1984";
        public double SemimajorAxis = 6378245.000000000000000000d;
        public double SemiminorAxis = 6356863.018773047300000000d;
        public double InverseFlattening = 298.300000000000010000d;

        public Spheroid()//WGS-84
        { 
        }

        public Spheroid(string name, double semimajorAxis, double semiminorAxis, double inverseFlattening)
        {
            Name = name;
            SemimajorAxis = semimajorAxis;
            SemiminorAxis = semiminorAxis;
            InverseFlattening = inverseFlattening;
        }

        //SPHEROID["WGS_1984",6378137.0,298.257223563]
        public Spheroid(WktItem wktItem)
        {
            string[] vs = wktItem.Value.Split(',');
            Name = vs[0].Replace("\"", string.Empty);
            SemimajorAxis = double.Parse(vs[1]);
            InverseFlattening = double.Parse(vs[2]);
        }

        public bool IsSame(Spheroid sp)
        {
            return sp != null && 
                      Math.Abs(SemiminorAxis - sp.SemiminorAxis) < double.Epsilon &&
                      Math.Abs(SemimajorAxis - sp.SemimajorAxis) < double.Epsilon &&
                      Math.Abs(InverseFlattening - sp.InverseFlattening) < double.Epsilon;
        }

        /*
         * SPHEROID["WGS_1984",6378137.0,298.257223563]
         */
        public override string ToString()
        {
            return "SPHEROID[\"" + Name+"\"," + SemimajorAxis.ToString()+","+InverseFlattening.ToString()+"]";
        }
    }
}
