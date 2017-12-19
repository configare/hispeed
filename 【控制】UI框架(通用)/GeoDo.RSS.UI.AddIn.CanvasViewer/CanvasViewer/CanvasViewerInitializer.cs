using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    internal class CanvasViewerInitializer : IDisposable,
        ICanvasViewerMenuHandlerManager
    {
        protected ICanvas _canvas;
        internal IAOIContainerLayer _aoiContainer;
        internal ISelectedAOILayer _selectedAOILayer;
        private ISmartSession _session;
        private ICanvasViewerContextMenuHandler _contextMenuHandler;
        private ContextMenuStrip _rightMouseMenu = null; //右键菜单
        private string _currentPencilToolText = null;
        private List<ToolStripItem> _rightMousePencilToolMenu = null; //右键PencilTool菜单项
        private List<ToolStripItem> _rightMouseNormalMenus = null;    //右键常规菜单项
        private bool _ismultiAoi = false;       //如果为false,则只允许一个AOI。
        private IContextMenuToolbarManager _toolbarManager;
        private FloatToolBarLayer _floatToolBar = null;

        public CanvasViewerInitializer(ICanvas canvas, ISmartSession session)
        {
            _canvas = canvas;
            _session = session;
            InitIsMultiAoi();
            _canvas.Container.KeyUp += new KeyEventHandler(Container_KeyUp);
            _canvas.Container.MouseUp += new MouseEventHandler(Container_MouseUp);
            LoadSystemLayers();
            _contextMenuHandler = new CanvasContextMenuHandler(session, _aoiContainer, this as ICanvasViewerMenuHandlerManager);
            _toolbarManager = new ContextMenuToolbarManager(_session);
        }

        void Container_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ShowMenuItem(e.X, e.Y);
            }
        }

        #region 右键菜单

        private void ShowMenuItem(int x, int y)
        {
            InitRightMouseNormalMenus();
            InitRightMousePencilToolMenu();
            InitRightMouseMenu();
            _rightMouseMenu.Show(_canvas.Container, x, y);
        }

        private void InitRightMouseMenu()
        {
            if (_rightMouseMenu == null)
                _rightMouseMenu = new ContextMenuStrip();
            else
                _rightMouseMenu.Items.Clear();
            if (_rightMousePencilToolMenu != null && _rightMousePencilToolMenu.Count != 0)
            {
                foreach (ToolStripItem pencilMenu in _rightMousePencilToolMenu)
                {
                    _rightMouseMenu.Items.Add(pencilMenu);
                }
            }
            foreach (ToolStripItem menu in _rightMouseNormalMenus)
            {
                _rightMouseMenu.Items.Add(menu);
            }
        }
        //右键常规菜单
        private void InitRightMouseNormalMenus()
        {
            if (_rightMouseNormalMenus == null)
            {
                ToolStripMenuItem menuFitImage = new ToolStripMenuItem("全图显示");
                menuFitImage.Click += new EventHandler(menuFitImage_Click);
                ToolStripMenuItem menuOrginImage = new ToolStripMenuItem("实际像素");
                menuOrginImage.Click += new EventHandler(menuOrginImage_Click);
                ToolStripMenuItem menuFitWidth = new ToolStripMenuItem("适应宽度");
                menuFitWidth.Click += new EventHandler(menuFitWidth_Click);
                ToolStripMenuItem menuBands = new ToolStripMenuItem("波段选择");
                menuBands.Click += new EventHandler(menuBands_Click);
                ToolStripMenuItem menuPixed = new ToolStripMenuItem("像元信息");
                menuPixed.Click += new EventHandler(menuPixed_Click);
                ToolStripMenuItem menuLayermanager = new ToolStripMenuItem("层管理器");
                menuLayermanager.Click += new EventHandler(menuLayermanager_Click);
                ToolStripMenuItem menuRemoveAllShape = new ToolStripMenuItem("移除所有矢量");
                menuRemoveAllShape.Click += new EventHandler(menuRemoveAllShape_Click);
                _rightMouseNormalMenus = new List<ToolStripItem>();
                _rightMouseNormalMenus.Add(menuFitImage);
                _rightMouseNormalMenus.Add(menuOrginImage);
                _rightMouseNormalMenus.Add(menuFitWidth);
                _rightMouseNormalMenus.Add(new ToolStripSeparator());
                _rightMouseNormalMenus.Add(menuPixed);
                _rightMouseNormalMenus.Add(menuBands);
                _rightMouseNormalMenus.Add(menuLayermanager);
                _rightMouseNormalMenus.Add(new ToolStripSeparator());
                _rightMouseNormalMenus.Add(menuRemoveAllShape);
            }
        }

        //右键工具菜单
        private void InitRightMousePencilToolMenu()
        {
            if (_rightMousePencilToolMenu == null)
                _rightMousePencilToolMenu = new List<ToolStripItem>();
            else
            {
                _rightMousePencilToolMenu.ForEach((item) => { item.Click -= new EventHandler(canclePencilTool_Click); });
                _rightMousePencilToolMenu.Clear();
            }
            if (_canvas.CurrentViewControl is PencilToolLayer)
            {
                PencilToolLayer pencilTool = _canvas.CurrentViewControl as PencilToolLayer;
                string pencilToolAlias = _currentPencilToolText;
                if (pencilTool.PencilType == enumPencilType.ControlFreeCurve)
                    pencilToolAlias = string.IsNullOrWhiteSpace(pencilTool.Alias) ? "冰缘线绘制" : pencilTool.Alias;
                //停止当前激活的PencilTool
                ToolStripMenuItem canclePencilTool = new ToolStripMenuItem("停止[" + (string.IsNullOrWhiteSpace(pencilToolAlias) ? "绘制" : pencilToolAlias) + "]工具");
                canclePencilTool.Click += new EventHandler(canclePencilTool_Click);

                _rightMousePencilToolMenu.Add(canclePencilTool);
                _rightMousePencilToolMenu.Add(new ToolStripSeparator());
            }
        }

        void canclePencilTool_Click(object sender, EventArgs e)
        {
            TryResetPencilTool();
            _toolbarManager.Close();
        }

        private void TryResetPencilTool()
        {
            PencilToolLayer pencilTool = _canvas.CurrentViewControl as PencilToolLayer;
            if (pencilTool != null)
            {
            }
            _selectedAOILayer.Edit = false;
            _canvas.CurrentViewControl = new DefaultControlLayer();
            _currentPencilToolText = null;
        }

        void menuRemoveAllShape_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4050);
            if (cmd != null)
                cmd.Execute();
        }

        void menuPixed_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9000);
            if (cmd != null)
                cmd.Execute();
        }

        void menuBands_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9003);
            if (cmd != null)
                cmd.Execute();
        }

        void menuLayermanager_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9002);
            if (cmd != null)
                cmd.Execute();
        }
        //全图
        void menuFitImage_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(1004);
            if (cmd != null)
                cmd.Execute();
        }
        //原分辨率
        void menuOrginImage_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(1005);
            if (cmd != null)
                cmd.Execute("1");
        }
        //适应宽度
        void menuFitWidth_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(1007);
            if (cmd != null)
                cmd.Execute();
        }
        #endregion

        void Container_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                _canvas.CurrentViewControl = new DefaultControlLayer();
            if (e.KeyCode == Keys.A)
            {
                ICommand commd = _session.CommandEnvironment.Get(6633);
                if (commd != null)
                    commd.Execute();
            }
        }

        private void LoadSystemLayers()
        {
            //浮动工具栏
            IFloatToolBarLayer toolbar = new FloatToolBarLayer();
            toolbar.ToolItemClicked = new Action<FloatToolItem>(FloatToolItemClick);
            toolbar.IsAutoHide = false;
            FloatToolItem[] items = GetFloatToolItems();
            toolbar.ToolItems.AddRange(items);
            _canvas.LayerContainer.Layers.Add(toolbar as GeoDo.RSS.Core.DrawEngine.ILayer);
            _floatToolBar = toolbar as FloatToolBarLayer;
            //AOI容器
            _aoiContainer = new AOIContainerLayer();
            _aoiContainer.Color = Color.Red;
            _aoiContainer.LineWidth = 1;
            _aoiContainer.IsOnlyOneAOI = !_ismultiAoi;
            _canvas.LayerContainer.Layers.Add(_aoiContainer as GeoDo.RSS.Core.DrawEngine.ILayer);
            //SelectedAOI容器
            _selectedAOILayer = new SelectedAOILayer();
            _selectedAOILayer.Color = Color.Yellow;
            _selectedAOILayer.LineWidth = 1;
            _canvas.LayerContainer.Layers.Add(_selectedAOILayer as GeoDo.RSS.Core.DrawEngine.ILayer);
            //蒙板
            ILayer lyr = new MaskLayer();
            _canvas.LayerContainer.Layers.Add(lyr);
        }

        internal PencilToolLayer StartAOIDrawing(enumPencilType pencilType)
        {
            PencilToolLayer aoiLayer = new PencilToolLayer();
            aoiLayer.PencilType = pencilType;
            aoiLayer.PencilIsFinished = (result) =>
            {
                _aoiContainer.AddAOI(result);
                //按住Control键可连续绘制
                //if (Control.ModifierKeys != Keys.Control)
                //    _canvas.CurrentViewControl = new DefaultControlLayer();
            };
            _canvas.CurrentViewControl = aoiLayer;
            return aoiLayer;
        }

        internal void FloatToolItemClick(FloatToolItem item)
        {
            IPencilToolLayer aoiLayer = null;
            switch (item.Text)
            {
                case "绘制AOI":
                    _toolbarManager.Close();
                    aoiLayer = new PencilToolLayer();
                    aoiLayer.PencilType = enumPencilType.FreeCurve;
                    aoiLayer.PencilIsFinished = (result) =>
                    {
                        //if (!_ismultiAoi)
                        //    ClearAoi();
                        _aoiContainer.AddAOI(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //  _canvas.CurrentViewControl = new DefaultControlLayer();
                    };
                    _canvas.CurrentViewControl = aoiLayer;
                    CloseOtherLayerEdit();
                    break;
                case "绘制AOI(矩形)":
                    _toolbarManager.Close();
                    aoiLayer = new PencilToolLayer();
                    aoiLayer.PencilType = enumPencilType.Rectangle;
                    aoiLayer.PencilIsFinished = (result) =>
                    {
                        //if (!_ismultiAoi)
                        //    ClearAoi();
                        _aoiContainer.AddAOI(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //_canvas.CurrentViewControl = new DefaultControlLayer();
                    };
                    _canvas.CurrentViewControl = aoiLayer;
                    CloseOtherLayerEdit();
                    break;
                case "绘制AOI(多边形)":
                    _toolbarManager.Close();
                    aoiLayer = new PencilToolLayer();
                    aoiLayer.PencilType = enumPencilType.Polygon;
                    aoiLayer.PencilIsFinished = (result) =>
                    {
                        //if (!_ismultiAoi)
                        //    ClearAoi();
                        _aoiContainer.AddAOI(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //    _canvas.CurrentViewControl = new DefaultControlLayer();
                    };
                    _canvas.CurrentViewControl = aoiLayer;
                    CloseOtherLayerEdit();
                    break;
                case "绘制AOI(圆形)":
                    _toolbarManager.Close();
                    aoiLayer = new PencilToolLayer();
                    aoiLayer.PencilType = enumPencilType.Circle;
                    aoiLayer.PencilIsFinished = (result) =>
                    {
                        //if (!_ismultiAoi)
                        //    ClearAoi();
                        _aoiContainer.AddAOI(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //    _canvas.CurrentViewControl = new DefaultControlLayer();
                    };
                    _canvas.CurrentViewControl = aoiLayer;
                    CloseOtherLayerEdit();
                    break;
                case "导入AOI":
                    _toolbarManager.Close();
                    _canvas.CurrentViewControl = new PencilToolLayer();
                    if (_contextMenuHandler.HandleImportAOI())
                        _canvas.Refresh(enumRefreshType.All);
                    _canvas.CurrentViewControl = new DefaultControlLayer();
                    CloseOtherLayerEdit();
                    break;
                case "交互选择AOI":
                    _toolbarManager.Close();
                    aoiLayer = new PencilToolLayer();
                    aoiLayer.PencilType = enumPencilType.Rectangle;
                    _selectedAOILayer.AOIContaingerLayer = _aoiContainer;
                    _selectedAOILayer.Edit = true;
                    aoiLayer.PencilIsFinished = (result) =>
                    {
                        if (_contextMenuHandler.HandleSelectAOIFromFeatures(result))
                        {
                            _canvas.CurrentViewControl = new DefaultControlLayer();
                            _canvas.Refresh(enumRefreshType.All);
                        }
                    };
                    _canvas.CurrentViewControl = aoiLayer;
                    break;
                case "删除AOI区域":
                    _toolbarManager.Close();
                    ClearAoiAndResetCurrentViewControl();
                    break;
                case "魔术棒":
                    _toolbarManager.Close();
                    IPencilToolLayer magicLayer = new PencilToolLayer();
                    _canvas.CurrentViewControl = magicLayer;
                    magicLayer.PencilIsFinished = (result) =>
                    {
                        _contextMenuHandler.HandleAdsorb(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //    _canvas.CurrentViewControl = new DefaultControlLayer();
                        _canvas.Refresh(enumRefreshType.All);
                    };
                    CloseOtherLayerEdit();
                    break;
                case "魔术棒判识":
                    _toolbarManager.Close();
                    IContextMenuArgProvider argProvider = _toolbarManager.Show(_contextMenuHandler.GetArgProviderUI(enumCanvasViewerMenu.MagicWand));
                    if (argProvider == null)
                        return;
                    IPencilToolLayer magicWandLayer = new PencilToolLayer();
                    magicWandLayer.PencilType = enumPencilType.Point;
                    _canvas.CurrentViewControl = magicWandLayer;
                    ContextMenuToolbarManager barManager = (_toolbarManager as ContextMenuToolbarManager);
                    if (barManager != null)
                        barManager.ucDispose = new ContextMenuToolbarManager.UCDisposeDeleg(UCDispose);
                    magicWandLayer.PencilIsFinished = (result) =>
                    {
                        if (IsFitOtherToolItem(item))
                            return;
                        Dictionary<string, object> args = new Dictionary<string, object>();
                        args.Add("tolerance", argProvider.GetArg("tolerance"));
                        args.Add("iscontinued", argProvider.GetArg("iscontinued"));
                        //获取参数
                        _contextMenuHandler.HandleMagicWandExtracting(result, args);
                        _currentPencilToolText = item.Text;
                        //_canvas.CurrentViewControl = new DefaultControlLayer();
                    };
                    CloseOtherLayerEdit();
                    break;
                case "橡皮檫":
                    _toolbarManager.Close();
                    IPencilToolLayer earseLayer = new PencilToolLayer();
                    _canvas.CurrentViewControl = earseLayer;
                    earseLayer.PencilIsFinished = (result) =>
                    {
                        _contextMenuHandler.HandleErase(result);
                        _currentPencilToolText = item.Text;
                        //if (Control.ModifierKeys != Keys.Control)
                        //    _canvas.CurrentViewControl = new DefaultControlLayer();
                        _canvas.Refresh(enumRefreshType.All);
                    };
                    CloseOtherLayerEdit();
                    break;
                case "闪烁":
                    _contextMenuHandler.HandleFlash();
                    CloseOtherLayerEdit();
                    break;
                case "撤销":
                    _contextMenuHandler.HandleUnDo();
                    CloseOtherLayerEdit();
                    break;
                case "重做":
                    _contextMenuHandler.HandleReDo();
                    CloseOtherLayerEdit();
                    break;
                case "清除判识结果":
                    _contextMenuHandler.HandleRemoveAll();
                    CloseOtherLayerEdit();
                    break;
                case "多感兴趣区域":
                    TryResetPencilTool();
                    _ismultiAoi = !_ismultiAoi;
                    _aoiContainer.IsOnlyOneAOI = !_ismultiAoi;
                    item.Image = GetMultiAoiImage();
                    try
                    {
                        _canvas.Refresh(enumRefreshType.All);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    CloseOtherLayerEdit();
                    break;
            }
        }

        private bool IsFitOtherToolItem(FloatToolItem item)
        {
            if (_floatToolBar.CurrentToolItem != null && _floatToolBar.CurrentToolItem.Text != item.Text)
                return true;
            return false;
        }

        private void CloseOtherLayerEdit()
        {
            _selectedAOILayer.Edit = false;
        }

        private void UCDispose()
        {
            TryResetPencilTool();
        }

        private void InitIsMultiAoi()
        {
            try
            {
                IConfiger config = _session.Configer;
                object ismultiAoi = config.GetConfigItemValue("IsMultiAoi");
                if (ismultiAoi != null)
                {
                    _ismultiAoi = false;
                    bool.TryParse(ismultiAoi.ToString(), out _ismultiAoi);
                }
            }
            catch
            {
                _ismultiAoi = false;
            }
        }

        private System.Drawing.Image GetMultiAoiImage()
        {
            if (_ismultiAoi)
                return GetImageResource("Extracting_checked.png");
            else
                return GetImageResource("Extracting_uncheck.png");
        }

        private void ClearAoi()
        {
            IPencilToolLayer pencilLayer = _canvas.CurrentViewControl as IPencilToolLayer;
            if (pencilLayer != null)
            {
                pencilLayer.Reset();
            }
            _aoiContainer.Reset();
            _selectedAOILayer.Reset();
            _canvas.Refresh(enumRefreshType.All);
        }

        private void ClearAoiAndResetCurrentViewControl()
        {
            IPencilToolLayer pencilLayer = _canvas.CurrentViewControl as IPencilToolLayer;
            if (pencilLayer != null)
            {
                pencilLayer.Reset();
                _canvas.CurrentViewControl = new DefaultControlLayer();
            }
            if (_selectedAOILayer.AOIs != null && _selectedAOILayer.AOIs.Count() != 0)
            {
                _aoiContainer.RemoveSelectedAOI(_selectedAOILayer.AOIs.ToArray());
                _selectedAOILayer.Reset();
            }
            else
            {
                _aoiContainer.Reset();
                _selectedAOILayer.Reset();
            }
            _canvas.Refresh(enumRefreshType.All);
        }

        private FloatToolItem[] GetFloatToolItems()
        {
            return new FloatToolItem[] 
            {
                new FloatToolItem("多感兴趣区域",GetMultiAoiImage()),
                new FloatToolItem("绘制AOI",GetImageResource("Extracting_笔.png")),
                new FloatToolItem("绘制AOI(矩形)",GetImageResource("Extracting_矩形.png")),
                new FloatToolItem("绘制AOI(多边形)",GetImageResource("Extracting_多边形.png")),
                new FloatToolItem("绘制AOI(圆形)",GetImageResource("Extracting_圆形.png")),
                new FloatToolItem("导入AOI",GetImageResource("Extracting_导入AOI.png")),
                //new FloatToolItem("输入AOI",GetImageResource("Extracting_输入AOI.png")),
                new FloatToolItem("交互选择AOI",GetImageResource("Extracting_AOIArrow.png")),
                new FloatToolItem("-"),
                new FloatToolItem("删除AOI区域",GetImageResource("Extracting_删除AOI.png")),
                new FloatToolItem("-"),
                new FloatToolItem("魔术棒",GetImageResource("Extracting_魔术棒.png")),
                new FloatToolItem("魔术棒判识",GetImageResource("Extracting_魔术棒判识.png")),
                new FloatToolItem("橡皮檫",GetImageResource("Extracting_橡皮檫.png")),
                new FloatToolItem("闪烁",GetImageResource("Extracting_闪烁.png")),
                //new FloatToolItem("撤销",GetImageResource("Extracting_撤销.png")),
                //new FloatToolItem("重做",GetImageResource("Extracting_重做.png")),
                new FloatToolItem("-"),
                new FloatToolItem("清除判识结果",GetImageResource("Extracting_清除判识结果.png"))
            };
        }

        private System.Drawing.Image GetImageResource(string resName)
        {
            return _session.UIFrameworkHelper.GetImage("system:" + resName);
        }

        public void Dispose()
        {
            _rightMouseMenu = null;
            _rightMouseNormalMenus = null;
            _rightMousePencilToolMenu = null;
            _contextMenuHandlers.Clear();
            if (_canvas != null)
            {
                _canvas.Container.KeyUp -= new KeyEventHandler(Container_KeyUp);
                _canvas.Container.MouseUp -= new MouseEventHandler(Container_MouseUp);
                _canvas = null;
            }
            if (_aoiContainer != null)
            {
                (_aoiContainer as AOIContainerLayer).Dispose();
                _aoiContainer = null;
            }
            if (_selectedAOILayer != null)
            {
                (_selectedAOILayer as SelectedAOILayer).Dispose();
                _selectedAOILayer = null;
            }
            if (_session != null)
                _session = null;
            if (_contextMenuHandler != null)
            {
                (_contextMenuHandler as CanvasContextMenuHandler).HandleRemoveAll();
                _contextMenuHandler = null;
            }
            if (_toolbarManager != null)
            {
                _toolbarManager.Close();
                _toolbarManager = null;
            }
        }

        private List<object[]> _contextMenuHandlers = new List<object[]>();

        public IEnumerable<object[]> Handlers
        {
            get { return _contextMenuHandlers; }
        }

        public void Register(string identify, string menuItem, Action<object, Dictionary<string, object>> action, string argProviderUI)
        {
            _contextMenuHandlers.Add(new object[] { identify, menuItem, action, argProviderUI });
        }

        public void UnRegister(string identify)
        {
            _contextMenuHandlers.RemoveAll((obj) => { return obj[1].ToString() == identify; });
        }
    }
}
