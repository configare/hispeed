using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCHistoryDBLVImport : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _currentRasterFile = null;
        private ISmartSession _session = null;

        public UCHistoryDBLVImport()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "判识结果文件(*.dat,*.mvg)|*.dat;*.mvg";
                frm.Multiselect = false;
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text = frm.FileName;
                }

            }
        }

        public object GetArgumentValue()
        {
            return null;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            if (panel == null)
                return;
            UCExtractPanel ucPanel = panel as UCExtractPanel;
            if (ucPanel == null)
                return;
            IMonitoringSubProduct subProduct = ucPanel.MonitoringSubProduct;
            if (subProduct == null)
                return;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp != null)
            {
                if (arp.DataProvider != null)
                    _currentRasterFile = arp.DataProvider.fileName;
                object obj = arp.GetArg("SmartSession");
                if (obj != null)
                    _session = obj as ISmartSession;
            }
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentRasterFile) || string.IsNullOrEmpty(txtFileDir.Text))
                return;
            if (_session == null)
                return;
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return;
            int[] drawedAOI = cv.AOIProvider.GetIndexes();
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            IMonitoringSubProduct subProduct = msession.ActiveMonitoringSubProduct;
            IPixelIndexMapper pixelMapper = (_session.MonitoringSession as IMonitoringSession).ExtractingSession.GetBinaryValuesMapper(subProduct.Definition.ProductDef.Identify, subProduct.Definition.Identify);
            IPixelIndexMapper resultPixelMapper = GenerateHistoryResultByAOI(pixelMapper, txtFileDir.Text, drawedAOI);
            if (pixelMapper != null)
                DisplayResultClass.DisplayResult(_session, subProduct, resultPixelMapper, true);
        }

        private IPixelIndexMapper GenerateHistoryResultByAOI(IPixelIndexMapper pixelMapper, string historyFile, int[] drawedAOI)
        {
            if (!File.Exists(historyFile) || drawedAOI == null || drawedAOI.Length < 1)
                return null;
            IRasterDataProvider historyPrd = null;
            IRasterDataProvider prd = null;
            string outFileName = null;
            try
            {
                historyPrd = GeoDataDriver.Open(historyFile) as IRasterDataProvider;
                prd = GeoDataDriver.Open(_currentRasterFile) as IRasterDataProvider;
                IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FLD", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                if (pixelMapper != null && pixelMapper.Indexes.Count() > 0)
                {
                    foreach (int i in pixelMapper.Indexes)
                        result.Put(i);
                }
                List<RasterMaper> rms = new List<RasterMaper>();
                RasterMaper rm = new RasterMaper(prd, new int[] { 1 });
                RasterMaper oldRm = new RasterMaper(historyPrd, new int[] { 1 });
                rms.AddRange(new RasterMaper[] { rm, oldRm });
                using (IRasterDataProvider outRaster = GetTempOutRaster(historyFile, prd))
                {
                    //栅格数据映射
                    RasterMaper[] fileIns = rms.ToArray();
                    RasterMaper[] fileOuts = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                    //创建处理模型
                    RasterProcessModel<short, short> rfr = null;
                    rfr = new RasterProcessModel<short, short>(null);
                    rfr.SetRaster(fileIns, fileOuts);
                    rfr.RegisterCalcModel(new RasterCalcHandler<short, short>((rvInVistor, rvOutVistor, aoi) =>
                    {
                        int y = rvInVistor[0].IndexY * rvInVistor[0].Raster.Width;
                        //修改aoi区域超出索引问题
                        int length = rvInVistor[1].RasterBandsData[0].Length;
                        foreach (int i in drawedAOI)
                        {
                            //修改aoi区域超出索引问题
                            if (i < y || i - y >= length)
                                continue;
                            if (rvInVistor[1].RasterBandsData[0][i - y] == 1)
                            {
                                rvOutVistor[0].RasterBandsData[0][i - y] = 1;
                                result.Put(i);
                            }
                        }
                    }));
                    rfr.Excute();
                }
                return result;
            }
            finally
            {
                if (historyPrd != null)
                    historyPrd.Dispose();
                if (prd != null)
                    prd.Dispose();
                if (File.Exists(outFileName))
                    File.Delete(outFileName);
            }
        }

        private IRasterDataProvider GetTempOutRaster(string historyFile, IRasterDataProvider currentRasterPrd)
        {
            string outFileName = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir//" + Path.GetFileNameWithoutExtension(historyFile) + "_temp.ldf";
            if (!Directory.Exists(Path.GetDirectoryName(outFileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(outFileName));
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            CoordEnvelope outEnv = currentRasterPrd.CoordEnvelope.Clone();
            int width = currentRasterPrd.Width;
            int height = currentRasterPrd.Height;
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, currentRasterPrd.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
