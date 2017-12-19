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
using GeoDo.RSS.MIF.UI;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class AngleSettings : UserControl, IArgumentEditorUI2
    {
        private bool _isExcuteArgumentChanged = false;
        private Action<object> _handler;
        private IArgumentProvider _arp = null;
        public AngleSettings()
        {
            InitializeComponent();
            //初始化自定义控件内容
            this.txtasunz.TextChanged += TextChanged;
            this.txtasuna.TextChanged += TextChanged;
            this.txtasatz.TextChanged += TextChanged;
            this.txtasata.TextChanged += TextChanged;

        }
        /// <summary>
        /// 根据当前影像绑定角度信息
        /// </summary>
        /// <param name="sourcerasterfilename"></param>
        public void IntivalAngles(string sourcerasterfilename)
        {


            string asunz = "SolarZenith";//太阳天顶角
            string asuna = "SolarAzimuth";// 太阳方位角
            string asatz = "SensorZenith";//卫星天顶角 
            string asata = "SensorAzimuth";//卫星方位角
            string file_name = sourcerasterfilename.Insert(sourcerasterfilename.LastIndexOf('.') + 1, "#.");
            this.txtasunz.Text = File.Exists(file_name.Replace("#", asunz)) ? file_name.Replace("#", asunz) : string.Empty;
            this.txtasuna.Text = File.Exists(file_name.Replace("#", asuna)) ? file_name.Replace("#", asuna) : string.Empty;
            this.txtasatz.Text = File.Exists(file_name.Replace("#", asatz)) ? file_name.Replace("#", asatz) : string.Empty;
            this.txtasata.Text = File.Exists(file_name.Replace("#", asata)) ? file_name.Replace("#", asata) : string.Empty;


        }
        /// <summary>
        /// 实现接口类：负责参数传递到实际处理过程
        /// </summary>
        /// <returns></returns>
        public object GetArgumentValue()
        {
            AngleParModel model = PackageModel();
            if (model != null)
            {
                return model;
            }
            else
            {
                return null;
            }


        }
        public AngleParModel PackageModel()
        {
            try
            {
                AngleParModel model = new AngleParModel();
                model.FileAsunZ = this.txtasunz.Text.Trim();
                model.FileAsatZ = this.txtasuna.Text.Trim();
                model.FileAsunA = this.txtasatz.Text.Trim();
                model.FileAsatA = this.txtasata.Text.Trim();
                return model;

            }
            catch (Exception ex)
            {
                return null;
            }
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
            if (_arp.DataProvider != null)
            {
                string rasterfilename = _arp.DataProvider.fileName;
                IntivalAngles(rasterfilename);
            }



        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentChanged;
            }
            set
            {
                _isExcuteArgumentChanged = false;
            }
        }

        public object ParseArgumentValue(XElement ele)
        {

            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
        private void TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgumentValue());

        }

        private void btnsunz_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;
            op.Filter = "太阳方位角文件(*.ldf)|*.ldf";
            if (op.ShowDialog() == DialogResult.OK)
            {
                this.txtasunz.Text = op.FileName;
            }
        }

        private void btnsuna_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;
            op.Filter = "太阳天顶角文件(*.ldf)|*.ldf";
            if (op.ShowDialog() == DialogResult.OK)
            {
                this.txtasuna.Text = op.FileName;
            }
        }

        private void btnsatz_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;
            op.Filter = "载荷方位角文件(*.ldf)|*.ldf";
            if (op.ShowDialog() == DialogResult.OK)
            {
                this.txtasatz.Text = op.FileName;
            }
        }

        private void btnsata_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = false;
            op.Filter = "载荷天顶角文件(*.ldf)|*.ldf";
            if (op.ShowDialog() == DialogResult.OK)
            {
                this.txtasata.Text = op.FileName;
            }
        }

    }
}
