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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RasterProject;
using GeoDo.Project;
using GeoDo.FileProject;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.DF.GDAL.HDF4Universal;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.MIF.Prds.PrdVal
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
        public UniversalDataProcess()
        { 
        }

        public string ExtractFile(string filename,string dataset, PrjEnvelope env ,string regionNam, string outdir)
        {
            string projectionType = null;
            string dstFileName = null;
            string[] dsNames = null;
            if (GeoDo.HDF5.HDF5Helper.IsHdf5(filename))
            {
                Dictionary<string, string> _fileAttributes = new Dictionary<string, string>();
                using (Hdf5Operator oper = new Hdf5Operator(filename))
                {
                    dsNames = oper.GetDatasetNames;
                    int id = 0;
                    foreach (string ds in dsNames)
                    {
                        if (ds == dataset)
                            id = 1;
                    }
                    if (id == 0)
                        return null;
                    _fileAttributes = oper.GetAttributes();
                }
                double minX = 0d, maxX = 0d, minY = 0d, maxY = 0d, resolutionX = 0d, resolutionY = 0d;
                if (_fileAttributes.ContainsKey("Left-Bottom Latitude"))
                {
                    string minLat = _fileAttributes["Left-Bottom Latitude"];
                    if (double.TryParse(minLat, out minY))
                    { }
                }
                if (_fileAttributes.ContainsKey("Left-Bottom Longitude"))
                {
                    string minlon = _fileAttributes["Left-Bottom Longitude"];
                    if (double.TryParse(minlon, out minX))
                    { }
                }
                if (_fileAttributes.ContainsKey("Left-Top Latitude"))
                {
                    string maxLat = _fileAttributes["Left-Top Latitude"];
                    if (double.TryParse(maxLat, out maxY))
                    { }
                }
                if (_fileAttributes.ContainsKey("Left-Top Longitude"))
                {
                    string maxLon = _fileAttributes["Right-Top Longitude"];
                    if (double.TryParse(maxLon, out maxX))
                    { }
                }
                if (_fileAttributes.ContainsKey("Latitude Resolution"))
                {
                    string latRegolution = _fileAttributes["Latitude Resolution"];
                    if (double.TryParse(latRegolution, out resolutionY))
                    { }
                }
                if (_fileAttributes.ContainsKey("Longitude Resolution"))
                {
                    string lonRegolution = _fileAttributes["Longitude Resolution"];
                    if (double.TryParse(lonRegolution, out resolutionX))
                    { }
                }
                if (_fileAttributes.ContainsKey("Projection Type"))
                {
                    projectionType = _fileAttributes["Projection Type"];
                }
                string[] args = new string[1];
                args[0] = "datasets = " + dataset;
                IRasterDataProvider prd = GeoDataDriver.Open(filename, args) as IRasterDataProvider;
                double dminx = env.MinX, dmax = env.MaxX, dminy = env.MinY, dmaxy = env.MaxY;
                int xoffset = (int)(Math.Round(dminx - minX) / resolutionX);
                int yoffset = (int)(Math.Round(maxY - dmaxy) / resolutionY);
                int width = (int)(Math.Round((dmax - dminx) / resolutionX));
                int height = (int)(Math.Round((dmaxy - dminy) / resolutionY));
                float tResolutionX;
                float tResolutionY;
                tResolutionX = Convert.ToSingle(resolutionX);
                tResolutionY = Convert.ToSingle(resolutionY);
                string[] optionString = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF="+ projectionType,
                "MAPINFO={" + 1 + "," + 1 + "}:{" + dminx + "," + dmaxy+ "}:{" + tResolutionX + "," + tResolutionY + "}"
                };
                enumDataType dataType = enumDataType.Int16;
                if (!Directory.Exists(outdir))
                    Directory.CreateDirectory(outdir);
                dstFileName = outdir+ "\\"+ Path.GetFileNameWithoutExtension(filename).Replace("GBAL", regionNam) + ".ldf";
                if (File.Exists(dstFileName))
                {
                    System.Windows.Forms.MessageBox.Show(dstFileName + "已存在");
                    return null;
                }
                IRasterDataDriver dataDriver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                IRasterDataProvider dstRaster = dataDriver.Create(dstFileName, width, height, 1, dataType, optionString);
                Int16[] buffer = new Int16[width * height];
                unsafe
                {
                    fixed (Int16* ptr = buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        prd.GetRasterBand(1).Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Int16, width, height);
                        fixed (Int16* wptr = buffer)
                        {
                            IntPtr newBuffer = new IntPtr(wptr);
                            dstRaster.GetRasterBand(1).Write(0, 0, width, height, newBuffer, enumDataType.Int16, width, height);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通用MODIS 正弦数据产品投影转换
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="env">范围</param>
        /// <param name="res">分辨率</param>
        /// <returns></returns>
        public string PrjMODSIN(string file, string outdir, PrjEnvelope env, float res, string regionNam, Action<int, string> progressTracker)
        {
            //根据文件名解析tile号nh,nv
            UInt16 nh = 0; UInt16 nv = 0;
            string fnam = Path.GetFileNameWithoutExtension(file).ToLower();
            string[] parts = fnam.Split(new char[] { '.' });
            foreach (string part in parts)
            {
                if (part.Contains("h") & part.Contains("v"))
                {
                    nh = Convert.ToUInt16(part.Substring(1, 2));
                    nv = Convert.ToUInt16(part.Substring(4, 2));
                }
            }
            //计算图像四角坐标;
            double minX = -20015109.354 + nh * 1111950.5196666666;
            double maxX = -20015109.354 + (nh + 1) * 1111950.5196666666;
            double maxY = -10007554.677 + (18 - nv) * 1111950.5196666666;
            double minY = -10007554.677 + (18 - nv - 1) * 1111950.5196666666;
            string[] args = new string[3];
            args[0] = "datasets = 0";
            args[1] = "geoinfo=" + Convert.ToString(minX) + "," + Convert.ToString(maxX) + "," + Convert.ToString(minY) + "," + Convert.ToString(maxY) + ",1000.000,1000.000";
            args[2] = "proj4 = +proj=sinu +lon_0=0 +x_0=0 +y_0=0 +ellps=WGS84 +datum=WGS84 +units=m +no_defs";
            ISpatialReference dstSpatialRef = SpatialReference.GetDefault();
            FilePrjSettings prjSetting = new FilePrjSettings();
            using (IRasterDataProvider prd = GeoDataDriver.Open(file, args) as IRasterDataProvider)
            {
                prjSetting.OutEnvelope = env;
                prjSetting.OutResolutionX = res;
                prjSetting.OutResolutionY = res;
                prjSetting.OutFormat = "LDF";
                prjSetting.OutPathAndFileName = outdir + "\\" + Path.GetFileNameWithoutExtension(file) + "_" + regionNam + "_LST.ldf";
                ProjectedFileTransform proj = new ProjectedFileTransform();
                proj.Project(prd, prjSetting, dstSpatialRef, progressTracker);
            }
            return prjSetting.OutPathAndFileName;
        }

        public string MODJoin(string prjfile, string outdir, float res, string regionNam, string outfile)
        {
            int bandNo = 1;
            float resolution = res;
            List<RasterMaper> rms = new List<RasterMaper>();
            IRasterDataProvider outRaster = null;
            try
            {
                IRasterDataProvider inRaster = GeoDataDriver.Open(prjfile) as IRasterDataProvider;
                RasterMaper rm = new RasterMaper(inRaster, new int[] { bandNo });
                rms.Add(rm);
                if (File.Exists(outfile))
                {
                    outRaster = GeoDataDriver.Open(outfile, enumDataProviderAccess.Update, null) as IRasterDataProvider;
                }
                else
                {
                    outRaster = CreateOutRaster(outfile, rms.ToArray(), resolution);
                }
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                //创建处理模型
                RasterProcessModel<UInt16, UInt16> rfr = null;
                rfr = new RasterProcessModel<UInt16, UInt16>();
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                    int n = rvInVistor.Count(); //输入文件个数
                    UInt16[,] save = new UInt16[n + 1, dataLength];// 第一维是文件的顺序号，第二维是文件的数值
                    UInt16[,] flag = new UInt16[n + 1, dataLength];
                    int j = 0; //记录输入文件的序号，从0开始
                    foreach (RasterVirtualVistor<UInt16> rv in rvInVistor)
                    {
                        if (rv.RasterBandsData == null)
                            continue;
                        for (int index = 0; index < dataLength; index++)
                        {
                            save[j, index] = rv.RasterBandsData[0][index];
                            //掩膜 0，1
                            if (save[j, index] < 0 || save[j, index] > 0)
                                flag[j, index] = 1;
                            else
                                flag[j, index] = 0;
                        }
                        j++;
                    }
                    for (int index = 0; index < dataLength; index++)//输出文件也参与拼接计算
                    {
                        save[n, index] = rvOutVistor[0].RasterBandsData[0][index];
                        //掩膜 0，1
                        if (rvOutVistor[0].RasterBandsData[0][index] < 0 || rvOutVistor[0].RasterBandsData[0][index] > 0)
                            flag[n, index] = 1;
                        else
                            flag[n, index] = 0;
                    }
                    UInt16[] sumData = new UInt16[dataLength];
                    UInt16[] sumDataFlag = new UInt16[dataLength];
                    for (int m = 0; m < n + 1; m++)
                    {
                        for (int q = 0; q < dataLength; q++)
                        {
                            sumData[q] += save[m, q];
                            sumDataFlag[q] += flag[m, q];
                        }
                    }
                    for (int index = 0; index < dataLength; index++)
                    {
                        if (sumDataFlag[index] == 0)
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = 0;
                        }
                        else
                        {
                            rvOutVistor[0].RasterBandsData[0][index] = Convert.ToUInt16(sumData[index] / sumDataFlag[index] * 0.02);
                        }
                    }
                }));
                rfr.Excute();
            }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if (outRaster != null)
                    outRaster.Dispose();
            }
            return outfile;
        }
        //创建输出删格
        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.UInt16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

    }
}
