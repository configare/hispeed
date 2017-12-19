using System;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.HAZ.UCControl;
using GeoDo.RSS.MIF.UI;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class UCPathFile : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handle = null;
        private string txtFile;
        private bool _isInit = false;

        public UCPathFile()
        {
            InitializeComponent();
            Init();
        }

        private void GetDefaultPathFromTxt()
        {
            if (!File.Exists(txtFile))
                return;
            _isInit = true;
            using (var sr = new StreamReader(txtFile, Encoding.GetEncoding("GB2312")))
            {
                string dirPath = sr.ReadLine();
                if (Directory.Exists(dirPath))
                {
                    SetDir(dirPath);
                }
            }
            _isInit = false;
        }

        private void SetDir(string dirPath)
        {
            string day = DateTime.Now.ToString("yyyyMMdd");
            txtNatrueColorDir.Text = Path.Combine(dirPath, day);
        }

        private void Init()
        {
            GetDefaultPathFromTxt();
        }

        private string NatrueColorDir { set { txtNatrueColorDir.Text = value; } get { return txtNatrueColorDir.Text; } }

        private SimpleValue FileName
        {
            set
            {
                if (value == null) return;
                txtNatrueColorFile.Text = value.ToString();
                txtNatrueColorFile.Tag = value;
            }
            get
            {
                return txtNatrueColorFile.Tag as SimpleValue;
            }
        }

        private string FileFullName
        {
            get
            {
                SimpleValue sv = FileName;
                if (sv == null) return null;
                string source = string.Format("{0}.ldf", Path.Combine(NatrueColorDir, sv.ID));
                string target = string.Format("{0}.ldf", Path.Combine(sv.ToString()));
                return string.Format("{0},{1}", source, target);
            }
        }

        public object GetArgumentValue()
        {
            string argument = FileFullName;
            return argument;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _isInit = true;
            txtNatrueColorDir.Tag = null;
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
            SaveArgsFile(null, null);
            _isInit = false;
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            if (ele == null)
                return null;
            XElement dirEle = ele.Element(XName.Get("FileDirs"));
            if (dirEle != null)
            {
                txtFile = GetDir(dirEle, "FileDir");
                GetDefaultPathFromTxt();
            }
            XElement labelEle = ele.Element(XName.Get("Label"));
            if (labelEle != null)
            {
                string label = labelEle.Value.ToString();
                label6.Text = label + "路径";
                label1.Text = label + "文件";
            }
            return null;
        }

        private string GetDir(XElement dirEle, string eleStr)
        {
            IEnumerable<XElement> segs = dirEle.Elements(XName.Get(eleStr));
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
                        return fileName;
                    }
                }
            }
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handle = handler;
        }

        private void SaveArgsFile(object sender, EventArgs e)
        {
            if (_handle != null)
                _handle(GetArgumentValue());
            if (!_isInit)
            {
                //保存文件路径
                SaveDefaultFile();
            }
        }

        private void SaveDefaultFile()
        {
            if (!File.Exists(txtFile))
            {
                string dir = Path.GetDirectoryName(txtFile);
                Directory.CreateDirectory(dir);
            }
            using (StreamWriter sw = new StreamWriter(txtFile, false, Encoding.GetEncoding("GB2312")))
            {
                string dir = txtNatrueColorDir.Text;
                sw.WriteLine(Path.GetDirectoryName(dir));
            }
        }

        private void btnNatrueColorDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SetDir(folderDialog.SelectedPath);
            }
        }

        private void txtObservationData_TextChanged(object sender, EventArgs e)
        {
            SaveArgsFile(null, null);
        }

        private void txtNatrueColorDir_TextChanged(object sender, EventArgs e)
        {
            string dir = txtNatrueColorDir.Text;
            ClearFileLst();
            if (!Directory.Exists(dir))
                return;

            var filter = "*.ldf";
            var files = Directory.GetFiles(dir, filter);
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                SimpleValue sv = new SimpleValue { ID = filename, Name = filename };
                lstFiles.Items.Add(sv);
            }
            if (lstFiles.Items.Count > 0)
                lstFiles.SelectedIndex = 0;
        }

        private class SimpleValue
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }

        private void ClearFileLst()
        {
            lstFiles.Items.Clear();
            FileName = null;
        }

        private void lstFiles_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            foreach (var selectedItem in lstFiles.SelectedItems)
            {
                SimpleValue sv = selectedItem as SimpleValue;
                FileName = sv;
                SaveArgsFile(null, null);
                break;
            }
        }
    }
}
