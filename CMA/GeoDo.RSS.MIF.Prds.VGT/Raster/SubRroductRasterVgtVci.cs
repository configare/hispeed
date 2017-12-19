using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubRroductRasterVgtVci_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubRroductRasterVgtVci_old()
            : base()
        {

        }

        public SubRroductRasterVgtVci_old(SubProductDef subProductDef)
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
            if (_argumentProvider.GetArg("mainfiles") == null)
            {
                PrintInfo("请选择NDVI数据。");
                return null;
            }
            string ndviFile = _argumentProvider.GetArg("mainfiles").ToString();
            if (!File.Exists(ndviFile))
            {
                PrintInfo("选择的数据\""+ndviFile+"\"不存在。");
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
            //add at 2012.9.22,屏蔽背景库数据也选择了同样的ndvi文件所出现的错误
            if (backFile == ndviFile)
            {
                PrintInfo("请选择正确的背景库数据！");
                return null;
            }

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("mainfiles", new FilePrdMap(ndviFile, resultZoom, new VaildPra(float.MinValue, float.MaxValue), new int[] { ndviCH }));
            filePrdMap.Add("BackFile", new FilePrdMap(backFile, resultZoom, new VaildPra(float.MinValue, float.MaxValue), new int[] { ndviMaxCH, ndviMinCH }));

            //为防止用户选择的波段超出数据波段的界限
            if (filePrdMap["mainfiles"].BandCount < ndviCH || filePrdMap["BackFile"].BandCount < ndviMinCH || filePrdMap["BackFile"].BandCount < ndviMaxCH)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider prd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (prd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
            IRasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(new ArgumentProvider(prd, null));

            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("0VCI", prd.Width * prd.Height, new System.Drawing.Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(new int[] { filePrdMap["mainfiles"].StartBand,
                                                          filePrdMap["BackFile"].StartBand,
                                                          filePrdMap["BackFile"].StartBand +1},
                (idx, values) =>
                {
                    result.Put(idx, ((values[1] - values[2]) == 0 ? (Int16)0 : (Int16)((float)(values[0] - values[2]) / (values[1] - values[2]) * resultZoom)));
                }
                );
            return result;
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

