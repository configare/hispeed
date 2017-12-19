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
    public partial class UCSetNearTool : UserControl, IArgumentEditorUI2
    {
        private IArgumentProvider _arp = null;
        private Action<object> _handler;

        public int[] AOIIndexs = null;
        public Dictionary<string, string> Dic = new Dictionary<string, string>();

        public UCSetNearTool()
        {
            InitializeComponent();
            this.txtpa.TextChanged += new System.EventHandler(this.InputTextChanged);
            this.txtpb.TextChanged += new System.EventHandler(this.InputTextChanged);
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
            _arp.SetArg("UCSetNearTool", this);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
        private void btnNear_Click(object sender, EventArgs e)
        {
            this.ckbone.Checked = true;
            if (_handler != null)
            {
                _handler(GetArgumentValue());
                return;
            }
        }
        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            XElement nodea = ele.Element("ValueItemA");
            XElement nodeb = ele.Element("ValueItemB");
           
            string a=nodea.Attribute("value").Value;
            string b = nodeb.Attribute("value").Value;
          
            txtpa.Text = a;
            txtpb.Text = b;
            return null;
        }

        public void btnGetAOIIndex(object sender, EventArgs e)
        {
           
            this.AOIIndexs = _arp.AOI;
            
            //根据选定的AOI区域进行自动计算水体近红外反射率最小值
            IRasterDataProvider prd = _arp.DataProvider;
            int bandNI = (int)_arp.GetArg("NearInfrared");
            ArgumentProvider ap = new ArgumentProvider(prd, null);
            Size size = new Size(prd.Width, prd.Height);
            Rectangle rect = AOIHelper.ComputeAOIRect(this.AOIIndexs, size);
            using (RasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(ap))
            {
                List<double> nears = new List<double>();
                visitor.VisitPixel(rect, this.AOIIndexs, new int[] { bandNI },
                    (index, values) =>
                    {
                        if (values[0]> 0)//近红外反射率>0
                        {
                            nears.Add(Math.Round(values[0] / 1000f, 4));

                        }
                    });
                if (nears.Count >= 0)
                {
                    //设置界面值
                    float pa = float.Parse(txtpa.Text);
                    float pb = float.Parse(txtpb.Text);
                    nears.Sort();//从小到大排列
                    nears.RemoveRange(0, (int)(0.05 * nears.Count));//去除百分之一最小值
                    float minnear = (float)nears.Min();
                    float MinN = minnear + (pa + pb) / 10;
                    this.txtNear.Text = minnear.ToString();
                    this.txtnearmin.Text = MinN.ToString();
                    this.NDVIMultiBar.SetValues(new double[] { MinN });
                }
            }
           this.ckbone.Checked = false;//重置状态
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
                this.txtnearmin.Text = value.ToString("0.##");
                
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

       
        private void InputTextChanged(object sender,EventArgs e)
        {
            TextBox obj = sender as TextBox;
            if(string.IsNullOrEmpty(obj.Text))
            {
                return;
            }
            try
            {
                float pa = float.Parse(txtpa.Text);
                float pb = float.Parse(txtpb.Text);
                float minnear = float.Parse(txtNear.Text);
                this.txtnearmin.Text = (minnear + (pa + pb) / 10).ToString();
                this.NDVIMultiBar.SetValues(new double[] { minnear + (pa + pb) / 10 });
            }
            catch(Exception ex)
            {
                return;
            }
        }

    }
}
