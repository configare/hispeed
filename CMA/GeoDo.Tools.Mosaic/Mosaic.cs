using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RasterProject;
using GeoDo.Project;
using GeoDo.RSS.DF.LDF;
using GeoDo.RSS.BlockOper;
using GeoDo.FileProject;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.Tools.Mosaic
{
    /// <summary>
    /// 镶嵌，拼接
    /// 支持按轨道拼接，或者按照指定范围拼接
    /// Mosaic：镶嵌，源文件，镶嵌到目标文件
    /// Splice：拼接，两个文件直接拼接，拼接后，替换原来的文件。
    /// </summary>
    public class MosaicSplice
    {
        private MosaicInputArg inArg = null;
        private Action<int, string> action;
        private int _mosaicSize = 1024;

        public MosaicSplice(MosaicInputArg inArg, Action<int, string> action)
        {
            this.inArg = inArg;
            this.action = action;

            string size = System.Configuration.ConfigurationManager.AppSettings["MosaicThumbnailSize"];
            if (!string.IsNullOrWhiteSpace(size))
                int.TryParse(size, out _mosaicSize);
        }

        public void DoMosaicSplice()
        {
            CheckArgs(inArg);
            if (inArg.MosaicMode == "Mosaic")//镶嵌，多个文件镶嵌到指定区域
            {
               MosaicToFile();
            }
            else if (inArg.MosaicMode == "Splice")//拼接，多个文件拼接到一起
            {
                SpliceToFile();
            }
            else
                throw new ArgumentException("输入参数xml节点MosaicMode的值只能是Mosaic或者Splice，请检查", "MosaicMode");
        }

        private void CheckArgs(MosaicInputArg inArg)
        {
            if (string.IsNullOrWhiteSpace(inArg.InputFilename))
                throw new ArgumentNullException("输入的文件为空");
            if (!File.Exists(inArg.InputFilename))
                throw new FileNotFoundException("未发现该文件", inArg.InputFilename);
        }
        
        /// <summary>
        /// 拼接
        /// 需要保证tFilename不存在。
        /// </summary>
        private void SpliceToFile()
        {
            using (IRasterDataProvider inputRaster = RasterDataDriver.Open(inArg.InputFilename) as IRasterDataProvider)
            {
                StringBuilder str = new StringBuilder();
                List<OutFileArg> outFiles = new List<OutFileArg>();
                string logLevel = "info";
                foreach (PrjEnvelopeItem envItem in inArg.MosaicEnvelopes)
                {
                    try
                    {
                        if (!ProjectionFactory.HasInvildEnvelope(inputRaster, envItem.PrjEnvelope))
                        {
                            str.AppendLine("数据不在范围内：" + envItem.Name + envItem.PrjEnvelope.ToString());
                            continue;
                        }
                        string tFilename = GetSpliceFilename(inputRaster, inArg, envItem.Name);
                        tFilename = Path.Combine(inArg.OutputDir, tFilename);
                        if (!Directory.Exists(inArg.OutputDir))
                            Directory.CreateDirectory(inArg.OutputDir);
                        //文件不存在，直接复制当前文件为目标文件
                        if (!File.Exists(tFilename))
                        {
                            File.Copy(inArg.InputFilename, tFilename);
                            OutFileArg fileArg = new OutFileArg();
                            string tHdrFilename = FilenameIdentify.HdrFileName(tFilename);
                            string tOverviewFilename = FilenameIdentify.OverviewFileName(tFilename);
                            fileArg.Envelope = new PrjEnvelopeItem(envItem.Name, CoordToEnvelope(inputRaster.CoordEnvelope));
                            fileArg.ResolutionX = inputRaster.ResolutionX.ToString();
                            fileArg.ResolutionY = inputRaster.ResolutionY.ToString();
                            fileArg.OutputFilename = tFilename;
                            fileArg.Length = new FileInfo(tFilename).Length;
                            if (File.Exists(FilenameIdentify.HdrFileName(inArg.InputFilename)))
                            {
                                File.Copy(FilenameIdentify.HdrFileName(inArg.InputFilename), tHdrFilename);
                                fileArg.ExtendFiles = Path.GetFileName(tHdrFilename);
                            }
                            if (File.Exists(FilenameIdentify.OverviewFileName(inArg.InputFilename)))
                            {
                                File.Copy(FilenameIdentify.OverviewFileName(inArg.InputFilename), tOverviewFilename);
                                fileArg.Thumbnail = Path.GetFileName(tOverviewFilename);
                            }
                            else if (File.Exists(tFilename))
                            {
                                OnProgress(0, "生成缩略图");
                                tOverviewFilename = OverViewHelper.OverView(tFilename, _mosaicSize);
                                OnProgress(100, "完成缩略图");
                                fileArg.Thumbnail = Path.GetFileName(tOverviewFilename);
                            }
                            outFiles.Add(fileArg);
                        }
                        else
                        {
                            OutFileArg fileArg = SpliceToExistFile(inputRaster, tFilename);
                            fileArg.Envelope.Name = envItem.Name;
                            outFiles.Add(fileArg);
                        }
                    }
                    catch (Exception ex)
                    {
                        logLevel = "error";
                        str.AppendLine("拼接区域失败：" + envItem.Name + envItem.PrjEnvelope.ToString());
                        str.AppendLine(" 输入文件：" + inArg.InputFilename);
                        str.AppendLine(" 详细信息：" + ex.Message);
                        LogFactory.WriteLine(ex);
                    }
                }
                //写输出参数。
                MosaicOutputArg outArg = new MosaicOutputArg();
                outArg.InputFilename = Path.GetFileName(inArg.InputFilename);
                outArg.OutputFiles = outFiles.ToArray();
                outArg.LogLevel = logLevel;
                outArg.LogInfo = str.ToString();
                string outXml = FilenameIdentify.OutPutXmlFilename(inArg.OutputDir,inArg.InputFilename);
                outArg.ToXml(outXml);
            }
        }

        ///将inputRaster拼接到tFilename。
        private OutFileArg SpliceToExistFile(IRasterDataProvider inputRaster, string tFilename)
        {
            OutFileArg fileArg = new OutFileArg();
            string tmpDir = Path.GetDirectoryName(tFilename);
            string tmpFilename = CreateTempFilename(tmpDir);
            try
            {
                using (IRasterDataProvider tRaster = RasterDataDriver.Open(tFilename) as IRasterDataProvider)
                {
                    if (inputRaster.BandCount != tRaster.BandCount)
                        throw new ExceptionExt(2, "待拼接文件和目标文件的波段数不同，无法完成文件拼接[" + inputRaster.fileName + "]" + "[" + tFilename + "]");
                    CoordEnvelope tEnv = inputRaster.CoordEnvelope.Union(tRaster.CoordEnvelope);
                    PrjEnvelope tEnvelope = CoordToEnvelope(tEnv);
                    using (IRasterDataProvider tmpFileRaster = CreateMosaicFile(inputRaster, tEnvelope, tmpFilename))
                    {
                        RasterMoasicProcesser mo = new RasterMoasicProcesser();
                        mo.Moasic(new IRasterDataProvider[] { inputRaster, tRaster }, tmpFileRaster, true, new string[] { "0" }, action);
                        fileArg.Envelope = new PrjEnvelopeItem("", tEnvelope);
                        fileArg.ResolutionX = tmpFileRaster.ResolutionX.ToString();
                        fileArg.ResolutionY = tmpFileRaster.ResolutionY.ToString();
                    }
                }
                if (File.Exists(tmpFilename))
                {
                    try
                    {
                        string tmpHdrFilename = FilenameIdentify.HdrFileName(tmpFilename);
                        string tmpOverviewFilename = FilenameIdentify.OverviewFileName(tmpFilename);
                        string tHdrFilename = FilenameIdentify.HdrFileName(tFilename);
                        string tOverviewFilename = FilenameIdentify.OverviewFileName(tFilename);
                        File.Delete(tFilename);//删除原文件
                        if (File.Exists(tHdrFilename))
                            File.Delete(tHdrFilename);
                        if (File.Exists(tOverviewFilename))
                            File.Delete(tOverviewFilename);
                        //拼接后的文件重命名为目标文件
                        File.Move(tmpFilename, tFilename);
                        fileArg.OutputFilename = Path.GetFileName(tFilename);
                        fileArg.Length = new FileInfo(tFilename).Length;
                        if (File.Exists(tmpHdrFilename))
                        {
                            File.Move(tmpHdrFilename, tHdrFilename);
                            fileArg.ExtendFiles = Path.GetFileName(tHdrFilename);
                        }
                        if (File.Exists(tmpOverviewFilename))
                        {
                            File.Move(tmpOverviewFilename, tOverviewFilename);
                            fileArg.Thumbnail = Path.GetFileName(tOverviewFilename);
                        }
                        else if (File.Exists(tFilename))
                        {
                            OnProgress(0, "生成缩略图");
                            tOverviewFilename = OverViewHelper.OverView(tFilename, _mosaicSize);
                            OnProgress(100, "完成缩略图");
                            fileArg.Thumbnail = Path.GetFileName(tOverviewFilename);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("拼接完成后，重命名新拼接文件失败。" + ex.Message, ex);
                    }
                }
                return fileArg;
            }
            finally
            {
                TryDeleteLdfs(tmpFilename);
            }
        }

        private void TryDeleteLdfs(string tmpFilename)
        {
            try
            {
                if (File.Exists(tmpFilename))
                {
                    File.Delete(tmpFilename);
                    string tHdrFilename = FilenameIdentify.HdrFileName(tmpFilename);
                    string tOverviewFilename = FilenameIdentify.OverviewFileName(tmpFilename);
                    if (File.Exists(tHdrFilename))
                        File.Delete(tHdrFilename);
                    if (File.Exists(tOverviewFilename))
                        File.Delete(tOverviewFilename);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("删除临时文件失败，请手动删除" + tmpFilename + ex.Message);
            }
        }

        private void OnProgress(int progress, string text)
        {
            if (action != null)
                action(progress, text);
        }

        private string CreateTempFilename(string dir)
        {
            return Path.Combine(dir, Guid.NewGuid().ToString() + ".LDF");
        }

        //NOAA18_AVHRR_CHINA_GLL_L1_20000101_0001_1000M_Day.LDF
        private string GetSpliceFilename(IRasterDataProvider inputRaster, MosaicInputArg inArg, string envName)
        {
            float resolution = inputRaster.ResolutionX;
            DataIdentify dataIdentify = inputRaster.DataIdentify;
            //PrjEnvelopeItem item = inArg.MosaicEnvelopes[0];
            string projectionIdentify = "GLL";
            string dayOrNight = inArg.DayOrNight;
            string orbitIdentify = inArg.OrbitIdentify;
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_MOSA.ldf",
                dataIdentify.Satellite,
                dataIdentify.Sensor,
                envName,
                projectionIdentify,
                "L1",
                dataIdentify.OrbitDateTime.ToString("yyyyMMdd"),
                orbitIdentify.PadRight(4, '0'),
                projectionIdentify == "GLL" ? GLLResolutionIdentify(resolution) : ResolutionIdentify(resolution),
                dayOrNight
                );
        }

        /// <summary>
        /// 镶嵌
        /// </summary>
        /// <returns></returns>
        private void MosaicToFile()
        {
            using (IRasterDataProvider inputRaster = RasterDataDriver.Open(inArg.InputFilename) as IRasterDataProvider)
            {
                StringBuilder str = new StringBuilder();
                List<OutFileArg> outFiles = new List<OutFileArg>();
                string logLevel = "info";
                if (inArg.MosaicEnvelopes == null)
                {
                    logLevel = "error";
                    str.AppendLine("镶嵌(Mosaic)必须指定镶嵌输出范围");
                    return;
                }
                else
                {
                    for (int i = 0; i < inArg.MosaicEnvelopes.Length; i++)
                    {
                        PrjEnvelopeItem envItem = inArg.MosaicEnvelopes[i];
                        try
                        {
                            if (envItem == null)
                            {
                                str.AppendLine("镶嵌(Mosaic)必须指定镶嵌输出范围");
                                continue;
                            }
                            if (!ProjectionFactory.HasInvildEnvelope(inputRaster, envItem.PrjEnvelope))
                            {
                                str.AppendLine("数据不在范围内：" + envItem.Name + envItem.PrjEnvelope.ToString());
                                continue;
                            }
                            string mosaicFilename = GetSpliceFilename(inputRaster, inArg, envItem.Name);
                            mosaicFilename = Path.Combine(inArg.OutputDir, mosaicFilename);
                            if (!Directory.Exists(inArg.OutputDir))
                                Directory.CreateDirectory(inArg.OutputDir);
                            //文件不存在，则需要先创建目标文件。
                            IRasterDataProvider mosaicRaster = null;
                            if (!File.Exists(mosaicFilename))
                                mosaicRaster = CreateMosaicFile(inputRaster, envItem.PrjEnvelope, mosaicFilename);
                            else
                                mosaicRaster = RasterDataDriver.Open(mosaicFilename, enumDataProviderAccess.Update, null) as IRasterDataProvider;
                            if (File.Exists(mosaicFilename))
                            {
                                OutFileArg fileArg = new OutFileArg();
                                try
                                {
                                    Mosaic(inputRaster, mosaicRaster);
                                    fileArg.Envelope = new PrjEnvelopeItem(envItem.Name, envItem.PrjEnvelope);
                                    fileArg.ResolutionX = mosaicRaster.ResolutionX.ToString();
                                    fileArg.ResolutionY = mosaicRaster.ResolutionY.ToString();
                                    fileArg.OutputFilename = mosaicFilename;
                                    fileArg.Length = new FileInfo(mosaicFilename).Length;
                                }
                                finally
                                {
                                    if (mosaicRaster != null)
                                        mosaicRaster.Dispose();
                                }
                                try
                                {
                                    string tHdrFilename = FilenameIdentify.HdrFileName(mosaicFilename);
                                    string tOverviewFilename = FilenameIdentify.OverviewFileName(mosaicFilename);
                                    if (File.Exists(mosaicFilename))
                                    {
                                        tOverviewFilename = OverViewHelper.OverView(mosaicFilename, _mosaicSize);
                                        fileArg.Thumbnail = Path.GetFileName(tOverviewFilename);
                                    }
                                    if (File.Exists(tHdrFilename))
                                    {
                                        fileArg.ExtendFiles = Path.GetFileName(tHdrFilename);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    str.AppendLine("拼接完成后，生成缩略图失败" + ex.Message);
                                }
                                outFiles.Add(fileArg);
                            }
                        }
                        catch (Exception ex)
                        {
                            logLevel = "error";
                            str.AppendLine("镶嵌区域失败：" + envItem.Name + envItem.PrjEnvelope.ToString());
                            str.AppendLine(" 输入文件：" + inArg.InputFilename);
                            str.AppendLine(" 详细信息：" + ex.Message);
                            LogFactory.WriteLine(ex);
                        }
                        finally
                        {
                        }
                    }
                }
                //写输出参数。
                MosaicOutputArg outArg = new MosaicOutputArg();
                outArg.InputFilename = Path.GetFileName(inArg.InputFilename);
                outArg.OutputFiles = outFiles.ToArray();
                outArg.LogLevel = logLevel;
                outArg.LogInfo = str.ToString();
                string outXml = FilenameIdentify.OutPutXmlFilename(inArg.OutputDir, inArg.InputFilename);
                outArg.ToXml(outXml);
            }
        }

        private void Mosaic(IRasterDataProvider inputRaster, IRasterDataProvider mosaicRaster)
        {
            //RasterMoasicProcesser mo = new RasterMoasicProcesser();
            //mo.Moasic(new IRasterDataProvider[] { inputRaster }, mosaicRaster, true, new string[] { "0" }, action);
            switch (mosaicRaster.DataType)
            {
                case enumDataType.Atypism:
                    break;
                case enumDataType.Bits:
                    break;
                case enumDataType.Byte:
                    Mosaic<Byte>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Double:
                    Mosaic<Int16>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Float:
                    Mosaic<Int16>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Int16:
                    Mosaic<Int16>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Int32:
                    Mosaic<Int32>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Int64:
                    Mosaic<Int64>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.UInt16:
                    Mosaic<UInt16>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.UInt32:
                    Mosaic<UInt32>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.UInt64:
                    Mosaic<UInt64>(inputRaster, mosaicRaster, 0);
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }

        }

        private void Mosaic<T>(IRasterDataProvider inputRaster, IRasterDataProvider mosaicRaster, T invalidValue)
        {
            RasterMaper inmaper = new RSS.MIF.Core.RasterMaper(inputRaster, null);
            RasterMaper outmaper = new RSS.MIF.Core.RasterMaper(mosaicRaster, null);
            RasterProcessModel<T, T> rp = new RasterProcessModel<T, T>(action);
            rp.SetRaster(new RasterMaper[] { inmaper }, new RasterMaper[] { outmaper });
            rp.RegisterCalcModel(new RasterCalcHandler<T, T>(
                (inR, outR, aoi) =>
                {
                    if (inR[0].RasterBandsData[0] == null)
                        return;
                    int length = inR[0].RasterBandsData[0].Length;
                    int bandCount = inR[0].RasterBandsData.Length;
                    for (int b = 0; b < bandCount; b++)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            if (outR[0].RasterBandsData[b][i].Equals(invalidValue) && !inR[0].RasterBandsData[b][i].Equals(invalidValue))
                            {
                                outR[0].RasterBandsData[b][i] = inR[0].RasterBandsData[b][i];
                            }
                        }
                    }
                }
                ));
            rp.Excute();
        }
        
        private IRasterDataProvider CreateMosaicFile(IRasterDataProvider copyRaster, PrjEnvelope env, string filename)
        {
            ISpatialReference spatialRef = copyRaster.SpatialRef;
            string bandNames = BandNameString(copyRaster as ILdfDataProvider);
            Size outSize = env.GetSize(copyRaster.ResolutionX, copyRaster.ResolutionY);
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + copyRaster.ResolutionX + "," + copyRaster.ResolutionY + "}",
                            "BANDNAMES="+ bandNames
                        };
            return IdentifyOutFile(filename, copyRaster.BandCount, outSize, copyRaster.DataType, options);
        }

        private string BandNameString(ILdfDataProvider fileRaster)
        {
            if (fileRaster == null)
                return null;
            string[] bandNames = (fileRaster as ILdfDataProvider).Hdr.BandNames;
            if (bandNames == null || bandNames.Length == 0)
                return null;
            string bandNameString = "";
            foreach (string b in bandNames)
            {
                bandNameString = bandNameString + b + ",";
            }
            return bandNameString.TrimEnd(',');
        }

        internal IRasterDataProvider IdentifyOutFile(string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            string dir = Path.GetDirectoryName(outfilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount,dataType, options) as IRasterDataProvider;
        }

        private PrjEnvelope CoordToEnvelope(CoordEnvelope coordEnvelope)
        {
            return new PrjEnvelope(coordEnvelope.MinX, coordEnvelope.MaxX, coordEnvelope.MinY, coordEnvelope.MaxY);
        }

        ///// <summary>
        ///// FY3A_MERSI_GBAL_L1_20120808_0000_1000M_MS.HDF
        ///// </summary>
        ///// <param name="inArg"></param>
        ///// <param name="dataIdentify"></param>
        ///// <returns></returns>
        //private string CreateMosaicFilename(MosaicInputArg inArg, DataIdentify dataIdentify, string projectionIdentify, float resolution, string station, string dayOrNight)
        //{
        //    PrjEnvelopeItem item = inArg.MosaicEnvelopes[0];
        //    return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}.ldf",
        //        dataIdentify.Satellite,
        //        dataIdentify.Sensor,
        //        item.Name,
        //        projectionIdentify,
        //        "L1",
        //        dataIdentify.OrbitDateTime.ToString("yyyyMMdd"),
        //        dayOrNight,
        //        projectionIdentify == "GLL" ? GLLResolutionIdentify(resolution) : ResolutionIdentify(resolution),
        //        station
        //        );
        //}

        private string ResolutionIdentify(float resolution)
        {
            if (resolution == 1000f)
                return "1000M";
            else if (resolution == 250f)
                return "0250M";
            else if (resolution == 500f)
                return "0500M";
            else if (resolution == 5000f)
                return "5000M";
            else if (resolution == 10000f)
                return "010KM";
            else if (resolution == 25000f)
                return "025KM";
            else if (resolution == 100000f)
                return "100KM";
            else
                return "00000";
        }

        /// <summary>
        /// 0250M、0500M、1000M、5000M、010KM、025KM、100KM、00000
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private string GLLResolutionIdentify(float resolution)
        {
            if (resolution == 0.01f)
                return "1000M";
            else if (resolution == 0.0025f)
                return "0250M";
            else if (resolution == 0.005f)
                return "0500M";
            else if (resolution == 0.05f)
                return "5000M";
            else if (resolution == 0.1f)
                return "010KM";
            else if (resolution == 0.25f)
                return "025KM";
            else if (resolution == 1.0f)
                return "100KM";
            else
                return "00000";
        }
    }
}
