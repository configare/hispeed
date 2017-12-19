using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.RasterDrawing;
using System.Diagnostics;
using GeoDo.RSS.UI.AddIn.Tools;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public partial class ExtractPanelWindowContent : UserControl
    {
        protected ISmartSession _session;
        internal IExtractPanel _extractPanel;
        private bool _isAutoSave = false;
        private bool _isNullResult = false;
        //禁用参数改变时启动判识动作
        private bool _disableIntimeExtracting = false;
        private IWorkspace _currentWks;

        public ExtractPanelWindowContent()
        {
            InitializeComponent();
            UCExtractPanel c = new UCExtractPanel();
            _extractPanel = c as IExtractPanel;
            c.Dock = DockStyle.Fill;
            panel2.Controls.Add(c);
            c.OnAlgorithmChanged += new OnAlgorithmChangedHandler(c_OnAlgorithmChanged);
            c.OnArgumentValueChanged += new OnArgumentValueChangedHandler(c_OnArgumentValueChanged);
            c.OnArgumentValueChanging += new OnArgumentValueChangingHandler(c_OnArgumentValueChanging);
        }

        /// <summary>
        /// 禁用参数改变时启动判识动作
        /// </summary>
        public bool DisableIntimeExtracting
        {
            get { return _disableIntimeExtracting; }
            set { _disableIntimeExtracting = value; }
        }

        public bool IsShowSaveButton
        {
            get { return btnSave.Visible; }
            set
            {
                if (!SetShowSaveButtonFromDef())
                    btnSave.Visible = value;
            }
        }

        private bool SetShowSaveButtonFromDef()
        {
            IMonitoringSubProduct subProduct = GetCurrentSubProduct();
            if (subProduct == null)
                return false;
            if (!subProduct.Definition.VisiableSaveBtn)
            {
                btnSave.Visible = false;
                return true;
            }
            else
                return false;
        }

        public bool IsShowIntimeCheckBox
        {
            get { return ckIntimeExtracting.Visible; }
            set { ckIntimeExtracting.Visible = value; }
        }

        void c_OnArgumentValueChanging(object sender, IArgumentProvider arg)
        {
            if (_disableIntimeExtracting)
                return;
            if (ckIntimeExtracting.Checked)
                btnExtract_Click(null, null);
        }

        void c_OnArgumentValueChanged(object sender, IArgumentProvider arg)
        {
            if (_disableIntimeExtracting)
                return;
            IExtractingSession es = (_session.MonitoringSession as IMonitoringSession).ExtractingSession;
            if (es != null && es.IsActive || ckIntimeExtracting.Checked)
                btnExtract_Click(null, null);
        }

        void c_OnAlgorithmChanged(object sender, AlgorithmDef algDef)
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            IMonitoringSubProduct subProduct = msession.ActiveMonitoringSubProduct;
            if (subProduct == null)
                return;
            if (subProduct.Definition.IsNeedCurrentRaster)
            {
                DataIdentify did = GetCurrentRasterIdentify();
                if (did != null)
                    subProduct.ResetArgumentProvider(algDef.Identify, did.Satellite, did.Sensor);
            }
            else
            {
                subProduct.ResetArgumentProvider(algDef.Identify);
            }
            subProduct.ArgumentProvider.SetArg("AlgorithmName", algDef.Identify);
            SetSystemArguments(subProduct);
            SetCurrentRasterArgument(subProduct);
            SetBandArgs(subProduct);
        }

        private DataIdentify GetCurrentRasterIdentify()
        {
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return null;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            return drawing.DataProvider.DataIdentify;
        }

        public void DoOk()
        {
            btnExtract.PerformClick();
        }

        public void Apply(Core.UI.ISmartSession session)
        {
            _session = session;
        }

        public void Apply(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            OnlyApply(wks, subProduct);
            _extractPanel.Apply(wks, subProduct);
        }

        public void CanResetUserControl()
        {
            _extractPanel.CanResetUserControl();
        }

        private void OnlyApply(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            _currentWks = wks;
            (_extractPanel as UserControl).Enabled = true;
            if (subProduct != null)
            {
                ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
                if (subProduct.Definition.IsNeedCurrentRaster)
                    this.Enabled = CurrentRasterIsOK(cv);
                else if (this.Enabled == false)
                    this.Enabled = true;
                SetDefaultArgumentProvider(subProduct);
                //
                TrySetDataProvider(subProduct, cv);
                //
                SetSystemArguments(subProduct);
                //
                if (!SetShowSaveButtonFromDef())
                    btnSave.Visible = subProduct.Definition.IsNeedCurrentRaster;
            }
            //
            IMonitoringSessionEvents ms = _session.MonitoringSession as IMonitoringSessionEvents;
            if (ms != null)
                if (ms.OnMonitoringSubProductLoaded != null)
                    ms.OnMonitoringSubProductLoaded(this, (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringProduct, subProduct);
        }

        public void ApplyWithoutPanel(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            if (subProduct != null)
            {
                ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
                if (subProduct.Definition.IsNeedCurrentRaster)
                    this.Enabled = CurrentRasterIsOK(cv);
                SetDefaultArgumentProvider(subProduct);
                //
                TrySetDataProvider(subProduct, cv);
                //
                SetSystemArguments(subProduct);
                //
                btnSave.Visible = subProduct.Definition.IsNeedCurrentRaster;
            }
        }

        private void SetDefaultArgumentProvider(IMonitoringSubProduct subProduct)
        {
            if (subProduct.Definition.IsNeedCurrentRaster)
            {
                //根据卫星、传感器选择算法
                DataIdentify did = GetCurrentRasterIdentify();
                if (did != null)
                {
                    IArgumentProvider prd = subProduct.ArgumentProvider;
                    subProduct.ResetArgumentProvider(did.Satellite, did.Sensor);
                    object algorithm = prd.GetArg("AlgorithmName");
                    string algorithmName = (algorithm == null ? "" : algorithm.ToString());
                    if (prd != null && !string.IsNullOrWhiteSpace(algorithmName))
                        subProduct.ResetArgumentProvider(algorithmName);
                }
            }
        }

        private void SetCurrentRasterArgument(IMonitoringSubProduct subProduct)
        {
            if (subProduct.Definition.IsNeedCurrentRaster)
            {
                ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
                if (cv != null)
                {
                    IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
                    subProduct.ArgumentProvider.DataProvider = drawing != null ? drawing.DataProvider : null;
                }
                else
                {
                    if (subProduct.ArgumentProvider.DataProvider != null)
                    {
                        subProduct.ArgumentProvider.DataProvider.Dispose();
                        subProduct.ArgumentProvider.DataProvider = null;
                    }
                }
            }
        }

        private void SetSystemArguments(IMonitoringSubProduct subProduct)
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            subProduct.ArgumentProvider.SetArg("ThemeGraphyGenerator", msession.ThemeGraphGenerator);
            subProduct.ArgumentProvider.SetArg("FileNameGenerator", FileNameGeneratorDefault.GetFileNameGenerator());
            subProduct.ArgumentProvider.SetArg("SmartSession", _session);
            if (subProduct.Definition.IsNeedCurrentRaster)
            {
                ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
                if (cv != null)
                    subProduct.ArgumentProvider.SetArg(ArgumentProvider.ARGUMENT_NAME_INTERACTIVER, cv as ICurrentRasterInteractiver);
            }
            subProduct.ArgumentProvider.SetArg(ArgumentProvider.ENV_VAR_PROVIDER, _session.MonitoringSession as IEnvironmentVarProvider);
        }

        private void TrySetDataProvider(IMonitoringSubProduct subProduct, ICanvasViewer cv)
        {
            if (cv == null)
                return;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            IArgumentProvider prd = subProduct.ArgumentProvider;
            prd.DataProvider = drawing.DataProviderCopy;
        }

        private bool CurrentRasterIsOK(ICanvasViewer cv)
        {
            if (cv == null)
                return false;
            return (cv.ActiveObject is IRasterDrawing);
        }

        public void Free()
        {
            _session = null;
            if (_extractPanel != null)
            {
                (_extractPanel as UCExtractPanel).OnAlgorithmChanged -= new OnAlgorithmChangedHandler(c_OnAlgorithmChanged);
                (_extractPanel as UCExtractPanel).OnArgumentValueChanged -= new OnArgumentValueChangedHandler(c_OnArgumentValueChanged);
                (_extractPanel as UCExtractPanel).OnArgumentValueChanging -= new OnArgumentValueChangingHandler(c_OnArgumentValueChanging);
                (_extractPanel as UCExtractPanel).Dispose();
                _extractPanel.MonitoringSubProductTag = null;
                _extractPanel = null;
            }
            //if (_currentWks != null)
            //{
            //    (_currentWks as UCWorkspace).Dispose();
            //    _currentWks = null;
            //}
        }

        public void DoExtract(bool isAutoSave)
        {
            _isNullResult = false;
            _isAutoSave = isAutoSave;
            btnExtract_Click(null, null);
            if (!_isNullResult && _isAutoSave)
                btnSave_Click(null, null);
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            IMonitoringSubProduct subProduct = GetCurrentSubProduct();
            if (subProduct == null)
                return;
            IContextMessage msg = _session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            SetAOIArgument(subProduct);
            IProgressMonitor tracker = _session.ProgressMonitorManager.DefaultProgressMonitor;
            IExtractResult result = null;
            try
            {
                subProduct.ArgumentProvider.SetArg("SmartSession", _session);
                //不能在这里设置SelectedPrimaryFiles参数，会将界面上设置好参数值的覆盖掉。
                //TrySetSelectedPrimaryFilesArgs(_session, subProduct);

                tracker.Start(false);
                tracker.Reset("正在生成...", 100);
                result = subProduct.Make((pro, tip) =>
                {
                    if (tracker != null)
                        tracker.Boost(pro, tip);
                },
                msg);
                if (result == null)
                {
                    _isNullResult = true;
                    return;
                }
            }
            finally
            {
                tracker.Finish();
            }
            if (!(result is IPixelIndexMapper) && !(result is IExtractResultArray))
                _isAutoSave = false;
            if (result is IExtractResultArray)
                SetIsAutoSave(result as IExtractResultArray);
            DisplayResultClass.DisplayResult(_session, subProduct, result, true);
        }

        private void SetIsAutoSave(IExtractResultArray extractArray)
        {
            foreach (IExtractResultBase extractBase in extractArray.PixelMappers)
            {
                if (!(extractBase is IFileExtractResult))
                    return;
            }
            _isAutoSave = false;
        }

        private void TrySetSelectedPrimaryFilesArgs(ISmartSession session, IMonitoringSubProduct subProduct)
        {
            if (!SetSelectedPrimaryFiles(session, subProduct.ArgumentProvider))
                return;
        }

        private bool SetSelectedPrimaryFiles(ISmartSession session, IArgumentProvider iArgumentProvider)
        {
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            iArgumentProvider.SetArg("SelectedPrimaryFiles", fnames);
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            IMonitoringSubProduct subProduct = GetCurrentSubProduct();
            if (subProduct == null)
                return;
            if (!subProduct.Definition.IsNeedCurrentRaster)
                return;
            IWorkspace wks = (_session.MonitoringSession as IMonitoringSession).Workspace;
            (_session.MonitoringSession as IMonitoringSession).ExtractingSession.AddToWorkspace(wks);
            (_session.MonitoringSession as IMonitoringSession).ExtractingSession.Saved();

            //后续产品制作
            IPixelIndexMapper piexd = (_session.MonitoringSession as IMonitoringSession).ExtractingSession.GetBinaryValuesMapper(subProduct.Definition.ProductDef.Identify, subProduct.Definition.Identify);
            IExtractResult result = (subProduct).MakeExtProduct(piexd, null);
            if (result == null)
                return;
            if (!(result is IPixelIndexMapper) && !(result is IExtractResultArray))
                _isAutoSave = false;
            DisplayResultClass.DisplayResult(_session, subProduct, result, true);
        }

        private void SetBandArgs(IMonitoringSubProduct subProduct)
        {
            if (!subProduct.Definition.IsNeedCurrentRaster)
                return;
            DataIdentify id = GetCurrentRasterIdentify();
            if (id == null)
                return;
            if (!subProduct.ArgumentProvider.BandArgsIsSetted)
            {
                MonitoringThemeFactory.SetBandArgs(subProduct, id.Satellite, id.Sensor);
            }
            else
            {
                AlgorithmDef alg = subProduct.ArgumentProvider.CurrentAlgorithmDef;
                if (!alg.Satellites.Contains(id.Satellite) || !alg.Sensors.Contains(id.Sensor))
                {
                    MonitoringThemeFactory.SetBandArgs(subProduct, id.Satellite, id.Sensor);
                }
            }
        }

        public Control Panel2Control0
        {
            get
            {
                Control control = null;
                if (this.panel2.Controls.Count > 0)
                {
                    control = panel2.Controls[0];
                }
                return control;
            }
        }

        private void SetAOIArgument(IMonitoringSubProduct subProduct)
        {
            int[] drawedAOI = null;
            if (_session.SmartWindowManager.ActiveCanvasViewer != null && subProduct.Definition.IsNeedCurrentRaster)
                drawedAOI = _session.SmartWindowManager.ActiveCanvasViewer.AOIProvider.GetIndexes();
            int[] aoiFromTemplates = GetAOITemplates(subProduct);
            //
            ICanvasViewer activeCanvas = _session.SmartWindowManager.ActiveCanvasViewer;
            if (activeCanvas != null)
            {
                IRasterDrawing drawing = _session.SmartWindowManager.ActiveCanvasViewer.ActiveObject as IRasterDrawing;
                if (drawing != null)
                {
                    IRasterDataProvider prd = drawing.DataProvider;
                    Size size = new System.Drawing.Size(prd.Width, prd.Height);
                    //
                    subProduct.ArgumentProvider.AOI = AOIHelper.Intersect(drawedAOI, aoiFromTemplates, size);
                    if (drawedAOI != null && drawedAOI.Length != 0)
                    {
                        List<Feature> fets = new List<Feature>();
                        AOI2ShapeFile aoi2ShpFile = new AOI2ShapeFile();
                        Feature fet = aoi2ShpFile.Export(GetEnvelope(drawing.DataProviderCopy),
                            new System.Drawing.Size(drawing.DataProviderCopy.Width, drawing.DataProviderCopy.Height),
                            drawedAOI);
                        fets.Add(fet);
                        subProduct.ArgumentProvider.AOIs = fets.ToArray();
                    }
                    else
                        subProduct.ArgumentProvider.AOIs = null;
                }
            }
        }

        private CodeCell.AgileMap.Core.Envelope GetEnvelope(IRasterDataProvider dataProvider)
        {
            return new CodeCell.AgileMap.Core.Envelope(dataProvider.CoordEnvelope.MinX,
                dataProvider.CoordEnvelope.MinY,
                dataProvider.CoordEnvelope.MaxX,
                dataProvider.CoordEnvelope.MaxY);
        }

        private int[] GetAOITemplates(IMonitoringSubProduct subProduct)
        {
            string[] aoiTemplates = _extractPanel.AOIs;
            if (aoiTemplates == null || aoiTemplates.Length == 0)
                return null;
            IRasterDataProvider dataprovider = GetCurrentRasterDataProvider();
            if (dataprovider == null)
                return null;
            CoordEnvelope evp = dataprovider.CoordEnvelope;
            int[] aoi;
            int[] retAOI = null;
            foreach (string aoiname in aoiTemplates)
            {
                if (string.IsNullOrEmpty(aoiname))
                    continue;
                aoi = AOITemplateFactory.MakeAOI(aoiname, evp.MinX, evp.MaxX, evp.MinY, evp.MaxY, new Size(dataprovider.Width, dataprovider.Height));
                if (aoi == null)
                    continue;
                if (subProduct != null)
                {
                    if (aoiname.Contains("海陆模版") && subProduct.Identify == "ICE")
                        aoi = AOIHelper.Reverse(aoi, new Size(dataprovider.Width, dataprovider.Height));
                }
                if (retAOI == null)
                    retAOI = aoi;
                else
                {
                    retAOI = AOIHelper.Merge(new int[][] { aoi, retAOI });
                }
            }
            return retAOI;
        }

        private IRasterDataProvider GetCurrentRasterDataProvider()
        {
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return null;
            IRasterDrawing drawing = cv.ActiveObject as IRasterDrawing;
            return drawing.DataProvider;
        }

        private IMonitoringSubProduct GetCurrentSubProduct()
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            return msession.ActiveMonitoringSubProduct;
        }

        private void brnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认恢复默认参数?", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //IMonitoringSubProduct subProduct = GetCurrentSubProduct();
                //IArgumentProvider prd = subProduct.ArgumentProvider;
                //if (prd != null && prd.GetArg("AlgorithmName") != null)
                //    subProduct.ResetArgumentProvider(prd.GetArg("AlgorithmName").ToString());
                //Apply(_currentWks, subProduct);

                //IMonitoringSubProduct subProduct = GetCurrentSubProduct();
                //SetDefaultArgumentProvider(subProduct);

                _extractPanel.ResetArgsValue();
            }
        }
    }
}
