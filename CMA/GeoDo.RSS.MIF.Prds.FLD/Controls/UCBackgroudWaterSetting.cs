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

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UCBackgroudWaterSetting : UserControl, IArgumentEditorUI2
    {
        private string _fileDir = null;
        private Action<object> _handler;
        private bool _isExcuteArgumentValueChangedEvent = false;

        public UCBackgroudWaterSetting()
        {
            InitializeComponent();
            InitFileDir();
        }

        private void InitFileDir()
        {
            _fileDir = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\FLD\BackWaterFile\";
            if (!Directory.Exists(_fileDir))
            {
                Directory.CreateDirectory(_fileDir);
                return;
            }
            string[] fileNames = Directory.GetFiles(_fileDir);
            if (fileNames != null && fileNames.Length > 0)
            {
                txtFileDir.Text = fileNames[0];
                if (_handler != null)
                {
                    _handler(GetArgumentValue());
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (frmBackWaterImport frm = new frmBackWaterImport())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    txtFileDir.Text = frm.FileName;
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
            return txtFileDir.Text;
        }

        private void txtFileDir_TextChanged(object sender, EventArgs e)
        {
            if (_handler != null)
                _handler(txtFileDir.Text);
            //保存至指定文件目录下
            string[] fileNames = Directory.GetFiles(_fileDir);
            if (fileNames.Length != 0)
            {
                foreach(string item in fileNames)
                {
                    if (item == txtFileDir.Text)
                        return;
                }
            }
            File.Copy(txtFileDir.Text, _fileDir + "\\" + Path.GetFileName(txtFileDir.Text),true);
            if (Path.GetExtension(txtFileDir.Text).ToLower() == ".shp")
            {
                string dir = Path.GetDirectoryName(txtFileDir.Text);
                string fileName = Path.GetFileNameWithoutExtension(txtFileDir.Text);
                string[] existFileNames = Directory.GetFiles(dir, fileName + ".*");
                foreach (string item in existFileNames)
                {
                    switch (Path.GetExtension(item).ToLower())
                    {
                        case ".sbx":
                        case ".sbn":
                        case ".dbf":
                        case ".prj":
                        case ".shx": File.Copy(item, Path.Combine(_fileDir, Path.GetFileName(item)),true);
                            break;
                    }
                }
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

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            InitFileDir();
        }
    }
}
