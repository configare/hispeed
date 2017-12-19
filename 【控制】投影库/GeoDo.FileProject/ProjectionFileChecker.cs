using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public interface IFileChecker
    {
        string GetFileType(string file);
        string GetFileType(IRasterDataProvider file);
    }

    public abstract class FileCheckerBase : IFileChecker
    {
        public string GetFileType(string file)
        {
            IRasterDataProvider rad = GetSrcRaster(file);
            try
            {
                return GetFileType(rad);
            }
            finally
            {
                if (rad != null)
                    rad.Dispose();
            }
        }

        public abstract string GetFileType(IRasterDataProvider file);

        private IRasterDataProvider GetSrcRaster(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }
    }

    public class FileChecker : FileCheckerBase
    {
        public FileChecker()
        { }

        public override string GetFileType(IRasterDataProvider file)
        {
            try
            {
                if (file == null)
                    throw new Exception("文件为空");
                DataIdentify dataIdentify = file.DataIdentify;
                if (dataIdentify.IsOrbit)
                {
                    if (dataIdentify.Sensor == "VIRR")       //dataIdentify.Satellite == "FY3B"||"FY3A"
                    {
                        if (dataIdentify.Satellite == "FY3C")
                            return "FY3C_VIRR_L1";
                        else
                            return "VIRR_L1";
                    }
                    else if (dataIdentify.Sensor == "MERSI")
                    {
                        if (dataIdentify.Satellite == "FY3C")
                        {
                            if (file.fileName.Contains("1000M"))
                                return "FY3C_MERSI_1KM_L1";
                            else if (file.fileName.Contains("0250M"))
                                return "FY3C_MERSI_QKM_L1";
                        }
                        else
                        {
                            if (file.fileName.Contains("1000M"))
                                return "MERSI_1KM_L1";
                            else if (file.fileName.Contains("0250M"))
                                return "MERSI_QKM_L1";
                        }
                    }
                    else if (modisSensor.Contains(dataIdentify.Sensor) || modisSatellite.Contains(dataIdentify.Satellite)) //dataIdentify.Satellite == "EOST"||"EOST"//TERRA
                    {
                        if (file.fileName.Contains("MOD021KM") || file.fileName.Contains("MYD021KM"))//MOD/MYD021KM)、MOD/MYD03GEO、
                            return "MODIS_1KM_L1";
                        else if (file.fileName.Contains("MOD02HKM") || file.fileName.Contains("MYD02HKM"))
                            return "MODIS_HKM_L1";
                        else if (file.fileName.Contains("MOD02QKM") || file.fileName.Contains("MYD02QKM"))
                            return "MODIS_QKM_L1";
                    }
                    else if ((dataIdentify.Satellite == "FY1D" || dataIdentify.Satellite == "FY1C") && dataIdentify.Sensor == "AVHRR")
                        return "FY1X_1A5";
                    else if (dataIdentify.Sensor == "AVHRR") //dataIdentify.Satellite == "NOAA-18" && 
                        return "NOAA_1BD";
                    else if (dataIdentify.Satellite == "NOAA" && dataIdentify.Sensor == "AVHRR")
                        return "NOAA_1A5_L1";
                    else if (dataIdentify.Sensor == "VISSR" || fy2Satellite.Contains(dataIdentify.Satellite))
                        return "FY2NOM";
                }
                else if (file.SpatialRef != null)
                    return "PROJECTED";
            }
            catch (Exception ex)
            {
                throw new Exception("未获得是轨道数据或者是已投影数据标识，无法确定正确的数据来源", ex.InnerException);
            }
            throw new Exception("未获得是轨道数据或者是已投影数据标识，无法确定正确的数据来源");
        }

        private static string[] modisSensor = new string[] { "MODIS", "MOD" };
        private static string[] modisSatellite = new string[] { "TERRA", "AQUA", "EOST", "EOSA" };
        private static string[] fy2Satellite = new string[] { "FY2C", "FY2D", "FY2E", "FY2F" };
    }
}
