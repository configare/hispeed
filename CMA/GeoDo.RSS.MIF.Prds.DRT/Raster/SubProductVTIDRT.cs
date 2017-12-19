using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class SubProductVTIDRT : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        //private string _errorStr = "";
        private IArgumentProvider _curArguments = null;
        //private IPixelFeatureMapper<Int16> _result = null;

        public SubProductVTIDRT(SubProductDef subProductDef)
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

            //public override IExtractResult Make(Action<int, string> progressTracker)
            //{
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "NDVILSTAlgorithm")
            {
                return VTIAlgorithm();
            }
            return null;
        }

        private IExtractResult VTIAlgorithm()
        {
            int NDVIBandCH = (int)_curArguments.GetArg("NDVIBand");
            int NDVIBackBandMinCH = (int)_curArguments.GetArg("NDVIBackBandMin");
            int NDVIBackBandMaxCH = (int)_curArguments.GetArg("NDVIBackBandMax");

            int LSTBandCH = (int)_curArguments.GetArg("LSTBand");
            int LSTBackBandMinCH = (int)_curArguments.GetArg("LSTBackBandMin");
            int LSTBackBandMaxCH = (int)_curArguments.GetArg("LSTBackBandMax");

            double NDVIZoom = (float)_curArguments.GetArg("NDVIZoom");
            double NDVIBackZoom = (float)_curArguments.GetArg("NDVIBackZoom");

            double LSTZoom = (float)_curArguments.GetArg("LSTZoom");
            double LSTBackZoom = (float)_curArguments.GetArg("LSTBackZoom");

            float ndviVaildMin = (float)_curArguments.GetArg("ndviVaildMin");
            float ndviVaildMax = (float)_curArguments.GetArg("ndviVaildMax");

            float lstVaildMin = (float)_curArguments.GetArg("lstVaildMin");
            float lstVaildMax = (float)_curArguments.GetArg("lstVaildMax");

            double VTIZoom = (float)_curArguments.GetArg("VTIZoom");
            string errorStr = null;
            if (NDVIBandCH == -1 || NDVIBackBandMinCH == -1 || NDVIBackBandMaxCH == -1 || LSTBandCH == -1 || LSTBackBandMinCH == -1 || LSTBackBandMaxCH == -1 ||
                _curArguments.GetArg("NDVIFile") == null || _curArguments.GetArg("NDVIBackFile") == null ||
                 _curArguments.GetArg("LSTFile") == null || _curArguments.GetArg("LSTBackFile") == null)
            {
                errorStr = "VTI生产所用文件或通道未设置完全,请检查!";
                return null;
            }
            string[] ndviFileNames = GetStringArray("NDVIFile");
            string[] lstFileNames = GetStringArray("LSTFile");


            string NDVIFile = ndviFileNames == null || ndviFileNames.Length == 1 ? _curArguments.GetArg("NDVIFile").ToString() : MAxValue(ndviFileNames, NDVIBandCH, (float)NDVIZoom);
            if (ndviFileNames != null && ndviFileNames.Length != 1)
                NDVIBandCH = 1;
            string NDVIBackFile = _curArguments.GetArg("NDVIBackFile").ToString();
            string LSTFile = lstFileNames == null || lstFileNames.Length == 1 ? _curArguments.GetArg("LSTFile").ToString() : MAxValue(lstFileNames, LSTBandCH, (float)LSTZoom);
            if (lstFileNames != null && lstFileNames.Length != 1)
                LSTBandCH = 1;
            string LSTBackFile = _curArguments.GetArg("LSTBackFile").ToString();

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("NDVIFile", new FilePrdMap(NDVIFile, NDVIZoom, new VaildPra(ndviVaildMin, ndviVaildMax), new int[] { NDVIBandCH }));
            filePrdMap.Add("NDVIBackFile", new FilePrdMap(NDVIBackFile, NDVIBackZoom, new VaildPra(ndviVaildMin, ndviVaildMax), new int[] { NDVIBackBandMinCH, NDVIBackBandMaxCH }));
            filePrdMap.Add("LSTFile", new FilePrdMap(LSTFile, LSTZoom, new VaildPra(lstVaildMin, lstVaildMax), new int[] { LSTBandCH }));
            filePrdMap.Add("LSTBackFile", new FilePrdMap(LSTBackFile, LSTBackZoom, new VaildPra(lstVaildMin, lstVaildMax), new int[] { LSTBackBandMinCH, LSTBackBandMaxCH }));

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = null;
            try
            {
                vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                if (vrd == null)
                    throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("0VTI", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                float vci = 0f;
                float tci = 0f;
                rpVisitor.VisitPixel(new int[] { filePrdMap["NDVIFile"].StartBand,
                                                 filePrdMap["NDVIBackFile"].StartBand,
                                                 filePrdMap["NDVIBackFile"].StartBand+1,
                                                 filePrdMap["LSTFile"].StartBand,
                                                 filePrdMap["LSTBackFile"].StartBand,
                                                 filePrdMap["LSTBackFile"].StartBand+1},
                    (index, values) =>
                    {
                        if (values[0] < filePrdMap["NDVIFile"].VaildValue.Min || values[0] > filePrdMap["NDVIFile"].VaildValue.Max ||
                            values[1] < filePrdMap["NDVIBackFile"].VaildValue.Min || values[1] > filePrdMap["NDVIBackFile"].VaildValue.Max ||
                            values[2] < filePrdMap["NDVIBackFile"].VaildValue.Min || values[2] > filePrdMap["NDVIBackFile"].VaildValue.Max ||
                            values[3] < filePrdMap["LSTFile"].VaildValue.Min || values[3] > filePrdMap["LSTFile"].VaildValue.Max ||
                            values[4] < filePrdMap["LSTBackFile"].VaildValue.Min || values[4] > filePrdMap["LSTBackFile"].VaildValue.Max ||
                            values[5] < filePrdMap["LSTBackFile"].VaildValue.Min || values[5] > filePrdMap["LSTBackFile"].VaildValue.Max)
                            result.Put(index, 0);
                        else
                        {
                            //计算VCI
                            vci = (values[0] - values[1]) / (values[2] - values[1]);
                            //计算TCI
                            tci = (values[3] - values[4]) / (values[5] - values[4]);
                            result.Put(index, (Int16)((0.5 * vci + 0.5 * tci) * VTIZoom));
                        }
                    });
                return result;
            }
            finally
            {
                if (vrd != null)
                    vrd.Dispose();
            }
        }

        private string MAxValue(string[] fileNames, int bandNo, float zoom)
        {
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            for (int i = 0; i < fileNames.Length; i++)
            {
                FilePrdMap map = new FilePrdMap(fileNames[i], zoom, new VaildPra(float.MinValue, float.MaxValue), new int[] { bandNo });
                if (map.BandCount < 1)
                {
                    PrintInfo("请选择正确的数据进行最大值合成。");
                    return null;
                }
                filePrdMap.Add("ndviFile" + i.ToString(), map);
            }

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider prd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (prd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
            IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(new ArgumentProvider(prd, null));

            List<int> bands = new List<int>();
            for (int i = 0; i < fileNames.Length; i++)
                bands.Add(filePrdMap["ndviFile" + i.ToString()].StartBand);
            PrintInfo("正在进行最大值合成,请稍后...!");
            IPixelFeatureMapper<float> result = new MemPixelFeatureMapper<float>("0MAX", prd.Width * prd.Height, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(bands.ToArray(),
                (idx, values) =>
                {
                    result.Put(idx, values.Max() * zoom);
                }
                );
            RasterIdentify rid = new RasterIdentify(fileNames);
            IInterestedRaster<float> iir = new InterestedRaster<float>(rid, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            iir.Put(result);
            iir.Dispose();
            return iir.FileName;
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
