using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtDvi_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;
        private string _orbitFilesDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Workspace\\VGT\\待计算数据";

        public SubProductRasterVgtDvi_old()
            : base()
        {

        }

        public SubProductRasterVgtDvi_old(SubProductDef subProductDef)
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
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "0DVI")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            ComputeArgument arg = GetComputeArguments(contextMessage);
            MultiFileComputer computer = new MultiFileComputer(_argumentProvider, _orbitFilesDirPath, contextMessage, "0NDVI", arg);
            Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
            if (pathDic == null)
            {
                return computer.ComputeByCurrentRaster(progressTracker);
            }
            if (pathDic.Keys.Contains("CurrentRaster")) //选择当前影像
                return computer.ComputeByCurrentRaster(progressTracker); 
            if (pathDic.Keys.Contains("DirectoryPath")) //选择局地文件夹路径
                return computer.ComputeByDirPath(pathDic["DirectoryPath"][0]);
            if (pathDic.Keys.Contains("FileNames")) //选择多个文件进行计算
                return computer.ComputeByFiles(pathDic["FileNames"]); 
            return null;
        }

        private ComputeArgument GetComputeArguments(IContextMessage contextMessage)
        {
            int bandCount = _argumentProvider.DataProvider.BandCount;
            if (bandCount == 0 || bandCount == 1)
            {
                PrintInfo("请选择正确的局地文件进行计算。");
                return null;
            }
            if (_argumentProvider.GetArg("Visible") == null)
            {
                PrintInfo("参数\"Visible\"为空。");
                return null;
            }
            int bandV = (int)_argumentProvider.GetArg("Visible");

            if (_argumentProvider.GetArg("NearInfrared") == null)
            {
                PrintInfo("参数\"NearInfrared\"为空。");
                return null;
            }
            int bandN = (int)_argumentProvider.GetArg("NearInfrared");
            if (bandN < 1 || bandV < 1 || bandV > bandCount || bandN > bandCount)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }
            if (_argumentProvider.GetArg("Visible_Zoom") == null)
            {
                PrintInfo("参数\"Visible_Zoom\"为空。");
                return null;
            }
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            if (_argumentProvider.GetArg("NearInfrared_Zoom") == null)
            {
                PrintInfo("参数\"NearInfrared_Zoom\"为空。");
                return null;
            }
            double nearInfrared = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
          int[]  bandNos = new int[] { bandV, bandN };
            //express = "(band" + bandNos[1] + "/" + nearInfrared + "-band" + bandNos[0] + "/" + visibleZoom + ")*" + _argumentProvider.GetArg("resultZoom") + "/100";
            //改为Int16
          string  express = "(Int16)((band" + bandNos[1] + "/" + nearInfrared + "-band" + bandNos[0] + "/" + visibleZoom + ")*" + _argumentProvider.GetArg("resultZoom") + "/100)";
          return new ComputeArgument(bandNos, express);
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
