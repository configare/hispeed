using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.MathAlg;

namespace GeoDo.RSS.RasterTools
{
    public interface IScatterPlotTool:IRasterTool
    {
        void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi,LinearFitObject fitObj, Action<int, string> progressTracker);
        void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi,XYAxisEndpointValue endpointValues, LinearFitObject fitObj, Action<int, string> progressTracker);
    }
}
