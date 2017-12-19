using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductISOTICE : CmaMonitoringSubProduct
    {
        private string _errorStr = "";
        private IArgumentProvider _curArguments = null;
        private IContextMessage _contextMessage;
        private Action<int, string> _progressTracker = null;

        public SubProductISOTICE(SubProductDef subProductDef)
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
            _curArguments = _argumentProvider;
            if (_curArguments == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName") == null)
                return null;
            if (_curArguments.GetArg("AlgorithmName").ToString() == "ISOTAlgorithm")
            {
                return ISOTAlgorithm();
            }
            return null;
        }

        private IExtractResult ISOTAlgorithm()
        {
            int FarInfraredCH = (int)_curArguments.GetArg("FarInfrared");
            double FarInfraredZoom = (double)_curArguments.GetArg("FarInfrared_Zoom");
            int smaping = (int)_curArguments.GetArg("Smaping");
            int tempratureMin = (int)(((int)_curArguments.GetArg("TempratureMin") + 273) * FarInfraredZoom);
            int tempratureMax = (int)(((int)_curArguments.GetArg("TempratureMax") + 273) * FarInfraredZoom);
            int interval = (int)((int)_curArguments.GetArg("IntervalMax") * FarInfraredZoom);

            if (FarInfraredCH == -1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            if (_argumentProvider.DataProvider == null)
            {
                PrintInfo("获取文件错误,可能是没有打开影像文件。");
                return null;
            }
            try
            {
                string shpFile = @"C:\Users\Administrator\Desktop\1.shp";
                GenerateContourLines gcl = new GenerateContourLines(_progressTracker, _contextMessage);
                gcl.DoGenerateContourLines(_argumentProvider.DataProvider, FarInfraredCH, _argumentProvider.AOI, interval, tempratureMin, tempratureMax, smaping, shpFile);
                _curArguments.SetArg("SelectedPrimaryFiles", new string[] { shpFile });
                return ThemeGraphyResult(null);
            }
            finally
            {
                _curArguments.SetArg("SelectedPrimaryFiles", null);
            }
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
