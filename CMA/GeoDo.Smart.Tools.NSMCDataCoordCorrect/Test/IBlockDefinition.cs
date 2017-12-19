using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    public interface IBlockDefinition
    {
        string Name { get; }
        BlockItem[] GetAllBlockItems();
        BlockItem[] GetChinaRegionBlockItems();
        BlockItem GetBlockItem(double longitude, double latitude);
        BlockItem GetBlockItem(string name);
    }
}
