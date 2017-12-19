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

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class UCAITxt : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handle = null;
        private string txtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "HAZ", "AI指数数据路径.txt");
        string _compareDateStr = "yyyyMMdd";
        private bool _isInit = false;

        public UCAITxt()
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
                    txtAITxtDir.Text = dirPath;
            }
            _isInit = false;
        }

        private void Init()
        {
            GetDefaultPathFromTxt();
        }

        private string AITxtDir { set { txtAITxtDir.Text = value; } get { return txtAITxtDir.Text; } }
        private string FileName { set { txtAITxtFile.Text = value; } get { return txtAITxtFile.Text; } }
        private string FileFullName { get { return string.Format("{0}.txt", Path.Combine(AITxtDir, FileName)); } }

        public object GetArgumentValue()
        {
            string argument = FileFullName;
            return argument;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            _isInit = true;
            txtAITxtDir.Tag = null;
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
            txtAITxtDir.Tag = arp.DataProvider == null ? null : arp.DataProvider.fileName;
            GetDefaultPathFromTxt();
            SearchCurRasterAI();
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
                sw.WriteLine(txtAITxtDir.Text);
            }
        }

        private void btnNatrueColorDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                txtAITxtDir.Text = folderDialog.SelectedPath;
            }
        }

        private void SearchCurRasterAI()
        {
            FileName = string.Empty;
            if (txtAITxtDir.Tag == null || !File.Exists(txtAITxtDir.Tag.ToString()))
                return;
            RasterIdentify rid = new RasterIdentify(txtAITxtDir.Tag.ToString());
            string filter = "*";
            filter += string.Format("{0}*{1}*{2}", rid.Satellite, "TOU", rid.OrbitDateTime.ToString(_compareDateStr));
            filter += "*.txt";
            if (string.IsNullOrEmpty(txtAITxtDir.Text))
                return;
            string[] files = Directory.GetFiles(txtAITxtDir.Text, filter, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            files = files.OrderByDescending((cur) => (new RasterIdentify(cur)).OrbitDateTime).ToArray();
            FileName = Path.GetFileNameWithoutExtension(files[0]);
        }

        private void txtObservationData_TextChanged(object sender, EventArgs e)
        {
            SaveArgsFile(null, null);
        }

        private void txtAITxtDir_TextChanged(object sender, EventArgs e)
        {
            string dir = txtAITxtDir.Text;
            if (!Directory.Exists(dir))
                return;
            SearchCurRasterAI();
        }

        private void btnAITxtFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (!string.IsNullOrEmpty(txtAITxtDir.Text))
                dialog.InitialDirectory = txtAITxtDir.Text;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtAITxtDir.TextChanged -= new EventHandler(txtAITxtDir_TextChanged);
                AITxtDir = Path.GetDirectoryName(dialog.FileName);
                txtAITxtDir.Text = AITxtDir;
                FileName = Path.GetFileNameWithoutExtension(dialog.FileName);
                txtAITxtFile.Text = FileName;
                txtAITxtDir.TextChanged += new EventHandler(txtAITxtDir_TextChanged);
            }
        }
    }
}
