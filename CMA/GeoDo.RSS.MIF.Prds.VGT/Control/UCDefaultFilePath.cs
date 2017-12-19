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
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public partial class UCDefaultFilePath : UserControl, IArgumentEditorUI
    {
        private string _defaultPath = null;
        private string _saveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "VGT", "NDVI轨道数据地址.txt");
        private Action<object> _handler;

        public UCDefaultFilePath()
        {
            InitializeComponent();
            InitControls();
            GetDefaultPathFromTxt();
        }

        private void InitControls()
        {
            lbFile.Location = new System.Drawing.Point(3, 6);
            cmbFilePath.Location = new System.Drawing.Point(lbFile.Right + 6, 6);
            cmbFilePath.Size = new System.Drawing.Size(this.Width - lbFile.Width - 36, 20);
            btOpen.Location = new System.Drawing.Point(this.Width - 24, 3);
            btOpen.Size = new System.Drawing.Size(24, 24);
            btOpen.UseVisualStyleBackColor = true;
            this.Height = 30;
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _defaultPath = dlg.SelectedPath;
                    cmbFilePath.Text = _defaultPath;
                    if (_handler != null)
                        _handler(GetSelectedPath());
                }
            }
            SaveDefaultFile();
        }

        private object GetSelectedPath()
        {
            return _defaultPath;
        }

        private void SaveDefaultFile()
        {
            if (string.IsNullOrEmpty(_defaultPath))
                return;
            if (!File.Exists(_saveFile))
            {
                string dir = Path.GetDirectoryName(_saveFile);
                Directory.CreateDirectory(dir);
            }
            using (StreamWriter sw = new StreamWriter(_saveFile, false, Encoding.GetEncoding("GB2312")))
            {
                sw.Write(_defaultPath);
            }
        }

        private void GetDefaultPathFromTxt()
        {
            if (!File.Exists(_saveFile))
                return;
            using (StreamReader sr = new StreamReader(_saveFile, Encoding.GetEncoding("GB2312")))
            {
                _defaultPath = sr.ReadToEnd();
                if (Directory.Exists(_defaultPath))
                    cmbFilePath.Text = _defaultPath;
            }
        }

        #region IArgumentEditorUI 成员

        public object GetArgumentValue()
        {
            return _defaultPath;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (string.IsNullOrEmpty(_defaultPath))
                GetDefaultPathFromTxt();
            return _defaultPath;
        }
        #endregion
    }
}
