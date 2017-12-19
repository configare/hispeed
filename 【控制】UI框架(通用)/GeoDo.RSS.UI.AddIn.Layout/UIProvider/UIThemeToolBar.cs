using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI.Data;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public partial class UIThemeToolBar : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        //图层管理器
        RadButtonElement _layerManager = null;
        RadRibbonBarGroup _groupLayerMager = null;

        #region 专题图文档
        RadRibbonBarGroup _groupNewThem = null; //专题图文档
        RadButtonElement _btnNew = null; //新建
        RadButtonElement _btnSave = null; //保存
        #endregion
        #region 视图操作
        RadRibbonBarGroup _groupView = null;
        RadButtonElement _btnPanLayout = null; //漫游专题图
        RadButtonElement _btnPanData = null; //漫游数据
        RadButtonElement _btnToFullEnvelopeData;
        RadButtonElement _btnRefreshData = null;
        RadButtonElement _btnSelect = null; //选择
        RadDropDownListElement _dltMapScale = null; //地图比例尺
        RadDropDownListElement _dltViewScale = null; //视图比例
        #endregion
        #region 专题图模板
        RadRibbonBarGroup _groupTemplate = null;
        // RadButtonElement _btnOpen = null; //打开模板
        RadDropDownButtonElement _dbtnSave = null; // 保存模版
        RadButtonElement _btnDelete = null; //移除模板
        RadButtonElement _btnMore = null; //更多模板
        RadButtonElement _btnEditElement = null; //批量专题图定制
        #endregion
        #region 整饰工具
        RadRibbonBarGroup _groupTools = null;
        RadButtonElement _btnMoreTools = null; //更多整饰元素
        RadButtonElement _btnAddPic = null; //添加图片
        RadButtonElement _btnAddDataFrame = null; //插入数据框
        RadDropDownButtonElement _ddeAddText = null; //插入文本

        RadGalleryItem _itemLine = null; //直线
        RadGalleryItem _itemArrow = null; //带箭头的直线
        RadGalleryItem _itemBezierCurve = null; //贝塞尔曲线
        RadGalleryItem _itemCardinalCurve = null;//基数样条曲线
        RadGalleryItem _itemEllipticArc = null;//圆弧
        RadGalleryItem _itemPie = null;//扇形
        RadGalleryItem _itemPoint = null; //点
        RadGalleryItem _itemPolygon = null;//多边形
        RadGalleryItem _itemPolyline = null;//折线
        RadGalleryItem _itemRect = null; //矩形
        RadGalleryItem _itemCir = null;//圆形

        #endregion
        #region 编辑
        RadRibbonBarGroup _groupEdit = null; //编辑
        RadButtonElement _btnLeft = null; //左对齐
        RadButtonElement _btnVerMid = null; //左右居中
        RadButtonElement _btnRight = null; //右对齐
        RadButtonElement _btnTop = null;  //顶端对齐
        RadButtonElement _btnMiddle = null; //上下居中
        RadButtonElement _btnBottom = null;  //底端对齐
        RadButtonElement _btnWidAlign = null;//横向分布
        RadButtonElement _btnHeiAlign = null; //纵向分布

        RadButtonElement _btnCirRight = null; //向右旋转
        RadButtonElement _btnCirLeft = null; //向左旋转
        RadButtonElement _btnCirV = null; //垂直旋转
        RadButtonElement _btnCirH = null; //水平旋转
        RadButtonElement _btnHStrech = null; // 横向填充
        RadButtonElement _btnVStrech = null;//纵向填充

        RadButtonElement _btnLock = null; //加锁
        RadButtonElement _btnUnLocker = null; //解锁
        RadButtonElement _btnGroup = null; //编组
        RadButtonElement _btnCancleGroup = null; //取消编组
        RadButtonElement _btnMoveSelected = null; //清空选择集
        RadTextBoxElement _txtRotate;
        #endregion
        #region 专题图规格
        RadRibbonBarGroup _groupSize = null;
        RadTextBoxElement _txtColor = null;
        RadButtonElement _btnColorSel = null;  //选择背景颜色
        RadTextBoxElement _txtWidth = null;
        RadTextBoxElement _txtHeight = null;
        RadDropDownListElement _lblUnit = null;
        #endregion
        #region 专题图输出
        RadRibbonBarGroup _groupOutput = null;
        RadButtonElement _btnExCartoon = null;  //导出动画
        RadButtonElement _btnExTxt = null; //导出为文档格式
        RadButtonElement _btnPrint = null; //打印
        #endregion

        private Timer _uiUpdateTimer;

        #region Creat UIFramework
        public UIThemeToolBar()
        {
            InitializeComponent();
            CreatToolBar();
        }

        private void CreatToolBar()
        {
            _bar = new RadRibbonBar();
            _tab = new RibbonTab();
            _bar.Dock = DockStyle.Top;
            _tab.Title = "专题制图";
            //_tab.Text = "专题制图";
            _tab.Name = "专题制图";
            _bar.CommandTabs.Add(_tab);
            Controls.Add(_bar);

            CreateLayerMager();
            CreatThemeGraphDocument();
            CreatGroupViewOpe();
            CreatGroupTemplate();
            CreatGroupTools();
            CreatGroupEdit();
            CreatGroupSize();
            CreatGroupOutput();

            _tab.Items.AddRange(new RadItem[] { _groupNewThem, _groupLayerMager, _groupView, _groupTemplate, _groupTools, _groupEdit, _groupSize, _groupOutput });
        }

        private void CreateLayerMager()
        {
            _groupLayerMager = new RadRibbonBarGroup();
            _groupLayerMager.Text = "图层管理器";
            _layerManager = new RadButtonElement("图层管理器");
            _layerManager.ImageAlignment = ContentAlignment.TopCenter;
            _layerManager.TextAlignment = ContentAlignment.MiddleCenter;
            _layerManager.TextImageRelation = TextImageRelation.ImageAboveText;
            _layerManager.Click += new EventHandler(_layerManager_Click);
            _groupLayerMager.Items.Add(_layerManager);
        }

        private void CreatThemeGraphDocument()
        {
            _groupNewThem = new RadRibbonBarGroup();
            _groupNewThem.Text = "专题图文档";

            _btnNew = new RadButtonElement("新建");
            _btnNew.ImageAlignment = ContentAlignment.TopCenter;
            _btnNew.TextAlignment = ContentAlignment.MiddleCenter;
            _btnNew.TextImageRelation = TextImageRelation.ImageAboveText;
            //  _btnNew.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            //   _btnNew.Alignment = ContentAlignment.MiddleCenter;
            _btnNew.Click += new EventHandler(_btnNew_Click);

            _btnSave = new RadButtonElement("保存");
            _btnSave.ImageAlignment = ContentAlignment.TopCenter;
            _btnSave.TextAlignment = ContentAlignment.MiddleCenter;
            _btnSave.TextImageRelation = TextImageRelation.ImageAboveText;
            // _btnSave.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            _btnSave.Click += new EventHandler(_btnSave_Click);

            _groupNewThem.Items.AddRange(new RadItem[] { _btnNew, _btnSave });
        }

        private void CreatGroupViewOpe()
        {
            _groupView = new RadRibbonBarGroup();
            _groupView.Text = "视图操作";

            _btnPanLayout = new RadButtonElement("  漫游\n专题图");
            _btnPanLayout.ImageAlignment = ContentAlignment.TopCenter;
            _btnPanLayout.TextAlignment = ContentAlignment.MiddleCenter;
            _btnPanLayout.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnPanLayout.Click += new EventHandler(_btnTranslation_Click);

            _btnPanData = new RadButtonElement("  漫游\n数据");
            _btnPanData.ImageAlignment = ContentAlignment.TopCenter;
            _btnPanData.TextAlignment = ContentAlignment.MiddleCenter;
            _btnPanData.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnPanData.Click += new EventHandler(_btnPanData_Click);


            _btnToFullEnvelopeData = new RadButtonElement("  缩放到\n全图");
            _btnToFullEnvelopeData.ImageAlignment = ContentAlignment.TopCenter;
            _btnToFullEnvelopeData.TextAlignment = ContentAlignment.MiddleCenter;
            _btnToFullEnvelopeData.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnToFullEnvelopeData.Click += new EventHandler(_toFullEnvelopeData_Click);

            _btnRefreshData = new RadButtonElement("刷新\n数据");
            _btnRefreshData.ImageAlignment = ContentAlignment.TopCenter;
            _btnRefreshData.TextAlignment = ContentAlignment.MiddleCenter;
            _btnRefreshData.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnRefreshData.Click += new EventHandler(_btnRefreshData_Click);

            _btnSelect = new RadButtonElement("选择");
            _btnSelect.ImageAlignment = ContentAlignment.TopCenter;
            _btnSelect.TextAlignment = ContentAlignment.MiddleCenter;
            _btnSelect.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnSelect.Click += new EventHandler(_btnSelect_Click);

            _dltMapScale = new RadDropDownListElement();
            _dltViewScale = new RadDropDownListElement();

            RadLabelElement lblMapScale = new RadLabelElement();
            lblMapScale.Text = "地图比例 1:";
            _dltMapScale.DropDownStyle = RadDropDownStyle.DropDownList;
            //_dltMapScale.Items.Add("5000");
            //_dltMapScale.Items.Add("10000");
            //_dltMapScale.Items.Add("20000");
            //_dltMapScale.Items.Add("40000");
            //_dltMapScale.Items.Add("50000");
            //_dltMapScale.Items.Add("100000");
            _dltMapScale.MaxSize = new System.Drawing.Size(85, 22);
            _dltMapScale.MinSize = new System.Drawing.Size(80, 22);
            if (_dltMapScale.TextBox != null)
            {
                _dltMapScale.TextBox.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
                _dltMapScale.TextBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            }
            _dltMapScale.SelectedIndex = 0;
            _dltMapScale.SelectedIndexChanged += new PositionChangedEventHandler(_dltMapScale_SelectedIndexChanged);

            RadRibbonBarButtonGroup groupMapScale = new RadRibbonBarButtonGroup();
            groupMapScale.Items.AddRange(new RadItem[] { lblMapScale, _dltMapScale });
            groupMapScale.Orientation = Orientation.Horizontal;
            groupMapScale.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            groupMapScale.ShowBorder = false;

            RadLabelElement lblViewScale = new RadLabelElement();
            lblViewScale.Text = "视图比例:";
            _dltViewScale.Items.Add("适合窗口");
            _dltViewScale.Items.Add("40%");
            _dltViewScale.Items.Add("50%");
            _dltViewScale.Items.Add("60%");
            _dltViewScale.Items.Add("70%");
            _dltViewScale.Items.Add("75%");
            _dltViewScale.Items.Add("80%");
            _dltViewScale.Items.Add("90%");
            _dltViewScale.Items.Add("100%");
            _dltViewScale.Items.Add("200%");
            _dltViewScale.Items.Add("400%");
            _dltViewScale.SelectedIndex = 0;
            _dltViewScale.DropDownMaxSize = new System.Drawing.Size(85, 200);
            _dltViewScale.DropDownMinSize = new System.Drawing.Size(80, 200);
            _dltViewScale.DropDownStyle = RadDropDownStyle.DropDownList;
            _dltViewScale.MaxSize = new System.Drawing.Size(85, 22);
            _dltViewScale.MinSize = new System.Drawing.Size(80, 22);
            _dltViewScale.SelectedIndexChanged += new PositionChangedEventHandler(_dltViewScale_SelectedIndexChanged);
            _dltViewScale.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            //_dltViewScale.Margin = new System.Windows.Forms.Padding(12, 5, 0, 0);
            RadRibbonBarButtonGroup groupViewScale = new RadRibbonBarButtonGroup();
            groupViewScale.Items.AddRange(new RadItem[] { lblViewScale, _dltViewScale });
            groupViewScale.Orientation = Orientation.Horizontal;
            // groupViewScale.Orientation = Orientation.Vertical;
            groupViewScale.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            groupViewScale.ShowBorder = false;

            RadRibbonBarButtonGroup group1 = new RadRibbonBarButtonGroup();
            group1.Items.AddRange(new RadItem[] {/* groupRomance,*/ groupMapScale, groupViewScale });
            group1.Orientation = Orientation.Vertical;
            group1.ShowBorder = false;

            _groupView.Items.AddRange(new RadItem[] { _btnPanLayout, _btnPanData, _btnToFullEnvelopeData, _btnRefreshData, _btnSelect, group1 });
        }

        private void CreatGroupTemplate()
        {
            _groupTemplate = new RadRibbonBarGroup();
            _groupTemplate.Text = "专题图模版";

            _dbtnSave = new RadDropDownButtonElement();
            _dbtnSave.Text = "保存";
            _dbtnSave.ImageAlignment = ContentAlignment.TopCenter;
            _dbtnSave.TextAlignment = ContentAlignment.MiddleCenter;
            _dbtnSave.TextImageRelation = TextImageRelation.ImageAboveText;
            _dbtnSave.DropDownDirection = RadDirection.Down;
            _dbtnSave.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem item1 = new RadMenuItem("保存");
            item1.Click += new EventHandler(item1_Click);
            RadMenuItem item2 = new RadMenuItem("另存为");
            item2.Click += new EventHandler(_btnExTheme_Click);
            _dbtnSave.Items.AddRange(new RadItem[] { item1, item2 });

            _btnMore = new RadButtonElement("更多");
            _btnMore.ImageAlignment = ContentAlignment.TopCenter;
            _btnMore.TextAlignment = ContentAlignment.MiddleCenter;
            _btnMore.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnMore.Click += new EventHandler(_btnMore_Click);

            _btnDelete = new RadButtonElement("移除");
            _btnDelete.ImageAlignment = ContentAlignment.TopCenter;
            _btnDelete.TextAlignment = ContentAlignment.MiddleCenter;
            _btnDelete.TextImageRelation = TextImageRelation.ImageAboveText;

            _btnEditElement = new RadButtonElement("定制");
            _btnEditElement.ImageAlignment = ContentAlignment.TopCenter;
            _btnEditElement.TextAlignment = ContentAlignment.MiddleCenter;
            _btnEditElement.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnEditElement.Click += new EventHandler(_btnEditElement_Click);

            _groupTemplate.Items.AddRange(new RadItem[] { _dbtnSave,/* _btnDelete, */_btnMore, _btnEditElement });
        }

        private void CreatGroupTools()
        {
            _groupTools = new RadRibbonBarGroup();
            _groupTools.Text = "整饰元素";

            _btnMoreTools = new RadButtonElement("整饰\n工具箱");
            _btnMoreTools.ImageAlignment = ContentAlignment.TopCenter;
            _btnMoreTools.TextAlignment = ContentAlignment.MiddleCenter;
            _btnMoreTools.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnMoreTools.Click += new EventHandler(_btnToolBox_Click);

            _btnAddPic = new RadButtonElement("添加图片");
            _btnAddPic.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnAddPic.TextAlignment = ContentAlignment.MiddleLeft;
            _btnAddPic.Margin = new System.Windows.Forms.Padding(2, 2, 2, 4);
            _btnAddPic.Click += new EventHandler(_btnAddPic_Click);
            _btnAddPic.ShowBorder = false;

            _btnAddDataFrame = new RadButtonElement("添加数据框");
            _btnAddDataFrame.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnAddDataFrame.TextAlignment = ContentAlignment.MiddleLeft;
            _btnAddDataFrame.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            _btnAddDataFrame.ShowBorder = false;
            _btnAddDataFrame.Click += new EventHandler(_btnAddDataFrame_Click);

            _ddeAddText = new RadDropDownButtonElement();
            _ddeAddText.Text = "添加文本";
            _ddeAddText.TextImageRelation = TextImageRelation.ImageBeforeText;
            _ddeAddText.TextAlignment = ContentAlignment.MiddleLeft;
            _ddeAddText.DropDownDirection = RadDirection.Right;
            _ddeAddText.Margin = new System.Windows.Forms.Padding(2, 4, 2, 0);

            RadMenuItem itemTitle = new RadMenuItem("标题");
            itemTitle.Click += new EventHandler(itemTitle_Click);
            RadMenuItem itemText = new RadMenuItem("文本");
            itemText.Click += new EventHandler(itemText_Click);
            RadMenuItem itemDate = new RadMenuItem("当前日期");
            itemDate.Click += new EventHandler(itemDate_Click);
            RadMenuItem itemTime = new RadMenuItem("当前时间");
            RadMenuItem itemRate = new RadMenuItem("比例文本");
            _ddeAddText.Items.AddRange(new RadItem[] { itemTitle, itemText,/* itemRate,*/ itemDate,/* itemTime*/ });

            RadRibbonBarButtonGroup group = new RadRibbonBarButtonGroup();
            group.Items.AddRange(new RadItem[] { _btnAddPic, _btnAddDataFrame, _ddeAddText });
            group.Orientation = Orientation.Vertical;
            group.ShowBorder = false;

            #region 插入形状
            RadGalleryElement geShapes = new RadGalleryElement();
            geShapes.MaxRows = 3;
            geShapes.MaxColumns = 4;
            geShapes.MaxSize = geShapes.MinSize = new System.Drawing.Size(107, 66);

            List<RadGalleryItem> itemlist = new List<RadGalleryItem>();

            _itemArrow = new RadGalleryItem();
            _itemArrow.ToolTipText = "箭头";
            _itemBezierCurve = new RadGalleryItem();
            _itemBezierCurve.ToolTipText = "贝塞尔曲线";
            _itemCardinalCurve = new RadGalleryItem();
            _itemCardinalCurve.ToolTipText = "基数样条曲线";
            _itemCir = new RadGalleryItem();
            _itemCir.ToolTipText = "圆";//
            _itemEllipticArc = new RadGalleryItem();
            _itemEllipticArc.ToolTipText = "圆弧";//
            _itemLine = new RadGalleryItem();
            _itemLine.ToolTipText = "直线";//
            _itemPie = new RadGalleryItem();
            _itemPie.ToolTipText = "扇形";//
            _itemPoint = new RadGalleryItem();
            _itemPoint.ToolTipText = "点";//
            _itemPolygon = new RadGalleryItem();
            _itemPolygon.ToolTipText = "多边形";//
            _itemPolyline = new RadGalleryItem();
            _itemPolyline.ToolTipText = "折线";//
            _itemRect = new RadGalleryItem();
            _itemRect.ToolTipText = "矩形";
            _itemPoint.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemPoint);
            _itemLine.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemLine);
            _itemArrow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            // itemlist.Add(_itemArrow);
            _itemPolyline.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemPolyline);
            _itemRect.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemRect);
            _itemPolygon.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemPolygon);
            _itemCir.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemCir);
            _itemEllipticArc.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemEllipticArc);
            _itemPie.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemPie);
            _itemBezierCurve.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemBezierCurve);
            _itemCardinalCurve.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            itemlist.Add(_itemCardinalCurve);

            foreach (RadGalleryItem item in itemlist)
                geShapes.Items.Add(item);
            #endregion

            _groupTools.Items.AddRange(new RadItem[] { _btnMoreTools, group });//, geShapes });//隐藏交互元素编辑功能
        }

        private void CreatGroupEdit()
        {
            _groupEdit = new RadRibbonBarGroup();
            _groupEdit.Text = "元素编辑";

            #region 对齐方式
            RadRibbonBarButtonGroup group1 = new RadRibbonBarButtonGroup();
            _btnLeft = new RadButtonElement();
            _btnLeft.ShowBorder = false;
            _btnLeft.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnLeft.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            _btnLeft.AutoToolTip = true;
            _btnLeft.ToolTipText = "左对齐";
            _btnLeft.MinSize = new System.Drawing.Size(30, 20);
            _btnLeft.MaxSize = new System.Drawing.Size(30, 20);
            _btnLeft.Click += new EventHandler(_btnLeft_Click);

            _btnBottom = new RadButtonElement();
            _btnBottom.ShowBorder = false;
            _btnBottom.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnBottom.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnBottom.ToolTipText = "底端对齐";
            _btnBottom.MinSize = new System.Drawing.Size(30, 20);
            _btnBottom.MaxSize = new System.Drawing.Size(30, 20);
            _btnBottom.Click += new EventHandler(_btnBottom_Click);

            _btnRight = new RadButtonElement();
            _btnRight.ShowBorder = false;
            _btnRight.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnRight.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnRight.ToolTipText = "右对齐";
            _btnRight.MinSize = new System.Drawing.Size(30, 20);
            _btnRight.MaxSize = new System.Drawing.Size(30, 20);
            _btnRight.Click += new EventHandler(_btnRight_Click);

            _btnTop = new RadButtonElement();
            _btnTop.ShowBorder = false;
            _btnTop.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnTop.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnTop.ToolTipText = "顶端对齐";
            _btnTop.MinSize = new System.Drawing.Size(30, 20);
            _btnTop.MaxSize = new System.Drawing.Size(30, 20);
            _btnTop.Click += new EventHandler(_btnTop_Click);

            group1.Items.AddRange(new RadItem[] { _btnLeft, _btnBottom, _btnRight, _btnTop });
            group1.Orientation = Orientation.Horizontal;

            _btnMiddle = new RadButtonElement();
            _btnMiddle.ShowBorder = false;
            _btnMiddle.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnMiddle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            _btnMiddle.ToolTipText = "上下居中";
            _btnMiddle.MinSize = new System.Drawing.Size(30, 20);
            _btnMiddle.MaxSize = new System.Drawing.Size(30, 20);
            _btnMiddle.Click += new EventHandler(_btnVerMid_Click);

            _btnVerMid = new RadButtonElement();
            _btnVerMid.ShowBorder = false;
            _btnVerMid.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnVerMid.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnVerMid.ToolTipText = "左右居中";
            _btnVerMid.MinSize = new System.Drawing.Size(30, 20);
            _btnVerMid.MaxSize = new System.Drawing.Size(30, 20);
            _btnVerMid.Click += new EventHandler(_btnMiddle_Click);

            _btnWidAlign = new RadButtonElement();
            _btnWidAlign.ShowBorder = false;
            _btnWidAlign.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnWidAlign.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnWidAlign.ToolTipText = "横向分布";
            _btnWidAlign.MinSize = new System.Drawing.Size(30, 20);
            _btnWidAlign.MaxSize = new System.Drawing.Size(30, 20);
            _btnWidAlign.Click += new EventHandler(_btnWidAlign_Click);

            _btnHeiAlign = new RadButtonElement();
            _btnHeiAlign.ShowBorder = false;
            _btnHeiAlign.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnHeiAlign.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnHeiAlign.ToolTipText = "纵向分布";
            _btnHeiAlign.MinSize = new System.Drawing.Size(30, 20);
            _btnHeiAlign.MaxSize = new System.Drawing.Size(30, 20);
            _btnHeiAlign.Click += new EventHandler(_btnHeiAlign_Click);

            RadRibbonBarButtonGroup group2 = new RadRibbonBarButtonGroup();
            group2.Items.AddRange(new RadItem[] { _btnMiddle, _btnVerMid, _btnWidAlign, _btnHeiAlign });
            group2.Orientation = Orientation.Horizontal;

            _btnCirRight = new RadButtonElement();
            _btnCirRight.ShowBorder = false;
            _btnCirRight.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnCirRight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            _btnCirRight.ToolTipText = "向右旋转90度";
            _btnCirRight.MinSize = new System.Drawing.Size(30, 20);
            _btnCirRight.MaxSize = new System.Drawing.Size(30, 20);
            _btnCirRight.Click += new EventHandler(_btnCirRight_Click);

            _btnCirLeft = new RadButtonElement();
            _btnCirLeft.ShowBorder = false;
            _btnCirLeft.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnCirLeft.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnCirLeft.ToolTipText = "向左旋转90度";
            _btnCirLeft.MinSize = new System.Drawing.Size(30, 20);
            _btnCirLeft.MaxSize = new System.Drawing.Size(30, 20);
            _btnCirLeft.Click += new EventHandler(_btnCirLeft_Click);

            _btnCirV = new RadButtonElement();
            _btnCirV.ShowBorder = false;
            _btnCirV.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnCirV.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnCirV.ToolTipText = "垂直翻转";
            _btnCirV.MinSize = new System.Drawing.Size(30, 20);
            _btnCirV.MaxSize = new System.Drawing.Size(30, 20);
            _btnCirV.Click += new EventHandler(_btnCirV_Click);

            _btnCirH = new RadButtonElement();
            _btnCirH.ShowBorder = false;
            _btnCirH.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnCirH.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnCirH.ToolTipText = "水平翻转";
            _btnCirH.MinSize = new System.Drawing.Size(30, 20);
            _btnCirH.MaxSize = new System.Drawing.Size(30, 20);

            _btnHStrech = new RadButtonElement();
            _btnHStrech.ShowBorder = false;
            _btnHStrech.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnHStrech.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnHStrech.ToolTipText = "横向填充";
            _btnHStrech.MinSize = new System.Drawing.Size(30, 20);
            _btnHStrech.MaxSize = new System.Drawing.Size(30, 20);
            _btnHStrech.Click += new EventHandler(_btnHStrech_Click);

            _btnVStrech = new RadButtonElement();
            _btnVStrech.ShowBorder = false;
            _btnVStrech.ImageAlignment = ContentAlignment.MiddleCenter;
            _btnVStrech.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            _btnVStrech.ToolTipText = "纵向填充";
            _btnVStrech.MinSize = new System.Drawing.Size(30, 20);
            _btnVStrech.MaxSize = new System.Drawing.Size(30, 20);
            _btnVStrech.Click += new EventHandler(_btnVStrech_Click);
            #endregion

            RadRibbonBarButtonGroup group3 = new RadRibbonBarButtonGroup();
            group3.Items.AddRange(new RadItem[] { _btnCirRight, _btnCirLeft,/* _btnCirV, _btnCirH*/_btnHStrech, _btnVStrech });
            group3.Orientation = Orientation.Horizontal;

            _btnLock = new RadButtonElement("锁定");
            _btnLock.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnLock.TextAlignment = ContentAlignment.MiddleLeft;
            _btnLock.Margin = new System.Windows.Forms.Padding(4, 2, 2, 4);
            _btnLock.ShowBorder = false;
            _btnLock.Click += new EventHandler(_btnLock_Click);

            _btnUnLocker = new RadButtonElement("解锁");
            _btnUnLocker.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnUnLocker.TextAlignment = ContentAlignment.MiddleLeft;
            _btnUnLocker.Margin = new System.Windows.Forms.Padding(4, 0, 2, 0);
            _btnUnLocker.ShowBorder = false;
            _btnUnLocker.Click += new EventHandler(_btnUnLocker_Click);

            RadLabelElement rotate = new RadLabelElement();
            rotate.Text = "旋转";
            _txtRotate = new RadTextBoxElement();
            _txtRotate.MinSize = new System.Drawing.Size(32, 12);
            _txtRotate.MaxSize = new System.Drawing.Size(32, 12);
            _txtRotate.Text = "0";
            _txtRotate.KeyPress += new KeyPressEventHandler(txtRotate_KeyPress);
            _txtRotate.KeyDown += new KeyEventHandler(txtRotate_KeyDown);
            RadLabelElement unit = new RadLabelElement();
            unit.Text = "°";
            RadRibbonBarButtonGroup groupRo = new RadRibbonBarButtonGroup();
            groupRo.Items.AddRange(new RadItem[] { rotate, _txtRotate, unit });
            groupRo.Orientation = Orientation.Horizontal;
            groupRo.ShowBorder = false;
            groupRo.Margin = new System.Windows.Forms.Padding(4, 4, 2, 0);

            RadRibbonBarButtonGroup group4 = new RadRibbonBarButtonGroup();
            group4.Items.AddRange(new RadItem[] { _btnLock, _btnUnLocker, groupRo });
            group4.Orientation = Orientation.Vertical;
            group4.ShowBorder = false;

            _btnGroup = new RadButtonElement("编组");
            _btnGroup.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnGroup.TextAlignment = ContentAlignment.MiddleLeft;
            _btnGroup.Margin = new System.Windows.Forms.Padding(2, 2, 2, 4);
            _btnGroup.ShowBorder = false;
            _btnGroup.Click += new EventHandler(_btnGroup_Click);

            _btnCancleGroup = new RadButtonElement("取消编组");
            _btnCancleGroup.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnCancleGroup.TextAlignment = ContentAlignment.MiddleLeft;
            _btnCancleGroup.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            _btnCancleGroup.ShowBorder = false;
            _btnCancleGroup.Click += new EventHandler(_btnCancleGroup_Click);

            _btnMoveSelected = new RadButtonElement("清空选择集");
            _btnMoveSelected.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnMoveSelected.TextAlignment = ContentAlignment.MiddleLeft;
            _btnMoveSelected.Margin = new System.Windows.Forms.Padding(2, 4, 2, 0);
            _btnMoveSelected.Click += new EventHandler(_btnMoveSelected_Click);
            _btnMoveSelected.ShowBorder = false;

            RadRibbonBarButtonGroup group5 = new RadRibbonBarButtonGroup();
            group5.Items.AddRange(new RadItem[] { _btnGroup, _btnCancleGroup, _btnMoveSelected });
            group5.Orientation = Orientation.Vertical;
            group5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            group5.ShowBorder = false;

            RadRibbonBarButtonGroup group6 = new RadRibbonBarButtonGroup();
            group6.Items.AddRange(new RadItem[] { group4, group5 });
            group6.Orientation = Orientation.Horizontal;
            group6.ShowBorder = false;

            RadRibbonBarButtonGroup group7 = new RadRibbonBarButtonGroup();
            group7.Items.AddRange(new RadItem[] { group1, group2, group3 });
            group7.Orientation = Orientation.Vertical;
            group7.ShowBorder = false;

            _groupEdit.Items.AddRange(new RadItem[] { group7, group6 });
            _groupEdit.Orientation = Orientation.Horizontal;
        }

        private void CreatGroupSize()
        {
            _groupSize = new RadRibbonBarGroup();
            _groupSize.Text = "专题图规格";

            _btnColorSel = new RadButtonElement();
            _btnColorSel.Text = "背景颜色";
            _btnColorSel.ImageAlignment = ContentAlignment.TopCenter;
            _btnColorSel.TextAlignment = ContentAlignment.MiddleCenter;
            _btnColorSel.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnColorSel.MinSize = new System.Drawing.Size(50, 31);
            _btnColorSel.MaxSize = new System.Drawing.Size(60, 31);
            _btnColorSel.ShowBorder = false;

            _txtColor = new RadTextBoxElement();
            _txtColor.CanFocus = false;
            _txtColor.MaxSize = new System.Drawing.Size(60, 33);
            _txtColor.MinSize = new System.Drawing.Size(60, 33);
            _txtColor.Font = new System.Drawing.Font("微软雅黑", 18);
            _txtColor.BackColor = Color.White;
            _txtColor.Enabled = false;

            _btnColorSel.Click += new EventHandler(_dbtnColorSel_Click);

            RadRibbonBarButtonGroup group1 = new RadRibbonBarButtonGroup();
            group1.Items.AddRange(new RadItem[] { _txtColor, _btnColorSel });
            group1.Orientation = Orientation.Vertical;

            RadLabelElement lblSize = new RadLabelElement();
            lblSize.Text = "尺寸";

            RadLabelElement lblWidth = new RadLabelElement();
            lblWidth.Text = "宽:";
            _txtWidth = new RadTextBoxElement();
            _txtWidth.MinSize = new System.Drawing.Size(60, 15);
            _txtWidth.MaxSize = new System.Drawing.Size(70, 15);
            _txtWidth.Text = "800";
            _txtWidth.KeyPress += new KeyPressEventHandler(txtWidth_KeyPress);
            _txtWidth.KeyDown += new KeyEventHandler(txtWidth_KeyDown);
            _txtWidth.LostMouseCapture += new MouseEventHandler(_txtWidth_LostMouseCapture);
            RadRibbonBarButtonGroup groupWidth = new RadRibbonBarButtonGroup();
            groupWidth.Items.AddRange(new RadItem[] { lblWidth, _txtWidth });
            groupWidth.Orientation = Orientation.Horizontal;
            groupWidth.Margin = new System.Windows.Forms.Padding(10, 2, 0, 0);
            groupWidth.ShowBorder = false;

            RadLabelElement lblHeight = new RadLabelElement();
            lblHeight.Text = "高:";
            _txtHeight = new RadTextBoxElement();
            _txtHeight.MinSize = new System.Drawing.Size(60, 15);
            _txtHeight.MaxSize = new System.Drawing.Size(70, 15);
            _txtHeight.Text = "600";
            _txtHeight.KeyPress += new KeyPressEventHandler(txtWidth_KeyPress);
            _txtHeight.KeyDown += new KeyEventHandler(txtWidth_KeyDown);
            _txtHeight.LostMouseCapture += new MouseEventHandler(_txtWidth_LostMouseCapture);
            RadRibbonBarButtonGroup groupHeight = new RadRibbonBarButtonGroup();
            groupHeight.Items.AddRange(new RadItem[] { lblHeight, _txtHeight });
            groupHeight.Orientation = Orientation.Horizontal;
            groupHeight.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            groupHeight.ShowBorder = false;

            RadRibbonBarButtonGroup group2 = new RadRibbonBarButtonGroup();
            group2.Items.AddRange(new RadItem[] { lblSize, groupWidth, groupHeight });
            group2.Orientation = Orientation.Vertical;
            group2.ShowBorder = false;
            group2.Margin = new System.Windows.Forms.Padding(2, 2, 0, 0);

            _lblUnit = new RadDropDownListElement();
            _lblUnit.Items.Add("像素");
            _lblUnit.Items.Add("厘米");
            _lblUnit.MinSize = new System.Drawing.Size(52, 44);
            _lblUnit.MaxSize = new System.Drawing.Size(55, 44);
            _lblUnit.SelectedIndex = 0;
            _lblUnit.DropDownStyle = RadDropDownStyle.DropDownList;
            _lblUnit.Margin = new System.Windows.Forms.Padding(0, 22, 0, 0);
            _lblUnit.SelectedIndexChanged += new PositionChangedEventHandler(_lblUnit_SelectedIndexChanged);
            RadRibbonBarButtonGroup group3 = new RadRibbonBarButtonGroup();
            group3.Items.Add(_lblUnit);
            group3.ShowBorder = false;

            _groupSize.Items.AddRange(new RadItem[] { group1, group2, group3 });
        }

        private void CreatGroupOutput()
        {
            _groupOutput = new RadRibbonBarGroup();
            _groupOutput.Text = "专题图输出";

            _btnExCartoon = new RadButtonElement("导出\n动画");
            _btnExTxt = new RadButtonElement("导出为\n图片");
            _btnPrint = new RadButtonElement("打印");

            _btnExCartoon.ImageAlignment = ContentAlignment.TopCenter;
            _btnExCartoon.TextAlignment = ContentAlignment.MiddleCenter;
            _btnExCartoon.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnExCartoon.Click += new EventHandler(_btnExCartoon_Click);

            _btnExTxt.ImageAlignment = ContentAlignment.TopCenter;
            _btnExTxt.TextAlignment = ContentAlignment.MiddleCenter;
            _btnExTxt.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnExTxt.Click += new EventHandler(_btnExTxt_Click);

            _btnPrint.ImageAlignment = ContentAlignment.TopCenter;
            _btnPrint.TextAlignment = ContentAlignment.MiddleCenter;
            _btnPrint.TextImageRelation = TextImageRelation.ImageAboveText;

            _groupOutput.Items.AddRange(new RadItem[] { _btnExCartoon, _btnExTxt });//, _btnPrint });暂时隐藏打印功能
        }

        private void SetControlImages()
        {
            _layerManager.Image = _session.UIFrameworkHelper.GetImage("system:layermanager.png");

            _btnNew.Image = _session.UIFrameworkHelper.GetImage("system:newTemplate.png");
            _btnPanLayout.Image = _session.UIFrameworkHelper.GetImage("system:panLayout.png");
            _btnPanData.Image = _session.UIFrameworkHelper.GetImage("system:zoom_pan2.png");
            _btnRefreshData.Image = _session.UIFrameworkHelper.GetImage("system:Layout_RefreshData.png");
            _btnToFullEnvelopeData.Image = _session.UIFrameworkHelper.GetImage("system:Layout_ToFullEnvelope.png");
            _btnSelect.Image = _session.UIFrameworkHelper.GetImage("system:Select.png");
            _btnDelete.Image = _session.UIFrameworkHelper.GetImage("system:deleteTemplates.png");
            _btnMore.Image = _session.UIFrameworkHelper.GetImage("system:moreTemplates.png");
            _dbtnSave.Image = _session.UIFrameworkHelper.GetImage("system:saveTemplates.png");
            _btnSave.Image = _session.UIFrameworkHelper.GetImage("system:saveTemplates.png");
            _btnEditElement.Image = _session.UIFrameworkHelper.GetImage("system:editTemplateElement.png");
            
            //output
            _btnExCartoon.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            _btnPrint.Image = _session.UIFrameworkHelper.GetImage("system:printer32.png");
            _btnExTxt.Image = _session.UIFrameworkHelper.GetImage("system:exportBmp.png");

            //themeTools
            _btnMoreTools.Image = _session.UIFrameworkHelper.GetImage("system:element.png");

            //edit-对齐方式
            _btnLeft.Image = _session.UIFrameworkHelper.GetImage("system:alignLeft.png");
            _btnVerMid.Image = _session.UIFrameworkHelper.GetImage("system:alignVerMid.png");
            _btnRight.Image = _session.UIFrameworkHelper.GetImage("system:alignRight.png");
            _btnTop.Image = _session.UIFrameworkHelper.GetImage("system:alignTop.png");
            _btnMiddle.Image = _session.UIFrameworkHelper.GetImage("system:alignMiddle.png");
            _btnBottom.Image = _session.UIFrameworkHelper.GetImage("system:alignBottom.png");
            _btnWidAlign.Image = _session.UIFrameworkHelper.GetImage("system:alignWid.png");
            _btnHeiAlign.Image = _session.UIFrameworkHelper.GetImage("system:alignHei.png");
            _btnLock.Image = _session.UIFrameworkHelper.GetImage("system:lock.png");
            _btnUnLocker.Image = _session.UIFrameworkHelper.GetImage("system:unlock.png");
            _btnMoveSelected.Image = _session.UIFrameworkHelper.GetImage("system:buttonDelete16.png");
            _btnGroup.Image = _session.UIFrameworkHelper.GetImage("system:group.png");
            _btnCancleGroup.Image = _session.UIFrameworkHelper.GetImage("system:unGroup.png");
            _btnVStrech.Image = _session.UIFrameworkHelper.GetImage("system:VStrech.png");
            _btnHStrech.Image = _session.UIFrameworkHelper.GetImage("system:HStrech.png");

            //edit-旋转选项
            _btnCirRight.Image = _session.UIFrameworkHelper.GetImage("system:circleRight.png");
            _btnCirLeft.Image = _session.UIFrameworkHelper.GetImage("system:circleLeft.png");
            _btnCirV.Image = _session.UIFrameworkHelper.GetImage("system:circleV.png");
            _btnCirH.Image = _session.UIFrameworkHelper.GetImage("system:circleH.png");

            //插入
            _btnAddDataFrame.Image = _session.UIFrameworkHelper.GetImage("system:addDataframe.png");
            _btnAddPic.Image = _session.UIFrameworkHelper.GetImage("system:picture16.png");
            _ddeAddText.Image = _session.UIFrameworkHelper.GetImage("system:addText.png");

            //插入形状
            _itemLine.Image = _session.UIFrameworkHelper.GetImage("system:ToolLine.png");
            _itemPoint.Image = _session.UIFrameworkHelper.GetImage("system:ToolPoint.png");
            _itemArrow.Image = _session.UIFrameworkHelper.GetImage("system:ToolLine.png");
            _itemBezierCurve.Image = _session.UIFrameworkHelper.GetImage("system:toolDrawBezierCurve.png");
            _itemCardinalCurve.Image = _session.UIFrameworkHelper.GetImage("system:ToolCurve.png");
            _itemCir.Image = _session.UIFrameworkHelper.GetImage("system:ToolCircle.png");
            _itemEllipticArc.Image = _session.UIFrameworkHelper.GetImage("system:toolDrawEllipticArc.png");
            _itemPie.Image = _session.UIFrameworkHelper.GetImage("system:toolDrawPie.png");
            _itemPolygon.Image = _session.UIFrameworkHelper.GetImage("system:ToolPolygon.png");
            _itemPolyline.Image = _session.UIFrameworkHelper.GetImage("system:toolDrawPolyLine.png");
            _itemRect.Image = _session.UIFrameworkHelper.GetImage("system:ToolRectangle.png");
        }
        #endregion

        #region private functions
        //限定文本框只能输入大于0的整数
        void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8)
            {
                if (sender != null && sender is RadTextBoxElement)
                {
                    e.Handled = false;
                }
            }
            else
                e.Handled = true;
        }

        //限定文本框只能输入-180-180的整数
        void txtRotate_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 45)
            {
                if (sender != null && sender is RadTextBoxElement && keyValue == 45)
                {
                    if (((RadTextBoxElement)sender).Text.IndexOf("-") == 0)
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
            }
            else
                e.Handled = true;
        }

        //限定文本输入框只能输入数字、小数点和控制键
        void txtWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyValue = (int)e.KeyChar;
            if ((keyValue >= 48 && keyValue <= 57) || keyValue == 8 || keyValue == 46)
            {
                if (sender != null && sender is RadTextBoxElement && keyValue == 46)
                {
                    if (((RadTextBoxElement)sender).Text.IndexOf(".") >= 0)
                        e.Handled = true;
                    else
                        e.Handled = false;
                }
                else
                    e.Handled = false;
            }
            else
                e.Handled = true;
        }
        #endregion

        #region Click events

        void _layerManager_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9002);
            if (cmd != null)
                cmd.Execute();
        }

        #region 专题图文档
        //保存专题图文档
        void _btnSave_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6036).Execute();
        }

        //新建专题图
        void _btnNew_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6000).Execute("新专题图"); //传入专题图名字
        }
        #endregion
        #region 视图
        //应用比例尺
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string text = _dltMapScale.EditableElementText;
                if (string.IsNullOrEmpty(text))
                    return;
                float value = float.Parse(text);
                if (value <= 0)
                    return;
                _session.CommandEnvironment.Get(6666).Execute(text);
            }
        }

        void _dltMapScale_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            if (_dltMapScale.SelectedItem == null)
                return;
            if (string.IsNullOrEmpty(_dltMapScale.SelectedItem.Text))
                return;
            _session.CommandEnvironment.Get(6666).Execute(_dltMapScale.SelectedItem.Text);
        }

        //刷新数据
        void _btnRefreshData_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(6032);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 漫游数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnPanData_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6031).Execute();
        }

        /// <summary>
        /// 缩放到全图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _toFullEnvelopeData_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6099).Execute();
        }

        //适合大小
        void _dltViewScale_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            if (_dltViewScale.SelectedIndex == 0)
                _session.CommandEnvironment.Get(6002).Execute();
            else
                _session.CommandEnvironment.Get(6001).Execute(_dltViewScale.SelectedItem.Text);
        }

        //选择
        void _btnSelect_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6003).Execute();
        }

        //漫游专题图
        void _btnTranslation_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6004).Execute();
        }
        #endregion
        #region 专题图模板
        /// <summary>
        /// 保存模版
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item1_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6029).Execute();
        }

        //另存为模板
        void _btnExTheme_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6006).Execute();
        }

        //弹出更多模板窗口
        void _btnMore_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(5002).Execute();
        }

        ///专题图定制
        void _btnEditElement_Click(object sender, EventArgs e)
        {
            using (GeoDo.RSS.Layout.Elements.frmCustomTemplate frm = new GeoDo.RSS.Layout.Elements.frmCustomTemplate())
            {
                frm.ShowDialog();
            }
        }

        #endregion
        #region 整饰元素
        /// <summary>
        /// 添加数据框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnAddDataFrame_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6033).Execute();
        }

        //插入标题
        void itemTitle_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6008).Execute("双击修改专题图标题");
        }

        //自定义添加文字
        void itemText_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6024).Execute();
        }

        //添加当前日期
        void itemDate_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6023).Execute();
        }

        //添加图片
        void _btnAddPic_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6009).Execute();
        }

        //弹出整饰工具箱窗口
        void _btnToolBox_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(5001).Execute();
        }
        #endregion
        #region 编辑
        //清空所有选择集
        void _btnMoveSelected_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6007).Execute();
        }

        /// <summary>
        /// 纵向填充
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnVStrech_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6035).Execute();
        }

        /// <summary>
        /// 横向填充
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnHStrech_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6034).Execute();
        }

        //上下居中
        void _btnVerMid_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6025).Execute();
        }

        //左右居中
        void _btnMiddle_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6022).Execute();
        }

        //纵向分布
        void _btnHeiAlign_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6027).Execute();
        }

        //横向分布
        void _btnWidAlign_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6026).Execute();
        }

        //取消编组
        void _btnCancleGroup_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6021).Execute();
        }

        //顶端对齐
        void _btnTop_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6019).Execute();
        }

        //底端对齐
        void _btnBottom_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6020).Execute();
        }

        //右对齐
        void _btnRight_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6018).Execute();
        }

        //左对齐
        void _btnLeft_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6017).Execute();
        }

        //编组
        void _btnGroup_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6016).Execute();
        }

        //向右旋转90度
        void _btnCirRight_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6039).Execute("90");
        }

        //向左旋转90度
        void _btnCirLeft_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6039).Execute("-90");
        }

        //应用旋转角度
        void txtRotate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            string s = (sender as RadTextBoxElement).Text;
            if (String.IsNullOrEmpty(s))
                return;
            _session.CommandEnvironment.Get(6030).Execute(s);
        }

        //垂直翻转
        void _btnCirV_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6030).Execute("180");
        }

        //应用尺寸单位
        void _lblUnit_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(_txtWidth.Text) || String.IsNullOrEmpty(_txtHeight.Text))
                return;
            if (_lblUnit.SelectedIndex == -1)
                return;
            ILayoutViewer viewer = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            float width = 0;
            bool ok = float.TryParse(_txtWidth.Text, out width);
            float height = 0;
            bool okOrg = float.TryParse(_txtHeight.Text, out height);
            float wid = 0;
            float heig = 0;
            if (ok && width > 0 && okOrg && height > 0f)
            {
                if (_lblUnit.SelectedItem.Text == "像素")
                {
                    wid = viewer.LayoutHost.LayoutRuntime.Centimeter2Pixel(width);
                    heig = viewer.LayoutHost.LayoutRuntime.Centimeter2Pixel(height);
                    _txtWidth.Text = (Math.Round(wid)).ToString();
                    _txtHeight.Text = (Math.Round(heig)).ToString();
                }
                else
                {
                    wid = viewer.LayoutHost.LayoutRuntime.Pixel2Centimeter((int)width);
                    heig = viewer.LayoutHost.LayoutRuntime.Pixel2Centimeter((int)height);
                    _txtWidth.Text = wid.ToString();
                    _txtHeight.Text = heig.ToString();
                }
            }
        }

        //在失去焦点时应用尺寸大小,未产生作用
        void _txtWidth_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ApplyBorderSize();
        }

        //应用尺寸大小
        void txtWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyBorderSize();
            }
        }

        private const int MAX_WIDTH = 100000;   //专题图最大尺寸
        private const int MAX_HEIGHT = 100000;

        private void ApplyBorderSize()
        {
            if (String.IsNullOrEmpty(_txtWidth.Text) || String.IsNullOrEmpty(_txtHeight.Text))
                return;
            ILayoutViewer viewer = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            float width = 0;
            bool ok = float.TryParse(_txtWidth.Text, out width);
            if (width == 0)
                return;
            float height = 0;
            bool okOrg = float.TryParse(_txtHeight.Text, out height);
            if (height == 0)
                return;
            if (width > MAX_WIDTH)
            {
                width = MAX_WIDTH;
                _txtWidth.Text = width.ToString();
            }
            if (height > MAX_HEIGHT)
            {
                height = MAX_HEIGHT;
                _txtHeight.Text = height.ToString();
            }
            if (_lblUnit.SelectedItem.Text == "像素")
                viewer.LayoutHost.LayoutRuntime.Layout.Size = new SizeF(width, height);
            else if (_lblUnit.SelectedItem.Text == "厘米")
            {
                float centiWid = viewer.LayoutHost.LayoutRuntime.Centimeter2Pixel(width);
                float centiHeig = viewer.LayoutHost.LayoutRuntime.Centimeter2Pixel(height);
                viewer.LayoutHost.LayoutRuntime.Layout.Size = new SizeF(centiWid, centiHeig);
            }
            viewer.LayoutHost.Render();
        }

        //锁定
        void _btnLock_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6010).Execute();
        }

        //解锁
        void _btnUnLocker_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6011).Execute();
        }
        #endregion
        #region 规格
        //选择背景颜色
        void _dbtnColorSel_Click(object sender, EventArgs e)
        {
            Color color = Color.Empty;
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    color = dlg.Color;
            }
            if (color == Color.Empty)
                return;
            _txtColor.BackColor = color;
            ILayoutViewer viewer = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            if (host.LayoutRuntime == null)
                return;
            ILayout layout = host.LayoutRuntime.Layout;
            if (layout == null)
                return;
            IBorder border = layout.GetBorder();
            if (border == null)
                return;
            border.BackColor = color;
            host.Render();
        }
        #endregion
        #region 输出
        //导出动画
        void _btnExCartoon_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6038).Execute();
        }

        //导出为图片
        void _btnExTxt_Click(object sender, EventArgs e)
        {
            _session.CommandEnvironment.Get(6005).Execute();
        }
        #endregion
        #endregion

        #region IUITabProvider members
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            SetControlImages();
            AttachEventsToSession(session);
        }

        public void UpdateStatus()
        {
        }

        #endregion

        private void AttachEventsToSession(ISmartSession session)
        {
            ISmartWindowManager mgr = session.SmartWindowManager;
            mgr.OnActiveWindowChanged += new OnActiveWindowChangedHandler(ActiveViewerChanged);
        }

        void ActiveViewerChanged(object sender, ISmartWindow oldWindow, ISmartWindow newWindow)
        {
            if (newWindow == null || !(newWindow is ILayoutViewer))
            {
                if (_uiUpdateTimer != null)
                {
                    _uiUpdateTimer.Stop();
                    _uiUpdateTimer.Dispose();
                    _uiUpdateTimer = null;
                }
                return;
            }
            if (_uiUpdateTimer == null)
            {
                _uiUpdateTimer = new Timer();
                _uiUpdateTimer.Interval = 50;
                _uiUpdateTimer.Tick += new EventHandler(_uiUpdateTimer_Tick);
                _uiUpdateTimer.Start();
            }
        }

        void _uiUpdateTimer_Tick(object sender, EventArgs e)
        {
            ILayoutViewer v = _session.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (v == null)
                return;
            //当鼠标在工具栏上时不执行同步属性的操作
            Point pt = _session.SmartWindowManager.ViewLeftUpperCorner;
            if (Control.MousePosition.Y < pt.Y)
                return;
            //同步视图比例
            SyncViewScale(v);
            //同步图廓大小
            SyncLayoutSize(v);
            //同步选中元素的角度
            SyncAngleOfSelected(v);
            //同步比例尺
            SyncLayoutScale(v);
        }

        private void SyncLayoutScale(ILayoutViewer v)
        {
            if (v.LayoutHost == null || v.LayoutHost.LayoutRuntime == null)
                return;
            if (v.LayoutHost.ActiveDataFrame == null)
            {
                _dltMapScale.Text = string.Empty;
                return;
            }
            _dltMapScale.Text = ((int)v.LayoutHost.ActiveDataFrame.LayoutScale).ToString();
        }

        private void SyncAngleOfSelected(ILayoutViewer v)
        {
            if (v.LayoutHost == null || v.LayoutHost.LayoutRuntime == null)
                return;
            if (v.LayoutHost.LayoutRuntime.Selection == null || v.LayoutHost.LayoutRuntime.Selection.Length == 0)
            {
                _txtRotate.Text = "0";
                return;
            }
            ISizableElement ele = v.LayoutHost.LayoutRuntime.Selection[0] as ISizableElement;
            if (ele == null)
            {
                _txtRotate.Text = "0";
                return;
            }
            string angle = ((int)(ele.Angle + 0.5f)).ToString();
            _txtRotate.Text = angle;
        }

        private void SyncLayoutSize(ILayoutViewer v)
        {
            if (v.LayoutHost == null || v.LayoutHost.LayoutRuntime == null)
                return;
            float width = v.LayoutHost.LayoutRuntime.Layout.Size.Width;
            float height = v.LayoutHost.LayoutRuntime.Layout.Size.Height;
            if (_lblUnit.Text == "像素")
            {
                v.LayoutHost.LayoutRuntime.Layout2Pixel(ref width, ref height);
                _txtWidth.Text = width.ToString();
                _txtHeight.Text = height.ToString();
            }
            else if (_lblUnit.Text == "厘米")
            {
                v.LayoutHost.LayoutRuntime.Layout2Pixel(ref width, ref height);
                _txtWidth.Text = v.LayoutHost.LayoutRuntime.Pixel2Centimeter((int)width).ToString();
                _txtHeight.Text = v.LayoutHost.LayoutRuntime.Pixel2Centimeter((int)height).ToString();
            }
        }

        private void SyncViewScale(ILayoutViewer v)
        {
            if (v.LayoutHost == null || v.LayoutHost.LayoutRuntime == null)
                return;
            _dltViewScale.Text = ((int)(v.LayoutHost.LayoutRuntime.Scale * 100)).ToString(".##") + "%";
        }
    }
}
