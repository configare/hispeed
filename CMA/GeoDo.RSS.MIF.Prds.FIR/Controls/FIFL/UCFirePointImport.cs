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

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class UCFirePointImport : UserControl,IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;
        private string _currentFileName = null;
        private string _fileDir = null;

        public UCFirePointImport()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            if (string.IsNullOrEmpty(txtGFRFile.Text))
                return null;
            string cfrFile = txtGFRFile.Text;
            return cfrFile;
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
                filename = arp.GetArg("mainfiles").ToString();
            _currentFileName = filename;
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
                dlg.Filter = "HDF File(*.hdf)|*.hdf";
                dlg.InitialDirectory = _fileDir;
                dlg.FileName = GetSeachString();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtGFRFile.Text = dlg.FileName;
                    if (_handler != null)
                        _handler(GetArgumentValue());
                }
            }
        }

        private string GetSeachString()
        {
            if(string.IsNullOrEmpty(_currentFileName))
               return string.Empty;
            RasterIdentify rid = new RasterIdentify(_currentFileName);
            string satellite = rid.Satellite;
            string sensor = rid.Sensor;
            string orbitTime;
            if(rid.OrbitDateTime!=DateTime.MaxValue&&rid.OrbitDateTime!=DateTime.MinValue)
               orbitTime = rid.OrbitDateTime.ToString("yyyyMMdd_hhmm");
            else
               orbitTime=null;
            string seachString=string.Empty;
            seachString += satellite == null ? "*" : satellite;
            if (sensor == null)
            {
                if (!seachString.EndsWith("*"))
                    seachString += "_*";
            }
            else
                seachString += ("_" + sensor);
            if (seachString.EndsWith("*"))
                seachString += "_GFR_*";
            else
                seachString += "*_GFR_*";
            if (orbitTime == null)
                seachString += ".hdf";
            else
                seachString += "_" + orbitTime + "_*.hdf";
            return seachString;      
        }
    }
}
