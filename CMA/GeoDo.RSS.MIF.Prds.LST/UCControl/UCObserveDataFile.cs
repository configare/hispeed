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
using System.IO;
using GeoDo.RSS.MIF.UI;

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UCObserveDataFile : UserControl, IArgumentEditorUI2
    {
        private bool _isExcuteArgumentChanged = false;
        private Action<object> _handler;
        string _searchDateStr = "yyMMdd";
        string _compareDateStr = "yyMMddHH";

        public UCObserveDataFile()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "观测文件(*.000)|*.000";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilename.Text = Path.GetFileName(dialog.FileName);
                    txtFilename.Tag = dialog.FileName;
                }
            }
        }

        public object GetArgumentValue()
        {
            return txtFilename.Tag == null ? string.Empty : txtFilename.Tag.ToString();
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            txtDir.Tag = null;
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
            txtDir.Tag = filename;
            if (_handler != null)
                _handler(GetArgFileSetting());
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _isExcuteArgumentChanged;
            }
            set
            {
                _isExcuteArgumentChanged = value;
            }
        }

        public object ParseArgumentValue(XElement ele)
        {
            if (ele == null)
                return null;
            XElement dirEle = ele.Element(XName.Get("FileDirs"));
            if (dirEle == null)
                return null;
            IEnumerable<XElement> segs = dirEle.Elements(XName.Get("FileDir"));
            if (segs == null || segs.Count() == 0)
                return null;
            foreach (XElement item in segs)
            {
                if (item != null)
                {
                    string fileName = item.Attribute(XName.Get("dir")).Value;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (fileName.First() == '@')
                        {
                            fileName = fileName.Replace("@", AppDomain.CurrentDomain.BaseDirectory);
                        }
                        txtDir.Text = fileName;
                        return fileName;
                    }
                }
            }
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(GetArgFileSetting());
        }

        private object GetArgFileSetting()
        {
            return txtFilename.Tag == null ? string.Empty : txtFilename.Tag.ToString();
        }

        private void btnOpenDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtDir.Text = folderDialog.SelectedPath;
                SearchOvserveDataFile();
            }
        }

        private void SearchOvserveDataFile()
        {
            if (txtDir.Tag == null)
                return;
            RasterIdentify rid = new RasterIdentify(Path.GetFileName(txtDir.Tag.ToString()));
            if (rid == null)
                return;
            string[] files = Directory.GetFiles(txtDir.Text, "*" + rid.OrbitDateTime.AddMinutes(30).ToString(_compareDateStr) + "*.000", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            txtFilename.Text = Path.GetFileName(files[0]);
            txtFilename.Tag = files[0];
        }

        private void txtDir_TextChanged(object sender, EventArgs e)
        {
            SearchOvserveDataFile();
            if (_handler != null)
                _handler(GetArgFileSetting());
        }
    }
}
