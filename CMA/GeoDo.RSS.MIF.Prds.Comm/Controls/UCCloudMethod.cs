using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public partial class UCCloudMethod : UserControl, IArgumentEditorUI2
    {
        private bool _useNDSI = true;
        private double _visiableMinValue = 20f;
        private double _FarInfraredMaxValue = 275f;
        private double _NDSImin = -1;
        private double _NDSIMax = 0.3;
        private double _NearVisiableMin = 0;
        private double _NearVisiableMax = 2;
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;
        private bool _useNearVisiable = true;
        private bool _useFarInfrared = true;
        private bool _printFeatrues = false;
        private IRasterDataProvider _dataProvider = null;

        private bool _useNSVI = false;

        public UCCloudMethod()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> GetCloudPro()
        {
            if (Tag == null)
                return null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("VisibleMin", _visiableMinValue.ToString());
            dic.Add("UseNDSI", _useNDSI.ToString());
            if (_useNDSI)
            {
                dic.Add("NDSIMin", _NDSImin.ToString());
                dic.Add("NDSIMax", _NDSIMax.ToString());
            }
            dic.Add("UseFarInfrared", _useFarInfrared.ToString());
            if (_useFarInfrared)
                dic.Add("FarInfraredMax", _FarInfraredMaxValue.ToString());
            dic.Add("UseNearVisiable", _useNearVisiable.ToString());
            if (_useNearVisiable)
            {
                dic.Add("NearVisableMin", _NearVisiableMin.ToString());
                dic.Add("NearVisableMax", _NearVisiableMax.ToString());
            }
            if (_printFeatrues)
                dic.Add("PrintFeatrue", _printFeatrues.ToString());
            if(_useNSVI)//近红外/短波红外 配置参数
            {
                dic.Add("NSVI", this.txtnsvi.Text.Trim());
            }
            return dic;
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
            if (arp == null)
                return;
            Tag = arp;
            _dataProvider = arp.DataProvider;
            if (_handler != null)
                _handler(GetCloudPro());
        }

        public object GetArgumentValue()
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _isExcuteArgumentValueChangedEvent = true;
            if (_handler != null)
                _handler(GetCloudPro());
            _isExcuteArgumentValueChangedEvent = false;
        }

        /*
           <NDSI value="false"/>
           <Visiabel value="20"/>
           <FarInfrared value="275"/>
           <NDSIMin value="-1"/>
           <NDSIMax value="0.3"/>
         */
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            XElement node = ele.Element("NDSI");
            if (node != null)
                _useNDSI = bool.Parse(node.Value);
            node = ele.Element("NearVisiable");
            if (node != null)
                _useNearVisiable = bool.Parse(node.Value);
            node = ele.Element("FarInfrared");
            if (node != null)
                _useFarInfrared = bool.Parse(node.Value);
            node = ele.Element("VisiableMin");
            if (node != null)
                _visiableMinValue = double.Parse(node.Value);
            node = ele.Element("FarInfraredMax");
            if (node != null)
                _FarInfraredMaxValue = double.Parse(node.Value);
            node = ele.Element("NDSIMin");
            if (node != null)
                _NDSImin = double.Parse(node.Value);
            node = ele.Element("NDSIMax");
            if (node != null)
                _NDSIMax = double.Parse(node.Value);
            node = ele.Element("PrintFeatrues");
            if (node != null)
                _printFeatrues = bool.Parse(node.Value);
            // 近红外/短波红外
            node = ele.Element("NSVI");
            if (node != null)
                _useNSVI = bool.Parse(node.Value);
            SetDefalutValue();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("VisibleMin", _visiableMinValue.ToString());
            dic.Add("UseNDSI", _useNDSI.ToString());
            if (_useNDSI)
            {
                dic.Add("NDSIMin", _NDSImin.ToString());
                dic.Add("NDSIMax", _NDSIMax.ToString());
            }
            dic.Add("UseFarInfrared", _useFarInfrared.ToString());
            if (_useFarInfrared)
                dic.Add("FarInfraredMax", _FarInfraredMaxValue.ToString());
            dic.Add("UseNearVisiable", _useNearVisiable.ToString());
            if (_useNearVisiable)
            {
                dic.Add("NearVisableMin", _NearVisiableMin.ToString());
                dic.Add("NearVisableMax", _NearVisiableMax.ToString());
            }
            if (_printFeatrues)
                dic.Add("PrintFeatrue", _printFeatrues.ToString());
            return dic;
        }

        private void SetDefalutValue()
        {
            NDSIMultiBar.Visible = _useNDSI;
            NDSIPanel.Visible = _useNDSI;
            VisiableTextBox.Text = _visiableMinValue.ToString();
            FarinfraredTextBox.Text = _FarInfraredMaxValue.ToString();
            NDSIMinTextBox.Text = _NDSImin.ToString();
            NDSIMaxTextBox.Text = _NDSIMax.ToString();
            VisiableMultiBar.SetValues(new double[] { _visiableMinValue });
            FarMultiBar.SetValues(new double[] { _FarInfraredMaxValue });
            NDSIMultiBar.SetValues(new double[] { _NDSImin, _NDSIMax });

            farInfraredPanel.Visible = _useFarInfrared;
            nearVisiablePanel.Visible = _useNearVisiable;
            nearVisiableMultiBar.SetValues(new double[] { _NearVisiableMin, _NearVisiableMax });
            nearVisiableMinTextBox.Text = _NearVisiableMin.ToString();
            nearVisiableMaxTextBox.Text = _NearVisiableMax.ToString();

            pane_NSVI.Visible = _useNSVI;
            mbnsvi.SetValues(new double[] { double.Parse(this.txtnsvi.Text) });

        }

        #region 同步滑块与文本框

        private void VisiableMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            VisiableTextBox.Text = value.ToString("0.##");
            _visiableMinValue = value;
        }

        private void FarMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            FarinfraredTextBox.Text = value.ToString("0.##");
            _FarInfraredMaxValue = value;
        }

        private void NDSIMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
            {
                NDSIMinTextBox.Text = value.ToString("0.##");
                _NDSImin = value;
            }
            else
            {
                NDSIMaxTextBox.Text = value.ToString("0.##");
                _NDSIMax = value;
            }
        }

        private void nearVisiableMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
            {
                nearVisiableMinTextBox.Text = value.ToString("0.##");
                _NearVisiableMin = value;
            }
            else
            {
                nearVisiableMaxTextBox.Text = value.ToString("0.##");
                _NearVisiableMax = value;
            }
        }

        private void VisiableTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _visiableMinValue = double.Parse(VisiableTextBox.Text);
                _visiableMinValue = CheckValue(_visiableMinValue, VisiableMultiBar, VisiableTextBox);
                KeyDownEvent(VisiableMultiBar, _visiableMinValue, double.MinValue);
            }
        }

        private double CheckValue(double value, UI.MultiBarTrack multiBar, DoubleTextBox text)
        {
            if (value > multiBar.MaxEndPointValue)
            {
                value = multiBar.MaxEndPointValue;
                text.Text = value.ToString();
            }
            else if (value < multiBar.MinEndPointValue)
            {
                value = multiBar.MinEndPointValue;
                text.Text = value.ToString();
            }
            return value;
        }

        private void KeyDownEvent(MultiBarTrack MultiBar, double value, double nextValue)
        {
            _isExcuteArgumentValueChangedEvent = true;
            if (nextValue == double.MinValue)
                MultiBar.SetValues(new double[] { value });
            else
                MultiBar.SetValues(new double[] { value, nextValue });
            btnOK_Click(null, null);
            _isExcuteArgumentValueChangedEvent = false;
        }

        private void FarinfraredTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _FarInfraredMaxValue = double.Parse(FarinfraredTextBox.Text);
                _FarInfraredMaxValue = CheckValue(_FarInfraredMaxValue, FarMultiBar, FarinfraredTextBox);
                KeyDownEvent(FarMultiBar, double.Parse(FarinfraredTextBox.Text), double.MinValue);
            }
        }

        private void NDSIMinTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _NDSImin = double.Parse(NDSIMinTextBox.Text);
                _NDSImin = CheckValue(_NDSImin, NDSIMultiBar, NDSIMinTextBox);
                KeyDownEvent(NDSIMultiBar, _NDSImin, double.Parse(NDSIMaxTextBox.Text));
            }
        }

        private void NDSIMaxTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _NDSIMax = double.Parse(NDSIMaxTextBox.Text);
                _NDSIMax = CheckValue(_NDSIMax, NDSIMultiBar, NDSIMaxTextBox);
                KeyDownEvent(NDSIMultiBar, double.Parse(NDSIMinTextBox.Text), _NDSIMax);
            }
        }

        private void nearVisiableMinTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _NearVisiableMin = double.Parse(nearVisiableMinTextBox.Text);
                _NearVisiableMin = CheckValue(_NearVisiableMin, nearVisiableMultiBar, nearVisiableMinTextBox);
                KeyDownEvent(nearVisiableMultiBar, _NearVisiableMin, double.Parse(nearVisiableMaxTextBox.Text));
            }
        }

        private void nearVisiableMaxTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _NearVisiableMax = double.Parse(nearVisiableMaxTextBox.Text);
                _NearVisiableMax = CheckValue(_NearVisiableMax, nearVisiableMultiBar, nearVisiableMaxTextBox);
                KeyDownEvent(nearVisiableMultiBar, double.Parse(nearVisiableMinTextBox.Text), _NearVisiableMax);
            }
        }

        #endregion

        private void MultiBar_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }

        private void nearVisiableMultiBar_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChangedEvent;
            }
            set
            {

            }
        }

        private ContextMenuStrip contextMenuStrip1 = new ContextMenuStrip();
        void pairTrack_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Items.Clear();
                ToolStripMenuItem mnuSetEndpointValue = new ToolStripMenuItem("设置端值");
                mnuSetEndpointValue.Tag = sender;
                mnuSetEndpointValue.Click += new EventHandler(mnuSetEndpointValue_Click);
                contextMenuStrip1.Items.Add(mnuSetEndpointValue);
                string minPoint = (sender as MultiBarTrack).MinEndPointValue.ToString();
                string maxPoint = (sender as MultiBarTrack).MaxEndPointValue.ToString();
                double minValue = int.Parse(minPoint);
                double maxValue = int.Parse(maxPoint);
                ToolStripMenuItem quickValue = new ToolStripMenuItem("默认端值：" + minValue + " " + maxValue);
                quickValue.Tag = new object[] { sender, new double[] { minValue, maxValue } };
                quickValue.Click += new EventHandler(quickValue_Click);
                contextMenuStrip1.Items.Add(quickValue);
                contextMenuStrip1.Show(sender as MultiBarTrack, e.Location);
            }
        }

        void mnuSetEndpointValue_Click(object sender, EventArgs e)
        {
            MultiBarTrack bar = (sender as ToolStripMenuItem).Tag as MultiBarTrack;
            SetEndPoint(bar);
        }

        void quickValue_Click(object sender, EventArgs e)
        {
            object[] obj = (sender as ToolStripMenuItem).Tag as object[];
            MultiBarTrack bar = obj[0] as MultiBarTrack;
            Double[] values = obj[1] as Double[];
            bar.MinEndPointValue = values[0];
            bar.MaxEndPointValue = values[1];
        }

        private void SetEndPoint(MultiBarTrack bar)
        {
            using (frmSetEndpointValue frm = new frmSetEndpointValue(bar.MinEndPointValue, bar.MaxEndPointValue))
            {
                frm.Location = Control.MousePosition;
                if (frm.Location.X + frm.Width > Screen.GetBounds(this).Width)
                    frm.Location = new Point(Screen.GetBounds(this).Width - frm.Width, Control.MousePosition.Y);
                if (frm.ShowDialog(bar) == DialogResult.OK)
                {
                    bar.MinEndPointValue = frm.MinValue;
                    bar.MaxEndPointValue = frm.MaxValue;
                }
            }
            this.Refresh();
        }

        private void txtnsvi_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void mbnsvi_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            txtnsvi.Text = value.ToString("0.##");
            
        }

        private void mbnsvi_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }
    }
}
