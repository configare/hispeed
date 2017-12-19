using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCMutiHistoryDBLVImport : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _currentRasterFile = null;
        private ISmartSession _session = null;

        public UCMutiHistoryDBLVImport()
        {
            InitializeComponent();
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
            if (string.IsNullOrEmpty(_currentRasterFile) || lstFiles.Items.Count<1)
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
            IPixelIndexMapper resultPixelMapper = GenerateHistoryResultByAOI(pixelMapper,drawedAOI);
            if (pixelMapper != null)
                DisplayResultClass.DisplayResult(_session, subProduct, resultPixelMapper, true);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "判识结果文件(*.dat,*.mvg)|*.dat;*.mvg";
                frm.Multiselect = true;
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in frm.FileNames)
                        lstFiles.Items.Add(file);
                }

            }
        }

        private IPixelIndexMapper GenerateHistoryResultByAOI(IPixelIndexMapper pixelMapper,int[] drawedAOI)
        {
            if (drawedAOI == null || drawedAOI.Length < 1)
                return null;
            if (lstFiles.Items.Count < 1)
                return null;
            foreach (string file in lstFiles.Items)
                if (!File.Exists(file))
                    return null;
            using(IRasterDataProvider prd=GeoDataDriver.Open(_currentRasterFile) as IRasterDataProvider)
            {
                List<int> aoiList=new List<int>();
                aoiList.AddRange(drawedAOI);
                IPixelIndexMapper result = PixelIndexMapperFactory.CreatePixelIndexMapper("FLD", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                if (pixelMapper != null && pixelMapper.Indexes.Count() > 0)
                {
                    foreach (int i in pixelMapper.Indexes)
                    {
                        if(aoiList.Contains(i))
                          result.Put(i);
                    }
                }
                //
                IInterestedRaster<Int16> iir = null;
                try
                {
                    RasterIdentify id = new RasterIdentify();
                    id.ThemeIdentify = "CMA";
                    id.ProductIdentify = "FLD";
                    id.SubProductIdentify = "DBLV";
                    iir = new InterestedRaster<Int16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
                    int[] idxs = result.Indexes.ToArray();
                    iir.Put(idxs, 1);
                }
                finally
                {
                    iir.Dispose();
                }
                Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
                filePrdMap.Add("currentDBLV", new FilePrdMap(iir.FileName, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
                int index = 0;
                foreach (string file in lstFiles.Items)
                {
                    filePrdMap.Add("dblv"+index, new FilePrdMap(file, 1, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { 1 }));
                    index++;
                }
                ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
                IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
                if (vrd == null)
                {
                    if (filePrdMap != null && filePrdMap.Count > 0)
                    {
                        foreach (FilePrdMap value in filePrdMap.Values)
                        {
                            if (value.Prd != null)
                                value.Prd.Dispose();
                        }
                    }
                    return null;
                }
                try
                {
                    ArgumentProvider ap = new ArgumentProvider(vrd, null);
                    RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                    int historyCount = lstFiles.Items.Count;
                    int[] bandNos = new int[historyCount+1];
                    for (int i = 0; i < bandNos.Length; i++)
                        bandNos[i] = i+1 ;
                    int[] difArray = new int[historyCount];
                    rpVisitor.VisitPixel(new Rectangle(0, 0, prd.Width,prd.Height),drawedAOI ,bandNos ,
                        (idx, values) =>
                        {
                            for (int i = 0; i < historyCount; i++)
                            {
                                if (values[0] != values[i + 1])
                                    difArray[i]++;
                            }
                        });
                    int min = difArray[0], minIndex = 0;
                    for (int i = 1; i < difArray.Length; i++)
                    {
                        if (min > difArray[i])
                        {
                            min = difArray[i];
                            minIndex = i;
                        }
                    }
                    rpVisitor.VisitPixel(bandNos, 
                        (idx, values) =>
                        {
                            if (values[minIndex + 1] == 1)
                            {
                                if(!aoiList.Contains(idx))
                                   result.Put(idx);
                            }
                        });
                    return result;
                }
                finally
                {
                    vrd.Dispose();
                    if (File.Exists(iir.FileName))
                        File.Delete(iir.FileName);
                }
            }
        }

        private IRasterDataProvider GetTempOutRaster(IRasterDataProvider currentRasterPrd)
        {
            string outFileName = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir//" + Path.GetFileNameWithoutExtension(currentRasterPrd.fileName) + "_temp.ldf";
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
