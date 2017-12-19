using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    /// <summary>
    /// 等值线提取工具
    /// </summary>
    public static class EqualValueCal
    {
        [DllImport("gdal17.dll")]
        public static extern CPLErr GDALMEMContourGenerate(double[] BandData, int nXSize, int nYSize, double[] padfTransfrom, string pszWKT,
                                                           double dfContourInterval, double dfContourBase,
                                                           int nFixedLevelCount, double[] padfFixedLevels,
                                                           int bUseNoData, double dfNoDataValue,
                                                           string pszDstFilename,
                                                           GDALProgressFunc pfnProgress, IntPtr pProgressArg);

    }
}
