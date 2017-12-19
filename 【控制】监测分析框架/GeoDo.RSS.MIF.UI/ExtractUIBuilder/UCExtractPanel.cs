using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.UI
{
    public partial class UCExtractPanel : UserControl, IExtractPanel
    {
        private IExtractPanelBuilder _builder;
        private IWorkspace _wks;
        private IMonitoringSubProduct _monitoringSubProduct;

        public event OnAlgorithmChangedHandler OnAlgorithmChanged;
        public event OnArgumentValueChangingHandler OnArgumentValueChanging;
        public event OnArgumentValueChangedHandler OnArgumentValueChanged;
        public event OnAOITempleteChangedHandler OnAOITempleteChanged;

        public UCExtractPanel()
        {
            InitializeComponent();
            _builder = new ExtractPanelBuilder(this);
            (_builder as ExtractPanelBuilder).OnAlgorithmChanged += new OnAlgorithmChangedHandler
                (
                (obj, alg) =>
                {
                    if (OnAlgorithmChanged != null)
                        OnAlgorithmChanged(this, alg);
                }
                );
            (_builder as ExtractPanelBuilder).OnArgumentValueChanged += new OnArgumentValueChangedHandler(
                (sender, arg) =>
                {
                    if (OnArgumentValueChanged != null)
                        OnArgumentValueChanged(sender, arg);
                });
            (_builder as ExtractPanelBuilder).OnArgumentValueChanging += new OnArgumentValueChangingHandler(
                (sender, arg) =>
                {
                    if (OnArgumentValueChanging != null)
                        OnArgumentValueChanging(sender, arg);
                });
            (_builder as ExtractPanelBuilder).OnAOITempleteChanged += new OnAOITempleteChangedHandler(
                (sender, aois) =>
                {
                    if (OnAOITempleteChanged != null)
                        OnAOITempleteChanged(sender, aois);
                });
        }

        public void Apply(IWorkspace wks, IMonitoringSubProduct subProduct)
        {
            if (subProduct != null && subProduct.Definition.IsKeepUserControl
                && _monitoringSubProduct != null && _monitoringSubProduct.Definition.IsKeepUserControl)
                return;
            _wks = wks;
            _monitoringSubProduct = subProduct;
            _builder.Build(wks, subProduct);
        }

        /// <summary>
        /// 使控制面板可以自动隐藏，或控制面板中的内容可以被替换
        /// </summary>
        public void CanResetUserControl()
        {
            if (_monitoringSubProduct != null && _monitoringSubProduct.Definition.IsKeepUserControl)
                _monitoringSubProduct.Definition.IsKeepUserControl = false;
        }

        public string[] AOIs
        {
            get { return _builder == null ? null : (_builder as ExtractPanelBuilder).AOIs; }
        }

        public void SetArg(string argName, object argValue)
        {
            _builder.SetArg(argName, argValue);
        }

        public IMonitoringSubProduct MonitoringSubProduct
        {
            get { return _monitoringSubProduct; }
        }

        public object MonitoringSubProductTag
        {
            get
            {
                if (_monitoringSubProduct != null && (_monitoringSubProduct as MonitoringSubProduct) != null)
                    return (_monitoringSubProduct as MonitoringSubProduct).Tag;
                return null;
            }
            set
            {
                if (_monitoringSubProduct != null && (_monitoringSubProduct as MonitoringSubProduct) != null)
                    (_monitoringSubProduct as MonitoringSubProduct).Tag = value;
            }
        }

        public IWorkspace Workspace
        {
            get { return _wks; }
        }

        public void ResetArgsValue()
        {
            //foreach (string argName in _monitoringSubProduct.ArgumentProvider.ArgNames)
            //{
            //    ArgumentDef argDef = _monitoringSubProduct.ArgumentProvider.GetArgDef(argName);
            //    if (argDef == null || string.IsNullOrWhiteSpace(argDef.Defaultvalue))
            //        continue;
            //    _monitoringSubProduct.ArgumentProvider.SetArg(argName, argDef.Defaultvalue);
            //}
            //IArgumentProvider prd = _monitoringSubProduct.ArgumentProvider;
            //if (prd != null && prd.GetArg("AlgorithmName") != null)
            //    _monitoringSubProduct.ResetArgumentProvider(prd.GetArg("AlgorithmName").ToString());

            //_builder.Build(_wks, _monitoringSubProduct);
            _builder.ResetDefaultValue();
        }
    }
}
