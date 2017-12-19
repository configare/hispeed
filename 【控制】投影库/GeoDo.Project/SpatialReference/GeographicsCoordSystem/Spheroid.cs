using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    [Serializable]
    public class Spheroid:ICloneable
    {
        public string Name = "WGS_1984";
        public double SemimajorAxis = 6378137.0d;
        public double SemiminorAxis = 6356752.3142451793d;   //6356863.018773047300000000d;
        public double InverseFlattening = 298.257223563d;   //
        private const double MINI_DOUBLE = 0.00000000001;   //比较时参照的最小浮点数        

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
            if (vs.Length == 3 || (vs.Length == 4 && vs[3] == string.Empty))
                InverseFlattening = double.Parse(vs[2]);
            else if (vs.Length == 4)
                InverseFlattening = double.Parse(vs[3]);
        }

        public bool IsSame(Spheroid sp)
        {
            return sp != null &&(
                     (Math.Abs(SemiminorAxis - sp.SemiminorAxis) < MINI_DOUBLE &&
                      Math.Abs(SemimajorAxis - sp.SemimajorAxis) < MINI_DOUBLE) ||
                     ( Math.Abs(SemimajorAxis - sp.SemimajorAxis) < MINI_DOUBLE &&
                      Math.Abs(InverseFlattening - sp.InverseFlattening) < MINI_DOUBLE));
        }

        /*
         * SPHEROID["WGS_1984",6378137.0,298.257223563]
         */
        public override string ToString()
        {
            return "SPHEROID[\"" + Name + "\"," + SemimajorAxis.ToString() + "," + InverseFlattening.ToString() + "]";
        }

        public object Clone()
        {
            Spheroid sp = new Spheroid();
            sp.InverseFlattening = this.InverseFlattening;
            sp.Name = this.Name;
            sp.SemimajorAxis = this.SemimajorAxis;
            sp.SemiminorAxis = this.SemiminorAxis;
            return sp;
        }
    }
}
