using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class AngularUnit
    {
        public string Name = "Degree";
        public double Value = 0d;

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
                return angularUnit.Name.ToUpper() == Name.ToUpper() && Math.Abs(Value - angularUnit.Value) < double.Epsilon;
            }
            catch { return false; }
        }

        public override string ToString()
        {
            return "UNIT[\"" + Name + "\"," + Value.ToString() + "]";
        }
    }
}
