using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class ProjectionFactory
    {
        private FileChecker _fileChecker = null;
        private GenericFilename _genericFilename;

        public ProjectionFactory()
        {
            _fileChecker = new FileChecker();
            _genericFilename = new GenericFilename();
        }

        public string[] Project(string file, PrjOutArg prjOutArg, Action<int, string> progress, out string messageBox)
        {
            using (IRasterDataProvider srcRaster = FileFinder.Open(file))
            {
                if (srcRaster == null)
                    throw new Exception("读取待投影数据失败" + file);
                return Project(srcRaster, prjOutArg, progress, out messageBox);
            }
        }

        public string[] Project(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out string messageBox)
        {
            messageBox = null;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string fileType = _fileChecker.GetFileType(srcRaster);
            if (string.IsNullOrWhiteSpace(fileType))
                throw new Exception("暂未支持该类数据的投影");
            if (prjOutArg == null)
                throw new Exception("投影参数为空");
            if (progress != null)
                progress(1, "启动投影");
            string[] outFiles = null;
            StringBuilder errorMessage = null;
            if (prjOutArg.Envelopes == null || prjOutArg.Envelopes.Length == 0)
                prjOutArg.Envelopes = new PrjEnvelopeItem[] { new PrjEnvelopeItem("WHOLE", null) };
            if (string.IsNullOrWhiteSpace(prjOutArg.OutDirOrFile))
                prjOutArg.OutDirOrFile = Path.GetDirectoryName(srcRaster.fileName);
            switch (fileType)
            {
                case "VIRR_L1":
                    outFiles = PrjVIRR(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "FY3C_VIRR_L1":
                    outFiles = PrjFY3C_VIRR(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "MERSI_1KM_L1":
                    outFiles = PrjMERSI_1KM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "MERSI_QKM_L1":
                case "MERSI_250M_L1":
                    outFiles = PrjMERSI_QKM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "FY3C_MERSI_1KM_L1":
                    outFiles = PrjFY3C_MERSI_1KM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "FY3C_MERSI_QKM_L1":
                    outFiles = PrjFY3C_MERSI_QKM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "MODIS_1KM_L1":
                    outFiles = PrjMODIS_1KM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "MODIS_HKM_L1":
                    outFiles = PrjMODIS_HKM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "MODIS_QKM_L1":
                    outFiles = PrjMODIS_QKM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "NOAA_1BD":
                    outFiles = PrjNOAA_1BD_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "FY1X_1A5":
                    outFiles = PrjFY1X_1A5_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "FY2NOM":
                    outFiles = PrjFY2NOM_L1(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                case "PROJECTED":
                    outFiles = PrjProjected(srcRaster, prjOutArg, progress, out errorMessage);
                    break;
                default:
                    break;
            }
            stopwatch.Stop();
            Console.WriteLine("投影耗时" + stopwatch.ElapsedMilliseconds.ToString() + "ms");
            if (errorMessage != null)
                messageBox = errorMessage.ToString();
            return outFiles;
        }

        private string[] PrjFY3C_VIRR(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            errorMessage = new StringBuilder();
            IRasterDataProvider geoRaster = FileFinder.TryFindGeoFileFromFY3C_VIRR(srcRaster);
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IFileProjector projector = null;
            int[] kmBands;
            FileFinder.GetVIRRBandmapTable(prjOutArg.SelectedBands, out kmBands);
            try
            {
                List<string> outFiles = new List<string>();
                projector = FileProjector.GetFileProjectByName("FY3C_VIRR");
                projector.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    try
                    {
                        if (prjOutArg.ResolutionX == 0)
                        {
                            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                            {
                                prjOutArg.ResolutionX = 0.01f;
                                prjOutArg.ResolutionY = 0.01f;
                            }
                            else
                            {
                                prjOutArg.ResolutionX = 1000f;
                                prjOutArg.ResolutionY = 1000f;
                            }
                        }
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
                        prjSetting.GeoFile = geoRaster;
                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        if (prjOutArg.Args != null)
                        {
                            if (prjOutArg.Args.Contains("NotRadiation"))
                                prjSetting.IsRadiation = false;
                            if (prjOutArg.Args.Contains("NotSolarZenith"))
                                prjSetting.IsSolarZenith = false;
                            if (prjOutArg.Args.Contains("IsSensorZenith"))
                                prjSetting.IsSensorZenith = true;
                            if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                prjSetting.IsClearPrjCache = true;
                            prjSetting.ExtArgs = prjOutArg.Args;
                        }
                        projector.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (geoRaster != null)
                    geoRaster.Dispose();
                if (projector != null)
                    projector.EndSession();
            }
        }

        private string[] PrjFY2NOM_L1(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IFileProjector projector = null;
            int[] kmBands;
            FileFinder.GetVISSRBandmapTable(prjOutArg.SelectedBands, out kmBands);
            errorMessage = new StringBuilder();
            try
            {
                List<string> outFiles = new List<string>();
                projector = FileProjector.GetFileProjectByName("FY2NOM");
                projector.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    if (IsDir(prjOutArg.OutDirOrFile))
                        outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                    else
                        outFileName = prjOutArg.OutDirOrFile;

                    Fy2_NOM_PrjSettings prjSetting = new Fy2_NOM_PrjSettings();
                    prjSetting.OutPathAndFileName = outFileName;
                    try
                    {
                        prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        if (prjOutArg.Args != null)
                        {
                            if (prjOutArg.Args.Contains("NotRadiation"))
                                prjSetting.IsRadiation = false;
                            if (prjOutArg.Args.Contains("NotSolarZenith"))
                                prjSetting.IsSolarZenith = false;
                            if (prjOutArg.Args.Contains("IsSensorZenith"))
                                prjSetting.IsSensorZenith = true;
                            if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                prjSetting.IsClearPrjCache = true;
                            prjSetting.ExtArgs = prjOutArg.Args;
                        }
                        projector.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        TryDeleteErrorFile(outFileName);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projector != null)
                    projector.EndSession();
            }
        }

        private void TryDeleteErrorFile(string filename)
        {
            try
            {
                if (File.Exists(filename))
                    File.Delete(filename);
                string hdr = Path.ChangeExtension(filename, ".hdr");
                if (File.Exists(hdr))
                    File.Delete(hdr);
            }
            catch
            {
            }
        }

        public static PrjEnvelope GetEnvelope(IRasterDataProvider srcRaster)
        {
            string fileType = new FileChecker().GetFileType(srcRaster);
            if (string.IsNullOrWhiteSpace(fileType))
                throw new Exception("暂未支持该类数据的投影");
            PrjEnvelope env = null;
            IFileProjector projector;
            switch (fileType)
            {
                case "VIRR_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_VIRR");
                    projector.ComputeDstEnvelope(srcRaster, SpatialReference.GetDefault(), out env, null);
                    break;
                case "MERSI_1KM_L1":
                case "MERSI_QKM_L1":
                case "MERSI_250M_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_MERSI");
                    projector.ComputeDstEnvelope(srcRaster, SpatialReference.GetDefault(), out env, null);
                    break;
                case "MODIS_1KM_L1":
                    projector = FileProjector.GetFileProjectByName("EOS");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFind03FileFromModisImgFile(srcRaster))
                    {
                        projector.ComputeDstEnvelope(locationRaster, SpatialReference.GetDefault(), out env, null);
                    }
                    break;
                case "NOAA_1BD":
                    projector = FileProjector.GetFileProjectByName("NOAA_1BD");
                    projector.ComputeDstEnvelope(srcRaster, SpatialReference.GetDefault(), out env, null);
                    break;
                case "FY2NOM":
                    projector = FileProjector.GetFileProjectByName("FY2NOM");
                    projector.ComputeDstEnvelope(srcRaster, SpatialReference.GetDefault(), out env, null);
                    break;
                case "PROJECTED":
                    CoordEnvelope coord = srcRaster.CoordEnvelope;
                    env = new PrjEnvelope(coord.MinX, coord.MaxX, coord.MinY, coord.MaxY);
                    break;
            }
            return env;
        }

        /// <summary>
        /// 数据在指定的经纬度内
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool HasInvildEnvelope(IRasterDataProvider raster, PrjEnvelope invalidEnv)
        {
            string fileType = new FileChecker().GetFileType(raster);
            if (string.IsNullOrWhiteSpace(fileType))
                throw new Exception("暂未支持该类数据的投影");
            PrjEnvelope env = null;
            IFileProjector projector;
            bool hasVaild = false;
            switch (fileType)
            {
                case "FY3C_VIRR_L1":
                    projector = FileProjector.GetFileProjectByName("FY3C_VIRR");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFindGeoFileFromFY3C_VIRR(raster))
                    {
                        hasVaild = projector.HasVaildEnvelope(locationRaster, invalidEnv, SpatialReference.GetDefault());
                    }
                    break;
                case "FY3C_MERSI_1KM_L1":
                case "FY3C_MERSI_QKM_L1":
                    projector = FileProjector.GetFileProjectByName("FY3C_MERSI");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFindGeoFileFromFY3C_MERSI(raster))
                    {
                        hasVaild = projector.HasVaildEnvelope(locationRaster, invalidEnv, SpatialReference.GetDefault());
                    }
                    break;
                case "VIRR_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_VIRR");
                    hasVaild = projector.HasVaildEnvelope(raster, invalidEnv, SpatialReference.GetDefault());
                    break;
                case "MERSI_1KM_L1":
                case "MERSI_QKM_L1":
                case "MERSI_250M_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_MERSI");
                    hasVaild = projector.HasVaildEnvelope(raster, invalidEnv, SpatialReference.GetDefault());
                    break;
                case "MODIS_1KM_L1":
                case "MODIS_HKM_L1":
                case "MODIS_QKM_L1":
                    projector = FileProjector.GetFileProjectByName("EOS");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFind03FileFromModisImgFile(raster))
                    {
                        hasVaild = projector.HasVaildEnvelope(locationRaster, invalidEnv, SpatialReference.GetDefault());
                    }
                    break;
                case "NOAA_1BD":
                    projector = FileProjector.GetFileProjectByName("NOAA_1BD");
                    hasVaild = projector.HasVaildEnvelope(raster, invalidEnv, SpatialReference.GetDefault());
                    break;
                case "FY2NOM":
                    {
                        projector = FileProjector.GetFileProjectByName("FY2NOM");
                        hasVaild = projector.HasVaildEnvelope(raster, invalidEnv, SpatialReference.GetDefault());
                    }
                    break;
                case "PROJECTED":
                    CoordEnvelope coord = raster.CoordEnvelope;
                    env = new PrjEnvelope(coord.MinX, coord.MaxX, coord.MinY, coord.MaxY);
                    hasVaild = env.IntersectsWith(invalidEnv);
                    break;
                default:
                    throw new Exception("尚未支持的数据类型" + fileType);
            }
            return hasVaild;
        }

        /// <summary>
        /// 数据在指定的经纬度内，并且计算出有效率，以及实际输出范围
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="validEnv"></param>
        /// <param name="validRate"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool ValidEnvelope(IRasterDataProvider raster, PrjEnvelope validEnv, SpatialReference envSpatialReference, out double validRate, out PrjEnvelope outEnv)
        {
            string fileType = new FileChecker().GetFileType(raster);
            if (string.IsNullOrWhiteSpace(fileType))
                throw new Exception("暂未支持该类数据的投影");
            IFileProjector projector = null;
            bool hasVaild = false;
            switch (fileType)
            {
                case "FY3C_VIRR_L1":
                    projector = FileProjector.GetFileProjectByName("FY3C_VIRR");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFindGeoFileFromFY3C_VIRR(raster))
                    {
                        hasVaild = projector.ValidEnvelope(locationRaster, validEnv, envSpatialReference, out validRate, out outEnv);
                    }
                    break;
                case "FY3C_MERSI_1KM_L1":
                case "FY3C_MERSI_QKM_L1":
                    projector = FileProjector.GetFileProjectByName("FY3C_MERSI");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFindGeoFileFromFY3C_MERSI(raster))
                    {
                        hasVaild = projector.ValidEnvelope(locationRaster, validEnv, envSpatialReference, out validRate, out outEnv);
                    }
                    break;
                case "VIRR_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_VIRR");
                    hasVaild = projector.ValidEnvelope(raster, validEnv, envSpatialReference, out validRate, out outEnv);
                    break;
                case "MERSI_1KM_L1":
                case "MERSI_QKM_L1":
                case "MERSI_250M_L1":
                    projector = FileProjector.GetFileProjectByName("FY3_MERSI");
                    hasVaild = projector.ValidEnvelope(raster, validEnv, envSpatialReference, out validRate, out outEnv);
                    break;
                case "MODIS_1KM_L1":
                    projector = FileProjector.GetFileProjectByName("EOS");
                    using (IRasterDataProvider locationRaster = FileFinder.TryFind03FileFromModisImgFile(raster))
                    {
                        hasVaild = projector.ValidEnvelope(locationRaster, validEnv, envSpatialReference, out validRate, out outEnv);
                    }
                    break;
                case "NOAA_1BD":
                    projector = FileProjector.GetFileProjectByName("NOAA_1BD");
                    hasVaild = projector.ValidEnvelope(raster, validEnv, envSpatialReference, out validRate, out outEnv);
                    break;
                case "FY2NOM":
                    {
                        projector = FileProjector.GetFileProjectByName("FY2NOM");
                        hasVaild = projector.ValidEnvelope(raster, validEnv, envSpatialReference, out validRate, out outEnv);
                    }
                    break;
                case "FY1X_1A5":
                    projector = FileProjector.GetFileProjectByName("FY1X_1A5");
                    hasVaild = projector.ValidEnvelope(raster, validEnv, envSpatialReference, out validRate, out outEnv);
                    break;
                case "PROJECTED":
                    CoordEnvelope coord = raster.CoordEnvelope;
                    outEnv = new PrjEnvelope(coord.MinX, coord.MaxX, coord.MinY, coord.MaxY);
                    hasVaild = outEnv.IntersectsRate(validEnv, out validRate);
                    break;
                default:
                    throw new Exception("尚未支持的数据类型" + fileType);
            }
            validRate = 0;
            outEnv = null;
            return hasVaild;
        }

        private string[] PrjProjected(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            IFileProjector projTor = null;
            errorMessage = new StringBuilder();
            try
            {
                FileFinder.TryFindMODIS_HKM_L1From03(srcRaster);

                List<string> outFiles = new List<string>();
                projTor = FileProjector.GetFileProjectByName("ProjectedTransform");
                projTor.BeginSession(srcRaster);
                PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
                if (prjOutArg.ResolutionX == 0)
                {
                    if (IsGeoSpatial(prjOutArg.ProjectionRef))
                    {
                        if (IsGeoSpatial(srcRaster.SpatialRef))
                        {
                            prjOutArg.ResolutionX = srcRaster.ResolutionX;
                            prjOutArg.ResolutionY = srcRaster.ResolutionY;
                        }
                        else
                        {
                            prjOutArg.ResolutionX = srcRaster.ResolutionX / 100000f;
                            prjOutArg.ResolutionY = srcRaster.ResolutionY / 100000f;
                        }
                    }
                    else
                    {
                        if (IsGeoSpatial(srcRaster.SpatialRef))
                        {
                            prjOutArg.ResolutionX = srcRaster.ResolutionX * 100000f;
                            prjOutArg.ResolutionY = srcRaster.ResolutionY * 100000f;
                        }
                        else
                        {
                            prjOutArg.ResolutionX = srcRaster.ResolutionX;
                            prjOutArg.ResolutionY = srcRaster.ResolutionY;
                        }
                    }
                }
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    try
                    {
                        if (IsDir(prjOutArg.OutDirOrFile))
                            if (srcRaster.Driver.Name == "MEM")
                                outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, prjOutArg.ProjectionRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".dat");
                            else
                                outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, prjOutArg.ProjectionRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        FilePrjSettings prjSetting = new FilePrjSettings();
                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        prjSetting.OutEnvelope = prjEnvelopes[i].PrjEnvelope;
                        ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;

                        projTor.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projTor != null)
                    projTor.EndSession();
            }
        }

        private bool IsGeoSpatial(ISpatialReference dstSpatialRef)
        {
            return dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null;
        }

        private string[] PrjVIRR(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IRasterDataProvider srcRaster = fileName;
            IFileProjector projector = null;
            int[] kmBands;
            FileFinder.GetVIRRBandmapTable(prjOutArg.SelectedBands, out kmBands);
            errorMessage = new StringBuilder();
            try
            {
                List<string> outFiles = new List<string>();
                projector = FileProjector.GetFileProjectByName("FY3_VIRR");
                projector.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    try
                    {
                        if (prjOutArg.ResolutionX == 0)
                        {
                            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                            {
                                prjOutArg.ResolutionX = 0.01f;
                                prjOutArg.ResolutionY = 0.01f;
                            }
                            else
                            {
                                prjOutArg.ResolutionX = 1000f;
                                prjOutArg.ResolutionY = 1000f;
                            }
                        }
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        FY3_VIRR_PrjSettings prjSetting = new FY3_VIRR_PrjSettings();
                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        if (prjOutArg.Args != null)
                        {
                            if (prjOutArg.Args.Contains("NotRadiation"))
                                prjSetting.IsRadiation = false;
                            if (prjOutArg.Args.Contains("NotSolarZenith"))
                                prjSetting.IsSolarZenith = false;
                            if (prjOutArg.Args.Contains("IsSensorZenith"))
                                prjSetting.IsSensorZenith = true;
                            if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                prjSetting.IsClearPrjCache = true;
                            prjSetting.ExtArgs = prjOutArg.Args;
                        }
                        projector.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projector != null)
                    projector.EndSession();
            }
        }

        private bool IsDir(string path)
        {
            if (Directory.Exists(path))
                return true;
            else if (File.Exists(path))
                return false;
            else if (Path.HasExtension(path))
                return false;
            else
                return true;
        }

        private string[] PrjFY1X_1A5_L1(IRasterDataProvider raster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IRasterDataProvider srcRaster = raster;
            IFileProjector projTor = null;
            int[] kmBands;
            FileFinder.GetNoaaBandmapTable(prjOutArg.SelectedBands, out kmBands);
            errorMessage = new StringBuilder();
            try
            {
                List<string> outFiles = new List<string>();
                projTor = FileProjector.GetFileProjectByName("FY1X_1A5");
                projTor.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    try
                    {
                        if (prjOutArg.ResolutionX == 0)
                        {
                            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                            {
                                prjOutArg.ResolutionX = 0.01f;
                                prjOutArg.ResolutionY = 0.01f;
                            }
                            else
                            {
                                prjOutArg.ResolutionX = 1000f;
                                prjOutArg.ResolutionY = 1000f;
                            }
                        }
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        NOAA_PrjSettings prjSetting = new NOAA_PrjSettings();
                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutEnvelope = prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        if (prjOutArg.Args != null)
                        {
                            //if (prjOutArg.Args.Contains("NotRadiation"))//NotRadiation
                                prjSetting.IsRadiation = false;
                            //if (prjOutArg.Args.Contains("NotSolarZenith"))
                                prjSetting.IsSolarZenith = false;
                            if (prjOutArg.Args.Contains("IsSensorZenith"))
                                prjSetting.IsSensorZenith = true;
                            if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                prjSetting.IsClearPrjCache = true;
                            prjSetting.ExtArgs = prjOutArg.Args;
                        }
                        projTor.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projTor != null)
                    projTor.EndSession();
            }
        }

        private string[] PrjNOAA_1BD_L1(IRasterDataProvider raster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IRasterDataProvider srcRaster = raster;
            IFileProjector projTor = null;
            int[] kmBands;
            FileFinder.GetNoaaBandmapTable(prjOutArg.SelectedBands, out kmBands);
            errorMessage = new StringBuilder();
            try
            {
                List<string> outFiles = new List<string>();
                projTor = FileProjector.GetFileProjectByName("NOAA_1BD");
                projTor.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    string outFileName = null;
                    try
                    {
                        if (prjOutArg.ResolutionX == 0)
                        {
                            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                            {
                                prjOutArg.ResolutionX = 0.01f;
                                prjOutArg.ResolutionY = 0.01f;
                            }
                            else
                            {
                                prjOutArg.ResolutionX = 1000f;
                                prjOutArg.ResolutionY = 1000f;
                            }
                        }
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        NOAA_PrjSettings prjSetting = new NOAA_PrjSettings();
                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutEnvelope = prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        if (prjOutArg.Args != null)
                        {
                            if (prjOutArg.Args.Contains("NotRadiation"))//NotRadiation
                                prjSetting.IsRadiation = false;
                            if (prjOutArg.Args.Contains("NotSolarZenith"))
                                prjSetting.IsSolarZenith = false;
                            if (prjOutArg.Args.Contains("IsSensorZenith"))
                                prjSetting.IsSensorZenith = true;
                            if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                prjSetting.IsClearPrjCache = true;
                            prjSetting.ExtArgs = prjOutArg.Args;
                        }
                        projTor.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projTor != null)
                    projTor.EndSession();
            }
        }

        /// <summary>
        /// 规范Noaa轨道文件名
        /// p1bn07a5.n16.12.1bd
        /// 为
        /// NOAA16_AVHRR_L1_20121108_1058.1bd
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="outfilename"></param>
        private void TryUpdateNoaaOrbitFilename(IRasterDataProvider raster, ref string outfilename)
        {
            if (outfilename.IndexOf("AVHRR") != -1)
                return;
            string dir = Path.GetDirectoryName(outfilename);
            string filename = Path.GetFileNameWithoutExtension(outfilename);
            string fileext = Path.GetExtension(outfilename);
            DataIdentify identify = raster.DataIdentify;
            if (identify == null)
                return;
            filename = identify.Satellite.Replace("-", "") + "_"
                    + raster.DataIdentify.Sensor + "_"
                    + "L1_"
                    + raster.DataIdentify.OrbitDateTime.ToString("yyyyMMdd") + "_"
                    + raster.DataIdentify.OrbitDateTime.ToString("HHmm") + "_"
                    + "1000M"
                    + fileext;
            outfilename = Path.Combine(dir, filename);
        }

        private string[] PrjMODIS_QKM_L1(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            errorMessage = new StringBuilder();
            IRasterDataProvider mod03File = FileFinder.TryFind03FileFromModisImgFile(srcRaster);
            if (mod03File == null)
                return null;
            IRasterDataProvider qkmRaster = srcRaster;
            IRasterDataProvider hkmRaster = FileFinder.TryFindMODIS_HKM_L1From03(mod03File);
            IRasterDataProvider kmRaster = FileFinder.TryFindMODIS_1KM_L1From03(mod03File);
            return PrjMODIS(prjOutArg, errorMessage, mod03File, ref qkmRaster, ref hkmRaster, kmRaster, progress);
        }

        private string[] PrjMODIS(PrjOutArg prjOutArg, StringBuilder errorMessage, IRasterDataProvider locationRaster,
            ref IRasterDataProvider qkmRaster, ref IRasterDataProvider hkmRaster, IRasterDataProvider kmRaster, Action<int, string> progress)
        {
            //默认分辨率，默认参数
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            float outResolutionX = prjOutArg.ResolutionX;
            float outResolutionY = prjOutArg.ResolutionY;
            SetDefaultResolutionForModis(qkmRaster, hkmRaster, dstSpatialRef, ref outResolutionX, ref outResolutionY);
            prjOutArg.ResolutionX = outResolutionX;
            prjOutArg.ResolutionY = outResolutionY;
            FilterRasterForModis(ref qkmRaster, ref hkmRaster, kmRaster, dstSpatialRef, outResolutionX, outResolutionY);
            int[] qkmBands;
            int[] hkmBands;
            int[] kmBands;
            FileFinder.GetModisBandmapTable(kmRaster, hkmRaster, qkmRaster, prjOutArg.SelectedBands, out qkmBands, out hkmBands, out kmBands);
            int bandCount = (qkmBands == null ? 0 : qkmBands.Length) + (hkmBands == null ? 0 : hkmBands.Length) + (kmBands == null ? 0 : kmBands.Length);
            if (bandCount == 0)
            {
                errorMessage.Append("没有获取要投影的通道");
                return null;
            }
            string bandNames = "BANDNAMES="
                + (qkmBands == null || qkmBands.Length == 0 ? "" : BandNameString(qkmBands) + ",")
                + (hkmBands == null || hkmBands.Length == 0 ? "" : BandNameString(hkmBands) + ",")
                + BandNameString(kmBands);
            IFileProjector projtor = FileProjector.GetFileProjectByName("EOS");
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            try
            {
                List<string> outFiles = new List<string>();
                IRasterDataProvider outRaster = null;
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    PrjEnvelope envelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                    if (envelope == null)
                    {
                        projtor.ComputeDstEnvelope(kmRaster, dstSpatialRef, out envelope, null);
                        if (envelope == null)
                        {
                            errorMessage.Append("未能读取出文件的经纬度范围：" + kmRaster.fileName);
                            continue;
                        }
                    }
                    string outFileName = null;
                    try
                    {
                        IRasterDataProvider rad = qkmRaster != null ? qkmRaster : (kmRaster != null ? kmRaster : null);
                        string orbitFileName = rad == null ? null : rad.fileName;
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, rad.fileName, dstSpatialRef, prjEnvelopes[i].Name, rad.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;
                        outRaster = CreateRaster(outFileName, envelope, outResolutionX, outResolutionY, bandCount, dstSpatialRef, bandNames);
                        outFiles.Add(outFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        errorMessage.Append(ex.Message);
                        outFiles.Add(null);
                    }
                    finally
                    {
                        if (outRaster != null)
                        {
                            outRaster.Dispose();
                            outRaster = null;
                        }
                    }
                }
                bool hasAngle = false;
                int perBandBegin = 0;
                int curBandBegin = 0;
                if (qkmBands != null && qkmBands.Length != 0)
                {
                    curBandBegin = 0;
                    perBandBegin = qkmBands.Length;
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                            prjSetting.LocationFile = locationRaster;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutBandNos = qkmBands;
                            prjSetting.OutPathAndFileName = outFiles[i];
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(qkmRaster, prjSetting, outRaster, curBandBegin, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                    projtor.EndSession();
                }
                if (hkmBands != null && hkmBands.Length != 0)
                {
                    curBandBegin += perBandBegin;
                    perBandBegin += hkmBands.Length;
                    projtor.BeginSession(hkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                            prjSetting.LocationFile = locationRaster;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutBandNos = hkmBands;
                            prjSetting.OutPathAndFileName = outFiles[i];
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(hkmRaster, prjSetting, outRaster, curBandBegin, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                    projtor.EndSession();
                }
                if (kmBands != null && kmBands.Length != 0)
                {
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        bool isSuccess = false;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                            prjSetting.LocationFile = locationRaster;
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutBandNos = kmBands;
                            prjSetting.OutPathAndFileName = outFiles[i];
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(kmRaster, prjSetting, outRaster, perBandBegin, progress);
                            isSuccess = true;
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                            if (!isSuccess && File.Exists(outFiles[i]))
                            {
                                File.Delete(outFiles[i]);
                                outFiles[i] = null;
                            }
                        }
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projtor != null)
                    projtor.EndSession();
                if (kmRaster != null)
                    kmRaster.Dispose();
            }
        }

        private static void FilterRasterForModis(ref IRasterDataProvider qkmRaster, ref IRasterDataProvider hkmRaster, IRasterDataProvider kmRaster, ISpatialReference dstSpatialRef, float outResolutionX, float outResolutionY)
        {
            float baseResolutionK = 0.01f;
            float baseResolutionH = 0.005f;
            if (dstSpatialRef.ProjectionCoordSystem != null)
            {
                baseResolutionK = 1000f;
                baseResolutionH = 500f;
            }
            if (outResolutionX >= baseResolutionK && outResolutionY >= baseResolutionK)
            {
                if (kmRaster != null)
                {
                    if (qkmRaster != null)
                    {
                        qkmRaster.Dispose();
                        qkmRaster = null;
                    }
                    if (hkmRaster != null)
                    {
                        hkmRaster.Dispose();
                        hkmRaster = null;
                    }
                }
            }
            else if (outResolutionX >= baseResolutionH && outResolutionY >= baseResolutionH)
            {
                if (kmRaster != null || hkmRaster != null)
                {
                    if (qkmRaster != null)
                    {
                        qkmRaster.Dispose();
                        qkmRaster = null;
                    }
                }
            }
        }

        private static void SetDefaultResolutionForModis(IRasterDataProvider qkmRaster, IRasterDataProvider hkmRaster, ISpatialReference dstSpatialRef, ref float outResolutionX, ref float outResolutionY)
        {
            if (outResolutionX == 0 || outResolutionY == 0)
            {
                if (qkmRaster != null)
                {
                    if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                    {
                        outResolutionX = 0.0025f;
                        outResolutionY = 0.0025f;
                    }
                    else
                    {
                        outResolutionX = 250f;
                        outResolutionY = 250f;
                    }
                }
                else if (hkmRaster != null)
                {
                    if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                    {
                        outResolutionX = 0.005f;
                        outResolutionY = 0.005f;
                    }
                    else
                    {
                        outResolutionX = 500f;
                        outResolutionY = 500f;
                    }
                }
                else
                {
                    if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
                    {
                        outResolutionX = 0.01f;
                        outResolutionY = 0.01f;
                    }
                    else
                    {
                        outResolutionX = 1000f;//投影坐标系
                        outResolutionY = 1000f;
                    }
                }
            }
        }

        protected string BandNameString(int[] outBandNos)
        {
            if (outBandNos == null || outBandNos.Length == 0)
                return "";
            string bandNames = string.Empty;
            foreach (int bandNo in outBandNos)
                bandNames += ("band " + bandNo + ",");
            if (bandNames.EndsWith(","))
                bandNames = bandNames.Substring(0, bandNames.Length - 1);
            return bandNames;
        }

        private string[] PrjMODIS_HKM_L1(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            errorMessage = new StringBuilder();
            IRasterDataProvider mod03File = FileFinder.TryFind03FileFromModisImgFile(srcRaster);
            if (mod03File == null)
                return null;
            IRasterDataProvider qkmRaster = FileFinder.TryFindMODIS_QKM_L1From03(mod03File);
            IRasterDataProvider hkmRaster = srcRaster;
            IRasterDataProvider kmRaster = FileFinder.TryFindMODIS_1KM_L1From03(mod03File);
            return PrjMODIS(prjOutArg, errorMessage, mod03File, ref qkmRaster, ref hkmRaster, kmRaster, progress);
        }

        private string[] PrjMODIS_1KM_L1(IRasterDataProvider srcRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            errorMessage = new StringBuilder();
            IRasterDataProvider mod03File = FileFinder.TryFind03FileFromModisImgFile(srcRaster);
            if (mod03File == null)
                return null;
            IRasterDataProvider qkmRaster = FileFinder.TryFindMODIS_QKM_L1From03(mod03File);
            IRasterDataProvider hkmRaster = FileFinder.TryFindMODIS_HKM_L1From03(mod03File);
            IRasterDataProvider kmRaster = srcRaster;
            return PrjMODIS(prjOutArg, errorMessage, mod03File, ref qkmRaster, ref hkmRaster, kmRaster, progress);
        }

        private string[] PrjMODIS_1KM_L1Only(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            IRasterDataProvider srcRaster = fileName;
            IRasterDataProvider locationRaster = null;
            IFileProjector projTor = null;
            errorMessage = new StringBuilder();
            int[] qkmBands;
            int[] hkmBands;
            int[] kmBands;
            FileFinder.GetModisBandmapTable(fileName, null, null, prjOutArg.SelectedBands, out qkmBands, out hkmBands, out kmBands);
            try
            {
                List<string> outFiles = new List<string>();
                locationRaster = FileFinder.TryFind03FileFromModisImgFile(fileName);
                projTor = FileProjector.GetFileProjectByName("EOS");
                projTor.BeginSession(srcRaster);
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    try
                    {
                        EOS_MODIS_PrjSettings prjSetting = new EOS_MODIS_PrjSettings();
                        prjSetting.LocationFile = locationRaster;
                        //string outfilename = IsDir(prjOutArg.OutDirOrFile) ? GetOutPutFile(prjOutArg.OutDirOrFile, fileName.fileName, dstSpatialRef, prjEnvelopes[i].Name) : prjOutArg.OutDirOrFile;

                        string outFileName = null;
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, srcRaster.fileName, dstSpatialRef, prjEnvelopes[i].Name, srcRaster.DataIdentify, prjOutArg.ResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;

                        prjSetting.OutPathAndFileName = outFileName;
                        prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                        prjSetting.OutBandNos = kmBands;
                        prjSetting.OutResolutionX = prjOutArg.ResolutionX;
                        prjSetting.OutResolutionY = prjOutArg.ResolutionY;
                        projTor.Project(srcRaster, prjSetting, dstSpatialRef, progress);
                        outFiles.Add(prjSetting.OutPathAndFileName);
                    }
                    catch (Exception ex)
                    {
                        outFiles.Add(null);
                        errorMessage.AppendLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (locationRaster != null)
                    locationRaster.Dispose();
                if (projTor != null)
                    projTor.EndSession();
            }
        }

        private string[] PrjMERSI_QKM_L1(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            IRasterDataProvider qkmRaster = fileName;
            IRasterDataProvider kmRaster = FileFinder.TryFindMERSI_1KM_L1FromQKM(qkmRaster);
            return PrjMersi(qkmRaster, kmRaster, prjOutArg, progress, out errorMessage);
        }

        private string[] PrjMERSI_1KM_L1(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            IRasterDataProvider qkmRaster = FileFinder.TryFindMERSI_QKM_L1FromKM(fileName);
            IRasterDataProvider kmRaster = fileName;
            return PrjMersi(qkmRaster, kmRaster, prjOutArg, progress, out errorMessage);
        }

        // FY3A_VIRR_06_GLL_L1_20120909_Day3_1000M.LDF
        private string GetOutPutFile(string outDir, string filename, ISpatialReference projRef, string blockname, DataIdentify identify, float resolution, string extName)
        {
            string outFileNmae = "";
            string filenameOnly = Path.GetFileName(filename);
            string prjIdentify;
            prjIdentify = GetPrjIdentify(projRef);
            if (identify != null && !string.IsNullOrWhiteSpace(identify.Satellite)
                && !string.IsNullOrWhiteSpace(identify.Sensor))
            {
                string satellite = identify.Satellite;
                string sensor = identify.Sensor;
                DateTime datetime = identify.OrbitDateTime;//TimeOfDay == TimeSpan.Zero ? dataIdentify.OrbitDateTime : identify.OrbitDateTime;
                string otname = _genericFilename.GetL1PrjFilename(satellite, sensor, datetime, filenameOnly, prjIdentify, blockname, resolution, extName);
                outFileNmae = Path.Combine(outDir, otname);
            }
            else
            {
                string otname = _genericFilename.PrjBlockFilename(filenameOnly, prjIdentify, blockname,extName);
                outFileNmae = Path.Combine(outDir, otname);
            }
            return CreateOnlyFilename(outFileNmae);
        }

        private static string GetPrjIdentify(ISpatialReference projRef)
        {
            string prjIdentify;
            if (projRef == null || string.IsNullOrWhiteSpace(projRef.Name))
                prjIdentify = "GLL";
            else if (projRef.ProjectionCoordSystem == null)
                prjIdentify = GenericFilename.GetProjectionIdentify(projRef.GeographicsCoordSystem.Name);
            else
                prjIdentify = GenericFilename.GetProjectionIdentify(projRef.ProjectionCoordSystem.Name.Name);
            return prjIdentify;
        }

        /// <summary>
        /// 生成非重复的文件名：如果已经存在了，自动在其后添加(1)或(2)等。
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string CreateOnlyFilename(string filename)
        {
            if (File.Exists(filename))
            {
                string dir = Path.GetDirectoryName(filename);
                string filenameWithExt = Path.GetFileNameWithoutExtension(filename);
                string fileExt = Path.GetExtension(filename);
                int i = 1;
                string outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i + ")" + fileExt);
                while (File.Exists(outFileNmae))
                    outFileNmae = Path.Combine(dir, filenameWithExt + "(" + i++ + ")" + fileExt);
                return outFileNmae;
            }
            else
                return filename;
        }

        private string GetBlockName()
        {
            return "DXX";
        }

        private string[] PrjMersi(IRasterDataProvider qkmRaster, IRasterDataProvider kmRaster, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            float outResolutionX = prjOutArg.ResolutionX;
            float outResolutionY = prjOutArg.ResolutionY;
            SetDefaultResolutionForMersi(qkmRaster, dstSpatialRef, ref outResolutionX, ref outResolutionY);
            prjOutArg.ResolutionX = outResolutionX;
            prjOutArg.ResolutionY = outResolutionY;
            string outDir = prjOutArg.OutDirOrFile;

            float baseResolutionK = 0.01f;
            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
            {
                baseResolutionK = 0.01f;
            }
            else
            {
                baseResolutionK = 1000f;
            }
            if (baseResolutionK / outResolutionX < 1.5 && baseResolutionK / outResolutionY < 1.5)
            {
                if (qkmRaster != null && kmRaster != null)
                {
                    qkmRaster.Dispose();
                    qkmRaster = null;
                }
            }

            errorMessage = new StringBuilder();

            int[] qkmBands = null;
            int[] kmBands = null;
            FileFinder.GetBandmapTableMERSI(qkmRaster, kmRaster, prjOutArg.SelectedBands, out qkmBands, out kmBands);
            int bandCount = (qkmBands == null ? 0 : qkmBands.Length) + (kmBands == null ? 0 : kmBands.Length);
            if (bandCount == 0)
            {
                errorMessage.Append("没有获取要投影的通道");
                return null;
            }
            string bandNames = "BANDNAMES="
                + (qkmBands == null || qkmBands.Length == 0 ? "" : BandNameString(qkmBands) + ",")
                + BandNameString(kmBands);
            IFileProjector projtor = FileProjector.GetFileProjectByName("FY3_MERSI");
            try
            {
                List<string> outFiles = new List<string>();
                IRasterDataProvider outRaster = null;
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    PrjEnvelope envelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                    if (envelope == null)
                    {
                        projtor.ComputeDstEnvelope(kmRaster, dstSpatialRef, out envelope, null);
                        if (envelope == null)
                        {
                            errorMessage.Append("未能读取出文件的经纬度范围：" + kmRaster.fileName);
                            continue;
                        }
                    }
                    string outFileName = null;
                    try
                    {
                        string orbitFileName = qkmRaster != null ? qkmRaster.fileName : (kmRaster != null ? kmRaster.fileName : null);
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, orbitFileName, dstSpatialRef, prjEnvelopes[i].Name, kmRaster.DataIdentify, outResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;
                        outRaster = CreateRaster(outFileName, envelope, outResolutionX, outResolutionY, bandCount, dstSpatialRef, bandNames);
                        outFiles.Add(outFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        errorMessage.Append(ex.Message);
                        outFiles.Add(null);
                    }
                    finally
                    {
                        if (outRaster != null)
                        {
                            outRaster.Dispose();
                            outRaster = null;
                        }
                    }
                }
                bool hasAngle = false;
                int perBandCount = 0;
                List<string> errorFiles = new List<string>();
                if (qkmBands != null && qkmBands.Length != 0)
                {
                    perBandCount = qkmBands.Length;
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                            prjSetting.SecondaryOrbitRaster = kmRaster;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutBandNos = qkmBands;
                            prjSetting.OutPathAndFileName = outFiles[i];
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(qkmRaster, prjSetting, outRaster, 0, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                            errorFiles.Add(outFiles[i]);
                            outFiles[i] = null;
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                    projtor.EndSession();
                }

                if (kmBands != null && kmBands.Length != 0)
                {
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                            prjSetting.OutPathAndFileName = outFiles[i];
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutBandNos = kmBands;
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                                prjSetting.ExtArgs = prjOutArg.Args;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(kmRaster, prjSetting, outRaster, perBandCount, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                            errorFiles.Add(outFiles[i]);
                            outFiles[i] = null;
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                }
                foreach (string errorfile in errorFiles)
                {
                    TryDeleteErrorFile(errorfile);
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projtor != null)
                    projtor.EndSession();
                if (kmRaster != null)
                    kmRaster.Dispose();
            }
        }

        private string[] PrjFY3C_MERSI_1KM_L1(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            IRasterDataProvider kmRaster = fileName;
            IRasterDataProvider qkmRaster = FileFinder.TryFindMERSI_QKM_L1FromKM(fileName);
            IRasterDataProvider geoRaster = FileFinder.TryFindGeoFileFromFY3C_MERSI(fileName);
            IRasterDataProvider qkmGeoRaster = FileFinder.TryFindQkmGeoFileFromFY3C_MERSI(fileName);//qkmGeo中仅存储了地理坐标信息。
            if (geoRaster == null && qkmGeoRaster == null)
            {
                throw new Exception("无法找到角度数据(如经纬度等)文件[._GEO1K_...HDF]或[._GEOQK_...HDF]");
            }
            return PrjFY3C_MERSI(qkmRaster, kmRaster, qkmGeoRaster, geoRaster, prjOutArg, progress, out errorMessage);
        }

        private string[] PrjFY3C_MERSI_QKM_L1(IRasterDataProvider fileName, PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            IRasterDataProvider qkmRaster = fileName;
            IRasterDataProvider kmRaster = FileFinder.TryFindFY3C_MERSI_1KM_L1FromQKM(fileName);
            IRasterDataProvider kmGeoRaster = FileFinder.TryFindkmGeoFileFromFY3C_MERSI(fileName);
            IRasterDataProvider qkmGeoRaster = FileFinder.TryFindQkmGeoFileFromFY3C_MERSI(fileName);
            if (kmGeoRaster == null && qkmGeoRaster == null)
            {
                throw new Exception("无法找到角度数据(如经纬度等)文件[._GEO1K_...HDF]或[._GEOQK_...HDF]"); 
            }
            return PrjFY3C_MERSI(qkmRaster, kmRaster, qkmGeoRaster, kmGeoRaster, prjOutArg, progress, out errorMessage);
        }

        private string[] PrjFY3C_MERSI(IRasterDataProvider qkmRaster, IRasterDataProvider kmRaster, IRasterDataProvider qkmGeoRaster, IRasterDataProvider kmGeoRaster,
            PrjOutArg prjOutArg, Action<int, string> progress, out StringBuilder errorMessage)
        {
            ISpatialReference dstSpatialRef = prjOutArg.ProjectionRef;
            PrjEnvelopeItem[] prjEnvelopes = prjOutArg.Envelopes;
            float outResolutionX = prjOutArg.ResolutionX;
            float outResolutionY = prjOutArg.ResolutionY;
            SetDefaultResolutionForMersi(qkmRaster, dstSpatialRef, ref outResolutionX, ref outResolutionY);
            prjOutArg.ResolutionX = outResolutionX;
            prjOutArg.ResolutionY = outResolutionY;
            string outDir = prjOutArg.OutDirOrFile;

            float baseResolutionK = 0.01f;
            if (dstSpatialRef == null || dstSpatialRef.ProjectionCoordSystem == null)
            {
                baseResolutionK = 0.01f;
            }
            else
            {
                baseResolutionK = 1000f;
            }
            if (baseResolutionK / outResolutionX < 1.5 && baseResolutionK / outResolutionY < 1.5)
            {
                if (qkmRaster != null && kmRaster != null)
                {
                    qkmRaster.Dispose();
                    qkmRaster = null;
                }
            }

            errorMessage = new StringBuilder();

            int[] qkmBands = null;
            int[] kmBands = null;
            FileFinder.GetBandmapTableMERSI(qkmRaster, kmRaster, prjOutArg.SelectedBands, out qkmBands, out kmBands);
            int bandCount = (qkmBands == null ? 0 : qkmBands.Length) + (kmBands == null ? 0 : kmBands.Length);
            if (bandCount == 0)
            {
                errorMessage.Append("没有获取要投影的通道");
                return null;
            }
            string bandNames = "BANDNAMES="
                + (qkmBands == null || qkmBands.Length == 0 ? "" : BandNameString(qkmBands) + ",")
                + BandNameString(kmBands);
            IFileProjector projtor = FileProjector.GetFileProjectByName("FY3C_MERSI");
            try
            {
                List<string> outFiles = new List<string>();
                IRasterDataProvider outRaster = null;
                for (int i = 0; i < prjEnvelopes.Length; i++)
                {
                    PrjEnvelope envelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                    if (envelope == null)
                    {
                        if (kmGeoRaster != null)
                            projtor.ComputeDstEnvelope(kmGeoRaster, dstSpatialRef, out envelope, null);
                        else
                            projtor.ComputeDstEnvelope(qkmGeoRaster, dstSpatialRef, out envelope, null);
                        if (envelope == null)
                        {
                            errorMessage.Append("未能读取出文件的经纬度范围：" + kmRaster.fileName);
                            continue;
                        }
                    }
                    string outFileName = null;
                    try
                    {
                        string orbitFileName = null;
                        DataIdentify dataIdentify = null;
                        if (qkmRaster != null)
                        {
                            orbitFileName = qkmRaster.fileName;
                            dataIdentify = qkmRaster.DataIdentify;
                        }
                        else
                        {
                            orbitFileName = kmRaster.fileName;
                            dataIdentify = kmRaster.DataIdentify;
                        }
                        if (IsDir(prjOutArg.OutDirOrFile))
                            outFileName = GetOutPutFile(prjOutArg.OutDirOrFile, orbitFileName, dstSpatialRef, prjEnvelopes[i].Name, dataIdentify, outResolutionX, ".ldf");
                        else
                            outFileName = prjOutArg.OutDirOrFile;
                        outRaster = CreateRaster(outFileName, envelope, outResolutionX, outResolutionY, bandCount, dstSpatialRef, bandNames);
                        outFiles.Add(outFileName);
                    }
                    catch (Exception ex)
                    {
                        TryDeleteErrorFile(outFileName);
                        errorMessage.Append(ex.Message);
                        outFiles.Add(null);
                    }
                    finally
                    {
                        if (outRaster != null)
                        {
                            outRaster.Dispose();
                            outRaster = null;
                        }
                    }
                }
                bool hasAngle = false;
                int perBandCount = 0;
                if (qkmBands != null && qkmBands.Length != 0)
                {
                    perBandCount = qkmBands.Length;
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                            prjSetting.SecondaryOrbitRaster = kmRaster;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutBandNos = qkmBands;
                            prjSetting.OutPathAndFileName = outFiles[i];
                            prjSetting.GeoFile = qkmGeoRaster != null ? qkmGeoRaster : kmGeoRaster;
                            prjSetting.AngleFile = kmGeoRaster;//
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                            }
                            if (!hasAngle)//只输出一次角度数据
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(qkmRaster, prjSetting, outRaster, 0, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                    projtor.EndSession();
                }
                if (kmBands != null && kmBands.Length != 0)
                {
                    projtor.BeginSession(qkmRaster);
                    for (int i = 0; i < prjEnvelopes.Length; i++)
                    {
                        if (outFiles[i] == null)
                            continue;
                        try
                        {
                            outRaster = OpenUpdate(outFiles[i]);
                            FY3_MERSI_PrjSettings prjSetting = new FY3_MERSI_PrjSettings();
                            prjSetting.OutPathAndFileName = outFiles[i];
                            prjSetting.OutEnvelope = prjEnvelopes[i] == null ? null : prjEnvelopes[i].PrjEnvelope;
                            prjSetting.OutResolutionX = outResolutionX;
                            prjSetting.OutResolutionY = outResolutionY;
                            prjSetting.OutBandNos = kmBands;
                            prjSetting.GeoFile = kmGeoRaster != null ? kmGeoRaster : qkmGeoRaster;
                            prjSetting.AngleFile = kmGeoRaster;//
                            if (prjOutArg.Args != null)
                            {
                                if (prjOutArg.Args.Contains("NotRadiation"))
                                    prjSetting.IsRadiation = false;
                                if (prjOutArg.Args.Contains("NotSolarZenith"))
                                    prjSetting.IsSolarZenith = false;
                                if (prjOutArg.Args.Contains("IsSensorZenith"))
                                    prjSetting.IsSensorZenith = true;
                                if (prjOutArg.Args.Contains("IsClearPrjCache"))
                                    prjSetting.IsClearPrjCache = true;
                                prjSetting.ExtArgs = prjOutArg.Args;
                            }
                            if (!hasAngle)
                            {
                                prjSetting.ExtArgs = prjOutArg.Args;
                                hasAngle = true;
                            }
                            projtor.Project(kmRaster, prjSetting, outRaster, perBandCount, progress);
                        }
                        catch (Exception ex)
                        {
                            errorMessage.AppendLine(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            if (outRaster != null)
                            {
                                outRaster.Dispose();
                                outRaster = null;
                            }
                        }
                    }
                }
                return outFiles.ToArray();
            }
            finally
            {
                if (projtor != null)
                    projtor.EndSession();
                if (kmRaster != null)
                    kmRaster.Dispose();
            }
        }

        private static void SetDefaultResolutionForMersi(IRasterDataProvider qkmRaster, ISpatialReference dstSpatialRef, ref float outResolutionX, ref float outResolutionY)
        {
            if (outResolutionX == 0 || outResolutionY == 0)
            {
                if (qkmRaster != null)
                {
                    if (dstSpatialRef.ProjectionCoordSystem == null)
                    {
                        outResolutionX = 0.0025F;
                        outResolutionY = 0.0025F;
                    }
                    else
                    {
                        outResolutionX = 250F;//投影坐标系
                        outResolutionY = 250F;
                    }
                }
                else
                {
                    if (dstSpatialRef.ProjectionCoordSystem == null)
                    {
                        outResolutionX = 0.01f;
                        outResolutionY = 0.01f;
                    }
                    else
                    {
                        outResolutionX = 1000;//投影坐标系
                        outResolutionY = 1000;
                    }
                }
            }
        }

        private IRasterDataProvider CreateRaster(string outfilename, PrjEnvelope envelope, float outResolutionX, float outResolutionY, int bandCount, ISpatialReference spatialRef, string bandNames)
        {
            Size outSize = envelope.GetSize(outResolutionX, outResolutionY);
            string[] options = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + spatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + envelope.MinX + "," + envelope.MaxY + "}:{" + outResolutionX + "," + outResolutionY + "}"
                ,bandNames
            };
            return CreateOutFile(outfilename, bandCount, outSize, options);
        }

        private IRasterDataProvider CreateOutFile(string outfilename, int dstBandCount, Size outSize, string[] options)
        {
            string dir = Path.GetDirectoryName(outfilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount, enumDataType.UInt16, options) as IRasterDataProvider;
        }

        public static PrjOutArg GetDefaultArg(string fileName)
        {
            IRasterDataProvider rad = null;
            try
            {
                rad = RasterDataDriver.Open(fileName) as IRasterDataProvider;
                FileChecker checker = new FileChecker();
                string type = checker.GetFileType(rad);
                switch (type)
                {
                    case "VIRR_L1":
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.01f, 0.01f, Path.GetDirectoryName(fileName));
                    case "MERSI_1KM_L1":
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.01f, 0.01f, Path.GetDirectoryName(fileName));
                    case "MERSI_QKM_L1":
                    case "MERSI_250M_L1":
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.0025f, 0.0025f, Path.GetDirectoryName(fileName));
                    case "MODIS_1KM_L1":
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.0025f, 0.0025f, Path.GetDirectoryName(fileName));
                    case "NOAA_1BD":
                    case "FY1X_1A5": 
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.01f, 0.01f, Path.GetDirectoryName(fileName));
                    case "FY2NOM":
                        return new PrjOutArg("", new PrjEnvelopeItem[] { null }, 0.05f, 0.05f, Path.GetDirectoryName(fileName));
                    default:
                        return null;
                }
            }
            finally
            {
                if (rad != null)
                    rad.Dispose();
            }
        }

        public static IRasterDataProvider OpenUpdate(string filename)
        {
            return GeoDataDriver.Open(filename, enumDataProviderAccess.Update, null) as IRasterDataProvider;
        }

        public static IRasterDataProvider CheckPrjArg(IRasterDataProvider rasterIn)
        {
            string fileType = new FileChecker().GetFileType(rasterIn);
            switch (fileType)
            {
                case "VIRR_L1":
                case "FY1X_1A5":
                case "MERSI_1KM_L1":
                case "MODIS_1KM_L1":
                case "NOAA_1BD":
                case "PROJECTED":
                case "FY2NOM":
                    return rasterIn;
                case "FY3C_VIRR_L1":
                    using (IRasterDataProvider geo = FileFinder.TryFindGeoFileFromFY3C_VIRR(rasterIn))
                    {
                        return rasterIn;
                    }
                case "FY3C_MERSI_1KM_L1":
                case "FY3C_MERSI_QKM_L1":
                    using (IRasterDataProvider geo = FileFinder.TryFindGeoFileFromFY3C_MERSI(rasterIn))
                    {
                        return rasterIn;
                    }
                case "MERSI_QKM_L1":
                case "MERSI_250M_L1":
                    return FileFinder.TryFindMERSI_1KM_L1FromQKM(rasterIn);
                case "MODIS_HKM_L1":
                    using (IRasterDataProvider mod03 = FileFinder.TryFind03FileFromModisImgFile(rasterIn))
                    {
                        return FileFinder.TryFindMODIS_1KM_L1From03(mod03);
                    }
                case "MODIS_QKM_L1":
                    using (IRasterDataProvider mod03 = FileFinder.TryFind03FileFromModisImgFile(rasterIn))
                    {
                        return FileFinder.TryFindMODIS_1KM_L1From03(mod03);
                    }
                default:
                    return null;
            }
        }
    }
}
