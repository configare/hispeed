using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureDataSource:IDataSource,IGridReader,IQueryFeatures
    {
        bool ReadIsFinished { get; }
        int GetFullGridCount();
        enumShapeType GetShapeType();
        string[] GetFieldNames();
        GridStateIndicator GridStateIndicator { get; }
    }
}
