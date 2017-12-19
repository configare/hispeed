using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Diagnostics;
using GeoDo.FileProject;
using System.IO;
using System.Windows.Forms;

namespace GeoDo.FileProject
{
    public class ProjectionQuicklySelectBands
    {
        private Action<int, string> _progress = null;
        private FileChecker _fileChecker = null;

        public ProjectionQuicklySelectBands(Action<int, string> progress)
        {
            _fileChecker = new FileChecker();
            _progress = progress;
        }

        public string Project(string file, ISpatialReference projRef)
        {
            IRasterDataProvider fileName = null;
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                fileName = GetSrcRaster(file);
                string fileType = _fileChecker.GetFileType(fileName);
                if (string.IsNullOrWhiteSpace(fileType))
                    throw new Exception("暂未支持该类数据的投影");
                if (_progress != null)
                    _progress(1, "启动投影");
                string outFile = "";
                switch (fileType)
                {
                    case "VIRR_L1":
                        outFile = PrjVIRR(fileName, projRef);
                        break;
                    case "MERSI_1KM_L1":
                        outFile = PrjMERSI_1KM_L1(fileName, projRef);
                        break;
                    case "MERSI_QKM_L1":
                        outFile = PrjMERSI_QKM_L1(fileName, projRef);
                        break;
                    case "MODIS_1KM_L1":
                        outFile = PrjMODIS_1KM_L1(fileName, projRef);
                        break;
                    case "NOAA_1BD":
                        outFile = PrjNOAA_1BD_L1(fileName, projRef);
                        break;
                    default:
                        break;
                }
                stopwatch.Stop();
                Console.WriteLine("投影耗时" + stopwatch.ElapsedMilliseconds.ToString() + "ms");
                if (_progress != null)
                    _progress(100, "投影完成,正在打开数据" + stopwatch.ElapsedMilliseconds.ToString());
                return outFile;
            }
            finally
            {
                if (fileName != null)
                {
                    fileName.Dispose();
                    fileName = null;
                }
            }
        }
        
        //453
        private string PrjNOAA_1BD_L1(IRasterDataProvider fileName, ISpatialReference projRef)
        {
            IRasterDataProvider srcRaster = fileName;
            try
            {
                IFileProjector projTor = FileProjector.GetFileProjectByName("NOAA_1BD");
                NOAA_PrjSettings prjSetting = new NOAA_PrjSettings();
                prjSetting.OutPathAndFileName = GetOutPutFile(fileName.fileName, projRef);
                prjSetting.OutBandNos = new int[] { 3, 4, 5 };
                ISpatialReference dstSpatialRef = projRef;
                projTor.Project(srcRaster, prjSetting, dstSpatialRef, _progress);
                return prjSetting.OutPathAndFileName;
            }
            finally
            {
            }
        }

        private string PrjMODIS_1KM_L1(IRasterDataProvider fileName, ISpatialReference projRef)
        {//
            IRasterDataProvider srcRaster = fileName;
            IRasterDataProvider locationRaster = null;
            try
            {
                List<BandMap> bandmapList = new List<BandMap>();
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr1km_RefSB", File = srcRaster, BandIndex = 0 });//1
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr1km_RefSB", File = srcRaster, BandIndex = 1 });//2
                bandmapList.Add(new BandMap() { DatasetName = "EV_500_Aggr1km_RefSB", File = srcRaster, BandIndex = 0 });//3

                locationRaster = TryFindMODIS_1KM_L103File(fileName);
                IFileProjector projTor = FileProjector.GetFileProjectByName("EOS");
                EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                prjSetting.BandMapTable = bandmapList;
                prjSetting.LocationFile = locationRaster;
                prjSetting.OutPathAndFileName = GetOutPutFile(fileName.fileName, projRef);
                ISpatialReference dstSpatialRef = projRef;
                projTor.Project(srcRaster, prjSetting, dstSpatialRef, _progress);
                return prjSetting.OutPathAndFileName;
            }
            finally
            {
                if (locationRaster != null)
                    locationRaster.Dispose();
            }
        }

        private string PrjMERSI_QKM_L1(IRasterDataProvider fileName, ISpatialReference projRef)
        {
            IRasterDataProvider srcRaster = fileName;
            IRasterDataProvider locationRaster = null;
            try
            {
                List<BandMap> bandmapList = new List<BandMap>();
                //bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB_b1", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB_b2", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB_b3", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_RefSB_b4", File = srcRaster, BandIndex = 0 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_250_Emissive", File = srcRaster, BandIndex = 0 });

                locationRaster = TryFindMERSI_1KM_L1FromQKM(fileName);
                IFileProjector projTor = FileProjector.GetFileProjectByName("FY3_MERSI");
                FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                prjSetting.BandMapTable = bandmapList;
                prjSetting.SecondaryOrbitRaster = locationRaster;
                prjSetting.OutPathAndFileName = GetOutPutFile(fileName.fileName, projRef);
                ISpatialReference dstSpatialRef = projRef;
                projTor.Project(srcRaster, prjSetting, dstSpatialRef, _progress);
                return prjSetting.OutPathAndFileName;
            }
            finally
            {
                if (locationRaster != null)
                    locationRaster.Dispose();
            }
        }

        //3、4、2
        private string PrjMERSI_1KM_L1(IRasterDataProvider srcRaster, ISpatialReference projRef)
        {
            try
            {
                List<BandMap> bandmapList = new List<BandMap>();
                //bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = srcRaster, BandIndex = 0 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = srcRaster, BandIndex = 1 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = srcRaster, BandIndex = 2 });
                bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = srcRaster, BandIndex = 3 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_Emissive", File = srcRaster, BandIndex = 0 });//5
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 0 });//6
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 1 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 2 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 3 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 4 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 5 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 6 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 7 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 8 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 9 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 10 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 11 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 12 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 13 });
                //bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = srcRaster, BandIndex = 14 });
                
                IFileProjector projTor = FileProjector.GetFileProjectByName("FY3_MERSI");
                FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                prjSetting.OutPathAndFileName = GetOutPutFile(srcRaster.fileName, projRef);
                prjSetting.BandMapTable = bandmapList;
                ISpatialReference dstSpatialRef = projRef;
                projTor.Project(srcRaster, prjSetting, dstSpatialRef, _progress);
                return prjSetting.OutPathAndFileName;
            }
            finally
            { 
            }
        }

        //621
        private string PrjVIRR(IRasterDataProvider srcRaster, ISpatialReference projRef)
        {
            List<BandMap> bandmapList = new List<BandMap>();
            bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 0 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 1 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 0 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 1 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_Emissive", File = srcRaster, BandIndex = 2 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 2 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 3 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 4 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 5 });
            //bandmapList.Add(new BandMap() { DatasetName = "EV_RefSB", File = srcRaster, BandIndex = 6 });            

            IFileProjector projTor = FileProjector.GetFileProjectByName("FY3_VIRR");
            FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
            prjSetting.OutPathAndFileName = GetOutPutFile(srcRaster.fileName, projRef);
            prjSetting.BandMapTable = bandmapList;

            ISpatialReference dstSpatialRef = projRef;
            projTor.Project(srcRaster, prjSetting, dstSpatialRef, _progress);
            return prjSetting.OutPathAndFileName;
        }
        
        private string GetOutPutFile(string file, ISpatialReference projRef)
        {
            if (projRef == null || string.IsNullOrWhiteSpace(projRef.Name))
                return Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_PRJ.ldf");
            else
            {
                string sName = GetShortName(projRef.Name);
                return Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_" + sName + ".ldf");
            }
        }

        private string GetShortName(string name)
        {
            switch (name)
            {
                case "GCS_WGS_1984":
                    return "GLL";
                case "Polar Stereographic":
                    return "PSG";
                case "Albers Conical Equal Area":
                    return "AEA";
                case "Lambert Conformal Conic":
                    return "LBT";
                case "Mercator":
                    return "MCT";
                case "Hammer":
                    return "HAM";
                default:
                    return "PRJ";
            }
        }

        private IRasterDataProvider TryFindMODIS_1KM_L103File(IRasterDataProvider fileName)
        {
            try
            {
                string kmFileName = fileName.fileName;
                string file = kmFileName.Replace("MOD021KM", "MOD03");//MOD02HKM,MOD02QKM
                if (file == kmFileName)
                    throw new Exception("无法找到经纬度文件");
                return GetSrcRaster(file);
            }
            catch (Exception ex)
            {
                throw new Exception("无法找到经纬度文件", ex);
            }
        }

        private IRasterDataProvider TryFindMERSI_1KM_L1FromQKM(IRasterDataProvider fileName)
        {
            try
            {
                string kmFileName = fileName.fileName;
                string file = kmFileName.Replace("_0250M_", "_1000M_");
                if (file == kmFileName)
                    throw new Exception("无法找到对应1KM文件(获取太阳天顶角数据使用)");
                return GetSrcRaster(file);
            }
            catch (Exception ex)
            {
                throw new Exception("无法找到对应1KM文件(获取太阳天顶角数据使用)", ex);
            }
        }

        private IRasterDataProvider GetSrcRaster(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }
    }
}
