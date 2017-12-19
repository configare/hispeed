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
    public class SubProductSWIDRT : CmaMonitoringSubProduct
    {
        //private string _errorStr = "";
        private IArgumentProvider _curArguments = null;
        //private IPixelFeatureMapper<Int16> _result = null;

        public SubProductSWIDRT(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "DemAlgorithm")
            {
                return DemAlgorithm();
            }
            return null;
        }

        private IExtractResult DemAlgorithm()
        {
            int DNTBandCH = (int)_curArguments.GetArg("DNTBand");
            double DNTZoom = (float)_curArguments.GetArg("DNTZoom");
            float DNTVaildMin = (float)_curArguments.GetArg("DNTVaildMin");
            float DNTVaildMax = (float)_curArguments.GetArg("DNTVaildMax");

            Int16 DemMin = (Int16)_curArguments.GetArg("DemMin");
            float DemCorrect = (float)_curArguments.GetArg("DemCorrect");

            double SWIZoom = (float)_curArguments.GetArg("SWIZoom");
            DRTExpCoefficientCollection ExpCoefficient = _curArguments.GetArg("ExpCoefficient") as DRTExpCoefficientCollection;
            string errorStr = null;
            if (DNTBandCH == -1 || _curArguments.GetArg("DNTFile") == null || _curArguments.GetArg("DemFile") == null || ExpCoefficient == null)
            {
                errorStr = "SWI生产所用文件或通道未设置完全,请检查!";
                return null;
            }
            string DNTFile = _curArguments.GetArg("DNTFile").ToString();
            string DemFile = _curArguments.GetArg("DemFile").ToString();
            if (string.IsNullOrEmpty(ExpCoefficient.EgdesFilename) || !File.Exists(ExpCoefficient.EgdesFilename))
            {
                string fileanme = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\RasterTemplate\\China_Province.dat";
                if (File.Exists(fileanme))
                    ExpCoefficient.EgdesFilename = fileanme;
                else
                {
                    errorStr = "SWI生产所用的经验系数边界文件不存在,请检查!";
                    return null;
                }
            };
            string EdgesFile = ExpCoefficient.EgdesFilename;

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("DNTFile", new FilePrdMap(DNTFile, DNTZoom, new VaildPra(DNTVaildMin, DNTVaildMax), new int[] { DNTBandCH }));
            filePrdMap.Add("DemFile", new FilePrdMap(DemFile, 1, new VaildPra(DemMin, float.MaxValue), new int[] { 1 }));
            filePrdMap.Add("EdgesFile", new FilePrdMap(EdgesFile, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            DRTExpCoefficientItem item = null;
            IVirtualRasterDataProvider vrd = null;
            try
            {
                vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                if (vrd == null)
                    throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<Int16>  result = new MemPixelFeatureMapper<Int16>("0SWI", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                double swiTemp = 0f;
                rpVisitor.VisitPixel(new int[] { filePrdMap["DNTFile"].StartBand,
                                                 filePrdMap["DemFile"].StartBand,
                                                 filePrdMap["EdgesFile"].StartBand},
                    (index, values) =>
                    {
                        if (values[0] < filePrdMap["DNTFile"].VaildValue.Min || values[0] > filePrdMap["DNTFile"].VaildValue.Max)
                            result.Put(index, 0);
                        else
                        {
                            item = ExpCoefficient.GetExpItemByNum((int)values[2]);
                            if (item == null)
                                result.Put(index, 0);
                            else
                            {
                                //计算SWI
                                swiTemp = item.APara * values[0] + item.BPara + item.CPara;
                                if (values[1] > filePrdMap["DemFile"].VaildValue.Min)
                                    swiTemp += DemCorrect;
                                result.Put(index, (Int16)(swiTemp * SWIZoom));
                            }
                        }
                    });
                return result;
            }
            finally
            {
                if(vrd!=null)
                    vrd.Dispose();
            }
        }
    }
}
