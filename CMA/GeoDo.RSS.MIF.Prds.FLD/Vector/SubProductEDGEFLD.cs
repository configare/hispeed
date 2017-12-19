using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductEDGEFLD : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProductEDGEFLD(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "EDGEAlgorithm")
            {
                return EDGEAlgorithm();
            }
            return null;
        }

        private IExtractResult EDGEAlgorithm()
        {
            int band = (int)_argumentProvider.GetArg("band");
            int smaping = (int)_argumentProvider.GetArg("Smaping");
            float dataValue = (float)_argumentProvider.GetArg("dataValue");
            bool isOutputUncompleted = (bool)_argumentProvider.GetArg("IsOutputUncompleted");
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string extInfos = GetStringArgument("extinfo");
            if (files == null || files.Length == 0)
                return null;
            try
            {
                ExtractResultArray array = new ExtractResultArray("FLD");
                IRasterDataProvider rdp = null;
                string shpFile = null;
                GenerateContourLines gcl = null;
                FileExtractResult res = null;
                RasterIdentify rid;
                foreach (string file in files)
                {
                    try
                    {
                        //生成等值线shp文件
                        shpFile = GenEDGEFiename(file);
                        gcl = new GenerateContourLines(_progressTracker, _contextMessage);
                        rdp = GeoDataDriver.Open(file, null) as IRasterDataProvider;
                        rid = new RasterIdentify(rdp);
                        if (rid.SubProductIdentify == "DBLV" && dataValue == 1)
                            dataValue = 0;
                        gcl.DoGenerateContourLines(rdp, band, _argumentProvider.AOI, new double[] { dataValue }, smaping, shpFile, isOutputUncompleted);
                        if (!File.Exists(shpFile))
                        {
                            PrintInfo(Path.GetFileName(file) + "：生成等值线数据失败。");
                            continue;
                        }
                        res = new FileExtractResult(_subProductDef.Identify, shpFile, true);
                        res.SetDispaly(false);
                        array.Add(res);
                    }
                    finally
                    {
                        rdp.Dispose();
                    }
                }
                return array.PixelMappers == null || array.PixelMappers.Length == 0 ? null : array;
            }
            finally
            {

            }
        }

        private string GenEDGEFiename(string rasterFile)
        {
            RasterIdentify rd = new RasterIdentify(rasterFile);
            rd.SubProductIdentify = _subProductDef.Identify;
            rd.ProductIdentify = _subProductDef.ProductDef.Identify;
            rd.Format = ".shp";
            return rd.ToWksFullFileName(".shp");
        }

        public void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
