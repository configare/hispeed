using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IClass:IDisposable
    {
        int ID { get; }
        string Name { get; set; }
        Envelope FullEnvelope { get; }
        ISpatialReference SpatialReference { get; }
        enumCoordinateType CoordinateType { get; }
        enumCoordinateType OriginalCoordinateType { get; }
        IDataSource DataSource { get; }
        void Project(IRuntimeProjecter projecter, enumCoordinateType toCoordinateType);
        bool Disposed { get; }
        bool IsEmpty();
    }
}
