using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.Components
{
    public class ArgShpFile:ArgFile
    {
        protected override string GetFilter()
        {
            return "ESRI Shape Files(*.shp)|*.shp";
        }
    }
}
