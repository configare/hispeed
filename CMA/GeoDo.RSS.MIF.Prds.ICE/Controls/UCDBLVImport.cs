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
using System.IO;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public partial class UCDBLVImport : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private string _currentRasterFile = null;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCDBLVImport()
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
            string filename = arp.DataProvider.fileName;
            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                return;
            SearchDBLVFormWorkSpace(filename, subProduct.Identify);
        }

        private void SearchDBLVFormWorkSpace(string filename,string subProductIdentify)
        {
            RasterIdentify ri = new RasterIdentify(filename);
            ri.ProductIdentify = "ICE";
            ri.SubProductIdentify = "DBLV";
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
