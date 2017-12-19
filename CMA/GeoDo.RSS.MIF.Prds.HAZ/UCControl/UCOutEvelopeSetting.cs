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
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class UCOutEvelopeSetting : UserControl, IArgumentEditorUI2
    {
        private double _lonMin = -1;
        private double _lonMax = 0.3;
        private double _latMin = 0;
        private double _latMax = 2;
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCOutEvelopeSetting()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> GetOutEvenlope()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            //if (Tag == null)
            //    return null;
            dic.Add("outlonMin", _lonMin.ToString());
            dic.Add("outlonMax", _lonMax.ToString());
            dic.Add("outlatMin", _latMin.ToString());
            dic.Add("outlatMax", _latMax.ToString());
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
            object obj = arp.GetArg("SmartSession");
            ISmartSession session = null;
            if (obj != null)
                session = obj as ISmartSession;
            Tag = arp;
            if (arp.DataProvider != null && session != null && session.SmartWindowManager.ActiveCanvasViewer != null)
            {
                _lonMin = Math.Round(arp.DataProvider.CoordEnvelope.MinX, 4);
                _lonMax = Math.Round(arp.DataProvider.CoordEnvelope.MaxX, 4);
                _latMin = Math.Round(arp.DataProvider.CoordEnvelope.MinY, 4);
                _latMax = Math.Round(arp.DataProvider.CoordEnvelope.MaxY, 4);
            }
            SetDefalutValue();
            if (_handler != null)
                _handler(GetOutEvenlope());
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
            //_isExcuteArgumentValueChangedEvent = true;
            if (_handler != null)
                _handler(GetOutEvenlope());
            //_isExcuteArgumentValueChangedEvent = false;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            XElement node = ele.Element("outlonMin");
            if (node != null)
                _lonMin = double.Parse(node.Value);
            node = ele.Element("outlonMax");
            if (node != null)
                _lonMax = double.Parse(node.Value);
            node = ele.Element("outlatMin");
            if (node != null)
                _latMin = double.Parse(node.Value);
            node = ele.Element("outlatMax");
            if (node != null)
                _latMax = double.Parse(node.Value);
            SetDefalutValue();
            Dictionary<string, string> dic = GetOutEvenlope();
            return dic;
        }

        private void SetDefalutValue()
        {
            lonMinTextBox.Text = _lonMin.ToString();
            lonMaxTextBox.Text = _lonMax.ToString();
            OutLonMultiBar.SetValues(new double[] { _lonMin, _lonMax });

            OutLatMultiBar.SetValues(new double[] { _latMin, _latMax });
            latMinTextBox.Text = _latMin.ToString();
            latMaxTextBox.Text = _latMax.ToString();
        }

        #region 同步滑块与文本框

        private void OutLonMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
            {
                lonMinTextBox.Text = value.ToString("0.##");
                _lonMin = value;
            }
            else
            {
                lonMaxTextBox.Text = value.ToString("0.##");
                _lonMax = value;
            }
        }

        private void OutLatMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
            {
                latMinTextBox.Text = value.ToString("0.##");
                _latMin = value;
            }
            else
            {
                latMaxTextBox.Text = value.ToString("0.##");
                _latMax = value;
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
            //_isExcuteArgumentValueChangedEvent = true;
            if (nextValue == double.MinValue)
                MultiBar.SetValues(new double[] { value });
            else
                MultiBar.SetValues(new double[] { value, nextValue });
            btnOK_Click(null, null);
            //_isExcuteArgumentValueChangedEvent = false;
        }

        private void LonMinTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _lonMin = double.Parse(lonMinTextBox.Text);
                _lonMin = CheckValue(_lonMin, OutLonMultiBar, lonMinTextBox);
                KeyDownEvent(OutLonMultiBar, _lonMin, double.Parse(lonMaxTextBox.Text));
            }
        }

        private void LonMaxTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _lonMax = double.Parse(lonMaxTextBox.Text);
                _lonMax = CheckValue(_lonMax, OutLonMultiBar, lonMaxTextBox);
                KeyDownEvent(OutLonMultiBar, double.Parse(lonMinTextBox.Text), _lonMax);
            }
        }

        private void LatMinTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _latMin = double.Parse(latMinTextBox.Text);
                _latMin = CheckValue(_latMin, OutLatMultiBar, latMinTextBox);
                KeyDownEvent(OutLatMultiBar, _latMin, double.Parse(latMaxTextBox.Text));
            }
        }

        private void LatMaxTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _latMax = double.Parse(latMaxTextBox.Text);
                _latMax = CheckValue(_latMax, OutLatMultiBar, latMaxTextBox);
                KeyDownEvent(OutLatMultiBar, double.Parse(latMinTextBox.Text), _latMax);
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
    }
}
