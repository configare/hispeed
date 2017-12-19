using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtEvi_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;
        private string _orbitFilesDirPath = AppDomain.CurrentDomain.BaseDirectory + "\\Workspace\\VGT\\待计算数据";

        public SubProductRasterVgtEvi_old()
            : base()
        {

        }

        public SubProductRasterVgtEvi_old(SubProductDef subProductDef)
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
            if (algorith != "0EVI")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }

            ComputeArgument arg = GetComputeArguments(contextMessage);
            MultiFileComputer computer = new MultiFileComputer(_argumentProvider, _orbitFilesDirPath, contextMessage, "0EVI", arg);
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
            int bands = _argumentProvider.DataProvider.BandCount;
            if (bands == 0 || bands == 1)
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
            if (_argumentProvider.GetArg("Blue") == null)
            {
                PrintInfo("参数\"Blue\"为空。");
                return null;
            }
            int bandB = (int)_argumentProvider.GetArg("Blue");
            if (bandV < 1 || bandB < 1 || bandN < 1 || bandV > bands || bandB > bands || bandN > bands)
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
            if (_argumentProvider.GetArg("Blue_Zoom") == null)
            {
                PrintInfo("参数\"Blue_Zoom\"为空。");
                return null;
            }
            double blueZoom = (double)_argumentProvider.GetArg("Blue_Zoom");

            string express = "";
            int[] bandNos = null;
            string[] argNames = null;
            bandNos = new int[] { bandV, bandN, bandB };
            argNames = GetArgNames(_algorithmDefs[0]);
            //express = "(float)(" + _argumentProvider.GetArg(argNames[0]) + "*(band" + bandNos[1] + "/"
            //               + nearInfrared + "-band" + bandNos[0] + "/" + visibleZoom + ")/(band"
            //               + bandNos[1] + "/" + nearInfrared + "+" + _argumentProvider.GetArg(argNames[1])
            //               + "*band" + bandNos[0] + "/" + visibleZoom + "-" + _argumentProvider.GetArg(argNames[2])
            //               + "*band" + bandNos[2] + "/" + blueZoom + "+" + _argumentProvider.GetArg(argNames[3])
            //               + "))*" + _argumentProvider.GetArg(argNames[4]);
            //改为Int16
            express = "(Int16)(" + _argumentProvider.GetArg(argNames[0]) + "*(float)(band" + bandNos[1] + "/"
                          + nearInfrared + "-band" + bandNos[0] + "/" + visibleZoom + ")/(band"
                          + bandNos[1] + "/" + nearInfrared + "+" + _argumentProvider.GetArg(argNames[1])
                          + "*band" + bandNos[0] + "/" + visibleZoom + "-" + _argumentProvider.GetArg(argNames[2])
                          + "*band" + bandNos[2] + "/" + blueZoom + "+" + _argumentProvider.GetArg(argNames[3])
                          + ")*" + _argumentProvider.GetArg(argNames[4]) + ")";
            return new ComputeArgument(bandNos, express);
        }

        private string[] GetArgNames(AlgorithmDef algDef)
        {
            string[] argNames = null;
            argNames = new string[algDef.Arguments.Count()];
            for (int i = 0; i < algDef.Arguments.Count(); i++)
            {
                argNames[i] = algDef.Arguments[i].Name;
            }
            return argNames;
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
