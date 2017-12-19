using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductTHANBAG : CmaMonitoringSubProduct
    {
        public SubProductTHANBAG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "THANExtract")
            {
                return THANExtract(progressTracker);
            }
            return null;
        }

        private IExtractResult THANExtract(Action<int, string> progressTracker)
        {
            object obj = _argumentProvider.GetArg("ucAnlysisTool");
            UCAnlysisTool ucAnlysisTool = null;
            if (obj != null)
            {
                ucAnlysisTool = obj as UCAnlysisTool;
                ucAnlysisTool.btnGetInfos_Click(null, null);
            }
            return null;
        }

        public override IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker)
        {
            object obj = _argumentProvider.GetArg("ucAnlysisTool");
            UCAnlysisTool ucAnlysisTool = null;
            if (obj != null)
                ucAnlysisTool = obj as UCAnlysisTool;
            else
                return null;
            IRasterDataProvider sourceraster = _argumentProvider.DataProvider as IRasterDataProvider;
            RasterIdentify rid = new RasterIdentify(_argumentProvider.DataProvider.fileName);
            rid.ProductIdentify = _subProductDef.ProductDef.Identify;
            rid.SubProductIdentify = _subProductDef.Identify;
            string tempFile = rid.ToWksFullFileName(".dat");
            string outFileName = Path.Combine(Path.GetDirectoryName(tempFile), Path.GetFileNameWithoutExtension(tempFile) + GetExtInfo(ucAnlysisTool.cmbType.Text) + ".ldf");
            IExtractResultArray array = new ExtractResultArray("BAG");
            bool onlyTxt = bool.Parse(_argumentProvider.GetArg("OnlyTxt").ToString());
            if (!onlyTxt)
            {
                int totalLength = 0;
                //原始影像raster
                List<RasterMaper> listRaster = new List<RasterMaper>();
                RasterMaper rmsoure = new RasterMaper(sourceraster, GetBandArray(sourceraster.BandCount));
                int totalbandcount = sourceraster.BandCount;
                listRaster.Add(rmsoure);
                try
                {
                    using (IRasterDataProvider outRaster = CreateOutM_BandRaster(outFileName, listRaster.ToArray(), totalbandcount))
                    {
                        //波段总数
                        RasterMaper[] fileIns = listRaster.ToArray();
                        RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, GetBandArray(totalbandcount)) };
                        //创建处理模型
                        RasterProcessModel<UInt16, UInt16> rfr = null;
                        rfr = new RasterProcessModel<UInt16, UInt16>(progressTracker);
                        rfr.SetRaster(fileIns, fileOuts);
                        rfr.SetFeatureAOI(_argumentProvider.AOIs);
                        rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                        {
                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                            if (rvInVistor[0].RasterBandsData == null || rvInVistor[0].RasterBandsData[0] == null)
                            {
                                totalLength += dataLength;
                                return;
                            }
                            for (int index = 0; index < dataLength; index++)
                            {
                                if (_argumentProvider.AOI.Contains(totalLength + index))
                                    for (int i = 0; i < totalbandcount; i++)
                                        rvOutVistor[0].RasterBandsData[i][index] = rvInVistor[0].RasterBandsData[i][index];
                            }
                            totalLength += dataLength;
                        }
                        ));
                        //执行
                        rfr.Excute(0);
                    }
                    string dstfilename = outFileName.Replace(".ldf", ".dat");
                    if (File.Exists(dstfilename))
                        File.Delete(dstfilename);
                    File.Move(outFileName, dstfilename);
                    FileExtractResult res = new FileExtractResult("BAG", dstfilename, true);
                    res.SetDispaly(false);
                    array.Add(res);
                }
                finally
                {

                }
            }
            if (!string.IsNullOrEmpty(ucAnlysisTool.txtInfos.Text))
            {
                string txtFile = outFileName.Replace(".ldf", ".txt");
                File.WriteAllLines(txtFile, ucAnlysisTool.WriteText, Encoding.Unicode);
                FileExtractResult resTxt = new FileExtractResult("BAG", txtFile, true);
                resTxt.SetDispaly(false);
                array.Add(resTxt);
            }
            return array;

        }

        private string GetExtInfo(string extInfo)
        {
            if (string.IsNullOrEmpty(extInfo))
                return string.Empty;
            if (extInfo.StartsWith("_"))
                return extInfo;
            else
                return "_" + extInfo;
        }

        private int[] GetBandArray(int bandcount)
        {
            List<int> listband = new List<int>();
            for (int i = 1; i <= bandcount; i++)
            {
                listband.Add(i);
            }
            return listband.ToArray();
        }

        protected IRasterDataProvider CreateOutM_BandRaster(string outFileName, RasterMaper[] inrasterMaper, int bandcount)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            string[] optionString = new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=LDF",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef==null?"":("SPATIALREF=" +inrasterMaper[0].Raster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + outEnv.MinX + "," + outEnv.MaxY + "}:{" + resX + "," + resY + "}"
                    };
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, bandcount, enumDataType.UInt16, optionString) as RasterDataProvider;
            return outRaster;
        }

    }
}
