using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.UI;
using CodeCell.AgileMap.Core;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UCSetNDVITool : UserControl, IArgumentEditorUI2
    {
        private IArgumentProvider _arp = null;
        private Action<object> _handler;

        public int[] AOIIndexs = null;


        public UCSetNDVITool()
        {
            InitializeComponent();
        }
        public object GetArgumentValue()
        {
            return this;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
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
            _arp = subProduct.ArgumentProvider;
            if (_arp == null)
                return;
            _arp.SetArg("ucSetNDVITool", this);
        }

        private void btnNDVI_Click(object sender, EventArgs e)
        {
            this.ckbaoi.Checked = true;
            if(_handler!=null)
            {
                _handler(GetArgumentValue());
                return;
            }         
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            XElement nodea = ele.Element("ValueItemA");
            XElement nodeb = ele.Element("ValueItemB");
            XElement nodemax = ele.Element("ValueItemMax");
            string a=nodea.Attribute("value").Value;
            string b = nodeb.Attribute("value").Value;
            string maxdefaule = nodemax.Attribute("value").Value;
            txta.Text = a;
            txtb.Text = b;
            this.txtndvimax.Text = maxdefaule;
            return null;
        }
        public void btnGetAOIIndex(object sender, EventArgs e)
        {
            this.AOIIndexs = _arp.AOI;
            
            //根据选定的AOI区域进行自动计算水体NDVI最小值
            IRasterDataProvider prd = _arp.DataProvider;
            int bandNI = (int)_arp.GetArg("NearInfrared");
            int bandVI = (int)_arp.GetArg("Visible");
            ArgumentProvider ap = new ArgumentProvider(prd, null);
            Size size = new Size(prd.Width, prd.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(this.AOIIndexs, size);
            using (RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap))
            {
                List<double> ndvis = new List<double>();
                visitor.VisitPixel(rect, this.AOIIndexs, new int[] { bandVI,bandNI },
                    (index, values) =>
                    {
                        if (values[1] + values[0] != 0)//是判断两个值都不等0的意思？
                        {
                            ndvis.Add(Math.Round((float)(values[1] - values[0]) / (values[1] + values[0]), 4));

                        }
                    });
                if (ndvis.Count >= 0)
                {
                    //设置界面值
                    float a = float.Parse(txta.Text);
                    float b = float.Parse(txtb.Text);
                    ndvis.Sort();//从小到大排列
                    ndvis.RemoveRange(0, (int)(0.01 * ndvis.Count));//去除百分之一最小值
                    float avgndvi = (float)ndvis.Min();
                    float MinNDVI = a * avgndvi + b;
                    this.txtNDVI.Text = avgndvi.ToString();
                    this.txtminndvi.Text = MinNDVI.ToString();
                    this.txtndvimin.Text = MinNDVI.ToString();
                    this.NDVIMultiBar.SetValues(new double[] { this.txtndvimin.Value,this.txtndvimax.Value });
                }
            }
            this.ckbaoi.Checked = false;//重置状态
            //设置完成之后自动生成
            if (_handler != null)
            {
                _handler(GetArgumentValue());
                return;
            }         
        }
        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        private void NDVIMultiBar_BarValueChanged(object sender, int barIndex, double value, Point location)
        {
            if (barIndex == 0)
            {
                this.txtndvimin.Text = value.ToString("0.##");
                
            }
            else
            {
                this.txtndvimax.Text = value.ToString("0.##");
               
            }
        }

        private void NDVIMultiBar_BarValueChangedFinished(object sender, int barIndex, double value)
        {
            if (_handler != null)
            {
                _handler(GetArgumentValue());
                return;
            }         
        }

    }
}
