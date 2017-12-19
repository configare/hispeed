using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;


namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class SubProductFLARFIR : CmaMonitoringSubProduct
    {

        public SubProductFLARFIR(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }
        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FlareAlgorithm")
            {
                return FRAREAlgorithm(progressTracker);
            }
            return null;
        }
        private IExtractResult FRAREAlgorithm(Action<int, string> progressTracker)
        {
            AngleParModel model = _argumentProvider.GetArg("anglesettings") as AngleParModel;
            double glintmax = double.Parse(_argumentProvider.GetArg("glint").ToString());
            //band
            int angleband = (int)_argumentProvider.GetArg("angle");
            //zoom
            float anglezoom = float.Parse(_argumentProvider.GetArg("angle_Zoom").ToString());
            string rasterfile = _argumentProvider.DataProvider.fileName;
            IRasterDataProvider outRaster=null;
            List<RasterMaper> rms = new List<RasterMaper>();
            try
            {
                IRasterDataProvider sunzinRaster = RasterDataDriver.Open(model.FileAsunZ) as IRasterDataProvider;
                RasterMaper brmsunz = new RasterMaper(sunzinRaster, new int[] { angleband });
                rms.Add(brmsunz);
                IRasterDataProvider sunainRaster = RasterDataDriver.Open(model.FileAsunA) as IRasterDataProvider;
                RasterMaper brmsuna = new RasterMaper(sunainRaster, new int[] { angleband });
                rms.Add(brmsuna);
                IRasterDataProvider satzinRaster = RasterDataDriver.Open(model.FileAsatZ) as IRasterDataProvider;
                RasterMaper brmsatz = new RasterMaper(satzinRaster, new int[] { angleband });
                rms.Add(brmsatz);
                IRasterDataProvider satainRaster = RasterDataDriver.Open(model.FileAsatA) as IRasterDataProvider;
                RasterMaper brmsata = new RasterMaper(satainRaster, new int[] { angleband });
                rms.Add(brmsata);

                RasterIdentify ri = GetRasterIdentifyID(rasterfile);
                ri.SubProductIdentify = "FRAM";
                string outFileName = ri.ToWksFullFileName(".dat");
                IPixelIndexMapper result = null;
                IPixelFeatureMapper<Int16> resultTag = null;
                int totalDatalength = 0;
                Dictionary<int, FireAngleFeature> listfeature = new Dictionary<int, FireAngleFeature>(); 
                outRaster = CreateOutRaster(outFileName, rms.ToArray());
                    result = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", outRaster.Width, outRaster.Height, outRaster.CoordEnvelope, outRaster.SpatialRef);
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<Int16, UInt16> rfr = null;
                    rfr = new RasterProcessModel<Int16, UInt16>(progressTracker);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<Int16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;

                        for (int index = 0; index < dataLength; index++)
                        {
                            double asunz = rvInVistor[0].RasterBandsData[0][index] / anglezoom;
                            double asuna = rvInVistor[1].RasterBandsData[0][index] / anglezoom;
                            double asatz = rvInVistor[2].RasterBandsData[0][index] / anglezoom;
                            double asata = rvInVistor[3].RasterBandsData[0][index] / anglezoom;
                            double glintangle = Math.Acos(Math.Sin(asunz) * Math.Sin(asatz) * Math.Cos(asuna - asata) + Math.Cos(asunz) * Math.Cos(asatz));
                            if (glintangle != 0)
                            {
                                if (glintangle * 180 / Math.PI < glintmax)
                                {
                                    result.Put(totalDatalength + index);
                                    
                                }
                                //增加像元信息显示
                                FireAngleFeature feature = new FireAngleFeature();
                                feature.SunZ = asunz;
                                feature.SunA = asuna;
                                feature.SatZ = asatz;
                                feature.SatA = asata;
                                feature.Glint = Math.Round(glintangle * 180 / Math.PI,2);
                                listfeature.Add(totalDatalength + index, feature);

                            }
                            rvOutVistor[0].RasterBandsData[0][index] = Convert.ToUInt16(glintangle * 180 * anglezoom / Math.PI);
                          
                        }
                        totalDatalength += dataLength;

                    }));
                    //执行
                    rfr.Excute();
                    result.Tag = new FireAngleCollection("耀斑角信息", listfeature);
                    IExtractResultArray array = new ExtractResultArray("FIR");
                    array.Add(result);
                    FileExtractResult angleresult = new FileExtractResult(ri.SubProductIdentify, outFileName);
                    angleresult.SetDispaly(false);
                    array.Add(angleresult);

                    return array;
                }
            finally
            {
                foreach (RasterMaper rm in rms)
                {
                    rm.Raster.Dispose();
                }
                if(outRaster!=null)
                {
                    outRaster.Dispose();
                }
                
            }
        }
        private bool CheckArgs()
        {
            if (_argumentProvider.GetArg("anglesettings") == null)
            {
                return false;
            }
            if (_argumentProvider.GetArg("glint") == null)
            {
                return false;
            }
            if (_argumentProvider.DataProvider == null)
            {
                return false;
            }
            //band info
            if (_argumentProvider.GetArg("angle")==null)
            {
                return false;
            }
            
            // ban zoom info
            if (_argumentProvider.GetArg("angle_Zoom") == null)
            {
                return false;
            }
            
            
            return true;
        }
        private RasterIdentify GetRasterIdentifyID(string rasterfile)
        {
            RasterIdentify rst = new RasterIdentify(rasterfile);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;
            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();

            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }
        private IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

    }
}
