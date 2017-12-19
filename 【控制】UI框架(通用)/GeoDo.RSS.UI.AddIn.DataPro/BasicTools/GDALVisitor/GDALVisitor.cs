using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public static class GDALVisitor
    {
        public enum GDALAccess
        {
            GA_ReadOnly = 0,
            GA_Update = 1
        }

        [DllImport("gdal14.dll")]
        public static extern void GDALAllRegister();

        [DllImport("gdal14.dll")]
        public static extern IntPtr GDALOpen(string pszFilename, GDALAccess eAccess);

        [DllImport("gdal14.dll")]
        public static extern int GDALGetRasterXSize(IntPtr GdalDataset);
        [DllImport("gdal14.dll")]
        public static extern int GDALGetRasterYSize(IntPtr GdalDataset);
        [DllImport("gdal14.dll")]
        public static extern int GDALGetRasterCount(IntPtr GdalDataset);

        [DllImport("gdal14.dll")]
        public static extern IntPtr GDALGetRasterBand(IntPtr GDALDatasetH, int n);

        [DllImport("gdal14.dll")]
        public static extern void GDALClose(IntPtr GDALDatasetH);

        [DllImport("gdal14.dll")]
        public static extern IntPtr GDALGetDriverByName(string name);

        [DllImport("gdal14.dll")]
        public static extern IntPtr GDALCreate(IntPtr hDriver,
                                  string name, int width, int height, int nBands, LDataType tyoe,
                                  ref string papszOptions);


        [DllImport("gdal14.dll")]
        public static extern unsafe int GDALDatasetRasterIO(
        IntPtr hDS, LFRWFlag eRWFlag,
         int nDSXOff, int nDSYOff, int nDSXSize, int nDSYSize,
         void* pBuffer, int nBXSize, int nBYSize, LDataType eBDataType,
         int nBandCount, int[] panBandCount,
         int nPixelSpace, int nLineSpace, int nBandSpace);


        [DllImport("gdal14.dll")]
        public static extern unsafe int GDALRasterIO(IntPtr hRBand, LFRWFlag eRWFlag,
               int nDSXOff, int nDSYOff, int nDSXSize, int nDSYSize,
               void* pBuffer, int nBXSize, int nBYSize, LDataType eBDataType,
               int nPixelSpace, int nLineSpace);


        [DllImport("gdal14.dll")]
        public static extern string GDALGetProjectionRef(IntPtr GDALDatasetH);
        [DllImport("gdal14.dll")]
        public static extern int GDALSetProjection(IntPtr GDALDatasetH, string pszPrj);
        [DllImport("gdal14.dll")]
        public static extern int GDALGetGeoTransform(IntPtr GDALDatasetH, double[] padf);
        [DllImport("gdal14.dll")]
        public static extern int GDALSetGeoTransform(IntPtr GDALDatasetH, double[] padf);

    }
}
