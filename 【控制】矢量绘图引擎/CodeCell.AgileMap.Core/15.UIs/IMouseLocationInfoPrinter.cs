using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMouseLocationInfoPrinter
    {
        void Print(object sender, int x, int y, float prjX, float prjY, float geoX, float geoY);
    }
}
