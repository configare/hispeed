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
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CommandProjectAOI : GeoDo.RSS.Core.UI.Command
    {
        public CommandProjectAOI()
        {
            _id = 4021;
            _name = "GLLProjection(AOI)";
            _text = "等经纬度投影(AOI)";
            _toolTip = "等经纬度投影(AOI)";
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
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope[] envelopes;
                fileName = TryCheckArgsEnvironment(out envelopes);
                proj = SpatialReference.GetDefault();
                List<PrjEnvelopeItem> lstEnvelope = new List<PrjEnvelopeItem>();
                for (int i = 0; i < envelopes.Length; i++)
                {
                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope env = envelopes[i];
                    if (env.IsEmpty())
                        throw new Exception("获得的感兴趣区域是空值:" + env.ToString());
                    PrjEnvelope prjEnv = new PrjEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                    lstEnvelope.Add(new PrjEnvelopeItem("AOI", prjEnv));
                }
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new Exception("无法从参数中解析出文件名:" + argument);
                string errorMsg;
                string[] outFiles = null;
                IProgressMonitor progressMonitor = null;
                Action<int, string> progress = null;
                try
                {
                    progressMonitor = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                    progress = new Action<int, string>(
                        (int progerss, string text) =>
                        {
                            if (progressMonitor != null)
                                progressMonitor.Boost(progerss, text);
                        });
                    if (progressMonitor != null)
                    {
                        progressMonitor.Reset("", 100);
                        progressMonitor.Start(false);
                        progressMonitor.Boost(0, "准备文件");
                    }
                    string outdir = GetOutDirFromConfig(fileName);
                    PrjOutArg prjOutArg = new PrjOutArg(proj, lstEnvelope.ToArray(), 0, 0, outdir);
                    //添加对自定义感兴趣区的支持。
                    ProjectionFactory quick = new ProjectionFactory();
                    outFiles = quick.Project(fileName, prjOutArg, progress, out errorMsg);
                }
                finally
                {
                    if (progressMonitor != null)
                        progressMonitor.Finish();
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
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        private string TryCheckArgsEnvironment(out GeoDo.RSS.Core.DrawEngine.CoordEnvelope[] envelopes)
        {
            ICanvasViewer canViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                throw new Exception("未获得激活的数据窗口");
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if(aoiProvider==null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            Core.DrawEngine.CoordEnvelope geoEnvelope = aoiProvider.GetGeoRect();
            if (geoEnvelope == null)
                throw new Exception("未从激活的数据窗口中获取感兴趣区域");
            envelopes = new Core.DrawEngine.CoordEnvelope[] { geoEnvelope };

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
