using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CmdProjectOther : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CmdProjectOther()
        {
            _id = 4002;
            _name = "等经纬度投影(整轨)";
            _text = "等经纬度投影(整轨)";
            _toolTip = "Projection";
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
                IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                ProjectionFactory quick = new ProjectionFactory();
                string errorMsg;
                string[] outFiles = null;
                try
                {
                    _progressMonitor = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                    if (_progressMonitor != null)
                    {
                        _progressMonitor.Reset("", 100);
                        _progressMonitor.Start(false);
                        _progressMonitor.Boost(0, "准备文件");
                    }
                    PrjOutArg prjOutArg = new PrjOutArg(proj, null, 0, 0, Path.GetDirectoryName(fileName));
                    outFiles = quick.Project(fileName, prjOutArg,new Action<int, string>(OnProgress), out errorMsg);
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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
            OpenFileFactory.Open(file);
        }
    }
}
