using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IGeoPanAdjust
    {
        GeoDo.RSS.Core.DF.CoordEnvelope EnvelopeBeforeAdjusting { get; }
        void Start();
        bool IsAdjusting { get; }
        void ApplyAdjust(double offsetGeoX, double offsetGeoY);
        void Cancel();
        void Save();
        void Stop(bool isSave);
        bool IsHasUnsavedGeoAdjusted { get; }
    }
}
