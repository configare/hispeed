#region Version Info
/*========================================================================
* 功能概述：海冰厚度子产品
* 
* 创建者：DongW     时间：2013/11/20 14:03:03
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
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    /// <summary>
    /// 类名：SubProduct0SITICE
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/11/20 14:03:03
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SubProduct0SITICE:CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;

        public SubProduct0SITICE(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
            if (_argumentProvider == null || _argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "IceThinknessAlgorithm")
            {
                return IceThinknessAlgorithm(progressTracker);
            }
            return null;
        }

        private IExtractResult IceThinknessAlgorithm(Action<int, string> progressTracker)
        {
            IRasterDataProvider dataPrd = _argumentProvider.DataProvider;
            if (dataPrd == null)
                return null;
            if (_argumentProvider.GetArg("DBLVFile") == null)
                return null;
            string dblvFile = _argumentProvider.GetArg("DBLVFile").ToString();
            if (string.IsNullOrEmpty(dblvFile) ||!File.Exists(dblvFile))
                return null;
            IRasterDataProvider dblvRaster=GeoDataDriver.Open(dblvFile) as IRasterDataProvider;
            float t1 = Obj2Float(_argumentProvider.GetArg("T1"));
            float t2 = Obj2Float(_argumentProvider.GetArg("T2"));
            float t3 = Obj2Float(_argumentProvider.GetArg("T3"));
            if (float.IsNaN(t1) || float.IsNaN(t2) || float.IsNaN(t3))
            {
                PrintInfo("获取计算参数失败。");
                return null;
            }
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            int bandNo = TryGetBandNo(bandNameRaster, "FarInfrared");
            if (bandNo == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            string[] aois = _argumentProvider.GetArg("AOITemplate") as string[];
            string aoiTemplate = (aois == null || aois.Length == 0) ? null : aois[0];
            //输入文件准备
            List<RasterMaper> rms = new List<RasterMaper>();
            RasterMaper rm = new RasterMaper(dataPrd, new int[] { bandNo });
            RasterMaper dblvRm = new RasterMaper(dblvRaster, new int[] { 1 });
            rms.Add(rm);
            rms.Add(dblvRm);
            //创建结果数据
            using (IRasterDataProvider outRaster = CreateOutRaster(rm))
            {
                //栅格数据映射
                RasterMaper[] fileIns = rms.ToArray();
                RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                //创建处理模型
                RasterProcessModel<short, short> rfr = null;
                rfr = new RasterProcessModel<short, short>(progressTracker);
                rfr.SetRaster(fileIns, fileOuts);
                rfr.SetTemplateAOI("vector:海陆模版_反");
                rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                {
                    if (rvInVistor[0].RasterBandsData[0] != null)
                    {
                        if (aoi != null && aoi.Length != 0)
                        {
                            int index;
                            for (int i = 0; i < aoi.Length; i++)
                            {
                                index = aoi[i];
                                if (rvInVistor[1].RasterBandsData[0][index] == 1)
                                {
                                    short value = rvInVistor[0].RasterBandsData[0][index];
                                    if (value > t1)
                                        rvOutVistor[0].RasterBandsData[0][index] = -1;
                                    else
                                    {
                                        if (value > t2)
                                            rvOutVistor[0].RasterBandsData[0][index] = 1;
                                        else if (value > t3)
                                            rvOutVistor[0].RasterBandsData[0][index] = 5;
                                        else
                                            rvOutVistor[0].RasterBandsData[0][index] = 10;
                                    }
                                }
                            }
                        }
                    }
                }));
                //执行
                rfr.Excute(Int16.MinValue);
                FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outRaster.fileName, true);
                res.SetDispaly(false);
                return res;
            }
        }

        private IRasterDataProvider CreateOutRaster(RasterMaper rm)
        {
            RasterIdentify ri = new RasterIdentify(rm.Raster.fileName);
            ri.ProductIdentify = _subProductDef.ProductDef.Identify;
            ri.SubProductIdentify = _subProductDef.Identify;
            string outFileName = ri.ToWksFullFileName(".dat");
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = rm.Raster.CoordEnvelope;
            string mapInfo = outEnv.ToMapInfoString(new Size(rm.Raster.Width, rm.Raster.Height));
            RasterDataProvider outRaster = raster.Create(outFileName, rm.Raster.Width, rm.Raster.Height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }

        private float Obj2Float(object v)
        {
            if (v == null || v.ToString() == string.Empty)
                return float.NaN;
            return (float)v;
        }
    }
}
