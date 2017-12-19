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
    public class CommandProjectBlock : GeoDo.RSS.Core.UI.Command
    {
        IProgressMonitor _progressMonitor = null;

        public CommandProjectBlock()
        {
            _id = 4022;
            _name = "GLLProjection(分幅)";
            _text = "等经纬度投影(分幅)";
            _toolTip = "等经纬度投影(分幅)";
        }

        public override void Execute()
        {
            Execute("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument">格式"*#*"冒号前面部分表示文件名称，冒号后面部分表示所选投影坐标的Wkt格式字符串</param>
        public override void Execute(string argument)
        {
            try
            {
                string fileName = "";
                ISpatialReference proj = null;
                AOIItem[] aois;
                fileName = TryGetInfoFromActiveView(out aois);
                proj = SpatialReference.GetDefault();
                List<PrjEnvelopeItem> lstEnvelope = new List<PrjEnvelopeItem>();
                for (int i = 0; i < aois.Length; i++)
                {
                    AOIItem aoi = aois[i];
                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope env = aoi.GeoEnvelope;
                    if (env.IsEmpty())
                        throw new Exception("获得的感兴趣区域是空值:" + aoi.ToString());
                    PrjEnvelope prjEnv = new PrjEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                    lstEnvelope.Add(new PrjEnvelopeItem("AOI", prjEnv));
                }
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new Exception("无法从参数中解析出文件名:" + argument);
                Project(fileName, proj, lstEnvelope == null || lstEnvelope.Count == 0 ? null : lstEnvelope.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Project(string fileName, ISpatialReference proj, PrjEnvelopeItem[] lstEnvelopes)
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
                PrjOutArg prjOutArg = new PrjOutArg(proj, lstEnvelopes, 0, 0, outdir);
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

        private string TryGetInfoFromActiveView(out AOIItem[] aoiItems)
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if (aoiProvider == null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            aoiItems = aoiProvider.GetAOIItems();
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

        private string GetOutDirFromConfig(string fileName)
        {
            //从投影配置文件读取投影设置位置
            bool isUsed;
            ProjectConfig config = new ProjectConfig();
            string outdir = config.GetConfigValue("ProjectDir");
            string usedValue = config.GetConfigValue("IsUsed");
            if (string.IsNullOrEmpty(outdir) || string.IsNullOrEmpty(usedValue) || bool.TryParse(usedValue, out isUsed) && !isUsed)
                outdir = Path.GetDirectoryName(fileName) + @"\Prj\";
            return outdir;
        }
    }
}
