using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public interface IFeatureClassWriter:IDisposable
    {
        ISpatialReference SpatialReference { get; }
        void Write(IFeatureClassReader reader, IProgressTracker tracker,string name,string displayName,string description);
    }
}
