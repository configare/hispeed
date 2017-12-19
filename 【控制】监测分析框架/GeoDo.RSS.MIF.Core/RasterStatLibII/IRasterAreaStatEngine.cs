using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterAreaStatEngine
    {
        /*
         * 分段面积统计＋行政区划
         */
        bool IsOnlyCount{get;set;}
        double Area(IRasterBand raster);
        double Area(IRasterBand raster, int[] aoi);
        double Area(IRasterBand raster, int[] aoi, object filter);//Func<T,bool>
        Dictionary<string, double> Area(StatisticDim dim, IRasterBand raster);
        Dictionary<string, double> Area(StatisticDim dim, IRasterBand raster, int[] aoi);
        Dictionary<string, double> Area(StatisticDim dim, IRasterBand raster, int[] aoi, object filter);
        Dictionary<string, Dictionary<string, double>> Area(StatisticDim dim1, StatisticDim dim2, IRasterBand raster);
        Dictionary<string, Dictionary<string, double>> Area(StatisticDim dim1, StatisticDim dim2, IRasterBand raster,int[] aoi);
        Dictionary<string, Dictionary<string, double>> Area(StatisticDim dim1, StatisticDim dim2, IRasterBand raster,int[] aoi,object filter);
    }
}
