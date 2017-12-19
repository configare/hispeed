using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public static class SupportedFileFilters
    {
        public static string HdfFilter = "HDF文件(*.hdf)|*.hdf";
        public static string LdfFilter = "局地文件(*.ldf,*.ldff,*.ld2,*.ld3)|*.ldf;*.ld2;*.ld3";
        public static string NoaaFilter = "NOAA数据(*.1bd,*.L1b,*1b,*.1a5)|*.1bd;*L1b;*1b;*.1a5";
        public static string HJFilter = "环境星数据(*.xml,*.tif)|*.xml;*tif";
        public static string ImageFilterString = "普通栅格(*.tiff,*.tif,*.img,*.bmp,*.jpg,*.jpeg)|*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.img";
        public static string VectorFilterString = "矢量数据(*.shp)|*.shp";
        public static string SrfFilterString = "SMART Raster(*.dat)|*.dat";
        public static string ThemeTemplateFilterString = "专题图(*.gxd,*.gxt)|*.gxt;*.gxd";
        public static string ExcelFilterString = "MS Excel Files(*.xls,*.xlsx)|*.xls;*.xlsx";
        public static string TextFilterString = "文本文件(*.txt)|*.txt";
        //public static string AnimationFilterString = "动画文件(*.GIF)|*.GIF|AVI文件(*.AVI)|*.AVI";
        //public static string MvgFilterString = "二值图/多值图(*.mvg)|*.mvg";
        //public static string UnknowImageFilterString = "其他栅格数据(*.*)|*.*";
        public static string MicapsFilterString = "常规观测数据(*.000)|*.000";
        public static string ContourFilterString = "等值线(*.xml)|*.xml";
        public static string AllFilterString = "所有格式(*.*)|*.*";
        public static string TilFilterString = "TIL File(*.til)|*.til";
        public static string GPCFilterString = "GPC数据(*.GPC)|*.GPC";

        public static string[] AllFileFilters = new string[] 
        {
            LdfFilter,
            HdfFilter,
            NoaaFilter,
            HJFilter,
            ImageFilterString,
            VectorFilterString,
            SrfFilterString,
            MicapsFilterString,
            //MvgFilterString,
            //UnknowImageFilterString,
            ThemeTemplateFilterString,
            ExcelFilterString,
            TextFilterString,
            ContourFilterString,
            TilFilterString,
            //AnimationFilterString,
#if !PUB
            GPCFilterString,
#endif
            AllFilterString
        };
    }
}
