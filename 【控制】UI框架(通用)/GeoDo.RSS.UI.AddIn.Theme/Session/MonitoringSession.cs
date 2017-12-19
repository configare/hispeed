using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.MEF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using System.IO;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class MonitoringSession : IMonitoringSession, IMonitoringSessionEvents
    {
        private IMonitoringProduct[] _monitoringProducts;
        private IMonitoringProduct _activeMonitoringProduct;
        private IMonitoringSubProduct _activeMonitoringSubProduct;
        private IExtractingSession _extractingSession;
        private ISmartSession _session;
        private IThemeGraphGenerator _themeGraphGenerator;
        private IFileNameGenerator _filenameGenerator;
        private ICanvasViewer _currentCanvasViewer;
        private MonitoringProductLoadedHandler _monitoringProductLoaded;
        private MonitoringSubProductLoadedHandler _monitoringSubProductLoaded;

        public MonitoringSession(ISmartSession session)
        {
            _session = session;
            _themeGraphGenerator = new ThemeGraphGenerator(session);
            _filenameGenerator = new FileNameGeneratorDefault();
            _extractingSession = new ExtractingSession(session);
            LoadMonitoringTheme();
        }

        private void LoadMonitoringTheme()
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("监测分析专题");
            if (dlls == null || dlls.Length == 0)
                return;
            List<IMonitoringProduct> mps = new List<IMonitoringProduct>();
            using (IComponentLoader<IMonitoringProduct> loader = new ComponentLoader<IMonitoringProduct>())
            {
                for (int i = 0; i < dlls.Length; i++)
                {
                    if (File.Exists(dlls[i]))
                    {
                        IMonitoringProduct[] prds = null;
                        try
                        {
                            prds = loader.LoadComponents(dlls[i]);
                        }
                        catch (Exception ex)//这里通过去除产品定义文件的方式令加载失败，这里需要记录日志
                        {
                            Logger.Trace(ex);
                            prds = null;
                        }
                        if (prds != null && prds.Length != 0)
                            mps.AddRange(prds);
                    }
                }
            }
            _monitoringProducts = mps.ToArray();
        }

        public ICanvasViewer CurrentCanvasViewer
        {
            get { return _currentCanvasViewer; }
        }

        public IMonitoringProduct ActiveMonitoringProduct
        {
            get { return _activeMonitoringProduct; }
        }

        public IMonitoringSubProduct ActiveMonitoringSubProduct
        {
            get { return _activeMonitoringSubProduct; }
        }

        public void ClosedActiveMonitoringSubProduct()
        {
            _activeMonitoringSubProduct = null;
        }

        public IMonitoringProduct ChangeActiveProduct(string identify, bool isOpenWorkspace)
        {
            if (_activeMonitoringProduct != null && _activeMonitoringProduct.Identify == identify)
                return _activeMonitoringProduct;
            _activeMonitoringProduct = null;
            _activeMonitoringSubProduct = null;
            if (_monitoringProducts == null || _monitoringProducts.Length == 0)
            {
                if (isOpenWorkspace)
                    OpenAndFillWorkspace();
                return null;
            }
            foreach (MonitoringProduct prd in _monitoringProducts)
            {
                if (prd.Identify == identify)
                {
                    _activeMonitoringProduct = prd;
                    break;
                }
            }
            if (_activeMonitoringProduct == null)
            {
                _extractingSession.Stop();
                return null;
            }
            if (isOpenWorkspace)
                OpenAndFillWorkspace();
            //
            if (_monitoringProductLoaded != null)
                _monitoringProductLoaded(this, _activeMonitoringProduct);
            //
            if (ExtractPanelWidow != null)
                ExtractPanelWidow.Apply(Workspace, null);
            OpenWndWithProductChanged();
            return _activeMonitoringProduct;
        }

        private void OpenWndWithProductChanged()
        {
            //by chennan 窗口上下文
            CmdExecute(9000);
            CmdExecute(1004);
        }

        private void CloseWndWithProductChanged()
        {
            //by chennan 窗口上下文
            //CloseToolExecute(9000);
            CloseNoCanvasAllWnd();
            CmdExecute(1004);
        }

        /// <summary>
        /// 关闭除数据窗口的所有窗口
        /// </summary>
        private void CloseNoCanvasAllWnd()
        {
            ISmartWindow[] wnds = _session.SmartWindowManager.GetSmartWindows((wnd) =>
              {
                  if (wnd is ICanvasViewer)
                      return false;
                  else return true;
              });
            if (wnds == null || wnds.Length == 0)
                return;
            foreach (ISmartWindow item in wnds)
                if (item is DockWindow)
                    (item as DockWindow).Close();
        }

        private void CloseToolExecute(int toolWndID)
        {
            ISmartWindow wnd = _session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(toolWndID);
            if (wnd != null)
            {
                (wnd as DockWindow).Select();
                (wnd as DockWindow).Close();
                (wnd as DockWindow).Dispose();
            }
        }

        private bool CmdExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd != null)
            {
                cmd.Execute();
                return true;
            }
            return false;
        }

        public IMonitoringProduct ChangeActiveProduct(string identify)
        {
            if (string.IsNullOrEmpty(identify))
                CloseWndWithProductChanged();
            return ChangeActiveProduct(identify, true);
        }

        public void CanResetUserControl()
        {
            if (ExtractPanelWidow != null)
                ExtractPanelWidow.CanResetUserControl();
        }

        public IMonitoringSubProduct ChangeActiveSubProduct(string identify)
        {
            _activeMonitoringSubProduct = null;
            if (_activeMonitoringProduct == null || string.IsNullOrEmpty(identify))
                return null;
            foreach (IMonitoringSubProduct subprd in _activeMonitoringProduct.SubProducts)
            {
                if (subprd.Identify == identify)
                {
                    _activeMonitoringSubProduct = subprd;
                    break;
                }
            }
            if (ExtractPanelWidow != null)
                ExtractPanelWidow.Apply(Workspace, _activeMonitoringSubProduct);
            //by chennan 20130330 不明白做什么用
            //if (!_activeMonitoringSubProduct.Definition.IsDisplayPanel)
            //{ 
            //}
            return _activeMonitoringSubProduct;
        }

        public void DoAutoExtract(bool isOpenWorkspace)
        {
            SetCurrentCanvasViewer();
            if (isOpenWorkspace)
                OpenAndFillWorkspace();
            StartExtractingSession();
            TrySetInstanceArgs();
            CallPanelExtractWithoutUI();
        }

        /// <summary>
        /// 如果确定当前是处理的实例的话，为实例设置初始的参数
        /// 确定为实例的依据是：通过参数"OutFileIdentify"，尝试查找当前子产品下的实例，找到了就认为当前处理的是实例。
        /// </summary>
        /// 
        SubProductInstanceDef _instanceDef = null;
        private string[] _selectedFiles = null;
        private void TrySetInstanceArgs()
        {
            _instanceDef = TryGetInstanceFromOutFileIdentify();
            if (_instanceDef == null)
                return;
            IArgumentProvider argumentProvider = _activeMonitoringSubProduct.ArgumentProvider;
            bool isSpecifyFiles = false;
            object obj = argumentProvider.GetArg("isSpecifyFiles");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                isSpecifyFiles = bool.Parse(obj.ToString());
            string fileProvider = _instanceDef.FileProvider;
            string[] fps = fileProvider.Split(':');
            if (fps == null || fps.Length != 2)
                return;
            string key = fps[0];
            string needIdentify = fps[1];
            if (key == "ContextEnvironment")
            {
                if (needIdentify == "CurrentRasterFile")    //多通道合成图，当前区域
                {
                    //(_currentCanvasViewer.ActiveDrawing as RasterDrawing).DataProvider;
                }
                else
                {
                    if (!isSpecifyFiles)
                    {
                        IWorkspace wks = Workspace;
                        if (wks == null)
                            return;

                        string[] selectedFiles = null;
                        if (wks.ActiveCatalog != null)
                            selectedFiles = wks.ActiveCatalog.GetSelectedFiles(needIdentify);
                        if (selectedFiles == null)
                        {
                            ICatalog cat = wks.GetCatalogByIdentify(needIdentify);
                            if (cat != null)
                                selectedFiles = cat.GetSelectedFiles(needIdentify);
                        }
                        _selectedFiles = selectedFiles;
                        if (selectedFiles != null)
                            argumentProvider.SetArg("SelectedPrimaryFiles", selectedFiles);
                    }
                }
            }

            //IEnvironmentVarProvider varPrd = argumentProvider.EnvironmentVarProvider;
        }

        private SubProductInstanceDef TryGetInstanceFromOutFileIdentify()
        {
            if (_activeMonitoringSubProduct == null || _activeMonitoringSubProduct.ArgumentProvider == null)
                return null;
            string instanceIdentify = _activeMonitoringSubProduct.ArgumentProvider.GetArg("OutFileIdentify") as string;
            if (!string.IsNullOrWhiteSpace(instanceIdentify))
            {
                SubProductDef subProductDef = _activeMonitoringSubProduct.Definition;
                if (subProductDef == null || subProductDef.SubProductInstanceDefs == null)
                    return null;
                foreach (SubProductInstanceDef instance in subProductDef.SubProductInstanceDefs)
                {
                    if (instance.OutFileIdentify == instanceIdentify)
                    {
                        SubProductInstanceDef instance2 = instance;
                        string themeGraphTemplateName = _activeMonitoringSubProduct.ArgumentProvider.GetArg("HEAThemeGraphTemplateName") as string;
                        if (!string.IsNullOrWhiteSpace(themeGraphTemplateName))
                            instance2.LayoutName = themeGraphTemplateName;
                        return instance;
                    }
                }
            }
            return null;
        }

        public void DoAutoExtract()
        {
            DoAutoExtract(true);
        }

        private void CallPanelExtractWithoutUI()
        {
            ////注意：创建面板过程中，会将OutIdentify参数设置为子产品的，现在如果是做的实例的话，需要修正回来。
            using (ExtractPanelWindowContent ui = new ExtractPanelWindowContent())
            {
                ui.Apply(_session);
                if (_instanceDef == null && _activeMonitoringSubProduct.Definition.IsDisplayPanel)
                {
                    ui.Apply(Workspace, _activeMonitoringSubProduct);
                }
                else
                    ui.ApplyWithoutPanel(Workspace, _activeMonitoringSubProduct);
                //去掉该设置，使用这种情况下，不加载面板，
                //if (_instanceDef != null)
                //{
                //    _activeMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", _instanceDef.OutFileIdentify);
                //    _activeMonitoringSubProduct.ArgumentProvider.SetArg("SelectedPrimaryFiles", _selectedFiles);
                //}
                ui.DoExtract(true);
            }
        }

        public void DoManualExtract(bool isOpenWorkspace)
        {
            if (_activeMonitoringSubProduct == null)
                return;
            SetCurrentCanvasViewer();
            if (isOpenWorkspace)
                OpenAndFillWorkspace();

            TrySetInstanceArgs();

            OpenAndFillExtractPanel();
            StartExtractingSession();
        }

        private void SetCurrentCanvasViewer()
        {
            if (_currentCanvasViewer != _session.SmartWindowManager.ActiveCanvasViewer)
                _currentCanvasViewer = _session.SmartWindowManager.ActiveCanvasViewer;
        }

        public void DoManualExtract()
        {
            DoManualExtract(true);
        }

        private void StartExtractingSession()
        {
            if (_activeMonitoringSubProduct.Definition.IsNeedCurrentRaster)
            {
                _extractingSession.Start(_session.SmartWindowManager.ActiveCanvasViewer, _activeMonitoringProduct, _activeMonitoringSubProduct);
            }
        }

        private void OpenAndFillExtractPanel()
        {
            if (!_activeMonitoringSubProduct.Definition.IsDisplayPanel)
                return;
            _currentCanvasViewer = null;
            if (_activeMonitoringSubProduct.Definition.IsNeedCurrentRaster)
            {
                if (!(CurrentRasterIsOK()))
                    return;
                _currentCanvasViewer = _session.SmartWindowManager.ActiveCanvasViewer;
            }
            IExtractPanelWindow ep = ExtractPanelWidow;
            if (ep == null)
            {
                //打开判识面板
                ICommand cmd = _session.CommandEnvironment.Get(9019);
                if (cmd != null)
                    cmd.Execute();
            }
            ep = ExtractPanelWidow;
            if (ep == null)
                return;
            (ep as ISmartWindow).OnWindowClosed += new EventHandler((sender, e) => { OnCloseExtractPanel(); });
            if (_activeMonitoringProduct != null)
            {
                if (_activeMonitoringSubProduct != null)
                    ep.Apply(Workspace, _activeMonitoringSubProduct);
                else
                    ep.Apply(Workspace, null);
            }
        }

        private bool CurrentRasterIsOK()
        {
            ICanvasViewer cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return false;
            return (cv.ActiveObject is IRasterDrawing);
        }

        private void OnCloseExtractPanel()
        {
            if (_extractingSession.IsNeedSave())
            {
                if (MsgBox.ShowQuestionYesNo("当前判识结果未保存,要保存吗？\n按【是】保存，按【否】放弃。") == System.Windows.Forms.DialogResult.Yes)
                {
                    _extractingSession.AddToWorkspace(Workspace);
                }
                _extractingSession.Stop();
                _activeMonitoringSubProduct = null;
            }
        }

        private void OpenAndFillWorkspace()
        {
            IWorkspace wks = Workspace;
            if (wks == null)
            {
                //打开工作空间
                ICommand cmd = _session.CommandEnvironment.Get(9020);
                if (cmd != null)
                    cmd.Execute();
            }
            wks = Workspace;
            if (_activeMonitoringProduct != null)
                wks.Apply(_activeMonitoringProduct.Identify);
            else
                wks.Apply(null);
        }

        public IWorkspace GetWorkspace()
        {
            IWorkspace wks = Workspace;
            if (wks == null)
            {
                ICommand cmd = _session.CommandEnvironment.Get(9020);
                if (cmd != null)
                    cmd.Execute();
            }
            return Workspace;
        }

        public IWorkspace Workspace
        {
            get
            {
                ISmartWindow wnd = _session.SmartWindowManager.GetSmartWindow((w) => { return w is WorkspaceWindow; });
                if (wnd == null)
                    return null;
                return (wnd as WorkspaceWindow).Workspace;
            }
        }

        public IExtractPanelWindow ExtractPanelWidow
        {
            get
            {
                ISmartWindow wnd = _session.SmartWindowManager.GetSmartWindow((w) => { return w is ExtractPanelWindow; });
                if (wnd == null)
                    return null;
                return wnd as IExtractPanelWindow;
            }
        }

        public IExtractingSession ExtractingSession
        {
            get { return _extractingSession; }
        }

        public IThemeGraphGenerator ThemeGraphGenerator
        {
            get { return _themeGraphGenerator; }
        }

        public IFileNameGenerator FileNameGenerator
        {
            get { return _filenameGenerator; }
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }

        object IEnvironmentVarProvider.GetVar(string varName)
        {
            ICanvasViewer cv;
            switch (varName)
            {
                case "CurrentRasterFile":
                    cv = _session.SmartWindowManager.ActiveCanvasViewer;
                    if (cv == null)
                        return null;
                    return (cv.ActiveObject as IRasterDrawing).DataProvider.fileName;
                case "CurrentDataProvider":
                    cv = _session.SmartWindowManager.ActiveCanvasViewer;
                    if (cv == null)
                        return null;
                    return (cv.ActiveObject as IRasterDrawing).DataProvider;
                case "ProgressTracker":
                    return _session.ProgressMonitorManager.DefaultProgressMonitor;
                case "AOI":
                    return GetAOI();
            }
            return null;
        }

        private object GetAOI()
        {
            ICanvasViewer cv;
            cv = _session.SmartWindowManager.ActiveCanvasViewer;
            if (cv == null)
                return null;
            return (cv as ICanvasViewer).AOIProvider.GetIndexes();
        }

        MonitoringProductLoadedHandler IMonitoringSessionEvents.OnMonitoringProductLoaded
        {
            get { return _monitoringProductLoaded; }
            set { _monitoringProductLoaded = value; }
        }

        MonitoringSubProductLoadedHandler IMonitoringSessionEvents.OnMonitoringSubProductLoaded
        {
            get { return _monitoringSubProductLoaded; }
            set { _monitoringSubProductLoaded = value; }
        }

        public IMonitoringProduct FindMonitoringProduct(string identify)
        {
            if (string.IsNullOrWhiteSpace(identify))
                return null;
            if (_monitoringProducts == null || _monitoringProducts.Length == 0)
                return null;
            for (int i = 0; i < _monitoringProducts.Length; i++)
            {
                if (_monitoringProducts[i].Identify == identify)
                    return _monitoringProducts[i];
            }
            return null;
        }
    }
}
