using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCCloudDBLVImport : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _currentRasterFile = null;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCCloudDBLVImport()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "判识结果文件(*.dat)|*.dat";
                frm.Multiselect = false;
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text = frm.FileName;
                    if (_handler != null)
                        _handler(GetArgumentValue());
                }
            }
        }

        public object GetArgumentValue()
        {
            if (string.IsNullOrEmpty(txtFileDir.Text))
                return null;
            return txtFileDir.Text;
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
            if (arp.DataProvider == null || string.IsNullOrEmpty(arp.DataProvider.fileName))
                return;
            string filename = arp.DataProvider.fileName;
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return;
            SearchDBLVFormWorkSpace(filename, subProduct.Identify);
        }

        private void SearchDBLVFormWorkSpace(string filename, string subProductIdentify)
        {
            RasterIdentify ri = new RasterIdentify(filename);
            ri.ProductIdentify = "LST";
            ri.SubProductIdentify = "0CLM";
            string dblvFileName = ri.ToWksFullFileName(".dat");
            if (File.Exists(dblvFileName))
            {
                txtFileDir.Text = dblvFileName;
                if (_handler != null)
                    _handler(GetArgumentValue());
            }
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

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }
    }
}
