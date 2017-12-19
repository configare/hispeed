using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IDataSource:IDisposable
    {        
        string Name { get; }
        Envelope GetFullEnvelope();
        enumCoordinateType GetCoordinateType();
        ISpatialReference GetSpatialReference();
    }
}
