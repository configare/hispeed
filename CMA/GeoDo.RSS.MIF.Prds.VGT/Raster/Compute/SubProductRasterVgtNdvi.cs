using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.IO;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtNdvi_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage;
        private string _orbitFilesDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Workspace\\VGT\\待计算数据";

        public SubProductRasterVgtNdvi_old()
            : base()
        {
        }

        public SubProductRasterVgtNdvi_old(SubProductDef subProductDef)
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
            string algorithmName = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorithmName != "NDVI")
            {
                PrintInfo("指定的算法\"" + algorithmName + "\"没有实现。");
                return null;
            }
            ComputeArgument arg = GetComputeArguments(contextMessage);
            MultiFileComputer computer = new MultiFileComputer(_argumentProvider, _orbitFilesDirPath, contextMessage, "NDVI",arg);
            Dictionary<string, string[]> pathDic = _argumentProvider.GetArg("OrbitFileSelectType") as Dictionary<string, string[]>;
            if (pathDic == null)
                return computer.ComputeByCurrentRaster(progressTracker);
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
            int bandV = (int)_argumentProvider.GetArg("Visible");
            int bandNear = (int)_argumentProvider.GetArg("NearInfrared");
            if (bandV == -1 || bandNear == -1)
            {
                PrintInfo("获取波段序号失败，可能是波段映射表配置错误或判识算法波段参数配置错误！");
                return null;
            }
            if (bandV < 1)
            {
                PrintInfo("无法正确获取可见光波段序号！");
                return null;
            }
            if (bandNear < 1)
            {
                PrintInfo("无法正确获取近红外波段序号！");
                return null;
            }
           int[] bandNos = new int[] { bandV, bandNear };
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            double nearInfrared = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
           string express = "(band" + bandNos[1] + "+band" + bandNos[0] + ")==0?(Int16)0:(Int16)((float)(band" + bandNos[1] + "/" + nearInfrared + "-band" + bandNos[0] + "/" + visibleZoom + ")/(band" + bandNos[1] + "/" + nearInfrared + "+band" + bandNos[0] + "/" + visibleZoom + ") * " + _argumentProvider.GetArg("resultZoom") + ")";
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
