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
using System.Xml.Linq;
using System.IO;
using GeoDo.RSS.DI;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    /// <summary>
    /// 需要的信息：产品："FIR", 子产品："DBLV",当前影像： provider, 导入数据：fileanme, 
    /// </summary>
    public partial class UCHistroyImport : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = true;
        private string _currentFileName = null;
        private string _fileDir = null;
        private string _productIdentify;
        private string _subProductIdentify;

        public UCHistroyImport()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            if (string.IsNullOrEmpty(txtHistroyFile.Text) || !File.Exists(txtHistroyFile.Text))
                return null;
            IDataImportDriver driver = DataImport.GetDriver(_productIdentify, _subProductIdentify, txtHistroyFile.Text, null);
            if (driver == null)
                return null;
            string error = null;
            if (!string.IsNullOrEmpty(_currentFileName))
                using (RasterDataProvider provider = GeoDataDriver.Open(_currentFileName) as RasterDataProvider)
                {
                    return driver.Do(_productIdentify, _subProductIdentify, provider, txtHistroyFile.Text, out error);
                }
            return null;
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
            _productIdentify = subProduct.Definition.ProductDef.Identify;
            _subProductIdentify = subProduct.Identify;
            IArgumentProvider arp = subProduct.ArgumentProvider;
            if (arp == null)
                return;
            _currentFileName = arp.DataProvider.fileName;
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentValueChangedEvent;
            }
            set
            {
                _isExcuteArgumentValueChangedEvent = value;
            }
        }

        public object ParseArgumentValue(XElement ele)
        {
            if (ele == null)
                return string.Empty;
            XElement node = ele.Element("FileFullName");
            if (node != null)
                return node.Attribute("value").Value;
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Prodcut File(*.dat;*.mvg)|*.dat;*.mvg";
                dlg.InitialDirectory = _fileDir;
                dlg.FileName = GetSeachString();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtHistroyFile.Text = dlg.FileName;
                    if (_handler != null)
                        _handler(GetArgumentValue());
                }
            }
        }

        private string GetSeachString()
        {
            if (string.IsNullOrEmpty(_currentFileName))
                return string.Empty;
            RasterIdentify rid = new RasterIdentify(_currentFileName);
            string satellite = rid.Satellite;
            string sensor = rid.Sensor;
            string orbitTime;
            if (rid.OrbitDateTime != DateTime.MaxValue && rid.OrbitDateTime != DateTime.MinValue)
                orbitTime = rid.OrbitDateTime.ToString("yyyyMMdd") + "*" + rid.OrbitDateTime.ToString("HHmm");
            else
                orbitTime = null;
            string seachString = "*" + _productIdentify + "*" + _subProductIdentify + "*";
            seachString += satellite == null ? "*" : satellite;
            if (sensor == null)
            {
                if (!seachString.EndsWith("*"))
                    seachString += "*";
            }
            else
                seachString += ("*" + sensor);
            if (orbitTime != null)
                seachString += "*" + orbitTime + "*";
            return seachString;
        }
    }
}
