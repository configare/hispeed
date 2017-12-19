using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class LabelLocation:ICloneable
    {
        public ShapePoint Location = null;
        public int Angle = 0;

        public LabelLocation()
        { 
        }

        public LabelLocation(ShapePoint location, int angle)
        {
            Location = location;
            Angle = angle;
        }

        #region ICloneable 成员

        public object Clone()
        {
            return new LabelLocation(Location.Clone() as ShapePoint, Angle);
        }

        #endregion
    }
}
