using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public interface IContourGenerateTool:IRasterTool
    {
        bool IsOutputUncompleted { get; set; }
        double NoDataForOutsideAOI { get; set; }
        int Sample { get; set; }
        double[] ContourValues { get; }
        ContourLine[] Generate(IRasterBand raster, double[] contourValues,Action<int,string> tracker);
        ContourLine[] Generate(IRasterBand raster, double[] contourValues, int[] aoi, Action<int, string> tracker);
        ContourLine[] Generate(IRasterBand raster, double beginValue, double interleave, double endValue, Action<int, string> tracker);
        ContourLine[] Generate(IRasterBand raster, double beginValue, double interleave, double endValue, int[] aoi, Action<int, string> tracker);
    }
}
