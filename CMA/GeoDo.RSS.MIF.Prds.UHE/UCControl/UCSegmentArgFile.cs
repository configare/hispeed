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

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public partial class UCSegmentArgFile : UserControl, IArgumentEditorUI2
    {
        private bool _isExcuteArgumentChanged = false;
        private Action<object> _handler;

        public UCSegmentArgFile()
        {
            InitializeComponent();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            using (frmTemperatureRegion frm = new frmTemperatureRegion(txtFileName.Text))
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Filter = "参数文件(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFileName.Text = dialog.FileName;
                }
            }
        }

        public object GetArgumentValue()
        {
            return txtFileName.Text;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
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
                _isExcuteArgumentChanged=value;
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
                        if (fileName.First()=='@')
                        {
                            fileName = fileName.Replace("@", AppDomain.CurrentDomain.BaseDirectory);
                        }
                        txtFileName.Text = fileName;
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
            return txtFileName.Text;
        }
    }
}
