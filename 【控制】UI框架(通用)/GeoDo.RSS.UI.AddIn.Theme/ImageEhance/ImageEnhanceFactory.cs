using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public static class ImageEnhanceFactory
    {
        private static string _dirName = AppDomain.CurrentDomain.BaseDirectory + "图像增强方案";

        public static string[] GetEnhanceFiles()
        {
            if (!Directory.Exists(_dirName))
                return null;
            return Directory.GetFiles(_dirName, "*.xml", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 通过文件名信息获取增强方案文件名
        /// </summary>
        /// <param name="satellite">卫星名</param>
        /// <param name="sensor">传感器名</param>
        /// <param name="dateTime">时间</param>
        /// <param name="productName">产品名，如：沙尘、海冰等，可通过session.ActiveMonitoringProduct.Name获取</param>
        /// <returns></returns>
        public static string GetEnhanceFileByFileInfo(string satellite, string sensor, DateTime dateTime, string productName)
        {
            return GetEnhanceFileByFileInfo(satellite, sensor, dateTime, productName, false);
        }

        public static string GetEnhanceFileByFileInfo(string satellite, string sensor, DateTime dateTime, string productName, bool isOrbit)
        {
            if (string.IsNullOrEmpty(productName))
                productName = "其它";
            string name = string.Empty;
            foreach (char c in productName.ToCharArray(0, productName.Length))
            {
                if (c != Convert.ToChar(" "))
                    name += c;
            }
            if (!string.IsNullOrEmpty(satellite))
                satellite = "_" + satellite;
            else
                satellite = "_其它";
            if (!string.IsNullOrEmpty(sensor))
                sensor = "_" + sensor;
            else
                sensor = "_其它";
            string enhanceName = string.Empty;
            if (dateTime.Year != 1)  //无法获取时间信息
            {
                dateTime = dateTime.AddHours(8);
                string season = GetSeasonByDateTime(dateTime.ToString("MMdd"));
                string time = GetTimeByDateTime(dateTime.Hour);
                name = name + satellite + sensor + "_" + season + time + ".xml";
            }
            else
                name = name + satellite + sensor + ".xml";
            if (isOrbit)
                return "轨道_" + name;
            else
                return name;
        }

        private static string GetTimeStringByDateTime(DateTime dateTime)
        {
            if (dateTime.Year == 1)  //无法获取时间信息
                return string.Empty;
            string season = GetSeasonByDateTime(dateTime.ToString("MMdd"));
            string time = GetTimeByDateTime(dateTime.Hour);
            return season + time;
        }

        /*12~2月 冬季
	      3~5月  春季
	      6~8月  秋季
	      9~11月 冬季*/

        //4.1-6.15   //春
        //6.16-9-15  //夏
        //9.16-11.30 //秋
        //12.1-3.31; //冬
        private static string GetSeasonByDateTime(string mmdd)
        {
            if (string.Compare(mmdd, "0401") >= 0 && string.Compare(mmdd, "0615") <= 0)
            {
                return "春";
            }
            else if (string.Compare(mmdd, "0616") >= 0 && string.Compare(mmdd, "0915") <= 0)
            {
                return "夏";
            }
            else if (string.Compare(mmdd, "0916") >= 0 && string.Compare(mmdd, "1130") <= 0)
            {
                return "秋";
            }
            else
            {
                return "冬";
            }
        }

        /*[7:00-12)  上午
         * [12-19)   下午
         * [19-7:00) 晚上 
         * 
         * 
         * [0,5)晚上
          [5,12)上午
          [12,19)下午
          [19,24)晚上*/
        private static string GetTimeByDateTime(int h)
        {
            if (h >= 7 && h < 12)
                return "上午";
            else if (h >= 12 && h < 19)
                return "下午";
            else
                return "晚上";
        }

        /// <summary>
        /// 根据栅格文件名查找图像增强方案文件路径
        /// </summary>
        /// <param name="rasterName"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        private static string GetEnhanceFileByRasterFileName(string rasterName, string productName)
        {
            using (IRasterDataProvider prd = RasterDataDriver.Open(rasterName) as IRasterDataProvider)
            {
                //首先判断是栅格文件再执行
                return GetEnhanceNameByRasterFileName(prd, productName);
            }
        }

        private static string getEnhanceFileName(string enhanceName)
        {
            if (string.IsNullOrWhiteSpace(enhanceName))
                return null;
            string[] files = GetEnhanceFiles();
            if (files == null || files.Length == 0)
                return null;
            foreach (string file in files)
            {
                string fname = Path.GetFileName(file);
                if (fname == enhanceName)
                    return file;
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据栅格文件名获取图像增强方案文件名
        /// </summary>
        /// <param name="rasterName"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static string GetEnhanceNameByRasterFileName(IRasterDataProvider raster, string productName)
        {
            DataIdentify did = raster.DataIdentify;
            if (did == null || !did.IsOrbit)
                return GetEnhanceNameByRasterFileName(raster, productName, false);
            else
                return GetEnhanceNameByRasterFileName(raster, productName, true);
        }

        public static string FindEnhanceFileNameByRasterFileName(IRasterDataProvider raster, string productName)
        {
            string enhanceName = GetEnhanceNameByRasterFileName(raster, productName);
            return getEnhanceFileName(enhanceName);
        }

        private static string GetEnhanceNameByRasterFileName(IRasterDataProvider raster, string productName, bool isOrbit)
        {
            if (raster == null)
                return string.Empty;
            RasterIdentify identify = new RasterIdentify(raster);
            if (identify == null)
                return string.Empty;
            string enhanceName = GetEnhanceFileByFileInfo(identify.Satellite, identify.Sensor, identify.OrbitDateTime, productName, isOrbit);
            return enhanceName;
        }

        public static string GetEnhanceNameCurDrawing(ISmartSession session)
        {
            if (session.SmartWindowManager == null)
                return null;
            ICanvasViewer viewer = session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return null;
            IRasterDataProvider raster = drawing.DataProviderCopy;
            if (!EnhancePicture(raster))//如果不是通道数大于3的图片 则不进行图像增强
                return null;
            IMonitoringSession msession = session.MonitoringSession as IMonitoringSession;
            string product = GetProduct(msession);
            string enhancePath = FindEnhanceFileNameByRasterFileName(raster, product);
            return enhancePath;
        }
        /// <summary>
        /// 判断文件是否是需要增强的图片 通道数大于3
        /// </summary>
        /// <param name="raster"></param>
        /// <returns></returns>
        private static bool EnhancePicture(IRasterDataProvider raster)
        {
           string[] _imageFormat=new string[] { ".BMP", ".PNG", ".JPG", ".JPEG", ".TIFF", ".TIF" };
           string ext = Path.GetExtension(raster.fileName);
           if (_imageFormat.Contains(ext.ToUpper()))//图片格式
               return raster.BandCount > 3 ? true : false;
           else
               return true;          
        }

        private static string GetProduct(IMonitoringSession session)
        {
            if (session == null)
                return "其它";
            if (session.ActiveMonitoringProduct == null)
                return "其它";
            return session.ActiveMonitoringProduct.Name;
        }

        private static DataIdentify GetDataIdentify(string fname)
        {
            using (IRasterDataProvider prd = RasterDataDriver.Open(fname) as IRasterDataProvider)
            {
                return prd == null ? null : prd.DataIdentify;
            }
        }
    }
}
