using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public interface IGRIB2GridDefinitionSection
    {
        int GridTemplateNo { get; }
        string GridName { get; }
        int PointsNumber { get; }
        int Nx { get; }
        int Ny { get; }
        double LatFirstPoint { get; }
        double LonFirstPoint { get; }
        double LatEndPoint { get; }
        double LonEndPoint { get; }
        float Dx { get; }
        float Dy { get; }
        int ScanMode { get; }
    }
}
