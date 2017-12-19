using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    [Serializable]
    public class AngularUnit:ICloneable
    {
        public string Name = "Degree";
        public double Value = 0d;
        private const double MINI_DOUBLE = 0.00000000001;   //比较时参照的最小正浮点数

        public AngularUnit(string name, double value)
        {
            Name = name;
            Value = value;
        }

        //UNIT["Degree",0.0174532925199433]
        public AngularUnit(WktItem wktItem)
        {
            string[] vs = wktItem.Value.Split(',');
            Name = vs[0].Replace("\"",string.Empty);
            Value = double.Parse(vs[1]);
        }

        public bool IsSame(AngularUnit angularUnit)
        {
            if (angularUnit == null)
                return false;
            try
            {
                return angularUnit.Name.ToUpper() == Name.ToUpper() && Math.Abs(Value - angularUnit.Value) < MINI_DOUBLE;
            }
            catch { return false; }
        }

        public override string ToString()
        {
            return "UNIT[\"" + Name + "\"," + Value.ToString() + "]";
        }

        public object Clone()
        {
            return new AngularUnit(this.Name, this.Value);
        }
    }
}
