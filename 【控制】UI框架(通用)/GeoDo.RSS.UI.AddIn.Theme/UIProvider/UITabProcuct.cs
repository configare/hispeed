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
using GeoDo.RSS.MIF.Core;
using System.IO;
using Telerik.WinControls;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    /// <summary>
    /// 修改记录：
    /// 2013-11-14 罗战克
    ///     移除自定义按钮
    ///     产品按钮添加时候，判断对应产品是否存在，只加载存在的产品
    /// </summary>
    public partial class UITabProcuct : UserControl, IUITabProvider, IThemeProductsTab
    {
        public static UITabProcuct Instance;
        RibbonTab _tab = null;
        ISmartSession _session = null;
        List<RadButtonElement> _tempButtons;
        RadButtonElement btnNewProduct = new RadButtonElement();
        RadButtonElement btnSaveEnhance = new RadButtonElement("保存方案");
        private List<RadGalleryItem> _galItemList = null;
        private RadRibbonBarGroup _rbgzq = null;

        public UITabProcuct()
        {
            InitializeComponent();
            InitContent();
            Instance = this;
        }

        private void InitContent()
        {
            _tab = new RibbonTab();
            _tab.Title = "监测分析专题";
            _tab.Text = "监测分析专题";
            _tab.Name = "监测分析专题";
        }

        private void CreateRibbonBar()
        {
            Padding productPadding = new Padding(0, 5, 0, 0);
            ThemeDef[] ths = MonitoringThemeFactory.GetAllThemes();
            Dictionary<string, RadRibbonBarGroup> dic = new Dictionary<string, RadRibbonBarGroup>();
            _tempButtons = new List<RadButtonElement>();
            foreach (ThemeDef theme in ths)
            {
                if (theme == null || theme.Products == null || theme.Products.Length == 0)
                    continue;
                #region 监测专题产品
                foreach (ProductDef item in theme.Products)
                {
                    //这里需要将没加载成功的产品过滤掉。
                    if (HasProduct(item.Identify))
                    {
                        RadButtonElement btn = new RadButtonElement(item.Name);
                        btn.Tag = item;
                        btn.ImageAlignment = ContentAlignment.TopCenter;
                        btn.TextAlignment = ContentAlignment.BottomCenter;
                        btn.Padding = productPadding;
                        btn.Click += new EventHandler(btn_Click);
                        if (dic.ContainsKey(item.Group))
                        {
                            dic[item.Group].Items.Add(btn);
                        }
                        else
                        {
                            dic.Add(item.Group, new RadRibbonBarGroup());
                            dic[item.Group].Text = item.Group;
                            dic[item.Group].Items.Add(btn);
                        }
                        _tempButtons.Add(btn);
                    }
                }
                #endregion 监测专题产品
            }

            //RadRibbonBarGroup rbgUserDef = new RadRibbonBarGroup();
            //rbgUserDef.Text = "自定义";
            //btnNewProduct.Text = "扩展专题产品";
            //btnNewProduct.TextAlignment = ContentAlignment.BottomCenter;
            //btnNewProduct.Margin = new Padding(0, 5, 0, 0);
            //btnNewProduct.ImageAlignment = ContentAlignment.TopCenter;
            //rbgUserDef.Items.Add(btnNewProduct);

            #region 图像增强方案
            _rbgzq = new RadRibbonBarGroup();
            _rbgzq.Text = "图像增强方案";
            btnSaveEnhance.TextAlignment = ContentAlignment.BottomCenter;
            btnSaveEnhance.ImageAlignment = ContentAlignment.TopCenter;
            btnSaveEnhance.Margin = new Padding(0, 5, 0, 0);
            btnSaveEnhance.Click += new EventHandler(btnSaveEnhance_Click);
            _rbgzq.Items.Add(btnSaveEnhance);

            //galItemList.Add(new RadGalleryItem("     火情\nFY2D_VISSR\n    夏上午"));
            //galItemList.Add(new RadGalleryItem("     大雾\nFY2D_VISSR\n    夏下午"));
            //galItemList.Add(new RadGalleryItem("     沙尘\nFY2E_VISSR\n    夏上午"));
            //galItemList.Add(new RadGalleryItem("     暴雨\nFY2E_VISSR\n    夏下午"));

            //修改为从文件夹中查找的方式 modify by wangyu 20121020           
            CreatGalleryElement(null, null);
            #endregion

            foreach (RadRibbonBarGroup grp in dic.Values)
                _tab.Items.Add(grp);
            //_tab.Items.Add(rbgUserDef);
            _tab.Items.Add(_rbgzq);
        }

        private bool HasProduct(string productIdentify)
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            IMonitoringProduct prd = msession.FindMonitoringProduct(productIdentify);
            return prd != null;
        }

        //创建显示“图像增强方案”的Gallery控件
        private void CreatGalleryElement(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\图像增强方案";
            if (!Directory.Exists(path))
                return;
            _galItemList = new List<RadGalleryItem>();
            ParseEnhanceDirectory(path);
            RadGalleryElement geEnhance = null;
            foreach (RadItem item in _rbgzq.Items)
            {
                if (item is RadGalleryElement)
                {
                    geEnhance = item as RadGalleryElement;
                    geEnhance.Items.Clear();
                    break;
                }
            }
            if (geEnhance == null)
            {
                geEnhance = new RadGalleryElement();
                geEnhance.MaxRows = 1;
                geEnhance.MaxColumns = 4;
            }
            Padding galItemPadding = new Padding(5, 5, 5, 5);
            foreach (RadGalleryItem item in _galItemList)
            {
                item.Margin = galItemPadding;
                geEnhance.Items.Add(item);
            }
            _rbgzq.Items.Add(geEnhance);
        }

        //从文件夹中读取图像增强方案的文件名，并显示
        private void ParseEnhanceDirectory(string path)
        {
            string[] files = Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            string fname = string.Empty;
            foreach (string fi in files)
            {
                fname = Path.GetFileNameWithoutExtension(fi);
                string[] parts = fname.Split('_');
                int count = parts.Length;
                RadGalleryItem item = null;
                switch (count)
                {
                    case 5:
                        {
                            item = new RadGalleryItem("  " + parts[0] + "_" + parts[1] + "\n" + parts[2] + "_" + parts[3] + "\n     " + parts[4]);
                            break;
                        }
                    case 4: //"火情_EOST_MODIS_夏晚上.xml",信息齐全
                        {
                            item = new RadGalleryItem("     " + parts[0] + "\n" + parts[1] + "_" + parts[2] + "\n     " + parts[3]);
                            break;
                        }
                    case 3: //"火情_EOST_MODIS.xml",没有时间信息
                        {
                            item = new RadGalleryItem("     " + parts[0] + "\n" + parts[1] + "_" + parts[2]);
                            break;
                        }
                    case 2://"火情_EOST.xml",没有传感器信息
                        {
                            item = new RadGalleryItem("     " + parts[0] + "\n" + parts[1]);
                            break;
                        }
                    case 1://"火情.xml",只有当前产品标识
                        {
                            item = new RadGalleryItem("     " + parts[0]);
                            break;
                        }
                    default:
                        continue;
                }
                item.Tag = fi;
                item.Click += new EventHandler(item_Click);
                _galItemList.Add(item);
            }
        }

        //应用图像增强方案
        void item_Click(object sender, EventArgs e)
        {
            RadGalleryItem item = sender as RadGalleryItem;
            if (item == null)
                return;
            string enhanceName = item.Tag.ToString();
            if (string.IsNullOrEmpty(enhanceName))
                return;
            ICommand cmd = _session.CommandEnvironment.Get(9112);
            if (cmd != null)
                cmd.Execute(enhanceName);
        }

        //保存图像增强方案
        void btnSaveEnhance_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(9111);
            if (cmd != null)
            {
                cmd.Execute();
                CreatGalleryElement(null, null);
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            RadButtonElement btn = sender as RadButtonElement;
            if (btn == null || btn.Tag == null || !(btn.Tag is ProductDef))
                return;
            ProductDef prdDef = btn.Tag as ProductDef;
            string uiProvider = prdDef.UIProvider;
            ContentOfUIProvider prd = new ContentOfUIProvider(uiProvider);
            if (prd != null)
            {
                RibbonTab tab = (RibbonTab)(prd.Control as IUIProvider).Content;
                tab.Tag = prdDef;
                if (tab != null)
                {
                    (prd.Control as IUITabProvider).Init(_session);
                    int idx = _session.UIFrameworkHelper.IndexOf("监测分析专题");
                    _session.UIFrameworkHelper.InsertTab(idx + 1, tab);
                    TryChangeActiveProduct(prdDef.Identify);
                }
            }
        }

        private void TryChangeActiveProduct(string productIdentify)
        {
            IMonitoringSession msession = _session.MonitoringSession as IMonitoringSession;
            msession.ChangeActiveProduct(productIdentify, _isOpenWorkspace);
        }

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateRibbonBar();
            _session.UIFrameworkHelper.SetTabChangedEvent((obj) =>
            {
                ProductDef prdDef = obj as ProductDef;
                if (prdDef != null)
                {
                    (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(prdDef.Identify, _isOpenWorkspace);
                }
            });
            SetImage();
        }

        public void UpdateStatus()
        {
        }

        private void SetImage()
        {
            if (_session == null)
                return;
            btnNewProduct.Image = _session.UIFrameworkHelper.GetImage("system:newProduct.png");
            btnSaveEnhance.Image = _session.UIFrameworkHelper.GetImage("system:save.png");
            if (_tempButtons == null || _tempButtons.Count == 0)
                return;
            foreach (RadButtonElement btn in _tempButtons)
            {
                ProductDef def = btn.Tag as ProductDef;
                if (def == null || string.IsNullOrEmpty(def.Image))
                    continue;
                btn.Image = _session.UIFrameworkHelper.GetImage(def.Image);
            }
        }

        public bool IsOpenWorkspace
        {
            get { return _isOpenWorkspace; }
            set { _isOpenWorkspace = value; }
        }

        private bool _isOpenWorkspace = true;
        void IThemeProductsTab.ActiveProduct(string identify, bool isOpenWorkspace)
        {
            bool old = _isOpenWorkspace;
            _isOpenWorkspace = isOpenWorkspace;
            try
            {
                foreach (RadButtonElement btn in _tempButtons)
                {
                    ProductDef def = btn.Tag as ProductDef;
                    if (def != null && def.Identify == identify)
                    {
                        btn.PerformClick();
                    }
                }
            }
            finally
            {
                _isOpenWorkspace = old;
            }
        }
    }
}
