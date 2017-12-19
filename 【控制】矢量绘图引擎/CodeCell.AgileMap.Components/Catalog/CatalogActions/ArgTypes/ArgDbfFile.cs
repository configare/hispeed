using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;

namespace CodeCell.AgileMap.Components
{
    public class ArgDbfFile:ArgFile
    {
        protected override string GetFilter()
        {
            return "dBase Files(*.dbf)|*.dbf";
        }
    }
}
