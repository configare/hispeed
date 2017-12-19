using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.Project;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RasterProject;
using GeoDo.RSS.UI.AddIn.DataPro;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    /// <summary>
    /// 积雪网络二值图
    /// 1、识别区域，然后根据区域设置输出数据范围。
    /// </summary>
    public class SubProductNIMGSNW : CmaMonitoringSubProduct
    {
        public SubProductNIMGSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NIMGAlgorithm")
            {
                return IMGAlgorithm();
            }
            return null;
        }

        private IExtractResult IMGAlgorithm()
        {
            object obj = _argumentProvider.GetArg("ThemeGraphyGenerator");
            if (obj == null)
                return null;
            IThemeGraphGenerator tgg = obj as IThemeGraphGenerator;
            if (tgg == null)
                return null;
            string aoiTemplateName = string.Empty;
            Dictionary<string, int[]> aoi = null;
            GetAOI(out aoiTemplateName, out aoi);
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string templatName = GetStringArgument("ThemeGraphTemplateName");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            foreach (string item in files)
            {
                if (!File.Exists(item))
                    return null;
            }
            //计算网络二值图dat文件
            string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\RasterTemplate\\China_LandRaster.dat";
            if (!File.Exists(dir))
                return null;
            string datFileName = GenerateNetImageFile(dir, files[0]);
            if (string.IsNullOrEmpty(datFileName))
                return null;
            string colorTabelName = GetColorTableName("colortablename");
            if (outFileIdentify == "NSNI")  //需要限定输出范围
            {
                RasterIdentify rstId = new RasterIdentify(files[0]);
                if (!string.IsNullOrWhiteSpace(rstId.RegionIdentify))
                {
                    DefinedRegionParse reg = new DefinedRegionParse();
                    BlockItemGroup blockGroup = reg.BlockDefined.FindGroup("积雪");
                    PrjEnvelopeItem envItem = blockGroup.GetPrjEnvelopeItem(rstId.RegionIdentify);
                    if (envItem != null)
                    {
                        RasterProject.PrjEnvelope prjEnvelope = RasterProject.PrjEnvelope.CreateByCenter(envItem.PrjEnvelope.CenterX, envItem.PrjEnvelope.CenterY, 10, 10);
                        (tgg as CmaThemeGraphGenerator).SetEnvelope(new Layout.GxdEnvelope(prjEnvelope.MinX, prjEnvelope.MaxX, prjEnvelope.MinY, prjEnvelope.MaxY));
                        (tgg as ThemeGraphGenerator).IsFitToTemplateWidth = false;
                    }
                }
            }
            tgg.Generate(datFileName, templatName, null, null, outFileIdentify, colorTabelName);
            string resultFilename = tgg.Save();
            if (string.IsNullOrEmpty(resultFilename))
                return null;
            return new FileExtractResult(outFileIdentify, resultFilename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rasterFile">土地利用类型文件</param>
        /// <param name="dblvFile">二值图文件</param>
        /// <returns></returns>
        private unsafe string GenerateNetImageFile(string rasterFile, string dblvFile)
        {
            using (IRasterDataProvider rasterprd = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                using (IRasterDataProvider dblvprd = GeoDataDriver.Open(dblvFile) as IRasterDataProvider)
                {
                    //step1:将文件转换为类型一致
                    IRasterDataProviderConverter converter = new RasterDataProviderConverter();
                    string dstFileName = MifEnvironment.GetFullFileName("1.dat");
                    using (IRasterDataProvider dstDataProvider = converter.ConvertDataType<UInt16, Byte>(dblvprd, enumDataType.Byte, dstFileName,
                        (v) => { return (Byte)v; }))
                    {
                        //step2:裁切文件以保证大小一致
                        using (IRasterDataProvider sameSizeDataProvider = GetSameSizeDataProvider(rasterprd, dstDataProvider))
                        {
                            byte[] dataBlock = new byte[sameSizeDataProvider.Width * sameSizeDataProvider.Height];
                            fixed (byte* buffer = dataBlock)
                            {
                                IntPtr ptr = new IntPtr(buffer);
                                sameSizeDataProvider.GetRasterBand(1).Read(0, 0, sameSizeDataProvider.Width, sameSizeDataProvider.Height, ptr, enumDataType.Byte, sameSizeDataProvider.Width, sameSizeDataProvider.Height);
                                sameSizeDataProvider.Read(0, 0, sameSizeDataProvider.Width, sameSizeDataProvider.Height, ptr, enumDataType.Byte, sameSizeDataProvider.Width, sameSizeDataProvider.Height, 1, new int[] { 1 }, enumInterleave.BSQ);
                            }
                            if (sameSizeDataProvider == null)
                                return null;
                            //step3:生成虚拟文件
                            VirtualRasterDataProvider vrd = new VirtualRasterDataProvider(new IRasterDataProvider[] { sameSizeDataProvider, dstDataProvider });
                            //step4:遍历虚拟文件生成结果
                            ArgumentProvider ap = new ArgumentProvider(vrd, null);
                            RasterPixelsVisitor<Byte> visitor = new RasterPixelsVisitor<Byte>(ap);
                            //生成结果
                            MemPixelFeatureMapper<Byte> memresult = new MemPixelFeatureMapper<Byte>("NIMG", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                            visitor.VisitPixel(new int[] { 1, 2 },
                                (index, values) =>
                                {
                                    if (values[0] == 0)
                                        memresult.Put(index, 0);
                                    else if (values[1] == 0)
                                        memresult.Put(index, 100);
                                    else if (values[1] == 1)
                                        memresult.Put(index, (byte)(100 + values[0]));
                                });
                            //保存文件
                            RasterIdentify idNIMG = new RasterIdentify(dblvFile);
                            idNIMG.ThemeIdentify = "CMA";
                            idNIMG.ProductIdentify = "SNW";
                            idNIMG.SubProductIdentify = "NIMG";
                            idNIMG.GenerateDateTime = DateTime.Now;
                            idNIMG.Satellite = dblvprd.DataIdentify.Satellite;
                            idNIMG.Sensor = dblvprd.DataIdentify.Sensor;
                            string fileName = MifEnvironment.GetFullFileName(idNIMG.ToLongString() + ".dat");
                            if (File.Exists(fileName))
                                idNIMG.GenerateDateTime = DateTime.Now.AddSeconds(1);
                            IInterestedRaster<Byte> iirNIMG = new InterestedRaster<Byte>(idNIMG, new Size(sameSizeDataProvider.Width, sameSizeDataProvider.Height), sameSizeDataProvider.CoordEnvelope.Clone(), sameSizeDataProvider.SpatialRef);
                            iirNIMG.Put(memresult);
                            iirNIMG.Dispose();
                            if (File.Exists(dstFileName))
                                File.Delete(dstFileName);
                            return iirNIMG.FileName;
                        }
                    }
                }
            }
        }

        private unsafe IRasterDataProvider GetSameSizeDataProvider(IRasterDataProvider bigDataProvider, IRasterDataProvider smallDataProvider)
        {
            double minX = smallDataProvider.CoordEnvelope.MinX;
            double maxX = smallDataProvider.CoordEnvelope.MaxX;
            double minY = smallDataProvider.CoordEnvelope.MinY;
            double maxY = smallDataProvider.CoordEnvelope.MaxY;
            double bigDataMinX = bigDataProvider.CoordEnvelope.MinX;
            double bigDataMaxX = bigDataProvider.CoordEnvelope.MaxX;
            double bigDataMinY = bigDataProvider.CoordEnvelope.MinY;
            double bigDataMaxY = bigDataProvider.CoordEnvelope.MaxY;
            if (maxY > bigDataMaxY || minY < bigDataMinY || maxX > bigDataMaxX || minX < bigDataMinX)
                return null;
            int beginRow = (int)((bigDataProvider.CoordEnvelope.MaxY - maxY) / bigDataProvider.ResolutionY);
            int beginCol = (int)((minX - bigDataProvider.CoordEnvelope.MinX) / bigDataProvider.ResolutionX);
            int rows = (int)Math.Round(((maxY - minY) / bigDataProvider.ResolutionY), 0);
            int cols = (int)Math.Round(((maxX - minX) / bigDataProvider.ResolutionX), 0);
            byte[] dataBlock = new byte[smallDataProvider.Width * smallDataProvider.Height];
            string dstFileName = MifEnvironment.GetFullFileName("1.ldf"); 
            fixed (byte* buffer = dataBlock)
            {
                IntPtr ptr = new IntPtr(buffer);
                bigDataProvider.GetRasterBand(1).Read(beginCol, beginRow, cols, rows, ptr, enumDataType.Byte, smallDataProvider.Width, smallDataProvider.Height);
                using (IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver)
                {
                    IRasterDataProvider prdWriter = drv.Create(dstFileName, smallDataProvider.Width, smallDataProvider.Height, 1,
                           enumDataType.Byte, "INTERLEAVE=BSQ", "VERSION=LDF", "WITHHDR=TRUE", "SPATIALREF=" + GetSpatialRefString(bigDataProvider),
                           GetMapInfoString(smallDataProvider.CoordEnvelope, smallDataProvider.Width, smallDataProvider.Height)) as IRasterDataProvider;
                    IRasterBand band = prdWriter.GetRasterBand(1);
                    band.Write(0, 0, band.Width, band.Height, ptr, enumDataType.Byte, band.Width, band.Height);
                    return prdWriter;
                }
            }
        }

        private string GetSpatialRefString(IRasterDataProvider srcDataProvider)
        {
            return srcDataProvider.SpatialRef != null ? srcDataProvider.SpatialRef.ToProj4String() : string.Empty;
        }

        private string GetMapInfoString(CoordEnvelope coordEnvelope, int width, int height)
        {
            return coordEnvelope != null ? coordEnvelope.ToMapInfoString(new Size(width, height)) : string.Empty;
        }
    }
}
