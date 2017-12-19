using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.DF;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.BAG
{

    /// <summary>
    /// add by ca
    /// 2015年11月5日
    /// </summary>
    public partial class UCCloud : UserControl, IArgumentEditorUI2
    {

        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;
        private IRasterDataProvider _dataProvider = null;
        private bool _isPrintFeature = false;
        public UCCloud()
        {
            InitializeComponent();
            this.mbkvisiable.MouseClick += new MouseEventHandler(pairTrack_MouseClick);
            this.bmknearinfrared.MouseClick += pairTrack_MouseClick;
            this.mbnsvi.MouseClick += pairTrack_MouseClick;
            this.mbkndvi.MouseClick += pairTrack_MouseClick;

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
                _handler(GetArgumentValue());
        }

        /// <summary>
        /// 传递界面参数到详细算法实现部分
        /// </summary>
        /// <returns>封装算法阈值参数</returns>
        public object GetArgumentValue()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                dic.Add("MinVisiable", txtvisiable.Text.Trim());
                dic.Add("MinNearInFrared", txtnearinfrared.Text.Trim());
                dic.Add("MaxNSVI", txtnsvi.Text.Trim());
                dic.Add("MinNDVI", txtmaxndvi.Text.Trim());
                dic.Add("PrintFeature", _isPrintFeature.ToString());
                return dic;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _isExcuteArgumentValueChangedEvent = true;
            if (_handler != null)
                _handler(GetArgumentValue());
            _isExcuteArgumentValueChangedEvent = false;
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            //这里用来对bag.xml 私有配置进行初始化
            //
            if (ele == null)
                return null;
            XElement node = ele.Element("MinVisiable");
            if (node != null)
                this.txtvisiable.Text = node.Attribute("value").Value;
            node = ele.Element("MinNearInFrared");
            if (node != null)
                this.txtnearinfrared.Text = node.Attribute("value").Value;
            node = ele.Element("MaxNSVI");
            if (node != null)
                this.txtnsvi.Text = node.Attribute("value").Value;
            node = ele.Element("MinNDVI");
            if (node != null)
                this.txtmaxndvi.Text = node.Attribute("value").Value; ;
            node = ele.Element("PrintFeatrue");
            if (node != null)
                _isPrintFeature = bool.Parse(node.Attribute("value").Value);
            SetDefalutValue();
            return null;
        }
        private void SetDefalutValue()
        {
            mbkvisiable.SetValues(new double[] { double.Parse(this.txtvisiable.Text) });
            bmknearinfrared.SetValues(new double[] { double.Parse(this.txtnearinfrared.Text) });
            mbnsvi.SetValues(new double[] { double.Parse(this.txtnsvi.Text) });
            mbkndvi.SetValues(new double[] { double.Parse(this.txtmaxndvi.Text) });
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


        #region 滑块拖动事件
        private void mbkvisiable_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            txtvisiable.Text = value.ToString("0.00");
        }

        private void bmknearinfrared_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            txtnearinfrared.Text = value.ToString("0.00");
        }

        private void mbnsvi_BarValueChanged_1(object sender, int barIndex, double value, Point location)
        {
            txtnsvi.Text = value.ToString("0.00");
        }

        private void mbkndvi_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            txtmaxndvi.Text = value.ToString("0.00");
        }
        #endregion

        #region 文本框改变事件Enter处理

        private void txtvisiable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            else
            {
                double fixvalue = CheckValue(mbkvisiable, txtvisiable);
                KeyDownEvent(mbkvisiable, double.Parse(txtvisiable.Text), double.MinValue);
            }
        }

        private void txtnearinfrared_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            else
            {
                double fixvalue = CheckValue(bmknearinfrared, txtnearinfrared);
                KeyDownEvent(bmknearinfrared, double.Parse(txtnearinfrared.Text), double.MinValue);
            }
        }

        private void txtnsvi_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            else
            {
                double fixvalue = CheckValue(mbnsvi, txtnsvi);
                KeyDownEvent(mbnsvi, double.Parse(txtnsvi.Text), double.MinValue);
            }
        }
        private void txtmaxndvi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            else
            {
                double fixvalue = CheckValue(mbkndvi, txtmaxndvi);
                KeyDownEvent(mbnsvi, double.Parse(txtmaxndvi.Text), double.MinValue);
            }
        }
        private double CheckValue(UI.MultiBarTrack multiBar, DoubleTextBox text)
        {
            double value = double.Parse(text.Text.Trim());
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
        /// <summary>
        /// 文本内容调整滑块
        /// </summary>
        /// <param name="MultiBar">滑块</param>
        /// <param name="value">滑块值端值1</param>
        /// <param name="nextValue">滑块端值2</param>
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
        #endregion

        #region 设置滑块允许值范围

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
        #endregion

        private void mbkvisiable_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }

        private void bmknearinfrared_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }

        private void mbnsvi_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }

        private void mbkndvi_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            btnOK_Click(null, null);
        }
    }
}
