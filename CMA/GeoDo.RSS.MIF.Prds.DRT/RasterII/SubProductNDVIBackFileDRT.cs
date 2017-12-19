using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductNDVIBackFileDRT : CmaDrtMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductNDVIBackFileDRT()
            : base()
        {
        }

        public SubProductNDVIBackFileDRT(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "NDVIBackAlgorithm")
            {
                return BackNDVIFileAlg(progressTracker);
                
            }
            return null;
        }
       
        //生成NDVI背景库算法 
        private IExtractResult BackNDVIFileAlg(Action<int, string> progressTracker)
        {
            //此处已从theme中读取配置文件
            ///参数提取
            int NDVIBandCH = (int)_argumentProvider.GetArg("NDVIBand");
            //NDVI有效值范围
            Int16 ndviVaildMin = Convert.ToInt16(_argumentProvider.GetArg("VaildRegionMin"));//-------如果有效值是范围是放大之前的范围（确定）这样是否能取到？
            Int16 ndviVaildMax = Convert.ToInt16(_argumentProvider.GetArg("VaildRegionMax"));
            //NDVI数据放大倍数
            double NDVIZoom = Convert.ToDouble(_argumentProvider.GetArg("NDVIZoom"));
            //NDVI生成的背景库放大倍数
            double NDVIBackZoom = Convert.ToDouble(_argumentProvider.GetArg("OutNDVIBackZoom"));
            //NDVI输入文件
            string[] ndviFileNames = GetStringArray("NDVIFile");
            Int16[] cloudyValues = GetUnValueValues("CloudyValue");
            Int16[] waterValues = GetUnValueValues("WaterValue");
            List<Int16> specialValues = new List<short>();
            specialValues.AddRange(cloudyValues);
            specialValues.AddRange(waterValues);
            Int16 defCloudy = Convert.ToInt16(_argumentProvider.GetArg("defCloudy"));
            
            //NDVI输出文件--根据输入文件信息输出
            string outndvibackfile = GetFileName(ndviFileNames, _subProductDef.ProductDef.Identify, _identify, ".ldf", null);//默认位置输出
            List<RasterMaper> rasterInputMaps = new List<RasterMaper>();//从配置文件中读取需要待合成数据
            foreach(string itemfile in ndviFileNames)
            {
                IRasterDataProvider inraster = RasterDataDriver.Open(itemfile) as IRasterDataProvider;
                rasterInputMaps.Add(new RasterMaper(inraster,new int[]{NDVIBandCH}));
            }
            IRasterDataProvider outNDVIbackRaster = null;
            try
            {
                outNDVIbackRaster = CreateOutLDFRaster(outndvibackfile, rasterInputMaps.ToArray(),7);//固定七个波段
                //栅格数据映射
                RasterMaper[] fileIns = rasterInputMaps.ToArray();
                RasterMaper[] fileOuts;

                fileOuts = new RasterMaper[] { new RasterMaper(outNDVIbackRaster, new int[] { 1,2,3,4,5,6,7 }) };//输出为固定7个波段数据
                //创建处理模型
                RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(progressTracker);
                rfr.SetRaster(fileIns, fileOuts);
                rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                {
                    int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;//输出格点
                    for (int index = 0; index < dataLength; index++)
                    {

                        try
                        {
                            List<Int16> listtemp = new List<Int16>();
                            List<Int16> listavg = new List<short>();
                            //初始化各波段原始数据
                            Int16 maxNDVI = Int16.MinValue;
                            Int16 minNDVI = Int16.MinValue;
                            Int16 secmaxNDVI = Int16.MinValue;
                            Int16 secminNDVI = Int16.MinValue;
                            Int16 maxvableNDVI = Int16.MinValue;
                            Int16 minvableNDVI = Int16.MinValue;
                            double averageNDVI = Int16.MinValue;
                            for (int fileindex = 0; fileindex < rvInVistor.Length; fileindex++)
                            {
                                listtemp.Add(rvInVistor[fileindex].RasterBandsData[0][index]);
                                listavg.Add(rvInVistor[fileindex].RasterBandsData[0][index]);
                            }
                            //调整排序
                            listtemp.Sort();//按照从小到大升序排列
                            listtemp = listtemp.Distinct().ToList();//去重
                            if (listtemp.Count == 1 && listtemp[0] == 0)//0
                            {
                                maxNDVI = 0;
                                minNDVI = 0;
                                secmaxNDVI = 0;
                                secminNDVI = 0;
                                maxvableNDVI = 0;
                                minvableNDVI = 0;
                                averageNDVI = 0;
                            }
                            // 云水判识
                            int cloudcount = listtemp.Count(num => cloudyValues.Contains(num));//获取该集合中标识云的元素个数
                            int watercount = listtemp.Count(num => waterValues.Contains(num));//获取该集合中标识水的元素个数

                            if (cloudcount == listtemp.Count) //全是云值
                            {
                                maxNDVI = defCloudy;
                                minNDVI = defCloudy;
                                secmaxNDVI = defCloudy;
                                secminNDVI = defCloudy;
                                maxvableNDVI = defCloudy;
                                minvableNDVI = defCloudy;
                                averageNDVI = defCloudy;
                            }
                            else if (watercount == listtemp.Count)//全是水值
                            {
                                if (waterValues.Length > 0)
                                {

                                }
                                maxNDVI = waterValues[0];
                                minNDVI = waterValues[0];
                                secmaxNDVI = waterValues[0];
                                secminNDVI = waterValues[0];
                                maxvableNDVI = waterValues[0];
                                minvableNDVI = waterValues[0];
                                averageNDVI = waterValues[0];
                            }
                            else if (cloudcount + watercount == listtemp.Count) //云+水 初始化默认值不用修改
                            {
                                //填充云 
                                maxNDVI = defCloudy;
                                minNDVI = defCloudy;
                                secmaxNDVI = defCloudy;
                                secminNDVI = defCloudy;
                                maxvableNDVI = defCloudy;
                                minvableNDVI = defCloudy;
                                averageNDVI = defCloudy;
                            }
                            else //正常条件数据下
                            {
                               
                                //排除掉云水值

                                listtemp = listtemp.Where(num => !specialValues.Contains(num)).ToList();
                                maxNDVI = listtemp[listtemp.Count - 1];// 最大值
                                minNDVI = listtemp[0];//最小值
                                secmaxNDVI = listtemp.Count > 1 ? listtemp[listtemp.Count - 2] : listtemp[listtemp.Count - 1];//次大值 
                                secminNDVI = listtemp.Count > 1 ? listtemp[1] : listtemp[0];//次小值
                                //去除云值水值 限定有效区间 取平均值
                                averageNDVI=listavg.Where(num => !specialValues.Contains(num))
                                                    .Where(num => (num >= ndviVaildMin * NDVIZoom && num <= ndviVaildMax * NDVIZoom))
                                                    .Average(num => (int)num);//平均值
                               
                                //排除掉有效区间之外的数据
                                listtemp = listtemp.Where(num => (num >= ndviVaildMin * NDVIZoom && num <= ndviVaildMax * NDVIZoom)).ToList();//实际值比较
                                maxvableNDVI = listtemp.Count > 0 ? listtemp[listtemp.Count - 1] : Int16.MinValue;//理论最大值
                                minvableNDVI = listtemp.Count > 0 ? listtemp[0] : Int16.MinValue;//理论最小值
                            }
                            //给输出Raster填值，此处为固定七个波段
                            //最大值波段
                            rvOutVistor[0].RasterBandsData[0][index] = (Int16)(maxNDVI * NDVIBackZoom / NDVIZoom);
                            //最小值波段
                            rvOutVistor[0].RasterBandsData[1][index] = (Int16)(minNDVI * NDVIBackZoom / NDVIZoom);
                            //次大值波段
                            rvOutVistor[0].RasterBandsData[2][index] = (Int16)(secmaxNDVI * NDVIBackZoom / NDVIZoom);
                            //次小值波段
                            rvOutVistor[0].RasterBandsData[3][index] = (Int16)(secminNDVI * NDVIBackZoom / NDVIZoom);
                            //理论最大值波段
                            rvOutVistor[0].RasterBandsData[4][index] = (Int16)(maxvableNDVI * NDVIBackZoom / NDVIZoom);
                            //理论最小值波段
                            rvOutVistor[0].RasterBandsData[5][index] = (Int16)(minvableNDVI * NDVIBackZoom / NDVIZoom);
                            //平均值波段
                            rvOutVistor[0].RasterBandsData[6][index] = (Int16)(averageNDVI * NDVIBackZoom / NDVIZoom);
                        }
                        catch (Exception ex)
                        {
                            string err = ex.StackTrace;
                            string errmsg = err;
                        }
                    }
                   
                }));
                rfr.Excute();
                //输出到界面
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outndvibackfile, true);
                res.SetDispaly(false);
                return res;

            }
            catch (Exception ex)
            {
                PrintInfo("NDVI背景库生产出错" + ex.StackTrace);
                return null;
            }
            finally
            {
               
                if (outNDVIbackRaster != null)
                    outNDVIbackRaster.Dispose();
               
            }
        }
        /// <summary>
        /// 获取云值水值参数
        /// </summary>
        /// <param name="argumentName"></param>
        /// <returns></returns>
        protected short[] GetUnValueValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<short> values = new List<short>();
                    short value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 创建LDFRaster
        /// </summary>
        /// <param name="outFileName">LDF文件名称</param>
        /// <param name="inrasterMaper">输入文件参考</param>
        /// <param name="outbandcount">输出文件波段</param>
        /// <returns></returns>
        protected IRasterDataProvider CreateOutLDFRaster(string outFileName, RasterMaper[] inrasterMaper,int outbandcount)
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
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, outbandcount, enumDataType.Int16, optionString) as RasterDataProvider;
            return outRaster;
        }
        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}