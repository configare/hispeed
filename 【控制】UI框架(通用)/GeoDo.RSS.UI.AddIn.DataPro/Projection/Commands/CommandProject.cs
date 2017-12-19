using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CmdProjectGLL : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CmdProjectGLL()
        {
            _id = 4001;
            _name = "GLLProjection(整轨)";
            _text = "等经纬度投影(整轨)";
            _toolTip = "等经纬度投影(整轨)";
        }

        public override void Execute()
        {
            Execute("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">
        /// 格式"*#*"冒号前面部分表示文件名称，冒号后面部分表示所选投影坐标的Wkt格式字符串
        /// </param>
        public override void Execute(string argument)
        {
            try
            {
                string fileName = "";
                string prjString = "";
                ISpatialReference proj = null;
                if (string.IsNullOrWhiteSpace(argument))
                {
                    fileName = TryGetFileFromActiveView();
                    proj = SpatialReference.GetDefault();
                }
                else
                {
                    MatchCollection matches = Regex.Matches(argument, @"(?<file>[^#]*)#(?<proj>.*)");
                    fileName = matches[0].Groups["file"].Value;
                    prjString = matches[0].Groups["proj"].Value;
                    if (string.IsNullOrWhiteSpace(fileName) || fileName.ToLower() == "null")
                        fileName = TryGetFileFromActiveView();
                    if (string.IsNullOrWhiteSpace(prjString) || prjString.ToLower() == "null")
                        proj = SpatialReference.GetDefault();
                    else
                        proj = SpatialReference.FromWkt(prjString, enumWKTSource.EsriPrjFile);
                }
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new Exception("无法从参数中解析出文件名:" + argument);
                string extName = Path.GetExtension(fileName).ToUpper();
                if (extName == ".HDF" || extName == ".1BD" || extName == ".LDF" || extName == ".DAT")
                    Project(fileName, proj);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Project(string fileName, ISpatialReference proj)
        {
            string errorMsg;
            string[] outFiles = null;
            ProjectionFactory quick = new ProjectionFactory();
            try
            {
                _progressMonitor = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                if (_progressMonitor != null)
                {
                    _progressMonitor.Reset("", 100);
                    _progressMonitor.Start(false);
                    _progressMonitor.Boost(0, "准备文件");
                }
                string outdir = GetOutDirFromConfig(fileName);
                PrjOutArg prjOutArg = new PrjOutArg(proj, null, 0, 0, outdir);
                outFiles = quick.Project(fileName, prjOutArg, new Action<int, string>(OnProgress), out errorMsg);
            }
            finally
            {
                if (_progressMonitor != null)
                    _progressMonitor.Finish();
            }
            if (outFiles != null && outFiles.Length != 0)
            {
                for (int i = 0; i < outFiles.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(outFiles[i]))
                        OpenFileToWindows(outFiles[i]);
                }
            }
            if (errorMsg != null && errorMsg.Length != 0)
                MsgBox.ShowInfo(errorMsg);
        }

        private void OnProgress(int progerss, string text)
        {
            if (_progressMonitor != null)
                _progressMonitor.Boost(progerss, text);
        }

        private string TryGetFileFromActiveView()
        {
            ISmartViewer viewer = _smartSession.SmartWindowManager.ActiveViewer;
            if (viewer == null)
                throw new Exception("未获得激活的数据窗口");
            ICanvasViewer canViewer = viewer as ICanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IRasterDrawing rd = canViewer.ActiveObject as IRasterDrawing;
            if (rd == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            IRasterDataProvider rdp = rd.DataProvider;
            if (rdp == null)
                throw new Exception("未从激活的数据窗口中获取数据提供者");
            return rdp.fileName;
        }

        private void OpenFileToWindows(string file)
        {
            _smartSession.CommandEnvironment.Get(2000).Execute(file);
        }

        private string GetOutDirFromConfig(string fileName)
        {
            //从投影配置文件读取投影设置位置
            bool isUsed;
            ProjectConfig config = new ProjectConfig();
            string outdir = config.GetConfigValue("ProjectDir");
            string usedValue = config.GetConfigValue("IsUsed");
            if (string.IsNullOrEmpty(outdir) || string.IsNullOrEmpty(usedValue) || bool.TryParse(usedValue, out isUsed) && !isUsed)
                outdir = Path.GetDirectoryName(fileName);
            return outdir;
        }
    }
}
