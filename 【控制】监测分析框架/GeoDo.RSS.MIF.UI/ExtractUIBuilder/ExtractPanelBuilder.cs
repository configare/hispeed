using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Reflection;
using CodeCell.AgileMap.Core;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.UI
{
    internal class ExtractPanelBuilder : IExtractPanelBuilder
    {
        private OnAlgorithmChangedHandler _algorithmChangedHandler;
        private OnArgumentValueChangingHandler _argumentValueChangingHandler;
        private OnArgumentValueChangedHandler _argumentValueChangedHandler;
        private OnAOITempleteChangedHandler _onAOIChangedHandler;
        private Control _container = null;// 整个算法参数配置子控件
        private Control _aoiTemplateControl = null;//aoi自定义区域 这个是所有算法通用的
        private RadPageView _algPageView = null; //tabpage控件 包含算法和波段信息
        private RadPageViewItemPage _tabPage = null;// 算法参数page  
        private RadPageViewItemPage _bandPage = null; //波段信息page
        private RadGroupBox _argBox = null;//算法显示box
        private RadGroupBox _algGroupBox = null;//算法对应参数配置box
        //为了分组增加以下控件
        private List<RadPageViewItemPage> _listtabPages=new List<RadPageViewItemPage>();//分组的算法tab页
        private Dictionary<string, List<AlgorithmDef>> _listgroups=new Dictionary<string,List<AlgorithmDef>>();//分组算法 key 组名称
        private Dictionary<string, RadGroupBox> _listargBox=new Dictionary<string,RadGroupBox>();//分组算法 显示算法详细内容
        private Dictionary<string, RadGroupBox> _listalgGroupBox=new Dictionary<string,RadGroupBox>();//分组算法 显示该组算法名称box 
        //
        private IArgumentProvider _argumentProvider;
        private SubProductDef _subProductDef = null;           //当前子产品
        private AlgorithmDef _selectedAlgorithm = null;        //当前产品算法
        private Dictionary<AlgorithmDef, ArgumentCopy> _algArgumentCopyDics = new Dictionary<AlgorithmDef, ArgumentCopy>(); //记录算法及其参数值
        private IMonitoringSubProduct _monitoringSubProduct;
        //private List<string> _aois = new List<string>();
        private IWorkspace _wks;
        
        private ExtractPanelHelper _entractHelper;
        private IRasterDataProvider _dataProvider;
        private ucAOITemplate _aoiTemplate;

        public ExtractPanelBuilder(Control container)
        {
            _entractHelper = new ExtractPanelHelper();
            _container = container;
        }

        #region Event
        public OnAlgorithmChangedHandler OnAlgorithmChanged
        {
            get { return _algorithmChangedHandler; }
            set { _algorithmChangedHandler = value; }
        }

        public OnArgumentValueChangingHandler OnArgumentValueChanging
        {
            get { return _argumentValueChangingHandler; }
            set { _argumentValueChangingHandler = value; }
        }

        public OnArgumentValueChangedHandler OnArgumentValueChanged
        {
            get { return _argumentValueChangedHandler; }
            set { _argumentValueChangedHandler = value; }
        }

        public OnAOITempleteChangedHandler OnAOITempleteChanged
        {
            get { return _onAOIChangedHandler; }
            set { _onAOIChangedHandler = value; }
        }
        #endregion

        public string[] AOIs
        {
            get { return _aoiTemplate == null ? null : _aoiTemplate.Aois; }
        }

        public void Build(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            Reset();
            if (subProduct == null || subProduct.Definition == null || subProduct.ArgumentProvider == null)
                return;
            _wks = wks;
            _monitoringSubProduct = subProduct;
            _subProductDef = subProduct.Definition;
            _argumentProvider = subProduct.ArgumentProvider;
            _dataProvider = subProduct.ArgumentProvider.DataProvider;
            try
            {
                _container.SuspendLayout();
                this._listgroups = GetGroupAlgDefs();
                BuildSimple();
                //BuildOnlyControl();
            }
            finally
            {
                foreach (KeyValuePair<string, RadGroupBox> item in _listalgGroupBox)
                {
                    item.Value.ResumeLayout(false);
                    item.Value.PerformLayout();
                }
                foreach (KeyValuePair<string, RadGroupBox> item in _listargBox)
                {
                    item.Value.ResumeLayout(false);
                    item.Value.PerformLayout();
                }
                for (int i = 0; i < _listtabPages.Count;i++)
                {
                    _listtabPages[i].ResumeLayout(false);
                    _listtabPages[i].PerformLayout();

                }
                   
                if (_bandPage != null)
                {
                    _bandPage.ResumeLayout(false);
                    _bandPage.PerformLayout();
                }
                if (_algPageView != null)
                {
                    _algPageView.Visible = subProduct.Definition.IsDisplayPanel;
                    _algPageView.ResumeLayout(false);
                    _algPageView.PerformLayout();
                }
                _container.ResumeLayout(false);
                _container.PerformLayout();
                _container.Invalidate();
                _container.Refresh();
            }
        }
 
        private void BuildSimple()
        {
            //感兴趣区域模板
            if (_subProductDef.IsUseAoiTemplate)
            {
                _aoiTemplateControl = BuildAOITemplate(_subProductDef);
                _aoiTemplateControl.Dock = DockStyle.Top;
                _aoiTemplateControl.Size = new Size(_container.Width - 4, _aoiTemplateControl.Height);
            }
            else
            {
                _aoiTemplateControl = null;
            }
            _algPageView = new RadPageView();
            _algPageView.SuspendLayout();
            //初始化 算法显示和算法配置所在的tabcontrol
            List<Control> groups = new List<Control>();
            _listtabPages = new List<RadPageViewItemPage>();
            BuildSubProductUIByGroup(_subProductDef, _listtabPages);

            #region 折叠
            _bandPage = new RadPageViewItemPage();
            _bandPage.Text = "波段设置";
            CreateBandPanel(_selectedAlgorithm, _bandPage);
            _bandPage.Dock = DockStyle.Fill;
            _bandPage.Location = new Point(0, 0);
            int pageHeight =_listtabPages.Max(o=>o.Height) + 18;
            _algPageView.Dock = DockStyle.Top;
            _algPageView.Size = new Size(_container.Width - 1, pageHeight + 40);
            foreach (Control contrl in _listtabPages)
            {
                _algPageView.Pages.Add(contrl as RadPageViewPage);
            }
            _algPageView.Pages.Add(_bandPage);
            _algPageView.SelectedPageChanged += new EventHandler(_algPageView_SelectedPageChanged);
            #endregion 
            _container.Controls.Add(_algPageView);
            if (_aoiTemplateControl != null)
                _container.Controls.Add(_aoiTemplateControl);
        }
       

        private void BuildOnlyControl()
        {
            _selectedAlgorithm = _subProductDef.Algorithms[0];
            AlgChanged(_selectedAlgorithm);
            CreateArgPanelOnlyControl(_selectedAlgorithm, _container);
            //BuildSubProductUIOnlyControl(_subProductDef, _container);
        }



        private void CreateArgPanelOnlyControl(AlgorithmDef algDef, Control container)
        {
            RadPanel algPanel = new RadPanel();
            algPanel.Dock = DockStyle.Top;
            algPanel.Height = 0;
            ArgumentBase[] args = algDef.Arguments;
            BuildArgument(algPanel, args);
            //container.Controls.Clear();
            container.Controls.Add(algPanel);
        }

        private void BuildSubProductUIOnlyControl(SubProductDef subProduct, Control container)
        {
            container.Text = subProduct.Name ?? string.Empty;
            if (subProduct.Algorithms != null && subProduct.Algorithms.Length > 0)
            {
                if (_argBox == null)
                    _argBox = new RadGroupBox();
                else
                    _argBox.Controls.Clear();
                AlgorithmDef defaultAlg = GetDefaultAlg();
                _algGroupBox = new RadGroupBox();
                _algGroupBox.Text = "选择监测算法:";
                _algGroupBox.Width = container.Width;
                _algGroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                int idx = 0;
                int x = 0;
                int y = 20;
                _algGroupBox.Height = 20;
                List<RadRadioButton> btns = new List<RadRadioButton>();
                foreach (AlgorithmDef algDef in subProduct.Algorithms)//所有算法的参数都创建起来
                {
                    RadRadioButton btn = new RadRadioButton();
                    btn.AutoSize = true;
                    if (idx % 2 == 0)
                    {
                        if (idx != 0)
                            y += 26;
                        //else
                        //    btn.IsChecked = true;
                        x = 20;
                        btn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                        _algGroupBox.Height += 26;
                    }
                    else
                    {
                        x = container.Width / 2 - 30;
                        btn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    }
                    btn.Text = algDef.Name;
                    btn.Left = x;
                    btn.Top = y;
                    btn.Tag = algDef;
                    btn.ToggleStateChanged += new StateChangedEventHandler(btn_ToggleStateChanged);
                    btns.Add(btn);
                    _algGroupBox.Controls.Add(btn);
                    idx++;
                }
                container.Controls.Add(_algGroupBox);
                SetDefaultAlg(defaultAlg, btns.ToArray());

                _argBox.Text = "算法参数:";
                _argBox.Location = new Point(0, y + 32);
                _argBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                _argBox.Size = new Size(container.Width, container.Height);
                AlgChanged(_selectedAlgorithm);
                CreateArgPanelOnlyControl(_selectedAlgorithm, container);
                _argBox.Height = _argBox.Height + 19;
                container.Controls.Add(_argBox);
            }
        }

        void _algPageView_SelectedPageChanged(object sender, EventArgs e)
        {
            if (_algPageView.SelectedPage == null)
                return;
            if (_algPageView.SelectedPage.Text == "波段设置")
                CreateBandPanel(_selectedAlgorithm, _bandPage);
        }

        private void Reset()
        {
            this._container.Controls.Clear();
            _subProductDef = null;
            _selectedAlgorithm = null;
            _aoiTemplate = null;
            //_aois.Clear();
        }

        #region Band
        private void CreateBandPanel(AlgorithmDef algorithmDef, Control container)
        {
            container.Controls.Clear();
            ucBandPanel bandPanel = new ucBandPanel(_argumentProvider, new Action<string, int>((string bandName, int argValue) => SetCurArgOnly(bandName, argValue)));
            bandPanel.Dock = DockStyle.Fill;
            bandPanel.Location = new Point(0, 0);
            bandPanel.CreateBandPanel(algorithmDef);
            container.Controls.Add(bandPanel);
        }

        #endregion

        private Control BuildAOITemplate(SubProductDef subProductDef)
        {
            _aoiTemplate = new ucAOITemplate();
            _aoiTemplate.Height = 96;
            _aoiTemplate.BuildControl(subProductDef.ProductDef.AOITemplates, subProductDef.AoiTemplates);
            _aoiTemplate.AOIChangedHandler += new OnAOITempleteChangedHandler(_onAOIChangedHandler);
            return _aoiTemplate;
        }

        public void ResetDefaultValue()
        {
            string algIdentify = _selectedAlgorithm.Identify;

            //DataIdentify did = _dataProvider.DataIdentify;
            //_monitoringSubProduct.ResetArgumentProvider(algIdentify, did.Satellite, did.Sensor);

            if (_monitoringSubProduct.ArgumentProvider != null && !string.IsNullOrWhiteSpace(algIdentify))
                _monitoringSubProduct.ResetArgumentProvider(algIdentify);

            if (_argumentProvider != null)
                SetCurArgOnly("AlgorithmName", _selectedAlgorithm.Identify);
            if (_algorithmChangedHandler != null)
                _algorithmChangedHandler(this, _selectedAlgorithm);

            UpdateCopyValue();
            //CreateArgPanel(_selectedAlgorithm, _argBox);
            string group=_selectedAlgorithm.GroupTypeName;
            if(!string.IsNullOrEmpty(group)&&_listargBox!=null)
            {
                CreateArgPanel(_selectedAlgorithm, _listargBox[group]);
            }
           
        }

        private void UpdateCopyValue()
        {
            ArgumentCopy argCopy = _algArgumentCopyDics[_selectedAlgorithm];
            ArgumentCopy argCopyNew = new ArgumentCopy();
            foreach (string argName in argCopy.Args.Keys)
            {
                object value = _monitoringSubProduct.ArgumentProvider.GetArg(argName);
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    value = argCopy.Args[argName];
                if (argCopyNew.Args.ContainsKey(argName))
                    argCopyNew.Args[argName] = value;
                else
                    argCopyNew.Args.Add(argName, value);
            }
            _algArgumentCopyDics[_selectedAlgorithm] = argCopyNew;
        }
        /// <summary>
        /// 根据子产品创建参数面板内容
        /// </summary>
        /// <param name="subProduct"></param>
        /// <param name="container"></param>
        private void BuildSubProductUIByGroup(SubProductDef subProduct, List<RadPageViewItemPage> listcontrol)
        {
            _selectedAlgorithm = GetDefaultAlg();
            List<RadRadioButton> listallbtns = new List<RadRadioButton>();
            foreach (KeyValuePair<string,List<AlgorithmDef>> item in _listgroups)
            {
                //循环每一组算法
                RadPageViewItemPage container = null;//
                RadGroupBox _argBoxItem = new RadGroupBox();
                RadGroupBox _algGroupBoxItem = new RadGroupBox();
                container = new RadPageViewItemPage();
                container.Dock = DockStyle.Fill;
                container.Location = new Point(0, 0);
                container.Text = item.Key;
                #region 组内操作
                if (item.Value != null && item.Value.Count > 0)
                {
                   
                    _algGroupBoxItem.Text = "选择监测算法:";
                    _algGroupBoxItem.Width = container.Width;
                    _algGroupBoxItem.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    int idx = 0;
                    int x = 0;
                    int y = 20;
                    _algGroupBoxItem.Height = 20;
                    List<RadRadioButton> btns = new List<RadRadioButton>();
                    foreach (AlgorithmDef algDef in item.Value)//所有算法的参数都创建起来
                    {
                        RadRadioButton btn = new RadRadioButton();
                        btn.AutoSize = true;
                        if (idx % 2 == 0)
                        {
                            if (idx != 0)
                                y += 26;
                            //else
                            //    btn.IsChecked = true;
                            x = 20;
                            btn.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                            _algGroupBoxItem.Height += 26;
                        }
                        else
                        {
                            x = container.Width / 2 - 30;
                            btn.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                        }
                        btn.Text = algDef.Name;
                        btn.Left = x;
                        btn.Top = y;
                        btn.Tag = algDef;
                        btn.ToggleStateChanged += new StateChangedEventHandler(btn_ToggleStateChanged);
                        btns.Add(btn);
                        _algGroupBoxItem.Controls.Add(btn);
                        idx++;
                    }
                    container.Controls.Add(_algGroupBoxItem);

                    _argBoxItem.Text = "算法参数:";
                    _argBoxItem.Location = new Point(0, y + 32);
                    _argBoxItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    _argBoxItem.Size = new Size(container.Width, container.Height);
                    AlgChanged(_selectedAlgorithm);
                    CreateArgPanel(_selectedAlgorithm, _argBoxItem);
                    _argBoxItem.Height = _argBoxItem.Height + 19;
                    container.Controls.Add(_argBoxItem);
                    //创建算法groups
                    if(!_listargBox.Keys.Contains(item.Key))
                    {
                        _listargBox.Add(item.Key, _argBoxItem);
                    }
                    if(!_listalgGroupBox.Keys.Contains(item.Key))
                    {
                        _listalgGroupBox.Add(item.Key, _algGroupBoxItem);
                    }
                    listallbtns.AddRange(btns);
                }
                #endregion 
                container.Height = _algGroupBoxItem.Height + _argBoxItem.Height + 18;
                listcontrol.Add(container);
                SetDefaultAlg(_selectedAlgorithm, listallbtns.ToArray());
                
            }
            //设置默认算法
           
            
               
                
        }
        private Dictionary<string,List<AlgorithmDef>> GetGroupAlgDefs()
        {
            
            Dictionary<string, List<AlgorithmDef>> dic = new Dictionary<string, List<AlgorithmDef>>();
            List<string> keygroups=new List<string>();
            //该步骤用了两次次循环，可以优化为只使用一次循环 
            for (int i = 0; i < _subProductDef.Algorithms.Length; i++)
            {
                string keyname = string.Empty;
               
                if (!keygroups.Contains(_subProductDef.Algorithms[i].GroupTypeName))
                {
                    keygroups.Add(_subProductDef.Algorithms[i].GroupTypeName);
                }
            }
            for (int i = 0; i < keygroups.Count; i++)
            {
                dic.Add(keygroups[i], _subProductDef.Algorithms.Where(o => o.GroupTypeName == keygroups[i]).ToList());
            }
            return dic;

        }

        #region AlgorithmDef
        void btn_ToggleStateChanged(object sender, StateChangedEventArgs args)
        {
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On || (sender as RadRadioButton).IsChecked)
            {
                AlgorithmDef alg = (sender as RadRadioButton).Tag as AlgorithmDef;
                //算法相同的情况下不触发算法已经改变的事件
                if (_selectedAlgorithm != null && _selectedAlgorithm.Identify == alg.Identify)
                    return;
                _selectedAlgorithm = GetAlgorithmDef(alg.Identify);
                AlgChanged(_selectedAlgorithm);
                //CreateArgPanel(_selectedAlgorithm, _argBox);
                string group = _selectedAlgorithm.GroupTypeName;
                if (!string.IsNullOrEmpty(group) && _listargBox != null)
                {
                    CreateArgPanel(_selectedAlgorithm, _listargBox[group]);
                }
               
            }
        }

        private AlgorithmDef GetAlgorithmDef(string identify)
        {
            if (string.IsNullOrWhiteSpace(identify))
                return null;
            foreach (AlgorithmDef alg in _subProductDef.Algorithms)
            {
                if (alg.Identify == identify)
                    return alg;
            }
            return null;
        }

        private void AlgChanged(AlgorithmDef alg)
        {
            _selectedAlgorithm = alg;
            if (_argumentProvider != null)
                SetCurArgOnly("AlgorithmName", _selectedAlgorithm.Identify);
            if (_algorithmChangedHandler != null)
                _algorithmChangedHandler(this, _selectedAlgorithm);
            SetCurrentValue();
        }

        private void SetCurrentValue()
        {
            ArgumentCopy argCopy = _algArgumentCopyDics[_selectedAlgorithm];
            foreach (string argName in argCopy.Args.Keys)
            {
                object value = argCopy.Args[argName];
                _argumentProvider.SetArg(argName, value);
            }
        }

        private AlgorithmDef GetDefaultAlg()
        {
            object obj = _argumentProvider.GetArg("AlgorithmName");
            if (obj == null)
                return _subProductDef.Algorithms[0];
            string algIdentyfy = obj.ToString();
            if (!string.IsNullOrWhiteSpace(algIdentyfy))
            {
                for (int i = 0; i < _subProductDef.Algorithms.Length; i++)
                {
                    if (_subProductDef.Algorithms[i].Identify == algIdentyfy)
                        return _subProductDef.Algorithms[i];
                }
            }
            return _subProductDef.Algorithms[0];
        }
        #endregion

        private void SetDefaultAlg(AlgorithmDef defaultAlg, RadRadioButton[] btns)
        {
            if (defaultAlg == null)
            {
                _selectedAlgorithm = btns[0].Tag as AlgorithmDef;
                btns[0].IsChecked = true;
                return;
            }
            foreach (RadRadioButton btn in btns)
            {
                if ((btn.Tag as AlgorithmDef).Identify == defaultAlg.Identify)
                {
                    _selectedAlgorithm = btn.Tag as AlgorithmDef;
                    btn.IsChecked = true;
                    return;
                }
            }
            _selectedAlgorithm = btns[0].Tag as AlgorithmDef;
            btns[0].IsChecked = true;
        }

        internal class ArgumentCopy
        {
            private Dictionary<string, object> _args = new Dictionary<string, object>();

            public Dictionary<string, object> Args
            {
                get
                {
                    return _args;
                }
            }

            public void SetArg(string name, object value)
            {
                if (_args.ContainsKey(name))
                    _args[name] = value;
                else
                    _args.Add(name, value);
            }

            public object GetArg(string name)
            {
                return _args.ContainsKey(name) ? _args[name] : null;
            }
        }

        private void CreateArgPanel(AlgorithmDef algDef, RadGroupBox argContainer)
        {
            
            if (argContainer == null)
                return;
            RadPanel algPanel = new RadPanel();
            algPanel.Dock = DockStyle.Top;
            algPanel.Height = 0;
            ArgumentBase[] args = algDef.Arguments;
            BuildArgument(algPanel, args);
            argContainer.Controls.Clear();
            argContainer.Controls.Add(algPanel);
            argContainer.Height = algPanel.Height + 19;
            //if (_algPageView != null && _algGroupBox != null)
            //    _algPageView.Height = _algGroupBox.Height + _argBox.Height + 56;
        }

        private void BuildArgument(Control algPanel, ArgumentBase[] args)
        {
            for (int j = args.Length - 1; j >= 0; j--)
            {
                ArgumentBase arg = args[j];
                if (arg.Visible)
                {
                    if (arg is ArgumentPair)
                    {
                        AddArgumentPair(arg as ArgumentPair, algPanel);
                    }
                    else if (arg is ArgumentGroup)
                    {
                        BuildArgumentGroup(arg as ArgumentGroup, algPanel);
                    }
                    else if (arg is ArgumentDef)
                    {
                        AddArgumentDef(arg as ArgumentDef, algPanel);
                    }
                }
            }
        }

        private void BuildArgumentGroup(ArgumentGroup argGroup, Control container)
        {
            if (argGroup.Arguments == null || argGroup.Arguments.Length == 0)
                return;
            Label title = new Label();
            title.AutoSize = false;
            title.Dock = DockStyle.Top;
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Height = 18;
            title.BackColor = Color.FromArgb(200, 153, 180, 209);
            title.ForeColor = Color.Blue;
            title.BorderStyle = BorderStyle.FixedSingle;
            title.Text = argGroup.Description;
            Panel gp = new Panel();
            gp.Height = 0;
            gp.Dock = DockStyle.Top;
            BuildArgument(gp, argGroup.Arguments);
            gp.Tag = gp.Height;
            container.Controls.Add(gp);
            container.Controls.Add(title);
            container.Height += (gp.Height + title.Height);
        }

        /// <summary>
        /// (1)如果minvalue和maxvalue属性都存在且不为空则生成滑块,滑块不标记有效段
        /// (2)如果minvalue和maxvalue属性都不存在或者值全部为空，则根据数据类型生成录入框
        ///     整数生成：NumericDown控件
        ///     浮点生成：DoubleTextBox
        /// (3)如果minvalue和maxvalue只存在一个，则按照(2)处理
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="container"></param>
        private void AddArgumentDef(ArgumentDef arg, Control container)
        {
            if (string.IsNullOrWhiteSpace(arg.Name) || arg.Name.ToUpper().EndsWith("_ZOOM") || arg.Name.ToUpper().EndsWith("_CENTERWAVENUM"))
                return;
            int height = 0;
            if (!string.IsNullOrWhiteSpace(arg.EditorUiProvider))
            {
                Control edit = CreateEditorUI(arg);
                if (edit == null)
                    return;
                edit.Tag = _argumentProvider;
                edit.Dock = DockStyle.Top;
                container.Controls.Add(edit);
                height += edit.Height;
            }
            else if (!string.IsNullOrWhiteSpace(arg.MinValue) && !string.IsNullOrWhiteSpace(arg.MaxValue))//有两个端值的一个值
            {
                #region MultiBarTrack
                Panel panel = new Panel();
                Label title = new Label();
                DoubleTextBox showValue = new DoubleTextBox();
                MultiBarTrack singleTrack = new MultiBarTrack();

                panel.Height = 21;
                panel.Left = 2;
                panel.Location = new Point(2, 2);
                panel.Dock = DockStyle.Top;

                title.Location = new Point(2, 2);
                title.TextAlign = ContentAlignment.MiddleLeft;
                title.AutoSize = true;
                title.Text = arg.Description + ":";

                showValue.Multiline = true;
                showValue.Size = new Size(57, 21);
                showValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                //showValue.Location = new Point(title.Right + 2, 2);
                showValue.Location = new Point(panel.Right - 122 - 2, 2);
                showValue.Text = arg.Defaultvalue;
                showValue.Tag = singleTrack;
                //showValue.KeyPressEnter += new KeyPressEventHandler(showValue_OnKeyPressEnter);
                showValue.LostFocusValueChanged += new EventHandler(showValue_LostFocusValueChanged);

                //singleTrack.Height = 34;
                singleTrack.Dock = DockStyle.Top;
                singleTrack.Left = 2;
                singleTrack.BarItemCount = 1;
                singleTrack.ValidPartion = MultiBarTrack.enumValidPartion.SinglePoint;
                singleTrack.MinEndPointValue = double.Parse(arg.MinValue);
                singleTrack.MaxEndPointValue = double.Parse(arg.MaxValue);
                object obj = GetArgValue(arg.Name);
                double argValue = 0f;
                if (obj != null)
                    double.TryParse(obj.ToString(), out argValue);
                singleTrack.SetValues(new double[] { argValue });
                singleTrack.Tag = new ArgumentTag(arg, new Control[] { showValue });
                singleTrack.BarValueChanged += new MultiBarTrack.BarValueChangedHandler(singleTrack_BarValueChanged);
                singleTrack.BarValueChangedFinished += new MultiBarTrack.BarValueChangedFinishedHandler(singleTrack_BarValueChangedFinished);
                singleTrack.DoubleClick += new EventHandler(pairTrack_DoubleClick);
                singleTrack.MouseClick += new MouseEventHandler(pairTrack_MouseClick);

                panel.Controls.Add(showValue);
                panel.Controls.Add(title);

                container.Controls.Add(singleTrack);
                container.Controls.Add(panel);
                height += singleTrack.Height;
                height += panel.Height;
                #endregion
            }
            else if (arg.RefType == "file")
            {
                #region RefType: file 文件型算法参数
                string name = arg.Description;
                string filter = arg.RefFilter;
                bool ismultifile = arg.IsMultiSelect;
                if (!ismultifile)
                {
                    height = CreateUIForSingleFileArg(arg, container, height);
                }
                else
                {
                    MultiFileSelect multiFile = new MultiFileSelect();
                    multiFile.Dock = DockStyle.Top;
                    multiFile.Size = new Size(container.Width, 124);
                    multiFile.FileChanged += new Action<object>(multiFile_FileChanged);
                    multiFile.FileFilter = arg.RefFilter;
                    multiFile.Title = arg.Description + ":";
                    multiFile.Tag = arg;
                    object varValue = GetSelectedFiles(arg);
                    if (varValue != null && (varValue as string[]) != null)
                    {
                        multiFile.Files = (varValue as string[]);
                    }
                    container.Controls.Add(multiFile);
                    height += multiFile.Height;
                }
                #endregion
            }
            else
            {
                #region ArgumentDef double float decimal int
                switch (arg.Datatype)
                {
                    case "bool":
                        {
                            Panel intPanel = new Panel();
                            intPanel.Dock = DockStyle.Top;
                            intPanel.Height = 26;
                            object obj = GetArgValue(arg.Name);
                            bool val = false;
                            if (obj != null)
                                bool.TryParse(obj.ToString(), out val);
                            CheckBox chkBox = new CheckBox();
                            chkBox.Width = 120;
                            chkBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                            chkBox.Location = new Point(2, 2);
                            chkBox.Checked = val;
                            chkBox.Tag = new ArgumentTag(arg, null);
                            chkBox.KeyPress += new KeyPressEventHandler(text_KeyPress);
                            chkBox.CheckedChanged += new EventHandler(chkBox_CheckedChanged);
                            chkBox.Text = arg.Description;

                            intPanel.Controls.Add(chkBox);

                            container.Controls.Add(intPanel);
                            height += intPanel.Height;
                        }
                        break;
                    case "int":
                    case "short":
                    case "uint":
                    case "ushort":
                    case "Int32":
                    case "Int16":
                    case "UInt16":
                    case "UInt32":
                        {
                            Panel intPanel = new Panel();
                            intPanel.Dock = DockStyle.Top;
                            intPanel.Height = 26;
                            object obj = GetArgValue(arg.Name);
                            int val = 0;
                            if (obj != null)
                                int.TryParse(obj.ToString(), out val);
                            NumericUpDown text = new NumericUpDown();
                            text.Width = 120;
                            text.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            text.Location = new Point(intPanel.Right - 120 - 2, 2);
                            text.Minimum = val - Math.Abs(2 * val);
                            text.Maximum = val + Math.Abs(2 * val);
                            text.Value = val;
                            text.Tag = new ArgumentTag(arg, null);
                            text.KeyPress += new KeyPressEventHandler(text_KeyPress);

                            Label title = new Label();
                            title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                            title.Location = new Point(2, 2);
                            title.TextAlign = ContentAlignment.MiddleCenter;
                            title.AutoSize = true;
                            title.Text = arg.Description + ":";

                            intPanel.Controls.Add(text);
                            intPanel.Controls.Add(title);

                            container.Controls.Add(intPanel);
                            height += intPanel.Height;
                        }
                        break;
                    case "string":
                        {
                            Panel intPanel = new Panel();
                            intPanel.Dock = DockStyle.Top;
                            intPanel.Height = 26;
                            object obj = GetArgValue(arg.Name);
                            TextBoxEx text = new TextBoxEx();
                            text.Width = 120;
                            text.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            text.Location = new Point(intPanel.Right - 120 - 2, 2);
                            text.Text = obj == null ? "" : obj.ToString();
                            text.Tag = new ArgumentTag(arg, null);
                            text.LostFocusValueChanged += new EventHandler(text_LostFocusValueChanged);

                            Label title = new Label();
                            title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                            title.Location = new Point(2, 2);
                            title.TextAlign = ContentAlignment.MiddleCenter;
                            title.AutoSize = true;
                            title.Text = arg.Description + ":";

                            intPanel.Controls.Add(text);
                            intPanel.Controls.Add(title);
                            container.Controls.Add(intPanel);
                            height += intPanel.Height;
                        }
                        break;
                    case "float":
                    case "double":
                    case "decimal":
                    case "":
                    default:
                        {
                            Panel intPanel = new Panel();
                            intPanel.Dock = DockStyle.Top;
                            intPanel.Height = 26;
                            object obj = GetArgValue(arg.Name);
                            DoubleTextBox text = new DoubleTextBox();
                            text.Width = 120;
                            text.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                            text.Location = new Point(intPanel.Right - 120 - 2, 2);
                            text.Text = obj == null ? "0" : obj.ToString();
                            text.Tag = new ArgumentTag(arg, null);
                            //text.KeyPressEnter += new KeyPressEventHandler(text_KeyPress);
                            text.LostFocusValueChanged += new EventHandler(text_LostFocusValueChanged);

                            Label title = new Label();
                            title.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                            title.Location = new Point(2, 2);
                            title.TextAlign = ContentAlignment.MiddleCenter;
                            title.AutoSize = true;
                            title.Text = arg.Description + ":";

                            intPanel.Controls.Add(text);
                            intPanel.Controls.Add(title);
                            container.Controls.Add(intPanel);
                            height += intPanel.Height;
                        }
                        break;
                }
                #endregion
            }
            container.Height += height;
        }

        private int CreateUIForSingleFileArg(ArgumentDef arg, Control container, int height)
        {
            Panel panel = new Panel();
            ComboBox comboBox1 = new ComboBox();
            Button btnFileArg = new Button();
            Label label = new Label();

            container.Controls.Add(panel);
            panel.Controls.Add(btnFileArg);
            panel.Controls.Add(comboBox1);
            panel.Controls.Add(label);

            panel.Dock = DockStyle.Top;
            panel.Name = "panel1";
            panel.Size = new Size(container.Width, 30);

            label.AutoSize = true;
            label.Location = new System.Drawing.Point(3, 6);
            label.Text = arg.Description + ":";

            comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top) | (System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
            comboBox1.Location = new System.Drawing.Point(label.Right + 3, 6);
            comboBox1.Size = new System.Drawing.Size(container.Width - label.Width - 32, 20);
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.Tag = arg;
            object val = GetArgValue(arg.Name);
            if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
            {
                comboBox1.Items.Add(val.ToString());
                comboBox1.Text = val.ToString();
            }
            object varValue = GetSelectedFiles(arg);
            if (varValue != null && (varValue as string[]) != null && (varValue as string[]).Length != 0
                && !string.IsNullOrWhiteSpace((varValue as string[])[0]))
            {
                string nearExtract = (varValue as string[])[0];
                if (!comboBox1.Items.Contains(nearExtract))
                    comboBox1.Items.Add(nearExtract);
                SetCurArgOnly(arg.Name, nearExtract);
                comboBox1.Text = nearExtract;
            }
            //获取当前影像
            if (arg.RefIdentify == "CurrentRaster" && _argumentProvider != null)
            {
                IEnvironmentVarProvider varPrd = _argumentProvider.EnvironmentVarProvider;
                if (varPrd != null)
                {
                    object crtfName = varPrd.GetVar("CurrentRasterFile");
                    if (crtfName != null)
                    {
                        comboBox1.Items.Add(crtfName.ToString());
                        comboBox1.SelectedIndex = 0;
                        _argumentProvider.SetArg(arg.Name, crtfName.ToString());
                    }
                }
            }
            comboBox1.SelectedValueChanged += new EventHandler(comboBox1_SelectedValueChanged);

            btnFileArg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            btnFileArg.Image = Resource1.cmdOpen;
            btnFileArg.Width = 24;
            btnFileArg.Height = 24;
            btnFileArg.Location = new System.Drawing.Point(container.Width - 24, 3);
            btnFileArg.Size = new System.Drawing.Size(24, 24);
            btnFileArg.UseVisualStyleBackColor = true;
            btnFileArg.Tag = new ArgumentTag(arg, new Control[] { comboBox1 });
            btnFileArg.Click += new EventHandler(btnArgFile_Click);

            height += panel.Height;
            return height;
        }

        void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            ArgumentDef arg = comboBox.Tag as ArgumentDef;
            string argValue = comboBox.Text;
            SetCurArgOnly(arg.Name, argValue);
            TrySetBandFromArgFile(arg);
        }

        private void TrySetBandFromArgFile(ArgumentDef arg)
        {
            if (_selectedAlgorithm == null)
                return;
            BandDef[] bands = _selectedAlgorithm.Bands;
            if (bands == null || bands.Length == 0)
                return;
            for (int i = 0; i < bands.Length; i++)
            {
                if (bands[i] != null && bands[i].FromArgument == arg.Name)
                {
                    object obj = _argumentProvider.GetArg(arg.Name);
                    if (obj != null)
                        TrySetBandFromFile(obj.ToString(), bands[i]);
                }
            }
        }

        private void TrySetBandFromFile(string fileName, BandDef banddef)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (IRasterDataProvider file = RasterDataDriver.Open(fileName) as IRasterDataProvider)
                    {
                        if (file != null)
                        {
                            DataIdentify identify = file.DataIdentify;
                            if (identify != null)
                            {
                                BandnameRefTable ss = BandRefTableHelper.GetBandRefTable(identify.Satellite, identify.Sensor);
                                BandnameItem bandItem = null;
                                foreach (float wave in banddef.Wavelength)
                                {
                                    bandItem = ss.GetBandItem(wave);
                                    if (bandItem != null)
                                        break;
                                }
                                if (bandItem != null)
                                {
                                    SetCurArgOnly(banddef.Identify, bandItem.Index);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        void chkBox_CheckedChanged(object sender, EventArgs e)
        {
            ArgumentTag tag = (sender as Control).Tag as ArgumentTag;
            SetCurArgOnly(tag.Arg.Name, (sender as CheckBox).Checked);
        }

        void text_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//Enter
            {
                ArgumentTag tag = (sender as Control).Tag as ArgumentTag;
                SetArg(tag, 0, (sender as Control).Text);
            }
        }

        void text_LostFocusValueChanged(object sender, EventArgs e)
        {
            ArgumentTag tag = (sender as Control).Tag as ArgumentTag;
            SetArg(tag, 0, (sender as Control).Text);
        }

        void showValue_LostFocusValueChanged(object sender, EventArgs e)
        {
            try
            {
                DoubleTextBox txtBox = sender as DoubleTextBox;
                MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
                ArgumentTag tag = singleTrack.Tag as ArgumentTag;
                //if (e.KeyChar == (char)Keys.Enter)
                //{
                double newValue = singleTrack.SetValueAt(0, txtBox.Value);
                txtBox.Text = newValue.ToString("0.##");
                //}
            }
            finally
            {
            }
        }

        void multiFile_FileChanged(object sender)
        {
            MultiFileSelect multiFile = sender as MultiFileSelect;
            ArgumentDef arg = multiFile.Tag as ArgumentDef;
            string[] argValue = multiFile.Files;
            SetCurArgOnly(arg.Name, argValue);
        }

        private object GetSelectedFiles(ArgumentDef arg)
        {
            IEnvironmentVarProvider varPrd = _argumentProvider.EnvironmentVarProvider;
            if (varPrd != null)
            {
                object varValue = varPrd.GetVar(arg.Name);
                if (varValue != null)
                {
                    if (varValue is object[] && (varValue as object[]).Length != 0)
                        return varValue;
                }
                if (_wks != null)
                {
                    string[] selectedFile = null;
                    if (_wks.ActiveCatalog != null)
                    {
                        string refidentify = arg.RefIdentify;
                        if (!string.IsNullOrEmpty(refidentify))
                        {
                            string[] subRefDif = refidentify.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            List<string> selectFileLst = new List<string>();
                            string[] selectTemp = null;
                            foreach (string item in subRefDif)
                            {
                                selectTemp = _wks.ActiveCatalog.GetSelectedFiles(item);
                                if (selectTemp != null && selectTemp.Length != 0)
                                    selectFileLst.AddRange(selectTemp);
                            }
                            selectedFile = selectFileLst.Count == 0 ? null : selectFileLst.ToArray();
                        }
                    }
                    return selectedFile;
                }
            }
            return null;
        }

        void btnArgFile_Click(object sender, EventArgs e)
        {
            if (_argumentProvider == null)
                return;
            ArgumentTag tag = (sender as Control).Tag as ArgumentTag;
            ArgumentDef arg = tag.Arg as ArgumentDef;
            ComboBox control = tag.Labels[0] as ComboBox;
            if (arg == null)
                return;
            using (OpenFileDialog filediag = new OpenFileDialog())
            {
                filediag.Filter = arg.RefFilter;
                //filediag.Multiselect = arg.IsMultiSelect;
                //filediag.InitialDirectory = MifEnvironment.GetWorkspaceDir();
                if (filediag.ShowDialog() == DialogResult.OK)
                {
                    string argValue = filediag.FileName;
                    if (!control.Items.Contains(argValue))
                        control.Items.Add(argValue);
                    control.Text = argValue;
                    SetArgChanged(arg.Name, argValue);
                    TrySetBandFromArgFile(arg);
                }
            }
        }

        private Control CreateEditorUI(ArgumentDef arg)
        {
            try
            {
                string editUiProviderstring = arg.EditorUiProvider;
                string[] vs = editUiProviderstring.Split(':');
                if (vs == null || vs.Length != 2)
                    return null;
                string assFileName = vs[0];
                string typeName = vs[1];
                Assembly asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + assFileName);
                dynamic obj = asm.CreateInstance(typeName, true);   //IArgumentEditorUI
                if (obj != null)
                {
                    if (obj is IArgumentEditorUI)
                    {
                        obj.SetChangeHandler(new Action<object>((argValue) =>
                        {
                            SetCurArgOnly(arg.Name, argValue);
                            //SetArgChanged(arg.Name, argValue);
                        }
                        ));
                        object value = obj.ParseArgumentValue(arg.DefaultValueElement);
                    }
                    else if (obj is IArgumentEditorUI2)
                    {
                        obj.SetChangeHandler(new Action<object>((argValue) =>
                        {
                            if (obj.IsExcuteArgumentValueChangedEvent)
                                SetArgChanged(arg.Name, argValue);
                            else
                                SetCurArgOnly(arg.Name, argValue);
                        }
                        ));
                        object value = obj.ParseArgumentValue(arg.DefaultValueElement);
                        obj.InitControl(_container as IExtractPanel, arg);
                    }
                }
                return obj;
            }
            catch
            {
                return null;
            }
        }

        private void AddArgumentPair(ArgumentPair arg, Control container)
        {
            Panel panel = new Panel();
            Label title = new Label();
            DoubleTextBox minValue = new DoubleTextBox();
            DoubleTextBox maxValue = new DoubleTextBox();
            MultiBarTrack pairTrack = CreateTrack(arg as ArgumentPair);

            panel.Height = 25;
            panel.Left = 2;
            panel.Location = new Point(2, 2);
            panel.Dock = DockStyle.Top;

            title.Location = new Point(2, 2);
            title.TextAlign = ContentAlignment.MiddleLeft;
            title.Text = arg.Description + ":";

            //minValue.Location = new Point(title.Width + 2, 2);
            minValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            minValue.Location = new Point(panel.Right - 120 - 2, 2);
            minValue.Multiline = true;
            minValue.Size = new Size(58, 21);
            if ((arg as ArgumentPair).ArgumentMin != null)
            {
                object val = GetArgValue((arg as ArgumentPair).ArgumentMin.Name);
                if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    val = (arg as ArgumentPair).ArgumentMin.Defaultvalue;
                minValue.Text = val == null ? " " : val.ToString();
                minValue.FineTuning = GetDouble((arg as ArgumentPair).FineTuning);
                minValue.Tag = pairTrack;
                //minValue.KeyPressEnter += new KeyPressEventHandler(minValue_KeyPressEnter);
                minValue.LostFocusValueChanged += new EventHandler(minValue_LostFocusValueChanged);
            }
            else
            {
                minValue.Text = "";
                minValue.Visible = false;
            }

            maxValue.Location = new Point(panel.Right - 58 - 2, 2);
            maxValue.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            maxValue.Multiline = true;
            maxValue.Size = new Size(58, 21);
            if ((arg as ArgumentPair).ArgumentMax != null)
            {
                object val = GetArgValue((arg as ArgumentPair).ArgumentMax.Name);
                if (val == null || string.IsNullOrWhiteSpace(val.ToString()))
                    val = (arg as ArgumentPair).ArgumentMax.Defaultvalue;
                maxValue.Text = val == null ? " " : val.ToString();
                maxValue.FineTuning = GetDouble((arg as ArgumentPair).FineTuning);
                maxValue.Tag = pairTrack;
                //maxValue.KeyPressEnter += new KeyPressEventHandler(maxValue_KeyPressEnter);
                maxValue.LostFocusValueChanged += new EventHandler(maxValue_LostFocusValueChanged);
            }
            else
            {
                maxValue.Text = "";
                maxValue.Visible = false;
            }

            panel.Controls.Add(maxValue);
            panel.Controls.Add(minValue);
            panel.Controls.Add(title);

            pairTrack.Tag = new ArgumentTag(arg, new Control[] { minValue, maxValue });
            pairTrack.BarValueChanged += new MultiBarTrack.BarValueChangedHandler(pairTrack_BarValueChanged);
            pairTrack.BarValueChangedFinished += new MultiBarTrack.BarValueChangedFinishedHandler(pairTrack_BarValueChangedFinished);
            pairTrack.DoubleClick += new EventHandler(pairTrack_DoubleClick);
            pairTrack.MouseClick += new MouseEventHandler(pairTrack_MouseClick);

            container.Controls.Add(pairTrack);
            container.Controls.Add(panel);
            container.Height += pairTrack.Height;
            container.Height += panel.Height;
        }

        private double GetDouble(string doubleStr)
        {
            double v = 1;
            if (string.IsNullOrEmpty(doubleStr))
                return 1;
            else if (double.TryParse(doubleStr, out v))
                return v;
            return 1;
        }

        void maxValue_LostFocusValueChanged(object sender, EventArgs e)
        {
            DoubleTextBox txtBox = sender as DoubleTextBox;
            MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
            ArgumentTag tag = singleTrack.Tag as ArgumentTag;
            int barIndex = 0;
            if (singleTrack.BarItemCount == 2)
                barIndex = 1;
            double newValue = singleTrack.SetValueAt(barIndex, txtBox.Value);
            txtBox.Text = newValue.ToString("0.##");
        }

        void minValue_LostFocusValueChanged(object sender, EventArgs e)
        {
            DoubleTextBox txtBox = sender as DoubleTextBox;
            MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
            ArgumentTag tag = singleTrack.Tag as ArgumentTag;
            double newValue = singleTrack.SetValueAt(0, txtBox.Value);
            txtBox.Text = newValue.ToString("0.##");
        }

        /// <summary>
        /// 
        /// </summary>
        /// 需同步修改云判识产品：GeoDo.RSS.MIF.Prds.Comm\Controls\UCCloudMethod.cs 在UI中
        private ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        /// <summary>
        /// 滑块移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pairTrack_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Items.Clear();
                ToolStripMenuItem mnuSetEndpointValue = new ToolStripMenuItem("设置端值");
                mnuSetEndpointValue.Tag = sender;
                mnuSetEndpointValue.Click += new EventHandler(mnuSetEndpointValue_Click);
                contextMenuStrip1.Items.Add(mnuSetEndpointValue);
                string minPoint = ((sender as MultiBarTrack).Tag as ArgumentTag).Arg.MinValue;
                string maxPoint = ((sender as MultiBarTrack).Tag as ArgumentTag).Arg.MaxValue;
                double minValue = int.Parse(minPoint);
                double maxValue = int.Parse(maxPoint);
                ToolStripMenuItem quickValue = new ToolStripMenuItem("默认端值：" + minValue + " " + maxValue);
                quickValue.Tag = new object[] { sender, new double[] { minValue, maxValue } };
                quickValue.Click += new EventHandler(quickValue_Click);
                contextMenuStrip1.Items.Add(quickValue);
                if (_monitoringSubProduct.Definition.IsNeedCurrentRaster)
                {
                    if (_dataProvider != null)
                    {
                        foreach (BandDef bandDef in _selectedAlgorithm.Bands)
                        {
                            if (bandDef == null)
                                continue;
                            int bandNo = (int)_argumentProvider.GetArg(bandDef.Identify);
                            if (bandNo >= 0 && bandNo <= _dataProvider.BandCount)
                            {
                                IRasterBand rasterBand = _dataProvider.GetRasterBand(bandNo);
                                if (rasterBand != null)
                                {
                                    _dataProvider.GetStretcher(bandNo);
                                    LinearRgbStretcherUInt16 stretcher = rasterBand.Stretcher as LinearRgbStretcherUInt16;// StretcherConfigItem;
                                    if (stretcher != null)
                                    {
                                        quickValue = new ToolStripMenuItem("取波段设置端值：" + minValue + " " + maxValue);
                                        quickValue.Tag = new object[] { sender, new double[] { minValue, maxValue } };
                                        quickValue.Click += new EventHandler(quickValue_Click);
                                        contextMenuStrip1.Items.Add(quickValue);
                                    }
                                }
                            }
                        }
                    }
                }
                contextMenuStrip1.Show(sender as MultiBarTrack, e.Location);
            }
        }

        void quickValue_Click(object sender, EventArgs e)
        {
            object[] obj = (sender as ToolStripMenuItem).Tag as object[];
            MultiBarTrack bar = obj[0] as MultiBarTrack;
            Double[] values = obj[1] as Double[];
            bar.MinEndPointValue = values[0];
            bar.MaxEndPointValue = values[1];
        }

        void mnuSetEndpointValue_Click(object sender, EventArgs e)
        {
            MultiBarTrack bar = (sender as ToolStripMenuItem).Tag as MultiBarTrack;
            SetEndPoint(bar);
        }

        void maxValue_KeyPressEnter(object sender, KeyPressEventArgs e)
        {
            DoubleTextBox txtBox = sender as DoubleTextBox;
            MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
            ArgumentTag tag = singleTrack.Tag as ArgumentTag;
            int barIndex = 0;
            if (singleTrack.BarItemCount == 2)
                barIndex = 1;
            double newValue = singleTrack.SetValueAt(barIndex, txtBox.Value);
            txtBox.Text = newValue.ToString("0.##");
        }

        //void minValue_KeyPressEnter(object sender, KeyPressEventArgs e)
        //{
        //    DoubleTextBox txtBox = sender as DoubleTextBox;
        //    MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
        //    ArgumentTag tag = singleTrack.Tag as ArgumentTag;
        //    double newValue = singleTrack.SetValueAt(0, txtBox.Value);
        //    txtBox.Text = newValue.ToString("0.##");
        //}

        void pairTrack_DoubleClick(object sender, EventArgs e)
        {
            MultiBarTrack bar = sender as MultiBarTrack;
            SetEndPoint(bar);
        }

        private void SetEndPoint(MultiBarTrack bar)
        {
            using (frmSetEndpointValue frm = new frmSetEndpointValue(bar.MinEndPointValue, bar.MaxEndPointValue))
            {
                frm.Location = Control.MousePosition;
                if (frm.Location.X + frm.Width > Screen.GetBounds(_container).Width)
                    frm.Location = new Point(Screen.GetBounds(_container).Width - frm.Width, Control.MousePosition.Y);
                if (frm.ShowDialog(bar) == DialogResult.OK)
                {
                    bar.MinEndPointValue = frm.MinValue;
                    bar.MaxEndPointValue = frm.MaxValue;
                    ArgumentTag tag = bar.Tag as ArgumentTag;
                    if (tag != null && tag.Labels != null)
                    {
                        Control[] labels = tag.Labels;
                        foreach (Control item in labels)
                        {
                            if (item is DoubleTextBox)
                                (item as DoubleTextBox).FineTuning = frm.FineTuningValue;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据算法参数创建参数面板内容
        /// </summary>
        /// <param name="argumentPair"></param>
        /// <returns></returns>
        private MultiBarTrack CreateTrack(ArgumentPair argumentPair)
        {
            if (argumentPair == null || (argumentPair.ArgumentMin == null && argumentPair.ArgumentMax == null))
                return null;
            MultiBarTrack track = new MultiBarTrack();
            track.Height = 34;
            track.Dock = DockStyle.Top;
            track.Left = 2;
            string datatype = argumentPair.Datatype;
            double minPointValue;
            double maxPointValue;
            double.TryParse(argumentPair.MinValue, out minPointValue);
            double.TryParse(argumentPair.MaxValue, out maxPointValue);
            track.MinEndPointValue = minPointValue;
            track.MaxEndPointValue = maxPointValue;
            double minValue;
            double maxValue;
            if (argumentPair.ArgumentMin != null && argumentPair.ArgumentMax != null)
            {
                object val = GetArgValue(argumentPair.ArgumentMin.Name);
                if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                    double.TryParse(val.ToString(), out minValue);
                else
                    double.TryParse(argumentPair.ArgumentMin.Defaultvalue, out minValue);
                val = GetArgValue(argumentPair.ArgumentMax.Name);
                if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                    double.TryParse(val.ToString(), out maxValue);
                else
                    double.TryParse(argumentPair.ArgumentMax.Defaultvalue, out maxValue);
                track.BarItemCount = 2;
                track.ValidPartion = MultiBarTrack.enumValidPartion.MiddleSegment;
                track.SetValues(new double[] { minValue, maxValue });
            }
            else if (argumentPair.ArgumentMin != null)
            {
                object val = GetArgValue(argumentPair.ArgumentMin.Name);
                if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                    double.TryParse(val.ToString(), out minValue);
                else
                    double.TryParse(argumentPair.ArgumentMin.Defaultvalue, out minValue);
                track.BarItemCount = 1;
                track.ValidPartion = MultiBarTrack.enumValidPartion.RightSegment;
                track.SetValues(new double[] { minValue });
            }
            else
            {
                object val = GetArgValue(argumentPair.ArgumentMax.Name);
                if (val != null && !string.IsNullOrWhiteSpace(val.ToString()))
                    double.TryParse(val.ToString(), out maxValue);
                else
                    double.TryParse(argumentPair.ArgumentMax.Defaultvalue, out maxValue);
                track.BarItemCount = 1;
                track.ValidPartion = MultiBarTrack.enumValidPartion.LeftSegment;
                track.SetValues(new double[] { maxValue });
            }
            return track;
        }

        void showValue_OnKeyPressEnter(object sender, KeyPressEventArgs e)
        {
            try
            {
                DoubleTextBox txtBox = sender as DoubleTextBox;
                MultiBarTrack singleTrack = txtBox.Tag as MultiBarTrack;
                ArgumentTag tag = singleTrack.Tag as ArgumentTag;
                if (e.KeyChar == (char)Keys.Enter)
                {
                    double newValue = singleTrack.SetValueAt(0, txtBox.Value);
                    txtBox.Text = newValue.ToString("0.##");
                }
            }
            finally
            {
            }
        }

        void singleTrack_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            SetArg((sender as Control).Tag as ArgumentTag, barIndex, value);
        }

        void singleTrack_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            ArgumentTag tag = (sender as MultiBarTrack).Tag as ArgumentTag;
            ValueChanging(tag, barIndex, value);
        }

        void pairTrack_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            SetArg((sender as Control).Tag as ArgumentTag, barIndex, value);
        }

        void pairTrack_BarValueChanged(object sender, int barIndex, double value, System.Drawing.Point location)
        {
            ArgumentTag tag = (sender as MultiBarTrack).Tag as ArgumentTag;
            ValueChanging(tag, barIndex, value);
        }

        private void SetArgChanged(string argName, object argValue)
        {
            ArgumentBase argDef = _argumentProvider.GetArgDef(argName);
            SetCurArgOnly(argName, argValue);
            if (argDef != null && argDef.IsEventNotification && _argumentValueChangedHandler != null)
                _argumentValueChangedHandler(this, _argumentProvider);
        }

        private void SetArg(ArgumentTag argTag, int barIndex, object value)
        {
            if (_argumentProvider == null || argTag == null)
                return;
            ArgumentBase arg = argTag.Arg;
            if (arg is ArgumentPair)
            {
                object objValue = GetActualValue(arg.Datatype, value);
                if (barIndex == 0)
                {
                    if ((arg as ArgumentPair).ArgumentMin != null)
                        SetCurArgOnly((arg as ArgumentPair).ArgumentMin.Name, objValue);
                    else
                        SetCurArgOnly((arg as ArgumentPair).ArgumentMax.Name, objValue);
                }
                else
                    SetCurArgOnly((arg as ArgumentPair).ArgumentMax.Name, objValue);
            }
            else if (arg is ArgumentDef)
            {
                object obj = GetActualValue(arg.Datatype, value);
                SetCurArgOnly(arg.Name, obj);
            }
            if (arg.IsEventNotification && _argumentValueChangedHandler != null)
                _argumentValueChangedHandler(this, _argumentProvider);
        }

        private void SetCurArgOnly(string name, object value)
        {
            _argumentProvider.SetArg(name, value);
            UpdateArgCopy(name, value);
        }

        private void UpdateArgCopy(string argName, object ArgValue)
        {
            if (_algArgumentCopyDics.ContainsKey(_selectedAlgorithm))
                _algArgumentCopyDics[_selectedAlgorithm].SetArg(argName, ArgValue);
            else
            {
                ArgumentCopy argCopy = new ArgumentCopy();
                argCopy.SetArg(argName, ArgValue);
                _algArgumentCopyDics.Add(_selectedAlgorithm, argCopy);
            }
        }

        private object GetArgValue(string argName)
        {
            object value = _argumentProvider.GetArg(argName);
            UpdateArgCopy(argName, value);
            return value;
        }

        private static object GetActualValue(string type, object obj)
        {
            if (type == null)
                return null;
            type = type.ToUpper();
            string svalue = obj == null ? null : obj.ToString();
            switch (type)
            {
                case "BYTE":
                    {
                        byte ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !byte.TryParse(svalue, out ret))
                            return (byte)0;
                        else
                            return ret;
                    }
                case "UINT16":
                    {
                        ushort ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !ushort.TryParse(svalue, out ret))
                            return (ushort)0;
                        else
                            return ret;
                    }
                case "INT16":
                    {
                        short ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !short.TryParse(svalue, out ret))
                            return (short)0;
                        else
                            return ret;
                    }
                case "INT32":
                    {
                        int ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !int.TryParse(svalue, out ret))
                            return (int)0;
                        else
                            return ret;
                    }
                case "UINT32":
                    {
                        uint ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !uint.TryParse(svalue, out ret))
                            return (uint)0;
                        else
                            return ret;
                    }
                case "DOUBLE":
                    {
                        double ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !double.TryParse(svalue, out ret))
                            return (double)0;
                        else
                            return ret;
                    }
                case "SINGLE":
                case "FLOAT":
                    {
                        float ret = 0;
                        if (string.IsNullOrWhiteSpace(svalue) || !float.TryParse(svalue, out ret))
                            return (float)0;
                        else
                            return ret;
                    }
                case "STRING":
                    return svalue;
                case "CHAR":
                    if (svalue.Length > 0)
                        return svalue.ToCharArray()[0];
                    break;
                case "OBJECT":
                default:
                    return svalue;
            }
            return null;
        }

        private void ValueChanging(ArgumentTag argTag, int barIndex, double value)
        {
            if (argTag.Labels == null)
                return;
            Control[] lables = argTag.Labels;
            ArgumentBase arg = argTag.Arg;
            if (arg is ArgumentPair)
            {
                object objValue = GetActualValue(arg.Datatype, value);
                if (barIndex == 0)
                {
                    if ((arg as ArgumentPair).ArgumentMin != null)
                    {
                        lables[0].Text = value.ToString("0.##");
                        SetCurArgOnly((arg as ArgumentPair).ArgumentMin.Name, objValue);
                    }
                    else
                    {
                        lables[1].Text = value.ToString("0.##");
                        SetCurArgOnly((arg as ArgumentPair).ArgumentMax.Name, objValue);
                    }
                }
                else
                {
                    lables[1].Text = value.ToString("0.##");
                    SetCurArgOnly((arg as ArgumentPair).ArgumentMax.Name, objValue);
                }
            }
            else if (arg is ArgumentDef)
            {
                object objValue = GetActualValue(arg.Datatype, value);
                lables[0].Text = value.ToString("0.##");
                SetCurArgOnly(arg.Name, objValue);
            }
            if (_argumentValueChangingHandler != null)
                _argumentValueChangingHandler(this, _argumentProvider);
        }

        void gp_MouseClick(object sender, MouseEventArgs e)
        {
            if ((sender as RadGroupBox).Height == 30)
                (sender as RadGroupBox).Height = (int)(sender as RadGroupBox).Tag;
            else
                (sender as RadGroupBox).Height = 30;
        }


        void IExtractPanelBuilder.SetArg(string argName, object argValue)
        {
            SetArgChanged(argName, argValue);
        }
    }
}
