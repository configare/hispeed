using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureClass : IClass,IDisposable, IPersistable, IIdentifyFeatures
    {
        string[] FieldNames { get; }
        GridDefinition GridDefinition { get; }
        int FullGridCount { get; }
        IGrid[] Grids { get; }
        Feature[] GetVectorFeatures();
        void Remove(int[] oids);
        Envelope Envelope { get; }
        enumShapeType ShapeType { get; }
        MemoryGridLimiter GridLimiter { get; }
        void TryProject(IGrid grid);
        void AddGrid(IGrid grid);
        void ExChange(int gridNo, IGrid grid);
        void Remove(int gridNo);
        RepeatFeatureRecorder RepeatFeatureRecorder { get; }
        void ReLeveling();
        bool ImmidateReLeveling { get; set; }
        IRuntimeProjecter RuntimeProjecter { get; }
    }
}
