using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public partial class UCDensitySetting : UserControl, IArgumentEditorUI2
    {
        private string _fileDir = null;
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCDensitySetting()
        {
            InitializeComponent();
            InitFileDir();
        }

        private void InitFileDir()
        {
            _fileDir = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\UHI\";
            if (!Directory.Exists(_fileDir))
            {
                Directory.CreateDirectory(_fileDir);
                return;
            }
            txtFileDir.Text = GetMatchFile();
            if (_handler != null)
            {
                _handler(GetArgumentValue());
            }
        }

        private string GetMatchFile()
        {
            string[] files = Directory.GetFiles(_fileDir, "*.xml", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return string.Empty;
            if (Tag == null)
                return files[0];
            IArgumentProvider arp = Tag as IArgumentProvider;
            RasterIdentify rs = new RasterIdentify(arp.DataProvider.fileName);
            if (rs == null || (string.IsNullOrEmpty(rs.Satellite) && string.IsNullOrEmpty(rs.Sensor)))
                return files[0];
            string search = "*" + rs.Satellite + "*" + rs.Sensor + "*.xml";
            files = Directory.GetFiles(_fileDir, search, SearchOption.AllDirectories);
            return files == null || files.Length == 0 ? string.Empty : files[0];
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
            InitFileDir();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "密度文件(*.xml)|*.xml";
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\SystemData\ProductArgs\UHI\";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text =dlg.FileName;

                }
            }
        }

        public object GetArgumentValue()
        {
            return txtFileDir.Text;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(XElement ele)
        {
            return null;
        }

        private void txtFileDir_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(txtFileDir.Text);
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

        private void btDensity_Click(object sender, EventArgs e)
        {
            int selectBand = 0;
            UInt16 averValue = 0;
            ISmartSession session = GetSession(out selectBand, out averValue);
            if (session == null)
                return;
            ICommand commd = session.CommandEnvironment.Get(7110);
            if (commd == null)
                return;
            commd.Execute(string.Format("{0},{1},{2},{3}", txtFileDir.Text, selectBand, averValue, numInterval.Value));
        }

        private ISmartSession GetSession(out int selectBand, out UInt16 averValue)
        {
            selectBand = 0;
            averValue = (UInt16)0;
            if (Tag == null)
                return null;
            IArgumentProvider arp = Tag as IArgumentProvider;
            if (arp == null)
                return null;
            selectBand = (int)arp.GetArg("FarInfrared11");
            averValue = (UInt16)arp.GetArg("FixedPointSetting");
            return arp.GetArg("SmartSession") as ISmartSession;
        }
    }
}
