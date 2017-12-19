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
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RasterProject;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandProjectInput : GeoDo.RSS.Core.UI.Command
    {
        IProgressMonitor _progressMonitor = null;

        public CommandProjectInput()
        {
            _id = 4023;
            _name = "GLLProjection(交互)";
            _text = "等经纬度投影(交互)";
            _toolTip = "等经纬度投影(交互)";
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
                PrjEnvelope envelope = TryGetFirstAOI();
                PrjOutArg prjOutArg = ProjectionFactory.GetDefaultArg(fileName);
                if (prjOutArg == null)
                    throw new Exception("暂未提供该支持");
                if (envelope==null|| envelope.IsEmpty)
                    envelope = prjOutArg.Envelopes[0] == null ? null : prjOutArg.Envelopes[0].PrjEnvelope;
                if (envelope == null)
                    envelope = PrjEnvelope.CreateByCenter(105,30,10,10);
                //根据投影配置文件修改输出文件目录
                string outDir = null;
                if (TryGetOutDirFormConfig(out outDir))
                    prjOutArg.OutDirOrFile = outDir;
                using (frmPrjEnvelopeSet frmPrjSetArgs = new frmPrjEnvelopeSet())
                {
                    using (IRasterDataProvider file = FileHelper.Open(fileName))
                    {
                        frmPrjSetArgs.SetDefault(file);
                    }
                    frmPrjSetArgs.SetArgs(proj, envelope, prjOutArg.ResolutionX, prjOutArg.ResolutionY, prjOutArg.OutDirOrFile);
                    if (frmPrjSetArgs.ShowDialog() != DialogResult.OK)
                        return;
                    prjOutArg = frmPrjSetArgs.PrjOutArg;
                }
                Project(fileName, proj, prjOutArg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool TryGetOutDirFormConfig(out string outDir)
        {
            //从投影配置文件读取投影设置位置
            bool isUsed;
            ProjectConfig config = new ProjectConfig();
            outDir = config.GetConfigValue("ProjectDir");
            string usedValue = config.GetConfigValue("IsUsed");
            if (string.IsNullOrEmpty(outDir) || string.IsNullOrEmpty(usedValue) || bool.TryParse(usedValue, out isUsed) && !isUsed)
                return false;
            else
                return true;
        }

        private void Project(string fileName, ISpatialReference proj,  PrjOutArg prjOutArg )
        {
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

        private PrjEnvelope TryGetFirstAOI()
        {
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope env = TryGetInfoFromActiveView();
            if (env == null || env.IsEmpty())
                return null;
            return new PrjEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope TryGetInfoFromActiveView()
        {
            ICanvasViewer canViewer =  _smartSession.SmartWindowManager.ActiveCanvasViewer;
            IAOIProvider aoi = canViewer.AOIProvider;
            return aoi.GetGeoRect();
        }
    }
}
