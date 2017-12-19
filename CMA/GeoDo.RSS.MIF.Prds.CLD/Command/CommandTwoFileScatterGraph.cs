using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.RasterTools;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.MathAlg;
using GeoDo.RasterProject;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandTwoFileScatterGraph : Command
    {
        public CommandTwoFileScatterGraph()
            : base()
        {
            _name = "CommandTwoFileScatterGraph";
            _text = _toolTip = "文件间波段散点图";
            _id = 71020;
        }

        public override void Execute(string argument)
        {
            base.Execute();
        }

        public override void Execute()
        {
            IRasterDataProvider XdataProvider = null, YdataProvider = null;
            try
            {
                bool isNewX = false, isNewY = false;
                int[] bandNos = null;
                int[] viewerAoi = null,rightAoi=null;
                string aoiType = null;
                using (frmScatterTwoVarSelector frm = new frmScatterTwoVarSelector())
                {
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    //XdataProvider =GetCurrentDataProvider();
                    //if (XdataProvider!=null)
                    //{
                    //    frm.AOIName = "视图AOI";
                    //    aoiType = "viewer";
                    //    viewerAoi = GetAOI();
                    //    frm.Apply(XdataProvider, viewerAoi, true);
                    //} 
                    //else
                    {
                        if (StatRegionSet.UseRecgRegion||StatRegionSet.UseRegion)
                        {
                            frm.AOIName = StatRegionSet.SelectedRegionEnvelope.Name;
                            aoiType = "recg";
                        }
                        else if (StatRegionSet.UseVectorAOIRegion)
                        {
                            frm.AOIName = StatRegionSet.AOIName;
                            aoiType = "vector";
                        }
                    }
                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        XdataProvider = frm.XDataProvider;
                        isNewX = frm.IsNewXDataProvider;
                        YdataProvider = frm.YDataProvider;
                        isNewY = frm.IsNewYDataProvider;
                        bandNos = new int[] { frm.XBandNo, frm.YBandNo };
                        //判断两个文件的大小一致、空间范围一致
                        if (XdataProvider.Width != YdataProvider.Width )//|| XdataProvider.Height != YdataProvider.Height
                            throw new ArgumentException("两个文件大小不一致！目前仅支持相同大小文件！");
                        CoordEnvelope lenv = XdataProvider.CoordEnvelope, renv = YdataProvider.CoordEnvelope;
                        if (lenv == null || lenv.Width <= 0 || lenv.Height <= 0)
                            throw new ArgumentException("X轴文件的空间范围信息不可用！");
                        if (renv == null || renv.Width <= 0 || renv.Height <= 0)
                            throw new ArgumentException("Y轴文件的空间范围信息不可用！");
                        if (lenv.MinX != renv.MinX || lenv.MaxY!=renv.MaxY)
                            throw new ArgumentException("两个文件空间区域不一致！目前仅支持相同范围！！");
                        if (frm.AOIName!=null)//利用AOI
                        {
                            // 两个文件存在相交
                            PrjEnvelope lfilePrj, rfilePrj;
                            //PrjEnvelope lfilePrj = new PrjEnvelope(lenv.MinX, lenv.MaxX, lenv.MinY, lenv.MaxY), rfilePrj = new PrjEnvelope(renv.MinX, renv.MaxX, renv.MinY, renv.MaxY);
                            //PrjEnvelope env = PrjEnvelope.Intersect(lfilePrj, rfilePrj);
                            //if (env == null || env.Width <= 0 || env.Height <= 0)
                            //    throw new ArgumentException("两个文件不存在空间相交区域！");
                            //分别计算两个文件的AOI index
                            if (aoiType.ToLower()=="recg")
                            {
                                if (CloudParaFileStatics.CheckAOIIntersect(XdataProvider, StatRegionSet.SelectedRegionEnvelope.PrjEnvelope, out lfilePrj, out viewerAoi)&& 
                                    CloudParaFileStatics.CheckAOIIntersect(YdataProvider, StatRegionSet.SelectedRegionEnvelope.PrjEnvelope, out rfilePrj, out rightAoi))
                                {
                                }
                            }
                            else if (aoiType.ToLower() == "vector")
                            {
                                AOIContainerLayer aoiContainer = StatRegionSet.AoiContainer;
                                PrjEnvelope RegionEnv = StatRegionSet.AOIPrjEnvelope;
                                if (RegionEnv == null || RegionEnv.Height <= 0 || RegionEnv.Width <= 0)
                                    throw new ArgumentException("矢量AOI区域设置无效或范围不合法！");
                                if (aoiContainer != null)
                                {
                                    Size xSize = new Size(XdataProvider.Width, XdataProvider.Height);
                                    viewerAoi = CloudParaFileStaticsAOI.GetAOI(lenv, aoiContainer, xSize);
                                    if (viewerAoi == null || viewerAoi.Length <= 0)
                                        throw new ArgumentException(Path.GetFileName(XdataProvider.fileName) + "与矢量AOI区域" + frm.AOIName + "无相交区域！");
                                    Size ySize = new Size(YdataProvider.Width, YdataProvider.Height);
                                    rightAoi = CloudParaFileStaticsAOI.GetAOI(renv, aoiContainer, ySize);
                                    if (rightAoi == null || rightAoi.Length <= 0)
                                        throw new ArgumentException(Path.GetFileName(YdataProvider.fileName) + "与矢量AOI区域" + frm.AOIName + "无相交区域！");
                                }
                            }
                            //判断两个Index大小相等
                            if(viewerAoi==null||rightAoi==null||viewerAoi.Length!=rightAoi.Length)
                                throw new ArgumentException("两个文件AOI区域大小不一致！");
                        } 
                        //构建虚拟的dataProvider
                        IRasterBand xband = XdataProvider.GetRasterBand(bandNos[0]);
                        IRasterBand yband = YdataProvider.GetRasterBand(bandNos[1]);
                        IRasterDataProvider localprd = new LogicalRasterDataProvider(frm.AOIName + "区域", new IRasterBand[2] { xband, yband }, null);
                        //if (localprd.BandCount!=2)
                        //{
                        //    throw new ArgumentException("两个波段信息不一致，无法进行散点图运算！");
                        //}
                        IProgressMonitor progress = _smartSession.ProgressMonitorManager.DefaultProgressMonitor;
                        try
                        {
                            progress.Reset("正在准备生成散点图...", 100);
                            progress.Start(false);
                            frmScatterGraph frm1 = new frmScatterGraph();
                            frm1.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                            frm1.StartPosition = FormStartPosition.CenterScreen;
                            LinearFitObject fitObj =frm.FitObj;
                            frm1.Reset(localprd, 1, 2, viewerAoi,
                                       fitObj,
                                        (idx, tip) => { progress.Boost(idx, "正在准备生成散点图..."); }
                                       );
                            progress.Finish();
                            frm1.Show();
                            frm1.Rerender();
                            frm1.FormClosed += new FormClosedEventHandler((obj, e) =>
                            {
                                if (isNewX && XdataProvider != null)
                                {
                                    XdataProvider.Dispose();
                                    XdataProvider = null;
                                }
                                if (isNewY && YdataProvider != null)
                                {
                                    YdataProvider.Dispose();
                                    YdataProvider = null;
                                }
                            });
                        }
                        finally
                        {
                            progress.Finish();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private int[] GetAOI()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            return viewer.AOIProvider.GetIndexes();
        }

        private IRasterDataProvider GetCurrentDataProvider()
        {
            ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing != null)
                return drawing.DataProviderCopy;
            return null;
        }
    }
}
