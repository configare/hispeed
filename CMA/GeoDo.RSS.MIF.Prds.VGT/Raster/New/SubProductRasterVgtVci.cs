using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtVci : CmaVgtMonitoringSubProduct
    {
       private IContextMessage _contextMessage = null;

       public SubProductRasterVgtVci()
            : base()
        {
        }

       public SubProductRasterVgtVci(SubProductDef subProductDef)
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
           {
               PrintInfo("参数\"AlgorithmName\"为空。");
               return null;
           }
           string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
           if (algorith != "0VCI")
           {
               PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
               return null;
           }
           return CalcVci(algorith,progressTracker);
       }
       private IExtractResult CalcVci(string algorithmName,Action<int, string> progressTracker)
       {
           if (_argumentProvider.GetArg("mainfiles") == null)
           {
               PrintInfo("请选择NDVI数据。");
               return null;
           }
           string ndviFile = _argumentProvider.GetArg("mainfiles").ToString();
           if (!File.Exists(ndviFile))
           {
               PrintInfo("选择的数据\"" + ndviFile + "\"不存在。");
               return null;
           }

           if (_argumentProvider.GetArg("NdviCH") == null)
           {
               PrintInfo("参数\"NdviCH\"为空。");
               return null;
           }
           int ndviCH = (int)(_argumentProvider.GetArg("NdviCH"));
           if (_argumentProvider.GetArg("NdviMaxCH") == null)
           {
               PrintInfo("参数\"NdviMaxCH\"为空。");
               return null;
           }
           int ndviMaxCH = (int)(_argumentProvider.GetArg("NdviMaxCH"));
           if (_argumentProvider.GetArg("NdviMinCH") == null)
           {
               PrintInfo("参数\"NdviMinCH\"为空。");
               return null;
           }
           int ndviMinCH = (int)(_argumentProvider.GetArg("NdviMinCH"));
           if (ndviCH < 1 || ndviMaxCH < 1 || ndviMinCH < 1)
           {
               PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
               return null;
           }
           if (_argumentProvider.GetArg("resultZoom") == null)
           {
               PrintInfo("参数\"resultZoom\"为空。");
               return null;
           }
           UInt16 resultZoom = Convert.ToUInt16(_argumentProvider.GetArg("resultZoom"));
           string backFile = null;
           if (_argumentProvider.GetArg("BackFile") == null)
           {
               string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SystemData\\ndvi_0901.ldf");
               if (!File.Exists(defaultPath))
                   return null;
               backFile = defaultPath;
           }
           else
               backFile = _argumentProvider.GetArg("BackFile").ToString();
           if (backFile == ndviFile)
           {
               PrintInfo("请选择正确的背景库数据！");
               return null;
           }
           List<RasterMaper> rms = new List<RasterMaper>();
           try
           {
               IRasterDataProvider ndviRaster = RasterDataDriver.Open(ndviFile) as IRasterDataProvider;
               if (ndviRaster.BandCount < ndviCH)
               {
                   PrintInfo("");
                   return null;
               }
               RasterMaper rm = new RasterMaper(ndviRaster, new int[] { ndviCH });
               rms.Add(rm);
               IRasterDataProvider backRaster = RasterDataDriver.Open(backFile) as IRasterDataProvider;
               if (backRaster.BandCount < ndviMinCH || backRaster.BandCount < ndviMaxCH)
               {
                   PrintInfo("背景库通道设置错误，大于实际数据通道数");
                   return null;
               }
               RasterMaper bm = new RasterMaper(backRaster, new int[] { ndviMaxCH, ndviMinCH });
               rms.Add(bm);
               //输出文件准备（作为输入栅格并集处理）
               RasterIdentify ri = new RasterIdentify(ndviFile);
               ri.ProductIdentify = _subProductDef.ProductDef.Identify;
               ri.SubProductIdentify = _identify;
               string outFileName = ri.ToWksFullFileName(".dat");
               using (IRasterDataProvider outRaster = CreateOutRaster(outFileName, rms.ToArray()))
               {
                   //栅格数据映射
                   RasterMaper[] fileIns = rms.ToArray();
                   RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                   //创建处理模型
                   RasterProcessModel<short, short> rfr = null;
                   rfr = new RasterProcessModel<short, short>(progressTracker);
                   rfr.SetRaster(fileIns, fileOuts);
                   //rfr.SetTemplateAOI(aoiTemplate);
                   rfr.RegisterCalcModel(
                       new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                   {
                       if (rvInVistor[0].RasterBandsData[0]==null
                           ||rvInVistor[1].RasterBandsData[0] == null
                           ||rvInVistor[1].RasterBandsData[1]==null)
                           return;
                       int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                       for (int index = 0; index < dataLength; index++)
                       {
                           short backMax = rvInVistor[1].RasterBandsData[0][index];
                           short backMin = rvInVistor[1].RasterBandsData[1][index];
                           int divi = backMax - backMin;
                           if (divi == 0)
                               rvOutVistor[0].RasterBandsData[0][index] = 0;
                           else
                               rvOutVistor[0].RasterBandsData[0][index] = (short)((float)(rvInVistor[0].RasterBandsData[0][index] - rvInVistor[1].RasterBandsData[1][index]) / divi * resultZoom);
                       }
                   }));
                   //执行
                   rfr.Excute();
                   FileExtractResult res = new FileExtractResult(_subProductDef.Identify, outFileName, true);
                   res.SetDispaly(false);
                   return res;

               }
           }
           finally
           {
               foreach (RasterMaper rm in rms)
               {
                   rm.Raster.Dispose();
               }
           }
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
