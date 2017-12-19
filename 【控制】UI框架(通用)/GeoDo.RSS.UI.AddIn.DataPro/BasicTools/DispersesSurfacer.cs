using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    /// <summary>
    /// 离散点表面化工具
    /// </summary>
    public static class DispersesSurfacer
    {
        [DllImport("gdal17.dll")]
        public static extern CPLErr GDALGridCreate(GDALGridAlgorithm alg, IntPtr poOptions, UInt32 nPoints,
                 double[] padfX, double[] padfY, double[] padfZ,
                double dfXMin, double dfXMax, double dfYMin, double dfYMax,
                UInt32 nXSize, UInt32 nYSize, LDataType eType, IntPtr pData,
                GDALProgressFunc pfnProgress, IntPtr pProgressArg);

        [DllImport("gdal17.dll")]
        public static extern CPLErr GDALGridCreateTIF(GDALGridAlgorithm alg, IntPtr poOptions, UInt32 nPoints,
                 double[] padfX, double[] padfY, double[] padfZ,
                double dfXMin, double dfXMax, double dfYMin, double dfYMax,
                UInt32 nXSize, UInt32 nYSize, LDataType eType, string outFileName,
                GDALProgressFunc pfnProgress, IntPtr pProgressArg);

    }
}
