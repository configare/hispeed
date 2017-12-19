using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.UI.AddIn.DataRef;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Core.Grid;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    public partial class UITabDataRef : UserControl, IUITabProvider
    {
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region dropdowns define
        //层管理器
        RadButtonElement _layerManager = null;
        //移除
        RadButtonElement _removeAll = null; //移除所有
        RadDropDownButtonElement _removeRef = null;  //移除引用
        //基础矢量
        RadDropDownButtonElement _adminRegion = null; //行政区划
        RadDropDownButtonElement _rivers = null;  //河流湖泊
        RadDropDownButtonElement _trafficline = null; //交通线
        RadDropDownButtonElement _lonlatGrid = null;  //经纬网格
        RadDropDownButtonElement _rasterMap = null;  //矢量地图
        RadButtonElement _saveToRasterMap = null;  //存为矢量地图
        RadButtonElement _otherRaster = null;  //其它矢量
        //关注区域
        RadDropDownButtonElement _newInterestArea = null; //新增关注区域
        RadDropDownButtonElement _bookMarkGroups = null; //关注区域
        RadDropDownButtonElement _seaiceInter = null;  //海冰关注区
        RadMenuItem _creatBookMarks = null;  //创建关注区
        RadMenuItem _manageBookMarks = null;  //管理关注区
        //标准分幅
        RadDropDownButtonElement _definedSplit = null; //已定义分幅
        RadButtonElement _newSplit = null;  //新增分幅
        //常规观测数据
        RadDropDownButtonElement _specialData = null; // 5月13号16时数据
        RadButtonElement _dataSourceCon = null;  //数据源配置
        //更多数据源
        RadButtonElement _addDatasource = null; //添加数据源
        //AOI模版
        RadDropDownButtonElement _addAOITemplate = null;
        //AOI=>蒙板
        RadButtonElement _aoi2maskButton = null;
        RadButtonElement _removeMaskLayer;
        RadButtonElement _aoi2shp;
        //地图背景
        RadButtonElement _btnApplyDefatultBkgrdLayer;
        RadButtonElement _btnRemoveDefatultBkgrdLayer;
        #endregion

        public UITabDataRef()
        {
            InitializeComponent();
            CreatTab();
        }
        #region Creat UI framework
        private void CreatTab()
        {
            _tab = new RibbonTab();
            _tab.Title = "地理信息";
            _tab.Text = _tab.Name = "地理信息";

            _layerManager = new RadButtonElement("图层管理器");
            _layerManager.ImageAlignment = ContentAlignment.TopCenter;
            _layerManager.TextAlignment = ContentAlignment.MiddleCenter;
            _layerManager.TextImageRelation = TextImageRelation.ImageAboveText;
            _layerManager.Click += new EventHandler(_layerManager_Click);

            RadRibbonBarGroup group = new RadRibbonBarGroup();
            group.Text = "图层管理器";
            group.Items.Add(_layerManager);
            _tab.Items.Add(group);

            CreatDeleteGroup();
            CreatBasicRasterGroup();
            CreatInterestAreaGroup();
            CreatSplitGroup();
            CreatDataGroup();
            CreatDataSouceGroup();
            CreatAOITemplateGroup();
            CreateBackgroundLayersGroup();
        }

        private void CreatDeleteGroup()
        {
            //RadMenuItem basic = new RadMenuItem("基础矢量");
            //RadMenuItem inters = new RadMenuItem("关注区域");
            //RadMenuItem split = new RadMenuItem("标准分幅");
            //RadMenuItem data = new RadMenuItem("常规观测数据");

            _removeAll = new RadButtonElement();
            _removeAll.Text = "移除所有";
            _removeAll.ImageAlignment = ContentAlignment.TopCenter;
            _removeAll.TextAlignment = ContentAlignment.MiddleCenter;
            _removeAll.TextImageRelation = TextImageRelation.ImageAboveText;
            _removeAll.Click += new EventHandler(_removeAll_Click);

            _removeRef = new RadDropDownButtonElement();
            _removeRef.Name = "DeleteRef";
            _removeRef.Text = "移除引用";
            _removeRef.ImageAlignment = ContentAlignment.TopCenter;
            _removeRef.TextAlignment = ContentAlignment.MiddleCenter;
            _removeRef.TextImageRelation = TextImageRelation.ImageAboveText;
            _removeRef.DropDownDirection = RadDirection.Down;
            _removeRef.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _removeRef.Click += new EventHandler(_removeRef_Click);


            //basic.Click += new EventHandler(basic_Click);
            //inters.Click += new EventHandler(inters_Click);
            //split.Click += new EventHandler(split_Click);
            //data.Click += new EventHandler(data_Click);
            //_removeRef.Items.AddRange(new RadItem[] { basic, inters, split });

            RadRibbonBarGroup group1 = new RadRibbonBarGroup();
            group1.Name = "Delete";
            group1.Text = "移除";
            group1.Items.AddRange(new RadItem[] { _removeAll, _removeRef });
            _tab.Items.Add(group1);
        }

        #region 分类移除引用
        void _removeRef_Click(object sender, EventArgs e)
        {
            _removeRef.Items.Clear();
            if (_session == null)
                return;
            ILayoutHost host = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            if (canvas == null)
                return;
            AddGridLayerName(canvas);
            //矢量层
            AddVectorLayerName(canvas);
            _removeRef.ShowDropDown();
        }

        private void AddVectorLayerName(ICanvas canvas)
        {
            IVectorHostLayer vectorHost = canvas.LayerContainer.VectorHost;
            if (vectorHost == null)
                return;
            Map map = vectorHost.Map as Map;
            if (map == null)
                return;
            CodeCell.AgileMap.Core.ILayer[] layers = map.LayerContainer.Layers;
            if (layers == null || layers.Length == 0)
                return;
            foreach (CodeCell.AgileMap.Core.ILayer layer in layers)
            {
                if (layer == null)
                    continue;
                CreatItemByLayerName(layer.Name);
            }
        }

        //经纬网格
        private void AddGridLayerName(ICanvas canvas)
        {
            Layer gridLayer = null;
            foreach (Layer layer in canvas.LayerContainer.Layers)
            {
                if ((layer as GeoGridLayer) != null)
                {
                    gridLayer = layer;
                    RadMenuItem itemLayer = new RadMenuItem(gridLayer.Name);
                    itemLayer.Click += new EventHandler(itemLayer_Click);
                    itemLayer.Tag = gridLayer;
                    _removeRef.Items.Add(itemLayer);
                }
            }
        }

        private ICanvas GetCanvasFromSession(ref ILayoutHost host)
        {
            if (_session.SmartWindowManager.ActiveViewer is ICanvasViewer)
                return GetCanvasByCanvasViewer();
            else if (_session.SmartWindowManager.ActiveViewer is ILayoutViewer)
                return GetCanvasByLayoutDataFrame(ref host);
            return null;
        }

        private ICanvas GetCanvasByLayoutDataFrame(ref ILayoutHost host)
        {
            ILayoutViewer view = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return null;
            host = view.LayoutHost;
            if (host == null)
                return null;
            IDataFrame dataFrame = host.ActiveDataFrame;
            if (dataFrame == null)
                return null;
            return (dataFrame.Provider as IDataFrameDataProvider).Canvas;
        }

        private ICanvas GetCanvasByCanvasViewer()
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            return viewer.Canvas;
        }

        private void CreatItemByLayerName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            RadMenuItem itemLayer = new RadMenuItem(name);
            itemLayer.Click += new EventHandler(itemLayer_Click);
            itemLayer.Tag = name;
            if (_session != null && _session.UIFrameworkHelper != null)
                itemLayer.Image = _session.UIFrameworkHelper.GetImage("system:cmdDelete.png");
            _removeRef.Items.Add(itemLayer);
        }

        //移除选中层
        void itemLayer_Click(object sender, EventArgs e)
        {
            RadMenuItem item = sender as RadMenuItem;
            if (item == null)
                return;
            ILayoutHost host = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            if (canvas == null)
                return;
            if ((item.Tag as GeoGridLayer) != null)//经纬网格
            {
                RemoveGridLayer(canvas);
            }
            else // 矢量层
            {
                RemoveVectorLayer(canvas, item);
            }
            _removeRef.Items.Clear();
            canvas.Refresh(enumRefreshType.VectorLayer);
            if (host != null)
                host.Render(true);
            RefreshLayerManager();
        }


        private void RemoveVectorLayer(ICanvas canvas, RadMenuItem item)
        {
            IVectorHostLayer vectorHost = canvas.LayerContainer.VectorHost;
            if (vectorHost == null)
                return;
            Map map = vectorHost.Map as Map;
            if (map == null)
                return;
            CodeCell.AgileMap.Core.ILayer[] layers = map.LayerContainer.Layers;
            if (layers == null || layers.Length == 0)
                return;
            CodeCell.AgileMap.Core.ILayer removeLayer = null;
            foreach (CodeCell.AgileMap.Core.ILayer layer in layers)
            {
                if (layer.Name == item.Tag.ToString())
                    removeLayer = layer;
            }
            if (removeLayer != null)
                (vectorHost.Map as Map).LayerContainer.Remove(removeLayer);

        }

        private void RefreshLayerManager()
        {
            ISmartWindow layerManager = _session.SmartWindowManager.GetSmartWindow((wnd) => { return wnd is ILayerManager; });
            if (layerManager != null)
                (layerManager as ILayerManager).Update();
        }

        private void RemoveGridLayer(ICanvas canvas)
        {
            Layer gridLayer = null;
            foreach (Layer layer in canvas.LayerContainer.Layers)
            {
                if ((layer as GeoGridLayer) != null)
                    gridLayer = layer;
            }
            if (gridLayer != null)
                canvas.LayerContainer.Layers.Remove(gridLayer);
        }
        #endregion

        private void CreatBasicRasterGroup()
        {
            _adminRegion = new RadDropDownButtonElement();
            _adminRegion.Text = "行政区划";
            _adminRegion.ImageAlignment = ContentAlignment.TopCenter;
            _adminRegion.TextAlignment = ContentAlignment.MiddleCenter;
            _adminRegion.TextImageRelation = TextImageRelation.ImageAboveText;
            _adminRegion.DropDownDirection = RadDirection.Down;
            _adminRegion.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _adminRegion.Click += new EventHandler(_adminRegion_Click);

             _trafficline = new RadDropDownButtonElement();
             _trafficline.Text = "交通线";
             _trafficline.ImageAlignment = ContentAlignment.TopCenter;
             _trafficline.TextAlignment = ContentAlignment.MiddleCenter;
             _trafficline.TextImageRelation = TextImageRelation.ImageAboveText;
             _trafficline.DropDownDirection = RadDirection.Down;
             _trafficline.ArrowPosition = DropDownButtonArrowPosition.Bottom;
             _trafficline.Click += new EventHandler(_trafficline_Click);
           

            _rivers = new RadDropDownButtonElement();
            _rivers.Text = "河流湖泊";
            _rivers.ImageAlignment = ContentAlignment.TopCenter;
            _rivers.TextAlignment = ContentAlignment.MiddleCenter;
            _rivers.TextImageRelation = TextImageRelation.ImageAboveText;
            _rivers.DropDownDirection = RadDirection.Down;
            _rivers.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _rivers.Click += new EventHandler(_rivers_Click);

            RadMenuItem item1 = new RadMenuItem("1度网格");
            item1.Click += new EventHandler(item1_Click);
            RadMenuItem item2 = new RadMenuItem("5度网格");
            item2.Click += new EventHandler(item2_Click);
            RadMenuItem item3 = new RadMenuItem("10度网格");
            item3.Click += new EventHandler(item3_Click);
            RadMenuItem item4 = new RadMenuItem("自定义");
            item4.Click += new EventHandler(item4_Click);
            _lonlatGrid = new RadDropDownButtonElement();
            _lonlatGrid.Text = "经纬网格";
            _lonlatGrid.ImageAlignment = ContentAlignment.TopCenter;
            _lonlatGrid.TextAlignment = ContentAlignment.MiddleCenter;
            _lonlatGrid.TextImageRelation = TextImageRelation.ImageAboveText;
            _lonlatGrid.DropDownDirection = RadDirection.Down;
            _lonlatGrid.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _lonlatGrid.Items.AddRange(new RadItem[] { item1, item2, item3, item4 });

            _rasterMap = new RadDropDownButtonElement();
            _rasterMap.Text = "矢量地图";
            _rasterMap.ImageAlignment = ContentAlignment.TopCenter;
            _rasterMap.TextAlignment = ContentAlignment.MiddleCenter;
            _rasterMap.TextImageRelation = TextImageRelation.ImageAboveText;
            _rasterMap.DropDownDirection = RadDirection.Down;
            _rasterMap.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            CreatItemsByFolders("\\基础矢量\\矢量地图", _rasterMap, true);

            _saveToRasterMap = new RadButtonElement("存为矢量地图");
            _saveToRasterMap.ImageAlignment = ContentAlignment.TopCenter;
            _saveToRasterMap.TextAlignment = ContentAlignment.MiddleCenter;
            _saveToRasterMap.TextImageRelation = TextImageRelation.ImageAboveText;
            _saveToRasterMap.Click += new EventHandler(_saveToRasterMap_Click);

            _otherRaster = new RadButtonElement();
            _otherRaster.Text = "其它矢量";
            _otherRaster.ImageAlignment = ContentAlignment.TopCenter;
            _otherRaster.TextAlignment = ContentAlignment.MiddleCenter;
            _otherRaster.TextImageRelation = TextImageRelation.ImageAboveText;
            _otherRaster.Click += new EventHandler(_otherRaster_Click);

            RadRibbonBarGroup group2 = new RadRibbonBarGroup();
            group2.Text = "基础矢量";
            group2.Items.AddRange(new RadItem[] { _adminRegion, _rivers,  _trafficline, _lonlatGrid, _rasterMap, _otherRaster });

            _tab.Items.Add(group2);
        }

        void _rivers_Click(object sender, EventArgs e)
        {
            UpdateAllLoadShp();
            _rivers.Items.Clear();
            CreatItemsByFolders("\\基础矢量\\河流湖泊", _rivers, false);
            _rivers.ShowDropDown();
        }

        void _trafficline_Click(object sender, EventArgs e)
        {
            UpdateAllLoadShp();
            _trafficline.Items.Clear();
            CreatItemsByFolders("\\基础矢量\\交通信息", _trafficline, false);
            _trafficline.ShowDropDown();
        }


        void _adminRegion_Click(object sender, EventArgs e)
        {
            UpdateAllLoadShp();
            _adminRegion.Items.Clear();
            CreatItemsByFolders("\\基础矢量\\行政区划", _adminRegion, false);
            _adminRegion.ShowDropDown();
        }

        private void CreatInterestAreaGroup()
        {
            _bookMarkGroups = new RadDropDownButtonElement();
            _bookMarkGroups.Text = "关注区域";
            //_bookMarkGroups.Text = "重点水域";
            _bookMarkGroups.ImageAlignment = ContentAlignment.TopCenter;
            _bookMarkGroups.TextAlignment = ContentAlignment.MiddleCenter;
            _bookMarkGroups.TextImageRelation = TextImageRelation.ImageAboveText;
            _bookMarkGroups.DropDownDirection = RadDirection.Down;
            _bookMarkGroups.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            // CreatItemsByFolders("\\关注区域\\重点水域", _mainRivers, false);
            _bookMarkGroups.Click += new EventHandler(_groups_Click);

            //_seaiceInter = new RadDropDownButtonElement();
            //_seaiceInter.Text = "海冰关注区";
            //_seaiceInter.ImageAlignment = ContentAlignment.TopCenter;
            //_seaiceInter.TextAlignment = ContentAlignment.MiddleCenter;
            //_seaiceInter.TextImageRelation = TextImageRelation.ImageAboveText;
            //_seaiceInter.DropDownDirection = RadDirection.Down;
            //_seaiceInter.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            ////CreatItemsByFolders("\\关注区域\\海冰关注区", _seaiceInter, false);
            //_seaiceInter.Click += new EventHandler(_seaiceInter_Click);

            _newInterestArea = new RadDropDownButtonElement();
            _newInterestArea.Text = "管理关注区域";
            _newInterestArea.ImageAlignment = ContentAlignment.TopCenter;
            _newInterestArea.TextAlignment = ContentAlignment.MiddleCenter;
            _newInterestArea.TextImageRelation = TextImageRelation.ImageAboveText;
            _newInterestArea.DropDownDirection = RadDirection.Down;
            _newInterestArea.ArrowPosition = DropDownButtonArrowPosition.Bottom;

            _creatBookMarks = new RadMenuItem("创建关注区");
            _creatBookMarks.Click += new EventHandler(newInter_Click);
            _manageBookMarks = new RadMenuItem("管理关注区");
            _manageBookMarks.Click += new EventHandler(_manageBookMarks_Click);
            _newInterestArea.Items.AddRange(new RadItem[] { _creatBookMarks, _manageBookMarks });

            RadRibbonBarGroup group3 = new RadRibbonBarGroup();
            group3.Text = "关注区域";
            //group3.Items.AddRange(new RadItem[] { _newInterestArea, _mainRivers, _seaiceInter });
            group3.Items.AddRange(new RadItem[] { _bookMarkGroups,/*_seaiceInter,*/_newInterestArea });
            _tab.Items.Add(group3);
        }

        private void CreatSplitGroup()
        {
            _definedSplit = new RadDropDownButtonElement();
            _definedSplit.Text = "已定义分幅";
            _definedSplit.ImageAlignment = ContentAlignment.TopCenter;
            _definedSplit.TextAlignment = ContentAlignment.MiddleCenter;
            _definedSplit.TextImageRelation = TextImageRelation.ImageAboveText;
            _definedSplit.DropDownDirection = RadDirection.Down;
            _definedSplit.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            CreatItemsByFolders("\\标准分幅", _definedSplit, false);

            _newSplit = new RadButtonElement();
            _newSplit.Text = "新增分幅";
            _newSplit.ImageAlignment = ContentAlignment.TopCenter;
            _newSplit.TextAlignment = ContentAlignment.MiddleCenter;
            _newSplit.TextImageRelation = TextImageRelation.ImageAboveText;

            RadRibbonBarGroup group4 = new RadRibbonBarGroup();
            group4.Text = "标准分幅";
            group4.Items.AddRange(new RadItem[] { _definedSplit/*_newSplit*/ });
            _tab.Items.Add(group4);
        }

        private void CreatDataGroup()
        {
            _specialData = new RadDropDownButtonElement();
            _specialData.Text = "5月3号16时数据";
            _specialData.ImageAlignment = ContentAlignment.TopCenter;
            _specialData.TextAlignment = ContentAlignment.MiddleCenter;
            _specialData.TextImageRelation = TextImageRelation.ImageAboveText;
            _specialData.DropDownDirection = RadDirection.Down;
            _specialData.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            CreatItemsByFolders("\\常规观测数据\\5月3号16时数据", _specialData, false);

            _dataSourceCon = new RadButtonElement();
            _dataSourceCon.Text = "数据源配置";
            _dataSourceCon.ImageAlignment = ContentAlignment.TopCenter;
            _dataSourceCon.TextAlignment = ContentAlignment.MiddleCenter;
            _dataSourceCon.TextImageRelation = TextImageRelation.ImageAboveText;

            RadRibbonBarGroup group5 = new RadRibbonBarGroup();
            group5.Text = "常规观测数据";
            group5.Items.AddRange(new RadItem[] { _specialData, _dataSourceCon });

            //_tab.Items.Add(group5);
        }

        private void CreatDataSouceGroup()
        {
            _addDatasource = new RadButtonElement();
            _addDatasource.Text = "添加数据源";
            _addDatasource.ImageAlignment = ContentAlignment.TopCenter;
            _addDatasource.TextAlignment = ContentAlignment.MiddleCenter;
            _addDatasource.TextImageRelation = TextImageRelation.ImageAboveText;
            RadRibbonBarGroup group6 = new RadRibbonBarGroup();
            group6.Text = "更多数据源";
            group6.Items.AddRange(new RadItem[] { _addDatasource });

            //_tab.Items.Add(group6);
        }

        private void CreatAOITemplateGroup()
        {
            _addAOITemplate = new RadDropDownButtonElement();
            _addAOITemplate.Text = "省级行政区";
            _addAOITemplate.ImageAlignment = ContentAlignment.TopCenter;
            _addAOITemplate.TextAlignment = ContentAlignment.MiddleCenter;
            _addAOITemplate.TextImageRelation = TextImageRelation.ImageAboveText;
            _addAOITemplate.DropDownDirection = RadDirection.Down;
            _addAOITemplate.ArrowPosition = DropDownButtonArrowPosition.Bottom;

            CreatAOITemplateItemsByFile(_addAOITemplate);

            RadRibbonBarGroup group7 = new RadRibbonBarGroup();
            group7.Text = "AOI模版";
            group7.Items.Add(_addAOITemplate);
            //
            _aoi2maskButton = new RadButtonElement("AOI=>蒙板");
            _aoi2maskButton.Click += new EventHandler(_aoi2maskButton_Click);
            _aoi2maskButton.ImageAlignment = ContentAlignment.TopCenter;
            _aoi2maskButton.TextAlignment = ContentAlignment.MiddleCenter;
            _aoi2maskButton.TextImageRelation = TextImageRelation.ImageAboveText;
            group7.Items.Add(_aoi2maskButton);
            //
            _removeMaskLayer = new RadButtonElement("取消蒙板");
            _removeMaskLayer.Click += new EventHandler(_removeMaskLayer_Click);
            _removeMaskLayer.ImageAlignment = ContentAlignment.TopCenter;
            _removeMaskLayer.TextAlignment = ContentAlignment.MiddleCenter;
            _removeMaskLayer.TextImageRelation = TextImageRelation.ImageAboveText;
            group7.Items.Add(_removeMaskLayer);
            //
            _aoi2shp = new RadButtonElement("AOI->矢量");
            _aoi2shp.Click += new EventHandler(_aoi2shp_Click);
            _aoi2shp.ImageAlignment = ContentAlignment.TopCenter;
            _aoi2shp.TextAlignment = ContentAlignment.MiddleCenter;
            _aoi2shp.TextImageRelation = TextImageRelation.ImageAboveText;
            group7.Items.Add(_aoi2shp);

            _tab.Items.Add(group7);
        }

        void _aoi2shp_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(7103);
            if (cmd != null)
                cmd.Execute();
        }

        private void CreateBackgroundLayersGroup()
        {
            RadRibbonBarGroup grp = new RadRibbonBarGroup();
            grp.Text = "地图背景";
            _btnApplyDefatultBkgrdLayer = new RadButtonElement("应用默认背景");
            _btnApplyDefatultBkgrdLayer.Click += new EventHandler(_btnApplyDefatultBkgrdLayer_Click);
            _btnApplyDefatultBkgrdLayer.ImageAlignment = ContentAlignment.TopCenter;
            _btnApplyDefatultBkgrdLayer.TextAlignment = ContentAlignment.MiddleCenter;
            _btnApplyDefatultBkgrdLayer.TextImageRelation = TextImageRelation.ImageAboveText;
            grp.Items.Add(_btnApplyDefatultBkgrdLayer);
            _btnRemoveDefatultBkgrdLayer = new RadButtonElement("取消默认背景");
            _btnRemoveDefatultBkgrdLayer.Click += new EventHandler(_btnRemoveDefatultBkgrdLayer_Click);
            grp.Items.Add(_btnRemoveDefatultBkgrdLayer);
            foreach (RadButtonElement item in grp.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.MiddleCenter;
                item.TextImageRelation = TextImageRelation.ImageAboveText;
            }
            _tab.Items.Add(grp);
        }

        void _btnApplyDefatultBkgrdLayer_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4044);
            if (cmd != null)
                cmd.Execute();
        }

        void _btnRemoveDefatultBkgrdLayer_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4045);
            if (cmd != null)
                cmd.Execute();
        }

        void _removeMaskLayer_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4043);
            if (cmd != null)
                cmd.Execute();
        }

        void _aoi2maskButton_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4042);
            if (cmd != null)
                cmd.Execute();
        }

        private void CreatAOITemplateItemsByFile(RadDropDownButtonElement menu)
        {
            string[] values = (new AOIShortcutOfAdmin()).GetDisplayFieldNames();
            if (values == null || values.Length == 0)
                return;
            foreach (string s in values)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                RadMenuItem item = new RadMenuItem(s);
                item.Text = s;
                item.DisplayStyle = DisplayStyle.Text;
                item.Tag = s;
                item.Click += new EventHandler(item_Click);
                menu.Items.Add(item);
            }
        }

        private void CreatItemsByFolders(string name, RadDropDownButtonElement element, bool isMap)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\数据引用" + name;
            ParseFileFolders(path, element, isMap);
        }

        private void ParseFileFolders(string path, RadDropDownButtonElement element, bool isMap)
        {
            DirectoryInfo fDir = new DirectoryInfo(path);
            if (!fDir.Exists)
                return;
            string extention = null;
            FileSystemInfo[] finfos = fDir.GetFileSystemInfos();
            List<RadMenuItem> items = new List<RadMenuItem>();
            foreach (FileSystemInfo f in finfos)
            {
                if (f is DirectoryInfo)   //是文件夹才递归调用自己
                {
                    if (!isMap)
                    {
                        RadMenuHeaderItem header = new RadMenuHeaderItem();
                        header.Tag = f.FullName; //将文件的完整路径保存在节点的tag中
                        header.Text = f.Name;    //将文件的名字保存在节点的文本显示中
                        element.Items.Add(header);
                    }
                    ParseFileFolders(f.FullName, element, isMap);
                    continue;
                }
                string filename = null;
                if (!isMap)//shapfile
                {
                    extention = Path.GetExtension(f.Name);
                    if (extention.ToLower() == ".shp")
                        filename = f.Name;
                    else
                        continue;
                    //if (File.Exists(Path.ChangeExtension(f.FullName, "mcd")))
                    //    continue;
                }
                else//map
                {
                    extention = Path.GetExtension(f.Name);
                    if (extention.ToLower() == ".mcd")
                        filename = f.Name;
                    else
                        continue;
                }
                RadMenuItem item = new RadMenuItem();
                if (!isMap)
                {
                    item.IsChecked = HasShp(f.FullName);
                }
                item.Text = GetItemName(f);
                item.Tag = f.FullName;
                item.Click += new EventHandler(shpItem_Click);
                items.Add(item);
            }
            foreach (RadMenuItem item in items.OrderBy(item => item.Text))
                element.Items.Add(item);
        }

        private string GetItemName(FileSystemInfo f)
        {
            string nameValue = DataRefNameValues.TryFindValue(f.FullName);
            if (!string.IsNullOrWhiteSpace(nameValue))
                return nameValue;
            else if (f.Name.Contains("_"))
                return f.Name.Remove(f.Name.LastIndexOf('_'));
            else
                return f.Name.Remove(f.Name.LastIndexOf('.'));
        }

        Dictionary<string, CodeCell.AgileMap.Core.ILayer> shps = new Dictionary<string, CodeCell.AgileMap.Core.ILayer>();

        private void UpdateAllLoadShp()
        {
            if(shps!=null)
                shps.Clear();
            shps = GetLoadedShp();
        }

        private Dictionary<string, CodeCell.AgileMap.Core.ILayer> GetLoadedShp()
        {
            ILayoutHost host = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            if (canvas == null)
                return null;
            IVectorHostLayer vectorHost = canvas.LayerContainer.VectorHost;
            if (vectorHost == null)
                return null;
            Map map = vectorHost.Map as Map;
            if (map == null)
                return null;
            CodeCell.AgileMap.Core.ILayer[] layers = map.LayerContainer.Layers;
            if (layers == null || layers.Length == 0)
                return null;
            Dictionary<string, CodeCell.AgileMap.Core.ILayer> shps = new Dictionary<string, CodeCell.AgileMap.Core.ILayer>();
            foreach (CodeCell.AgileMap.Core.ILayer layer in layers)
            {
                try
                {
                    string shp = (((layer as CodeCell.AgileMap.Core.IFeatureLayer).Class as FeatureClass).DataSource as FileDataSource).FileUrl;
                    shps.Add(shp.ToLower(), layer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return shps;
        }

        private bool HasShp(string filename)
        {
            if (shps == null || shps.Count == 0)
                return false;
            foreach (string shp in shps.Keys)
            {
                if (shp.ToLower() == filename.ToLower())
                    return true;
            }
            return false;
        }
        #endregion

        #region IUITabProvider members
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;

            _layerManager.Image = _session.UIFrameworkHelper.GetImage("system:layermanager.png");
            _removeAll.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            _removeRef.Image = _session.UIFrameworkHelper.GetImage("system:wydclear.png");
            _adminRegion.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            _rivers.Image = _session.UIFrameworkHelper.GetImage("system:wydhlhp2.png");
            _lonlatGrid.Image = _session.UIFrameworkHelper.GetImage("system:wydwg1.png");
            _rasterMap.Image = _session.UIFrameworkHelper.GetImage("system:wydOther1.png");
            _otherRaster.Image = _session.UIFrameworkHelper.GetImage("system:otherRaster.png");
            _newInterestArea.Image = _session.UIFrameworkHelper.GetImage("system:wydgzqy2.png");
            _bookMarkGroups.Image = _session.UIFrameworkHelper.GetImage("system:wydzdsy3.png");
            //_seaiceInter.Image = _session.UIFrameworkHelper.GetImage("system:wydhbgzq1.png");
            _definedSplit.Image = _session.UIFrameworkHelper.GetImage("system:wydgrid2.png");
            _newSplit.Image = _session.UIFrameworkHelper.GetImage("system:wydgrid1.png");
            _specialData.Image = _session.UIFrameworkHelper.GetImage("system:wydsatelite.png");
            _dataSourceCon.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");
            _addDatasource.Image = _session.UIFrameworkHelper.GetImage("system:wydmoredata.png");
            _addAOITemplate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_导入AOI.png");
            _btnApplyDefatultBkgrdLayer.Image = _session.UIFrameworkHelper.GetImage("system:DataRef_DefaultBkgrd.png");
            _aoi2maskButton.Image = _session.UIFrameworkHelper.GetImage("system:AOI_AOI2Mask.png");
            _removeMaskLayer.Image = _session.UIFrameworkHelper.GetImage("system:AOI_RemoveMask.png");
            _btnRemoveDefatultBkgrdLayer.Image = _session.UIFrameworkHelper.GetImage("system:DataRef_RemoveBkgrd.png");
            _creatBookMarks.Image = _session.UIFrameworkHelper.GetImage("system:creatBookMark.png");
            _manageBookMarks.Image = _session.UIFrameworkHelper.GetImage("system:bookMarkManager.png");
            _trafficline.Image = _session.UIFrameworkHelper.GetImage("system:Traffic.png");
        }

        public void UpdateStatus()
        {
        }

        #endregion

        #region click events
        /// <summary>
        /// 关注区域管理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _manageBookMarks_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            ILayoutHost host = null;
            frmBookMarkGroupsManager manager = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            if (canvas == null)
                manager = new frmBookMarkGroupsManager(true);
            else
            {
                CoordEnvelope geoEvp = GetGeoEnvelopeByCanvas(canvas);
                manager = new frmBookMarkGroupsManager(false, geoEvp);
            }
            if (manager.ShowDialog() == DialogResult.OK)
            {
                CoordEnvelope envelope = manager.ApplyEnvelope;
                if (envelope == null)
                    return;
                if (canvas == null)
                    return;
                IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
                if (drawing == null)
                    return;
                // EnlargeEnvelope(ref envelop);
                CoordEnvelope evp = canvas.CurrentEnvelope.Clone();
                canvas.CoordTransform.Geo2Prj(envelope);
                //当所选的关注区域不在当前的范围内时，给出对话框提示是否定位
                if (evp.MaxY < envelope.MinY || evp.MinY > envelope.MaxY || evp.MaxX < envelope.MinX || evp.MinX > envelope.MaxX)
                {
                    DialogResult dialogRe = MessageBox.Show("选中区域不在当前影像范围内，是否定位到选中区域？", "提示", MessageBoxButtons.YesNo);
                    if (dialogRe == DialogResult.No)
                        return;
                }
                canvas.CurrentEnvelope = envelope;
                canvas.Refresh(enumRefreshType.All);
                if (host != null)
                    host.Render(true);
            }
        }

        /// <summary>
        /// 创建关注区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newInter_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            ILayoutHost host = null;
            frmCreatBookMarkGroup frmArea = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            CoordEnvelope geoEvp = GetGeoEnvelopeByCanvas(canvas);
            if (geoEvp == null)
                frmArea = new frmCreatBookMarkGroup();
            else
                frmArea = new frmCreatBookMarkGroup(geoEvp);
            frmArea.Show();
        }

        private CoordEnvelope GetGeoEnvelopeByCanvas(ICanvas canvas)
        {
            if (canvas == null)
                return null;
            CoordEnvelope evp = canvas.CurrentEnvelope;
            double minX = 0, maxX = 0, minY = 0, maxY = 0;
            canvas.CoordTransform.Prj2Geo(evp.MinX, evp.MinY, out minX, out minY);
            canvas.CoordTransform.Prj2Geo(evp.MaxX, evp.MaxY, out maxX, out maxY);
            return new CoordEnvelope(minX, maxX, minY, maxY);
        }

        void _groups_Click(object sender, EventArgs e)
        {
            CreatInterstItem(sender);
        }

        //从配置文件中读取关注区的信息，并添加到DropDownButton中
        private void CreatInterstItem(object sender)
        {
            RadDropDownButtonElement dbt = sender as RadDropDownButtonElement;
            if (dbt == null)
                return;
            dbt.Items.Clear();
            BookMarkParser parser = new BookMarkParser();
            BookMarkGroup[] groups = parser.BookMarkGroups;
            if (groups == null || groups.Length == 0)
                return;
            foreach (BookMarkGroup group in groups)
            {
                RadMenuHeaderItem headItem = new RadMenuHeaderItem(group.GroupName);
                dbt.Items.Add(headItem);
                CreatBookMarkItems(dbt, group);
            }
            dbt.ShowDropDown();
        }

        private void CreatBookMarkItems(RadDropDownButtonElement dbt, BookMarkGroup group)
        {
            Dictionary<string, CoordEnvelope> marks = group.BookMarks;
            foreach (string key in marks.Keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;
                RadMenuItem interItem = new RadMenuItem(key);
                interItem.Tag = marks[key];
                interItem.Click += new EventHandler(interItem_Click);
                dbt.Items.Add(interItem);
            }
        }

        //应用关注区域
        void interItem_Click(object sender, EventArgs e)
        {
            RadMenuItem item = sender as RadMenuItem;
            if (item == null)
                return;
            CoordEnvelope envelop = item.Tag as CoordEnvelope;
            if (envelop == null)
                return;
            ILayoutHost host = null;
            ICanvas canvas = GetCanvasFromSession(ref host);
            if (canvas == null)
            {
                MessageBox.Show("当前无法定位到关注区域，请打开影像数据！", "提示", MessageBoxButtons.OK);
                return;
            }
            IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return;
            // EnlargeEnvelope(ref envelop);
            CoordEnvelope evp = canvas.CurrentEnvelope.Clone();
            canvas.CoordTransform.Geo2Prj(envelop);
            //当所选的关注区域不在当前的范围内时，给出对话框提示是否定位
            if (evp.MaxY < envelop.MinY || evp.MinY > envelop.MaxY || evp.MaxX < envelop.MinX || evp.MinX > envelop.MaxX)
            {
                DialogResult dialogRe = MessageBox.Show("选中区域不在当前影像范围内，是否定位到选中区域？", "提示", MessageBoxButtons.YesNo);
                if (dialogRe == DialogResult.No)
                    return;
            }
            canvas.CurrentEnvelope = envelop;
            canvas.Refresh(enumRefreshType.All);
            if (host != null)
                host.Render(true);
        }

        /// <summary>
        /// 四周都扩大一点经纬度范围，使选中的分区周围留出边界，看的更加方便
        /// </summary>
        /// <param name="envelop"></param>
        private void EnlargeEnvelope(ref CoordEnvelope envelop)
        {
            if (envelop == null)
                return;
            envelop = new CoordEnvelope(envelop.MinX - 0.1, envelop.MaxX + 0.1, envelop.MinY - 0.1, envelop.MaxY + 0.1);
        }

        void item_Click(object sender, EventArgs e)
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            if (_session.SmartWindowManager.ActiveCanvasViewer.AOIContainerLayer == null)
                return;
            string name = (sender as RadMenuItem).Tag.ToString();
            Feature fet = (new AOIShortcutOfAdmin()).GetFeatureByName(name);
            if (fet == null)
                return;
            IAOIContainerLayer container = _session.SmartWindowManager.ActiveCanvasViewer.AOIContainerLayer;
            container.AddAOI(fet);
        }

        void _layerManager_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9002);
            if (cmd != null)
                cmd.Execute();
        }

        void shpItem_Click(object sender, EventArgs e)
        {
            string filename = (sender as RadMenuItem).Tag.ToString();
            if ((sender as RadMenuItem).IsChecked)
            {
                RemoveVectorLayer(filename);
            }
            else
            {
                ICommand cmd = _session.CommandEnvironment.Get(4040);
                if (cmd != null)
                    cmd.Execute(filename);
            }
        }

        private void RemoveVectorLayer(string filename)
        {
            try
            {
                ILayoutHost host = null;
                ICanvas canvas = GetCanvasFromSession(ref host);
                if (canvas == null)
                    return;
                IVectorHostLayer vectorHost = canvas.LayerContainer.VectorHost;
                if (vectorHost == null)
                    return;
                Map map = vectorHost.Map as Map;
                if (map == null)
                    return;
                CodeCell.AgileMap.Core.ILayer _layer = shps[filename.ToLower()];
                map.LayerContainer.Remove(_layer);

                canvas.Refresh(enumRefreshType.VectorLayer);
                if (host != null)
                    host.Render(true);
                RefreshLayerManager();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void _removeAll_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4050);
            if (cmd != null)
            {
                cmd.Execute();
                RefreshLayerManager();
            }
        }

        void _otherRaster_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4020);
            if (cmd != null)
                cmd.Execute();
        }

        //添加1度经纬网格
        void item1_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4041);
            if (cmd != null)
                cmd.Execute("1");
        }

        //添加5度经纬网格
        void item2_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4041);
            if (cmd != null)
                cmd.Execute("5");
        }

        //添加10度经纬网格
        void item3_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4041);
            if (cmd != null)
                cmd.Execute("10");
        }

        //自定义经纬网格
        void item4_Click(object sender, EventArgs e)
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null || _session.SmartWindowManager.ActiveViewer == null)
                return;
            using (frmCustomGridSetting gridfrm = new frmCustomGridSetting(_session))
            {
                gridfrm.ShowDialog();
            }
        }

        void data_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(4051);
            if (cmd != null)
                cmd.Execute();
        }
        #endregion

        void _saveToRasterMap_Click(object sender, EventArgs e)
        {
        }
    }
}
