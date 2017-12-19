#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-6-23 17:33:15
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using GeoDo.HDF5;
using GeoDo.HDF;
using GeoDo.HDF4;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RasterProject;
using GeoDo.Project;
using GeoDo.FileProject;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.DF.GDAL.HDF4Universal;
using System.Text.RegularExpressions;
using System.ComponentModel;
using GeoDo.Smart.Tools.MODLSTPro;
using HDF5DotNet;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    /// <summary>
    /// 类名：UniversalDataProcess
    /// 属性描述：数据预处理通用方法:MODIS 正弦投影数据到等经纬度转换，拼接。 FY全球云产品区域提取
    /// 创建者：lxj   创建日期：2014-6-23 17:33:15
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class UniversalDataProcess
    {
        public static bool PrjTransf(string srcFile, PrjEnvelope srcEnvelope, Dictionary<string, H5T.H5Type> dataSets, string dstFile, PrjEnvelope dstEnvelope, float dstResoultion)
        {
            Action<int, string> progressTracker = null;
            string[] args = new string[3];
            string datasetstr = string.Empty;
            foreach (string itemdest in dataSets.Keys)
            {
                datasetstr += itemdest + ",";
            }
            datasetstr = datasetstr.Remove(datasetstr.LastIndexOf(','));
            args[0] = string.Format("datasets ={0}", datasetstr);
            args[1] = string.Format("geoinfo={0},{1},{2},{3},1000.000,1000.000", srcEnvelope.MinX, srcEnvelope.MaxX, srcEnvelope.MinY, srcEnvelope.MaxY);
            args[2] = "proj4 = +proj=sinu +lon_0=0 +x_0=0 +y_0=0 +a=6371007.181 +b=6371007.181 +units=m +no_defs";
            ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
            FilePrjSettings prjSetting = new FilePrjSettings();

            IRasterDataProvider prd = GeoDataDriver.Open(srcFile, args) as IRasterDataProvider;
            if (prd != null)
            {
                prjSetting.OutEnvelope = dstEnvelope;
                prjSetting.OutResolutionX = dstResoultion;
                prjSetting.OutResolutionY = dstResoultion;
                prjSetting.OutPathAndFileName = dstFile;
                ProjectedFileTransform proj = new ProjectedFileTransform();
                proj.ProjectNew(prd, prjSetting, dstSpatialRef, progressTracker, dataSets);
                prd.Dispose();
                return true;
            }
            return false;
        }

        public string[] CheckFiles(string inputdir)
        {
            string[] rawm06Files = Directory.GetFiles(inputdir, "MOD06_L2*.HDF", SearchOption.AllDirectories);
            List<string> newm06FilesList = new List<string>();
            string newName;
            foreach (string originfile in rawm06Files)
            {
                if (Path.GetFileNameWithoutExtension(originfile).Split('.').Length == 3)
                {
                    if (!newm06FilesList.Contains(originfile))
                        newm06FilesList.Add(originfile);
                    continue;
                }
                newName = RegularFileNames(originfile);
                if (!newm06FilesList.Contains(newName))
                    newm06FilesList.Add(newName);
            }
            string[] m06Files = newm06FilesList.ToArray();
            List<string> mod06f = m06Files.ToList();
            for (int i = 0; i < m06Files.Length; i++)
            {
                if (!GeoDo.HDF4.HDF4Helper.IsHdf4(m06Files[i]))
                {
                    mod06f.Remove(m06Files[i]);
                }
            }

            return mod06f.ToArray();
        }
        private string RegularFileNames(string file)
        {
            string nfname = Path.GetFileNameWithoutExtension(file);
            string[] parts = nfname.Split('.');
            string nameformat = "{0}.{1}.{2}.hdf";
            string newfname = string.Format(nameformat, parts[0], parts[1], parts[2]);
            String newName = Path.Combine(Path.GetDirectoryName(file), newfname);
            if (File.Exists(newName))
                File.Delete(newName);
            System.IO.File.Move(file, newName);
            return newName;
        }
        /// <summary>
        /// mod06 云产品5公里数据投影
        /// </summary>
        /// <param name="file"></param>
        /// <param name="outdir"></param>
        /// <param name="dataset"></param>
        /// <param name="env"></param>
        /// <param name="res"></param>
        /// <param name="regionNam"></param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        public string Projectmod06(string file, string outdir, string dataset, PrjEnvelope env, float res, string regionNam)
        {
            Action<int, string> progressTracker = null;
            IRasterDataProvider mainRaster = null;
            IRasterDataProvider locationRaster = null;
            string outfile = outdir + "\\" + Path.GetFileNameWithoutExtension(file) + "." + regionNam + "." + dataset + ".ldf";
            try
            {
                string[] openArgs = new string[] { "datasets=" + dataset };
                mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
                string[] locationArgs = new string[] { "datasets=" + "Latitude,Longitude", "geodatasets=" + "Latitude,Longitude" };
                locationRaster = RasterDataDriver.Open(file, locationArgs) as IRasterDataProvider;
                if (locationRaster == null || locationRaster.BandCount == 0)
                {
                    return null;
                }
                HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                setting.LocationFile = locationRaster;
                setting.OutFormat = "LDF";
                setting.OutResolutionX = setting.OutResolutionY = res;
                Dictionary<string, double> exargs = new Dictionary<string, double>();
                if (dataset.Contains("Cloud_Top_Temperature"))
                    exargs.Add("FillValue", -32768);
                if (dataset.Contains("Cloud_Fraction"))
                    exargs.Add("FillValue", 127);
                setting.ExtArgs = new object[] { exargs };
                setting.OutPathAndFileName = outfile;
                HDF4FileProjector projector = new HDF4FileProjector();
                GeoDo.RasterProject.PrjEnvelope dstmainPrj = env;
                if (dstmainPrj != null)
                {
                    setting.OutEnvelope = dstmainPrj;
                    projector.Project(mainRaster, setting, SpatialReference.GetDefault(), null);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (mainRaster != null)
                {
                    mainRaster.Dispose();
                    mainRaster = null;
                }
                if (locationRaster != null)
                {
                    locationRaster.Dispose();
                    locationRaster = null;
                }
            }
            return outfile;
        }

        //创建输出删格
        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, float resolution, string driverName, int bandNos, enumDataType dataType)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName(driverName) as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandNos, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
