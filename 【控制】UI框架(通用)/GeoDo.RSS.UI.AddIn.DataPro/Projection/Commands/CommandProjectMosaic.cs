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
    public class CommandProjectMosaic : GeoDo.RSS.Core.UI.Command
    {
        private IProgressMonitor _progressMonitor = null;

        public CommandProjectMosaic()
        {
            _id = 4003;
            _name = "MosiacProjection";
            _text = "拼接投影";
            _toolTip = "拼接投影";
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
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(mosaicProjectionPage); });
            if (smartWindow == null)
            {
                smartWindow = new mosaicProjectionPage(_smartSession);
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
            }
            else
                _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }

        private void Project(string fileName, ISpatialReference proj)
        {
            string errorMsg;
            string[] outFiles = null;
            Action<int, string> progress = new Action<int, string>(OnProgress);
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
                PrjOutArg prjOutArg = new PrjOutArg(proj, null, 0, 0, Path.GetDirectoryName(fileName));
                outFiles = quick.Project(fileName, prjOutArg, progress, out errorMsg);
            }
            finally
            {
                if (_progressMonitor != null)
                    _progressMonitor.Finish();
                progress = null;
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
    }
}
