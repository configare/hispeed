using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCStageToArea : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler = null;
        private List<StageRegionDef> _stageRegions = null;
        private IArgumentProvider _argumentProvider = null;

        public UCStageToArea()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            List<string> contexts = new List<string>();
            contexts.Add("公式:" + txtFormula.Text);
            contexts.Add("水位:" + txtStage.Text);
            contexts.Add("面积:" + lbStageArea.Text);
            contexts.Add("备注:" + txtTip.Text);
            return contexts.ToArray();
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            //如果需要自动定位  实现
            if (_stageRegions == null || _stageRegions.Count == 0)
                return;
            cbRegion.Items.Clear();
            foreach (StageRegionDef region in _stageRegions)
                cbRegion.Items.Add(region.Name);
            cbRegion.SelectedIndex = 0;
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
                _argumentProvider = arp;
                CoordEnvelope dataCoordEnv = arp.DataProvider.CoordEnvelope;
                Envelope dataEnvelope = new Envelope(dataCoordEnv.MinX, dataCoordEnv.MinY, dataCoordEnv.MaxX, dataCoordEnv.MaxY);
                foreach (StageRegionDef item in _stageRegions)
                {
                    if (item.Evelope != null && dataEnvelope.Contains(item.Evelope))
                    {
                        cbRegion.Text = item.Name;
                        break;
                    }
                }
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
            //解析区域及初始化公式
            var eles = ele.Elements("Region");
            if (eles == null)
                return null;
            _stageRegions = new List<StageRegionDef>();
            string name = null;
            string formula = null;
            string envlopeStr = null;
            StageRegionDef stageRegion = null;
            foreach (XElement item in eles)
            {
                name = item.Attribute("name").Value;
                formula = item.Attribute("formula").Value;
                envlopeStr = item.Attribute("envelope").Value;
                stageRegion = new StageRegionDef(name, formula);
                stageRegion.Evelope = GetRegionEnvelope(envlopeStr);
                if (!_stageRegions.Contains(stageRegion))
                    _stageRegions.Add(stageRegion);
            }
            return _stageRegions;
        }

        private Envelope GetRegionEnvelope(string envlopeStr)
        {
            if (string.IsNullOrEmpty(envlopeStr))
                return null;
            string[] envelopeSplit = envlopeStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (envelopeSplit == null || envelopeSplit.Length != 4)
                return null;
            //minx maxx miny maxy
            return new Envelope(float.Parse(envelopeSplit[0]), float.Parse(envelopeSplit[2]),
                                float.Parse(envelopeSplit[1]), float.Parse(envelopeSplit[3]));
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void txtStage_TextChanged(object sender, EventArgs e)
        {
            //根据计算公式，具体计算面积
            float area = GetArea();
            if (area < 0) 
            {
                area = 0;
            }
            lbStageArea.Text = area.ToString("0.00");
        }

        private float GetArea()
        {
            float area = 0f;
            string model = txtFormula.Text.ToLower().Replace("x", txtStage.Text);
            IFeatureSimpleComputeFuncProvider<float, float> prd = ExtractFuncProviderFactory.CreateSimpleFeatureComputeFuncProvider<float, float>(model);
            Func<float, float> _featureComputer = prd.GetComputeFunc();
            if (_featureComputer != null)
                area = _featureComputer(0);
            if (_handler != null)
                _handler(GetArgumentValue());
            return area;
        }

        private void cbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string regionName = cbRegion.Text;
            if (string.IsNullOrEmpty(regionName))
                return;
            foreach (StageRegionDef item in _stageRegions)
            {
                if (item.Name == regionName)
                {
                    txtFormula.Text = item.Formula;
                    break;
                }
            }
        }

        private void txtFormula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                float area = GetArea();
                lbStageArea.Text = area.ToString("0.00");
            }
        }

        private void txtTip_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());
        }
    }

    public class StageRegionDef
    {
        public string Name;
        public string Formula;
        public Envelope Evelope;

        public StageRegionDef(string name, string formula)
        {
            Name = name;
            Formula = formula;
        }
    }
}
